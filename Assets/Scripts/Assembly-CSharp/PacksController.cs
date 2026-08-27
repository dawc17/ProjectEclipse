using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class PacksController
{
	private static PacksController _Instance;

	private List<JBKAOMLJCEL> ABGFNEJHKND = new List<JBKAOMLJCEL>();

	private XmlDocument _docPacks;

	public static PacksController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public List<JBKAOMLJCEL> MDEDBJGLMJI
	{
		get
		{
			return CHOJDOCKBOL();
		}
	}

	public static PacksController ELEBLBJKDBI()
	{
		if (_Instance == null)
		{
			_Instance = new PacksController();
		}
		return _Instance;
	}

	public List<JBKAOMLJCEL> CHOJDOCKBOL()
	{
		return ABGFNEJHKND;
	}

	public void GDNFPIBDDBO()
	{
		_docPacks = XmlUtils.OpenXMLDocument(SF2Paths.IDLJHPEDOEH(), string.Empty, XmlUtils.EBLFEPIOMOL.ForcedExternal);
		if (_docPacks == null)
		{
			_docPacks = new XmlDocument();
		}
		XmlNode xmlNode = _docPacks["Packs"];
		if (xmlNode == null)
		{
			return;
		}
		foreach (XmlNode item in xmlNode)
		{
			JBKAOMLJCEL jBKAOMLJCEL = GALFCDACJEI(item);
			AddBundle(jBKAOMLJCEL);
			ABGFNEJHKND.Add(jBKAOMLJCEL);
		}
	}

	private void AddBundle(JBKAOMLJCEL LMHLLOBNKMB)
	{
		if (LMHLLOBNKMB.NBEEINKJMPK)
		{
			try
			{
				BundleManager.AddBundle(LMHLLOBNKMB.Name);
			}
			catch (Exception ex)
			{
				LLLOJBFMONN.Error(ex.ToString());
			}
		}
	}

	private JBKAOMLJCEL GALFCDACJEI(XmlNode node)
	{
		JBKAOMLJCEL jBKAOMLJCEL = new JBKAOMLJCEL();
		jBKAOMLJCEL.Name = node.Attributes["Name"].CIPOICEEIBK();
		jBKAOMLJCEL.Url = node.Attributes["Url"].CIPOICEEIBK();
		jBKAOMLJCEL.Version = node.Attributes["Version"].ParseInt();
		jBKAOMLJCEL.Size = node.Attributes["Size"].CIPOICEEIBK();
		jBKAOMLJCEL.EFJLHFFGCIF = node.Attributes["Reload"].ParseBool();
		jBKAOMLJCEL.NBEEINKJMPK = node.Attributes["Attach"].ParseBool();
		return jBKAOMLJCEL;
	}

	public bool IsPackByName(string name)
	{
		JBKAOMLJCEL jBKAOMLJCEL = GeneralConfig.NNFMKNJJDDD.OCKOCHAINHG(name);
		foreach (JBKAOMLJCEL item in ABGFNEJHKND)
		{
			if (item.Name.Equals(name))
			{
				return jBKAOMLJCEL == null || jBKAOMLJCEL.Url.Equals(item.Url);
			}
		}
		return false;
	}

	public JBKAOMLJCEL OCKOCHAINHG(string name)
	{
		return ABGFNEJHKND.Find((JBKAOMLJCEL DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
	}

	public void DDKKLHDOFNG(string name, string BEPKJNKCKPH, string version, long NKKKMPPEMKE, bool AHDLCJFCJMJ)
	{
		if (_docPacks == null)
		{
			LLLOJBFMONN.Error("PacksContainer.AddPack _docPacks is null");
			return;
		}
		XmlNode packsXML = _docPacks["Packs"];
		if (packsXML == null)
		{
			packsXML = _docPacks.KDPLHGGPJHN("Packs");
		}
		List<XmlNode> list = new List<XmlNode>();
		foreach (XmlNode childNode in packsXML.ChildNodes)
		{
			string text = childNode.Attributes["Name"].CIPOICEEIBK();
			if (text.Equals(name))
			{
				list.Add(childNode);
			}
		}
		list.ForEach((XmlNode DHDMNHCIPEH) =>
		{
			packsXML.RemoveChild(DHDMNHCIPEH);
		});
		XmlNode xmlNode2 = packsXML.ACBPMPMPKJJ("Pack");
		xmlNode2.LLIKNHNLGJJ("Name").Value = name;
		xmlNode2.LLIKNHNLGJJ("Url").Value = BEPKJNKCKPH;
		xmlNode2.LLIKNHNLGJJ("Version").Value = version;
		xmlNode2.LLIKNHNLGJJ("Attach").Value = ((!AHDLCJFCJMJ) ? "0" : "1");
		if (NKKKMPPEMKE >= 0)
		{
			xmlNode2.LLIKNHNLGJJ("EndDate").Value = NKKKMPPEMKE.ToString();
		}
		_docPacks.Save(SF2Paths.IDLJHPEDOEH());
		JBKAOMLJCEL jBKAOMLJCEL = ABGFNEJHKND.Find((JBKAOMLJCEL DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
		if (jBKAOMLJCEL != null)
		{
			ABGFNEJHKND.Remove(jBKAOMLJCEL);
		}
		jBKAOMLJCEL = GALFCDACJEI(xmlNode2);
		if (jBKAOMLJCEL != null)
		{
			ABGFNEJHKND.Add(jBKAOMLJCEL);
			AddBundle(jBKAOMLJCEL);
		}
	}

	public void DeletePack(string name)
	{
		if (_docPacks == null)
		{
			LLLOJBFMONN.Error("PacksContainer.AddPack _docPacks is null");
			return;
		}
		XmlNode xmlNode = _docPacks["Packs"];
		if (xmlNode == null)
		{
			xmlNode = _docPacks.CreateNode(XmlNodeType.Element, "Packs", null);
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string text = childNode.Attributes["Name"].CIPOICEEIBK();
			if (text.Equals(name))
			{
				xmlNode.RemoveChild(childNode);
			}
		}
		_docPacks.Save(SF2Paths.IDLJHPEDOEH());
		JBKAOMLJCEL jBKAOMLJCEL = ABGFNEJHKND.Find((JBKAOMLJCEL DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
		if (jBKAOMLJCEL != null)
		{
			ABGFNEJHKND.Remove(jBKAOMLJCEL);
		}
	}

	public void Reset()
	{
		ABGFNEJHKND.Clear();
		BundleManager.Reset();
	}

	public void AKMIAJPGHDC()
	{
		if (File.Exists(SF2Paths.IDLJHPEDOEH()))
		{
			File.Delete(SF2Paths.IDLJHPEDOEH());
			UserDataValidator.KAFMCNCGOJH(SF2Paths.IDLJHPEDOEH());
		}
		SF2Paths.CKCGLNHIDFN();
	}
}
