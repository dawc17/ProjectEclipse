using System;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Utils;

public class RecipeItemInfo : ItemInfo
{
	private Recipe KFAHMNKAMKC;
	private RecipePrice HKBJMPIJOOA;
	private UserItem NKBIOFJMONB;
	private uint MDKBMLJNAGK;
	private uint FKPHJOEDCDJ;
	private long _RecipeDeliveryTime;

	public Recipe BOKDNFECGMI => OIMGNCLBPHD();
	public RecipePrice NJFPKIIBFOP => ADAJKDEOAAG();
	public UserItem FGBNJDPGOFN => MFEAIEJFDAM();
	public long KCJOBLHNFEG => HGDELDFDFNH();

	public int ItemLevel => (int)MDKBMLJNAGK;
	public int PlayerLevel => (int)FKPHJOEDCDJ;
	public long RecipeDeliveryTime => _RecipeDeliveryTime;
	public long TimeLeft => Math.Max(0L, _RecipeDeliveryTime - CurrentTimeSeconds());
	public bool IsStillInOrder => _RecipeDeliveryTime > 0L && TimeLeft > 0L;
	public string ItemAndRecipeInfo => Name;

	public RecipeItemInfo(Recipe recipe, UserItem userItem, RecipePrice price)
	{
		KFAHMNKAMKC = recipe;
		NKBIOFJMONB = userItem;
		HKBJMPIJOOA = price;
		Type = "Recipe";
		if (price != null && price.DeliveryTime > 0)
		{
			_RecipeDeliveryTime = CurrentTimeSeconds() + price.DeliveryTime;
			KLHOKKPALOK = price.BonusDeliveryPrice;
		}
		else
		{
			_RecipeDeliveryTime = 0L;
			KLHOKKPALOK = (ObscuredLong)0L;
		}
		if (userItem != null)
		{
			ItemInfo info = userItem.DBLCMCEGJGI(false) ?? userItem.BHKHOJPANHE();
			if (info != null) MDKBMLJNAGK = (uint)Math.Max(0, info.MHGODOLNDLE);
		}
		Roster roster = ListSF.CCDKHLAMKKO();
		FKPHJOEDCDJ = (uint)Math.Max(0, roster == null ? 0 : roster.PINDEKDNCNL());
		Name = (userItem == null ? string.Empty : userItem.get_Name()) + "|" +
			(recipe == null ? string.Empty : recipe.Name);
	}

	public RecipeItemInfo(XmlNode node, UserItem userItem)
	{
		NKBIOFJMONB = userItem;
		string recipeName = node?.Attributes?["Name"].CIPOICEEIBK(string.Empty) ?? string.Empty;
		KFAHMNKAMKC = ForgeManager.ELEBLBJKDBI().GetRecipeByName(recipeName);
		MDKBMLJNAGK = node?.Attributes?["ItemLevel"].ParseUint() ?? 0u;
		_RecipeDeliveryTime = node?.Attributes?["DeliveryTime"].ParseLong(0L) ?? 0L;
		FKPHJOEDCDJ = node?.Attributes?["PlayerLevel"].ParseUint() ?? 0u;
		HKBJMPIJOOA = KFAHMNKAMKC?.GetPriceByItemLevel(userItem, (int)MDKBMLJNAGK);
		Type = "Recipe";
		if (HKBJMPIJOOA != null) KLHOKKPALOK = HKBJMPIJOOA.BonusDeliveryPrice;
		Name = (userItem == null ? string.Empty : userItem.get_Name()) + "|" + recipeName;
	}

	private static long CurrentTimeSeconds()
	{
		long currentTime = ListSF.IDMJOMOMDOJ();
		return currentTime > 0L ? currentTime : GlobalTimer.get_GetTime();
	}

	public Recipe OIMGNCLBPHD() => KFAHMNKAMKC;
	public RecipePrice ADAJKDEOAAG() => HKBJMPIJOOA;
	public UserItem MFEAIEJFDAM() => NKBIOFJMONB;
	public long HGDELDFDFNH() => _RecipeDeliveryTime;
}
