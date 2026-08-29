using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Content
{
	public static class InternalSettingsCompatibility
	{
		private static XmlElement FindMatchingChild(XmlElement target, XmlElement fallbackChild)
		{
			string[] identityAttributes = { "Name", "Type", "ID", "Gems", "PlatformID" };
			foreach (XmlNode childNode in target.ChildNodes)
			{
				XmlElement child = childNode as XmlElement;
				if (child == null || child.Name != fallbackChild.Name)
				{
					continue;
				}
				bool keyed = false;
				bool matches = true;
				foreach (string identityAttribute in identityAttributes)
				{
					if (!fallbackChild.HasAttribute(identityAttribute))
					{
						continue;
					}
					keyed = true;
					if (child.GetAttribute(identityAttribute) != fallbackChild.GetAttribute(identityAttribute))
					{
						matches = false;
						break;
					}
				}
				if (!keyed || matches)
				{
					return child;
				}
			}
			return null;
		}

		public static int MergeMissingSettings(
			XmlDocument document,
			XmlElement target,
			XmlElement fallback)
		{
			int imported = 0;
			foreach (XmlAttribute attribute in fallback.Attributes)
			{
				if (!target.HasAttribute(attribute.Name))
				{
					target.SetAttribute(attribute.Name, attribute.Value);
					imported++;
				}
			}
			foreach (XmlNode fallbackNode in fallback.ChildNodes)
			{
				XmlElement fallbackChild = fallbackNode as XmlElement;
				if (fallbackChild == null)
				{
					continue;
				}
				XmlElement targetChild = FindMatchingChild(target, fallbackChild);
				if (targetChild == null)
				{
					target.AppendChild(document.ImportNode(fallbackChild, true));
					imported++;
					continue;
				}
				imported += MergeMissingSettings(document, targetChild, fallbackChild);
			}
			return imported;
		}

		public static int RemoveUnsupportedQualityOptions(XmlDocument document)
		{
			int removed = 0;
			XmlNodeList options = document.SelectNodes("/Settings/QualityOptions/Option[@Name]");
			List<XmlNode> unsupported = new List<XmlNode>();
			foreach (XmlNode option in options)
			{
				string name = option.Attributes["Name"].Value;
				if (name != "ReduceFPS" && name != "ParticlesOff" && name != "SequencesOff")
				{
					unsupported.Add(option);
				}
			}
			foreach (XmlNode option in unsupported)
			{
				option.ParentNode.RemoveChild(option);
				removed++;
			}
			return removed;
		}

		public static bool DisableAlwaysMagicMode(XmlDocument document)
		{
			XmlElement alwaysMagic = document.SelectSingleNode("/Settings/AlwaysMagicMode") as XmlElement;
			if (alwaysMagic == null || alwaysMagic.GetAttribute("Value") == "0")
			{
				return false;
			}
			alwaysMagic.SetAttribute("Value", "0");
			return true;
		}
	}
}
