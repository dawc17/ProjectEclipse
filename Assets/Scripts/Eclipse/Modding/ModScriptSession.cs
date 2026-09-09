using System;
using System.Collections.Generic;
using System.Text;

namespace Eclipse.Modding
{
    public sealed class ModScriptSession : IDisposable
    {
        private readonly List<IModScriptContext> _contexts;
        private readonly List<ModDiagnostic> _stateDiagnostics = new List<ModDiagnostic>();

        public string RuntimeName { get; }
        public IReadOnlyList<ModDescriptor> ActiveMods { get; }
        public IReadOnlyList<ModDiagnostic> Diagnostics { get; }
        public IReadOnlyList<ModDiagnostic> StateDiagnostics => _stateDiagnostics.AsReadOnly();
        public ModContentCatalog Content { get; }
        public ModStateRuntime State { get; }

        public bool HasErrors
        {
            get
            {
                foreach (ModDiagnostic diagnostic in Diagnostics)
                    if (diagnostic.Severity == ModDiagnosticSeverity.Error) return true;
                foreach (ModDiagnostic diagnostic in _stateDiagnostics)
                    if (diagnostic.Severity == ModDiagnosticSeverity.Error) return true;
                return false;
            }
        }

        private ModScriptSession(string runtimeName, List<IModScriptContext> contexts,
            ModDescriptor[] activeMods, ModDiagnostic[] diagnostics, ModContentCatalog content,
            ModStateRuntime state)
        {
            RuntimeName = runtimeName ?? string.Empty;
            _contexts = contexts;
            ActiveMods = Array.AsReadOnly(activeMods ?? Array.Empty<ModDescriptor>());
            Diagnostics = Array.AsReadOnly(diagnostics ?? Array.Empty<ModDiagnostic>());
            Content = content ?? throw new ArgumentNullException(nameof(content));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        internal static ModScriptSession Start(ModHost host, IModScriptRuntime runtime,
            Action<ModLogEntry> logger, Action<ModContentCatalog> importCore)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (runtime == null) throw new ArgumentNullException(nameof(runtime));

            var contexts = new List<IModScriptContext>();
            var active = new List<ModDescriptor>();
            var activeIds = new HashSet<ModId>();
            var diagnostics = new List<ModDiagnostic>();
            var content = new ModContentCatalog();
            var state = new ModStateRuntime();
            importCore?.Invoke(content);

            foreach (ModDescriptor mod in host.EnabledMods)
            {
                ModId unavailable;
                if (TryFindUnavailableDependency(mod, activeIds, out unavailable))
                {
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "SCRIPT002", mod.Id.Value,
                        "Dependency '" + unavailable + "' did not complete script initialization, so this mod was skipped."));
                    continue;
                }

