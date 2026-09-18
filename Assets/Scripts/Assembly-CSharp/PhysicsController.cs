using System.Xml;

public static class PhysicsController
{
	private static float _Friction;

	private static float _Gravity;

	private static int _IterativeProcess;

	private static float _FrictionForce;

	//Friction and FrictionForce seems to be the same value??

	public static float OCIFDDGOJBH
	{
		get
		{
			return GetFriction();
		}
	}

	public static float FNJAONFEGPP
	{
		get
		{
			return GetGravity();
		}
	}

	public static int IterativeProcess
	{
		get
		{
			return GetIterativeProcess();
		}
	}

	public static float JMGEOLKAGMH
	{
		get
		{
			return GetFrictionForce();
		}
		set
		{
			SetFrictionForce(value);
		}
	}

	public static float GetFriction()
	{
		return _Friction;
	}

	public static float GetGravity()
	{
		return _Gravity;
	}

	public static int GetIterativeProcess()
	{
		return _IterativeProcess;
	}

	public static float GetFrictionForce()
	{
		return _FrictionForce;
	}

	public static void SetFrictionForce(float value)
	{
		_FrictionForce = value;
	}

	public static void Parse(XmlNode node)
	{
		_Friction = node["FrictionForce"].Attributes["Value"].ParseFloat(0.2f);
		_Gravity = node["Gravitation"].Attributes["Value"].ParseFloat(0.4f);
		_IterativeProcess = node["IterativeProcess"].Attributes["Value"].ParseInt(2);
	}
}
