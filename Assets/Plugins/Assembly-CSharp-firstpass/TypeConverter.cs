using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

public static class TypeConverterHelper
{
	public delegate bool DNEKCDAFFIG<T>(string value, out T DCJLKCFKCOM);

	public static void JCPBBODBIBI<TConvertible, TConverter>() where TConverter : global::System.ComponentModel.TypeConverter
	{
		if (!TypeDescriptor.GetAttributes(typeof(TConvertible)).OfType<TypeConverterAttribute>().Any((TypeConverterAttribute LHBNIMGFKIB) => LHBNIMGFKIB.ConverterTypeName == typeof(TConverter).AssemblyQualifiedName))
		{
			TypeDescriptor.AddAttributes(typeof(TConvertible), new TypeConverterAttribute(typeof(TConverter)));
		}
	}

	public static T ChangeType<T>(object value)
	{
		return (T)ChangeType(value, typeof(T));
	}

	public static T ChangeType<T>(object value, IFormatProvider EEGMFLOPLLH)
	{
		return (T)ChangeType(value, typeof(T), EEGMFLOPLLH);
	}

	public static T ChangeType<T>(object value, CultureInfo AGADJJPNKHG)
	{
		return (T)ChangeType(value, typeof(T), AGADJJPNKHG);
	}

	public static object ChangeType(object value, Type ILDBENPMPNB)
	{
		return ChangeType(value, ILDBENPMPNB, CultureInfo.InvariantCulture);
	}

	public static object ChangeType(object value, Type ILDBENPMPNB, IFormatProvider EEGMFLOPLLH)
	{
		return ChangeType(value, ILDBENPMPNB, new CultureInfoAdapter(CultureInfo.CurrentCulture, EEGMFLOPLLH));
	}

	public static object ChangeType(object value, Type ILDBENPMPNB, CultureInfo AGADJJPNKHG)
	{
		if (value == null || value is DBNull)
		{
			return (!ILDBENPMPNB.KLAAGAMNBOB()) ? null : Activator.CreateInstance(ILDBENPMPNB);
		}
		Type type = value.GetType();
		if (ILDBENPMPNB.IsAssignableFrom(type))
		{
			return value;
		}
		if (ILDBENPMPNB.DOGPNFBHJAC())
		{
			Type genericTypeDefinition = ILDBENPMPNB.GetGenericTypeDefinition();
			if (genericTypeDefinition == typeof(Nullable<>))
			{
				Type iLDBENPMPNB = ILDBENPMPNB.GetGenericArguments()[0];
				object obj = ChangeType(value, iLDBENPMPNB, AGADJJPNKHG);
				return Activator.CreateInstance(ILDBENPMPNB, obj);
			}
		}
		if (ILDBENPMPNB.LCAJNDEBEFB())
		{
			string text = value as string;
			return (text == null) ? value : Enum.Parse(ILDBENPMPNB, text, true);
		}
		if (ILDBENPMPNB == typeof(bool))
		{
			if ("0".Equals(value))
			{
				return false;
			}
			if ("1".Equals(value))
			{
				return true;
			}
		}
		System.ComponentModel.TypeConverter converter = TypeDescriptor.GetConverter(value);
		if (converter != null && converter.CanConvertTo(ILDBENPMPNB))
		{
			return converter.ConvertTo(null, AGADJJPNKHG, value, ILDBENPMPNB);
		}
		System.ComponentModel.TypeConverter converter2 = TypeDescriptor.GetConverter(ILDBENPMPNB);
		if (converter2 != null && converter2.CanConvertFrom(type))
		{
			return converter2.ConvertFrom(null, AGADJJPNKHG, value);
		}
		Type[] array = new Type[2] { type, ILDBENPMPNB };
		foreach (Type lFLGCDNKNJI in array)
		{
			foreach (MethodInfo item in lFLGCDNKNJI.GetPublicMethods())
			{
				if (!item.IsSpecialName || (!(item.Name == "op_Implicit") && !(item.Name == "op_Explicit")) || !ILDBENPMPNB.IsAssignableFrom(item.ReturnParameter.ParameterType))
				{
					continue;
				}
				ParameterInfo[] parameters = item.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(type))
				{
					try
					{
						return item.Invoke(null, new object[1] { value });
					}
					catch (TargetInvocationException mPFFFAOGBJE)
					{
						throw mPFFFAOGBJE.Unwrap();
					}
				}
			}
		}
		if (type == typeof(string))
		{
			try
			{
				MethodInfo methodInfo = ILDBENPMPNB.HBBFJLHBHPF("Parse", typeof(string), typeof(IFormatProvider));
				if (methodInfo != null)
				{
					return methodInfo.Invoke(null, new object[2] { value, AGADJJPNKHG });
				}
				methodInfo = ILDBENPMPNB.HBBFJLHBHPF("Parse", typeof(string));
				if (methodInfo != null)
				{
					return methodInfo.Invoke(null, new object[1] { value });
				}
			}
			catch (TargetInvocationException mPFFFAOGBJE2)
			{
				throw mPFFFAOGBJE2.Unwrap();
			}
		}
		if (ILDBENPMPNB == typeof(TimeSpan))
		{
			return TimeSpan.Parse((string)ChangeType(value, typeof(string), CultureInfo.InvariantCulture));
		}
		return Convert.ChangeType(value, ILDBENPMPNB, CultureInfo.InvariantCulture);
	}

	public static T TryParse<T>(string value) where T : struct
	{
		switch (typeof(T).GetTypeCode())
		{
		case TypeCode.Boolean:
			return (T)(object)TryParse<bool>(value, bool.TryParse);
		case TypeCode.Byte:
			return (T)(object)TryParse<byte>(value, byte.TryParse);
		case TypeCode.DateTime:
			return (T)(object)TryParse<DateTime>(value, DateTime.TryParse);
		case TypeCode.Decimal:
			return (T)(object)TryParse<decimal>(value, decimal.TryParse);
		case TypeCode.Double:
			return (T)(object)TryParse<double>(value, double.TryParse);
		case TypeCode.Int16:
			return (T)(object)TryParse<short>(value, short.TryParse);
		case TypeCode.Int32:
			return (T)(object)TryParse<int>(value, int.TryParse);
		case TypeCode.Int64:
			return (T)(object)TryParse<long>(value, long.TryParse);
		case TypeCode.SByte:
			return (T)(object)TryParse<sbyte>(value, sbyte.TryParse);
		case TypeCode.Single:
			return (T)(object)TryParse<float>(value, float.TryParse);
		case TypeCode.UInt16:
			return (T)(object)TryParse<ushort>(value, ushort.TryParse);
		case TypeCode.UInt32:
			return (T)(object)TryParse<uint>(value, uint.TryParse);
		case TypeCode.UInt64:
			return (T)(object)TryParse<ulong>(value, ulong.TryParse);
		default:
			throw new NotSupportedException(string.Format("Cannot parse type '{0}'.", typeof(T).FullName));
		}
	}

	public static T? TryParse<T>(string value, DNEKCDAFFIG<T> EGJGOEKBKEK) where T : struct
	{
		T DCJLKCFKCOM;
		return (!EGJGOEKBKEK(value, out DCJLKCFKCOM)) ? ((T?)null) : new T?(DCJLKCFKCOM);
	}
}
