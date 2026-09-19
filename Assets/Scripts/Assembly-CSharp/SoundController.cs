public class SoundController
{
    private static void SaveMusicSettings()
    {
        UnityEngine.PlayerPrefs.SetFloat("Eclipse.MusicVolume", Sound.EAIGFAPKILL());
        UnityEngine.PlayerPrefs.SetInt("Eclipse.MusicMuted", Sound.ELHMADOKHHE() ? 1 : 0);
        UnityEngine.PlayerPrefs.Save();
        ListSF.CCDKHLAMKKO()?.APDCCIEJLMD();
    }

    private static void SaveSoundSettings()
    {
        UnityEngine.PlayerPrefs.SetFloat("Eclipse.SoundVolume", Sound.NBHPABEBLOP());
        UnityEngine.PlayerPrefs.SetInt("Eclipse.SoundMuted", Sound.AAFLCDKJEPL() ? 1 : 0);
        UnityEngine.PlayerPrefs.Save();
        ListSF.CCDKHLAMKKO()?.ABODKHDPHMI();
    }

    internal static void ApplySavedVolumes()
    {
        // Title-screen settings exist before a roster and take precedence once it loads.
        if (UnityEngine.PlayerPrefs.HasKey("Eclipse.MusicVolume"))
        {
            Sound.OAFCOFNOIJK(UnityEngine.PlayerPrefs.GetFloat("Eclipse.MusicVolume"));
            Sound.FMLHEDIPGAF(UnityEngine.PlayerPrefs.GetInt("Eclipse.MusicMuted") != 0);
            ListSF.GKAOOOICJAI = Sound.ELHMADOKHHE();
        }
        if (UnityEngine.PlayerPrefs.HasKey("Eclipse.SoundVolume"))
        {
            Sound.JOFLPDCONNC(UnityEngine.PlayerPrefs.GetFloat("Eclipse.SoundVolume"));
            Sound.FLOFHMBDHNM(UnityEngine.PlayerPrefs.GetInt("Eclipse.SoundMuted") != 0);
        }
    }

	public const string MUSIC_MENU = "menu";

	public static bool IsBackgroundMusicIntro;

	public static float IHHCOMFHFEI
	{
		get
		{
			return GetMusicVolume();
		}
		set
		{
			SetMusicVolume(value);
		}
	}

	public static float PBBIGECFMBM
	{
		get
		{
			return GetSoundVolume();
		}
		set
		{
			SetSoundVolume(value);
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

	// best guess for name

	public static float GetMusicVolume()
	{
		return (!Sound.ELHMADOKHHE()) ? Sound.EAIGFAPKILL() : 0f;
	}

	// best guess for name

	public static void SetMusicVolume(float value)
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
			SaveMusicSettings();
		}
	}

	// best guess for name

	public static float GetSoundVolume()
	{
		return (!Sound.AAFLCDKJEPL()) ? Sound.NBHPABEBLOP() : 0f;
	}

	// best guess for name

	public static void SetSoundVolume(float value)
	{
		Sound.JOFLPDCONNC(value);
		bool flag = value <= 0f;
		bool flag2 = Sound.AAFLCDKJEPL();
		if (flag && !flag2)
		{
			FLOFHMBDHNM(true);
		}
		else if (!flag && flag2)
		{
			FLOFHMBDHNM(false);
		}
		else
		{
			SaveSoundSettings();
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
			SaveMusicSettings();
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
			SaveSoundSettings();
		}
	}
}
