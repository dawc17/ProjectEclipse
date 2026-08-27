public static class GraphicsController
{
	private static string IFOOEAMIMGM = string.Empty;

	public static void JLDMJOEGJLF()
	{
		bool flag = OPEHHMBJABL();
		SetIsBigController(!flag);
	}

	public static void SetIsBigController(bool value)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.SessionSettings("ControllerScale", value.ToString());
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	public static bool OPEHHMBJABL()
	{
		return ELILPDACMDJ();
	}

	public static string PMAODLMLDLK()
	{
		string text = DKALBDKBCFP();
		if (text == string.Empty)
		{
			return SystemProperties.PMAODLMLDLK();
		}
		QualityOption.HPNJCDGIHLI hPNJCDGIHLI = QualityOption.ONPFEBDGLFO(text);
		QualityOption.HPNJCDGIHLI hPNJCDGIHLI2 = QualityOption.ONPFEBDGLFO(SystemProperties.PMAODLMLDLK());
		if (hPNJCDGIHLI <= hPNJCDGIHLI2)
		{
			return text;
		}
		return SystemProperties.PMAODLMLDLK();
	}

	public static void FBIJKFHGOJK(string value)
	{
		IFOOEAMIMGM = value;
		ListSF.CCDKHLAMKKO().SessionSettings("QualityCondition", IFOOEAMIMGM);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE(1);
	}

	public static string DKALBDKBCFP()
	{
		if (IFOOEAMIMGM == string.Empty)
		{
			IFOOEAMIMGM = HPEJGFMAKFP();
		}
		return IFOOEAMIMGM;
	}

	public static bool AFLFDJKLIEE()
	{
		string text = PMAODLMLDLK();
		string text2 = GetNextGraphicsQuality(text);
		bool flag = text != text2;
		if (flag)
		{
			FBIJKFHGOJK(text2);
		}
		return flag;
	}

	public static string GetNextGraphicsQuality(string HEPNIDFNHBA)
	{
		string text = SystemProperties.PMAODLMLDLK();
		string text2 = QualityOption.GetNextQualityCondition(HEPNIDFNHBA, text);
		if (QualityOption.CompareQualityCondition(text2, text))
		{
			return HEPNIDFNHBA;
		}
		return text2;
	}

	public static void FELIOKHNIKI()
	{
		SystemProperties.LOHALAKNGFB bAINMLLIKOL = ((GHLDNALLEKN() == SystemProperties.LOHALAKNGFB.PATH_SMALL) ? SystemProperties.LOHALAKNGFB.PATH_BIG : SystemProperties.LOHALAKNGFB.PATH_SMALL);
		KGGPNMGAJAH(bAINMLLIKOL);
	}

	public static void KGGPNMGAJAH(SystemProperties.LOHALAKNGFB value)
	{
		string text = PNBGEDGDCDF(value);
		if (text != string.Empty)
		{
			PBOMIGDFBLL(text);
		}
	}

	public static SystemProperties.LOHALAKNGFB GHLDNALLEKN()
	{
		string bAINMLLIKOL = AIGJNJNMODH();
		return GAEGDDNCCHP(bAINMLLIKOL);
	}

	public static SystemProperties.LOHALAKNGFB GAEGDDNCCHP(string value)
	{
		SystemProperties.LOHALAKNGFB result = SystemProperties.LOHALAKNGFB.PATH_DEFAULT;
		if (value == "HIGH")
		{
			result = SystemProperties.LOHALAKNGFB.PATH_BIG;
		}
		else if (value == "LOW")
		{
			result = SystemProperties.LOHALAKNGFB.PATH_SMALL;
		}
		return result;
	}

	public static string PNBGEDGDCDF(SystemProperties.LOHALAKNGFB value)
	{
		string empty = string.Empty;
		switch (value)
		{
		case SystemProperties.LOHALAKNGFB.PATH_BIG:
			return "HIGH";
		case SystemProperties.LOHALAKNGFB.PATH_SMALL:
			return "LOW";
		default:
			return string.Empty;
		}
	}

	private static bool ELILPDACMDJ()
	{
		bool result = !SystemProperties.FBGNIKBPCFB();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP.FJGCOOAACLD("ControllerScale"))
		{
			result = nKGLHEGIKKP.GetSettingsXML("ControllerScale") == "True" || nKGLHEGIKKP.GetSettingsXML("ControllerScale") == "1";
		}
		return result;
	}

	private static string HPEJGFMAKFP()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (!nKGLHEGIKKP.FJGCOOAACLD("QualityCondition"))
		{
			FBIJKFHGOJK(SystemProperties.PMAODLMLDLK());
		}
		return nKGLHEGIKKP.GetSettingsXML("QualityCondition");
	}

	private static void PBOMIGDFBLL(string value)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.SessionSettings("LocationResolution", value);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	private static string AIGJNJNMODH()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (!nKGLHEGIKKP.FJGCOOAACLD("LocationResolution"))
		{
			string bAINMLLIKOL = PNBGEDGDCDF(SystemProperties.JGBFPENNILG());
			PBOMIGDFBLL(bAINMLLIKOL);
		}
		return nKGLHEGIKKP.GetSettingsXML("LocationResolution");
	}
}
