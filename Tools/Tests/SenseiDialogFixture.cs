using System;
using System.Collections.Generic;
using System.Linq;
using Eclipse.Modding;

// Controlled stand-in for the native StoryDialog host behind sf2.ui.story_dialog.
// It records the typed request the production binding builds, resolves its
// native localization keys through the real catalog, and reports OK/Back as
// acknowledgement and scene/profile teardown as cancellation. It does not render.
sealed class FakeDialogPart
{
    public string Text;
    public AssetId? Sprite;
    public bool Mirrored;
}

sealed class FakeDialog : IDisposable
{
    private readonly FakeDialogHost host;
    private Action<bool> done;
    public ModStoryDialogRequest Request { get; }
    public string Id { get; }
    public bool IsClosed { get; private set; }

    internal FakeDialog(FakeDialogHost host, ModStoryDialogRequest request, Action<bool> done, string id)
    { this.host = host; Request = request; this.done = done; Id = id; }

    public bool TryClick(string node)
    {
        if (node != "continue" || IsClosed || host.Blocked) return false;
        End(true); return true;
    }

    public bool Back()
    {
        if (IsClosed || host.Blocked || Request.IgnoreBack) return false;
        End(true); return true;
    }

    public void Close(ModUiCloseReason reason) { if (!IsClosed) End(false); }
    public void Dispose() { Close(ModUiCloseReason.Scene); }

    private void End(bool acknowledged)
    {
        IsClosed = true;
        var notify = done; done = null;
        notify?.Invoke(acknowledged);
    }

    public FakeDialogPart Read(string node)
    {
        switch (node)
        {
            case "speaker": return new FakeDialogPart { Text = host.Resolve(Request.Title) };
            case "body": return new FakeDialogPart { Text = host.Resolve(Request.Lines[0].Text) };
            case "continue": return new FakeDialogPart { Text = host.Resolve(Request.Button) };
            case "portrait": return new FakeDialogPart { Sprite = AssetId.Parse(Request.Portrait), Mirrored = Request.Mirrored };
            default: throw new ArgumentException("Unknown story dialog part: " + node);
        }
    }
}

sealed class FakeDialogHost
{
    private readonly ModContentCatalog catalog;
    public readonly List<FakeDialog> Views = new List<FakeDialog>();
    public bool Blocked;
    public bool Refuse;
    public Action<FakeDialog> Opened;

    public FakeDialogHost(ModContentCatalog catalog)
    {
        this.catalog = catalog;
        ModStoryDialogAccess.Open = Open;
    }

    private IDisposable Open(ModStoryDialogRequest request, Action<bool> done)
    {
        if (Refuse) return null;
        if (Views.Any(view => !view.IsClosed)) throw new InvalidOperationException("Story dialogs overlapped");
        var dialog = new FakeDialog(this, request, done, Classify(request.Title));
        Views.Add(dialog);
        Opened?.Invoke(dialog);
        return dialog;
    }

    // Module identity comes from the owning text module's localization prefix.
    private static string Classify(string title)
    {
        int slash = title.IndexOf("localization/", StringComparison.Ordinal);
        string local = slash < 0 ? title : title.Substring(slash + "localization/".Length);
        if (local.StartsWith("sensei.entry.")) return "sensei_entry";
        if (local.StartsWith("sensei.victory.")) return "sensei_victory";
        if (local.StartsWith("sensei.defeat.")) return "sensei_defeat";
        if (local.StartsWith("sensei.notify.")) return "sensei_notification";
        return "story_dialog";
    }

    public string Resolve(string key)
    {
        if (DefinitionId.TryParse(key, out var id) && catalog.TryGetLocalization(id, out var definition))
            return definition.GetOrEnglish("eng");
        return key;
    }

    public FakeDialog Live => Views.LastOrDefault(view => !view.IsClosed);
    public bool Back() { var live = Live; return live != null && live.Back(); }
    public void SetBlocked(bool blocked) { Blocked = blocked; }
}
