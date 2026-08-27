using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RecoveredSkillIconImporter
{
	private static readonly string[] IconRoots =
	{
		"Assets/Resources/UI/Skills",
		"Assets/Resources/UI/Enchantments"
	};
	private static bool scheduled;

	static RecoveredSkillIconImporter()
	{
		ScheduleImport();
	}

	internal static void ScheduleImport()
	{
		if (scheduled)
		{
			return;
		}
		scheduled = true;
		EditorApplication.delayCall += ImportIcons;
	}

	[MenuItem("SF2/Import Recovered Skill Icons")]
	private static void ImportIcons()
	{
		scheduled = false;
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			return;
		}

		AssetDatabase.Refresh();
		int imported = 0;
		foreach (string iconRoot in IconRoots)
		{
			if (!Directory.Exists(iconRoot))
			{
				continue;
			}

			foreach (string file in Directory.GetFiles(iconRoot, "*.png", SearchOption.TopDirectoryOnly))
			{
				string assetPath = file.Replace('\\', '/');
				TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
				if (importer == null ||
					(importer.textureType == TextureImporterType.Sprite &&
					 importer.spriteImportMode == SpriteImportMode.Single &&
					 !importer.mipmapEnabled && importer.alphaIsTransparency &&
					 importer.filterMode == FilterMode.Bilinear && importer.wrapMode == TextureWrapMode.Clamp &&
					 importer.textureCompression == TextureImporterCompression.Uncompressed))
				{
					continue;
				}

				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Single;
				importer.mipmapEnabled = false;
				importer.alphaIsTransparency = true;
				importer.filterMode = FilterMode.Bilinear;
				importer.wrapMode = TextureWrapMode.Clamp;
				importer.textureCompression = TextureImporterCompression.Uncompressed;
				importer.SaveAndReimport();
				imported++;
			}
		}

		if (imported > 0)
		{
			AssetDatabase.SaveAssets();
			Debug.Log("[RecoveredSkills] Imported " + imported + " standalone skill icons.");
		}
	}
}

internal sealed class RecoveredSkillIconPostprocessor : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach (string path in importedAssets)
		{
			if ((path.StartsWith("Assets/Resources/UI/Skills/", StringComparison.OrdinalIgnoreCase) ||
				path.StartsWith("Assets/Resources/UI/Enchantments/", StringComparison.OrdinalIgnoreCase)) &&
				path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
			{
				RecoveredSkillIconImporter.ScheduleImport();
				return;
			}
		}
	}
}
