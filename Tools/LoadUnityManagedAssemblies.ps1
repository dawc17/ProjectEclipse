function Import-SF2ManagedRuntime([string]$ProjectPath) {
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
    $firstpass = Join-Path $ProjectPath 'Temp/Bin/Debug/Assembly-CSharp-firstpass.dll'
    if (Test-Path -LiteralPath $firstpass -PathType Leaf) {
        [Reflection.Assembly]::LoadFrom($firstpass) | Out-Null
    }
    return [Reflection.Assembly]::LoadFrom((Join-Path $ProjectPath 'Temp/Bin/Debug/Assembly-CSharp.dll'))
}
