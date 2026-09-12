using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks;
    const string Exclusion = "sf2.forge.exclude_candidate{profile=sf2.forge.profile('Complex'),perk=sf2.perks.get('core:perks/PERK_MONK_SET_WHIRL'),equipment='weapon'}";
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static ModContentCatalog Catalog()
    {
        var catalog = new ModContentCatalog();
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[]{"Complex","Simple"});
        var xml = new XmlDocument(); xml.LoadXml("<Perks><Perk Name='PERK_MONK_SET_WHIRL'/></Perks>");
        CoreContentImporter.ImportPerks(catalog, xml.DocumentElement.ChildNodes.Cast<XmlNode>());
        xml.LoadXml("<Items><Item Name='fixture_weapon' Type='Weapon' WeaponDamage='5'/></Items>");
        CoreContentImporter.ImportWeapons(catalog,xml.DocumentElement.ChildNodes.Cast<XmlNode>(),null);
        return catalog;
    }
    static void Load(string root, ModContentCatalog catalog, string id, string script, bool patch = true)
    {
        string parent = Path.Combine(root, id), folder = Path.Combine(parent, id);
        Directory.CreateDirectory(Path.Combine(folder,"scripts"));
        File.WriteAllText(Path.Combine(folder,"mod.toml"),
            "schema = 1\nid = \""+id+"\"\nname = \"Forge test\"\nversion = \"1.0.0\"\napi = \">=0.52 <1.0\"\nauthors = [\"Eclipse\"]\nentrypoint = \"scripts/main.lua\"\ncapabilities = [\"content.register\""+(patch?", \"content.patch\"":"")+"]\n[[dependencies]]\nid = \"core\"\nversion = \">=1.0 <2.0\"\n");
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"), "local sf2=require('sf2')\n"+script);
        Directory.CreateDirectory(Path.Combine(folder,"assets/sprites"));
        Directory.CreateDirectory(Path.Combine(folder,"assets/models"));
        File.WriteAllBytes(Path.Combine(folder,"assets/sprites/icon.png"),new byte[]{0});
        File.WriteAllText(Path.Combine(folder,"assets/models/model.xml"),"<Model/>");
        var mod = ModDiscovery.DiscoverLoose(parent).Mods.Single();
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null)))
        { tx.AddLocalization("fixture","eng","Fixture"); context.ExecuteEntrypoint(); tx.Commit(); }
    }
    static void Reject(Action action, string expected)
    {
        Exception failure=null;
        try { action(); } catch(Exception error) { failure=error; }
        Check(failure!=null,"Expected rejection: "+expected);
        Check(failure.ToString().IndexOf(expected,StringComparison.OrdinalIgnoreCase)>=0,"Wrong failure: "+failure);
    }
    static void Main(string[] args)
    {
        string root=args[0];
        var catalog=Catalog();
        Load(root,catalog,"test.first",Exclusion);
        Check(catalog.ForgeCandidateExclusions.Count==1,"Exclusion not committed");
        var record=catalog.ForgeCandidateExclusions.Single();
        Check(record.Owner.Value=="test.first" && record.Equipment==ModEquipmentKind.Weapon,"Owner/category lost");
        Check(record.Profile==DefinitionId.Parse("core:forge-profiles/Complex") && record.Perk==DefinitionId.Parse("core:perks/PERK_MONK_SET_WHIRL"),"Targets changed");
        Reject(()=>Load(root,catalog,"test.conflict",Exclusion.Replace("'weapon'","'armor'")+"\n"+Exclusion),"test.first");
        Check(catalog.ForgeCandidateExclusions.Count==1,"Conflicting transaction partially committed");
        Load(root,catalog,"test.other",Exclusion.Replace("'weapon'","'armor'"));
        Check(catalog.ForgeCandidateExclusions.Count==2,"Independent equipment conflicts");
        var invalid=Catalog();
        Reject(()=>Load(root,invalid,"test.duplicate",Exclusion+"\n"+Exclusion),"Duplicate");
        Reject(()=>Load(root,invalid,"test.denied",Exclusion,false),"content.patch");
        Reject(()=>Load(root,invalid,"test.category",Exclusion.Replace("'weapon'","'invalid'")),"equipment");
        Reject(()=>Load(root,invalid,"test.field",Exclusion.Replace("equipment=","typo=1,equipment=")),"typo");
        Reject(()=>Load(root,invalid,"test.handle",Exclusion.Replace("sf2.forge.profile('Complex')","{}")),"profile");
        Reject(()=>Load(root,invalid,"test.missing",Exclusion.Replace("'Complex'","'Absent'")),"Absent");
        Check(invalid.ForgeCandidateExclusions.Count==0,"Failed loading published exclusions");
        Load(root,invalid,"test.retry",Exclusion);
        Check(invalid.ForgeCandidateExclusions.Count==1,"Rollback prevented retry");
        const string deviation="sf2.forge.override_deviation{profile=sf2.forge.profile('Simple'),equipment='weapon',minimum=15,maximum=75}";
        var changed=Catalog();
        Load(root,changed,"test.deviation",deviation);
        Check(changed.ForgeDeviations.Single().Minimum==15 && changed.ForgeDeviations.Single().Maximum==75,"Deviation bounds lost");
        Reject(()=>Load(root,changed,"test.dconflict",deviation.Replace("'weapon'","'armor'")+"\n"+deviation),"test.deviation");
        Check(changed.ForgeDeviations.Count==1,"Deviation conflict partially committed");
        var rejected=Catalog();
        Reject(()=>Load(root,rejected,"test.ddenied",deviation,false),"content.patch");
        Reject(()=>Load(root,rejected,"test.dduplicate",deviation+"\n"+deviation),"Duplicate");
        Reject(()=>Load(root,rejected,"test.dbounds",deviation.Replace("minimum=15","minimum=76")),"minimum");
        Reject(()=>Load(root,rejected,"test.dlower",deviation.Replace("minimum=15","minimum=-10001")),"minimum");
        Reject(()=>Load(root,rejected,"test.dupper",deviation.Replace("maximum=75","maximum=10001")),"maximum");
        Reject(()=>Load(root,rejected,"test.dfraction",deviation.Replace("minimum=15","minimum=1.5")),"integer");
        Reject(()=>Load(root,rejected,"test.dfield",deviation.Replace("minimum=15","minimum=15,price=0")),"price");
        Check(rejected.ForgeDeviations.Count==0,"Invalid deviation published");
        string Hash(ModContentCatalog value)=>ModSaveData.ComputeContentSetFingerprint(Array.Empty<ModDescriptor>(),value);
        var different=Catalog(); Load(root,different,"test.deviation",deviation.Replace("maximum=75","maximum=76"));
        Check(Hash(changed)!=Hash(different),"Changed bound missing from compatibility hash");
        var orderA=Catalog(); var orderB=Catalog();
        Load(root,orderA,"test.order",deviation+"\n"+deviation.Replace("'weapon'","'armor'"));
        Load(root,orderB,"test.order",deviation.Replace("'weapon'","'armor'")+"\n"+deviation);
        Check(Hash(orderA)==Hash(orderB),"Registration order changed equivalent content hash");
        Check(Hash(changed)!=Hash(Catalog()),"Overlay absent from compatibility hash");
        const string loadout="sf2.items.set_default_enchantments{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={{perk=sf2.perks.get('core:perks/PERK_MONK_SET_WHIRL'),aspect=123}}}";
        var equipped=Catalog(); Load(root,equipped,"test.loadout",loadout);
        Check(equipped.ItemDefaultEnchantments.Single().Entries.Single().Aspect==123,"Lua loadout aspect lost");
        Reject(()=>Load(root,equipped,"test.lconflict",loadout),"test.loadout");
        Check(equipped.ItemDefaultEnchantments.Count==1,"Conflicting Lua loadout published");
        var invalidLoadout=Catalog();
        Reject(()=>Load(root,invalidLoadout,"test.ldenied",loadout,false),"content.patch");
        Reject(()=>Load(root,invalidLoadout,"test.lfraction",loadout.Replace("aspect=123","aspect=1.5")),"integer");
        Reject(()=>Load(root,invalidLoadout,"test.lfield",loadout.Replace("aspect=123","aspect=123,raw_xml='bad'")),"raw_xml");
        Reject(()=>Load(root,invalidLoadout,"test.lsparse",loadout.Replace("entries={{","entries={[2]={")),"array");
        Reject(()=>Load(root,invalidLoadout,"test.lforged",loadout.Replace("sf2.items.get('core:items/weapon/fixture_weapon')","{}")),"item");
        Check(invalidLoadout.ItemDefaultEnchantments.Count==0,"Invalid loadout published");
        var empty=Catalog(); Load(root,empty,"test.lempty","sf2.items.set_default_enchantments{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={}}");
        Check(empty.ItemDefaultEnchantments.Single().Entries.Count==0,"Empty Lua loadout rejected");
        var omitted=Catalog(); Load(root,omitted,"test.lomitted",loadout.Replace(",aspect=123",""));
        Check(!omitted.ItemDefaultEnchantments.Single().Entries.Single().Aspect.HasValue,"Omitted aspect invented a value");
        const string innate="sf2.items.set_innate_perks{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={{perk=sf2.perks.get('core:perks/PERK_MONK_SET_WHIRL'),parameters={Aspect=100,Power=0.5}}}}";
        var permanent=Catalog(); Load(root,permanent,"test.innate",innate+"\n"+loadout);
        Check(permanent.ItemInnatePerks.Count==1 && permanent.ItemDefaultEnchantments.Count==1,"Innate/default Lua composition failed");
        Check(permanent.ItemInnatePerks.Single().Entries.Single().Parameters["Power"]==0.5f,"Numeric innate parameters changed");
        Reject(()=>Load(root,permanent,"test.iconflict",innate),"test.innate");
        var badInnate=Catalog();
        Reject(()=>Load(root,badInnate,"test.idenied",innate,false),"content.patch");
        Reject(()=>Load(root,badInnate,"test.iduplicate",innate+"\n"+innate),"Duplicate");
        Reject(()=>Load(root,badInnate,"test.iexpression",innate.Replace("Power=0.5","Power='?Player[Level]'")),"numeric");
        Reject(()=>Load(root,badInnate,"test.iinfinite",innate.Replace("Power=0.5","Power=1/0")),"finite");
        Reject(()=>Load(root,badInnate,"test.inan",innate.Replace("Power=0.5","Power=0/0")),"finite");
        Reject(()=>Load(root,badInnate,"test.iname",innate.Replace("Power=0.5","['bad key']=0.5")),"identifiers");
        Reject(()=>Load(root,badInnate,"test.isparse",innate.Replace("entries={{","entries={[2]={")),"array");
        Check(badInnate.ItemInnatePerks.Count==0,"Invalid innate loadouts published");
        var noInnate=Catalog(); Load(root,noInnate,"test.iempty","sf2.items.set_innate_perks{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={}}");
        Check(noInnate.ItemInnatePerks.Single().Entries.Count==0,"Empty innate list rejected");
        var noParams=Catalog(); Load(root,noParams,"test.idefault",innate.Replace(",parameters={Aspect=100,Power=0.5}",""));
        Check(noParams.ItemInnatePerks.Single().Entries.Single().Parameters.Count==0,"Omitted parameters invented values");
        var reordered=Catalog(); Load(root,reordered,"test.innate",innate.Replace("Aspect=100,Power=0.5","Power=0.5,Aspect=100")+"\n"+loadout);
        Check(Hash(permanent)==Hash(reordered),"Parameter insertion order affected compatibility");
        const string scripted="local title=sf2.localization.key('fixture'); local behavior=sf2.behaviors.register{id='brain',on_round_begin=function(self,fighter) end}; local perk=sf2.perks.register{id='owned',display_name=title,description=title,kind=sf2.perks.SINGLE,behavior=behavior}; ";
        const string attach="sf2.items.set_innate_perks{item=sf2.items.get('core:items/weapon/fixture_weapon'),entries={{perk=perk}}}";
        var owned=Catalog(); Load(root,owned,"test.iowned",scripted+attach);
        Check(owned.ItemInnatePerks.Single().Entries.Single().Perk.Namespace.Value=="test.iowned","Pending Lua-backed perk did not attach");
        var badParameters=Catalog();
        Reject(()=>Load(root,badParameters,"test.iparams",scripted+attach.Replace("perk=perk","perk=perk,parameters={Aspect=10}")),"registering the perk");
        Check(badParameters.ItemInnatePerks.Count==0 && !badParameters.Perks.Any(p=>!p.IsCore),"Rejected Lua-backed parameters partially published definitions");
        const string weaponScript="sf2.items.register_weapon{id='mace',display_name=sf2.localization.key('fixture'),icon=sf2.assets.sprite('sprites/icon'),model=sf2.assets.model('models/model'),subtype='TwoHandedBlunt',tactic_subtype='TwoHanded'}";
        var grouped=Catalog(); Load(root,grouped,"test.group",weaponScript);
        var weapon=grouped.Weapons.Single(w=>!w.IsCore);
        Check(weapon.SubType=="TwoHandedBlunt" && weapon.TacticSubtype=="TwoHanded","Lua tactic group changed physical subtype or was lost");
        var groupChanged=Catalog(); Load(root,groupChanged,"test.group",weaponScript.Replace("tactic_subtype='TwoHanded'","tactic_subtype='Staff'"));
        Check(Hash(grouped)!=Hash(groupChanged),"Tactic group missing from compatibility fingerprint");
        var defaultGroup=Catalog(); Load(root,defaultGroup,"test.group",weaponScript.Replace(",tactic_subtype='TwoHanded'",""));
        Check(defaultGroup.Weapons.Single(w=>!w.IsCore).TacticSubtype==null,"Omitted group did not preserve native fallback");
        var badGroup=Catalog();
        foreach (string value in new[]{"''","'a b'","'x/y'","123","true","'"+new string('x',129)+"'"})
            Reject(()=>Load(root,badGroup,"test.badgroup",weaponScript.Replace("'TwoHanded'",value)),"tactic_subtype");
        Check(!badGroup.Weapons.Any(w=>!w.IsCore),"Invalid group partially published weapon");
        const string overrideGroup="sf2.items.set_tactic_subtype{item=sf2.items.get('core:items/weapon/fixture_weapon'),group='Katars'}";
        var patchedGroup=Catalog(); Load(root,patchedGroup,"test.patchgroup",overrideGroup);
        Check(patchedGroup.ItemTacticSubtypes.Single().Group=="Katars","Core tactic group not committed");
        Reject(()=>Load(root,patchedGroup,"test.groupconflict",overrideGroup),"already patched");
        var invalidPatch=Catalog();
        Reject(()=>Load(root,invalidPatch,"test.groupdenied",overrideGroup,false),"content.patch");
        Reject(()=>Load(root,invalidPatch,"test.groupdup",overrideGroup+"\n"+overrideGroup),"Duplicate");
        foreach (string value in new[]{"nil","true","12","'a b'","'"+new string('x',129)+"'"})
            Reject(()=>Load(root,invalidPatch,"test.groupbad",overrideGroup.Replace("group='Katars'","group="+value)),"group");
        Check(invalidPatch.ItemTacticSubtypes.Count==0,"Invalid group patch partially committed");
        var fallback=Catalog(); Load(root,fallback,"test.patchgroup",overrideGroup.Replace("'Katars'","''"));
        Check(fallback.ItemTacticSubtypes.Single().Group=="","Explicit subtype fallback rejected");
        Check(Hash(fallback)!=Hash(patchedGroup),"Changed override group missing from compatibility hash");
        var ownedPatch=Catalog(); Load(root,ownedPatch,"test.ownedpatch",weaponScript.Replace("sf2.items.register_weapon{","local item=sf2.items.register_weapon{")+"\nsf2.items.set_tactic_subtype{item=item,group='Staff'}");
        Check(ownedPatch.ItemTacticSubtypes.Single().Item.Namespace.Value=="test.ownedpatch","Pending owned weapon override rejected");
        Console.WriteLine("PASS: "+checks+" actual Lua forge/loadout registration and compatibility checks.");
    }
}
