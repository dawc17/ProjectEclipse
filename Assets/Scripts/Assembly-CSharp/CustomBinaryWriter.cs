using System.IO;
using System.Text;

public class CustomBinaryWriter : BinaryWriter
{
	private Encoding KIAEBFEDGHA = Encoding.UTF8;

	public CustomBinaryWriter()
		: base(new MemoryStream())
	{
	}

	public override void Write(string value)
	{
		value = value.Replace("\n", string.Empty);
		Write(KIAEBFEDGHA.GetBytes(value + "\n"));
	}

	public byte[] IBOIAEAAEGD()
	{
		byte[] array = new byte[16384];
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BaseStream.Position = 0L;
			int count;
			while ((count = BaseStream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, count);
			}
			memoryStream.Flush();
			return memoryStream.ToArray();
		}
	}
}
