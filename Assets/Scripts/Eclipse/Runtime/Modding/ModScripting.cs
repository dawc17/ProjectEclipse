using System;

namespace Eclipse.Modding
{
    public enum ModLogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public readonly struct ModLogEntry
    {
        public ModId ModId { get; }
        public ModLogLevel Level { get; }
        public string Message { get; }

        public ModLogEntry(ModId modId, ModLogLevel level, string message)
        {
            ModId = modId;
            Level = level;
            Message = message ?? string.Empty;
        }

        public override string ToString()
        {
            return "[" + ModId + "] " + Level + ": " + Message;
        }
    }

    public sealed class ModApiFacade
    {
        private readonly Action<ModLogEntry> _logger;

        public ModDescriptor Mod { get; }
        public AssetResolver Assets { get; }

        public ModApiFacade(ModDescriptor mod, AssetResolver assets, Action<ModLogEntry> logger)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Assets = assets ?? throw new ArgumentNullException(nameof(assets));
            _logger = logger;
        }

        public AssetId QualifyAsset(string reference)
        {
            AssetId id = Assets.Qualify(Mod.Id, reference);
            if (id.Namespace == Mod.Id) return id;

            foreach (ModDependency dependency in Mod.Manifest.Dependencies)
            {
                if (dependency.Id == id.Namespace) return id;
            }

            throw new InvalidOperationException("Mod '" + Mod.Id +
                "' cannot reference undeclared dependency namespace '" + id.Namespace + "'.");
        }

        public bool AssetExists(string reference)
        {
            AssetMetadata metadata;
            return Assets.TryDescribe(QualifyAsset(reference), out metadata);
        }

        public void Log(ModLogLevel level, string message)
        {
            _logger?.Invoke(new ModLogEntry(Mod.Id, level, message));
        }
    }

    public interface IModScriptRuntime
    {
        string Name { get; }
        IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api);
    }

    public interface IModScriptContext : IDisposable
    {
        ModDescriptor Mod { get; }
        void ExecuteEntrypoint();
    }

    public sealed class ModScriptException : Exception
    {
        public ModId ModId { get; }
        public string SourceName { get; }

        public ModScriptException(ModId modId, string sourceName, string message, Exception innerException = null)
            : base(message, innerException)
        {
            ModId = modId;
            SourceName = sourceName ?? string.Empty;
        }
    }
}
