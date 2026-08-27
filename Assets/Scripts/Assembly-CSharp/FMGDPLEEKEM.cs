using System.Collections.Generic;
using System.Xml;
using SimpleJSON;

public class FMGDPLEEKEM : FNEEAGNNFNN
{
	public void GGGEHAGCLGC(bool AJAJBBKANGD)
	{
		ListSF.ELEBLBJKDBI().HandleAuthenticateResult(AJAJBBKANGD);
	}

	public bool JAMADKCIMMB(XmlAttribute GICKLJAIHFC)
	{
		if (GICKLJAIHFC == null)
		{
			return false;
		}
		KFOGMCKGJOE(GICKLJAIHFC.Value);
		GICKLJAIHFC.OwnerElement.RemoveAttributeNode(GICKLJAIHFC);
		GGGEHAGCLGC(true);
		return true;
	}

	private void KFOGMCKGJOE(string GHDPPHAAPCA)
	{
		List<JLDHCFFAIPK> list = ICFMIHIKGOD.MDLJADJGDOL();
		List<JLDHCFFAIPK> list2 = ICFMIHIKGOD.JJIKCAEFPIO();
		list.Clear();
		list2.Clear();
		JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
		bool NPEJDEBKFDA = false;
		foreach (JSONClass child in jSONNode.Children)
		{
			JLDHCFFAIPK item = FKOJNMFMHMB(child, ref NPEJDEBKFDA);
			if (NPEJDEBKFDA)
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
		}
	}

	public void BPEPGALPBAE(XmlNode MEEAKLDGLDF)
	{
		List<JLDHCFFAIPK> list = ICFMIHIKGOD.MDLJADJGDOL();
		List<JLDHCFFAIPK> list2 = ICFMIHIKGOD.JJIKCAEFPIO();
		list.Clear();
		list2.Clear();
		XmlNode xmlNode = MEEAKLDGLDF["InProgress"];
		XmlNode xmlNode2 = MEEAKLDGLDF["Completed"];
		if (xmlNode != null)
		{
			foreach (XmlNode item2 in xmlNode)
			{
				JLDHCFFAIPK item = LNNDIPDCELI(item2);
				list.Add(item);
			}
		}
		if (xmlNode2 == null)
		{
			return;
		}
		foreach (XmlNode item3 in xmlNode2)
		{
			JLDHCFFAIPK item = COBPMKLJGPE(item3);
			list2.Add(item);
		}
	}

