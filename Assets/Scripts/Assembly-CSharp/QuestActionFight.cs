using System.Xml;

public class QuestActionFight : QuestAction
{
	private string _name = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ListSF.ELEBLBJKDBI().ClearQuestsStack();
		if (string.IsNullOrEmpty(_name))
		{
			// The newer quest graph deliberately uses <Fight /> in the error branch of
			// ScriptsResumeOnStart.  It means "there was no interrupted fight to
			// resume, return to the normal game screen", rather than a malformed fight.
			Module.DLOKJOHNDID(ScreenType.ModuleDojo);
			OGIJONMKABB();
			return;
		}
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		string bAINMLLIKOL = lNIDLHOIHIM.ToString();
		FightIDS mOCEDDJOAEB = new FightIDS();
		mOCEDDJOAEB.SetFightIDSByString(bAINMLLIKOL);
		FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(mOCEDDJOAEB);
		if (jDIPBIHBGPF != null)
		{
			GameUtils.StartFight(jDIPBIHBGPF);
		}
		else
		{
			Module.DLOKJOHNDID(ScreenType.ModuleDojo);
		}
		OGIJONMKABB();
	}
}
