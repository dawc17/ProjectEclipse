using System;
using System.Globalization;

public sealed class ScalarNodeDeserializer : INodeDeserializer
{
	private static readonly NumberFormatInfo MNNCBNBBPHN = new NumberFormatInfo
	{
		CurrencyDecimalSeparator = ".",
		CurrencyGroupSeparator = "_",
		CurrencyGroupSizes = new int[1] { 3 },
		CurrencySymbol = string.Empty,
		CurrencyDecimalDigits = 99,
		NumberDecimalSeparator = ".",
		NumberGroupSeparator = "_",
		NumberGroupSizes = new int[1] { 3 },
		NumberDecimalDigits = 99
	};

	bool INodeDeserializer.Deserialize(EventReader reader, Type MBLGNMBFHBI, Func<EventReader, Type, object> IJBAEAEDMCC, out object value)
	{
		Scalar lEACOCDHICF = reader.GNNPKHDPGLN<Scalar>();
		if (lEACOCDHICF == null)
		{
			value = null;
			return false;
		}
		if (MBLGNMBFHBI.LCAJNDEBEFB())
		{
			value = Enum.Parse(MBLGNMBFHBI, lEACOCDHICF.OEAKCOHMIHH());
		}
		else
		{
			switch (MBLGNMBFHBI.GetTypeCode())
			{
			case TypeCode.Boolean:
				value = bool.Parse(lEACOCDHICF.OEAKCOHMIHH());
				break;
			case TypeCode.Byte:
				value = byte.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Int16:
				value = short.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Int32:
				value = int.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Int64:
				value = long.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.SByte:
				value = sbyte.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.UInt16:
				value = ushort.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.UInt32:
				value = uint.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.UInt64:
				value = ulong.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Single:
				value = float.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Double:
				value = double.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.Decimal:
				value = decimal.Parse(lEACOCDHICF.OEAKCOHMIHH(), MNNCBNBBPHN);
				break;
			case TypeCode.String:
				value = lEACOCDHICF.OEAKCOHMIHH();
				break;
			case TypeCode.Char:
				value = lEACOCDHICF.OEAKCOHMIHH()[0];
				break;
			case TypeCode.DateTime:
				value = DateTime.Parse(lEACOCDHICF.OEAKCOHMIHH(), CultureInfo.InvariantCulture);
				break;
			default:
				if (MBLGNMBFHBI == typeof(object))
				{
					value = lEACOCDHICF.OEAKCOHMIHH();
				}
				else
				{
					value = TypeConverterHelper.ChangeType(lEACOCDHICF.OEAKCOHMIHH(), MBLGNMBFHBI);
				}
				break;
			}
		}
		return true;
	}
}
