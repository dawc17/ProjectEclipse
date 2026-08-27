using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionToggleBattle : QuestAction
{
	private bool _toggle;

	private string _name;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		string text = EPKLCPOEELO.Attributes["Toggle"].CIPOICEEIBK(string.Empty);
		if (text.Equals("on"))
		{
			_toggle = true;
		}
		else if (text.Equals("off"))
		{
			_toggle = false;
		}
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		FightIDS mOCEDDJOAEB = new FightIDS();
		mOCEDDJOAEB.SetFightIDSByString(lNIDLHOIHIM.resultSTR);
		Battle cGJCGEBPCAF = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
		RosterBattle dDNLCGOPAGC = ((cGJCGEBPCAF == null) ? null : cGJCGEBPCAF.NNPNEABKHPP());
		if (dDNLCGOPAGC != null)
		{
			dDNLCGOPAGC.HCEOCBOFIGC(!_toggle);
		}
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null && cGJCGEBPCAF != null)
		{
			current.UpdateBattleButtonHidden(cGJCGEBPCAF);
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		OGIJONMKABB();
	}
}
