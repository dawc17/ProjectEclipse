using System;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	public class SFMonoBehaviour<T> : MonoBehaviour, global::IEventDispatcher<T>
	{
		private global::EventDispatcher<T> NBKJBIIPPNB = new global::EventDispatcher<T>();

		public int AddEventListener(int name, Action<T> ODDEOFKLIAG)
		{
			return NBKJBIIPPNB.AddEventListener(name, ODDEOFKLIAG);
		}

		public int CallEvent(int name, T EHCLMBADLKH)
		{
			return NBKJBIIPPNB.CallEvent(name, EHCLMBADLKH);
		}

		public int RemoveAllEventListener()
		{
			return NBKJBIIPPNB.RemoveAllEventListener();
		}

		public int RemoveEvent(int name)
		{
			return NBKJBIIPPNB.RemoveEvent(name);
		}

		public int RemoveEventListener(int name, Action<T> ODDEOFKLIAG)
		{
			return NBKJBIIPPNB.RemoveEventListener(name, ODDEOFKLIAG);
		}
	}
}
