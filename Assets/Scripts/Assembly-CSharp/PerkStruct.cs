using System.Collections.Generic;
using System.Xml;

public class PerkStruct
{
	public const string EclipseEnchantmentAttribute = "EclipseEnchantment";

	public const string EclipseKindAttribute = "EclipseKind";

	private string _name = string.Empty;

	private List<string> _itemTypes = new List<string>();

	private List<KeyValuePair<string, string>> MAPHFLOPAOD = new List<KeyValuePair<string, string>>();

	private string _eclipseEnchantment = string.Empty;

	private string _eclipseKind = string.Empty;

	private readonly Dictionary<string, string> _eclipseParameters =
		new Dictionary<string, string>(System.StringComparer.Ordinal);

	public List<string> NMOKPAPJLCN
	{
		get
		{
			return DCAOANOEJGF();
		}
	}

	public List<KeyValuePair<string, string>> Pairs
	{
		get
		{
			return EOLPAHGCMHH();
		}
	}

	public string EclipseEnchantment
	{
		get
		{
			return _eclipseEnchantment;
		}
	}

	public string EclipseKind
	{
		get
		{
			return _eclipseKind;
		}
	}

	public IReadOnlyDictionary<string, string> EclipseParameters => _eclipseParameters;

	public PerkStruct(PerkStruct NOLFMPDGCOC)
	{
		_name = string.Copy(NOLFMPDGCOC.get_Name());
		_eclipseEnchantment = string.Copy(NOLFMPDGCOC._eclipseEnchantment);
		_eclipseKind = string.Copy(NOLFMPDGCOC._eclipseKind);
		foreach (KeyValuePair<string, string> pair in NOLFMPDGCOC._eclipseParameters)
			_eclipseParameters.Add(string.Copy(pair.Key), string.Copy(pair.Value));
		_itemTypes = new List<string>();
		NOLFMPDGCOC._itemTypes.ForEach((string DHDMNHCIPEH) =>
		{
			_itemTypes.Add(string.Copy(DHDMNHCIPEH));
		});
		MAPHFLOPAOD = new List<KeyValuePair<string, string>>();
		NOLFMPDGCOC.MAPHFLOPAOD.ForEach((KeyValuePair<string, string> DHDMNHCIPEH) =>
		{
			string key = string.Copy(DHDMNHCIPEH.Key);
			string value = string.Copy(DHDMNHCIPEH.Value);
			MAPHFLOPAOD.Add(new KeyValuePair<string, string>(key, value));
		});
	}

	public PerkStruct(XmlNode node)
	{
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_eclipseEnchantment = node.Attributes[EclipseEnchantmentAttribute].CIPOICEEIBK(string.Empty);
		_eclipseKind = node.Attributes[EclipseKindAttribute].CIPOICEEIBK(string.Empty);
		string text = node.Attributes["ItemType"].CIPOICEEIBK(string.Empty);
		if (text != null)
		{
			string[] collection = text.Split('|');
			_itemTypes.AddRange(collection);
		}
		XmlNode parameters = node[Eclipse.Modding.ModEffectSaveData.NodeName];
		if (parameters != null && parameters.Attributes?["Format"]?.Value == Eclipse.Modding.ModEffectSaveData.Format)
		{
			foreach (XmlNode parameter in parameters.ChildNodes)
			{
				if (parameter.NodeType != XmlNodeType.Element ||
					parameter.Name != Eclipse.Modding.ModEffectSaveData.ParameterNodeName) continue;
				string key = parameter.Attributes?["Name"]?.Value;
				XmlAttribute value = parameter.Attributes?["Value"];
				if (string.IsNullOrEmpty(key) || value == null || _eclipseParameters.ContainsKey(key)) continue;
				_eclipseParameters.Add(key, value.Value);
			}
		}
		XmlNode xmlNode = node["Set"];
		if (xmlNode != null)
		{
			foreach (XmlAttribute attribute in xmlNode.Attributes)
			{
				KeyValuePair<string, string> item = new KeyValuePair<string, string>(attribute.Name, attribute.CIPOICEEIBK(string.Empty));
				MAPHFLOPAOD.Add(item);
			}
		}
	}

	public string get_Name()
	{
		return _name;
	}

	public List<string> DCAOANOEJGF()
	{
		return _itemTypes;
	}

	public List<KeyValuePair<string, string>> EOLPAHGCMHH()
	{
		return MAPHFLOPAOD;
	}

	private bool CompareItemType(string LMNNBBKHMEI)
	{
		foreach (string item in _itemTypes)
		{
			if (item == LMNNBBKHMEI)
			{
				return true;
			}
		}
		return false;
	}

	public void MLONLJGHDEA()
	{
		FunctionExtension oPIFBDJNMKD = new FunctionExtension();
		PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(_name);
		if (aCONCDFDNJH == null)
		{
			return;
		}
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		foreach (KeyValuePair<string, string> item in MAPHFLOPAOD)
		{
			oPIFBDJNMKD.Parse(item.Value);
			oPIFBDJNMKD.PBPBNENGLPA(aCONCDFDNJH.HJFEFJIEINN);
			oPIFBDJNMKD.DMPCFMACDJM(aCONCDFDNJH.OKPFNCJFLDL);
			FunctionResult dEIHAOLOPLC = oPIFBDJNMKD.IBCPKBBAFNH();
			list.Add(new KeyValuePair<string, string>(item.Key, dEIHAOLOPLC.DCJLKCFKCOM));
		}
		MAPHFLOPAOD = list;
	}
}
