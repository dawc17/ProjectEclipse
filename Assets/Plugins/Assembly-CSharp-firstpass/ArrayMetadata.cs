using System;

internal struct ArrayMetadata
{
	private Type element_type;

	private bool IGDHCIAPEIP;

	private bool NLDEAEEJNHE;

	public Type FIFGGAOMEEB
	{
		get
		{
			return LPINKHOCABG();
		}
		set
		{
			set_ElementType(value);
		}
	}

	public bool MENGHLDLPDP
	{
		get
		{
			return NKLOBJNAFOL();
		}
		set
		{
			GDKDBPFDCIJ(value);
		}
	}

	public bool DLMLICFOLPO
	{
		get
		{
			return FOIBIKPNLJD();
		}
		set
		{
			ICHOKOLOLKC(value);
		}
	}

	public Type LPINKHOCABG()
	{
		if (element_type == null)
		{
			return typeof(JsonData);
		}
		return element_type;
	}

	public void set_ElementType(Type value)
	{
		element_type = value;
	}

	public bool NKLOBJNAFOL()
	{
		return IGDHCIAPEIP;
	}

	public void GDKDBPFDCIJ(bool value)
	{
		IGDHCIAPEIP = value;
	}

	public bool FOIBIKPNLJD()
	{
		return NLDEAEEJNHE;
	}

	public void ICHOKOLOLKC(bool value)
	{
		NLDEAEEJNHE = value;
	}
}
