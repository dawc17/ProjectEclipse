using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Actual sensei_raid_charge_state.lua through the production profile.equipment
// binding, with controlled equipment snapshots. Not a fight or native perk test.
static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static XmlDocument Read(string path) { var doc = new XmlDocument(); doc.Load(path); return doc; }
    static void Main(string[] args)
    {
        string package = Path.Combine(args[0], "fixture.raid");
        File.WriteAllText(Path.Combine(package, "mod.toml"), """
schema = 1
id = "fixture.raid"
name = "Sensei RaidCharge availability checks"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.events", "profile.read"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
""");
        File.WriteAllText(Path.Combine(package, "scripts/main.lua"), """
local sf2=require("sf2")
local available=require("content.sensei_raid_charge_state").create()
sf2.story.on("scene_enter",function() sf2.log.info("raid:"..tostring(available())) end)
""");
        var mod = ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog = new ModContentCatalog();
        var items = Read(Path.Combine(args[1], "Assets/vanillaXml/list.xml")).SelectNodes("/List/Items/Item").Cast<XmlNode>().ToArray();
        CoreContentImporter.ImportWeapons(catalog, items, null);
        CoreContentImporter.ImportArmors(catalog, items, null);
        CoreContentImporter.ImportHelms(catalog, items, null);
        CoreContentImporter.ImportMagic(catalog, items, null);
        CoreContentImporter.ImportRanged(catalog, items, null);
        CoreContentImporter.ImportPerks(catalog, Read(Path.Combine(args[1], "Assets/vanillaXml/perks.xml")).DocumentElement.ChildNodes.Cast<XmlNode>());
        var logs = new List<string>();
        var bus = new ModStoryEvents((owner, message) => logs.Add("error:" + message));
        var equipment = new List<ModProfileEquipmentSnapshot>();
        ModProfileAccess.Equipment = () => equipment.AsReadOnly();
        using (var tx = catalog.BeginRegistration(mod))
        using (var context = new MoonSharpScriptRuntime(null, null, null, bus).CreateContext(mod,
            new ModApiFacade(mod, new AssetResolver(new IAssetProvider[] { new LooseModProvider(mod) }), tx, new ModStateRuntime(), entry => logs.Add(entry.Message))))
        {
            context.ExecuteEntrypoint(); tx.Commit(); bus.BindProfile();
            bool Query()
            {
                logs.Clear();
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene: "fight"));
                var line = logs.SingleOrDefault(value => value.StartsWith("raid:") || value.StartsWith("error:"));
                Check(line != null, "No reader output: " + string.Join("|", logs));
                Check(line == "raid:true" || line == "raid:false", "Reader failed: " + line);
                return line == "raid:true";
            }
            ModProfileEquipmentSnapshot Record(string item, bool owned, params string[] perks) =>
                new ModProfileEquipmentSnapshot(DefinitionId.Parse(item), new ModProfileItemSnapshot(true, owned ? 1 : 0, true, 0),
                    perks.Select(DefinitionId.Parse).ToArray());
            Check(!Query(), "Empty equipment made RaidCharge available");
            equipment.Add(Record("core:items/weapon/WEAPON_KATANA", true, "core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON"));
            Check(!Query(), "Ordinary enchantment made RaidCharge available");
            foreach (var perk in new[] { "PERK_SHADOW_CLOAK", "PERK_HERMITSTORM", "PERK_EARTHQUAKE", "PERK_WASPFLY", "PERK_TELEPORTATION",
                "PERK_ASSISTANTS", "PERK_RAT_WAVE", "PERK_WAR_WHIRL", "PERK_FEAR_RAY", "PERK_LIGHTING_CHAIN", "PERK_POWER_FIELD", "PERK_GRASP_OF_DARKNESS" })
            {
                equipment.Add(Record("core:items/armor/ARMOR_GREEN", true, "core:perks/" + perk));
                Check(Query(), "Ability enchantment did not enable RaidCharge: " + perk + " " + string.Join("|", logs));
                equipment.RemoveAt(equipment.Count - 1);
                equipment.Add(Record("core:items/armor/ARMOR_GREEN", false, "core:perks/" + perk));
                Check(!Query(), "Unowned record enabled RaidCharge: " + perk);
                equipment.RemoveAt(equipment.Count - 1);
            }
            var set = new[] { "weapon/WEAPON_C1_Z4_NEO_WANDERER", "armor/ARMOR_C1_Z4_NEO_WANDERER", "helm/HELM_C1_Z4_NEO_WANDERER",
                "ranged/RANGED_C1_Z4_NEO_WANDERER", "magic/MAGIC_C1_Z4_NEO_WANDERER" };
            equipment.Clear();
            for (int i = 0; i < set.Length; i++)
            {
                Check(!Query(), "Incomplete Neo Wanderer set enabled RaidCharge at " + i);
                equipment.Add(Record("core:items/" + set[i], true));
            }
            Check(Query(), "Complete Neo Wanderer set did not enable RaidCharge");
            ModProfileAccess.Equipment = () => null;
            logs.Clear();
            bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene: "fight"));
            Check(logs.Any(value => value.Contains("No active game profile")), "Missing profile did not raise: " + string.Join("|", logs));
        }
        ModProfileAccess.Clear();
        Console.WriteLine("PASS: " + checks + " Sensei RaidCharge availability checks; twelve ability enchantments, ownership, Neo Wanderer set completeness and missing profile through the actual Lua and binding. Controlled equipment snapshots; no native perk activation or fight.");
    }
}
