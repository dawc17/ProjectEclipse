using System;
using System.Globalization;

public sealed class TypeAssigningEventEmitter : ChainedEventEmitter
{
	private readonly bool LMNFCJPEKHL;

	public TypeAssigningEventEmitter(IEventEmitter JDJEJDIJLLE, bool KAHOIHHBHGG)
		: base(JDJEJDIJLLE)
	{
		LMNFCJPEKHL = KAHOIHHBHGG;
	}

	public override void Emit(ScalarEventInfo FNHCFCAALAE)
	{
		FNHCFCAALAE.LBGFNDOAEED(true);
		FNHCFCAALAE.KHFMMPCKMKE(IBEOFCPMMJJ.Plain);
		TypeCode typeCode = ((FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH() != null) ? FNHCFCAALAE.EHKMMGBHNDB().get_Type().GetTypeCode() : TypeCode.Empty);
		switch (typeCode)
		{
		case TypeCode.Boolean:
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:bool");
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
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:int");
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.DGIAFODNLNN(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.Single:
		case TypeCode.Double:
		case TypeCode.Decimal:
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:float");
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.DGIAFODNLNN(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.Char:
		case TypeCode.String:
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:str");
			FNHCFCAALAE.set_RenderedValue(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH().ToString());
			FNHCFCAALAE.KHFMMPCKMKE(IBEOFCPMMJJ.Any);
			break;
		case TypeCode.DateTime:
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:timestamp");
			FNHCFCAALAE.set_RenderedValue(YamlFormatter.AHNEOKMPCPD(FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH()));
			break;
		case TypeCode.Empty:
			FNHCFCAALAE.set_Tag("tag:yaml.org,2002:null");
			FNHCFCAALAE.set_RenderedValue(string.Empty);
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
		FJKGHHMPMHH(FNHCFCAALAE);
		base.Emit(FNHCFCAALAE);
	}

	public override void Emit(PBGMOJFHMGI FNHCFCAALAE)
	{
		FJKGHHMPMHH(FNHCFCAALAE);
		base.Emit(FNHCFCAALAE);
	}

	private void FJKGHHMPMHH(ObjectEventInfo FNHCFCAALAE)
	{
		if (LMNFCJPEKHL && FNHCFCAALAE.EHKMMGBHNDB().OEAKCOHMIHH() != null && FNHCFCAALAE.EHKMMGBHNDB().get_Type() != FNHCFCAALAE.EHKMMGBHNDB().HEOINHLCBOO())
		{
			FNHCFCAALAE.set_Tag("!" + FNHCFCAALAE.EHKMMGBHNDB().get_Type().AssemblyQualifiedName);
		}
	}
}
