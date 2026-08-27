using System;
using System.Xml;

public class RewardCurrency : Rewardable
{
	public string Name = string.Empty;

	private float MDIAPHDGFBA;

	private bool _expectedValueEmpty;

	private float OOICKNKHPOD;

	private float FMKKMJGINOO;

	private float HNIKLFKNLDI;

	public RewardCurrency(XmlNode node)
	{
		Parse(node);
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_CURRENCY;
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_expectedValueEmpty = node.Attributes["ExpectedValue"].Empty();
		MDIAPHDGFBA = node.Attributes["ExpectedValue"].ParseFloat();
		OOICKNKHPOD = node.Attributes["DeviationCoefficient"].ParseFloat();
		FMKKMJGINOO = node.Attributes["DecayCoefficient"].ParseFloat();
		HNIKLFKNLDI = node.Attributes["ValueMultiplier"].ParseFloat();
	}

	public int NAHFILGJAPC()
	{
		EHEFCBECODJ();
		double num = 0.0;
		double num2 = Math.Floor(MDIAPHDGFBA);
		double num3 = Math.Floor((double)(MDIAPHDGFBA * OOICKNKHPOD) + 0.5);
		if (FMKKMJGINOO <= 0f)
		{
			num = num2;
		}
		else
		{
			double num4 = Math.Min(num3, num2);
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			if (FMKKMJGINOO >= 1f)
			{
				num5 = num2 - num4 / 2.0;
				num6 = num2 + 1.0 + num3 / 2.0;
			}
			else
			{
				num7 = Math.Pow(FMKKMJGINOO, -1.0 / Math.Max(num3, 1.0));
				double num8 = 1.0 - Math.Pow(num7, 0.0 - num4) * (num4 * Math.Log(num7) + 1.0);
				double num9 = Math.Pow(Math.Log(num7), 2.0);
				num5 = num2 - (1.0 - Math.Pow(num7, 0.0 - num4) * (num4 * Math.Log(num7) + 1.0)) / Math.Pow(Math.Log(num7), 2.0);
				num8 = 1.0 - Math.Pow(num7, 0.0 - num3) * (num4 * Math.Log(num7) + 1.0);
				num9 = Math.Pow(Math.Log(num7), 2.0);
				num6 = num2 + 1.0 + (1.0 - Math.Pow(num7, 0.0 - num3) * (num4 * Math.Log(num7) + 1.0)) / Math.Pow(Math.Log(num7), 2.0);
			}
			double num10 = ((double)MDIAPHDGFBA - num5) / (num6 - (double)MDIAPHDGFBA);
			float num11 = NekkiMath.randomFloat(0f, 1f);
			float num12 = NekkiMath.randomFloat(0f, 1f);
			if ((double)num12 <= 1.0 / (1.0 + num10))
			{
				if (FMKKMJGINOO >= 1f)
				{
					num = num2 - Math.Floor((double)num11 * num4 + 0.5);
				}
				else
				{
					double num13 = (0.0 - Math.Log(1.0 - (double)num11 * (1.0 - Math.Pow(num7, 0.0 - num4)))) / Math.Log(num7);
					double num14 = num13 - Math.Floor(num13);
					double num15 = 0.0;
					num15 = ((!(num14 < (0.0 - Math.Log((2.0 * Math.Log(num7) - num7 + 1.0) / (num7 * Math.Log(num7)))) / Math.Log(num7))) ? (Math.Floor(num13) + 1.0) : Math.Floor(num13));
					num = num2 - num15;
				}
			}
			else if (FMKKMJGINOO >= 1f)
			{
				num = num2 + 1.0 + Math.Floor((double)num11 * num3 + 0.5);
			}
			else
			{
				double num16 = (0.0 - Math.Log(1.0 - (double)num11 * (1.0 - Math.Pow(num7, 0.0 - num3)))) / Math.Log(num7);
				double num17 = num16 - Math.Floor(num16);
				double num18 = 0.0;
				num18 = ((!(num17 < (0.0 - Math.Log((2.0 * Math.Log(num7) - num7 + 1.0) / (num7 * Math.Log(num7)))) / Math.Log(num7))) ? (Math.Floor(num16) + 1.0) : Math.Floor(num16));
				num = num2 + 1.0 + num18;
			}
		}
		num = BBDICEKCFAG(num);
		return (int)num;
	}

	public long MFPJMGJLKMH()
	{
		EHEFCBECODJ();
		double num = Math.Floor((double)(MDIAPHDGFBA * OOICKNKHPOD) + 0.5);
		double num2 = Math.Floor((double)MDIAPHDGFBA - num);
		return (!(num2 >= 0.0)) ? 0 : ((long)num2);
	}

	private void EHEFCBECODJ()
	{
		if (_expectedValueEmpty)
		{
			float num = GameUtils.KIGEPCLPEIE.GetBaseValue(Name);
			MDIAPHDGFBA = HNIKLFKNLDI * num;
			_expectedValueEmpty = false;
		}
	}

	private double BBDICEKCFAG(double value)
	{
		if (value == 0.0)
		{
			value = 1.0;
		}
		double num = Math.Log10((float)value) + 1.0;
		if (num == 1.0)
		{
			return value;
		}
		double num2 = 0.0;
		double num3 = 1.0;
		if (num > 2.0)
		{
			num3 = Math.Pow(10.0, num - 2.0);
			num2 = value / num3;
		}
		else
		{
			num2 = value;
		}
		double num4 = num2 % 5.0;
		if (num4 != 0.0)
		{
			num2 = ((!(num4 > 2.0)) ? (num2 - num4) : (num2 + 5.0 - num4));
		}
		return num2 * num3;
	}
}
