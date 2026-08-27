using System;
using System.Collections.Generic;

public class EventDispatcher<T> : global::IEventDispatcher<T>
{
	private Dictionary<int, Action<T>> APNNBCCKAJA;

	public EventDispatcher()
	{
		APNNBCCKAJA = new Dictionary<int, Action<T>>();
	}

	public int AddEventListener(int name, Action<T> ODDEOFKLIAG)
	{
		if (ODDEOFKLIAG == null)
		{
			return -1;
		}
		if (APNNBCCKAJA.ContainsKey(name))
		{
			Dictionary<int, Action<T>> aPNNBCCKAJA;
			int key;
			(aPNNBCCKAJA = APNNBCCKAJA)[key = name] = (Action<T>)Delegate.Combine(aPNNBCCKAJA[key], ODDEOFKLIAG);
			return 0;
		}
		APNNBCCKAJA.Add(name, ODDEOFKLIAG);
		return 1;
	}

	public int RemoveEventListener(int name, Action<T> ODDEOFKLIAG)
	{
		if (ODDEOFKLIAG == null)
		{
			return -1;
		}
		if (APNNBCCKAJA.ContainsKey(name))
		{
			Dictionary<int, Action<T>> aPNNBCCKAJA;
			int key;
			(aPNNBCCKAJA = APNNBCCKAJA)[key = name] = (Action<T>)Delegate.Remove(aPNNBCCKAJA[key], ODDEOFKLIAG);
			if (APNNBCCKAJA[name] == null)
			{
				RemoveEvent(name);
			}
			return 0;
		}
		return 1;
	}

	public int RemoveAllEventListener()
	{
		APNNBCCKAJA.Clear();
		return 0;
	}

	public int RemoveEvent(int name)
	{
		APNNBCCKAJA.Remove(name);
		return 1;
	}

	public int CallEvent(int name, T EHCLMBADLKH)
	{
		if (APNNBCCKAJA.ContainsKey(name))
		{
			APNNBCCKAJA[name](EHCLMBADLKH);
			return 0;
		}
		return 1;
	}
}
