using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Shop;

public class QuestActionChangeTab : QuestAction
{
	protected string HAFLDLPCMLE = string.Empty;

	protected string OKPIBMMMIDL = string.Empty;

	protected SliderType _TabType;

	protected ScreenType _ScreenType = ScreenType.ModuleNone;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		HAFLDLPCMLE = EPKLCPOEELO.Attributes["Tab"].CIPOICEEIBK(string.Empty);
		OKPIBMMMIDL = EPKLCPOEELO.Attributes["Focus"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string empty = string.Empty;
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(HAFLDLPCMLE, lNIDLHOIHIM);
		empty = lNIDLHOIHIM.ToString();
		_TabType = EPIGNANCLDB(empty);
		_ScreenType = BODGLLCANLF(_TabType);
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		if (iPKNDMINFMJ == _ScreenType)
		{
			switch (_ScreenType)
			{
			case ScreenType.ModuleShop:
			{
				ShopScene current3 = Scene<ShopScene>.get_Current();
				if (current3 != null)
				{
					break;
				}
				Module.ELEBLBJKDBI().AddEventListener(1, DOHEMBEEHBB);
				return;
			}
			case ScreenType.ModuleProfile:
			{
				ProfileScene current2 = Scene<ProfileScene>.get_Current();
				if (current2 != null)
				{
					break;
				}
				Module.ELEBLBJKDBI().AddEventListener(1, DOHEMBEEHBB);
				return;
			}
			case ScreenType.ModuleMap:
			{
				MapScene current = Scene<MapScene>.get_Current();
				if (current != null)
				{
					current.ScrollToItemByName(_TabType);
					break;
				}
				Module.ELEBLBJKDBI().AddEventListener(1, DOHEMBEEHBB);
				return;
			}
			}
		}
		OGIJONMKABB();
	}

	protected void DOHEMBEEHBB(object data)
	{
		switch (_ScreenType)
		{
		case ScreenType.ModuleShop:
		{
			ShopScene current2 = Scene<ShopScene>.get_Current();
			if (!(current2 != null))
			{
			}
			break;
		}
		case ScreenType.ModuleProfile:
		{
			ProfileScene current3 = Scene<ProfileScene>.get_Current();
			if (!(current3 != null))
			{
			}
			break;
		}
		case ScreenType.ModuleMap:
		{
			MapScene current = Scene<MapScene>.get_Current();
			if (current != null)
			{
				current.ScrollToItemByName(_TabType);
			}
			break;
		}
		}
		OGIJONMKABB();
		Module.ELEBLBJKDBI().RemoveEventListener(1, DOHEMBEEHBB);
	}

	protected SliderType EPIGNANCLDB(string PMJGENGKNPA)
	{
		switch (PMJGENGKNPA)
		{
		case "Weapon":
			return SliderType.SliderWeapon;
		case "Armor":
			return SliderType.SliderArmor;
		case "Helm":
			return SliderType.SliderHelmet;
		case "Ranged":
			return SliderType.SliderMissile;
		case "Magic":
			return SliderType.SliderMagic;
		case "Ruby":
			return SliderType.SliderRuby;
		case "Free":
			return SliderType.SliderFree;
		case "Perks":
			return SliderType.SliderPerks;
		case "Moves":
			return SliderType.SliderTricks;
		case "Achievements":
			return SliderType.SliderAchievements;
		case "QuestItems":
			return SliderType.SliderSeals;
		case "RaidMapStage":
			return SliderType.SliderRaidMap;
		case "StoryMapStage":
			return SliderType.SliderStoryMap;
		default:
			return SliderType.SliderNone;
		}
	}

	protected ScreenType BODGLLCANLF(SliderType _sliderType)
	{
		switch (_sliderType)
		{
		case SliderType.SliderWeapon:
		case SliderType.SliderArmor:
		case SliderType.SliderHelmet:
		case SliderType.SliderMissile:
		case SliderType.SliderMagic:
		case SliderType.SliderRuby:
		case SliderType.SliderFree:
			return ScreenType.ModuleShop;
		case SliderType.SliderPerks:
		case SliderType.SliderTricks:
		case SliderType.SliderAchievements:
		case SliderType.SliderSeals:
			return ScreenType.ModuleProfile;
		case SliderType.SliderStoryMap:
		case SliderType.SliderRaidMap:
			return ScreenType.ModuleMap;
		default:
			return ScreenType.ModuleNone;
		}
	}
}
