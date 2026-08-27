using System;
using System.Globalization;

public sealed class JsonEventEmitter : ChainedEventEmitter
{
	public JsonEventEmitter(IEventEmitter JDJEJDIJLLE)
		: base(JDJEJDIJLLE)
	{
	}

	public override void Emit(AliasEventInfo FNHCFCAALAE)
	{
		throw new NotSupportedException("Aliases are not supported in JSON");
	}

	public override void Emit(ScalarEventInfo FNHCFCAALAE)
	{
		FNHCFCAALAE.LBGFNDOAEED(true);
		FNHCFCAALAE.KHFMMPCKMKE(IBEOFCPMMJJ.Plain);
		TypeCode typeCode = ((FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH() != null) ? FNHCFCAALAE.EHKMMGBHNDB().get_Type().GetTypeCode() : TypeCode.Empty);
		switch (typeCode)
		{
		case TypeCode.Boolean:
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.NMBPLFHGICK(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.DGIAFODNLNN(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.Char:
		case TypeCode.String:
			FNHCFCAALAE.set_RenderedValue(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH().ToString());
			FNHCFCAALAE.KHFMMPCKMKE(IBEOFCPMMJJ.DoubleQuoted);
			break;
		case TypeCode.DateTime:
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.AHNEOKMPCPD(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.Empty:
			FNHCFCAALAE.set_RenderedValue("null");
			break;
		default:
			if (FNHCFCAALAE.EHKMMGBHNDB().get_Type() == typeof(TimeSpan))
			{
				FNHCFCAALAE.set_RenderedValue(YamlFormatter.ALEIMPLLAHI(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
				break;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
		}
		base.Emit(FNHCFCAALAE);
	}

	public override void Emit(LPADMPIAIPF FNHCFCAALAE)
	{
		FNHCFCAALAE.KHFMMPCKMKE(FGDKNBEFPFN.Flow);
		base.Emit(FNHCFCAALAE);
	}

	public override void Emit(PBGMOJFHMGI FNHCFCAALAE)
	{
		FNHCFCAALAE.KHFMMPCKMKE(NBCBGEPFIKG.Flow);
		base.Emit(FNHCFCAALAE);
	}
}
