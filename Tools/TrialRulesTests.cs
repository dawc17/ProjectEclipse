using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

internal sealed class TrialEmptyCoreProvider : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id, out AssetMetadata metadata) { metadata = null; return false; }
}

internal static class TrialRulesTests
{
    private static int _checks;
    private static void Check(bool value, string message) { _checks++; if (!value) throw new Exception(message); }

    private static ModDescriptor Discover(string modsRoot)
    {
        ModDiscoveryResult discovery = ModDiscovery.DiscoverLoose(modsRoot);
        Check(!discovery.HasErrors, "Trial fixture discovery failed: " + string.Join(" | ", discovery.Diagnostics));
        DependencyResolutionResult resolution = DependencyResolver.Resolve(discovery.Mods, ModPlatformVersions.Core);
        Check(!resolution.HasErrors && resolution.OrderedMods.Count == 1, "Trial fixture dependency resolution failed.");
        return resolution.OrderedMods[0];
    }

    private static ModContentCatalog ExecuteLua(ModDescriptor mod)
    {
        var catalog = new ModContentCatalog();
        var perkDocument = new XmlDocument { XmlResolver = null };
        perkDocument.LoadXml("<Perks><Perk Name='PERK_TEST_TRIAL'/></Perks>");
        CoreContentImporter.ImportPerks(catalog, perkDocument.DocumentElement.ChildNodes.Cast<XmlNode>());
        var state = new ModStateRuntime();
        var assets = new AssetResolver(new IAssetProvider[] { new TrialEmptyCoreProvider(), new LooseModProvider(mod) });
        using (ModRegistrationTransaction registration = catalog.BeginRegistration(mod))
        using (IModScriptContext context = new MoonSharpScriptRuntime().CreateContext(mod,
            new ModApiFacade(mod, assets, registration, state, null)))
        {
            context.ExecuteEntrypoint();
            registration.Commit();
        }
        catalog.Freeze();
        return catalog;
    }

    private static FightRuleDefinition Rule(ModContentCatalog catalog, string localId)
    {
        Check(catalog.TryGetFightRule(DefinitionId.Parse("trial.fixture:rules/" + localId), out var rule),
            "Missing committed trial rule: " + localId);
        return rule;
    }

    private static void CheckManagedAndLua(ModContentCatalog catalog)
    {
        var hot = Rule(catalog, "hot");
        Check(hot.Kind == ModFightRuleKind.HotGround && hot.Trial.Frames == 420 && hot.Target == ModRuleTarget.Player,
            "Hot-ground typed payload changed.");
        Check(hot.Trial.Nodes.Count == 2 && hot.Trial.Nodes[0].Name == "NToeTip_1" && hot.Trial.Nodes[0].Axis == ModTrialAxis.Y &&
            hot.Trial.Nodes[0].Maximum == 15f && !hot.Trial.Nodes[0].Minimum.HasValue && hot.Trial.Animations.SequenceEqual(new[] { "Jump", "ThrowFall" }),
            "Hot-ground nodes/animations were not copied from Lua.");
        var ring = Rule(catalog, "ring");
        Check(ring.Trial.Node == "NPivot" && ring.Trial.Axis == ModTrialAxis.X && ring.Trial.Minimum == -600f && ring.Trial.Maximum == 600f,
            "Ring-out payload changed.");
        var regen = Rule(catalog, "regen");
        Check(Math.Abs(regen.Trial.Rate - 0.001f) < 0.000001f && regen.Trial.FramesAfterHit == 180 && regen.Target == ModRuleTarget.Opponent,
            "Regeneration payload changed.");
        var noAnimation = Rule(catalog, "no_jump");
        Check(noAnimation.Kind == ModFightRuleKind.NoAnimation && noAnimation.Trial.Node == "Jump" && noAnimation.Target == ModRuleTarget.All,
            "No-animation payload changed.");
        var remove = Rule(catalog, "no_block");
        Check(remove.Trial.IntervalType == ModTrialIntervalType.Block && remove.Target == ModRuleTarget.Player,
            "Remove-interval payload changed.");
        var perk = Rule(catalog, "buff");
        Check(perk.Kind == ModFightRuleKind.Perk && perk.PerkAspect == 100000d,
            "Perk rule aspect did not cross the Lua boundary.");

        // Returned collections must be detached from registration input.
        var nodes = hot.Trial.Nodes.ToArray();
        Check(nodes.Length == 2 && !ReferenceEquals(nodes, hot.Trial.Nodes), "Trial nodes expose a mutable registration array.");
    }

