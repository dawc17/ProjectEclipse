function Import-SF2ManagedRuntime([string]$ProjectPath, [string]$AssemblyDirectory = '', [string]$UnityManagedDirectory = '') {
    if ($PSVersionTable.PSVersion.Major -lt 7) {
        throw 'Unity managed runtime checks require PowerShell 7 (pwsh), not Windows PowerShell 5.1.'
    }
    $projectFile = Join-Path $ProjectPath 'Assembly-CSharp.csproj'
    [xml]$project = Get-Content -Raw -LiteralPath $projectFile
    $namespace = New-Object Xml.XmlNamespaceManager($project.NameTable)
    $namespace.AddNamespace('msb', $project.Project.NamespaceURI)
    $references = $project.SelectNodes('//msb:Reference[starts-with(@Include, "UnityEngine")]/msb:HintPath', $namespace)
    foreach ($reference in $references) {
        $path = $reference.InnerText
        if (Test-Path -LiteralPath $path -PathType Leaf) {
            [Reflection.Assembly]::LoadFrom($path) | Out-Null
        }
    }
    if ($UnityManagedDirectory) {
        Get-ChildItem -LiteralPath $UnityManagedDirectory -Filter 'UnityEngine*.dll' | ForEach-Object {
            [Reflection.Assembly]::LoadFrom($_.FullName) | Out-Null
        }
    }
    if (!$AssemblyDirectory) { $AssemblyDirectory = Join-Path $ProjectPath 'Temp/Bin/Debug' }
    $json = Get-ChildItem -Path (Join-Path $ProjectPath 'Library/PackageCache/com.unity.nuget.newtonsoft-json*/Runtime/Newtonsoft.Json.dll') -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($json) { [Reflection.Assembly]::LoadFrom($json.FullName) | Out-Null }
    $runtime = Join-Path $AssemblyDirectory 'Eclipse.Runtime.dll'
    if (Test-Path -LiteralPath $runtime -PathType Leaf) {
        [Reflection.Assembly]::LoadFrom($runtime) | Out-Null
    }
    $firstpass = Join-Path $AssemblyDirectory 'Assembly-CSharp-firstpass.dll'
    if (Test-Path -LiteralPath $firstpass -PathType Leaf) {
        [Reflection.Assembly]::LoadFrom($firstpass) | Out-Null
    }
    return [Reflection.Assembly]::LoadFrom((Join-Path $AssemblyDirectory 'Assembly-CSharp.dll'))
}
