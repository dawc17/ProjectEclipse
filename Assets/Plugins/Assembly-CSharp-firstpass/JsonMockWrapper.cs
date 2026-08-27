using System;
using System.Collections;

public class JsonMockWrapper : IDictionary, IList, IEnumerable, ICollection, IOrderedDictionary, IJsonWrapper
{
	bool IList.IsFixedSize
	{
		get
		{
			return true;
		}
	}

	bool IList.IsReadOnly
	{
		get
		{
			return true;
		}
	}

	object IList.this[int index]
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	int ICollection.Count
	{
		get
		{
			return 0;
		}
	}

	bool ICollection.IsSynchronized
	{
		get
		{
			return false;
		}
	}

	object ICollection.SyncRoot
	{
		get
		{
			return null;
		}
	}

	bool IDictionary.IsFixedSize
	{
		get
		{
			return true;
		}
	}

	bool IDictionary.IsReadOnly
	{
		get
		{
			return true;
		}
	}

	ICollection IDictionary.Keys
	{
		get
		{
			return null;
		}
	}

	ICollection IDictionary.Values
	{
		get
		{
			return null;
		}
	}

	object IDictionary.this[object KGBGENDIMBC]
	{
		get
		{
			return null;
		}
		set
		{
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

	object IOrderedDictionary.get_DLKPBAJDHBO(int index)
	{
		throw new NotSupportedException();
	}

	void IOrderedDictionary.set_DLKPBAJDHBO(int index, object value)
	{
		throw new NotSupportedException();
	}

	public bool NKLOBJNAFOL()
	{
		return false;
	}

	public bool DBAOMEBNMPH()
	{
		return false;
	}

	public bool OEIGDMENBKN()
	{
		return false;
	}

	public bool BGDHACEDILB()
	{
		return false;
	}

	public bool BPKJMLDOLPH()
	{
		return false;
	}

	public bool HKCKGNMIKBM()
	{
		return false;
	}

	public bool FMFILGDCAKM()
	{
		return false;
	}

	public bool GetBoolean()
	{
		return false;
	}

	public double GetDouble()
	{
		return 0.0;
	}

	public int GetInt()
	{
		return 0;
	}

	public GGIECEPGFNH NCGOKKHFKJF()
	{
		return GGIECEPGFNH.None;
	}

	public long GetLong()
	{
		return 0L;
	}

	public string GetString()
	{
		return string.Empty;
	}

	public void SetBoolean(bool PKHDLOGJKAD)
	{
	}

	public void SetDouble(double PKHDLOGJKAD)
	{
	}

	public void SetInt(int PKHDLOGJKAD)
	{
	}

	public void FJKDNANFIHA(GGIECEPGFNH LFLGCDNKNJI)
	{
	}

	public void SetLong(long PKHDLOGJKAD)
	{
	}

	public void SetString(string PKHDLOGJKAD)
	{
	}

	public string ToJson()
	{
		return string.Empty;
	}

	public void ToJson(JsonWriter writer)
	{
	}

	int IList.Add(object value)
	{
		return 0;
	}

	void IList.Clear()
	{
	}

	bool IList.Contains(object value)
	{
		return false;
	}

	int IList.IndexOf(object value)
	{
		return -1;
	}

	void IList.Insert(int i, object AFIEJABPAKA)
	{
	}

	void IList.Remove(object value)
	{
	}

	void IList.RemoveAt(int index)
	{
	}

	void ICollection.CopyTo(Array HFPDMGAEJJE, int index)
	{
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return null;
	}

	void IDictionary.Add(object KJBMNAEJIHG, object AFIEJABPAKA)
	{
	}

	void IDictionary.Clear()
	{
	}

	bool IDictionary.Contains(object KGBGENDIMBC)
	{
		return false;
	}

	void IDictionary.Remove(object KGBGENDIMBC)
	{
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return null;
	}

	private object LitJson_002EIOrderedDictionary_002Eget_Item(int OOPOEMNCCGH)
	{
		return null;
	}

	private void LitJson_002EIOrderedDictionary_002Eset_Item(int OOPOEMNCCGH, object value)
	{
	}

	IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
	{
		return null;
	}

	void IOrderedDictionary.Insert(int i, object KJBMNAEJIHG, object AFIEJABPAKA)
	{
	}

	void IOrderedDictionary.RemoveAt(int i)
	{
	}
}
