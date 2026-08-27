using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

public class JsonMapper
{
	private static int max_nesting_depth;

	private static IFormatProvider datetime_format;

	private static IDictionary<Type, NHMEKPMHION> IMCAADAMJHN;

	private static IDictionary<Type, NHMEKPMHION> OMCHNENIPCG;

	private static IDictionary<Type, IDictionary<Type, IPPLMFLBMNF>> NPNPINLKCJI;

	private static IDictionary<Type, IDictionary<Type, IPPLMFLBMNF>> LAGBLNLAEMP;

	private static IDictionary<Type, ArrayMetadata> AKKLPIBNOBF;

	private static readonly object LDIBELAMPFN;

	private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;

	private static readonly object EOHFPICDBPO;

	private static IDictionary<Type, ObjectMetadata> PPHNFLFDOKC;

	private static readonly object KNHOHFIBAKL;

	private static IDictionary<Type, IList<PropertyMetadata>> GDHIBNDGFJF;

	private static readonly object MEGNGAGLMJK;

	private static JsonWriter EEDAGKPADCB;

	private static readonly object PLLPBCJPKDD;

	static JsonMapper()
	{
		LDIBELAMPFN = new object();
		EOHFPICDBPO = new object();
		KNHOHFIBAKL = new object();
		MEGNGAGLMJK = new object();
		PLLPBCJPKDD = new object();
		max_nesting_depth = 100;
		AKKLPIBNOBF = new Dictionary<Type, ArrayMetadata>();
		conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
		PPHNFLFDOKC = new Dictionary<Type, ObjectMetadata>();
		GDHIBNDGFJF = new Dictionary<Type, IList<PropertyMetadata>>();
		EEDAGKPADCB = new JsonWriter();
		datetime_format = DateTimeFormatInfo.InvariantInfo;
		IMCAADAMJHN = new Dictionary<Type, NHMEKPMHION>();
		OMCHNENIPCG = new Dictionary<Type, NHMEKPMHION>();
		NPNPINLKCJI = new Dictionary<Type, IDictionary<Type, IPPLMFLBMNF>>();
		LAGBLNLAEMP = new Dictionary<Type, IDictionary<Type, IPPLMFLBMNF>>();
		DHJHOMFGKEI();
		CHDOGHKLCKK();
	}

	private static bool HasInterface(Type LFLGCDNKNJI, string name)
	{
		return LFLGCDNKNJI.GetInterface(name, true) != null;
	}

	public static PropertyInfo[] GetPublicInstanceProperties(Type LFLGCDNKNJI)
	{
		return LFLGCDNKNJI.GetProperties();
	}

	private static void KAAODNAIGGM(Type LFLGCDNKNJI)
	{
		if (AKKLPIBNOBF.ContainsKey(LFLGCDNKNJI))
		{
			return;
		}
		ArrayMetadata value = default(ArrayMetadata);
		value.GDKDBPFDCIJ(LFLGCDNKNJI.IsArray);
		if (HasInterface(LFLGCDNKNJI, "System.Collections.IList"))
		{
			value.ICHOKOLOLKC(true);
		}
		PropertyInfo[] array = GetPublicInstanceProperties(LFLGCDNKNJI);
		foreach (PropertyInfo propertyInfo in array)
		{
			if (!(propertyInfo.Name != "Item"))
			{
				ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
				if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
				{
					value.set_ElementType(propertyInfo.PropertyType);
				}
			}
		}
		lock (LDIBELAMPFN)
		{
			try
			{
				AKKLPIBNOBF.Add(LFLGCDNKNJI, value);
			}
			catch (ArgumentException)
			{
			}
		}
	}

