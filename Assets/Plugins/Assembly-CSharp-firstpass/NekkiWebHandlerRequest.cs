using System.Collections.Generic;

public class NekkiWebHandlerRequest : NekkiWebHandler
{
	private readonly List<byte> _buffer;

	public NekkiWebHandlerRequest(NekkiUri IACLKBNEBDM)
		: base(IACLKBNEBDM)
	{
		_buffer = new List<byte>();
	}

	protected override void LKECEJOMPGF(byte[] data, int IAFIGGBIKOD, int HIGBAHGOFIJ)
	{
		_buffer.Capacity += HIGBAHGOFIJ;
		for (int i = 0; i < HIGBAHGOFIJ; i++)
		{
			_buffer.Add(data[i]);
		}
	}

	protected override byte[] GetData()
	{
		return _buffer.ToArray();
	}
}
