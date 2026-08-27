using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class YamlMemberAttribute : Attribute
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Type AHFBNBCEGPG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int PAOBFNKOJED;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NICBGBGLFML;

	public Type CNPNDGPFOLC
	{
		get
		{
			return FDDGCEPMIJG();
		}
		set
		{
			set_SerializeAs(value);
		}
	}

	public int PECDGDLCAAA
	{
		get
		{
			return BHDEMLGCNOJ();
		}
		set
		{
			set_Order(value);
		}
	}

	public string HBCNKNFPAIM
	{
		get
		{
			return MIDPFGENBCF();
		}
		set
		{
			set_Alias(value);
		}
	}

	public YamlMemberAttribute()
	{
	}

	public YamlMemberAttribute(Type JDBOFNJPMPH)
	{
		set_SerializeAs(JDBOFNJPMPH);
	}

	public Type FDDGCEPMIJG()
	{
		return AHFBNBCEGPG;
	}

	public void set_SerializeAs(Type value)
	{
		AHFBNBCEGPG = value;
	}

	public int BHDEMLGCNOJ()
	{
		return PAOBFNKOJED;
	}

	public void set_Order(int value)
	{
		PAOBFNKOJED = value;
	}

	public string MIDPFGENBCF()
	{
		return NICBGBGLFML;
	}

	public void set_Alias(string value)
	{
		NICBGBGLFML = value;
	}
}
