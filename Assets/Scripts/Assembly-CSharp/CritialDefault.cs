using System.Xml;

public class CritialDefault
{
	private global::Pair<float, string> JLIBFOIHBMN = new global::Pair<float, string>(0f, null);

	private global::Pair<float, string> _Damage = new global::Pair<float, string>(0f, null);

	public void Parse(XmlNode node)
	{
		XmlNode xmlNode = node["Probability"];
		JLIBFOIHBMN.First = xmlNode.Attributes["Base"].ParseFloat();
		JLIBFOIHBMN.Second = xmlNode.Attributes["Attribute"].CIPOICEEIBK(string.Empty);
		XmlNode xmlNode2 = node["Damage"];
		_Damage.First = xmlNode2.Attributes["Base"].ParseFloat();
		_Damage.Second = xmlNode2.Attributes["Attribute"].CIPOICEEIBK(string.Empty);
	}

	public float JJNCDHOKEIA(Model ACENLMONNPA)
	{
		int OEMALIFPGPO = 0;
		if (ACENLMONNPA.KMMJCHDKBDO.IBLHIAHECLK.Get(JLIBFOIHBMN.Second, ref OEMALIFPGPO) && !string.IsNullOrEmpty(JLIBFOIHBMN.Second))
		{
			return JLIBFOIHBMN.First * (float)OEMALIFPGPO;
		}
		return JLIBFOIHBMN.First;
	}

	public float KDPAKCJCNMI(Model ACENLMONNPA)
	{
		int OEMALIFPGPO = 0;
		if (ACENLMONNPA.KMMJCHDKBDO.IBLHIAHECLK.Get(_Damage.Second, ref OEMALIFPGPO) && !string.IsNullOrEmpty(_Damage.Second))
		{
			return _Damage.First * (float)OEMALIFPGPO;
		}
		return _Damage.First;
	}
}
