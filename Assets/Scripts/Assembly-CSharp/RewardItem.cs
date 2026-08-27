using System.Collections.Generic;
using System.Xml;

public class RewardItem : Rewardable
{
	public string Name;

	public uint UpgradeNumber;

	protected string JNPPCEGFJLE;

	public List<PerkStruct> LDLPCOFHFKE = new List<PerkStruct>();

	public RewardItem(XmlNode node)
	{
		Parse(node);
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_ITEM;
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		JNPPCEGFJLE = node.Attributes["Level"].CIPOICEEIBK(string.Empty);
		UpgradeNumber = node.Attributes["UpgradeNumber"].ParseUint();
		XmlNode xmlNode = node["Enchantments"];
		if (xmlNode == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			PerkStruct item = new PerkStruct(childNode);
			LDLPCOFHFKE.Add(item);
		}
	}

	public int CMEFKONFDKN()
	{
		FunctionExtension oPIFBDJNMKD = new FunctionExtension();
		oPIFBDJNMKD.Parse(JNPPCEGFJLE);
		oPIFBDJNMKD.PBPBNENGLPA(HJFEFJIEINN);
		oPIFBDJNMKD.DMPCFMACDJM(OKPFNCJFLDL);
		FunctionResult dEIHAOLOPLC = oPIFBDJNMKD.IBCPKBBAFNH();
		return dEIHAOLOPLC.DCJLKCFKCOM.ToInt();
	}

	public void OKPFNCJFLDL(FunctionExtension.CallbackResult DCJLKCFKCOM)
	{
	}

	public void HJFEFJIEINN(FunctionExtension.CallbackResult DCJLKCFKCOM)
	{
		FunctionExtension.GLBAFLLMOOH gLBAFLLMOOH = DCJLKCFKCOM.data as FunctionExtension.GLBAFLLMOOH;
		FunctionResult nAGGNMIFFGK = DCJLKCFKCOM.NAGGNMIFFGK;
		if (gLBAFLLMOOH.FJLOLCPJACB.Equals("Player"))
		{
			BJAOOMLBIHK(gLBAFLLMOOH, nAGGNMIFFGK);
		}
	}

	private void BJAOOMLBIHK(FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.HBDLDIKHFEG.Equals("Level"))
		{
			DCJLKCFKCOM.DCJLKCFKCOM = ListSF.CCDKHLAMKKO().PINDEKDNCNL().ToString();
		}
	}
}
