using UnityEngine;

public class StoryDialogContent
{
	public enum MFHMNFAPAOH
	{
		CONTENT_TYPE_REGULAR = 0,
		CONTENT_TYPE_PRICELINE = 1
	}

	public string GGDJIPKMKFC = string.Empty;

	public string AJELOOEBCPO = string.Empty;

	public string LKIKHJNCBEI = string.Empty;

	public string DLKPBAJDHBO = string.Empty;

	public string KEHBCHJDCND = string.Empty;

	public int Id;

	public long Timer;

	public bool CheckTimer;

	public UserItem FGBNJDPGOFN;

	public RecipeItemInfo KMNGHHBCEGD;

	public TextTimer MALKNOOGNBA;

	public MFHMNFAPAOH NGEPEDCCMAI;

	public Color FontColor = Constants.PJJIMHMJPAL;

	public StoryDialogContent(string _text = "", string ADDKGJGCBMB = "", string KLIDPJCCAME = "", string MJEBMLFLLHO = "", int KMFDBBKMLOO = 0, int _id = -1, UserItem NKBIOFJMONB = null, TextTimer OIHKOMFCFME = null, MFHMNFAPAOH _type = MFHMNFAPAOH.CONTENT_TYPE_REGULAR, RecipeItemInfo DMDLCMBKEHA = null)
	{
		GGDJIPKMKFC = _text;
		AJELOOEBCPO = ADDKGJGCBMB;
		DLKPBAJDHBO = KLIDPJCCAME;
		KEHBCHJDCND = MJEBMLFLLHO;
		Id = _id;
		Timer = KMFDBBKMLOO;
		FGBNJDPGOFN = NKBIOFJMONB;
		KMNGHHBCEGD = DMDLCMBKEHA;
		MALKNOOGNBA = OIHKOMFCFME;
		NGEPEDCCMAI = _type;
	}

	public StoryDialogContent(StoryDialogContent NOLFMPDGCOC)
	{
		GGDJIPKMKFC = NOLFMPDGCOC.GGDJIPKMKFC;
		AJELOOEBCPO = NOLFMPDGCOC.AJELOOEBCPO;
		DLKPBAJDHBO = NOLFMPDGCOC.DLKPBAJDHBO;
		KEHBCHJDCND = NOLFMPDGCOC.KEHBCHJDCND;
		Id = NOLFMPDGCOC.Id;
		Timer = NOLFMPDGCOC.Timer;
		FGBNJDPGOFN = NOLFMPDGCOC.FGBNJDPGOFN;
		KMNGHHBCEGD = NOLFMPDGCOC.KMNGHHBCEGD;
		MALKNOOGNBA = NOLFMPDGCOC.MALKNOOGNBA;
		NGEPEDCCMAI = NOLFMPDGCOC.NGEPEDCCMAI;
	}

	public bool JHOPPPIADHN()
	{
		if (DLKPBAJDHBO != string.Empty || KEHBCHJDCND != string.Empty)
		{
			UserItem dKCHDHMLKHN = ListSF.CMGOCLGHNLH(DLKPBAJDHBO);
			if (dKCHDHMLKHN == null)
			{
				Timer = 0L;
			}
			else
			{
				Timer = GameUtils.GetLeftTime(dKCHDHMLKHN.IJGAOHJNLAH());
			}
			CheckTimer = true;
			FGBNJDPGOFN = dKCHDHMLKHN;
			if (Timer == 0)
			{
				return false;
			}
		}
		return true;
	}
}
