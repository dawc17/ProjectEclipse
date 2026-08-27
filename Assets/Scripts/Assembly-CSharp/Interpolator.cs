using System.Collections.Generic;

public class Interpolator
{
	private List<IntervalSet> NFLDEGMEJAK = new List<IntervalSet>();

	private float innerTime;

	private int nowInterval;

	private void BECBFCHBIDJ()
	{
		for (int i = 0; i < NFLDEGMEJAK.Count; i++)
		{
			float bAINMLLIKOL = NFLDEGMEJAK[i].value;
			float num = ((i != NFLDEGMEJAK.Count - 1) ? NFLDEGMEJAK[i + 1].value : NFLDEGMEJAK[0].value);
			if (NFLDEGMEJAK[i].GKIHFPFHKCI == 0f)
			{
				NFLDEGMEJAK[i].AAOIAEJJINO = (NFLDEGMEJAK[i].ILHDJDNPFKH = 0f);
				break;
			}
			if (NFLDEGMEJAK[i].JENJFNNFGLD == 0f)
			{
				NFLDEGMEJAK[i].AAOIAEJJINO = (num - bAINMLLIKOL) / NFLDEGMEJAK[i].GKIHFPFHKCI;
				NFLDEGMEJAK[i].ILHDJDNPFKH = bAINMLLIKOL;
			}
			else
			{
				NFLDEGMEJAK[i].AAOIAEJJINO = (num - bAINMLLIKOL - NFLDEGMEJAK[i].JENJFNNFGLD * NFLDEGMEJAK[i].GKIHFPFHKCI * NFLDEGMEJAK[i].GKIHFPFHKCI) / (2f * NFLDEGMEJAK[i].JENJFNNFGLD * NFLDEGMEJAK[i].GKIHFPFHKCI);
				NFLDEGMEJAK[i].ILHDJDNPFKH = bAINMLLIKOL - NFLDEGMEJAK[i].JENJFNNFGLD * NFLDEGMEJAK[i].AAOIAEJJINO * NFLDEGMEJAK[i].AAOIAEJJINO;
			}
		}
	}

	public bool EIOGKOBGBFK(float GKIHFPFHKCI, float value, float JENJFNNFGLD)
	{
		if (GKIHFPFHKCI < 0f)
		{
			return false;
		}
		IntervalSet aAPMFNMJAFG = new IntervalSet();
		aAPMFNMJAFG.GKIHFPFHKCI = GKIHFPFHKCI;
		aAPMFNMJAFG.value = value;
		aAPMFNMJAFG.JENJFNNFGLD = JENJFNNFGLD;
		NFLDEGMEJAK.Add(aAPMFNMJAFG);
		BECBFCHBIDJ();
		return true;
	}

	public bool HJGPLENNFCK(float HDJFIPHOLMP)
	{
		if (HDJFIPHOLMP < 0f)
		{
			return false;
		}
		if (!HNJDHGDLLPD())
		{
			innerTime += HDJFIPHOLMP;
			return true;
		}
		innerTime += HDJFIPHOLMP;
		while (innerTime > NFLDEGMEJAK[nowInterval].GKIHFPFHKCI)
		{
			if (innerTime > NFLDEGMEJAK[nowInterval].GKIHFPFHKCI)
			{
				innerTime -= NFLDEGMEJAK[nowInterval].GKIHFPFHKCI;
				nowInterval++;
			}
			if (nowInterval >= NFLDEGMEJAK.Count && NFLDEGMEJAK.Count > 0)
			{
				nowInterval = 0;
			}
		}
		return true;
	}

	public float OAGPELOHACM()
	{
		if (NFLDEGMEJAK.Count == 0)
		{
			return 0f;
		}
		float num = ((NFLDEGMEJAK[nowInterval].JENJFNNFGLD == 0f) ? (NFLDEGMEJAK[nowInterval].AAOIAEJJINO * innerTime) : (NFLDEGMEJAK[nowInterval].JENJFNNFGLD * (innerTime + NFLDEGMEJAK[nowInterval].AAOIAEJJINO) * (innerTime + NFLDEGMEJAK[nowInterval].AAOIAEJJINO)));
		return num + NFLDEGMEJAK[nowInterval].ILHDJDNPFKH;
	}

	public bool HNJDHGDLLPD()
	{
		if (NFLDEGMEJAK.Count > 0)
		{
			return true;
		}
		return false;
	}
}
