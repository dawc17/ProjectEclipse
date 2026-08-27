using System;
using System.Diagnostics;

[Obsolete("Please use YamlMember instead")]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class YamlAliasAttribute : Attribute
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NICBGBGLFML;

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

	public YamlAliasAttribute(string LOKLDPLAPOL)
	{
		set_Alias(LOKLDPLAPOL);
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
