using System;
using System.Diagnostics;

public sealed class PropertyDescriptor : IPropertyDescriptor
{
	private readonly IPropertyDescriptor PNBMNIMMEOF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int PAOBFNKOJED;

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

	public Type JDCDCGFHLPC
	{
		get
		{
			return MAGHEGMMNOF();
		}
		set
		{
			set_TypeOverride(value);
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

	public bool KBHICFPAIFJ
	{
		get
		{
			return HHHGHBBDMHC();
		}
	}

	public PropertyDescriptor(IPropertyDescriptor PNBMNIMMEOF)
	{
		this.PNBMNIMMEOF = PNBMNIMMEOF;
		set_Name(PNBMNIMMEOF.get_Name());
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	public void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public Type get_Type()
	{
		return PNBMNIMMEOF.get_Type();
	}

	public Type MAGHEGMMNOF()
	{
		return PNBMNIMMEOF.MAGHEGMMNOF();
	}

	public void set_TypeOverride(Type value)
	{
		PNBMNIMMEOF.set_TypeOverride(value);
	}

	public int BHDEMLGCNOJ()
	{
		return PAOBFNKOJED;
	}

	public void set_Order(int value)
	{
		PAOBFNKOJED = value;
	}

	public bool HHHGHBBDMHC()
	{
		return PNBMNIMMEOF.HHHGHBBDMHC();
	}

	public void Write(object target, object value)
	{
		PNBMNIMMEOF.Write(target, value);
	}

	public T PJLLHGDNCIF<T>() where T : Attribute
	{
		return PNBMNIMMEOF.PJLLHGDNCIF<T>();
	}

	public IObjectDescriptor Read(object target)
	{
		return PNBMNIMMEOF.Read(target);
	}
}
