using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Eclipse.Modding;

namespace Eclipse.Modding
{
    public sealed class ModHost
    {
        public IReadOnlyList<ModDescriptor> EnabledMods { get; }
        public AssetResolver Assets { get; }
        public ModHost(IEnumerable<ModDescriptor> mods)
        {
            var order = DependencyResolver.Resolve(mods.ToArray(), ModPlatformVersions.Core);
            if (order.HasErrors) throw new Exception(string.Join("; ", order.Diagnostics));
            EnabledMods = order.OrderedMods;
            Assets = new AssetResolver(EnabledMods.Select(value => (IAssetProvider)new LooseModProvider(value)));
        }
    }
}

static class Program
{
    static int checks;
    static readonly ModId Owner = ModId.Parse("diagnostics.fixture");
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Observe(ModCallbackDiagnostics diagnostics, string source, string error = null, bool budget = false)
    { var ticket = diagnostics.Begin(); diagnostics.End(ticket, Owner, source, budget ? 4 : 0, budget, error); }
    static void Main(string[] args)
    {
        BoundsAndSnapshots();
        RealLua(args[0]);
        Console.WriteLine("PASS: " + checks + " callback diagnostics checks (production Lua, nested framework calls, budgets, recovery, bounded histories and immutable snapshots).");
    }

