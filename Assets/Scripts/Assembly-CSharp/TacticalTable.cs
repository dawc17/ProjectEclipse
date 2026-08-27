using System.Collections.Generic;

public class TacticalTable
{
	public List<Intervals> OCFKLCDIEBF = new List<Intervals>();

	public string Label = string.Empty;

	public int FirstFrameIndex;

	public int GLJMJNIAKFN
	{
		get
		{
			return get_MaxFrame();
		}
	}

	public int get_MaxFrame()
	{
		return FirstFrameIndex + OCFKLCDIEBF.Count - 1;
	}

	private static void Load(string PMFEIPCHENB)
	{
	}

	public Intervals GetFrameByFrameIndex(int FMNGLKIGFNA)
	{
		int num = GetArrayIndexByFrameIndex(FMNGLKIGFNA);
		if (-1 < num)
		{
			return OCFKLCDIEBF[num];
		}
		return null;
	}

	public int GetArrayIndexByFrameIndex(int FMNGLKIGFNA)
	{
		int num = FMNGLKIGFNA - FirstFrameIndex;
		if (num < OCFKLCDIEBF.Count)
		{
			return num;
		}
		return -1;
	}
}
