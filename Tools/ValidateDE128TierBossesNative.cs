using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            if (EditorApplication.timeSinceStartup - started > 180)
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
