using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks; static string fixture, repo;
    static void Check(bool value,string message) { checks++; if(!value)throw new Exception(message); }
    sealed class Voice : IModAudioVoice
    {
        public bool Active=true;public double Volume;public int Stops;
        public bool IsActive=>Active;
        public void SetVolume(double value){Volume=value;}
        public void Dispose(){Stops++;Active=false;}
    }
    sealed class Backend : IModAudioBackend
    {
        public readonly List<Voice> Voices=new List<Voice>(); public readonly List<ModAudioOptions> Options=new List<ModAudioOptions>();
        public bool Reject,Throws;
        public bool TryPlay(AssetId audio,ModAudioOptions options,out IModAudioVoice voice,out string error)
        {
            voice=null;error=null;if(Throws)throw new Exception("decode failed");if(Reject){error="global limit";return false;}
            var v=new Voice{Volume=options.Volume};Voices.Add(v);Options.Add(options);voice=v;return true;
        }
    }
    sealed class Loaded : IDisposable
    {
        public IModScriptContext Context; public Backend Backend; public List<ModUiSurface> Surfaces; public ModContentCatalog Content;
        public void Dispose()=>Context.Dispose();
    }
    static Loaded Load(string body,string caps="content.register,audio.play,ui.create",bool backend=true,string entry="",bool freeze=true)
    {
        string parent=Path.Combine(fixture,Guid.NewGuid().ToString("N")),folder=Path.Combine(parent,"fixture.audio");Directory.CreateDirectory(Path.Combine(folder,"scripts"));Directory.CreateDirectory(Path.Combine(folder,"assets/audio"));
        // Metadata fixture only. Full-game acceptance uses the real shipped WAV.
        File.WriteAllBytes(Path.Combine(folder,"assets/audio/test.wav"),new byte[]{1,2,3});
        File.WriteAllText(Path.Combine(folder,"mod.toml"),"schema=1\nid=\"fixture.audio\"\nname=\"Audio\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=["+string.Join(",",caps.Split(',').Select(c=>"\""+c+"\""))+"]\n");
        File.WriteAllText(Path.Combine(folder,"scripts/main.lua"),"local sf2=require('sf2');local clip=sf2.assets.audio('audio/test');local sound,hud;local calls=0;"+entry+";sf2.behaviors.register{id='test',on_round_begin=function() calls=calls+1;"+body+" end};");
        var discovery=ModDiscovery.DiscoverLoose(parent);Check(discovery.Diagnostics.Count==0,"Manifest invalid");var mod=discovery.Mods.Single();var content=new ModContentCatalog();using var tx=content.BeginRegistration(mod);var b=new Backend();var surfaces=new List<ModUiSurface>();
        var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        var context=new MoonSharpScriptRuntime(surfaces.Add,null,null,null,backend?()=>b:null).CreateContext(mod,api);
        try {context.ExecuteEntrypoint();tx.Commit();if(freeze)content.Freeze();}catch{context.Dispose();throw;}
        return new Loaded{Context=context,Backend=b,Surfaces=surfaces,Content=content};
    }
    static bool Invoke(Loaded loaded,out string error)=>((IModBehaviorScriptContext)loaded.Context).TryInvokeBehavior(DefinitionId.Parse("fixture.audio:behaviors/test"),ModEffectEvent.RoundBegin,null,null,out error);
    static void Event(Loaded loaded){Check(Invoke(loaded,out var error),error);}
    static void Main(string[] args)
    {
        fixture=args[0];repo=args[1];
        using(var l=Load("if calls==1 then local e;sound,e=sf2.audio.play(clip);assert(sound and e==nil and sf2.audio.is_playing(sound));assert(sf2.audio.set_volume(sound,.25)) else assert(sf2.audio.stop(sound));assert(not sf2.audio.is_playing(sound));assert(not sf2.audio.stop(sound));assert(not sf2.audio.set_volume(sound,.5)) end"))
        {Event(l);Check(l.Backend.Options[0].Volume==1&&!l.Backend.Options[0].Loop&&l.Backend.Options[0].Clock==ModAudioClock.Game,"Defaults");Check(l.Backend.Voices[0].Volume==.25,"Volume update");Event(l);Check(l.Backend.Voices[0].Stops==1,"Stop idempotency");}
        using(var l=Load("sound=assert(sf2.audio.play(clip,{volume=.6,loop=true,clock='real'}))"))
        {Event(l);Check(l.Backend.Options[0].Loop&&l.Backend.Options[0].Clock==ModAudioClock.Real,"Options");l.Dispose();Check(l.Backend.Voices[0].Stops==1,"Shutdown");}
        using(var l=Load("if calls==1 then hud=sf2.ui.open{id='owner',mount='hud',root={id='root',kind='text',text='Audio',width=200,height=40}};sound=assert(sf2.audio.play(clip,{owner=hud,loop=true})) else assert(not sf2.audio.is_playing(sound));local s,e=sf2.audio.play(clip,{owner=hud});assert(s==nil and e:find('closed')) end"))
        {Event(l);l.Surfaces[0].Close();Check(!l.Backend.Voices[0].Active,"Owner cleanup");Event(l);Check(l.Backend.Voices.Count==1,"Closed owner allocated voice");}
        using(var l=Load("if calls==1 then sound=assert(sf2.audio.play(clip)) else assert(not sf2.audio.is_playing(sound));assert(not sf2.audio.stop(sound));assert(sf2.audio.play(clip)) end"))
        {Event(l);l.Backend.Voices[0].Active=false;Event(l);Check(l.Backend.Voices[0].Stops==1,"Completed scope cleanup");}
        using(var l=Load("local voices={};for i=1,16 do voices[i]=assert(sf2.audio.play(clip,{loop=true})) end;local v,e=sf2.audio.play(clip);assert(v==nil and e:find('16'));assert(sf2.audio.stop(voices[3]));assert(sf2.audio.play(clip))"))
        {Event(l);Check(l.Backend.Voices.Count==17,"Local limit or freed slot");}
        foreach(string bad in new[]{"sf2.audio.play()","sf2.audio.play({})","sf2.audio.play('audio/test')","sf2.audio.play(clip,{},true)","sf2.audio.play(clip,2)","sf2.audio.play(clip,{volume='1'})","sf2.audio.play(clip,{volume=0/0})","sf2.audio.play(clip,{volume=2})","sf2.audio.play(clip,{volume=-1})","sf2.audio.play(clip,{loop=1})","sf2.audio.play(clip,{clock='sim'})","sf2.audio.play(clip,{clock=1})","sf2.audio.play(clip,{owner={}})","sf2.audio.play(clip,{pitch=2})","sf2.audio.stop({})","sf2.audio.is_playing({})","sf2.audio.set_volume({},.5)"})
        using(var l=Load(bad)){Check(!Invoke(l,out var error)&&!string.IsNullOrEmpty(error),"Invalid API accepted: "+bad);Check(l.Backend.Voices.Count==0,"Invalid API allocated");}
        foreach(string bad in new[]{"sf2.audio.stop(sound,true)","sf2.audio.is_playing(sound,true)","sf2.audio.set_volume(sound)","sf2.audio.set_volume(sound,1,true)","sf2.audio.set_volume(sound,'1')","sf2.audio.set_volume(sound,math.huge)"})
        using(var l=Load("sound=assert(sf2.audio.play(clip));"+bad)){Check(!Invoke(l,out var error),"Invalid instance call accepted: "+bad);Check(l.Backend.Voices[0].Active,"Bad update stopped voice");}
        using(var l=Load("sf2.audio.play(clip)","content.register")){Check(!Invoke(l,out var error)&&error.Contains("audio.play"),"Capability");}
        bool rejected=false;try{using var l=Load("",entry:"sf2.audio.play(clip)");}catch(Exception error){rejected=error.Message.Contains("runtime callback");}Check(rejected,"Entrypoint playback accepted");
        using(var l=Load("assert(sf2.audio.play(clip))",freeze:false)){Check(!Invoke(l,out var error)&&error.Contains("registration")&&l.Backend.Voices.Count==0,"Loaded provider played while dependent registration was open");l.Content.Freeze();Event(l);}
        using(var l=Load("local v,e=sf2.audio.play(clip);assert(v==nil and e:find('unavailable'))",backend:false))Event(l);
        using(var l=Load("local v,e=sf2.audio.play(clip);assert(v==nil and e=='global limit')")){l.Backend.Reject=true;Event(l);}
        using(var l=Load("local v,e=sf2.audio.play(clip);assert(v==nil and e:find('decode failed'))")){l.Backend.Throws=true;Event(l);}
        using(var l=Load("hud=sf2.ui.open{id='owner',mount='hud',root={id='root',kind='text',text='Audio',width=200,height=40},on_close=function() sf2.audio.play(clip) end};sf2.ui.close(hud)")){Event(l);Check(l.Backend.Voices.Count==0,"Cleanup restarted audio");}
        // Per-context budgets are independent; a stopped/completed scope does not
        // mutate another mod's retained voices even when the backend is shared.
        var shared=new Backend();using(var a=new ModAudioScope(shared))using(var b=new ModAudioScope(shared))
        {var audio=AssetId.Parse("fixture.audio:audio/test");for(int i=0;i<16;i++){Check(a.TryPlay(audio,new ModAudioOptions(),null,out _,out _),"Scope A");Check(b.TryPlay(audio,new ModAudioOptions(),null,out _,out _),"Scope B");}a.Dispose();Check(shared.Voices.Take(32).Where((v,i)=>i%2==1).All(v=>v.Active),"Cross-scope shutdown");}
        Shipped();
        Console.WriteLine("Audio runtime PASS: "+checks+" checks; production Lua/ownership/arguments/capabilities/lifetimes/budgets with controlled audio backend.");
    }
    static void Shipped()
    {
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(repo,"Mods")).Mods.Single(m=>m.Id.Value=="example.audio-lab");
        Check(mod.Manifest.Dependencies.Single().Version.Contains(SemanticVersion.Parse("1.0.0")),"Shipped dependency range invalid");
        var content=new ModContentCatalog();var stages=new XmlDocument();stages.Load(Path.Combine(repo,"Assets/vanillaXml/stages.xml"));CoreContentImporter.ImportStages(content,stages.SelectSingleNode("Stages/Zones"));
        using var tx=content.BeginRegistration(mod);var backend=new Backend();var surfaces=new List<ModUiSurface>();var api=new ModApiFacade(mod,new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)}),tx,new ModStateRuntime(),null);
        using var context=new MoonSharpScriptRuntime(surfaces.Add,null,null,null,()=>backend).CreateContext(mod,api);context.ExecuteEntrypoint();tx.Commit();content.Freeze();
        void Event(ModEffectEvent kind){Check(((IModBehaviorScriptContext)context).TryInvokeBehavior(content.FightRules.Single().Behavior,kind,null,null,out var error),error);}
        Event(ModEffectEvent.RoundBegin);var hud=surfaces.Single();Check(hud.TryClick("game")&&backend.Voices.Single().Active&&backend.Options.Single().Clock==ModAudioClock.Game,"Actual game button");
        Check(hud.TryClick("quiet")&&backend.Voices[0].Volume==.25,"Actual quiet button");Check(hud.TryClick("real")&&!backend.Voices[0].Active&&backend.Voices[1].Active&&backend.Options[1].Clock==ModAudioClock.Real,"Actual real button replacement");
        Check(hud.TryClick("stop")&&!backend.Voices[1].Active,"Actual stop button");Check(hud.TryClick("game"),"Restart button");Event(ModEffectEvent.RoundEnd);Check(hud.IsClosed&&!backend.Voices[2].Active,"Round end audio cleanup");
        Event(ModEffectEvent.RoundBegin);var next=surfaces.Last();Check(next!=hud&&next.TryClick("real"),"New-round HUD");Event(ModEffectEvent.FightEnd);Check(next.IsClosed&&!backend.Voices.Last().Active,"Fight end audio cleanup");
    }
}
