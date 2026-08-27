using System.Collections.Generic;

public class Attributes
{
	private Dictionary<string, int> JMMIKHLIKOE;

	public int Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
	}

	public Attributes()
	{
		JMMIKHLIKOE = new Dictionary<string, int>();
	}

	public Attributes(Attributes NOLFMPDGCOC)
	{
		JMMIKHLIKOE = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> item in NOLFMPDGCOC.JMMIKHLIKOE)
		{
			JMMIKHLIKOE.Add(item.Key, item.Value);
		}
	}

	public int OFOPFCJNEBL()
	{
		return JMMIKHLIKOE.Count;
	}

	public void AddRange(Attributes NOLFMPDGCOC)
	{
		foreach (KeyValuePair<string, int> item in NOLFMPDGCOC.JMMIKHLIKOE)
		{
			JMMIKHLIKOE[item.Key] = item.Value;
		}
	}

	public bool Get(string KGBGENDIMBC, ref int OEMALIFPGPO, bool PIDPHPGMLOD = true, bool OMHMHCLDBFA = false)
	{
		if (OMHMHCLDBFA)
		{
			Aspect hOHAPDGFMHL = GameUtils.MGDCJKKGKAB(KGBGENDIMBC);
			if (hOHAPDGFMHL != null)
			{
				return Get(hOHAPDGFMHL.EJPCHOLGGJJ(), ref OEMALIFPGPO, PIDPHPGMLOD, OMHMHCLDBFA);
			}
		}
		if (KGBGENDIMBC != null && JMMIKHLIKOE.ContainsKey(KGBGENDIMBC))
		{
			int num = JMMIKHLIKOE[KGBGENDIMBC];
			if (PIDPHPGMLOD)
			{
				Aspect hOHAPDGFMHL2 = GameUtils.MGDCJKKGKAB(KGBGENDIMBC);
				if (hOHAPDGFMHL2 != null)
				{
					int gNLOCMLBNHF = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
					int bIJKNKAJBHH = GameUtils.HBDECHDPLCA().MAIPAOKJMED();
					float num2 = GameUtils.HBDECHDPLCA().OEAKCOHMIHH();
					OEMALIFPGPO = hOHAPDGFMHL2.GetValue(num, gNLOCMLBNHF, bIJKNKAJBHH, num2);
					if (JMMIKHLIKOE.ContainsKey(hOHAPDGFMHL2.EJPCHOLGGJJ()))
					{
						num = JMMIKHLIKOE[hOHAPDGFMHL2.EJPCHOLGGJJ()];
						OEMALIFPGPO += num;
					}
					return true;
				}
			}
			OEMALIFPGPO = num;
			return true;
		}
		return false;
	}

	public void Set(string KGBGENDIMBC, int value, bool OMHMHCLDBFA = false)
	{
		if (OMHMHCLDBFA)
		{
			Aspect hOHAPDGFMHL = GameUtils.MGDCJKKGKAB(KGBGENDIMBC);
			if (hOHAPDGFMHL != null)
			{
				string key = hOHAPDGFMHL.EJPCHOLGGJJ();
				JMMIKHLIKOE[key] = value;
				return;
			}
		}
		JMMIKHLIKOE[KGBGENDIMBC] = value;
	}

	public void Clear()
	{
		JMMIKHLIKOE.Clear();
	}
}
