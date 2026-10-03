// The complete production FightFighterMotion partial is compiled by the runner.
// This fixture controls model translation and fight/session inputs. It does not
// claim native rig, keyframe, collision or rendering acceptance.
using System;
using System.Collections.Generic;
using Eclipse.Modding;
namespace UnityEngine { public static class Debug { public static void LogWarning(object value) { } } }
namespace Eclipse.Modding
{
    public sealed class ModScriptSession { public ModContentCatalog Content = new ModContentCatalog(); }
    public static class ModRuntime { public static ModScriptSession Scripts = new ModScriptSession(); }
    public static class ModModeRuntime { public static bool OfflineRaid; public static bool IsRaid(FightList fight)=>OfflineRaid; }
}
public static class StageType { public enum FDBBPEGEGMK { STAGE_FIGHT, STAGE_END_STANCE } }
public sealed class FightList { public BattleType Type=BattleType.FightTournament; public BattleType get_Type()=>Type; }
public sealed class RoundData { public int round=1; public bool processing=true; }
public sealed class Vector3f { public float X,Y,Z; public Vector3f(float x,float y,float z){X=x;Y=y;Z=z;} }
public sealed class Model
{
    public readonly List<InfoAnimation> Animations=new List<InfoAnimation>();
    public string Playing; public int Plays; public bool RejectPlayback; public Action OnPlay;
    public List<InfoAnimation> GetAvailableAnimations()=>Animations;
    public bool PlayAnimation(string name){if(Throw)throw new InvalidOperationException("Controlled playback failure");if(RejectPlayback)return false;Playing=name;Plays++;OnPlay?.Invoke();return true;}
    public double Health=1,X,Y,Z; public int Translations; public bool AnimationShifted,Throw;
    public void ShiftModelPosition(Vector3f offset,bool animation)
    {
        if(Throw)throw new InvalidOperationException("Controlled native failure");
        X+=offset.X;Y+=offset.Y;Z+=offset.Z;Translations++;AnimationShifted=animation;
    }
}
public sealed class InfoAnimation { public string Name; }
public partial class Fight
{
    public static Fight Current;
    public Model Player=new Model(),Enemy=new Model();
    public readonly RoundData round=new RoundData();
    public FightList FightDefinition=new FightList();
    public bool IsLocalVersus,IsTitleSparring,_modelTransitionsClosed,_eclipseFightEndDispatched,Paused;
    public bool isEndRound,isGameOver,isStopFight;
    public int _eclipseEndedRound=-1,Clock=1;
    public StageType.FDBBPEGEGMK stageType=StageType.FDBBPEGEGMK.STAGE_FIGHT;
    public Fight(){Current=this;ModRuntime.Scripts=new ModScriptSession();ModModeRuntime.OfflineRaid=false;}
    public static Fight GetCurrentFight()=>Current;
    public Model GetPlayerModel()=>Player;
    public Model GetEnemyModel()=>Enemy;
    public bool get_IsRaidFight()=>FightDefinition.Type==BattleType.FightRaid;
    public bool IsPaused()=>Paused;
    public bool Queue(Model body,double x,double y,double z,out string reason)=>TryQueueEclipseFighterMotion(body,x,y,z,out reason);
    public void Step()=>ApplyEclipseFighterMotion();
    public void PlaybackStep()=>ApplyEclipseFighterPlayback();
    public void CancelPlayback()=>CancelEclipseFighterPlayback();
    public bool QueuePlayback(Model body,DefinitionId move,Action<bool,string> complete,out string reason)=>TryQueueEclipseFighterPlayback(body,move,complete,out reason);
    public void Cancel()=>CancelEclipseFighterMotion();
    public IModFighterOperations Operations()=>new EclipseFighterOperations(this,Player);
    public sealed class EclipseFighterOperations : IModFighterOperations, IModFighterMotion, IModFighterPlayback, IModFighterTargets, IModCombatSnapshotSource
    {
        readonly Fight fight; readonly Model body;
        public EclipseFighterOperations(Fight fight,Model body){this.fight=fight;this.body=body;}
        public double Health=>body.Health;
        public ModCombatSnapshot CaptureCombatSnapshot(){var other=body==fight.Player?fight.Enemy:fight.Player;return new ModCombatSnapshot(new ModFighterSnapshot(body.Health,1,1,body.X,body.Y,body.Z),new ModFighterSnapshot(other.Health,1,1,other.X,other.Y,other.Z),fight.Clock,fight.round.processing);}
        public IModFighterOperations Opponent=>new EclipseFighterOperations(fight,body==fight.Player?fight.Enemy:fight.Player);
        public bool TryMoveBy(double x,double y,double z,out string error)=>fight.Queue(body,x,y,z,out error);
        public bool TryPlayMove(DefinitionId move,Action<bool,string> complete,out string error)=>fight.QueuePlayback(body,move,complete,out error);
        public bool TryChangeHealth(double amount,out string error){error=null;body.Health+=amount;return true;}
        public bool TryAddMagicCharge(double amount,out string error){error="Controlled unavailable operation";return false;}
    }
}
