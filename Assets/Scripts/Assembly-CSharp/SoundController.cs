public class SoundController
{
	public const string MUSIC_MENU = "menu";

	public static bool IsBackgroundMusicIntro;

	public static float IHHCOMFHFEI
	{
		get
		{
			return FGFHCAAFODL();
		}
		set
		{
			IDLBNOCKEBK(value);
		}
	}

	public static float PBBIGECFMBM
	{
		get
		{
			return LOLBPMLPBGL();
		}
		set
		{
			EDPABAPLCGN(value);
		}
	}

	public static bool DBLLOGFKAGN
	{
		get
		{
			return ELHMADOKHHE();
		}
		set
		{
			FMLHEDIPGAF(value);
		}
	}

	public static bool BMDKHPCCFGB
	{
		get
		{
			return AAFLCDKJEPL();
		}
		set
		{
			FLOFHMBDHNM(value);
		}
	}

	public static void KHPHDKFDCLL(string name = "menu", bool KKHJAJFEPPA = true)
	{
		if (!IsBackgroundMusicIntro)
		{
			IsBackgroundMusicIntro = true;
			Sound.PlayMusic(name, KKHJAJFEPPA);
		}
	}

	public static void NDBJCCIBAIO()
	{
		IsBackgroundMusicIntro = false;
		Sound.FAJONFGJBPD();
	}

	public static float FGFHCAAFODL()
	{
		return (!Sound.ELHMADOKHHE()) ? Sound.EAIGFAPKILL() : 0f;
	}

	public static void IDLBNOCKEBK(float value)
	{
		Sound.OAFCOFNOIJK(value);
		bool flag = value <= 0f;
		bool flag2 = Sound.ELHMADOKHHE();
		if (flag && !flag2)
		{
			FMLHEDIPGAF(true);
		}
		else if (!flag && flag2)
		{
			FMLHEDIPGAF(false);
		}
		else
		{
			ListSF.CCDKHLAMKKO().APDCCIEJLMD();
		}
	}

	public static float LOLBPMLPBGL()
	{
		return (!Sound.AAFLCDKJEPL()) ? Sound.NBHPABEBLOP() : 0f;
	}

	public static void EDPABAPLCGN(float value)
	{
		Sound.JOFLPDCONNC(value);
		bool flag = value <= 0f;
		bool flag2 = Sound.AAFLCDKJEPL();
		if (flag && !flag2)
		{
			Sound.FLOFHMBDHNM(true);
		}
		else if (!flag && flag2)
		{
			Sound.FLOFHMBDHNM(false);
		}
		else
		{
			ListSF.CCDKHLAMKKO().ABODKHDPHMI();
		}
	}

	public static bool ELHMADOKHHE()
	{
		return Sound.ELHMADOKHHE();
	}

	public static void FMLHEDIPGAF(bool value)
	{
		if (value != Sound.ELHMADOKHHE())
		{
			if (!value && Sound.EAIGFAPKILL() == 0f)
			{
				Sound.OAFCOFNOIJK(1f);
			}
			Sound.FMLHEDIPGAF(value);
			ListSF.GKAOOOICJAI = value;
			ListSF.CCDKHLAMKKO().APDCCIEJLMD();
		}
	}

	public static bool AAFLCDKJEPL()
	{
		return Sound.AAFLCDKJEPL();
	}

	public static void FLOFHMBDHNM(bool value)
	{
		if (value != Sound.AAFLCDKJEPL())
		{
			if (!value && Sound.NBHPABEBLOP() == 0f)
			{
				Sound.JOFLPDCONNC(1f);
			}
			Sound.FLOFHMBDHNM(value);
			ListSF.CCDKHLAMKKO().ABODKHDPHMI();
		}
	}
}
