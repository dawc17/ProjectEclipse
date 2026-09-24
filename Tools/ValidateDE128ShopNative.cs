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

// Runs only in the independently copied project made by TestDE128ShopNative.py.
[InitializeOnLoad]
public static class ValidateDE128ShopNative
{
    const string Active = "Eclipse.DE128ShopNative.Active";
    const string Prefix = "[DE128ShopNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static double started;
    static bool campaign;
    static bool shopRequested;
    static double shopSelectedAt;
    static int previewStage;

    static ValidateDE128ShopNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-shop-fixture.marker")))
            throw new InvalidOperationException("The shop acceptance test requires an isolated project copy.");
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
            if (EditorApplication.timeSinceStartup - started > 300)
                throw new Exception("Timed out during actual game startup.");
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
            if (scripts == null || roster == null || Module.GetInstance() == null) return;
            var screen = Module.GetInstance().NMCNDOPKFJD();
            if (scripts.Diagnostics.Count != 0)
                throw new Exception("Mod initialization diagnostics: " + string.Join("; ", scripts.Diagnostics));
            string phase = Environment.GetEnvironmentVariable("ECLIPSE_DE128_SHOP_PHASE");
            if (phase == "shop_preview")
            {
                Preview(roster, screen);
                return;
            }
            if (screen != ScreenType.ModuleDojo && screen != ScreenType.ModuleMap) return;

