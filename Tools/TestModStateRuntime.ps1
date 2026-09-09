$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path $root 'Temp/ModStateRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null

$code = @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Modding;

internal sealed class FakeStateContext : IModScriptContext, IModStateMigrationScriptContext
{
    private readonly bool _fail;
    public ModDescriptor Mod { get; }

    public FakeStateContext(ModDescriptor mod, bool fail = false)
    {
        Mod = mod;
        _fail = fail;
    }

    public void ExecuteEntrypoint() { }
    public void Dispose() { }

    public bool TryMigrateState(int fromVersion, IReadOnlyDictionary<string, ModParameterValue> values,
        out IReadOnlyDictionary<string, ModParameterValue> migrated, out string error)
    {
        migrated = null;
        if (_fail)
        {
            error = "intentional migration failure";
            return false;
        }
        if (fromVersion != 1)
        {
            error = "unexpected source schema";
            return false;
        }

        var next = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, ModParameterValue> pair in values) next.Add(pair.Key, pair.Value);
        next["migrated"] = ModParameterValue.FromBoolean(true);
        migrated = next;
        error = string.Empty;
        return true;
    }
}

internal static class Program
{
    private static void Assert(bool value, string message)
    {
        if (!value) throw new Exception(message);
    }

    private static void RejectContent(Action action, string message)
    {
        try { action(); }
        catch (ModContentException) { return; }
        throw new Exception(message);
    }

    private static string Manifest(string id, string version)
    {
        return "schema = 1\n" +
            "id = \"" + id + "\"\n" +
            "name = \"" + id + "\"\n" +
            "version = \"" + version + "\"\n" +
            "api = \">=0.1 <1.0\"\n" +
            "authors = [\"Test\"]\n" +
            "entrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"state.read\", \"state.write\"]\n\n" +
            "[[dependencies]]\n" +
            "id = \"core\"\n" +
            "version = \">=1.0 <2.0\"\n";
    }

    private static ModDescriptor Descriptor(string version)
    {
        ModManifest manifest = ModManifestReader.ParseExternal(Manifest("state.fixture", version), "state.fixture/mod.toml");
        return new ModDescriptor(manifest, Path.GetFullPath("state.fixture"), ModSourceKind.Loose);
    }

    private static ModParameterSchema SchemaV1()
    {
        return new ModParameterSchema(new[]
        {
            new ModParameterDefinition("wins", ModParameterType.Integer, true, ModParameterValue.FromInteger(0)),
            new ModParameterDefinition("done", ModParameterType.Boolean, true, ModParameterValue.FromBoolean(false)),
            new ModParameterDefinition("note", ModParameterType.String, false),
        });
    }

    private static ModParameterSchema SchemaV2()
    {
        return new ModParameterSchema(new[]
        {
            new ModParameterDefinition("victories", ModParameterType.Integer, true, ModParameterValue.FromInteger(0)),
            new ModParameterDefinition("done", ModParameterType.Boolean, true, ModParameterValue.FromBoolean(false)),
            new ModParameterDefinition("title", ModParameterType.String, false),
            new ModParameterDefinition("migrated", ModParameterType.Boolean, true, ModParameterValue.FromBoolean(false)),
        });
    }

    private static XmlElement StateNode(XmlDocument save)
    {
        return (XmlElement)save.SelectSingleNode("/Warrior/EclipseMods/Mod[@id='state.fixture']/State");
    }

    private static XmlElement ModNode(XmlDocument save)
    {
        return (XmlElement)save.SelectSingleNode("/Warrior/EclipseMods/Mod[@id='state.fixture']");
    }

    private static XmlElement AddValue(XmlElement state, string name, string type, string value)
    {
        XmlElement node = state.OwnerDocument.CreateElement("Value");
        node.SetAttribute("name", name);
        node.SetAttribute("type", type);
        node.SetAttribute("value", value);
        state.AppendChild(node);
        return node;
    }