                IModScriptContext context = null;
                ModRegistrationTransaction registration = null;
                try
                {
                    registration = content.BeginRegistration(mod);
                    ModLocalizationLoader.Load(mod, host.Assets, registration);
                    var api = new ModApiFacade(mod, host.Assets, registration, state, logger);
                    context = runtime.CreateContext(mod, api);
                    if (context == null) throw new InvalidOperationException("Script runtime returned a null context.");
                    context.ExecuteEntrypoint();
                    registration.Commit();
                    contexts.Add(context);
                    active.Add(mod);
                    activeIds.Add(mod.Id);
                    context = null;
                }
                catch (Exception exception)
                {
                    state.RemoveDefinition(mod.Id);
                    string source = mod.Manifest.Entrypoint;
                    ModScriptException scriptException = exception as ModScriptException;
                    if (scriptException != null && !string.IsNullOrEmpty(scriptException.SourceName))
                        source = scriptException.SourceName;
                    diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "SCRIPT001", mod.Id.Value,
                        "Entrypoint '" + source + "' failed: " + exception.Message));
                }
                finally
                {
                    registration?.Dispose();
                    context?.Dispose();
                }
            }

            content.Freeze();
            state.FreezeDefinitions();
            return new ModScriptSession(runtime.Name, contexts, active.ToArray(), diagnostics.ToArray(), content, state);
        }

        public string FormatReport()
        {
            var builder = new StringBuilder();
            builder.Append(RuntimeName).Append(" | active ").Append(ActiveMods.Count)
                .Append(" | diagnostics ").Append(Diagnostics.Count).AppendLine();
            foreach (ModDescriptor mod in ActiveMods)
                builder.Append("+ ").Append(mod.Id).Append(' ').Append(mod.Version).AppendLine();
            foreach (ModDiagnostic diagnostic in Diagnostics)
                builder.Append("! ").AppendLine(diagnostic.ToString());
            foreach (ModDiagnostic diagnostic in _stateDiagnostics)
                builder.Append("! ").AppendLine(diagnostic.ToString());
            return builder.ToString().TrimEnd();
        }

        public IReadOnlyList<ModDiagnostic> BindState(System.Xml.XmlNode warrior)
        {
            _stateDiagnostics.Clear();
            IReadOnlyList<ModDiagnostic> diagnostics = State.Bind(warrior, _contexts);
            for (int i = 0; i < diagnostics.Count; i++) _stateDiagnostics.Add(diagnostics[i]);
            return StateDiagnostics;
        }

        public bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
            IReadOnlyDictionary<string, ModParameterValue> parameters,
            IReadOnlyDictionary<string, string> context, out string error)
        {
            return TryInvokeBehavior(behaviorId, effectEvent, parameters, context, null, out error);
        }

        public bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
            IReadOnlyDictionary<string, ModParameterValue> parameters,
            IReadOnlyDictionary<string, string> context, IModFighterOperations fighter, out string error)
        {
            error = string.Empty;
            if (behaviorId.Category != "behaviors")
            {
                error = "Behavior ID must use the behaviors category: '" + behaviorId + "'.";
                return false;
            }
            for (int i = 0; i < _contexts.Count; i++)
            {
                IModScriptContext scriptContext = _contexts[i];
                if (scriptContext == null || scriptContext.Mod.Id != behaviorId.Namespace) continue;
                IModBehaviorScriptContext behaviorContext = scriptContext as IModBehaviorScriptContext;
                if (behaviorContext == null)
                {
                    error = "Script runtime does not expose behavior callbacks for '" + behaviorId + "'.";
                    return false;
                }
                if (!behaviorContext.HasBehaviorHandler(behaviorId, effectEvent))
                {
                    if (effectEvent == ModEffectEvent.DamageReceived || effectEvent == ModEffectEvent.FightBegin) return true;
                    error = "Behavior '" + behaviorId + "' has no handler for " + effectEvent + ".";
                    return false;
                }
                if (fighter != null)
                {
                    IModInteractiveBehaviorScriptContext interactive = scriptContext as IModInteractiveBehaviorScriptContext;
                    if (interactive == null)
                    {
                        error = "Script runtime does not expose fighter operations for '" + behaviorId + "'.";
                        return false;
                    }
                    return interactive.TryInvokeBehavior(behaviorId, effectEvent, parameters, context, fighter, out error);
                }
                return behaviorContext.TryInvokeBehavior(behaviorId, effectEvent, parameters, context, out error);
            }
            error = "Behavior owner mod is not active: '" + behaviorId.Namespace + "'.";
            return false;
        }

        public void Dispose()
        {
            for (int i = _contexts.Count - 1; i >= 0; i--)
                _contexts[i]?.Dispose();
            _contexts.Clear();
        }

        private static bool TryFindUnavailableDependency(ModDescriptor mod, HashSet<ModId> activeIds,
            out ModId unavailable)
        {
            foreach (ModDependency dependency in mod.Manifest.Dependencies)
            {
                if (dependency.Id.Value == "core") continue;
                if (activeIds.Contains(dependency.Id)) continue;
                unavailable = dependency.Id;
                return true;
            }
            unavailable = default;
            return false;
        }
    }
}
