using System.Collections.Generic;
using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionUnlockBattle : QuestAction
{
	private bool _toggle;

	private bool LNKJGCAAJHN;

	private bool LIHHPCMHCCE;

	private bool INMFGOMPJEO;

	private string _name = string.Empty;

	private string GFAMDGCPINA = string.Empty;

	private string _hidden = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		string name = EPKLCPOEELO.Name;
		_toggle = name == "ShowBattle";
		LNKJGCAAJHN = EPKLCPOEELO.Attributes["Locked"].ParseBool();
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		LIHHPCMHCCE = EPKLCPOEELO.Attributes["Instant"].ParseBool();
		_hidden = EPKLCPOEELO.Attributes["Hidden"].CIPOICEEIBK(string.Empty);
		GFAMDGCPINA = EPKLCPOEELO.Attributes["ReplayCount"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters JCICKLIMBEF)
	{
		base.DEJMHFMLKIC(JCICKLIMBEF);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(JCICKLIMBEF);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		int oAHPBDFKJOK = 0;
		if (!string.IsNullOrEmpty(GFAMDGCPINA))
		{
			ConditionExtension.CompareResult lNIDLHOIHIM2 = new ConditionExtension.CompareResult();
			kKDGLNECFHA.MCPIOGALBMK(GFAMDGCPINA, lNIDLHOIHIM2);
			oAHPBDFKJOK = (int)lNIDLHOIHIM2.resultNumber;
		}
		if (!string.IsNullOrEmpty(_hidden))
		{
			// 2.41.x intermission data uses this compact infix form.  The legacy
			// quest evaluator only understands ?Sub(...), so it used to parse as
			// false and exposed Eclipse battles while the mode was disabled.
			if (_hidden.Trim() == "1 - _$InEclipseMode")
			{
				Roster roster = ListSF.CCDKHLAMKKO();
				INMFGOMPJEO = roster == null || !roster.JPMPIDFGCJL();
			}
			else
			{
				ConditionExtension.CompareResult lNIDLHOIHIM3 = new ConditionExtension.CompareResult();
				kKDGLNECFHA.MCPIOGALBMK(_hidden, lNIDLHOIHIM3);
				INMFGOMPJEO = lNIDLHOIHIM3.resultNumber > 0.0;
			}
		}
		FightIDS mOCEDDJOAEB = new FightIDS();
		mOCEDDJOAEB.SetFightIDSByString(lNIDLHOIHIM.resultSTR);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.KJIMPNEGNAN(mOCEDDJOAEB, true, _toggle, LNKJGCAAJHN, INMFGOMPJEO, oAHPBDFKJOK);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		ListSF.CGJCKGAFPED();
		Zone pKCPOJKLMOK = ListSF.CFEDCFACBLE(mOCEDDJOAEB.PELHCAEAOFE());
		bool flag = JLIJLEGKEJA(pKCPOJKLMOK);
		Battle cGJCGEBPCAF = ((pKCPOJKLMOK == null) ? null : pKCPOJKLMOK.MJINKOFNIAE(mOCEDDJOAEB.CPHDPCAECJN()));
		if (cGJCGEBPCAF != null)
		{
			cGJCGEBPCAF.DCHJDPCEODD = _toggle;
		}
		bool flag2 = JLIJLEGKEJA(pKCPOJKLMOK);
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null)
		{
			if ((!flag && flag2) || (flag && !flag2))
			{
				current.ReloadZones();
			}
			current.ActiveBattleByFightIDS(mOCEDDJOAEB, _toggle, false, LIHHPCMHCCE);
		}
		OGIJONMKABB();
	}

	private bool JLIJLEGKEJA(Zone HLJKOKMKMLM)
	{
		if (HLJKOKMKMLM == null)
		{
			return false;
		}
		List<Battle> lGIIBNJFADA = HLJKOKMKMLM.LGIIBNJFADA;
		for (int i = 0; i < lGIIBNJFADA.Count; i++)
		{
			if (lGIIBNJFADA[i].DCHJDPCEODD)
			{
				return true;
			}
		}
		return false;
	}
}
