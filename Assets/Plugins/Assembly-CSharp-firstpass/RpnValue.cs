using System;
using System.Globalization;
using System.Runtime.CompilerServices;

public class RpnValue<T>
{
	private const string NIBHABIAIKI = "?CLC_";

	private const string OCPFCPGBHNO = "?";

	private bool isConst;

	private T value;

	private RpnParser.Formula DPABILBDPFF;

	public T Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public RpnValue(T value)
	{
		this.value = value;
		isConst = true;
	}

	public RpnValue(string BOADBNLBJAN, bool AEKOOFJLNFP = false)
	{
		DPABILBDPFF = new RpnParser.Formula(BOADBNLBJAN);
		if (BOADBNLBJAN.Contains("?CLC_") || !BOADBNLBJAN.Contains("?"))
		{
			value = ConvertTo(DPABILBDPFF.ODHJHHMEEOI().ToString());
			isConst = true;
			DPABILBDPFF = null;
		}
	}

	[SpecialName]
	public static T op_Implicit(global::RpnValue<T> NMICDDBHMDN)
	{
		return NMICDDBHMDN.OEAKCOHMIHH();
	}

	[SpecialName]
	public static global::RpnValue<T> op_Implicit(T value)
	{
		return new global::RpnValue<T>(value);
	}

	[SpecialName]
	public static global::RpnValue<T> op_Implicit(string BOADBNLBJAN)
	{
		return new global::RpnValue<T>(BOADBNLBJAN);
	}

	private static T ConvertTo(string BHMCGLHBCBI)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(int))
		{
			return (T)(object)int.Parse(BHMCGLHBCBI);
		}
		if (typeFromHandle == typeof(float))
		{
			return (T)(object)float.Parse(BHMCGLHBCBI, CultureInfo.InvariantCulture);
		}
		if (typeFromHandle == typeof(bool))
		{
			return (T)(object)ParseBool(BHMCGLHBCBI);
		}
		if (typeFromHandle == typeof(string))
		{
			return (T)(object)BHMCGLHBCBI;
		}
		return default(T);
	}

	private static bool ParseBool(string BHMCGLHBCBI)
	{
		return BHMCGLHBCBI.ToLower() == "true" || BHMCGLHBCBI == "1";
	}

	public T OEAKCOHMIHH()
	{
		if (isConst)
		{
			return value;
		}
		return ConvertTo(DPABILBDPFF.ODHJHHMEEOI().ToString());
	}
}
