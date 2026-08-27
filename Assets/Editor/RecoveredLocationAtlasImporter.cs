using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RecoveredLocationAtlasImporter
{
	private const string TextureRoot = "Assets/Resources/Textures";
	private static bool importScheduled;
	private static bool importRunning;
	private static readonly Regex FramePattern = new Regex(
		@"\{\{(-?\d+),(-?\d+)\},\{(\d+),(\d+)\}\}",
		RegexOptions.Compiled);

	static RecoveredLocationAtlasImporter()
	{
		ScheduleImport();
	}

	internal static void ScheduleImport()
	{
		if (importScheduled)
		{
			return;
		}
		importScheduled = true;
		EditorApplication.delayCall += RunScheduledImport;
	}

	private static void RunScheduledImport()
	{
		importScheduled = false;
		ImportRecoveredAtlases();
	}

	[MenuItem("SF2/Import Recovered Texture Atlases")]
	private static void ImportRecoveredAtlases()
	{
		if (importRunning || EditorApplication.isPlayingOrWillChangePlaymode || !Directory.Exists(TextureRoot))
		{
			return;
		}

		importRunning = true;
		try
		{
			AssetDatabase.Refresh();
			int imported = 0;
			string[] metadataFiles = Directory.GetFiles(TextureRoot, "*_xml.txt", SearchOption.AllDirectories);
			foreach (string metadataFile in metadataFiles)
			{
				string normalizedMetadataPath = metadataFile.Replace('\\', '/');
				string texturePath = normalizedMetadataPath.Substring(0, normalizedMetadataPath.Length - "_xml.txt".Length) + ".png";
				if (!File.Exists(texturePath))
				{
					continue;
				}

				Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
				TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
				if (texture == null || importer == null)
				{
					continue;
				}

				List<SpriteMetaData> sprites;
				try
				{
					sprites = ReadSpriteMetadata(normalizedMetadataPath, texture.height);
				}
				catch (Exception exception)
				{
					Debug.LogWarning("[RecoveredAtlases] Could not parse " + normalizedMetadataPath + ": " + exception.Message);
					continue;
				}

				if (sprites.Count == 0)
				{
					continue;
				}

				bool settingsMatch = importer.textureType == TextureImporterType.Sprite &&
					importer.spriteImportMode == SpriteImportMode.Multiple &&
					Mathf.Approximately(importer.spritePixelsPerUnit, 1f) &&
					!importer.mipmapEnabled && importer.alphaIsTransparency &&
					importer.filterMode == FilterMode.Point && importer.wrapMode == TextureWrapMode.Clamp &&
					importer.textureCompression == TextureImporterCompression.Uncompressed &&
					HasMatchingSpriteSheet(importer.spritesheet, sprites);
				if (settingsMatch)
				{
					continue;
				}

				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Multiple;
				importer.spritePixelsPerUnit = 1f;
				importer.mipmapEnabled = false;
				importer.alphaIsTransparency = true;
				importer.filterMode = FilterMode.Point;
				importer.wrapMode = TextureWrapMode.Clamp;
				importer.textureCompression = TextureImporterCompression.Uncompressed;
				importer.spritesheet = sprites.ToArray();
				importer.SaveAndReimport();
				imported++;
			}

			if (imported > 0)
			{
				AssetDatabase.SaveAssets();
				Debug.Log("[RecoveredAtlases] Imported " + imported + " TexturePacker atlases.");
			}
		}
		finally
		{
			importRunning = false;
		}
	}

	private static List<SpriteMetaData> ReadSpriteMetadata(string metadataPath, int textureHeight)
	{
		XmlReaderSettings settings = new XmlReaderSettings
		{
			DtdProcessing = DtdProcessing.Ignore,
			XmlResolver = null
		};
		XmlDocument document = new XmlDocument();
		using (XmlReader reader = XmlReader.Create(metadataPath, settings))
		{
			document.Load(reader);
		}

		XmlNode frames = FindDictionary(document.SelectSingleNode("/plist/dict"), "frames");
		List<SpriteMetaData> result = new List<SpriteMetaData>();
		if (frames == null)
		{
			return result;
		}

		for (XmlNode key = frames.FirstChild; key != null; key = key.NextSibling)
		{
			if (key.NodeType != XmlNodeType.Element || key.Name != "key")
			{
				continue;
			}
			XmlNode frameDictionary = NextElement(key);
			if (frameDictionary == null || frameDictionary.Name != "dict")
			{
				continue;
			}

			string frameValue = FindValue(frameDictionary, "frame");
			Match match = FramePattern.Match(frameValue ?? string.Empty);
			if (!match.Success)
			{
				continue;
			}

			int x = int.Parse(match.Groups[1].Value);
			int cocosY = int.Parse(match.Groups[2].Value);
			int width = int.Parse(match.Groups[3].Value);
			int height = int.Parse(match.Groups[4].Value);
			bool rotated = string.Equals(FindValue(frameDictionary, "rotated"), "true", StringComparison.OrdinalIgnoreCase);
			int spriteWidth = rotated ? height : width;
			int spriteHeight = rotated ? width : height;
			int unityY = textureHeight - cocosY - spriteHeight;
			if (x < 0 || unityY < 0 || x + spriteWidth > int.MaxValue || unityY + spriteHeight > textureHeight)
			{
				continue;
			}

			SpriteMetaData sprite = new SpriteMetaData
			{
				name = Path.GetFileNameWithoutExtension(key.InnerText),
				rect = new Rect(x, unityY, spriteWidth, spriteHeight),
				alignment = (int)SpriteAlignment.Center,
				pivot = new Vector2(0.5f, 0.5f),
				border = Vector4.zero
			};
			result.Add(sprite);
			key = frameDictionary;
		}
		return result;
	}

	private static XmlNode FindDictionary(XmlNode dictionary, string keyName)
	{
		if (dictionary == null)
		{
			return null;
		}
		foreach (XmlNode child in dictionary.ChildNodes)
		{
			if (child.Name == "key" && child.InnerText == keyName)
			{
				return NextElement(child);
			}
		}
		return null;
	}

	private static string FindValue(XmlNode dictionary, string keyName)
	{
		XmlNode value = FindDictionary(dictionary, keyName);
		return value == null ? null : (value.Name == "true" || value.Name == "false" ? value.Name : value.InnerText);
	}

	private static XmlNode NextElement(XmlNode node)
	{
		for (XmlNode sibling = node.NextSibling; sibling != null; sibling = sibling.NextSibling)
		{
			if (sibling.NodeType == XmlNodeType.Element)
			{
				return sibling;
			}
		}
		return null;
	}

	private static bool HasMatchingSpriteSheet(SpriteMetaData[] existing, List<SpriteMetaData> expected)
	{
		if (existing == null || existing.Length != expected.Count)
		{
			return false;
		}
		Dictionary<string, SpriteMetaData> sprites = new Dictionary<string, SpriteMetaData>(StringComparer.Ordinal);
		foreach (SpriteMetaData sprite in existing)
		{
			sprites[sprite.name] = sprite;
		}
		foreach (SpriteMetaData sprite in expected)
		{
			SpriteMetaData actual;
			if (!sprites.TryGetValue(sprite.name, out actual) || actual.rect != sprite.rect ||
				actual.pivot != sprite.pivot || actual.border != sprite.border || actual.alignment != sprite.alignment)
			{
				return false;
			}
		}
		return true;
	}
}

internal sealed class RecoveredLocationAtlasPostprocessor : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach (string path in importedAssets)
		{
			if (path.StartsWith("Assets/Resources/Textures/", StringComparison.OrdinalIgnoreCase) &&
				(path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
				 path.EndsWith("_xml.txt", StringComparison.OrdinalIgnoreCase)))
			{
				RecoveredLocationAtlasImporter.ScheduleImport();
				return;
			}
		}
	}
}
