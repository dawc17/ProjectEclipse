using System;
using System.Collections.Generic;

internal struct ObjectMetadata
{
	private Type element_type;

	private bool is_dictionary;

	private IDictionary<string, PropertyMetadata> properties;

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

	public bool KPLJCLFPMED
	{
		get
		{
			return PMPFFNMIKAN();
		}
		set
		{
			set_IsDictionary(value);
		}
	}

	public IDictionary<string, PropertyMetadata> FPDMBBMEOAK
	{
		get
		{
			return FABLBHDIKCN();
		}
		set
		{
			IJAFNNMLFNF(value);
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

	public bool PMPFFNMIKAN()
	{
		return is_dictionary;
	}

	public void set_IsDictionary(bool value)
	{
		is_dictionary = value;
	}

	public IDictionary<string, PropertyMetadata> FABLBHDIKCN()
	{
		return properties;
	}

	public void IJAFNNMLFNF(IDictionary<string, PropertyMetadata> value)
	{
		properties = value;
	}
}
