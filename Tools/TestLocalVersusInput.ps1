$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$fixture=Join-Path $root ('Temp/LocalVersusInput-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
$code=@'
using System;
using System.Collections.Generic;
using Eclipse.Input;
using UnityEngine;

namespace UnityEngine {
 public enum KeyCode { A=97,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z }
 public struct Vector2 { public float x,y; public Vector2(float x,float y){this.x=x;this.y=y;} public float sqrMagnitude=>x*x+y*y; public static Vector2 zero=>new Vector2(); public static bool operator !=(Vector2 a,Vector2 b)=>a.x!=b.x||a.y!=b.y; public static bool operator ==(Vector2 a,Vector2 b)=>!(a!=b); public override bool Equals(object o)=>false; public override int GetHashCode()=>0; }
 public struct Vector3 { public static implicit operator Vector2(Vector3 v)=>new Vector2(); public static Vector3 zero=>new Vector3(); }
 public static class Mathf { public const float Rad2Deg=57.2957795f; public static float Atan2(float y,float x)=>(float)Math.Atan2(y,x); }
 public static class Input { public static readonly HashSet<KeyCode> Keys=new(); public static string[] Devices=Array.Empty<string>(); public static bool GetKey(KeyCode k)=>Keys.Contains(k); public static string[] GetJoystickNames()=>Devices; }
}
public static class GamePad {
 public enum Player { Any=0,One=1,Two=2 } public enum Stick { LeftStick,RightStick,Dpad } public enum Button { A,B,Y,X,RightShoulder } public enum Trigger { LeftTrigger,RightTrigger }
 public static readonly Dictionary<(Player,Stick),Vector2> Axes=new(); public static readonly HashSet<(Player,int)> Buttons=new();
 public static Vector2 GetStick(Stick s,Player p,bool raw=false)=>Axes.TryGetValue((p,s),out var v)?v:Vector2.zero;
}
namespace Eclipse.Input {
 public static class FightControllerBindings { public static GamePad.Stick MovementStick=>GamePad.Stick.LeftStick; public static int Get(int a)=>a; public static bool IsPressed(int a,GamePad.Player p)=>GamePad.Buttons.Contains((p,a)); }
 public static class FightKeyBindings { public static KeyCode Get(KeyCode k)=>k; }
}
class Program {
 static int checks; static readonly List<string> A=new(),B=new(); static bool enabled=true;
 static void Check(bool x,string m){checks++;if(!x)throw new Exception(m);} static void Emit(List<string> l,int t,FightCID c)=>l.Add(t+":"+c);
 static void Clear(){Input.Keys.Clear();GamePad.Axes.Clear();GamePad.Buttons.Clear();A.Clear();B.Clear();enabled=true;Input.Devices=new[]{"pad1","pad2"};}
 static void Main(){
  var p1=new FightGamepadInput(c=>enabled,(t,c)=>Emit(A,t,c),GamePad.Player.One); var p2=new FightGamepadInput(c=>enabled,(t,c)=>Emit(B,t,c),GamePad.Player.One);
  Clear(); Input.Keys.Add(KeyCode.W); Input.Keys.Add(KeyCode.O); GamePad.Buttons.Add((GamePad.Player.One,1)); p1.Poll(true,true,false); p2.Poll(false,false,true);
  Check(A.Contains("0:QuadrantUp")&&A.Contains("0:Punch")&&!A.Contains("0:Kick"),"keyboard-only P1 routing failed"); Check(B.Count==1&&B[0]=="0:Kick","pad One P2 routing/cross-control failed");
  Clear(); var one=new FightGamepadInput(c=>true,(t,c)=>Emit(A,t,c),GamePad.Player.One); var two=new FightGamepadInput(c=>true,(t,c)=>Emit(B,t,c),GamePad.Player.Two); GamePad.Buttons.Add((GamePad.Player.One,0)); GamePad.Buttons.Add((GamePad.Player.Two,1)); one.Poll(false,false,true);two.Poll(false,false,true);Check(A[0]=="0:Punch"&&B[0]=="0:Kick","two pads not distinct");
  one.Poll(false,false,true);Check(A.Count==1,"held pad duplicated transition");GamePad.Buttons.Clear();one.Poll(false,false,true);Check(A.Count==2&&A[1]=="1:Punch","pad up did not release");
  Clear(); var move=new FightGamepadInput(c=>true,(t,c)=>Emit(A,t,c)); Input.Keys.Add(KeyCode.W);Input.Keys.Add(KeyCode.S);move.Poll(true,false,false);Check(A.Count==0,"opposite movement not neutral");Input.Keys.Remove(KeyCode.S);Input.Keys.Add(KeyCode.D);move.Poll(true,false,false);Check(A.Count==1&&A[0]=="0:QuadrantUpForward","diagonal press failed");Input.Keys.Remove(KeyCode.W);move.Poll(true,false,false);Check(A.Count==3&&A[1]=="1:QuadrantUpForward"&&A[2]=="0:QuadrantForward","diagonal release did not restore direction");
  Clear(); var disc=new FightGamepadInput(c=>true,(t,c)=>Emit(A,t,c));GamePad.Buttons.Add((GamePad.Player.One,0));disc.Poll(false,false,true);Input.Devices=Array.Empty<string>();disc.Poll(false,false,true);Check(A.Count==2&&A[1]=="1:Punch","disconnect did not neutralize");Check(!FightGamepadInput.IsConnected(GamePad.Player.One),"disconnected slot reported connected");Input.Devices=new[]{"","pad2"};Check(!FightGamepadInput.IsConnected(GamePad.Player.One)&&FightGamepadInput.IsConnected(GamePad.Player.Two),"IsConnected slot mapping failed");
  Clear(); var rel=new FightGamepadInput(c=>enabled,(t,c)=>Emit(A,t,c));GamePad.Buttons.Add((GamePad.Player.One,0));rel.Poll(false,false,true);rel.ReleaseAll();int n=A.Count;rel.ReleaseAll();Check(A.Count==n&&A[n-1]=="1:Punch","ReleaseAll not idempotent");
  Clear(); var gate=new FightGamepadInput(c=>enabled,(t,c)=>Emit(A,t,c));GamePad.Buttons.Add((GamePad.Player.One,0));gate.Poll(false,false,true);enabled=false;gate.Poll(false,false,true);Check(A.Count==2&&A[1]=="1:Punch","enabled gate did not release active control");
  Clear(); var keyup=new FightGamepadInput(c=>true,(t,c)=>Emit(A,t,c));Input.Keys.Add(KeyCode.O);keyup.Poll(true,true,false);keyup.Poll(true,true,false);Input.Keys.Clear();keyup.Poll(true,true,false);Check(A.Count==2&&A[0]=="0:Punch"&&A[1]=="1:Punch","keyboard hold/up transitions failed");
  foreach(var control in new[]{FightCID.Punch,FightCID.Kick,FightCID.MissileButton,FightCID.MagicButton,FightCID.RaidChargeButton}) {
   var rules=new FightControlRuleGate();
   Check(rules.Press(control),"Available rule control refused");
   Check(rules.SetBlocked(control,true),"Blocking a held control did not request release");
   Check(!rules.SetBlocked(control,true)&&!rules.Press(control),"Repeated block/press escaped gate");
   rules.SetBlocked(control,false);Check(!rules.Press(control),"Held input replayed when rule ended");
   Check(!rules.Release(control)&&rules.Press(control)&&rules.Release(control),"Neutral did not restore control or repeated release leaked");
   rules.SetBlocked(control,true);Check(!rules.Press(control),"New blocked input accepted");
   Check(!rules.Release(control),"Blocked press generated unmatched release");
   rules.SetBlocked(control,false);Check(rules.Press(control)&&rules.Release(control),"Released control remained blocked");
   Check(rules.Press(FightCID.QuadrantForward)&&rules.Release(FightCID.QuadrantForward),"Button rule gated movement");
  }
  Console.WriteLine("PASS: "+checks+" production FightGamepadInput routing/state and button-rule gate checks.");
 }
}
'@
$code|Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$inputSourcePath=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Input/FightGamepadInput.cs'))
$cid=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightCID.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$inputSourcePath"/><Compile Include="$cid"/></ItemGroup></Project>
"@|Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Local versus input regression failed.'}
