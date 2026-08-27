using System;

public class WarriorAttribute
{
	private string _Name;

	public string MJBPMLCLMFN;

	public string HBCNKNFPAIM;

	public int Point;

	public bool GDCBBAHKCIE;

	public bool GDECIAJAFHH;

	public bool KDKHPMHNPCN;

	public bool GMPLHIHNHMD;

	public string CGNPILCDCCF;

	public string HCCKLLOEPJN;

	public Attributes IBLHIAHECLK = new Attributes();

	public WarriorAttribute()
	{
		Point = 0;
		GDCBBAHKCIE = false;
		GDECIAJAFHH = false;
		KDKHPMHNPCN = false;
		GMPLHIHNHMD = false;
	}

	public string get_Name()
	{
		return _Name;
	}

	public void set_Name(string value)
	{
		_Name = value;
	}

	public string ToString(int value)
	{
		string text = Math.Abs(value).ToString();
		string text2 = value.ToString();
		int length = text.Length;
		int length2 = text2.Length;
		if (Point != 0 && Point < length)
		{
			text2 = text2.Insert(length2 - Point, ".");
			string text3 = text2.Substring(length2 - Point, length2);
			int num = 0;
			int num2 = text3.Length - 1;
			while (num2 > 0 && (text3[num2] == '0' || text3[num2] == '.'))
			{
				num++;
				num2--;
			}
			num = ((text3.Length - 1 != num) ? num : (num + 1));
			text2 = text2.Substring(0, length2 + 1 - num);
		}
		return text2 + CGNPILCDCCF;
	}
}
