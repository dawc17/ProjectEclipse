using System.Text;

public static class NekkiExtentions
{
	public static void Clear(this StringBuilder value)
	{
		value.Length = 0;
		value.Capacity = 0;
	}
}