    public static int Main()
    {
        ModDescriptor v1Mod = Descriptor("1.0.0");
        var save = new XmlDocument();
        save.LoadXml("<Warrior FutureRoot='keep'><OpaqueRoot value='preserve'/></Warrior>");
        Assert(ModSaveData.RecordContext(save.DocumentElement, new[] { v1Mod }),
            "Could not create EclipseMods context for initial state.");

        var stateV1 = new ModStateRuntime();
        stateV1.RegisterDefinition(v1Mod, 1, SchemaV1());
        stateV1.FreezeDefinitions();
        var fingerprintCatalog = new ModContentCatalog();
        string stateFingerprintV1 = ModSaveData.ComputeContentSetFingerprint(
            new[] { v1Mod }, fingerprintCatalog, stateV1);
        var fingerprintStateV2 = new ModStateRuntime();
        fingerprintStateV2.RegisterDefinition(v1Mod, 2, SchemaV2(),
            new Dictionary<string, string>(StringComparer.Ordinal) { { "wins", "victories" } },
            Array.Empty<string>());
        fingerprintStateV2.FreezeDefinitions();
        string stateFingerprintV2 = ModSaveData.ComputeContentSetFingerprint(
            new[] { v1Mod }, fingerprintCatalog, fingerprintStateV2);
        Assert(stateFingerprintV1.StartsWith("sha256:", StringComparison.Ordinal) &&
            stateFingerprintV1 != stateFingerprintV2,
            "Content fingerprint did not include the registered mod-state schema.");
        IReadOnlyList<ModDiagnostic> initialDiagnostics = stateV1.Bind(save.DocumentElement,
            new IModScriptContext[] { new FakeStateContext(v1Mod) });
        Assert(initialDiagnostics.Count == 0, "Initial state bind produced diagnostics.");
        ModParameterValue value;
        Assert(stateV1.TryGetValue(v1Mod.Id, "wins", out value) && value.Integer == 0,
            "Initial integer default was not materialized.");
        Assert(stateV1.TryGetValue(v1Mod.Id, "done", out value) && !value.Boolean,
            "Initial boolean default was not materialized.");

        stateV1.SetValues(v1Mod.Id, new Dictionary<string, ModParameterValue>(StringComparer.Ordinal)
        {
            { "wins", ModParameterValue.FromInteger(3) },
            { "done", ModParameterValue.FromBoolean(true) },
            { "note", ModParameterValue.FromString("first-run") },
        });
        XmlElement v1StateNode = StateNode(save);
        Assert(v1StateNode != null && v1StateNode.GetAttribute("version") == "1" &&
            v1StateNode.GetAttribute("format") == "1", "State identity/version was not serialized.");

        XmlElement futureValue = AddValue(v1StateNode, "future_field", "string", "opaque");
        futureValue.SetAttribute("future", "keep");
        XmlElement futureChild = save.CreateElement("FutureChild");
        futureChild.SetAttribute("opaque", "1");
        v1StateNode.AppendChild(futureChild);
        string createdState = v1StateNode.OuterXml;

        // Exact roadmap sequence: disable/remove the mod and save again. State must remain opaque.
        Assert(ModSaveData.RecordContext(save.DocumentElement, Array.Empty<ModDescriptor>()),
            "Absent-mod context update failed.");
        Assert(ModNode(save).GetAttribute("active") == "false", "Absent mod was not marked inactive.");
        Assert(StateNode(save).OuterXml == createdState, "Absent mod state changed during save-context update.");
        string absentSerialized = save.OuterXml;
        var absentReload = new XmlDocument();
        absentReload.LoadXml(absentSerialized);
        Assert(StateNode(absentReload).OuterXml == createdState, "Absent mod state changed across XML save/reload.");

        // Reinstall the same version and prove ownership/state comes back.
        Assert(ModSaveData.RecordContext(absentReload.DocumentElement, new[] { v1Mod }),
            "Reinstalled mod context update failed.");
        var restoredRuntime = new ModStateRuntime();
        restoredRuntime.RegisterDefinition(v1Mod, 1, SchemaV1());
        restoredRuntime.FreezeDefinitions();
        IReadOnlyList<ModDiagnostic> restoredDiagnostics = restoredRuntime.Bind(absentReload.DocumentElement,
            new IModScriptContext[] { new FakeStateContext(v1Mod) });
        Assert(restoredDiagnostics.Count == 0, "Reinstalled mod state did not bind.");
        Assert(restoredRuntime.TryGetValue(v1Mod.Id, "wins", out value) && value.Integer == 3,
            "Reinstalled mod lost integer progression.");
        Assert(restoredRuntime.TryGetValue(v1Mod.Id, "done", out value) && value.Boolean,
            "Reinstalled mod lost boolean progression.");
        Assert(restoredRuntime.TryGetValue(v1Mod.Id, "note", out value) && value.String == "first-run",
            "Reinstalled mod lost string progression.");
        Assert(StateNode(absentReload).SelectSingleNode("Value[@name='future_field']") != null &&
            StateNode(absentReload)["FutureChild"] != null,
            "Unknown state data was not preserved while the owning mod was active.");

        // Prepare an old-schema fixture with a declarative alias and tombstone for v2.
        XmlElement oldForMigration = StateNode(absentReload);
        AddValue(oldForMigration, "legacy_note", "string", "renamed-value");
        AddValue(oldForMigration, "obsolete", "boolean", "1");
        string v1BeforeMigration = oldForMigration.OuterXml;

        ModDescriptor v2Mod = Descriptor("2.0.0");
        var aliases = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "wins", "victories" },
            { "legacy_note", "title" },
        };
        var tombstones = new[] { "obsolete" };
        var stateV2 = new ModStateRuntime();
        stateV2.RegisterDefinition(v2Mod, 2, SchemaV2(), aliases, tombstones);
        stateV2.FreezeDefinitions();
        IReadOnlyList<ModDiagnostic> migratedDiagnostics = stateV2.Bind(absentReload.DocumentElement,
            new IModScriptContext[] { new FakeStateContext(v2Mod) });
        Assert(migratedDiagnostics.Count == 0, "Successful v1 -> v2 migration produced diagnostics.");
        XmlElement migratedState = StateNode(absentReload);
        Assert(migratedState.GetAttribute("version") == "2", "Successful migration did not bump state schema.");
        Assert(stateV2.TryGetValue(v2Mod.Id, "victories", out value) && value.Integer == 3,
            "Migration did not move wins into victories.");
        Assert(stateV2.TryGetValue(v2Mod.Id, "migrated", out value) && value.Boolean,
            "Migration callback output was not persisted.");
        Assert(stateV2.TryGetValue(v2Mod.Id, "title", out value) && value.String == "renamed-value",
            "Declarative state alias did not move the historical field.");
        Assert(migratedState.SelectSingleNode("Value[@name='wins']") == null &&
            migratedState.SelectSingleNode("Value[@name='legacy_note']") == null &&
            migratedState.SelectSingleNode("Value[@name='obsolete']") == null,
            "Migrated alias/tombstone fields were not canonicalized.");
        Assert(migratedState.SelectSingleNode("Value[@name='future_field']") != null && migratedState["FutureChild"] != null,
            "Migration discarded opaque future state.");

        // Batch writes are transactional. A later invalid field must not partially commit an earlier one.
        string beforeBadWrite = migratedState.OuterXml;
        RejectContent(() => stateV2.SetValues(v2Mod.Id, new Dictionary<string, ModParameterValue>(StringComparer.Ordinal)
        {
            { "victories", ModParameterValue.FromInteger(99) },
            { "done", ModParameterValue.FromString("wrong-type") },
        }), "Invalid batch state write was accepted.");
        Assert(StateNode(absentReload).OuterXml == beforeBadWrite,
            "Failed batch state write partially mutated the save DOM.");
        Assert(stateV2.TryGetValue(v2Mod.Id, "victories", out value) && value.Integer == 3,
            "Failed batch state write partially mutated in-memory state.");

        stateV2.SetValues(v2Mod.Id, new Dictionary<string, ModParameterValue>(StringComparer.Ordinal)
        {
            { "victories", ModParameterValue.FromInteger(4) },
            { "done", ModParameterValue.FromBoolean(false) },
        });
        Assert(stateV2.TryGetValue(v2Mod.Id, "victories", out value) && value.Integer == 4,
            "Valid batch state write did not commit.");
        stateV2.UnsetValue(v2Mod.Id, "title");
        Assert(!stateV2.TryGetValue(v2Mod.Id, "title", out value), "Optional state field could not be unset.");

        // Force the same v1 -> v2 migration to fail. The original State element must be untouched.
        var failingSave = new XmlDocument();
        failingSave.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='state.fixture' version='1.0.0' active='true'>" +
            v1BeforeMigration + "</Mod></EclipseMods></Warrior>");
        string failingOriginal = StateNode(failingSave).OuterXml;
        var failingRuntime = new ModStateRuntime();
        failingRuntime.RegisterDefinition(v2Mod, 2, SchemaV2(), aliases, tombstones);
        failingRuntime.FreezeDefinitions();
        IReadOnlyList<ModDiagnostic> failedDiagnostics = failingRuntime.Bind(failingSave.DocumentElement,
            new IModScriptContext[] { new FakeStateContext(v2Mod, true) });
        Assert(failedDiagnostics.Count == 1 && failedDiagnostics[0].Code == "STATE003" &&
            failedDiagnostics[0].Message.Contains("intentional migration failure"),
            "Failed migration was not diagnosed precisely.");
        Assert(StateNode(failingSave).OuterXml == failingOriginal,
            "Failed migration rewrote old state instead of rolling back.");
        RejectContent(() => failingRuntime.Snapshot(v2Mod.Id),
            "Failed migration left partially bound state accessible.");

        // A newer save schema is also opaque and must not be downgraded.
        var newerSave = new XmlDocument();
        newerSave.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='state.fixture' active='true'>" +
            "<State format='1' version='99'><Value name='future_field' type='string' value='future'/></State>" +
            "</Mod></EclipseMods></Warrior>");
        string newerOriginal = StateNode(newerSave).OuterXml;
        var olderRuntime = new ModStateRuntime();
        olderRuntime.RegisterDefinition(v2Mod, 2, SchemaV2());
        olderRuntime.FreezeDefinitions();
        IReadOnlyList<ModDiagnostic> newerDiagnostics = olderRuntime.Bind(newerSave.DocumentElement,
            new IModScriptContext[] { new FakeStateContext(v2Mod) });
        Assert(newerDiagnostics.Count == 1 && newerDiagnostics[0].Code == "STATE003",
            "Newer state schema was not rejected.");
        Assert(StateNode(newerSave).OuterXml == newerOriginal,
            "Newer opaque state was changed by an older mod schema.");

        Console.WriteLine("Mod state lifecycle: PASS (typed defaults, transactional writes, absent preservation, reinstall, migration, aliases/tombstones, rollback, future schema preservation).");
        return 0;
    }
}
'@

$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code, [Text.UTF8Encoding]::new($false))
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
if (!$csc) { throw 'Could not locate the Visual Studio Roslyn compiler.' }
$sources = @(Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' | Select-Object -ExpandProperty FullName)
$exe = Join-Path $testRoot 'ModStateRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw 'Mod state regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Mod state regression failed.' }
