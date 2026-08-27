using System.Collections.Generic;

public class Intervals
{
	public List<IntervalNew> MFFPCMPGEBK = new List<IntervalNew>();

	public int AddAnimationFromDistance(float OIOMNNFMDOO, List<InfoAnimation> OEMALIFPGPO)
	{
		int num = 0;
		for (int i = 0; i < MFFPCMPGEBK.Count; i++)
		{
			int num2 = MFFPCMPGEBK[i].GetInterframeByDistance(OIOMNNFMDOO);
			if (0 < num2)
			{
				if (num2 != 1)
				{
				}
				OEMALIFPGPO.Add(MFFPCMPGEBK[i].FGICHADOEHF);
				num++;
			}
		}
		return num;
	}
}
