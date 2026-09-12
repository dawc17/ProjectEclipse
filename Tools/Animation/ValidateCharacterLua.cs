using System;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
static class Program
{
    static void Main(string[] args)
    {
        var mod=ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog=new ModContentCatalog(); var stages=new XmlDocument(); stages.Load(args[1]);
        CoreContentImporter.ImportWarriorTemplates(catalog,stages.SelectSingleNode("Stages/Warriors/Templates"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        using(var tx=catalog.BeginRegistration(mod))
        using(var script=new MoonSharpScriptRuntime().CreateContext(mod,new ModApiFacade(mod,assets,tx,new ModStateRuntime(),null)))
        {
            script.ExecuteEntrypoint(); tx.Commit();
            var warrior=catalog.Warriors.Single();
            if(warrior.BodyModel.Namespace!=mod.Id || warrior.SkinModels.Count!=1) throw new Exception("Authored model handles did not reach character definition");
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
