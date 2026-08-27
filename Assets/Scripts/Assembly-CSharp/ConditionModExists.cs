using System.Collections.Generic;
using System.Xml;

public class ConditionModExists : ConditionAnimation
{
	private string _Name;

	private string LJMGGAJCOBF;

	public string MBDDKGIOOGD
	{
		get
		{
			return DFOELJAEEGG();
		}
	}

	public ConditionModExists(XmlNode node)
		: base(DGAGKLODADD.MOD_EXISTS)
	{
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		LJMGGAJCOBF = node.Attributes["Perk"].CIPOICEEIBK(string.Empty);
	}

	public string get_Name()
	{
		return _Name;
	}

	public string DFOELJAEEGG()
	{
		return LJMGGAJCOBF;
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		List<PerksStage.ActionPerk> list = null;
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			list = conditions.LPGJIICFIKF;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			list = conditions.CBMFGJHKKMJ;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_BOTH:
			list = new List<PerksStage.ActionPerk>(conditions.LPGJIICFIKF);
			list.AddRange(conditions.CBMFGJHKKMJ);
			break;
		}
		bool flag = false;
		PerksStage.ActionPerk oAJGINIDKJD = null;
		for (int i = 0; i < list.Count; i++)
		{
			oAJGINIDKJD = list[i];
			if ((string.IsNullOrEmpty(LJMGGAJCOBF) || LJMGGAJCOBF.Equals(oAJGINIDKJD.LGMFEIFGGDG())) && _Name.Equals(oAJGINIDKJD.DDBPICENEJE()))
			{
				flag = true;
				break;
			}
		}
		return (!IsNot) ? flag : (!flag);
	}
}
