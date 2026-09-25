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
        => MoveCombatPatchRuntime.Apply(moves,patches,condition => condition.Kind == ModMoveConditionKind.Keys
            ? new ConditionKeys(condition.Keys.Single().Key) : new ConditionAnimation());
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
        foreach (string reaction in new[] { "Physycal", "HighLong", "NoReaction" })
        foreach (bool initialized in new[] { false, true })
        {
            var move = Move(initialized: initialized);
            var attack = (IntervalAttack)move.MoveData.Intervals[1];
            using (Apply(new[] { move }, new MoveCombatPatch(Owner, "Test", hit: new ModMoveHitPatch("High", reaction))))
            {
                if (!initialized) attack.Init();
                Check(attack.HitReactions.Single().Name == reaction, "Physical-fall patch changed native spelling.");
            }
            Check(attack.HitReactions.Single().Name == "High", "Physical-fall patch rollback failed.");
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
        var disabled = Move("Disabled"); var original = disabled.SelectionConditions[0];
        using (Apply(new[] { disabled }, new MoveCombatPatch(Owner, "Disabled", disable: true)))
        {
            Check(disabled.SelectionConditions.Count == 2 && ReferenceEquals(disabled.SelectionConditions[0], original), "Disable condition did not append.");
            Check(!disabled.SelectionConditions[1].IsEqual(new ModelConditions()) &&
                !disabled.SelectionConditions[1].IsEqual(new Model(), disabled), "Disabled move remained selectable.");
        }
        Check(disabled.SelectionConditions.SequenceEqual(new[] { original }), "Disabled move rollback changed original conditions.");
        var inputMove = Move("Input"); inputMove.Priority = 1000;
        inputMove.SelectionConditions[0] = new ConditionKeys("Super");
        var originalInput = inputMove.SelectionConditions[0];
        var inputPatch = new MoveCombatPatch(Owner, "Input", input: new ModMoveInputPatch("Super", "RaidCharge"),
            priority: new ModMovePriorityPatch(1000, 200));
        using (Apply(new[] { inputMove }, inputPatch))
        {
            Check(inputMove.Priority == 200 && inputMove.SelectionConditions[0] is ConditionKeys changed &&
                changed.Key == "RaidCharge", "Native input/priority patch did not apply.");
        }
        Check(inputMove.Priority == 1000 && ReferenceEquals(inputMove.SelectionConditions[0], originalInput),
            "Input/priority rollback did not restore the original native move.");
        var inserted = new ConditionAnimation();
        var shiftedInput = Apply(new[] { inputMove }, inputPatch);
        inputMove.SelectionConditions.Insert(0, inserted);
        shiftedInput.Dispose();
        Check(inputMove.SelectionConditions.SequenceEqual(new[] { inserted, originalInput }),
            "Input rollback damaged a later sibling insertion.");
        inputMove.SelectionConditions.Remove(inserted);
        Reject(new[] { inputMove }, new[] { new MoveCombatPatch(Owner, "Input",
            input: new ModMoveInputPatch("Magic", "RaidCharge")) }, "Wrong expected input accepted.");
        Reject(new[] { inputMove }, new[] { new MoveCombatPatch(Owner, "Input",
            priority: new ModMovePriorityPatch(900, 200)) }, "Wrong expected priority accepted.");
        var ambiguousInput = Move("Input"); ambiguousInput.SelectionConditions[0] = new ConditionKeys("Super");
        ambiguousInput.SelectionConditions.Add(new ConditionKeys("Super"));
        Reject(new[] { ambiguousInput }, new[] { new MoveCombatPatch(Owner, "Input",
            input: new ModMoveInputPatch("Super", "RaidCharge")) }, "Ambiguous native input accepted.");
        var lateInput = Move("LateInput"); lateInput.SelectionConditions[0] = new ConditionKeys("Super");
        Reject(new[] { inputMove, lateInput }, new[] { inputPatch,
            new MoveCombatPatch(Owner, "LateInput", priority: new ModMovePriorityPatch(1000, 200)) },
            "Late priority failure partially applied input patch.");
        Check(inputMove.Priority == 1000 && ReferenceEquals(inputMove.SelectionConditions[0], originalInput),
            "Failed patch batch changed the original input.");
        var clipMove = Move("Clip"); clipMove.FileName = "old_clip.bytes"; clipMove.AnimationEndFrame = 60;
        var clipPatch = new MoveCombatPatch(Owner, "Clip", animation: new ModMoveAnimationPatch(
            "old_clip.bytes", AssetId.Parse("fixture.move-patches:animations/new_clip")));
        using (Apply(new[] { clipMove }, clipPatch))
            Check(clipMove.FileName == "fixture.move-patches:animations/new_clip" && clipMove.AnimationEndFrame == 28,
                "Parsed native clip was not replaced.");
        Check(clipMove.FileName == "old_clip.bytes" && clipMove.AnimationEndFrame == 60,
            "Clip patch did not restore the original parsed animation.");
        Reject(new[] { clipMove }, new[] { new MoveCombatPatch(Owner, "Clip", animation:
            new ModMoveAnimationPatch("wrong_clip.bytes", AssetId.Parse("fixture.move-patches:animations/new_clip"))) },
            "Wrong expected native clip accepted.");
        foreach (bool initialized in new[] { false, true })
        {
            var removeMove = Move("Remove", initialized);
            var evade = new IntervalAnimation { Type = IntervalAnimation.IntervalType.INTERVAL_INVULNERABLE,
                NodeInterval = Node("<Interval Name='Evade' Type='Invulnerable' Start='0' End='47'/>") };
            if (initialized) evade.Init();
            removeMove.MoveData.Intervals.Insert(1, evade);
            var removal = new MoveCombatPatch(Owner, "Remove", removeInterval:
                new ModMoveIntervalRemoval("Evade", "Invulnerable", 0, 47));
            using (Apply(new[] { removeMove }, removal))
                Check(!removeMove.MoveData.Intervals.Contains(evade) && removeMove.MoveData.Intervals.Count == 2,
                    "Guarded native interval removal did not apply.");
            Check(ReferenceEquals(removeMove.MoveData.Intervals[1], evade),
                "Removed interval did not return to its original position.");
            Reject(new[] { removeMove }, new[] { new MoveCombatPatch(Owner, "Remove", removeInterval:
                new ModMoveIntervalRemoval("Evade", "Invulnerable", 0, 46)) },
                "Wrong expected interval bounds accepted.");
            Reject(new[] { removeMove }, new[] { new MoveCombatPatch(Owner, "Remove", removeInterval:
                new ModMoveIntervalRemoval("Evade", "Block", 0, 47)) },
                "Wrong expected interval type accepted.");
        }
        foreach (bool initialized in new[] { false, true })
        {
            var boundsMove = Move("Bounds", initialized);
            var interval = boundsMove.MoveData.Intervals[0];
            var originalNode = interval.NodeInterval;
            using (Apply(new[] { boundsMove }, new MoveCombatPatch(Owner, "Bounds",
                intervalStart: new ModMoveFramePatch("Uninterrupt", 2, 0),
                intervalEnd: new ModMoveFramePatch("Uninterrupt", 42, 35))))
            {
                if (initialized)
                    Check(interval.Start == 0 && interval.EndFrame == 35, "Parsed interval bounds did not apply together.");
                else
                    Check(interval.NodeInterval.Attributes["Start"].Value == "0" &&
                        interval.NodeInterval.Attributes["End"].Value == "35" &&
                        originalNode.Attributes["Start"].Value == "2", "Deferred interval bounds did not apply together.");
            }
            if (initialized)
                Check(interval.Start == 2 && interval.EndFrame == 42, "Parsed interval bounds did not roll back.");
            else
                Check(ReferenceEquals(interval.NodeInterval, originalNode), "Deferred interval bounds did not roll back.");
        }
        Reject(new[] { Move() }, new[] { new MoveCombatPatch(Owner, "Test",
            intervalStart: new ModMoveFramePatch("Uninterrupt", 9, 0)) }, "Wrong expected start accepted.");
        Reject(new[] { Move() }, new[] { new MoveCombatPatch(Owner, "Test",
            intervalStart: new ModMoveFramePatch("Uninterrupt", 2, 50)) }, "Start beyond end accepted.");
        var batch = Move("Batch");
        Reject(new[] { disabled, batch }, new[] { new MoveCombatPatch(Owner, "Disabled", disable: true),
            new MoveCombatPatch(Owner, "Batch", intervalEnd: new ModMoveFramePatch("Uninterrupt", 41, 40)) },
            "Late validation failure partially disabled a move.");
        Check(disabled.SelectionConditions.SequenceEqual(new[] { original }), "Failed batch left a disabled move.");
        Console.WriteLine("PASS: "+count+" production move-patch batch/rollback checks. Controlled native containers exercise deferred and parsed intervals; native Unity acceptance is separate.");
    }
}

