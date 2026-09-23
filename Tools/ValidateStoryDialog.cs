using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// sf2.ui.story_dialog through the production MoonSharp binding with a controlled
// host service. Native StoryDialog rendering is covered by a Unity playtest only.
sealed class DialogPortraits : IAssetByteProvider
{
    readonly LooseModProvider loose;
    internal DialogPortraits(ModDescriptor mod) { loose = new LooseModProvider(mod); }
    public ModId Namespace => ModId.Parse("fixture.dialog");
    public bool TryRead(AssetId id, out AssetBytes bytes) => loose.TryRead(id, out bytes);
    public bool TryDescribe(AssetId id, out AssetMetadata metadata)
    {
        metadata = null;
        if (id.Namespace != Namespace || !id.Path.StartsWith("portraits/")) return loose.TryDescribe(id, out metadata);
        metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "controlled portrait metadata");
        return true;
    }
}

static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static XmlDocument Read(string path) { var doc = new XmlDocument(); doc.Load(path); return doc; }

    sealed class Lease : IDisposable { public bool Disposed; public Action OnDispose; public void Dispose() { Disposed = true; OnDispose?.Invoke(); } }

    static void Write(string package, string capabilities, string main)
    {
        Directory.CreateDirectory(Path.Combine(package, "scripts"));
        File.WriteAllText(Path.Combine(package, "mod.toml"), "schema = 1\nid = \"fixture.dialog\"\nname = \"Story dialog checks\"\nversion = \"1.0.0\"\nauthors = [\"Eclipse tests\"]\nentrypoint = \"scripts/main.lua\"\ncapabilities = [" + capabilities + "]\n[[dependencies]]\nid = \"core\"\nversion = \">=1.0 <2.0\"\n");
        File.WriteAllText(Path.Combine(package, "scripts/main.lua"), main);
    }

    const string LuaMain = """
local sf2=require("sf2")
local title=sf2.localization.register{id="title",language="eng",value="SENSEI"}
local body=sf2.localization.register{id="body",language="eng",value="Many years ago..."}
local more=sf2.localization.register{id="more",language="eng",value="MORE"}
local ok=sf2.localization.register{id="ok",language="eng",value="OK"}
local core=sf2.localization.key("core:localization/WEAPON_KATANA")
local portrait=sf2.assets.sprite("portraits/sensei")
local function valid(extra)
    local definition={title=title,portrait=portrait,mirrored=true,ignore_back=true,button=ok,
        lines={{text=body,button=more},{text=core}},
        on_complete=function() sf2.log.info("complete") end,on_cancel=function() sf2.log.info("cancel") end}
    for key,value in pairs(extra or {}) do definition[key]=value end
    return definition
end
local invalid={
    missing_button=function() local d=valid() d.button=nil return d end,
    missing_portrait=function() local d=valid() d.portrait=nil return d end,
    empty_lines=function() return valid{lines={}} end,
    many_lines=function() local l={} for i=1,17 do l[i]={text=body} end return valid{lines=l} end,
    sparse_lines=function() return valid{lines={[1]={text=body},[3]={text=body}}} end,
    string_text=function() return valid{lines={{text="Hello"}}} end,
    string_title=function() return valid{title="SENSEI"} end,
    wrong_portrait=function() return valid{portrait=title} end,
    extra_field=function() return valid{id="x"} end,
    extra_line_field=function() return valid{lines={{text=body,frames=5}}} end,
    bad_complete=function() return valid{on_complete=5} end,
    bad_mirrored=function() return valid{mirrored="yes"} end,
}
sf2.story.on("item_acquired",function(event)
    local command=event.item:match("items/(.+)$")
    if command=="open" then sf2.log.info("opened:"..tostring(sf2.ui.story_dialog(valid())))
    elseif command=="minimal" then sf2.log.info("opened:"..tostring(sf2.ui.story_dialog{portrait=portrait,button=ok,lines={{text=body}}}))
    elseif command=="failing" then sf2.log.info("opened:"..tostring(sf2.ui.story_dialog(valid{on_complete=function() error("boom") end})))
    elseif command=="chain" then
        sf2.ui.story_dialog(valid{on_complete=function() sf2.log.info("chained:"..tostring(sf2.ui.story_dialog(valid()))) end})
    end
end)
if INVALID then sf2.ui.story_dialog(invalid[INVALID]()) end
""";

    static void Main(string[] args)
    {
        string package = Path.Combine(args[0], "fixture.dialog");
        Write(package, "\"content.register\", \"story.events\", \"ui.create\"", LuaMain);
        var items = Read(Path.Combine(args[1], "Assets/vanillaXml/list.xml")).SelectNodes("/List/Items/Item").Cast<XmlNode>().ToArray();
        var requests = new List<ModStoryDialogRequest>();
        var completions = new List<Action<bool>>();
        var leases = new List<Lease>();
        bool refuse = false;
        ModStoryDialogAccess.Open = (request, done) =>
        {
            if (refuse) return null;
            requests.Add(request); completions.Add(done);
            var lease = new Lease(); leases.Add(lease); return lease;
        };
        var logs = new List<string>();
        var mod = ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog = new ModContentCatalog();
        CoreContentImporter.ImportWeapons(catalog, items, null);
        var bus = new ModStoryEvents((owner, message) => logs.Add("error:" + message));
        var tx = catalog.BeginRegistration(mod);
        var context = new MoonSharpScriptRuntime(null, null, null, bus).CreateContext(mod,
            new ModApiFacade(mod, new AssetResolver(new IAssetProvider[] { new DialogPortraits(mod) }), tx, new ModStateRuntime(), entry => logs.Add(entry.Message)));
        context.ExecuteEntrypoint(); tx.Commit(); bus.BindProfile();
        void Run(string command) { logs.Clear(); bus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired, DefinitionId.Parse("fixture.dialog:items/" + command), previousCount: 0, count: 1)); }

        Run("open");
        Check(logs.SequenceEqual(new[] { "opened:true" }) && requests.Count == 1, "Valid dialog was not opened: " + string.Join("|", logs));
        var first = requests[0];
        Check(first.Title == "fixture.dialog:localization/title" && first.Button == "fixture.dialog:localization/ok", "Mod localization keys differ");
        Check(first.Lines.Count == 2 && first.Lines[0].Text == "fixture.dialog:localization/body" && first.Lines[0].Button == "fixture.dialog:localization/more", "Line keys differ");
        Check(first.Lines[1].Text == "WEAPON_KATANA" && first.Lines[1].Button == "", "Core localization did not use its legacy native key");
        Check(first.Portrait == "fixture.dialog:portraits/sensei" && first.Mirrored && first.IgnoreBack, "Portrait/mirroring/IgnoreBack differ");
        Run("open");
        Check(logs.SequenceEqual(new[] { "opened:false" }) && requests.Count == 1, "Concurrent dialog was not refused");
        logs.Clear(); completions[0](true); completions[0](true); completions[0](false);
        Check(logs.SequenceEqual(new[] { "complete" }), "Completion was not exactly once: " + string.Join("|", logs));

        Run("open"); logs.Clear(); completions[1](false); completions[1](true);
        Check(logs.SequenceEqual(new[] { "cancel" }), "Cancellation was not exactly once: " + string.Join("|", logs));

        Run("minimal");
        Check(logs.SequenceEqual(new[] { "opened:true" }) && requests[2].Title == "" && !requests[2].Mirrored && !requests[2].IgnoreBack, "Optional fields did not default");
        logs.Clear(); completions[2](true);
        Check(logs.Count == 0, "Omitted callbacks were invoked");

        Run("chain"); logs.Clear(); completions[3](true);
        Check(logs.SequenceEqual(new[] { "chained:true" }) && requests.Count == 5, "A completion callback could not open the next dialog: " + string.Join("|", logs));
        completions[4](true);

        Run("failing"); logs.Clear(); completions[5](true);
        Check(logs.Any(value => value.Contains("Story dialog callback failed") && value.Contains("boom")), "Callback error was not reported");
        Run("open"); Check(logs.SequenceEqual(new[] { "opened:true" }), "Failed callback left the dialog slot busy");
        completions[6](true);

        refuse = true; Run("open"); refuse = false;
        Check(logs.SequenceEqual(new[] { "opened:false" }) && requests.Count == 7, "Host refusal did not return false");

        foreach (var command in new[] { "missing_button", "missing_portrait", "empty_lines", "many_lines", "sparse_lines", "string_text",
            "string_title", "wrong_portrait", "extra_field", "extra_line_field", "bad_complete", "bad_mirrored" })
        {
            string root = Path.Combine(Path.GetDirectoryName(args[0]), "invalid-" + command);
            string invalidPackage = Path.Combine(root, "fixture.dialog");
            Write(invalidPackage, "\"content.register\", \"story.events\", \"ui.create\"", "INVALID=\"" + command + "\"" + Environment.NewLine + LuaMain);
            var invalidMod = ModDiscovery.DiscoverLoose(root).Mods.Single();
            var invalidCatalog = new ModContentCatalog(); CoreContentImporter.ImportWeapons(invalidCatalog, items, null);
            string failure = null;
            using (var invalidTx = invalidCatalog.BeginRegistration(invalidMod))
            using (var invalidContext = new MoonSharpScriptRuntime(null, null, null, new ModStoryEvents((o, m) => { })).CreateContext(invalidMod,
                new ModApiFacade(invalidMod, new AssetResolver(new IAssetProvider[] { new DialogPortraits(invalidMod) }), invalidTx, new ModStateRuntime(), entry => { })))
            {
                try { invalidContext.ExecuteEntrypoint(); } catch (Exception error) { failure = error.ToString(); }
            }
            Check(failure != null && failure.Contains("sf2.ui.story_dialog"), "Invalid request accepted: " + command + " " + failure);
        }
        Check(requests.Count == 7, "Invalid requests reached the host");

        Run("open"); var pending = leases.Last();
        context.Dispose(); logs.Clear(); completions.Last()(true);
        Check(pending.Disposed && logs.Count == 0, "Context teardown did not release the dialog silently");

        // Missing ui.create capability.
        Directory.Delete(package, true);
        Write(package, "\"content.register\", \"story.events\"", LuaMain);
        var restricted = ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog2 = new ModContentCatalog(); CoreContentImporter.ImportWeapons(catalog2, items, null);
        var bus2 = new ModStoryEvents((owner, message) => logs.Add("error:" + message));
        var tx2 = catalog2.BeginRegistration(restricted);
        using (var context2 = new MoonSharpScriptRuntime(null, null, null, bus2).CreateContext(restricted,
            new ModApiFacade(restricted, new AssetResolver(new IAssetProvider[] { new DialogPortraits(restricted) }), tx2, new ModStateRuntime(), entry => logs.Add(entry.Message))))
        {
            context2.ExecuteEntrypoint(); tx2.Commit(); bus2.BindProfile();
            logs.Clear(); int before = requests.Count;
            bus2.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired, DefinitionId.Parse("fixture.dialog:items/open"), previousCount: 0, count: 1));
            Check(requests.Count == before && logs.Any(value => value.Contains("ui.create")), "Missing capability opened a dialog: " + string.Join("|", logs));
        }
        ModStoryDialogAccess.Clear();
        Console.WriteLine("PASS: " + checks + " story dialog binding checks; native localization keys (mod and core), portrait, mirroring, IgnoreBack, busy/refused, exactly-once completion and cancellation, chaining, callback errors, validation, capability and teardown. Controlled host; no native rendering.");
    }
}
