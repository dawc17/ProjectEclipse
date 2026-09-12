$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Location.cs')
$start=$source.IndexOf("`t`tmusics.Clear();")
$end=$source.IndexOf("`t`tXmlAttribute cJBEMNNNHDM",$start)
if($start -lt 0 -or $end -le $start){throw 'Location music selection block not found.'}
$block=$source.Substring($start,$end-$start)
$fixture=@'
using System;
using System.Xml;
using System.Collections.Generic;
public static class NativeMusicExtensions {
 public static string CIPOICEEIBK(this XmlAttribute value){return value==null?"":value.Value;}
}
public static class NativeMusicFixture {
 public sealed class Entry {public string MusicAsset;}
 public static List<string> Select(bool hasExternalLocation,string single,string list,string PINIIFIOECE){
  var musics=new List<string>{"stale"};var externalLocation=new Entry{MusicAsset=single};
  var xmlDocument=new XmlDocument();xmlDocument.LoadXml("<Root/>");xmlDocument.DocumentElement.SetAttribute("Music",list);
  /* BLOCK */
  return musics;
 }
 public static void Run(){
  if(string.Join("|",Select(true,"","core:audio/a|core:audio/b","fight"))!="core:audio/a|core:audio/b")throw new Exception("Choices lost priority or identity");
  if(string.Join("|",Select(true,"single","","fight"))!="single")throw new Exception("Single track changed");
  if(string.Join("|",Select(true,"","","fight"))!="fight")throw new Exception("External default fallback changed");
  if(string.Join("|",Select(false,"","1|2","fight"))!="fight")throw new Exception("Core fight override changed");
  if(string.Join("|",Select(false,"","1|2",""))!="1|2")throw new Exception("Core random music changed");
  if(string.Join("|",Select(true,"","core:audio/b",""))!="core:audio/b")throw new Exception("Single choice changed");
  Console.WriteLine("PASS: 6 production location music selection checks (no audio playback).");
 }
}
'@
Add-Type -TypeDefinition $fixture.Replace('/* BLOCK */',$block)
[NativeMusicFixture]::Run()
