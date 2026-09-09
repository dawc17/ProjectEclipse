using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eclipse.Modding;

internal static class Phase3RuntimeTests
{
    private sealed class Core : IAssetProvider
    {
        public ModId Namespace => ModId.Parse("core");
        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "fixture");
            return id.Namespace == Namespace;
        }
    }
    private sealed class Fighter : IModFighterOperations
    {
        public bool TryChangeHealth(double n, out string error) { error = "unexpected"; return false; }
        public bool TryAddMagicCharge(double n, out string error) { error = "unexpected"; return false; }
    }
    private static int checks;
    private static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    private static void Reject(Action action)
    {
        try { action(); } catch (ModContentException) { checks++; return; }
        throw new Exception("Invalid registration accepted.");
    }
    public static void Main(string[] args)
    {
        var discovery = ModDiscovery.DiscoverLoose(args[0]);
        Check(!discovery.HasErrors && discovery.Mods.Count == 1, "Discovery failed.");
        var mod = discovery.Mods.Single();
        var dependencies = DependencyResolver.Resolve(discovery.Mods, ModPlatformVersions.Api, ModPlatformVersions.Core);
        Check(!dependencies.HasErrors, "API/dependency resolution failed.");
        var assets = new AssetResolver(new IAssetProvider[] { new Core(), new LooseModProvider(mod) });
        var catalog = new ModContentCatalog();
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[] { "Simple" });
        var state = new ModStateRuntime();
        using var tx = catalog.BeginRegistration(mod);
        ModLocalizationLoader.Load(mod, assets, tx);
        var api = new ModApiFacade(mod, assets, tx, state, null);
        using var context = new MoonSharpScriptRuntime().CreateContext(mod, api);
        context.ExecuteEntrypoint();
        tx.Commit();
        Check(catalog.Counters.Count == 1 && catalog.Achievements.Count == 2 && catalog.AssetReplacements.Count == 1, "P3 sample definitions missing.");
        Check(catalog.Zones.Count == 0 && catalog.ForgeRecipeFamilies.Count == 1, "Sample unexpectedly adds a map zone or has no forge recipe.");
        var counter = catalog.Counters.Single();
        Reject(() => api.ReadCounter(counter.Id)); // no profile during startup
        int count = 0;
        ModProgressionAccess.Read = id => count;
        ModProgressionAccess.Advance = (id, n) => count += n;
        var behavior = catalog.Behaviors.Single();
        var script = (IModInteractiveBehaviorScriptContext)context;
        foreach (var result in new[] { "loss", "surrender", "win", "win", "win" })
            Check(script.TryInvokeBehavior(behavior.Id, ModEffectEvent.FightEnd,
                behavior.Parameters.ResolveValues(new Dictionary<string, ModParameterValue>()),
                new Dictionary<string, string> { { "side", "player" }, { "player_result", result } }, new Fighter(), out var error), error);
        Check(count == 3, "Only victories should count.");
        Check(script.TryInvokeBehavior(behavior.Id, ModEffectEvent.FightEnd,
            behavior.Parameters.ResolveValues(new Dictionary<string, ModParameterValue>()),
            new Dictionary<string, string> { { "side", "enemy" }, { "player_result", "loss" } }, new Fighter(), out var npcError), npcError);
        Check(count == 3, "NPC progress leaked into player counter.");
        Reject(() => api.AdvanceCounter(counter.Id, -1));
        Reject(() => api.ReadCounter(DefinitionId.Parse("other.mod:counters/wins")));
        var replacement = catalog.AssetReplacements.Single();
        assets.SetReplacements(catalog.AssetReplacements);
        Check(assets.Resolve(replacement.Target) == replacement.Replacement, "Redirect not active.");
        Check(assets.TryRead(replacement.Target, out var bytes) && bytes.Data.Length > 0 && bytes.Metadata.Id == replacement.Replacement, "Read did not retain replacement provenance.");
        assets.SetReplacements(null);
        Check(assets.Resolve(replacement.Target) == replacement.Target, "Unmount did not restore base identity.");
        string before = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, catalog);
        using (var failed = catalog.BeginRegistration(mod))
        {
            failed.RegisterCounter("rollback_probe", 5);
            failed.ReplaceAsset(replacement.Target, replacement.Replacement, replacement.Kind);
            Reject(() => failed.Commit());
        }
        Check(catalog.Counters.Count == 1 && before == ModSaveData.ComputeContentSetFingerprint(new[] { mod }, catalog), "Failed conflict partially committed.");
        using (var invalid = new ModContentCatalog().BeginRegistration(mod))
        {
            Reject(() => invalid.RegisterCounter("bad", 0));
            var c = invalid.RegisterCounter("bounded", 5);
            Reject(() => invalid.RegisterAchievement("bad", c.Id, catalog.Achievements.First().Title,
                catalog.Achievements.First().Description, replacement.Target, 6, false));
            Reject(() => invalid.ReplaceAsset(AssetId.Parse("undeclared.mod:icon.png"), replacement.Replacement, AssetKind.Sprite));
            Reject(() => invalid.ReplaceAsset(replacement.Target, AssetId.Parse("core:other"), AssetKind.Sprite));
            Reject(() => invalid.ReplaceAsset(replacement.Target, replacement.Replacement, AssetKind.Text));
        }
        string Fingerprint(int maximum)
        {
            var c = new ModContentCatalog();
            using var t = c.BeginRegistration(mod);
            t.RegisterCounter("hash", maximum); t.Commit();
            return ModSaveData.ComputeContentSetFingerprint(new[] { mod }, c);
        }
        Check(Fingerprint(3) != Fingerprint(4) && Fingerprint(3) == Fingerprint(3), "P3 counter bounds omitted from identity.");
        string ReplacementFingerprint(string path)
        {
            var c = new ModContentCatalog();
            using var t = c.BeginRegistration(mod);
            t.ReplaceAsset(replacement.Target, AssetId.Parse(mod.Id + ":" + path), AssetKind.Sprite);
            t.Commit();
            return ModSaveData.ComputeContentSetFingerprint(new[] { mod }, c);
        }
        Check(ReplacementFingerprint("sprites/a") != ReplacementFingerprint("sprites/b"), "Replacement destination omitted from provenance fingerprint.");
        // Validate through the public facade, not only the transaction's enum guard.
        string audioFolder = Path.Combine(mod.RootPath, "assets", "audio");
        Directory.CreateDirectory(audioFolder);
        File.WriteAllBytes(Path.Combine(audioFolder, "type_probe.wav"), new byte[] { 0 });
        var typedAssets = new AssetResolver(new IAssetProvider[] { new Core(), new LooseModProvider(mod) });
        using (var invalid = new ModContentCatalog().BeginRegistration(mod))
        {
            var typedApi = new ModApiFacade(mod, typedAssets, invalid, null);
            Reject(() => typedApi.ReplaceAsset(replacement.Target.ToString(), "audio/type_probe"));
            Reject(() => typedApi.ReplaceAsset(replacement.Target.ToString(), "sprites/missing"));
        }
        ModProgressionAccess.Clear();
        Console.WriteLine("PASS: Phase 3 public Lua, win/loss/NPC filtering, counter ownership/bounds, replacement mount/read/unmount, conflict rollback and fingerprint (" + checks + " checks).");
    }
}