	public void INECBIPEJNL(XmlNode MEEAKLDGLDF)
	{
		List<JLDHCFFAIPK> list = ICFMIHIKGOD.MDLJADJGDOL();
		List<JLDHCFFAIPK> list2 = ICFMIHIKGOD.JJIKCAEFPIO();
		MEEAKLDGLDF.RemoveAll();
		if (list.Count > 0)
		{
			XmlNode xmlNode = MEEAKLDGLDF["InProgress"] ?? MEEAKLDGLDF.ACBPMPMPKJJ("InProgress");
			xmlNode.RemoveAll();
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				IAEMOFMBCNP(list[i], xmlNode);
			}
		}
		if (list2.Count > 0)
		{
			XmlNode xmlNode2 = MEEAKLDGLDF["Completed"] ?? MEEAKLDGLDF.ACBPMPMPKJJ("Completed");
			xmlNode2.RemoveAll();
			int j = 0;
			for (int count2 = list2.Count; j < count2; j++)
			{
				FFMAKILOJBM(list2[j], xmlNode2);
			}
		}
	}

	private static JLDHCFFAIPK FKOJNMFMHMB(JSONClass MEEAKLDGLDF, ref bool NPEJDEBKFDA)
	{
		string bGMLFNGKDHI = ((!MEEAKLDGLDF.HasValue("orderID")) ? string.Empty : MEEAKLDGLDF["orderID"].Value);
		string oDJCLFJHKFP = ((!MEEAKLDGLDF.HasValue("productID")) ? string.Empty : MEEAKLDGLDF["productID"].Value);
		string dNHKNDPBGNM = ((!MEEAKLDGLDF.HasValue("receipt")) ? string.Empty : MEEAKLDGLDF["receipt"].Value);
		string text = ((!MEEAKLDGLDF.HasValue("dataSignature")) ? null : MEEAKLDGLDF["dataSignature"].Value);
		text = ((text != null || !MEEAKLDGLDF.HasValue("data") || !MEEAKLDGLDF["data"].HasValue("signature")) ? null : MEEAKLDGLDF["data"]["signature"].Value);
		string pPJBKHKCONC = ((!MEEAKLDGLDF.HasValue("data") || !MEEAKLDGLDF["data"].HasValue("receiptPurchaseDate")) ? string.Empty : MEEAKLDGLDF["data"]["receiptPurchaseDate"].Value);
		bool flag = MEEAKLDGLDF.HasValue("isConfirmed") && MEEAKLDGLDF["isConfirmed"].AsBool;
		bool flag2 = MEEAKLDGLDF.HasValue("isDelivered") && MEEAKLDGLDF["isDelivered"].AsBool;
		bool flag3 = MEEAKLDGLDF.HasValue("isRestore") && MEEAKLDGLDF["isRestore"].AsBool;
		bool flag4 = MEEAKLDGLDF.HasValue("isInProgress") && MEEAKLDGLDF["isInProgress"].AsBool;
		bool flag5 = MEEAKLDGLDF.HasValue("isVerificationFail") && MEEAKLDGLDF["isVerificationFail"].AsBool;
		JLDHCFFAIPK jLDHCFFAIPK;
		if (!flag && (flag4 || flag2))
		{
			jLDHCFFAIPK = ((!flag2) ? JLDHCFFAIPK.KMIMHNOGDBI(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, text) : JLDHCFFAIPK.OCFHBLHDFED(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, text, pPJBKHKCONC));
			NPEJDEBKFDA = true;
		}
		else
		{
			jLDHCFFAIPK = (flag2 ? JLDHCFFAIPK.PCDJBFCLKED(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, text, pPJBKHKCONC) : ((!flag5) ? JLDHCFFAIPK.KADDCOFNAEC(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, text) : JLDHCFFAIPK.DBNJNPBIJGD(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, text)));
			NPEJDEBKFDA = false;
		}
		jLDHCFFAIPK.CCLPNJMEMCG(false);
		return jLDHCFFAIPK;
	}

	private static JLDHCFFAIPK LNNDIPDCELI(XmlNode MEEAKLDGLDF)
	{
		string bGMLFNGKDHI = MEEAKLDGLDF.Attributes["Id"].CIPOICEEIBK(string.Empty);
		string oDJCLFJHKFP = MEEAKLDGLDF.Attributes["ProductId"].CIPOICEEIBK(string.Empty);
		string dNHKNDPBGNM = MEEAKLDGLDF.Attributes["Receipt"].CIPOICEEIBK(string.Empty);
		string bGLGHEMMANM = MEEAKLDGLDF.Attributes["Signature"].CIPOICEEIBK();
		string pPJBKHKCONC = MEEAKLDGLDF.Attributes["Date"].CIPOICEEIBK(string.Empty);
		bool flag = MEEAKLDGLDF.Attributes["Verified"].ParseBool();
		bool bAINMLLIKOL = MEEAKLDGLDF.Attributes["Cheating"].ParseBool();
		JLDHCFFAIPK jLDHCFFAIPK = ((!flag) ? JLDHCFFAIPK.KMIMHNOGDBI(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, bGLGHEMMANM) : JLDHCFFAIPK.OCFHBLHDFED(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, bGLGHEMMANM, pPJBKHKCONC));
		jLDHCFFAIPK.CCLPNJMEMCG(bAINMLLIKOL);
		return jLDHCFFAIPK;
	}

	private static JLDHCFFAIPK COBPMKLJGPE(XmlNode MEEAKLDGLDF)
	{
		string bGMLFNGKDHI = MEEAKLDGLDF.Attributes["Id"].CIPOICEEIBK(string.Empty);
		string oDJCLFJHKFP = MEEAKLDGLDF.Attributes["ProductId"].CIPOICEEIBK(string.Empty);
		string dNHKNDPBGNM = MEEAKLDGLDF.Attributes["Receipt"].CIPOICEEIBK(string.Empty);
		string bGLGHEMMANM = MEEAKLDGLDF.Attributes["Signature"].CIPOICEEIBK();
		string pPJBKHKCONC = MEEAKLDGLDF.Attributes["Date"].CIPOICEEIBK(string.Empty);
		bool flag = MEEAKLDGLDF.Attributes["Verified"].ParseBool();
		bool bAINMLLIKOL = MEEAKLDGLDF.Attributes["Cheating"].ParseBool();
		JLDHCFFAIPK jLDHCFFAIPK = (flag ? JLDHCFFAIPK.PCDJBFCLKED(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, bGLGHEMMANM, pPJBKHKCONC) : ((MEEAKLDGLDF.Attributes["VerificationFailed"] == null) ? JLDHCFFAIPK.KADDCOFNAEC(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, bGLGHEMMANM) : JLDHCFFAIPK.DBNJNPBIJGD(oDJCLFJHKFP, bGMLFNGKDHI, dNHKNDPBGNM, bGLGHEMMANM)));
		jLDHCFFAIPK.CCLPNJMEMCG(bAINMLLIKOL);
		return jLDHCFFAIPK;
	}

	private static void IAEMOFMBCNP(JLDHCFFAIPK PAENLDALDGB, XmlNode JIIIKGLGCBJ)
	{
		XmlElement xmlElement = JIIIKGLGCBJ.ACBPMPMPKJJ("Payment");
		xmlElement.SetAttribute("Id", PAENLDALDGB.EJFAHFANGFM());
		xmlElement.SetAttribute("ProductId", PAENLDALDGB.JLDEALIEEJI());
		xmlElement.SetAttribute("Receipt", PAENLDALDGB.KGNGCPEGMJP());
		xmlElement.SetAttribute("Verified", (!PAENLDALDGB.IIHBCPBNCCB()) ? "0" : "1");
		if (PAENLDALDGB.MCDDGNJEKEO() != null)
		{
			xmlElement.SetAttribute("Signature", PAENLDALDGB.MCDDGNJEKEO());
		}
		if (PAENLDALDGB.HLCBFCOBIKP())
		{
			xmlElement.SetAttribute("Cheating", "1");
		}
		if (PAENLDALDGB.IIHBCPBNCCB())
		{
			xmlElement.SetAttribute("Date", PAENLDALDGB.HCDLKBDNMOE());
		}
	}

	public void FFMAKILOJBM(JLDHCFFAIPK PAENLDALDGB, XmlNode JIIIKGLGCBJ)
	{
		XmlElement xmlElement = JIIIKGLGCBJ.ACBPMPMPKJJ("Payment");
		xmlElement.SetAttribute("Id", PAENLDALDGB.EJFAHFANGFM());
		xmlElement.SetAttribute("ProductId", PAENLDALDGB.JLDEALIEEJI());
		xmlElement.SetAttribute("Receipt", PAENLDALDGB.KGNGCPEGMJP());
		xmlElement.SetAttribute("Verified", (!PAENLDALDGB.IIHBCPBNCCB()) ? "0" : "1");
		if (PAENLDALDGB.MCDDGNJEKEO() != null)
		{
			xmlElement.SetAttribute("Signature", PAENLDALDGB.MCDDGNJEKEO());
		}
		if (PAENLDALDGB.HLCBFCOBIKP())
		{
			xmlElement.SetAttribute("Cheating", "1");
		}
		if (PAENLDALDGB.IIHBCPBNCCB())
		{
			xmlElement.SetAttribute("Date", PAENLDALDGB.HCDLKBDNMOE());
		}
		else if (PAENLDALDGB.HPGPEHCMANA())
		{
			xmlElement.SetAttribute("VerificationFailed", "1");
		}
	}
}
