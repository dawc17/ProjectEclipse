using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

internal static class MoveCombatPatchTests
{
    private static int count;
    private static void Check(bool value, string why) { count++; if (!value) throw new Exception(why); }
    private static XmlElement Node(string xml) { var d = new XmlDocument(); d.LoadXml(xml); return d.DocumentElement; }
    private static readonly ModId Owner = ModId.Parse("fixture.move-patches");
    private static InfoAnimation Move(string name = "Test", bool initialized = false)
    {
        var move = new InfoAnimation { Name = name };
        move.MoveData.Intervals.Add(new IntervalAnimation { NodeInterval = Node("<Interval Name='Uninterrupt' Start='2' End='42'/>") });
        move.MoveData.Intervals.Add(new IntervalAttack { NodeInterval = Node("<Interval Type='Attack' Start='0' End='50'><Hit Name='High'/></Interval>") });
        move.ScheduledActions.Add(new ActionSound("snd_disk",18));
        move.SelectionConditions.Add(new ConditionAnimation());
        if (initialized) foreach (var interval in move.MoveData.Intervals) interval.Init();
        return move;
    }
    private static MoveCombatPatch Patch(string move = "Test") => new MoveCombatPatch(Owner,move,
        new[] { new ModMoveCondition(ModMoveConditionKind.ModExists,"Stun",not:true) },
        new ModMoveFramePatch("Uninterrupt",42,40),new ModMoveHitPatch("High","MiddleShortPlus"),new ModMoveFramePatch("snd_disk",18,16));
    private static MoveCombatPatchRuntime.Lifetime Apply(InfoAnimation[] moves, params MoveCombatPatch[] patches)
        => MoveCombatPatchRuntime.Apply(moves,patches,_ => new ConditionAnimation());
    private static void Reject(InfoAnimation[] moves, MoveCombatPatch[] patches, string reason)
    {
        bool rejected = false; try { Apply(moves,patches); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected,reason);
    }
    public static void Main()
    {
        foreach (bool initialized in new[] { false,true })
        foreach (bool consume in new[] { false,true })
        {
            var move=Move(initialized:initialized); var baseCondition=move.SelectionConditions[0];
            var interval=move.MoveData.Intervals[0]; var attack=(IntervalAttack)move.MoveData.Intervals[1];
            var source=interval.NodeInterval; var hitSource=attack.NodeInterval;
            var lifetime=Apply(new[]{move},Patch());
            Check(move.SelectionConditions.Count==2 && ReferenceEquals(move.SelectionConditions[0],baseCondition),"Condition order changed.");
            Check(move.ScheduledActions[0].ScheduledFrame==16,"Sound frame unchanged.");
            if (!initialized)
            {
                Check(source.Attributes["End"].Value=="42" && hitSource["Hit"].Attributes["Name"].Value=="High","Shared source nodes mutated.");
                Check(!ReferenceEquals(interval.NodeInterval,source) && !ReferenceEquals(attack.NodeInterval,hitSource),"Pending nodes not isolated.");
            }
            if (consume && !initialized) foreach(var value in move.MoveData.Intervals) value.Init();
            if (initialized || consume) Check(interval.EndFrame==40 && attack.HitReactions.Single().Name=="MiddleShortPlus","Parsed target not patched.");
            else Check(interval.NodeInterval.Attributes["End"].Value=="40" && attack.NodeInterval["Hit"].Attributes["Name"].Value=="MiddleShortPlus","Deferred target not patched.");
            var unrelated=new ConditionAnimation(); move.SelectionConditions.Add(unrelated);
            lifetime.Dispose(); lifetime.Dispose();
            Check(move.SelectionConditions.SequenceEqual(new[]{baseCondition,unrelated}),"Rollback damaged unrelated conditions.");
            Check(move.ScheduledActions[0].ScheduledFrame==18,"Sound rollback failed.");
            if (initialized || consume) Check(interval.EndFrame==42 && attack.HitReactions.Single().Name=="High","Post-init rollback failed.");
            else Check(ReferenceEquals(interval.NodeInterval,source)&&ReferenceEquals(attack.NodeInterval,hitSource),"Pre-init rollback failed.");
        }
        var first=Move("First"); var second=Move("Second"); var firstNode=first.MoveData.Intervals[0].NodeInterval;
        Reject(new[]{first,second},new[]{Patch("First"),new MoveCombatPatch(Owner,"Second",intervalEnd:new ModMoveFramePatch("Uninterrupt",41,40))},"Wrong expected value accepted.");
        Check(ReferenceEquals(first.MoveData.Intervals[0].NodeInterval,firstNode) && first.SelectionConditions.Count==1 && first.ScheduledActions[0].ScheduledFrame==18,"Late validation failure partially applied batch.");
        Reject(new[]{Move()},new[]{Patch("Missing")},"Missing target accepted.");
        Reject(new[]{Move(),Move()},new[]{Patch()},"Ambiguous target accepted.");
        Reject(new[]{Move()},new[]{Patch(),Patch()},"Duplicate patch accepted.");
        var duplicate=Move(); duplicate.MoveData.Intervals.Add(duplicate.MoveData.Intervals[0]);
        Reject(new[]{duplicate},new[]{Patch()},"Ambiguous interval accepted.");
        duplicate=Move(); duplicate.MoveData.Intervals.Add(duplicate.MoveData.Intervals[1]);
        Reject(new[]{duplicate},new[]{Patch()},"Ambiguous attack accepted.");
        duplicate=Move(); duplicate.ScheduledActions.Add(new ActionSound("snd_disk",18));
        Reject(new[]{duplicate},new[]{Patch()},"Ambiguous sound accepted.");
        var ranged=Move(); ((XmlElement)ranged.MoveData.Intervals[1].NodeInterval["Hit"]).SetAttribute("Start","3");
        Reject(new[]{ranged},new[]{Patch()},"Partial-range hit accepted.");
        var eventSound=Move(); eventSound.ScheduledActions[0].Frame=null;
        Reject(new[]{eventSound},new[]{Patch()},"Event sound accepted.");
        var low=Move(); Reject(new[]{low},new[]{new MoveCombatPatch(Owner,"Test",intervalEnd:new ModMoveFramePatch("Uninterrupt",42,1))},"End before start accepted.");
        var later=Move(initialized:true);var life=Apply(new[]{later},Patch());
        later.MoveData.Intervals[0].EndFrame=39;((IntervalAttack)later.MoveData.Intervals[1]).HitReactions[0].Name="Low";later.ScheduledActions[0].SetScheduledFrame(20);
        life.Dispose();Check(later.MoveData.Intervals[0].EndFrame==39 && ((IntervalAttack)later.MoveData.Intervals[1]).HitReactions[0].Name=="Low" && later.ScheduledActions[0].ScheduledFrame==20,"Rollback overwrote later field changes.");
        var shared=Move(); var other=Move("Other");other.MoveData.Intervals[0].NodeInterval=shared.MoveData.Intervals[0].NodeInterval;
        life=Apply(new[]{shared,other},Patch());other.MoveData.Intervals[0].Init();Check(other.MoveData.Intervals[0].EndFrame==42,"Patch leaked through shared template node.");life.Dispose();
        Console.WriteLine("PASS: "+count+" production move-patch batch/rollback checks. Controlled native containers exercise deferred and parsed intervals; native Unity acceptance is separate.");
    }
}

