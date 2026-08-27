using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

[DefaultMember("Item")]
public class JsonData : IEquatable<JsonData>, IDictionary, IList, IEnumerable, ICollection, IOrderedDictionary, IJsonWrapper
{
	private IList<JsonData> inst_array;

	private bool inst_boolean;

	private double inst_double;

	private int inst_int;

	private long inst_long;

	private IDictionary<string, JsonData> inst_object;

	private string MBHOBGCDLDB;

	private string EMDHMHOKGFP;

	private GGIECEPGFNH LFLGCDNKNJI;

	private IList<KeyValuePair<string, JsonData>> object_list;

	int ICollection.Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
	}

	bool ICollection.IsSynchronized
	{
		get
		{
			return ECPEGHBJOKG().IsSynchronized;
		}
	}

	object ICollection.SyncRoot
	{
		get
		{
			return ECPEGHBJOKG().SyncRoot;
		}
	}

	bool IDictionary.IsFixedSize
	{
		get
		{
			return DBMLGGNFGFM().IsFixedSize;
		}
	}

	bool IDictionary.IsReadOnly
	{
		get
		{
			return DBMLGGNFGFM().IsReadOnly;
		}
	}

	ICollection IDictionary.Keys
	{
		get
		{
			DBMLGGNFGFM();
			IList<string> list = new List<string>();
			foreach (KeyValuePair<string, JsonData> item in object_list)
			{
				list.Add(item.Key);
			}
			return (ICollection)list;
		}
	}

	ICollection IDictionary.Values
	{
		get
		{
			DBMLGGNFGFM();
			IList<JsonData> list = new List<JsonData>();
			foreach (KeyValuePair<string, JsonData> item in object_list)
			{
				list.Add(item.Value);
			}
			return (ICollection)list;
		}
	}

	bool IList.IsFixedSize
	{
		get
		{
			return BFMPNAHENOG().IsFixedSize;
		}
	}

	bool IList.IsReadOnly
	{
		get
		{
			return BFMPNAHENOG().IsReadOnly;
		}
	}

	object IDictionary.this[object KGBGENDIMBC]
	{
		get
		{
			return DBMLGGNFGFM()[KGBGENDIMBC];
		}
		set
		{
			if (!(KGBGENDIMBC is string))
			{
				throw new ArgumentException("The key has to be a string");
			}
			JsonData bAINMLLIKOL = ToJsonData(value);
			set_Item((string)KGBGENDIMBC, bAINMLLIKOL);
		}
	}

	// C# has no syntax for parameterized property 'IOrderedDictionary.Item'.
	object IOrderedDictionary.get_Item(int OOPOEMNCCGH)
	{
		return LitJson_002EIOrderedDictionary_002Eget_Item(OOPOEMNCCGH);
	}

	void IOrderedDictionary.set_Item(int OOPOEMNCCGH, object value)
	{
		LitJson_002EIOrderedDictionary_002Eset_Item(OOPOEMNCCGH, value);
	}

	object IList.this[int index]
	{
		get
		{
			return BFMPNAHENOG()[index];
		}
		set
		{
			BFMPNAHENOG();
			JsonData bAINMLLIKOL = ToJsonData(value);
			set_Item(index, bAINMLLIKOL);
		}
	}

	public int Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
	}

	public bool MENGHLDLPDP
	{
		get
		{
			return NKLOBJNAFOL();
		}
	}

	public bool BKKEJEHCHAK
	{
		get
		{
			return DBAOMEBNMPH();
		}
	}

	public bool LPEHBKJIAJB
	{
		get
		{
			return OEIGDMENBKN();
		}
	}

	public bool MKGEMBAAPBL
	{
		get
		{
			return BGDHACEDILB();
		}
	}

	public bool LNLKOGMCNNF
	{
		get
		{
			return BPKJMLDOLPH();
		}
	}

	public bool PDKNNMDCPDJ
	{
		get
		{
			return HKCKGNMIKBM();
		}
	}

	public bool JDALJCCIBIN
	{
		get
		{
			return FMFILGDCAKM();
		}
	}

	public ICollection<string> BJACLIMKPAE
	{
		get
		{
			return IPPIHHKHGNI();
		}
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public JsonData get_DLKPBAJDHBO(string FGGONFKCLMP)
	{
		return get_Item(FGGONFKCLMP);
	}

	public void set_DLKPBAJDHBO(string FGGONFKCLMP, JsonData value)
	{
		set_Item(FGGONFKCLMP, value);
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public JsonData get_DLKPBAJDHBO(int index)
	{
		return get_Item(index);
	}

	object IOrderedDictionary.get_DLKPBAJDHBO(int index)
	{
		return get_DLKPBAJDHBO(index);
	}

	void IOrderedDictionary.set_DLKPBAJDHBO(int index, object value)
	{
		set_DLKPBAJDHBO(index, ToJsonData(value));
	}

	public void set_DLKPBAJDHBO(int index, JsonData value)
	{
		set_Item(index, value);
	}

	public JsonData()
	{
	}

	public JsonData(bool CIGMFMBICLJ)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Boolean;
		inst_boolean = CIGMFMBICLJ;
	}

	public JsonData(double number)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Double;
		inst_double = number;
	}

	public JsonData(int number)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Int;
		inst_int = number;
	}

	public JsonData(long number)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Long;
		inst_long = number;
	}

	public JsonData(object AOMLCBHAJJH)
	{
		if (AOMLCBHAJJH is bool)
		{
			LFLGCDNKNJI = GGIECEPGFNH.Boolean;
			inst_boolean = (bool)AOMLCBHAJJH;
			return;
		}
		if (AOMLCBHAJJH is double)
		{
			LFLGCDNKNJI = GGIECEPGFNH.Double;
			inst_double = (double)AOMLCBHAJJH;
			return;
		}
		if (AOMLCBHAJJH is int)
		{
			LFLGCDNKNJI = GGIECEPGFNH.Int;
			inst_int = (int)AOMLCBHAJJH;
			return;
		}
		if (AOMLCBHAJJH is long)
		{
			LFLGCDNKNJI = GGIECEPGFNH.Long;
			inst_long = (long)AOMLCBHAJJH;
			return;
		}
		if (AOMLCBHAJJH is string)
		{
			LFLGCDNKNJI = GGIECEPGFNH.String;
			MBHOBGCDLDB = (string)AOMLCBHAJJH;
			return;
		}
		throw new ArgumentException("Unable to wrap the given object with JsonData");
	}

	public JsonData(string IGGFGLLIGCG)
	{
		LFLGCDNKNJI = GGIECEPGFNH.String;
		MBHOBGCDLDB = IGGFGLLIGCG;
	}

	public int OFOPFCJNEBL()
	{
		return ECPEGHBJOKG().Count;
	}

	public bool NKLOBJNAFOL()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Array;
	}

	public bool DBAOMEBNMPH()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Boolean;
	}

	public bool OEIGDMENBKN()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Double;
	}

	public bool BGDHACEDILB()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Int;
	}

	public bool BPKJMLDOLPH()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Long;
	}

	public bool HKCKGNMIKBM()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.Object;
	}

	public bool FMFILGDCAKM()
	{
		return LFLGCDNKNJI == GGIECEPGFNH.String;
	}

	public ICollection<string> IPPIHHKHGNI()
	{
		DBMLGGNFGFM();
		return inst_object.Keys;
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsArray()
	{
		return NKLOBJNAFOL();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsBoolean()
	{
		return DBAOMEBNMPH();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsDouble()
	{
		return OEIGDMENBKN();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsInt()
	{
		return BGDHACEDILB();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsLong()
	{
		return BPKJMLDOLPH();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsObject()
	{
		return HKCKGNMIKBM();
	}

	private bool LitJson_002EIJsonWrapper_002Eget_IsString()
	{
		return FMFILGDCAKM();
	}

	private object LitJson_002EIOrderedDictionary_002Eget_Item(int OOPOEMNCCGH)
	{
		DBMLGGNFGFM();
		return object_list[OOPOEMNCCGH].Value;
	}

	private void LitJson_002EIOrderedDictionary_002Eset_Item(int OOPOEMNCCGH, object value)
	{
		DBMLGGNFGFM();
		JsonData jsonData = ToJsonData(value);
		KeyValuePair<string, JsonData> keyValuePair = object_list[OOPOEMNCCGH];
		inst_object[keyValuePair.Key] = jsonData;
		KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, jsonData);
		object_list[OOPOEMNCCGH] = keyValuePair2;
	}

	public JsonData get_Item(string FGGONFKCLMP)
	{
		DBMLGGNFGFM();
		return inst_object[FGGONFKCLMP];
	}

	public void set_Item(string FGGONFKCLMP, JsonData value)
	{
		DBMLGGNFGFM();
		KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(FGGONFKCLMP, value);
		if (inst_object.ContainsKey(FGGONFKCLMP))
		{
			for (int i = 0; i < object_list.Count; i++)
			{
				if (object_list[i].Key == FGGONFKCLMP)
				{
					object_list[i] = keyValuePair;
					break;
				}
			}
		}
		else
		{
			object_list.Add(keyValuePair);
		}
		inst_object[FGGONFKCLMP] = value;
		EMDHMHOKGFP = null;
	}

	public JsonData get_Item(int index)
	{
		ECPEGHBJOKG();
		if (LFLGCDNKNJI == GGIECEPGFNH.Array)
		{
			return inst_array[index];
		}
		return object_list[index].Value;
	}

	public void set_Item(int index, JsonData value)
	{
		ECPEGHBJOKG();
		if (LFLGCDNKNJI == GGIECEPGFNH.Array)
		{
			inst_array[index] = value;
		}
		else
		{
			KeyValuePair<string, JsonData> keyValuePair = object_list[index];
			KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
			object_list[index] = keyValuePair2;
			inst_object[keyValuePair.Key] = keyValuePair2.Value;
		}
		EMDHMHOKGFP = null;
	}

	[SpecialName]
	public static JsonData op_Implicit(bool data)
	{
		return new JsonData(data);
	}

	[SpecialName]
	public static JsonData op_Implicit(double data)
	{
		return new JsonData(data);
	}

	[SpecialName]
	public static JsonData op_Implicit(int data)
	{
		return new JsonData(data);
	}

	[SpecialName]
	public static JsonData op_Implicit(long data)
	{
		return new JsonData(data);
	}

	[SpecialName]
	public static JsonData op_Implicit(string data)
	{
		return new JsonData(data);
	}

	public static explicit operator bool(JsonData data)
	{
		if (data.LFLGCDNKNJI != GGIECEPGFNH.Boolean)
		{
			throw new InvalidCastException("Instance of JsonData doesn't hold a double");
		}
		return data.inst_boolean;
	}

	public static explicit operator double(JsonData data)
	{
		if (data.LFLGCDNKNJI != GGIECEPGFNH.Double)
		{
			throw new InvalidCastException("Instance of JsonData doesn't hold a double");
		}
		return data.inst_double;
	}

	public static explicit operator int(JsonData data)
	{
		if (data.LFLGCDNKNJI != GGIECEPGFNH.Int)
		{
			throw new InvalidCastException("Instance of JsonData doesn't hold an int");
		}
		return data.inst_int;
	}

	public static explicit operator long(JsonData data)
	{
		if (data.LFLGCDNKNJI != GGIECEPGFNH.Long)
		{
			throw new InvalidCastException("Instance of JsonData doesn't hold an int");
		}
		return data.inst_long;
	}

	public static explicit operator string(JsonData data)
	{
		if (data.LFLGCDNKNJI != GGIECEPGFNH.String)
		{
			throw new InvalidCastException("Instance of JsonData doesn't hold a string");
		}
		return data.MBHOBGCDLDB;
	}

	void ICollection.CopyTo(Array HFPDMGAEJJE, int index)
	{
		ECPEGHBJOKG().CopyTo(HFPDMGAEJJE, index);
	}

	void IDictionary.Add(object KGBGENDIMBC, object value)
	{
		JsonData jsonData = ToJsonData(value);
		DBMLGGNFGFM().Add(KGBGENDIMBC, jsonData);
		KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)KGBGENDIMBC, jsonData);
		object_list.Add(item);
		EMDHMHOKGFP = null;
	}

	void IDictionary.Clear()
	{
		DBMLGGNFGFM().Clear();
		object_list.Clear();
		EMDHMHOKGFP = null;
	}

	bool IDictionary.Contains(object KGBGENDIMBC)
	{
		return DBMLGGNFGFM().Contains(KGBGENDIMBC);
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IOrderedDictionary)this).GetEnumerator();
	}

	void IDictionary.Remove(object KGBGENDIMBC)
	{
		DBMLGGNFGFM().Remove(KGBGENDIMBC);
		for (int i = 0; i < object_list.Count; i++)
		{
			if (object_list[i].Key == (string)KGBGENDIMBC)
			{
				object_list.RemoveAt(i);
				break;
			}
		}
		EMDHMHOKGFP = null;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ECPEGHBJOKG().GetEnumerator();
	}

	bool IJsonWrapper.GetBoolean()
	{
		if (LFLGCDNKNJI != GGIECEPGFNH.Boolean)
		{
			throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
		}
		return inst_boolean;
	}

	double IJsonWrapper.GetDouble()
	{
		if (LFLGCDNKNJI != GGIECEPGFNH.Double)
		{
			throw new InvalidOperationException("JsonData instance doesn't hold a double");
		}
		return inst_double;
	}

	int IJsonWrapper.GetInt()
	{
		if (LFLGCDNKNJI != GGIECEPGFNH.Int)
		{
			throw new InvalidOperationException("JsonData instance doesn't hold an int");
		}
		return inst_int;
	}

	long IJsonWrapper.GetLong()
	{
		if (LFLGCDNKNJI != GGIECEPGFNH.Long)
		{
			throw new InvalidOperationException("JsonData instance doesn't hold a long");
		}
		return inst_long;
	}

	string IJsonWrapper.GetString()
	{
		if (LFLGCDNKNJI != GGIECEPGFNH.String)
		{
			throw new InvalidOperationException("JsonData instance doesn't hold a string");
		}
		return MBHOBGCDLDB;
	}

	void IJsonWrapper.SetBoolean(bool PKHDLOGJKAD)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Boolean;
		inst_boolean = PKHDLOGJKAD;
		EMDHMHOKGFP = null;
	}

	void IJsonWrapper.SetDouble(double PKHDLOGJKAD)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Double;
		inst_double = PKHDLOGJKAD;
		EMDHMHOKGFP = null;
	}

	void IJsonWrapper.SetInt(int PKHDLOGJKAD)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Int;
		inst_int = PKHDLOGJKAD;
		EMDHMHOKGFP = null;
	}

	void IJsonWrapper.SetLong(long PKHDLOGJKAD)
	{
		LFLGCDNKNJI = GGIECEPGFNH.Long;
		inst_long = PKHDLOGJKAD;
		EMDHMHOKGFP = null;
	}

	void IJsonWrapper.SetString(string PKHDLOGJKAD)
	{
		LFLGCDNKNJI = GGIECEPGFNH.String;
		MBHOBGCDLDB = PKHDLOGJKAD;
		EMDHMHOKGFP = null;
	}

	string IJsonWrapper.ToJson()
	{
		return ToJson();
	}

	void IJsonWrapper.ToJson(JsonWriter writer)
	{
		ToJson(writer);
	}

	int IList.Add(object value)
	{
		return Add(value);
	}

	void IList.Clear()
	{
		BFMPNAHENOG().Clear();
		EMDHMHOKGFP = null;
	}

	bool IList.Contains(object value)
	{
		return BFMPNAHENOG().Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return BFMPNAHENOG().IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		BFMPNAHENOG().Insert(index, value);
		EMDHMHOKGFP = null;
	}

	void IList.Remove(object value)
	{
		BFMPNAHENOG().Remove(value);
		EMDHMHOKGFP = null;
	}

	void IList.RemoveAt(int index)
	{
		BFMPNAHENOG().RemoveAt(index);
		EMDHMHOKGFP = null;
	}

	IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
	{
		DBMLGGNFGFM();
		return new OrderedDictionaryEnumerator(object_list.GetEnumerator());
	}

	void IOrderedDictionary.Insert(int OOPOEMNCCGH, object KGBGENDIMBC, object value)
	{
		string text = (string)KGBGENDIMBC;
		JsonData eODAPIHAEAK = ToJsonData(value);
		set_Item(text, eODAPIHAEAK);
		KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, eODAPIHAEAK);
		object_list.Insert(OOPOEMNCCGH, item);
	}

	void IOrderedDictionary.RemoveAt(int OOPOEMNCCGH)
	{
		DBMLGGNFGFM();
		inst_object.Remove(object_list[OOPOEMNCCGH].Key);
		object_list.RemoveAt(OOPOEMNCCGH);
	}

	private ICollection ECPEGHBJOKG()
	{
		if (LFLGCDNKNJI == GGIECEPGFNH.Array)
		{
			return (ICollection)inst_array;
		}
		if (LFLGCDNKNJI == GGIECEPGFNH.Object)
		{
			return (ICollection)inst_object;
		}
		throw new InvalidOperationException("The JsonData instance has to be initialized first");
	}

	private IDictionary DBMLGGNFGFM()
	{
		if (LFLGCDNKNJI == GGIECEPGFNH.Object)
		{
			return (IDictionary)inst_object;
		}
		if (LFLGCDNKNJI != GGIECEPGFNH.None)
		{
			throw new InvalidOperationException("Instance of JsonData is not a dictionary");
		}
		LFLGCDNKNJI = GGIECEPGFNH.Object;
		inst_object = new Dictionary<string, JsonData>();
		object_list = new List<KeyValuePair<string, JsonData>>();
		return (IDictionary)inst_object;
	}

	private IList BFMPNAHENOG()
	{
		if (LFLGCDNKNJI == GGIECEPGFNH.Array)
		{
			return (IList)inst_array;
		}
		if (LFLGCDNKNJI != GGIECEPGFNH.None)
		{
			throw new InvalidOperationException("Instance of JsonData is not a list");
		}
		LFLGCDNKNJI = GGIECEPGFNH.Array;
		inst_array = new List<JsonData>();
		return (IList)inst_array;
	}

	private JsonData ToJsonData(object AOMLCBHAJJH)
	{
		if (AOMLCBHAJJH == null)
		{
			return null;
		}
		if (AOMLCBHAJJH is JsonData)
		{
			return (JsonData)AOMLCBHAJJH;
		}
		return new JsonData(AOMLCBHAJJH);
	}

	private static void CHOOAGEMOMD(IJsonWrapper AOMLCBHAJJH, JsonWriter writer)
	{
		if (AOMLCBHAJJH == null)
		{
			writer.Write(null);
		}
		else if (AOMLCBHAJJH.FMFILGDCAKM())
		{
			writer.Write(AOMLCBHAJJH.GetString());
		}
		else if (AOMLCBHAJJH.DBAOMEBNMPH())
		{
			writer.Write(AOMLCBHAJJH.GetBoolean());
		}
		else if (AOMLCBHAJJH.OEIGDMENBKN())
		{
			writer.Write(AOMLCBHAJJH.GetDouble());
		}
		else if (AOMLCBHAJJH.BGDHACEDILB())
		{
			writer.Write(AOMLCBHAJJH.GetInt());
		}
		else if (AOMLCBHAJJH.BPKJMLDOLPH())
		{
			writer.Write(AOMLCBHAJJH.GetLong());
		}
		else if (AOMLCBHAJJH.NKLOBJNAFOL())
		{
			writer.AGGBIHCJOKF();
			foreach (object item in (IEnumerable)AOMLCBHAJJH)
			{
				CHOOAGEMOMD((JsonData)item, writer);
			}
			writer.FMIALOIGMFH();
		}
		else
		{
			if (!AOMLCBHAJJH.HKCKGNMIKBM())
			{
				return;
			}
			writer.ACCDHGHBCHM();
			foreach (DictionaryEntry item2 in (IDictionary)AOMLCBHAJJH)
			{
				writer.MPKEMEAPPJL((string)item2.Key);
				CHOOAGEMOMD((JsonData)item2.Value, writer);
			}
			writer.KDAIDMBDFHB();
		}
	}

	public int Add(object value)
	{
		JsonData jsonData = ToJsonData(value);
		EMDHMHOKGFP = null;
		return BFMPNAHENOG().Add(jsonData);
	}

	public void Clear()
	{
		if (HKCKGNMIKBM())
		{
			((IDictionary)this).Clear();
		}
		else if (NKLOBJNAFOL())
		{
			((IList)this).Clear();
		}
	}

	public bool Equals(JsonData DHDMNHCIPEH)
	{
		if (DHDMNHCIPEH == null)
		{
			return false;
		}
		if (DHDMNHCIPEH.LFLGCDNKNJI != LFLGCDNKNJI)
		{
			return false;
		}
		switch (LFLGCDNKNJI)
		{
		case GGIECEPGFNH.None:
			return true;
		case GGIECEPGFNH.Object:
			return inst_object.Equals(DHDMNHCIPEH.inst_object);
		case GGIECEPGFNH.Array:
			return inst_array.Equals(DHDMNHCIPEH.inst_array);
		case GGIECEPGFNH.String:
			return MBHOBGCDLDB.Equals(DHDMNHCIPEH.MBHOBGCDLDB);
		case GGIECEPGFNH.Int:
			return inst_int.Equals(DHDMNHCIPEH.inst_int);
		case GGIECEPGFNH.Long:
			return inst_long.Equals(DHDMNHCIPEH.inst_long);
		case GGIECEPGFNH.Double:
			return inst_double.Equals(DHDMNHCIPEH.inst_double);
		case GGIECEPGFNH.Boolean:
			return inst_boolean.Equals(DHDMNHCIPEH.inst_boolean);
		default:
			return false;
		}
	}

	public GGIECEPGFNH NCGOKKHFKJF()
	{
		return LFLGCDNKNJI;
	}

	public void FJKDNANFIHA(GGIECEPGFNH LFLGCDNKNJI)
	{
		if (this.LFLGCDNKNJI != LFLGCDNKNJI)
		{
			switch (LFLGCDNKNJI)
			{
			case GGIECEPGFNH.Object:
				inst_object = new Dictionary<string, JsonData>();
				object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case GGIECEPGFNH.Array:
				inst_array = new List<JsonData>();
				break;
			case GGIECEPGFNH.String:
				MBHOBGCDLDB = null;
				break;
			case GGIECEPGFNH.Int:
				inst_int = 0;
				break;
			case GGIECEPGFNH.Long:
				inst_long = 0L;
				break;
			case GGIECEPGFNH.Double:
				inst_double = 0.0;
				break;
			case GGIECEPGFNH.Boolean:
				inst_boolean = false;
				break;
			}
			this.LFLGCDNKNJI = LFLGCDNKNJI;
		}
	}

	public string ToJson()
	{
		if (EMDHMHOKGFP != null)
		{
			return EMDHMHOKGFP;
		}
		StringWriter stringWriter = new StringWriter();
		JsonWriter iGOCJFDLBMG = new JsonWriter(stringWriter);
		iGOCJFDLBMG.BHMCFLJJJNM(false);
		CHOOAGEMOMD(this, iGOCJFDLBMG);
		EMDHMHOKGFP = stringWriter.ToString();
		return EMDHMHOKGFP;
	}

	public void ToJson(JsonWriter writer)
	{
		bool bAINMLLIKOL = writer.EPCAKOLMCMC();
		writer.BHMCFLJJJNM(false);
		CHOOAGEMOMD(this, writer);
		writer.BHMCFLJJJNM(bAINMLLIKOL);
	}

	public override string ToString()
	{
		switch (LFLGCDNKNJI)
		{
		case GGIECEPGFNH.Array:
			return "JsonData array";
		case GGIECEPGFNH.Boolean:
			return inst_boolean.ToString();
		case GGIECEPGFNH.Double:
			return inst_double.ToString();
		case GGIECEPGFNH.Int:
			return inst_int.ToString();
		case GGIECEPGFNH.Long:
			return inst_long.ToString();
		case GGIECEPGFNH.Object:
			return "JsonData object";
		case GGIECEPGFNH.String:
			return MBHOBGCDLDB;
		default:
			return "Uninitialized JsonData";
		}
	}
}
