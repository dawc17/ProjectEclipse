using System.Xml;

public class EventRoundStage : EventAnimation
{
	private StageType.FDBBPEGEGMK ABENLPAPJIC;

	public StageType.FDBBPEGEGMK CCELFHJCJBN
	{
		get
		{
			return IEDKJLFBCBK();
		}
	}

	public EventRoundStage()
		: base(EECEJKADLCK.EVENT_ROUND_STAGE)
	{
		ABENLPAPJIC = StageType.FDBBPEGEGMK.STAGE_NONE;
	}

	public StageType.FDBBPEGEGMK IEDKJLFBCBK()
	{
		return ABENLPAPJIC;
	}

	protected override bool Compare(EventAnimation FOPOKALJIIJ)
	{
		EventRoundStage gBIJAGPBADA = FOPOKALJIIJ as EventRoundStage;
		bool flag = gBIJAGPBADA.ABENLPAPJIC == ABENLPAPJIC;
		return (!IsNot) ? flag : (!flag);
	}

	protected override void Parse(XmlNode MEEAKLDGLDF)
	{
		ABENLPAPJIC = StageType.GetStageByName(LJICHLHMBFA);
	}
}
