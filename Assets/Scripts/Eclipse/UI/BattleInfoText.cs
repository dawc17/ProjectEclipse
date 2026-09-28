namespace Eclipse.UI
{
    // Map info panel captions. The recovered code appended the count after the whole
    // "Replays: {0}" string, which showed the raw placeholder or "Replays 0". Eclipse-mode
    // counterparts (<battle>_ECLIPSEMODE) are marked so a first Eclipse run no longer
    // reads as an ordinary replay.
    public static class BattleInfoText
    {
        public static string Replays(BattleReplayable battle)
        {
            int count = battle.HLBOMMKJAAO();
            string replays = LocalizationManager.GetString("replays", count.ToString());
            if (string.IsNullOrEmpty(replays)) replays = "Replays: " + count;
            bool eclipse = battle.get_Name().EndsWith("_ECLIPSEMODE", System.StringComparison.Ordinal);
            if (!eclipse) return replays;
            return count == 0 ? "ECLIPSE" : "ECLIPSE \u00b7 " + replays;
        }
    }
}
