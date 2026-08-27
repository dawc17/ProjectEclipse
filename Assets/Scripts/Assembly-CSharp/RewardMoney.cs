using System;
using System.Xml;

public class RewardMoney : Rewardable
{
	private bool _valueEmpty;

	private long _value;

	private float HNIKLFKNLDI;

	public RewardMoney(XmlNode node)
	{
		Parse(node);
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_MONEY;
		_valueEmpty = node.Attributes["Value"].Empty();
		_value = node.Attributes["Value"].ParseLong(0L);
		HNIKLFKNLDI = node.Attributes["ValueMultiplier"].ParseFloat();
	}

	public long BANPBCOOFMB()
	{
		EGIAFPBJDIF();
		_value = BBDICEKCFAG(_value);
		return _value;
	}

	private void EGIAFPBJDIF()
	{
		if (_valueEmpty)
		{
			int oMHDLKNHNMJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			long num = GameUtils.NFJEPNHJPEE.GetBaseValue(oMHDLKNHNMJ);
			_value = (long)HNIKLFKNLDI * num;
			_valueEmpty = false;
		}
	}

	private long BBDICEKCFAG(long value)
	{
		if (value == 0)
		{
			value = 1L;
		}
		double num = Math.Log10((float)value) + 1.0;
		if (num == 1.0)
		{
			return value;
		}
		long num2 = 0L;
		long num3 = 1L;
		if (num > 2.0)
		{
			num3 = (long)Math.Pow(10.0, num - 2.0);
			num2 = value / num3;
		}
		else
		{
			num2 = value;
		}
		long num4 = num2 % 5;
		if (num4 != 0)
		{
			num2 = ((num4 <= 2) ? (num2 - num4) : (num2 + 5 - num4));
		}
		return num2 * num3;
	}
}
