// Production round arbitration/score/winner methods are compiled by the runner.
// These inputs control models, clock, presentation and settlement; no game rewards
// or native animations are executed by this managed fixture.
using System;
using System.Collections.Generic;
using Eclipse.Modding;
namespace CodeStage.AntiCheat.ObscuredTypes
{
    public struct ObscuredFloat { float value; public static implicit operator ObscuredFloat(float x)=>new ObscuredFloat{value=x}; public static implicit operator float(ObscuredFloat x)=>x.value; }
}
#if !UNITY_5_3_OR_NEWER
namespace UnityEngine { public static class Debug { public static void Log(object message) { } } }
#endif
namespace Eclipse.Multiplayer
{
    public static class VersusTickDriver { public static bool Barrier()=>false; }
    public static class LocalVersusRoundRules { public static int ResolveWinner(float player,float enemy)=>player>=enemy?0:1; }
    public static class LocalVersusSession { public static void Complete(Fight fight,bool surrender) { } }
}
namespace Eclipse.Modding
{
    public sealed class FixtureScripts { public ModContentCatalog Content; }
    public static partial class ModRuntime { public static FixtureScripts Scripts; }
    public static class ModModeRuntime { public static bool OfflineRaid; public static bool IsRaid(FightList fight)=>OfflineRaid; public static IReadOnlyList<DefinitionId> ActiveRules(string id)=>null; }
}
public static class StageType { public enum FDBBPEGEGMK { STAGE_FIGHT,STAGE_END_STANCE } }
public sealed class ModelParameters
{
    public int RoundsWon;
    public bool IsPlayer, IsWinner, BHHLEBHLBLH, PCALDKCJGCK;
    public float Health=1;
    public EndRoundType EndRoundType;
    public float KKMCHCNOHMB()=>Health;
    public float HABJPOFCIHA()=>Health;
}
public sealed class Model { public ModelParameters Parameters; }
public sealed class FightList
{
    public bool TrackFightProgress;
    public BattleType Type=BattleType.FightTournament;
    public string FightId="core:fights/fixture";
    public BattleType get_Type()=>Type;
    public bool CBJOENICLAF()=>false;
}
public sealed class PreFight
{
    public bool Timeout;
    public int ScoreUpdates;
    public bool IsTimeOut()=>Timeout;
    public void ViewerUpdateVictorys()=>ScoreUpdates++;
    public ComboStatistic GetStatistic(int side)=>null;
    public int get_TimeLeft()=>20;
}
public static class FightEvent { public const int TimeoutEvent=1; }
public sealed class RulesInspector
{
    public bool RulesActive=true;
    public int Stops;
    public void StopRules()=>Stops++;
    public void CheckEvent(int value,RuleAppliance target,object context) { }
}
public sealed class Rule { public RuleAppliance Target; public RuleAppliance IMINMDOFHMG()=>Target; }
public sealed class RoundData { public int round=1,roundTotal=1; public bool processing=true; }
public sealed class EndData { public ModelParameters ABKBEJBICOA,LEBLJJCFKOP; }
public sealed class FixtureCamera { public void DFKKNMDAFDC(bool value) { } }
public sealed class FixtureController { public void StopController() { } public void ClearScriptControlBlocks() { } }
public sealed partial class ListSF { public static readonly ListSF Instance=new ListSF(); public static ListSF CCDKHLAMKKO()=>Instance; public bool Eclipse; public bool IsEclipseMode()=>Eclipse; public ListSF KJNPJKEHGLE()=>this; public void BFCLLIKOJGD() { } }
public sealed class ComboStatistic { }
public static class Sound { public static void StopLoopedSounds() { } }
public sealed class FixtureCounters { public void Complete(int rounds,bool surrender) { } public void HOCBEHCHOFL(bool surrender) { } }
public static class GameUtils { public static void EndFight(ComboStatistic player,FightList fight,object winner,object loser,GameOverTypes result,ComboStatistic enemy,object data) {Fight.Current.Settlements++;} }
public sealed partial class Fight : IModFighterOperations, IModRoundOutcomes, IModCombatSnapshotSource, IModDamageEventSource, IModIncomingHitSource
{
    // Movement is outside this round-arbitration fixture; covered by FighterMotionTests.
    private void CancelEclipseFighterMotion() { }
    private void CancelEclipseFighterPlayback() { }
    public readonly ModelParameters Player=new ModelParameters{IsPlayer=true}, Enemy=new ModelParameters();
    ModelParameters NMNCKBPFCCP,AKBNKDBHCEO;
    Model playerModel,enemyModel;
    public static Fight Current;
    public FightList FightDefinition=new FightList();
    public readonly PreFight preFight=new PreFight();
    public readonly RoundData round=new RoundData();
    public bool IsLocalVersus,IsTitleSparring,Paused;
    public bool isEndRound,isGameOver,isStopFight,_eclipseFightEndDispatched;
    public bool GJMHPBIBHMO,BDDBMCNFNMG,KJKJOJCMDGH,OMBDLIKCNIP,FCCPOLAMJNO,MNEOALEBNNA=true,IDMICHMHCKE;
    public Rule _endFightRule;
    public int ADJAMFGBOAP,HJCJMEELHPC,Settlements,RoundEndEvents,Stops;
    public readonly List<ModelParameters> IDAAONBIBJM=new List<ModelParameters>();
    public readonly EndData LBKDADMLJOE=new EndData();
    public readonly FixtureCamera _Camera=new FixtureCamera();
    public readonly FixtureController Controller=new FixtureController();
    public readonly RulesInspector _rulesInspector=new RulesInspector();
    public object DPONLGICLEH;
    public StageType.FDBBPEGEGMK stageType=StageType.FDBBPEGEGMK.STAGE_FIGHT;
    public EndRoundType _endRoundType=EndRoundType.EndRoundTypeZeroHealth;
    public readonly ModRoundOutcomeState _eclipseRoundOutcomes=new ModRoundOutcomeState();
    public readonly ModBattleRuleInstances _eclipseBattleRules=new ModBattleRuleInstances();
    public readonly Dictionary<string,string> _eclipseShields=new Dictionary<string,string>();
    public readonly FixtureCounters MOBFFOHPCOE=new FixtureCounters();
    public string _eclipsePlayerResult;
    public object DKDMOJJJHHL;
    bool _eclipseFightBeginDispatched=true;
    int _eclipseEndedRound;
    public Fight() {NMNCKBPFCCP=Player;AKBNKDBHCEO=Enemy;playerModel=new Model{Parameters=Player};enemyModel=new Model{Parameters=Enemy};IDAAONBIBJM.Add(Enemy);Current=this;}
    public static Fight GetCurrentFight()=>Current;
    public Model GetPlayerModel()=>playerModel;
    public Model GetEnemyModel()=>enemyModel;
    public bool IsPaused()=>Paused;
    public bool get_IsRaidFight()=>FightDefinition.Type==BattleType.FightRaid;
    public bool TryEndRound(DefinitionId rule,bool won,out string error)=>TryQueueRoundOutcome(rule,won,out error);
    public void Step()=>RenderRound();
    public ModelParameters Winner()=>GetWinner(true);
    public void CompleteStance()
    {
        LBKDADMLJOE.ABKBEJBICOA=GetWinner(true);LBKDADMLJOE.LEBLJJCFKOP=GetWinner(false);
        FinishRound();isStopFight=false;GJMHPBIBHMO=true;
    }
    public void Surrender()=>HCNDAFDHACI(GameOverTypes.GAME_OVER_SURRENDER);
    public void ResetForNextRound() {round.round++;round.processing=true;isStopFight=isEndRound=isGameOver=GJMHPBIBHMO=false;stageType=StageType.FDBBPEGEGMK.STAGE_FIGHT;_eclipseRoundOutcomes.BeginRound(-1,null);}
    public bool TryChangeHealth(double amount,out string error) {error=null;return true;}
    public bool TryAddMagicCharge(double amount,out string error) {error=null;return true;}
    public ModCombatSnapshot CaptureCombatSnapshot()=>new ModCombatSnapshot(new ModFighterSnapshot(Player.Health,1,1,0,0,0),new ModFighterSnapshot(Enemy.Health,1,1,10,0,0),Clock,round.processing);
    public int Clock=1;
    public ModDamageEvent DamageEvent { get; set; }
    public ModIncomingHit IncomingHit { get; set; }
    sealed class EclipseFighterOperations { readonly Model model; public EclipseFighterOperations(Fight fight,Model value){model=value;} public double Health=>model.Parameters.Health; }
    void GameOver(ModelParameters winner,ModelParameters loser) {isGameOver=true;}
    void EndFight() {Settlements++;isGameOver=false;}
    void EndFightRaid()=>EndFight();
    void HGGGBDFFGNM() { }
    void ActionModels(bool active) {if(!active)Stops++;}
    void IFKFINOGOLC(bool value) { }
    void ResetModels(bool value) { }
    void ResetParameters() { }
    void CMAOBIFAOCI(bool value) { }
    void NextRound()=>ResetForNextRound();
    void UpdateFightData(int value) { }
    void SetStage(StageType.FDBBPEGEGMK value)=>stageType=value;
    void DispatchEclipseCombatEvent(ModEffectEvent kind) {if(kind==ModEffectEvent.RoundEnd)RoundEndEvents++;}
    void DispatchEclipseOpponent(ModEffectEvent kind) { }
    void ClearEclipseStatusIcons() { }
}
