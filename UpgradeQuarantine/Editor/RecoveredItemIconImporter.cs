using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RecoveredItemIconImporter
{
	private const string ItemRoot = "Assets/Resources/UI/Items";

	static RecoveredItemIconImporter()
	{
		EditorApplication.delayCall += ImportRecoveredIcons;
	}

	[MenuItem("SF2/Import Recovered Item Icons")]
	private static void ImportRecoveredIcons()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode || !Directory.Exists(ItemRoot))
		{
			return;
		}

		AssetDatabase.Refresh();
		int imported = 0;
		foreach (string file in Directory.GetFiles(ItemRoot, "*.png", SearchOption.TopDirectoryOnly))
		{
			string assetPath = file.Replace('\\', '/');
			string resourceName = Path.GetFileNameWithoutExtension(assetPath);
			bool isRecoveredStandalone = resourceName.Contains(".") ||
				resourceName.StartsWith("img_") || resourceName == "tickets";
			if (!isRecoveredStandalone)
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
			Debug.Log("[RecoveredItems] Imported " + imported + " standalone item icons.");
		}
	}
}
