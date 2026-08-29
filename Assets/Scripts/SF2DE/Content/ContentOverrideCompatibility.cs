using System;
using System.Collections.Generic;
using System.Xml;

namespace SF2DE.Content
{
	public static class ContentOverrideCompatibility
	{
		public static bool Validate(
			XmlDocument document,
			string fileName,
			bool isModelRequest,
			out string reason)
		{
			reason = null;
			if (isModelRequest)
			{
				XmlElement scene = document.DocumentElement;
				if (scene == null || !string.Equals(scene.Name, "Scene", StringComparison.Ordinal) ||
					scene["Figures"] == null)
				{
					reason = "model XML must have a <Scene> root and <Figures> section";
					return false;
				}
				return true;
			}

			string expectedRoot = null;
			if (string.Equals(fileName, "quests.xml", StringComparison.OrdinalIgnoreCase))
			{
				expectedRoot = "Quests";
			}
			else if (string.Equals(fileName, "list.xml", StringComparison.OrdinalIgnoreCase))
			{
				expectedRoot = "List";
			}
			else if (string.Equals(fileName, "stages.xml", StringComparison.OrdinalIgnoreCase))
			{
				expectedRoot = "Stages";
			}
			else if (string.Equals(fileName, "moves.xml", StringComparison.OrdinalIgnoreCase))
			{
				expectedRoot = "Movesxml";
			}

			if (expectedRoot != null && (document.DocumentElement == null ||
				!string.Equals(document.DocumentElement.Name, expectedRoot, StringComparison.Ordinal)))
			{
				reason = "expected <" + expectedRoot + "> root, found <" +
					(document.DocumentElement == null ? "none" : document.DocumentElement.Name) + ">";
				return false;
			}

			if (string.Equals(fileName, "moves.xml", StringComparison.OrdinalIgnoreCase))
			{
				XmlNode templatesNode = document.SelectSingleNode("/Movesxml/Templates");
				XmlNode movesNode = document.SelectSingleNode("/Movesxml/Moves");
				if (templatesNode == null || movesNode == null)
				{
					reason = "Movesxml must contain Templates and Moves sections";
					return false;
				}

				HashSet<string> templateNames = new HashSet<string>(StringComparer.Ordinal);
				foreach (XmlNode templateNode in templatesNode.ChildNodes)
				{
					XmlAttribute nameAttribute = templateNode.Attributes == null ? null : templateNode.Attributes["Name"];
					if (nameAttribute != null)
					{
						templateNames.Add(nameAttribute.Value);
					}
				}

				List<string> missingTemplates = new List<string>();
				XmlNodeList templatedNodes = document.SelectNodes(
					"/Movesxml/Templates/Template[@Template] | /Movesxml/Moves/Move[@Template]");
				foreach (XmlNode templatedNode in templatedNodes)
				{
					foreach (string templateName in templatedNode.Attributes["Template"].Value.Split('|'))
					{
						if (!templateNames.Contains(templateName) && !missingTemplates.Contains(templateName))
						{
							missingTemplates.Add(templateName);
						}
					}
				}
				if (missingTemplates.Count != 0)
				{
					reason = "undefined move templates: " + string.Join(", ", missingTemplates.ToArray());
					return false;
				}
			}

			if (!string.Equals(fileName, "internalSettings.xml", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			XmlNode settings = document["Settings"];
			string[] requiredSections =
			{
				"AssemblySettings",
				"Internet",
				"Supports",
				"EULA",
				"Log",
				"ForcedLogConditions",
				"StarterPackTimer"
			};
			List<string> missingSections = new List<string>();
			foreach (string section in requiredSections)
			{
				if (settings == null || settings[section] == null)
				{
					missingSections.Add(section);
				}
			}
			if (missingSections.Count == 0)
			{
				return true;
			}

			reason = "missing sections: " + string.Join(", ", missingSections.ToArray());
			return false;
		}
	}
}
