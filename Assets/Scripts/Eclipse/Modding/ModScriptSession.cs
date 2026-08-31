using System;
using System.Collections.Generic;
using System.Text;

namespace Eclipse.Modding
{
    public sealed class ModScriptSession : IDisposable
    {
        private readonly List<IModScriptContext> _contexts;

        public string RuntimeName { get; }
        public IReadOnlyList<ModDescriptor> ActiveMods { get; }
        public IReadOnlyList<ModDiagnostic> Diagnostics { get; }
        public ModContentCatalog Content { get; }

        public bool HasErrors
        {
            get
            {
                foreach (ModDiagnostic diagnostic in Diagnostics)
                    if (diagnostic.Severity == ModDiagnosticSeverity.Error) return true;
                return false;
            }
        }

        private ModScriptSession(string runtimeName, List<IModScriptContext> contexts,
            ModDescriptor[] activeMods, ModDiagnostic[] diagnostics, ModContentCatalog content)
        {
            RuntimeName = runtimeName ?? string.Empty;
            _contexts = contexts;
            ActiveMods = Array.AsReadOnly(activeMods ?? Array.Empty<ModDescriptor>());
            Diagnostics = Array.AsReadOnly(diagnostics ?? Array.Empty<ModDiagnostic>());
            Content = content ?? throw new ArgumentNullException(nameof(content));
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
                    var api = new ModApiFacade(mod, host.Assets, registration, logger);
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
            return new ModScriptSession(runtime.Name, contexts, active.ToArray(), diagnostics.ToArray(), content);
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
            return builder.ToString().TrimEnd();
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
