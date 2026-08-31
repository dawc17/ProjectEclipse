using System;
using System.Collections.Generic;
using System.Linq;

namespace Eclipse.Modding
{
    public static class DependencyResolver
    {
        public static DependencyResolutionResult Resolve(IEnumerable<ModDescriptor> mods,
            SemanticVersion apiVersion, SemanticVersion coreVersion)
        {
            if (mods == null) throw new ArgumentNullException(nameof(mods));
            var diagnostics = new List<ModDiagnostic>();
            var byId = new Dictionary<ModId, ModDescriptor>();

            foreach (ModDescriptor mod in mods)
            {
                if (mod == null) continue;
                if (byId.ContainsKey(mod.Id))
                {
                    diagnostics.Add(Error("DEP001", mod.Id, "Duplicate mod id in dependency input."));
                    continue;
                }
                byId.Add(mod.Id, mod);
            }

            var incoming = new Dictionary<ModId, int>();
            var dependents = new Dictionary<ModId, List<ModId>>();
            foreach (ModDescriptor mod in byId.Values)
            {
                incoming[mod.Id] = 0;
                dependents[mod.Id] = new List<ModId>();
                if (!mod.Manifest.Api.Contains(apiVersion))
                {
                    diagnostics.Add(Error("DEP002", mod.Id, "Requires Mod API '" + mod.Manifest.Api +
                        "', current API is " + apiVersion + "."));
                }
            }

            foreach (ModDescriptor mod in byId.Values)
            {
                foreach (ModDependency dependency in mod.Manifest.Dependencies)
                {
                    if (dependency.Id == mod.Id)
                    {
                        diagnostics.Add(Error("DEP003", mod.Id, "A mod may not depend on itself."));
                        continue;
                    }

                    if (dependency.Id.Value == "core")
                    {
                        if (!dependency.Version.Contains(coreVersion))
                        {
                            diagnostics.Add(Error("DEP004", mod.Id, "Requires core '" + dependency.Version +
                                "', current core is " + coreVersion + "."));
                        }
                        continue;
                    }

                    ModDescriptor target;
                    if (!byId.TryGetValue(dependency.Id, out target))
                    {
                        diagnostics.Add(Error("DEP005", mod.Id, "Missing dependency '" + dependency.Id + "'."));
                        continue;
                    }
                    if (!dependency.Version.Contains(target.Version))
                    {
                        diagnostics.Add(Error("DEP006", mod.Id, "Dependency '" + dependency.Id + "' requires '" +
                            dependency.Version + "', found " + target.Version + "."));
                        continue;
                    }

                    incoming[mod.Id] = incoming[mod.Id] + 1;
                    dependents[dependency.Id].Add(mod.Id);
                }
            }

            var ready = new SortedSet<string>(StringComparer.Ordinal);
            foreach (KeyValuePair<ModId, int> pair in incoming)
            {
                if (pair.Value == 0) ready.Add(pair.Key.Value);
            }

            var ordered = new List<ModDescriptor>();
            while (ready.Count != 0)
            {
                string nextText = ready.Min;
                ready.Remove(nextText);
                ModId next = ModId.Parse(nextText);
                ordered.Add(byId[next]);
                dependents[next].Sort((a, b) => string.CompareOrdinal(a.Value, b.Value));
                foreach (ModId dependent in dependents[next])
                {
                    incoming[dependent] = incoming[dependent] - 1;
                    if (incoming[dependent] == 0) ready.Add(dependent.Value);
                }
            }

            if (ordered.Count != byId.Count)
            {
                string[] blocked = incoming.Where(pair => pair.Value > 0)
                    .Select(pair => pair.Key.Value)
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .ToArray();
                diagnostics.Add(new ModDiagnostic(ModDiagnosticSeverity.Error, "DEP007", string.Join(",", blocked),
                    "Dependency cycle detected among: " + string.Join(", ", blocked) + "."));
            }

            bool hasErrors = diagnostics.Any(x => x.Severity == ModDiagnosticSeverity.Error);
            return new DependencyResolutionResult(hasErrors ? Array.Empty<ModDescriptor>() : ordered.ToArray(),
                diagnostics.ToArray());
        }

        private static ModDiagnostic Error(string code, ModId source, string message)
        {
            return new ModDiagnostic(ModDiagnosticSeverity.Error, code, source.Value, message);
        }
    }
}
