using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public sealed class ReadablePropertiesTypeInspector : TypeInspectorSkeleton
{
	private sealed class DADEDECJHLJ : IPropertyDescriptor
	{
		private readonly PropertyInfo _propertyInfo;

		private readonly ITypeResolver IIIIGEFELNH;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type CHCJICCKKDF;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int PAOBFNKOJED;

		public string MENAJEAJJBE
		{
			get
			{
				return get_Name();
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

		public DADEDECJHLJ(PropertyInfo OOEBLPMKOIH, ITypeResolver CBMKGNIHPFO)
		{
			_propertyInfo = OOEBLPMKOIH;
			IIIIGEFELNH = CBMKGNIHPFO;
		}

		public string get_Name()
		{
			return _propertyInfo.Name;
		}

		public Type get_Type()
		{
			return _propertyInfo.PropertyType;
		}

		public Type MAGHEGMMNOF()
		{
			return CHCJICCKKDF;
		}

		public void set_TypeOverride(Type value)
		{
			CHCJICCKKDF = value;
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
			return _propertyInfo.CanWrite;
		}

		public void Write(object target, object value)
		{
			_propertyInfo.SetValue(target, value, null);
		}

		public T PJLLHGDNCIF<T>() where T : Attribute
		{
			object[] customAttributes = _propertyInfo.GetCustomAttributes(typeof(T), true);
			return (T)customAttributes.FirstOrDefault();
		}

		public IObjectDescriptor Read(object target)
		{
			object value = _propertyInfo.GetValue(target, null);
			Type lFLGCDNKNJI = MAGHEGMMNOF() ?? IIIIGEFELNH.Resolve(get_Type(), value);
			return new ObjectDescriptor(value, lFLGCDNKNJI, get_Type());
		}
	}

	private readonly ITypeResolver IIIIGEFELNH;

	public ReadablePropertiesTypeInspector(ITypeResolver CBMKGNIHPFO)
	{
		if (CBMKGNIHPFO == null)
		{
			throw new ArgumentNullException("typeResolver");
		}
		IIIIGEFELNH = CBMKGNIHPFO;
	}

	private static bool DKAIPPGKJKP(PropertyInfo JLCGLCLEGBD)
	{
		return JLCGLCLEGBD.CanRead && JLCGLCLEGBD.GetGetMethod().GetParameters().Length == 0;
	}

	public override IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO)
	{
		return LFLGCDNKNJI.GetPublicProperties().Where(DKAIPPGKJKP).Select((Func<PropertyInfo, IPropertyDescriptor>)((PropertyInfo PIIEECCHMAC) => new DADEDECJHLJ(PIIEECCHMAC, IIIIGEFELNH)));
	}
}
