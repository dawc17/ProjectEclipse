$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
# Build the production MoonSharp/runtime sources using the existing isolated harness.
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/P2ACombat-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $fixture 'Mods') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Mods/example.phase2') -Destination (Join-Path $fixture 'Mods') -Recurse
$entry = Join-Path $fixture 'Mods/example.phase2/scripts/main.lua'
@'
local retained
sf2.behaviors.register {
    id = "lifetime_probe",
    on_fight_begin = function(p, fighter) retained = fighter end,
    on_damage_received = function(p, fighter, event)
        assert(type(event.round) == "number" and type(event.blocked) == "boolean")
        assert(type(event.critical) == "boolean" and type(event.damage) == "number")
        retained:add_magic_charge(1)
    end,
}
sf2.behaviors.register {
    id = "failure_probe",
    on_damage_received = function(p, fighter, event) error("isolated failure") end,
}
'@ | Add-Content -LiteralPath $entry
@'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
public sealed class Core : IAssetProvider {
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id, out AssetMetadata metadata) {
        metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "fixture"); return true;
    }
}
public sealed class Fighter : IModFighterOperations, IModDamageEventSource {
    public double Charge; public ModDamageEvent DamageEvent { get; set; }
    public bool TryChangeHealth(double amount, out string error) { error = "unexpected health mutation"; return false; }
    public bool TryAddMagicCharge(double amount, out string error) { Charge += amount; error = ""; return true; }
}
public static class Program {
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Main(string[] args) {
        var discovered = ModDiscovery.DiscoverLoose(args[0]);
        Check(!discovered.HasErrors && discovered.Mods.Count == 1, "Sample discovery failed.");
        var mod = discovered.Mods[0]; var catalog = new ModContentCatalog(); var state = new ModStateRuntime();
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[] { "Simple" });
        var assets = new AssetResolver(new IAssetProvider[] { new Core(), new LooseModProvider(mod) });
        using (var tx = catalog.BeginRegistration(mod)) {
            ModLocalizationLoader.Load(mod, assets, tx);
            var api = new ModApiFacade(mod, assets, tx, state, null);
            using (var context = new MoonSharpScriptRuntime().CreateContext(mod, api)) {
                context.ExecuteEntrypoint(); tx.Commit();
                var save = new XmlDocument(); save.LoadXml("<Warrior/>");
                ModSaveData.RecordContext(save.DocumentElement, new[] { mod }, catalog, state);
                state.Bind(save.DocumentElement, new[] { context });
                var interactive = (IModInteractiveBehaviorScriptContext)context;
                var fighter = new Fighter { DamageEvent = new ModDamageEvent(1, 1, 0.9, false, false) };
                var ctx = new Dictionary<string,string> { { "item_id", "weapon" }, { "perk_id", "resolve" } };
                string error;
                bool Invoke(DefinitionId behavior, ModEffectEvent ev, IReadOnlyDictionary<string,ModParameterValue> p) =>
                    interactive.TryInvokeBehavior(behavior, ev, p, ctx, fighter, out error);
                var normal = catalog.Perks.Single(p => p.Id.LocalId == "resolve");
                var quick = catalog.Perks.Single(p => p.Id.LocalId == "resolve_quick");
                Check(normal.Behavior == quick.Behavior, "Variants do not reuse one behavior.");
                catalog.TryGetBehavior(normal.Behavior, out var definition);
                var parameters = definition.Parameters.ResolveValues(normal.InitialParameters);
                Check(Invoke(normal.Behavior, ModEffectEvent.FightBegin, parameters), "Fight reset failed.");
                Check(Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters) && fighter.Charge == 0, "First hit charged.");
                fighter.DamageEvent = new ModDamageEvent(1, 0.9, 0.8, true, false);
                Check(Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters) && fighter.Charge == 0, "Blocked hit counted.");
                fighter.DamageEvent = new ModDamageEvent(1, 0.9, 0.8, false, true);
                Check(Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters) && fighter.Charge == 0, "Second hit charged.");
                Check(Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters) && fighter.Charge == 0.2, "Third hit did not charge.");
                Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters);
                Invoke(normal.Behavior, ModEffectEvent.FightBegin, parameters);
                Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters);
                Invoke(normal.Behavior, ModEffectEvent.DamageReceived, parameters);
                Check(fighter.Charge == 0.2, "Fight reset retained hit counter.");
                ctx["perk_id"] = "quick"; parameters = definition.Parameters.ResolveValues(quick.InitialParameters);
                Invoke(quick.Behavior, ModEffectEvent.FightBegin, parameters);
                Invoke(quick.Behavior, ModEffectEvent.DamageReceived, parameters);
                Invoke(quick.Behavior, ModEffectEvent.DamageReceived, parameters);
                Check(Math.Abs(fighter.Charge - 0.3) < 1e-9, "Quick variant parameters were ignored.");
                var empty = new Dictionary<string,ModParameterValue>();
                var lifetime = DefinitionId.Parse("example.phase2:behaviors/lifetime_probe");
                Check(Invoke(lifetime, ModEffectEvent.FightBegin, empty), "Lifetime setup failed.");
                Check(!Invoke(lifetime, ModEffectEvent.DamageReceived, empty), "Retained fighter handle remained usable.");
                Check(!Invoke(DefinitionId.Parse("example.phase2:behaviors/failure_probe"), ModEffectEvent.DamageReceived, empty), "Handler exception escaped isolation.");
                Check(Invoke(quick.Behavior, ModEffectEvent.DamageReceived, parameters), "Failure disabled unrelated behavior.");
                Check(state.TryGetValue(mod.Id, "activations", out var activations) && activations.ToWireString() == "2",
                    "Successful activations did not update mod-owned state exactly twice.");
                Check(catalog.ForgeRecipeFamilies.Count == 2, "Forge variants missing.");
            }
        }
        Console.WriteLine("PASS: P2A real Lua decisions, shared parameterized behavior, hit filtering, fight reset, capability expiration and failure isolation.");
    }
}
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'P2A.csproj')
dotnet run --project (Join-Path $fixture 'P2A.csproj') -- (Join-Path $fixture 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'P2A combat runtime fixture failed.' }
