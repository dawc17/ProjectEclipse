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
                CheckProjectileActionParsing();
                CheckSharedMovePatches();
                CheckRestoredWeapons();
                CheckRestoredEquipment();
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
