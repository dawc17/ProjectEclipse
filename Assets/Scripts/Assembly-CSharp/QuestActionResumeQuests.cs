using System.Collections.Generic;
using System.Xml;

public class QuestActionResumeQuests : QuestAction
{
	private QuestActionsSequence DBONDAIEBPN = new QuestActionsSequence();

	private QuestActionsSequence LDDDPGLPHCO = new QuestActionsSequence();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		XmlNode ePKLCPOEELO = EPKLCPOEELO["Success"];
		XmlNode ePKLCPOEELO2 = EPKLCPOEELO["Error"];
		APKBANHAEGN(ePKLCPOEELO, DBONDAIEBPN, PMPMGDFGOML);
		APKBANHAEGN(ePKLCPOEELO2, LDDDPGLPHCO, OnActionComplete);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		int num = 0;
		List<RosterQuest> list = nKGLHEGIKKP.JNHBGEDJBLJ();
		foreach (RosterQuest item in list)
		{
			if (ONGHPGEIJEN != item.Name && item.get_Parameters() != null)
			{
				QuestStage mLLKDGBEGJI = ListSF.ELEBLBJKDBI().PBGCEEBDBGG(item.Name);
				if (mLLKDGBEGJI != null && !mLLKDGBEGJI.IDGAAJAFCHC())
				{
					num++;
				}
			}
		}
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		bool flag = false;
		if (num == 0 && (iPKNDMINFMJ == ScreenType.ModulePreloader || iPKNDMINFMJ == ScreenType.ModuleNone || flag))
		{
			LDDDPGLPHCO.DEJMHFMLKIC(GFIHPBCEEOB);
		}
		else
		{
			DBONDAIEBPN.DEJMHFMLKIC(GFIHPBCEEOB);
		}
	}

	private void OnActionComplete(object data)
	{
		OGIJONMKABB();
	}

	private void PMPMGDFGOML(object data)
	{
		OGIJONMKABB();
		ListSF.CCDKHLAMKKO().PBOFBNFALNN();
	}

	public override void GKFMJKAAJCA()
	{
		DBONDAIEBPN.FHPKJMMLIEG();
		LDDDPGLPHCO.FHPKJMMLIEG();
	}
}
