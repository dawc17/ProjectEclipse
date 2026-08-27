using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

public class PlistDocument
{
	public PlistElementDict AFHNINCKJEE;

	public string version;

	public PlistDocument()
	{
		AFHNINCKJEE = new PlistElementDict();
		version = "1.0";
	}

	internal static XDocument ParseXmlNoDtd(string HCPNFPMHFCM)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ProhibitDtd = false;
		xmlReaderSettings.XmlResolver = null;
		XmlReader reader = XmlReader.Create(new StringReader(HCPNFPMHFCM), xmlReaderSettings);
		return XDocument.Load(reader);
	}

	internal static string CleanDtdToString(XDocument EELFNMOHGJL)
	{
		if (EELFNMOHGJL.DocumentType != null)
		{
			XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), new XDocumentType(EELFNMOHGJL.DocumentType.Name, EELFNMOHGJL.DocumentType.PublicId, EELFNMOHGJL.DocumentType.SystemId, null), new XElement(EELFNMOHGJL.Root.Name));
			return string.Concat(string.Empty, xDocument.Declaration, "\n", xDocument.DocumentType, "\n", EELFNMOHGJL.Root);
		}
		XDocument xDocument2 = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(EELFNMOHGJL.Root.Name));
		return string.Concat(string.Empty, xDocument2.Declaration, Environment.NewLine, EELFNMOHGJL.Root);
	}

	private static string GetText(XElement GGBAJPGKOEM)
	{
		return string.Join(string.Empty, (from DHDMNHCIPEH in GGBAJPGKOEM.Nodes().OfType<XText>()
			select DHDMNHCIPEH.Value).ToArray());
	}

	private static PlistElement JPHIJFLDECK(XElement GGBAJPGKOEM)
	{
		switch (GGBAJPGKOEM.Name.LocalName)
		{
		case "dict":
		{
			List<XElement> list2 = GGBAJPGKOEM.Elements().ToList();
			PlistElementDict jDMGABPEDFI = new PlistElementDict();
			if (list2.Count % 2 == 1)
			{
				throw new Exception("Malformed plist file");
			}
			for (int i = 0; i < list2.Count - 1; i++)
			{
				if (list2[i].Name != "key")
				{
					throw new Exception("Malformed plist file");
				}
				string kGBGENDIMBC = GetText(list2[i]).Trim();
				PlistElement lBMGKAJIDAJ2 = JPHIJFLDECK(list2[i + 1]);
				if (lBMGKAJIDAJ2 != null)
				{
					i++;
					jDMGABPEDFI.AGGAMCGBFAF(kGBGENDIMBC, lBMGKAJIDAJ2);
				}
			}
			return jDMGABPEDFI;
		}
		case "array":
		{
			List<XElement> list = GGBAJPGKOEM.Elements().ToList();
			PlistElementArray gHFPDLCPEBH = new PlistElementArray();
			{
				foreach (XElement item in list)
				{
					PlistElement lBMGKAJIDAJ = JPHIJFLDECK(item);
					if (lBMGKAJIDAJ != null)
					{
						gHFPDLCPEBH.AMMFNLMJJFM.Add(lBMGKAJIDAJ);
					}
				}
				return gHFPDLCPEBH;
			}
		}
		case "string":
			return new PlistElementString(GetText(GGBAJPGKOEM));
		case "integer":
		{
			int result;
			if (int.TryParse(GetText(GGBAJPGKOEM), out result))
			{
				return new PlistElementInteger(result);
			}
			return null;
		}
		case "true":
			return new PlistElementBoolean(true);
		case "false":
			return new PlistElementBoolean(false);
		default:
			return null;
		}
	}

	public void LJJFGDFHEDG(string path)
	{
		AJBOOGKEGID(File.ReadAllText(path));
	}

	public void ReadFromStream(TextReader JFOEFIABDEO)
	{
		AJBOOGKEGID(JFOEFIABDEO.ReadToEnd());
	}

	public void AJBOOGKEGID(string HCPNFPMHFCM)
	{
		XDocument xDocument = ParseXmlNoDtd(HCPNFPMHFCM);
		version = (string)xDocument.Root.Attribute("version");
		XElement gGBAJPGKOEM = xDocument.XPathSelectElement("plist/dict");
		PlistElement lBMGKAJIDAJ = JPHIJFLDECK(gGBAJPGKOEM);
		if (lBMGKAJIDAJ == null)
		{
			throw new Exception("Error parsing plist file");
		}
		AFHNINCKJEE = lBMGKAJIDAJ as PlistElementDict;
		if (AFHNINCKJEE == null)
		{
			throw new Exception("Malformed plist file");
		}
	}

	private static XElement AOHOBAEHMCO(PlistElement NCDBEBJGHPP)
	{
		if (NCDBEBJGHPP is PlistElementBoolean)
		{
			PlistElementBoolean oFLCDAOIAOH = NCDBEBJGHPP as PlistElementBoolean;
			return new XElement((!oFLCDAOIAOH.value) ? "false" : "true");
		}
		if (NCDBEBJGHPP is PlistElementInteger)
		{
			PlistElementInteger kOANOMOBJFK = NCDBEBJGHPP as PlistElementInteger;
			return new XElement("integer", kOANOMOBJFK.value.ToString());
		}
		if (NCDBEBJGHPP is PlistElementString)
		{
			PlistElementString jEMBONBEBCN = NCDBEBJGHPP as PlistElementString;
			return new XElement("string", jEMBONBEBCN.value);
		}
		if (NCDBEBJGHPP is PlistElementDict)
		{
			PlistElementDict jDMGABPEDFI = NCDBEBJGHPP as PlistElementDict;
			XElement xElement = new XElement("dict");
			{
				foreach (KeyValuePair<string, PlistElement> item in jDMGABPEDFI.NGEGAPEEGPN())
				{
					XElement content = new XElement("key", item.Key);
					XElement xElement2 = AOHOBAEHMCO(item.Value);
					if (xElement2 != null)
					{
						xElement.Add(content);
						xElement.Add(xElement2);
					}
				}
				return xElement;
			}
		}
		if (NCDBEBJGHPP is PlistElementArray)
		{
			PlistElementArray gHFPDLCPEBH = NCDBEBJGHPP as PlistElementArray;
			XElement xElement3 = new XElement("array");
			{
				foreach (PlistElement item2 in gHFPDLCPEBH.AMMFNLMJJFM)
				{
					XElement xElement4 = AOHOBAEHMCO(item2);
					if (xElement4 != null)
					{
						xElement3.Add(xElement4);
					}
				}
				return xElement3;
			}
		}
		return null;
	}

	public void OBPOMPDFKAD(string path)
	{
		File.WriteAllText(path, AEGHEINPNIM());
	}

	public void WriteToStream(TextWriter MOAHDLDDEDF)
	{
		MOAHDLDDEDF.Write(AEGHEINPNIM());
	}

	public string AEGHEINPNIM()
	{
		XElement content = AOHOBAEHMCO(AFHNINCKJEE);
		XElement xElement = new XElement("plist");
		xElement.Add(new XAttribute("version", version));
		xElement.Add(content);
		XDocument xDocument = new XDocument();
		xDocument.Add(xElement);
		return CleanDtdToString(xDocument);
	}
}
