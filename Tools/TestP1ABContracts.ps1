$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$sources = @(
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\DefinitionId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\SemanticVersion.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\VersionRange.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModManifest.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModManifestReader.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModDiagnostics.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModDiscovery.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\DependencyResolver.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetProvider.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetResolver.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\LooseModProvider.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModScripting.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModScriptingP1C.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModScriptingP1D.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModWarriorTemplates.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModSaveData.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\CoreContentImporter.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\CoreContentImporterP1C.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModContent.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModContentP1B.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModContentP1C.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModContentP1D.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModLocalizationLoader.cs')
)

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$csc = $null
if (Test-Path $vswhere) {
    $csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild `
        -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
}
if (-not $csc -or -not (Test-Path $csc)) { throw 'Could not locate Roslyn csc.exe.' }

$testRoot = Join-Path $root 'Temp\P1ABContracts'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$harness = Join-Path $testRoot 'Program.cs'
$exe = Join-Path $testRoot 'P1ABContracts.exe'

@'
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Modding;

internal static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Reject(Action action, string message)
    {
        try { action(); }
        catch (ModContentException) { return; }
        throw new Exception(message);
    }

    private static string Manifest(string id, bool patch)
    {
        return "schema = 1\n" +
            "id = \"" + id + "\"\n" +
            "name = \"" + id + "\"\n" +
            "version = \"1.0.0\"\n" +
            "api = \">=0.1 <1.0\"\n" +
            "authors = [\"P1AB Contract\"]\n" +
            "entrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"content.register\"" + (patch ? ", \"content.patch\"" : "") + "]\n" +
            "\n[[dependencies]]\n" +
            "id = \"core\"\n" +
            "version = \">=1.0 <2.0\"\n";
    }

    private static ModDescriptor Descriptor(string id, bool patch = false)
    {
        ModManifest manifest = ModManifestReader.ParseExternal(Manifest(id, patch), id + "/mod.toml");
        return new ModDescriptor(manifest, Path.GetFullPath(id), ModSourceKind.Loose);
    }

    private static void ImportCoreStage(ModContentCatalog catalog)
    {
        var doc = new XmlDocument();
        doc.LoadXml("<Zones><Zone Name='ZONE_TEST'><Battle Name='BATTLE_TEST' Type='STORY' X='5' Y='7'>" +
            "<Fight Name='FIGHT_TEST' Power='2' Rounds='3' RoundTime='90' Description='base description'/>" +
            "</Battle></Zone></Zones>");
        Assert(CoreContentImporter.ImportStages(catalog, doc.DocumentElement) == 1,
            "Core stage fixture did not import exactly one fight.");
    }

    private static void ImportWarriorTemplate(ModContentCatalog catalog)
    {
        var doc = new XmlDocument();
        doc.LoadXml("<Templates><Warrior Name='NINJA_TEST'/></Templates>");
        Assert(CoreContentImporter.ImportWarriorTemplates(catalog, doc.DocumentElement) == 1,
            "Core warrior template fixture did not import.");
    }

    public static int Main()
    {
        CheckStageGraphAndP1B();
        CheckPatchConflictAndRestore();
        CheckRollbackAndInvalidReferences();
        Console.WriteLine("P1A/P1B focused contracts: PASS");
        return 0;
    }

    private static void CheckStageGraphAndP1B()
    {
        var catalog = new ModContentCatalog();
        ImportCoreStage(catalog);
        ImportWarriorTemplate(catalog);
        ModDescriptor mod = Descriptor("p1ab.contract");

        using (ModRegistrationTransaction tx = catalog.BeginRegistration(mod))
        {
            DefinitionId title = tx.AddLocalization("weapon.contract", "eng", "Contract Blade");
            WeaponDefinition weapon = tx.RegisterWeapon("contract_blade", title,
                AssetId.Parse("p1ab.contract:sprites/contract_blade"),
                AssetId.Parse("p1ab.contract:models/contract_blade"), "Katana");

            WarriorTemplateDefinition template = tx.GetWarriorTemplate("core:warrior-templates/ninja_test");
            var attributes = new Dictionary<string, float>(StringComparer.Ordinal) { { "Damage", 1.25f } };
            var alignments = new[] { new WarriorAttributeAlignmentDefinition(1.5f, -2f, 4, ModRuleMode.Eclipse) };
            WarriorDefinition warrior = tx.RegisterWarrior("enemy", "Contract", "Enemy", "avatar", "voice", 12,
                "TACTIC_TEST", new[] { weapon.Id }, null, template.Id, true, "group-a", 2, attributes, alignments);
            Assert(warrior.HasTemplate && warrior.Template == template.Id && warrior.Attributes["Damage"] == 1.25f &&
                warrior.AttributeAlignments.Count == 1 && warrior.AttributeAlignments[0].Mode == ModRuleMode.Eclipse,
                "Typed warrior template/attributes/alignment values changed while staged.");

            FightRuleDefinition noPerks = tx.RegisterNoPerksRule("no_perks", ModRuleTarget.Opponent,
                ModRuleMode.Normal, new[] { 1, 3 }, "contract");
            FightRuleDefinition require = tx.RegisterRequireItemRule("require", weapon.Id, 7,
                ModRuleMode.All, new[] { 2 });
            FightRuleDefinition equip = tx.RegisterEquipItemRule("equip", weapon.Id, 8, ModRuleTarget.Player,
                ModRuleMode.Eclipse, new[] { 1 });
            FightRuleDefinition recharge = tx.RegisterRechargeMagicRule("recharge", ModRuleTarget.All,
                ModRuleMode.All, null);
            FightRuleDefinition attrs = tx.RegisterAttributesRule("attrs", ModRuleTarget.Opponent,
                ModRuleMode.Normal, new[] { 2 }, new Dictionary<string, float> { { "Health", 0.75f } });
            Assert(noPerks.Kind == ModFightRuleKind.NoPerks && noPerks.Target == ModRuleTarget.Opponent &&
                require.Kind == ModFightRuleKind.RequireItem && require.Item == weapon.Id && require.MinimumLevel == 7 &&
                equip.Kind == ModFightRuleKind.EquipItem && equip.Mode == ModRuleMode.Eclipse &&
                recharge.Kind == ModFightRuleKind.RechargeMagicEachRound &&
                attrs.Kind == ModFightRuleKind.Attributes && attrs.Attributes["Health"] == 0.75f,
                "Typed fight rule values changed while staged.");

            var choice = new RewardChoiceDefinition(new[]
            {
                new RewardChoiceItem(new RewardItemGrant(weapon.Id, 3), 2.5f),
                new RewardChoiceItem(new RewardItemGrant(weapon.Id), 1f)
            });
            RewardDefinition reward = tx.RegisterReward("items_only",
                new[] { new RewardItemGrant(weapon.Id, 2) }, new[] { choice });
            Assert(reward.Items.Count == 1 && reward.Items[0].UpgradeNumber == 2 && reward.Choices.Count == 1 &&
                reward.Choices[0].Items.Count == 2 && Math.Abs(reward.Choices[0].Items[0].Weight - 2.5f) < 0.0001f,
                "Typed item reward/RewardChoice values changed while staged.");

            ZoneDefinition zone = tx.RegisterZone("zone_local", "contract-zone.xml", false);
            BattleDefinition battleZ = tx.RegisterBattle("battle_z", zone.Id, ModBattleKind.Story, 20, 30,
                null, "Z", null, null, null, null, null, null, false, null, null);
            BattleDefinition battleA = tx.RegisterBattle("battle_a", zone.Id, ModBattleKind.Challenge, 10, 15,
                null, "A", null, null, null, null, null, null, false, null, null);
            FightDefinition fightZ = tx.RegisterFight("fight_z", battleZ.Id, 0, 0, 1, 2, 60, "loc", "music", -1f,
                1f, "z", false, null, new[] { warrior.Id }, new[] { noPerks.Id, require.Id, equip.Id, recharge.Id, attrs.Id },
                new[] { reward.Id });
            FightDefinition fightA = tx.RegisterFight("fight_a", battleZ.Id, 0, 0, 1, 2, 60, "loc", "music", -1f,
                1f, "a", false, null, new[] { warrior.Id }, null, null);

            ZoneDefinition coreZone = tx.GetZone("core:zones/zone_test");
            BattleDefinition appended = tx.RegisterBattle("core_append", coreZone.Id, ModBattleKind.Story, 1, 2,
                null, "Core append", null, null, null, null, null, null, false, null, null);
            FightDefinition appendedFight = tx.RegisterFight("core_append_fight", appended.Id, 0, 0, 1, 1, 45,
                "loc", "music", -1f, 1f, "append", false, null, new[] { warrior.Id }, null, null);

            var condition = new ModQuestCondition(ModQuestCompareOperator.Equal,
                new ModQuestOperand(ModQuestOperandKind.FightId, appendedFight.Id),
                new ModQuestOperand(ModQuestOperandKind.EventFight));
            var nestedButton = new ModQuestDialogButton("Continue", new[]
            {
                ModQuestAction.ToggleBattle(appended.Id, false),
                ModQuestAction.StartCurrentFight(),
                ModQuestAction.UpdateEclipseBattles()
            }, "Green");
            ModQuestAction dialog = ModQuestAction.Dialog("Contract", "hero",
                new[] { new ModQuestDialogLine("Line", "Next") }, nestedButton);
            QuestDefinition quest = tx.RegisterQuest("typed_quest", 11, true, false, ModQuestActionPlace.Map,
                new[] { "contract" }, new[] { "done" }, new[] { ModQuestEventKind.FightEnter, ModQuestEventKind.Dialog },
                new[] { condition }, new[] { dialog, ModQuestAction.ToggleBattle(appended.Id, true),
                    ModQuestAction.StartFight(appendedFight.Id), ModQuestAction.StartCurrentFight(),
                    ModQuestAction.UpdateEclipseBattles() });
            Assert(quest.Events.Count == 2 && quest.Events[0] == ModQuestEventKind.FightEnter &&
                quest.Conditions[0].Left.Kind == ModQuestOperandKind.FightId &&
                quest.Conditions[0].Left.Reference == appendedFight.Id &&
                quest.Actions[0].Button.Actions.Count == 3 &&
                quest.Actions[0].Button.Actions[0].Kind == ModQuestActionKind.ToggleBattle &&
                quest.Actions[0].Button.Actions[1].Kind == ModQuestActionKind.StartCurrentFight &&
                quest.Actions[0].Button.Actions[2].Kind == ModQuestActionKind.UpdateEclipseBattles,
                "Typed P1B event/condition/nested actions lost exact values.");

            tx.Commit();
        }

        ZoneDefinition committedZone;
        Assert(catalog.TryGetZone(DefinitionId.Parse("p1ab.contract:zones/zone_local"), out committedZone) &&
            committedZone.Battles.Count == 2 &&
            committedZone.Battles[0] == DefinitionId.Parse("p1ab.contract:battles/battle_z") &&
            committedZone.Battles[1] == DefinitionId.Parse("p1ab.contract:battles/battle_a"),
            "Zone child order no longer follows registration order.");
        BattleDefinition committedBattle;
        Assert(catalog.TryGetBattle(DefinitionId.Parse("p1ab.contract:battles/battle_z"), out committedBattle) &&
            committedBattle.Fights.Count == 2 &&
            committedBattle.Fights[0] == DefinitionId.Parse("p1ab.contract:fights/fight_z") &&
            committedBattle.Fights[1] == DefinitionId.Parse("p1ab.contract:fights/fight_a"),
            "Battle fight order no longer follows registration order.");
        Assert(catalog.Fights[0].Id.ToString().CompareTo(catalog.Fights[catalog.Fights.Count - 1].Id.ToString()) <= 0,
            "Global fight registry is not deterministic.");

        DefinitionId coreZoneId = CoreContentImporter.ZoneId("ZONE_TEST");
        DefinitionId appendBattleId = DefinitionId.Parse("p1ab.contract:battles/core_append");
        bool foundAppendPatch = false;
        for (int i = 0; i < catalog.Patches.Count; i++)
        {
            ModContentPatchRecord patch = catalog.Patches[i];
            if (patch.Target == coreZoneId && patch.Operation == ModContentPatchOperation.Append &&
                patch.Field == ModContentPolicies.ZoneBattleChildren + appendBattleId.ToString()) foundAppendPatch = true;
        }
        Assert(foundAppendPatch, "Core-zone external battle append did not retain typed patch ownership/provenance.");
    }

    private static void CheckPatchConflictAndRestore()
    {
        var catalog = new ModContentCatalog();
        ImportCoreStage(catalog);
        DefinitionId fightId = CoreContentImporter.FightId("ZONE_TEST", "BATTLE_TEST", "FIGHT_TEST");
        FightDefinition fight;
        Assert(catalog.TryGetFight(fightId, out fight) && fight.Description == "base description" && fight.Rounds == 3,
            "Core fight patch fixture baseline is wrong.");

        using (ModRegistrationTransaction first = catalog.BeginRegistration(Descriptor("patch.one", true)))
        {
            first.PatchFightDescription(fightId.ToString(), "patched description");
            first.PatchFightRounds(fightId.ToString(), 5);
            first.Commit();
        }
        Assert(catalog.TryGetFight(fightId, out fight) && fight.Description == "patched description" && fight.Rounds == 5,
            "Committed typed fight patch did not update the projected fight.");

        int patchesBeforeConflict = catalog.Patches.Count;
        using (ModRegistrationTransaction conflict = catalog.BeginRegistration(Descriptor("patch.two", true)))
        {
            conflict.PatchFightDescription(fightId.ToString(), "must not win");
            Reject(() => conflict.Commit(), "Overlapping core fight field patch was not rejected.");
        }
        Assert(catalog.Patches.Count == patchesBeforeConflict && catalog.TryGetFight(fightId, out fight) &&
            fight.Description == "patched description", "Rejected patch conflict partially mutated the catalog.");

        var rebuilt = new ModContentCatalog();
        ImportCoreStage(rebuilt);
        Assert(rebuilt.TryGetFight(fightId, out fight) && fight.Description == "base description" && fight.Rounds == 3,
            "Rebuilding without the patching mod did not restore the canonical core fight projection.");
    }

    private static void CheckRollbackAndInvalidReferences()
    {
        var catalog = new ModContentCatalog();
        ImportCoreStage(catalog);
        ImportWarriorTemplate(catalog);
        ModDescriptor mod = Descriptor("rollback.p1ab");
        using (ModRegistrationTransaction tx = catalog.BeginRegistration(mod))
        {
            ZoneDefinition zone = tx.RegisterZone("must_rollback");
            BattleDefinition battle = tx.RegisterBattle("must_rollback", zone.Id, ModBattleKind.Story,
                0, 0, null, null, null, null, null, null, null, null, false, null, null);
            DefinitionId missingFight = DefinitionId.Parse("rollback.p1ab:fights/missing");
            Reject(() => tx.RegisterQuest("invalid", 0, false, false, ModQuestActionPlace.Map, null, null,
                new[] { ModQuestEventKind.Session }, null,
                new[] { ModQuestAction.StartFight(missingFight), ModQuestAction.ToggleBattle(battle.Id, true) }),
                "Quest accepted an unavailable fight reference.");
        }
        Assert(catalog.Zones.Count == 1 && catalog.Battles.Count == 1 && catalog.Fights.Count == 1 && catalog.Quests.Count == 0 &&
            !catalog.TryGetZone(DefinitionId.Parse("rollback.p1ab:zones/must_rollback"), out ZoneDefinition ignored),
            "Invalid P1B registration/dispose leaked staged P1A/P1B content.");

        using (ModRegistrationTransaction tx = catalog.BeginRegistration(mod))
        {
            Reject(() => tx.RegisterWarrior("bad_template", "x", "y", null, null, 1, null, null, null,
                DefinitionId.Parse("core:warrior-templates/missing"), true, null, 0, null, null),
                "Warrior accepted a missing core template reference.");
            Reject(() => tx.RegisterReward("bad_reward",
                new[] { new RewardItemGrant(DefinitionId.Parse("core:items/weapon/missing")) }, null),
                "Reward accepted a missing item reference.");
        }
        Assert(catalog.Warriors.Count == 0 && catalog.Rewards.Count == 0,
            "Invalid warrior/reward references leaked staged content.");
    }
}
'@ | Set-Content -LiteralPath $harness -Encoding UTF8

& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw "P1A/P1B contract compilation failed with exit code $LASTEXITCODE." }
& $exe
if ($LASTEXITCODE -ne 0) { throw "P1A/P1B focused contracts failed with exit code $LASTEXITCODE." }

# Runtime adapter assertions intentionally inspect the live adapter source instead of copying its
# semantics into a second test-only implementation. The executable harness above covers the typed
# public graph. These checks pin the exact recovered XML bridge and teardown calls that make that
# graph effective at runtime.
$adapterPath = Join-Path $root 'Assets\Scripts\Eclipse\Modding\LegacyContentAdapter.cs'
$adapter = Get-Content -Raw -LiteralPath $adapterPath
$requiredAdapterContracts = @(
    'case ModQuestOperandKind.FightId: return LegacyFightId(operand.Reference);',
    'node = document.CreateElement("ToggleBattle"); node.SetAttribute("Name", LegacyBattleId(action.Reference));',
    'node.SetAttribute("Toggle", action.Flag ? "on" : "off"); return node;',
    'node = document.CreateElement("Fight"); node.SetAttribute("Name", "_$Fight"); return node;',
    'return document.CreateElement("UpdateEclipseBattles");',
    'button.AppendChild(BuildQuestActionNode(document, action.Button.Actions[i]));',
    'list.AddExternalBattle(zone.LegacyName, battleNode);',
    'list.RemoveExternalBattle(_externalBattles[i].ZoneName, _externalBattles[i].BattleName);',
    'binding.Battle.RestoreSourceDefinitionForModding(binding.Original, out ignored);',
    'RemoveQuests(ListSF.ELEBLBJKDBI());',
    'RemoveStages(ListSF.ELEBLBJKDBI());'
)
foreach ($contract in $requiredAdapterContracts) {
    if (-not $adapter.Contains($contract)) { throw "Legacy P1A/P1B runtime bridge contract missing: $contract" }
}

Write-Host 'P1A/P1B legacy bridge contracts: PASS (exact FightId, nested dialog actions, ToggleBattle, current fight, eclipse refresh, stage/quest teardown, fight restore).'
