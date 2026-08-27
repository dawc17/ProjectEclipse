using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public static class XmlUtils
{
	public enum EBLFEPIOMOL
	{
		Normal = 0,
		ForcedResourced = 1,
		ForcedExternal = 2
	}

	public const string OIGMHMDJOIC = "#comment";

	public static bool EDILLCEIKFI(XmlNode MEEAKLDGLDF)
	{
		return MEEAKLDGLDF.NodeType == XmlNodeType.Comment;
	}

	public static int ParseInt(this XmlAttribute CJBEMNNNHDM, int KDLNPAGLMHF = 0)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		int result;
		return (!int.TryParse(CJBEMNNNHDM.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static long ParseLong(this XmlAttribute CJBEMNNNHDM, long KDLNPAGLMHF = 0L)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		long result;
		return (!long.TryParse(CJBEMNNNHDM.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static uint ParseUint(this XmlAttribute CJBEMNNNHDM, uint KDLNPAGLMHF = 0u)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		uint result;
		return (!uint.TryParse(CJBEMNNNHDM.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static float ParseFloat(this XmlAttribute CJBEMNNNHDM, float KDLNPAGLMHF = 0f)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		float result;
		return (!float.TryParse(CJBEMNNNHDM.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static bool ParseBool(this XmlAttribute CJBEMNNNHDM, bool KDLNPAGLMHF = false)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		int result;
		return (!int.TryParse(CJBEMNNNHDM.Value, out result)) ? KDLNPAGLMHF : (result > 0);
	}

	public static string ParseString(XmlAttribute CJBEMNNNHDM, string KDLNPAGLMHF = null)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		return CJBEMNNNHDM.Value;
	}

	public static string CIPOICEEIBK(this XmlAttribute CJBEMNNNHDM, string KDLNPAGLMHF = null)
	{
		if (CJBEMNNNHDM == null)
		{
			return KDLNPAGLMHF;
		}
		return CJBEMNNNHDM.Value;
	}

	public static KeyValuePair<int, int> MMHOOIPHOMI(this XmlNode MEEAKLDGLDF, int JLJBICAKJJH = 0, int PPHPIDGJOCJ = 0)
	{
		if (MEEAKLDGLDF == null)
		{
			return new KeyValuePair<int, int>(JLJBICAKJJH, PPHPIDGJOCJ);
		}
		int key = MEEAKLDGLDF.Attributes["Min"].ParseInt(JLJBICAKJJH);
		int value = MEEAKLDGLDF.Attributes["Max"].ParseInt(PPHPIDGJOCJ);
		return new KeyValuePair<int, int>(key, value);
	}

	public static Vector2 JIIENECAAEH(this XmlNode MEEAKLDGLDF, float COPBJEEJIBB = 0f, float FLOKDJLEJCK = 0f)
	{
		Vector2 result = new Vector2(COPBJEEJIBB, FLOKDJLEJCK);
		if (MEEAKLDGLDF != null)
		{
			result.x = MEEAKLDGLDF.Attributes["In"].ParseFloat(COPBJEEJIBB);
			result.y = MEEAKLDGLDF.Attributes["Out"].ParseFloat(FLOKDJLEJCK);
		}
		return result;
	}

	public static XmlAttribute PNJPEDPDMCP(this XmlNode MEEAKLDGLDF)
	{
		if (MEEAKLDGLDF != null && MEEAKLDGLDF.Attributes.Count > 0)
		{
			return MEEAKLDGLDF.Attributes[0];
		}
		return null;
	}

	public static XmlAttribute MGCGBMLHIDP(this XmlNode MEEAKLDGLDF)
	{
		if (MEEAKLDGLDF != null && MEEAKLDGLDF.Attributes.Count > 0)
		{
			return MEEAKLDGLDF.Attributes[MEEAKLDGLDF.Attributes.Count - 1];
		}
		return null;
	}

	public static XmlAttribute Attribute(this XmlNode MEEAKLDGLDF, string PNPLADGGOJN)
	{
		if (MEEAKLDGLDF != null)
		{
			return MEEAKLDGLDF.Attributes[PNPLADGGOJN];
		}
		return null;
	}

	public static T IHKEMJJAEJO<T>(XmlAttribute CJBEMNNNHDM, T JEALBOJLKFM)
	{
		if (CJBEMNNNHDM == null)
		{
			return JEALBOJLKFM;
		}
		try
		{
			return (T)Enum.Parse(typeof(T), CJBEMNNNHDM.Value, true);
		}
		catch
		{
			return JEALBOJLKFM;
		}
	}

	public static bool Empty(this XmlAttribute CJBEMNNNHDM)
	{
		if (CJBEMNNNHDM == null)
		{
			return true;
		}
		return CJBEMNNNHDM.Value.Equals(string.Empty);
	}

	public static XmlElement ACBPMPMPKJJ(this XmlNode MEEAKLDGLDF, string JLEKBBJBLOE)
	{
		XmlDocument xmlDocument = ((!(MEEAKLDGLDF is XmlDocument)) ? MEEAKLDGLDF.OwnerDocument : ((XmlDocument)MEEAKLDGLDF));
		XmlElement xmlElement = xmlDocument.CreateElement(JLEKBBJBLOE);
		MEEAKLDGLDF.AppendChild(xmlElement);
		return xmlElement;
	}

	public static XmlNode LCOLFMJJDJE(this XmlNode MEEAKLDGLDF, XmlNode NBMGOEMJJAF)
	{
		XmlDocument xmlDocument = MEEAKLDGLDF.OwnerDocument;
		if (xmlDocument == null)
		{
			xmlDocument = MEEAKLDGLDF as XmlDocument;
		}
		XmlNode xmlNode = xmlDocument.ImportNode(NBMGOEMJJAF.Clone(), true);
		MEEAKLDGLDF.AppendChild(xmlNode);
		return xmlNode;
	}

	public static void LCOLFMJJDJE(this XmlNode MEEAKLDGLDF, XmlAttribute NBMGOEMJJAF)
	{
		((XmlElement)MEEAKLDGLDF).SetAttribute(NBMGOEMJJAF.Name, NBMGOEMJJAF.Value);
	}

	public static XmlAttribute LLIKNHNLGJJ(this XmlNode MEEAKLDGLDF, string MJMEBBCLHII)
	{
		XmlAttribute xmlAttribute = MEEAKLDGLDF.OwnerDocument.CreateAttribute(MJMEBBCLHII);
		MEEAKLDGLDF.Attributes.Append(xmlAttribute);
		return xmlAttribute;
	}

	public static XmlAttribute IHNEFFHCDDJ(this XmlNode MEEAKLDGLDF, string MJMEBBCLHII)
	{
		XmlAttribute xmlAttribute = MEEAKLDGLDF.OwnerDocument.CreateAttribute(MJMEBBCLHII);
		MEEAKLDGLDF.Attributes.Prepend(xmlAttribute);
		return xmlAttribute;
	}

	public static XmlNode KDPLHGGPJHN(this XmlDocument JMCOLDENNDH, string IMGCANJHPND)
	{
		XmlNode xmlNode = JMCOLDENNDH.CreateNode(XmlNodeType.Element, IMGCANJHPND, null);
		JMCOLDENNDH.AppendChild(xmlNode);
		return xmlNode;
	}

	public static XmlNode KDPLHGGPJHN(this XmlNode MEEAKLDGLDF, string IMGCANJHPND)
	{
		XmlNode xmlNode = MEEAKLDGLDF.OwnerDocument.CreateNode(XmlNodeType.Element, IMGCANJHPND, null);
		MEEAKLDGLDF.AppendChild(xmlNode);
		return xmlNode;
	}

	public static XmlNode LJGLMGNAFHJ(this XmlNode MEEAKLDGLDF, string name, string MJMEBBCLHII)
	{
		if (MEEAKLDGLDF != null)
		{
			foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
			{
				if (childNode.Name.Equals(name))
				{
					XmlAttribute xmlAttribute = childNode.Attributes[MJMEBBCLHII];
					if (xmlAttribute != null)
					{
						return childNode;
					}
				}
			}
		}
		return null;
	}

	public static XmlNode LJGLMGNAFHJ(this XmlNode MEEAKLDGLDF, string name, string MJMEBBCLHII, string FOOKNBHPOOA)
	{
		if (MEEAKLDGLDF != null)
		{
			foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
			{
				if (childNode.Name.Equals(name))
				{
					XmlAttribute xmlAttribute = childNode.EKEKDENJPDP(MJMEBBCLHII, FOOKNBHPOOA);
					if (xmlAttribute != null)
					{
						return childNode;
					}
				}
			}
		}
		return null;
	}

	public static XmlAttribute EKEKDENJPDP(this XmlNode MEEAKLDGLDF, string MJMEBBCLHII, string FOOKNBHPOOA)
	{
		if (MEEAKLDGLDF != null)
		{
			foreach (XmlAttribute attribute in MEEAKLDGLDF.Attributes)
			{
				if (attribute.Name.Equals(MJMEBBCLHII) && attribute.Value.Equals(FOOKNBHPOOA))
				{
					return attribute;
				}
			}
		}
		return null;
	}

	public static XmlDocument BOJDEHMPJIL(byte[] OIOHECBCFJA, bool LELJDDBPCNL = true)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = LELJDDBPCNL;
		try
		{
			MemoryStream stream = new MemoryStream(OIOHECBCFJA);
			using (XmlReader aEHOOKGCGLO = XmlReader.Create(stream, xmlReaderSettings))
			{
				return OpenXMLDocument(aEHOOKGCGLO);
			}
		}
		catch
		{
			return null;
		}
	}

	public static XmlDocument DGOAOLEEMDG(string NGEPNAJJHCD, bool LELJDDBPCNL = true, bool PGKCOJBBOOH = false)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = LELJDDBPCNL;
		try
		{
			XmlDocument xmlDocument = GMLKAGPEOJB(NGEPNAJJHCD, xmlReaderSettings, PGKCOJBBOOH);
			if (xmlDocument == null)
			{
				Debug.LogError("Error open xml from string: " + NGEPNAJJHCD);
			}
			return xmlDocument;
		}
		catch
		{
			return null;
		}
	}

	public static XmlDocument OpenXMLDocument(string ONEIGMLOGDC, string LOBFDOKFJIP = "", EBLFEPIOMOL HDCCAKLHKBD = EBLFEPIOMOL.Normal, bool LELJDDBPCNL = true, bool PGKCOJBBOOH = false)
	{
		try
		{
			string text = ONEIGMLOGDC + ((!(LOBFDOKFJIP != string.Empty)) ? string.Empty : ("/" + LOBFDOKFJIP));
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = LELJDDBPCNL;
			// TexturePacker emits Apple plist files with a DOCTYPE declaration.
			// These files never need external entity resolution, but rejecting the
			// declaration makes every recovered location atlas look empty.
			xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
			xmlReaderSettings.XmlResolver = null;
			XmlDocument xmlDocument = null;
			switch (HDCCAKLHKBD)
			{
			case EBLFEPIOMOL.Normal:
				xmlDocument = ((!ONEIGMLOGDC.StartsWith(SF2Paths.FFKEDOBDLOL)) ? GMLKAGPEOJB(ResourceManager.GetText(text), xmlReaderSettings, PGKCOJBBOOH) : GMLKAGPEOJB(ResourceManager.KIHHJGJKMIC(text), xmlReaderSettings, PGKCOJBBOOH));
				break;
			case EBLFEPIOMOL.ForcedExternal:
				xmlDocument = GMLKAGPEOJB(ResourceManager.KIHHJGJKMIC(text), xmlReaderSettings, PGKCOJBBOOH);
				break;
			case EBLFEPIOMOL.ForcedResourced:
				xmlDocument = GMLKAGPEOJB(ResourceManager.IJMMFCDCOAC(text), xmlReaderSettings, PGKCOJBBOOH);
				break;
			}
			if (xmlDocument == null)
			{
				Debug.LogError("Error open xml " + text);
			}
			return xmlDocument;
		}
		catch
		{
			return null;
		}
	}

	private static XmlDocument GMLKAGPEOJB(string GHDPPHAAPCA, XmlReaderSettings ENBBEFMEILD, bool PGKCOJBBOOH)
	{
		if (PGKCOJBBOOH && (string.IsNullOrEmpty(GHDPPHAAPCA) || !GHDPPHAAPCA.TrimStart().StartsWith("<")))
		{
			string text = XmlCryptoUtils.OIMNHACBGNH(GHDPPHAAPCA);
			if (!string.IsNullOrEmpty(text))
			{
				GHDPPHAAPCA = text;
			}
		}
		using (XmlReader aEHOOKGCGLO = XmlReader.Create(new StringReader(GHDPPHAAPCA), ENBBEFMEILD))
		{
			return OpenXMLDocument(aEHOOKGCGLO);
		}
	}

	private static XmlDocument OpenXMLDocument(XmlReader AEHOOKGCGLO)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(AEHOOKGCGLO);
			return xmlDocument;
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Write(ex.ToString());
			return null;
		}
	}

	public static void CGACOAOCGJE(string AMNCLCPADOO, string IFIOLDFCLIE)
	{
		XmlDocument xmlDocument = OpenXMLDocument(AMNCLCPADOO, string.Empty, EBLFEPIOMOL.ForcedExternal);
		if (xmlDocument == null)
		{
			Debug.LogError("[XmlUtils]: try to trim spaces from incorrect xml - " + AMNCLCPADOO);
			return;
		}
		try
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = false;
			xmlWriterSettings.NewLineChars = string.Empty;
			using (XmlWriter xmlWriter = XmlWriter.Create(IFIOLDFCLIE, xmlWriterSettings))
			{
				xmlDocument.Save(xmlWriter);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void KINEBDGEJJL(string ONEIGMLOGDC)
	{
		CGACOAOCGJE(ONEIGMLOGDC, ONEIGMLOGDC);
	}

	public static string GKIGHFNLGIC(this XmlDocument GPIBAMAMGKD)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.OmitXmlDeclaration = true;
		xmlWriterSettings.Indent = true;
		using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
		{
			GPIBAMAMGKD.Save(xmlWriter);
		}
		return stringBuilder.ToString();
	}

	public static XmlDocument AIFIAKNJMHG(string ONEIGMLOGDC, string LOBFDOKFJIP = "", EBLFEPIOMOL HDCCAKLHKBD = EBLFEPIOMOL.Normal, bool LELJDDBPCNL = true)
	{
		XmlDocument xmlDocument = OpenXMLDocument(ONEIGMLOGDC, LOBFDOKFJIP, HDCCAKLHKBD, LELJDDBPCNL);
		if (xmlDocument != null && (HDCCAKLHKBD == EBLFEPIOMOL.Normal || HDCCAKLHKBD == EBLFEPIOMOL.ForcedExternal))
		{
			string oNEIGMLOGDC = ONEIGMLOGDC + ((!(LOBFDOKFJIP != string.Empty)) ? string.Empty : ("/" + LOBFDOKFJIP));
			UserDataValidator.CheckFileHash(xmlDocument, oNEIGMLOGDC);
		}
		return xmlDocument;
	}

	public static void ONLDJNLKKAL(XmlDocument JMCOLDENNDH, string KPFELJFPGHJ)
	{
		JMCOLDENNDH.Save(KPFELJFPGHJ);
		UserDataValidator.UpdateFileHash(JMCOLDENNDH, KPFELJFPGHJ);
	}

	public static void NLIKPADNFMF(string AMNCLCPADOO, string IFIOLDFCLIE)
	{
		XmlDocument xmlDocument = OpenXMLDocument(AMNCLCPADOO, string.Empty);
		xmlDocument.Save(IFIOLDFCLIE);
		UserDataValidator.UpdateFileHash(xmlDocument, IFIOLDFCLIE);
	}

	public static void IBPEILODDJP(string EFGLOMANJHN)
	{
		try
		{
			if (Path.GetExtension(EFGLOMANJHN).ToLower().Equals(".xml"))
			{
				XmlDocument lOBFDOKFJIP = OpenXMLDocument(EFGLOMANJHN, string.Empty);
				UserDataValidator.UpdateFileHash(lOBFDOKFJIP, EFGLOMANJHN);
			}
			else
			{
				UserDataValidator.UpdateFileHash(EFGLOMANJHN);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception: " + ex.Message);
		}
	}
}
