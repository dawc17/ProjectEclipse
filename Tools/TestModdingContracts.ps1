$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$sources = @(
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\DefinitionId.cs')
)

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$csc = $null
if (Test-Path $vswhere) {
    $csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild `
        -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
}
if (-not $csc -or -not (Test-Path $csc)) {
    $csc = Get-ChildItem (Join-Path $env:ProgramFiles 'Microsoft Visual Studio') -Recurse `
        -Filter csc.exe -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $csc -or -not (Test-Path $csc)) {
    throw 'Could not locate the Visual Studio Roslyn C# compiler.'
}

$testRoot = Join-Path $root 'Temp\ModdingContractsTest'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$harness = Join-Path $testRoot 'Program.cs'
$exe = Join-Path $testRoot 'ModdingContractsTest.exe'

@'
using System;
using Eclipse.Modding;

internal static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Reject(Action action, string message)
    {
        try
        {
            action();
        }
        catch (FormatException)
        {
            return;
        }
        throw new Exception(message);
    }

    public static int Main()
    {
        ModId mod = ModId.Parse("example.weapon");
        Assert(mod.Value == "example.weapon", "ModId changed a valid ID.");
        Reject(() => ModId.Parse("Example.Weapon"), "Uppercase mod ID was accepted.");
        Reject(() => ModId.Parse("example/weapon"), "Slash in mod ID was accepted.");
        Reject(() => ModId.Parse(" example.weapon"), "Whitespace in mod ID was accepted.");

        AssetId asset = AssetId.Parse(@"core:Sprites\Shop\Katana");
        Assert(asset.ToString() == "core:sprites/shop/katana", "AssetId canonicalization is wrong.");
        Assert(asset == AssetId.Parse("core:sprites/shop/katana"), "Canonical AssetIds are not equal.");
        Reject(() => AssetId.Parse("sprites/shop/katana"), "Unqualified AssetId was accepted.");
        Reject(() => AssetId.Parse("core:../katana"), "Traversal AssetId was accepted.");
        Reject(() => AssetId.Parse("core:sprites//katana"), "Empty AssetId segment was accepted.");
        Reject(() => AssetId.Parse(@"core:C:\katana"), "Drive-qualified AssetId was accepted.");

        DefinitionId definition = DefinitionId.Parse("example.weapon:Items/Weapon/Example_Blade");
        Assert(definition.Namespace.Value == "example.weapon", "Definition namespace is wrong.");
        Assert(definition.Category == "items", "Definition category is wrong.");
        Assert(definition.LocalId == "weapon/example_blade", "Definition local ID is wrong.");
        Assert(definition.ToString() == "example.weapon:items/weapon/example_blade", "DefinitionId canonicalization is wrong.");
        Reject(() => DefinitionId.Parse("example.weapon:weapon"), "Definition without category/id split was accepted.");
        Reject(() => DefinitionId.Parse("example.weapon:items/../weapon"), "Traversal DefinitionId was accepted.");

        Console.WriteLine("Modding identity contracts: PASS");
        return 0;
    }
}
'@ | Set-Content -LiteralPath $harness -Encoding UTF8

& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw "Contract compilation failed with exit code $LASTEXITCODE." }

& $exe
if ($LASTEXITCODE -ne 0) { throw "Contract tests failed with exit code $LASTEXITCODE." }
