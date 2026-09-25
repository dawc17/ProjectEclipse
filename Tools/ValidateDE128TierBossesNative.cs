using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Eclipse.Modding;
using Eclipse.Underworld;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Dialogs;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only in the independently copied project made by TestDE128UnderworldNative.py.
[InitializeOnLoad]
public static class ValidateDE128TierBossesNative
{
    const string Active = "Eclipse.DE128TierBossesNative.Active";
    const string Prefix = "[DE128TierBossesNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly BindingFlags HiddenStatic = BindingFlags.Static | BindingFlags.NonPublic;
    static double started, lastPress, lastReport, lastWaveDefeat;
    static bool campaign, mapRequested, raidPrepared, entryRequested, surrenderRequested, ownerArtValidated;
    static int targetIndex, storyPresses, storyIntros;
    static int waspWaveDefeats, waspEnteredAt = -1, waspFirstFlyAt = -1, waspLastFlyAt = -1;
    static int butcherWaveDefeats, butcherEnteredAt = -1, butcherSelectedAt = -1;
    static bool butcherChildSpawned, butcherChildMove, butcherChildAttack, butcherChildDeleted;
    static int hermitWaveDefeats, hermitEnteredAt = -1, hermitSelectedAt = -1, hermitIdleAt = -1;
    static int hermitStormSpawns, hermitIdleStormSpawns;
    static readonly HashSet<int> hermitAttackStarts = new HashSet<int>();
    static int hermitPlayerDefeatedAt = -1;
    static int warEnteredAt = -1, warSelectedAt = -1;
    static bool warAttack, warEffectStopped, warCaptured;
    static readonly HashSet<string> warEffects = new HashSet<string>();
    static int gatekeeperEnteredAt = -1, gatekeeperSelectedAt = -1;
    static bool gatekeeperChild, gatekeeperAttack, gatekeeperElectro, gatekeeperPowerEffects, gatekeeperCaptured;
    static bool gatekeeperElectroAnchored, gatekeeperPowerAnchored, gatekeeperPowerCaptured;
    static int blacknessEnteredAt = -1, blacknessSelectedAt = -1;
    static bool blacknessChild, blacknessTransition, blacknessAttack, blacknessDeleted, blacknessEffect, blacknessCaptured;
    static Model blacknessHand;
    static int saturnEnteredAt = -1, saturnSelectedAt = -1, saturnSecondAt = -1;
    static bool saturnWasCasting;
    static bool saturnPistol, saturnBullet1, saturnBullet2, saturnBulletAttack1, saturnBulletAttack2;
    static bool saturnEffect1, saturnEffect2, saturnCaptured, saturnPistolDeleted;
    static readonly HashSet<string> saturnPistolPhases = new HashSet<string>();
    static int mercenaryWaveDefeats, mercenaryEnteredAt = -1;
    static List<FightDefinition> targets;
    static string combatException;

