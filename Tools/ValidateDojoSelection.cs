using System;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
static class Program
{
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static XmlDocument Profile(string body="",string schema="1") {var d=new XmlDocument();d.LoadXml("<Warrior><EclipseMods schema='"+schema+"' untouched='yes'>"+body+"</EclipseMods></Warrior>");return d;}
 static void Reject(Action action,string message){bool rejected=false;try{action();}catch(ModContentException){rejected=true;}Check(rejected,message);}
 static void Main()
 {
  var a=DefinitionId.Parse("example.garden:locations/garden");
  var b=DefinitionId.Parse("example.snow:locations/snow");
  var runtime=new ModDojoSelection();runtime.SetChoices(new[]{a,b});
  Reject(()=>runtime.Select(a),"Selection allowed without profile");
  var first=Profile("<Unknown value='preserve'/>");string untouched=first.OuterXml;
  runtime.Bind(first.DocumentElement);Check(runtime.Resolve("dojo")=="dojo","Fresh fallback missing");
  Check(first.OuterXml==untouched,"Binding mutated fresh save");
  runtime.Select(a);Check(runtime.Resolve("dojo")==a.ToString(),"Selected location unresolved");
  Check(first.SelectSingleNode("//Unknown")!=null,"Unknown save node lost");
  string saved=first.OuterXml;runtime.SetChoices(new[]{b});
  Check(runtime.Resolve("dojo")=="dojo","Missing mod did not fall back");
  Check(first.OuterXml==saved&&runtime.SavedLocation==a.ToString(),"Missing choice destroyed preference");
  Reject(()=>runtime.Select(a),"Missing choice selectable");Check(first.OuterXml==saved,"Rejected selection mutated save");
  runtime.SetChoices(new[]{a,b});Check(runtime.Resolve("dojo")==a.ToString(),"Reinstalled mod did not restore preference");
  var reloaded=new XmlDocument();reloaded.LoadXml(saved);runtime.Bind(reloaded.DocumentElement);
  Check(runtime.Resolve("dojo")==a.ToString(),"Serialized selection lost on reload");
  Check(ModSaveData.RecordContext(reloaded.DocumentElement,Array.Empty<ModDescriptor>()),"Save context rejected supported preference");
  Check(runtime.Resolve("dojo")==a.ToString()&&reloaded.SelectSingleNode("//DojoSelection/@location").Value==a.ToString(),"Normal save context recording removed preference");
  var second=Profile();runtime.Bind(second.DocumentElement);Check(runtime.Resolve("dojo")=="dojo","Previous profile leaked");
  runtime.Select(b);Check(runtime.Resolve("dojo")==b.ToString(),"Second profile choice missing");
  Check(first.OuterXml==saved,"Second profile changed first profile");
  runtime.Bind(first.DocumentElement);Check(runtime.Resolve("dojo")==a.ToString(),"Profile switch did not restore first choice");
  Reject(()=>runtime.SetChoices(new[]{b,b}),"Duplicate choices accepted");
  Check(runtime.Resolve("dojo")==a.ToString(),"Failed registration replaced valid choices");
  Reject(()=>runtime.SetChoices(new[]{DefinitionId.Parse("example.garden:items/sword")}),"Non-location accepted");
  Reject(()=>runtime.SetChoices(Enumerable.Range(0,257).Select(i=>DefinitionId.Parse("example.garden:locations/a"+i))),"Unbounded choices accepted");
  foreach(var invalid in new[]{Profile("<DojoSelection schema='2' location='future'/>") ,Profile("<DojoSelection schema='1' location='bad'/>") ,Profile("<DojoSelection schema='1'/><DojoSelection schema='1'/>") ,Profile("","2")})
  {
   runtime.Bind(first.DocumentElement);string original=invalid.OuterXml;
   Reject(()=>runtime.Bind(invalid.DocumentElement),"Unknown save accepted");
   Check(!runtime.IsBound&&runtime.Resolve("dojo")=="dojo","Failed binding leaked previous profile");
   Check(invalid.OuterXml==original,"Invalid save was rewritten");
  }
  runtime.Bind(first.DocumentElement);var node=(XmlElement)first.SelectSingleNode("//DojoSelection");node.SetAttribute("future","keep");
  runtime.Reset();Check(runtime.Resolve("dojo")=="dojo"&&runtime.SavedLocation=="","Explicit reset failed");
  Check(node.GetAttribute("future")=="keep","Reset lost unrelated data");
  runtime.Select(a);string beforeClear=first.OuterXml;runtime.Clear();
  Check(!runtime.IsBound&&runtime.Resolve("dojo")=="dojo"&&first.OuterXml==beforeClear,"Shutdown changed saved selection");
  runtime.Bind(first.DocumentElement);Check(runtime.Resolve("dojo")=="dojo","Clear retained active choices");
  Console.WriteLine("PASS: "+checks+" dojo preference save/lifetime checks; full-game scene and persistence acceptance remain pending.");
 }
}