            int count = CheckCatalog();
            if (phase == "buy") Buy(roster);
            else if (phase == "reload") Reload(roster);
            else if (phase == "equip_upgrade") EquipUpgrade(roster);
            else if (phase == "reload_equip_upgrade") ReloadEquipUpgrade(roster);
            else if (phase == "inspect") Inspect(roster);
            else throw new Exception("Unknown shop acceptance phase: " + phase);
            Debug.Log(Prefix + "PASS " + phase + ": " + count + " live catalog rows; native purchase, inventory, currency and save checks.");
            Finish(0);
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static int CheckCatalog()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        var baseItems = new XmlDocument { XmlResolver = null };
        var archived = new XmlDocument { XmlResolver = null };
        baseItems.Load(Path.Combine(root, "Assets/vanillaXml/list.xml"));
        archived.Load(Path.Combine(root, "Assets/DExml/list.xml"));
        var originals = baseItems.SelectNodes("/List/Items/Item").Cast<XmlElement>()
            .GroupBy(row => row.GetAttribute("Name")).ToDictionary(group => group.Key, group => group.First());
        int count = 0;
        foreach (var row in archived.SelectNodes("/List/Items/Item").Cast<XmlElement>()
            .GroupBy(item => item.GetAttribute("Name")).Select(group => group.First()))
        {
            if (!originals.TryGetValue(row.GetAttribute("Name"), out var source) ||
                source.GetAttribute("ShopHide") != "1" || row.GetAttribute("ShopHide") == "1") continue;
            if (!new[] { "Weapon", "Armor", "Helm", "Ranged", "Magic" }.Contains(row.GetAttribute("Type"))) continue;
            var item = ListSF.GetItems().GetItemByName(row.GetAttribute("Name"));
            if (item == null || !ModRuntime.Scripts.Content.TryResolveRuntimeItem(item.Name,
                    item.NodeXML == null ? null : item.NodeXML.OuterXml, out var id) ||
                !ModRuntime.Scripts.Content.TryGetItemAvailability(id, out var policy) ||
                policy.Visibility != ModItemVisibility.ForceVisible)
                throw new Exception("DE shop row lacks a live force-visible policy: " + row.GetAttribute("Name"));
            long expectedCoins = row.HasAttribute("Price") ? long.Parse(row.GetAttribute("Price")) : 0;
            long expectedGems = row.HasAttribute("BonusPrice") ? long.Parse(row.GetAttribute("BonusPrice")) : 0;
            if ((long)item.CoinPrice != expectedCoins)
                throw new Exception("Live coin price differs: " + item.Name);
            if ((long)item.GemPrice != expectedGems)
                throw new Exception("Live gem price differs: " + item.Name);
            count++;
        }
        if (count != 221) throw new Exception("Expected 221 live DE shop rows, found " + count);
        return count;
    }

    static void Buy(Roster roster)
    {
        roster.OIOOMAKNIOB(10000000);
        roster.LLNELLFMMBB(1000, Roster.HPOIJPGPOCF.CHANGE_INIT);
        var scythe = Item("WEAPON_HW15_SCYTHE");
        var armor = Item("ARMOR_ANNIVERSARY_10TH");
        var helm = Item("HELM_STARTER_PACK");
        if ((long)scythe.CoinPrice != 2550000 || (long)scythe.GemPrice != 97 ||
            (long)armor.GemPrice != 79 || (long)helm.GemPrice != 24 || helm.LocalUpgrades.Count != 0)
            throw new Exception("Representative archived prices or starter-helm upgrade profile changed.");
        if (!ItemBuyHelper.IHHKNBPKGHD(scythe) || roster.BFBOEGMAMNF() != 7450000 || roster.EHFJHFDACMP() != 1000)
            throw new Exception("Native dual-currency coin purchase failed.");
        if (!ItemBuyHelper.MGMAJHLAICA(armor) || roster.BFBOEGMAMNF() != 7450000 || roster.EHFJHFDACMP() != 921)
            throw new Exception("Native gem purchase failed.");
        if (!ItemBuyHelper.MGMAJHLAICA(helm) || roster.EHFJHFDACMP() != 897)
            throw new Exception("Native starter helm purchase failed.");
        foreach (var item in new[] { scythe, armor, helm })
        {
            var owned = roster.KHCNHPCPFII().CMGOCLGHNLH(item);
            if (owned == null || owned.OFOPFCJNEBL() != 1 || owned.IJGAOHJNLAH() > 0 ||
                owned.GetEnchantments().Count != item.DefaultEnchantments.Count)
                throw new Exception("Immediate native inventory/enchantment mismatch: " + item.Name);
        }
        roster.GGGEHAGCLGC(true);
    }

    static void Reload(Roster roster)
    {
        if (roster.BFBOEGMAMNF() != 7450000 || roster.EHFJHFDACMP() != 897)
            throw new Exception("Native saved balances were not reloaded.");
        foreach (string name in new[] { "WEAPON_HW15_SCYTHE", "ARMOR_ANNIVERSARY_10TH", "HELM_STARTER_PACK" })
        {
            var item = Item(name);
            var owned = roster.KHCNHPCPFII().CMGOCLGHNLH(item);
            if (owned == null || owned.OFOPFCJNEBL() != 1 || owned.IJGAOHJNLAH() > 0 ||
                owned.GetEnchantments().Count != item.DefaultEnchantments.Count)
                throw new Exception("Purchased item did not survive native save/reload: " + name);
        }
    }

    static void EquipUpgrade(Roster roster)
    {
        roster.Level = 50;
        var warlock = Item("ARMOR_C4_Z1_WARLOCK");
        var scythe = Item("WEAPON_HW15_SCYTHE");
        var helm = Item("HELM_STARTER_PACK");
        Debug.Log(Prefix + "Warlock before purchase: model=" + warlock.ModelFileName +
            " gemPrice=" + (long)warlock.GemPrice + " gems=" + roster.EHFJHFDACMP() +
            " owned=" + (roster.KHCNHPCPFII().CMGOCLGHNLH(warlock)?.OFOPFCJNEBL() ?? 0));
        var existingWarlock = roster.KHCNHPCPFII().CMGOCLGHNLH(warlock);
        if (warlock.ModelFileName != "core:gamedata/models/mdl_armor_super_cloak_no_spikes" ||
            (existingWarlock == null && !ItemBuyHelper.MGMAJHLAICA(warlock)) ||
            roster.EHFJHFDACMP() != 853 ||
            roster.KHCNHPCPFII().CMGOCLGHNLH(warlock)?.OFOPFCJNEBL() != 1)
            throw new Exception("Changed-model armor did not purchase through the native gem path.");
        var inventory = roster.KHCNHPCPFII();
        inventory.EEDJEDBMIMI(scythe, true);
        inventory.EEDJEDBMIMI(warlock, true);
        if (!inventory.CMGOCLGHNLH(scythe).EFMFGEPDAOP() ||
            !inventory.CMGOCLGHNLH(warlock).EFMFGEPDAOP() ||
            !roster.get_Parameters().DGMDEDKLGMB().Any(item => item.Name == scythe.Name) ||
            !roster.get_Parameters().DGMDEDKLGMB().Any(item => item.Name == warlock.Name))
            throw new Exception("Native equip did not project archived weapon and changed-model armor.");

        var ownedHelm = inventory.CMGOCLGHNLH(helm);
        // Native upgrades target the current player level. A level-50 test
        // profile would select the level-50 milestone (18 trillion coins), so
        // exercise the first available level-7 upgrade then restore level 50.
        roster.Level = 7;
        ownedHelm.CDFODJBJIPI(roster.Level);
        var upgrade = ownedHelm.HADDPFNDPDG();
        if (upgrade == null || upgrade.UpgradeLevel <= ownedHelm.DHNNCAEEMLL() ||
            (long)upgrade.CoinPrice <= 0)
            throw new Exception("Starter helm has no valid next shared-template upgrade.");
        roster.OIOOMAKNIOB(100000000);
        long price = (long)upgrade.CoinPrice;
        int level = upgrade.UpgradeLevel;
        if (!ItemBuyHelper.APICBINEPGJ(helm) || roster.BFBOEGMAMNF() != 100000000 - price ||
            ownedHelm.DHNNCAEEMLL() != level || ownedHelm.IJGAOHJNLAH() > 0)
            throw new Exception("Native starter-helm coin upgrade did not settle immediately.");
        roster.Level = 50;
        ownedHelm.CDFODJBJIPI(roster.Level);
        roster.GGGEHAGCLGC(true);
        // Ordinary upgrades mark the profile dirty; the game's next save tick
        // writes it. Force that native save before terminating this short run.
        ListSF.GetInstance().OnAuthenticate(true);
        string root = Directory.GetParent(Application.dataPath).FullName;
        File.WriteAllText(Path.Combine(root, "de128-shop-expected.txt"),
            roster.BFBOEGMAMNF() + "\n" + level + "\n");
    }

    static void Inspect(Roster roster)
    {
        var warlock = Item("ARMOR_C4_Z1_WARLOCK");
        var helm = roster.KHCNHPCPFII().CMGOCLGHNLH(Item("HELM_STARTER_PACK"));
        if (helm != null) helm.CDFODJBJIPI(roster.Level);
        var nextHelm = helm?.HADDPFNDPDG();
        Debug.Log(Prefix + "INSPECT: level=" + roster.Level + " coins=" + roster.BFBOEGMAMNF() +
            " gems=" + roster.EHFJHFDACMP() + " warlockModel=" + warlock.ModelFileName +
            " warlockGemPrice=" + (long)warlock.GemPrice +
            " warlockOwned=" + (roster.KHCNHPCPFII().CMGOCLGHNLH(warlock)?.OFOPFCJNEBL() ?? 0) +
            " helmUpgrade=" + (helm?.DHNNCAEEMLL() ?? -1) +
            " helmDelivery=" + (helm?.IJGAOHJNLAH() ?? -1) +
            " nextUpgrade=" + (nextHelm?.UpgradeLevel ?? -1) +
            " nextCoinPrice=" + (nextHelm == null ? -1 : (long)nextHelm.CoinPrice));
    }

    static void ReloadEquipUpgrade(Roster roster)
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string[] expected = File.ReadAllLines(Path.Combine(root, "de128-shop-expected.txt"));
        if (roster.Level != 50 || roster.BFBOEGMAMNF() != long.Parse(expected[0]) ||
            roster.EHFJHFDACMP() != 853)
            throw new Exception("Native saved level/currency after equip and upgrade did not reload: level=" +
                roster.Level + " coins=" + roster.BFBOEGMAMNF() + " gems=" + roster.EHFJHFDACMP() +
                " expectedCoins=" + expected[0]);
        var inventory = roster.KHCNHPCPFII();
        foreach (string name in new[] { "WEAPON_HW15_SCYTHE", "ARMOR_C4_Z1_WARLOCK" })
        {
            var item = Item(name);
            var owned = inventory.CMGOCLGHNLH(item);
            if (owned == null || owned.OFOPFCJNEBL() != 1 || !owned.EFMFGEPDAOP() ||
                !roster.get_Parameters().DGMDEDKLGMB().Any(equipped => equipped.Name == name))
                throw new Exception("Equipped archived item did not survive native reload: " + name);
        }
        var helm = inventory.CMGOCLGHNLH(Item("HELM_STARTER_PACK"));
        if (helm == null || helm.DHNNCAEEMLL() != int.Parse(expected[1]) || helm.IJGAOHJNLAH() > 0)
            throw new Exception("Starter helm upgrade did not survive native reload.");
    }

    static void Preview(Roster roster, ScreenType screen)
    {
        if (!shopRequested)
        {
            if (screen != ScreenType.ModuleDojo && screen != ScreenType.ModuleMap) return;
            if (roster.Level != 50) throw new Exception("Shop preview requires the saved high-level test profile.");
            roster.AddShopLock("ACT_4");
            Module.DLOKJOHNDID(ScreenType.ModuleShop, null, null, false);
            shopRequested = true;
            return;
        }
        if (screen != ScreenType.ModuleShop) return;
        var scene = UnityEngine.Object.FindObjectOfType<Nekki.SF2.GUI.Shop.ShopScene>();
        if (scene == null) return;
        var armor = (System.Collections.Generic.List<ItemInfo>)typeof(Nekki.SF2.GUI.Shop.ShopScene)
            .GetField("KBMOJAPFLAO", Hidden).GetValue(scene);
        if (armor.Count == 0) return;
        var warlock = Item("ARMOR_C4_Z1_WARLOCK");
        if (!ShopAvailabilityPolicy.IsAvailable(warlock, roster) || !armor.Contains(warlock))
            throw new Exception("The live shop did not list the visible changed-model armor.");
        var weapons = (System.Collections.Generic.List<ItemInfo>)typeof(Nekki.SF2.GUI.Shop.ShopScene)
            .GetField("DOHLAAPAOOO", Hidden).GetValue(scene);
        var wakizashi = Item("WEAPON_WAKIDZASHI");
        if (!ShopAvailabilityPolicy.IsAvailable(wakizashi, roster) || !weapons.Contains(wakizashi))
            throw new Exception("The live shop did not list the changed-icon weapon after its act gate.");
        var table = (TableView)typeof(Nekki.SF2.GUI.Shop.ShopScene)
            .GetField("_shopTableView", Hidden).GetValue(scene);
        if (previewStage == 0)
        {
            scene.SetShopSection(ShopSection.Armor);
            table.ScrollToCell(armor.IndexOf(warlock));
            shopSelectedAt = EditorApplication.timeSinceStartup;
            previewStage = 1;
            return;
        }
        var selected = table.get_SelectedCell() as Nekki.SF2.GUI.Shop.ShopTableViewCell;
        var expected = previewStage == 1 ? warlock : wakizashi;
        if (selected?.get_ItemInfo()?.Name != expected.Name)
        {
            int targetRow = previewStage == 1 ? armor.IndexOf(warlock) : weapons.IndexOf(wakizashi);
            table.ScrollToCell(targetRow);
            if (EditorApplication.timeSinceStartup - shopSelectedAt < 8) return;
            bool scrolling = (bool)typeof(TableView).GetField("BKJCHFPNIIB", Hidden).GetValue(table);
            throw new Exception("The live shop did not select " + expected.Name + ": " +
                selected?.get_ItemInfo()?.Name + "; targetRow=" + targetRow +
                "; rows=" + table.NumberOfRows() + "; scrolling=" + scrolling +
                "; position=" + table.get_Position());
        }
        var icon = (Nekki.SF2.GUI.ResolutionImage)typeof(Nekki.SF2.GUI.Shop.ShopTableViewCell)
            .GetField("_image", Hidden).GetValue(selected);
        if (icon == null || icon.sprite == null)
            throw new Exception("The live shop row did not render its item icon.");
        if (previewStage == 1)
        {
            scene.SetShopSection(ShopSection.Weapon);
            table.ScrollToCell(weapons.IndexOf(wakizashi));
            shopSelectedAt = EditorApplication.timeSinceStartup;
            previewStage = 2;
            return;
        }
        if (!string.Equals(icon.sprite.name, "weapon_wakidzashi", StringComparison.OrdinalIgnoreCase))
            throw new Exception("The changed-icon weapon rendered the wrong sprite: " + icon.sprite.name);
        Debug.Log(Prefix + "PASS shop_preview: real ShopScene listed and selected Warlock armor and Wakizashi; the changed icon rendered.");
        Finish(0);
    }

    static ItemInfo Item(string name) => ListSF.GetItems().GetItemByName(name) ??
        throw new Exception("Missing shop item: " + name);

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        EditorApplication.Exit(code);
    }
}
