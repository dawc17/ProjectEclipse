using System;
using System.Collections.Generic;
using Eclipse.Modding;

public partial class Fight
{
    private sealed class PendingFighterPlayback
    {
        public int Round;
        public ModScriptSession Session;
        public string Name;
        public Action<bool, string> Complete;
    }
    private readonly Dictionary<Model, PendingFighterPlayback> _eclipseFighterPlayback =
        new Dictionary<Model, PendingFighterPlayback>();
    private bool _applyingEclipseFighterPlayback;

    private static bool HasEclipseMove(Model model, string name)
    {
        foreach (var animation in model.GetAvailableAnimations())
            if (animation != null && animation.Name == name) return true;
        return false;
    }

    private bool TryQueueEclipseFighterPlayback(Model model, DefinitionId move,
        Action<bool, string> complete, out string error)
    {
        error = null;
        var session = ModRuntime.Scripts;
        // Motion and playback share active-round/main-body eligibility.
        if (session == null || !CanMoveEclipseFighter(model) || IsPaused() || _applyingEclipseFighterPlayback)
        { error = "Move playback requires a living main fighter during an active offline simulation callback."; return false; }
        if (!session.Content.TryGetMove(move, out var definition) || !HasEclipseMove(model, definition.RuntimeName))
        { error = "The registered move is unavailable on this fighter's current equipment/rig."; return false; }
        if (_eclipseFighterPlayback.TryGetValue(model, out var pending))
        {
            if (pending.Round == round.round && ReferenceEquals(pending.Session, session))
            { error = "This fighter already has a move playback request pending for this step."; return false; }
            _eclipseFighterPlayback.Remove(model);
            FinishEclipseFighterPlayback(pending, false, "Move playback was cancelled by a round/session change.");
        }
        _eclipseFighterPlayback.Add(model, new PendingFighterPlayback {
            Round = round.round, Session = session, Name = definition.RuntimeName, Complete = complete });
        return true;
    }

    private void ApplyEclipseFighterPlayback()
    {
        if (_eclipseFighterPlayback.Count == 0 || IsPaused()) return;
        // Detach the batch before native animation work. Requests cannot recurse
        // through the native start/end notifications while this boundary runs.
        var pending = new Dictionary<Model, PendingFighterPlayback>(_eclipseFighterPlayback);
        _eclipseFighterPlayback.Clear();
        _applyingEclipseFighterPlayback = true;
        try
        {
            ApplyEclipseFighterPlayback(GetPlayerModel(), pending);
            ApplyEclipseFighterPlayback(GetEnemyModel(), pending);
            foreach (var item in pending.Values)
                FinishEclipseFighterPlayback(item, false, "Fighter was replaced or detached before move playback.");
        }
        finally { _applyingEclipseFighterPlayback = false; }
    }

    private void ApplyEclipseFighterPlayback(Model model, Dictionary<Model, PendingFighterPlayback> pending)
    {
        if (model == null || !pending.TryGetValue(model, out var request)) return;
        pending.Remove(model);
        if (request.Round != round.round || !ReferenceEquals(request.Session, ModRuntime.Scripts) ||
            !CanMoveEclipseFighter(model) || !HasEclipseMove(model, request.Name))
        { FinishEclipseFighterPlayback(request, false, "Fighter, move, round or script session became unavailable."); return; }
        try
        {
            // Explicit authored playback uses the existing named action path.
            // It deliberately does not simulate input/AI selection conditions.
            bool started = model.PlayAnimation(request.Name);
            FinishEclipseFighterPlayback(request, started, started ? null : "Native model state rejected move playback.");
        }
        catch (Exception error) { FinishEclipseFighterPlayback(request, false, "Native move playback failed: " + error.Message); }
    }

    private static void FinishEclipseFighterPlayback(PendingFighterPlayback request, bool success, string error)
    {
        var complete = request.Complete; request.Complete = null;
        try { complete?.Invoke(success, error); }
        catch (Exception failure) { UnityEngine.Debug.LogWarning("[ModPlayback] Receipt update failed: " + failure.Message); }
    }

    private void CancelEclipseFighterPlayback()
    {
        var pending = new List<PendingFighterPlayback>(_eclipseFighterPlayback.Values);
        _eclipseFighterPlayback.Clear();
        foreach (var request in pending)
            FinishEclipseFighterPlayback(request, false, "Move playback was cancelled by round/fight teardown.");
    }
}
