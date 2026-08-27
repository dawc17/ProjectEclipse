using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class RecoveredUserSpriteImporter
{
	private const string UserRoot = "Assets/Resources/ui/users";
	private const int AlphaPadding = 2;
	private static bool running;

	static RecoveredUserSpriteImporter()
	{
		EditorApplication.delayCall += ImportRecoveredPortraits;
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			EditorApplication.delayCall += ImportRecoveredPortraits;
		}
	}

	[MenuItem("SF2/Import Recovered User Portraits")]
	private static void ImportRecoveredPortraits()
	{
		if (running || EditorApplication.isPlayingOrWillChangePlaymode || !Directory.Exists(UserRoot))
		{
			return;
		}

		running = true;
		try
		{
			AssetDatabase.Refresh();
			int imported = 0;
			foreach (string file in Directory.GetFiles(UserRoot, "*.png", SearchOption.TopDirectoryOnly))
			{
				string texturePath = file.Replace('\\', '/');
				string spritePath = Path.ChangeExtension(texturePath, ".asset");
				if (File.Exists(spritePath))
				{
					continue;
				}

				try
				{
					if (CreateTrimmedSprite(texturePath, spritePath))
					{
						imported++;
					}
				}
				catch (Exception exception)
				{
					Debug.LogWarning("[RecoveredUsers] Could not import " + texturePath + ": " + exception.Message);
				}
			}

			if (imported > 0)
			{
				AssetDatabase.SaveAssets();
				Debug.Log("[RecoveredUsers] Created " + imported + " exact trimmed portrait sprites.");
			}
		}
		finally
		{
			running = false;
		}
	}

	private static bool CreateTrimmedSprite(string texturePath, string spritePath)
	{
		TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
		if (importer == null)
		{
			return false;
		}

		// Read alpha from the PNG itself. Temporarily making every Unity texture
		// readable would require a second import per portrait and causes a very
		// noticeable editor stall for the recovered library.
		Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
		if (!source.LoadImage(File.ReadAllBytes(texturePath), false))
		{
			UnityEngine.Object.DestroyImmediate(source);
			return false;
		}
		Color32[] pixels = source.GetPixels32();
		int sourceWidth = source.width;
		int sourceHeight = source.height;
		UnityEngine.Object.DestroyImmediate(source);

		importer.textureType = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Single;
		importer.spritePixelsPerUnit = 100f;
		importer.mipmapEnabled = false;
		importer.alphaIsTransparency = true;
		importer.isReadable = false;
		importer.filterMode = FilterMode.Bilinear;
		importer.wrapMode = TextureWrapMode.Clamp;
		importer.textureCompression = TextureImporterCompression.Uncompressed;
		importer.SaveAndReimport();

		Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
		if (texture == null)
		{
			return false;
		}

		int minX = sourceWidth;
		int minY = sourceHeight;
		int maxX = -1;
		int maxY = -1;
		for (int y = 0; y < sourceHeight; y++)
		{
			for (int x = 0; x < sourceWidth; x++)
			{
				if (pixels[y * sourceWidth + x].a == 0)
				{
					continue;
				}
				minX = Math.Min(minX, x);
				minY = Math.Min(minY, y);
				maxX = Math.Max(maxX, x);
				maxY = Math.Max(maxY, y);
			}
		}

		Rect rect;
		if (maxX < minX || maxY < minY)
		{
			rect = new Rect(0f, 0f, sourceWidth, sourceHeight);
		}
		else
		{
			minX = Math.Max(0, minX - AlphaPadding);
			minY = Math.Max(0, minY - AlphaPadding);
			maxX = Math.Min(sourceWidth - 1, maxX + AlphaPadding);
			maxY = Math.Min(sourceHeight - 1, maxY + AlphaPadding);
			rect = new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
		}

		Vector2 fullTextureCenter = new Vector2(sourceWidth * 0.5f, sourceHeight * 0.5f);
		Vector2 pivot = new Vector2(
			(fullTextureCenter.x - rect.x) / rect.width,
			(fullTextureCenter.y - rect.y) / rect.height);
		Sprite sprite = Sprite.Create(texture, rect, pivot, 100f, 0u, SpriteMeshType.Tight);
		sprite.name = Path.GetFileNameWithoutExtension(texturePath);
		AssetDatabase.CreateAsset(sprite, spritePath);
		return true;
	}
}
