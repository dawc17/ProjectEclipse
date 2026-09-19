$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/CallbackWorkerRegression'
New-Item -ItemType Directory -Force $fixture | Out-Null
$source = [IO.File]::ReadAllText((Join-Path $root 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs'))
function Extract-Block([string] $signature) {
    $start = $source.IndexOf($signature)
    if ($start -lt 0) { throw "Missing production block: $signature" }
    $open = $source.IndexOf('{', $start)
    $depth = 1; $end = $open + 1
    while ($depth -gt 0) { if ($source[$end] -eq '{') { $depth++ }; if ($source[$end] -eq '}') { $depth-- }; $end++ }
    return $source.Substring($start, $end - $start)
}
$blocks = @(
    (Extract-Block 'private sealed class CallbackWorker'),
    (Extract-Block 'private DynValue RunBounded(DynValue function, string sourceName, int maxSlices, DynValue[] args)'),
    (Extract-Block 'private CallbackWorker CreateCallbackWorker()')
) -join "`n"
$prefix = @'
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
sealed class Runner {
    private readonly Script _script = new Script(CoreModules.Preset_HardSandbox);
    private readonly Stack<CallbackWorker> _callbackWorkers = new Stack<CallbackWorker>();
    private DynValue _callbackWorkerFactory;
    private bool _disposed = false;
    private const long InstructionSlice = 50000;
    public Script Script => _script;
    public int PoolCount => _callbackWorkers.Count;
    public object IdleWorker => _callbackWorkers.Peek();
    public DynValue Invoke(DynValue fn, params DynValue[] args) => RunBounded(fn, "regression", 4, args);
'@
$suffix = @'
}
static class Program {
 static int checks;
 static void Check(bool ok, string label) { if(!ok) throw new Exception(label); checks++; }
 static void Rejected(Action action, string message) { try { action(); throw new Exception("Expected rejection"); } catch(ScriptRuntimeException e) { Check(e.Message.Contains(message), e.Message); } }
 static void Main() {
  var r=new Runner(); var echo=r.Script.DoString("return function(...) return ... end");
  Check(r.Invoke(echo,DynValue.NewNumber(42)).Number==42,"first call"); var worker=r.IdleWorker;
  var tuple=r.Invoke(echo,DynValue.NewNumber(7),DynValue.Nil,DynValue.NewString("last"),DynValue.Nil);
  Check(tuple.Type==DataType.Tuple && tuple.Tuple.Length==4 && tuple.Tuple[1].IsNil() && tuple.Tuple[2].String=="last" && tuple.Tuple[3].IsNil(),"tuple and trailing nil");
  Check(r.Invoke(echo).IsNil(),"empty return");
  Check(Object.ReferenceEquals(worker,r.IdleWorker),"worker reused");
  r.Script.Globals.Set("nested",DynValue.NewCallback((c,a)=>r.Invoke(echo,DynValue.NewNumber(99))));
  Check(r.Invoke(r.Script.DoString("return function() return nested() end")).Number==99,"nested callback");
  Check(r.PoolCount==2,"nested workers isolated");
  Rejected(()=>r.Invoke(r.Script.DoString("return function() error('expected failure') end")),"expected failure");
  Check(r.PoolCount==1,"failed worker discarded");
  Check(r.Invoke(echo,DynValue.NewNumber(11)).Number==11,"recovery after error");
  r.Script.Globals.Set("badYield",DynValue.NewCallback((c,a)=>DynValue.NewYieldReq(new[]{DynValue.NewNumber(1)})));
  Rejected(()=>r.Invoke(r.Script.DoString("return function() badYield() end")),"Unexpected Lua yield");
  Check(r.PoolCount==0,"unexpected yield discarded");
  Rejected(()=>r.Invoke(r.Script.DoString("return function() while true do end end")),"instruction budget exceeded");
  Check(r.PoolCount==0,"budget exhausted worker discarded");
  Check(r.Invoke(echo,DynValue.NewNumber(12)).Number==12,"recovery after budget exhaustion");
  var fn=r.Script.DoString("return function() return 1 end");
  for(int i=0;i<20;i++) r.Invoke(fn);
  long start=GC.GetAllocatedBytesForCurrentThread();
  for(int i=0;i<1000;i++) r.Invoke(fn);
  long reused=(GC.GetAllocatedBytesForCurrentThread()-start)/1000;
  start=GC.GetAllocatedBytesForCurrentThread();
  for(int i=0;i<100;i++){var co=r.Script.CreateCoroutine(fn).Coroutine;co.AutoYieldCounter=50000;co.Resume();}
  long fresh=(GC.GetAllocatedBytesForCurrentThread()-start)/100;
  Check(reused<16384 && fresh>2000000,"allocation regression");
  Console.WriteLine($"PASS {checks} checks; fresh={fresh} bytes/call; reused={reused} bytes/call; reduction={100.0*(1.0-(double)reused/fresh):F2}%");
 }
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'), $prefix + "`n" + $blocks + "`n" + $suffix)
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content (Join-Path $fixture 'Regression.csproj')
dotnet run --project (Join-Path $fixture 'Regression.csproj') -v:q
if ($LASTEXITCODE -ne 0) { throw 'Callback worker regression failed.' }
