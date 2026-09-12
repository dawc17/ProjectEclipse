using System;
using UnityEngine;

namespace Eclipse.Modding
{
    public sealed class ModCharacterCondition : ConditionAnimation
    {
        private readonly string character;
        public ModCharacterCondition(string character) : base(DGAGKLODADD.ECLIPSE_CHARACTER) { this.character=character; }
        public override bool IsEqual(ModelConditions conditions)
        {
            bool matches=!string.IsNullOrEmpty(character) && conditions != null && conditions.EclipseCharacterId==character;
            return IsNot ? !matches : matches;
        }
    }

    // Runs a completed mode request after Lua has returned. Scene destruction
    // cancels it, and no coroutine or continuation is serialized into the save.
    public sealed class ModPendingEncounter : MonoBehaviour
    {
        private ModModeRequest request;
        private Action ready, cancel;
        public void Configure(ModModeRequest request, Action ready, Action cancel)
        { this.request=request; this.ready=ready; this.cancel=cancel; }
        private void Update()
        {
            if (request == null || request.IsPending) return;
            var pending=request; request=null;
            var action=pending.Plan == null ? cancel : ready;
            try { action?.Invoke(); }
            catch (Exception error) { pending.Invalidate(); cancel?.Invoke(); Debug.LogWarning("[ModMode] " + error.Message); }
            finally { ready=cancel=null; Destroy(gameObject); }
        }
        private void OnDestroy()
        {
            if (request == null) return;
            request.Invalidate(); request=null; cancel?.Invoke(); ready=cancel=null;
        }
    }

    public static partial class ModRuntime
    {
        public static bool HasAiHandler(string tactic) => _scripts != null && _scripts.HasAiHandler(tactic);

        public static int? DecideAi(string tactic, object instance, Model self, Model opponent, int frame,
            System.Collections.Generic.IReadOnlyList<InfoAnimation> actions)
        {
            try
            {
                if (_scripts == null) return null;
                var candidates = new ModAiActionSnapshot[actions.Count];
                for (int i = 0; i < candidates.Length; i++)
                    candidates[i] = AiActionSnapshot(actions[i]);
                return _scripts.DecideAi(tactic, instance, new ModCombatSnapshot(AiSnapshot(self), AiSnapshot(opponent), Math.Max(0,frame), true), candidates);
            }
            catch (Exception error) { Debug.LogWarning("[ModAI] Using native tactics after decision failure. " + error.Message); return null; }
        }

        private static ModAiActionSnapshot AiActionSnapshot(InfoAnimation action)
        {
            string type = action.Type == InfoAnimation.MGHNBEPCKIF.AnimationAttack ? "attack" :
                action.Type == InfoAnimation.MGHNBEPCKIF.AnimationMove ? "move" : "none";
            var timing = new ModAiActionTiming(action.GOBJCKFGIPA, action.LHHAGECFIOL,
                action.MNHGBPOIHKG, action.NCEKKNIMHAG());
            var inputs = new System.Collections.Generic.List<ModAiActionInput>();
            var keys = action.ILBCHANCOBP()?.FONEJOKEIEN;
            if (keys != null)
            {
                AppendAiInputs(inputs, keys.IGEEOAGOMEM, "tap");
                AppendAiInputs(inputs, keys.CEPODJDDLBF, "hold");
                AppendAiInputs(inputs, keys.HPEOJLAMIHC, "release");
            }
            return new ModAiActionSnapshot(action.Name, type, action.Priority, timing, inputs);
        }

