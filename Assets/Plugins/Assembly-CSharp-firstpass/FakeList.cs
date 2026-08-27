using System;
using System.Collections.Generic;
using System.Reflection;

[DefaultMember("Item")]
public class FakeList<T>
{
	private readonly IEnumerator<T> collection;

	private int currentIndex = -1;

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public T get_DLKPBAJDHBO(int index)
	{
		return get_Item(index);
	}

	public FakeList(IEnumerator<T> collection)
	{
		this.collection = collection;
	}

	public FakeList(IEnumerable<T> collection)
		: this(collection.GetEnumerator())
	{
	}

	public T get_Item(int index)
	{
		if (index < currentIndex)
		{
			collection.Reset();
			currentIndex = -1;
		}
		while (currentIndex < index)
		{
			if (!collection.MoveNext())
			{
				throw new ArgumentOutOfRangeException("index");
			}
			currentIndex++;
		}
		return collection.Current;
	}
}
