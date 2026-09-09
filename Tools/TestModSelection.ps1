$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ModSelection-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $fixture | Out-Null
$sources = @('ModId', 'SemanticVersion', 'VersionRange', 'ModManifest', 'ModManifestReader', 'ModDiagnostics', 'ModDiscovery', 'DependencyResolver', 'ModSelection')
$compile = ($sources | ForEach-Object { '<Compile Include="' + [Security.SecurityElement]::Escape((Join-Path $root "Assets/Scripts/Eclipse/Runtime/Modding/$_.cs")) + '"/>' }) -join "`n"
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup><ItemGroup>
$compile
</ItemGroup></Project>
"@ | Set-Content "$fixture/Test.csproj"
@'
using System;
using System.IO;
using System.Linq;
using Eclipse.Modding;
public static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static ModId Id(string s) => ModId.Parse(s);
    public static void Main(string[] args)
    {
        string root = args[0];
        void Mod(string name, string dependency)
        {
            string folder = Path.Combine(root, name); Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "mod.toml"), "schema = 1\nid = \"" + name + "\"\nname = \"" + name + "\"\nversion = \"1.0.0\"\napi = \">=0.7 <1.0\"\nauthors = [\"Test\"]\nentrypoint = \"scripts/main.lua\"\ncapabilities = []\n[[dependencies]]\nid = \"" + dependency + "\"\nversion = \">=1.0 <2.0\"\n");
        }
        Mod("test.library", "core"); Mod("test.child", "test.library"); Mod("test.grandchild", "test.child"); Mod("test.other", "core");
        var mods = ModDiscovery.DiscoverLoose(root).Mods;
        Check(mods.Count == 4, "Fixture discovery failed.");
        string path = Path.Combine(root, "settings.xml");
        var selection = ModSelection.Load(path);
        Check(selection.Filter(mods).Count == 4, "New mods should default enabled.");
        selection.SetEnabled(Id("test.library"), false, mods);
        Check(selection.Filter(mods).Single().Id == Id("test.other"), "Disable did not cascade through dependents.");
        Check(!File.Exists(path), "Draft toggle persisted before Apply.");
        selection.Save(path);
        var restored = ModSelection.Load(path);
        Check(restored.Filter(mods).Count == 1, "Restart lost selection.");
        var resolution = DependencyResolver.Resolve(restored.Filter(mods), ModPlatformVersions.Api, ModPlatformVersions.Core);
        Check(!resolution.HasErrors && resolution.OrderedMods.Count == 1, "Intentionally disabled mods reached dependency resolution.");
        restored.SetEnabled(Id("test.grandchild"), true, mods);
        Check(restored.Filter(mods).Count == 4, "Enabling child did not enable requirements.");
        restored.SetEnabled(Id("test.child"), false, mods);
        Check(restored.IsEnabled(Id("test.library")) && !restored.IsEnabled(Id("test.grandchild")), "Disabling child incorrectly disabled its dependency.");
        restored.Save(path); restored.Save(path);
        Check(ModSelection.Load(path).Filter(mods).Count == 2, "Atomic replacement save failed.");
        Check(Directory.GetFiles(root, "*.tmp").Length == 0, "Temporary selection files leaked.");
        restored.SetEnabled(Id("test.absent"), false, mods); restored.Save(path);
        Check(!ModSelection.Load(path).IsEnabled(Id("test.absent")), "Absent mod preference discarded.");
        bool rejected = false; try { restored.SetEnabled(Id("core"), false, mods); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && restored.IsEnabled(Id("core")), "Core disabled.");
        File.WriteAllText(path, "<ModSelection version='99'/>");
        rejected = false; try { ModSelection.Load(path); } catch (InvalidDataException) { rejected = true; }
        Check(rejected, "Unknown settings version silently reset.");
        Console.WriteLine("PASS: " + checks + " mod selection checks (draft, dependency cascades, persistence, isolation, absent IDs, core protection, invalid schema).");
    }
}
'@ | Set-Content "$fixture/Program.cs"
dotnet run --project "$fixture/Test.csproj" -- "$fixture/Mods"
if ($LASTEXITCODE -ne 0) { throw 'Mod selection tests failed.' }
