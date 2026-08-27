public class EventHit : EventAnimation
{
	public EventHit()
		: base(EECEJKADLCK.EVENT_HIT)
	{
	}

	protected override bool Compare(EventAnimation FOPOKALJIIJ)
	{
		bool flag = false;
		EventHit eLGNDOJMOBH = FOPOKALJIIJ as EventHit;
		if (string.IsNullOrEmpty(LJICHLHMBFA))
		{
			flag = true;
		}
		else
		{
			Model.StrikeResult jEGHAGLEJCB = (Model.StrikeResult)FOPOKALJIIJ.JIFAHHGNPFH.StrikeResult;
			Model gAIBPAGPEGK = jEGHAGLEJCB.GAIBPAGPEGK;
			IntervalAttack hFIIPNLCIEE = (IntervalAttack)gAIBPAGPEGK.OCPMJKIEPIG().HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK);
			if (hFIIPNLCIEE != null)
			{
				string text = hFIIPNLCIEE.GetReactionName(gAIBPAGPEGK.NODAINEDAKJ());
				flag = text == LJICHLHMBFA;
			}
		}
		if (flag && !LONCGFHLFKA.BKOIKMEEHDK())
		{
			bool flag2 = LONCGFHLFKA == eLGNDOJMOBH.LONCGFHLFKA;
			flag = flag && flag2;
		}
		if (flag && !PLNBENLPIBD.BKOIKMEEHDK())
		{
			bool flag3 = PLNBENLPIBD == eLGNDOJMOBH.PLNBENLPIBD;
			flag = flag && flag3;
		}
		return flag;
	}
}
