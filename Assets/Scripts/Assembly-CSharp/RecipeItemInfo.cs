using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class RecipeItemInfo : ItemInfo
{
	private Recipe KFAHMNKAMKC;

	private RecipePrice HKBJMPIJOOA;

	private UserItem NKBIOFJMONB;

	private uint MDKBMLJNAGK;

	private uint FKPHJOEDCDJ;

	private long _RecipeDeliveryTime;

	public Recipe BOKDNFECGMI
	{
		get
		{
			return OIMGNCLBPHD();
		}
	}

	public RecipePrice NJFPKIIBFOP
	{
		get
		{
			return ADAJKDEOAAG();
		}
	}

	public UserItem FGBNJDPGOFN
	{
		get
		{
			return MFEAIEJFDAM();
		}
	}

	public long KCJOBLHNFEG
	{
		get
		{
			return HGDELDFDFNH();
		}
	}

	public RecipeItemInfo(Recipe LKJDNEFANOB, UserItem NDMCFNGEPOA, RecipePrice LMNMPHGIFAF)
	{
		KFAHMNKAMKC = LKJDNEFANOB;
		NKBIOFJMONB = NDMCFNGEPOA;
		HKBJMPIJOOA = LMNMPHGIFAF;
		Type = "Recipe";
		if (LMNMPHGIFAF != null && LMNMPHGIFAF.EHKNIKHPGDN > 0)
		{
			_RecipeDeliveryTime = ListSF.BLBNJKJKMBM() + LMNMPHGIFAF.EHKNIKHPGDN;
			KLHOKKPALOK = LMNMPHGIFAF.KLHOKKPALOK;
		}
		else
		{
			_RecipeDeliveryTime = 0L;
			KLHOKKPALOK = (ObscuredLong)(0L);
		}
		if (NDMCFNGEPOA != null)
		{
			MDKBMLJNAGK = (uint)NDMCFNGEPOA.DBLCMCEGJGI(false).MHGODOLNDLE;
		}
		FKPHJOEDCDJ = (uint)ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		Name = NDMCFNGEPOA.get_Name();
		Name = Name + "|" + LKJDNEFANOB.get_Name();
	}

	public RecipeItemInfo(XmlNode node, UserItem NDMCFNGEPOA)
	{
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		KFAHMNKAMKC = ForgeManager.ELEBLBJKDBI().GetRecipeByName(gOHIIMFFFJI);
		NKBIOFJMONB = NDMCFNGEPOA;
		MDKBMLJNAGK = node.Attributes["ItemLevel"].ParseUint();
		_RecipeDeliveryTime = node.Attributes["DeliveryTime"].ParseInt();
		FKPHJOEDCDJ = node.Attributes["PlayerLevel"].ParseUint();
	}

	public Recipe OIMGNCLBPHD()
	{
		return KFAHMNKAMKC;
	}

	public RecipePrice ADAJKDEOAAG()
	{
		return HKBJMPIJOOA;
	}

	public UserItem MFEAIEJFDAM()
	{
		return NKBIOFJMONB;
	}

	public long HGDELDFDFNH()
	{
		return _RecipeDeliveryTime;
	}
}
