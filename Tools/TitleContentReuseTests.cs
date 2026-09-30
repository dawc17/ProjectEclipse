using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.UI;
using Eclipse.Modding;

static class Probe
{
    public static readonly Dictionary<string, int> Counts = new();
    public static bool FailPreview, FailZones;
    public static int Checks;
    public static void Hit(string name) { Counts[name] = Count(name) + 1; }
    public static int Count(string name) => Counts.TryGetValue(name, out int n) ? n : 0;
    public static void Check(bool condition, string message) { Checks++; if (!condition) throw new Exception(message); }
    public static XmlDocument Profile(int denomination)
    {
        var doc = new XmlDocument();
        doc.LoadXml("<Root><CurrentUser ID='1'/><Warriors><Warrior ID='1' Denomination='" + denomination + "'/></Warriors><Billing/></Root>");
        return doc;
    }
    public static void Clean()
    {
        TitleScreen.Discard(); Nekki.SF2.GUI.Scenes.GameLoaderScene.Stop();
        Eclipse.Saves.CampaignSaveSession.PreviewDirectory = null;
        Eclipse.Multiplayer.LocalVersusSession.IsActive = false;
        Counts.Clear(); FailPreview = FailZones = false;
    }
}

static class Program
{
    public static void Main(string[] args)
    {
        SF2Paths.FFKEDOBDLOL = args[0]; Directory.CreateDirectory(args[0]);
        foreach (bool local in new[] { false, true })
        {
            Probe.Clean();
            TitleScreen.Load(); TitleScreen.Load();
            var items = ListSF.GetItems(); var scripts = ModRuntime.Scripts;
            var oldRoster = ListSF.CCDKHLAMKKO();
            string preview = Eclipse.Saves.CampaignSaveSession.PreviewDirectory;
            Probe.Check(Probe.Count("scripts") == 1 && Probe.Count("items") == 1, "Preview loaded twice");
            Probe.Check(Probe.Count("bind") == 0 && Probe.Count("migrate") == 0 && Probe.Count("story") == 0, "Preview bound mod save state");
            Probe.Check(items.Price == 1000 && Probe.Count("denominate") == 0 && Probe.Count("billing") == 0, "Preview changed shared prices");
            Probe.Check(Directory.Exists(preview), "Preview sandbox missing");
            Probe.Check(GameUtils.FDEJIIDIPBI.GFPFNILGJML().Count == 2 && PerkTree.GBPBIPFIOJH().Available == "campaign", "Preview mutated perk catalog");
            var hidden = new ItemInfo { DCHJDPCEODD = false, UpgradeLevel = 7 };
            var previewItem = new UserItem(); previewItem.KIGHKCOCJFJ(hidden);
            Probe.Check(!hidden.DCHJDPCEODD && previewItem.Definition.DCHJDPCEODD && !ReferenceEquals(hidden, previewItem.Definition), "Preview unlocked shared shop definition");
            Eclipse.Multiplayer.LocalVersusSession.IsActive = local;
            TitleScreen.Enter(); TitleScreen.Enter();
            Probe.Check(ListSF.CCDKHLAMKKO() == null && !oldRoster.HasSaveListener, "Preview roster still attached");
            Probe.Check(ListSF.GetInstance().ProfileReleased, "Preview XML/pending save retained");
            Probe.Check(Eclipse.Saves.CampaignSaveSession.PreviewDirectory == null && !Directory.Exists(preview), "Sandbox survived handoff");
            previewItem.Definition.IsNew = true;
            Probe.Check(!hidden.IsNew, "Delayed preview inventory changed shared item");
            var gameItem = new UserItem(); gameItem.KIGHKCOCJFJ(hidden);
            Probe.Check(ReferenceEquals(gameItem.Definition, hidden) && hidden.DCHJDPCEODD, "Real inventory did not unlock its shared definition");
            var parse = new ParseModule(); parse.JLPMOKPFECK(); parse.JLPMOKPFECK();
            Probe.Check(ReferenceEquals(items, ListSF.GetItems()) && ReferenceEquals(scripts, ModRuntime.Scripts), "Content session replaced on entry");
            foreach (string step in new[] { "settings", "animations", "AI", "items", "scripts", "conditions", "battle types", "sound", "localization" })
                Probe.Check(Probe.Count(step) == 1, "Shared step repeated: " + step);
            foreach (string step in new[] { "profile", "perk tree", "settings finish", "finish" })
                Probe.Check(Probe.Count(step) == 2, "Profile step missing: " + step);
            foreach (string step in new[] { "denominate", "warriors", "zones", "stages", "quests", "mod quests", "periodic", "packs", "timer", "locale metadata", "legacy localization", "roster finish", "bind", "migrate", "story", "billing" })
                Probe.Check(Probe.Count(step) == 1, "Campaign step did not run once: " + step);
            Probe.Check(items.Price == 10, "Selected profile denomination not applied exactly once");
            Probe.Check(GameUtils.FDEJIIDIPBI.GFPFNILGJML().Count == 2 && PerkTree.GBPBIPFIOJH().Available == "preview", "Campaign inherited preview perk choices");
            Probe.Check(!ReferenceEquals(oldRoster, ListSF.CCDKHLAMKKO()) && ListSF.CCDKHLAMKKO().Denomination == 2, "Selected profile not loaded");
            Probe.Check(ListSF.GetInstance().IsDisposable == local, "Local versus profile ownership lost");
            Probe.Check(ReferenceEquals(ModRuntime.BoundWarrior.OwnerDocument, XmlUtils.Selected) != local, "Local profile clone ownership incorrect");
            Probe.Check(!TitleScreen.TryResumeGameDataPreview(), "Handoff consumed twice");
        }
        Probe.Clean(); TitleScreen.Load(); var priorItems = ListSF.GetItems(); var priorScripts = ModRuntime.Scripts;
        TitleScreen.Discard();
        Probe.Check(!ListSF.CanResumeTitlePreview && ListSF.CCDKHLAMKKO() == null && ModRuntime.Scripts == null, "Mod restart retained content");
        TitleScreen.Load(); TitleScreen.Enter(); new ParseModule().JLPMOKPFECK();
        Probe.Check(Probe.Count("scripts") == 2 && Probe.Count("animations") == 2, "Mod restart did not reload content");
        Probe.Check(!ReferenceEquals(priorItems, ListSF.GetItems()) && !ReferenceEquals(priorScripts, ModRuntime.Scripts), "Restart reused old selection");
        Probe.Clean(); new ParseModule().JLPMOKPFECK();
        Probe.Check(Probe.Count("scripts") == 1 && Probe.Count("profile") == 1 && Probe.Count("zones") == 1, "Cold boot broken");
        Probe.Clean(); Probe.FailPreview = true; TitleScreen.Load(); Probe.FailPreview = false;
        Probe.Check(!ListSF.CanResumeTitlePreview && Eclipse.Saves.CampaignSaveSession.PreviewDirectory == null, "Failed preview retained partial content");
        new ParseModule().JLPMOKPFECK(); Probe.Check(Probe.Count("zones") == 1 && Probe.Count("bind") == 1, "Failed preview blocked cold boot");
        Probe.Clean(); TitleScreen.Load(); TitleScreen.Enter();
        Nekki.SF2.GUI.Scenes.GameLoaderScene.Stop(); new ParseModule().JLPMOKPFECK();
        Probe.Check(Probe.Count("scripts") == 2 && Probe.Count("zones") == 1, "Stale handoff skipped cold boot");
        Probe.Clean(); TitleScreen.Load(); TitleScreen.Enter(); Probe.FailZones = true;
        try { new ParseModule().JLPMOKPFECK(); throw new Exception("Expected load failure"); } catch (InvalidOperationException) { }
        Probe.Check(!ListSF.CanResumeTitlePreview && ModRuntime.Scripts == null, "Failed handoff retained partial state");
        Probe.FailZones = false; new ParseModule().JLPMOKPFECK();
        Probe.Check(Probe.Count("scripts") == 2 && Probe.Count("denominate") == 2 && ListSF.GetItems().Price == 10, "Failed handoff retry reused denominated items");
        Console.WriteLine("PASS: " + Probe.Checks + " title/campaign/local-versus reuse, profile isolation, denominations, restart and failure checks.");
        Probe.Clean();
    }
}