    static ValidateDE128TierBossesNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        Application.logMessageReceived += CaptureCombatException;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-underworld-fixture.marker")))
            throw new InvalidOperationException("Tier boss acceptance requires an isolated project copy.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        string profileTag = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_PROFILE_TAG");
        PlayerSettings.productName = Path.GetFileName(root) +
            (string.IsNullOrEmpty(profileTag) ? string.Empty : "-" + profileTag);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (combatException != null) throw new Exception("Native boss combat exception: " + combatException);
            double timeout = Environment.GetEnvironmentVariable("ECLIPSE_DE128_MERCENARY_WAVE") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_BUTCHER_WAVE") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_HERMIT_WAVE") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_WAR_WHIRL") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_GATEKEEPER_FIELD") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_BLACKNESS_GRASP") == "1" ||
                Environment.GetEnvironmentVariable("ECLIPSE_DE128_SATURN_BLASTER") == "1" ? 420 : 180;
            if (EditorApplication.timeSinceStartup - started > timeout)
                throw new Exception("Timed out on boss " + targetIndex + " of " + (targets?.Count ?? 0) +
                    ": entry=" + entryRequested + " cards=" + storyPresses +
                    " fight=" + (Fight.GetCurrentFight() != null));
            if (!campaign && Eclipse.UI.TitleScreen.IsOpen)
            {
                var title = UnityEngine.Object.FindObjectOfType<Eclipse.UI.TitleScreen>();
                if (title != null)
                {
                    typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign", Hidden).Invoke(title, null);
                    campaign = true;
                }
                return;
            }
            var scripts = ModRuntime.Scripts;
            var roster = ListSF.CCDKHLAMKKO();
            var module = Module.GetInstance();
            if (scripts == null || roster == null || module == null) return;
            if (scripts.Diagnostics.Count != 0 || scripts.StateDiagnostics.Count != 0)
                throw new Exception("DE128 diagnostics: " + string.Join("; ", scripts.Diagnostics) +
                    "; state=" + string.Join("; ", scripts.StateDiagnostics));
            if (!mapRequested && module.NMCNDOPKFJD() == ScreenType.ModuleDojo)
            {
                var menu = MainMenu.get_Instance();
                if (menu == null) return;
                scripts.State.SetValues(ModId.Parse("de128"), new Dictionary<string, ModParameterValue> {
                    { "uw_intro", ModParameterValue.FromInteger(2) }
                });
                roster.Level = 4;
                menu.SkipTutorial();
                mapRequested = true;
                return;
            }
            if (surrenderRequested)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var returned = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (returned == null) return;
                if (IsSpotlightFight(Target.Id.ToString()) && GameObject.Find("LightInTheDarkness") != null)
                    throw new Exception("Spotlight survived the fight return: " + Target.Id);
                if (returned.GetCurrentState() != MapScene.NMFLNANKNOJ.RaidMode)
                    throw new Exception("Boss " + Target.Id + " surrendered to the story map.");
                Debug.Log(Prefix + "Returned from " + Target.Id + " to the Underworld map.");
                if (storyPresses > 0) storyIntros++;
                targetIndex++;
                started = EditorApplication.timeSinceStartup;
                storyPresses = 0;
                entryRequested = surrenderRequested = false;
                combatException = null;
                if (targetIndex == targets.Count)
                {
                    bool storyMatrix = Environment.GetEnvironmentVariable(
                        "ECLIPSE_DE128_UNDERWORLD_MATRIX") == "story";
                    if (storyMatrix && storyIntros < 30)
                        throw new Exception("Only " + storyIntros + " of 32 archived boss intros appeared on this profile.");
                    Debug.Log(Prefix + "PASS: " + targets.Count + " boss fights rendered arenas and fighter rigs, " +
                        "ran 30 combat frames each, and returned to Underworld; " + storyIntros +
                        " native story intros appeared on this profile.");
                    Finish(0);
                }
                return;
            }
            if (!entryRequested)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var scene = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (scene == null) return;
                if (!raidPrepared)
                {
                    ValidateOwnerRaidArt();
                    scene.SwitchToRaidMap();
                    var container = (MapContainer)typeof(MapScene).GetField("_storyContainer", Hidden).GetValue(scene);
                    if (container.GetZonesCount() != 8)
                        throw new Exception("Native raid map did not load eight tiers.");
                    targets = SelectTargets(scripts.Content);
                    raidPrepared = true;
                    return;
                }
                if (typeof(ModRuntime).GetMethod("ReadyProgressionMap", HiddenStatic).Invoke(null, null) == null)
                {
                    PressBlockingStory();
                    if (EditorApplication.timeSinceStartup - lastReport > 15)
                    {
                        lastReport = EditorApplication.timeSinceStartup;
                        Debug.Log(Prefix + "Waiting for map readiness at boss " + targetIndex + ".");
                    }
                    return;
                }
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(Target.Id)));
                if (encounter == null || !UnderworldZonePolicy.IsRaidZone(encounter.Battle?.OAEIILGHJMG))
                    throw new Exception("Tier boss has no native raid encounter: " + Target.Id);
                bool immediate = GameUtils.StartFight(encounter, false, null, true, false);
                entryRequested = true;
                bool preparing = (bool)typeof(ModModeRuntime).GetProperty("HasPendingPreparation", HiddenStatic).GetValue(null);
                if (!immediate && !preparing && !StoryBus.FightEntries.HasPending)
                    throw new Exception("Tier boss entry was refused: " + Target.Id);
                Debug.Log(Prefix + "Requested boss " + (targetIndex + 1) + "/" + targets.Count + ": " + Target.Id);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null)
            {
                if (hermitPlayerDefeatedAt >= 0)
                    throw new Exception("Hermit finished the round without selecting his authored victory move.");
                if (EditorApplication.timeSinceStartup - lastPress < 0.2) return;
                var presenter = UnityEngine.Object.FindObjectsOfType<ModStoryDialogPresenter>()
                    .FirstOrDefault(value => value != null && value.gameObject.activeInHierarchy);
                if (presenter == null) return;
                var dialog = typeof(ModStoryDialogPresenter).GetField("dialog", Hidden).GetValue(presenter) as StoryDialog;
                if (dialog == null || dialog.get_ButtonOK() == null || ++storyPresses > 24)
                    throw new Exception("Tier boss story has no usable native card: " + Target.Id);
                lastPress = EditorApplication.timeSinceStartup;
                PressStoryButton(dialog);
                return;
            }
            if (StoryBus.FightEntries.HasPending)
                throw new Exception("Tier boss retained its native story hold: " + Target.Id);
            var enemy = (Model)typeof(Fight).GetField("CKNCPOABFBO", Hidden).GetValue(fight);
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (enemy == null || player == null) return;
            player.Parameters.set_IsImmortalityEnabled(true);
            if (fight.get_FightTimeInFrames() < 30) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_WASP_WAVE") == "1" &&
                !ObserveWaspWave(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_BUTCHER_WAVE") == "1" &&
                !ObserveButcherWave(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_HERMIT_WAVE") == "1" &&
                !ObserveHermitWave(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_WAR_WHIRL") == "1" &&
                !ObserveWarWhirl(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_GATEKEEPER_FIELD") == "1" &&
                !ObserveGatekeeperField(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_BLACKNESS_GRASP") == "1" &&
                !ObserveBlacknessGrasp(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_SATURN_BLASTER") == "1" &&
                !ObserveSaturnBlaster(fight, enemy)) return;
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_MERCENARY_WAVE") == "1" &&
                !ObserveMercenaryWave(fight, enemy)) return;
            var live = fight.OGNINOBBHIG();
            if (live?.FightId?.ToString() != new FightIDS(scripts.Content.RuntimeFightId(Target.Id)).ToString() ||
                enemy.CLDMEJKGLBA() == null || player.CLDMEJKGLBA() == null)
                throw new Exception("Tier boss fight or native fighter rig differs: " + Target.Id);
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_TITAN_EQUIPMENT") == "1")
            {
                string[] names = { "de128:items/weapon/titans_desolator", "de128:items/armor/titans_form",
                    "de128:items/helm/titans_helm", "de128:items/ranged/titans_harpoon",
                    "de128:items/magic/titans_mind_throw" };
                var equipped = player.Parameters.PJNJIJIODHE().Select(item => item.Name).ToArray();
                if (names.Any(name => !equipped.Contains(name)))
                    throw new Exception("The native fighter lost saved Titan equipment: " + string.Join(",", equipped));
                var macros = player.CLDMEJKGLBA().BLFJJAEFKKP();
                int bodyNodes = macros.Count(node => node.GetName().StartsWith("TitanBody_", StringComparison.Ordinal));
                int headNodes = macros.Count(node => node.GetName().StartsWith("TitanHead_", StringComparison.Ordinal));
                if (bodyNodes != 8 || headNodes != 12)
                    throw new Exception("Titan's archived body or helm geometry fell back: body=" +
                        bodyNodes + " helm=" + headNodes);
                Debug.Log(Prefix + "Five saved Titan items loaded; 8 body and 12 helm macro nodes in the native rig.");
            }
            var location = (Location)typeof(Fight).GetField("_location", Hidden).GetValue(fight);
            int sprites = location?.layers?.Sum(layer => layer.ICDCIANNAAI == null ? 0 :
                layer.ICDCIANNAAI.GetComponentsInChildren<SpriteRenderer>(true).Length) ?? 0;
            if (location?.name != live.Location || sprites == 0)
                throw new Exception("Tier boss arena did not render: " + Target.Id +
                    " location=" + location?.name + " sprites=" + sprites);
            if (IsSpotlightFight(Target.Id.ToString())) ValidateSpotlight(player, Target.Id.ToString());
            ValidateRecoveredAliasEquipment(live, enemy, Target.Id.ToString());
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_CAPTURE_SPOTLIGHT") == "1" &&
                (IsSpotlightFight(Target.Id.ToString()) ||
                 Target.Id.ToString() == "de128:fights/uw_boss_13_1"))
                CaptureCombatFrame(Target.Id.ToString());
            if (Target.Id.ToString() == "de128:fights/uw_boss_berstuuk_1")
            {
                var armor = enemy.Parameters.Armor;
                var helm = enemy.Parameters.Helm;
                if (armor?.Name != "de128:items/armor/berstuuk_form" ||
                    armor.ModelFileName != "de128:models/underworld/mdl_body_berstuuk_early" ||
                    helm?.Name != "de128:items/helm/berstuuk_mask" ||
                    helm.ModelFileName != "de128:models/underworld/mdl_head_berstuuk")
                    throw new Exception("Berstuuk's native body or mask model was not equipped.");
                var macros = enemy.CLDMEJKGLBA().BLFJJAEFKKP();
                int bodyNodes = macros.Count(node => node.GetName().StartsWith("MacroBerstuukBody-", StringComparison.Ordinal));
                int headNodes = macros.Count(node => node.GetName().StartsWith("MacroBerstuukHead-", StringComparison.Ordinal));
                if (bodyNodes == 0 || headNodes == 0)
                    throw new Exception("Berstuuk model geometry fell back to a generic rig: body=" +
                        bodyNodes + " head=" + headNodes);
                Debug.Log(Prefix + "Berstuuk's archived body and mask geometry loaded: " +
                    bodyNodes + " body nodes, " + headNodes + " mask nodes.");
            }
            Debug.Log(Prefix + "Boss " + Target.Id + " ran 30 frames at " + location.name +
                " with " + sprites + " arena sprites and " + storyPresses + " story presses.");
            typeof(Fight).GetMethod("SurrenderButtonCallback", Hidden).Invoke(fight, new object[] { null });
            surrenderRequested = true;
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static FightDefinition Target => targets[targetIndex];

    static bool ObserveSaturnBlaster(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_boss_12_1" &&
            Target.Id.ToString() != "de128:fights/uw_boss_12_hardmode_1")
            throw new Exception("Saturn Blaster acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (saturnEnteredAt < 0)
        {
            saturnEnteredAt = frame;
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            var caster = localMoves.SingleOrDefault(move => move.Name == "SaturnBlasterAbilityPlayer");
            var keyNode = new XmlDocument();
            keyNode.LoadXml("<Keys><Key Type='RaidCharge' PressType='Tap'/></Keys>");
            var expected = new ConditionKeys(keyNode.DocumentElement);
            expected.Parse(keyNode.DocumentElement);
            if (caster == null || caster.Priority != 200 ||
                localMoves.Count(move => move.Name == "SaturnBlasterAbilityPlayer") != 1 ||
                localMoves.Count(move => move.Name == "SaturnBlasterAbilityPlayerHideWeapon" ||
                    move.Name == "SaturnBlasterAbilityStrikePlayer") != 2 ||
                !caster.SelectionConditions.OfType<ConditionKeys>().Single().HasSameKeyRequirementAs(expected))
                throw new Exception("Saturn's native caster lost its guarded RaidCharge/priority patch or linked player moves.");
            enemy.AddEventListener(6, value =>
            {
                var child = value as Model;
                if (child == null) return;
                string name = child.get_Name();
                var childMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(child);
                if (name == "SaturnBlaster")
                {
                    saturnPistol = true;
                    if (child.Parameters.Weapon?.Name != "ABILITY_SATURN_BLASTER" ||
                        childMoves.Count(move => move.Name == "SaturnBlasterModel" ||
                            move.Name == "SaturnBlasterModelHide" || move.Name == "SaturnBlasterModelStrike") != 3)
                        throw new Exception("Saturn's native pistol lost its hidden item or linked move phases.");
                    child.AddEventListener(5, ignored => saturnPistolDeleted = true);
                }
                else if (name == "SaturnBlasterBullet" || name == "SaturnBlasterBullet2")
                {
                    bool first = name == "SaturnBlasterBullet";
                    if (child.Parameters.Weapon?.Name != "MAGIC_PROJECTILE" ||
                        !childMoves.Any(move => move.Name == (first ? "SaturnProjectileStart1" : "SaturnProjectileStart2")))
                        throw new Exception("Saturn's native projectile lost its hidden item or flight move: " + name);
                    if (first) saturnBullet1 = true; else saturnBullet2 = true;
                    child.OCPMJKIEPIG().AddEventListener(2, action =>
                    {
                        if (action is IntervalAttack attack && attack.Start == 2 && attack.MOILKOLCNBP())
                        {
                            if (first) saturnBulletAttack1 = true; else saturnBulletAttack2 = true;
                        }
                    });
                }
            });
            Debug.Log(Prefix + "Saturn's native RaidCharge caster, pistol branches and two projectile moves loaded.");
        }
        string animation = enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name;
        bool casting = animation == "SaturnBlasterAbilityPlayer";
        if (casting && !saturnWasCasting)
        {
            if (saturnSelectedAt < 0)
            {
                saturnSelectedAt = frame;
                if (frame < 300) throw new Exception("Saturn cast before the archived 300-frame opening cooldown.");
                Debug.Log(Prefix + "Saturn selected Blaster at frame " + frame + ".");
            }
            else if (saturnSecondAt < 0)
            {
                int cooldown = Target.Id.ToString().Contains("hardmode") ? 550 : 660;
                if (frame - saturnSelectedAt < cooldown)
                    throw new Exception("Saturn recast before the archived " + cooldown + "-frame cooldown.");
                saturnSecondAt = frame;
                Debug.Log(Prefix + "Saturn recast Blaster at frame " + frame + ".");
            }
        }
        saturnWasCasting = casting;
        if (animation == "SaturnBlasterAbilityPlayerHideWeapon" ||
            animation == "SaturnBlasterAbilityStrikePlayer") saturnPistolPhases.Add(animation);
        if (GameObject.Find("SaturnShot1") != null) saturnEffect1 = true;
        if (GameObject.Find("SaturnShot2") != null) saturnEffect2 = true;
        if (!saturnCaptured && (saturnEffect1 || saturnEffect2))
        {
            CaptureCombatFrame(Target.Id.ToString() + "_blaster");
            saturnCaptured = true;
        }
        if (saturnSecondAt >= 0 && saturnPistol &&
            saturnBullet1 && saturnBullet2 && saturnBulletAttack1 && saturnBulletAttack2 &&
            saturnEffect1 && saturnEffect2 && saturnCaptured && saturnPistolPhases.Count > 0 &&
            saturnPistolDeleted)
        {
            Debug.Log(Prefix + "Saturn's cast, pistol branch, dual damaging projectiles, effects and cleanup completed in native combat.");
            return true;
        }
        if (frame - saturnEnteredAt >= 6000)
            throw new Exception("Saturn Blaster did not complete in native combat: cast=" + saturnSelectedAt +
                " recast=" + saturnSecondAt + " pistol=" + saturnPistol + " bullets=" + saturnBullet1 + "/" + saturnBullet2 +
                " attacks=" + saturnBulletAttack1 + "/" + saturnBulletAttack2 +
                " effects=" + saturnEffect1 + "/" + saturnEffect2 +
                " branch=" + string.Join(",", saturnPistolPhases) + " deleted=" + saturnPistolDeleted);
        return false;
    }

    static bool ObserveBlacknessGrasp(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_boss_13_1" &&
            Target.Id.ToString() != "de128:fights/uw_boss_13_hardmode_1")
            throw new Exception("Blackness Grasp acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (blacknessEnteredAt < 0)
        {
            blacknessEnteredAt = frame;
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            var caster = localMoves.SingleOrDefault(move => move.Name == "de128:moves/blackness_grasp_player");
            if (caster == null ||
                localMoves.Any(move => move.Name.StartsWith("BlacknessGraspAbility", StringComparison.Ordinal) &&
                    move.SelectionConditions.Last().GetType().Name != "DisabledMoveCondition"))
                throw new Exception("Blackness lacks the authored caster or retained a core selector: " +
                    string.Join(",", localMoves.Where(move => move.Name.Contains("Blackness") || move.Name.Contains("blackness"))
                        .Select(move => move.Name + "/" + move.SelectionConditions.Last().GetType().Name)));
            var transition = caster.ScheduledActions.OfType<ActionPlayAnimation>().SingleOrDefault();
            if (transition == null || transition.ChildName != "BlackHand" ||
                transition.AnimationName != "de128:moves/blackness_grasp_hand_attack" || !transition.NeedStart(17))
                throw new Exception("Blackness's frame-17 named-child transition did not reach the native parser.");
            enemy.AddEventListener(6, value =>
            {
                var child = value as Model;
                if (child?.get_Name() != "BlackHand") return;
                if (child.Parameters.Weapon?.Name != "MAGIC_ACID_CLOUD")
                    throw new Exception("BlackHand lost its hidden native item.");
                var childMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(child);
                if (childMoves.Count(move => move.Name == "de128:moves/blackness_grasp_hand_start" ||
                    move.Name == "de128:moves/blackness_grasp_hand_attack") != 2 ||
                    childMoves.Any(move => move.Name.StartsWith("BlacknessGraspAbilityHand", StringComparison.Ordinal) &&
                        move.SelectionConditions.Last().GetType().Name != "DisabledMoveCondition"))
                    throw new Exception("BlackHand lacks its authored start/attack or retained a core child selector: " +
                        string.Join(",", childMoves.Where(move => move.Name.Contains("Blackness") ||
                            move.Name.Contains("blackness")).Select(move => move.Name + "/" +
                            move.SelectionConditions.Last().GetType().Name)));
                blacknessHand = child;
                blacknessChild = true;
                child.OCPMJKIEPIG().AddEventListener(2, action =>
                {
                    if (action is IntervalAttack attack && attack.Start == 7 &&
                        attack.HitReactions.SingleOrDefault()?.Name == "Physycal" && attack.MOILKOLCNBP())
                        blacknessAttack = true;
                });
                child.AddEventListener(5, ignored => blacknessDeleted = true);
                Debug.Log(Prefix + "Blackness spawned the named BlackHand actor.");
            });
            Debug.Log(Prefix + "Blackness's three authored Grasp moves and timed child action loaded.");
        }
        if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/blackness_grasp_player" &&
            blacknessSelectedAt < 0)
        {
            blacknessSelectedAt = frame;
            if (frame < 600) throw new Exception("Blackness cast before the archived 600-frame opening cooldown.");
            Debug.Log(Prefix + "Blackness selected Grasp at frame " + frame + ".");
        }
        if (blacknessHand?.OCPMJKIEPIG()?.NNMAFFCCMHC()?.Name ==
            "de128:moves/blackness_grasp_hand_attack") blacknessTransition = true;
        if (GameObject.Find("BlackHandEFX") != null)
        {
            blacknessEffect = true;
            if (!blacknessCaptured && blacknessSelectedAt >= 0)
            {
                CaptureCombatFrame(Target.Id.ToString() + "_grasp");
                blacknessCaptured = true;
            }
        }
        if (frame - blacknessEnteredAt < 1800 ||
            (blacknessSelectedAt >= 0 && frame - blacknessSelectedAt < 180)) return false;
        if (blacknessSelectedAt < 0 || !blacknessChild || !blacknessTransition ||
            !blacknessAttack || !blacknessDeleted || !blacknessEffect || !blacknessCaptured)
            throw new Exception("Blackness Grasp did not complete in native combat: selected=" +
                blacknessSelectedAt + " child=" + blacknessChild + " transition=" + blacknessTransition +
                " attack=" + blacknessAttack + " deleted=" + blacknessDeleted + " effect=" + blacknessEffect +
                " captured=" + blacknessCaptured);
        Debug.Log(Prefix + "Blackness cast, effect, named hand transition and 0.4-damage attack completed in native combat.");
        return true;
    }

    static bool ObserveWarWhirl(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_boss_9_1" &&
            Target.Id.ToString() != "de128:fights/uw_boss_9_hardmode_1")
            throw new Exception("War Whirl acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (warEnteredAt < 0)
        {
            warEnteredAt = frame;
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            var authored = localMoves.SingleOrDefault(move => move.Name == "de128:moves/war_whirl_player");
            if (authored == null || localMoves.Count(move => move.Name == "MagicWarAbilityPlayer" &&
                move.SelectionConditions.Last().GetType().Name == "DisabledMoveCondition") != 1)
                throw new Exception("War lacks her archived Whirl or retained the core selector.");
            if (authored.ScheduledActions.Count(action => action is ActionStopSound stop &&
                stop.get_Name() == "snd_blade_fury") != 2)
                throw new Exception("War's native Whirl lost its two sound cleanup events.");
            enemy.OCPMJKIEPIG().AddEventListener(2, value =>
            {
                if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != "de128:moves/war_whirl_player" ||
                    !(value is IntervalAttack attack)) return;
                if (attack.Start != 14 || attack.EndFrame != 70 ||
                    attack.HitReactions.SingleOrDefault()?.Name != "Physycal" || !attack.MOILKOLCNBP())
                    throw new Exception("War's live Whirl attack lost its archived window, hit or block bypass.");
                warAttack = true;
            });
            enemy.AddEventListener(7, value =>
            {
                if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != "de128:moves/war_whirl_player" ||
                    !(value is ActionEffect effect)) return;
                warEffects.Add(effect.get_Name() + ":" + effect.EPDMGFELIMC());
            });
            enemy.AddEventListener(8, value =>
            {
                if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/war_whirl_player" &&
                    value is ActionStopEffect effect && effect.get_Name() == "MagicWarWhirlffectMiddle")
                    warEffectStopped = true;
            });
            Debug.Log(Prefix + "War's authored Whirl, disabled core selector and native sound cleanup loaded.");
        }
        if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/war_whirl_player" &&
            warSelectedAt < 0)
        {
            warSelectedAt = frame;
            Debug.Log(Prefix + "War selected archived Whirl at frame " + frame + ".");
        }
        if (!warCaptured && warSelectedAt >= 0 && frame - warSelectedAt >= 50 &&
            frame - warSelectedAt <= 100 && GameObject.Find("MagicWarWhirlffectMiddle") != null)
        {
            CaptureCombatFrame(Target.Id.ToString() + "_war_whirl");
            warCaptured = true;
        }
        if (frame - warEnteredAt < 1200) return false;
        int decisionFrame = (int)typeof(ModelAi).GetField("_modDecisionFrame", Hidden)
            .GetValue(enemy.EEIGOJBKFGE());
        if (decisionFrame < 300 || warSelectedAt < 0 || !warAttack || !warEffectStopped || !warCaptured ||
            !warEffects.Contains("MagicWarWhirlffectStart:mgc_war_ability_start") ||
            !warEffects.Contains("MagicWarWhirlffectMiddle:mgc_war_ability_middle") ||
            !warEffects.Contains("MagicWarWhirlffectMiddle:mgc_war_ability_end"))
            throw new Exception("War's full Whirl sequence did not execute within 1200 frames: AI=" +
                decisionFrame + " selected=" + warSelectedAt + " attack=" + warAttack +
                " stop=" + warEffectStopped + " effects=" + string.Join(",", warEffects));
        Debug.Log(Prefix + "War Whirl attacked and ran all three native effects with cleanup.");
        return true;
    }

    static bool ObserveGatekeeperField(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_boss_11_1" &&
            Target.Id.ToString() != "de128:fights/uw_boss_11_hardmode_1")
            throw new Exception("Gatekeeper field acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (gatekeeperEnteredAt < 0)
        {
            gatekeeperEnteredAt = frame;
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            var cast = localMoves.SingleOrDefault(move => move.Name == "de128:moves/gatekeeper_power_field");
            if (cast == null || localMoves.Count(move => move.Name == "GateKeeperPowerField" &&
                move.SelectionConditions.Last().GetType().Name == "DisabledMoveCondition") != 1)
                throw new Exception("Gatekeeper lacks the authored cast or retained the core selector.");
            var electro = cast.ScheduledActions.OfType<ActionEffect>().Single();
            if (electro.Attachment == null || !electro.DIGCODDLDAD())
                throw new Exception("Gatekeeper's effect lost its native node attachment.");
            enemy.AddEventListener(7, value =>
            {
                if (value is ActionEffect effect && effect.get_Name() == "ElectroEffect" &&
                    enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == cast.Name)
                    gatekeeperElectro = true;
            });
            Debug.Log(Prefix + "Gatekeeper's authored cast and node attachment loaded.");
        }
        if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/gatekeeper_power_field" &&
            gatekeeperSelectedAt < 0)
        {
            gatekeeperSelectedAt = frame;
            Debug.Log(Prefix + "Gatekeeper selected Power Field at frame " + frame + ".");
        }
        Model child = enemy.NMGNPBMFJKP(ModelType.KEIDBIOIFGA.MODEL_CHILD);
        if (child != null && child.get_Name() == "AbilityPowerField")
        {
            if (!gatekeeperChild)
            {
                if (child.Parameters.Weapon?.Name != "MAGIC_FIRE_AURA")
                    throw new Exception("Gatekeeper's power field lost its hidden native item.");
                var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(child);
                var surge = localMoves.SingleOrDefault(move => move.Name == "de128:moves/gatekeeper_power_surge");
                if (surge == null || surge.ScheduledActions.OfType<ActionEffect>().Count(effect => effect.Attachment != null) != 2)
                    throw new Exception("Gatekeeper's spawned field lost its two parent-attached effects.");
                child.OCPMJKIEPIG().AddEventListener(2, value =>
                {
                    if (value is IntervalAttack attack && attack.Start == 2 && attack.EndFrame == 5 &&
                        attack.HitReactions.SingleOrDefault()?.Name == "ElectrocutionPowerfield" && attack.MOILKOLCNBP())
                        gatekeeperAttack = true;
                });
                gatekeeperChild = true;
                Debug.Log(Prefix + "Gatekeeper spawned the hidden-item Power Field actor.");
            }
        }
        var electroObject = GameObject.Find("ElectroEffect");
        if (electroObject != null)
        {
            var cast = ((List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy))
                .Single(move => move.Name == "de128:moves/gatekeeper_power_field");
            Vector3 target;
            Quaternion orientation;
            if (!cast.ScheduledActions.OfType<ActionEffect>().Single().Attachment.TryGetTransform(enemy, out target, out orientation) ||
                Vector3.Distance(target, electroObject.transform.localPosition) > 80f)
                throw new Exception("Gatekeeper ElectroEffect drifted from its live model anchor: target=" +
                    target + " actual=" + electroObject.transform.localPosition + " world=" +
                    electroObject.transform.position + " parent=" + electroObject.transform.parent?.position);
            gatekeeperElectroAnchored = true;
        }
        var powerObject = GameObject.Find("PowerFieldEffect");
        var powerObject2 = GameObject.Find("PowerFieldEffect2");
        if (powerObject != null && powerObject2 != null)
        {
            gatekeeperPowerEffects = true;
            if (child == null) throw new Exception("Gatekeeper's parent-attached effect lost its child actor.");
            var surge = ((List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(child))
                .Single(move => move.Name == "de128:moves/gatekeeper_power_surge");
            var attachment = surge.ScheduledActions.OfType<ActionEffect>().First().Attachment;
            Vector3 target;
            Quaternion orientation;
            if (!attachment.TryGetTransform(child, out target, out orientation) ||
                Vector3.Distance(target, powerObject.transform.localPosition) > 80f ||
                Vector3.Distance(target, powerObject2.transform.localPosition) > 80f)
                throw new Exception("Gatekeeper's field effects drifted from their parent model anchor.");
            gatekeeperPowerAnchored = true;
            if (!gatekeeperPowerCaptured)
            {
                CaptureCombatFrame(Target.Id.ToString() + "_power_surge");
                gatekeeperPowerCaptured = true;
            }
        }
        if (!gatekeeperCaptured && gatekeeperSelectedAt >= 0 && frame - gatekeeperSelectedAt >= 22 &&
            frame - gatekeeperSelectedAt <= 80 && GameObject.Find("ElectroEffect") != null)
        {
            CaptureCombatFrame(Target.Id.ToString() + "_power_field");
            gatekeeperCaptured = true;
        }
        // Verify one complete cast. A later recast can leave same-named effect
        // objects alive briefly while the old sequence finishes.
        if (gatekeeperSelectedAt < 0 || frame - gatekeeperSelectedAt < 180) return false;
        if (gatekeeperSelectedAt < 0 || !gatekeeperChild || !gatekeeperAttack || !gatekeeperElectro ||
            !gatekeeperPowerEffects || !gatekeeperCaptured || !gatekeeperElectroAnchored ||
            !gatekeeperPowerAnchored || !gatekeeperPowerCaptured)
            throw new Exception("Gatekeeper Power Field did not complete its first cast: selected=" +
                gatekeeperSelectedAt + " child=" + gatekeeperChild + " attack=" + gatekeeperAttack +
                " electro=" + gatekeeperElectro + " powerEffects=" + gatekeeperPowerEffects +
                " capture=" + gatekeeperCaptured + " anchors=" + gatekeeperElectroAnchored +
                "," + gatekeeperPowerAnchored);
        Debug.Log(Prefix + "Gatekeeper cast, attached effects and spawned 0.3-damage field completed in native combat.");
        return true;
    }

    static bool ObserveHermitWave(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_survival_demon_1")
            throw new Exception("Hermit wave acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (enemy.Parameters.Weapon?.Name != "WEAPON_HERMIT_SWORDS")
        {
            if (hermitEnteredAt >= 0) throw new Exception("Hermit left his wave before Storm acceptance.");
            if (hermitWaveDefeats > 1) throw new Exception("Hermit did not enter after one survival defeat.");
            if (EditorApplication.timeSinceStartup - lastWaveDefeat < 0.7) return false;
            lastWaveDefeat = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent())
            {
                hermitWaveDefeats++;
                Debug.Log(Prefix + "Advanced Demon survival to Hermit: defeat " + hermitWaveDefeats + ".");
            }
            return false;
        }
        if (hermitEnteredAt < 0)
        {
            hermitEnteredAt = frame;
            if (hermitWaveDefeats != 1)
                throw new Exception("Hermit entered outside his second archived survival wave.");
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            foreach (string name in new[] { "player", "idle", "win" })
                if (localMoves.Count(move => move.Name == "de128:moves/hermit_storm_" + name) != 1)
                    throw new Exception("Hermit lacks the authored Storm " + name + " move.");
            foreach (string name in new[] { "HermitStormPlayer", "HermitStormPlayerIdle", "Win_HermitStorm" })
                if (localMoves.Any(move => move.Name == name &&
                    move.SelectionConditions.Last().GetType().Name != "DisabledMoveCondition"))
                    throw new Exception("Hermit retained the core " + name + " selector.");
            enemy.AddEventListener(6, value =>
            {
                var child = value as Model;
                if (child?.get_Name() != "HermitStorm") return;
                if (child.Parameters.Weapon?.Name != "HERMIT_STORM")
                    throw new Exception("Hermit Storm child did not equip the native hidden item: " +
                        child.Parameters.Weapon?.Name);
                hermitStormSpawns++;
                if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/hermit_storm_idle")
                    hermitIdleStormSpawns++;
                Debug.Log(Prefix + "Hermit Storm child spawn " + hermitStormSpawns + ".");
            });
            enemy.OCPMJKIEPIG().AddEventListener(2, value =>
            {
                if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != "de128:moves/hermit_storm_player" ||
                    !(value is IntervalAttack attack)) return;
                if (attack.Start != 8 && attack.Start != 11 && attack.Start != 20)
                    throw new Exception("Hermit Storm used an unauthored attack interval: " + attack.Start);
                hermitAttackStarts.Add(attack.Start);
            });
            Debug.Log(Prefix + "Reached Hermit after one native survival defeat; three DE Storm moves are installed.");
        }
        string active = enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name;
        if (hermitPlayerDefeatedAt >= 0)
        {
            if (active == "de128:moves/hermit_storm_win")
            {
                Debug.Log(Prefix + "PASS: Hermit selected his authored victory move after a native player defeat.");
                Finish(0);
                return false;
            }
            if (frame - hermitPlayerDefeatedAt > 240)
                throw new Exception("Hermit did not select his authored victory move after player defeat; active=" + active);
            return false;
        }
        if (active == "de128:moves/hermit_storm_player" && hermitSelectedAt < 0)
        {
            hermitSelectedAt = frame;
            Debug.Log(Prefix + "Hermit selected archived Storm at frame " + frame + ".");
        }
        if (active == "de128:moves/hermit_storm_idle" && hermitIdleAt < 0)
        {
            hermitIdleAt = frame;
            Debug.Log(Prefix + "Hermit entered archived Storm idle at frame " + frame + ".");
        }
        if (frame - hermitEnteredAt < 1200) return false;
        int decisionFrame = (int)typeof(ModelAi).GetField("_modDecisionFrame", Hidden)
            .GetValue(enemy.EEIGOJBKFGE());
        if (decisionFrame < 300 || hermitSelectedAt < 0 || hermitIdleAt < 0 ||
            hermitStormSpawns < 3 || hermitIdleStormSpawns < 2 ||
            !hermitAttackStarts.SetEquals(new[] { 8, 11, 20 }))
            throw new Exception("Hermit's Storm sequence did not execute within 1200 frames: AI=" +
                decisionFrame + " selected=" + hermitSelectedAt + " idle=" + hermitIdleAt +
                " spawns=" + hermitStormSpawns + " idleSpawns=" + hermitIdleStormSpawns +
                " attacks=" + string.Join(",", hermitAttackStarts));
        Debug.Log(Prefix + "Hermit Storm caster, idle, attack intervals and two hidden-item spawns completed in native combat.");
        if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_HERMIT_VICTORY") == "1")
        {
            bool defeated = (bool)typeof(Fight).GetMethod("KillModel", Hidden)
                .Invoke(fight, new object[] { true, false });
            if (!defeated) return false;
            hermitPlayerDefeatedAt = frame;
            Debug.Log(Prefix + "Defeated the native player to test Hermit's victory transition.");
            return false;
        }
        return true;
    }

    static bool ObserveButcherWave(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_survival_demon_1")
            throw new Exception("Butcher wave acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (enemy.Parameters.Weapon?.Name != "WEAPON_BUTCHER_KNIVES")
        {
            if (butcherEnteredAt >= 0) throw new Exception("Butcher left his wave before Earthquake acceptance.");
            if (butcherWaveDefeats > 3) throw new Exception("Butcher did not enter after two survival defeats.");
            if (EditorApplication.timeSinceStartup - lastWaveDefeat < 0.7) return false;
            lastWaveDefeat = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent())
            {
                butcherWaveDefeats++;
                Debug.Log(Prefix + "Advanced Demon survival to Butcher: defeat " + butcherWaveDefeats + ".");
            }
            return false;
        }
        if (butcherEnteredAt < 0)
        {
            butcherEnteredAt = frame;
            if (butcherWaveDefeats != 2)
                throw new Exception("Butcher entered outside his third archived survival wave.");
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            if (localMoves.Count(move => move.Name == "de128:moves/butcher_earthquake_player") != 1 ||
                localMoves.Count(move => move.Name == "ButcherEarthquakePlayer" &&
                    move.SelectionConditions.Last().GetType().Name == "DisabledMoveCondition") != 1)
                throw new Exception("Butcher lacks the selectable DE Earthquake or retained the core selector.");
            enemy.AddEventListener(6, value =>
            {
                var child = value as Model;
                if (child?.get_Name() != "Earthquake") return;
                butcherChildSpawned = true;
                if (child.Parameters.Weapon?.Name != "MAGIC_BUTCHER_EARTHQUAKE")
                    throw new Exception("Butcher's projectile did not equip the hidden native earthquake item.");
                child.OCPMJKIEPIG().AddEventListener(0, animation =>
                {
                    if ((animation as InfoAnimation)?.Name == "de128:moves/butcher_earthquake_start")
                        butcherChildMove = true;
                });
                child.OCPMJKIEPIG().AddEventListener(2, interval =>
                {
                    if (child.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != "de128:moves/butcher_earthquake_start" ||
                        !(interval is IntervalAttack attack)) return;
                    if (attack.Start != 2 || attack.HitReactions.SingleOrDefault()?.Name != "Earthquake" ||
                        !attack.MOILKOLCNBP())
                        throw new Exception("Butcher's live projectile attack lost its archived Earthquake reaction or block bypass.");
                    butcherChildAttack = true;
                });
                child.AddEventListener(5, ignored => butcherChildDeleted = true);
                Debug.Log(Prefix + "Butcher spawned Earthquake with its hidden native item.");
            });
            Debug.Log(Prefix + "Reached Butcher after two native survival defeats; DE Earthquake is selectable.");
        }
        if (enemy.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/butcher_earthquake_player" &&
            butcherSelectedAt < 0)
        {
            butcherSelectedAt = frame;
            Debug.Log(Prefix + "Butcher selected archived Earthquake at frame " + frame + ".");
        }
        if (frame - butcherEnteredAt < 1200) return false;
        int decisionFrame = (int)typeof(ModelAi).GetField("_modDecisionFrame", Hidden)
            .GetValue(enemy.EEIGOJBKFGE());
        if (decisionFrame < 300)
            throw new Exception("Butcher's Lua AI clock did not advance in live combat: " + decisionFrame);
        if (butcherSelectedAt < 0 || !butcherChildSpawned || !butcherChildMove || !butcherChildAttack || !butcherChildDeleted)
            throw new Exception("Butcher's complete Earthquake sequence did not execute within 1200 frames: selected=" +
                butcherSelectedAt + " spawned=" + butcherChildSpawned + " move=" + butcherChildMove +
                " attack=" + butcherChildAttack +
                " deleted=" + butcherChildDeleted);
        Debug.Log(Prefix + "Butcher Earthquake cast, child attack and deletion completed in native combat.");
        return true;
    }

    static bool ObserveMercenaryWave(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_survival_mercenary_1")
            throw new Exception("Mercenary wave acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (enemy.Parameters.Weapon?.Name != "WEAPON_SUPER_FANS")
        {
            if (mercenaryEnteredAt >= 0)
                throw new Exception("Girl Fan left her wave before equipment acceptance.");
            if (mercenaryWaveDefeats > 23)
                throw new Exception("Girl Fan did not enter after 22 survival defeats.");
            if (EditorApplication.timeSinceStartup - lastWaveDefeat < 0.7) return false;
            lastWaveDefeat = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent())
            {
                mercenaryWaveDefeats++;
                Debug.Log(Prefix + "Advanced Mercenary survival: defeat " + mercenaryWaveDefeats +
                    ", current weapon=" + enemy.Parameters.Weapon?.Name + ".");
            }
            return false;
        }
        if (mercenaryEnteredAt < 0)
        {
            mercenaryEnteredAt = frame;
            if (mercenaryWaveDefeats != 22 ||
                enemy.Parameters.Armor?.Name != "ARMOR_IM_CEREMONIAL" ||
                enemy.Parameters.Helm?.Name != "HELM_IM_CEREMONIAL")
                throw new Exception("Girl Fan's native survival slot or ceremonial equipment differs: " +
                    mercenaryWaveDefeats + " defeats, armor=" + enemy.Parameters.Armor?.Name +
                    ", helm=" + enemy.Parameters.Helm?.Name);
            Debug.Log(Prefix + "Reached Girl Fan after 22 native survival defeats; ceremonial armor and helm equipped.");
        }
        if (frame - mercenaryEnteredAt < 90) return false;
        var macros = enemy.CLDMEJKGLBA()?.BLFJJAEFKKP();
        if (macros == null || macros.Count < 10)
            throw new Exception("Girl Fan's native fighter model did not render through 90 combat frames.");
        CaptureCombatFrame(Target.Id.ToString() + "_girl_fan");
        Debug.Log(Prefix + "Girl Fan fought 90 frames with " + macros.Count + " native model nodes.");
        return true;
    }

    static bool ObserveWaspWave(Fight fight, Model enemy)
    {
        if (Target.Id.ToString() != "de128:fights/uw_survival_demon_1")
            throw new Exception("Wasp wave acceptance selected the wrong fight.");
        int frame = fight.get_FightTimeInFrames();
        if (enemy.Parameters.Weapon?.Name != "WEAPON_WASP_NAGINATA")
        {
            if (waspEnteredAt >= 0) throw new Exception("Wasp left her wave before ability acceptance.");
            if (waspWaveDefeats > 12) throw new Exception("Wasp did not enter after twelve survival defeats.");
            if (EditorApplication.timeSinceStartup - lastWaveDefeat < 0.7) return false;
            lastWaveDefeat = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent())
            {
                waspWaveDefeats++;
                Debug.Log(Prefix + "Advanced Demon survival to Wasp: defeat " + waspWaveDefeats +
                    ", current weapon=" + enemy.Parameters.Weapon?.Name + ".");
            }
            return false;
        }
        if (waspEnteredAt < 0)
        {
            waspEnteredAt = frame;
            var localMoves = (List<InfoAnimation>)typeof(Model).GetField("OHAMEHHMEAL", Hidden).GetValue(enemy);
            var registered = AnimationData.Animations.Where(move => move.Name.StartsWith("de128:moves/wasp_fly_", StringComparison.Ordinal))
                .Select(move => move.Name).ToArray();
            var loaded = localMoves.Where(move => move.Name.StartsWith("de128:moves/wasp_fly_", StringComparison.Ordinal))
                .Select(move => move.Name).ToArray();
            if (registered.Length != 4 || loaded.Length != 4 ||
                localMoves.Count(move => move.Name.StartsWith("WaspFly_", StringComparison.Ordinal) &&
                    !move.Name.EndsWith("_PVP", StringComparison.Ordinal) &&
                    move.SelectionConditions.Last().GetType().Name == "DisabledMoveCondition") != 4)
                throw new Exception("Wasp did not receive four selectable DE moves and four disabled core versions.");
            var perks = enemy.Parameters.JBIOECDAAKP();
            var items = enemy.Parameters.PJNJIJIODHE();
            var locks = new ModelConditions { OJIAKDDCGLB = items, POBNMMADAJJ = perks,
                IBBALIJOJMC = enemy.Parameters.IBBALIJOJMC };
            if (localMoves.Where(move => move.Name.StartsWith("de128:moves/wasp_fly_", StringComparison.Ordinal))
                .Any(move => !move.HPPGNJJCEGF(locks, move.MoveData.Locks)))
                throw new Exception("Wasp's DE Fly move failed its live perk or skeleton lock.");
            Debug.Log(Prefix + "Wasp Fly registered=" + string.Join(",", registered) +
                " fighter moves=" + string.Join(",", loaded) + "; native core versions disabled.");
            Debug.Log(Prefix + "Reached Wasp at frame " + frame + " after " + waspWaveDefeats +
                " native survival defeats.");
        }
        bool flying = enemy.FHBLLPCEAHG()?.CNPFHBMGDFP("WaspFly") == true;
        if (flying && waspFirstFlyAt < 0)
        {
            waspFirstFlyAt = frame;
            var active = enemy.OCPMJKIEPIG().NNMAFFCCMHC();
            var attack = active?.MoveData.Intervals.OfType<IntervalAttack>().SingleOrDefault();
            if (attack == null || !attack.MOILKOLCNBP() || !attack.NPHDDMAIGKN() ||
                attack.DNPLIFOABPB()?.Count != 0 || attack.KBENFIOADCG()?.Count != 0 ||
                attack.HitReactions.SingleOrDefault()?.Name != "WaspFly")
                throw new Exception("Wasp's live Fly attack lost its archived hit or bypass behavior.");
            Debug.Log(Prefix + "Wasp selected Fly at frame " + frame + ".");
        }
        if (flying) waspLastFlyAt = frame;
        if (frame - waspEnteredAt < 720) return false;
        if (waspFirstFlyAt < 0)
            throw new Exception("Wasp did not use her archived Fly ability within 720 native combat frames.");
        if (waspLastFlyAt - waspFirstFlyAt < 20)
            throw new Exception("Wasp's Fly did not play through its attack interval.");
        Debug.Log(Prefix + "Wasp Fly remained active through frame " + waspLastFlyAt + ".");
        return true;
    }

    static bool IsSpotlightFight(string id) => id == "de128:fights/uw_boss_13_hardmode_1" ||
        id == "de128:fights/uw_boss_son_of_the_sun_hardmode_1";

    static void ValidateRecoveredAliasEquipment(FightList fight, Model enemy, string id)
    {
        if (id == "de128:fights/uw_boss_halloween_puppeteer_1" ||
            id == "de128:fights/uw_boss_halloween_puppeteer_hardmode_1")
        {
            if (enemy.Parameters.Ranged?.Name != "RANGED_NEEDLES")
                throw new Exception("Puppeteer's archived needle identity was not equipped: " + id +
                    " actual=" + enemy.Parameters.Ranged?.Name);
            Debug.Log(Prefix + "Puppeteer's archived needles equipped in " + id + ".");
        }
        if (id == "de128:fights/uw_survival_mercenary_1")
        {
            var girl = fight.OFKJMHPMCCD().Where(warrior =>
                warrior.Armor?.Name == "ARMOR_IM_CEREMONIAL").ToArray();
            if (girl.Length != 1 || girl[0].Helm?.Name != "HELM_IM_CEREMONIAL")
                throw new Exception("Girl Fan's ceremonial armor and helm did not resolve in the native survival roster: " +
                    string.Join(",", fight.OFKJMHPMCCD().Select(warrior => warrior.Armor?.Name + "/" + warrior.Helm?.Name)));
            Debug.Log(Prefix + "Girl Fan's archived ceremonial armor and helm resolved in the native survival roster.");
        }
    }

    static void CaptureCombatFrame(string id)
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null) throw new Exception("No main camera for spotlight frame capture.");
        var target = new RenderTexture(512, 288, 24);
        var image = new Texture2D(512, 288, TextureFormat.RGBA32, false);
        var oldTarget = camera.targetTexture;
        var oldActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, 512, 288), 0, 0);
            image.Apply();
            string root = Directory.GetParent(Application.dataPath).FullName;
            string path = Path.Combine(root, "spotlight-" + id.Substring(id.LastIndexOf('/') + 1) + ".png");
            File.WriteAllBytes(path, image.EncodeToPNG());
            Debug.Log(Prefix + "Captured combat frame: " + path);
        }
        finally
        {
            camera.targetTexture = oldTarget;
            RenderTexture.active = oldActive;
            UnityEngine.Object.DestroyImmediate(image);
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    static void ValidateSpotlight(Model player, string id)
    {
        var mask = GameObject.Find("LightInTheDarkness")?.GetComponent<SpriteRenderer>();
        if (mask == null || !mask.enabled || mask.sprite == null || mask.sharedMaterial == null ||
            mask.sharedMaterial.shader.name != "Eclipse/Fight/Light In The Darkness")
            throw new Exception("Native spotlight material did not load: " + id);
        var material = mask.sharedMaterial;
        var center = material.GetVector("_Center");
        var logical = player.PLBNCDCFPML();
        var fighterWorld = player.MJNPBMOAFML().transform.parent.TransformPoint(
            new Vector3(logical.GetX(), logical.GetY(), 0f));
        var onMask = mask.transform.InverseTransformPoint(fighterWorld);
        if (Mathf.Abs(material.GetFloat("_Radius") - 0.2f) > 0.0001f ||
            Mathf.Abs(material.GetFloat("_Shape") - 1f) > 0.0001f ||
            center.x < 0f || center.x > 1f || center.y < 0f || center.y > 1f ||
            Mathf.Abs(center.x - 0.5f - onMask.x) > 0.02f ||
            Mathf.Abs(center.y - 0.5f - onMask.y) > 0.02f ||
            Mathf.Abs(mask.transform.localScale.x - 2048f) > 0.01f)
            throw new Exception("Native spotlight radius, shape, center or mask scale differs: " +
                id + " center=" + center);
        Debug.Log(Prefix + "Spotlight " + id + " material loaded and followed the live fighter at " + center + ".");
    }

    static void ValidateOwnerRaidArt()
    {
        if (ownerArtValidated) return;
        string[] avatars = {
            "boss_architect_hummer_new", "boss_arkhos_hardmode_new", "boss_bison_hard_new",
            "boss_crystal_hardmode_new", "boss_fatum_hardmode_new", "boss_fire_hardmode_new",
            "boss_hoaxen_hardmode_new", "boss_hunger_hardmode_new", "boss_lamb_fungus_hard_new",
            "boss_lamb_hard_new", "boss_lamb_hunger_hard_new", "boss_mushroom_hardmode_new",
            "boss_rakshasa_hardmode_new", "boss_ravana_hard_new", "boss_saturn_hard_new",
            "boss_tenebris_hardmode_new", "boss_vortex_hardmode_new", "boss_war_hardmode_new",
            "boss_whisper_hardmode_new", "new_man_shuang_gou_hardmode_new"
        };
        var loader = ModRuntime.Host.TypedAssets;
        foreach (string name in avatars)
        {
            var sprite = loader.LoadSprite(AssetId.Parse("de128:sprites/underworld/" + name));
            if (sprite == null || sprite.texture == null || sprite.vertices.Length < 3 ||
                sprite.rect.width < 600 || sprite.rect.height < 700 ||
                Math.Abs(sprite.pixelsPerUnit - 200) > 0.01f)
                throw new Exception("Owner raid avatar did not decode at source scale: " + name);
        }
        foreach (string state in new[] { "base", "active" })
        {
            var sprite = loader.LoadSprite(AssetId.Parse("de128:sprites/underworld/battlebtnprince_" + state));
            if (sprite == null || sprite.texture == null || sprite.vertices.Length < 3 ||
                sprite.rect.width != 300 || sprite.rect.height != 300)
                throw new Exception("Prince map button did not decode: " + state);
        }
        ownerArtValidated = true;
        Debug.Log(Prefix + "Decoded 20 owner raid avatars and both Prince map button sprites.");
    }

    static List<FightDefinition> SelectTargets(ModContentCatalog content)
    {
        string requested = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_TARGETS");
        if (!string.IsNullOrEmpty(requested))
        {
            var ids = requested.Split(',');
            string matrix = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_MATRIX");
            bool countValid = matrix == "single" ? ids.Length == 1 :
                matrix == "subset" ? ids.Length >= 1 && ids.Length <= 76 : ids.Length == 32;
            if (!countValid || ids.Distinct(StringComparer.Ordinal).Count() != ids.Length)
                throw new Exception("The requested encounter matrix has an invalid fight list.");
            var storyFights = new List<FightDefinition>();
            foreach (string id in ids)
            {
                var fight = content.Fights.FirstOrDefault(value => value.Id.ToString() == id);
                if (fight == null) throw new Exception("Archived boss story has no live fight: " + id);
                storyFights.Add(fight);
                Debug.Log(Prefix + "Selected story boss " + fight.Id);
            }
            return storyFights;
        }
        var zones = content.Zones.Where(value => value.Underworld && value.Id.Namespace.Value == "de128")
            .OrderBy(value => value.LegacyName, StringComparer.Ordinal).ToArray();
        if (zones.Length != 8) throw new Exception("Expected eight registered Underworld zones.");
        var result = new List<FightDefinition>();
        if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_MATRIX") == "all")
        {
            foreach (var zone in zones)
                foreach (var battleId in zone.Battles)
                {
                    if (!content.TryGetBattle(battleId, out var battle))
                        throw new Exception("Underworld zone has no live battle: " + battleId);
                    foreach (var fightId in battle.Fights)
                    {
                        if (!content.TryGetFight(fightId, out var fight))
                            throw new Exception("Underworld battle has no live fight: " + fightId);
                        result.Add(fight);
                        Debug.Log(Prefix + "Selected " + zone.Id + " -> " + fight.Id);
                    }
                }
            if (result.Count != 76 || result.Select(value => value.Id).Distinct().Count() != 76)
                throw new Exception("Expected 76 distinct native Underworld fights, got " + result.Count);
            return result;
        }
        foreach (var zone in zones)
        {
            BattleDefinition boss = null;
            foreach (var id in zone.Battles)
            {
                if (!content.TryGetBattle(id, out var candidate) || candidate.Kind != ModBattleKind.Final ||
                    candidate.PowerMode == ModPowerMode.Power || candidate.Fights.Count == 0) continue;
                boss = candidate;
                break;
            }
            if (boss == null || !content.TryGetFight(boss.Fights[0], out var fight))
                throw new Exception("Underworld tier has no normal-mode boss fight: " + zone.Id);
            result.Add(fight);
            Debug.Log(Prefix + "Selected " + zone.Id + " -> " + fight.Id);
        }
        return result;
    }

    static void PressBlockingStory()
    {
        if (EditorApplication.timeSinceStartup - lastPress < 0.2) return;
        var active = typeof(DialogsManager).GetField("OALIPPPOHCL", HiddenStatic).GetValue(null) as StoryDialog;
        if (active == null || !active.IsQuestDialog || active.get_ButtonOK() == null) return;
        lastPress = EditorApplication.timeSinceStartup;
        PressStoryButton(active);
    }

    static void PressStoryButton(StoryDialog dialog)
    {
        typeof(StoryDialog).GetMethod("GPEKKGLDKDF", Hidden).Invoke(dialog, new object[] { null });
    }

    static ModStoryEvents StoryBus => (ModStoryEvents)typeof(ModRuntime)
        .GetField("StoryEvents", HiddenStatic).GetValue(null);

    static void CaptureCombatException(string message, string stack, LogType type)
    {
        if (type != LogType.Exception || Fight.GetCurrentFight() == null || combatException != null ||
            string.IsNullOrEmpty(stack)) return;
        if (stack.Contains("Fight.RenderFight") || stack.Contains("FightScene.FixedUpdate") ||
            stack.Contains("Model.") || stack.Contains("ModelAi.") || stack.Contains("Location."))
            combatException = message + "\n" + stack;
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= CaptureCombatException;
        EditorApplication.Exit(code);
    }
}
