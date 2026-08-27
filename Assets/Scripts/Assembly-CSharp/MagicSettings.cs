using System.Collections.Generic;
using System.Xml;

public class MagicSettings
{
	private class MagicCharge
	{
		private string _Name;

		private float _Base;

		private string HMKPNFBMHMP;

		public float Base
		{
			get
			{
				return FINOFPCBLDK();
			}
		}

		public string Attribute
		{
			get
			{
				return EJPCHOLGGJJ();
			}
		}

		public string get_Name()
		{
			return _Name;
		}

		public float FINOFPCBLDK()
		{
			return _Base;
		}

		public string EJPCHOLGGJJ()
		{
			return HMKPNFBMHMP;
		}

		public void Parse(XmlNode node)
		{
			_Name = node.Name;
			_Base = XmlUtils.ParseFloat(node.Attributes["Base"]);
			HMKPNFBMHMP = XmlUtils.ParseString(node.Attributes["Attribute"]);
		}
	}

	private List<MagicCharge> APLFDBEPGBK = new List<MagicCharge>();

	private MagicCharge EDIFEKEHLJF(string name)
	{
		for (int i = 0; i < APLFDBEPGBK.Count; i++)
		{
			if (APLFDBEPGBK[i].get_Name() == name)
			{
				return APLFDBEPGBK[i];
			}
		}
		return null;
	}

	public void Parse(XmlNode node)
	{
		APLFDBEPGBK.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			MagicCharge eHONFFPDKOI = new MagicCharge();
			APLFDBEPGBK.Add(eHONFFPDKOI);
			eHONFFPDKOI.Parse(childNode);
		}
	}

	private float ACGMAJCAKIK(string JLEKBBJBLOE, ModelParameters IHEFAMAFBIA = null)
	{
		MagicCharge eHONFFPDKOI = EDIFEKEHLJF(JLEKBBJBLOE);
		if (eHONFFPDKOI == null)
		{
			LLLOJBFMONN.Error(JLEKBBJBLOE + " for Magic not found");
			return 0f;
		}
		if (IHEFAMAFBIA == null)
		{
			return eHONFFPDKOI.FINOFPCBLDK();
		}
		int OEMALIFPGPO = 0;
		if (IHEFAMAFBIA.IBLHIAHECLK.Get(eHONFFPDKOI.EJPCHOLGGJJ(), ref OEMALIFPGPO))
		{
			return eHONFFPDKOI.FINOFPCBLDK() * (float)OEMALIFPGPO;
		}
		return eHONFFPDKOI.FINOFPCBLDK();
	}

	public float HCJBIAGKIGI(Model ACENLMONNPA)
	{
		return HCJBIAGKIGI(ACENLMONNPA.KMMJCHDKBDO);
	}

	public float MPIOONCNFOK(Model ACENLMONNPA)
	{
		return MPIOONCNFOK(ACENLMONNPA.KMMJCHDKBDO);
	}

	public float LLKJJLOMNID(Model ACENLMONNPA)
	{
		return LLKJJLOMNID(ACENLMONNPA.KMMJCHDKBDO);
	}

	public float HCJBIAGKIGI(ModelParameters IHEFAMAFBIA)
	{
		return ACGMAJCAKIK("InitialCharge", IHEFAMAFBIA);
	}

	public float NKOILNKJOAA()
	{
		return ACGMAJCAKIK("InitialCharge");
	}

	public float MPIOONCNFOK(ModelParameters IHEFAMAFBIA)
	{
		return ACGMAJCAKIK("PainRecharge", IHEFAMAFBIA);
	}

	public float JGHFCAPPDED()
	{
		return ACGMAJCAKIK("PainRecharge");
	}

	public float LLKJJLOMNID(ModelParameters IHEFAMAFBIA)
	{
		return ACGMAJCAKIK("DamageRecharge", IHEFAMAFBIA);
	}

	public float HOOBFKANPEK()
	{
		return ACGMAJCAKIK("DamageRecharge");
	}
}
