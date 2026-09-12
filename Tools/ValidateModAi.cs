using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

sealed class Core : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id,out AssetMetadata metadata)
    { metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture"); return true; }
}

static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Main(string[] args)
    {
        string mods = args[0], entry = Path.Combine(mods,"example.charge-ui/scripts/main.lua");
        foreach (string body in new[] {
            "memory.calls=(memory.calls or 0)+1; assert(event.self.health==40 and event.opponent.health==30); if memory.calls==1 then return event.actions[2] end; return 'wait'",
            "return nil", "return {}", "while true do end",
            "if memory.saved then return memory.saved end; memory.saved=event.actions[1]; return memory.saved"
        })
        {
            File.WriteAllText(entry,"local sf2=require('sf2')\nsf2.tactics.register{id='brain',template='Standard',on_decide=function(memory,event) "+body+" end}");
            var mod = ModDiscovery.DiscoverLoose(mods).Mods.Single();
            var catalog = new ModContentCatalog();
            using (var tx = catalog.BeginRegistration(mod))
            using (var context = new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null)))
            {
                context.ExecuteEntrypoint(); tx.Commit();
                var ai = (IModAiScriptContext)context;
                string tactic = "example.charge-ui:tactics/brain";
                Check(ai.HasAiHandler(tactic) && !ai.HasAiHandler("other.mod:tactics/brain"),"AI owner isolation failed");
                var snapshot = new ModCombatSnapshot(new ModFighterSnapshot(40,50,1,10,0,0),new ModFighterSnapshot(30,50,1,60,0,0),60,true);
                var instance = new object();
                bool valid = ai.TryDecideAi(tactic,instance,snapshot,new[]{"punch","kick"},out var first,out var error);
                if (body.StartsWith("memory.calls"))
                {
                    Check(valid && first==1,error);
                    Check(ai.TryDecideAi(tactic,instance,snapshot,new[]{"punch"},out var next,out error) && next==-1,"AI memory/wait failed");
                    Check(ai.TryDecideAi(tactic,new object(),snapshot,new[]{"punch","kick"},out var other,out error) && other==1,"Fighter memories leaked");
                }
                else if (body=="return nil") Check(valid && first==null,"Native fallback failed");
                else if (body.StartsWith("if memory.saved"))
                {
                    Check(valid && first==0,error);
                    Check(!ai.TryDecideAi(tactic,instance,snapshot,new[]{"punch"},out _,out error),"Stale action accepted");
                }
                else
                {
                    Check(!valid && error != null,"Forged/unbounded AI accepted");
                    Check(ai.TryDecideAi(tactic,instance,snapshot,new[]{"punch"},out var fallback,out error) && fallback==null,"Broken AI was retried instead of falling back");
                }
                context.Dispose(); Check(!ai.HasAiHandler(tactic),"Disposed context retained AI");
            }
        }
        var shipped=ModDiscovery.DiscoverLoose(Path.Combine(args[1],"Mods")).Mods.Single(m=>m.Id.Value=="example.programmable-ai");
        var content=new ModContentCatalog(); var stages=new XmlDocument(); stages.Load(Path.Combine(args[1],"Assets/vanillaXml/stages.xml"));
        CoreContentImporter.ImportWarriorTemplates(content,stages.SelectSingleNode("Stages/Warriors/Templates"));
        var resolver=new AssetResolver(new IAssetProvider[]{new Core(),new LooseModProvider(shipped)});
        using(var tx=content.BeginRegistration(shipped))
        using(var context=new MoonSharpScriptRuntime().CreateContext(shipped,new ModApiFacade(shipped,resolver,tx,new ModStateRuntime(),null)))
        {
            ModLocalizationLoader.Load(shipped,resolver,tx); context.ExecuteEntrypoint(); tx.Commit();
            Check(content.Warriors.Count==3 && content.Modes.Count==1,"AI Dojo failed to register its distinct fighters and map mode");
            var ai=(IModAiScriptContext)context;
            var close=new ModCombatSnapshot(new ModFighterSnapshot(40,50,1,10,0,0),new ModFighterSnapshot(30,50,1,60,0,0),60,true);
            var later=new ModCombatSnapshot(close.Self,close.Opponent,180,true);
            var names=new[]{"HighKick","LowKick","KatanaStepBack","KatanaStepForward"};
            foreach(var brain in new[]{"patient","footwork","alternating"})
            {
                var instance=new object(); string tactic=shipped.Id+":tactics/"+brain;
                Check(ai.TryDecideAi(tactic,instance,close,names,out var selected,out var error) && selected==(brain=="footwork"?2:0),"Shipped "+brain+" decision failed: "+error);
                Check(ai.TryDecideAi(tactic,instance,close,names,out selected,out error) && selected==-1,"Shipped "+brain+" cooldown failed");
                Check(ai.TryDecideAi(tactic,instance,later,names,out selected,out error) && selected==(brain=="alternating"?1:brain=="footwork"?2:0),"Shipped "+brain+" follow-up failed: "+error);
            }
        }
        Console.WriteLine("PASS: "+checks+" actual Lua AI decision, shipped AI Dojo, state isolation, action lifetime, fallback and instruction-budget checks. Native combat playtest remains separate.");
    }
}
