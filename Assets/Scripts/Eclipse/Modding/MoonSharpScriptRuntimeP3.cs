using System.Collections.Generic;
using MoonSharp.Interpreter;
namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly Dictionary<Table,DefinitionId> _counterHandles=new Dictionary<Table,DefinitionId>();
            private DefinitionId ProfileReference(DynValue value, Dictionary<Table,DefinitionId> handles, string category, string function)
            {
                if(value.Type==DataType.String) return _api.ValidateProfileReference(value.String,category);
                if(value.Type==DataType.Table && handles.TryGetValue(value.Table,out var id)) return id;
                throw new ModContentException(function+" requires a qualified definition ID or a handle from this mod context.");
            }
            private readonly System.Runtime.CompilerServices.ConditionalWeakTable<Table,ModStorySubscription> _storyHandles =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Table,ModStorySubscription>();
            private void AddP3Modules(Table root)
            {
                var scenes=new Table(_script);
                scenes.Set("open",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.scenes.open",()=>{
                    _api.RequireCapability("presentation.navigate");
                    ThrowIfDisposed();
                    if(_uiCloseDepth!=0)throw new ModContentException("Scene navigation is unavailable during UI cleanup.");
                    string destination=args.AsType(0,"sf2.scenes.open",DataType.String,false).String;
                    if(destination!="map"&&destination!="shop"&&destination!="profile"&&destination!="dojo")
                        throw new ModContentException("Unsupported menu destination: "+destination);
                    if(ModSceneAccess.Open==null)throw new ModContentException("Scene navigation is unavailable in this host.");
                    return DynValue.NewBoolean(ModSceneAccess.Open(destination));
                })));
                root.Set("scenes",DynValue.NewTable(scenes));
                var story = new Table(_script);
                story.Set("on",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.story.on",()=>{
                    const string function="sf2.story.on";
                    _api.RequireCapability("story.events");
                    string name=args.AsType(0,function,DataType.String,false).String;
                    ModStoryEventKind kind;
                    if(name=="purchase")kind=ModStoryEventKind.Purchase;
                    else if(name=="enchantment")kind=ModStoryEventKind.Enchantment;
                    else if(name=="level_up")kind=ModStoryEventKind.LevelUp;
                    else if(name=="scene_enter")kind=ModStoryEventKind.SceneEnter;
                    else if(name=="item_acquired")kind=ModStoryEventKind.ItemAcquired;
                    else if(name=="battle_result")kind=ModStoryEventKind.BattleResult;
                    else throw new ModContentException("Unsupported story event: "+name);
                    var callback=args.AsType(1,function,DataType.Function,false);
                    if(_disposed || _storyScope==null)throw new ModContentException("Story subscriptions are unavailable.");
                    var subscription=_storyScope.Subscribe(kind,notification=>{
                        var value=new Table(_script);
                        value.Set("kind",DynValue.NewString(name));
                        value.Set("item",notification.Item.HasValue?DynValue.NewString(notification.Item.Value.ToString()):DynValue.Nil);
                        value.Set("recipe",notification.Recipe.HasValue?DynValue.NewString(notification.Recipe.Value.ToString()):DynValue.Nil);
                        value.Set("previous_level",notification.PreviousLevel.HasValue?DynValue.NewNumber(notification.PreviousLevel.Value):DynValue.Nil);
                        value.Set("level",notification.Level.HasValue?DynValue.NewNumber(notification.Level.Value):DynValue.Nil);
                        value.Set("previous_count",notification.PreviousCount.HasValue?DynValue.NewNumber(notification.PreviousCount.Value):DynValue.Nil);
                        value.Set("count",notification.Count.HasValue?DynValue.NewNumber(notification.Count.Value):DynValue.Nil);
                        value.Set("scene",notification.Scene!=null?DynValue.NewString(notification.Scene):DynValue.Nil);
                        if(notification.Battle!=null)
                        {
                            var battle=notification.Battle;
                            value.Set("fight",battle.Fight.HasValue?DynValue.NewString(battle.Fight.Value.ToString()):DynValue.Nil);
                            value.Set("outcome",DynValue.NewString(battle.Outcome));
                            value.Set("eclipse",DynValue.NewBoolean(battle.Eclipse));
                            if(battle.Equipment!=null)
                            {
                                var equipment=new Table(_script);
                                for(int i=0;i<battle.Equipment.Count;i++)
                                {
                                    var item=battle.Equipment[i];var entry=new Table(_script);
                                    entry.Set("item",item.Item.HasValue?DynValue.NewString(item.Item.Value.ToString()):DynValue.Nil);
                                    entry.Set("type",item.Type==null?DynValue.Nil:DynValue.NewString(item.Type));
                                    entry.Set("subtype",item.Subtype==null?DynValue.Nil:DynValue.NewString(item.Subtype));
                                    equipment.Set(i+1,DynValue.NewTable(entry));
                                }
                                value.Set("equipment",DynValue.NewTable(equipment));
                            }
                        }
                        RunBounded(callback,Mod.Id+":story/"+name,MaxBehaviorInstructionSlices,new[]{DynValue.NewTable(value)});
                    });
                    var handle=new Table(_script);
                    _storyHandles.Add(handle,subscription);
                    return DynValue.NewTable(handle);
                })));
                story.Set("off",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.story.off",()=>{
                    _api.RequireCapability("story.events");
                    var handle=args.AsType(0,"sf2.story.off",DataType.Table,false).Table;
                    if(!_storyHandles.TryGetValue(handle,out var subscription))throw new ModContentException("Expected an owned story subscription handle.");
                    subscription.Dispose();
                    return DynValue.Nil;
                })));
                story.Set("is_active",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.story.is_active",()=>{
                    _api.RequireCapability("story.events");
                    var handle=args.AsType(0,"sf2.story.is_active",DataType.Table,false).Table;
                    if(!_storyHandles.TryGetValue(handle,out var subscription))throw new ModContentException("Expected an owned story subscription handle.");
                    return DynValue.NewBoolean(subscription.IsActive);
                })));
                root.Set("story",DynValue.NewTable(story));
                var profile=new Table(_script);
                profile.Set("level",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.profile.level",()=>{
                    _api.RequireCapability("profile.read");
                    int? level=ModProfileAccess.Level?.Invoke();
                    if(!level.HasValue) throw new ModContentException("No active game profile is available.");
                    return DynValue.NewNumber(level.Value);
                })));
                profile.Set("item",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.profile.item",()=>{
                    const string function="sf2.profile.item";
                    _api.RequireCapability("profile.read");
                    var id=ProfileReference(args[0],_itemHandles,"items",function);
                    var snapshot=ModProfileAccess.Item?.Invoke(id);
                    if(snapshot==null) throw new ModContentException("No active game profile is available.");
                    var result=new Table(_script);
                    result.Set("present",DynValue.NewBoolean(snapshot.Present));
                    result.Set("owned",DynValue.NewBoolean(snapshot.Owned));
                    result.Set("count",DynValue.NewNumber(snapshot.Count));
                    result.Set("equipped",DynValue.NewBoolean(snapshot.Equipped));
                    result.Set("type",snapshot.Type==null?DynValue.Nil:DynValue.NewString(snapshot.Type));
                    result.Set("subtype",snapshot.Subtype==null?DynValue.Nil:DynValue.NewString(snapshot.Subtype));
                    result.Set("upgrade",snapshot.Upgrade.HasValue?DynValue.NewNumber(snapshot.Upgrade.Value):DynValue.Nil);
                    return DynValue.NewTable(result);
                })));
                root.Set("profile",DynValue.NewTable(profile));
                profile.Set("equipment",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.profile.equipment",()=>{
                    _api.RequireCapability("profile.read");
                    var equipment=ModProfileAccess.Equipment?.Invoke();
                    if(equipment==null)throw new ModContentException("No active game profile is available.");
                    var result=new Table(_script);
                    for(int i=0;i<equipment.Count;i++)
                    {
                        var entry=equipment[i];var state=entry.State;var value=new Table(_script);
                        value.Set("item",entry.Item.HasValue?DynValue.NewString(entry.Item.Value.ToString()):DynValue.Nil);
                        value.Set("count",DynValue.NewNumber(state.Count));
                        value.Set("owned",DynValue.NewBoolean(state.Owned));
                        value.Set("upgrade",state.Upgrade.HasValue?DynValue.NewNumber(state.Upgrade.Value):DynValue.Nil);
                        value.Set("type",state.Type==null?DynValue.Nil:DynValue.NewString(state.Type));
                        value.Set("subtype",state.Subtype==null?DynValue.Nil:DynValue.NewString(state.Subtype));
                        result.Set(i+1,DynValue.NewTable(value));
                    }
                    return DynValue.NewTable(result);
                })));
                profile.Set("perk",DynValue.NewCallback((ctx,args)=>ApiCall("sf2.profile.perk",()=>{
                    const string function="sf2.profile.perk";
                    _api.RequireCapability("profile.read");
                    var id=ProfileReference(args[0],_perkHandles,"perks",function);
                    var snapshot=ModProfileAccess.Perk?.Invoke(id);
                    if(snapshot==null)throw new ModContentException("No active game profile is available.");
                    var result=new Table(_script);
                    result.Set("learned",DynValue.NewBoolean(snapshot.Learned));
                    result.Set("upgrade",snapshot.Upgrade.HasValue?DynValue.NewNumber(snapshot.Upgrade.Value):DynValue.Nil);
                    return DynValue.NewTable(result);
                })));
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
