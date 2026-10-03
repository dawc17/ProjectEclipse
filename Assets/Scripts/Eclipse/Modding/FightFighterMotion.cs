using System;
using System.Collections.Generic;
using Eclipse.Modding;

// Native hooks remain in Fight; owned request/lifetime policy stays here.
public partial class Fight
{
    private sealed class PendingFighterMotion
    {
        public int Round, Requests;
        public double X, Y, Z;
        public ModScriptSession Session;
    }
    private readonly Dictionary<Model, PendingFighterMotion> _eclipseFighterMotion =
        new Dictionary<Model, PendingFighterMotion>();

    private bool CanMoveEclipseFighter(Model model) => model != null && GetCurrentFight() == this &&
        !IsLocalVersus && !IsTitleSparring && FightDefinition != null &&
        FightDefinition.get_Type() != BattleType.FightNone && FightDefinition.get_Type() != BattleType.FightPVP &&
        (!get_IsRaidFight() || ModModeRuntime.IsRaid(FightDefinition)) &&
        stageType == StageType.FDBBPEGEGMK.STAGE_FIGHT && !isEndRound && !isGameOver && !isStopFight &&
        !_modelTransitionsClosed && round.processing &&
        !_eclipseFightEndDispatched && _eclipseEndedRound != round.round &&
        (model == GetPlayerModel() || model == GetEnemyModel()) &&
        new EclipseFighterOperations(this, model).Health > 0;

    private bool TryQueueEclipseFighterMotion(Model model, double x, double y, double z, out string error)
    {
        error = null;
        var session = ModRuntime.Scripts;
        if (session == null || !CanMoveEclipseFighter(model) || IsPaused())
        { error = "Fighter motion requires a living main fighter during an active offline round."; return false; }
        if (!ModFighterMotionLimits.IsValid(x, y, z))
        { error = "Displacement must be finite and within -1000..1000 on each axis."; return false; }
        if (x == 0 && y == 0 && z == 0) return true;
        _eclipseFighterMotion.TryGetValue(model, out var pending);
        if (pending != null && (pending.Round != round.round || !ReferenceEquals(pending.Session, session)))
        { _eclipseFighterMotion.Remove(model); pending = null; }
        if (pending != null && pending.Requests >= ModFighterMotionLimits.MaximumRequestsPerStep)
        { error = "This fighter already has 32 nonzero movement requests pending for this step."; return false; }
        double totalX = (pending?.X ?? 0) + x, totalY = (pending?.Y ?? 0) + y, totalZ = (pending?.Z ?? 0) + z;
        if (!ModFighterMotionLimits.IsValid(totalX, totalY, totalZ))
        { error = "Combined fighter displacement exceeds 1000 native units on an axis this step."; return false; }
        if (pending == null)
            _eclipseFighterMotion.Add(model, pending = new PendingFighterMotion { Round = round.round, Session = session });
        pending.X = totalX; pending.Y = totalY; pending.Z = totalZ; pending.Requests++;
        return true;
    }

    private void ApplyEclipseFighterMotion()
    {
        if (_eclipseFighterMotion.Count == 0 || IsPaused()) return;
        // A displacement is additive, not a last-writer absolute-position override.
        // Apply once per body, in participant order, after model/collision/animation
        // processing and before round arbitration. No Lua runs inside this method.
        ApplyEclipseFighterMotion(GetPlayerModel());
        ApplyEclipseFighterMotion(GetEnemyModel());
        _eclipseFighterMotion.Clear();
    }

    private void ApplyEclipseFighterMotion(Model model)
    {
        if (model == null || !_eclipseFighterMotion.TryGetValue(model, out var pending)) return;
        if (pending.Round != round.round || !ReferenceEquals(pending.Session, ModRuntime.Scripts) ||
            !CanMoveEclipseFighter(model)) return;
        if (pending.X == 0 && pending.Y == 0 && pending.Z == 0) return;
        try
        {
            // Native translation updates current/previous rig points, physics and
            // derived geometry. true also shifts running keyframes and buffers,
            // preventing the next frame snapping back. Native constraints and
            // friction can still correct rig positions/velocity during translation.
            model.ShiftModelPosition(new Vector3f((float)pending.X, (float)pending.Y, (float)pending.Z), true);
        }
        catch (Exception exception) { UnityEngine.Debug.LogWarning("[ModMotion] Native fighter displacement failed: " + exception.Message); }
    }

    private void CancelEclipseFighterMotion() => _eclipseFighterMotion.Clear();
}
