$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$animationSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/AnimationData.cs')
$methods=[regex]::Match($animationSource,'(?ms)^\tinternal sealed class ExternalMoveReplacementLifetime.*?^\t\}').Value
$methods += [regex]::Match($animationSource,'(?ms)^\tinternal static ExternalMoveReplacementLifetime ReplaceExternalMoves\(.*?^\t\}').Value
$methods += [regex]::Match($animationSource,'(?ms)^\tprivate static void SwapTemplateMember\(.*?^\t\}').Value
$methods += [regex]::Match($animationSource,'(?ms)^\tinternal static void RebuildCapabilityTables\(.*?^\t\}').Value
$methods += [regex]::Match($animationSource,'(?ms)^\tpublic static void CreateCapabilityTables\(.*?^\t\}').Value
$methods += [regex]::Match($animationSource,'(?ms)^\tpublic static void CreateCapabilityTable\(.*?^\t\}').Value
if(($methods -split 'ReplaceExternalMoves\(').Count -lt 2 -or $methods -notmatch 'SwapTemplateMember\(' -or $methods -notmatch 'class ExternalMoveReplacementLifetime'){throw 'Move replacement extraction failed.'}
$templateSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/TemplateAnimation.cs')
$fixture=Join-Path $root ('Temp/MoveReplacementTemplates-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Execute the production native move replacement lifetime and the real
// TemplateAnimation membership code. The moves parser is controlled: it keeps the
// native side effects that matter here (declared templates gain the parsed move,
// and a per-move template is added only when its name is new).
static class ListExtension
{
    public static int AddIfNotExist<T>(this List<T> list, T item)
    { int index = list.IndexOf(item); if (index >= 0) return index; list.Add(item); return list.Count - 1; }
}
static class XmlUtils { public static string ParseString(XmlAttribute attribute) => attribute?.Value ?? string.Empty; }
class Trick { }
class Trigger { }
public class KeyData { public bool IsVariable(KeyData other) => false; }
public class ConditionKeys { public KeyData RequiredKeys = new KeyData(); }
public class InfoAnimation
{
    public class CapabilityTable { public List<InfoAnimation> HigherPriorityMoves = new List<InfoAnimation>(); }
    public string Name, FileName;
    public int Priority;
    public CapabilityTable PriorityConflicts = new CapabilityTable();
    public readonly List<string> TemplateNames = new List<string>();
    public List<ConditionKeys> MOPMGFIIFGA() => new List<ConditionKeys>();
    public void AddTemplateName(string name) { if (!TemplateNames.Contains(name)) TemplateNames.Add(name); }
}
static class MovesParser
{
    internal static int ParseAdditional(XmlDocument document, List<InfoAnimation> moves,
        Dictionary<string, TemplateAnimation> templates, List<Trick> tricks, List<Trigger> triggers)
    {
        int added = 0;
        foreach (XmlNode node in document["Movesxml"]["Moves"].ChildNodes)
        {
            var move = new InfoAnimation { Name = node.Attributes["Name"].Value, FileName = node.Attributes["File"].Value };
            foreach (XmlNode template in node.SelectNodes("Template"))
                if (templates.TryGetValue(template.Attributes["Name"].Value, out var owner)) owner.MBJCDIDIBDJ(move);
            if (!templates.ContainsKey(move.Name)) templates.Add(move.Name, new TemplateAnimation(move));
            moves.Add(move); added++;
        }
        return added;
    }
}
TEMPLATE_SOURCE
static class AnimationData
{
    private static readonly Dictionary<string, TemplateAnimation> _TemplatesByName = new Dictionary<string, TemplateAnimation>();
    private static readonly Dictionary<string, InfoAnimation> _AnimationsByName = new Dictionary<string, InfoAnimation>();
    private static readonly List<InfoAnimation> _Animations = new List<InfoAnimation>();
    private static readonly List<string> _WeaponTypeList = new List<string>();
    public static Dictionary<string, TemplateAnimation> Templates => _TemplatesByName;
    public static List<InfoAnimation> Animations => _Animations;
    public static void Reset()
    { _TemplatesByName.Clear(); _AnimationsByName.Clear(); _Animations.Clear(); }
    public static InfoAnimation Add(string name, string file, params string[] templates)
    {
        var move = new InfoAnimation { Name = name, FileName = file };
        foreach (string template in templates)
        {
            if (!_TemplatesByName.TryGetValue(template, out var owner))
            {
                var node = new XmlDocument().CreateElement("Template"); node.SetAttribute("Name", template);
                owner = new TemplateAnimation(node); _TemplatesByName.Add(template, owner);
            }
            owner.MBJCDIDIBDJ(move);
        }
        _TemplatesByName.Add(name, new TemplateAnimation(move));
        _Animations.Add(move); _AnimationsByName.Add(name, move);
        return move;
    }
    public static InfoAnimation Get(string name) => _AnimationsByName[name];
REPLACEMENT_METHODS
}
static class Program
{
    static int checks; static readonly List<string> failures = new List<string>();
    static void Check(bool condition, string message) { checks++; if (!condition) failures.Add(message); }
    static List<InfoAnimation> Members(string template) => AnimationData.Templates[template].LDEBJOPLCKO();
    static XmlDocument Document(params (string name, string file, string[] templates)[] moves)
    {
        var document = new XmlDocument();
        var root = document.AppendChild(document.CreateElement("Movesxml"));
        var list = root.AppendChild(document.CreateElement("Moves"));
        foreach (var move in moves)
        {
            var node = (XmlElement)list.AppendChild(document.CreateElement("Move"));
            node.SetAttribute("Name", move.name); node.SetAttribute("File", move.file);
            foreach (string template in move.templates)
                ((XmlElement)node.AppendChild(document.CreateElement("Template"))).SetAttribute("Name", template);
        }
        return document;
    }
    static Dictionary<string, string[]> Snapshot() => AnimationData.Templates.ToDictionary(
        pair => pair.Key, pair => pair.Value.LDEBJOPLCKO().Select(move => move.Name + "@" + move.FileName).ToArray());
    static bool Same(Dictionary<string, string[]> left, Dictionary<string, string[]> right) =>
        left.Count == right.Count && left.All(pair => right.TryGetValue(pair.Key, out var other) && pair.Value.SequenceEqual(other));
    static void Setup()
    {
        AnimationData.Reset();
        AnimationData.Add("Before", "before.bytes", "1key", "BossAbility");
        AnimationData.Add("Target", "target.bytes", "1key", "BossAbility", "OnlyOriginal");
        AnimationData.Add("After", "after.bytes", "1key", "OnlyReplacement");
    }
    static int Main()
    {
        Setup();
        var original = AnimationData.Get("Target");
        var baseline = Snapshot();
        var lifetime = AnimationData.ReplaceExternalMoves(Document(("Target", "new.bytes", new[] { "1key", "BossAbility", "OnlyReplacement" })),
            new Dictionary<string, string> { ["Target"] = "target.bytes" });
        var replacement = AnimationData.Get("Target");
        Check(!ReferenceEquals(replacement, original) && replacement.FileName == "new.bytes", "Replacement was not installed.");
        Check(Members("1key").Select(m => m.Name).SequenceEqual(new[] { "Before", "Target", "After" }) &&
            ReferenceEquals(Members("1key")[1], replacement), "Shared template did not keep native order with the replacement.");
        Check(ReferenceEquals(Members("BossAbility")[1], replacement) && Members("BossAbility").Count == 2,
            "Second shared template kept the original or a duplicate.");
        Check(Members("Target").Count == 1 && ReferenceEquals(Members("Target")[0], replacement),
            "Per-move template still resolves the original move.");
        Check(Members("OnlyOriginal").Count == 0, "Template only the original declared still lists it.");
        Check(Members("OnlyReplacement").Count == 2 && ReferenceEquals(Members("OnlyReplacement")[1], replacement),
            "Template only the replacement declared does not list it.");
        Check(AnimationData.Templates.Values.All(t => !t.LDEBJOPLCKO().Contains(original)), "Original remains in a live template.");
        lifetime.Dispose();
        Check(ReferenceEquals(AnimationData.Get("Target"), original), "Dispose did not restore the original move.");
        Check(Same(baseline, Snapshot()) && Members("1key").Count == 3 && ReferenceEquals(Members("1key")[1], original) &&
            ReferenceEquals(Members("OnlyOriginal")[0], original) && Members("OnlyReplacement").Count == 1,
            "Dispose did not restore native template membership and order.");
        lifetime.Dispose();
        Check(Same(baseline, Snapshot()), "Second dispose changed templates.");

        foreach (var failing in new[] {
            (Document(("Target", "new.bytes", new[] { "1key" })), new Dictionary<string, string> { ["Target"] = "wrong.bytes" }, "expected filename mismatch"),
            (Document(("Missing", "new.bytes", new[] { "1key" })), new Dictionary<string, string> { ["Target"] = "target.bytes" }, "unguarded target"),
            (Document(("Target", "new.bytes", new[] { "1key" }), ("After", "b.bytes", new[] { "BossAbility" })),
                new Dictionary<string, string> { ["Target"] = "target.bytes" }, "count mismatch") })
        {
            Setup(); var before = Snapshot(); bool rejected = false;
            try { AnimationData.ReplaceExternalMoves(failing.Item1, failing.Item2); } catch (InvalidOperationException) { rejected = true; }
            Check(rejected, "Replacement accepted a " + failing.Item3 + ".");
            Check(Same(before, Snapshot()), "Rejected " + failing.Item3 + " left parsed moves in live templates.");
        }
        foreach (string failure in failures) Console.Error.WriteLine("FAIL: " + failure);
        if (failures.Count != 0) { Console.Error.WriteLine(failures.Count + " of " + checks + " move replacement template checks failed."); return 1; }
        Console.WriteLine("PASS: " + checks + " production native move replacement template checks. Actual replacement lifetime, template membership swap/restore and rejected-batch rollback execute with the real TemplateAnimation; the moves parser is controlled and no Unity gameplay runs.");
        return 0;
    }
}
'@
$code=$code.Replace('TEMPLATE_SOURCE',$templateSource.Replace('using System.Collections.Generic;','').Replace('using System.Xml;','')).Replace('REPLACEMENT_METHODS',$methods)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>CS0649;CS0414</NoWarn></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Move replacement template checks failed.'}
Remove-Item -LiteralPath $fixture -Recurse -Force
