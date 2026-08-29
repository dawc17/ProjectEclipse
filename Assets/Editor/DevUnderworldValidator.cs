using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

// Explicit, read-only checks against Unity's imported assets. Does not launch
// fights, initialize a profile, modify progression, or edit the user's save.
internal static class DevUnderworldValidator
{
	[MenuItem("SF2/Validate Underworld Integration")]
	private static void Validate()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			Debug.LogWarning("[UnderworldValidation] Stop Play mode before validating imported assets.");
			return;
		}
		List<string> errors = new List<string>();
		HashSet<string> locations = new HashSet<string>(StringComparer.Ordinal);
		XmlDocument stages = Load("Assets/vanillaXml/raid_stages_default.xml");
		int battles = stages.SelectNodes("//Battle").Count;
		foreach (XmlNode node in stages.SelectNodes("//Battle[@Location] | //Fight[@Location]"))
			locations.Add(node.Attributes["Location"].Value);
		int checkedImages = 0;
		foreach (string location in locations)
		{
			string directory = "Assets/vanillaXml/locations/" + location;
			string[] files = Directory.Exists(directory) ? Directory.GetFiles(directory, "*params.xml") : new string[0];
			if (files.Length != 1)
			{
				errors.Add(location + ": expected exactly one custom params file");
				continue;
			}
			XmlDocument doc = Load(files[0]);
			if (doc.SelectNodes("/Root/Layer/ModelsViewer").Count != 1)
				errors.Add(location + ": missing/duplicate fighter placement");
			foreach (XmlNode layer in doc.SelectNodes("/Root/Layer"))
			{
				string folder = "Textures/" + (Attr(layer, "Path") ?? "Locations/" + location).Trim('/');
				string atlas = Attr(layer, "Atlas");
				Sprite[] members = string.IsNullOrEmpty(atlas) ? new Sprite[0] : Resources.LoadAll<Sprite>(folder + "/" + atlas);
				CheckMetadata(folder + "/" + atlas + "_xml", errors);
				foreach (XmlNode image in layer.SelectNodes("Image | SpriteMask | SimpleEffect[@Type='Picture']"))
				{
					string name = Attr(image, "ClassName");
					Sprite sprite = Array.Find(members, item => item.name == name) ?? Resources.Load<Sprite>(folder + "/" + name);
					checkedImages++;
					if (sprite == null || sprite.texture == null || sprite.rect.width <= 0 || sprite.rect.height <= 0 || sprite.vertices.Length < 3)
						errors.Add(location + ": invalid imported sprite " + folder + "/" + name);
				}
				foreach (XmlNode effect in layer.SelectNodes("SimpleEffect[@Type='Sequention']"))
				{
					string path = Attr(effect, "Path");
					string basePath = path != null ? "Textures/" + path.Trim('/') :
						Attr(effect, "PictureLocation") == "global" ? "Textures/Location_effects" : folder;
					string resource = basePath + "/Atlases/" + Attr(effect, "ClassName");
					if (Resources.Load<TextAsset>(resource + "_xml") == null || Resources.LoadAll<Sprite>(resource).Length == 0)
						errors.Add(location + ": missing imported animation " + resource);
					CheckMetadata(resource + "_xml", errors);
				}
				foreach (XmlNode effect in layer.SelectNodes("ParticleEffect | NewParticleEffect"))
				{
					string name = Attr(effect, "ClassName");
					GameObject prefab = Resources.Load<GameObject>("Textures/Location_effects/Particles/" + name);
					if (prefab == null || prefab.GetComponentInChildren<ParticleSystem>(true) == null)
						errors.Add(location + ": missing particle system " + name);
				}
			}
		}
		foreach (string name in new[] { "RaidMisc.shield", "RaidMap.raid_down", "RaidMap.raid_up", "RaidMap.raid_down_arrow", "RaidHardmodeUI.checkboxOn", "RaidHardmodeUI.checkboxGray" })
		{
			Sprite sprite = Resources.Load<Sprite>("ui/atlases/" + name);
			if (sprite == null || sprite.texture == null || sprite.uv.Length < 3)
				errors.Add("Invalid raid UI sprite " + name);
			else if (name.StartsWith("RaidMap.", StringComparison.Ordinal))
			{
				Vector4 uv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
				Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(sprite);
				if (uv.z <= uv.x || uv.w <= uv.y || padding != Vector4.zero)
					errors.Add("Invalid raid navigation UVs/padding " + name);
			}
		}
		foreach (string error in errors)
			Debug.LogError("[UnderworldValidation] " + error);
		if (errors.Count != 0)
			throw new InvalidOperationException("Underworld import validation failed: " + errors.Count + " issue(s).");
		Debug.Log("[UnderworldValidation] PASS: " + battles + " battles, " + locations.Count +
			" locations, " + checkedImages + " image/mask/effect references. Combat and visual playtesting still required.");
	}

	private static string Attr(XmlNode node, string key)
	{
		return node.Attributes[key] == null ? null : node.Attributes[key].Value;
	}

	private static XmlDocument Load(string path)
	{
		XmlDocument document = new XmlDocument { XmlResolver = null };
		document.Load(path);
		return document;
	}

	private static void CheckMetadata(string resource, List<string> errors)
	{
		TextAsset asset = Resources.Load<TextAsset>(resource);
		if (asset == null) return; // standalone sprites legitimately have no plist
		try
		{
			XmlDocument doc = new XmlDocument { XmlResolver = null };
			doc.LoadXml(asset.text);
			foreach (XmlNode value in doc.SelectNodes("//key[.='sourceSize']/following-sibling::string[1]"))
			{
				var frame = new CocosAnimationData.SpriteFrameCocos();
				frame.AAHNBCAFBMG(value.InnerText);
				if (frame.PFIECJPOFFB().x <= 0 || frame.PFIECJPOFFB().y <= 0)
					errors.Add(resource + ": nonpositive source size");
			}
		}
		catch (Exception exception) { errors.Add(resource + ": " + exception.Message); }
	}
}
