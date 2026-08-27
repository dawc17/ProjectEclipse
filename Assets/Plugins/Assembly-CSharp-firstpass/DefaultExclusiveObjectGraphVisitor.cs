using System;
using System.Collections.Generic;
using System.ComponentModel;

public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
{
	private static readonly IEqualityComparer<object> HNFBBKMHILG = EqualityComparer<object>.Default;

	public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor GDMFLLGPLNO)
		: base(GDMFLLGPLNO)
	{
	}

	private static object GANJKJMCHIP(Type LFLGCDNKNJI)
	{
		return (!LFLGCDNKNJI.KLAAGAMNBOB()) ? null : Activator.CreateInstance(LFLGCDNKNJI);
	}

	public override bool EnterMapping(IObjectDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		return !HNFBBKMHILG.Equals(value, GANJKJMCHIP(value.get_Type())) && base.EnterMapping(KGBGENDIMBC, value);
	}

	public override bool EnterMapping(IPropertyDescriptor KGBGENDIMBC, IObjectDescriptor value)
	{
		DefaultValueAttribute defaultValueAttribute = KGBGENDIMBC.PJLLHGDNCIF<DefaultValueAttribute>();
		object y = ((defaultValueAttribute == null) ? GANJKJMCHIP(KGBGENDIMBC.get_Type()) : defaultValueAttribute.Value);
		return !HNFBBKMHILG.Equals(value.OEAKCOHMIHH(), y) && base.EnterMapping(KGBGENDIMBC, value);
	}
}
