using System;
using System.Collections.Generic;
using System.Linq;

namespace Eclipse.Modding
{
    public interface IModRoundOutcomes
    {
        bool TryEndRound(DefinitionId rule, bool playerWins, out string error);
    }

    public static class ModOutcomeAuthority
    {
        public static void Validate(IEnumerable<FightRuleDefinition> rules, string where)
        {
            var owners = rules.Where(rule => rule.ControlsOutcome).GroupBy(rule => rule.Id).Select(group => group.First()).ToArray();
            for (int i = 0; i < owners.Length; i++)
                for (int j = i + 1; j < owners.Length; j++)
                {
                    var left = owners[i]; var right = owners[j];
                    bool modes = left.Mode == ModRuleMode.All || right.Mode == ModRuleMode.All || left.Mode == right.Mode;
                    bool rounds = left.Rounds.Count == 0 || right.Rounds.Count == 0 || left.Rounds.Any(right.Rounds.Contains);
                    if (modes && rounds) throw new ModContentException(where + " has conflicting round outcome authorities '" + left.Id + "' and '" + right.Id + "'. Use one controller or disjoint mode/round filters.");
                }
        }
    }

    // Stores a request, never settles a result or grants a reward. The native
    // simulation consumes it after checking lethal hits, timeout and native rules.
    public sealed class ModRoundOutcomeState
    {
        public int Round { get; private set; } = -1;
        public DefinitionId? Authority { get; private set; }
        public bool? PendingPlayerWins { get; private set; }
        public bool? ResolvedPlayerWins { get; private set; }
        public void BeginRound(int round, DefinitionId? authority)
        {
            Round = round; Authority = authority; PendingPlayerWins = null; ResolvedPlayerWins = null;
        }
        public bool TryRequest(DefinitionId rule, bool playerWins, out string error)
        {
            if (!Authority.HasValue || Authority.Value != rule) { error = "This rule does not control this round's outcome."; return false; }
            if (ResolvedPlayerWins.HasValue) { error = "The round outcome is already resolved."; return false; }
            if (PendingPlayerWins.HasValue && PendingPlayerWins.Value != playerWins)
            { error = "A different outcome is already pending for this round."; return false; }
            PendingPlayerWins = playerWins; error = null; return true;
        }
        public bool TryConsume(out bool playerWins)
        {
            playerWins = PendingPlayerWins ?? false;
            if (!PendingPlayerWins.HasValue || ResolvedPlayerWins.HasValue) return false;
            ResolvedPlayerWins = PendingPlayerWins; PendingPlayerWins = null; return true;
        }
        public void Cancel() { PendingPlayerWins = null; ResolvedPlayerWins = null; }
    }

    public sealed partial class ModContentCatalog
    {
        private void ValidateOutcomeAuthorities(FightDefinition[] fights, FightRuleDefinition[] rules,
            Dictionary<DefinitionId, FightDefinition> replacements)
        {
            var pending = rules.ToDictionary(rule => rule.Id);
            foreach (var fight in fights.Concat(replacements.Values))
            {
                var attached = new List<FightRuleDefinition>();
                foreach (var id in fight.Rules)
                    if (pending.TryGetValue(id, out var rule) || TryGetFightRule(id, out rule)) attached.Add(rule);
                ModOutcomeAuthority.Validate(attached, "Fight '" + fight.Id + "'");
            }
        }
    }
}
