using System.Xml;

public static class PhysicsController
{
	private static float LMKIAHKHELF;

	private static float KFLFCNCNGLK;

	private static int _IterativeProcess;

	private static float APBHHHNNIFD;

	public static float OCIFDDGOJBH
	{
		get
		{
			return EOBGEGHEPOA();
		}
	}

	public static float FNJAONFEGPP
	{
		get
		{
			return KKAJIHOJMPN();
		}
	}

	public static int IterativeProcess
	{
		get
		{
			return HDEOPNEEMBJ();
		}
	}

	public static float JMGEOLKAGMH
	{
		get
		{
			return ECOHOOEMDNH();
		}
		set
		{
			set_FrictionForce(value);
		}
	}

	public static float EOBGEGHEPOA()
	{
		return LMKIAHKHELF;
	}

	public static float KKAJIHOJMPN()
	{
		return KFLFCNCNGLK;
	}

	public static int HDEOPNEEMBJ()
	{
		return _IterativeProcess;
	}

	public static float ECOHOOEMDNH()
	{
		return APBHHHNNIFD;
	}

	public static void set_FrictionForce(float value)
	{
		APBHHHNNIFD = value;
	}

	public static void Parse(XmlNode node)
	{
		LMKIAHKHELF = node["FrictionForce"].Attributes["Value"].ParseFloat(0.2f);
		KFLFCNCNGLK = node["Gravitation"].Attributes["Value"].ParseFloat(0.4f);
		_IterativeProcess = node["IterativeProcess"].Attributes["Value"].ParseInt(2);
	}
}
