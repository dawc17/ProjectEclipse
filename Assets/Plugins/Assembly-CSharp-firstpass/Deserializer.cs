using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public sealed class Deserializer
{
	private class NLKHGNDHMPN : ITypeInspector
	{
		public ITypeInspector FAOFCLFMJCE;

		public IEnumerable<IPropertyDescriptor> GHIBHNJKIHN(Type LFLGCDNKNJI, object EGJHGBCEPHO)
		{
			return FAOFCLFMJCE.GHIBHNJKIHN(LFLGCDNKNJI, EGJHGBCEPHO);
		}

		public IPropertyDescriptor DBLHKMEGOEK(Type LFLGCDNKNJI, object EGJHGBCEPHO, string name, bool GNFDAJLHBCN)
		{
			return FAOFCLFMJCE.DBLHKMEGOEK(LFLGCDNKNJI, EGJHGBCEPHO, name, GNFDAJLHBCN);
		}
	}

	private static readonly Dictionary<string, Type> LNGMOAOAOJD = new Dictionary<string, Type>
	{
		{
			"tag:yaml.org,2002:map",
			typeof(Dictionary<object, object>)
		},
		{
			"tag:yaml.org,2002:bool",
			typeof(bool)
		},
		{
			"tag:yaml.org,2002:float",
			typeof(double)
		},
		{
			"tag:yaml.org,2002:int",
			typeof(int)
		},
		{
			"tag:yaml.org,2002:str",
			typeof(string)
		},
		{
			"tag:yaml.org,2002:timestamp",
			typeof(DateTime)
		}
	};

	private readonly Dictionary<string, Type> NKEHCGOLJDA;

	private readonly List<IYamlTypeConverter> JNONHBMNKDK;

	private NLKHGNDHMPN GIJPGEHPILC = new NLKHGNDHMPN();

	private FFBEMOKFDNL BMPLGIJNAMB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IList<INodeDeserializer> GAPFMLMAKEG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IList<INodeTypeResolver> AIHKIAPGMFP;

	public IList<INodeDeserializer> JLMLOIPNMMH
	{
		get
		{
			return BBENCENNNED();
		}
		private set
		{
			LEJFLAHHNIK(value);
		}
	}

	public IList<INodeTypeResolver> LBEGHDIFABA
	{
		get
		{
			return MJDOCMKDKCG();
		}
		private set
		{
			LFPINFCODLG(value);
		}
	}

	public Deserializer(IObjectFactory EJPHFDCKCCE = null, INamingConvention LELOAKPLJEH = null, bool GNFDAJLHBCN = false)
	{
		EJPHFDCKCCE = EJPHFDCKCCE ?? new DefaultObjectFactory();
		LELOAKPLJEH = LELOAKPLJEH ?? new GCEHGHHBNOA();
		GIJPGEHPILC.FAOFCLFMJCE = new YamlAttributesTypeInspector(new NamingConventionTypeInspector(new ReadableAndWritablePropertiesTypeInspector(new ReadablePropertiesTypeInspector(new NBGCAMIJKDA())), LELOAKPLJEH));
		JNONHBMNKDK = new List<IYamlTypeConverter>();
		foreach (IYamlTypeConverter item in YamlTypeConverters.LGAMHDFHDHG())
		{
			JNONHBMNKDK.Add(item);
		}
		LEJFLAHHNIK(new List<INodeDeserializer>());
		BBENCENNNED().Add(new TypeConverterNodeDeserializer(JNONHBMNKDK));
		BBENCENNNED().Add(new NullNodeDeserializer());
		BBENCENNNED().Add(new ScalarNodeDeserializer());
		BBENCENNNED().Add(new ArrayNodeDeserializer());
		BBENCENNNED().Add(new GenericDictionaryNodeDeserializer(EJPHFDCKCCE));
		BBENCENNNED().Add(new EBINODDAIEO(EJPHFDCKCCE));
		BBENCENNNED().Add(new GenericCollectionNodeDeserializer(EJPHFDCKCCE));
		BBENCENNNED().Add(new CNJMKCEKKNN(EJPHFDCKCCE));
		BBENCENNNED().Add(new EnumerableNodeDeserializer());
		BBENCENNNED().Add(new ObjectNodeDeserializer(EJPHFDCKCCE, GIJPGEHPILC, GNFDAJLHBCN));
		NKEHCGOLJDA = new Dictionary<string, Type>(LNGMOAOAOJD);
		LFPINFCODLG(new List<INodeTypeResolver>());
		MJDOCMKDKCG().Add(new TagNodeTypeResolver(NKEHCGOLJDA));
		MJDOCMKDKCG().Add(new CKPADCLAMFF());
		MJDOCMKDKCG().Add(new FDDLCAEBEHB());
		BMPLGIJNAMB = new AliasValueDeserializer(new NodeValueDeserializer(BBENCENNNED(), MJDOCMKDKCG()));
	}

	public IList<INodeDeserializer> BBENCENNNED()
	{
		return GAPFMLMAKEG;
	}

	private void LEJFLAHHNIK(IList<INodeDeserializer> value)
	{
		GAPFMLMAKEG = value;
	}

	public IList<INodeTypeResolver> MJDOCMKDKCG()
	{
		return AIHKIAPGMFP;
	}

	private void LFPINFCODLG(IList<INodeTypeResolver> value)
	{
		AIHKIAPGMFP = value;
	}

	public void RegisterTagMapping(string EDLADAAKMDF, Type LFLGCDNKNJI)
	{
		NKEHCGOLJDA.Add(EDLADAAKMDF, LFLGCDNKNJI);
	}

	public void JCPBBODBIBI(IYamlTypeConverter OOBNDNCCFJI)
	{
		JNONHBMNKDK.Add(OOBNDNCCFJI);
	}

	public T Deserialize<T>(TextReader NILNDHEKNLJ)
	{
		return (T)Deserialize(NILNDHEKNLJ, typeof(T));
	}

	public object Deserialize(TextReader NILNDHEKNLJ)
	{
		return Deserialize(NILNDHEKNLJ, typeof(object));
	}

	public object Deserialize(TextReader NILNDHEKNLJ, Type LFLGCDNKNJI)
	{
		return Deserialize(new EventReader(new APMHDDIADMF(NILNDHEKNLJ)), LFLGCDNKNJI);
	}

	public T Deserialize<T>(EventReader reader)
	{
		return (T)Deserialize(reader, typeof(T));
	}

	public object Deserialize(EventReader reader)
	{
		return Deserialize(reader, typeof(object));
	}

	public object Deserialize(EventReader reader, Type LFLGCDNKNJI)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		if (LFLGCDNKNJI == null)
		{
			throw new ArgumentNullException("type");
		}
		bool flag = reader.GNNPKHDPGLN<StreamStart>() != null;
		bool flag2 = reader.GNNPKHDPGLN<DocumentStart>() != null;
		object result = null;
		if (!reader.GPHIFFOGOGN<DocumentEnd>() && !reader.GPHIFFOGOGN<HNKFEGCMBJB>())
		{
			using (SerializerState mLKGCMPCCCB = new SerializerState())
			{
				result = BMPLGIJNAMB.BBNMBCMJOFM(reader, LFLGCDNKNJI, mLKGCMPCCCB, BMPLGIJNAMB);
				mLKGCMPCCCB.INOFEFDGNFL();
			}
		}
		if (flag2)
		{
			reader.DODGGCGJJLL<DocumentEnd>();
		}
		if (flag)
		{
			reader.DODGGCGJJLL<HNKFEGCMBJB>();
		}
		return result;
	}
}
