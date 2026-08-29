using System;
using System.Collections.Generic;
using System.Xml;

namespace SF2DE.Content
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
}