namespace UnityEngine { public static class Debug { public static void Log(object value) {} public static void LogWarning(object value) {} } }
public class LoadingModule { protected bool CHIHBINEGFL; public virtual void Start() {} public bool GCHANFIHDGH() => CHIHBINEGFL; public virtual void JLPMOKPFECK() { CHIHBINEGFL = true; } }
public class PreInitializationModule : LoadingModule { }
public class AntichitingModule : LoadingModule { }
public class InitializationModule : LoadingModule { }
public static class SF2Paths { public static string FFKEDOBDLOL; public static string KKIDGPBOBNI() => "content"; public static string GetUserDataDirectory() => Eclipse.Saves.CampaignSaveSession.PreviewDirectory ?? "selected"; }
public static class Constants { public const string OJMIJINKBPJ = "users.xml"; }
public static class XmlCryptoUtils { public static object NNLGALNDJCL() => null; }
public static class XmlUtils
{
    public enum EBLFEPIOMOL { Normal }
    public static XmlDocument Selected = Probe.Profile(2);
    public static XmlDocument OpenXMLDocument(string path, string name, EBLFEPIOMOL mode, bool required, object crypto) => Probe.Profile(1);
    public static void ONLDJNLKKAL(XmlDocument doc, string path) => doc.Save(path);
    public static XmlDocument AIFIAKNJMHG(string path, string name)
    {
        Probe.Hit("profile");
        if (path == "selected") return Selected;
        var doc = new XmlDocument(); doc.Load(Path.Combine(path, name)); return doc;
    }
}
public static class XmlExtensions { public static string CIPOICEEIBK(this XmlAttribute value, string fallback = "") => value?.Value ?? fallback; public static int ParseInt(this XmlAttribute value) => int.Parse(value.Value); }
public class Items { public int Price = 1000; public object HCDLKHKBEPF() => this; }
public class Roster
{
    public int Denomination; public bool HasSaveListener = true;
    public Roster(XmlNode node, object parameters) { Denomination = int.Parse(node?.Attributes["Denomination"].Value ?? "0"); }
    public void RemoveEventListener(int id, Action<object> handler) => HasSaveListener = false;
    public Roster KHCNHPCPFII() => this; public void HOMCPNCGPDB(object items) { }
    public Roster get_Parameters() => this; public void NOBKKLBJFIL() { }
    public void AFAKCAMAACM() => Probe.Hit("roster finish");
    public Roster JLBDOBLHHAF() => this;
    public PerkHistory GIAEMMLABDL => new() { JOGBKOJCINM = new() { new PerkHistory.Perk { Name = Denomination == 1 ? "preview" : "campaign" } } };
}
public partial class ListSF
{
    static ListSF _instance; static Roster ANEHEDFAPCH; static Items _items = new();
    XmlDocument IEDEFCBFJAD, IEGJHNHFJFA; XmlNode _CurrentUserNode;
    bool _localVersusProfile, GJEJCLBAPMP, JGFGMICMBKL;
    internal static Action<string, long> StepTimer;
    public bool ProfileReleased => IEDEFCBFJAD == null && _CurrentUserNode == null && !GJEJCLBAPMP;
    public bool IsDisposable => _localVersusProfile;
    public static ListSF GetInstance() => _instance ??= new();
    public static Roster CCDKHLAMKKO() => ANEHEDFAPCH;
    public static Items GetItems() => _items;
    void EJANJEEGOOE(object data) => GJEJCLBAPMP = true;
    void ILFBDHDMHPD(object data) { }
    void EAFEBFMIDLF() => Probe.Hit("battle types");
    void NMMBHENGDJO() => Probe.Hit("items");
    void OFIPOGGCKIN() => Probe.Hit("warriors");
    void KIEEPEOPJGB() { if (Probe.FailZones) throw new InvalidOperationException("zones failed"); Probe.Hit("zones"); }
    void PDCHBPKOBFI(string source) => Probe.Hit("quests");
    void LDADJAGGGPA() => Probe.Hit("periodic");
    Roster NHAMDLEDOHM(XmlNode node) => new(node, null);
    void JMDJEEFELCD(XmlNode node) => Probe.Hit("billing");
}
public class ItemInfo
{
    public bool DCHJDPCEODD, IsNew; public int UpgradeLevel;
    public ItemInfo Clone() => (ItemInfo)MemberwiseClone();
    public static void DenominateItems() { Probe.Hit("denominate"); ListSF.GetItems().Price /= (int)Math.Pow(10, ListSF.CCDKHLAMKKO().Denomination); }
}
public partial class UserItem
{
    ItemInfo EOFKPLMPGLD; int IKNDJDEODFD = -1;
    public ItemInfo Definition => EOFKPLMPGLD;
    void FMMDLMGHPIB(int level) => IKNDJDEODFD = level;
}
public class GlobalTimer { static readonly GlobalTimer Timer = new(); public static GlobalTimer get_Instance() => Timer; public void removeEventListener(int id, Action<object> action) {} public void addEventListener(int id, Action<object> action) => Probe.Hit("timer"); }
public static class QuestsManager { public static void Reset() {} }
public static class Module { public static void Reset() {} }
public static class ServerProvider { public static void Reset() {} }
public static class AnimationData { public static void BCILLFEBJHK() {} }
public static class AiData { public static void ClearAll() {} }
public class PacksController { public static PacksController ELEBLBJKDBI() => new(); public void GDNFPIBDDBO() => Probe.Hit("packs"); }
public static class GameUtils
{
    public static bool OBJEKOBDMOE, GCDIGFODNFO;
    public static Conditions OJNHPHEPFLI = new();
    public static PerkItems FDEJIIDIPBI = new();
    public class Conditions { public void AEPHNNABOEK() => Probe.Hit("conditions"); }
    public static void InitVariables() {} public static void OEKOKKCILAG() => Probe.Hit("finish");
}
public static class GameSettings { public static void OCIPKAONMOP() => Probe.Hit("settings"); public static void LNNLDPLDABI() => Probe.Hit("settings finish"); }
public static class GameLoader { public static void BJLLJHDFMOO() { Probe.Hit("animations"); if (Probe.FailPreview) throw new Exception("animations failed"); } public static void POLKDKOOACO() => Probe.Hit("AI"); public static void SetSound() => Probe.Hit("sound"); }
public static class LocalizationManager { public static void Init() => Probe.Hit("localization"); }
public class PerkInfoItem { public string Name; }
public class PerkHistory { public class Perk { public string Name; } public List<Perk> JOGBKOJCINM; }
public class PerkItems { readonly List<PerkInfoItem> Definitions = new() { new() { Name = "preview" }, new() { Name = "campaign" } }; public List<PerkInfoItem> GFPFNILGJML() => Definitions; }
public partial class PerkTree
{
    static readonly PerkTree Instance = new(); public static PerkTree GBPBIPFIOJH() => Instance;
    public class PerkBranch { public int Level; public List<object> OJIAKDDCGLB = new(); }
    List<object> HBDGGFOPFFB = new(), DPPMNFCIIGP = new(); List<PerkInfoItem> PPHJHENDCLL = new();
    public string Available => PPHJHENDCLL.Count == 1 ? PPHJHENDCLL[0].Name : "";
    public List<PerkBranch> LGGMDGDHJJP() => new(); void AHPDPEDGJLM(int level, int count) { }
    void AEOKBBBAANA(PerkBranch branch) { }
    void AEOKBBBAANA(PerkHistory.Perk perk) => PPHJHENDCLL.RemoveAll(p => p.Name == perk.Name);
    void DCBCNJJCAMP() => Probe.Hit("perk tree");
}
namespace Eclipse.Saves { public static class CampaignSaveSession { public static string PreviewDirectory; } }
namespace Eclipse.Multiplayer
{
    public static class LocalVersusSession { public static bool IsActive; }
    public static class VersusRoster { public static bool GameDataLoaded => Probe.Count("items") > Probe.Count("shutdown"); public static void Reset() {} }
}
namespace Eclipse.Modding
{
    public class ModContentException : Exception { }
    public class ModDiagnostic { }
    public class StateStub { public void Unbind() {} }
    public class Session { public object ActiveMods, Content; public StateStub State = new(); public IReadOnlyList<ModDiagnostic> BindState(XmlNode node) { Bound(node); return Array.Empty<ModDiagnostic>(); } void Bound(XmlNode node) { Probe.Hit("bind"); Probe.Hit("migrate"); ModRuntime.BoundWarrior = node; } }
    public static partial class ModRuntime
    {
        static Session _scripts; public static Session Scripts => _scripts;
        static int _profileMutationState; static IDisposable _battleLotteryPresentation;
        static XmlNode _lotteryProfileNode; static Roster _profileRoster;
        public static XmlNode BoundWarrior;
        public static void StartGameContent() { Probe.Hit("scripts"); _scripts = new(); }
        public static void Shutdown() { Probe.Hit("shutdown"); _scripts = null; }
        public static void UnbindProfile() => BoundWarrior = null;
        public static void ApplyStageContent() => Probe.Hit("stages"); public static void ApplyQuestContent() => Probe.Hit("mod quests");
        public static void ApplyLocaleMetadata() => Probe.Hit("locale metadata"); public static void ApplyLegacyLocalization() => Probe.Hit("legacy localization");
    }
    public static class ModActScreenPresenter { public static void CancelActive() {} }
    public static class ModStoryDialogPresenter { public static void CancelActive() {} }
    public static class StoryEvents { public static void UnbindProfile() {} public static void BindProfile() => Probe.Hit("story"); }
    public static class DojoSelection { public static void Unbind() {} public static void Bind(XmlNode node) {} }
    public static class ModSaveData { public static bool RecordContext(XmlNode node, object mods, object content, object state) => true; }
    public static class ModModeRuntime { public static void Bind(XmlNode node) {} }
}
