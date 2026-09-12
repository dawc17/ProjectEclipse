using System;
using System.Collections.Generic;
using Eclipse.Modding;

static class Program
{
    static int checks;
    static readonly ModId A = ModId.Parse("example.a");
    static readonly ModId B = ModId.Parse("example.b");
    static readonly ModStoryEvent Purchase = new ModStoryEvent(ModStoryEventKind.Purchase, DefinitionId.Parse("core:items/weapon/test"));
    static readonly ModStoryEvent Enchantment = new ModStoryEvent(ModStoryEventKind.Enchantment, null);
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Throws<T>(Action action, string message) where T : Exception
    { try { action(); } catch (T) { checks++; return; } throw new Exception(message); }
    static void Main()
    {
        var log = new List<string>();
        var bus = new ModStoryEvents((owner, message) => log.Add(owner + ":" + message));
        var scope = bus.CreateScope(A);
        var order = new List<string>();
        var first = scope.Subscribe(ModStoryEventKind.Purchase, e => order.Add("first"));
        scope.Subscribe(ModStoryEventKind.Enchantment, e => order.Add("enchant"));
        Check(!bus.Publish(Purchase) && order.Count == 0, "unbound publication");
        bus.BindProfile();
        Check(bus.Publish(Purchase) && string.Join(",", order) == "first", "kind filter");
        bus.Publish(Enchantment);
        Check(string.Join(",", order) == "first,enchant", "enchantment delivery");
        first.Dispose(); first.Dispose();
        Check(!first.IsActive, "idempotent cancel");
        bus.Publish(Purchase);
        Check(order.Count == 2, "no canceled delivery");
        scope.Dispose(); scope.Dispose();
        Throws<ObjectDisposedException>(() => scope.Subscribe(ModStoryEventKind.Purchase, e => {}), "disposed scope");

        scope = bus.CreateScope(A);
        order.Clear();
        ModStorySubscription later = null;
        bool added = false;
        scope.Subscribe(ModStoryEventKind.Purchase, e => {
            order.Add("a"); later.Dispose();
            if (!added) { added = true; scope.Subscribe(ModStoryEventKind.Purchase, n => order.Add("new")); }
        });
        later = scope.Subscribe(ModStoryEventKind.Purchase, e => order.Add("removed"));
        bus.Publish(Purchase);
        Check(string.Join(",", order) == "a", "add and remove within dispatch");
        bus.Publish(Purchase);
        Check(string.Join(",", order) == "a,a,new", "new subscriber next event");
        scope.Dispose();

        scope = bus.CreateScope(A);
        order.Clear();
        scope.Subscribe(ModStoryEventKind.Purchase, e => { order.Add("p1"); bus.Publish(Enchantment); order.Add("p2"); });
        scope.Subscribe(ModStoryEventKind.Purchase, e => order.Add("p3"));
        scope.Subscribe(ModStoryEventKind.Enchantment, e => order.Add("e"));
        bus.Publish(Purchase);
        Check(string.Join(",", order) == "p1,p2,p3,e", "nested FIFO without recursion");
        scope.Dispose();

        scope = bus.CreateScope(A);
        int good = 0;
        var failed = scope.Subscribe(ModStoryEventKind.Purchase, e => { throw new Exception("broken"); });
        scope.Subscribe(ModStoryEventKind.Purchase, e => good++);
        bus.Publish(Purchase); bus.Publish(Purchase);
        Check(!failed.IsActive && good == 2, "isolated failure cancels only failed handler");
        Check(log.Count == 1 && log[0].Contains("example.a") && log[0].Contains("broken"), "owner failure diagnostic");
        scope.Dispose();

        scope = bus.CreateScope(A);
        bool switchProfile = true;
        good = 0;
        scope.Subscribe(ModStoryEventKind.Purchase, e => {
            if (!switchProfile) return;
            switchProfile = false;
            bus.Publish(Enchantment);
            bus.UnbindProfile(); bus.BindProfile();
        });
        scope.Subscribe(ModStoryEventKind.Purchase, e => good++);
        scope.Subscribe(ModStoryEventKind.Enchantment, e => good += 100);
        bus.Publish(Purchase);
        Check(good == 0, "profile switch drops queued and remaining old-profile callbacks");
        bus.Publish(Purchase);
        Check(good == 1, "subscriptions survive profile switch");
        scope.Dispose();
        scope = bus.CreateScope(A);
        good = 0;
        scope.Subscribe(ModStoryEventKind.Purchase, e => {
            bus.Publish(Enchantment);
            bus.BindProfile();
            bus.Publish(Enchantment);
        });
        scope.Subscribe(ModStoryEventKind.Purchase, e => good += 100);
        scope.Subscribe(ModStoryEventKind.Enchantment, e => good++);
        bus.Publish(Purchase);
        Check(good == 1, "new-profile notification survives while old-profile queue and remainder are discarded");
        bus.Clear();
        Throws<ObjectDisposedException>(() => scope.Subscribe(ModStoryEventKind.Purchase, e => {}), "clear invalidates old scopes");
        Check(!bus.Publish(Purchase), "clear unbinds");

        scope = bus.CreateScope(A);
        var secondScope = bus.CreateScope(A);
        for (int i = 0; i < ModStoryEvents.MaximumSubscriptionsPerMod; i++) scope.Subscribe(ModStoryEventKind.Purchase, e => {});
        Throws<InvalidOperationException>(() => secondScope.Subscribe(ModStoryEventKind.Enchantment, e => {}), "owner limit across scopes and kinds");
        scope.Dispose();
        Check(secondScope.Subscribe(ModStoryEventKind.Purchase, e => {}).IsActive, "capacity reclaimed on dispose");
        bus.Clear();
        for (int i = 0; i < 4; i++) {
            var s = bus.CreateScope(ModId.Parse("example.owner" + i));
            for (int j = 0; j < 64; j++) s.Subscribe(ModStoryEventKind.Purchase, e => {});
        }
        Throws<InvalidOperationException>(() => bus.CreateScope(B).Subscribe(ModStoryEventKind.Purchase, e => {}), "global capacity");
        bus.Clear(); bus.BindProfile();

        scope = bus.CreateScope(A);
        int calls = 0, rejected = 0;
        scope.Subscribe(ModStoryEventKind.Purchase, e => { calls++; if (!bus.Publish(Purchase)) rejected++; });
        bus.Publish(Purchase);
        Check(calls == ModStoryEvents.MaximumEventsPerDispatch && rejected == 1, "bounded self publication");
        calls = 0;
        bus.Publish(Purchase);
        Check(calls == ModStoryEvents.MaximumEventsPerDispatch, "next root gets fresh budget");
        scope.Dispose();
        scope = bus.CreateScope(A);
        calls = 0;
        scope.Subscribe(ModStoryEventKind.Purchase, e => bus.Publish(Purchase));
        for (int i = 0; i < 63; i++) scope.Subscribe(ModStoryEventKind.Purchase, e => calls++);
        bus.Publish(Purchase);
        Check(calls == 1008, "callback budget counts every invocation including publisher");
        Check(log.Exists(x => x.Contains("callback dispatch limit")), "callback overflow diagnostic");
        scope.Dispose();

        scope = bus.CreateScope(A);
        good = 0;
        scope.Subscribe(ModStoryEventKind.Purchase, e => scope.Dispose());
        scope.Subscribe(ModStoryEventKind.Purchase, e => good++);
        var foreignScope = bus.CreateScope(B);
        foreignScope.Subscribe(ModStoryEventKind.Purchase, e => good += 10);
        bus.Publish(Purchase);
        Check(good == 10, "scope disposal inside callback cancels own remainder, preserves other owner");
        foreignScope.Dispose();

        var throwingLogger = new ModStoryEvents((id, message) => { throw new Exception("logger"); });
        var s2 = throwingLogger.CreateScope(B);
        s2.Subscribe(ModStoryEventKind.Purchase, e => { throw new Exception("script"); });
        throwingLogger.BindProfile();
        Check(throwingLogger.Publish(Purchase), "logging errors cannot fail native publication");
        Throws<ArgumentException>(() => bus.CreateScope(default), "invalid owner");
        Throws<ArgumentNullException>(() => bus.Publish(null), "null notification");
        Throws<ArgumentOutOfRangeException>(() => new ModStoryEvent((ModStoryEventKind)999, null), "invalid kind");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.Purchase, DefinitionId.Parse("core:quests/test")), "invalid item category");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.Purchase, null, DefinitionId.Parse("example.a:forge-recipes/test")), "purchase cannot have recipe");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.LevelUp, null), "missing level pair");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.LevelUp, null, null, 0, 1), "zero previous level");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.LevelUp, null, null, 2, 2), "unchanged level");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.LevelUp, null, null, 3, 2), "decreasing level");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.LevelUp, Purchase.Item, null, 1, 2), "level event item rejected");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.Purchase, null, null, 1, 2), "purchase level fields rejected");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.SceneEnter, null), "missing destination");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene:"loader"), "unsupported destination");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.SceneEnter, Purchase.Item, scene:"map"), "scene item rejected");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.SceneEnter, null, null, 1, 2, "map"), "scene level fields rejected");
        Throws<ArgumentException>(() => new ModStoryEvent(ModStoryEventKind.Purchase, null, scene:"map"), "purchase destination rejected");
        var results=new ModStoryEvents();
        Check(results.BeginEncounter()==null,"unbound encounter creation");
        Check(!results.TryBeginEncounterResult(null)&&!results.TryCompleteEncounterResult(null),"null encounter accepted");
        results.BindProfile();var attempt=results.BeginEncounter();
        Check(!results.TryCompleteEncounterResult(attempt),"unreserved result completed");
        Check(results.TryBeginEncounterResult(attempt),"result reservation failed");
        Check(!results.TryBeginEncounterResult(attempt),"reentrant result accepted");
        Check(results.TryCompleteEncounterResult(attempt),"reserved result not completed");
        Check(!results.TryCompleteEncounterResult(attempt)&&!results.TryBeginEncounterResult(attempt),"duplicate result accepted");
        var previous=results.BeginEncounter();var next=results.BeginEncounter();
        Check(!results.TryBeginEncounterResult(previous),"superseded attempt accepted");
        results.CancelEncounter(previous);
        Check(results.TryBeginEncounterResult(next),"old cancellation erased new encounter");
        results.UnbindProfile();results.BindProfile();
        Check(!results.TryCompleteEncounterResult(next),"result crossed profile boundary");
        attempt=results.BeginEncounter();results.TryBeginEncounterResult(attempt);results.CancelEncounter(attempt);
        Check(!results.TryCompleteEncounterResult(attempt)&&!results.TryBeginEncounterResult(attempt),"failed attempt retried");
        attempt=results.BeginEncounter();var other=new ModStoryEvents();other.BindProfile();
        Check(!other.TryBeginEncounterResult(attempt),"foreign service accepted token");
        Check(results.TryBeginEncounterResult(attempt),"foreign operation consumed token");
        results.Clear();Check(!results.TryCompleteEncounterResult(attempt),"clear retained attempt");
        results.BindProfile();attempt=results.BeginEncounter();
        Check(results.TryBeginEncounterResult(attempt)&&results.TryCompleteEncounterResult(attempt),"new profile encounter unavailable");
        var deferred=new ModStoryEvents();deferred.BindProfile();var delivered=new List<ModStoryEventKind>();
        var deferredScope=deferred.CreateScope(A);
        deferredScope.Subscribe(ModStoryEventKind.Purchase,e=>delivered.Add(e.Kind));
        deferredScope.Subscribe(ModStoryEventKind.Enchantment,e=>delivered.Add(e.Kind));
        deferred.RunDeferred(()=>{
            deferred.Publish(Purchase);
            deferred.RunDeferred(()=>deferred.Publish(Enchantment));
            Check(delivered.Count==0,"nested deferral flushed early");
            deferred.Publish(Purchase);
        });
        Check(string.Join(",",delivered)=="Purchase,Enchantment,Purchase","deferred FIFO order");
        delivered.Clear();
        Throws<InvalidOperationException>(()=>deferred.RunDeferred(()=>{deferred.Publish(Purchase);throw new InvalidOperationException();}),"failed deferral swallowed exception");
        Check(delivered.Count==0,"failed deferral delivered");
        deferred.RunDeferred(()=>{
            deferred.Publish(Purchase);
            try{deferred.RunDeferred(()=>{deferred.Publish(Enchantment);throw new InvalidOperationException();});}catch(InvalidOperationException){}
            deferred.Publish(Purchase);
        });
        Check(string.Join(",",delivered)=="Purchase,Purchase","failed child discarded parent notifications");
        delivered.Clear();
        deferred.RunDeferred(()=>{deferred.Publish(Purchase);deferred.BindProfile();deferred.Publish(Enchantment);});
        Check(string.Join(",",delivered)=="Enchantment","deferral crossed profile generation");
        delivered.Clear();int accepted=0;
        deferred.RunDeferred(()=>{for(int i=0;i<ModStoryEvents.MaximumEventsPerDispatch+5;i++)if(deferred.Publish(Purchase))accepted++;});
        Check(accepted==ModStoryEvents.MaximumEventsPerDispatch&&delivered.Count==accepted,"deferred event budget reset per publication");
        delivered.Clear();deferred.Publish(Enchantment);Check(delivered.Count==1,"deferral retained budget across independent dispatches");
        Throws<ArgumentNullException>(()=>deferred.RunDeferred(null),"null deferred action accepted");
        Console.WriteLine("PASS: " + checks + " story event transport checks. No Lua or native event delivery claimed.");
    }
}
