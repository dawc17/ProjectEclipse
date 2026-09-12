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
                var names = new string[actions.Count];
                for (int i = 0; i < names.Length; i++) names[i] = actions[i].Name;
                return _scripts.DecideAi(tactic, instance, new ModCombatSnapshot(AiSnapshot(self), AiSnapshot(opponent), Math.Max(0,frame), true), names);
            }
            catch (Exception error) { Debug.LogWarning("[ModAI] Using native tactics after decision failure. " + error.Message); return null; }
        }

        private static ModFighterSnapshot AiSnapshot(Model model)
        {
            if (model == null || model.KMMJCHDKBDO == null || model.PLBNCDCFPML() == null) return null;
            var position = model.PLBNCDCFPML();
            return new ModFighterSnapshot(model.KKMCHCNOHMB(), model.KMMJCHDKBDO.CIDCNCDFONA,
                model.KMMJCHDKBDO.HealthBarCount, position.GILCBJJPKBK(),position.OBIMBNIBEFG(),position.KMFEKANLCFO());
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
