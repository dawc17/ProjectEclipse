using System;

public interface IEventDispatcher<T>
{
	int AddEventListener(int name, Action<T> ODDEOFKLIAG);

	int RemoveEventListener(int name, Action<T> ODDEOFKLIAG);

	int RemoveAllEventListener();

	int RemoveEvent(int name);

	int CallEvent(int name, T EHCLMBADLKH);
}
