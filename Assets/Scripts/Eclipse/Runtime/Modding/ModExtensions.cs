using System;
using System.Collections.Generic;
using System.Linq;

namespace Eclipse.Modding
{
    public sealed class ModExtensionDefinition
    {
        public DefinitionId Id { get; }
        public int Version { get; }
        public ModParameterSchema Request { get; }
        public ModParameterSchema Response { get; }

        public ModExtensionDefinition(DefinitionId id, int version, ModParameterSchema request, ModParameterSchema response)
        {
            if (id.Category != "extensions") throw new ModContentException("Expected an extensions definition ID.");
            if (version < 1 || version > 1000000) throw new ModContentException("Extension version must be 1..1000000.");
            Id = id;
            Version = version;
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly DefinitionRegistry<ModExtensionDefinition> _extensionDefinitions =
            new DefinitionRegistry<ModExtensionDefinition>(value => value.Id);
        public IReadOnlyList<ModExtensionDefinition> Extensions => _extensionDefinitions.Values;
        internal void ValidateExtensions(ModExtensionDefinition[] values) => _extensionDefinitions.ValidateCanAdd(values);
        internal void AddExtensions(ModExtensionDefinition[] values) => _extensionDefinitions.AddRange(values);
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, ModExtensionDefinition> _extensions =
            new Dictionary<DefinitionId, ModExtensionDefinition>();

        public ModExtensionDefinition RegisterExtension(string localId, int version,
            ModParameterSchema request, ModParameterSchema response)
        {
            ThrowIfCompleted();
            EnsureCapacityForNewRegistration();
            var id = Qualify("extensions", localId);
            if (_extensions.ContainsKey(id)) throw new ModContentException("Duplicate extension '" + id + "'.");
            var definition = new ModExtensionDefinition(id, version, request, response);
            _extensions.Add(id, definition);
            return definition;
        }
        private void ValidateExtensionsCommit() => _catalog.ValidateExtensions(_extensions.Values.ToArray());
        private void ApplyExtensionsCommit() => _catalog.AddExtensions(_extensions.Values.ToArray());
    }

    // Session-owned host routing. No Lua closures or native handles cross script boundaries.
    public sealed class ModExtensionRegistry : IDisposable
    {
        public const int MaxCallsPerExecution = 32;
        public const int MaxDepth = 8;
        private sealed class Endpoint
        {
            public ModExtensionDefinition Definition;
            public Func<ModId, IReadOnlyDictionary<string, ModParameterValue>, IReadOnlyDictionary<string, ModParameterValue>> Handler;
        }
        private readonly Dictionary<DefinitionId, Endpoint> _endpoints = new Dictionary<DefinitionId, Endpoint>();
        private readonly HashSet<ModId> _active = new HashSet<ModId>();
        private readonly HashSet<ModId> _executing = new HashSet<ModId>();
        private int _calls;
        private int _depth;
        private bool _disposed;

        public void Stage(ModExtensionDefinition definition,
            Func<ModId, IReadOnlyDictionary<string, ModParameterValue>, IReadOnlyDictionary<string, ModParameterValue>> handler)
        {
            ThrowIfDisposed();
            if (definition == null || handler == null) throw new ArgumentNullException();
            if (_active.Contains(definition.Id.Namespace)) throw new ModContentException("Extension registration is closed.");
            if (_endpoints.ContainsKey(definition.Id)) throw new ModContentException("Duplicate extension '" + definition.Id + "'.");
            _endpoints.Add(definition.Id, new Endpoint { Definition = definition, Handler = handler });
        }

        public void Activate(ModId owner) { ThrowIfDisposed(); _active.Add(owner); }
        public void RemoveOwner(ModId owner)
        {
            _active.Remove(owner);
            foreach (var id in _endpoints.Keys.Where(id => id.Namespace == owner).ToArray()) _endpoints.Remove(id);
        }

        public ModExtensionDefinition Resolve(ModDescriptor caller, DefinitionId id, int version)
        {
            ThrowIfDisposed();
            bool permitted = id.Namespace == caller.Id || caller.Manifest.Dependencies.Any(value => value.Id == id.Namespace);
            if (id.Category != "extensions" || !permitted)
                throw new ModContentException("Extension '" + id + "' requires its owner's direct dependency in mod '" + caller.Id + "'.");
            if (!_active.Contains(id.Namespace) || !_endpoints.TryGetValue(id, out var endpoint))
                throw new ModContentException("Extension is unavailable: '" + id + "'.");
            if (version != endpoint.Definition.Version)
                throw new ModContentException("Extension '" + id + "' has version " + endpoint.Definition.Version + ", requested " + version + ".");
            return endpoint.Definition;
        }

        public IReadOnlyDictionary<string, ModParameterValue> Call(ModDescriptor caller, DefinitionId id, int version,
            IReadOnlyDictionary<string, ModParameterValue> request)
        {
            var definition = Resolve(caller, id, version);
            if (_executing.Contains(id.Namespace))
                throw new ModContentException("Extension call would re-enter executing mod '" + id.Namespace + "'. Use local functions within a mod.");
            if (_depth >= MaxDepth || ++_calls > MaxCallsPerExecution)
                throw new ModContentException("Extension call budget exceeded (depth " + MaxDepth + ", calls " + MaxCallsPerExecution + ").");
            var input = definition.Request.ResolveValues(request);
            _depth++;
            try
            {
                return definition.Response.ResolveValues(_endpoints[id].Handler(caller.Id, input));
            }
            catch (Exception exception)
            {
                throw new ModContentException("Extension '" + id + "' called by '" + caller.Id + "' failed: " + exception.Message, exception);
            }
            finally { _depth--; }
        }

        public IDisposable EnterExecution(ModId owner)
        {
            ThrowIfDisposed();
            if (_executing.Count == 0) _calls = 0;
            return new ExecutionScope(this, owner, _executing.Add(owner));
        }
        private sealed class ExecutionScope : IDisposable
        {
            private ModExtensionRegistry _registry;
            private readonly ModId _owner;
            private readonly bool _added;
            public ExecutionScope(ModExtensionRegistry registry, ModId owner, bool added) { _registry = registry; _owner = owner; _added = added; }
            public void Dispose() { if (_added) _registry?._executing.Remove(_owner); _registry = null; }
        }
        private void ThrowIfDisposed() { if (_disposed) throw new ObjectDisposedException(nameof(ModExtensionRegistry)); }
        public void Dispose() { _disposed = true; _active.Clear(); _endpoints.Clear(); _executing.Clear(); }
    }
}
