using System.Collections.Generic;

public class EventAnimationStart : EventAnimation
{
	public EventAnimationStart()
		: base(EECEJKADLCK.EVENT_ANIMATION_START)
	{
	}

	protected override bool Compare(EventAnimation FOPOKALJIIJ)
	{
		EventAnimationStart hPCJMBAJKLJ = FOPOKALJIIJ as EventAnimationStart;
		bool flag = IsCompareNames(hPCJMBAJKLJ.JIFAHHGNPFH.NNPJJLPCOHD);
		return (!IsNot) ? flag : (!flag);
	}

	private bool IsCompareNames(List<string> NIKHAICFGNM)
	{
		int i = 0;
		for (int count = NIKHAICFGNM.Count; i < count; i++)
		{
			if (NIKHAICFGNM[i] == LJICHLHMBFA)
			{
				return true;
			}
		}
		return false;
	}
}
