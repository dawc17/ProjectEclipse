using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

static class Program
{
    static int checks;
    static string root;
    static void Check(bool condition, string reason) { checks++; if (!condition) throw new Exception(reason); }
    static ModParameterDefinition Integer(string name, long seed) =>
        new ModParameterDefinition(name, ModParameterType.Integer, true, ModParameterValue.FromInteger(seed));

    sealed class Fixture : IDisposable
    {
        public readonly ModDescriptor Mod;
        public readonly ModStateRuntime State = new ModStateRuntime();
        public readonly ModContentCatalog Catalog = new ModContentCatalog();
        public readonly ModApiFacade Api;
        public readonly IModScriptContext Script;
        public XmlDocument Save = new XmlDocument();
        public ModUiSurface View;
        public string LastLog;
        public Fixture(string code = "", string capabilities = "\"state.read\", \"state.write\"", bool bind = true, string id = "random.fixture")
        {
            string directory = Path.Combine(root, Guid.NewGuid().ToString("N"));
            string folder = Path.Combine(directory, id);
            Directory.CreateDirectory(Path.Combine(folder,"scripts"));
            File.WriteAllText(Path.Combine(folder,"mod.toml"),
                "schema=1\nid=\""+id+"\"\nname=\"Random fixture\"\nversion=\"1.0.0\"\napi=\">=0.20 <1.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[\"ui.create\"" +
                (capabilities.Length == 0 ? "" : "," + capabilities) + "]\n");
            bool canRegister=capabilities.Contains("state.write");
            File.WriteAllText(Path.Combine(folder,"scripts/main.lua"), "local sf2=require('sf2')\n" +
                (canRegister ? @"sf2.state.register {version=1,fields={
route={type='integer',default=12345},other={type='integer',default=12345},wide={type='integer',default=2147483648},
text={type='string',default='12345'},number={type='number',default=12345},optional={type='integer',required=false}}}
" : "") + @"sf2.ui.open {id='probe',mount='menu',root={id='go',kind='button',width=100,height=40,text='Draw'},
on_click=function() " + code + " end}");
            Mod = ModDiscovery.DiscoverLoose(directory).Mods.Single();
            var assets = new AssetResolver(new IAssetProvider[] { new LooseModProvider(Mod) });
            // Direct host setup lets the missing-capability cases reach the draw.
            if(!canRegister) State.RegisterDefinition(Mod, 1, new ModParameterSchema(new[] {
                Integer("route",12345), Integer("other",12345), Integer("wide",2147483648L),
                new ModParameterDefinition("text",ModParameterType.String,true,ModParameterValue.FromString("12345")),
                new ModParameterDefinition("number",ModParameterType.Number,true,ModParameterValue.FromNumber(12345)),
                new ModParameterDefinition("optional",ModParameterType.Integer,false)
            }), null, null);
            var transaction = Catalog.BeginRegistration(Mod);
            Api = new ModApiFacade(Mod, assets, transaction, State, entry => LastLog=entry.Message);
            Script = new MoonSharpScriptRuntime(view => View = view).CreateContext(Mod, Api);
            Script.ExecuteEntrypoint(); transaction.Commit(); transaction.Dispose();
            Save.LoadXml("<Warrior/>");
            ModSaveData.RecordContext(Save.DocumentElement,new[]{Mod},Catalog,State);
            if (bind) Bind();
        }
        public void Bind() => Check(State.Bind(Save.DocumentElement,new[]{Script}).Count == 0,"State bind failed");
        public long Value(string name = "route") { State.TryGetValue(Mod.Id,name,out var value); return value.Integer; }
        public void Seed(long value) => State.SetValues(Mod.Id,new Dictionary<string,ModParameterValue>{{"route",ModParameterValue.FromInteger(value)}});
        public void Reload() { var copy = new XmlDocument(); copy.LoadXml(Save.OuterXml); Save=copy; Bind(); }
        public void Dispose() => Script.Dispose();
    }

