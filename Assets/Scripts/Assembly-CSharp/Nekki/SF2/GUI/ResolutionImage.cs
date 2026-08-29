using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI_Nekki/ResolutionImage")]
	public class ResolutionImage : Image
	{
		private const float MenuButtonArtworkScale = 0.78f;

		private static readonly Dictionary<Sprite, Sprite> RepairedSprites = new Dictionary<Sprite, Sprite>();

		private static readonly HashSet<Sprite> RepairedSpriteValues = new HashSet<Sprite>();

		private static readonly HashSet<string> LoggedCompatibilitySprites = new HashSet<string>();

		public const string DefaultAtlasPath = "UI/Atlases/";

		public const string LowQualitySuffix = "_low";

		public const string ResFolder = "Resources/";

		[SerializeField]
		private string _TexturePath;

		[SerializeField]
		private string _SpriteName;

		public string KBIHPPDNFJD
		{
			get
			{
				return get_TexturePath();
			}
			set
			{
				set_TexturePath(value);
			}
		}

		public string NKMJLGBPGDD
		{
			get
			{
				return get_SpriteName();
			}
			set
			{
				set_SpriteName(value);
			}
		}

		public float IANOMIEILDG
		{
			get
			{
				return get_Alpha();
			}
			set
			{
				set_Alpha(value);
			}
		}

		public void set_TexturePath(string value)
		{
			_TexturePath = value;
		}

		public string get_TexturePath()
		{
			return _TexturePath;
		}

		public void set_SpriteName(string value)
		{
			_SpriteName = value;
			DKGIPADFGCO();
		}

		public string get_SpriteName()
		{
			return _SpriteName;
		}

		public void set_Alpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
		}

		public float get_Alpha()
		{
			return color.a;
		}

		protected override void Awake()
		{
			base.Awake();
			DKGIPADFGCO();
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			if (string.IsNullOrEmpty(_SpriteName))
			{
				return;
			}

			Sprite displayedSprite = overrideSprite != null ? overrideSprite : sprite;
			if (_SpriteName.StartsWith("BattleBtn") && displayedSprite != null && RepairedSpriteValues.Contains(displayedSprite))
			{
				// AssetRipper collapsed transparent padding from recovered battle-button
				// atlas members. The remaining trimmed pixels are then stretched across
				// the map-button rect, making normal-mode icons oversized and distorted
				// while their bundle-backed Eclipse variants retain the correct padding.
				// Restore the trimmed sprite's native footprint and center offset only in
				// the generated mesh so the hit area and child label layout stay intact.
				Rect targetRect = rectTransform.rect;
				float scaleX = (targetRect.width > 0f) ? Mathf.Min(1f, displayedSprite.rect.width / targetRect.width) : 1f;
				float scaleY = (targetRect.height > 0f) ? Mathf.Min(1f, displayedSprite.rect.height / targetRect.height) : 1f;
				Vector2 offset = displayedSprite.bounds.center * displayedSprite.pixelsPerUnit;
				TransformMesh(toFill, new Vector2(scaleX, scaleY), offset);
			}

			if (!_SpriteName.StartsWith("MenuButtons."))
			{
				return;
			}
			// The exported atlas members lost the transparent source padding that
			// surrounded the navigation artwork. UGUI consequently stretches the
			// trimmed pixels across the full button rect. Restore that padding in
			// the generated mesh so the original hit area and child badges stay put.
			TransformMesh(toFill, new Vector2(MenuButtonArtworkScale, MenuButtonArtworkScale), Vector2.zero);
		}

		private void TransformMesh(VertexHelper toFill, Vector2 scale, Vector2 offset)
		{
			Vector2 center = rectTransform.rect.center;
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < toFill.currentVertCount; i++)
			{
				toFill.PopulateUIVertex(ref vertex, i);
				Vector2 position = (Vector2)vertex.position - center;
				position = new Vector2(position.x * scale.x, position.y * scale.y);
				vertex.position = center + position + offset;
				toFill.SetUIVertex(vertex, i);
			}
		}

		private void DKGIPADFGCO()
		{
			// Some exported prefabs only have m_Sprite populated.  Do not erase that
			// serialized fallback when there is no logical resource name to resolve.
			if (string.IsNullOrEmpty(_SpriteName))
			{
				return;
			}
			Sprite sprite = GetSprite(_TexturePath, _SpriteName);
			// Scroll views recycle their cells. If the new logical sprite cannot be
			// resolved, retaining the old Sprite makes icons appear to loop as the
			// user scrolls through content whose atlases are not installed.
			base.sprite = sprite;
		}

		public static Sprite GetSprite(string texturePath, string JGIGOMLGLPN)
		{
			if (string.IsNullOrEmpty(JGIGOMLGLPN))
			{
				return null;
			}
			// A few reconstructed prefabs (notably the in-fight perk widgets) put
			// the complete Resources path in _SpriteName and leave _TexturePath
			// empty. Treat the final path component as the sprite name. Without
			// this normalization UI/Skills/IconFoo is searched from the Resources
			// root, bypassing both the exact skill assets and their compatibility
			// fallback, which leaves the icon as a white/blank image.
			string normalizedName = JGIGOMLGLPN.Replace('\\', '/');
			if (string.IsNullOrEmpty(texturePath))
			{
				int num = normalizedName.LastIndexOf('/');
				if (num >= 0)
				{
					texturePath = normalizedName.Substring(0, num + 1);
					JGIGOMLGLPN = normalizedName.Substring(num + 1);
				}
			}
			// AssetStudio exported both atlas members and ordinary sprites as
			// individual .asset files.  Their resource names often contain dots
			// (VS_Fon_left.img, Map1.1, Weapon1.img_weapon_...), so a dot alone is
			// not enough to identify an atlas reference.  Try the exact standalone
			// asset first and only use the original atlas lookup as a fallback.
			Sprite sprite = OPHFAHOKBOK(texturePath, JGIGOMLGLPN);
			if (sprite != null)
			{
				return sprite;
			}
			string[] array = JGIGOMLGLPN.Split('.');
			if (array.Length > 1)
			{
				sprite = MINPKAHEHDE(texturePath, JGIGOMLGLPN);
				if (sprite != null)
				{
					return sprite;
				}

				// AssetStudio also exported most atlas members as standalone sprites.
				// New XML still names them Atlas.Member, so retry the member name.
				sprite = OPHFAHOKBOK(texturePath, array[array.Length - 1]);
				if (sprite != null)
				{
					return sprite;
				}

				// The shop asks for the small, transparent members of the legacy
				// Enchantments atlas.  Modern perk-card sprites use the same member
				// names but include a large framed background, so keep the recovered
				// shop variants in a separate resource directory.  UI/Skills remains
				// a last-resort fallback for names absent from the legacy atlas.
				if (array[0].Equals("Enchantments", System.StringComparison.OrdinalIgnoreCase))
				{
					sprite = OPHFAHOKBOK("UI/Enchantments/", array[array.Length - 1]);
					if (sprite == null)
					{
						sprite = OPHFAHOKBOK("UI/Skills/", array[array.Length - 1]);
					}
					if (sprite != null)
					{
						return sprite;
					}
				}
			}
			sprite = GetCompatibilitySprite(texturePath, JGIGOMLGLPN);
			if (sprite == null)
			{
				string missingKey = ((texturePath ?? string.Empty) + JGIGOMLGLPN).Replace('\\', '/');
				if (LoggedCompatibilitySprites.Add("missing:" + missingKey))
				{
					Debug.LogWarning("[UI] Missing sprite '" + missingKey + "'.");
				}
			}
			return sprite;
		}

		private static Sprite GetCompatibilitySprite(string texturePath, string spriteName)
		{
			string normalizedPath = (texturePath ?? string.Empty).Replace('\\', '/').TrimEnd('/');
			string fallback = null;
			if (normalizedPath.EndsWith("UI/Users", System.StringComparison.OrdinalIgnoreCase))
			{
				fallback = GetAvatarFallback(spriteName);
			}
			else if (normalizedPath.EndsWith("UI/Skills", System.StringComparison.OrdinalIgnoreCase))
			{
				fallback = GetSkillFallback(spriteName);
			}
			else if (normalizedPath.EndsWith("UI/Achievements", System.StringComparison.OrdinalIgnoreCase))
			{
				fallback = GetAchievementFallback(spriteName);
			}
			if (string.IsNullOrEmpty(fallback))
			{
				return null;
			}
			Sprite sprite = OPHFAHOKBOK(texturePath, fallback);
			if (sprite != null)
			{
				string key = normalizedPath + "/" + spriteName;
				if (LoggedCompatibilitySprites.Add(key))
				{
					Debug.LogWarning("[UI] Missing sprite '" + key + "'; using '" + fallback + "'.");
				}
			}
			return sprite;
		}

		private static string GetAvatarFallback(string spriteName)
		{
			string name = (spriteName ?? string.Empty).ToLowerInvariant();
			// Newer shop XML prefixes reward portraits with "img_", while the
			// recovered files retain their original drop_* resource names.
			if (name.StartsWith("img_drop_")) return name.Substring(4);

			// Deterministic aliases for newer variants whose portrait was not in
			// the recovered CDN bundles. These keep the same named character (or
			// its closest version) instead of selecting an unrelated fighter from
			// the weapon-name heuristic below.
			if (name == "boss_butcher_young") return "boss_butcher";
			if (name == "boss_hermit_young") return "boss_hermit";
			if (name == "boss_shogun_young") return "boss_shogun";
			if (name == "boss_wasp_young") return "boss_wasp";
			if (name.StartsWith("boss_lamb")) return "boss_lamb_fungus_hard";
			if (name == "boss_puppeteer_hw21") return "character_puppeteer";
			if (name.StartsWith("witch_halloween")) return "character_witch15";
			if (name == "girl_sword_2") return "girl_swords_2";
			if (name == "new_man_shuang_gou") return "man_shuang_gou";
			if (name == "character_pirate") return "character_corsair";
			if (name == "looter_girl_agony") return "looter_girl_scythe";

			// Reward art must never fall through to a fighter portrait. If its
			// exact recovered file is unavailable, leave it empty and log the
			// missing resource instead of displaying an actively wrong character.
			if (name.StartsWith("img_") || name.StartsWith("drop_") ||
				name.Contains("bag_") || name.Contains("chest") || name.Contains("casket") ||
				name.Contains("booster") || name.Contains("ticket") || name.Contains("pile_"))
			{
				return null;
			}
			if (name.StartsWith("boss_") || name.StartsWith("character_")) return "avatar_masked";
			bool girl = name.Contains("girl") || name.Contains("woman") || name.Contains("witch") ||
				name.Contains("huntress") || name.Contains("wasp") || name.Contains("widow");
			bool ninja = name.Contains("ninja");
			if (name.Contains("lynx")) return "boss_lynx";
			if (name.Contains("knife") || name.Contains("dagger") || name.Contains("keris") ||
				name.Contains("crescent") || name.Contains("stiletto") || name.Contains("katar"))
				return girl ? "girl_knives" : (ninja ? "ninja_man_knives" : "man_knives");
			if (name.Contains("nunchaku")) return girl ? "girl_nunchaku_2" : "man_nunchaku";
			if (name.Contains("staff") || name.Contains("glaive") || name.Contains("spear") ||
				name.Contains("yari") || name.Contains("naginata") || name.Contains("trident") || name.Contains("scythe"))
				return girl ? "girl_staff" : "man_staff";
			if (name.Contains("baton") || name.Contains("tonfa")) return ninja ? "ninja_man_batons" : "man_batons";
			if (name.Contains("sword") || name.Contains("sabre") || name.Contains("katana") ||
				name.Contains("dadao") || name.Contains("wakidzashi") || name.Contains("machete"))
				return girl ? "girl_sword" : (ninja ? "ninja_man_ninja_sword" : "man_sword");
			if (name.Contains("axe") || name.Contains("hammer") || name.Contains("mace")) return "man_axes_2";
			if (name.Contains("claw") || name.Contains("knuckle") || name.Contains("fist") || name.Contains("kungfu"))
				return ninja ? "ninja_man_kungfu" : "man_fist";
			if (girl) return "girl_sword";
			if (ninja) return "ninja_man_kungfu";
			return "man_fist";
		}

		private static string GetSkillFallback(string spriteName)
		{
			string name = (spriteName ?? string.Empty);
			string lower = name.ToLowerInvariant();
			string colorSuffix = lower.Contains("_red") ? "_Red" : (lower.Contains("_blue") ? "_Blue" : string.Empty);
			if (lower.Contains("chargestea")) return "IconChargeSteal" + colorSuffix;
			if (lower.Contains("evasion")) return "IconAgility" + colorSuffix;
			if (lower.Contains("shield") || lower.Contains("armor")) return "IconSolidBlock" + colorSuffix;
			if (lower.Contains("regen") || lower.Contains("life")) return "IconRegeneration" + colorSuffix;
			if (lower.Contains("blood") || lower.Contains("bleed")) return "IconBleeding" + colorSuffix;
			return "IconPowerSurge" + colorSuffix;
		}

		private static string GetAchievementFallback(string spriteName)
		{
			string name = spriteName ?? string.Empty;
			int dot = name.LastIndexOf('.');
			if (dot >= 0)
			{
				name = name.Substring(dot + 1);
			}
			if (name.EndsWith("_eclipse", System.StringComparison.OrdinalIgnoreCase))
			{
				return name.Substring(0, name.Length - "_eclipse".Length) + "_gold";
			}
			return null;
		}

		private static Sprite OPHFAHOKBOK(string texturePath, string JGIGOMLGLPN)
		{
			string spriteName = DIHMNAGPFCG(JGIGOMLGLPN);
			string normalizedTexturePath = (texturePath ?? string.Empty).Replace('\\', '/');
			string oNEIGMLOGDC = normalizedTexturePath + spriteName;
			int dot = spriteName.IndexOf('.');
			if (dot > 0 && !normalizedTexturePath.EndsWith("/", System.StringComparison.Ordinal))
			{
				string atlasName = spriteName.Substring(0, dot);
				int slash = normalizedTexturePath.LastIndexOf('/');
				string textureName = (slash < 0) ? normalizedTexturePath : normalizedTexturePath.Substring(slash + 1);
				if (textureName.Equals(atlasName, System.StringComparison.OrdinalIgnoreCase))
				{
					oNEIGMLOGDC = ((slash < 0) ? string.Empty : normalizedTexturePath.Substring(0, slash + 1)) + spriteName;
				}
			}
			return RepairInvalidRecoveredSprite(ResourcesAndBundles.Load<Sprite>(oNEIGMLOGDC));
		}

		private static Sprite RepairInvalidRecoveredSprite(Sprite sprite)
		{
			if (sprite == null || sprite.texture == null)
			{
				return sprite;
			}

			Vector2[] vertices = sprite.vertices;
			Vector2[] uv = sprite.uv;
			bool invalidGeometry = vertices == null || vertices.Length < 3;
			bool invalidUv = uv == null || uv.Length < 3;
			if (!invalidUv)
			{
				invalidUv = true;
				Vector2 first = uv[0];
				for (int i = 1; i < uv.Length; i++)
				{
					if ((uv[i] - first).sqrMagnitude > 1E-08f)
					{
						invalidUv = false;
						break;
					}
				}
			}
			if (!invalidGeometry && !invalidUv)
			{
				return sprite;
			}

			Sprite repaired;
			if (RepairedSprites.TryGetValue(sprite, out repaired) && repaired != null)
			{
				return repaired;
			}

			// AssetRipper preserved many atlas crop rectangles but emitted either no
			// mesh at all or four vertices whose UVs are all (0,0). The latter
			// produces the solid white rectangles seen in currency and difficulty
			// glyphs. Rebuild only structurally invalid sprites from their original
			// atlas rectangle. Valid bundle and recovered sprites are left untouched.
			Rect rect = sprite.rect;
			if (rect.width <= 0f || rect.height <= 0f)
			{
				return sprite;
			}
			Vector2 pivot = new Vector2(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height);
			repaired = Sprite.Create(sprite.texture, rect, pivot, sprite.pixelsPerUnit, 0u, SpriteMeshType.FullRect, sprite.border);
			repaired.name = sprite.name;
			RepairedSprites[sprite] = repaired;
			RepairedSpriteValues.Add(repaired);
			return repaired;
		}

		private static Sprite MINPKAHEHDE(string texturePath, string JGIGOMLGLPN)
		{
			string[] array = JGIGOMLGLPN.Split('.');
			string text = array[0];
			if (text.Equals("Attributes"))
			{
				int num = 0;
			}
			Sprite sprite = AtlasCache.GetSpriteFromAtlas(texturePath + MPOGAEPOJCO(text), JGIGOMLGLPN);
			if (sprite == null)
			{
				sprite = AtlasCache.GetSpriteFromAtlas("UI/Atlases/" + MPOGAEPOJCO(text), JGIGOMLGLPN);
			}
			return RepairInvalidRecoveredSprite(sprite);
		}

		protected static string DIHMNAGPFCG(string JGIGOMLGLPN)
		{
			return JGIGOMLGLPN;
		}

		protected static string MPOGAEPOJCO(string JLEKBBJBLOE)
		{
			return JLEKBBJBLOE;
		}

		public override void SetNativeSize()
		{
			base.SetNativeSize();
			PMHFOCJKBGJ();
		}

		protected virtual void PMHFOCJKBGJ()
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public static string GetTexturePath(Sprite GBIOHMNNEJI)
		{
			return string.Empty;
		}
	}
}
