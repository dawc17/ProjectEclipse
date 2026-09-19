$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$selectorSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/SelectAnimation.cs')
$modelSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Model.cs')
$aiSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelAi.cs')
$selection=[regex]::Match($selectorSource,'(?ms)^\tpublic class SelectInfo.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^    internal void PrepareFormAnimation.*?^    \}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate void CheckAnimations\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate void PlayAnimation\(Model.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static void SetTransitions\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool IDLPMHIHDNO\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool AMECGJPMJBF\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool CGAJAFBPFAC\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool MCLDKKPMBGL\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool HMEMGEOKAGG\(.*?^\t\}').Value
$selection+=[regex]::Match($selectorSource,'(?ms)^\tprivate static bool IsNames\(.*?^\t\}').Value
$delay=[regex]::Match($modelSource,'(?ms)^\tpublic void PlayAnimationDelay\(.*?^\t\}').Value
$delay+=[regex]::Match($modelSource,'(?ms)^\tpublic bool RenderAnimationDelay\(.*?^\t\}').Value
$delay+=[regex]::Match($modelSource,'(?ms)^\tpublic bool IBIDGACDJNF\(.*?^\t\}').Value
$delay+=[regex]::Match($modelSource,'(?ms)^\tpublic InfoAnimation EJOGECPBJCE\(.*?^\t\}').Value
# Execute the exact readiness prefix through SetFactors. The remainder of native
# AI decision selection is outside this fixture and is not copied or simulated.
$readiness=[regex]::Match($aiSource,'(?s)\tpublic InfoAnimation Render\(Model.*?TacticFactors fJCBLOKOBBD = SetFactors\(FNKFIMEDNLP\);').Value
$observation=[regex]::Match($aiSource,'(?ms)^\tpublic void StartAnimationEnemy\(.*?^\t\}').Value
$observation+=[regex]::Match($aiSource,'(?ms)^\tprivate bool get_IsEnabled\(.*?^\t\}').Value
if(!$selection -or !$delay -or !$readiness -or !$observation){throw 'Form animation entry extraction failed.'}
$fixture=Join-Path $root ('Temp/FormAnimationEntry-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Random = UnityEngine.Random;
using ObscuredFloat = System.Single;

namespace UnityEngine {
    static class Random {
        public static int Choice;
        public static int Range(int minimum, int maximum) {
            if (maximum <= minimum) throw new Exception("empty native priority choice");
            return minimum + Choice % (maximum - minimum);
        }
    }
}
static class StageType { public enum FDBBPEGEGMK { STAGE_NONE, STAGE_START_STANCE, STAGE_FIGHT, STAGE_END_STANCE } }
static class ModelType { public enum KEIDBIOIFGA { MODEL_NULL, MODEL_THIS, MODEL_OTHER, MODEL_PARENT, MODEL_CHILD, MODEL_BOTH } }
class EventAnimation {
    public enum EECEJKADLCK { EVENT_NONE, EVENT_ROUND_STAGE, EVENT_KEY_PRESSED, EVENT_KEY_RELEASED,
        EVENT_ANIMATION_START, EVENT_ANIMATION_END, EVENT_INTERVAL_START, EVENT_INTERVAL_END,
        EVENT_HIT, EVENT_STRIKE, EVENT_EVERY_FRAME, EVENT_BIRTH, EVENT_MOD_EXPIRES }
    public EECEJKADLCK Type;
    public ModelType.KEIDBIOIFGA IHJJBIDMEMB=ModelType.KEIDBIOIFGA.MODEL_THIS;
    public string LJICHLHMBFA=string.Empty,LONCGFHLFKA=string.Empty;
    public bool IsNot;
}
class EventModelDelayed { public EventAnimation.EECEJKADLCK Type; public object Data; public Model KJDFJPBIGJC,GAIBPAGPEGK; public bool IsRandom; }
class IntervalAnimation { }
class ItemInfo { public string SubType; }
class ModelParameters {
    public int RemainingHealthBars=1;
    public bool IsPlayer,AiControlled=true,KMNLACDHAFE;
    public float CIDCNCDFONA=100;
    public float KKMCHCNOHMB()=>100;
    public ItemInfo KDABEFBJMOD(string type)=>null;
}
class Vector3f {
    public float GetX()=>X;
    public void SetX(float value){X=value;}
    public float X,Y,Z;
    public Vector3f(float x=0,float y=0,float z=0){X=x;Y=y;Z=z;}
    public Vector3f(Vector3f value){X=value.X;Y=value.Y;Z=value.Z;}
    public bool IsEqual(float x,float y,float z)=>X==x&&Y==y&&Z==z;
    public float GILCBJJPKBK()=>X;
    public void JPFALPBDBAP(float value){X=value;}
}
class ModelConditions {
    public int Epoch,JMHJDHLBHLK=2,PCAOCHAIBJC,FOIHIKCEBJF;
    public bool IDCHHGHAENM,IsPlayer;
    public List<string> PDKPGKPBBIL=new List<string>(),NNPJJLPCOHD=new List<string>(),MGFNFEHILNF=new List<string>(),
        DHHADKMMOHP=new List<string>(),NKPMIACBKDE=new List<string>();
}
class TransitionAnimation {
    public bool Matches,IsFrameShift; public int FrameShift;
    public bool HPPGNJJCEGF(ModelConditions conditions)=>Matches;
}
class Orientation { public int OLBDPMKCJIF; }
class AnimationDefinition {
    public List<EventAnimation> AJCMBMJGJEG=new List<EventAnimation>();
    public List<TransitionAnimation> ELFBPNOBDKC=new List<TransitionAnimation>();
    public Orientation ILOEBFFAEAN=new Orientation();
}
class InfoAnimation {
    public enum MGHNBEPCKIF { AnimationNone,AnimationAttack,AnimationIdle }
    public string Name; public int Priority,Sign=1; public bool Eligible=true,FBKGDALBNDJ;
    public MGHNBEPCKIF Type=MGHNBEPCKIF.AnimationIdle;
    public AnimationDefinition MoveData=new AnimationDefinition();
    public InfoAnimation Equivalent; public Vector3f Velocity=new Vector3f();
    public int ConditionCalls;
    public InfoAnimation IMFGMAAEMIC()=>Equivalent;
    public List<string> FOLOOGCLPNE()=>new List<string>{Name};
    public int CEDEDCLGJDE(ModelConditions conditions,int sign)=>Sign;
    public bool IsItemRequired(string type,string subtype)=>false;
    public Vector3f LBJFGCFGMDI()=>Velocity;
    public bool HPPGNJJCEGF(Model model,object unused,EventAnimation cause){
        if(model.Conditions.Epoch==0)throw new Exception("conditions were not refreshed before entry selection");
        ConditionCalls++;return Eligible;
    }
}
class AnimationCandidates {
    public List<InfoAnimation> Values=new List<InfoAnimation>();
    public EventAnimation.EECEJKADLCK Requested;
    // Supply already loadout-filtered candidates. Production event/side matching
    // still runs over their actual EventAnimation records in CheckAnimations.
    public List<InfoAnimation> NCNDKFCPLEH(EventAnimation.EECEJKADLCK kind){Requested=kind;return Values;}
}
class PendingAnimation {
    public InfoAnimation FGICHADOEHF; public int GFHOIKMBNHF,FrameShift; public bool IsFrameShift;
    public bool KLNLNKBIDGD()=>FGICHADOEHF==null;
    public void Clear(){FGICHADOEHF=null;}
}
class Physics { public bool Running;public int Stops;public bool IsPhysics()=>Running;public bool EGNOOKHNFLK()=>Running;public void Stop(){Stops++;Running=false;} }
class ModelAnimation {
    public InfoAnimation Current; public bool Playing=true;public Vector3f Moved;
    public InfoAnimation NNMAFFCCMHC()=>Current;
    public bool NMEEPBDJHMG()=>Playing&&Current!=null;
    public int KFCNPADAMHA()=>1;
    public int HILLKPNMCIP()=>17;
    public void MoveByVelocity(Vector3f velocity){Moved=velocity;}
}
class Statistics {
    readonly Dictionary<InfoAnimation,int> observations=new Dictionary<InfoAnimation,int>();
    public int Reads;
    public void GetCountAndDamage(bool incoming,InfoAnimation animation,ref float count,ref float damage,ref float value){
        // Dictionary null-key behavior is real; native statistics/accounting
        // beyond this dependency is controlled in the readiness fixture.
        if(!observations.ContainsKey(animation))observations.Add(animation,0);
        Reads++;count=observations[animation];
    }
}
class Model {
    public bool RenderStrikeDelay()=>false;
    DELAY_METHODS
    public readonly ModelParameters Parameters=new ModelParameters();
    public readonly ModelConditions Conditions=new ModelConditions();
    public readonly AnimationCandidates CEOOLFLLIMC=new AnimationCandidates();
    public readonly ModelAnimation _Animation=new ModelAnimation();
    public readonly Physics _Physics=new Physics();
    public readonly PendingAnimation AAJKEBAIJAP=new PendingAnimation();
    public readonly Statistics Stats=new Statistics();
    public Model Enemy,Owner;public int FirstFrameActions,PlayedSign,PlayedShift;public bool PlayedFrameShift,RejectPlay;
    public bool POCBCFMBKLO;public StrikeResult GHHCDAFIKJE=new StrikeResult();
    public class StrikeResult { public bool DFOHNJEBDED; }
    public InfoAnimation.MGHNBEPCKIF DFLPNNBIFFN;public EventAnimation.EECEJKADLCK KMDKCFHMECJ;
    public ModelConditions EBABHGHPLFK()=>Conditions;
    public ModelAnimation OCPMJKIEPIG()=>_Animation;
    public InfoAnimation FHBLLPCEAHG()=>_Animation.Current;
    public Model EGGEACCDAEK()=>Enemy;
    public Model NJDJHGDMCIJ()=>Owner;
    public Model NMGNPBMFJKP(ModelType.KEIDBIOIFGA kind)=>null;
    public bool EDJFLMILEBA()=>false;
    public bool FGKAFKFBFEM()=>Parameters.AiControlled;
    public Statistics FGACEEPJBIF()=>Stats;
    public int GLEKCPCMINJ()=>0;
    public int LPOJKGLFMAL()=>0;
    public void IFDGGKPAHMC(InfoAnimation animation,bool hit){}
    public bool PlayAnimation(InfoAnimation animation,int sign,bool shifted,int shift){
        if(RejectPlay)return false;
        FirstFrameActions++;_Animation.Current=animation;PlayedSign=sign;PlayedFrameShift=shifted;PlayedShift=shift;return true;
    }
}
class WeaponModel:Model { }
static class GameUtils { public static Balance BJACOFCAHPD=new Balance();public class Balance{public float BeginnerCheat;} }
static class AiData { public static bool Enabled=true,Both;public static bool get_BothBotEnabled()=>Both; }
class TacticFactors {
    public float EOGLBDCLMBM,KFMJMBANIGF,AAKOCIPFDNM,MGICNNKKCAN,DDGNCMJGDAG;
    public int OLCKGMBDGOG,NGMLGDJGBCD;public InfoAnimation Selected;
    public TacticFactors(Statistics stats,int ranged,int magic){}
}
class ModelAi {
    AI_READINESS_PREFIX
        return fJCBLOKOBBD.Selected;
    }
    AI_OBSERVATION_METHODS
    readonly Model _Model;readonly ModelAnimation _ModelAnimation;readonly ModelParameters NHDAJBADMND;
    public InfoAnimation COKFBIJAFLH;
    public int Randomizations,FactorCalls,ResponseChecks,EBEHPENMJLK;
    public bool Ignore;public readonly InfoAnimation Result=new InfoAnimation{Name="fixture decision"};
    public ModelAi(Model model){_Model=model;_ModelAnimation=model._Animation;NHDAJBADMND=model.Parameters;}
    static bool get_AiOn()=>AiData.Enabled;
    void RandomizeBehavior(Model enemy){Randomizations++;}
    bool IsIgnoredEnemyAnimation(InfoAnimation animation)=>Ignore;
    int ChildMaxModelFrame(Model model)=>0;
    int GetResponseDelay(TacticFactors factors){ResponseChecks++;return 9;}
    TacticFactors SetFactors(Model enemy){
        FactorCalls++;var factors=new TacticFactors(_Model.Stats,0,0){Selected=Result};
        _Model.Stats.GetCountAndDamage(true,COKFBIJAFLH,ref factors.EOGLBDCLMBM,ref factors.KFMJMBANIGF,ref factors.AAKOCIPFDNM);
        return factors;
    }
}
class SelectAnimation {
    SELECTOR_METHODS
    public readonly List<Model> BPIFJBJBKHA=new List<Model>();
    readonly List<ModelConditions> _ModelsConditions=new List<ModelConditions>();
    public readonly List<object> CFKGCLIKKOC=new List<object>(),KPHAPCNOPNP=new List<object>(),IJIHPHBMEOI=new List<object>();
    public readonly List<Model> _ExplicitBirthModels=new List<Model>(),HKOBFBADDJN=new List<Model>();
    public int Updates;
    void UpdateConditions(){
        Updates++;_ModelsConditions.Clear();
        foreach(var model in BPIFJBJBKHA){model.Conditions.Epoch=Updates;model.Conditions.IsPlayer=model.Parameters.IsPlayer;_ModelsConditions.Add(model.Conditions);}
    }
    void PlayAnimationRandom(Model model,List<SelectInfo> selections){throw new Exception("entry unexpectedly used AI random choice");}
    static bool KLAJPEHFFAP(EventAnimation value,StageType.FDBBPEGEGMK stage)=>false;
    static bool NDAMNPDILFE(EventAnimation value)=>true;
    static bool IIJINHHFJNA(EventAnimation value)=>true;
    static bool KBMLJHIEOIK(EventAnimation value,IntervalAnimation interval)=>false;
    static bool KJHNKIAEHIM(EventAnimation value,IntervalAnimation interval)=>false;
    static bool IsHit(EventAnimation value,EventModelDelayed delayed,bool critical=false,bool shock=false)=>false;
    static bool HDLPPBKLCBF(EventAnimation value)=>true;
    static bool HBNBGLGIEFF(EventAnimation value)=>true;
    static bool DHFHPKJBJHB(EventAnimation value,string name)=>false;
}
static class ValidateFormAnimationEntry {
    static int checks;
    static void Check(bool value,string why){checks++;if(!value)throw new Exception(why);}
    static void Reject(Action operation,string why){bool failed=false;try{operation();}catch(InvalidOperationException){failed=true;}Check(failed,why);}
    static InfoAnimation Move(string name,int priority,EventAnimation.EECEJKADLCK kind=EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END){
        var move=new InfoAnimation{Name=name,Priority=priority};move.MoveData.AJCMBMJGJEG.Add(new EventAnimation{Type=kind});return move;
    }
    static void Selection(){
        foreach(bool player in new[]{false,true})foreach(int sign in new[]{-1,1}){
            var form=new Model();form.Parameters.IsPlayer=player;
            var other=new Model();var previous=Move("ongoing opponent",0);other._Animation.Current=previous;
            form.Enemy=other;other.Enemy=form;
            var selector=new SelectAnimation();selector.BPIFJBJBKHA.AddRange(player?new[]{form,other}:new[]{other,form});
            var marker=new object();selector.CFKGCLIKKOC.Add(marker);selector.KPHAPCNOPNP.Add(marker);selector.IJIHPHBMEOI.Add(marker);
            selector._ExplicitBirthModels.Add(other);selector.HKOBFBADDJN.Add(other);
            var idle=Move("generic idle",0);var transition=Move("arbitrary mod entry",7);transition.Sign=sign;
            transition.Velocity=new Vector3f(2,3,0);
            transition.MoveData.ELFBPNOBDKC.Add(new TransitionAnimation{Matches=false,FrameShift=91});
            transition.MoveData.ELFBPNOBDKC.Add(new TransitionAnimation{Matches=true,IsFrameShift=true,FrameShift=6});
            var denied=Move("ineligible high priority",99);denied.Eligible=false;
            var otherEvent=Move("opponent only",100);otherEvent.MoveData.AJCMBMJGJEG[0].IHJJBIDMEMB=ModelType.KEIDBIOIFGA.MODEL_OTHER;
            var namedEnd=Move("unrelated animation end",101);namedEnd.MoveData.AJCMBMJGJEG[0].LJICHLHMBFA="missing old animation";
            var birth=Move("helper birth",102,EventAnimation.EECEJKADLCK.EVENT_BIRTH);
            form.CEOOLFLLIMC.Values.AddRange(new[]{idle,transition,transition,denied,otherEvent,namedEnd,birth});
            selector.PrepareFormAnimation(form);
            Check(form.EJOGECPBJCE()==transition,"native priority chooses eligible entry without a hardcoded animation name");
            Check(transition.ConditionCalls==1,"duplicate candidate references are deduplicated by native selection");
            Check(form.FHBLLPCEAHG()==null&&form.FirstFrameActions==0,"preparation schedules but does not execute animation actions");
            Check(other.FHBLLPCEAHG()==previous&&!other.IBIDGACDJNF()&&other.FirstFrameActions==0,"opponent animation stays untouched");
            Check(form.Conditions.JMHJDHLBHLK==2,"selection preserves current fight stage");
            Check(form.CEOOLFLLIMC.Requested==EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END,"requests native animation-end candidates");
            Check(selector.CFKGCLIKKOC.SequenceEqual(new[]{marker})&&selector.KPHAPCNOPNP.SequenceEqual(new[]{marker})&&selector.IJIHPHBMEOI.SequenceEqual(new[]{marker}),"pending animation and trigger records are unchanged");
            Check(selector._ExplicitBirthModels.SequenceEqual(new[]{other})&&selector.HKOBFBADDJN.SequenceEqual(new[]{other}),"helper creation queues are unchanged");
            Check(form._Animation.Moved.X==2*sign&&form._Animation.Moved.Y==3&&transition.Velocity.X==2,"native scheduling applies facing to an independent velocity vector");
            Check(form.RenderAnimationDelay()&&form.FHBLLPCEAHG()==transition,"first native delayed-render step starts selected animation");
            Check(form.PlayedSign==sign&&form.PlayedFrameShift&&form.PlayedShift==6,"native matching transition and facing survive scheduling");
            Check(!form.RenderAnimationDelay()&&form.FirstFrameActions==1,"first animation actions execute exactly once");
        }
    }
    static void Rejections(){
        var selector=new SelectAnimation();var form=new Model();selector.BPIFJBJBKHA.Add(form);
        Reject(()=>selector.PrepareFormAnimation(null),"null body rejects");
        Reject(()=>selector.PrepareFormAnimation(new Model()),"unregistered body rejects");
        Check(selector.Updates==0,"identity rejection occurs before condition updates");
        form._Animation.Current=Move("started",0);Reject(()=>selector.PrepareFormAnimation(form),"started body rejects");
        form._Animation.Current=null;form.AAJKEBAIJAP.FGICHADOEHF=Move("pending",0);
        Reject(()=>selector.PrepareFormAnimation(form),"already scheduled body rejects");form.AAJKEBAIJAP.Clear();
        Reject(()=>selector.PrepareFormAnimation(form),"no eligible move rejects before commit");
        Check(!form.IBIDGACDJNF()&&form.FirstFrameActions==0,"empty candidate failure does not start body");
        var denied=Move("denied",1);denied.Eligible=false;form.CEOOLFLLIMC.Values.Add(denied);
        Reject(()=>selector.PrepareFormAnimation(form),"native condition rejection cannot report entry success");
        form.CEOOLFLLIMC.Values.Clear();var hit=Move("strike branch",1);hit.FBKGDALBNDJ=true;form.CEOOLFLLIMC.Values.Add(hit);
        Reject(()=>selector.PrepareFormAnimation(form),"selection that does not schedule an animation is rejected");
        form.CEOOLFLLIMC.Values.Clear();var one=Move("tie one",2);var two=Move("tie two",2);
        form.CEOOLFLLIMC.Values.AddRange(new[]{one,two});UnityEngine.Random.Choice=1;selector.PrepareFormAnimation(form);
        Check(form.EJOGECPBJCE()==two,"equal highest priorities use native selection pool");UnityEngine.Random.Choice=0;
    }
    static void Readiness(){
        var owner=new Model();var enemy=new Model();var ai=new ModelAi(owner);
        enemy._Animation.Current=Move("already running opponent",0);
        Check(ai.Render(enemy,180)==null&&ai.FactorCalls==0&&ai.Randomizations==0,"AI defers before own initial animation");
        owner._Animation.Current=Move("newly started form",0);
        Check(ai.Render(null,181)==null&&ai.FactorCalls==0,"missing opponent defers AI");
        enemy._Animation.Current=null;
        Check(ai.Render(enemy,182)==null&&ai.FactorCalls==0&&ai.COKFBIJAFLH==null,"unstarted opponent cannot populate observation or factors");
        var original=Move("opponent motion",0);var equivalent=Move("native tactic equivalent",0);original.Equivalent=equivalent;
        enemy._Animation.Current=original;enemy._Animation.Playing=false;
        Check(ai.Render(enemy,183)==null&&ai.Randomizations==0,"nonplaying opponent does not fake an animation observation");
        enemy._Animation.Playing=true;
        Check(ai.Render(enemy,184)==ai.Result&&ai.COKFBIJAFLH==equivalent,"native enemy observation seeds tactic equivalent before factors");
        Check(ai.Randomizations==1&&ai.ResponseChecks==1&&ai.EBEHPENMJLK==9&&ai.FactorCalls==1,"native observation initialization happens once");
        Check(ai.Render(enemy,185)==ai.Result&&ai.Randomizations==1&&ai.FactorCalls==2,"ready controller does not repeat observation initialization");
        var next=Move("next opponent motion",0);enemy._Animation.Current=next;ai.StartAnimationEnemy(enemy);
        Check(ai.COKFBIJAFLH==next&&ai.Randomizations==2,"ordinary later native observation still updates cache");
        foreach(bool global in new[]{false,true})foreach(bool bot in new[]{false,true}){
            AiData.Enabled=global;owner=new Model();owner.Parameters.AiControlled=bot;owner._Animation.Current=Move("own",0);ai=new ModelAi(owner);
            var result=ai.Render(enemy,186);
            Check((result!=null)==(global&&bot),"original global and per-fighter AI eligibility remains effective");
        }
        AiData.Enabled=true;AiData.Both=true;owner=new Model();owner.Parameters.AiControlled=false;owner._Animation.Current=Move("player",0);ai=new ModelAi(owner);
        Check(ai.Render(enemy,187)==ai.Result,"both-bot native override still initializes observation");AiData.Both=false;
    }
    public static void Main(){
        Selection();Rejections();Readiness();
        Console.WriteLine("PASS: "+checks+" production form animation entry/readiness checks. Actual candidate/event/side matching, priority, transition and delayed playback orchestration; exact AI Render readiness prefix and native StartAnimationEnemy. Rig/condition evaluation, playback, statistics and downstream AI decisions controlled; no Unity gameplay.");
    }
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('SELECTOR_METHODS',$selection).Replace('DELAY_METHODS',$delay).Replace('AI_READINESS_PREFIX',$readiness).Replace('AI_OBSERVATION_METHODS',$observation))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form animation entry/readiness checks failed.'}
