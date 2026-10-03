using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Eclipse.Modding
{
    /// <summary>Session-owned, main-thread Lua observations. Never retains Lua values or native handles.</summary>
    public sealed class ModCallbackDiagnostics
    {
        public const int MaximumCallbacks = 1024;
        public const int TraceCapacity = 128;
        public const int FailureCapacity = 64;
        public const int MaximumMessageLength = 2048;
        private const int MaximumDepth = 64;
        private readonly long _origin = Stopwatch.GetTimestamp();
        private readonly Dictionary<(ModId, string), Counters> _counters = new Dictionary<(ModId, string), Counters>();
        private readonly List<ActiveCall> _active = new List<ActiveCall>();
        private readonly ModCallbackTrace[] _trace = new ModCallbackTrace[TraceCapacity];
        private readonly ModCallbackTrace[] _failures = new ModCallbackTrace[FailureCapacity];
        private long _sequence;
        private int _traceNext, _traceCount, _failureNext, _failureCount;

        public bool Recording { get; set; }
        public long TimedCalls { get; private set; }
        public long FailureCount { get; private set; }
        public long BudgetExceededCount { get; private set; }
        public long UntrackedCalls { get; private set; }
        public long UntrackedTimings { get; private set; }

        private sealed class Counters
        {
            public long Calls, Failures, Budgets;
            public double Total, Self, Maximum;
        }

        private struct ActiveCall
        {
            public long Sequence, Parent, Started;
            public int Depth;
            public double Children;
        }

        // Zero is a disabled measurement. Keeping the ticket a value avoids an
        // IDisposable/Stopwatch allocation on every combat tick.
        public long Begin()
        {
            if (!Recording) return 0;
            if (_active.Count >= MaximumDepth) { UntrackedCalls++; return 0; }
            long sequence = ++_sequence;
            _active.Add(new ActiveCall {
                Sequence = sequence, Parent = _active.Count == 0 ? 0 : _active[_active.Count - 1].Sequence,
                Depth = _active.Count, Started = Stopwatch.GetTimestamp()
            });
            return sequence;
        }

        public void End(long ticket, ModId owner, string source, int forcedYields, bool budgetExceeded, string error)
        {
            bool timed = ticket != 0;
            bool failed = error != null;
            if (!timed && !failed) return;
            long sequence = ticket, parent = 0;
            int depth = 0;
            double elapsed = 0, self = 0;
            if (timed)
            {
                int last = _active.Count - 1;
                if (last < 0 || _active[last].Sequence != ticket)
                    throw new InvalidOperationException("Callback diagnostic measurements must end in nesting order.");
                var call = _active[last];
                elapsed = Milliseconds(Stopwatch.GetTimestamp() - call.Started);
                self = Math.Max(0, elapsed - call.Children);
                parent = call.Parent; depth = call.Depth;
                _active.RemoveAt(last);
                if (last > 0)
                {
                    var enclosing = _active[last - 1];
                    enclosing.Children += elapsed;
                    _active[last - 1] = enclosing;
                }
                TimedCalls++;
            }
            else sequence = ++_sequence;
            source = Clip(source, 512);
            var key = (owner, source);
            if (!_counters.TryGetValue(key, out var counters) && _counters.Count < MaximumCallbacks)
                _counters.Add(key, counters = new Counters());
            if (counters != null)
            {
                if (timed) { counters.Calls++; counters.Total += elapsed; counters.Self += self; counters.Maximum = Math.Max(counters.Maximum, elapsed); }
                if (failed) counters.Failures++;
                if (budgetExceeded) counters.Budgets++;
            }
            else UntrackedTimings++;
            var record = new ModCallbackTrace(sequence, parent, depth, owner, source, timed,
                Milliseconds(Stopwatch.GetTimestamp() - _origin), elapsed, self, forcedYields, budgetExceeded,
                failed ? Clip(error, MaximumMessageLength) : null);
            if (timed) Append(_trace, ref _traceNext, ref _traceCount, record);
            if (failed) { FailureCount++; Append(_failures, ref _failureNext, ref _failureCount, record); }
            if (budgetExceeded) BudgetExceededCount++;
        }

        public IReadOnlyList<ModCallbackTrace> RecentCalls => Copy(_trace, _traceNext, _traceCount);
        public IReadOnlyList<ModCallbackTrace> RecentFailures => Copy(_failures, _failureNext, _failureCount);
        public IReadOnlyList<ModCallbackStatistics> Statistics
        {
            get
            {
                var rows = new List<ModCallbackStatistics>(_counters.Count);
                foreach (var pair in _counters)
                    rows.Add(new ModCallbackStatistics(pair.Key.Item1, pair.Key.Item2, pair.Value.Calls,
                        pair.Value.Failures, pair.Value.Budgets, pair.Value.Total, pair.Value.Self, pair.Value.Maximum));
                rows.Sort((a, b) => {
                    int order = b.SelfMilliseconds.CompareTo(a.SelfMilliseconds);
                    if (order == 0) order = StringComparer.Ordinal.Compare(a.Owner.Value, b.Owner.Value);
                    return order == 0 ? StringComparer.Ordinal.Compare(a.Source, b.Source) : order;
                });
                return rows.AsReadOnly();
            }
        }

        public string FormatSummary()
        {
            var text = new StringBuilder();
            text.Append("Lua: ").Append(TimedCalls).Append(" timed calls, ").Append(FailureCount).Append(" failures");
            var rows = Statistics;
            for (int i = 0; i < Math.Min(2, rows.Count); i++)
                text.Append('\n').Append(Clip(rows[i].Source.StartsWith(rows[i].Owner.Value + ":", StringComparison.Ordinal)
                    ? rows[i].Source : rows[i].Owner + "/" + rows[i].Source, 40)).Append(" self ")
                    .Append(Number(rows[i].SelfMilliseconds)).Append(" ms");
            return text.ToString();
        }

        public string FormatReport()
        {
            var text = new StringBuilder();
            text.AppendLine("Lua callback diagnostics (session totals; timing only while overlay is on)");
            text.Append("Recording: ").Append(Recording).Append("; timed calls: ").Append(TimedCalls)
                .Append("; failures: ").Append(FailureCount).Append("; budget exceeded: ").Append(BudgetExceededCount)
                .Append("; depth overflow: ").Append(UntrackedCalls).Append("; statistics overflow: ").Append(UntrackedTimings).AppendLine();
            text.AppendLine("Inclusive time includes nested callbacks and host operations; self time subtracts measured nested callbacks.");
            text.AppendLine("Timings are wall-clock observations, not instruction counts or deterministic simulation data.");
            text.AppendLine("owner | callback | timed_calls | failures | budget_exceeded | total_ms | self_ms | max_ms");
            foreach (var row in Statistics)
                text.Append(row.Owner).Append(" | ").Append(row.Source).Append(" | ").Append(row.TimedCalls)
                    .Append(" | ").Append(row.Failures).Append(" | ").Append(row.BudgetExceeded)
                    .Append(" | ").Append(Number(row.TotalMilliseconds)).Append(" | ").Append(Number(row.SelfMilliseconds))
                    .Append(" | ").Append(Number(row.MaximumMilliseconds)).AppendLine();
            text.AppendLine("Recent timed calls (oldest first; parents may already have left this bounded history):");
            foreach (var row in RecentCalls) AppendTrace(text, row);
            text.Append("Recent failures (latest ").Append(_failureCount).Append(" of ").Append(FailureCount).AppendLine(", oldest first):");
            foreach (var row in RecentFailures) AppendTrace(text, row);
            return text.ToString();
        }

        private static void AppendTrace(StringBuilder text, ModCallbackTrace row) => text.Append('#').Append(row.Sequence)
            .Append(" parent=").Append(row.ParentSequence).Append(" depth=").Append(row.Depth)
            .Append(" at_ms=").Append(Number(row.AtMilliseconds)).Append(' ').Append(row.Owner).Append(' ').Append(row.Source)
            .Append(row.Timed ? " total_ms=" + Number(row.TotalMilliseconds) + " self_ms=" + Number(row.SelfMilliseconds) : " untimed")
            .Append(" forced_yields=").Append(row.ForcedYields).Append(row.BudgetExceeded ? " BUDGET_EXCEEDED" : "")
            .Append(row.Error == null ? " OK" : " ERROR " + row.Error).AppendLine();
        private static double Milliseconds(long ticks) => ticks * (1000.0 / Stopwatch.Frequency);
        private static string Number(double number) => number.ToString("0.000", CultureInfo.InvariantCulture);
        private static string Clip(string value, int limit)
        {
            value = (value ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            return value.Length <= limit ? value : value.Substring(0, limit - 3) + "...";
        }
        private static void Append(ModCallbackTrace[] ring, ref int next, ref int count, ModCallbackTrace value)
        { ring[next] = value; next = (next + 1) % ring.Length; count = Math.Min(count + 1, ring.Length); }
        private static IReadOnlyList<ModCallbackTrace> Copy(ModCallbackTrace[] ring, int next, int count)
        {
            var rows = new ModCallbackTrace[count];
            for (int i = 0; i < count; i++) rows[i] = ring[(next - count + i + ring.Length) % ring.Length];
            return Array.AsReadOnly(rows);
        }
    }

    public sealed class ModCallbackStatistics
    {
        public ModId Owner { get; }
        public string Source { get; }
        public long TimedCalls { get; }
        public long Failures { get; }
        public long BudgetExceeded { get; }
        public double TotalMilliseconds { get; }
        public double SelfMilliseconds { get; }
        public double MaximumMilliseconds { get; }
        internal ModCallbackStatistics(ModId owner, string source, long calls, long failures, long budgets, double total, double self, double maximum)
        { Owner = owner; Source = source; TimedCalls = calls; Failures = failures; BudgetExceeded = budgets; TotalMilliseconds = total; SelfMilliseconds = self; MaximumMilliseconds = maximum; }
    }

    public sealed class ModCallbackTrace
    {
        public long Sequence { get; }
        public long ParentSequence { get; }
        public int Depth { get; }
        public ModId Owner { get; }
        public string Source { get; }
        public bool Timed { get; }
        public double AtMilliseconds { get; }
        public double TotalMilliseconds { get; }
        public double SelfMilliseconds { get; }
        public int ForcedYields { get; }
        public bool BudgetExceeded { get; }
        public string Error { get; }
        internal ModCallbackTrace(long sequence, long parent, int depth, ModId owner, string source, bool timed,
            double at, double total, double self, int yields, bool budget, string error)
        { Sequence = sequence; ParentSequence = parent; Depth = depth; Owner = owner; Source = source; Timed = timed; AtMilliseconds = at; TotalMilliseconds = total; SelfMilliseconds = self; ForcedYields = yields; BudgetExceeded = budget; Error = error; }
    }
}
