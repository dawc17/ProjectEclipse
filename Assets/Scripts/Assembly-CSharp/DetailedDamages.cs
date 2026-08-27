using System.Collections.Generic;

public class DetailedDamages
{
	public Dictionary<string, Dictionary<string, float>> BEOLFOFKIAG = new Dictionary<string, Dictionary<string, float>>();

	public void Add(float CKKFKEIELCP, string BBNKIBKPBLO, string target)
	{
		if (!BEOLFOFKIAG.ContainsKey(BBNKIBKPBLO))
		{
			BEOLFOFKIAG[BBNKIBKPBLO] = new Dictionary<string, float>();
		}
		Dictionary<string, float> dictionary = BEOLFOFKIAG[BBNKIBKPBLO];
		bool flag = dictionary.ContainsKey(target);
		dictionary[target] = ((!flag) ? CKKFKEIELCP : (dictionary[target] + CKKFKEIELCP));
	}

	public void NBAEKDHNBNL(DetailedDamages NOLFMPDGCOC)
	{
		foreach (KeyValuePair<string, Dictionary<string, float>> item in NOLFMPDGCOC.BEOLFOFKIAG)
		{
			foreach (KeyValuePair<string, float> item2 in item.Value)
			{
				Add(item2.Value, item.Key, item2.Key);
			}
		}
	}

	public float GetTotalDamage()
	{
		float num = 0f;
		foreach (KeyValuePair<string, Dictionary<string, float>> item in BEOLFOFKIAG)
		{
			foreach (KeyValuePair<string, float> item2 in item.Value)
			{
				num += item2.Value;
			}
		}
		return num;
	}

	public float IEPOFCFIKOP()
	{
		float num = 0f;
		foreach (KeyValuePair<string, Dictionary<string, float>> item in BEOLFOFKIAG)
		{
			if (!item.Key.Equals("RaidChargeDamage"))
			{
				continue;
			}
			foreach (KeyValuePair<string, float> item2 in item.Value)
			{
				num += item2.Value;
			}
		}
		return num;
	}
}
