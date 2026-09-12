using System;
using System.Collections.Generic;
using Eclipse.Modding;

static class Program
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static void Reject(Action action, string message)
    {
        bool failed = false;
        try { action(); } catch (ArgumentException) { failed = true; } catch (InvalidOperationException) { failed = true; }
        Check(failed, message);
    }
    static ModUiNode Root() => new ModUiNode("root", ModUiKind.Column, 360, 220, gap: 8, children: new[] {
        new ModUiNode("title", ModUiKind.Text, 320, 40, text: "Energy"),
        new ModUiNode("meter", ModUiKind.Progress, 320, 20, value: .25),
        new ModUiNode("button", ModUiKind.Button, 160, 40, text: "Activate")
    });
    static void Main()
    {
        var errors = new List<Exception>();
        var scope = new ModUiScope(ModId.Parse("example.ui"), errors.Add);
        var other = new ModUiScope(ModId.Parse("other.ui"));
        int clicks = 0;
        var surface = scope.Open("meter", ModUiMount.CombatHud, Root(), _ => clicks++);
        var color = new ModUiColor("#Ab12Ef80");
        Check(color.R==171 && color.G==18 && color.B==239 && color.A==128,"RGBA color parsing failed");
        Check(new ModUiColor("#abcdef").A==255,"RGB color not opaque");
        Reject(()=>new ModUiColor("red"),"Named color accepted");
        Reject(()=>new ModUiColor("#00ffgg"),"Invalid hex accepted");
        Reject(()=>new ModUiStyle(fontSize:7),"Unreadably small font accepted");
        Reject(()=>new ModUiStyle(textAlign:"justify"),"Invalid alignment accepted");
        Reject(()=>new ModUiNode("bad",ModUiKind.Progress,100,20,style:new ModUiStyle(textColor:"#ffffff")),"Progress accepted text style");
        Reject(()=>new ModUiNode("bad",ModUiKind.Text,100,20,style:new ModUiStyle(backgroundColor:"#ffffff")),"Text accepted background");
        Check(surface.Placement.Anchor == "center" && surface.Placement.X == 0 && surface.Placement.AnchorY == .5, "Default placement changed");
        Reject(() => new ModUiPlacement("unknown"), "Unknown anchor accepted");
        Reject(() => new ModUiPlacement(x: double.NaN), "NaN placement accepted");
        Reject(() => new ModUiPlacement(y: double.PositiveInfinity), "Infinite placement accepted");
        Reject(() => new ModUiPlacement(x: 8193), "Unbounded placement accepted");
        var independent = other.Open("meter", ModUiMount.CombatHud, Root());
        Check(surface.WidgetCount == 4 && scope.Count == 1 && surface.Owner == scope.Owner, "Wrong owner/tree");
        Reject(() => scope.Open("meter", ModUiMount.Modal, Root()), "Duplicate surface accepted");
        Check(scope.Count == 1, "Duplicate mutated scope");
        var before = surface.Read("meter");
        int changed = 0; string changedId = null;
        surface.Changed += id => { changed++; changedId = id; };
        surface.SetValue("meter", .75);
        surface.SetValue("meter", .75);
        Check(changed == 1 && changedId == "meter" && before.Value == .25 && surface.Read("meter").Value == .75, "Updates/snapshots not isolated");
        Check(independent.Read("meter").Value == .25, "Cross-owner mutation");
        Reject(() => surface.SetValue("meter", double.NaN), "NaN accepted");
        Reject(() => surface.SetValue("meter", 1.1), "Out-of-range progress accepted");
        Reject(() => surface.SetValue("title", .5), "Text received progress");
        Reject(() => surface.SetText("meter", "oops"), "Progress received text");
        Reject(() => surface.SetText("title", new string('x', 8193)), "Unbounded text accepted");
        Check(changed == 1 && surface.Read("meter").Value == .75, "Rejected update mutated state");
        surface.SetText("title", "Energy 75%");
        Check(surface.Read("title").Text == "Energy 75%", "Text update failed");
        Check(!surface.TryClick("title") && !surface.TryClick("unknown"), "Non-button dispatched");
        Check(surface.TryClick("button") && clicks == 1, "Button did not dispatch");
        surface.SetVisible("root", false);
        Check(!surface.TryClick("button"), "Hidden ancestor allowed input");
        surface.SetVisible("root", true); surface.SetEnabled("root", false);
        Check(!surface.TryClick("button"), "Disabled ancestor allowed input");
        surface.SetEnabled("root", true); surface.SetEnabled("button", false);
        Check(!surface.TryClick("button"), "Disabled button allowed input");
        surface.SetEnabled("button", true);
        Check(surface.TryClick("button") && clicks == 2, "Button did not recover");
        Reject(() => surface.Read("missing"), "Missing ID accepted");
        int closed = 0;
        surface.Closed += () => { closed++; throw new Exception("renderer teardown failed"); };
        surface.Closed += () => closed++;
        surface.Close(); surface.Close();
        Check(closed == 2 && errors.Count == 1 && scope.Count == 0 && surface.IsClosed && surface.WidgetCount == 0,
            "Close failed to isolate observers/release state");
        Check(!surface.TryClick("button"), "Stale input dispatched");
        Reject(() => surface.SetText("title", "stale"), "Closed surface mutated");
        var reopened = scope.Open("meter", ModUiMount.CombatHud, Root());
        Check(reopened.Read("title").Text == "Energy", "Reopen reused stale state");
        ModUiSurface recursive = null;
        recursive = scope.Open("recursive", ModUiMount.Modal, Root(), _ => {
            Check(!recursive.TryClick("button"), "Callback reentered itself");
            recursive.SetText("title", "Updated inside click");
        });
        Check(recursive.TryClick("button") && recursive.Read("title").Text == "Updated inside click", "Callback update failed");
        var failing = scope.Open("failure", ModUiMount.Menu, Root(), _ => { throw new Exception("Lua callback failed"); });
        Check(!failing.TryClick("button") && failing.IsClosed && errors.Count == 2, "Callback error did not close its surface");
        Check(!reopened.IsClosed && !independent.IsClosed, "Callback error closed another surface");
        var renderer = scope.Open("renderer", ModUiMount.Menu, Root());
        renderer.Changed += _ => { throw new Exception("render update failed"); };
        renderer.SetText("title", "change");
        Check(renderer.IsClosed && errors.Count == 3, "Renderer error retained broken surface");
        ModUiSurface selfClosing = null;
        selfClosing = scope.Open("self-close", ModUiMount.Modal, Root(), _ => selfClosing.Close());
        Check(selfClosing.TryClick("button") && selfClosing.IsClosed, "Close inside click failed");
        var nodes = new List<ModUiNode> { new ModUiNode("child", ModUiKind.Text, 20, 20) };
        var immutable = new ModUiNode("immutable", ModUiKind.Column, 20, 20, children: nodes);
        nodes.Clear(); Check(immutable.Children.Count == 1, "Authored children mutated after construction");
        Reject(() => new ModUiNode("bad/id", ModUiKind.Text, 20, 20), "Invalid widget ID accepted");
        Reject(() => new ModUiNode("bad", ModUiKind.Text, double.PositiveInfinity, 20), "Invalid size accepted");
        Reject(() => new ModUiNode("bad", ModUiKind.Scroll, 20, 20), "Empty scroll accepted");
        Reject(() => new ModUiNode("bad", ModUiKind.Button, 20, 20, children: new[]{Root()}), "Leaf children accepted");
        var duplicate = new ModUiNode("duplicates", ModUiKind.Column, 20, 20, children: new[]{Root(),Root()});
        int validCount = scope.Count;
        Reject(() => scope.Open("bad", ModUiMount.Menu, duplicate), "Duplicate widget IDs accepted");
        Check(scope.Count == validCount, "Invalid tree partially registered");
        var depth = new ModUiNode("leaf", ModUiKind.Text, 20, 20);
        for (int i=0;i<16;i++) depth = new ModUiNode("depth"+i, ModUiKind.Column, 20, 20, children:new[]{depth});
        Reject(() => scope.Open("deep", ModUiMount.Menu, depth), "Deep tree accepted");
        var wide = new List<ModUiNode>();
        for (int i=0;i<256;i++) wide.Add(new ModUiNode("wide"+i, ModUiKind.Text, 20, 20));
        Reject(() => scope.Open("wide", ModUiMount.Menu, new ModUiNode("wideRoot",ModUiKind.Column,20,20,children:wide)), "Oversized tree accepted");
        for (int i=scope.Count;i<8;i++) scope.Open("limit"+i, ModUiMount.Menu, Root());
        Reject(() => scope.Open("ninth", ModUiMount.Menu, Root()), "Surface budget ignored");
        scope.Dispose(); scope.Dispose();
        Check(scope.IsClosed && scope.Count == 0 && reopened.IsClosed && recursive.IsClosed && !independent.IsClosed, "Scope cleanup leaked/crossed owners");
        Reject(() => scope.Open("closed", ModUiMount.Menu, Root()), "Closed scope reopened");
        other.Dispose();
        Layers();
        Controls();
        CloseNotifications();
        Console.WriteLine("PASS: " + checks + " UI ownership, validation, state, input and teardown checks; no Unity renderer or Lua API claimed.");
    }

    static void Controls()
    {
        var gate = new ModUiControlGate<string>();
        Check(gate.Press("punch") && gate.Press("left"), "Normal controls blocked");
        var releases = gate.SetCaptured(true);
        Check(releases.Length == 2 && releases[0]=="punch" && releases[1]=="left",
            "Capture did not release active controls");
        Check(gate.SetCaptured(true).Length == 0, "Repeated capture released twice");
        Check(!gate.Press("punch") && !gate.Press("kick"), "Captured press leaked");
        gate.SetCaptured(false);
        Check(!gate.Press("punch") && !gate.Press("kick"), "Held UI control replayed after close");
        Check(!gate.Release("punch") && !gate.Release("kick"), "Synthetic release repeated after physical release");
        Check(gate.Press("punch") && gate.Release("punch"), "Fresh press failed after neutral");
        Check(gate.Release("untracked"), "Unrelated native release changed");
        Check(gate.Press("right") && gate.Press("right"), "Ordinary duplicate native events changed");
        Check(gate.SetCaptured(true).Length == 1, "Duplicate press caused duplicate synthetic release");
        Check(!gate.Release("left"), "Initially held control lost suppression");
        gate.SetCaptured(false);
        Check(gate.Press("left"), "Released direction remained suppressed");
    }

    static void CloseNotifications()
    {
        foreach(ModUiCloseReason reason in Enum.GetValues(typeof(ModUiCloseReason)))
        using(var scope=new ModUiScope(ModId.Parse("example.close")))
        using(var layers=new ModUiLayerStack())
        {
            int calls=0; bool released=false;
            ModUiSurface view=null;
            view=scope.Open("close",ModUiMount.Menu,Root(),onClose:actual=>{
                calls++;
                Check(actual==reason,"Wrong close reason");
                Check(view.IsClosed && !view.IsMounted && released && scope.Count==0 && !layers.HasExclusiveInput,
                    "Close notification ran before native cleanup/input release");
                view.Close();
            });
            view.Closed+=()=>released=true;layers.Add(view);
            if(reason==ModUiCloseReason.Back)layers.Back();
            else if(reason==ModUiCloseReason.Scene)layers.Dispose();
            else if(reason==ModUiCloseReason.Shutdown)scope.Dispose();
            else view.Close(reason);
            view.Close();Check(calls==1,"Repeated/reentrant close notified more than once");
        }
        var failures=new List<Exception>();
        using(var scope=new ModUiScope(ModId.Parse("example.close"),failures.Add))
        {
            var view=scope.Open("failure",ModUiMount.Menu,Root(),onClose:_=>throw new Exception("handler failure"));
            view.Close();Check(view.IsClosed && scope.Count==0 && failures.Count==1,"Notification error prevented teardown");
            ModUiCloseReason? reason=null;
            var broken=scope.Open("renderer",ModUiMount.Menu,Root(),onClose:value=>reason=value);
            broken.Changed+=_=>throw new Exception("renderer failure");
            broken.SetText("title","changed");Check(reason==ModUiCloseReason.Error,"Renderer failure reported normal close");
        }
    }

    static void Layers()
    {
        using (var a = new ModUiScope(ModId.Parse("example.a")))
        using (var b = new ModUiScope(ModId.Parse("example.b")))
        using (var layers = new ModUiLayerStack())
        {
            int clicks = 0;
            var hud = a.Open("hud", ModUiMount.CombatHud, Root(), _ => clicks++);
            layers.Add(hud);
            Check(layers.Foreground == hud && !layers.HasExclusiveInput && hud.TryClick("button"), "HUD routing");
            var menu = b.Open("menu", ModUiMount.Menu, Root(), _ => clicks++);
            layers.Add(menu);
            Check(layers.Foreground == menu && layers.HasExclusiveInput && !hud.TryClick("button"), "Menu priority");
            var modal = a.Open("modal", ModUiMount.Modal, Root(), _ => clicks++);
            layers.Add(modal);
            Check(layers.Foreground == modal && !menu.TryClick("button") && modal.TryClick("button"), "Modal priority");
            var newerHud = b.Open("hud", ModUiMount.CombatHud, Root(), _ => clicks++);
            layers.Add(newerHud);
            Check(layers.Foreground == modal, "New HUD displaced modal");
            var newerModal = b.Open("modal", ModUiMount.Modal, Root(), _ => clicks++);
            layers.Add(newerModal);
            Check(layers.Foreground == newerModal && !modal.TryClick("button"), "Equal-priority ordering");
            newerModal.SetVisible("root", false);
            Check(layers.Foreground == modal && !newerModal.TryClick("button"), "Hidden root retained input");
            newerModal.SetVisible("root", true);
            Check(layers.Foreground == newerModal, "Visible root did not recover order");
            layers.SetBlocked(true);
            Check(layers.Foreground == null && !layers.HasExclusiveInput && !newerModal.TryClick("button") && !layers.Back(), "Native blocker bypassed");
            layers.SetBlocked(false);
            Check(layers.Foreground == newerModal, "Native unblock did not recover foreground");
            Check(layers.Back() && newerModal.IsClosed && layers.Foreground == modal, "Back closed wrong overlay");
            Reject(() => layers.Add(modal), "Duplicate layer mount accepted");
            using (var secondStack = new ModUiLayerStack())
                Reject(() => secondStack.Add(modal), "Cross-stack surface mount accepted");
            int refreshes = 0; layers.Changed += () => refreshes++;
            modal.SetValue("meter", .5);
            Check(refreshes == 0, "Widget-only update rebuilt layer order");
            a.Dispose();
            Check(layers.Foreground == menu && hud.IsClosed && modal.IsClosed && !menu.IsClosed, "Owner disposal damaged unrelated overlays");
            Check(layers.Back() && layers.Foreground == newerHud && !layers.Back(), "Back closed decorative HUD");
            layers.Dispose();
            Check(newerHud.IsClosed && !b.IsClosed && layers.Foreground == null, "Scene disposal retained UI or disposed script scope");
            var future = b.Open("future", ModUiMount.Menu, Root());
            Reject(() => layers.Add(future), "Disposed scene accepted mount");
            Check(!future.IsClosed, "Rejected mount unexpectedly destroyed another scope's unmounted surface");
        }
    }
}
