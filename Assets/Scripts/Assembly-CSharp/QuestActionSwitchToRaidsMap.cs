using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionSwitchToRaidsMap : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null)
		{
			current.SwitchToRaidMap();
		}
		OGIJONMKABB();
	}
}
