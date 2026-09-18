using System.Collections.Generic;
using System.Globalization;
using System.Xml;

public class RewardItem : Rewardable
{
	public string Name;

	public uint UpgradeNumber;

	internal string UpgradeLevelExpression { get; private set; }

	internal string EclipseRewardId { get; private set; }

	internal int EclipseGrantIndex { get; private set; } = -1;

	internal bool HasEclipseGrantConfiguration => !string.IsNullOrEmpty(EclipseRewardId);

	private XmlElement _sourceNode;

	internal sealed class ConfiguredGrantEnchantment
	{
		internal string Name { get; }

		internal string Aspect { get; }

		internal string EclipseKind { get; }

		internal ConfiguredGrantEnchantment(string name, string aspect, string eclipseKind)
		{
			Name = name;
			Aspect = aspect;
			EclipseKind = eclipseKind;
		}
	}

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
		EclipseRewardId = node.Attributes["EclipseReward"].CIPOICEEIBK(string.Empty);
		int grantIndex;
		if (!string.IsNullOrEmpty(EclipseRewardId) &&
			int.TryParse(node.Attributes["EclipseGrant"].CIPOICEEIBK(string.Empty), NumberStyles.None,
				CultureInfo.InvariantCulture, out grantIndex)) EclipseGrantIndex = grantIndex;
		if (HasEclipseGrantConfiguration && node is XmlElement sourceElement)
			_sourceNode = (XmlElement)sourceElement.CloneNode(true);
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

	internal RewardItem CloneForConfiguredGrant(int level, IReadOnlyList<ConfiguredGrantEnchantment> enchantments)
	{
		if (_sourceNode == null) throw new System.InvalidOperationException("Reward item source XML is unavailable: " + Name);
		XmlElement item = (XmlElement)_sourceNode.CloneNode(true);
		item.SetAttribute("Level", level.ToString(CultureInfo.InvariantCulture));
		XmlNode oldEnchantments = item["Enchantments"];
		if (oldEnchantments != null) item.RemoveChild(oldEnchantments);
		if (enchantments != null && enchantments.Count != 0)
		{
			XmlElement enchantmentsNode = item.OwnerDocument.CreateElement("Enchantments");
			for (int i = 0; i < enchantments.Count; i++)
			{
				XmlElement perk = item.OwnerDocument.CreateElement("Perk");
				perk.SetAttribute("Name", enchantments[i].Name);
				if (!string.IsNullOrEmpty(enchantments[i].EclipseKind))
					perk.SetAttribute(PerkStruct.EclipseKindAttribute, enchantments[i].EclipseKind);
				if (!string.IsNullOrEmpty(enchantments[i].Aspect))
				{
					XmlElement set = item.OwnerDocument.CreateElement("Set");
					set.SetAttribute("Aspect", enchantments[i].Aspect);
					perk.AppendChild(set);
				}
				enchantmentsNode.AppendChild(perk);
			}
			item.AppendChild(enchantmentsNode);
		}
		return new RewardItem(item);
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
