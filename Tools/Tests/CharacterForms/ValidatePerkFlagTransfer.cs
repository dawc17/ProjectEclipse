// Template populated with production methods by TestPerkFlagTransfer.ps1.
// Model services, expression evaluation and final event routing are controlled.
// The production action lifecycle, namespace registry and form stages execute here.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

public class FunctionResult
{
    public string DCJLKCFKCOM;
    public int ToInt() => int.Parse(DCJLKCFKCOM, CultureInfo.InvariantCulture);
}

public class FunctionExtension
{
    public class CallbackResult { }
    string value;
    public int Reads;
    public void Parse(string text) { value = text; }
    public FunctionResult IBCPKBBAFNH() { Reads++; return new FunctionResult { DCJLKCFKCOM = value }; }
    public void PBPBNENGLPA(Action<CallbackResult> callback) { }
    public void DMPCFMACDJM(Action<CallbackResult> callback) { }
    public void set_Target(object target) { }
}

// The native flag/variable classes and their modifier base are compiled separately.
// Only the common XML/expression services below are controlled by the fixture.
public class PerkAction
{
    string name, scope;
    ActionType type;
    bool modifier;
    FunctionExtension frames;
    public PerkInfoItem Definition;
    public PerkAction() { }
    public PerkAction(PerkAction source)
    {
        name = source.name; scope = source.scope; type = source.type;
        modifier = source.modifier; frames = source.frames; Definition = source.Definition;
    }
    public virtual void Parse(XmlNode node)
    {
        name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
        scope = node.Attributes["Namespace"].CIPOICEEIBK(string.Empty);
        if (node.Attributes["Frames"] != null)
        {
            frames = new FunctionExtension();
            frames.Parse(node.Attributes["Frames"].Value);
        }
    }
    public string get_Name() => name;
    public string IONIEDIPEGB() => scope;
    public ActionType get_Type() => type;
    protected void set_Type(ActionType value) { type = value; }
    public bool NKAEEFNNBEN() => modifier;
    protected void set_Modificator(bool value) { modifier = value; }
    public FunctionExtension BFJEFNHKPJI() => frames;
    public PerkInfoItem JMDLAMHAJLN() => Definition;
}

class PerkActionSetAttributes : PerkActionModificator
{
    public PerkActionSetAttributes() { set_Type(ActionType.ACTION_SET_ATTRIBUTES); }
}
class ModHealthChange : PerkActionModificator { }
class UnsupportedModifier : PerkActionModificator
{
    public UnsupportedModifier(ActionType type) { set_Type(type); }
}
class ItemInfo { }
public class PerkInfoItem
{
    public string Name = "fixture:flag-lifetime";
    public void HJFEFJIEINN(FunctionExtension.CallbackResult result) { }
    public void OKPFNCJFLDL(FunctionExtension.CallbackResult result) { }
}
class PerkData
{
    public PerkInfoItem MBDDKGIOOGD;
    public bool Enabled = true;
    public PerkData(PerkInfoItem definition) { MBDDKGIOOGD = definition; }
}
public class ModelParameters { public List<PerkInfoItem> Perks = new List<PerkInfoItem>(); }
public class Conditions
{
    public Dictionary<string, float> PerkVariables = new Dictionary<string, float>();
    public Dictionary<string, string> PerkStringVariables = new Dictionary<string, string>();
}
public class Model
{
    public readonly string Name;
    public ModelParameters Parameters = new ModelParameters();
    public Conditions Conditions = new Conditions();
    public int Modifier;
    public Model(string name) { Name = name; }
    public Conditions EBABHGHPLFK() => Conditions;
    public bool HasTransientPerkFlag(string name) => false;
    public Action CopyFormModifiersFrom(Model source)
    {
        int previous = Modifier;
        Modifier = source.Modifier;
        return () => Modifier = previous;
    }
}
class PerkModelStruct
{
    Model model;
    readonly List<InfoPerk> effects = new List<InfoPerk>();
    readonly List<PerkData> data = new List<PerkData>();
    public Model get_Model() => model;
    public void set_Model(Model value) { model = value; }
    public List<InfoPerk> HIPOGANEPMI() => effects;
    public List<PerkData> ANPCFJGEJPO() => data;
}
class PerkEvent { public enum KNKIIEPDCPN { EVENT_MOD_EXPIRES } }
class PerkTrigger { }
public abstract class PerkCondition
{
    protected enum NHDGLPNNNLH { CONDITION_MOD_EXISTS }
    protected void set_Type(NHDGLPNNNLH type) { }
    public virtual void Parse(XmlNode node) { }
    protected Model EPCPGEPPHLO(Model model) => model;
    public abstract bool IsEqual(Model model, List<string> names);
}
static class FixtureExtensions
{
    public static string CIPOICEEIBK(this XmlAttribute value, string fallback) => value?.Value ?? fallback;
    public static void AddIfNotExist<T>(this List<T> list, T value)
    {
        if (!list.Contains(value)) list.Add(value);
    }
}

