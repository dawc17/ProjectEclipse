$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$fightSource = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$modIdPath = Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs'
$definitionIdPath = Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/DefinitionId.cs'

# Compile the actual recovered Fight dispatch method with only unrelated fight/runtime systems
# stubbed. This keeps the regression tied to the real one-shot integration seam rather than a
# duplicate implementation in the test.
$dispatch = [regex]::Match($fightSource,
    '(?ms)^\tprivate void DispatchEclipseCombatEvent\([^\r\n]*\).*?^\t\}(?=\r?\n\r?\n\tprivate void StartStance)').Value
if (!$dispatch) { throw 'Could not extract DispatchEclipseCombatEvent from Fight.cs.' }
$damageBefore = $fightSource.IndexOf('float eclipseHealthBefore =')
$damageApply = $fightSource.IndexOf('UpdateLife(EGHPHELLOGO.KJDFJPBIGJC', $damageBefore)
$damageDispatch = $fightSource.IndexOf('DispatchEclipseCombatEvent(ModEffectEvent.DamageReceived', $damageApply)
$damageBookkeeping = $fightSource.IndexOf('KDMDOBOKAIB(EGHPHELLOGO.KJDFJPBIGJC', $damageDispatch)
if ($damageBefore -lt 0 -or $damageApply -le $damageBefore -or $damageDispatch -le $damageApply -or $damageBookkeeping -le $damageDispatch) {
    throw 'Damage callback is not ordered after health application and before hit bookkeeping.'
}

$testRoot = Join-Path $root 'Temp/ModFightBeginRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null

$code = @'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

namespace UnityEngine
{
    public static class Debug
    {
        public static readonly List<string> Warnings = new List<string>();
        public static void LogWarning(object value) { Warnings.Add(value == null ? string.Empty : value.ToString()); }
    }
}

namespace Eclipse.Modding
{
    public enum ModEffectEvent { FightBegin, DamageReceived }
    public sealed class ModDamageEvent {}
    public sealed class ModIncomingHit {}
    public sealed class ModCombatActivityEvent {}
    public sealed class ModBattleRuleInstances {}
    public interface IModFighterOperations
    {
        bool TryChangeHealth(double amount, out string error);
        bool TryAddMagicCharge(double amount, out string error);
    }

    public sealed class EnchantmentDefinition
    {
        public DefinitionId Behavior;
        public bool HasBehavior;
    }

    public sealed class PerkDefinition
    {
        public DefinitionId Behavior;
        public bool HasBehavior;
    }

    public sealed class ModContentCatalog
    {
        private readonly Dictionary<DefinitionId, EnchantmentDefinition> _enchantments =
            new Dictionary<DefinitionId, EnchantmentDefinition>();
        private readonly Dictionary<DefinitionId, PerkDefinition> _perks =
            new Dictionary<DefinitionId, PerkDefinition>();

        public void Add(string id, bool hasBehavior)
        {
            _enchantments[DefinitionId.Parse(id)] = new EnchantmentDefinition { HasBehavior = hasBehavior };
        }

        public bool TryGetEnchantment(DefinitionId id, out EnchantmentDefinition definition)
        {
            return _enchantments.TryGetValue(id, out definition);
        }

        public void AddPerk(string id, bool hasBehavior)
        {
            _perks[DefinitionId.Parse(id)] = new PerkDefinition { HasBehavior = hasBehavior };
        }

        public bool TryGetPerk(DefinitionId id, out PerkDefinition definition)
        {
            return _perks.TryGetValue(id, out definition);
        }
    }

    public sealed class ModScriptSession
    {
        public ModContentCatalog Content { get; } = new ModContentCatalog();
        public bool HasBehaviorHandler(DefinitionId id, ModEffectEvent kind) => true;
    }

    public static class ModRuntime
    {
        public static void DispatchBattleRules(ModBattleRuleInstances instances,string runtimeId,bool player,int round,bool eclipse,string fightId,string result,ModEffectEvent kind,IModFighterOperations fighter) {}
        public sealed class Invocation
        {
            public string Id;
            public ModEffectEvent Event;
            public Dictionary<string, string> Context;
            public XmlNode Node;
        }

        public static ModScriptSession Scripts;
        public static Action OnInvoke;
        public static readonly List<Invocation> Invocations = new List<Invocation>();
        public static readonly HashSet<string> FailIds = new HashSet<string>(StringComparer.Ordinal);
        public static readonly HashSet<string> ThrowIds = new HashSet<string>(StringComparer.Ordinal);

