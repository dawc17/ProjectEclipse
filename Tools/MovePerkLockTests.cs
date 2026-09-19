using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Eclipse.Modding;

namespace Eclipse.Modding
{
    public sealed class ModContentException : Exception { public ModContentException(string message) : base(message) { } }
    public readonly struct DefinitionId : IEquatable<DefinitionId>
    {
        public string Namespace { get; }
        public string Category { get; }
        public string LocalId { get; }
        public DefinitionId(string ns, string category, string local) { Namespace=ns; Category=category; LocalId=local; }
        public static DefinitionId Parse(string value)
        {
            int colon=value.IndexOf(':'), slash=value.IndexOf('/',colon+1);
            return new DefinitionId(value.Substring(0,colon),value.Substring(colon+1,slash-colon-1),value.Substring(slash+1));
        }
        public bool Equals(DefinitionId other) => Namespace==other.Namespace&&Category==other.Category&&LocalId==other.LocalId;
        public override bool Equals(object value) => value is DefinitionId other&&Equals(other);
        public override int GetHashCode() => HashCode.Combine(Namespace,Category,LocalId);
        public static bool operator ==(DefinitionId a,DefinitionId b)=>a.Equals(b);
        public static bool operator !=(DefinitionId a,DefinitionId b)=>!a.Equals(b);
        public override string ToString()=>Namespace+":"+Category+"/"+LocalId;
    }
    public sealed class PerkDefinition
    {
        public DefinitionId Id { get; }
        public string LegacyName { get; }
        public bool IsCore => Id.Namespace=="core";
        public PerkDefinition(DefinitionId id,string legacyName=null){Id=id;LegacyName=legacyName;}
    }
    public sealed partial class ModRegistrationTransaction
    {
        internal readonly ModContentCatalog _catalog;
        internal readonly Dictionary<DefinitionId,PerkDefinition> Known = new Dictionary<DefinitionId,PerkDefinition>();
        internal bool AllowCore = true;
        public ModRegistrationTransaction(ModContentCatalog catalog){_catalog=catalog;}
        private void ThrowIfCompleted() { }
        private bool CanReferenceNamespace(string ns) => ns=="core"&&AllowCore;
        public PerkDefinition GetPerk(string reference)
        {
            DefinitionId id=DefinitionId.Parse(reference);
            if(Known.TryGetValue(id,out var perk)) return perk;
            throw new ModContentException("Perk is not registered: '"+id+"'.");
        }
        private void EnsureCapacityForNewRegistration() { }
    }
    public sealed partial class ModApiFacade
    {
        internal ModRegistrationTransaction Transaction;
        internal string Capability;
        private void RequireCapability(string capability){Capability=capability;}
        private ModRegistrationTransaction RequireRegistration()=>Transaction;
    }
}

public class ConditionAnimation { public readonly string Kind; public ConditionAnimation(string kind){Kind=kind;} }
public sealed class ConditionPerk : ConditionAnimation
{
    private readonly string _name;
    public ConditionPerk(string name):base("Perk"){_name=name;}
    public string get_Name()=>_name;
}
public sealed class MoveInside { public readonly List<ConditionAnimation> Locks=new List<ConditionAnimation>(); }
public sealed class InfoAnimation { public string Name; public MoveInside MoveData=new MoveInside(); }
public static class ConditionsParser
{
    public static ConditionAnimation Create(XmlNode node)
        => node.Name=="Perk" ? new ConditionPerk(node.Attributes?["Name"]?.Value??"") : new ConditionAnimation(node.Name);
}
public static class AnimationData
{
    public static readonly List<InfoAnimation> Animations=new List<InfoAnimation>();
}
public static class SF2Paths { public static string MCFPDHOLNGB()=>"fixture"; }
public static class XmlUtils
{
    public static XmlDocument Source;
    public static XmlDocument OpenXMLDocument(string path,string file)=>Source;
}

