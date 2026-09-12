using System;
using System.IO;
using System.Linq;
using Eclipse.Modding;

static class Program
{
    static string root;
    static int count;
    static void Check(bool value, string message) { count++; if (!value) throw new Exception(message); }
    static ModDescriptor Mod(string id, string script, string capabilities = "\"content.patch\",\"content.register\"", string dependency = "core")
    {
        string dir = Path.Combine(root,id); Directory.CreateDirectory(Path.Combine(dir,"scripts"));
        File.WriteAllText(Path.Combine(dir,"mod.toml"), "schema=1\nid=\""+id+"\"\nname=\"Fixture\"\nversion=\"1.0.0\"\napi=\">=0.23 <1.0\"\nauthors=[\"Eclipse\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+capabilities+"]\n"+
            (dependency==null?"":"[[dependencies]]\nid=\""+dependency+"\"\nversion=\">=1.0 <2.0\"\n"));
        File.WriteAllText(Path.Combine(dir,"scripts/main.lua"),"local sf2=require('sf2')\n"+script);
        return ModDiscovery.DiscoverLoose(root).Mods.Single(m=>m.Id.Value==id);
    }
    static void Run(ModContentCatalog catalog,ModDescriptor mod)
    {
        using(var tx=catalog.BeginRegistration(mod))
        using(var context=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null)))
        { context.ExecuteEntrypoint(); tx.Commit(); }
    }
    static void Reject(ModContentCatalog catalog,ModDescriptor mod)
    {
        int patches=catalog.Patches.Count,quests=catalog.Quests.Count;
        bool failed=false;try{Run(catalog,mod);}catch(Exception){failed=true;}
        Check(failed,"Invalid suppression was accepted: "+mod.Id);
        Check(catalog.Patches.Count==patches && catalog.Quests.Count==quests,"Failed registration was not atomic");
    }
    static void Main(string[] args)
    {
        root=args[0];Directory.CreateDirectory(root);
        var catalog=new ModContentCatalog();
        int imported=CoreContentImporter.ImportQuestSources(catalog,args[1]);
        Check(imported>700,"Canonical quest source inventory was not imported");
        string target="core:quests/quest_extensions/energy.xml/energychecker";
        Check(catalog.TryGetQuestSource(DefinitionId.Parse(target),out var key) && key=="quest_extensions/energy.xml#EnergyChecker","Native source identity lost");
        string call="sf2.quests.suppress{target='"+target+"'}";
        var first=Mod("test.first",call);
        string before=ModSaveData.ComputeContentSetFingerprint(new[]{first},catalog);
        Run(catalog,first);
        Check(catalog.SuppressedQuestKeys.Single()==key,"Suppression did not reach host projection");
        Check(before!=ModSaveData.ComputeContentSetFingerprint(new[]{first},catalog),"Suppression missing from fingerprint");
        Reject(catalog,Mod("test.conflict","sf2.quests.register{id='pending',events={'session'},actions={{type='dialog',lines={'pending'}}}}\n"+call));
        Reject(new ModContentCatalog(),Mod("test.unknown",call));
        var clean=new ModContentCatalog();CoreContentImporter.ImportQuestSources(clean,args[1]);
        Reject(clean,Mod("test.duplicate",call+"\n"+call));
        Reject(clean,Mod("test.capability",call,"\"content.register\""));
        Reject(clean,Mod("test.dependency",call,"\"content.patch\"",null));
        Reject(clean,Mod("test.fields","sf2.quests.suppress{target='"+target+"',enabled=false}"));
        Run(clean,Mod("test.owned","sf2.quests.register{id='old',events={'session'},actions={{type='dialog',lines={'old'}}}}\nsf2.quests.suppress{target='test.owned:quests/old'}", "\"content.register\",\"content.patch\"",null));
        Check(clean.SuppressedQuestKeys.Single()=="test.owned#test.owned:quests/old","Owned quest source mismatch");
        var restored=new ModContentCatalog();CoreContentImporter.ImportQuestSources(restored,args[1]);
        Check(!restored.SuppressedQuestKeys.Any() && restored.TryGetQuestSource(DefinitionId.Parse(target),out var restoredKey) && restoredKey==key,"Disabled policy did not preserve base identity");
        var example=new ModContentCatalog();
        Run(example,Mod("example.quest-suppression",File.ReadAllText(args[2]),"\"content.register\",\"content.patch\"",null));
        Check(example.Quests.Count==2 && example.SuppressedQuestKeys.Single()=="example.quest-suppression#example.quest-suppression:quests/old_introduction","Shipped example did not suppress only its old quest");
        Console.WriteLine("PASS: "+count+" Lua/catalog suppression assertions; "+imported+" core source identities.");
    }
}
