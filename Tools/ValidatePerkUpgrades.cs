using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;

public sealed class Fighter : IModFighterOperations, IModIncomingHitSource {
    public ModIncomingHit IncomingHit { get; set; }
    public bool TryChangeHealth(double amount,out string error){error="";return true;}
    public bool TryAddMagicCharge(double amount,out string error){error="";return true;}
}
public static class Program {
    static int checks;
    static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
    static void Main(string[] args) {
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        var saved=new XmlDocument();saved.LoadXml("<Perk UpgradeLevel='0'/>");
        for(int reload=0;reload<2;reload++) {
            var catalog=new ModContentCatalog();
            using(var tx=catalog.BeginRegistration(mod)) {
                ModLocalizationLoader.Load(mod,assets,tx);
                using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null))) {
                    script.ExecuteEntrypoint();tx.Commit();
                    var perk=catalog.Perks.Single(); var behavior=catalog.Behaviors.Single();
                    Check(perk.Upgrades.Count==3,"Example upgrades missing");
                    Check(catalog.TryGetProgressionBranch(2,out var branch) && branch.Entries[0].Perk==perk.Id,"Example unlock branch missing");
                    if(reload==0) ModEffectSaveData.Write(saved.DocumentElement,perk.Id,behavior.Parameters,perk.InitialParameters);
                    for(int level=0;level<=3;level++) {
                        saved.DocumentElement.SetAttribute("UpgradeLevel",level.ToString());string before=saved.OuterXml;
                        Check(ModEffectSaveData.TryRead(saved.DocumentElement,perk.Id,behavior.Parameters,out var instance,out var readError),readError);
                        var effective=perk.ResolveSavedUpgradeParameters(saved.DocumentElement,instance.Values);
                        double damage=100;
                        var fighter=new Fighter{IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n)};
                        var fields=new Dictionary<string,string>{{"source","perk"},{"round","1"},{"fight_id","test"}};
                        Check(((IModInteractiveBehaviorScriptContext)script).TryInvokeBehavior(perk.Behavior,ModEffectEvent.DamageResolving,effective,fields,new ModInstanceFighter(fighter,saved.DocumentElement),out var error),error);
                        Check(Math.Abs(damage-(90-level*10))<0.000001,"Selected upgrade did not affect Lua damage");
                        Check(saved.OuterXml==before && Math.Abs(instance.Values["scale"].Number-0.9)<0.000001,"Callback persisted an upgrade override");
                    }
                }
            }
            // A disabled mod has no active script/catalog; its XML is retained and loaded again.
            var copy=new XmlDocument();copy.LoadXml(saved.OuterXml);saved=copy;
        }
        string entry=Path.Combine(mod.RootPath,"scripts/main.lua");
        // Test the actual public schema parser and transaction rollback.
        string original=File.ReadAllText(entry);
        foreach(string change in new[]{"level = 2, description = sf2.localization.key(\"first\")","level = 0, description = sf2.localization.key(\"first\")","level = 1, unknown = true, description = sf2.localization.key(\"first\")"}) {
            File.WriteAllText(entry,original.Replace("level = 1, description = sf2.localization.key(\"first\")",change));
            var catalog=new ModContentCatalog();bool rejected=false;
            using(var tx=catalog.BeginRegistration(mod)) {
                ModLocalizationLoader.Load(mod,assets,tx);
                using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null))) {
                    try {script.ExecuteEntrypoint();tx.Commit();}catch(ModScriptException){rejected=true;}
                }
            }
            Check(rejected && catalog.Perks.Count==0,"Invalid upgrade registration committed partial content");
        }
        File.WriteAllText(entry,original);
        Console.WriteLine("PASS: "+checks+" perk upgrade checks: real example Lua, saved parameter overlay, callback damage at four levels, reload/re-enable and invalid registration rollback.");
    }
}