class PerksStage
{
    /* STAGE_METHODS */

    public readonly List<PerkModelStruct> MPJMCCGKEOD = new List<PerkModelStruct>();
    public readonly List<ActionPerk> JLAKGOEOHMN = new List<ActionPerk>();
    public static readonly Dictionary<string, List<ActionPerk>> PNAALKAHAKG = new Dictionary<string, List<ActionPerk>>();
    readonly Dictionary<string, object> map = new Dictionary<string, object> { ["ModExpires"] = null };
    public readonly List<(Model Target, string Name, string Scope, object Parent)> Events = new List<(Model, string, string, object)>();
    public Action<Model> OnExpiry;
    public Dictionary<string, object> OFKIKABKDFD() => map;
    public void OPACOCIKEOL(PerkModelStruct registration, PerkInfoItem definition)
    {
        registration.ANPCFJGEJPO().Add(new PerkData(definition));
    }
    public bool JALOHCICLGN(Model target, PerkEvent.KNKIIEPDCPN type, bool include)
    {
        if (type != PerkEvent.KNKIIEPDCPN.EVENT_MOD_EXPIRES) throw new Exception("Unexpected event.");
        Events.Add((target, (string)map["ModExpires"], (string)map["Namespace"], map["ParentPerk"]));
        OnExpiry?.Invoke(target);
        return true;
    }
    public static void ANPAFFMJMNG(string name) { throw new Exception("Unexpected perk-use mutation."); }
}

class Fight
{
    public static Fight Current;
    readonly PerksStage stage;
    public Fight(PerksStage value) { stage = value; }
    public static Fight GetCurrentFight() => Current;
    public PerksStage IEEGPNLEKHH() => stage;
}

class InfoPerk
{
    public PerkData DCMHONAFOGI;
    readonly List<PerksStage.ActionPerk> NIDKKJFBNHO = new List<PerksStage.ActionPerk>();
    readonly List<PerksStage.ActionPerk> NBFBBDHELEJ = new List<PerksStage.ActionPerk>();
    readonly List<string> PCOPAMLECKI = new List<string>();
    readonly List<string> IEDBEDCKAIE = new List<string>();
    public int Starts, Ends;

    /* INFO_METHODS */

