using System.Collections.Generic;

public class IntervalNew
{
	public InfoAnimation FGICHADOEHF;

	public List<float> Distances = new List<float>();

	public List<int> Interframes = new List<int>();

	public bool IsDistanceWithin(float OIOMNNFMDOO)
	{
		int num = Distances.Count - 1;
		if (0 < num)
		{
			return Distances[0] <= OIOMNNFMDOO && OIOMNNFMDOO < Distances[num];
		}
		return false;
	}

	public int GetInterframeByDistance(float OIOMNNFMDOO)
	{
		if (IsDistanceWithin(OIOMNNFMDOO))
		{
			int i = 1;
			for (int count = Distances.Count; i < count; i++)
			{
				if (OIOMNNFMDOO < Distances[i])
				{
					return Interframes[i - 1];
				}
			}
		}
		return -1;
	}
}
