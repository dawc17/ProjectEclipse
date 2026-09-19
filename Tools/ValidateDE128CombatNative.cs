using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ValidateDE128CombatNative
{
    const string Active = "Eclipse.DE128Native.Active";
    const string Move = "de128:moves/chinese_swords_super_slash";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static double started, reported;
    static bool campaign, entered, attached, requested, selected, released, finished;
    static bool mapLockChecked;
    static string failure;
    static int requestedAt, selectedAt, attacks, swishes;
    static Model actor;
    static Model sphere;
    static bool spellRequested, spellSelected, spellReleased, sphereMiddle, sphereDeleted, chargeConsumed;
    static int spellFrame, sphereCount;
    static bool mindHit, mindFollowup, mindFinalAttack;
    static bool mindWall;
    static readonly bool WallMiss = Environment.GetEnvironmentVariable("ECLIPSE_DE128_TEST_WALL_MISS") == "1";
    static int mindExpiries;
    static bool targetStepped, targetReleased;
    const string FlagName = "fixture.de128-combat:behaviors/animation_lifecycle:cast";
    static int flagExpiries;
    static bool flagSetChecked, flagClearChecked;
    static readonly HashSet<string> Lifecycle = new HashSet<string>();
    static readonly string Spell = Environment.GetEnvironmentVariable("ECLIPSE_DE128_TEST_SPELL") ?? "Sphere1";
    static string SpellPrefix => "de128:moves/" + (Spell == "MindThrowNormal" ? "mind_throw" : Spell == "ComboSphere3" ? "combo_sphere3" : Spell.ToLowerInvariant());
    static string SpellMove => SpellPrefix + "_player";
    static string SpellMiddle => SpellPrefix + (Spell == "ComboSphere3" ? "_start" : "_middle");
    static readonly HashSet<int> AttackFrames = new HashSet<int>();
    static readonly HashSet<int> SoundFrames = new HashSet<int>();

    static ValidateDE128CombatNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        Application.logMessageReceived += Capture;
    }
    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-native-fixture.marker"))) throw new Exception("Requires isolated DE128 fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }
    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (failure != null) throw new Exception(failure);
            if (EditorApplication.timeSinceStartup - started > 300) throw new Exception("Timeout: campaign=" + campaign + " entered=" + entered + " attached=" + attached);
            if (EditorApplication.timeSinceStartup - reported > 15)
            {
                reported = EditorApplication.timeSinceStartup;
                Debug.Log("[DE128Native] Waiting campaign=" + campaign + " entered=" + entered + " selected=" + selected + " actions=" + swishes);
                if (!mapLockChecked && ModRuntime.Scripts != null)
                {
                    var flags = BindingFlags.Static | BindingFlags.NonPublic;
                    var blocker = typeof(Eclipse.UI.Modding.ModUiGameBridge).GetProperty("NativeInputBlocked", flags);
                    var lockScreen = Nekki.SF2.GUI.LockScreen.get_Instance();
                    Debug.Log("[DE128Native] Map gates: scene=" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name +
                        " module=" + Module.GetInstance()?.NMCNDOPKFJD() +
                        " input=" + blocker?.GetValue(null) + " lock=" + (lockScreen != null && lockScreen.gameObject.activeInHierarchy) +
                        " mutation=" + typeof(ModRuntime).GetField("_profileMutationState", flags).GetValue(null) +
                        " quest=" + Nekki.SF2.Core.Quests.QuestsManager.get_Instance()?.CurrentQuestName);
                }
            }
            if (!campaign && Eclipse.UI.TitleScreen.IsOpen)
            {
                var title = UnityEngine.Object.FindObjectOfType<Eclipse.UI.TitleScreen>();
                if (title != null) { typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign", Hidden).Invoke(title, null); campaign = true; }
                return;
            }
            if (!entered)
            {
                if (ModRuntime.Scripts == null || ListSF.CCDKHLAMKKO() == null || Module.GetInstance() == null) return;
                var screen = Module.GetInstance().NMCNDOPKFJD();
                if (screen != ScreenType.ModuleDojo && screen != ScreenType.ModuleMap) return;
                if (!mapLockChecked)
                {
                    if (screen != ScreenType.ModuleMap)
                    {
                        if (ModBattleAccess.SetLocked(DefinitionId.Parse("fixture.de128-combat:battles/lock_check"), false))
                            throw new Exception("Battle progression accepted outside map.");
                        if (ModBattleAccess.Reveal(DefinitionId.Parse("fixture.de128-combat:battles/reveal_check"), false) ||
                            ModBattleAccess.Focus(DefinitionId.Parse("fixture.de128-combat:battles/reveal_check")))
                            throw new Exception("Reveal/focus accepted outside map.");
                        // This fixture tests map progression, not campaign tab gates.
                        // The ordinary scene loader still initializes the actual map.
                        Module.DLOKJOHNDID(ScreenType.ModuleMap, null, null, false);
                        return;
                    }
                    if (!CheckMapBattleLock()) return;
                    CheckMapBattleReveal();
                    CheckEclipseModeSwitch();
                    mapLockChecked = true;
                }
                CheckProjectileActionParsing();
                CheckSharedMovePatches();
                CheckRestoredWeapons();
                CheckRestoredEquipment();
                CheckWarriorPerkLoadouts();
                CheckSenseiRewards();
                CheckProfileFightProgress();
                var definition = ModRuntime.Scripts.Content.Fights.FirstOrDefault(value => value.Id.ToString() == "fixture.de128-combat:fights/" + (Spell == "Sphere1" ? "jian" : Spell.ToLowerInvariant()));
                if (definition == null) throw new Exception("Fixture fight missing; check mod initialization errors.");
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(ModRuntime.Scripts.Content.RuntimeFightId(definition.Id)));
                entered = GameUtils.StartFight(encounter, false, null, true, false);
                Debug.Log("[DE128Native] StartFight=" + entered);
                if (!entered) throw new Exception("Fixture fight rejected by native availability: " + definition.Id);
                return;
            }
            var fight = Fight.GetCurrentFight(); if (fight == null) return;
            var enemy = (Model)typeof(Fight).GetField("CKNCPOABFBO", Hidden).GetValue(fight);
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (enemy == null || player == null || fight.get_FightTimeInFrames() < 1) return;
            int frame = fight.get_FightTimeInFrames();
            player.Parameters.set_IsImmortalityEnabled(true); enemy.Parameters.set_IsImmortalityEnabled(true);
            enemy.Parameters.AiControlled = false;
            if (!attached)
            {
                var weapon = enemy.Parameters.DGMDEDKLGMB().FirstOrDefault(item => item.Type == "Weapon");
                if (weapon == null || weapon.Name != "WEAPON_CHNY21_JIAN" || weapon.SubType != "ChineseSwords") throw new Exception("Jian runtime equipment/subtype incorrect: " + weapon?.Name + "/" + weapon?.SubType);
                actor = enemy; attached = true;
                fight.IEEGPNLEKHH().AddEventListener((int)PerkEvent.KNKIIEPDCPN.EVENT_MOD_EXPIRES, value => {
                    if ((value?.Data as string) == FlagName) flagExpiries++;
                    if ((value?.Data as string) == "de128:behaviors/mind_throw:pending") mindExpiries++;
                });
                player.OCPMJKIEPIG().AddEventListener(0, value => {
                    if ((value as InfoAnimation)?.Name == "de128:moves/mind_throw_hit") mindHit = true;
                });
                var animation = actor.OCPMJKIEPIG();
                animation.AddEventListener(0, OnAnimation);
                animation.AddEventListener(1, OnAnimationEnd);
                animation.AddEventListener(2, OnInterval);
                animation.AddEventListener(4, OnActions);
                Debug.Log("[DE128Native] Native Jian body ready.");
            }
            if (!requested && frame >= 120)
            {
                var move = AnimationData.Animations.Single(value => value.Name == Move);
                var keys = new KeyData(move.MOPMGFIIFGA().Single().FONEJOKEIEN);
                // KeyData at the controller boundary uses screen directions;
                // the move's Forward condition is relative to the fighter.
                keys.Reverse(actor.KFCNPADAMHA());
                actor.PlayAnimation(keys);
                requested = true; requestedAt = frame;
                Debug.Log("[DE128Native] Submitted native double-tap/Forward input at " + frame);
            }
            if (requested && !selected && frame > requestedAt + 30) throw new Exception("Native input did not select ChineseSwords; current=" + actor.OCPMJKIEPIG().NNMAFFCCMHC()?.Name);
            if (selected && !released)
            {
                // End the synthetic tap/hold sequence; this enemy has no physical
                // input source to deliver the corresponding release.
                actor.PlayAnimation(new KeyData()); released = true;
            }
            if (selected && frame > selectedAt + 180)
            {
                if (!finished || attacks != 4 || swishes != 4 || !AttackFrames.SetEquals(new[] { 11, 19, 27, 32 }) || !SoundFrames.SetEquals(new[] { 8, 17, 28, 33 }))
                    throw new Exception("Incomplete animation: ended=" + finished + " attacks=" + attacks + " swishes=" + swishes);
                if (actor.CLDMEJKGLBA() == null || actor.MJNPBMOAFML() == null || !actor.MJNPBMOAFML().activeInHierarchy) throw new Exception("Fighter lost active native rig.");
                CheckLiveSphere(frame, player);
            }
        }
        catch (Exception error) { Debug.LogError("[DE128Native] FAIL: " + error); Finish(1); }
    }
    static void CheckLiveSphere(int frame, Model target)
    {
        if (!spellRequested)
        {
            if (actor.Parameters.Magic?.SubType != Spell) throw new Exception("Fixture " + Spell + " equipment missing.");
            // The preceding Jian move closes the gap. Restore long-range spacing
            // so the projectile can finish its damaging startup before contact.
            actor.ShiftModelPosition(new Vector3f(400, 0, 0), true);
            target.ShiftModelPosition(new Vector3f(-200, 0, 0), true);
            actor.JJDNDOLCMMN = 1;
            actor.AddEventListener(6, OnSphereCreated);
            var cast = AnimationData.Animations.Single(value => value.Name == SpellMove);
            var keys = new KeyData(cast.MOPMGFIIFGA().Single().FONEJOKEIEN);
            keys.Reverse(actor.KFCNPADAMHA());
            spellRequested = true; spellFrame = frame; actor.PlayAnimation(keys);
            Debug.Log("[DE128Native] Requested " + Spell + " through native Magic input at " + frame);
        }
        if (!spellSelected && frame > spellFrame + 30) throw new Exception(Spell + " native input selection failed; current=" + actor.OCPMJKIEPIG().NNMAFFCCMHC()?.Name);
        if (spellSelected && !spellReleased) { actor.PlayAnimation(new KeyData()); spellReleased = true; }
        if (spellSelected && actor.JJDNDOLCMMN == 0) chargeConsumed = true;
        // Leave the initial crouched fists stance through normal movement input.
        if (Spell == "MindThrowNormal" && !WallMiss && !targetStepped && frame >= spellFrame + 1) {
            target.PressAnyKey(FightCID.QuadrantForward); targetStepped = true;
        }
        if (targetStepped && !targetReleased && frame >= spellFrame + 10) {
            target.ReleaseAnyKey(FightCID.QuadrantForward); targetReleased = true;
        }
        if (sphere != null && sphere.OCPMJKIEPIG()?.NNMAFFCCMHC()?.Name == SpellMiddle) sphereMiddle = true;
        if (frame > spellFrame + 360 && !sphereDeleted)
            throw new Exception(Spell + " cleanup failed; count=" + sphereCount + " child=" + sphere?.OCPMJKIEPIG()?.NNMAFFCCMHC()?.Name);
        if (sphereDeleted && frame > spellFrame + 240)
        {
            if (sphereCount != 1 || (!sphereMiddle && Spell != "MindThrowNormal") || !chargeConsumed || actor.KGGIDBLBMDJ().Contains(sphere as WeaponModel))
                throw new Exception("Incomplete live Sphere1: count=" + sphereCount + " middle=" + sphereMiddle + " consumed=" + chargeConsumed);
            Debug.Log("[DE128Native] PASS: prior Jian acceptance plus " + Spell + " native Magic-input selection, one inherited-equipment projectile, attack-phase selection, charge consumption and child deletion. No numerical damage, audible-output or shop-preview claim.");
            foreach (var phase in new[] { "AnimationStart", "AnimationEnd" })
            {
                // Successful MindThrow contact interrupts the initial cast;
                // the native callback contract reports natural ends only.
                if (Spell == "MindThrowNormal" && !WallMiss && phase == "AnimationEnd") continue;
                if (!Lifecycle.Contains(phase + "|player|opponent|" + SpellMove) ||
                    !Lifecycle.Contains(phase + "|opponent|self|" + SpellMove))
                    throw new Exception("Missing Lua caster lifecycle callback: " + phase);
            }
            if (Spell != "MindThrowNormal" && (!Lifecycle.Contains("AnimationStart|player|other|" + SpellMiddle) ||
                !Lifecycle.Contains("AnimationStart|opponent|other|" + SpellMiddle)))
                throw new Exception("Missing Lua projectile lifecycle callback.");
            if (!flagSetChecked || !flagClearChecked || flagExpiries != 1)
                throw new Exception("Lua/native flag handoff incomplete: set=" + flagSetChecked + " clear=" + flagClearChecked + " expiries=" + flagExpiries);
            Debug.Log("[DE128Native] PASS: real Lua caster lifecycle perspectives and native ModExists/ModExpires flag handoff.");
            if (Spell == "MindThrowNormal") {
                bool route = WallMiss ? mindWall && !mindHit && !mindFollowup && !mindFinalAttack : mindHit && mindFollowup && mindFinalAttack && !mindWall;
                if (!route || mindExpiries != 1)
                    throw new Exception("MindThrow handoff incomplete: hit="+mindHit+" followup="+mindFollowup+" final="+mindFinalAttack+" expiry="+mindExpiries);
                Debug.Log(WallMiss ? "[DE128Native] PASS: MindThrow wall deletion clears its innate Lua flag exactly once without a caster follow-up."
                    : "[DE128Native] PASS: MindThrow owned victim reaction, innate Lua flag expiry, caster follow-up selection and final direct attack interval.");
            }
            Finish(0);
        }
    }
    static void OnSphereCreated(object value)
    {
        var child = value as Model;
        if (child?.get_Name() != Spell) return;
        sphere = child; sphereCount++;
        child.OCPMJKIEPIG().AddEventListener(0, animation => {
            var name = (animation as InfoAnimation)?.Name;
            if (name == "de128:moves/mind_throw_wall") mindWall = true;
            if (name == SpellMiddle) sphereMiddle = true;
            Debug.Log("[DE128Native] " + Spell + " selected " + name);
        });
        if (child.Parameters.Weapon?.SubType != Spell) { failure = Spell + " projectile did not inherit magic equipment."; return; }
        child.AddEventListener(5, ignored => { sphereDeleted = true; Debug.Log("[DE128Native] " + Spell + " native deletion event."); });
        Debug.Log("[DE128Native] " + Spell + " native child created with inherited equipment.");
    }

    static void CheckProjectileActionParsing()
    {
        var move = AnimationData.Animations.Single(value => value.Name == "fixture.de128-combat:moves/projectile_actions");
        var actions = move.ScheduledActions;
        var spawn = actions.OfType<ActionCreateModel>().First();
        var doc = new System.Xml.XmlDocument();
        doc.Load(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Assets/DExml/animations/moves.xml"));
        var expected = new ActionCreateModel(doc.SelectSingleNode("//Move[@Name='Sphere1Player']/Actions/CreatePlayer"));
        if (spawn.ModelName != expected.ModelName || !spawn.NeedStart(2) || spawn.StartAnimation != expected.StartAnimation)
            throw new Exception("Projectile native name/frame/start differ from archive.");
        var itemsField = typeof(ActionCreateModel).GetField("IOHGFGNNCFA", Hidden);
        var actualItems = (List<CopyItemInfo>)itemsField.GetValue(spawn);
        var expectedItems = (List<CopyItemInfo>)itemsField.GetValue(expected);
        if (actualItems.Count != 2 || actualItems.Count != expectedItems.Count) throw new Exception("Projectile item count differs.");
        for (int i = 0; i < actualItems.Count; i++)
        {
            var a = actualItems[i]; var b = expectedItems[i];
            if (a.Type != b.Type || a.Name != b.Name || a.BLIKNEDFOFG != b.BLIKNEDFOFG || a.PCOBPICANEP != b.PCOBPICANEP)
                throw new Exception("Projectile native equipment inheritance differs.");
        }
        var preview = actions.OfType<ActionCreateModel>().Last();
        if (preview.StartAnimation != "fixture.de128-combat:moves/projectile_child" || !preview.NeedStart(3))
            throw new Exception("Owned projectile start_move did not reach native parser.");
        var child = AnimationData.Animations.Single(value => value.Name == preview.StartAnimation);
        var state = new ModelConditions { ModelName = "FixtureSphere", JJDNDOLCMMN = 1 };
        var nameCondition = child.SelectionConditions.OfType<ConditionName>().Single();
        var chargeCondition = child.SelectionConditions.OfType<ConditionBullets>().Single();
        if (!nameCondition.IsEqual(state) || !chargeCondition.IsEqual(state)) throw new Exception("Native spell conditions rejected matching state.");
        state.ModelName = "Other"; state.JJDNDOLCMMN = 0;
        if (nameCondition.IsEqual(state) || chargeCondition.IsEqual(state)) throw new Exception("Native spell conditions accepted invalid state.");
        var velocity = (Vector3f)typeof(InfoAnimation).GetField("KACPFNLDNND", Hidden).GetValue(child);
        var acceleration = (Vector3f)typeof(InfoAnimation).GetField("KNBDGOJAIAF", Hidden).GetValue(child);
        if (velocity.GetX() != 30 || acceleration.GetY() != -2 ||
            !(bool)typeof(InfoAnimation).GetField("AEDIIEEJKHE", Hidden).GetValue(child) ||
            !(bool)typeof(InfoAnimation).GetField("JCIKOMAMJDI", Hidden).GetValue(child))
            throw new Exception("Native projectile velocity/acceleration/recharge flags differ.");
        Debug.Log("[DE128Native] Authored actor/charge predicates and velocity, acceleration, preservation and no-recharge flags passed actual move parsing.");
        var bullets = actions.OfType<ActionAddBullets>().Single();
        var expectedBullets = new ActionAddBullets(doc.SelectSingleNode("//Move[@Name='Sphere1Player']/Actions/AddBullets"));
        if (bullets.Value != expectedBullets.Value || bullets.Value != -1 || !bullets.NeedStart(7))
            throw new Exception("Projectile charge action differs.");
        var delete = actions.OfType<ActionDelete>().Single();
        if (!delete.NeedStart(EventAnimation.EECEJKADLCK.EVENT_STRIKE)) throw new Exception("Projectile delete event differs.");
        Debug.Log("[DE128Native] Lua projectile actions match archived native equipment inheritance, owned start move, charge and deletion scheduling. Parsing only; no live projectile claim.");
    }

    static void CheckWarriorPerkLoadouts()
    {
        var catalog = ModRuntime.Scripts.Content;
        var adapter = new LegacyContentAdapter(catalog);
        int checkedPerks = 0;
        var bosses=catalog.Warriors.Where(value => value.Id.ToString().StartsWith("fixture.de128-combat:warriors/sensei_act_", StringComparison.Ordinal)).ToArray();
        if(bosses.Length!=12)throw new Exception("Expected twelve pending boss loadouts.");
        foreach (var warrior in bosses)
        {
            var document = new System.Xml.XmlDocument();
            var node = (System.Xml.XmlElement)typeof(LegacyContentAdapter).GetMethod("BuildWarriorNode", Hidden).Invoke(adapter, new object[] { document, warrior });
            foreach (System.Xml.XmlElement perk in node.SelectNodes("Perks/Perk"))
            {
                var baseline = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(perk.GetAttribute("Name"));
                if (baseline == null) throw new Exception("Missing native perk " + perk.GetAttribute("Name"));
                var originals=new[]{"Aspect","ChanceFactor","Chance","Frames"}.ToDictionary(field=>field,field=>baseline.EPBADFHIJAH().GetValue(field));
                var native = baseline.Clone(perk["Set"], null);
                var secondSettings = document.CreateElement("Set");
                secondSettings.SetAttribute("Aspect", "0");
                secondSettings.SetAttribute("Chance", "0");
                secondSettings.SetAttribute("Frames", "0");
                var second = baseline.Clone(secondSettings, null);
                var inherited = baseline.Clone(null, null);
                foreach(string field in new[]{"Aspect","ChanceFactor","Chance","Frames"})
                {
                    string original=originals[field];
                    string expected=perk["Set"].HasAttribute(field)?perk["Set"].GetAttribute(field):original;
                    if(baseline.EPBADFHIJAH().GetValue(field)!=original || native.EPBADFHIJAH().GetValue(field)!=expected || inherited.EPBADFHIJAH().GetValue(field)!=original ||
                        second.EPBADFHIJAH().GetValue(field)!=(secondSettings.HasAttribute(field)?"0":original))
                        throw new Exception("Native warrior clone lost settings/defaults or leaked between instances: "+warrior.Id+" "+field);
                }
                checkedPerks++;
            }
        }
        if (checkedPerks != bosses.Sum(value=>value.PerkLoadout.Count)) throw new Exception("Missing native boss perk instances.");
        Debug.Log("[DE128Native] PASS "+checkedPerks+" perk clones across twelve boss loadouts: Aspect/ChanceFactor/Chance/Frames, omitted defaults, explicit zero and instance isolation. Pending story is not activated; no boss AI/trigger playtest claim.");
    }

    static bool CheckMapBattleLock()
    {
        var id = DefinitionId.Parse("fixture.de128-combat:battles/lock_check");
        var catalog = ModRuntime.Scripts.Content;
        var definition = catalog.Battles.Single(value => value.Id == id);
        var zone = catalog.Zones.Single(value => value.Id == definition.Zone);
        var nativeId = new FightIDS(zone.LegacyName + "|" + definition.LegacyName + "|");
        var roster = ListSF.CCDKHLAMKKO();
        var record = roster.GetSavedBattles().SingleOrDefault(value => value.GetBattleId().Equals(nativeId));
        if (record == null)
        {
            // Controlled pre-existing map entry: this check is about changing a
            // lock, not native quest/session timing or initial story revelation.
            if (ModBattleAccess.SetLocked(id, false)) throw new Exception("Missing map entry was fabricated by a lock query.");
            roster.AddBattle(nativeId, true, true, true, false, 7);
            ListSF.MKHAAGMJOPG(nativeId).IsMapVisible = true;
            record = roster.GetSavedBattles().Single(value => value.GetBattleId().Equals(nativeId));
        }
        var nodeField = typeof(RosterBattle).GetField("_node", Hidden);
        var node = (System.Xml.XmlNode)nodeField.GetValue(record);
        string original = node.OuterXml;
        if (!ModBattleAccess.SetLocked(id, false)) return false; // Scene/quest initialization may still block input.
        if (record.IsLocked()) throw new Exception("Native roster stayed locked.");
        Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(true);
        try
        {
            if (ModBattleAccess.SetLocked(id, true) || record.IsLocked()) throw new Exception("Native input block did not protect progression.");
        }
        finally { Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(false); }
        var mutation = typeof(ModRuntime).GetField("_profileMutationState", BindingFlags.Static | BindingFlags.NonPublic);
        mutation.SetValue(null, 1);
        try
        {
            if (ModBattleAccess.SetLocked(id, true) || record.IsLocked()) throw new Exception("Settlement did not protect progression.");
        }
        finally { mutation.SetValue(null, 0); }
        var expected = new System.Xml.XmlDocument(); expected.LoadXml(original);
        expected.DocumentElement.SetAttribute("Locked", "0");
        if (node.OuterXml != expected.DocumentElement.OuterXml) throw new Exception("Lock update changed unrelated battle save fields.");
        var map = Nekki.SF2.GUI.Scene<Nekki.SF2.GUI.Map.MapScene>.get_Current();
        var nativeBattle = ListSF.MKHAAGMJOPG(nativeId);
        Func<Nekki.SF2.GUI.Map.BattleButton> currentButton = () => map.GetComponentsInChildren<Nekki.SF2.GUI.Map.MapPanel>(true)
            .SelectMany(panel => panel.GetZones()).Select(item => item.GetButtonByBattle(nativeBattle)).FirstOrDefault(value => value != null);
        var button = currentButton();
        if (button == null || button.Locked) throw new Exception("Native map button did not unlock.");
        if (!ModBattleAccess.SetLocked(id, true) || !record.IsLocked()) throw new Exception("Native relock failed.");
        button = currentButton();
        if (button == null || !button.Locked) throw new Exception("Native map button did not relock.");
        var registeredZones = new HashSet<Nekki.SF2.GUI.Map.ZoneScrollItem>(map.GetComponentsInChildren<Nekki.SF2.GUI.Map.MapPanel>(true).SelectMany(panel => panel.GetZones()));
        if (map.GetComponentsInChildren<Nekki.SF2.GUI.Map.ZoneScrollItem>(true).Any(item => item.gameObject.activeSelf && !registeredZones.Contains(item)))
            throw new Exception("Map reload left obsolete zones and buttons active.");
        string relocked = node.OuterXml;
        if (!ModBattleAccess.SetLocked(id, true) || node.OuterXml != relocked) throw new Exception("Repeated lock changed saved data.");
        Debug.Log("[DE128Native] PASS battle lock: map/input/settlement gates, native save flag and rendered button unlock/relock, unrelated fields preserved, idempotent repeat, obsolete zones inactive. Isolated fixture save only.");
        return true;
    }

    static void CheckMapBattleReveal()
    {
        var id = DefinitionId.Parse("fixture.de128-combat:battles/reveal_check");
        var catalog = ModRuntime.Scripts.Content;
        var definition = catalog.Battles.Single(value => value.Id == id);
        var zone = catalog.Zones.Single(value => value.Id == definition.Zone);
        var nativeId = new FightIDS(zone.LegacyName + "|" + definition.LegacyName + "|");
        var roster = ListSF.CCDKHLAMKKO();
        var native = ListSF.GetInstance().FindBattleForModding(zone.LegacyName, definition.LegacyName);
        var map = Nekki.SF2.GUI.Scene<Nekki.SF2.GUI.Map.MapScene>.get_Current();
        // Repeatable isolated fixture: remove this test entry, never an owner save.
        roster.HEHJKDPAPLA(nativeId);
        native.IsMapVisible = false;
        map.ReloadZones();
        int before = roster.GetSavedBattles().Count;
        if (ModBattleAccess.Focus(id) || ModBattleAccess.SetLocked(id, false) || roster.GetSavedBattles().Count != before)
            throw new Exception("Missing entry was created by focus/lock.");
        Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(true);
        try
        {
            if (ModBattleAccess.Reveal(id, true) || roster.GetSavedBattles().Count != before)
                throw new Exception("Blocked reveal mutated profile.");
        }
        finally { Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(false); }
        if (!ModBattleAccess.Reveal(id, true)) throw new Exception("Fresh reveal failed.");
        var record = roster.GetSavedBattles().Single(value => value.GetBattleId().Equals(nativeId));
        if (roster.GetSavedBattles().Count != before + 1 || !record.IsLocked() || !native.IsMapVisible)
            throw new Exception("Fresh reveal did not create exactly one locked, visible entry.");
        Func<Nekki.SF2.GUI.Map.BattleButton> currentButton = () => map.GetComponentsInChildren<Nekki.SF2.GUI.Map.MapPanel>(true)
            .SelectMany(panel => panel.GetZones()).Select(item => item.GetButtonByBattle(native)).FirstOrDefault(value => value != null);
        if (currentButton() == null || !currentButton().Locked || !currentButton().gameObject.activeSelf)
            throw new Exception("Fresh zone/battle not rendered after reveal.");
        record.FHCHCHPPMEI(7);
        var node = (System.Xml.XmlNode)typeof(RosterBattle).GetField("_node", Hidden).GetValue(record);
        string saved = node.OuterXml;
        if (!ModBattleAccess.Reveal(id, false) || node.OuterXml != saved || roster.GetSavedBattles().Count != before + 1)
            throw new Exception("Repeated reveal reset an existing lock/replay count or duplicated it.");
        if (!ModBattleAccess.Focus(id)) throw new Exception("Visible locked entry could not be focused.");
        var focus = roster.KNJNHKDCINB();
        if (focus.PELHCAEAOFE() != zone.LegacyName || focus.CPHDPCAECJN() != definition.LegacyName)
            throw new Exception("Focus did not update native profile selection.");
        if (!map.GetComponentsInChildren<Nekki.SF2.GUI.Map.MapPanel>(true).Any(panel => panel.GetCurrentZone()?.get_LastBattle() == native))
            throw new Exception("Focus did not select the live map entry.");
        record.HCEOCBOFIGC(true);
        map.ReloadZones();
        saved = node.OuterXml;
        if (ModBattleAccess.Focus(id) || !ModBattleAccess.Reveal(id, false) || node.OuterXml != saved)
            throw new Exception("Focus/reveal bypassed existing hidden state.");
        record.HCEOCBOFIGC(false);
        map.ReloadZones();
        if (!ModBattleAccess.SetLocked(id, false) || !ModBattleAccess.Reveal(id, true) || record.IsLocked())
            throw new Exception("Initial lock value was reapplied after progression.");
        var restored = new RosterBattle(node.CloneNode(true));
        if (restored.IsLocked() || !restored.GetBattleId().Equals(nativeId))
            throw new Exception("Revealed record failed native serialization roundtrip.");
        Debug.Log("[DE128Native] PASS reveal/focus: fresh saved zone entry, native button, blocked/missing guards, no duplicate/reset, hidden-state preservation, current map and saved focus, native record roundtrip. No full profile reload claim.");
    }

    static void CheckEclipseModeSwitch()
    {
        var roster=ListSF.CCDKHLAMKKO();
        var map=Nekki.SF2.GUI.Scene<Nekki.SF2.GUI.Map.MapScene>.get_Current();
        var buttons=MapButtonController.ELEBLBJKDBI();
        // Controlled native initial state in this isolated acceptance profile only.
        roster.SetEclipseMode(true);
        buttons.DMCBGLJHBPA("EclipseModeOff");buttons.DMCBGLJHBPA("EclipseModeOn");
        if(ModProfileAccess.SetEclipseMode(false)||!roster.IsEclipseMode())throw new Exception("Missing switch bypassed native availability.");
        buttons.GKIOOABOBFL(new MapButtonInfo("EclipseModeOff","eclipse","",new Vector2(-876,432),anchorMinX:1,anchorMaxX:1));
        Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(true);
        try { if(ModProfileAccess.SetEclipseMode(false)||!roster.IsEclipseMode())throw new Exception("Blocked switch changed mode."); }
        finally { Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked(false); }
        if(!ModProfileAccess.SetEclipseMode(false)||roster.IsEclipseMode())throw new Exception("Native mode-off request failed.");
        var active=map.GetComponentsInChildren<Nekki.SF2.GUI.Map.MapButton>().Where(button=>button.isActiveAndEnabled).ToArray();
        if(!active.Any(button=>button.get_MapButtonInfo().Name=="EclipseModeOn") || active.Any(button=>button.get_MapButtonInfo().Name=="EclipseModeOff"))
            throw new Exception("Native mode quest did not replace switch artwork/action.");
        if(roster.EPEDEDLCAJF()!=Color.white)throw new Exception("Native mode quest did not reset map tint.");
        if(!ModProfileAccess.SetEclipseMode(false))throw new Exception("Repeated mode request failed.");
        Debug.Log("[DE128Native] PASS Eclipse mode request: missing/blocked switch refused, actual map-button quest changed mode, replacement button and white map tint, idempotent repeat. Isolated profile only.");
    }

    static void CheckProfileFightProgress()
    {
        var id = CoreContentImporter.FightId("ZONE_1", "Tournament", "3");
        string nativeId = ModRuntime.Scripts.Content.RuntimeFightId(id);
        var roster = ListSF.CCDKHLAMKKO();
        var records = roster.NIDBIFOJMAP();
        var before = records.ToArray();
        var profileField = typeof(ModRuntime).GetField("_profileRoster", BindingFlags.Static | BindingFlags.NonPublic);
        var bound = profileField.GetValue(null);
        try
        {
            records.Clear();
            var absent = ModProfileAccess.Fight(id);
            if (absent.Present || absent.Wins != 0 || absent.Losses != 0 || records.Count != 0)
                throw new Exception("Read-only fight query created missing progress.");
            var document = new System.Xml.XmlDocument();
            document.LoadXml("<Fight IDS='" + nativeId + "' CompletedCount='7' LossCount='3' EclipseCompletedCount='99' EclipseLossCount='88'/>");
            var record = new RosterFight(document.DocumentElement);
            records.Add(record);
            string saved = document.OuterXml;
            var snapshot = ModProfileAccess.Fight(id);
            if (!snapshot.Present || snapshot.Wins != 7 || snapshot.Losses != 3 || document.OuterXml != saved || records.Count != 1)
                throw new Exception("Fight snapshot differs from saved native counters or mutates save data.");
            record.OBFNFKPHJIN(8);
            if (ModProfileAccess.Fight(id).Wins != 8 || snapshot.Wins != 7)
                throw new Exception("Fight snapshot is cached or mutable.");
            bool rejected = false;
            try { ModProfileAccess.Fight(DefinitionId.Parse("core:fights/missing")); } catch (ModContentException) { rejected = true; }
            if (!rejected) throw new Exception("Missing catalog fight returned fabricated zero progress.");
            profileField.SetValue(null, null);
            if (ModProfileAccess.Fight(id) != null) throw new Exception("Unbound profile exposed progress.");
        }
        finally
        {
            profileField.SetValue(null, bound);
            records.Clear(); records.AddRange(before);
        }
        Debug.Log("[DE128Native] PASS profile fight queries: actual native roster lookup, absent/no mutation, saved normal counters, fresh detached values, unknown fight and unbound profile. Original records restored.");
    }

    static void CheckSenseiRewards()
    {
        var catalog = ModRuntime.Scripts.Content;
        var adapter = new LegacyContentAdapter(catalog);
        var archive = new System.Xml.XmlDocument(); archive.Load("Assets/DExml/stages.xml");
        int count = 0;
        foreach (System.Xml.XmlElement zone in archive.SelectNodes("/Stages/Zones/Zone"))
        foreach (System.Xml.XmlElement battle in zone.SelectNodes("Battle[@Name='SENSEI_MEMORIES' or @Name='SENSEI_MEMORIES_ECLIPSEMODE']"))
        foreach (System.Xml.XmlElement fight in battle.SelectNodes("Fight"))
        {
            int wins = 0;
            foreach (System.Xml.XmlElement expected in fight.SelectNodes("Rewards/Reward"))
            {
                string suffix = battle.GetAttribute("Name") == "SENSEI_MEMORIES" ? "normal_" + fight.GetAttribute("Name") + "_" + wins : "eclipse_" + wins;
                string id = "fixture.de128-combat:rewards/sensei_act_" + zone.GetAttribute("Name").Replace("ZONE_", "") + "_" + suffix;
                var definition = catalog.Rewards.Single(value => value.Id.ToString() == id);
                var actual = (System.Xml.XmlElement)typeof(LegacyContentAdapter).GetMethod("BuildRewardNode", Hidden).Invoke(adapter, new object[] { new System.Xml.XmlDocument(), definition });
                foreach (ushort exponent in new ushort[] { 0, 2 })
                {
                    var result = EvaluateReward(actual, exponent);
                    var baseline = EvaluateReward(expected, exponent);
                    if (result.PMIHPJFAJIO.exp != baseline.PMIHPJFAJIO.exp || result.KMGLLBMIDHJ() != baseline.KMGLLBMIDHJ() ||
                        result.BNILCODHHKC() != baseline.BNILCODHHKC() ||
                        result.AIOMDIAFHGB.ECOOCLMNFJM.PJBCIEMHPNN != baseline.AIOMDIAFHGB.ECOOCLMNFJM.PJBCIEMHPNN)
                        throw new Exception("Sensei native result differs from archive: " + id);
                    if ((uint)result.PMIHPJFAJIO.exp != definition.Experience || result.BNILCODHHKC() != definition.Gems ||
                        result.AIOMDIAFHGB.ECOOCLMNFJM.PJBCIEMHPNN != (long)(definition.PrizeBase.Value * Mathf.Pow(10, exponent)))
                        throw new Exception("Sensei native result lost configured scalar: " + id);
                }
                wins++; count++;
            }
        }
        if (count != 57) throw new Exception("Expected 57 Sensei reward slots, got " + count);
        Debug.Log("[DE128Native] PASS all 57 Sensei slots: native FightResult experience, gems and performance coins match archive at two scales. No profile settlement or story activation.");
    }

    static FightResult EvaluateReward(System.Xml.XmlNode node, ushort exponent)
    {
        // Detached native results exercise the actual bonus calculation without
        // awarding currency/experience to any profile or starting story fights.
        var result = new FightResult();
        var statistics = new ComboStatistic { JDKFHFOJKPI = 2, MOLDOOIJELI = 1, KKJHBKBMPGN = 3, OGMOILIMCOM = 1 };
        result.BDLLAEPPAKL(new Reward(node, 0, exponent), -1, statistics, new ComboStatistic(), false);
        return result;
    }

    static void CheckSharedMovePatches()
    {
        var moves = AnimationData.Animations;
        var heavy = moves.Single(move => move.Name == "RangedHeavyPlayer");
        var interval = heavy.MoveData.Intervals.Single(value => (value.NodeInterval?.Attributes?["Name"]?.Value ?? value.Name) == "Uninterrupt");
        int end = interval.NodeInterval == null ? interval.EndFrame : int.Parse(interval.NodeInterval.Attributes["End"].Value);
        if (end != 40) throw new Exception("DE ranged uninterrupt end was not patched.");
        var fly = moves.Single(move => move.Name == "ChakramFly").MoveData.Intervals.OfType<IntervalAttack>().Single();
        string reaction = fly.NodeInterval == null ? fly.HitReactions.Single().Name : fly.NodeInterval["Hit"].Attributes["Name"].Value;
        if (reaction != "MiddleShortPlus") throw new Exception("DE Chakram reaction was not patched.");
        var sound = moves.Single(move => move.Name == "ShopRangedTryOnHeavyPlayer").ScheduledActions.OfType<ActionSound>().Single(value => value.get_Name() == "snd_disk");
        if (!sound.NeedStart(16) || sound.NeedStart(18)) throw new Exception("DE Chakram sound timing was not patched.");
        foreach (string name in new[] { "MassBombPlayer", "LightningArrowPlayer" })
        {
            var move = moves.Single(value => value.Name == name);
            if (!(move.SelectionConditions.Last() is ConditionModExists condition) || condition.get_Name() != "Stun" || !condition.IsNot)
                throw new Exception("DE not-Stun condition missing from " + name);
            var parameters = new ModelConditions();
            typeof(ModelConditions).GetField("LPGJIICFIKF").SetValue(parameters,new System.Collections.Generic.List<PerksStage.ActionPerk>());
            if (!condition.IsEqual(parameters)) throw new Exception("DE not-Stun condition rejected an unstunned fighter.");
            var action = new PerkAction();
            typeof(PerkAction).GetMethod("set_Name",Hidden).Invoke(action,new object[] { "Stun" });
            var active = new PerksStage.ActionPerk();
            typeof(PerksStage.ActionPerk).GetField("AMKJNPOCODK").SetValue(active,action);
            ((System.Collections.Generic.List<PerksStage.ActionPerk>)typeof(ModelConditions).GetField("LPGJIICFIKF").GetValue(parameters)).Add(active);
            if (condition.IsEqual(parameters)) throw new Exception("DE not-Stun condition accepted a stunned fighter.");
        }
        CheckNativePatchLifecycle();
        Debug.Log("[DE128Native] Five shared move patches reached native conditions, intervals, reactions and sound scheduling; native Stun predicates and patch rollback passed.");
    }
    static void CheckNativePatchLifecycle()
    {
        var source = new System.Xml.XmlDocument(); source.Load("Assets/vanillaXml/animations/moves.xml");
        var runtime = typeof(LegacyContentAdapter).Assembly.GetType("Eclipse.Modding.MoveCombatPatchRuntime");
        var apply = runtime.GetMethod("Apply",BindingFlags.Static|BindingFlags.NonPublic);
        foreach (bool initialized in new[] { false,true })
        {
            var move = new InfoAnimation { Name = "FixturePatchLifecycle" };
            var interval = new IntervalAnimation(IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE);
            interval.Parse(source.SelectSingleNode("//Moves/Move[@Name='RangedHeavyPlayer']/Intervals/Interval[@Name='Uninterrupt']").CloneNode(true));
            var attack = new IntervalAttack();
            attack.Parse(source.SelectSingleNode("//Moves/Move[@Name='ChakramFly']/Intervals/Interval[@Type='Attack']").CloneNode(true));
            attack.set_AnimationFinishFrame(50);
            var soundNode = new System.Xml.XmlDocument(); soundNode.LoadXml("<Sound Name='snd_disk' Frame='18'/>");
            var sound = new ActionSound(soundNode.DocumentElement);
            move.MoveData.Intervals.Add(interval);move.MoveData.Intervals.Add(attack);move.ScheduledActions.Add(sound);
            if (initialized) { interval.Init(); attack.Init(); }
            var patch = new MoveCombatPatch(ModId.Parse("fixture.native-patch"),move.Name,
                intervalEnd:new ModMoveFramePatch("Uninterrupt",42,40),hit:new ModMoveHitPatch("High","MiddleShortPlus"),
                soundFrame:new ModMoveFramePatch("snd_disk",18,16));
            Func<ModMoveCondition,ConditionAnimation> noConditions = value => throw new Exception("Unexpected fixture condition.");
            var lifetime = (IDisposable)apply.Invoke(null,new object[] { new[] { move },new[] { patch },noConditions });
            if (!initialized) { interval.Init(); attack.Init(); }
            if (interval.EndFrame != 40 || attack.GetReactionName(attack.Start) != "MiddleShortPlus" || !sound.NeedStart(16))
                throw new Exception("Native patch did not survive interval initialization.");
            lifetime.Dispose();lifetime.Dispose();
            if (interval.EndFrame != 42 || attack.GetReactionName(attack.Start) != "High" || !sound.NeedStart(18))
                throw new Exception("Native patch rollback after initialization failed.");
        }
    }
    static void CheckRestoredEquipment()
    {
        string[] ids = { "armor/dragon_carapace", "armor/old_legionnaire_armour", "armor/samurai_armour", "helm/gabled_helm",
            "helm/dragon_helm", "ranged/dragon_boomerangs", "magic/dragons_breath", "magic/lightning_arc", "magic/minor_charge_of_darkness", "magic/medium_charge_of_darkness", "magic/large_charge_of_darkness", "magic/blast_of_the_void", "magic/mind_throw" };
        string[] names = { "ARMOR_C2_Z5_DRAGON", "ARMOR_OLD_LEGIONER", "ARMOR_BIG_SHOGUN_OLD", "HELM_GABLED_OLD",
            "HELM_C2_Z5_DRAGON", "RANGED_C2_Z5_DRAGON_BOOMERANG", "MAGIC_C2_Z5_DRAGON_EARTHQUAKE", "MAGIC_LIGHTNING", "Sphere1", "Sphere2", "Sphere3", "ComboSphere3", "MAGIC_MIND_THROW_NORMAL" };
        var archive = new System.Xml.XmlDocument(); archive.Load("Assets/DExml/list.xml");
        for (int i = 0; i < ids.Length; i++)
        {
            var row = (System.Xml.XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + names[i] + "']");
            var id = DefinitionId.Parse("de128:items/" + ids[i]);
            if (!ModRuntime.Scripts.Content.TryGetItem(id, out var definition)) throw new Exception("Missing restored equipment " + id);
            var item = ListSF.GetItems().GetItemByName(id.ToString());
            if (item == null) throw new Exception("Missing native equipment " + id);
            foreach (string attribute in new[] { "Type", "SubType", "Level", "UpgradeLevel", "BonusPrice", "PackLabel" })
                if ((item.NodeXML.Attributes[attribute]?.Value ?? "") != row.GetAttribute(attribute)) throw new Exception("Native equipment " + attribute + " mismatch " + id);
            var archived = new ItemInfo(row);
            var field = typeof(ItemInfo).GetField("IBLHIAHECLK");
            var before = (Attributes)field.GetValue(archived); var after = (Attributes)field.GetValue(item);
            foreach (string stat in new[] { "WeaponDamage", "UnarmedDamage", "BodyDefense", "HeadDefense", "RangedDamage", "MagicDamage" })
            {
                int expected = 0, actual = 0;
                if (before.Get(stat, ref expected, false) != after.Get(stat, ref actual, false) || expected != actual)
                    throw new Exception("Native initial stat mismatch " + id + "/" + stat);
            }
            if (item.NodeXML["Upgrades"].Attributes["Template"].Value != row["Upgrades"].Attributes["Template"].Value)
                throw new Exception("Native upgrade template mismatch " + id);
            var perk = row.SelectSingleNode("Enchantments/Perk");
            if (item.DefaultEnchantmentPreviews.Count != 1 || item.DefaultEnchantments.Count != 1 ||
                item.DefaultEnchantments[0].get_Name() != perk.Attributes["Name"].Value ||
                item.DefaultEnchantments[0].Pairs.Single(pair => pair.Key == "Aspect").Value != perk["Set"].Attributes["Aspect"].Value)
                throw new Exception("Native equipment enchantment mismatch " + id);
            var assets = new CoreAssetProvider(); string model; UnityEngine.Sprite icon;
            if (!assets.TryLoadModelText(definition.Model, out model) || string.IsNullOrEmpty(model) ||
                !assets.TryLoadUnityAsset<UnityEngine.Sprite>(definition.Icon, out icon) || icon == null || icon.vertices.Length == 0)
                throw new Exception("Native equipment art missing " + id);
            Debug.Log("[DE128Native] Restored equipment matches archive and loads art: " + id);
        }
        Debug.Log("[DE128Native] Thirteen restored equipment definitions passed native stat-presence, listing, upgrade, enchantment and asset checks.");
    }
    static void CheckRestoredWeapons()
    {
        var vanilla = new System.Xml.XmlDocument(); vanilla.Load("Assets/vanillaXml/list.xml");
        var archive = new System.Xml.XmlDocument(); archive.Load("Assets/DExml/list.xml");
        int count = 0;
        foreach (System.Xml.XmlElement row in archive.SelectNodes("/List/Items/Item[@Type='Weapon']"))
        {
            if (vanilla.SelectSingleNode("/List/Items/Item[@Name='" + row.GetAttribute("Name") + "']") != null) continue;
            var weapon = ModRuntime.Scripts.Content.Weapons.Single(value => !value.IsCore && value.Model.ToString() ==
                ("core:gamedata/models/" + row.GetAttribute("Model")).ToLowerInvariant());
            var item = ListSF.GetItems().GetItemByName(weapon.Id.ToString());
            if (item == null) throw new Exception("Missing native item " + weapon.Id);
            foreach (string attribute in new[] { "SubType", "Level", "UpgradeLevel", "BonusPrice", "PackLabel" })
                if (item.NodeXML.Attributes[attribute]?.Value != row.GetAttribute(attribute)) throw new Exception("Native " + attribute + " mismatch: " + weapon.Id);
            if (row.HasAttribute("WeaponDamage") && item.NodeXML.Attributes["WeaponDamage"]?.Value != row.GetAttribute("WeaponDamage"))
                throw new Exception("Native damage mismatch: " + weapon.Id);
            if (item.DefaultEnchantments.Count != 1 || item.DefaultEnchantmentPreviews.Count != 1) throw new Exception("Native default enchantments missing: " + weapon.Id);
            var expectedPerk = row.SelectSingleNode("Enchantments/Perk");
            var granted = item.DefaultEnchantments.Single();
            if (granted.get_Name() != expectedPerk.Attributes["Name"].Value ||
                granted.Pairs.Single(pair => pair.Key == "Aspect").Value != expectedPerk["Set"].Attributes["Aspect"].Value)
                throw new Exception("Native enchantment identity/aspect mismatch: " + weapon.Id);
            if ((string)typeof(ItemInfo).GetField("MMHIKEIDDNB").GetValue(item) != row.GetAttribute("PackLabel"))
                throw new Exception("Native quest notification group mismatch: " + weapon.Id);
            var assets = new CoreAssetProvider();
            string modelText; UnityEngine.Sprite icon;
            if (!assets.TryLoadModelText(weapon.Model, out modelText) || string.IsNullOrEmpty(modelText) ||
                !assets.TryLoadUnityAsset<UnityEngine.Sprite>(weapon.Icon, out icon) || icon == null || icon.vertices.Length == 0)
                throw new Exception("Restored native model/icon unavailable: " + weapon.Id);
            var archived = new ItemInfo(row);
            var attributes = typeof(ItemInfo).GetField("IBLHIAHECLK");
            var before = (Attributes)attributes.GetValue(archived);
            var after = (Attributes)attributes.GetValue(item);
            int expected = 0, actual = 0;
            bool beforePresent = before.Get("WeaponDamage", ref expected, false);
            bool afterPresent = after.Get("WeaponDamage", ref actual, false);
            Debug.Log("[DE128Native] Restored " + weapon.Id + " damage=" + actual + " archived=" + expected + " present=" + beforePresent + "/" + afterPresent);
            if (actual != expected || beforePresent != afterPresent) throw new Exception("Native archived damage semantics differ: " + weapon.Id);
            if (row.GetAttribute("Name") == "WEAPON_MOON_FANS")
            {
                var upgrades = (System.Collections.Generic.List<UpgradeData>)typeof(ItemInfo).GetMethod("DNFDAGFAANJ")
                    .Invoke(item, new object[] { true, int.MaxValue });
                if (upgrades.Count == 0) throw new Exception("Moon Fans lost native upgrades.");
                var copy = item.Clone(); var archiveCopy = archived.Clone();
                typeof(ItemInfo).GetMethod("HPCGCMMGAAP").Invoke(copy, new object[] { upgrades[0] });
                typeof(ItemInfo).GetMethod("HPCGCMMGAAP").Invoke(archiveCopy, new object[] { upgrades[0] });
                int upgraded = 0, archivedUpgrade = 0;
                if (!((Attributes)attributes.GetValue(copy)).Get("WeaponDamage", ref upgraded, false) ||
                    !((Attributes)attributes.GetValue(archiveCopy)).Get("WeaponDamage", ref archivedUpgrade, false) ||
                    upgraded <= 0 || upgraded != archivedUpgrade) throw new Exception("Moon Fans native upgrade semantics differ.");
                Debug.Log("[DE128Native] Moon Fans absent initial damage preserved; native first upgrade damage=" + upgraded);
            }
            count++;
        }
        if (count != 10) throw new Exception("Native restored weapon count=" + count);
        Debug.Log("[DE128Native] Ten restored native equipment definitions match archive stats, pricing, group labels and have default enchantments.");
    }
    static void OnAnimation(object value)
    {
        if ((value as InfoAnimation)?.Name == "de128:moves/mind_throw_player2") { mindFollowup = true; Debug.Log("[DE128Native] MindThrow follow-up selected."); }
        if ((value as InfoAnimation)?.Name == SpellMove)
        {
            if (spellSelected) { failure = Spell + " selected repeatedly after input release."; return; }
            spellSelected = true; Debug.Log("[DE128Native] " + Spell + " cast selected."); return;
        }
        if ((value as InfoAnimation)?.Name != Move) return;
        if (selected) { failure = "Unexpected repeated selection after input release."; return; }
        selected = true; selectedAt = Fight.GetCurrentFight().get_FightTimeInFrames();
        Debug.Log("[DE128Native] Selected at frame " + selectedAt);
    }
    static void OnAnimationEnd(object value) { if ((value as InfoAnimation)?.Name == Move) finished = true; }
    static void OnInterval(object value)
    {
        if (actor.OCPMJKIEPIG().NNMAFFCCMHC()?.Name == "de128:moves/mind_throw_player2" && value is IntervalAttack final && final.Start == 48) mindFinalAttack = true;
        if (actor.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != Move || !(value is IntervalAttack attack)) return;
        var edges = (System.Collections.ICollection)typeof(ModelAnimation).GetField("ECNLLKIJIGP", Hidden).GetValue(actor.OCPMJKIEPIG());
        if (edges.Count != attack.IKPJJAEIOCG().Count) failure = "Attack edge binding failed at sample " + attack.Start + ": " + edges.Count + "/" + attack.IKPJJAEIOCG().Count;
        attacks++; AttackFrames.Add(attack.Start);
    }
    static void OnActions(object value)
    {
        if (actor.OCPMJKIEPIG().NNMAFFCCMHC()?.Name != Move || !(value is List<ActionAnimation> actions)) return;
        foreach (var action in actions)
            if (action is ActionRandomSound)
                foreach (int frame in new[] { 8, 17, 28, 33 }) if (action.NeedStart(frame)) { swishes++; SoundFrames.Add(frame); }
    }
    static void Capture(string message, string stack, LogType type)
    {
        if (message.Contains("[DE128Flag]"))
        {
            try
            {
                var stage = Fight.GetCurrentFight().IEEGPNLEKHH();
                var state = new ModelConditions();
                stage.AINGCNFDFMM(actor, state.LPGJIICFIKF);
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml("<ModExists Player='Me' Name='" + FlagName + "'/>");
                var condition = new ConditionModExists(doc.DocumentElement);
                condition.Parse(doc.DocumentElement);
                bool exists = condition.IsEqual(state);
                if (message.Contains("set|"))
                {
                    if (!exists || state.LPGJIICFIKF.Count(value => value.DDBPICENEJE() == FlagName) != 1)
                        throw new Exception("Lua set_flag did not create exactly one native flag.");
                    flagSetChecked = true;
                }
                else
                {
                    stage.KCEBAJBMJGF(actor, state.FPFKABHOEHP);
                    if (exists || flagExpiries != 1 || state.FPFKABHOEHP.Count(value => value.DDBPICENEJE() == FlagName) != 1)
                        throw new Exception("Lua clear_flag did not record exactly one native expiry.");
                    flagClearChecked = true;
                }
            }
            catch (Exception exception) { failure = exception.ToString(); }
        }
        int lifecycle = message.IndexOf("[DE128Lifecycle] ", StringComparison.Ordinal);
        if (lifecycle >= 0)
        {
            string[] fields = message.Substring(lifecycle + "[DE128Lifecycle] ".Length).Trim().Split('|');
            if (fields.Length != 5 || !int.TryParse(fields[4], out int frame) || frame < 0)
                failure = "Malformed Lua lifecycle event: " + message;
            else Lifecycle.Add(string.Join("|", fields.Take(4)));
        }
        if (attached && message.Contains("[ModCombat]") && message.Contains("Animation")) failure = message;
        if (attached && type == LogType.Exception && failure == null) failure = message + "\n" + stack;
    }
    static void Finish(int code)
    {
        SessionState.SetBool(Active, false); EditorApplication.update -= Update;
        Application.logMessageReceived -= Capture; EditorApplication.Exit(code);
    }
}
