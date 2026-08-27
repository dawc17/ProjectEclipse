using System.Collections.Generic;
using System.Xml;

public class AttributesRule : InFightRule
{
	private Dictionary<string, float> OCCMEFFDJEH = new Dictionary<string, float>();

	public AttributesRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleAttributes, EJPOJJKKICO, node)
	{
		foreach (GameUtils.AlignTargetAttribute item in GameUtils.FPIDOGKOPGC)
		{
			OCCMEFFDJEH[item.Name] = 0f;
		}
		Parse(node);
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		foreach (KeyValuePair<string, float> item in OCCMEFFDJEH)
		{
			float value = item.Value;
			switch (NDBMMPENJNJ)
			{
			case RuleAppliance.AppliancePlayer:
			{
				int OEMALIFPGPO2 = 0;
				oIFPCFEGFOB.NMNCKBPFCCP.IBLHIAHECLK.Get(item.Key, ref OEMALIFPGPO2);
				oIFPCFEGFOB.NMNCKBPFCCP.IBLHIAHECLK.Set(item.Key, OEMALIFPGPO2 + (int)value);
				break;
			}
			case RuleAppliance.ApplianceOpponent:
			{
				int OEMALIFPGPO = 0;
				oIFPCFEGFOB.AKBNKDBHCEO.IBLHIAHECLK.Get(item.Key, ref OEMALIFPGPO);
				oIFPCFEGFOB.AKBNKDBHCEO.IBLHIAHECLK.Set(item.Key, OEMALIFPGPO + (int)value);
				break;
			}
			default:
				LLLOJBFMONN.Error("AttributesRule::initRule - wrong appliance - %i", NDBMMPENJNJ);
				break;
			}
		}
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		foreach (XmlAttribute attribute in node.Attributes)
		{
			if (attribute.Name != "Round" && attribute.Name != "ApplyTo" && attribute.Name != "Eclipse" && attribute.Name != "WarriorPower")
			{
				if (!OCCMEFFDJEH.ContainsKey(attribute.Name))
				{
					OCCMEFFDJEH.Add(attribute.Name, attribute.ParseFloat());
				}
				else
				{
					OCCMEFFDJEH[attribute.Name] += attribute.ParseFloat();
				}
			}
			if (!(attribute.Name == "WarriorPower"))
			{
				continue;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, float> item in OCCMEFFDJEH)
			{
				list.Add(item.Key);
			}
			foreach (string item2 in list)
			{
				OCCMEFFDJEH[item2] += attribute.ParseFloat();
			}
		}
	}

	public Dictionary<string, float> MAKMDLMJNPO()
	{
		return OCCMEFFDJEH;
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new AttributesRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
