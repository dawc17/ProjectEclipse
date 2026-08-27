using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SerializerState : IDisposable
{
	private readonly IDictionary<Type, object> HELFDCAIJNE = new Dictionary<Type, object>();

	public T Get<T>() where T : class, new()
	{
		object value;
		if (!HELFDCAIJNE.TryGetValue(typeof(T), out value))
		{
			value = new T();
			HELFDCAIJNE.Add(typeof(T), value);
		}
		return (T)value;
	}

	public void INOFEFDGNFL()
	{
		foreach (KOOPFFDDANF item in HELFDCAIJNE.Values.OfType<KOOPFFDDANF>())
		{
			item.INOFEFDGNFL();
		}
	}

	public void Dispose()
	{
		foreach (IDisposable item in HELFDCAIJNE.Values.OfType<IDisposable>())
		{
			item.Dispose();
		}
	}
}
