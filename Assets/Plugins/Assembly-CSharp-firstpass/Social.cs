using Nekki.Social;

public class Social
{
	private static ISocialNetwork DKIPAEBHDDI;

	private static SocialWrapper _wrap;

	public static UserInfo LFIBBPIPPFJ
	{
		get
		{
			return NLEKLPFPLPC();
		}
	}

	public static ISocialNetwork FAAHDOEMDCJ
	{
		get
		{
			return CCOHIOKHFKI();
		}
	}

	public static UserInfo NLEKLPFPLPC()
	{
		return SocialWrapper.NLEKLPFPLPC();
	}

	public static ISocialNetwork CCOHIOKHFKI()
	{
		return DKIPAEBHDDI;
	}

	public static void Init(Callbacks EODBKOHACMO)
	{
		_wrap = SocialWrapper.Init(EODBKOHACMO, NNHKEJNGNKE);
	}

	private static void NNHKEJNGNKE(DFIPCKIEILP KPJKACAJHDF)
	{
		if (KPJKACAJHDF != DFIPCKIEILP.None && KPJKACAJHDF == DFIPCKIEILP.VKontakte)
		{
			DKIPAEBHDDI = new VK();
			DKIPAEBHDDI.Init(_wrap);
		}
	}
}
