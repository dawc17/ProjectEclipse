$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
msbuild (Join-Path $root 'Eclipse.Runtime.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Runtime compilation failed.' }
[Reflection.Assembly]::LoadFrom((Join-Path $root 'Temp/Bin/Debug/Eclipse.Runtime.dll')) | Out-Null
$script:checks=0
function Check([bool]$condition,[string]$message) { $script:checks++; if (!$condition) { throw $message } }
function Reject([scriptblock]$action) { $failed=$false; try { & $action } catch { $failed=$true }; Check $failed 'Expected failure.' }
function Reserve($ledger,$item,[int]$quantity,$transactions=$null,$units=$null) {
    $reservation=$null
    $success=$ledger.TryReserve($item,$quantity,$transactions,$units,[ref]$reservation)
    return @{Success=$success; Reservation=$reservation}
}
$a=[Eclipse.Modding.DefinitionId]::Parse('example.purchase:items/token')
$b=[Eclipse.Modding.DefinitionId]::Parse('core:items/weapon/WEAPON_KNIVES')
[xml]$profile='<User><Inventory Count="7"/></User>'
$ledger=[Eclipse.Modding.ModPurchaseLedger]::new($profile.DocumentElement)
$alias=[Eclipse.Modding.ModPurchaseLedger]::new($profile.DocumentElement)
$original=$profile.OuterXml
Check ($ledger.Read($a).Units -eq 0 -and $profile.OuterXml -eq $original) 'Reading unknown history wrote metadata or inferred inventory.'
$first=Reserve $ledger $a 3 1 3
Check ($first.Success -and $profile.OuterXml -eq $original) 'Reservation wrote an uncompleted purchase.'
Check (!(Reserve $alias $a 1).Success) 'Another ledger instance bypassed the reservation.'
$parallel=Reserve $alias $b 1
Check $parallel.Success 'Independent item reservation blocked.'
$parallel.Reservation.Commit()
$first.Reservation.Commit()
Check ($ledger.Read($a).Transactions -eq 1 -and $ledger.Read($a).Units -eq 3) 'Receipt confused transaction and unit totals.'
Check ($ledger.Read($b).Units -eq 1 -and $profile.User.Inventory.Count -eq '7') 'Independent receipt or inventory changed.'
Reject { $first.Reservation.Commit() }
$first.Reservation.Dispose()
Check (!(Reserve $ledger $a 1 1).Success) 'Transaction limit ignored.'
Check (!(Reserve $ledger $a 1 $null 3).Success) 'Unit limit ignored.'
$cancel=Reserve $ledger $a 1
$saved=$profile.OuterXml
$cancel.Reservation.Dispose()
Check ($profile.OuterXml -eq $saved) 'Cancelled purchase changed receipts.'
$fresh=Reserve $ledger $a 2 2 5
$cancel.Reservation.Dispose()
Check (!(Reserve $alias $a 1).Success) 'Stale disposal released another reservation.'
$fresh.Reservation.Commit()
Check ($ledger.Read($a).Transactions -eq 2 -and $ledger.Read($a).Units -eq 5) 'Second receipt lost historical totals.'
[xml]$reload=$profile.OuterXml
$loaded=[Eclipse.Modding.ModPurchaseLedger]::new($reload.DocumentElement)
Check ($loaded.Read($a).Units -eq 5 -and !(Reserve $loaded $a 1 2).Success) 'Reload lost history or limit.'
[xml]$other='<User/>'
$otherReservation=Reserve ([Eclipse.Modding.ModPurchaseLedger]::new($other.DocumentElement)) $a 1 1
Check $otherReservation.Success 'Profiles shared allowances.'
$otherReservation.Reservation.Dispose()
foreach ($quantity in @(0,-1)) { Reject { Reserve $ledger $a $quantity } }
Reject { Reserve $ledger $a 1 -1 }
Reject { $ledger.Read([Eclipse.Modding.DefinitionId]::Parse('core:perks/test')) }
Check (!(Reserve $ledger $b 1 0).Success) 'Zero limit accepted purchase.'
$change=Reserve $ledger $b 1
$profile.User.EclipsePurchases.SelectSingleNode("Item[@Id='$b']").SetAttribute('Units','2')
Reject { $change.Reservation.Commit() }
$change.Reservation.Dispose()
Check ($ledger.Read($b).Units -eq 2) 'Reservation overwrote intervening history.'
foreach ($payload in @('<EclipsePurchases Version="2"/>','<EclipsePurchases Version="1"><Item Id="bad" Transactions="1" Units="1"/></EclipsePurchases>',
 '<EclipsePurchases Version="1"><Item Id="core:items/x" Transactions="2" Units="1"/></EclipsePurchases>',
 '<EclipsePurchases Version="1"/><EclipsePurchases Version="1"/>',
 '<EclipsePurchases Version="1"><Item Id="core:items/x" Transactions="1" Units="1"/><Item Id="core:items/x" Transactions="1" Units="1"/></EclipsePurchases>')) {
    [xml]$bad="<User>$payload</User>"; $before=$bad.OuterXml
    $broken=[Eclipse.Modding.ModPurchaseLedger]::new($bad.DocumentElement)
    Reject { Reserve $broken $a 1 }
    Check ($bad.OuterXml -eq $before) 'Malformed receipt was silently rewritten.'
}
[xml]$full='<User><EclipsePurchases Version="1"><Item Id="example.purchase:items/token" Transactions="1" Units="9223372036854775807"/></EclipsePurchases></User>'
Check (!(Reserve ([Eclipse.Modding.ModPurchaseLedger]::new($full.DocumentElement)) $a 1).Success) 'Receipt arithmetic overflow accepted.'
[xml]$capacity='<User><EclipsePurchases Version="1"/></User>'
for ($i=0;$i -lt 8191;$i++) {
    $entry=$capacity.CreateElement('Item'); $entry.SetAttribute('Id',"core:items/entry$i"); $entry.SetAttribute('Transactions','1'); $entry.SetAttribute('Units','1')
    [void]$capacity.User.EclipsePurchases.AppendChild($entry)
}
$bounded=[Eclipse.Modding.ModPurchaseLedger]::new($capacity.DocumentElement)
$last=Reserve $bounded $a 1
Check $last.Success 'Final receipt slot rejected.'
Check (!(Reserve $bounded $b 1).Success) 'Concurrent reservation overbooked receipt capacity.'
$last.Reservation.Dispose()
$replacement=Reserve $bounded $b 1
Check $replacement.Success 'Cancelled reservation did not release capacity.'
$replacement.Reservation.Commit()
Check (!(Reserve $bounded $a 1).Success) 'Full ledger accepted another identity.'
$existing=Reserve $bounded $b 1
Check $existing.Success 'Full ledger blocked an existing identity.'
$existing.Reservation.Commit()
Check ($bounded.Read($b).Units -eq 2) 'Existing identity failed at full capacity.'
Write-Output "Purchase ledger: $script:checks checks passed (profile XML/reservations; no native grant or disk save)."
