using System.Collections.Generic;
using System.Xml;

public class RuleParser
{
	public static void EEPPJEMHBCK(XmlNode node, List<Rule> OEMALIFPGPO)
	{
		if (node == null)
		{
			return;
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string name = childNode.Name;
			if (name == "RulesWithConditions")
			{
				// This runtime predates conditional rule wrappers. Keep the contained
				// rules instead of rejecting the entire block. Their quest-style
				// Conditions remain in the source for a future conditional-rule port.
				EEPPJEMHBCK(childNode["RuleList"], OEMALIFPGPO);
				continue;
			}
			if (name == "Level")
			{
				FHIAKFOCBNK(childNode, OEMALIFPGPO);
				continue;
			}
			Rule gKAJMMNJBGA = LBDEIDNPJMO(childNode);
			if (gKAJMMNJBGA != null)
			{
				OEMALIFPGPO.Add(gKAJMMNJBGA);
			}
		}
	}

	public static Rule LBDEIDNPJMO(XmlNode node)
	{
		string name = node.Name;
		switch (name)
		{
		case "RequireItem":
			return new ItemRule(node);
		case "EquipItem":
			return new EquipItemRule(node);
		case "RandomAquiredItem":
			return new RandomAquiredItemRule(node);
		case "NoButton":
			return new NoButtonRule(node);
		case "NoAnimation":
			return new NoAnimationRule(node);
		case "Ringout":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleRingout, node);
		case "HotGround":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleHotGround, node);
		case "LoseFall":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleLoseFall, node);
		case "Regeneration":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleRegeneration, node);
		case "Attributes":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleAttributes, node);
		case "DamageFactor":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleDamageFactor, node);
		case "RemoveInterval":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleRemoveInterval, node);
		case "Crazy":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleCrazy, node);
		case "Lifesteal":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleLifeSteal, node);
		case "NoHealthBar":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleNoHealthBar, node);
		case "TimeOutWin":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleTimeoutWin, node);
		case "Combo":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleCombo, node);
		case "Darkness":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleDarkness, node);
		case "Points":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RulePoints, node);
		case "NoBulletsReplenishment":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleNoBulletsReplenishment, node);
		case "RechargeMagicEachRound":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound, node);
		case "Perk":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RulePerk, node);
		case "NoPerks":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleNoPerks, node);
		case "RandomRule":
			return new RandomRule(node);
		case "ComplexRule":
			return new ComplexRule(node);
		case "Description":
			return new DescriptionRule(node);
		case "WinCombo":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleWinCombo, node);
		case "WinStyle":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleWinStyle, node);
		case "WinShock":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleWinShock, node);
		case "ChangeFight":
			return new ChangeFightRule(node);
		case "SetTactic":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleTactic, node);
		case "InvertJoystick":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleInvertJoystick, node);
		case "RandomArea":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleRandomArea, node);
		case "RatingEvaluation":
			return new RatingEvaluationRule(node);
		case "Invulnerability":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleInvulnerability, node);
		case "CurrencyCost":
			return new CurrencyCostRule(node);
		case "RaidCurrencyCost":
			return new RaidCurrencyCostRule(node);
		case "Resistance":
			return HPFJOADKOEH(Rule.BCBLLMPAMLP.RuleResistance, node);
		case "Avatar":
			return new AvatarRule(node);
		case "Name":
			return new NameRule(node);
		default:
			LLLOJBFMONN.Error("RuleParser::parseRules - unknown node name: " + name);
			return null;
		}
	}

	public static FightStatistics.EMKEIEJMONM KMAKHHHMGMH(XmlNode node)
	{
		string text = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		switch (text)
		{
		case "Turtle":
			return FightStatistics.EMKEIEJMONM.STYLE_TURTLE;
		case "Hard":
			return FightStatistics.EMKEIEJMONM.STYLE_HARD;
		case "Brutal":
			return FightStatistics.EMKEIEJMONM.STYLE_BRUTAL;
		case "Aggressive":
			return FightStatistics.EMKEIEJMONM.STYLE_AGGRESSIVE;
		case "Crazy":
			return FightStatistics.EMKEIEJMONM.STYLE_CRAZY;
		case "Fantastic":
			return FightStatistics.EMKEIEJMONM.STYLE_FANTASTIC;
		default:
			LLLOJBFMONN.Error("RuleParser::parseStyleType - unknown type: " + text);
			return FightStatistics.EMKEIEJMONM.STYLE_TURTLE;
		}
	}

	protected static InFightRule HPFJOADKOEH(Rule.BCBLLMPAMLP LFLGCDNKNJI, XmlNode node)
	{
		string text = node.Attributes["ApplyTo"].CIPOICEEIBK("All");
		RuleAppliance eJPOJJKKICO = RuleAppliance.ApplianceNone;
		switch (text)
		{
		case "Player":
			eJPOJJKKICO = RuleAppliance.AppliancePlayer;
			break;
		case "Bot":
			eJPOJJKKICO = RuleAppliance.ApplianceOpponent;
			break;
		case "All":
			eJPOJJKKICO = RuleAppliance.ApplianceAll;
			break;
		default:
			LLLOJBFMONN.Error("RuleParser::parseInFightRule ERROR - wrong rule applyTo %s", text);
			break;
		}
		switch (LFLGCDNKNJI)
		{
		case Rule.BCBLLMPAMLP.RuleHotGround:
			return new HotGroundRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleRingout:
			return new RingOutRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleRegeneration:
			return new RegenerationRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleAttributes:
			return new AttributesRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleDamageFactor:
			return new DamageFactorRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleLoseFall:
			return new LoseFallRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleRemoveInterval:
			return new RemoveIntervalRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleCrazy:
			return new CrazyRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleLifeSteal:
			return new LifeStealRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleNoHealthBar:
			return new NoHealthBarRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleTimeoutWin:
			return new TimeoutWinRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleCombo:
			return new ComboRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleDarkness:
			return new DarknessRule(node, RuleAppliance.AppliancePlayer);
		case Rule.BCBLLMPAMLP.RulePoints:
			return new PointsRule(node, RuleAppliance.ApplianceAll);
		case Rule.BCBLLMPAMLP.RuleNoBulletsReplenishment:
			return new NoBulletsReplenishmentRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound:
			return new RechargeMagicEachRoundRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RulePerk:
			return new PerkRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleNoPerks:
			return new NoPerksRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleWinCombo:
			return new WinComboRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleWinStyle:
			return new WinStyleRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleWinShock:
			return new WinShockRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleTactic:
			return new TacticRule(node);
		case Rule.BCBLLMPAMLP.RuleInvertJoystick:
			return new InvertJoystickRule(node);
		case Rule.BCBLLMPAMLP.RuleRandomArea:
			return new RandomAreaRule(node);
		case Rule.BCBLLMPAMLP.RuleInvulnerability:
			return new InvulnerabilityRule(node, eJPOJJKKICO);
		case Rule.BCBLLMPAMLP.RuleResistance:
			return new ResistanceRule(node, eJPOJJKKICO);
		default:
			LLLOJBFMONN.Error("RuleParser::parseInFightRule ERROR - wrong rule type %i", LFLGCDNKNJI);
			return null;
		}
	}

	protected static void FHIAKFOCBNK(XmlNode node, List<Rule> OEMALIFPGPO)
	{
		int kLJOBCIINOF = node.Attributes["Min"].ParseInt();
		int nMPCMFDGOKA = node.Attributes["Max"].ParseInt(int.MaxValue);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Rule gKAJMMNJBGA = LBDEIDNPJMO(childNode);
			if (gKAJMMNJBGA != null)
			{
				gKAJMMNJBGA.NMPCMFDGOKA = nMPCMFDGOKA;
				gKAJMMNJBGA.KLJOBCIINOF = kLJOBCIINOF;
				OEMALIFPGPO.Add(gKAJMMNJBGA);
			}
		}
	}
}
