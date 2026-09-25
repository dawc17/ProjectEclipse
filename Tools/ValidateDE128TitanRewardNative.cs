using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Eclipse.Modding;
using Nekki.SF2.GUI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only in the independently copied project made by TestDE128TitanRewardNative.py.
[InitializeOnLoad]
public static class ValidateDE128TitanRewardNative
{
    const string Active = "Eclipse.DE128TitanRewardNative.Active";
    const string Prefix = "[DE128TitanRewardNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly string[] Names = {
        "de128:items/weapon/titans_desolator", "de128:items/armor/titans_form",
        "de128:items/helm/titans_helm", "de128:items/ranged/titans_harpoon",
        "de128:items/magic/titans_mind_throw"
    };
    static readonly string[] Perks = {
        "PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON", "PERK_ITEM_SPECIAL_SHIELDING_ARMOR",
        "PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_HEAD_HELM", "PERK_ITEM_SPECIAL_PRECISION_RANGED",
        "PERK_ITEM_SPECIAL_FRENZY_MAGIC"
    };
    static double started;
    static bool campaign;

    static ValidateDE128TitanRewardNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-titan-reward-fixture.marker")))
            throw new InvalidOperationException("Titan reward acceptance requires an isolated project copy.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        string profileTag = Environment.GetEnvironmentVariable("ECLIPSE_DE128_TITAN_PROFILE_TAG");
        PlayerSettings.productName = Path.GetFileName(root) + "-" +
            (string.IsNullOrEmpty(profileTag) ? "titanreward" : profileTag);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup - started > 300)
                throw new Exception("Titan reward acceptance timed out.");
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
            if (module.NMCNDOPKFJD() != ScreenType.ModuleDojo && module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
            if (scripts.Diagnostics.Count != 0 || scripts.StateDiagnostics.Count != 0)
                throw new Exception("DE128 diagnostics: " + string.Join("; ", scripts.Diagnostics) +
                    "; state=" + string.Join("; ", scripts.StateDiagnostics));
            string phase = Environment.GetEnvironmentVariable("ECLIPSE_DE128_TITAN_PHASE");
            if (phase == "grant") Grant(roster, scripts);
            else if (phase == "reload") Reload(roster);
            else throw new Exception("Unknown Titan reward acceptance phase: " + phase);
            Debug.Log(Prefix + "PASS " + phase + ": five archived grants, enchantments and native inventory.");
            Finish(0);
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static void Grant(Roster roster, ModScriptSession scripts)
    {
        roster.Level = 52;
        var id = DefinitionId.Parse("core:fights/zone_7/c3_boss_titan_eclipsemode/6");
        var fight = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(id)));
        var eclipse = fight?.OOOBLJIHBEP(1)?.LJLIFMOIAJJ;
        if (eclipse == null) throw new Exception("Final Eclipse Titan reward slot is unavailable.");
        var prize = eclipse.KOBOIFJNPMO(1);
        if (prize.HELFDCAIJNE.Count != 5)
            throw new Exception("Expected five live Titan drops, found " + prize.HELFDCAIJNE.Count);
        var result = new FightResult.ResultPrizeStruct();
        for (int i = 0; i < Names.Length; i++)
        {
            var grant = prize.HELFDCAIJNE[i];
            bool configured = (bool)typeof(RewardItem).GetProperty("HasEclipseGrantConfiguration", Hidden).GetValue(grant);
            int grantIndex = (int)typeof(RewardItem).GetProperty("EclipseGrantIndex", Hidden).GetValue(grant);
            if (grant.Name != Names[i] || !configured || grantIndex != i)
                throw new Exception("Titan reward identity or grant marker differs: " + i);
            result.KFJABAMAKOD(grant);
            if (result.HELFDCAIJNE.Count != i + 1)
                throw new Exception("Native Titan reward configuration skipped grant " + i);
            var projected = result.HELFDCAIJNE[i];
            if (projected.DLKPBAJDHBO.ItemLevel != roster.Level ||
                projected.NAIEGGHELIH.LDLPCOFHFKE.Count != 1 ||
                projected.NAIEGGHELIH.LDLPCOFHFKE[0].get_Name() != Perks[i])
                throw new Exception("Native level or enchantment projection differs: " + i);
            Debug.Log(Prefix + "Projected " + Names[i] + " at level " + projected.DLKPBAJDHBO.ItemLevel);
        }
        ListSF.GetInstance().IMDGMNFHFCN(result);
        var inventory = roster.KHCNHPCPFII();
        for (int i = 0; i < Names.Length; i++)
        {
            var owned = inventory.CMGOCLGHNLH(Names[i]);
            if (owned == null || owned.OFOPFCJNEBL() != 1 ||
                owned.Node?["Enchantments"]?["Perk"]?.Attributes?["Name"]?.Value != Perks[i])
                throw new Exception("Titan reward did not settle into native inventory: " + Names[i]);
            inventory.EEDJEDBMIMI(ListSF.GetItems().GetItemByName(Names[i]), true);
        }
        if (roster.get_Parameters().DGMDEDKLGMB().Count(item => Names.Contains(item.Name)) != 5)
            throw new Exception("The five Titan items did not equip on the player.");
        // Load every shipped model through the production typed loader. The XML
        // parser is the base game's native geometry path, not DE128 Lua content.
        foreach (string model in new[] { "mdl_body_titan", "mdl_head_titan", "mdl_ranged_titans_harpoon", "mdl_magic_fireball" })
        {
            string text = ModRuntime.Host.TypedAssets.LoadModelText(AssetId.Parse("de128:models/titan/" + model));
            var document = new XmlDocument { XmlResolver = null };
            document.LoadXml(text);
            if (document.SelectSingleNode("/Scene/Figures") == null)
                throw new Exception("Titan model geometry has no native figures: " + model);
        }
        ListSF.GetInstance().OnAuthenticate(true);
    }

    static void Reload(Roster roster)
    {
        if (roster.Level != 52) throw new Exception("Titan reward player level did not survive reload.");
        var inventory = roster.KHCNHPCPFII();
        for (int i = 0; i < Names.Length; i++)
        {
            var owned = inventory.CMGOCLGHNLH(Names[i]);
            string aspectText = owned?.Node?["Enchantments"]?["Perk"]?["Set"]?.Attributes?["Aspect"]?.Value;
            double aspect;
            if (owned == null || owned.OFOPFCJNEBL() != 1 || !owned.EFMFGEPDAOP() ||
                owned.Node?["Enchantments"]?["Perk"]?.Attributes?["Name"]?.Value != Perks[i] ||
                !double.TryParse(aspectText, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out aspect) ||
                Math.Abs(aspect - 1952.28) > 0.0001)
                throw new Exception("Saved Titan item or enchantment did not reload: " + Names[i]);
        }
        if (roster.get_Parameters().DGMDEDKLGMB().Count(item => Names.Contains(item.Name)) != 5)
            throw new Exception("Saved Titan equipment did not project after reload.");
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        EditorApplication.Exit(code);
    }
}
