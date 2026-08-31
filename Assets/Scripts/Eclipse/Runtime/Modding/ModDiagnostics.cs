using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModDiagnosticSeverity
    {
        Warning = 0,
        Error = 1
    }

    public sealed class ModDiagnostic
    {
        public ModDiagnosticSeverity Severity { get; }
        public string Code { get; }
        public string Source { get; }
        public string Message { get; }

        public ModDiagnostic(ModDiagnosticSeverity severity, string code, string source, string message)
        {
            Severity = severity;
            Code = code ?? string.Empty;
            Source = source ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public override string ToString()
        {
            string source = string.IsNullOrEmpty(Source) ? string.Empty : " [" + Source + "]";
            return Severity + " " + Code + source + ": " + Message;
        }
    }

    public abstract class ModResultBase
    {
        public IReadOnlyList<ModDiagnostic> Diagnostics { get; }

        protected ModResultBase(ModDiagnostic[] diagnostics)
        {
            Diagnostics = Array.AsReadOnly(diagnostics ?? Array.Empty<ModDiagnostic>());
        }

        public bool HasErrors
        {
            get
            {
                foreach (ModDiagnostic diagnostic in Diagnostics)
                    if (diagnostic.Severity == ModDiagnosticSeverity.Error) return true;
                return false;
            }
        }
    }

    public sealed class ModDiscoveryResult : ModResultBase
    {
        public IReadOnlyList<ModDescriptor> Mods { get; }

        internal ModDiscoveryResult(ModDescriptor[] mods, ModDiagnostic[] diagnostics) : base(diagnostics)
        {
            Mods = Array.AsReadOnly(mods ?? Array.Empty<ModDescriptor>());
        }
    }

    public sealed class DependencyResolutionResult : ModResultBase
    {
        public IReadOnlyList<ModDescriptor> OrderedMods { get; }

        internal DependencyResolutionResult(ModDescriptor[] mods, ModDiagnostic[] diagnostics) : base(diagnostics)
        {
            OrderedMods = Array.AsReadOnly(mods ?? Array.Empty<ModDescriptor>());
        }
    }
}
