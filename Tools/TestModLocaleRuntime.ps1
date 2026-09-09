$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ModLocaleRuntime-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null

# Compile the production locale implementation and parser against small host doubles.
$source = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModAssetLoader.cs')
$start = $source.IndexOf('    internal sealed class ExternalLocaleMetadata')
$end = $source.IndexOf('    internal static class ExternalCombatContentRuntime', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Cannot locate locale runtime source.' }
$locale = 'using System; using System.Collections.Generic; using System.Xml; namespace Eclipse.Modding {' + $source.Substring($start, $end - $start) + '}'
Set-Content -Encoding UTF8 (Join-Path $fixture 'LocaleRuntime.cs') $locale
$runtime = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntimeP1D.cs')
$start = $runtime.IndexOf('        public static void ApplyLocaleMetadata()')
$end = $runtime.IndexOf('        public static void ApplyP1DContent()', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'Cannot locate guarded locale binding source.' }
Set-Content -Encoding UTF8 (Join-Path $fixture 'LocaleBinding.cs') ('using System; using UnityEngine; namespace Eclipse.Modding { public static partial class ModRuntime {' + $runtime.Substring($start, $end - $start) + '}}')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ParseModule.cs') -Destination $fixture

@'
using System;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;

public static class LocalizationManager
{
    public sealed class Language
    {
        public string name, EOMNCDDELLB, PMFEIPCHENB;
        public int index;
        public XmlNode Metadata;
        public Language(XmlNode node, int position)
        {
            name = node.Attributes["Name"].Value;
            EOMNCDDELLB = node.Attributes["Locale"].Value;
            PMFEIPCHENB = name + ".xml";
            index = position;
            Metadata = node.CloneNode(true);
        }
    }
    public static List<Language> MCLNNPPCFFL = new List<Language>();
    public static string POIPGLLCCKC = "eng";
    public static Language ILAJKOBCHFH;
    public static Language NLFKNPBICED(string name) => MCLNNPPCFFL.Find(x => x.name == name);
    public static void Init() { }
}
public class LoadingModule { public bool CHIHBINEGFL; public virtual void JLPMOKPFECK() { } }
public static class GameUtils { public static void InitVariables() { } public static void OEKOKKCILAG() { } }
public static class GameSettings { public static void OCIPKAONMOP() { } public static void LNNLDPLDABI() { } }
public static class GameLoader { public static void BJLLJHDFMOO() { } public static void POLKDKOOACO() { } public static void SetSound() { } }
public sealed class ListSF
{
    public static int ParseCount;
    private static readonly ListSF Instance = new ListSF();
    public static ListSF ELEBLBJKDBI() => Instance;
    public static ListSF CCDKHLAMKKO() => Instance;
    public void IIKDNMBIHCM() { ParseCount++; }
    public void AFAKCAMAACM() { }
}
public sealed class PerkTree
{
    public static PerkTree GBPBIPFIOJH() => new PerkTree();
    public void LJHPGKAOIAE() { }
}
namespace UnityEngine { public static class Debug { public static void LogError(object value) { } } }
namespace Eclipse.Modding
{
    public static partial class ModRuntime
    {
        private static object _legacyContent = new object();
        public static int ShutdownCount;
        public static void Shutdown() { ShutdownCount++; ExternalLocaleRuntime.Clear(); _legacyContent = null; }
        public static void ApplyLegacyLocalization() { }
    }
}
internal static class Program
{
    private static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    private static void Reject(Action action, string message)
    {
        try { action(); } catch (InvalidOperationException) { return; }
        throw new Exception(message);
    }
    private static ExternalLocaleMetadata Entry(string name, string locale) => new ExternalLocaleMetadata { Name = name, Locale = locale };
    public static void Main(string[] args)
    {
        var document = new XmlDocument(); document.Load(args[0]);
        XmlNode languages = document["Localization"]["Languages"];
        LocalizationManager.POIPGLLCCKC = languages.Attributes["Default"].Value;
        foreach (XmlNode node in languages.ChildNodes)
            if (node.NodeType == XmlNodeType.Element)
                LocalizationManager.MCLNNPPCFFL.Add(new LocalizationManager.Language(node, LocalizationManager.MCLNNPPCFFL.Count));
        var baseline = LocalizationManager.MCLNNPPCFFL.ToArray();
        var fallback = LocalizationManager.NLFKNPBICED(LocalizationManager.POIPGLLCCKC);
        ExternalLocaleRuntime.Add(Entry("eng_showcase", "en"));
        Reject(() => ExternalLocaleRuntime.ValidateBaseLanguages(languages), "Original showcase collision was accepted.");
        ExternalLocaleRuntime.Clear();

        ExternalLocaleRuntime.Add(Entry("eng_showcase", "en-x-phase1"));
        ExternalLocaleRuntime.ValidateBaseLanguages(languages);
        ExternalLocaleRuntime.Apply(); ExternalLocaleRuntime.Apply();
        Check(LocalizationManager.MCLNNPPCFFL.Count == baseline.Length + 1, "Repeated apply duplicated locales.");
        var added = LocalizationManager.NLFKNPBICED("eng_showcase");
        Check(added.PMFEIPCHENB == fallback.PMFEIPCHENB, "Custom locale lost base game text.");
        Check(added.Metadata["Fonts"].OuterXml == fallback.Metadata["Fonts"].OuterXml, "Default fonts were not inherited.");
        LocalizationManager.ILAJKOBCHFH = added;
        ExternalLocaleRuntime.Clear();
        Check(LocalizationManager.ILAJKOBCHFH == fallback, "Teardown retained removed selection.");
        Check(LocalizationManager.MCLNNPPCFFL.Count == baseline.Length, "Teardown removed base locales or retained external locale.");
        for (int i = 0; i < baseline.Length; i++) Check(LocalizationManager.MCLNNPPCFFL[i] == baseline[i], "Base locale identity changed.");

        ExternalLocaleRuntime.Add(Entry("valid_overlay", "en-x-valid"));
        ExternalLocaleRuntime.Add(Entry("invalid_overlay", "EN"));
        Reject(() => ExternalLocaleRuntime.Apply(), "Late collision was accepted.");
        Check(LocalizationManager.MCLNNPPCFFL.Count == baseline.Length, "Late collision partially applied locales.");

        // Reproduce the late-bind failure at the actual ParseModule call site.
        var parser = new ParseModule(); parser.JLPMOKPFECK(); parser.JLPMOKPFECK();
        Check(parser.CHIHBINEGFL && ListSF.ParseCount == 1, "Locale failure restarted core parsing and duplicated zones.");
        Check(ModRuntime.ShutdownCount == 1, "Late locale failure did not disable the invalid overlay.");
        Check(LocalizationManager.MCLNNPPCFFL.Count == baseline.Length, "Failure cleanup damaged base locales.");
        Console.WriteLine("Mod locale runtime PASS: canonical collision, atomic/idempotent binding, font/text fallback, owned cleanup, parser retry regression.");
    }
}
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
@'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><Nullable>disable</Nullable></PropertyGroup>
</Project>
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'LocaleRuntime.csproj')
dotnet run --project (Join-Path $fixture 'LocaleRuntime.csproj') -- (Join-Path $root 'Assets/vanillaXml/localization.xml')
if ($LASTEXITCODE -ne 0) { throw "Locale runtime regression failed: $LASTEXITCODE" }
