using System;
using System.IO;
using System.Xml;
public static class GameUtils { public static PerkItems FDEJIIDIPBI = new PerkItems(); }
public static class XmlFixtureExtensions {
    public static string CIPOICEEIBK(this XmlAttribute value,string fallback="") => value?.Value ?? fallback;
    public static int ParseInt(this XmlAttribute value,int fallback=0) => int.TryParse(value?.Value,out int n)?n:fallback;
    public static XmlNode LCOLFMJJDJE(this XmlNode parent,XmlNode child) {
        var doc=parent as XmlDocument ?? parent.OwnerDocument;
        return parent.AppendChild(doc.ImportNode(child,true));
    }
    public static XmlNode ACBPMPMPKJJ(this XmlNode parent,string name) {
        var doc=parent as XmlDocument ?? parent.OwnerDocument; return parent.AppendChild(doc.CreateElement(name));
    }
}
// Only the unrelated perk parser/presentation surface is stubbed. Clone below is
// extracted verbatim from production, and PerkItems is compiled in full.
public sealed class PerkInfoItem {
    public string Name,MGNNJPBCOGD,JNBECGKCNBB;
    public bool BGFEPJKDHFB,GDCBBAHKCIE;
    public int Level,AKKLOMFOLNO;
    public XmlNode Source;
    public void Parse(XmlNode node) {Source=node.CloneNode(true);Name=node.Attributes["Name"].Value;MGNNJPBCOGD=node.Attributes["Description"]?.Value;}
    public XmlNode FHCNBDOKIML()=>Source;
    public void OEBOFOCIPBH() {}
    // CLONE_BODY
}
public static class Program {
    static int checks;
    static void Check(bool condition,string message) {checks++;if(!condition)throw new Exception(message);}
    static XmlNode Node(string xml){var d=new XmlDocument();d.LoadXml(xml);return d.DocumentElement;}
    public static void Main(string[] args) {
        var perks=GameUtils.FDEJIIDIPBI;
        perks.Parse(Node("<Perks><Perk Name='vanilla' Description='unchanged'/></Perks>"));
        perks.NLLMCPOPFCI(Node("<Perks><Perk Name='vanilla'/></Perks>"));
        var original=perks.LAAJJBEEDKL("vanilla");
        var archive=new XmlDocument();archive.Load(Path.Combine(args[0],"Assets/DExml/CharacterProgress.xml"));
        foreach(string name in new[]{"PERK_MASTER_OF_STYLE","PERK_RELENTLESS"}) {
            var source=archive.SelectSingleNode("CharacterProgress/Perks/Perk[@Name='"+name+"']");
            if(source==null)source=archive.SelectSingleNode("*/Perks/Perk[@Name='"+name+"']");
            Check(source!=null,"Archived upgrade record missing");
            string owned="example.upgrade:perks/"+name.ToLowerInvariant();
            var basePerk=perks.AddExternalBasePerk(Node("<Perk Name='"+owned+"' Description='base'><Set Unchanged='17'/></Perk>"));
            string before=basePerk.Source.OuterXml;
            perks.AddExternalPerkUpgrades(owned,source);
            Check(perks.GAEHBOAPMLI(owned).Count==6,"Expected base plus five upgrades");
            Check(perks.LAAJJBEEDKL(owned,0).MGNNJPBCOGD=="base","Base description changed");
            foreach(XmlNode upgrade in source.SelectNodes("UpgradeLevel")) {
                int level=int.Parse(upgrade.Attributes["Value"].Value);var variant=perks.LAAJJBEEDKL(owned,level);
                Check(variant!=null && variant.MGNNJPBCOGD==upgrade.Attributes["Description"].Value,"Upgrade description lost");
                foreach(XmlAttribute value in upgrade["Set"].Attributes)
                    Check(variant.Source["Set"].Attributes[value.Name].Value==value.Value,"Native upgrade parameter lost: "+value.Name);
                Check(variant.Source["Set"].Attributes["Unchanged"].Value=="17","Upgrade discarded base parameters");
            }
            Check(basePerk.Source.OuterXml==before,"Clone mutated base definition");
            bool duplicate=false;try{perks.AddExternalPerkUpgrades(owned,source);}catch(InvalidOperationException){duplicate=true;}
            Check(duplicate && perks.GAEHBOAPMLI(owned).Count==6,"Duplicate install changed progression");
            Check(perks.RemoveExternalBasePerk(owned) && perks.GAEHBOAPMLI(owned).Count==0,"External variants survived removal");
            Check(ReferenceEquals(original,perks.LAAJJBEEDKL("vanilla")),"Removal changed vanilla progression");
        }
        Console.WriteLine("PASS: "+checks+" native-source perk upgrade checks; production PerkItems and Clone, archive payloads, descriptions, isolation and teardown.");
    }
}
