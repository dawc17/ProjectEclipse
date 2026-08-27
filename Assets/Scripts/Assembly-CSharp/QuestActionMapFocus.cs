using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionMapFocus : QuestAction
{
	private string _BattleName = string.Empty;

	private float _Duration;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_BattleName = EPKLCPOEELO.Attributes["Battle"].CIPOICEEIBK(string.Empty);
		_Duration = EPKLCPOEELO.Attributes["Frames"].ParseFloat() / 60f;
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(_BattleName, lNIDLHOIHIM);
		ListSF.CCDKHLAMKKO().NDFLHPGHKMP(lNIDLHOIHIM.ToString());
		FightIDS dIAIIPCBMFL = ListSF.CCDKHLAMKKO().KNJNHKDCINB();
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null)
		{
			FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(dIAIIPCBMFL);
			if (jDIPBIHBGPF != null)
			{
				current.SelectFight(jDIPBIHBGPF, _Duration);
			}
			else
			{
				Battle dPOOIONCEOA = ListSF.MKHAAGMJOPG(dIAIIPCBMFL);
				current.SelectBattle(dPOOIONCEOA, _Duration);
			}
		}
		OGIJONMKABB();
	}
}
