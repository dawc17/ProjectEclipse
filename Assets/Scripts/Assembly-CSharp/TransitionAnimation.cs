using System.Collections.Generic;

public class TransitionAnimation
{
	private List<ConditionAnimation> KEJBANPKCFA = new List<ConditionAnimation>();

	public bool IsFrameShift;

	public int FrameShift;

	public List<ConditionAnimation> JIFAHHGNPFH
	{
		set
		{
			AJKANHBOADL(value);
		}
	}

	public TransitionAnimation()
	{
		IsFrameShift = false;
		FrameShift = 0;
	}

	public void AJKANHBOADL(List<ConditionAnimation> value)
	{
		KEJBANPKCFA = value;
	}

	public void CHDLHMGPDHL(ConditionAnimation IOFGGOCEIAM)
	{
		KEJBANPKCFA.Add(IOFGGOCEIAM);
	}

	public bool HPPGNJJCEGF(ModelConditions conditions)
	{
		for (int i = 0; i < KEJBANPKCFA.Count; i++)
		{
			if (!KEJBANPKCFA[i].IsEqual(conditions))
			{
				return false;
			}
		}
		return true;
	}
}
