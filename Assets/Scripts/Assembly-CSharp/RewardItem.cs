using System.Collections.Generic;
using System.Xml;

public class RewardItem : Rewardable
{
	public string Name;

	public uint UpgradeNumber;

	internal string UpgradeLevelExpression { get; private set; }

	protected string JNPPCEGFJLE;

	public List<PerkStruct> LDLPCOFHFKE = new List<PerkStruct>();

	public RewardItem(XmlNode node)
	{
		Parse(node);
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_ITEM;
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		JNPPCEGFJLE = node.Attributes["Level"].CIPOICEEIBK(string.Empty);
		UpgradeNumber = node.Attributes["UpgradeNumber"].ParseUint();
		UpgradeLevelExpression = node.Attributes["UpgradeLevel"].CIPOICEEIBK(string.Empty);
		if (UpgradeLevelExpression.Length != 0 && node.Attributes["UpgradeNumber"] != null)
		{
			throw new System.FormatException("Reward item cannot specify both UpgradeLevel and UpgradeNumber: " + Name);
		}
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
		return EvaluateLevelExpression(JNPPCEGFJLE).ToInt();
	}

	internal int EvaluateUpgradeLevel()
	{
		int value;
		if (!int.TryParse(EvaluateLevelExpression(UpgradeLevelExpression), out value))
			throw new System.FormatException("Reward upgrade level must evaluate to an integer: " + Name);
		return value;
	}

	private string EvaluateLevelExpression(string expression)
	{
		FunctionExtension oPIFBDJNMKD = new FunctionExtension();
		oPIFBDJNMKD.Parse(expression);
		oPIFBDJNMKD.PBPBNENGLPA(HJFEFJIEINN);
		oPIFBDJNMKD.DMPCFMACDJM(OKPFNCJFLDL);
		FunctionResult dEIHAOLOPLC = oPIFBDJNMKD.IBCPKBBAFNH();
		return dEIHAOLOPLC.DCJLKCFKCOM;
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