	private static void CDLLCDFOOFF(Type LFLGCDNKNJI)
	{
		if (PPHNFLFDOKC.ContainsKey(LFLGCDNKNJI))
		{
			return;
		}
		ObjectMetadata value = default(ObjectMetadata);
		if (HasInterface(LFLGCDNKNJI, "System.Collections.IDictionary"))
		{
			value.set_IsDictionary(true);
		}
		value.IJAFNNMLFNF(new Dictionary<string, PropertyMetadata>());
		PropertyInfo[] array = GetPublicInstanceProperties(LFLGCDNKNJI);
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.Name == "Item")
			{
				ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
				if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(string))
				{
					value.set_ElementType(propertyInfo.PropertyType);
				}
			}
			else
			{
				PropertyMetadata value2 = new PropertyMetadata
				{
					Info = propertyInfo,
					Type = propertyInfo.PropertyType
				};
				value.FABLBHDIKCN().Add(propertyInfo.Name, value2);
			}
		}
		FieldInfo[] fields = LFLGCDNKNJI.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			PropertyMetadata value3 = new PropertyMetadata
			{
				Info = fieldInfo,
				IsField = true,
				Type = fieldInfo.FieldType
			};
			value.FABLBHDIKCN().Add(fieldInfo.Name, value3);
		}
		lock (KNHOHFIBAKL)
		{
			try
			{
				PPHNFLFDOKC.Add(LFLGCDNKNJI, value);
			}
			catch (ArgumentException)
			{
			}
		}
	}

	private static void KKLKKHBEAPJ(Type LFLGCDNKNJI)
	{
		if (GDHIBNDGFJF.ContainsKey(LFLGCDNKNJI))
		{
			return;
		}
		IList<PropertyMetadata> list = new List<PropertyMetadata>();
		PropertyInfo[] array = GetPublicInstanceProperties(LFLGCDNKNJI);
		foreach (PropertyInfo propertyInfo in array)
		{
			if (!(propertyInfo.Name == "Item"))
			{
				list.Add(new PropertyMetadata
				{
					Info = propertyInfo,
					IsField = false
				});
			}
		}
		FieldInfo[] fields = LFLGCDNKNJI.GetFields();
		foreach (FieldInfo cGJLHJGIGHD in fields)
		{
			list.Add(new PropertyMetadata
			{
				Info = cGJLHJGIGHD,
				IsField = true
			});
		}
		lock (MEGNGAGLMJK)
		{
			try
			{
				GDHIBNDGFJF.Add(LFLGCDNKNJI, list);
			}
			catch (ArgumentException)
			{
			}
		}
	}

	private static MethodInfo GetConvOp(Type GKLIKMLCGFB, Type KGONIIAMHAP)
	{
		lock (EOHFPICDBPO)
		{
			if (!conv_ops.ContainsKey(GKLIKMLCGFB))
			{
				conv_ops.Add(GKLIKMLCGFB, new Dictionary<Type, MethodInfo>());
			}
		}
		if (conv_ops[GKLIKMLCGFB].ContainsKey(KGONIIAMHAP))
		{
			return conv_ops[GKLIKMLCGFB][KGONIIAMHAP];
		}
		MethodInfo method = GKLIKMLCGFB.GetMethod("op_Implicit", new Type[1] { KGONIIAMHAP });
		lock (EOHFPICDBPO)
		{
			try
			{
				conv_ops[GKLIKMLCGFB].Add(KGONIIAMHAP, method);
				return method;
			}
			catch (ArgumentException)
			{
				return conv_ops[GKLIKMLCGFB][KGONIIAMHAP];
			}
		}
	}

	private static object ReadValue(Type DHLFLIEGLOK, JsonReader reader)
	{
		reader.Read();
		if (reader.EACDJONMMAP() == GDDEBPANOCH.ArrayEnd)
		{
			return null;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Null)
		{
			if (!DHLFLIEGLOK.IsClass)
			{
				throw new JsonException(string.Format("Can't assign null to an instance of type {0}", DHLFLIEGLOK));
			}
			return null;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Double || reader.EACDJONMMAP() == GDDEBPANOCH.Int || reader.EACDJONMMAP() == GDDEBPANOCH.Long || reader.EACDJONMMAP() == GDDEBPANOCH.String || reader.EACDJONMMAP() == GDDEBPANOCH.Boolean)
		{
			Type type = reader.OEAKCOHMIHH().GetType();
			if (DHLFLIEGLOK.IsAssignableFrom(type))
			{
				return reader.OEAKCOHMIHH();
			}
			if (LAGBLNLAEMP.ContainsKey(type) && LAGBLNLAEMP[type].ContainsKey(DHLFLIEGLOK))
			{
				IPPLMFLBMNF iPPLMFLBMNF = LAGBLNLAEMP[type][DHLFLIEGLOK];
				return iPPLMFLBMNF(reader.OEAKCOHMIHH());
			}
			if (NPNPINLKCJI.ContainsKey(type) && NPNPINLKCJI[type].ContainsKey(DHLFLIEGLOK))
			{
				IPPLMFLBMNF iPPLMFLBMNF2 = NPNPINLKCJI[type][DHLFLIEGLOK];
				return iPPLMFLBMNF2(reader.OEAKCOHMIHH());
			}
			if (DHLFLIEGLOK.IsEnum)
			{
				return Enum.ToObject(DHLFLIEGLOK, reader.OEAKCOHMIHH());
			}
			MethodInfo methodInfo = GetConvOp(DHLFLIEGLOK, type);
			if (methodInfo != null)
			{
				return methodInfo.Invoke(null, new object[1] { reader.OEAKCOHMIHH() });
			}
			throw new JsonException(string.Format("Can't assign value '{0}' (type {1}) to type {2}", reader.OEAKCOHMIHH(), type, DHLFLIEGLOK));
		}
		object obj = null;
		if (reader.EACDJONMMAP() == GDDEBPANOCH.ArrayStart)
		{
			if (DHLFLIEGLOK.FullName == "System.Object")
			{
				DHLFLIEGLOK = typeof(object[]);
			}
			KAAODNAIGGM(DHLFLIEGLOK);
			ArrayMetadata gKAHCDHJLBP = AKKLPIBNOBF[DHLFLIEGLOK];
			if (!gKAHCDHJLBP.NKLOBJNAFOL() && !gKAHCDHJLBP.FOIBIKPNLJD())
			{
				throw new JsonException(string.Format("Type {0} can't act as an array", DHLFLIEGLOK));
			}
			IList list;
			Type type2;
			if (!gKAHCDHJLBP.NKLOBJNAFOL())
			{
				list = (IList)Activator.CreateInstance(DHLFLIEGLOK);
				type2 = gKAHCDHJLBP.LPINKHOCABG();
			}
			else
			{
				list = new ArrayList();
				type2 = DHLFLIEGLOK.GetElementType();
			}
			while (true)
			{
				object obj2 = ReadValue(type2, reader);
				if (obj2 == null && reader.EACDJONMMAP() == GDDEBPANOCH.ArrayEnd)
				{
					break;
				}
				list.Add(obj2);
			}
			if (gKAHCDHJLBP.NKLOBJNAFOL())
			{
				int count = list.Count;
				obj = Array.CreateInstance(type2, count);
				for (int i = 0; i < count; i++)
				{
					((Array)obj).SetValue(list[i], i);
				}
			}
			else
			{
				obj = list;
			}
		}
		else if (reader.EACDJONMMAP() == GDDEBPANOCH.ObjectStart)
		{
			if (DHLFLIEGLOK == typeof(object))
			{
				DHLFLIEGLOK = typeof(Dictionary<string, object>);
			}
			CDLLCDFOOFF(DHLFLIEGLOK);
			ObjectMetadata iNPMDAGENOB = PPHNFLFDOKC[DHLFLIEGLOK];
			obj = Activator.CreateInstance(DHLFLIEGLOK);
			while (true)
			{
				reader.Read();
				if (reader.EACDJONMMAP() == GDDEBPANOCH.ObjectEnd)
				{
					break;
				}
				string text = (string)reader.OEAKCOHMIHH();
				if (iNPMDAGENOB.FABLBHDIKCN().ContainsKey(text))
				{
					PropertyMetadata nOCGBHBELKO = iNPMDAGENOB.FABLBHDIKCN()[text];
					if (nOCGBHBELKO.IsField)
					{
						((FieldInfo)nOCGBHBELKO.Info).SetValue(obj, ReadValue(nOCGBHBELKO.Type, reader));
						continue;
					}
					PropertyInfo propertyInfo = (PropertyInfo)nOCGBHBELKO.Info;
					if (propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(obj, ReadValue(nOCGBHBELKO.Type, reader), null);
					}
					else
					{
						ReadValue(nOCGBHBELKO.Type, reader);
					}
				}
				else if (!iNPMDAGENOB.PMPFFNMIKAN())
				{
					if (!reader.AIGMJENINOM())
					{
						throw new JsonException(string.Format("The type {0} doesn't have the property '{1}'", DHLFLIEGLOK, text));
					}
					PKEKHFDDDEL(reader);
				}
				else
				{
					((IDictionary)obj).Add(text, ReadValue(iNPMDAGENOB.LPINKHOCABG(), reader));
				}
			}
		}
		return obj;
	}

	private static IJsonWrapper ReadValue(WrapperFactory DJFCIPIMOBC, JsonReader reader)
	{
		reader.Read();
		if (reader.EACDJONMMAP() == GDDEBPANOCH.ArrayEnd || reader.EACDJONMMAP() == GDDEBPANOCH.Null)
		{
			return null;
		}
		IJsonWrapper pIIMPPKAOCI = DJFCIPIMOBC();
		if (reader.EACDJONMMAP() == GDDEBPANOCH.String)
		{
			pIIMPPKAOCI.SetString((string)reader.OEAKCOHMIHH());
			return pIIMPPKAOCI;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Double)
		{
			pIIMPPKAOCI.SetDouble((double)reader.OEAKCOHMIHH());
			return pIIMPPKAOCI;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Int)
		{
			pIIMPPKAOCI.SetInt((int)reader.OEAKCOHMIHH());
			return pIIMPPKAOCI;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Long)
		{
			pIIMPPKAOCI.SetLong((long)reader.OEAKCOHMIHH());
			return pIIMPPKAOCI;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.Boolean)
		{
			pIIMPPKAOCI.SetBoolean((bool)reader.OEAKCOHMIHH());
			return pIIMPPKAOCI;
		}
		if (reader.EACDJONMMAP() == GDDEBPANOCH.ArrayStart)
		{
			pIIMPPKAOCI.FJKDNANFIHA(GGIECEPGFNH.Array);
			while (true)
			{
				IJsonWrapper pIIMPPKAOCI2 = ReadValue(DJFCIPIMOBC, reader);
				if (pIIMPPKAOCI2 == null && reader.EACDJONMMAP() == GDDEBPANOCH.ArrayEnd)
				{
					break;
				}
				pIIMPPKAOCI.Add(pIIMPPKAOCI2);
			}
		}
		else if (reader.EACDJONMMAP() == GDDEBPANOCH.ObjectStart)
		{
			pIIMPPKAOCI.FJKDNANFIHA(GGIECEPGFNH.Object);
			while (true)
			{
				reader.Read();
				if (reader.EACDJONMMAP() == GDDEBPANOCH.ObjectEnd)
				{
					break;
				}
				string key = (string)reader.OEAKCOHMIHH();
				pIIMPPKAOCI[key] = ReadValue(DJFCIPIMOBC, reader);
			}
		}
		return pIIMPPKAOCI;
	}

	private static void PKEKHFDDDEL(JsonReader reader)
	{
		BOIDOLGHDMO(() => new JsonMockWrapper(), reader);
	}

	private static void DHJHOMFGKEI()
	{
		IMCAADAMJHN[typeof(byte)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToInt32((byte)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(char)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToString((char)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(DateTime)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToString((DateTime)AOMLCBHAJJH, datetime_format));
		};
		IMCAADAMJHN[typeof(decimal)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write((decimal)AOMLCBHAJJH);
		};
		IMCAADAMJHN[typeof(sbyte)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToInt32((sbyte)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(short)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToInt32((short)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(ushort)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToInt32((ushort)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(uint)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write(Convert.ToUInt64((uint)AOMLCBHAJJH));
		};
		IMCAADAMJHN[typeof(ulong)] = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			writer.Write((ulong)AOMLCBHAJJH);
		};
	}

	private static void CHDOGHKLCKK()
	{
		IPPLMFLBMNF pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToByte((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(byte), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToUInt64((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(ulong), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToSByte((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(sbyte), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToInt16((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(short), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToUInt16((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(ushort), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToUInt32((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(uint), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToSingle((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(float), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToDouble((int)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(int), typeof(double), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToDecimal((double)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(double), typeof(decimal), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToUInt32((long)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(long), typeof(uint), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToChar((string)NILNDHEKNLJ);
		CGEKEABEIKN(NPNPINLKCJI, typeof(string), typeof(char), pAOKGMMLPNC);
		pAOKGMMLPNC = (object NILNDHEKNLJ) => Convert.ToDateTime((string)NILNDHEKNLJ, datetime_format);
		CGEKEABEIKN(NPNPINLKCJI, typeof(string), typeof(DateTime), pAOKGMMLPNC);
	}

	private static void CGEKEABEIKN(IDictionary<Type, IDictionary<Type, IPPLMFLBMNF>> BFGHBIMJHAK, Type FMOEMAEOMCK, Type MFCKGHCBHGC, IPPLMFLBMNF PAOKGMMLPNC)
	{
		if (!BFGHBIMJHAK.ContainsKey(FMOEMAEOMCK))
		{
			BFGHBIMJHAK.Add(FMOEMAEOMCK, new Dictionary<Type, IPPLMFLBMNF>());
		}
		BFGHBIMJHAK[FMOEMAEOMCK][MFCKGHCBHGC] = PAOKGMMLPNC;
	}

	private static void OBMNFCMPEPP(object AOMLCBHAJJH, JsonWriter writer, bool PHKCIGJLGDM, int depth)
	{
		if (depth > max_nesting_depth)
		{
			throw new JsonException(string.Format("Max allowed object depth reached while trying to export from type {0}", AOMLCBHAJJH.GetType()));
		}
		if (AOMLCBHAJJH == null)
		{
			writer.Write(null);
			return;
		}
		if (AOMLCBHAJJH is IJsonWrapper)
		{
			if (PHKCIGJLGDM)
			{
				writer.ONBOFGNMJEN().Write(((IJsonWrapper)AOMLCBHAJJH).ToJson());
			}
			else
			{
				((IJsonWrapper)AOMLCBHAJJH).ToJson(writer);
			}
			return;
		}
		if (AOMLCBHAJJH is string)
		{
			writer.Write((string)AOMLCBHAJJH);
			return;
		}
		if (AOMLCBHAJJH is double)
		{
			writer.Write((double)AOMLCBHAJJH);
			return;
		}
		if (AOMLCBHAJJH is int)
		{
			writer.Write((int)AOMLCBHAJJH);
			return;
		}
		if (AOMLCBHAJJH is bool)
		{
			writer.Write((bool)AOMLCBHAJJH);
			return;
		}
		if (AOMLCBHAJJH is long)
		{
			writer.Write((long)AOMLCBHAJJH);
			return;
		}
		if (AOMLCBHAJJH is Array)
		{
			writer.AGGBIHCJOKF();
			foreach (object item in (Array)AOMLCBHAJJH)
			{
				OBMNFCMPEPP(item, writer, PHKCIGJLGDM, depth + 1);
			}
			writer.FMIALOIGMFH();
			return;
		}
		if (AOMLCBHAJJH is IList)
		{
			writer.AGGBIHCJOKF();
			foreach (object item2 in (IList)AOMLCBHAJJH)
			{
				OBMNFCMPEPP(item2, writer, PHKCIGJLGDM, depth + 1);
			}
			writer.FMIALOIGMFH();
			return;
		}
		if (AOMLCBHAJJH is IDictionary)
		{
			writer.ACCDHGHBCHM();
			foreach (DictionaryEntry item3 in (IDictionary)AOMLCBHAJJH)
			{
				writer.MPKEMEAPPJL((string)item3.Key);
				OBMNFCMPEPP(item3.Value, writer, PHKCIGJLGDM, depth + 1);
			}
			writer.KDAIDMBDFHB();
			return;
		}
		Type type = AOMLCBHAJJH.GetType();
		if (OMCHNENIPCG.ContainsKey(type))
		{
			NHMEKPMHION nHMEKPMHION = OMCHNENIPCG[type];
			nHMEKPMHION(AOMLCBHAJJH, writer);
			return;
		}
		if (IMCAADAMJHN.ContainsKey(type))
		{
			NHMEKPMHION nHMEKPMHION2 = IMCAADAMJHN[type];
			nHMEKPMHION2(AOMLCBHAJJH, writer);
			return;
		}
		if (AOMLCBHAJJH is Enum)
		{
			Type underlyingType = Enum.GetUnderlyingType(type);
			if (underlyingType == typeof(long) || underlyingType == typeof(uint) || underlyingType == typeof(ulong))
			{
				writer.Write((ulong)AOMLCBHAJJH);
			}
			else
			{
				writer.Write((int)AOMLCBHAJJH);
			}
			return;
		}
		KKLKKHBEAPJ(type);
		IList<PropertyMetadata> list = GDHIBNDGFJF[type];
		writer.ACCDHGHBCHM();
		foreach (PropertyMetadata item4 in list)
		{
			if (item4.IsField)
			{
				writer.MPKEMEAPPJL(item4.Info.Name);
				OBMNFCMPEPP(((FieldInfo)item4.Info).GetValue(AOMLCBHAJJH), writer, PHKCIGJLGDM, depth + 1);
				continue;
			}
			PropertyInfo propertyInfo = (PropertyInfo)item4.Info;
			if (propertyInfo.CanRead)
			{
				writer.MPKEMEAPPJL(item4.Info.Name);
				OBMNFCMPEPP(propertyInfo.GetValue(AOMLCBHAJJH, null), writer, PHKCIGJLGDM, depth + 1);
			}
		}
		writer.KDAIDMBDFHB();
	}

	public static string ToJson(object AOMLCBHAJJH)
	{
		lock (PLLPBCJPKDD)
		{
			EEDAGKPADCB.Reset();
			OBMNFCMPEPP(AOMLCBHAJJH, EEDAGKPADCB, true, 0);
			return EEDAGKPADCB.ToString();
		}
	}

	public static void ToJson(object AOMLCBHAJJH, JsonWriter writer)
	{
		OBMNFCMPEPP(AOMLCBHAJJH, writer, false, 0);
	}

	public static JsonData ToObject(JsonReader reader)
	{
		return (JsonData)BOIDOLGHDMO(() => new JsonData(), reader);
	}

	public static JsonData ToObject(TextReader reader)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(reader);
		return (JsonData)BOIDOLGHDMO(() => new JsonData(), iJIMLLIHKGN);
	}

	public static JsonData ToObject(string EMDHMHOKGFP)
	{
		return (JsonData)BOIDOLGHDMO(() => new JsonData(), EMDHMHOKGFP);
	}

	public static T ToObject<T>(JsonReader reader)
	{
		return (T)ReadValue(typeof(T), reader);
	}

	public static T ToObject<T>(TextReader reader)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(reader);
		return (T)ReadValue(typeof(T), iJIMLLIHKGN);
	}

	public static T ToObject<T>(string EMDHMHOKGFP)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(EMDHMHOKGFP);
		return (T)ReadValue(typeof(T), iJIMLLIHKGN);
	}

	public static IJsonWrapper BOIDOLGHDMO(WrapperFactory DJFCIPIMOBC, JsonReader reader)
	{
		return ReadValue(DJFCIPIMOBC, reader);
	}

	public static IJsonWrapper BOIDOLGHDMO(WrapperFactory DJFCIPIMOBC, string EMDHMHOKGFP)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(EMDHMHOKGFP);
		return ReadValue(DJFCIPIMOBC, iJIMLLIHKGN);
	}

	public static void ICGBDPPPPOE<T>(global::NDIMGDKIMBD<T> ACIKPHKFNMH)
	{
		NHMEKPMHION value = (object AOMLCBHAJJH, JsonWriter writer) =>
		{
			ACIKPHKFNMH((T)AOMLCBHAJJH, writer);
		};
		OMCHNENIPCG[typeof(T)] = value;
	}

	public static void CGEKEABEIKN<TJson, TValue>(global::LADGKBEOMDK<TJson, TValue> PAOKGMMLPNC)
	{
		IPPLMFLBMNF pAOKGMMLPNC = (object NILNDHEKNLJ) => PAOKGMMLPNC((TJson)NILNDHEKNLJ);
		CGEKEABEIKN(LAGBLNLAEMP, typeof(TJson), typeof(TValue), pAOKGMMLPNC);
	}

	public static void AGBCKCOOBGL()
	{
		OMCHNENIPCG.Clear();
	}

	public static void KNMBLCIEKCM()
	{
		LAGBLNLAEMP.Clear();
	}
}
