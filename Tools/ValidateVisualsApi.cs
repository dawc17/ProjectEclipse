using System;
using System.IO;
using System.Linq;
using Eclipse.Modding;

// Headless checks for sf2.settings and sf2.visuals: the shipped Chiaroscuro
// package through real Lua, plus capability, validation and conflict failures.
static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }

    static ModContentCatalog Load(ModDescriptor mod, ModContentCatalog catalog = null)
    {
        catalog = catalog ?? new ModContentCatalog();
        using (var tx = catalog.BeginRegistration(mod))
        using (var context = new MoonSharpScriptRuntime().CreateContext(mod,
            new ModApiFacade(mod, new AssetResolver(new IAssetProvider[] { new LooseModProvider(mod) }), tx, new ModStateRuntime(), null)))
        {
            context.ExecuteEntrypoint();
            tx.Commit();
        }
        return catalog;
    }

    static ModDescriptor Fixture(string root, string id, string capabilities, string lua)
    {
        string dir = Path.Combine(root, id);
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
        Directory.CreateDirectory(Path.Combine(dir, "scripts"));
        File.WriteAllText(Path.Combine(dir, "mod.toml"), ("schema=1\nid='" + id + "'\nname='" + id + "'\nversion='1.0.0'\n" +
            "authors=['Eclipse']\nentrypoint='scripts/main.lua'\ncapabilities=[" + capabilities + "]\n").Replace('\'', '"'));
        File.WriteAllText(Path.Combine(dir, "scripts/main.lua"), "local sf2=require('sf2')\n" + lua);
        var found = ModDiscovery.DiscoverLoose(root);
        var mod = found.Mods.SingleOrDefault(m => m.Id.Value == id);
        if (mod == null) throw new Exception("Fixture " + id + " was not discovered: " + string.Join("; ", found.Diagnostics.Select(d => d.ToString())));
        return mod;
    }

    static void Rejects(string root, string name, string capabilities, string lua, string expected)
    {
        var mod = Fixture(root, "fixture." + name, capabilities, lua);
        try { Load(mod); }
        catch (Exception error)
        {
            string text = error.ToString();
            Check(text.Contains(expected), name + " failed with an unexpected error: " + error.Message);
            return;
        }
        throw new Exception(name + " was accepted.");
    }

    static void Main(string[] args)
    {
        string fixtures = args[0], mods = args[1];
        Directory.CreateDirectory(fixtures);

        // Shipped package.
        var chiaroscuro = ModDiscovery.DiscoverLoose(mods).Mods.Single(m => m.Id.Value == "chiaroscuro");
        var catalog = Load(chiaroscuro);
        Check(catalog.Visuals.Count == 7, "Chiaroscuro must configure all seven visual effects.");
        Check(catalog.SettingToggles.Count == 7 && catalog.SettingToggles.All(t => t.Default && t.Owner.Value == "chiaroscuro"),
            "Chiaroscuro must register seven default-on toggles.");
        foreach (var visual in catalog.Visuals.Values)
            Check(catalog.SettingToggles.Any(t => t.Name == visual.Setting), visual.Effect + " is not gated by its toggle.");
        var trails = catalog.Visuals[ModVisualEffect.WeaponTrails];
        Check(trails.Number("min_speed") == 900f && trails.Number("full_speed") == 2600f && trails.Color == null,
            "Weapon trail numbers were not read.");
        Check(catalog.Visuals[ModVisualEffect.BackgroundDepth].Number("strength") == 0.6f, "Background depth strength was not read.");
        var particles = catalog.Visuals[ModVisualEffect.AmbientParticles];
        Check(particles.DefaultStyle == ModParticleStyle.Dust && particles.Rules.Count == 3 &&
            particles.Rules[0].Style == ModParticleStyle.Snow && particles.Rules[1].Match.Contains("underworld") &&
            particles.Rules[2].Style == ModParticleStyle.Petals, "Particle rules were not read in order.");
        Check(catalog.Visuals[ModVisualEffect.Impact].Number("duration") == 0.3f, "Impact duration was not read.");

        // Defaults, settings.get and a colour.
        var colour = Load(Fixture(fixtures, "fixture.colour", "'presentation.visuals','ui.settings'",
            "local t=sf2.settings.toggle{id='trails',label='Trails'}\n" +
            "assert(sf2.settings.get(t)==false)\n" +
            "sf2.visuals.weapon_trails{color='#FF000080',setting=t}\n" +
            "sf2.visuals.bloom()\n"));
        var coloured = colour.Visuals[ModVisualEffect.WeaponTrails];
        Check(coloured.Color != null && coloured.Color.R == 255 && coloured.Color.A == 128 && coloured.Number("lifetime") == 0.11f,
            "Colour or defaults were not applied.");
        Check(colour.Visuals[ModVisualEffect.Bloom].Setting == null && colour.Visuals[ModVisualEffect.Bloom].Number("intensity") == 0.7f,
            "An ungated effect with defaults was not registered.");

        // Failures.
        Rejects(fixtures, "no-visuals", "'ui.settings'", "sf2.visuals.bloom{}", "presentation.visuals");
        Rejects(fixtures, "no-settings", "'presentation.visuals'", "sf2.settings.toggle{id='a',label='A'}", "ui.settings");
        Rejects(fixtures, "unknown-field", "'presentation.visuals'", "sf2.visuals.bloom{glow=1}", "glow");
        Rejects(fixtures, "range", "'presentation.visuals'", "sf2.visuals.depth_haze{strength=3}", "strength");
        Rejects(fixtures, "speeds", "'presentation.visuals'", "sf2.visuals.weapon_trails{min_speed=500,full_speed=400}", "full_speed");
        Rejects(fixtures, "duplicate", "'presentation.visuals'", "sf2.visuals.bloom{}\nsf2.visuals.bloom{}", "Duplicate visual effect");
        Rejects(fixtures, "style", "'presentation.visuals'", "sf2.visuals.ambient_particles{default_style='rain'}", "default_style");
        Rejects(fixtures, "match", "'presentation.visuals'", "sf2.visuals.ambient_particles{locations={{match={'Snow'},style='snow'}}}", "match word");
        Rejects(fixtures, "setting", "'presentation.visuals'", "sf2.visuals.bloom{setting={}}", "setting");
        Rejects(fixtures, "label", "'ui.settings'", "sf2.settings.toggle{id='a',label=''}", "label");
        Rejects(fixtures, "setting-id", "'ui.settings'", "sf2.settings.toggle{id='Bad Id',label='A'}", "Setting id");

        // Two mods configuring the same effect conflict at commit.
        var shared = new ModContentCatalog();
        Load(Fixture(fixtures, "fixture.first", "'presentation.visuals'", "sf2.visuals.bloom{}"), shared);
        bool conflict = false;
        try { Load(Fixture(fixtures, "fixture.second", "'presentation.visuals'", "sf2.visuals.bloom{}"), shared); }
        catch (Exception error) { conflict = error.ToString().Contains("already configured"); }
        Check(conflict && shared.Visuals[ModVisualEffect.Bloom].Owner.Value == "fixture.first", "A second bloom owner was not rejected.");

        Console.WriteLine("PASS: " + checks + " visuals/settings API checks. Shipped Chiaroscuro package, real Lua bindings, " +
            "defaults, colour, capabilities, validation and cross-mod conflicts. Rendering is not exercised headless.");
    }
}
