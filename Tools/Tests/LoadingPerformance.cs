// Managed regression and warm-cache benchmark fixture. Production quest/TAR sources
// compile directly; move table methods are extracted with a controlled key collector.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Eclipse.Modding;
using Eclipse.Content.TarAssets;

public sealed class KeyData
{
    public int Mask;
    public bool IsVariable(KeyData other) => (Mask & other.Mask) == other.Mask;
}
public sealed class ConditionKeys { public KeyData RequiredKeys = new KeyData(); }
public sealed class InfoAnimation
{
    public sealed class CapabilityTable { public List<InfoAnimation> HigherPriorityMoves = new List<InfoAnimation>(); }
    public int Priority;
    public CapabilityTable PriorityConflicts = new CapabilityTable();
    public List<ConditionKeys> Keys = new List<ConditionKeys>();
    public static int Collections;
    public List<ConditionKeys> MOPMGFIIFGA() { Collections++; return new List<ConditionKeys>(Keys); }
    public List<ConditionKeys> CollectKeyConditions() => MOPMGFIIFGA();
}
static class Program
{
    static int checks;
    internal static bool AllocationCounterAvailable()
    {
        long before = GC.GetAllocatedBytesForCurrentThread();
        GC.KeepAlive(new byte[1024]);
        return GC.GetAllocatedBytesForCurrentThread() > before;
    }
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static Dictionary<DefinitionId, string> Quests(ModContentCatalog catalog) =>
        (Dictionary<DefinitionId, string>)typeof(ModContentCatalog).GetField("_coreQuestSources", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(catalog);
    static string[] Snapshot(ModContentCatalog catalog) => Quests(catalog).Select(p => p.Key + "=" + p.Value).OrderBy(x => x, StringComparer.Ordinal).ToArray();
    static void QuestCase(string folder, string xml)
    {
        File.WriteAllText(Path.Combine(folder, "test.xml"), xml);
        var old = new ModContentCatalog(); var now = new ModContentCatalog();
        Exception a = null, b = null;
        try { BaselineQuests.ImportQuestSources(old, folder); } catch (Exception e) { a = e; }
        try { CoreContentImporter.ImportQuestSources(now, folder); } catch (Exception e) { b = e; }
        Check(a?.GetType() == b?.GetType(), "Quest rejection differs: " + xml);
        Check(Snapshot(old).SequenceEqual(Snapshot(now)), "Quest identities differ: " + xml);
        if (b != null) Check(Quests(now).Count == 0, "Rejected quest file changed the catalog");
    }
    static void Bench(string label, Action old, Action now, int iterations = 7)
    {
        old(); now();
        var a = new List<double>(); var b = new List<double>();
        var aa = new List<long>(); var bb = new List<long>();
        void Sample(Action action, List<double> times, List<long> bytes)
        {
            long before = GC.GetAllocatedBytesForCurrentThread(); var watch = Stopwatch.StartNew();
            action(); watch.Stop(); times.Add(watch.Elapsed.TotalMilliseconds);
            bytes.Add(GC.GetAllocatedBytesForCurrentThread() - before);
        }
        for (int i = 0; i < iterations; i++)
            if (i % 2 == 0) { Sample(old, a, aa); Sample(now, b, bb); }
            else { Sample(now, b, bb); Sample(old, a, aa); }
        a.Sort(); b.Sort(); aa.Sort(); bb.Sort();
        string allocation = AllocationCounterAvailable() ? $"; allocated {aa[iterations/2]:N0} -> {bb[iterations/2]:N0} B" : "; allocation counter unavailable";
        Console.WriteLine($"{label}: baseline {a[iterations/2]:F2} ms; current {b[iterations/2]:F2} ms{allocation} (median, warm cache)");
    }
    static string[] MoveSnapshot(List<InfoAnimation> moves) => moves.Select(m => string.Join(",", m.PriorityConflicts.HigherPriorityMoves.Select(x => moves.IndexOf(x)))).ToArray();
    static void Reset(List<InfoAnimation> moves) { foreach (var move in moves) move.PriorityConflicts.HigherPriorityMoves.Clear(); InfoAnimation.Collections = 0; }
    static void Moves()
    {
        var random = new Random(1234); var moves = new List<InfoAnimation>();
        for (int i = 0; i < 600; i++)
        {
            var move = new InfoAnimation { Priority = random.Next(12) };
            for (int k = random.Next(5); k > 0; k--) move.Keys.Add(new ConditionKeys { RequiredKeys = new KeyData { Mask = random.Next(1, 32) } });
            moves.Add(move);
        }
        BaselineMoves._Animations = CurrentMoves._Animations = moves;
        Action old = () => { Reset(moves); BaselineMoves.CreateCapabilityTables(); };
        Action now = () => { Reset(moves); CurrentMoves.CreateCapabilityTables(); };
        old(); var snapshot = MoveSnapshot(moves); int oldCollections = InfoAnimation.Collections;
        now(); Check(snapshot.SequenceEqual(MoveSnapshot(moves)), "Move priority conflicts or order changed");
        Check(InfoAnimation.Collections == moves.Count, "Move keys were collected more than once per rebuild");
        Console.WriteLine($"Move key collections: {oldCollections:N0} -> {InfoAnimation.Collections:N0}");
        // Rebuilds must observe patches, including moves that gain or lose keys.
        for (int i = 0; i < moves.Count; i += 3) { moves[i].Keys.Clear(); moves[i].Priority = 20; }
        old(); snapshot = MoveSnapshot(moves); now();
        Check(snapshot.SequenceEqual(MoveSnapshot(moves)), "Move rebuild reused stale key conditions");
        Reset(moves); BaselineMoves.CreateCapabilityTable(moves[1], moves.GetRange(0, 100)); snapshot = MoveSnapshot(moves);
        Reset(moves); CurrentMoves.CreateCapabilityTable(moves[1], moves.GetRange(0, 100));
        Check(snapshot.SequenceEqual(MoveSnapshot(moves)), "Incremental move table changed");
        Bench("600-move table rebuild (controlled key collector)", old, now);
    }
    static void Tar(string folder)
    {
        string path = Path.Combine(folder, "fixture.tar");
        var expected = new[] { "large-" + new string('x', 65536), "small", "", "Zażółć gęślą jaźń 🥷" };
        using (var stream = File.Create(path)) using (var writer = new TarWriter(stream))
            for (int i = 0; i < expected.Length; i++) writer.AddFile(i + ".meta", Encoding.UTF8.GetBytes(expected[i]));
        var tar = TarArchive.Open(path); var baseline = BaselineTarArchive.Open(path);
        var entries = tar.Entries.OrderBy(e => e.Name, StringComparer.Ordinal).ToArray();
        using (var reader = tar.OpenReader())
            for (int pass = 0; pass < 3; pass++) for (int i = 0; i < entries.Length; i++)
                Check(reader.ReadText(entries[i]) == expected[i], "Buffered TAR text changed or retained old bytes");
        Check(!tar.ContainsNonEmptyFile("2.meta") && tar.ContainsNonEmptyFile("3.meta") && !tar.ContainsNonEmptyFile("absent"), "TAR text presence changed");
        var oldEntries = baseline.Entries.ToArray();
        Bench("TAR repeated metadata reads", () => { using (var reader = baseline.OpenReader()) for (int pass = 0; pass < 100; pass++) foreach (var entry in oldEntries) reader.ReadText(entry); },
            () => { using (var reader = tar.OpenReader()) for (int pass = 0; pass < 100; pass++) foreach (var entry in entries) reader.ReadText(entry); });
    }
    static int Main(string[] args)
    {
        string folder = Path.Combine(args[0], "Temp", "LoadingPerformance", "Cases"); Directory.CreateDirectory(folder);
        foreach (string xml in new[] {
            "<Root><Quest Name='A'/><Quests/><Quest Name='B'/><Quests><Quest Name='C'><Quest Name='Ignored'/></Quest></Quests><Quest Name='D'/></Root>",
            "<Quests><Quest Name='A'/><Other><Quest Name='Ignored'/></Other><Quest Name='B'/></Quests>",
            "<Root xmlns='urn:x'><Quest Name='Ignored'/><Quest xmlns='' Name='AlsoIgnored'/></Root>",
            "<Root><Quests xmlns='urn:x'><Quest xmlns='' Name='Ignored'/></Quests><Quests><Quest Name='A'/></Quests></Root>",
            "<Root><Other><Quests><Quest Name='Ignored'/></Quests></Other><Quest Name='A'/></Root>",
            "<Other><Quest Name='Ignored'/></Other>", "<Root/>", "<Quests/>", "<Root><Quest/></Root>",
            "<Root><Quest Name='A#B'/></Root>", "<Root><Quest Name='A'/><Quest Name='a'/></Root>",
            "<Root><Quest Name='A'/><Quest Name='A'/></Root>", "<Root><Other><Broken></Other></Root>",
            "<Root><Quest Name='A'><Broken></Quest></Root>", "<Other><Broken></Other>", "<Root/><Root/>",
            "<!DOCTYPE Root [<!ENTITY x 'x'>]><Root/>", "<!--comment--><Root><Quests><!--comment--><Quest Name='A'/></Quests></Root>", "" }) QuestCase(folder, xml);
        File.Delete(Path.Combine(folder, "test.xml"));
        string vanilla = Path.Combine(args[0], "Assets", "vanillaXml");
        var old = new ModContentCatalog(); var now = new ModContentCatalog();
        BaselineQuests.ImportQuestSources(old, vanilla); CoreContentImporter.ImportQuestSources(now, vanilla);
        Check(Snapshot(old).SequenceEqual(Snapshot(now)), "Canonical vanilla quest identities differ");
        Console.WriteLine($"Canonical quest identities: {Quests(now).Count:N0}, identical");
        Bench("Canonical quest scan", () => BaselineQuests.ImportQuestSources(new ModContentCatalog(), vanilla), () => CoreContentImporter.ImportQuestSources(new ModContentCatalog(), vanilla));
        Moves(); Tar(folder);
        var resolver = new AssetResolver(Array.Empty<IAssetProvider>()); var id = AssetId.Parse("core:models/test");
        resolver.Resolve(id); long bytes = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 10000; i++) if (resolver.Resolve(id) != id) throw new Exception("Unredirected ID changed");
        if (AllocationCounterAvailable())
        {
            bytes = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 10000; i++) resolver.Resolve(id);
            Check(GC.GetAllocatedBytesForCurrentThread() == bytes, "Unredirected lookups allocate");
        }
        var next = AssetId.Parse("core:models/next"); var last = AssetId.Parse("core:models/last");
        ModAssetReplacement Redirect(AssetId target, AssetId replacement) => new ModAssetReplacement(ModId.Parse("fixture"), target, replacement, AssetKind.Model);
        resolver.SetReplacements(new[] { Redirect(id, next), Redirect(next, last) });
        Check(resolver.Resolve(id) == last && resolver.Resolve(next) == last, "Redirect chain changed");
        bool rejected = false;
        try { resolver.SetReplacements(new[] { Redirect(id, next), Redirect(next, id) }); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && resolver.Resolve(id) == last, "Rejected redirect cycle changed the previous graph");
        Console.WriteLine($"PASS: {checks} loading regression checks. No Unity gameplay or native sprite loading tested."); return 0;
    }
}
