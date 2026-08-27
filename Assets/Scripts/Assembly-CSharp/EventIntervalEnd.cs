using System.Collections.Generic;
using System.Xml;

public class EventIntervalEnd : EventAnimation
{
	private IntervalAnimation.NGAJJDIEDGF PLJCBEKDIMA;

	public EventIntervalEnd()
		: base(EECEJKADLCK.EVENT_INTERVAL_END)
	{
		PLJCBEKDIMA = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
	}

	protected override bool Compare(EventAnimation FOPOKALJIIJ)
	{
		EventIntervalEnd cNHAJBPAJAF = FOPOKALJIIJ as EventIntervalEnd;
		List<IntervalAnimation> cAANBJEPGAA = cNHAJBPAJAF.JIFAHHGNPFH.Intervals;
		bool flag = KDPJLEOGABP(cAANBJEPGAA);
		return (!IsNot) ? flag : (!flag);
	}

	protected override void Parse(XmlNode MEEAKLDGLDF)
	{
		switch (MEEAKLDGLDF.Attributes["Type"].CIPOICEEIBK(string.Empty))
		{
		case "Attack":
			PLJCBEKDIMA = IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK;
			break;
		case "Block":
			PLJCBEKDIMA = IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK;
			break;
		case "Invulnerable":
			PLJCBEKDIMA = IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE;
			break;
		default:
			PLJCBEKDIMA = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
			break;
		}
	}

	private bool KDPJLEOGABP(List<IntervalAnimation> NFLDEGMEJAK)
	{
		int i = 0;
		for (int count = NFLDEGMEJAK.Count; i < count; i++)
		{
			IntervalAnimation mNOIEOBBCMI = NFLDEGMEJAK[i];
			if ((PLJCBEKDIMA == IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE || PLJCBEKDIMA == mNOIEOBBCMI.Type) && (LJICHLHMBFA == string.Empty || LJICHLHMBFA == mNOIEOBBCMI.Name))
			{
				return true;
			}
		}
		return false;
	}
}
