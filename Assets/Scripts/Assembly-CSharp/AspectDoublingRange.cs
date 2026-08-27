using System.Xml;

public class AspectDoublingRange
{
	private float _value;

	private int _levelStep;

	public float Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public int FEJMNPFIHFI
	{
		get
		{
			return MAIPAOKJMED();
		}
	}

	public float OEAKCOHMIHH()
	{
		return _value;
	}

	public int MAIPAOKJMED()
	{
		return _levelStep;
	}

	public void Parse(XmlNode node)
	{
		_value = node.Attributes["Value"].ParseFloat();
		_levelStep = node.Attributes["LevelStep"].ParseInt();
	}
}
