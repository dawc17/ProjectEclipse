using System.Xml;

public struct CharProgLevel
{
	public uint LHNCHOAEGEA;

	public uint KAEPJHHLLPK;

	public long value;

	public CharProgLevel(XmlNode OPGGCJGNIPB)
	{
		LHNCHOAEGEA = OPGGCJGNIPB.Attributes["Min"].ParseUint();
		KAEPJHHLLPK = OPGGCJGNIPB.Attributes["Max"].ParseUint(uint.MaxValue);
		value = OPGGCJGNIPB.Attributes["Value"].ParseLong(0L);
	}
}
