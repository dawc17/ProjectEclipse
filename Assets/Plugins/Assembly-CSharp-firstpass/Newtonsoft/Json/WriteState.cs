namespace Newtonsoft.Json
{
	public enum WriteState
	{
		Error = 0,
		Closed = 1,
		Object = 2,
		Array = 3,
		Constructor = 4,
		Property = 5,
		Start = 6
	}
}