public class Model { }
public class ModelConditions { }
public class ConditionAnimation
{
    public enum ConditionType { NONE }
    public ConditionAnimation(ConditionType type = ConditionType.NONE) { }
    public virtual bool IsEqual(ModelConditions conditions) => true;
    public virtual bool IsEqual(Model model, InfoAnimation animation) => true;
}
public class ConditionKeys : ConditionAnimation
{
    public string Key;
    public ConditionKeys(string key) { Key = key; }
    public bool HasSameKeyRequirementAs(ConditionKeys other) => other != null && Key == other.Key;
}
public class InfoAnimation
{
    public string Name,FileName; public int Priority,AnimationEndFrame; public Data MoveData=new Data();
    public List<ConditionAnimation> SelectionConditions=new List<ConditionAnimation>();
    public List<ActionAnimation> ScheduledActions=new List<ActionAnimation>();
    public class Data { public List<IntervalAnimation> Intervals=new List<IntervalAnimation>(); }
    public void ReplaceClip(string fileName,int endFrame)
    { if(fileName.Contains("missing")) throw new InvalidOperationException("Missing clip");FileName=fileName;AnimationEndFrame=endFrame==0?28:endFrame; }
}
public class IntervalAnimation
{
    public enum IntervalType { INTERVAL_NONE, INTERVAL_UNINTERRUPT, INTERVAL_INVULNERABLE }
    public IntervalType Type;
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
