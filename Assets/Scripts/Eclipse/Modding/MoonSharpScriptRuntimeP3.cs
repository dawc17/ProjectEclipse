using System.Collections.Generic;
using MoonSharp.Interpreter;
namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly Dictionary<Table,DefinitionId> _counterHandles=new Dictionary<Table,DefinitionId>();
            private void AddP3Modules(Table root)
            {
                var counters=new Table(_script);
                counters.Set("register",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.counters.register",()=>{
                    const string f="sf2.counters.register";
                    var t=args.AsType(0,f,DataType.Table,false).Table;
                    ValidateFields(t,f,"id","maximum");
                    return NewHandle(_counterHandles,_api.RegisterCounter(RequiredString(t,"id",f),OptionalInt(t,"maximum",1000000000,f)).Id);
                })));
                counters.Set("get",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.counters.get",()=>{
                    var t=args.AsType(0,"sf2.counters.get",DataType.Table,false).Table;
                    if(!_counterHandles.TryGetValue(t,out var id)) throw new ModContentException("Expected counter handle.");
                    return DynValue.NewNumber(_api.ReadCounter(id));
                })));
                counters.Set("add",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.counters.add",()=>{
                    var t=args.AsType(0,"sf2.counters.add",DataType.Table,false).Table;
                    if(!_counterHandles.TryGetValue(t,out var id)) throw new ModContentException("Expected counter handle.");
                    double n=args.AsType(1,"sf2.counters.add",DataType.Number,false).Number;
                    if(double.IsNaN(n)||n<0||n>1000000000||n!=System.Math.Truncate(n)) throw new ModContentException("Counter increment must be a bounded nonnegative integer.");
                    return DynValue.NewNumber(_api.AdvanceCounter(id,(int)n));
                })));
                root.Set("counters",DynValue.NewTable(counters));
                var achievements=new Table(_script);
                achievements.Set("register",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.achievements.register",()=>{
                    const string f="sf2.achievements.register";
                    var t=args.AsType(0,f,DataType.Table,false).Table;
                    ValidateFields(t,f,"id","counter","title","description","icon","threshold","hidden");
                    var counter=RequiredHandle(t,"counter",_counterHandles,"counter",f);
                    var title=RequiredHandle(t,"title",_localizationHandles,"localization",f);
                    var description=RequiredHandle(t,"description",_localizationHandles,"localization",f);
                    var icon=RequiredHandle(t,"icon",_spriteHandles,"sprite",f);
                    _api.RegisterAchievement(RequiredString(t,"id",f),counter,title,description,icon,RequiredInt(t,"threshold",f),OptionalBool(t,"hidden",false,f));
                    return DynValue.Nil;
                })));
                root.Set("achievements",DynValue.NewTable(achievements));
                root.Get("assets").Table.Set("replace",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.assets.replace",()=>{
                    const string f="sf2.assets.replace";
                    var t=args.AsType(0,f,DataType.Table,false).Table;
                    ValidateFields(t,f,"target","replacement");
                    _api.ReplaceAsset(RequiredString(t,"target",f),RequiredString(t,"replacement",f));
                    return DynValue.Nil;
                })));
            }
        }
    }
}
