using System;
using System.Collections.Generic;
using System.IO;

namespace Eclipse.Content
{
	public static class ContentOverridePaths
	{
		public const string EmptyPacksManifest = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Packs />";

		public static bool TryGetGamedataRelativePath(string requestPath, out string relativePath)
		{
			string normalized = requestPath.Replace('\\', '/').TrimStart('/');
			int gamedataIndex = normalized.IndexOf("gamedata/", StringComparison.OrdinalIgnoreCase);
			if (gamedataIndex < 0)
			{
				relativePath = null;
				return false;
			}
			relativePath = normalized.Substring(gamedataIndex + "gamedata/".Length);
			return true;
		}

		public static bool IsModelRequest(string requestPath)
		{
			string normalizedPath = requestPath.Replace('\\', '/');
			return normalizedPath.IndexOf("/models/", StringComparison.OrdinalIgnoreCase) >= 0 ||
				normalizedPath.StartsWith("models/", StringComparison.OrdinalIgnoreCase) ||
				normalizedPath.StartsWith("assets/models/", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsPacksManifest(string gamedataRelativePath)
		{
			return string.Equals(Path.GetFileName(gamedataRelativePath), "packs.xml",
				StringComparison.OrdinalIgnoreCase);
		}

		public static List<string> BuildCandidates(string gamedataRelativePath)
		{
			List<string> candidates = new List<string>();
			candidates.Add(gamedataRelativePath);

			string normalizedRelativePath = gamedataRelativePath.Replace('\\', '/');
			if (normalizedRelativePath.EndsWith("/params.xml", StringComparison.OrdinalIgnoreCase))
			{
				string directory = Path.GetDirectoryName(gamedataRelativePath);
				string locationName = Path.GetFileName(directory);
				if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(locationName))
				{
					candidates.Add(Path.Combine(directory, locationName + "_params.xml"));
				}
			}
			if (gamedataRelativePath.StartsWith("assets/", StringComparison.OrdinalIgnoreCase))
			{
				candidates.Add(gamedataRelativePath.Substring("assets/".Length));
			}
			candidates.Add(Path.GetFileName(gamedataRelativePath));
			return candidates;
		}
	}
}