        private static void AppendAiInputs(System.Collections.Generic.List<ModAiActionInput> target,
            System.Collections.Generic.List<int> controls, string press)
        {
            if (controls.Count > 64 - target.Count) throw new ModContentException("AI action has too many input entries.");
            foreach (int control in controls)
            {
                string name;
                switch ((FightCID)control)
                {
                    case FightCID.QuadrantUp: name = "Up"; break;
                    case FightCID.QuadrantUpForward: name = "Up-Forward"; break;
                    case FightCID.QuadrantForward: name = "Forward"; break;
                    case FightCID.QuadrantDownForward: name = "Down-Forward"; break;
                    case FightCID.QuadrantDown: name = "Down"; break;
                    case FightCID.QuadrantDownBack: name = "Down-Back"; break;
                    case FightCID.QuadrantBack: name = "Back"; break;
                    case FightCID.QuadrantUpBack: name = "Up-Back"; break;
                    case FightCID.Punch: name = "Punch"; break;
                    case FightCID.Kick: name = "Kick"; break;
                    case FightCID.MissileButton: name = "Ranged"; break;
                    case FightCID.MagicButton: name = "Magic"; break;
                    case FightCID.RaidChargeButton: name = "RaidCharge"; break;
                    case FightCID.Super: name = "Super"; break;
                    default: name = "Unknown"; break;
                }
                target.Add(new ModAiActionInput(name, press));
            }
        }

        private static ModFighterSnapshot AiSnapshot(Model model)
        {
            if (model == null || model.KMMJCHDKBDO == null || model.PLBNCDCFPML() == null) return null;
            var position = model.PLBNCDCFPML();
            return new ModFighterSnapshot(model.KKMCHCNOHMB(), model.KMMJCHDKBDO.CIDCNCDFONA,
                model.KMMJCHDKBDO.HealthBarCount, position.GILCBJJPKBK(),position.OBIMBNIBEFG(),position.KMFEKANLCFO(),
                CaptureAnimationSnapshot(model));
        }

        public static ModAnimationSnapshot CaptureAnimationSnapshot(Model model)
        {
            var controller = model?.OCPMJKIEPIG();
            if (controller == null || !controller.NMEEPBDJHMG()) return null;
            var animation = controller.NNMAFFCCMHC();
            var active = controller.PCKKMNHDDMP();
            if (animation == null || animation.Name == null || active == null || active.Count > 256) return null;
            int facing = controller.KFCNPADAMHA();
            if (facing != -1 && facing != 1) return null;
            var intervals = new ModAnimationIntervalSnapshot[active.Count];
            for (int i = 0; i < intervals.Length; i++)
            {
                var interval = active[i];
                if (interval == null) return null;
                string kind;
                switch (interval.Type)
                {
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNSTABLE: kind = "unstable"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNINTERRUPT: kind = "uninterrupt"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_SELF_UNINTERRUPT: kind = "self_uninterrupt"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK: kind = "attack"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK: kind = "block"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE: kind = "invulnerable"; break;
                    case IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVISIBLE: kind = "invisible"; break;
                    default: kind = "none"; break;
                }
                intervals[i] = new ModAnimationIntervalSnapshot(interval.Name ?? string.Empty, kind);
            }
            string type = animation.Type == InfoAnimation.MGHNBEPCKIF.AnimationAttack ? "attack" :
                animation.Type == InfoAnimation.MGHNBEPCKIF.AnimationMove ? "move" : "none";
            return new ModAnimationSnapshot(animation.Name, type, facing, intervals);
        }

        public static void ApplyLocaleMetadata()
        {
            if (_legacyContent == null) return;
            try
            {
                ExternalLocaleRuntime.Apply();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to bind mod locales; disabling external mods without restarting the game parser. " + exception);
                Shutdown();
            }
        }

        public static void ApplyP1DContent()
        {
            if (_legacyContent == null) return;
            try
            {
                _legacyContent.ApplyP1DContent();
                Debug.Log("[ModContent] Applied P1D content: " + Scripts.Content.LocaleMetadata.Count +
                    " locales, " + Scripts.Content.Locations.Count + " locations, " +
                    Scripts.Content.MoveTemplates.Count + " move templates, " + Scripts.Content.Moves.Count +
                    " moves, " + Scripts.Content.MoveTriggers.Count + " move triggers, " +
                    Scripts.Content.Tactics.Count + " tactics.");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply P1D content; mod startup is invalid. " + exception);
                throw;
            }
        }
    }
}
