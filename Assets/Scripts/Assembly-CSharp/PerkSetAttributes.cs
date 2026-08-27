using System.Collections.Generic;

public class PerkSetAttributes
{
	public Dictionary<string, string> IBLHIAHECLK;

	public PerkSetAttributes()
	{
		IBLHIAHECLK = new Dictionary<string, string>();
	}

	public PerkSetAttributes(PerkSetAttributes NOLFMPDGCOC)
	{
		IBLHIAHECLK = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item in NOLFMPDGCOC.IBLHIAHECLK)
		{
			IBLHIAHECLK.Add(item.Key, item.Value);
		}
	}

	public void AddRange(PerkSetAttributes NOLFMPDGCOC)
	{
		if (NOLFMPDGCOC == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in NOLFMPDGCOC.IBLHIAHECLK)
		{
			IBLHIAHECLK[item.Key] = item.Value;
		}
	}

	public void ENDOOADOLEO(string name, string value)
	{
		IBLHIAHECLK[name] = value;
	}

	public string GetValue(string name)
	{
		if (IBLHIAHECLK.ContainsKey(name))
		{
			return IBLHIAHECLK[name];
		}
		return name;
	}
}