        public static bool TryInvokeSavedEnchantmentFightBegin(XmlNode perkNode,
            IReadOnlyDictionary<string, string> fighterContext, IModFighterOperations fighter, out string error, ModEffectEvent effectEvent = ModEffectEvent.FightBegin)
        {
            string id = perkNode.Attributes?[PerkStruct.EclipseEnchantmentAttribute]?.Value ?? string.Empty;
            var context = new Dictionary<string, string>(StringComparer.Ordinal);
            if (fighterContext != null)
            {
                foreach (KeyValuePair<string, string> pair in fighterContext) context[pair.Key] = pair.Value;
            }
            Invocations.Add(new Invocation
            {
                Id = id,
                Context = context,
            });
            if (ThrowIds.Contains(id)) throw new InvalidOperationException("synthetic host dispatch failure");
            if (FailIds.Contains(id))
            {
                error = "synthetic handler failure";
                return false;
            }
            error = string.Empty;
            return true;
        }

        public static bool TryInvokeSavedPerkFightBegin(XmlNode perkNode,
            IReadOnlyDictionary<string, string> fighterContext, IModFighterOperations fighter, out string error, ModEffectEvent effectEvent = ModEffectEvent.FightBegin)
        {
            string id = perkNode.Attributes?["Name"]?.Value ?? string.Empty;
            var context = new Dictionary<string, string>(StringComparer.Ordinal);
            if (fighterContext != null)
            {
                foreach (KeyValuePair<string, string> pair in fighterContext) context[pair.Key] = pair.Value;
            }
            Invocations.Add(new Invocation { Id = id, Context = context, Event = effectEvent, Node = perkNode });
            OnInvoke?.Invoke();
            error = string.Empty;
            return true;
        }
    }
}

public static class PerkStruct
{
    public const string EclipseEnchantmentAttribute = "EclipseEnchantment";
}

public sealed class PerkInfoItem
{
    public string Name;
}

public sealed class ItemInfo
{
    public List<PerkInfoItem> NHBIJEEKALC = new List<PerkInfoItem>();
    public string Name;
    public string Type;
    public bool GNDLEFFMJDJ;
}

public sealed class UserItem
{
    public XmlNode Node { get; }
    public UserItem(XmlNode node) { Node = node; }
}

public sealed class UserItems
{
    private readonly Dictionary<string, UserItem> _items = new Dictionary<string, UserItem>(StringComparer.Ordinal);
    public void Add(ItemInfo item, UserItem userItem) { _items[item.Name] = userItem; }
    public UserItem CMGOCLGHNLH(ItemInfo item)
    {
        if (item == null) return null;
        UserItem value;
        return _items.TryGetValue(item.Name, out value) ? value : null;
    }
}

public sealed class RosterStub
{
    public bool JPMPIDFGCJL() => false;
    public UserItems UserItems = new UserItems();
    public UserPerks UserPerks = new UserPerks();
    public UserItems KHCNHPCPFII() { return UserItems; }
    public UserPerks JLBDOBLHHAF() { return UserPerks; }
}

public sealed class RosterPerk
{
    public XmlNode Node { get; }
    public RosterPerk(XmlNode node) { Node = node; }
}

public sealed class UserPerks
{
    private readonly Dictionary<string, RosterPerk> _perks = new Dictionary<string, RosterPerk>(StringComparer.Ordinal);
    public void Add(string name, RosterPerk perk) { _perks[name] = perk; }
    public RosterPerk LKIEAGLHNON(string name)
    {
        RosterPerk value;
        return _perks.TryGetValue(name, out value) ? value : null;
    }
}

public static class ListSF
{
    public static readonly RosterStub Roster = new RosterStub();
    public static RosterStub CCDKHLAMKKO() { return Roster; }
}

public sealed class ModelParameters
{
    public bool IsPlayer;
    public readonly List<PerkInfoItem> NHBIJEEKALC = new List<PerkInfoItem>();
    public readonly List<PerkInfoItem> JGCNPHDGHAK = new List<PerkInfoItem>();
    public readonly List<ItemInfo> Items = new List<ItemInfo>();
    public List<ItemInfo> PJNJIJIODHE() { return Items; }
}

public sealed class Model { }

public sealed class RoundStub
{
    public int round;
}