public class ConditionAnimation { }
public class InfoAnimation
{
    public string Name; public Data MoveData=new Data();
    public List<ConditionAnimation> SelectionConditions=new List<ConditionAnimation>();
    public List<ActionAnimation> ScheduledActions=new List<ActionAnimation>();
    public class Data { public List<IntervalAnimation> Intervals=new List<IntervalAnimation>(); }
}
public class IntervalAnimation
{
    public XmlNode NodeInterval;public string Name;public int Start,EndFrame;
    public virtual void Init() { Name=NodeInterval.Attributes["Name"]?.Value;Start=int.Parse(NodeInterval.Attributes["Start"].Value);EndFrame=int.Parse(NodeInterval.Attributes["End"].Value);NodeInterval=null; }
}
public class IntervalAttack : IntervalAnimation
{
    public class Reaction { public string Name;public int Start,EndFrame; }
    public List<Reaction> HitReactions=new List<Reaction>();
    public override void Init() { var node=NodeInterval; base.Init();HitReactions.Add(new Reaction{Name=node["Hit"].Attributes["Name"].Value,Start=Start,EndFrame=EndFrame}); }
}
public class ActionAnimation { public int? Frame;public int? ScheduledFrame=>Frame;public void SetScheduledFrame(int frame){Frame=frame;} }
public class ActionSound : ActionAnimation { private string name;public ActionSound(string value,int frame){name=value;Frame=frame;}public string get_Name()=>name; }
