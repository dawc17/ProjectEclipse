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
public sealed class Fighter : IModFighterOperations, IModDamageEventSource, IModBehaviorInstanceSource, IModFighterTargets, IModIncomingHitSource, IModFighterEffects {
    public ModIncomingHit IncomingHit { get; set; }
    public readonly ModDamageShields Shields = new ModDamageShields();
    public bool TrySetDamageShield(object key,double fraction,int frames,out string error) => Shields.TrySet(key,fraction,frames,0,out error);
    public bool TryRemoveDamageShield(object key,out string error) { Shields.Remove(key); error=""; return true; }
    public XmlNode SavedInstance { get; set; }
    public double Health => 1;
    public IModFighterOperations Opponent => null;
    public double Charge; public ModDamageEvent DamageEvent { get; set; }
    public bool TryChangeHealth(double amount, out string error) { error = ""; return true; }
    public bool TryAddMagicCharge(double amount, out string error) { Charge += amount; error = ""; return true; }
}
public static class Program {
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Main(string[] args) {
        var discovered = ModDiscovery.DiscoverLoose(args[0]);
        Check(!discovered.HasErrors && discovered.Mods.Count == 1, "Sample discovery failed.");
        var mod = discovered.Mods[0]; var catalog = new ModContentCatalog(); var state = new ModStateRuntime();
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[] { "Simple" });
        var templates = new XmlDocument(); templates.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
        CoreContentImporter.ImportWarriorTemplates(catalog, templates.DocumentElement);
        var list = new XmlDocument(); list.Load(args[1]);
        var nodes = list.SelectNodes("//Item").Cast<XmlNode>().ToArray();
        var languages = new Dictionary<string,XmlDocument>();
        CoreContentImporter.ImportWeapons(catalog, nodes, languages);
        CoreContentImporter.ImportArmors(catalog, nodes, languages);
        CoreContentImporter.ImportHelms(catalog, nodes, languages);
        CoreContentImporter.ImportRanged(catalog, nodes, languages);
        CoreContentImporter.ImportMagic(catalog, nodes, languages);
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
                var instanceNode = save.CreateElement("Perk"); save.DocumentElement.AppendChild(instanceNode);
                var fighter = new Fighter { SavedInstance = instanceNode, DamageEvent = new ModDamageEvent(1, 1, 0.9, false, false) };
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
                Check(catalog.ForgeRecipeFamilies.Count == 4, "Forge variants missing.");
                Check(catalog.Warriors.Single(w => w.Id.LocalId == "trial_fighter").Tactic == "Standard",
                    "Phase 2 opponent needs an explicit AI tactic; the default template supplies none.");
                Check(catalog.Modes.Count == 3, "Integrated mode definitions missing.");
                ModPolicies.Content = catalog;
                Check(ModPolicies.DeliverySeconds("forge", 600) == 0 && !ModPolicies.SkipEnabled("forge"), "Forge timer policy ignored.");
                Check(!ModPolicies.FeatureEnabled("battle_pass") && !ModPolicies.FeatureEnabled("ads"), "Service policy ignored.");
                var trial = catalog.Modes.Single(m => m.Id.LocalId == "ascension_trial");
                var progress = new ModModeProgress(save.DocumentElement, trial);
                Check(progress.Step == 0, "Initial progression wrong.");
                progress.Enter(); progress.Complete(trial, true); progress.Complete(trial, true);
                Check(progress.Step == 1, "Duplicate result advanced progression twice.");
                var reload = new XmlDocument(); reload.LoadXml(save.OuterXml);
                progress = new ModModeProgress(reload.DocumentElement, trial);
                Check(progress.Step == 1 && !progress.Entered, "Reload lost progression.");
                progress.Enter(); progress.Complete(trial, false);
                Check(progress.Step == 0, "Loss did not reset trial.");
                for (int i=0;i<3;i++) { progress.Enter(); progress.Complete(trial,true); }
                Check(progress.Step == 0 && progress.Completions == 1, "Repeatable completion failed.");
                var raid = catalog.Modes.Single(m => m.Raid);
                Check(!raid.HasEntryItem && raid.EntryCount == 0, "Volcano must be replayable without tickets.");
                var raidProgress = new ModModeProgress(reload.DocumentElement, raid);
                raidProgress.Enter(); raidProgress.Complete(raid, true); raidProgress.Enter(); raidProgress.Complete(raid, false);
                Check(raid.Fights.Count == 1 && raidProgress.Step == 0 && raidProgress.Completions == 1, "Single-boss raid completion/reset failed.");
                catalog.TryGetFight(raid.Fights[0], out var raidFight); catalog.TryGetBattle(raidFight.Battle, out var raidBattle); catalog.TryGetZone(raidBattle.Zone,out var raidZone);
                Check(raidFight.RoundTime == 300 && raidFight.Rounds == 1, "Raid timer/rounds incorrect.");
                Check(catalog.Warriors.Single(w => w.Id.LocalId == "raid_boss").HealthBars == 10, "Raid boss lacks multi-bar health.");
                Check(catalog.Rewards.Single(r => r.Id.LocalId == "raid_gems").Gems == 25, "Raid gem reward missing.");
                Check(ModPolicies.IsRaidZone(raidZone.LegacyName) && !raidZone.LegacyName.StartsWith("ZONE_RAID"), "Raid still depends on legacy name.");
                using (var probe = catalog.BeginRegistration(mod)) {
                    foreach (int invalid in new[] { -1, 10001 }) {
                        bool rejectedPool = false;
                        try { probe.RegisterWarrior("invalid_pool", "", "", "", "", 1, "Standard", null, null, healthBars: invalid); }
                        catch (ModContentException) { rejectedPool = true; }
                        Check(rejectedPool, "Invalid health-bar count accepted.");
                    }
                    foreach (int invalid in new[] { -1, 1000001 }) {
                        bool rejectedGems = false;
                        try { probe.RegisterReward("invalid_gems", null, null, invalid); }
                        catch (ModContentException) { rejectedGems = true; }
                        Check(rejectedGems, "Invalid gem reward accepted.");
                    }
                    var scheduled = probe.RegisterMode("scheduled_probe", new[] { trial.Fights[0] }, false, false, false, false, 2, 100, 200, default, 0);
                    Check(!scheduled.IsAvailable(1,150) && !scheduled.IsAvailable(2,99) && scheduled.IsAvailable(2,100) && !scheduled.IsAvailable(2,200), "Schedule or eligibility boundary failed.");
                    var once = new ModModeProgress(reload.DocumentElement, scheduled); once.Enter(); once.Complete(scheduled,true);
                    Check(once.Step == 1 && once.Completions == 1, "One-time completion failed.");
                    probe.SetTimer("forge", 30, true);
                    bool rejected = false; try { probe.Commit(); } catch(ModContentException) { rejected = true; }
                    Check(rejected && catalog.Modes.Count == 3 && ModPolicies.DeliverySeconds("forge",600)==0, "Conflicting transaction partially committed.");
                }
                var veteran = catalog.Perks.Single(p => p.Id.LocalId == "veteran_guard");
                catalog.TryGetBehavior(veteran.Behavior, out var veteranDefinition);
                var veteranParameters = veteranDefinition.Parameters.ResolveValues(veteran.InitialParameters);
                fighter.SavedInstance = save.CreateElement("Perk");
                Check(Invoke(veteran.Behavior, ModEffectEvent.Block, veteranParameters), "Saved guard callback failed.");
                Check(fighter.SavedInstance.OuterXml.Contains("Value=\"1\""), "Instance state was not persisted.");
                var migrationNode = save.CreateElement("Perk");
                migrationNode.InnerXml = "<EclipseBehaviorState Behavior='example.phase2:behaviors/veteran_guard' Version='1'><Value Name='activations' Type='Integer' Value='7'/></EclipseBehaviorState>";
                fighter.SavedInstance = migrationNode;
                Check(Invoke(veteran.Behavior,ModEffectEvent.Block,veteranParameters) && migrationNode.OuterXml.Contains("Value=\"8\"") && migrationNode.OuterXml.Contains("Version=\"2\""), "Saved instance migration failed.");
                var futureNode = save.CreateElement("Perk"); futureNode.InnerXml = migrationNode.InnerXml.Replace("Version=\"2\"", "Version=\"99\""); fighter.SavedInstance=futureNode;
                string preserved=futureNode.OuterXml;
                Check(!Invoke(veteran.Behavior,ModEffectEvent.Block,veteranParameters) && futureNode.OuterXml==preserved, "Future saved state was overwritten.");
                var ward = catalog.Perks.Single(p => p.Id.LocalId == "opening_ward");
                catalog.TryGetBehavior(ward.Behavior, out var wardDefinition);
                var wardParameters = wardDefinition.Parameters.ResolveValues(ward.InitialParameters);
                fighter.SavedInstance = save.CreateElement("Perk");
                ctx["side"]="player"; ctx["fight_id"]="ward_fight"; ctx["round"]="1";
                double damage=10; fighter.IncomingHit=new ModIncomingHit(()=>damage, value=>damage=value);
                Check(Invoke(ward.Behavior,ModEffectEvent.DamageResolving,wardParameters) && damage==5, "Ward failed to mutate pending damage.");
                damage=10; Check(Invoke(ward.Behavior,ModEffectEvent.DamageResolving,wardParameters) && damage==10, "Ward repeated within round.");
                ctx["round"]="2"; Check(Invoke(ward.Behavior,ModEffectEvent.DamageResolving,wardParameters) && damage==5, "Round state did not reset automatically.");
                string shieldError;
                var shields = new ModDamageShields();
                Check(shields.TrySet("a",0.25,180,10,out shieldError) && shields.Scale(189)==0.75 && shields.Scale(190)==1, "Simulation-frame shield expiry is wrong.");
                Check(shields.TrySet("a",0.5,10,200,out shieldError) && shields.TrySet("a",0.25,10,200,out shieldError) && shields.Scale(200)==0.75, "Shield refresh stacked instead of replacing.");
                Check(!shields.TrySet("b",double.NaN,10,200,out shieldError) && !shields.TrySet("b",1,3601,200,out shieldError), "Invalid timed effect accepted.");
                shields.Clear(); Check(shields.Scale(200)==1, "Round cleanup retained timed effects.");
                var hostSave = new XmlDocument(); hostSave.LoadXml("<Warrior/>");
                ModModeRuntime.Bind(hostSave.DocumentElement);
                foreach (var entry in catalog.Fights) {
                    catalog.TryGetBattle(entry.Battle,out var owner);
                    string runtimeId=catalog.RuntimeFightId(entry.Id);
                    ListSF.Fights[runtimeId]=new FightList { BCKFACGMOKC=new FightIDS(runtimeId), CNAOMDMIGLJ=new Battle { Name=owner.LegacyName } };
                }
                var raidEntry=ListSF.Fights[catalog.RuntimeFightId(raid.Fights[0])];
                Check(ModPolicies.TryRaidBattle(raidEntry.CNAOMDMIGLJ.get_Name(), out var hardMode),
                    "Raid metadata must resolve before parsing assigns the fight's runtime ID.");
                Check(ModModeRuntime.TryProgress(raidEntry, out var completedSteps, out var stepCount) && completedSteps==0 && stepCount==1,
                    "Initial map indicators disagree with mode progress.");
                Check(ModModeRuntime.ResolveEntry(ref raidEntry) && ModModeRuntime.Begin(raidEntry), "Free raid entry was blocked.");
                Check(ModModeRuntime.Begin(raidEntry), "Duplicate entry failed.");
                ModModeRuntime.CancelLaunch(raidEntry);
                Check(ModModeRuntime.Begin(raidEntry), "Retry failed after scene-launch rollback.");
                ModModeRuntime.NotifyEntry(raidEntry);
                Check(ListSF.Quests.Events.Contains(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_ENTER), "Raid entry quest event missing.");
                Check(ModModeRuntime.CanResolve(raidEntry), "Active raid result rejected.");
                ModModeRuntime.Complete(raidEntry,true);
                Check(!ModModeRuntime.CanResolve(raidEntry), "Duplicate raid result accepted.");
                Check(ModModeRuntime.TryProgress(raidEntry, out completedSteps, out stepCount) && completedSteps==0 && stepCount==1,
                    "Single-boss raid indicators did not reset after victory.");
                Check(ModModeRuntime.TryCurrent(raidEntry.CNAOMDMIGLJ,out var next) && next.BCKFACGMOKC.ToString()==catalog.RuntimeFightId(raid.Fights[0]), "Map did not return to the repeatable boss.");
                string orphanSave=hostSave.OuterXml; ModModeRuntime.Clear(); ModPolicies.Content=new ModContentCatalog();
                Check(!ModModeRuntime.TryFind(raidEntry.BCKFACGMOKC.ToString(),out var missing) && hostSave.OuterXml==orphanSave, "Missing mod changed orphan progress.");
                ModPolicies.Content=catalog; ModModeRuntime.Bind(hostSave.DocumentElement);
                Check(ModModeRuntime.ResolveEntry(ref raidEntry) && raidEntry==next && ModModeRuntime.Begin(raidEntry), "Reinstall lost progression or key accounting.");
                ModModeRuntime.Complete(raidEntry,true); ModModeRuntime.NotifyResult(raidEntry);
                Check(ModModeRuntime.TryProgress(raidEntry, out completedSteps, out stepCount) && completedSteps==0,
                    "Repeatable mode indicators did not reset after completion.");
                Check(ListSF.Quests.Events.Contains(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_END), "Raid completion quest event missing.");
                ModModeRuntime.SetRaidResult(new FightResult());
                Check(ModModeRuntime.ShowRaidResult() && !ModModeRuntime.ShowRaidResult() && Fight.Instance.Presented==1, "Raid result presentation duplicated.");
                for (int replay=0; replay<3; replay++) {
                    Check(ModModeRuntime.EntryStatus(raidEntry)=="" && ModModeRuntime.ResolveEntry(ref raidEntry) && ModModeRuntime.Begin(raidEntry), "Completed raid became locked on replay.");
                    ModModeRuntime.Complete(raidEntry, replay!=1);
                }
                ModModeRuntime.Clear();
                ModPolicies.Content = null;
            }
        }
        Console.WriteLine("PASS: Phase 2 public Lua, typed instance state/migration, capability lifetimes, mode persistence/repeat/loss/schedule, raid metadata/keys, timer/service policy and atomic rollback.");
    }
}
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$hostSource = [Security.SecurityElement]::Escape((Join-Path $root "Assets/Scripts/Eclipse/Modding/ModModeRuntime.cs"))
$hostStubs = [Security.SecurityElement]::Escape((Join-Path $root "Tools/Phase2HostStubs.cs"))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Compile Include="$hostSource"/><Compile Include="$hostStubs"/>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'P2A.csproj')
dotnet run --project (Join-Path $fixture 'P2A.csproj') -- (Join-Path $fixture 'Mods') (Join-Path $root 'Assets/vanillaXml/list.xml')
if ($LASTEXITCODE -ne 0) { throw 'P2A combat runtime fixture failed.' }
