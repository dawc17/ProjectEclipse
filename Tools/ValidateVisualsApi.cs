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
        // Chiaroscuro's contents are the owner's choice; check its invariants, not its size.
        Check(catalog.SettingToggles.Count > 0 && catalog.SettingToggles.All(t => t.Default && t.Owner.Value == "chiaroscuro"),
            "Chiaroscuro switches must be its own and default on.");
        foreach (var visual in catalog.Visuals.Values)
            Check(catalog.SettingToggles.Any(t => t.Name == visual.Setting), visual.Effect + " is not gated by its toggle.");
        Check(catalog.Effects.All(e => catalog.SettingToggles.Any(t => t.Name == e.Setting)), "A Chiaroscuro effect is not gated by its toggle.");
        Check(!catalog.Visuals.ContainsKey(ModVisualEffect.WeaponTrails) && !catalog.Visuals.ContainsKey(ModVisualEffect.AmbientParticles),
            "Chiaroscuro builds trails and particles from sf2.fx blocks, not presets.");
        var trails = catalog.Effects.SingleOrDefault(e => e.Kind == ModFxKind.Trail);
        Check(trails == null || (trails.Weapon && trails.Scenes == ModFxScenes.Everywhere && trails.Number("min_speed") == 900f &&
            trails.Number("full_speed") == 2600f && trails.Number("alpha") == 0.55f && trails.Number("start_alpha") == 0.35f &&
            trails.Color == null), "Weapon trail block differs from the preset.");
        if (catalog.Visuals.TryGetValue(ModVisualEffect.BackgroundDepth, out var depth))
            Check(depth.Number("strength") == 0.6f, "Background depth strength was not read.");
        // While particle styles are shipped, each location gets at most one, first match winning.
        var styles = catalog.Effects.Where(e => e.Kind == ModFxKind.Particles &&
            e.Placement != ModFxPlacement.Hit && e.Placement != ModFxPlacement.Node).ToList();
        foreach (string location in new[] { "mountains_ny_25", "hw24_ancient_temple", "uw_boss_arena", "new_year_24_china_dojo", "dojo", "bridge", "hw_ny" })
            Check(styles.Count(e => e.MatchesLocation(location)) <= 1, "More than one particle style runs in " + location + ".");

        // Triggered effects exist only in fights and fire from hits; the knockout
        // fade keeps the blood colour; every hit burst names its trigger.
        Check(catalog.Effects.Where(e => e.Kind == ModFxKind.Particles && e.Placement == ModFxPlacement.Hit)
            .All(e => e.Trigger == ModFxTrigger.Hit || e.Trigger == ModFxTrigger.Block || e.Trigger == ModFxTrigger.Critical),
            "Chiaroscuro hit sparks lost their trigger.");
        var knockouts = catalog.Effects.Where(e => e.Kind == ModFxKind.Screen && e.Trigger == ModFxTrigger.Ko).ToList();
        Check(knockouts.Count == 0 || knockouts.Any(ko => ko.Number("saturation") == 0f && ko.AccentColor != null &&
            ko.Number("accent_strength") > 0f && ko.Number("time_scale") < 1f), "The knockout fade no longer keeps an accent colour or slows time.");
        Check(catalog.Effects.Where(e => e.Kind == ModFxKind.Light && e.Source == ModFxLightSource.Weapon)
            .All(e => e.Weapons.Count > 0 && e.MatchesWeapon("WEAPON_FIRE_BATONS", "FireBatons") != e.MatchesWeapon("WEAPON_ELECTRO_BATONS", "ElectroBatons")),
            "Each weapon light should pick out either fire or electric weapons.");
        var shafts = catalog.Effects.Where(e => e.Kind == ModFxKind.Overlay && e.Shape == ModFxShape.Shaft).ToList();
        Check(shafts.All(e => e.Blend == ModFxBlend.Additive && e.MatchesLocation("dojo") && !e.MatchesLocation("night_bridge")),
            "Light shafts must be additive and limited to lit stages.");
        if (catalog.Visuals.TryGetValue(ModVisualEffect.RimLight, out var rimLight))
            Check(rimLight.Number("ink") >= 0f && rimLight.Number("ink") <= 1f, "Rim ink was not read.");

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

        // sf2.fx building blocks: the shipped showcase through real Lua.
        var showcase = Load(ModDiscovery.DiscoverLoose(mods).Mods.Single(m => m.Id.Value == "example.custom-fx"));
        Check(showcase.Effects.Count == 5 && showcase.SettingToggles.Count == 5 &&
            showcase.Effects.All(e => e.Setting != null && e.Owner.Value == "example.custom-fx"), "Custom FX showcase must register five gated effects.");
        var sparks = showcase.Effects.Single(e => e.Name == "example.custom-fx.blade_sparks");
        Check(sparks.Kind == ModFxKind.Particles && sparks.Placement == ModFxPlacement.Node && sparks.Nodes.Single() == "Weapon-Node2_1" &&
            sparks.Blend == ModFxBlend.Additive && sparks.Scenes == ModFxScenes.Everywhere && sparks.Number("count") == 40f &&
            sparks.EndColor != null && sparks.EndColor.A == 0, "Node particles were not read.");
        var kick = showcase.Effects.Single(e => e.Name == "example.custom-fx.kick_trail");
        Check(kick.Kind == ModFxKind.Trail && !kick.Weapon && kick.Nodes.SequenceEqual(new[] { "NKnee_2", "NHeel_2" }) &&
            kick.Fighters == ModFxFighters.Player && kick.Number("min_speed") == 700f && kick.Number("full_speed") == 2600f,
            "Node trail was not read.");
        var fog = showcase.Effects.Single(e => e.Name == "example.custom-fx.ground_fog");
        Check(fog.Placement == ModFxPlacement.Behind && fog.Number("area_height") == 0.35f && fog.Color.A == 0x30, "Location particles were not read.");
        var glow = showcase.Effects.Single(e => e.Name == "example.custom-fx.dojo_glow");
        Check(glow.Kind == ModFxKind.Overlay && glow.Placement == ModFxPlacement.Background && glow.Number("depth") == 0.8f &&
            glow.MatchesLocation("new_year_24_china_dojo") && !glow.MatchesLocation("bridge"), "Overlay placement or matching failed.");
        var grade = showcase.Effects.Single(e => e.Name == "example.custom-fx.cinema_grade");
        Check(grade.Kind == ModFxKind.Screen && grade.Number("vignette") == 0.35f && grade.Color != null && grade.Number("saturation") == 0.9f,
            "Screen grade was not read.");

        Rejects(fixtures, "fx-trail-both", "'presentation.visuals'", "sf2.fx.trail{id='t',weapon=true,nodes={'NPivot','NTop'}}", "either weapon");
        Rejects(fixtures, "fx-trail-one", "'presentation.visuals'", "sf2.fx.trail{id='t',nodes={'NPivot'}}", "exactly two nodes");
        Rejects(fixtures, "fx-node", "'presentation.visuals'", "sf2.fx.particles{id='p',placement='node'}", "exactly one node");
        Rejects(fixtures, "fx-overlay", "'presentation.visuals'", "sf2.fx.overlay{id='o',placement='behind'}", "background or front");
        Rejects(fixtures, "fx-everywhere", "'presentation.visuals'", "sf2.fx.overlay{id='o',scenes='everywhere'}", "run everywhere");
        Rejects(fixtures, "fx-order", "'presentation.visuals'", "sf2.fx.particles{id='p',size_min=9,size_max=3}", "size_min");
        Rejects(fixtures, "fx-field", "'presentation.visuals'", "sf2.fx.screen{id='s',sharpness=1}", "sharpness");
        Rejects(fixtures, "fx-duplicate", "'presentation.visuals'", "sf2.fx.screen{id='s'}\nsf2.fx.screen{id='s'}", "Duplicate effect");
        Rejects(fixtures, "fx-capability", "'ui.settings'", "sf2.fx.screen{id='s'}", "presentation.visuals");
        Rejects(fixtures, "fx-exclude", "'presentation.visuals'", "sf2.fx.screen{id='s',exclude={'Bad'}}", "exclude word");
        // New building blocks: shadows, glints, hit bursts, triggers, shapes and grade fields.
        var blocks = Load(Fixture(fixtures, "fixture.fx-blocks", "'presentation.visuals'",
            "sf2.fx.shadow{id='shadow',width=120,color='#101010'}\n" +
            "sf2.fx.glint{id='glint',interval=2,scenes='everywhere'}\n" +
            "sf2.fx.particles{id='sparks',placement='hit',speed_min=100,speed_max=300,gravity=500}\n" +
            "sf2.fx.particles{id='crit',placement='hit',trigger='critical'}\n" +
            "sf2.fx.overlay{id='beam',shape='shaft',angle=-15,flicker=0.3}\n" +
            "sf2.fx.screen{id='ko',trigger='ko',saturation=0,accent='#B01010',accent_strength=1,hold=1,duration=2,time_scale=0.25}\n" +
            "sf2.fx.screen{id='look',grain=0.2,halation=0.3,halation_color='#FFA070',vignette=0.4,vignette_x=-0.3,flicker=0.2}\n" +
            "sf2.visuals.rim_light{ink=0.8,ink_color='#1A0C26'}\n" +
            "sf2.fx.light{id='fire',weapons={'fire','flame'},color='#FF8A3A',radius=300,scenes='everywhere'}\n" +
            "sf2.fx.light{id='magic',source='magic',glow=0}\n" +
            "sf2.fx.stain{id='blood',count=4,size_min=5,size_max=9,limit=12}\n" +
            "sf2.fx.stain{id='pool',trigger='ko'}\n"));
        Func<string, ModFxDefinition> block = id => blocks.Effects.Single(e => e.Name == "fixture.fx-blocks." + id);
        Check(block("shadow").Kind == ModFxKind.Shadow && block("shadow").Number("width") == 120f && block("shadow").Color.R == 0x10 &&
            block("shadow").Number("fade_height") == 350f, "Shadow was not read.");
        Check(block("glint").Kind == ModFxKind.Glint && block("glint").Weapon && block("glint").Scenes == ModFxScenes.Everywhere &&
            block("glint").Number("interval") == 2f, "Glint was not read.");
        Check(block("sparks").Placement == ModFxPlacement.Hit && block("sparks").Trigger == ModFxTrigger.Hit &&
            block("sparks").Number("gravity") == 500f && block("crit").Trigger == ModFxTrigger.Critical, "Hit particles were not read.");
        Check(block("beam").Shape == ModFxShape.Shaft && block("beam").Number("angle") == -15f && block("beam").Number("flicker") == 0.3f,
            "Overlay shape, angle or flicker was not read.");
        Check(block("ko").Trigger == ModFxTrigger.Ko && block("ko").AccentColor.R == 0xB0 && block("ko").Number("hold") == 1f &&
            block("ko").Number("time_scale") == 0.25f,
            "Triggered screen grade was not read.");
        Check(block("look").HalationColor != null && block("look").Number("grain") == 0.2f && block("look").Number("vignette_x") == -0.3f,
            "Screen grain, halation or vignette centre was not read.");
        Check(blocks.Visuals[ModVisualEffect.RimLight].Number("ink") == 0.8f && blocks.Visuals[ModVisualEffect.RimLight].Color.B == 0x26,
            "Rim ink or ink colour was not read.");
        Check(block("fire").Kind == ModFxKind.Light && block("fire").Source == ModFxLightSource.Weapon &&
            block("fire").MatchesWeapon("WEAPON_FIRE_BATONS", "FireBatons") && !block("fire").MatchesWeapon("WEAPON_KATANA", "Katana") &&
            block("fire").Number("radius") == 300f && block("fire").Scenes == ModFxScenes.Everywhere, "Weapon light was not read.");
        Check(block("magic").Source == ModFxLightSource.Magic && block("magic").Number("glow") == 0f &&
            block("magic").Number("fighter_light") == 0.8f, "Magic light was not read.");
        Check(block("blood").Kind == ModFxKind.Stain && block("blood").Trigger == ModFxTrigger.Hit && block("blood").Number("count") == 4f &&
            block("blood").Number("limit") == 12f && block("pool").Trigger == ModFxTrigger.Ko, "Stains were not read.");
        Rejects(fixtures, "fx-light-weapons", "'presentation.visuals'", "sf2.fx.light{id='l'}", "needs weapons");
        Rejects(fixtures, "fx-light-magic-weapons", "'presentation.visuals'", "sf2.fx.light{id='l',source='magic',weapons={'fire'}}", "does not accept weapons");
        Rejects(fixtures, "fx-light-magic-everywhere", "'presentation.visuals'", "sf2.fx.light{id='l',source='magic',scenes='everywhere'}", "run everywhere");
        Rejects(fixtures, "fx-light-source", "'presentation.visuals'", "sf2.fx.light{id='l',source='sun'}", "source must be");
        Rejects(fixtures, "fx-stain-block", "'presentation.visuals'", "sf2.fx.stain{id='s',trigger='block'}", "Stains trigger");
        Rejects(fixtures, "fx-stain-size", "'presentation.visuals'", "sf2.fx.stain{id='s',size_min=30,size_max=10}", "size_min");
        Rejects(fixtures, "fx-weapons-kind", "'presentation.visuals'", "sf2.fx.stain{id='s',weapons={'fire'}}", "weapons");
        Rejects(fixtures, "fx-hit-overlay", "'presentation.visuals'", "sf2.fx.overlay{id='o',placement='hit'}", "at hits");
        Rejects(fixtures, "fx-trigger-overlay", "'presentation.visuals'", "sf2.fx.overlay{id='o',trigger='hit'}", "'trigger'");
        Rejects(fixtures, "fx-screen-block", "'presentation.visuals'", "sf2.fx.screen{id='s',trigger='block'}", "Screen effects trigger");
        Rejects(fixtures, "fx-trigger-name", "'presentation.visuals'", "sf2.fx.screen{id='s',trigger='miss'}", "trigger must be");
        Rejects(fixtures, "fx-shape", "'presentation.visuals'", "sf2.fx.overlay{id='o',shape='star'}", "shape must be");
        Rejects(fixtures, "fx-slow-always", "'presentation.visuals'", "sf2.fx.screen{id='s',time_scale=0.5}", "time_scale needs a trigger");
        Rejects(fixtures, "fx-shape-kind", "'presentation.visuals'", "sf2.fx.screen{id='s',shape='glow'}", "shape");
        Rejects(fixtures, "fx-accent-kind", "'presentation.visuals'", "sf2.fx.overlay{id='o',accent='#FF0000'}", "accent");
        Rejects(fixtures, "fx-glint-nodes", "'presentation.visuals'", "sf2.fx.glint{id='g',nodes={'NPivot'}}", "nodes");
        Rejects(fixtures, "fx-shadow-everywhere", "'presentation.visuals'", "sf2.fx.shadow{id='s',scenes='everywhere'}", "run everywhere");
        Rejects(fixtures, "fx-speed-order", "'presentation.visuals'", "sf2.fx.particles{id='p',placement='hit',speed_min=9,speed_max=3}", "speed_min");
        // Several mods may add the same kind of effect.
        var stacked = new ModContentCatalog();
        Load(Fixture(fixtures, "fixture.fx-a", "'presentation.visuals'", "sf2.fx.screen{id='grade',vignette=0.2}"), stacked);
        Load(Fixture(fixtures, "fixture.fx-b", "'presentation.visuals'", "sf2.fx.screen{id='grade',saturation=0.5}"), stacked);
        Check(stacked.Effects.Count == 2, "Two mods could not both add a screen grade.");

        // Two mods configuring the same effect conflict at commit.
        var shared = new ModContentCatalog();
        Load(Fixture(fixtures, "fixture.first", "'presentation.visuals'", "sf2.visuals.bloom{}"), shared);
        bool conflict = false;
        try { Load(Fixture(fixtures, "fixture.second", "'presentation.visuals'", "sf2.visuals.bloom{}"), shared); }
        catch (Exception error) { conflict = error.ToString().Contains("already configured"); }
        Check(conflict && shared.Visuals[ModVisualEffect.Bloom].Owner.Value == "fixture.first", "A second bloom owner was not rejected.");

        Console.WriteLine("PASS: " + checks + " visuals/settings API checks. Shipped Chiaroscuro and Custom FX packages, real Lua bindings, " +
            "defaults, colour, capabilities, validation and cross-mod conflicts. Rendering is not exercised headless.");
    }
}
