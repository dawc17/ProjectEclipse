using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using UnityEngine;

public class QuestActionMapMask : QuestAction
{
	private Color PBHBBIOOEPJ;

	private bool JBFIICKLHEB;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		string oHJKNABLCMF = EPKLCPOEELO.Attributes["Color"].CIPOICEEIBK(string.Empty);
		PBHBBIOOEPJ = ColorUtils.DAAIIECAAFO(oHJKNABLCMF);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		if (Module.GetInstance().GetCurrentScreenType() == ScreenType.ModuleMap)
		{
			MapScene current = Scene<MapScene>.get_Current();
			if (current != null)
			{
				// Eclipse: ease the toggle instead of snapping the whole map to the new tint.
				current.FadeStoryZonesBackgroundMask(PBHBBIOOEPJ, 0.8f);
			}
		}
		ListSF.CCDKHLAMKKO().set_MapMaskColor(PBHBBIOOEPJ);
		OGIJONMKABB();
	}
}
