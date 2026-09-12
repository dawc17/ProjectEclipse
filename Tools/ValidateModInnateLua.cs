using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;

// Uses the compiled game ModRuntime and actual MoonSharp session; only physical
// health/magic operations are substituted. Fight source selection is checked by
// TestModFightBeginRuntime, not by this fixture.
static class Program
{
    sealed class Fighter : IModFighterOperations
    {
        public readonly List<double> Changes = new List<double>();
        public bool TryChangeHealth(double amount, out string error) { Changes.Add(amount); error = ""; return true; }
        public bool TryAddMagicCharge(double amount, out string error) { error = "unexpected magic"; return false; }
    }
    static int checks;
    static void Check(bool value, string error) { checks++; if (!value) throw new Exception(error); }
    static XmlElement Node(string name)
    {
        var document = new XmlDocument(); var node = document.CreateElement("Perk");
        document.AppendChild(node); node.SetAttribute("Name", name); return node;
    }
    static void Main(string[] args)
    {
        string folder = Path.Combine(args[0], "test.innate");
        Directory.CreateDirectory(Path.Combine(folder, "scripts"));
        Directory.CreateDirectory(Path.Combine(folder, "assets/sprites"));
        Directory.CreateDirectory(Path.Combine(folder, "assets/models"));
        // Metadata-only assets: this fixture never claims image/model decoding.
        File.WriteAllBytes(Path.Combine(folder,"assets/sprites/icon.png"),new byte[]{0});
        File.WriteAllText(Path.Combine(folder,"assets/models/model.xml"),"<Model/>");
        File.WriteAllText(Path.Combine(folder,"mod.toml"), "schema=1\nid=\"test.innate\"\nname=\"Innate execution\"\nversion=\"1.0.0\"\napi=\">=0.51 <1.0\"\nauthors=[\"Eclipse\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"content.register\",\"content.patch\",\"combat.change_life\"]\n[[dependencies]]\nid=\"core\"\nversion=\">=1.0 <2.0\"\n");
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"), @"
local sf2=require('sf2')
local title=sf2.localization.key('fixture')
local weapon=sf2.items.register_weapon{id='mace',display_name=title,
 icon=sf2.assets.sprite('sprites/icon'),model=sf2.assets.model('models/model'),
 subtype='TwoHandedBlunt',tactic_subtype='TwoHanded'}
sf2.shop.addItem{section=sf2.shop.WEAPONS,item=weapon,level=1,price=sf2.price.coins(1)}
local behavior=sf2.behaviors.register{
 id='counter', parameters={amount=sf2.behaviors.NUMBER},
 state={lifetime='fight',fields={hits={type=sf2.behaviors.INTEGER,required=true,default=0}}},
 on_round_begin=function(self,fighter)
  self.state.hits=self.state.hits+1
  fighter:change_health(self.params.amount*self.state.hits)
 end
}
local perk=sf2.perks.register{id='owned',display_name=title,description=title,
 kind=sf2.perks.SINGLE,behavior=behavior,parameters={amount=0.125}}
sf2.items.set_innate_perks{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={{perk=perk}}}
");
        var mod = ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog = new ModContentCatalog();
        var xml = new XmlDocument(); xml.LoadXml("<Items><Item Name='fixture_weapon' Type='Weapon' WeaponDamage='5'/></Items>");
        CoreContentImporter.ImportWeapons(catalog, xml.DocumentElement.ChildNodes.Cast<XmlNode>(), null);
        var state = new ModStateRuntime();
        var assets = new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        using (var tx = catalog.BeginRegistration(mod))
        using (var context = new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,state,null)))
        {
            tx.AddLocalization("fixture","eng","Fixture"); context.ExecuteEntrypoint(); tx.Commit(); catalog.Freeze();
            var adapter = new LegacyContentAdapter(catalog);
            var registeredWeapon = catalog.Weapons.Single(w=>!w.IsCore);
            var nativeNode = (XmlElement)typeof(LegacyContentAdapter).GetMethod("BuildItemNode",BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(adapter,new object[]{registeredWeapon,catalog.ShopListings.Single()});
            Check(nativeNode.GetAttribute("TacticSubtype")=="TwoHanded" && nativeNode.GetAttribute("SubType")=="TwoHandedBlunt","Native item builder lost independent AI classification");
            var nativeItem = new ItemInfo(null);
            typeof(ItemInfo).GetMethod("ReadCombatClassification",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(nativeItem,new object[]{nativeNode});
            Check((string)typeof(ItemInfo).GetProperty("EffectiveTacticSubtype",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nativeItem)=="TwoHanded","Projected item did not reach native classification");
            var session = (ModScriptSession)Activator.CreateInstance(typeof(ModScriptSession), BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[]{"MoonSharp",new List<IModScriptContext>{context},new[]{mod},Array.Empty<ModDiagnostic>(),catalog,state},null);
            var scripts = typeof(ModRuntime).GetField("_scripts",BindingFlags.Static | BindingFlags.NonPublic);
            object previous = scripts.GetValue(null);
            try
            {
                scripts.SetValue(null,session);
                string id = catalog.ItemInnatePerks.Single().Entries.Single().Perk.ToString();
                var first = Node(id); var second = Node(id); var fighter = new Fighter();
                var fields = new Dictionary<string,string>{{"side","player"},{"source","innate"},{"fight_id","fixture"},{"round","1"}};
                Check(ModRuntime.TryInvokeSavedPerkFightBegin(first,fields,fighter,out var error,ModEffectEvent.RoundBegin),error);
                Check(fighter.Changes.SequenceEqual(new[]{0.125}),"Registration-time parameter did not reach actual Lua");
                fields["round"]="2";
                Check(ModRuntime.TryInvokeSavedPerkFightBegin(first,fields,fighter,out error,ModEffectEvent.RoundBegin),error);
                Check(fighter.Changes.Last()==0.25,"Innate state was not retained across rounds");
                Check(ModRuntime.TryInvokeSavedPerkFightBegin(second,fields,fighter,out error,ModEffectEvent.RoundBegin),error);
                Check(fighter.Changes.Last()==0.125,"Detached instance leaked state to another fighter/fight");
                Check(!ModRuntime.TryInvokeSavedPerkFightBegin(Node("test.innate:perks/missing"),fields,fighter,out error,ModEffectEvent.RoundBegin),"Missing perk dispatched");
                Check(fighter.Changes.Count==3,"Missing perk mutated fighter");
            }
            finally { scripts.SetValue(null,previous); }
        }
        Console.WriteLine("Actual ModRuntime + Lua innate invocation: " + checks + " checks passed.");
    }
}
