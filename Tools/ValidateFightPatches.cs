using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

public static class Program
{
    sealed class HitFighter : IModFighterOperations, IModIncomingHitSource {
        public ModIncomingHit IncomingHit { get; set; }
        public bool TryChangeHealth(double n,out string e){e="";return true;}
        public bool TryAddMagicCharge(double n,out string e){e="";return true;}
    }
    static int checks;
    static string entry, manifest;
    const string target = "core:fights/test/trial/1";
    const string setup = "local sf2=require('sf2'); local b=sf2.behaviors.register {id='guard',on_round_begin=function() end}; local r=sf2.rules.behavior {id='guard',behavior=b}; ";
    static void Check(bool value,string message) { checks++; if(!value) throw new Exception(message); }
    static ModContentCatalog Catalog()
    {
        var content=new ModContentCatalog(); var doc=new XmlDocument();
        doc.LoadXml("<Stages><Zone Name='Test'><Battle Name='Trial' Type='TUTORIAL'><Fight Name='1' Music='1' Location='dojo' Power='7'><Warriors><Warrior Name='Original'/></Warriors><Rules><NoMagic/></Rules><Rewards><Reward Coins='123'/></Rewards></Fight></Battle></Zone></Stages>");
        CoreContentImporter.ImportStages(content,doc.DocumentElement); return content;
    }
    static bool Execute(string root,ModContentCatalog content,string lua,out string error,Action<IModInteractiveBehaviorScriptContext,ModContentCatalog> verify=null)
    {
        File.WriteAllText(entry,lua); var mod=ModDiscovery.DiscoverLoose(root).Mods.Single();
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        try {
            using(var tx=content.BeginRegistration(mod))
            using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null)))
            { script.ExecuteEntrypoint(); tx.Commit(); verify?.Invoke((IModInteractiveBehaviorScriptContext)script,content); }
            error=""; return true;
        } catch(Exception e) { error=e.Message; return false; }
    }
    static string Fingerprint(string root,ModContentCatalog content) => ModSaveData.ComputeContentSetFingerprint(ModDiscovery.DiscoverLoose(root).Mods,content);
    public static void Main(string[] args)
    {
        entry=Path.Combine(args[0],"example.battle-rules/scripts/main.lua");
        manifest=Path.Combine(args[0],"example.battle-rules/mod.toml");
        var originalManifest=File.ReadAllText(manifest);
        File.WriteAllText(manifest,originalManifest.Replace("\"content.register\"","\"content.register\", \"content.patch\""));
        string appendFingerprint=null;
        foreach(bool append in new[]{true,false})
        {
            var content=Catalog(); content.TryGetFight(DefinitionId.Parse(target),out var original);
            string baseline=Fingerprint(args[0],content);
            string field=append?"append_rules":"rules";
            Check(Execute(args[0],content,setup+"sf2.fights.patch {target='"+target+"',"+field+"={r},location='forest',music='6|7',rounds=3}",out var error),error);
            content.TryGetFight(original.Id,out var fight);
            Check(fight.Rules.Count==1 && fight.ReplacesLegacyRules==!append,"Rule patch mode lost");
            Check(fight.Location=="forest" && fight.Music=="6|7" && fight.Rounds==3,"Field patches lost");
            Check(fight.Power==7 && fight.Id==original.Id && fight.Battle==original.Battle && fight.LegacyName==original.LegacyName,"Identity or unrelated data changed");
            Check(original.Rules.Count==0 && original.Location=="dojo","Original definition mutated");
            var rules=new ModBattleRuleInstances();
            Check(rules.Applicable(content,content.RuntimeFightId(fight.Id),true,1,false).Count()==1,"Core fight Lua dispatch cannot find patched rule");
            string fingerprint=Fingerprint(args[0],content);
            Check(fingerprint!=baseline,"Patch absent from fingerprint");
            if(append) appendFingerprint=fingerprint; else Check(fingerprint!=appendFingerprint,"Append/replacement fingerprint collision");
            var doc=new XmlDocument(); doc.LoadXml("<Fight Name='1' Power='7'><Warriors><Warrior Name='Original'/></Warriors><Rules><NoMagic/></Rules><Rewards><Reward Coins='123'/></Rewards></Fight>");
            string originalXml=doc.OuterXml;
            var clone=(XmlElement)doc.DocumentElement.CloneNode(true);
            foreach(var patch in content.Patches)
                ModFightPatchProjection.Apply(clone,fight,patch.Field,content,_=>throw new Exception("Lua rule projected as XML"));
            Check((clone.SelectSingleNode("Rules/NoMagic")!=null)==append,"Native rules not preserved/replaced correctly");
            Check(clone.SelectSingleNode("Warriors/Warrior").Attributes["Name"].Value=="Original" && clone.SelectSingleNode("Rewards/Reward").Attributes["Coins"].Value=="123","Patch changed warriors/rewards");
            Check(clone.GetAttribute("Location")=="forest" && clone.GetAttribute("Music")=="6|7","Presentation not projected");
            Check(doc.OuterXml==originalXml,"Projection modified restoration source");
            Check(Execute(args[0],content,"local sf2=require('sf2'); sf2.fights.patch {target='"+target+"',round_time=121}",out error),"Independent field conflict: "+error);
            content.TryGetFight(original.Id,out var afterTime);
            Check(afterTime.ReplacesLegacyRules==!append && afterTime.Rules.Count==1 && afterTime.RoundTime==121,"Later field patch lost rule replacement intent");
            int count=content.Patches.Count;
            string committed=Fingerprint(args[0],content);
            Check(!Execute(args[0],content,"local sf2=require('sf2'); sf2.fights.patch {target='"+target+"',description='must rollback',music='8'}",out error),"Conflicting patch accepted");
            Check(content.Patches.Count==count && Fingerprint(args[0],content)==committed,"Conflict committed partial fields");
        }
        foreach(string fields in new[]{"rules={r},append_rules={r}","rules={r,r}","append_rules={}","rules={[2]=r}","rules={'fake'}","music=3","location=false","power=99","coins=999"})
        {
            var content=Catalog(); var baseline=Fingerprint(args[0],content);
            Check(!Execute(args[0],content,setup+"sf2.fights.patch {target='"+target+"',"+fields+"}",out var error),"Invalid field accepted: "+fields);
            Check(content.Patches.Count==0 && content.FightRules.Count==0 && Fingerprint(args[0],content)==baseline,"Invalid patch leaked transaction: "+fields);
        }
        var cleared=Catalog();
        Check(Execute(args[0],cleared,"local sf2=require('sf2'); sf2.fights.patch {target='"+target+"',rules={}}",out var clearError),clearError);
        cleared.TryGetFight(DefinitionId.Parse(target),out var empty);
        Check(empty.ReplacesLegacyRules && empty.Rules.Count==0,"Empty rules did not explicitly clear");
        var emptyXml=new XmlDocument();emptyXml.LoadXml("<Fight><Rules><NoMagic/></Rules></Fight>");
        ModFightPatchProjection.Apply(emptyXml.DocumentElement,empty,"fight/rules",cleared,_=>throw new Exception("Empty rule builder called"));
        Check(emptyXml.SelectSingleNode("Fight/Rules").ChildNodes.Count==0,"Empty replacement kept native rules");
        var native=Catalog();
        Check(Execute(args[0],native,"local sf2=require('sf2');local n=sf2.rules.no_perks {id='native'};sf2.fights.patch {target='"+target+"',append_rules={n}}",out var nativeError),nativeError);
        native.TryGetFight(DefinitionId.Parse(target),out var nativeFight);
        var nativeXml=new XmlDocument();nativeXml.LoadXml("<Fight><Rules><NoMagic/></Rules></Fight>");int built=0;
        ModFightPatchProjection.Apply(nativeXml.DocumentElement,nativeFight,"fight/rules",native,r=>{built++;return nativeXml.CreateElement("NativeFixture");});
        Check(built==1 && nativeXml.SelectSingleNode("Fight/Rules/NoMagic")!=null && nativeXml.SelectSingleNode("Fight/Rules/NativeFixture")!=null,"Static rule was not passed to native builder");
        File.WriteAllText(manifest,originalManifest);
        Check(!Execute(args[0],Catalog(),"local sf2=require('sf2'); sf2.fights.patch {target='"+target+"',music='8'}",out var denied),"Missing content.patch allowed");
        File.WriteAllText(manifest,originalManifest.Replace("\"content.register\"","\"content.register\", \"content.patch\""));
        var canonical = new ModContentCatalog(); var stages=new XmlDocument();
        stages.Load(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
        Check(CoreContentImporter.ImportStages(canonical,stages.SelectSingleNode("Stages/Zones"))>0,"Canonical stage fixture is empty");
        var originals=canonical.Fights.ToDictionary(f=>f.Id);
        Check(Execute(args[0],canonical,File.ReadAllText(Path.Combine(args[1],"Mods/example.core-fight/scripts/main.lua")),out var sampleError),sampleError);
        var sampleId=CoreContentImporter.FightId("ZONE_1","BOSS_LYNX","1");
        Check(canonical.Fights.Count==originals.Count,"Example added duplicate campaign fights");
        canonical.TryGetFight(sampleId,out var sampleFight);
        Check(sampleFight.Rules.Count==1 && !sampleFight.ReplacesLegacyRules && sampleFight.Location=="dojo","Core example patch missing");
        Check(canonical.Fights.Where(f=>f.Id!=sampleId).All(f=>ReferenceEquals(f,originals[f.Id])),"Unrelated canonical encounters changed");
        File.WriteAllText(manifest,File.ReadAllText(manifest).Replace("\"content.register\"","\"content.register\", \"combat.modify_outgoing_hit\""));
        var outgoingCatalog=new ModContentCatalog();CoreContentImporter.ImportStages(outgoingCatalog,stages.SelectSingleNode("Stages/Zones"));
        Check(Execute(args[0],outgoingCatalog,File.ReadAllText(Path.Combine(args[1],"Mods/example.outgoing-rule/scripts/main.lua")),out var outgoingError,(script,content)=>{
            var rule=content.FightRules.Single();var state=new XmlDocument();state.LoadXml("<Rule/>");
            for(int hit=1;hit<=7;hit++) {
                bool blocked=hit==2;int round=hit==7?2:1;double damage=10;
                var fighter=new HitFighter{IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n,blocked,false)};
                var fields=new System.Collections.Generic.Dictionary<string,string>{{"source","rule"},{"round",round.ToString()},{"fight_id","fixture"}};
                Check(script.TryInvokeBehavior(rule.Behavior,ModEffectEvent.DamageDealing,rule.InitialParameters,fields,new ModInstanceFighter(fighter,state.DocumentElement),out var callbackError),callbackError);
                Check(damage==(hit==4?20:10),"Third-hit rule ignored block/count/round semantics");
            }
        }),outgoingError);
        Console.WriteLine("PASS: "+checks+" fight patch checks: Lua validation, atomic conflicts, identity, fingerprint, native projection, core behavior dispatch and capability rejection.");
    }
}
