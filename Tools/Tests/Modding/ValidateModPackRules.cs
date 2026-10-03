using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
namespace Eclipse.Modding
{
    public sealed class ModHost
    {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public AssetResolver Assets { get; }
        public ModHost(IEnumerable<ModDescriptor> mods)
        {
            var order=DependencyResolver.Resolve(mods,ModPlatformVersions.Core);
            if(order.Diagnostics.Any(d=>d.Severity==ModDiagnosticSeverity.Error))throw new Exception(string.Join("; ",order.Diagnostics));
            EnabledMods=order.OrderedMods;Assets=new AssetResolver(EnabledMods.Select(mod=>(IAssetProvider)new LooseModProvider(mod)));
        }
    }
}
static class Program
{
    static int checks;
    static string root,repo;
    const string Target="core:fights/test/trial/1";
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static void Import(ModContentCatalog content)
    {
        var xml=new XmlDocument();xml.LoadXml("<Stages><Zone Name='Test'><Battle Name='Trial' Type='TUTORIAL'><Fight Name='1' Music='1' Location='dojo' Power='7'><Warriors><Warrior Name='Original'/></Warriors><Rules><NoMagic/></Rules><Rewards><Reward Coins='123'/></Rewards></Fight></Battle></Zone></Stages>");
        CoreContentImporter.ImportStages(content,xml.DocumentElement);
    }
    static ModDescriptor Write(string folder,string id,string body,params string[] dependencies)
    {
        var path=Path.Combine(folder,id);Directory.CreateDirectory(Path.Combine(path,"scripts"));
        File.WriteAllText(Path.Combine(path,"scripts/main.lua"),"local sf2=require('sf2');"+body);
        File.WriteAllText(Path.Combine(path,"mod.toml"),"schema=1\nid=\""+id+"\"\nname=\"Fixture\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"content.register\",\"content.patch\",\"combat.round_outcome\"]\n"+
            string.Concat(new[]{"core"}.Concat(dependencies).Select(dep=>"[[dependencies]]\nid=\""+dep+"\"\nversion=\">=1.0.0 <2.0.0\"\n")));
        return ModDiscovery.DiscoverLoose(folder).Mods.Single(mod=>mod.Id.Value==id);
    }
    static string Rule(string id,bool controller=false,string filters="")=>"local b=sf2.behaviors.register{id='"+id+"',on_tick=function() end};local r=sf2.rules.behavior{id='"+id+"',behavior=b,controls_outcome="+(controller?"true":"false")+filters+"};";
    static string Patch(bool append=true)=>"sf2.fights.patch{target='"+Target+"',"+(append?"append_rules":"rules")+"={r}};";
    static ModScriptSession Start(string folder,out List<ModUiSurface> surfaces,Action<ModContentCatalog> import=null,bool reverse=false)
    {
        var descriptors=ModDiscovery.DiscoverLoose(folder).Mods.ToArray();if(reverse)Array.Reverse(descriptors);
        surfaces=new List<ModUiSurface>();return ModScriptSession.Start(new ModHost(descriptors),new MoonSharpScriptRuntime(surfaces.Add),null,import??Import);
    }
    static void Main(string[] args)
    {
        root=args[0];repo=args[1];
        var pack=Path.Combine(root,"ordered");Write(pack,"pack.z",Rule("z")+Patch());Write(pack,"pack.a",Rule("a")+Patch(),"pack.z");Write(pack,"pack.m",Rule("m")+Patch());
        string fingerprint=null;
        foreach(bool reverse in new[]{false,true})
        {
            using var session=Start(pack,out _,reverse:reverse);Check(!session.HasErrors,session.FormatReport());
            session.Content.TryGetFight(DefinitionId.Parse(Target),out var fight);
            var expected=session.ActiveMods.Select(mod=>DefinitionId.Parse(mod.Id+":rules/"+mod.Id.Value.Substring(5))).ToArray();
            Check(fight.Rules.SequenceEqual(expected),"Composed rule order differs from dependency load order");
            Check(expected.Select(id=>id.Namespace.Value).SequenceEqual(new[]{"pack.m","pack.z","pack.a"}),"Fixture did not exercise dependency before lexical order");
            Check(session.Content.Patches.Count==3 && session.Content.Patches.All(p=>p.Operation==ModContentPatchOperation.Append),"Patch contributors or append operation lost");
            var instances=new ModBattleRuleInstances();Check(instances.Applicable(session.Content,session.Content.RuntimeFightId(fight.Id),true,1,false).Select(rule=>rule.Id).SequenceEqual(expected),"Combat dispatch changed composition order");
            var hash=ModSaveData.ComputeContentSetFingerprint(session.ActiveMods,session.Content);Check(fingerprint==null || fingerprint==hash,"Discovery order changed pack fingerprint");fingerprint=hash;
        }
        foreach(var combination in new[]{"append-replace","replace-append","replace-replace","controllers","disjoint"})
        {
            var folder=Path.Combine(root,combination);bool control=combination is "controllers" or "disjoint";
            Write(folder,"pack.a",Rule("first",control,combination=="disjoint"?",rounds={1}":"")+Patch(combination!="replace-append"&&combination!="replace-replace"));
            Write(folder,"pack.b",Rule("second",control,combination=="disjoint"?",rounds={2}":"")+"sf2.fights.patch{target='"+Target+"',description='must roll back'};"+Patch(combination!="append-replace"&&combination!="replace-replace"));
            Write(folder,"pack.c",Rule("third")+Patch());
            Write(folder,"pack.d",Rule("dependent")+Patch(),"pack.b");
            using var session=Start(folder,out _);bool disjoint=combination=="disjoint";
            Check(session.HasErrors!=disjoint,"Unexpected conflict acceptance for "+combination);
            session.Content.TryGetFight(DefinitionId.Parse(Target),out var fight);
            Check(session.ActiveMods.Any(mod=>mod.Id.Value=="pack.a") && session.ActiveMods.Any(mod=>mod.Id.Value=="pack.b")==disjoint,"Rejected owner remained active");
            Check(session.Content.Behaviors.Any(b=>b.Id.Namespace.Value=="pack.b")==disjoint,"Conflict leaked definitions");
            Check(fight.Description==(disjoint?"must roll back":""),"Conflict leaked an independent description");
            bool third=combination!="replace-append"&&combination!="replace-replace";
            Check(session.ActiveMods.Any(mod=>mod.Id.Value=="pack.c")==third,"A rejected transaction poisoned later independent append");
            Check(session.ActiveMods.Any(mod=>mod.Id.Value=="pack.d")==disjoint && session.Content.Behaviors.Any(b=>b.Id.Namespace.Value=="pack.d")==disjoint,"Dependency on failed controller remained active or leaked definitions");
            if(combination=="controllers")Check(session.Diagnostics.Any(d=>d.Message.Contains("first")&&d.Message.Contains("second")&&d.Message.Contains("conflicting round outcome")),"Controller diagnostic omitted both owners");
            else if(!disjoint)Check(session.Diagnostics.Any(d=>d.Message.Contains("pack.a")&&d.Message.Contains("pack.b")&&d.Message.Contains("replacement")),"Replacement conflict omitted owners");
        }
        AllContributorDiagnostic();StaticProjection();ShippedPack();
        Console.WriteLine("PASS: "+checks+" mod pack checks: composition/order/conflicts/rollback, production adapter projection/restoration and shipped Focus/objective lifecycle. Battle source storage and combat models are controlled.");
    }
    static void AllContributorDiagnostic()
    {
        var folder=Path.Combine(root,"contributors");
        Write(folder,"pack.a",Rule("a")+Patch());Write(folder,"pack.b",Rule("b")+Patch());Write(folder,"pack.c",Rule("c")+Patch(false));
        Write(folder,"pack.d",Rule("d")+Patch());
        using var session=Start(folder,out _);session.Content.TryGetFight(DefinitionId.Parse(Target),out var fight);
        Check(session.HasErrors && session.ActiveMods.Count==3 && fight.Rules.Count==3,"Failed replacement removed contributors or blocked later append");
        Check(session.Diagnostics.Any(d=>d.Message.Contains("pack.a")&&d.Message.Contains("pack.b")&&d.Message.Contains("pack.c")&&d.Message.Contains("replacement")),"Replacement diagnostic omitted existing contributors");
    }
    static void StaticProjection()
    {
        var folder=Path.Combine(root,"static");Write(folder,"pack.a","local r=sf2.rules.no_perks{id='a'};"+Patch());Write(folder,"pack.b","local r=sf2.rules.no_perks{id='b'};"+Patch());Write(folder,"pack.c",Rule("lua")+Patch());
        using var session=Start(folder,out _);Check(!session.HasErrors,session.FormatReport());
        var source=new XmlDocument();source.LoadXml("<Battle Name='Trial'><Fight Name='1' Music='1' Location='dojo' Power='7'><Warriors><Warrior Name='Original'/></Warriors><Rules><NoMagic/></Rules><Rewards><Reward Coins='123'/></Rewards></Fight></Battle>");
        var battle=new Battle{Source=source.DocumentElement};var list=new ListSF();list.Sources.Add("Test/Trial",battle);var original=battle.Source.OuterXml;
        var adapter=new PackAdapter(session.Content);adapter.Apply(list);
        Check(battle.Replacements==1,"Core battle was replaced more than once");
        Check(battle.Source.SelectNodes("Fight/Rules/NoPerks").Count==2 && battle.Source.SelectNodes("Fight/Rules/NoMagic").Count==1,"Composed static rules duplicated or dropped native entries");
        Check(battle.Source.SelectSingleNode("Fight/Warriors/Warrior").Attributes["Name"].Value=="Original" && battle.Source.SelectSingleNode("Fight/Rewards/Reward").Attributes["Coins"].Value=="123","Rule composition changed unrelated native data");
        Check(source.OuterXml==original,"Projection mutated restoration source");adapter.Remove(list);
        Check(battle.Source.OuterXml==original && battle.Restorations==1,"Disable did not restore original XML once");adapter.Remove(list);Check(battle.Restorations==1,"Repeated teardown restored twice");
        using var again=Start(folder,out _);var second=new PackAdapter(again.Content);second.Apply(list);
        Check(battle.Source.SelectNodes("Fight/Rules/NoPerks").Count==2 && battle.Source.SelectNodes("Fight/Rules/NoMagic").Count==1,"Reinstall accumulated static rules");second.Remove(list);
        // Enforce the aggregate limit across contributors, not just each Lua list.
        var maximum=Path.Combine(root,"maximum");
        string Many(int count)=>"local rules={};for i=1,"+count+" do rules[i]=sf2.rules.no_perks{id='r'..i} end;sf2.fights.patch{target='"+Target+"',append_rules=rules};";
        Write(maximum,"pack.a",Many(60));Write(maximum,"pack.b",Many(41));Write(maximum,"pack.c",Many(40));
        using var bounded=Start(maximum,out _);bounded.Content.TryGetFight(DefinitionId.Parse(Target),out var capped);
        Check(bounded.HasErrors && capped.Rules.Count==100 && bounded.ActiveMods.Count==2 && bounded.Content.FightRules.Count==100,"Aggregate rule limit leaked a rejected transaction or blocked retry");
        // Different owners appending the same dependency handle still conflict.
        var duplicate=Path.Combine(root,"duplicate");var a=Write(duplicate,"pack.a","");var b=Write(duplicate,"pack.b","","pack.a");var catalog=new ModContentCatalog();Import(catalog);
        DefinitionId shared;
        using(var tx=catalog.BeginRegistration(a)){shared=tx.RegisterNoPerksRule("shared",ModRuleTarget.All,ModRuleMode.All,null).Id;tx.PatchFightRules(Target,new[]{shared},true);tx.Commit();}
        using(var tx=catalog.BeginRegistration(b)){tx.PatchFightDescription(Target,"leak");tx.PatchFightRules(Target,new[]{shared},true);try{tx.Commit();throw new Exception("Duplicate attached handle accepted");}catch(ModContentException e){Check(e.Message.Contains("already attached"),e.Message);}}
        catalog.TryGetFight(DefinitionId.Parse(Target),out var unchanged);Check(unchanged.Description=="" && unchanged.Rules.Count==1 && catalog.Patches.Count==1,"Duplicate handle partially committed");
    }
    static void ShippedPack()
    {
        var folder=Path.Combine(root,"shipped");Directory.CreateDirectory(folder);
        foreach(var id in new[]{"example.focus-framework","example.focus-addon","example.hit-objective"})
            foreach(var file in Directory.GetFiles(Path.Combine(repo,"Mods",id),"*",SearchOption.AllDirectories))
            {var output=Path.Combine(folder,id,Path.GetRelativePath(Path.Combine(repo,"Mods",id),file));Directory.CreateDirectory(Path.GetDirectoryName(output));File.Copy(file,output);}
        void Vanilla(ModContentCatalog content){var xml=new XmlDocument();xml.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));CoreContentImporter.ImportStages(content,xml.SelectSingleNode("Stages/Zones"));}
        var profile=new XmlDocument();profile.LoadXml("<Warrior/>");
        using(var session=Start(folder,out var surfaces,Vanilla))
        {
            Check(!session.HasErrors && session.ActiveMods.Count==3,session.FormatReport());
            ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);
            Check(session.BindState(profile.DocumentElement).Count==0,"Initial framework profile bind failed");ModRuntime.Scripts=new FixtureScripts{Content=session.Content};
            var fight=new Fight();fight.FightDefinition.FightId=session.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"));var instances=new ModBattleRuleInstances();
            void Dispatch(ModEffectEvent kind)
            {
                foreach(var rule in instances.Applicable(session.Content,fight.FightDefinition.FightId,true,1,false))
                    Check(session.TryInvokeBehavior(rule.Behavior,kind,rule.InitialParameters,new Dictionary<string,string>{{"source","rule"},{"round","1"},{"side","player"},{"fight_id",fight.FightDefinition.FightId}},new ModInstanceFighter(fight,instances.Instance(rule.Id,true),rule),out var error),error);
            }
            Dispatch(ModEffectEvent.RoundBegin);Check(surfaces.Count==2 && surfaces.Select(s=>s.Owner).Distinct().Count()==2,"Pack HUD ownership merged");
            double damage=.1;fight.IncomingHit=new ModIncomingHit(()=>damage,value=>damage=value,false,false);Dispatch(ModEffectEvent.DamageDealing);
            Check(damage==.1 && surfaces.Single(s=>s.Id=="focus").Read("meter").Text=="Focus: 1/3","Framework did not update composed pack");
            for(int i=1;i<=3;i++){fight.DamageEvent=new ModDamageEvent(1,1,.9,false,false);Dispatch(ModEffectEvent.DamageDealt);}
            Dispatch(ModEffectEvent.Tick);fight.Step();Check(fight.Player.RoundsWon==1 && fight.Player.Health==1,"Composed objective failed native score");
            Dispatch(ModEffectEvent.FightEnd);Check(surfaces.All(s=>s.IsClosed),"Composed HUD teardown leaked");
            ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);
        }
        var disabled=Path.Combine(root,"disabled");Directory.CreateDirectory(disabled);
        foreach(var id in new[]{"example.focus-framework","example.focus-addon"})
            foreach(var file in Directory.GetFiles(Path.Combine(folder,id),"*",SearchOption.AllDirectories))
            {var output=Path.Combine(disabled,id,Path.GetRelativePath(Path.Combine(folder,id),file));Directory.CreateDirectory(Path.GetDirectoryName(output));File.Copy(file,output);}
        using(var session=Start(disabled,out _,Vanilla))
        {Check(!session.HasErrors,session.FormatReport());session.BindState(profile.DocumentElement);Check(session.Content.FightRules.Count==1 && session.State.TryGetValue(ModId.Parse("example.focus-framework"),"focus",out var value) && value.Integer==1,"Disabling objective removed framework rules/state");ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);}
        using(var session=Start(folder,out _,Vanilla))
        {Check(!session.HasErrors,session.FormatReport());session.BindState(profile.DocumentElement);Check(session.Content.FightRules.Count==2 && session.State.TryGetValue(ModId.Parse("example.focus-framework"),"focus",out var value) && value.Integer==1,"Reinstall lost framework state or duplicated rules");}
        var empty=Path.Combine(root,"empty");Directory.CreateDirectory(empty);
        using(var session=Start(empty,out _,Vanilla))
        {
            Check(!session.HasErrors && session.Content.FightRules.Count==0,"Removing pack left rule definitions");
            session.BindState(profile.DocumentElement);ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);
        }
        using(var session=Start(folder,out _,Vanilla))
        {Check(!session.HasErrors,session.FormatReport());session.BindState(profile.DocumentElement);Check(session.State.TryGetValue(ModId.Parse("example.focus-framework"),"focus",out var value)&&value.Integer==1,"Removing whole pack lost preserved owned data");}
    }
}