internal static class Program
{
    private static readonly string[] Moves={"DoubleJumpKick","ElbowStrike","TwoFootJumpKick","BackFlipKick","ThrowSuplex","ThrowSuplexProfile"};
    private static readonly string[] Perks={"PERK_DOUBLE_JUMP_KICK","PERK_ELBOW_STRIKE","PERK_TWO_FOOT_JUMP_KICK","PERK_BACK_FLIP_KICK","PERK_SUPLEX","PERK_SUPLEX"};
    private static void Assert(bool value,string message){if(!value)throw new Exception(message);}
    private static void Throws<T>(Action action,string message) where T:Exception { try{action();}catch(T){return;} throw new Exception(message); }
    private static XmlNode Move(XmlDocument doc,string name)
    {
        foreach(XmlNode node in doc["Movesxml"]["Moves"].ChildNodes)
            if(node.NodeType==XmlNodeType.Element&&node.Name=="Move"&&node.Attributes?["Name"]?.Value==name)return node;
        return null;
    }
    private static InfoAnimation BuildLive(XmlNode move)
    {
        var live=new InfoAnimation{Name=move.Attributes["Name"].Value};
        foreach(XmlNode node in move["Locks"].ChildNodes) if(node.NodeType==XmlNodeType.Element) live.MoveData.Locks.Add(ConditionsParser.Create(node));
        live.MoveData.Locks.Add(new ConditionPerk("INHERITED_SENTINEL"));
        return live;
    }
    private static bool HasDirectPerk(XmlNode move,string perk)
    {
        XmlNode locks=move?["Locks"]; if(locks==null)return false;
        foreach(XmlNode node in locks.ChildNodes) if(node.Name=="Perk"&&node.Attributes?["Name"]?.Value==perk)return true;
        return false;
    }
    private static void Main(string[] args)
    {
        string root=args[0];
        var vanilla=new XmlDocument(); vanilla.Load(Path.Combine(root,"Assets/vanillaXml/animations/moves.xml"));
        var de=new XmlDocument(); de.Load(Path.Combine(root,"Assets/DExml/animations/moves.xml"));
        XmlUtils.Source=vanilla;
        AnimationData.Animations.Clear();
        var original=new Dictionary<string,string[]>();
        for(int i=0;i<Moves.Length;i++)
        {
            XmlNode baseMove=Move(vanilla,Moves[i]), deMove=Move(de,Moves[i]);
            Assert(baseMove!=null&&deMove!=null,"Missing authoritative move "+Moves[i]);
            Assert(HasDirectPerk(baseMove,Perks[i]),"Vanilla direct perk lock missing: "+Moves[i]);
            Assert(!HasDirectPerk(deMove,Perks[i]),"DExml still has removed perk lock: "+Moves[i]);
            InfoAnimation live=BuildLive(baseMove); AnimationData.Animations.Add(live);
            var names=new List<string>(); foreach(var c in live.MoveData.Locks) names.Add(c is ConditionPerk p?"Perk:"+p.get_Name():c.Kind); original[Moves[i]]=names.ToArray();
        }
        var removals=new List<MovePerkLockRemoval>();
        for(int i=0;i<Moves.Length;i++) removals.Add(new MovePerkLockRemoval(Moves[i],new DefinitionId("core","perks",Perks[i].ToLowerInvariant()),Perks[i]));
        var rollback=ExternalCombatContentRuntime.ApplyMovePerkLocks(removals);
        for(int i=0;i<Moves.Length;i++)
        {
            var locks=AnimationData.Animations[i].MoveData.Locks;
            Assert(!locks.Exists(c=>c is ConditionPerk p&&p.get_Name()==Perks[i]),"Target lock survived: "+Moves[i]);
            Assert(locks.Exists(c=>c is ConditionPerk p&&p.get_Name()=="INHERITED_SENTINEL"),"Inherited lock was removed: "+Moves[i]);
            Assert(locks.Count==original[Moves[i]].Length-1,"Unexpected sibling lock mutation: "+Moves[i]);
        }
        ExternalCombatContentRuntime.RemoveMovePerkLocks(rollback);
        for(int i=0;i<Moves.Length;i++)
        {
            var locks=AnimationData.Animations[i].MoveData.Locks;
            Assert(locks.Count==original[Moves[i]].Length,"Rollback lock count: "+Moves[i]);
            for(int j=0;j<locks.Count;j++)
            {
                string current=locks[j] is ConditionPerk p?"Perk:"+p.get_Name():locks[j].Kind;
                Assert(current==original[Moves[i]][j],"Rollback ordering: "+Moves[i]);
            }
        }

        var catalog=new ModContentCatalog(); var tx=new ModRegistrationTransaction(catalog);
        var perkId=new DefinitionId("core","perks",Perks[0].ToLowerInvariant()); tx.Known[perkId]=new PerkDefinition(perkId,Perks[0]);
        tx.RemoveMovePerkLock(Moves[0],perkId);
        Throws<ModContentException>(()=>tx.RemoveMovePerkLock(Moves[0],perkId),"Pending duplicate accepted");
        typeof(ModRegistrationTransaction).GetMethod("ValidateMovePerkLockCommit",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(tx,null);
        typeof(ModRegistrationTransaction).GetMethod("ApplyMovePerkLockCommit",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(tx,null);
        Assert(catalog.MovePerkLockRemovals.Count==1,"Catalog commit missing");
        var tx2=new ModRegistrationTransaction(catalog); tx2.Known[perkId]=new PerkDefinition(perkId,Perks[0]); tx2.RemoveMovePerkLock(Moves[0],perkId);
        try { typeof(ModRegistrationTransaction).GetMethod("ValidateMovePerkLockCommit",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(tx2,null); throw new Exception("Catalog duplicate accepted"); }
        catch(TargetInvocationException e) when(e.InnerException is ModContentException) { }
        Throws<ModContentException>(()=>new MovePerkLockRemoval(" DoubleJumpKick",perkId,Perks[0]),"Noncanonical move name accepted");

        var wrongCase=new List<MovePerkLockRemoval>{new MovePerkLockRemoval(Moves[0],perkId,Perks[0].ToLowerInvariant())};
        Throws<InvalidOperationException>(()=>ExternalCombatContentRuntime.ApplyMovePerkLocks(wrongCase),"Case-insensitive native perk lock match accepted");

        var facade=new ModApiFacade{Transaction=new ModRegistrationTransaction(new ModContentCatalog())}; facade.Transaction.Known[perkId]=new PerkDefinition(perkId,Perks[0]);
        facade.RemoveMovePerkLock(Moves[0],perkId); Assert(facade.Capability=="content.patch","Facade capability mismatch");
        Console.WriteLine("PASS: 6 XML-authoritative move locks across 5 gate perks removed exactly; siblings/inherited locks preserved; rollback, exact casing, duplicate conflicts and capability validated.");
    }
}
