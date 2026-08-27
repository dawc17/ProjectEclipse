using System.Xml;

public static class GameLoader
{
	private const int KPPOOEGPPJA = 10;

	public static void EIBJELAJHCH()
	{
		GameSettings.IFBKAJPILOI();
		GameSettings.IIGOJINCIIF();
		GameSettings.AMOMFPOENBF();
	}

	public static void BFGENGJPCCN()
	{
		Sound.IFKCCDAIADF("snd_armor", 0f);
	}

	public static void SetSound(uint BELCFJOLEMF)
	{
		Sound.MaxPlayableSounds = BELCFJOLEMF;
	}

	public static void SetSound()
	{
		Sound.Init();
		SetSound(10u);
		BFGENGJPCCN();
	}

	public static void BJLLJHDFMOO()
	{
		AnimationData.Load(SF2Paths.MCFPDHOLNGB(), SystemProperties.DBBOCENKMGD());
	}

	public static void POLKDKOOACO()
	{
		AiData.Load();
	}

	public static void SetVersion(string APFECPFKMMH)
	{
		XmlDocument xmlDocument = XmlUtils.AIFIAKNJMHG(SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
		if (xmlDocument != null)
		{
			xmlDocument["Root"]["Versions"]["Version"].SetAttribute("Value", APFECPFKMMH);
			string kPFELJFPGHJ = string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
			string kPFELJFPGHJ2 = string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.GHKPPHAAMBL);
			XmlUtils.ONLDJNLKKAL(xmlDocument, kPFELJFPGHJ);
			XmlUtils.ONLDJNLKKAL(xmlDocument, kPFELJFPGHJ2);
		}
	}
}
