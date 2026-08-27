using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public sealed class DataContractAttribute : Attribute
{
	private string name;

	private string ODFFCOCKANC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KDBJPBOODLM;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		set
		{
			set_Name(value);
		}
	}

	public bool CEOOJHMGIPK
	{
		get
		{
			return OGFPFFHAIBA();
		}
		set
		{
			set_IsReference(value);
		}
	}

	public string get_Name()
	{
		return name;
	}

	public void set_Name(string value)
	{
		name = value;
	}

	public string IONIEDIPEGB()
	{
		return ODFFCOCKANC;
	}

	public void set_Namespace(string value)
	{
		ODFFCOCKANC = value;
	}

	public bool OGFPFFHAIBA()
	{
		return KDBJPBOODLM;
	}

	public void set_IsReference(bool value)
	{
		KDBJPBOODLM = value;
	}
}
