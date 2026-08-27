using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
	[Serializable]
	public class InsertionQueue<T>
	{
		private readonly IList<T> items = new List<T>();

		public int Count
		{
			get
			{
				return items.Count;
			}
		}

		public void JFGNCJCOCJA(T item)
		{
			items.Add(item);
		}

		public T HBPLGGGBDAB()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("The queue is empty");
			}
			T result = items[0];
			items.RemoveAt(0);
			return result;
		}

		public void Insert(int index, T item)
		{
			items.Insert(index, item);
		}
	}
}
