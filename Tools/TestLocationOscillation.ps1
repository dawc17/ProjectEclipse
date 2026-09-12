# Use production interpolators and ChangingSprite axis setters; no graphics or saves.
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ChangingSprite.cs')
$methods=@('NOHGIBJKJNC','PBDEFHJGBML','HMLBMLMDLOP','INPLHCAAJKP') | ForEach-Object {
    $match=[regex]::Match($source,'(?ms)^\tpublic void '+$_+'\(.*?^\t\}')
    if (!$match.Success) { throw "Native oscillation setter not found: $_" }
    $match.Value
}
$fixture=@'
using System;
using System.Xml;
using System.Globalization;
public class MotionFixture {
 public Interpolator APCJDEEGPNM=new Interpolator(),MCHBNCHNFKE=new Interpolator();
 /* METHODS */
}
public static class MotionTests {
 static int checks;
 static void Check(bool v,string message){checks++;if(!v)throw new Exception(message);}
 static float Number(XmlNode node,string field){return node.Attributes[field]==null?0:float.Parse(node.Attributes[field].Value,CultureInfo.InvariantCulture);}
 static MotionFixture Build(XmlNode curve,bool offset){
  var motion=new MotionFixture();float phase=offset?Number(curve,"Offset"):0;
  // Match Location.ParseSimpleEffect: offsets are applied BEFORE points.
  motion.PBDEFHJGBML(phase);motion.INPLHCAAJKP(phase);
  foreach(XmlNode point in curve.SelectNodes("Point")){
   float period=Number(point,"Period"),value=Number(point,"Value"),ease=Number(point,"Ease");
   Check(period>0,"Fixture curve requires positive periods");
   motion.NOHGIBJKJNC(period,value,ease);motion.HMLBMLMDLOP(period,value,ease);
  }
  return motion;
 }
 public static void Run(string file){
  var doc=new XmlDocument();doc.Load(file);int curves=0,shifted=0;
  foreach(XmlNode curve in doc.SelectNodes("//SimpleEffect/OscillationY")){
   curves++;var motion=Build(curve,true);var reference=Build(curve,false);
   reference.MCHBNCHNFKE.HJGPLENNFCK(Number(curve,"Offset"));
   for(int i=0;i<240;i++){
    motion.APCJDEEGPNM.HJGPLENNFCK(1f/60);motion.MCHBNCHNFKE.HJGPLENNFCK(1f/60);reference.MCHBNCHNFKE.HJGPLENNFCK(1f/60);
    float x=motion.APCJDEEGPNM.OAGPELOHACM(),y=motion.MCHBNCHNFKE.OAGPELOHACM();
    Check(!float.IsNaN(y)&&!float.IsInfinity(y),"Nonfinite native motion");
    Check(Math.Abs(x-y)<0.0001,"Vertical phase differs from horizontal phase");
    Check(Math.Abs(y-reference.MCHBNCHNFKE.OAGPELOHACM())<0.0001,"Offset did not advance the native curve");
   }
   if(Number(curve,"Offset")>0)shifted++;
  }
  Check(curves>0&&shifted>0,"Archive fixture has no shifted oscillations");
  doc.LoadXml("<OscillationY Offset='0.25'><Point Period='1' Value='0' Ease='0'/><Point Period='1' Value='8' Ease='0'/></OscillationY>");
  var a=Build(doc.DocumentElement,true);var b=Build(doc.DocumentElement,false);
  a.MCHBNCHNFKE.HJGPLENNFCK(0);b.MCHBNCHNFKE.HJGPLENNFCK(0);
  Check(Math.Abs(a.MCHBNCHNFKE.OAGPELOHACM()-2)<0.0001&&b.MCHBNCHNFKE.OAGPELOHACM()==0,"Linear phase displacement was lost");
  a.MCHBNCHNFKE.HJGPLENNFCK(2);Check(Math.Abs(a.MCHBNCHNFKE.OAGPELOHACM()-2)<0.0001,"Curve loop changed phase");
  Console.WriteLine("PASS: "+checks+" native oscillation assertions; "+curves+" archived curves, "+shifted+" nonzero offsets.");
 }
}
'@
$fixture=$fixture.Replace('/* METHODS */',($methods -join "`n"))
$temp=Join-Path $root ('Temp/LocationOscillation-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
$fixture | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $temp 'Fixture.cs')
Add-Type -Path @((Join-Path $temp 'Fixture.cs'),(Join-Path $root 'Assets/Scripts/Assembly-CSharp/Interpolator.cs'),(Join-Path $root 'Assets/Scripts/Assembly-CSharp/IntervalSet.cs'))
[MotionTests]::Run((Join-Path $root 'Assets/DExml/locations/arena_new/arena_new_params.xml'))
