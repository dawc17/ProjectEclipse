using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RecoveredEffectFrameImporter
{
	private const string EffectRoot = "Assets/Resources/textures/effects";
	private static bool scheduled;

	static RecoveredEffectFrameImporter()
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
		EditorApplication.delayCall += ImportFrames;
	}

	[MenuItem("SF2/Import Recovered Effect Frames")]
	private static void ImportFrames()
	{
		scheduled = false;
		if (EditorApplication.isPlayingOrWillChangePlaymode || !Directory.Exists(EffectRoot))
		{
			return;
		}

		AssetDatabase.Refresh();
		int imported = 0;
		foreach (string file in Directory.GetFiles(EffectRoot, "*.png", SearchOption.AllDirectories))
		{
			string assetPath = file.Replace('\\', '/');
			string directory = Path.GetDirectoryName(file);
			string atlasMetadata = Path.Combine(directory, Path.GetFileNameWithoutExtension(file) + "_xml.txt");
			if (File.Exists(atlasMetadata))
			{
				continue;
			}

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
			importer.spritePixelsPerUnit = 1f;
			importer.mipmapEnabled = false;
			importer.alphaIsTransparency = true;
			importer.filterMode = FilterMode.Bilinear;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.SaveAndReimport();
			imported++;
		}

		if (imported > 0)
		{
			AssetDatabase.SaveAssets();
			Debug.Log("[RecoveredEffects] Imported " + imported + " standalone effect frames.");
		}
	}
}

internal sealed class RecoveredEffectFramePostprocessor : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach (string path in importedAssets)
		{
			if (path.StartsWith("Assets/Resources/textures/effects/", StringComparison.OrdinalIgnoreCase) &&
				path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
			{
				RecoveredEffectFrameImporter.ScheduleImport();
				return;
			}
		}
	}
}
