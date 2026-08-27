using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Shop;

public class QuestActionShop : QuestAction
{
	private string ENBEKBKOLBA = string.Empty;

	private string item = string.Empty;

	private SliderType _sliderType;

	private string OHCGEEEKEJH = string.Empty;

	private ItemInfo PJDAGCBPLJE;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		ENBEKBKOLBA = EPKLCPOEELO.Attributes["Tab"].CIPOICEEIBK(string.Empty);
		item = EPKLCPOEELO.Attributes["Item"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		ConditionExtension.CompareResult lNIDLHOIHIM2 = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		if (!string.IsNullOrEmpty(ENBEKBKOLBA))
		{
			kKDGLNECFHA.MCPIOGALBMK(ENBEKBKOLBA, lNIDLHOIHIM);
		}
		if (!string.IsNullOrEmpty(item))
		{
			kKDGLNECFHA.MCPIOGALBMK(item, lNIDLHOIHIM2);
		}
		_sliderType = PNEBCFOGKEE(lNIDLHOIHIM.ToString());
		OHCGEEEKEJH = lNIDLHOIHIM2.ToString();
		PJDAGCBPLJE = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(OHCGEEEKEJH);
		GOMCDIMDNON();
	}

	private SliderType PNEBCFOGKEE(string name)
	{
		switch (name)
		{
		case "Weapon":
			return SliderType.SliderWeapon;
		case "Ranged":
			return SliderType.SliderMissile;
		case "Magic":
			return SliderType.SliderMagic;
		case "Armor":
			return SliderType.SliderArmor;
		case "Helm":
			return SliderType.SliderHelmet;
		case "Ruby":
			return SliderType.SliderRuby;
		case "Free":
			return SliderType.SliderFree;
		case "RaidConsumable":
			return SliderType.SliderRaidItemPack;
		default:
			return SliderType.SliderNone;
		}
	}

	private void JILPFNBAKGK(object data)
	{
		ShopScene current = Scene<ShopScene>.get_Current();
		if (current != null)
		{
			current.ScrollToItemByName(_sliderType, OHCGEEEKEJH);
		}
		Module.ELEBLBJKDBI().RemoveEventListener(1, JILPFNBAKGK);
		OGIJONMKABB();
	}

	private void DOHEMBEEHBB(object data)
	{
		Module.ELEBLBJKDBI().RemoveEventListener(1, DOHEMBEEHBB);
		OGIJONMKABB();
	}

	private void GOMCDIMDNON()
	{
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		ShopScene current = Scene<ShopScene>.get_Current();
		bool flag = current != null;
		bool flag2 = iPKNDMINFMJ == ScreenType.ModuleShop;
		if (flag && _sliderType != SliderType.SliderNone)
		{
			current.ScrollToItemByName(_sliderType, OHCGEEEKEJH);
			OGIJONMKABB();
		}
		else if (flag2)
		{
			Module.ELEBLBJKDBI().AddEventListener(1, JILPFNBAKGK);
		}
		else
		{
			Module.ELEBLBJKDBI().AddEventListener(1, DOHEMBEEHBB);
			Module.DLOKJOHNDID(ScreenType.ModuleShop, new DelayedStrike(_sliderType, PJDAGCBPLJE, true));
		}
	}
}
