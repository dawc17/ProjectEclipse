using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class FullObjectGraphTraversalStrategy : BIGFDIOHKIG
{
	protected readonly Serializer MGFPEIHBMLD;

	private readonly int maxRecursion;

	private readonly ITypeInspector GIJPGEHPILC;

	private readonly ITypeResolver CBMKGNIHPFO;

	private static readonly global::GenericInstanceMethod<FullObjectGraphTraversalStrategy> KONOFKFLIDP = new global::GenericInstanceMethod<FullObjectGraphTraversalStrategy>((FullObjectGraphTraversalStrategy s) => s.NMPBHMBDKCD<int, int>(null, null, 0));

	public FullObjectGraphTraversalStrategy(Serializer MGFPEIHBMLD, ITypeInspector GIJPGEHPILC, ITypeResolver CBMKGNIHPFO, int maxRecursion)
	{
		if (maxRecursion <= 0)
		{
			throw new ArgumentOutOfRangeException("maxRecursion", maxRecursion, "maxRecursion must be greater than 1");
		}
		this.MGFPEIHBMLD = MGFPEIHBMLD;
		if (GIJPGEHPILC == null)
		{
			throw new ArgumentNullException("typeDescriptor");
		}
		this.GIJPGEHPILC = GIJPGEHPILC;
		if (CBMKGNIHPFO == null)
		{
			throw new ArgumentNullException("typeResolver");
		}
		this.CBMKGNIHPFO = CBMKGNIHPFO;
		this.maxRecursion = maxRecursion;
	}

	void BIGFDIOHKIG.Traverse(IObjectDescriptor OFDNAFPEAGP, IObjectGraphVisitor NKECMANOOEM)
	{
		Traverse(OFDNAFPEAGP, NKECMANOOEM, 0);
	}

	protected virtual void Traverse(IObjectDescriptor value, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		if (++KDONPJHEEBI > maxRecursion)
		{
			throw new InvalidOperationException("Too much recursion when traversing the object graph");
		}
		if (!NKECMANOOEM.Enter(value))
		{
			return;
		}
		TypeCode typeCode = value.get_Type().GetTypeCode();
		switch (typeCode)
		{
		case TypeCode.Boolean:
		case TypeCode.Char:
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
		case TypeCode.DateTime:
		case TypeCode.String:
			NKECMANOOEM.VisitScalar(value);
			return;
		case TypeCode.DBNull:
			NKECMANOOEM.VisitScalar(new ObjectDescriptor(null, typeof(object), typeof(object)));
			return;
		case TypeCode.Empty:
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
		}
		if (value.OEAKCOHMIHH() == null || value.get_Type() == typeof(TimeSpan))
		{
			NKECMANOOEM.VisitScalar(value);
			return;
		}
		Type underlyingType = Nullable.GetUnderlyingType(value.get_Type());
		if (underlyingType != null)
		{
			Traverse(new ObjectDescriptor(value.OEAKCOHMIHH(), underlyingType, value.get_Type()), NKECMANOOEM, KDONPJHEEBI);
		}
		else
		{
			CDDKEFCFMGC(value, NKECMANOOEM, KDONPJHEEBI);
		}
	}

	protected virtual void CDDKEFCFMGC(IObjectDescriptor value, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		if (typeof(IDictionary).IsAssignableFrom(value.get_Type()))
		{
			HFFPLAENAOO(value, NKECMANOOEM, KDONPJHEEBI);
			return;
		}
		Type type = ReflectionUtility.JIDNEGBGBGL(value.get_Type(), typeof(IDictionary<, >));
		if (type != null)
		{
			IGAOOMJINDO(value, type, NKECMANOOEM, KDONPJHEEBI);
		}
		else if (typeof(IEnumerable).IsAssignableFrom(value.get_Type()))
		{
			OLPCLENDEPG(value, NKECMANOOEM, KDONPJHEEBI);
		}
		else
		{
			FGAICKPNBIH(value, NKECMANOOEM, KDONPJHEEBI);
		}
	}

	protected virtual void HFFPLAENAOO(IObjectDescriptor dictionary, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		NKECMANOOEM.VisitMappingStart(dictionary, typeof(object), typeof(object));
		foreach (DictionaryEntry item in (IDictionary)dictionary.OEAKCOHMIHH())
		{
			IObjectDescriptor mKEOBENKHGI = BMEHIHLGKGP(item.Key, typeof(object));
			IObjectDescriptor bAINMLLIKOL = BMEHIHLGKGP(item.Value, typeof(object));
			if (NKECMANOOEM.EnterMapping(mKEOBENKHGI, bAINMLLIKOL))
			{
				Traverse(mKEOBENKHGI, NKECMANOOEM, KDONPJHEEBI);
				Traverse(bAINMLLIKOL, NKECMANOOEM, KDONPJHEEBI);
			}
		}
		NKECMANOOEM.VisitMappingEnd(dictionary);
	}

	private void IGAOOMJINDO(IObjectDescriptor dictionary, Type DDOHIJFGGGO, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		Type[] genericArguments = DDOHIJFGGGO.GetGenericArguments();
		NKECMANOOEM.VisitMappingStart(dictionary, genericArguments[0], genericArguments[1]);
		KONOFKFLIDP.Invoke(genericArguments, this, dictionary.OEAKCOHMIHH(), NKECMANOOEM, KDONPJHEEBI);
		NKECMANOOEM.VisitMappingEnd(dictionary);
	}

	private void NMPBHMBDKCD<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		foreach (KeyValuePair<TKey, TValue> item in dictionary)
		{
			IObjectDescriptor mKEOBENKHGI = BMEHIHLGKGP(item.Key, typeof(TKey));
			IObjectDescriptor bAINMLLIKOL = BMEHIHLGKGP(item.Value, typeof(TValue));
			if (NKECMANOOEM.EnterMapping(mKEOBENKHGI, bAINMLLIKOL))
			{
				Traverse(mKEOBENKHGI, NKECMANOOEM, KDONPJHEEBI);
				Traverse(bAINMLLIKOL, NKECMANOOEM, KDONPJHEEBI);
			}
		}
	}

	private void OLPCLENDEPG(IObjectDescriptor value, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		Type type = ReflectionUtility.JIDNEGBGBGL(value.get_Type(), typeof(IEnumerable<>));
		Type type2 = ((type == null) ? typeof(object) : type.GetGenericArguments()[0]);
		NKECMANOOEM.VisitSequenceStart(value, type2);
		foreach (object item in (IEnumerable)value.OEAKCOHMIHH())
		{
			Traverse(BMEHIHLGKGP(item, type2), NKECMANOOEM, KDONPJHEEBI);
		}
		NKECMANOOEM.VisitSequenceEnd(value);
	}

	protected virtual void FGAICKPNBIH(IObjectDescriptor value, IObjectGraphVisitor NKECMANOOEM, int KDONPJHEEBI)
	{
		NKECMANOOEM.VisitMappingStart(value, typeof(string), typeof(object));
		foreach (IPropertyDescriptor item in GIJPGEHPILC.GHIBHNJKIHN(value.get_Type(), value.OEAKCOHMIHH()))
		{
			IObjectDescriptor bAINMLLIKOL = item.Read(value.OEAKCOHMIHH());
			if (NKECMANOOEM.EnterMapping(item, bAINMLLIKOL))
			{
				Traverse(new ObjectDescriptor(item.get_Name(), typeof(string), typeof(string)), NKECMANOOEM, KDONPJHEEBI);
				Traverse(bAINMLLIKOL, NKECMANOOEM, KDONPJHEEBI);
			}
		}
		NKECMANOOEM.VisitMappingEnd(value);
	}

	private IObjectDescriptor BMEHIHLGKGP(object value, Type FGDJAEMHFKC)
	{
		return new ObjectDescriptor(value, CBMKGNIHPFO.Resolve(FGDJAEMHFKC, value), FGDJAEMHFKC);
	}
}
