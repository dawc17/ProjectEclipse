// Controlled battle lookup/storage around extracted production adapter apply/remove.
// Builders, catalog validation, script sessions and rule dispatch are production.
using System;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;
internal sealed partial class Projection
{
    public XmlElement Rule(ModContentCatalog content,XmlDocument document,FightRuleDefinition rule)
    { _content=content;return BuildRuleNode(document,rule); }
}
public sealed class Battle
{
    public XmlNode Source;
    public int Replacements,Restorations;
    public XmlNode CloneSourceDefinitionForModding()=>Source?.CloneNode(true);
    public bool ReplaceSourceDefinitionForModding(XmlNode node,out string error)
    {Source=node.CloneNode(true);Replacements++;error=null;return true;}
    public bool RestoreSourceDefinitionForModding(XmlNode node,out string error)
    {Source=node.CloneNode(true);Restorations++;error=null;return true;}
}
public sealed partial class ListSF
{
    public readonly Dictionary<string,Battle> Sources=new Dictionary<string,Battle>();
    public Battle FindBattleForModding(string zone,string battle)=>Sources.TryGetValue(zone+"/"+battle,out var result)?result:null;
    public void RemoveExternalBattle(string zone,string battle) { }
    public void RemoveExternalZone(string zone) { }
    public void RemoveExternalTemplate(string template) { }
}
public sealed partial class PackAdapter
{
    readonly ModContentCatalog _content;
    readonly List<BattleSourceBinding> _battleSourceBindings=new List<BattleSourceBinding>();
    readonly List<(string ZoneName,string BattleName)> _externalBattles=new List<(string,string)>();
    readonly List<string> _externalZoneNames=new List<string>(),_externalTemplateNames=new List<string>();
    readonly Projection projection=new Projection();
    public PackAdapter(ModContentCatalog content){_content=content;}
    public void Apply(ListSF list)=>ApplyCoreFightPatches(list);
    public void Remove(ListSF list)=>RemoveStages(list);
    XmlElement BuildRuleNode(XmlDocument document,FightRuleDefinition rule)=>projection.Rule(_content,document,rule);
    XmlElement BuildWarriorNode(XmlDocument document,WarriorDefinition warrior)=>throw new Exception("Unused warrior builder");
    XmlElement BuildRewardNode(XmlDocument document,RewardDefinition reward)=>throw new Exception("Unused reward builder");
}
