using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class DataMemberAttribute : Attribute
{
	private bool CIJMHDHMGBB;

	private bool NFADCMLCMBL = true;

	private string name;

	private int order = -1;

	public bool HPBNIFNFJLB
	{
		get
		{
			return JGDGAIBHJCO();
		}
		set
		{
			CAKJHBPFLPG(value);
		}
	}

	public bool ONMGNOHJBKN
	{
		get
		{
			return OKKKGAKIJBA();
		}
		set
		{
			GAIALCENGJG(value);
		}
	}

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

	public bool JGDGAIBHJCO()
	{
		return NFADCMLCMBL;
	}

	public void CAKJHBPFLPG(bool value)
	{
		NFADCMLCMBL = value;
	}

	public bool OKKKGAKIJBA()
	{
		return CIJMHDHMGBB;
	}

	public void GAIALCENGJG(bool value)
	{
		CIJMHDHMGBB = value;
	}

	public string get_Name()
	{
		return name;
	}

	public void set_Name(string value)
	{
		name = value;
	}

	public int BHDEMLGCNOJ()
	{
		return order;
	}

	public void set_Order(int value)
	{
		order = value;
	}
}
