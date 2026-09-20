using System;
using System.Collections.Generic;
using System.Linq;

namespace Eclipse.Modding
{
    public enum ModFightEntryDecision { Continue, Deferred, Cancelled }

    // One owner per owned fight, one pending native entry per session. No native
    // object or resumable delegate crosses the Lua boundary.
    public sealed class ModFightEntries
    {
        private readonly Dictionary<DefinitionId, Binding> bindings = new Dictionary<DefinitionId, Binding>();
        private readonly Action<ModId, string> report;
        private ModFightEntryRequest pending;
        public bool HasHandlers => bindings.Count != 0;
        public bool HasPending => pending != null && pending.IsPending;
        public ModFightEntries(Action<ModId, string> report = null) { this.report = report; }
        public bool Contains(DefinitionId fight) => bindings.ContainsKey(fight);
        public IDisposable Register(ModId owner, DefinitionId fight, Func<ModFightEntryRequest, bool?> callback)
        {
            if (fight.Category != "fights" || fight.Namespace != owner || owner.Value == "core")
                throw new InvalidOperationException("Fight-entry handlers require an owned fight.");
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            if (bindings.ContainsKey(fight)) throw new InvalidOperationException("Fight already has an entry handler: " + fight);
            if (bindings.Count >= 256 || bindings.Values.Count(value => value.Owner == owner) >= 64)
                throw new InvalidOperationException("Fight-entry handler limit reached.");
            var binding = new Binding(this, owner, fight, callback); bindings.Add(fight, binding); return binding;
        }
        public ModFightEntryDecision Begin(DefinitionId fight, Func<bool> resume, Func<bool> valid, Func<bool> ready)
        {
            if (HasPending) return ModFightEntryDecision.Cancelled;
            if (!bindings.TryGetValue(fight, out var binding)) return ModFightEntryDecision.Continue;
            if (!valid() || !ready()) return ModFightEntryDecision.Cancelled;
            var request = new ModFightEntryRequest(this, fight, resume, valid, ready);
            pending = request;
            try
            {
                request.Dispatching = true;
                var decision = binding.Callback(request);
                request.Dispatching = false;
                if (!request.IsPending) return ModFightEntryDecision.Cancelled;
                if (decision == null) return ModFightEntryDecision.Deferred;
                request.Cancel();
                return decision.Value ? ModFightEntryDecision.Continue : ModFightEntryDecision.Cancelled;
            }
            catch (Exception error)
            {
                request.Dispatching = false; request.Cancel(); binding.Dispose();
                try { report?.Invoke(binding.Owner, "Fight-entry callback failed: " + error.Message); } catch { }
                return ModFightEntryDecision.Cancelled;
            }
        }
        internal void Release(ModFightEntryRequest request) { if (ReferenceEquals(pending, request)) pending = null; }
        public void CancelPending() { pending?.Cancel(); }
        public void ValidatePending() { if (pending != null && !pending.IsPending) pending.Cancel(); }
        public void Clear() { CancelPending(); foreach (var binding in bindings.Values.ToArray()) binding.Dispose(); }
        private sealed class Binding : IDisposable
        {
            private ModFightEntries registry;
            internal readonly ModId Owner;
            internal readonly DefinitionId Fight;
            internal readonly Func<ModFightEntryRequest, bool?> Callback;
            internal Binding(ModFightEntries registry, ModId owner, DefinitionId fight, Func<ModFightEntryRequest, bool?> callback)
            { this.registry = registry; Owner = owner; Fight = fight; Callback = callback; }
            public void Dispose()
            {
                var current = registry; registry = null; if (current == null) return;
                if (current.pending?.Fight == Fight) current.CancelPending();
                current.bindings.Remove(Fight);
            }
        }
    }

    public sealed class ModFightEntryRequest
    {
        private ModFightEntries registry;
        private Func<bool> resume, valid, ready;
        internal bool Dispatching;
        public DefinitionId Fight { get; }
        internal ModFightEntryRequest(ModFightEntries registry, DefinitionId fight, Func<bool> resume, Func<bool> valid, Func<bool> ready)
        { this.registry = registry; Fight = fight; this.resume = resume; this.valid = valid; this.ready = ready; }
        public bool IsPending => registry != null && valid();
        public bool Resume()
        {
            if (Dispatching) throw new InvalidOperationException("Resume a fight after its entry callback returns.");
            if (!IsPending) { Cancel(); return false; }
            if (!ready()) return false;
            var launch = resume; Cancel(); return launch();
        }
        public void Cancel()
        {
            var current = registry; registry = null; resume = valid = ready = null;
            current?.Release(this);
        }
    }
}
