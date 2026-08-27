using UnityEngine.SocialPlatforms;

public class CellSizes
{
	private float[] AAJFMHAGDGM;

	private float[] NHCFADBGNON;

	public int CumulativeIndex = -1;

	private float _spacing;

	public int KKPMCDPFCMP
	{
		get
		{
			return HGHCEDEOMHA();
		}
	}

	public int BHCJCNKLNFC
	{
		get
		{
			return BPDFPOEOLIN();
		}
	}

	public float EPDFGFIACAF
	{
		get
		{
			return FEHBEIFACMG();
		}
		set
		{
			set_Spacing(value);
		}
	}

	public int HGHCEDEOMHA()
	{
		return AAJFMHAGDGM.Length;
	}

	public int BPDFPOEOLIN()
	{
		return NHCFADBGNON.Length;
	}

	public float FEHBEIFACMG()
	{
		return _spacing;
	}

	public void set_Spacing(float value)
	{
		_spacing = value;
	}

	public void SetRowsCount(int count)
	{
		CumulativeIndex = -1;
		AAJFMHAGDGM = new float[count];
		NHCFADBGNON = new float[count];
	}

	public void KJPFDBAIKAH(float PEEOEOMEBFG, int IBAKGENOEPH)
	{
		if (!(PEEOEOMEBFG <= 0f) && IBAKGENOEPH < HGHCEDEOMHA())
		{
			AAJFMHAGDGM[IBAKGENOEPH] = PEEOEOMEBFG;
		}
	}

	public float IEMKAEEOMIH(int IBAKGENOEPH)
	{
		if (IBAKGENOEPH < 0)
		{
			return 0f;
		}
		return AAJFMHAGDGM[IBAKGENOEPH];
	}

	public float SumWithRange(Range JMPCNIOBPAI)
	{
		if (JMPCNIOBPAI.count == 0)
		{
			return 0f;
		}
		return PCKBCFLHKLO(JMPCNIOBPAI.from + JMPCNIOBPAI.count - 1) - PCKBCFLHKLO(JMPCNIOBPAI.from - 1);
	}

	public float PCKBCFLHKLO(int IBAKGENOEPH)
	{
		if (IBAKGENOEPH < 0)
		{
			return 0f;
		}
		while (CumulativeIndex < IBAKGENOEPH)
		{
			CumulativeIndex++;
			NHCFADBGNON[CumulativeIndex] = AAJFMHAGDGM[CumulativeIndex];
			if (CumulativeIndex > 0)
			{
				NHCFADBGNON[CumulativeIndex] += _spacing;
				NHCFADBGNON[CumulativeIndex] += NHCFADBGNON[CumulativeIndex - 1];
			}
		}
		return NHCFADBGNON[IBAKGENOEPH];
	}
}
