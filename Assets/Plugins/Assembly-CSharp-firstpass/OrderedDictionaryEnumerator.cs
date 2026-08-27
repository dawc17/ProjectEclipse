using System.Collections;
using System.Collections.Generic;

internal class OrderedDictionaryEnumerator : IEnumerator, IDictionaryEnumerator
{
	private IEnumerator<KeyValuePair<string, JsonData>> CNFOENGMCMB;


	public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> GEJJPNMHBJO)
	{
		CNFOENGMCMB = GEJJPNMHBJO;
	}

	public object Current
	{
		get
		{
			return Entry;
		}
	}

	public DictionaryEntry Entry
	{
		get
		{
			KeyValuePair<string, JsonData> current = CNFOENGMCMB.Current;
			return new DictionaryEntry(current.Key, current.Value);
		}
	}

	object IDictionaryEnumerator.Key
	{
		get
		{
			return CNFOENGMCMB.Current.Key;
		}
	}

	object IDictionaryEnumerator.Value
	{
		get
		{
			return CNFOENGMCMB.Current.Value;
		}
	}

	public bool MoveNext()
	{
		return CNFOENGMCMB.MoveNext();
	}

	public void Reset()
	{
		CNFOENGMCMB.Reset();
	}
}