public sealed class FightHarness
{
    private sealed class EclipseFighterOperations : Eclipse.Modding.IModFighterOperations
    {
        public EclipseFighterOperations(FightHarness fight, Model model, ModDamageEvent damageEvent = null, ModIncomingHit incomingHit = null, ModCombatActivityEvent activity = null) { }
        public bool TryChangeHealth(double amount, out string error) { error = string.Empty; return true; }
        public bool TryAddMagicCharge(double amount, out string error) { error = string.Empty; return true; }
    }

    private bool _eclipseFightBeginDispatched;
    private bool _eclipseOpponentDispatching;
    private readonly ModBattleRuleInstances _eclipseBattleRules = new ModBattleRuleInstances();
    private sealed class FightIdentity { public int BCKFACGMOKC; }
    private readonly FightIdentity KGKDKENMAOA = new FightIdentity();
    private readonly Dictionary<(Model,DefinitionId),XmlNode> _eclipseInnateInstances = new Dictionary<(Model,DefinitionId),XmlNode>();
    private string _eclipseFightId = "fixture";
    private string _eclipsePlayerResult = "none";
    private bool _eclipseCombatDispatching;
    private readonly RoundStub round = new RoundStub();
    private readonly ModelParameters NMNCKBPFCCP;
    private readonly Model _playerModel = new Model();

    public FightHarness(ModelParameters player, int roundNumber)
    {
        NMNCKBPFCCP = player;
        round.round = roundNumber;
    }

    public void Dispatch() { DispatchEclipseCombatEvent(); }
    public void Damage() { DispatchEclipseCombatEvent(ModEffectEvent.DamageReceived, new ModDamageEvent()); }

__DISPATCH__
}