    static void Main(string[] args)
    {
        root=args[0];
        // Golden words, independently calculated from the published v1 contract.
        uint[] words={1200724404u,818072533u,996137225u,2397394836u,4079075752u,2274189806u};
        using(var f=new Fixture())
        {
            foreach(uint word in words) {
                Check(f.Api.RandomNumber("route")==word/4294967296.0,"Number sequence changed");
                Check(f.Value("other")==12345,"Unrelated stream advanced");
                f.Reload();
            }
            f.Seed(12345);
            foreach(uint word in words)
                Check(f.Api.RandomInteger("route",int.MinValue,int.MaxValue)==(long)word+int.MinValue,"Full-width integer range overflowed");
            f.Seed(12345);
            Check(f.Api.RandomInteger("route",7,7)==7 && f.Value()==-1640519182,"Single-value range did not advance");
            f.Seed(12345);
            // Range 2^31+1 rejects the 4th and 5th words; golden sixth still rejects,
            // so check the eventual accepted range and multi-step state advancement.
            for(int i=0;i<3;i++)f.Api.RandomInteger("route",int.MinValue,0);
            long prior=f.Value();
            int value=f.Api.RandomInteger("route",int.MinValue,0);
            Check(value<=0 && f.Value()!=unchecked((int)((uint)prior+0x9e3779b9u)),"Biased modulo shortcut accepted a rejected word");
            foreach(long seed in new long[]{0,-1,int.MinValue,int.MaxValue}) {
                f.Seed(seed);double first=f.Api.RandomNumber("route");
                Check(first>=0 && first<1,"Number escaped its range");
                f.Seed(seed);Check(f.Api.RandomNumber("route")==first,"Seed reset is not reproducible");
            }
            f.Seed(12345);string before=f.Save.OuterXml;
            try { f.Api.RandomInteger("route",2,1); throw new Exception("Invalid range accepted"); }
            catch(ModContentException) { Check(f.Save.OuterXml==before,"Invalid range mutated save"); }
        }
        using(var f=new Fixture(@"
assert(sf2.random.integer('route',2,3)==2)
assert(sf2.random.number('route')==818072533/4294967296)
assert(sf2.state.get('route')==1013916587)
assert(sf2.state.get('other')==12345)
sf2.state.set {route=12345}
assert(sf2.random.number('route')==1200724404/4294967296)"))
            Check(f.View.TryClick("go"),"Lua mixed stream calls or state reset failed: "+f.LastLog);
        using(var f=new Fixture("")){
            string before=f.Save.OuterXml;
            long seed=f.Value();
            f.State.Unbind();f.State.Unbind();
            bool rejected=false;
            try{f.Api.RandomNumber("route");}catch(ModContentException){rejected=true;}
            Check(rejected,"Unbound profile still allowed state access");
            Check(f.Save.OuterXml==before,"Unbinding rewrote saved state");
            Check(f.State.TryGetDefinition(f.Mod.Id,out var retained),"Unbinding removed definitions");
            f.Bind();Check(f.Value()==seed,"Rebinding lost saved state");
        }
        foreach(string call in new[]{
            "sf2.random.number('missing')", "sf2.random.number('text')", "sf2.random.number('number')",
            "sf2.random.number('optional')", "sf2.random.number('wide')", "sf2.random.number({})",
            "sf2.random.number(123)", "sf2.random.number()", "sf2.random.number('other.mod:route')",
            "sf2.random.integer('route',2,1)", "sf2.random.integer('route',1.5,4)",
            "sf2.random.integer('route','1',4)", "sf2.random.integer('route',1,'4')",
            "sf2.random.integer('route',1,2147483648)", "sf2.random.integer('route',-2147483649,4)",
            "sf2.random.integer('route',0/0,4)", "sf2.random.integer('route',1,1/0)",
            "sf2.random.integer('route',1)", "sf2.random.integer('route',nil,4)"
        }) using(var f=new Fixture(call)) {
            string before=f.Save.OuterXml;
            Check(!f.View.TryClick("go"),"Invalid Lua draw accepted: "+call);
            Check(f.Save.OuterXml==before,"Invalid Lua draw changed state: "+call);
        }
        foreach(string caps in new[]{"", "\"state.read\"", "\"state.write\""})
        foreach(string call in new[]{"sf2.random.number('route')","sf2.random.integer('route',1,4)"})
        using(var f=new Fixture(call,caps)) {
            string before=f.Save.OuterXml;
            Check(!f.View.TryClick("go") && before==f.Save.OuterXml,"Missing capability advanced stream");
        }
        using(var f=new Fixture("sf2.random.number('route')",bind:false)) {
            string before=f.Save.OuterXml;
            Check(!f.View.TryClick("go") && before==f.Save.OuterXml,"Unbound state advanced stream");
        }
        using(var f=new Fixture("sf2.random.number('route'); error('later failure')"))
            Check(!f.View.TryClick("go") && f.Value()==-1640519182,"Later callback failure unexpectedly rolled back draw");
        using(var a=new Fixture()) using(var b=new Fixture()) {
            a.Api.RandomNumber("route");a.Api.RandomNumber("route");
            Check(b.Value()==12345 && b.Api.RandomNumber("route")==words[0]/4294967296.0,"Independent profile stream was shared");
        }
        using(var a=new Fixture()) using(var b=new Fixture(id:"another.fixture")) {
            b.State.TryGetDefinition(b.Mod.Id,out var definition);
            a.State.RegisterDefinition(b.Mod,1,definition.Fields,null,null);
            ModSaveData.RecordContext(a.Save.DocumentElement,new[]{a.Mod,b.Mod},a.Catalog,a.State);
            Check(a.State.Bind(a.Save.DocumentElement,new[]{a.Script,b.Script}).Count==0,"Shared profile could not bind both mods");
            var otherApi=new ModApiFacade(b.Mod,b.Api.Assets,null,a.State,null);
            a.Api.RandomNumber("route");a.Api.RandomNumber("route");
            Check(otherApi.RandomNumber("route")==words[0]/4294967296.0 && a.Value()==1013916587,"Mods shared a random stream in one profile");
            a.State.TryGetDefinition(a.Mod.Id,out var original);
            string orphan=a.Save.OuterXml;
            a.State.RemoveDefinition(a.Mod.Id);
            Check(a.State.Bind(a.Save.DocumentElement,new[]{b.Script}).Count==0 && a.Save.OuterXml==orphan,"Disable changed orphan random state");
            a.State.RegisterDefinition(a.Mod,1,original.Fields,null,null);
            Check(a.State.Bind(a.Save.DocumentElement,new[]{a.Script,b.Script}).Count==0,"Reinstall could not bind stream");
            Check(a.Api.RandomNumber("route")==words[2]/4294967296.0,"Reinstall reset saved stream");
        }
        using(var f=new Fixture("sf2.random.number('route')")) {
            ((XmlElement)f.Save.SelectSingleNode("Warrior/EclipseMods/Mod/State")).SetAttribute("version","2");
            string future=f.Save.OuterXml;
            Check(f.State.Bind(f.Save.DocumentElement,new[]{f.Script}).Count>0,"Future state unexpectedly bound");
            Check(!f.View.TryClick("go") && f.Save.OuterXml==future,"Draw rewrote future-version state");
        }
        Console.WriteLine("PASS: "+checks+" random stream checks (golden sequence, inclusive ranges, rejection, reload, Lua validation and capabilities).");
    }
}
