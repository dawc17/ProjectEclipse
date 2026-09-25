$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$gameSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/GameUtils.cs')
$parameterSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelParameters.cs')
$fightSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$modelSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Model.cs')
$initializer=[regex]::Match($gameSource,'(?ms)^    internal static ModelParameters InitializeFormParameters\(.*?^    \}').Value
$initializer += [regex]::Match($gameSource,'(?ms)^[\t ]*private static ModelParameters InitializeCombatParameters\(.*?^\t\}').Value
$initializer += [regex]::Match($gameSource,'(?ms)^\tprivate static List<int> LoadMoves\(.*?^\t\}').Value
$parameters=[regex]::Match($parameterSource,'(?ms)^\tpublic ObscuredInt OJLKDEHMIAC\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tpublic ObscuredFloat KKMCHCNOHMB\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tpublic bool AGICDDJBPLB\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tpublic void GFNCMLFKBGP\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tpublic void BCLGFKDDNKH\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tpublic void PPFDLIBLNDG\(.*?^\t\}').Value
$parameters += [regex]::Match($parameterSource,'(?ms)^\tprivate string OKALHAKMOLI\(.*?^\t\}').Value
$request=[regex]::Match($fightSource,'(?ms)^    internal bool TryQueueCharacterForm\(.*?^    \}').Value
$roles=[regex]::Match($modelSource,'(?ms)^\tpublic bool FGKAFKFBFEM\(.*?^\t\}').Value
$roles += [regex]::Match($modelSource,'(?ms)^\tpublic bool BCKKCJONNHG\(.*?^\t\}').Value
$roles += [regex]::Match($modelSource,'(?ms)^\tpublic bool EPCNJLEHJCB\(.*?^\t\}').Value
if(!$initializer -or !$parameters -or !$request -or !$roles){throw 'Form initialization extraction failed.'}
$fixture=Join-Path $root ('Temp/FormInitialization-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
using System.Linq;
using ObscuredInt = System.Int32;
using ObscuredFloat = System.Single;

// Production health/path/initialization/request methods are inserted below.
// XML projection, attributes, item rules, native body construction and queuing
// are controlled services. This fixture does not execute Unity or form commit.
class ItemInfo { public string ModelFileName; }
class Tactic { public string Name; }
struct DefinitionId { }
class ItemRule { public ItemInfo Weapon; }
class AnimationData { public static int Count = 5; public static int DJDLCMCLOJN() => Count; }
class ListSF
{
    public static readonly ListSF Instance = new ListSF();
    public readonly ItemInfo DefaultBody = new ItemInfo { ModelFileName = "default_body" };
    public int Lookups;
    public bool Fail;
    public static ListSF GetItems() => Instance;
    public ItemInfo GetItemByName(string name)
    {
        Lookups++;
        if (Fail) throw new InvalidOperationException("default body unavailable");
        if (name != "default_body") throw new InvalidOperationException("unexpected body lookup");
        return DefaultBody;
    }
}
class ModelParameters
{
    public bool IsPlayer, UserControlled, AiControlled, EAJHPCJJCDI, ABLMGLAKJBL, LNHMCKNCGDP;
    public int RoundsWon, DEGCGHDAMDA = -1;
    public float CIDCNCDFONA, _CurrentLife;
    public Tactic HBFMBOHLKPJ;
    public ItemInfo Skeleton, Weapon, Armor, Helm;
    public string EclipseBodyModel;
    public string[] EclipseSkinModels = Array.Empty<string>();
    public List<int> IAIHFLGBIPB = new List<int>();
    public List<string> ModelDocuments = new List<string>();
    public List<ItemInfo> HEKILHEHMMH = new List<ItemInfo>();
    public readonly List<bool> ItemPasses = new List<bool>();
    public int ItemRound, AttributePasses;
    public List<string> AttributePaths;
    public bool FailAttributes;
    PARAMETER_METHODS
    public void NOBKKLBJFIL()
    {
        if (FailAttributes) throw new InvalidOperationException("attribute preparation failed");
        AttributePasses++;
        AttributePaths = new List<string>(ModelDocuments);
    }
    public void KMPACCIOOLE(List<ItemRule> rules, bool second, int round)
    {
        ItemPasses.Add(second);
        ItemRound = round;
        foreach (var rule in rules) Weapon = rule.Weapon;
    }
}
class GameUtils
{
    static string GetDefaultSkeleton() => "default_body";
    INITIALIZER_METHODS
}
class Model
{
    public ModelParameters Parameters;
    public float KKMCHCNOHMB() => Parameters.KKMCHCNOHMB();
    ROLE_METHODS
}
class ModRuntime
{
    public static ModelParameters Destination;
    public static int Calls;
    public static bool RequestedPlayer;
    public static ModelParameters BuildFormParameters(DefinitionId character, bool player)
    {
        Calls++;
        RequestedPlayer = player;
        return Destination;
    }
}
class Rules
{
    public ModelParameters Current;
    public readonly List<ItemRule> Player = new List<ItemRule> {
        new ItemRule { Weapon = new ItemInfo { ModelFileName = "player_rule_weapon" } } };
    public readonly List<ItemRule> Enemy = new List<ItemRule> {
        new ItemRule { Weapon = new ItemInfo { ModelFileName = "enemy_rule_weapon" } } };
    public List<ItemRule> Selected;
    public List<ItemRule> GetPlayerItemRules() => Player;
    public List<ItemRule> GetEnemyItemRules() => Enemy;
    public void PrepareItemRules(List<ItemRule> rules)
    {
        var p = ModRuntime.Destination;
        if (p.CIDCNCDFONA <= 0 || p.KKMCHCNOHMB() <= 0 || p.IAIHFLGBIPB.Count != AnimationData.Count)
            throw new InvalidOperationException("item rules reached before native health/move initialization");
        if (p.IsPlayer != Current.IsPlayer || p.UserControlled != Current.UserControlled || p.AiControlled != Current.AiControlled)
            throw new InvalidOperationException("item rules reached before participant ownership restoration");
        Selected = rules;
    }
}
class Round { public bool processing = true; public int round = 3; }
class Fight
{
    public Model _playerModel, CKNCPOABFBO;
    public Round round = new Round();
    public bool _modelTransitionsClosed, _eclipseFightEndDispatched;
    public Dictionary<Model, object> _modelTransitions = new Dictionary<Model, object>();
    public Rules _rulesInspector = new Rules();
    public bool AcceptQueue = true;
    public int QueueCalls;
    public PreparedFormModel Queued;
    public sealed class PreparedFormModel : IDisposable
    {
        public static int Constructed;
        public readonly Model Model;
        public int Disposals;
        public PreparedFormModel(ModelParameters p)
        {
            if (p.CIDCNCDFONA <= 0 || p.KKMCHCNOHMB() <= 0)
                throw new InvalidOperationException("native construction received an empty health pool");
            if (!p.ItemPasses.SequenceEqual(new[] { false, true }) || p.AttributePasses < 2 ||
                !p.AttributePaths.SequenceEqual(p.ModelDocuments) ||
                !p.ModelDocuments.Contains(p.Weapon.ModelFileName + ".xml"))
                throw new InvalidOperationException("native construction received stale rule equipment/attributes");
            Constructed++;
            Model = new Model { Parameters = p };
        }
        public void Dispose() { Disposals++; }
    }
    bool QueuePreparedFighterForm(Model expected, PreparedFormModel prepared, Action<Exception> complete)
    {
        QueueCalls++;
        Queued = prepared;
        return AcceptQueue;
    }
    REQUEST_METHOD
}
class ValidateFormInitialization
{
    static int checks;
    static void Check(bool value, string message)
    {
        checks++;
        if (!value) throw new Exception(message);
    }
    static ModelParameters Current(bool player, bool controlled, bool ai) => new ModelParameters {
        IsPlayer = player, UserControlled = controlled, AiControlled = ai,
        CIDCNCDFONA = 200, _CurrentLife = 75, DEGCGHDAMDA = 200, RoundsWon = 2,
        EAJHPCJJCDI = true, ABLMGLAKJBL = false, HBFMBOHLKPJ = new Tactic { Name = "old_tactic" },
        IAIHFLGBIPB = new List<int> { 9 }, ModelDocuments = new List<string> { "live_body.xml" } };
    static ModelParameters Destination(int life) => new ModelParameters {
        DEGCGHDAMDA = life, CIDCNCDFONA = 0, _CurrentLife = 0, RoundsWon = 8,
        EAJHPCJJCDI = true, HBFMBOHLKPJ = new Tactic { Name = "new_tactic" },
        Weapon = new ItemInfo { ModelFileName = "destination_weapon" },
        IAIHFLGBIPB = new List<int> { 87 }, ModelDocuments = new List<string> { "stale.xml" } };
    static string State(ModelParameters p) => string.Join("|", new object[] { p.IsPlayer, p.UserControlled, p.AiControlled,
        p.CIDCNCDFONA, p.KKMCHCNOHMB(), p.OJLKDEHMIAC(), p.RoundsWon, p.EAJHPCJJCDI, p.ABLMGLAKJBL,
        p.HBFMBOHLKPJ?.Name, string.Join(",", p.IAIHFLGBIPB), string.Join(",", p.ModelDocuments) });
    static void Requests()
    {
        foreach (bool player in new[] { false, true })
        foreach (int requestedRound in new[] { 0, 3 })
        {
            var current = Current(player, player, !player);
            string before = State(current);
            var destination = Destination(-1);
            var tactic = destination.HBFMBOHLKPJ;
            var expected = new Model { Parameters = current };
            var fight = new Fight();
            if (player) fight._playerModel = expected; else fight.CKNCPOABFBO = expected;
            fight.round.round = requestedRound;
            fight._rulesInspector.Current = current;
            ModRuntime.Destination = destination;
            int callbacks = 0;
            bool accepted = fight.TryQueueCharacterForm(expected, default, error => callbacks++, out var failure);
            Check(accepted && failure == "", "healthy native form request must queue: " + failure);
            Check(ModRuntime.RequestedPlayer == player && fight.QueueCalls == 1 && callbacks == 0,
                "correct side projected and completion deferred to queue");
            Check(destination.ItemRound == Math.Max(1, requestedRound) &&
                fight._rulesInspector.Selected == (player ? fight._rulesInspector.Player : fight._rulesInspector.Enemy),
                "current side and one-based round select item rules");
            Check(destination.CIDCNCDFONA == 1 && destination.KKMCHCNOHMB() == 1 && destination.HBFMBOHLKPJ == tactic,
                "initialized fallback health and destination tactic reach body preparation");
            Check(State(current) == before && fight.Queued.Disposals == 0,
                "queue acceptance preserves live participant and transfers preparation ownership");

            destination = Destination(40);
            ModRuntime.Destination = destination;
            fight.AcceptQueue = false;
            accepted = fight.TryQueueCharacterForm(expected, default, error => callbacks++, out failure);
            Check(!accepted && failure.Contains("became unavailable") && fight.Queued.Disposals == 1 && callbacks == 0,
                "late queue rejection releases only prepared body");
            Check(State(current) == before, "late rejection leaves live health/history intact");

            int constructed = Fight.PreparedFormModel.Constructed, queued = fight.QueueCalls;
            destination = Destination(40); destination.FailAttributes = true; ModRuntime.Destination = destination;
            accepted = fight.TryQueueCharacterForm(expected, default, error => callbacks++, out failure);
            Check(!accepted && failure == "attribute preparation failed" && Fight.PreparedFormModel.Constructed == constructed &&
                fight.QueueCalls == queued && State(current) == before,
                "initialization failure cannot construct/queue a body or modify current fighter");
            fight._modelTransitions.Add(expected, new object());
            int projected = ModRuntime.Calls;
            accepted = fight.TryQueueCharacterForm(expected, default, error => callbacks++, out failure);
            Check(!accepted && ModRuntime.Calls == projected && callbacks == 0,
                "duplicate request rejects before native initialization");
        }
    }
    static void NativeInitialization()
    {
        foreach (bool player in new[] { false, true })
        foreach (bool controlled in new[] { false, true })
        foreach (bool ai in new[] { false, true })
        foreach (int life in new[] { -1, 0, 40 })
        {
            var current = Current(player, controlled, ai);
            string before = State(current);
            var liveMoves = current.IAIHFLGBIPB;
            var p = Destination(life);
            p.IsPlayer = !player; p.UserControlled = !controlled; p.AiControlled = !ai;
            var tactic = p.HBFMBOHLKPJ; var weapon = p.Weapon;
            var staleMoves = p.IAIHFLGBIPB;
            Check(GameUtils.InitializeFormParameters(p, current) == p, "initializer retains owned parameter identity");
            float maximum = life > 0 ? life : 1;
            Check(p.CIDCNCDFONA == maximum && p.KKMCHCNOHMB() == maximum, "native Life or one-bar fallback fills current health");
            var model = new Model { Parameters = p };
            Check(model.EPCNJLEHJCB() == player && model.BCKKCJONNHG() == controlled && model.FGKAFKFBFEM() == ai,
                "native side/input/AI accessors preserve all participant role combinations");
            Check(p.HBFMBOHLKPJ == tactic && p.Weapon == weapon, "destination tactic/equipment identities retained");
            Check(p.Skeleton == ListSF.Instance.DefaultBody &&
                p.ModelDocuments.SequenceEqual(new[] { "default_body.xml", "destination_weapon.xml" }),
                "native path assembly receives default body and destination equipment");
            Check(!p.EAJHPCJJCDI && p.ABLMGLAKJBL && p.RoundsWon == 0, "canonical preparation flags and wins initialized");
            Check(p.IAIHFLGBIPB.SequenceEqual(Enumerable.Range(0, AnimationData.Count)) && p.IAIHFLGBIPB != staleMoves &&
                staleMoves.SequenceEqual(new[] { 87 }), "native move indices replace detached stale list");
            Check(State(current) == before && current.IAIHFLGBIPB == liveMoves, "current participant is untouched");
            p.GFNCMLFKBGP(maximum * .375f);
            Check(p.KKMCHCNOHMB() == maximum * .375f, "initialized pool permits boundary health fraction assignment");
        }
    }
    static void IsolationAndFailure()
    {
        var current = Current(true, true, false);
        var p = Destination(40);
        var body = new ItemInfo { ModelFileName = "authored_skeleton" };
        p.Skeleton = body;
        p.EclipseBodyModel = "sample:models/body.xml";
        p.EclipseSkinModels = new[] { "sample:models/skin.xml" };
        int lookups = ListSF.Instance.Lookups;
        GameUtils.InitializeFormParameters(p, current);
        Check(ListSF.Instance.Lookups == lookups && p.Skeleton == body &&
            p.ModelDocuments.SequenceEqual(new[] { "sample:models/body.xml", "destination_weapon.xml", "sample:models/skin.xml" }),
            "authored body/skin bypass fallback and retain native composition priority");
        var another = Destination(40);
        GameUtils.InitializeFormParameters(another, current);
        p.IAIHFLGBIPB[0] = 99;
        Check(another.IAIHFLGBIPB[0] == 0 && current.IAIHFLGBIPB[0] == 9,
            "separate prepared forms and active fighter never share move-index list");
        string before = State(current);
        lookups = ListSF.Instance.Lookups;
        foreach (var pair in new[] {
            (Prepared: (ModelParameters)null, Current: current),
            (Prepared: p, Current: (ModelParameters)null),
            (Prepared: current, Current: current) })
        {
            bool rejected = false;
            try { GameUtils.InitializeFormParameters(pair.Prepared, pair.Current); }
            catch (ArgumentException) { rejected = true; }
            Check(rejected && ListSF.Instance.Lookups == lookups && State(current) == before,
                "null/shared initialization rejects before native mutation");
        }
        ListSF.Instance.Fail = true;
        bool failed = false;
        try { GameUtils.InitializeFormParameters(Destination(-1), current); }
        catch (InvalidOperationException error) { failed = error.Message == "default body unavailable"; }
        finally { ListSF.Instance.Fail = false; }
        Check(failed && State(current) == before, "native default-body failure preserves current participant");
    }
    public static void Main()
    {
        Requests();
        NativeInitialization();
        IsolationAndFailure();
        Console.WriteLine("PASS: " + checks + " production form initialization/request checks. Native initializer, health fill/clamp, move indices, model-path composition and side/control/AI accessors execute. Projection, item rules, attributes, native body construction and queue are controlled; no Unity gameplay.");
    }
}
'@
$code=$code.Replace('INITIALIZER_METHODS',$initializer).Replace('PARAMETER_METHODS',$parameters).Replace('ROLE_METHODS',$roles).Replace('REQUEST_METHOD',$request)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form initialization checks failed.'}
