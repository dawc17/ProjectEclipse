using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Content
{
	public sealed class MoveTemplateCompatibilityIssue
	{
		public string MoveName { get; private set; }
		public string[] MissingTemplates { get; private set; }

		public MoveTemplateCompatibilityIssue(string moveName, string[] missingTemplates)
		{
			MoveName = moveName;
			MissingTemplates = missingTemplates;
		}
	}

	public static class MoveCompatibility
	{
		public static int MergeMissingLegacyDefinitions(XmlDocument moves, XmlDocument baseline)
		{
			int restored = 0;
			string[] sections = { "Templates", "Moves", "Triggers" };
			foreach (string sectionName in sections)
			{
				XmlNode targetSection = moves.SelectSingleNode("/Movesxml/" + sectionName);
				XmlNode sourceSection = baseline.SelectSingleNode("/Movesxml/" + sectionName);
				if (targetSection == null || sourceSection == null)
				{
					continue;
				}

				HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
				foreach (XmlNode existing in targetSection.ChildNodes)
				{
					XmlAttribute name = existing.Attributes == null ? null : existing.Attributes["Name"];
					if (name != null)
					{
						names.Add(name.Value);
					}
				}

				foreach (XmlNode legacy in sourceSection.ChildNodes)
				{
					XmlAttribute name = legacy.Attributes == null ? null : legacy.Attributes["Name"];
					if (name != null && names.Add(name.Value))
					{
						XmlElement imported = (XmlElement)moves.ImportNode(legacy, true);
						if (sectionName == "Moves")
						{
							imported.SetAttribute("UseLegacyTemplates", "1");
						}
						targetSection.AppendChild(imported);
						restored++;
					}
				}
			}

			if (restored != 0)
			{
				// Imported legacy moves still depend on their original template bodies.
				// Keep those definitions separate so modern flattened moves do not
				// inherit duplicate actions or obsolete restrictions.
				XmlElement legacyTemplates = moves.CreateElement("LegacyTemplates");
				foreach (XmlNode template in baseline.SelectNodes("/Movesxml/Templates/Template"))
				{
					legacyTemplates.AppendChild(moves.ImportNode(template, true));
				}
				moves.DocumentElement.AppendChild(legacyTemplates);
			}
			return restored;
		}

		public static List<MoveTemplateCompatibilityIssue> RemoveUnavailableTemplates(XmlDocument moves)
		{
			List<MoveTemplateCompatibilityIssue> issues = new List<MoveTemplateCompatibilityIssue>();
			XmlNode templatesNode = moves.SelectSingleNode("/Movesxml/Templates");
			if (templatesNode == null)
			{
				return issues;
			}

			HashSet<string> templateNames = new HashSet<string>(StringComparer.Ordinal);
			foreach (XmlNode templateNode in templatesNode.ChildNodes)
			{
				XmlAttribute name = templateNode.Attributes == null ? null : templateNode.Attributes["Name"];
				if (name != null && !string.IsNullOrEmpty(name.Value))
				{
					templateNames.Add(name.Value);
				}
			}

			XmlNodeList templatedNodes = moves.SelectNodes(
				"/Movesxml/Templates/Template[@Template] | /Movesxml/Moves/Move[@Template]");
			foreach (XmlNode templatedNode in templatedNodes)
			{
				XmlAttribute attribute = templatedNode.Attributes["Template"];
				List<string> compatible = new List<string>();
				List<string> missing = new List<string>();
				foreach (string templateName in attribute.Value.Split('|'))
				{
					if (templateNames.Contains(templateName))
					{
						compatible.Add(templateName);
					}
					else if (!string.IsNullOrEmpty(templateName))
					{
						missing.Add(templateName);
					}
				}
				if (missing.Count == 0)
				{
					continue;
				}

				if (compatible.Count == 0)
				{
					templatedNode.Attributes.Remove(attribute);
				}
				else
				{
					attribute.Value = string.Join("|", compatible.ToArray());
				}

				XmlAttribute nameAttribute = templatedNode.Attributes["Name"];
				string moveName = nameAttribute == null ? templatedNode.Name : nameAttribute.Value;
				issues.Add(new MoveTemplateCompatibilityIssue(moveName, missing.ToArray()));
			}
			return issues;
		}
	}
	public sealed class BattleTypeMoveCondition : global::ConditionAnimation
	{
		private global::BattleType _expectedType;
		private readonly bool _hasExpectedType;

		public BattleTypeMoveCondition(XmlNode node)
			: base(DGAGKLODADD.BATTLE_TYPE)
		{
			string value = node == null || node.Attributes == null
				? string.Empty
				: XmlUtils.ParseString(node.Attributes["Value"], string.Empty);
			_hasExpectedType = Enum.TryParse(value, false, out _expectedType);
		}

		public override bool IsEqual(global::Model model, global::InfoAnimation animation)
		{
			global::Fight fight = global::Fight.OHNKFOHIAKG();
			global::FightList fightList = fight == null ? null : fight.OGNINOBBHIG();
			bool matches = _hasExpectedType && fightList != null && fightList.get_Type() == _expectedType;
			return IsNot ? !matches : matches;
		}
	}
	public sealed class BossAbilityStateMoveCondition : global::ConditionAnimation
	{
		private readonly int _expectedState;

		public BossAbilityStateMoveCondition(XmlNode node)
			: base(DGAGKLODADD.BOSS_ABILITY_STATE)
		{
			_expectedState = node == null || node.Attributes == null
				? 0
				: node.Attributes["Value"].ParseInt(0);
		}

		public override bool IsEqual(global::ModelConditions conditions)
		{
			bool matches = conditions != null && conditions.BossAbilityState == _expectedState;
			return IsNot ? !matches : matches;
		}
	}

	public sealed class CameraWeightMoveAction : global::ActionAnimation
	{
		public float MeWeight { get; private set; }
		public float EnemyWeight { get; private set; }
		public float Time { get; private set; }
		public float Delay { get; private set; }

		public CameraWeightMoveAction(XmlNode node)
			: base(FADAJCEEKIO.CAMERA_WEIGHT)
		{
			Parse(node);
			MeWeight = node.Attributes["MeWeight"].ParseFloat(0.5f);
			EnemyWeight = node.Attributes["EnemyWeight"].ParseFloat(0.5f);
			Time = node.Attributes["Time"].ParseFloat();
			Delay = node.Attributes["Delay"].ParseFloat();
		}

		public override void Visit(global::Model model)
		{
			// Every CameraWeight node in vanilla 2.41.9 is attached to a FightPVP
			// move. Eclipse currently has no PVP camera/state subsystem to receive
			// these weights, so retain and parse the data without fabricating camera
			// semantics for normal fights. Implement this when PVP itself is restored.
		}
	}

	public sealed class EnableBossAbilityMoveAction : global::ActionAnimation
	{
		private readonly int _state;

		public EnableBossAbilityMoveAction(XmlNode node)
			: base(FADAJCEEKIO.ENABLE_BOSS_ABILITY)
		{
			Parse(node);
			_state = node == null || node.Attributes == null
				? 0
				: node.Attributes["Value"].ParseInt(0);
		}

		public override void Visit(global::Model model)
		{
			if (model == null)
			{
				return;
			}
			global::Model target = model.NMGNPBMFJKP(OJLDHGKPLNC());
			if (target == null)
			{
				target = model;
			}
			global::ModelConditions conditions = target.EBABHGHPLFK();
			if (conditions != null)
			{
				conditions.BossAbilityState = _state;
			}
		}
	}

}
