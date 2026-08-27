using System;
using System.Diagnostics;

public sealed class ObjectDescriptor : IObjectDescriptor
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object IELPCLONGKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Type KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Type CGNJIMHPGJG;

	public Type CJJOAABHDGM
	{
		get
		{
			return HEOINHLCBOO();
		}
		private set
		{
			JGNIGIGJFEE(value);
		}
	}

	public ObjectDescriptor(object value, Type LFLGCDNKNJI, Type FGDJAEMHFKC)
	{
		set_Value(value);
		if (LFLGCDNKNJI == null)
		{
			throw new ArgumentNullException("type");
		}
		set_Type(LFLGCDNKNJI);
		if (FGDJAEMHFKC == null)
		{
			throw new ArgumentNullException("staticType");
		}
		JGNIGIGJFEE(FGDJAEMHFKC);
	}

	object IObjectDescriptor.Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public object OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	private void set_Value(object value)
	{
		IELPCLONGKP = value;
	}

	public Type get_Type()
	{
		return KAHHEBMBCFA;
	}

	private void set_Type(Type value)
	{
		KAHHEBMBCFA = value;
	}

	public Type HEOINHLCBOO()
	{
		return CGNJIMHPGJG;
	}

	private void JGNIGIGJFEE(Type value)
	{
		CGNJIMHPGJG = value;
	}
}
