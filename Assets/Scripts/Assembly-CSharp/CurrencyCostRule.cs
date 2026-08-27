using System.Xml;

public class CurrencyCostRule : Rule
{
	private string _currencyName = string.Empty;

	private int _currencyValue;

	public string FAEGJAEEMGH
	{
		get
		{
			return JFDCHNBPPNH();
		}
	}

	public int CPODJDDPJHB
	{
		get
		{
			return LHNHLANLHMN();
		}
	}

	public CurrencyCostRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleCurrencyCost, node)
	{
		Parse(node);
	}

	public CurrencyCostRule(CurrencyCostRule HNBFMAKFJAM)
		: base(HNBFMAKFJAM)
	{
		_currencyName = HNBFMAKFJAM._currencyName;
		_currencyValue = 999888777;
	}

	public string JFDCHNBPPNH()
	{
		return _currencyName;
	}

	public int LHNHLANLHMN()
	{
		return 999888777;
	}

	protected override void Parse(XmlNode node)
	{
		_currencyName = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_currencyValue = node.Attributes["Value"].ParseInt();
		if (_currencyValue < 0)
		{
			_currencyValue = 0;
		}
		_currencyValue = 999888777;
	}
}
