using System.Collections.Generic;

public class NewsItem
{
	public string Name = string.Empty;

	public string Url = string.Empty;

	public string MDDOAGNHAHE = string.Empty;

	public string NHKMCLPOMFK = string.Empty;

	public string Title = string.Empty;

	public string COIGFENOMJD = string.Empty;

	public string KJHMHHBJEDH = string.Empty;

	public bool DCHJDPCEODD;

	public bool CIKJHDEGHGD;

	public bool GAHGCJNGDMH;

	public bool EGBHELMJJKO;

	public int Id;

	public long EndDate;

	public List<NewsButton> DHKDOHFKOOJ = new List<NewsButton>();

	public NewsItem()
	{
	}

	public NewsItem(NewsItem AOMLCBHAJJH)
	{
		Name = AOMLCBHAJJH.Name;
		Url = AOMLCBHAJJH.Url;
		MDDOAGNHAHE = AOMLCBHAJJH.MDDOAGNHAHE;
		NHKMCLPOMFK = AOMLCBHAJJH.NHKMCLPOMFK;
		Title = AOMLCBHAJJH.Title;
		COIGFENOMJD = AOMLCBHAJJH.COIGFENOMJD;
		KJHMHHBJEDH = AOMLCBHAJJH.KJHMHHBJEDH;
		DCHJDPCEODD = AOMLCBHAJJH.DCHJDPCEODD;
		CIKJHDEGHGD = AOMLCBHAJJH.CIKJHDEGHGD;
		GAHGCJNGDMH = AOMLCBHAJJH.GAHGCJNGDMH;
		EGBHELMJJKO = AOMLCBHAJJH.EGBHELMJJKO;
		Id = AOMLCBHAJJH.Id;
		EndDate = AOMLCBHAJJH.EndDate;
		foreach (NewsButton item in AOMLCBHAJJH.DHKDOHFKOOJ)
		{
			DHKDOHFKOOJ.Add(new NewsButton(item));
		}
	}

	public bool FLEKKICJCNK(string value)
	{
		bool result = false;
		if (COIGFENOMJD == value)
		{
			result = true;
		}
		else
		{
			foreach (NewsButton item in DHKDOHFKOOJ)
			{
				if (item.COIGFENOMJD == value)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}
}
