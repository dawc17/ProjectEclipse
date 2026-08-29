using System;
using System.Collections.Generic;
using System.Xml;

namespace SF2DE.Content
{
	public sealed class ModelFallbackMapping
	{
		public string RequestedModel { get; private set; }
		public string FallbackModel { get; private set; }

		public ModelFallbackMapping(string requestedModel, string fallbackModel)
		{
			RequestedModel = requestedModel;
			FallbackModel = fallbackModel;
		}
	}

	public static class ItemListCompatibility
	{
		public static int AddHistoricalStageAliases(XmlDocument document)
		{
			Dictionary<string, string> stageItemAliases = new Dictionary<string, string>(StringComparer.Ordinal)
			{
				{ "ARMOR_IM_CEREMONIAL", "ARMOR_CEREMONIAL" },
				{ "HELM_IM_CEREMONIAL", "HELM_CEREMONIAL" },
				{ "MAGIC_DARK_WAVE", "MAGIC_C4_Z1_WARLOCK_DARK_WAVE" },
				{ "RANGED_NEEDLES", "RANGED_NEEDLE" },
				{ "WEAPON_SPEAR", "WEAPON_AE21_SPEAR" }
			};

			XmlElement items = document.SelectSingleNode("/List/Items") as XmlElement;
			int aliasesAdded = 0;
			if (items == null)
			{
				return aliasesAdded;
			}

			foreach (KeyValuePair<string, string> alias in stageItemAliases)
			{
				if (document.SelectSingleNode("/List/Items/Item[@Name='" + alias.Key + "']") != null)
				{
					continue;
				}
				XmlElement source = document.SelectSingleNode(
					"/List/Items/Item[@Name='" + alias.Value + "']") as XmlElement;
				if (source == null)
				{
					continue;
				}

				XmlElement itemAlias = (XmlElement)source.CloneNode(true);
				itemAlias.SetAttribute("Name", alias.Key);
				itemAlias.SetAttribute("ShopHide", "1");
				items.AppendChild(itemAlias);
				aliasesAdded++;
			}
			return aliasesAdded;
		}

		public static List<ModelFallbackMapping> HideUnavailableModelItems(
			XmlDocument document,
			Predicate<string> modelExists,
			out int hidden)
		{
			List<ModelFallbackMapping> fallbacks = new List<ModelFallbackMapping>();
			hidden = 0;
			XmlNodeList itemNodes = document.SelectNodes("/List/Items/Item[@Model]");
			foreach (XmlNode node in itemNodes)
			{
				XmlElement item = node as XmlElement;
				if (item == null)
				{
					continue;
				}

				string model = item.GetAttribute("Model");
				if (string.IsNullOrEmpty(model) || modelExists(model))
				{
					continue;
				}

				string fallback = GetModelFallback(item);
				if (modelExists(fallback))
				{
					fallbacks.Add(new ModelFallbackMapping(model, fallback));
				}
				if (item.GetAttribute("ShopHide") != "1")
				{
					item.SetAttribute("ShopHide", "1");
					hidden++;
				}
			}
			return fallbacks;
		}

		private static string GetModelFallback(XmlElement item)
		{
			string model = item.GetAttribute("Model").ToLowerInvariant();
			string type = item.GetAttribute("Type");
			string subType = item.GetAttribute("SubType");
			if (model.StartsWith("mdl_body") || model == "mdl_blackness_body")
			{
				return "mdl_body";
			}
			if (model.StartsWith("mdl_head"))
			{
				return "mdl_head";
			}
			if (model.StartsWith("mdl_helm"))
			{
				return "mdl_helm_light";
			}
			if (model.StartsWith("mdl_armor"))
			{
				return "mdl_armor_leather";
			}
			if (model.IndexOf("punch_bag") >= 0)
			{
				return type == "Skeleton" ? "mdl_skeleton_punching_bag" : "mdl_punching_bag";
			}
			if (type == "Skeleton")
			{
				return "mdl_skeleton";
			}
			if (type == "Helm")
			{
				return "mdl_helm_light";
			}
			if (type == "Armor")
			{
				return "mdl_armor_leather";
			}
			if (type == "Ranged")
			{
				if (subType == "Chakram") return "mdl_ranged_chakram";
				if (subType == "Kunai") return "mdl_ranged_kunai";
				return "mdl_ranged_shurikens";
			}
			if (type == "Magic")
			{
				if (subType == "HitBox" || subType == "VerticalTrigger")
				{
					return "mdl_magic_collision_box";
				}
				return "mdl_magic_energy_ball";
			}
			if (type == "Weapon")
			{
				switch (subType)
				{
				case "Kusarigama": return "mdl_weapon_super_kusarigama";
				case "OneHandedSword": return "mdl_one_handed_sword";
				case "Staff": return "mdl_weapon_staff";
				case "TwoHandedBlunt": return "mdl_weapon_super_hammers";
				case "Katana": return "mdl_weapon_katana";
				case "SteelClaws":
				case "Claws": return "mdl_weapon_claws";
				case "HunterClaws": return "mdl_hunter_claw";
				case "TwoHanded": return "mdl_weapon_two_hand_sword";
				case "Sickles": return "mdl_weapon_sickles";
				case "Batons": return "mdl_weapon_batons";
				case "Scythe": return "mdl_weapon_composite_scythe";
				case "CompositeSword": return "mdl_weapon_super_composite_sword";
				case "Daggers": return "mdl_weapon_daggers";
				case "Glaive": return "mdl_weapon_glaive";
				case "Knuckles":
				case "PowerFistsPrometheus": return "mdl_weapon_knuckles";
				case "Nunchaku": return "mdl_weapon_nunchaku";
				default: return "mdl_weapon_ninja_sword";
				}
			}
			return "mdl_skeleton";
		}
	}
}