    static void BoundsAndSnapshots()
    {
        var diagnostics = new ModCallbackDiagnostics();
        Observe(diagnostics, "success");
        Check(diagnostics.TimedCalls == 0 && diagnostics.Statistics.Count == 0 && diagnostics.RecentCalls.Count == 0, "Disabled success should retain nothing");
        long allocated = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 10000; i++) Observe(diagnostics, "success");
        Check(GC.GetAllocatedBytesForCurrentThread() == allocated, "Disabled success path allocated diagnostic records");
        Observe(diagnostics, "failure", "broken\nline");
        Check(diagnostics.FailureCount == 1 && !diagnostics.RecentFailures[0].Timed && diagnostics.RecentFailures[0].Error == "broken line", "Untimed failure missing or multiline");
        diagnostics.Recording = true;
        var outer = diagnostics.Begin();
        var inner = diagnostics.Begin();
        diagnostics.End(inner, Owner, "inner", 1, false, null);
        diagnostics.End(outer, Owner, "outer", 0, false, null);
        var child = diagnostics.RecentCalls[0]; var parent = diagnostics.RecentCalls[1];
        Check(child.ParentSequence == parent.Sequence && child.Depth == 1 && parent.Depth == 0, "Nested attribution incorrect");
        Check(parent.TotalMilliseconds >= child.TotalMilliseconds && parent.SelfMilliseconds <= parent.TotalMilliseconds - child.TotalMilliseconds + .000001, "Nested wall time double counted as self");
        var saved = diagnostics.Statistics;
        Observe(diagnostics, "outer");
        Check(saved.Single(row => row.Source == "outer").TimedCalls == 1 && diagnostics.Statistics.Single(row => row.Source == "outer").TimedCalls == 2, "Statistics snapshot changed after observation");
        for (int i = 0; i < 200; i++) Observe(diagnostics, "ring", "error" + i);
        Check(diagnostics.RecentCalls.Count == 128 && diagnostics.RecentFailures.Count == 64, "History not bounded");
        Check(diagnostics.RecentFailures[0].Error == "error136" && diagnostics.RecentFailures[63].Error == "error199", "Error ring order incorrect");
        var retained = diagnostics.RecentFailures;
        Observe(diagnostics, "ring", "new");
        Check(retained[63].Error == "error199", "Trace snapshot changed");
        Observe(diagnostics, "huge", new string('x', 8000));
        Check(diagnostics.RecentFailures.Last().Error.Length == 2048, "Unbounded diagnostic error");
        Observe(diagnostics, "budget", "instruction limit", true);
        Check(diagnostics.BudgetExceededCount == 1 && diagnostics.RecentFailures.Last().BudgetExceeded, "Budget classification missing");
        Observe(diagnostics, "fake budget", "Execution instruction budget exceeded");
        Check(diagnostics.BudgetExceededCount == 1 && !diagnostics.RecentFailures.Last().BudgetExceeded, "Message text forged an instruction-limit classification");
        var scopes = new List<long>();
        for (int i = 0; i < 65; i++) scopes.Add(diagnostics.Begin());
        Check(scopes.Last() == 0 && diagnostics.UntrackedCalls == 1, "Nested recording depth not bounded");
        for (int i = scopes.Count - 1; i >= 0; i--) diagnostics.End(scopes[i], Owner, "depth", 0, false, null);
        Observe(diagnostics, "recovered");
        Check(diagnostics.RecentCalls.Last().Depth == 0, "Depth overflow did not recover");
        var many = new ModCallbackDiagnostics { Recording = true };
        for (int i = 0; i < 1100; i++) Observe(many, "callback" + i);
        Check(many.Statistics.Count == 1024 && many.UntrackedTimings == 76 && many.TimedCalls == 1100, "Statistics capacity/overflow not visible");
        var culture = CultureInfo.CurrentCulture;
        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
            Check(diagnostics.FormatReport().Contains("total_ms=0.") && !diagnostics.FormatReport().Contains("total_ms=0,"), "Report numbers depend on player culture");
        } finally { CultureInfo.CurrentCulture = culture; }
        var before = diagnostics.TimedCalls;
        var pending = diagnostics.Begin(); diagnostics.Recording = false;
        diagnostics.End(pending, Owner, "toggle", 0, false, null);
        Observe(diagnostics, "disabled again");
        Check(diagnostics.TimedCalls == before + 1, "Recording toggle damaged active measurement or recorded disabled success");
    }

    static void Write(string folder, string id, string body, string caps, string dependency = null)
    {
        var path = Path.Combine(folder, id);
        Directory.CreateDirectory(Path.Combine(path, "scripts"));
        File.WriteAllText(Path.Combine(path, "scripts/main.lua"), "local sf2=require('sf2')\n" + body);
        File.WriteAllText(Path.Combine(path, "mod.toml"),
            "schema=1\nid=\"" + id + "\"\nname=\"Fixture\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[" + caps + "]\n" +
            (dependency == null ? "" : "[[dependencies]]\nid=\"" + dependency + "\"\nversion=\">=1.0.0 <2.0.0\"\n"));
    }

    static void RealLua(string root)
    {
        Write(root, "diagnostics.provider", "sf2.extensions.register{id='work',version=1,request={n='integer'},response={value='integer'},handler=function(r) " +
            "if r.n==2 then error('intentional provider failure') end; if r.n==3 then while true do end end; " +
            "local value=0; for i=1,500 do value=value+i end; return {value=value} end}", "\"extensions.provide\"");
        Write(root, "diagnostics.consumer", "local work=sf2.extensions.get('diagnostics.provider:extensions/work',1); " +
            "sf2.ui.open{id='probe',mount='menu',root={id='go',kind='button',width=100,height=40,text='Go'}, " +
            "on_click=function(view,id) local n=tonumber(string.sub(id,2)); local result=sf2.extensions.try_call(work,{n=n}); if n==1 then assert(result.value==125250) end end}",
            "\"extensions.call\",\"ui.create\"", "diagnostics.provider");
        // The real UI callback is invoked through its public surface, with legal
        // authored IDs for success, failure and budget cases.
        var consumerFile = Path.Combine(root, "diagnostics.consumer/scripts/main.lua");
        var consumer = File.ReadAllText(consumerFile).Replace("root={id='go',kind='button',width=100,height=40,text='Go'}",
            "root={id='root',kind='column',width=120,height=160,children={{id='b1',kind='button',width=100,height=40,text='Good'},{id='b2',kind='button',width=100,height=40,text='Fail'},{id='b3',kind='button',width=100,height=40,text='Budget'}}}");
        File.WriteAllText(consumerFile, consumer);
        var discovery = ModDiscovery.DiscoverLoose(root);
        Check(!discovery.HasErrors, "Fixture discovery failed");
        var host = new ModHost(discovery.Mods);
        var views = new List<ModUiSurface>();
        var diagnostics = new ModCallbackDiagnostics { Recording = true };
        using var session = ModScriptSession.Start(host, new MoonSharpScriptRuntime(views.Add), null, null, diagnostics);
        Check(!session.HasErrors && ReferenceEquals(session.CallbackDiagnostics, diagnostics), "Real Lua failed to load or diagnostics not shared: " + session.FormatReport());
        Check(diagnostics.TimedCalls == 2 && diagnostics.Statistics.Select(row => row.Owner.Value).Distinct().Count() == 2, "Entrypoints missing owners/timing");
        Check(views[0].TryClick("b1"), "Success callback did not execute");
        var recent = diagnostics.RecentCalls.TakeLast(2).ToArray();
        Check(recent[0].Owner.Value == "diagnostics.provider" && recent[1].Owner.Value == "diagnostics.consumer", "Cross-mod callbacks attributed to caller instead of owner");
        Check(recent[0].Source.EndsWith(":handler") && recent[1].Source.EndsWith(":on_click") && recent[0].ParentSequence == recent[1].Sequence, "Cross-mod trace lost parent/source");
        Check(views[0].TryClick("b2"), "Handled provider error broke consumer");
        Check(diagnostics.FailureCount == 1 && diagnostics.RecentFailures[0].Error.Contains("intentional provider failure"), "Actual Lua failure absent");
        Check(views[0].TryClick("b3"), "Handled provider instruction limit broke consumer");
        Check(diagnostics.BudgetExceededCount == 1 && diagnostics.RecentFailures.Last().ForcedYields == 4, "Actual budget limit not classified");
        Check(views[0].TryClick("b1") && diagnostics.RecentCalls.Last().Error == null, "Worker did not recover after budget failure");
        var before = diagnostics.TimedCalls;
        diagnostics.Recording = false;
        Check(views[0].TryClick("b1") && diagnostics.TimedCalls == before, "Disabled timing still recorded success");
        Check(views[0].TryClick("b2") && diagnostics.FailureCount == 3 && !diagnostics.RecentFailures.Last().Timed, "Disabled timing lost failure");
        Check(diagnostics.FormatReport().Contains("diagnostics.provider:extensions/work:handler") && diagnostics.FormatReport().Contains("BUDGET_EXCEEDED"), "Export lacks useful callback attribution");
        File.WriteAllText(Path.Combine(root, "callback-report.txt"), diagnostics.FormatReport());
        session.Dispose();
        Check(views.All(view => view.IsClosed), "Diagnostics damaged session teardown");
        // A new session starts clean; the retained old snapshot remains readable.
        using var next = ModScriptSession.Start(host, new MoonSharpScriptRuntime(views.Add), null, null);
        Check(!next.HasErrors && next.CallbackDiagnostics.FailureCount == 0 && next.CallbackDiagnostics.TimedCalls == 0, "New session inherited stale measurements: " + next.FormatReport());
    }
}
