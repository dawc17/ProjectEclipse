using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    // Host transport only. No Lua emit operation or borrowed native objects.
    public enum ModStoryEventKind { Purchase, Enchantment, LevelUp, SceneEnter }

    public sealed class ModStoryEvent
    {
        public ModStoryEventKind Kind { get; }
        public DefinitionId? Item { get; }
        public DefinitionId? Recipe { get; }
        public int? PreviousLevel { get; }
        public int? Level { get; }
        public string Scene { get; }

        public ModStoryEvent(ModStoryEventKind kind, DefinitionId? item, DefinitionId? recipe = null,
            int? previousLevel = null, int? level = null, string scene = null)
        {
            if (!Enum.IsDefined(typeof(ModStoryEventKind), kind)) throw new ArgumentOutOfRangeException(nameof(kind));
            if (kind == ModStoryEventKind.SceneEnter)
            {
                if (item.HasValue || recipe.HasValue ||
                    (scene != "map" && scene != "shop" && scene != "profile" && scene != "dojo" && scene != "fight"))
                    throw new ArgumentException("Scene entry requires a supported destination and no item or recipe.");
            }
            else if (scene != null) throw new ArgumentException("Scene values require a scene entry notification.");
            if (kind == ModStoryEventKind.LevelUp)
            {
                if (item.HasValue || recipe.HasValue || !previousLevel.HasValue || !level.HasValue ||
                    previousLevel.Value < 1 || level.Value <= previousLevel.Value)
                    throw new ArgumentException("Level-up notifications require an increasing positive level pair and no item or recipe.");
            }
            else if (previousLevel.HasValue || level.HasValue)
                throw new ArgumentException("Level values require a level-up notification.");
            if (item.HasValue && item.Value.Category != "items") throw new ArgumentException("Expected an item identity.", nameof(item));
            if (recipe.HasValue && (kind != ModStoryEventKind.Enchantment ||
                (recipe.Value.Category != "forge-recipes" && recipe.Value.Category != "forge-profiles")))
                throw new ArgumentException("Expected an enchantment recipe identity.", nameof(recipe));
            Kind = kind;
            Item = item;
            Recipe = recipe;
            PreviousLevel = previousLevel;
            Level = level;
            Scene = scene;
        }
    }

    // Main-thread service. A profile boundary invalidates in-flight notifications,
    // while a scope boundary releases callbacks owned by an unloaded script.
    public sealed class ModStoryEvents
    {
        public const int MaximumSubscriptions = 256;
        public const int MaximumSubscriptionsPerMod = 64;
        public const int MaximumEventsPerDispatch = 128;
        public const int MaximumCallbacksPerDispatch = 1024;
        private readonly List<ModStorySubscription> _subscriptions = new List<ModStorySubscription>();
        private readonly Queue<ModStoryEvent> _pending = new Queue<ModStoryEvent>();
        private readonly Action<ModId, string> _report;
        private bool _bound;
        private bool _dispatching;
        private int _generation;
        private int _accepted;
        private int _scopeGeneration;
        public int ProfileGeneration => _generation;

        public bool HasSubscribers(ModStoryEventKind kind)
        {
            if (!_bound) return false;
            foreach (var subscription in _subscriptions) if (subscription.Kind == kind) return true;
            return false;
        }

        public ModStoryEvents(Action<ModId, string> report = null) { _report = report; }

        public ModStoryScope CreateScope(ModId owner)
        {
            if (string.IsNullOrEmpty(owner.Value)) throw new ArgumentException("A subscription scope requires an owner.", nameof(owner));
            return new ModStoryScope(this, owner, _scopeGeneration);
        }

        public void BindProfile() { UnbindProfile(); _bound = true; }
        public void UnbindProfile()
        {
            _bound = false;
            unchecked { _generation++; }
            _pending.Clear();
        }

        public void Clear()
        {
            UnbindProfile();
            unchecked { _scopeGeneration++; }
            foreach (var subscription in _subscriptions.ToArray()) subscription.Dispose();
        }

        internal ModStorySubscription Subscribe(ModStoryScope scope, ModStoryEventKind kind, Action<ModStoryEvent> callback)
        {
            if (scope.Generation != _scopeGeneration) throw new ObjectDisposedException(nameof(ModStoryScope));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            if (!Enum.IsDefined(typeof(ModStoryEventKind), kind)) throw new ArgumentOutOfRangeException(nameof(kind));
            int owned = 0;
            foreach (var subscription in _subscriptions) if (subscription.Owner == scope.Owner) owned++;
            if (_subscriptions.Count >= MaximumSubscriptions || owned >= MaximumSubscriptionsPerMod)
                throw new InvalidOperationException("Story subscription capacity exceeded.");
            var result = new ModStorySubscription(this, scope, kind, callback);
            _subscriptions.Add(result);
            return result;
        }

        internal void Remove(ModStorySubscription subscription) { _subscriptions.Remove(subscription); }

        // False means no active profile, or the root dispatch exhausted its event
        // budget. Callback results never veto or replace a native action.
        public bool Publish(ModStoryEvent notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            if (!_bound) return false;
            if (!_dispatching) _accepted = 0;
            if (_accepted >= MaximumEventsPerDispatch)
            {
                Report(default, "Story event dispatch limit reached; notification dropped.");
                return false;
            }
            _accepted++;
            _pending.Enqueue(notification);
            if (_dispatching) return true;
            _dispatching = true;
            int callbacks = 0;
            try
            {
                while (_bound && _pending.Count != 0)
                {
                    var current = _pending.Dequeue();
                    int generation = _generation;
                    // Additions take effect on the next notification; removals
                    // take effect immediately, including during this snapshot.
                    var listeners = _subscriptions.ToArray();
                    foreach (var listener in listeners)
                    {
                        if (!_bound || generation != _generation) break;
                        if (!listener.IsActive || listener.Kind != current.Kind) continue;
                        if (callbacks >= MaximumCallbacksPerDispatch)
                        {
                            Report(listener.Owner, "Story callback dispatch limit reached; remaining notifications dropped.");
                            return true;
                        }
                        callbacks++;
                        try { listener.Invoke(current); }
                        catch (Exception error)
                        {
                            listener.Dispose();
                            Report(listener.Owner, "Story " + current.Kind + " subscription canceled: " + error.Message);
                        }
                    }
                }
                return true;
            }
            finally { _pending.Clear(); _dispatching = false; }
        }

        private void Report(ModId owner, string message)
        {
            // Logging must not turn a script error into a failed native action.
            try { _report?.Invoke(owner, message); } catch { }
        }
    }

    public sealed class ModStoryScope : IDisposable
    {
        private readonly ModStoryEvents _events;
        private readonly List<ModStorySubscription> _subscriptions = new List<ModStorySubscription>();
        private bool _disposed;
        public ModId Owner { get; }
        internal int Generation { get; }
        internal ModStoryScope(ModStoryEvents events, ModId owner, int generation) { _events = events; Owner = owner; Generation = generation; }
        public ModStorySubscription Subscribe(ModStoryEventKind kind, Action<ModStoryEvent> callback)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ModStoryScope));
            var subscription = _events.Subscribe(this, kind, callback);
            _subscriptions.Add(subscription);
            return subscription;
        }
        internal void Remove(ModStorySubscription subscription) { _subscriptions.Remove(subscription); }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var subscription in _subscriptions.ToArray()) subscription.Dispose();
        }
    }

    public sealed class ModStorySubscription : IDisposable
    {
        private readonly ModStoryEvents _events;
        private readonly ModStoryScope _scope;
        private Action<ModStoryEvent> _callback;
        public ModId Owner => _scope.Owner;
        public ModStoryEventKind Kind { get; }
        public bool IsActive => _callback != null;
        internal ModStorySubscription(ModStoryEvents events, ModStoryScope scope, ModStoryEventKind kind, Action<ModStoryEvent> callback)
        { _events = events; _scope = scope; Kind = kind; _callback = callback; }
        internal void Invoke(ModStoryEvent notification) { _callback?.Invoke(notification); }
        public void Dispose()
        {
            if (_callback == null) return;
            _callback = null;
            _events.Remove(this);
            _scope.Remove(this);
        }
    }
}
