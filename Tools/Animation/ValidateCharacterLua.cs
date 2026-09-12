using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
sealed class PreviewCore : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id,out AssetMetadata metadata)
    { metadata=new AssetMetadata(id,AssetKind.Sprite,AssetSourceKind.Core,"",-1,"fixture"); return true; }
}
static class Program
{
    static void Main(string[] args)
    {
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog=new ModContentCatalog(); var stages=new XmlDocument(); stages.Load(args[1]);
        CoreContentImporter.ImportWarriorTemplates(catalog,stages.SelectSingleNode("Stages/Warriors/Templates"));
        bool packaged=args.Length>2 && bool.Parse(args[2]);
        var assets=new AssetResolver(new IAssetProvider[]{new PreviewCore(),new LooseModProvider(mod)});
        using(var tx=catalog.BeginRegistration(mod))
        using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null)))
        {
            ModLocalizationLoader.Load(mod,assets,tx); script.ExecuteEntrypoint(); tx.Commit();
            var warrior=catalog.Warriors.Single();
            if(warrior.BodyModel.Namespace!=mod.Id || warrior.SkinModels.Count!=1) throw new Exception("Authored model handles did not reach character definition");
            if(packaged)
            {
                var move=catalog.Moves.Single();
                int sampleCount=BitConverter.ToInt32(File.ReadAllBytes(Path.Combine(mod.RootPath,"assets/animations/authored.bytes")),0);
                if(move.EndFrame!=sampleCount-1)
                    throw new Exception("Move bounds must index native samples independently of interpolation spacing");
                var mode=catalog.Modes.Single();
                var fight=catalog.Fights.Single();
                if(fight.Warriors.Single()!=warrior.Id || mode.Fights.Single()!=fight.Id || !mode.Repeatable || catalog.Quests.Count!=1)
                    throw new Exception("Packaged character is not connected to its repeatable map preview");
                var ai=(IModAiScriptContext)script;
                var snapshot=new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,60,true);
                if(!ai.TryDecideAi(warrior.Tactic,new object(),snapshot,new[]{move.RuntimeName},out var choice,out var error) || choice!=0)
                    throw new Exception("Packaged opponent does not select its authored move: "+error);
                Console.WriteLine("PASS: complete packaged character Lua, map/fight/mode binding, skin assets and authored-move AI selection.");
                return;
            }
            var attack=catalog.Moves.Single(m=>m.Id.LocalId=="attack");
            var projectionType=typeof(ModContentCatalog).Assembly.GetType("Projection",true);
            var projection=Activator.CreateInstance(projectionType,true);
            var body=(XmlElement)projectionType.GetMethod("Warrior").Invoke(projection,new object[]{catalog,warrior});
            if(body.GetAttribute("EclipseBodyModel")!=warrior.BodyModel.ToString() || body.SelectSingleNode("EclipseSkinModels/Model")?.Attributes["Asset"]?.Value!=warrior.SkinModels[0].ToString())
                throw new Exception("Authored body/skin did not reach native warrior projection");
            var xml=(XmlElement)projectionType.GetMethod("Move").Invoke(projection,new object[]{attack});
            if(xml.SelectSingleNode("Conditions/EclipseCharacter")?.Attributes["Name"]?.Value!=warrior.Id.ToString() ||
               xml.SelectSingleNode("Conditions/Keys/Key")?.Attributes["Type"]?.Value!="Kick" ||
               xml.SelectSingleNode("Events/KeyPressed")==null ||
               xml.SelectSingleNode("Intervals/Interval[@Start='6'][@End='8']/AttackingParts/Edge")?.Attributes["Name"]?.Value!="ECalf_2" ||
               xml.SelectSingleNode("Intervals/Interval/Damage/Damage")?.Attributes["Type"]?.Value!="UnarmedDamage")
                throw new Exception("Authored move did not project to native input, character and damage contracts");
            Console.WriteLine("PASS: exported Blender character module, typed body/skin bindings and production native move projection.");
        }
    }
}