public static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static UserItem SavedItem(string xml)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return new UserItem(document.DocumentElement);
    }

    public static int Main()
    {
        var scripts = new Eclipse.Modding.ModScriptSession();
        scripts.Content.Add("example.mod:enchantments/active", true);
        scripts.Content.Add("example.mod:enchantments/failing", true);
        scripts.Content.Add("example.mod:enchantments/throwing", true);
        scripts.Content.Add("example.mod:enchantments/compat", false);
        scripts.Content.AddPerk("example.mod:perks/active_perk", true);
        scripts.Content.AddPerk("example.mod:perks/inactive_perk", true);
        Eclipse.Modding.ModRuntime.Scripts = scripts;
        Eclipse.Modding.ModRuntime.FailIds.Add("example.mod:enchantments/failing");
        Eclipse.Modding.ModRuntime.ThrowIds.Add("example.mod:enchantments/throwing");

        var weapon = new ItemInfo { Name = "weapon_test", Type = "Weapon" };
        var player = new ModelParameters { IsPlayer = true };
        player.Items.Add(weapon);
        player.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/throwing" });
        player.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/active" });
        player.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/failing" });
        player.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/compat" });
        player.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:perks/active_perk" });
        player.JGCNPHDGHAK.Add(new PerkInfoItem { Name = "example.mod:perks/active_perk" });
        player.JGCNPHDGHAK.Add(new PerkInfoItem { Name = "example.mod:perks/inactive_perk" });
        ListSF.Roster.UserPerks.Add("example.mod:perks/active_perk", new RosterPerk(
            SavedItem("<Perk Name='example.mod:perks/active_perk'/>").Node));
        ListSF.Roster.UserItems.Add(weapon, SavedItem(
            "<Item Name='weapon_test'><Enchantments>" +
            "<Perk Name='example.mod:enchantments/throwing' EclipseEnchantment='example.mod:enchantments/throwing'/>" +
            "<Perk Name='example.mod:enchantments/active' EclipseEnchantment='example.mod:enchantments/active'/>" +
            "<Perk Name='example.mod:enchantments/failing' EclipseEnchantment='example.mod:enchantments/failing'/>" +
            "<Perk Name='example.mod:enchantments/compat' EclipseEnchantment='example.mod:enchantments/compat'/>" +
            "<Perk Name='example.mod:enchantments/not_active' EclipseEnchantment='example.mod:enchantments/active'/>" +
            "<Perk Name='missing.mod:enchantments/missing' EclipseEnchantment='missing.mod:enchantments/missing'/>" +
            "<Perk Name='PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON'/>" +
            "</Enchantments></Item>"));

        var fight = new FightHarness(player, 1);
        fight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 4,
            "FightBegin did not dispatch the active learned perk plus active saved enchantments.");
        Assert(Eclipse.Modding.ModRuntime.Invocations[0].Id == "example.mod:perks/active_perk" &&
            Eclipse.Modding.ModRuntime.Invocations[1].Id == "example.mod:enchantments/throwing" &&
            Eclipse.Modding.ModRuntime.Invocations[2].Id == "example.mod:enchantments/active" &&
            Eclipse.Modding.ModRuntime.Invocations[3].Id == "example.mod:enchantments/failing",
            "FightBegin dispatch order/identity did not follow learned perks then active saved enchantments.");
        Assert(Eclipse.Modding.ModRuntime.Invocations[0].Context["side"] == "player" &&
            Eclipse.Modding.ModRuntime.Invocations[0].Context["source"] == "perk" &&
            Eclipse.Modding.ModRuntime.Invocations[0].Context["perk_id"] == "example.mod:perks/active_perk",
            "Behavior-backed perk did not receive sanitized perk context.");
        foreach (Eclipse.Modding.ModRuntime.Invocation invocation in Eclipse.Modding.ModRuntime.Invocations.Skip(1))
        {
            Assert(invocation.Context["side"] == "player" && invocation.Context["source"] == "enchantment" &&
                invocation.Context["item_type"] == "Weapon" &&
                invocation.Context["item_id"] == "weapon_test" && invocation.Context["enchantment_id"] == invocation.Id,
                "FightBegin exposed incorrect sanitized fighter/item context.");
        }
        Assert(UnityEngine.Debug.Warnings.Count == 2 &&
            UnityEngine.Debug.Warnings[0].Contains("node dispatch failed") &&
            UnityEngine.Debug.Warnings[1].Contains("example.mod:enchantments/failing"),
            "Host/behavior failures were not isolated and surfaced as mod combat warnings.");

        fight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 4,
            "FightBegin dispatched more than once for the same Fight instance.");

        var laterRound = new FightHarness(player, 2);
        laterRound.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 4,
            "FightBegin dispatched on a later round.");

        var substitutedItem = new ItemInfo { Name = "weapon_rule_clone", Type = "Weapon", GNDLEFFMJDJ = true };
        var substitutedPlayer = new ModelParameters { IsPlayer = true };
        substitutedPlayer.Items.Add(substitutedItem);
        substitutedPlayer.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/active" });
        ListSF.Roster.UserItems.Add(substitutedItem, SavedItem(
            "<Item Name='weapon_rule_clone'><Enchantments>" +
            "<Perk Name='example.mod:enchantments/active' EclipseEnchantment='example.mod:enchantments/active'/>" +
            "</Enchantments></Item>"));
        new FightHarness(substitutedPlayer, 1).Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 4,
            "Rule-created/replaced item clone incorrectly consumed the player's saved enchantment behavior.");

        var opponent = new ModelParameters { IsPlayer = false };
        opponent.Items.Add(weapon);
        opponent.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:enchantments/active" });
        new FightHarness(opponent, 1).Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 4,
            "Saved UserItem behavior dispatched for a non-player fighter.");

        scripts.Content.AddPerk("example.mod:perks/forged", true);
        var forgedPlayer = new ModelParameters { IsPlayer = true };
        forgedPlayer.Items.Add(weapon);
        forgedPlayer.NHBIJEEKALC.Add(new PerkInfoItem { Name = "example.mod:perks/forged" });
        ListSF.Roster.UserItems.Add(weapon, SavedItem(
            "<Item Name='weapon_test'><Enchantments>" +
            "<Perk Name='example.mod:perks/forged'/><Perk Name='example.mod:perks/forged'/>" +
            "<Perk Name='example.mod:perks/inactive_perk'/>" +
            "</Enchantments></Item>"));
        var forgedFight = new FightHarness(forgedPlayer, 1);
        forgedFight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 5,
            "Forged perk did not dispatch exactly once or ignored active-perk filtering.");
        var forged = Eclipse.Modding.ModRuntime.Invocations.Last();
        Assert(forged.Id == "example.mod:perks/forged" && forged.Context["source"] == "enchantment" &&
            forged.Context["perk_id"] == forged.Id && forged.Context["item_id"] == "weapon_test" &&
            !forged.Context.ContainsKey("enchantment_id"), "Forged perk context lost its equipment provenance.");
        forgedFight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 5, "Forged perk dispatched twice in one fight.");
        Eclipse.Modding.ModRuntime.OnInvoke = forgedFight.Damage;
        forgedFight.Damage();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 6 &&
            Eclipse.Modding.ModRuntime.Invocations.Last().Event == ModEffectEvent.DamageReceived,
            "Damage dispatch was suppressed by FightBegin or re-entered recursively.");
        Eclipse.Modding.ModRuntime.OnInvoke = null;
        forgedFight.Damage();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count == 7, "Damage dispatch did not release its re-entry guard.");

        Eclipse.Modding.ModRuntime.Invocations.Clear();
        var innatePlayer = new ModelParameters { IsPlayer = true };
        var innateWeapon = new ItemInfo { Name="innate_weapon",Type="Weapon",GNDLEFFMJDJ=true };
        var innateArmor = new ItemInfo { Name="innate_armor",Type="Armor" };
        innateWeapon.NHBIJEEKALC.Add(new PerkInfoItem { Name="example.mod:perks/active_perk" });
        innateWeapon.NHBIJEEKALC.Add(new PerkInfoItem { Name="example.mod:perks/inactive_perk" });
        innateArmor.NHBIJEEKALC.Add(new PerkInfoItem { Name="example.mod:perks/active_perk" });
        innatePlayer.Items.Add(innateWeapon); innatePlayer.Items.Add(innateArmor);
        innatePlayer.NHBIJEEKALC.Add(new PerkInfoItem { Name="example.mod:perks/active_perk" });
        var savedItems=ListSF.Roster.UserItems;
        ListSF.Roster.UserItems=null;
        var innateFight=new FightHarness(innatePlayer,1);
        innateFight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==1,"Innate dispatch requires inventory, duplicates items, or ignores active filtering.");
        var firstInnate=Eclipse.Modding.ModRuntime.Invocations.Single();
        Assert(firstInnate.Context["source"]=="innate" && firstInnate.Context["item_id"]=="innate_weapon" && firstInnate.Context["side"]=="player","Innate provenance is missing.");
        Assert(firstInnate.Node.ParentNode is XmlDocument && firstInnate.Node.Attributes["Name"].Value==firstInnate.Id,"Innate instance lacks detached perk identity.");
        innateFight.Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==1,"Innate FightBegin fired twice.");
        Eclipse.Modding.ModRuntime.OnInvoke=innateFight.Damage;
        innateFight.Damage();
        Eclipse.Modding.ModRuntime.OnInvoke=null;
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==2 && ReferenceEquals(firstInnate.Node,Eclipse.Modding.ModRuntime.Invocations.Last().Node),"Innate instance does not persist across events or reentered recursively.");
        innatePlayer.NHBIJEEKALC.Clear(); innateFight.Damage();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==2,"Removed/filtered innate effect still dispatched.");
        innatePlayer.NHBIJEEKALC.Add(new PerkInfoItem { Name="example.mod:perks/active_perk" });
        var freshFight=new FightHarness(innatePlayer,1); freshFight.Dispatch();
        Assert(!ReferenceEquals(firstInnate.Node,Eclipse.Modding.ModRuntime.Invocations.Last().Node),"Innate state leaked into another fight.");
        ListSF.Roster.UserItems=savedItems;
        innatePlayer.JGCNPHDGHAK.Add(new PerkInfoItem { Name="example.mod:perks/active_perk" });
        int before=Eclipse.Modding.ModRuntime.Invocations.Count;
        new FightHarness(innatePlayer,1).Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==before+1 && Eclipse.Modding.ModRuntime.Invocations.Last().Context["source"]=="perk","Learned/innate duplicate suppression changed.");
        ListSF.Roster.UserPerks=new UserPerks();
        before=Eclipse.Modding.ModRuntime.Invocations.Count;
        new FightHarness(innatePlayer,1).Dispatch();
        Assert(Eclipse.Modding.ModRuntime.Invocations.Count==before+1 && Eclipse.Modding.ModRuntime.Invocations.Last().Context["source"]=="innate","Missing learned save node suppressed equipped innate source.");
        Console.WriteLine("Mod FightBegin runtime seam: PASS (learned/saved/innate sources, native filtering, provenance, transient instance isolation, one-shot and re-entry guards).");
        return 0;
    }
}
'@

$code = $code.Replace('__DISPATCH__', $dispatch)
$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code)

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' |
    Select-Object -First 1
if (!$csc) { throw 'Could not locate the Visual Studio Roslyn compiler.' }

$exe = Join-Path $testRoot 'ModFightBeginRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" $modIdPath $definitionIdPath $harness
if ($LASTEXITCODE -ne 0) { throw 'Mod FightBegin runtime regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Mod FightBegin runtime regression failed.' }