    void ALBIODLFMAK(PerksStage.ActionPerk action, bool start) { if (start) Starts++; else Ends++; }
    public Action TransferAttributeEffect(PerksStage.ActionPerk action, Model old, Model next)
    {
        throw new InvalidOperationException("Injected later attribute-transfer failure.");
    }
    public Action TransferHealthEffect(PerksStage.ActionPerk action, Model old, Model next) => throw Unexpected();
    static Exception Unexpected() => new Exception("An unrelated native effect handler executed.");
    void MBKLEKPDGOA(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void IEDBKHEFKDE(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void NMIGELMNBDF(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void PCCAPNKPOKB(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void APMFPHOALEO(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void IMJCCNPMHKC(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void BFBGNIICAHE(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void NPNJDBJABMG(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void KCELDPMGNMI(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void FHDDBMFJBJJ(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void EAFKPBMOMKI(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void PHHLFMLOPEK(PerksStage.ActionPerk action, bool remove) { throw Unexpected(); }
    void LICINJMMICM(PerksStage.ActionPerk action) { throw Unexpected(); }
    void LHNCAIDDJIJ(PerksStage.ActionPerk action) { throw Unexpected(); }
    void GKLCDJLBBAM(PerksStage.ActionPerk action) { throw Unexpected(); }
    void KGFBIAOGHFF(PerksStage.ActionPerk action) { throw Unexpected(); }
    void DLMEDFNIEHI(PerksStage.ActionPerk action) { throw Unexpected(); }
    void AGPDKNAEDPB(PerksStage.ActionPerk action) { throw Unexpected(); }
    void HNAIFDHOMPL(PerksStage.ActionPerk action) { throw Unexpected(); }
    void CPKHOBHFJDN(PerksStage.ActionPerk action) { throw Unexpected(); }
    void DGNKIJEICCJ(PerksStage.ActionPerk action) { throw Unexpected(); }
    void OLEBPFBJCII(PerksStage.ActionPerk action) { throw Unexpected(); }
    void MFKFMPAPHDG(PerksStage.ActionPerk action) { throw Unexpected(); }
    void LIPMLGCPAJG(PerksStage.ActionPerk action) { throw Unexpected(); }
    void JFNHLKEEJNC(PerksStage.ActionPerk action) { throw Unexpected(); }
    void NEKDJLPGMAH(PerksStage.ActionPerk action) { throw Unexpected(); }
    void DHPEJIFPLCF(PerksStage.ActionPerk action) { throw Unexpected(); }
    void DDOGCEKKDMK(PerksStage.ActionPerk action) { throw Unexpected(); }
}

static class ValidatePerkFlagTransfer
{
    static int checks;
    static void Check(bool condition, string description)
    {
        checks++;
        if (!condition) throw new Exception(description);
    }
    static XmlNode Node(string xml)
    {
        var document = new XmlDocument(); document.LoadXml(xml); return document.DocumentElement;
    }
    static PerksStage Stage()
    {
        PerksStage.EHFKNCOOCAA();
        var stage = new PerksStage();
        Fight.Current = new Fight(stage);
        return stage;
    }
    static InfoPerk Register(PerksStage stage, Model owner)
    {
        var definition = new PerkInfoItem();
        owner.Parameters.Perks.Add(definition);
        var registration = stage.PrepareModelRegistration(owner);
        var perk = new InfoPerk { DCMHONAFOGI = new PerkData(definition) };
        registration.HIPOGANEPMI().Add(perk);
        stage.MPJMCCGKEOD.Add(registration);
        return perk;
    }
    static PerksStage.ActionPerk Start(InfoPerk perk, Model target, Model source,
        string name = "BleedingCycle", int frames = 6, bool variable = false, string value = "42")
    {
        PerkAction definition = variable ? new PerkActionVariable() : new PerkActionFlag();
        definition.Definition = perk.DCMHONAFOGI.MBDDKGIOOGD;
        string element = variable ? "SetModVariable" : "ModFlag";
        definition.Parse(Node("<" + element + " Name='" + name + "' Namespace='fixture' Frames='" + frames + "' Value='" + value + "'/>"));
        var pending = new PerksStage.ActionPerk {
            KJDFJPBIGJC = target, BIKLKJMNGKP = source, AMKJNPOCODK = definition,
            FLNLMIHEDCI = definition.BFJEFNHKPJI().IBCPKBBAFNH().ToInt()
        };
        perk.MNLNLKOJPHO().Add(pending);
        perk.Run();
        var action = perk.HIPOGANEPMI().Last();
        Check(action != pending && action.AMKJNPOCODK == definition, "Native modifier start makes one action copy.");
        Check(perk.MNLNLKOJPHO().Count == 0 && action.KGNDJOLBBJF == 0, "Native start drains queued work without advancing time.");
        Check(PerksStage.AFAGHKFHHIF(name, "fixture") == action, "Native namespace registration indexes the active copy.");
        return action;
    }
    static bool Exists(Model model, InfoPerk perk, string name, bool namespaced)
    {
        var condition = new PerkConditionModExists();
        condition.Parse(Node("<ModExists Name='" + name + "'" + (namespaced ? " Namespace='fixture'" : "") + "/>"));
        return condition.IsEqual(model, perk.BFKDLIMHGFA());
    }
    static int Lifetime(bool transfer, bool variable = false, string value = "42")
    {
        var stage = Stage();
        var old = new Model("old"); var next = new Model("next"); var final = new Model("final");
        var perk = Register(stage, old);
        var action = Start(perk, old, old, variable ? "EnemyMagic" : "BleedingCycle", 6, variable, value);
        string name = action.AMKJNPOCODK.get_Name();
        if (variable)
        {
            Check(value == "42" ? old.Conditions.PerkVariables[name] == 42 : old.Conditions.PerkStringVariables[name] == value,
                "Native variable handler stores the evaluated numeric/string value.");
        }
        Check(Exists(old, perk, name, false) && Exists(old, perk, name, true), "Active modifier is visible to native ModExists lookups.");
        perk.Render(); perk.Render();
        int elapsed = action.KGNDJOLBBJF, reads = action.AMKJNPOCODK.BFJEFNHKPJI().Reads;
        var names = perk.BFKDLIMHGFA(); var active = perk.HIPOGANEPMI(); var scope = PerksStage.DOAECFNPKIO("fixture");
        Model current = old;
        if (transfer)
        {
            next.Parameters.Perks.Add(perk.DCMHONAFOGI.MBDDKGIOOGD);
            stage.TransferFormEffects(old, next);
            stage.ReplaceFormRegistration(old, next);
            stage.RequireFormReferencesTransferred(new HashSet<Model> { old });
            Check(stage.MPJMCCGKEOD[0].get_Model() == next && stage.MPJMCCGKEOD[0].HIPOGANEPMI().Single() == perk,
                "Replacement retains the same effect container.");
            Check(active == perk.HIPOGANEPMI() && names == perk.BFKDLIMHGFA() && scope == PerksStage.DOAECFNPKIO("fixture"),
                "Action, name and namespace collections retain their identity.");
            Check(active.Single() == action && scope.Single() == action, "Form transfer preserves the action identity in every live index.");
            Check(action.KJDFJPBIGJC == next && action.BIKLKJMNGKP == next, "Both self references follow the new body.");
            Check(action.KGNDJOLBBJF == elapsed && action.FLNLMIHEDCI == 6 && action.AMKJNPOCODK.BFJEFNHKPJI().Reads == reads,
                "Transfer preserves the exact timer without evaluating its expression.");
            Check(perk.Starts == 1 && perk.Ends == 0 && stage.Events.Count == 0, "Transfer never replays a start or sends early expiry.");
            if (variable) Check(((PerkActionVariable)action.AMKJNPOCODK).OEAKCOHMIHH().Reads == 1, "Variable expression is not evaluated again.");
            Check(Exists(next, perk, name, false) && Exists(next, perk, name, true), "Transferred state remains discoverable.");
            // Repeated changes are still one logical effect with one expiry.
            final.Parameters.Perks.Add(perk.DCMHONAFOGI.MBDDKGIOOGD);
            stage.TransferFormEffects(next, final);
            stage.ReplaceFormRegistration(next, final);
            stage.RequireFormReferencesTransferred(new HashSet<Model> { old, next });
            Check(action.KGNDJOLBBJF == elapsed && action.KJDFJPBIGJC == final, "Repeated replacement does not restart lifetime.");
            current = final;
        }
        stage.OnExpiry = actor => Check(actor == current && !Exists(actor, perk, name, false) && !Exists(actor, perk, name, true),
            "Native expiry removes name and namespace before delivering its event.");
        int remaining = 0;
        while (perk.HIPOGANEPMI().Count > 0 && remaining < 20) { perk.Render(); remaining++; }
        Check(remaining < 20 && perk.Ends == 1 && stage.Events.Count == 1, "Native Render expires exactly once.");
        var observed = stage.Events.Single();
        Check(observed.Target == current && observed.Name == name && observed.Scope == "fixture" && observed.Parent == perk.DCMHONAFOGI.MBDDKGIOOGD,
            "Expiry reports the current target and original native name, namespace and parent.");
        Check(stage.JLAKGOEOHMN.Single() == action && PerksStage.AFAGHKFHHIF(name, "fixture") == null,
            "Expired action keeps history identity and leaves the live namespace.");
        perk.Render(); perk.ClearActions(true);
        Check(perk.Ends == 1 && stage.Events.Count == 1, "Later rendering and teardown do not expire the action again.");
        return remaining;
    }
    static void ReferencesAndRollback(bool variable)
    {
        var stage = Stage(); var old = new Model("old") { Modifier = 7 }; var next = new Model("next"); var other = new Model("other");
        var owned = Register(stage, old); var external = Register(stage, other);
        var both = Start(owned, old, old, "both", variable: variable);
        var source = Start(owned, other, old, "source", variable: variable);
        var target = Start(external, old, other, "target", variable: variable);
        var untouched = Start(external, other, other, "unrelated", variable: variable);
        both.KGNDJOLBBJF = 3; both.PLNNKKBPDJK = true;
        // Alias the same record across active, queued, history and namespace indexes.
        owned.HIPOGANEPMI().Add(both); owned.MNLNLKOJPHO().Add(both); stage.JLAKGOEOHMN.Add(both);
        var undoQueued = stage.RebindQueuedFormActions(old, next);
        Check(both.KJDFJPBIGJC == old, "Queued transfer leaves active aliases for the effect-specific path.");
        var undo = stage.TransferFormEffects(old, next);
        stage.RequireFormReferencesTransferred(new HashSet<Model> { old });
        Check(both.KJDFJPBIGJC == next && both.BIKLKJMNGKP == next && source.KJDFJPBIGJC == other && source.BIKLKJMNGKP == next,
            "Both and source-only references move independently.");
        Check(target.KJDFJPBIGJC == next && target.BIKLKJMNGKP == other && untouched.KJDFJPBIGJC == other && untouched.BIKLKJMNGKP == other,
            "Opponent-owned target effects move while unrelated effects remain untouched.");
        Check(both.KGNDJOLBBJF == 3 && both.PLNNKKBPDJK && owned.HIPOGANEPMI().Count == 3 && owned.MNLNLKOJPHO().Single() == both,
            "Duplicate aliases, queued membership and pending removal are preserved without replay.");
        var originalRegistration = stage.MPJMCCGKEOD[0];
        next.Parameters.Perks.Add(owned.DCMHONAFOGI.MBDDKGIOOGD);
        var undoRegistration = stage.ReplaceFormRegistration(old, next);
        undoRegistration(); undo(); undoQueued();
        Check(stage.MPJMCCGKEOD[0] == originalRegistration && both.KJDFJPBIGJC == old && both.BIKLKJMNGKP == old && next.Modifier == 0,
            "Rollback restores participant registration, original effect references and earlier model changes.");
        Check(source.BIKLKJMNGKP == old && target.KJDFJPBIGJC == old && PerksStage.AFAGHKFHHIF("both", "fixture") == both,
            "Rollback restores all attribution while retaining namespace aliases.");
        owned.HIPOGANEPMI().RemoveAt(owned.HIPOGANEPMI().Count - 1);
        owned.MNLNLKOJPHO().Clear(); stage.JLAKGOEOHMN.Clear();
        var failing = new PerksStage.ActionPerk { KJDFJPBIGJC = old, AMKJNPOCODK = new PerkActionSetAttributes() };
        external.HIPOGANEPMI().Add(failing);
        bool failed = false;
        try { stage.TransferFormEffects(old, next); }
        catch (InvalidOperationException error) { failed = error.Message.Contains("later attribute-transfer"); }
        Check(failed && next.Modifier == 0 && both.KJDFJPBIGJC == old && source.BIKLKJMNGKP == old && target.KJDFJPBIGJC == old,
            "A later effect failure reverses already-transferred records and model state.");
        Check(both.KGNDJOLBBJF == 3 && both.FLNLMIHEDCI == 6 && both.PLNNKKBPDJK && stage.Events.Count == 0,
            "Failed transfer leaves timer, pending removal and event history unchanged.");
        if (variable)
        {
            Check(((PerkActionVariable)both.AMKJNPOCODK).OEAKCOHMIHH().Reads == 1 &&
                ((PerkActionVariable)target.AMKJNPOCODK).OEAKCOHMIHH().Reads == 1 && old.Conditions.PerkVariables["both"] == 42,
                "Reference transfer and rollback do not replay variable expressions or overwrite their stored value.");
        }
        external.HIPOGANEPMI().Remove(failing);
        owned.ClearActions();
        Check(stage.Events.Single().Target == old && stage.Events.Single().Name == "both",
            "After rollback native clearing expires the flagged record on the original fighter.");
    }
    static void IndefiniteAndPendingRemoval(bool variable)
    {
        foreach (int frames in new[] { 0, 20 })
        {
            var stage = Stage(); var old = new Model("old"); var next = new Model("next"); var perk = Register(stage, old);
            var action = Start(perk, old, old, "held", frames, variable);
            for (int i = 0; i < 3; i++) perk.Render();
            int elapsed = action.KGNDJOLBBJF;
            action.PLNNKKBPDJK = true;
            stage.TransferFormEffects(old, next);
            stage.RequireFormReferencesTransferred(new HashSet<Model> { old });
            Check(action.PLNNKKBPDJK && action.KGNDJOLBBJF == elapsed && stage.Events.Count == 0,
                "Pending removal survives transfer for indefinite and timed modifiers.");
            perk.ClearActions(); perk.ClearActions(true);
            Check(perk.HIPOGANEPMI().Count == 0 && stage.Events.Count == 1 && stage.Events[0].Target == next,
                "Native clearing ends indefinite/timed modifiers exactly once on the replacement.");
        }
    }
    static void UnresolvedReferencesStayGuarded()
    {
        foreach (ActionType kind in new[] { ActionType.ACTION_STEAL_MAGIC, ActionType.ACTION_NONE })
        {
            var stage = Stage(); var old = new Model("old"); var next = new Model("next"); var perk = Register(stage, old);
            var unresolved = new PerksStage.ActionPerk { KJDFJPBIGJC = old, AMKJNPOCODK = new UnsupportedModifier(kind) };
            perk.HIPOGANEPMI().Add(unresolved);
            var undo = stage.TransferFormEffects(old, next);
            bool rejected = false;
            try { stage.RequireFormReferencesTransferred(new HashSet<Model> { old }); }
            catch (InvalidOperationException error) { rejected = error.Message.Contains(kind.ToString()); }
            Check(rejected && unresolved.KJDFJPBIGJC == old, "Unimplemented applied effects still reject retirement.");
            undo();
        }
        foreach (bool variable in new[] { false, true })
        {
            var stage = Stage(); var old = new Model("old"); var next = new Model("next"); var perk = Register(stage, old);
            var orphan = Start(perk, old, old, "namespace-only", variable: variable);
            perk.HIPOGANEPMI().Remove(orphan);
            var undo = stage.TransferFormEffects(old, next);
            bool rejected = false;
            try { stage.RequireFormReferencesTransferred(new HashSet<Model> { old }); }
            catch (InvalidOperationException) { rejected = true; }
            Check(rejected && orphan.KJDFJPBIGJC == old, "Namespace-only state is not assumed safely expired or transferable.");
            undo();
        }
    }
    public static void Main()
    {
        int baseline = Lifetime(false);
        Check(Lifetime(true) == baseline, "Transferred flags expire on the same native step as uninterrupted flags.");
        foreach (bool variable in new[] { false, true })
        {
            ReferencesAndRollback(variable);
            IndefiniteAndPendingRemoval(variable);
        }
        foreach (string value in new[] { "42", "MAGIC_FIREBALL" })
        {
            int variableBaseline = Lifetime(false, true, value);
            Check(Lifetime(true, true, value) == variableBaseline, "Variable modifiers preserve native expiry timing.");
        }
        UnresolvedReferencesStayGuarded();
        Console.WriteLine("PASS: " + checks + " production flag/variable lifecycle and form-transfer checks. " +
            "Actual action copies, Run/start/Render/clear/expiry, ModExists, namespace indexes and form registration/transfer/gate; " +
            "models, expression evaluation and final event routing controlled. Variable dictionary handover is covered separately; no native fight playtest.");
    }
}