    private static void CheckProjection(ModContentCatalog catalog)
    {
        var projection = new TrialRuleProjection(catalog);
        XmlElement hot = projection.Build(Rule(catalog, "hot"));
        Check(hot.Name == "HotGround" && hot.GetAttribute("Frames") == "420" && hot.GetAttribute("ApplyTo") == "Player" &&
            hot.SelectNodes("Node").Count == 2 && hot.SelectNodes("Animation").Count == 2,
            "Hot-ground projection does not match recovered XML.");
        XmlElement ring = projection.Build(Rule(catalog, "ring"));
        Check(ring.Name == "Ringout" && ring.GetAttribute("Node") == "NPivot" && ring.GetAttribute("Axis") == "X" &&
            ring.GetAttribute("Min") == "-600" && ring.GetAttribute("Max") == "600" && ring.GetAttribute("ApplyTo") == "Player",
            "Ring-out projection does not match recovered XML.");
        XmlElement regen = projection.Build(Rule(catalog, "regen"));
        Check(regen.Name == "Regeneration" && regen.GetAttribute("Rate") == "0.001" && regen.GetAttribute("FramesAfterHit") == "180" &&
            regen.GetAttribute("ApplyTo") == "Bot", "Regeneration projection does not match recovered XML.");
        XmlElement noAnimation = projection.Build(Rule(catalog, "no_jump"));
        Check(noAnimation.Name == "NoAnimation" && noAnimation.GetAttribute("Name") == "Jump" && !noAnimation.HasAttribute("ApplyTo"),
            "NoAnimation gained unsupported target XML.");
        XmlElement remove = projection.Build(Rule(catalog, "no_block"));
        Check(remove.Name == "RemoveInterval" && remove.GetAttribute("Type") == "Block" && remove.GetAttribute("ApplyTo") == "Player",
            "RemoveInterval projection does not match recovered XML.");
        XmlElement perk = projection.Build(Rule(catalog, "buff"));
        Check(perk.Name == "Perk" && perk.GetAttribute("Name") == "PERK_TEST_TRIAL" &&
            perk.SelectSingleNode("Set[@Aspect='100000']") != null, "Perk aspect projection is missing its literal Set/Aspect.");
    }

    private static void CheckValidation(ModDescriptor mod)
    {
        var catalog = new ModContentCatalog();
        using (var tx = catalog.BeginRegistration(mod))
        {
            bool rejected = false;
            try { tx.RegisterHotGroundRule("bad_node", 420, new[] { new ModTrialNodeLimit("NPivot", ModTrialAxis.X) },
                Array.Empty<string>(), ModRuleTarget.Player, ModRuleMode.All, Array.Empty<int>()); }
            catch (ModContentException) { rejected = true; }
            Check(rejected, "Unbounded hot-ground node was accepted.");
            rejected = false;
            try { tx.RegisterRingOutRule("bad_ring", "NPivot", ModTrialAxis.X, 5, 5,
                ModRuleTarget.Player, ModRuleMode.All, Array.Empty<int>()); }
            catch (ModContentException) { rejected = true; }
            Check(rejected, "Degenerate ring-out bounds were accepted.");
            rejected = false;
            try { tx.RegisterRegenerationRule("bad_regen", 2f, 180,
                ModRuleTarget.Opponent, ModRuleMode.All, Array.Empty<int>()); }
            catch (ModContentException) { rejected = true; }
            Check(rejected, "Out-of-range regeneration rate was accepted.");
            rejected = false;
            try { tx.RegisterHotGroundRule("bad_all", 420,
                new[] { new ModTrialNodeLimit("NPivot", ModTrialAxis.Y, maximum: 30) }, Array.Empty<string>(),
                ModRuleTarget.All, ModRuleMode.All, Array.Empty<int>()); }
            catch (ModContentException) { rejected = true; }
            Check(rejected, "Hot-ground ApplyTo=All accepted despite recovered copy-type hazard.");
        }
    }

    private static void CheckRecoveredNativeConsumption(ModContentCatalog catalog)
    {
        var projection = new TrialRuleProjection(catalog);
        HotGroundRule hot = new HotGroundRule(projection.Build(Rule(catalog, "hot")), RuleAppliance.AppliancePlayer);
        Check(hot.NNOHILNKJEN() == 7 && hot.CheckAnimation("Jump"), "Recovered HotGroundRule did not consume frames/animations.");
        hot.Reset(); Check(hot.NNOHILNKJEN() == 7, "Recovered HotGroundRule reset lost its timer.");

        RingOutRule ring = new RingOutRule(projection.Build(Rule(catalog, "ring")), RuleAppliance.AppliancePlayer);
        Check(ring.EJHLFJBJHAN() == -600f && ring.JFBOKNFDFDO() == 600f && ring.Copy() is RingOutRule,
            "Recovered RingOutRule did not consume/copy bounds.");
        RegenerationRule regen = new RegenerationRule(projection.Build(Rule(catalog, "regen")), RuleAppliance.ApplianceOpponent);
        Check(Math.Abs(regen.BIGCPKBIJNA() - 0.001f) < 0.000001f && regen.Copy() is RegenerationRule,
            "Recovered RegenerationRule did not consume/copy rate.");
        NoAnimationRule noAnimation = new NoAnimationRule(projection.Build(Rule(catalog, "no_jump")));
        Check(noAnimation.DPKNMJMPEDM() == "Jump", "Recovered NoAnimationRule did not consume name.");
        RemoveIntervalRule remove = new RemoveIntervalRule(projection.Build(Rule(catalog, "no_block")), RuleAppliance.AppliancePlayer);
        Check(remove.Copy() is RemoveIntervalRule, "Recovered RemoveIntervalRule copy failed.");
    }

    public static int Main(string[] args)
    {
        if (args.Length != 1) throw new ArgumentException("Expected fixture Mods root.");
        ModDescriptor mod = Discover(args[0]);
        ModContentCatalog catalog = ExecuteLua(mod);
        CheckManagedAndLua(catalog);
        CheckProjection(catalog);
        CheckValidation(mod);
        CheckRecoveredNativeConsumption(catalog);
        Console.WriteLine("Trial rules PASS: " + _checks + " checks; actual Lua registration, typed payloads, recovered XML projection/native consumption, copies/reset, validation.");
        return 0;
    }
}
