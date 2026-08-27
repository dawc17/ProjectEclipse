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
		if (Module.ELEBLBJKDBI().NMCNDOPKFJD() == ScreenType.ModuleMap)
		{
			MapScene current = Scene<MapScene>.get_Current();
			if (current != null)
			{
				current.SetStoryZonesBackgroundMask(PBHBBIOOEPJ);
			}
		}
		ListSF.CCDKHLAMKKO().set_MapMaskColor(PBHBBIOOEPJ);
		OGIJONMKABB();
	}
}
