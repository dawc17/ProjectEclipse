using System.Xml;
using Nekki.SF2.Core;

public class QuestActionWait : QuestAction
{
	private int DOODNMJOHJB;

	private int frames;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		frames = EPKLCPOEELO.Attributes["Frames"].ParseInt();
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ApplicationController.add_OnUpdate(OnEveryFrame);
	}

	private void OnEveryFrame()
	{
		DOODNMJOHJB++;
		if (DOODNMJOHJB >= frames)
		{
			Stop();
		}
	}

	private void Stop()
	{
		ApplicationController.remove_OnUpdate(OnEveryFrame);
		OGIJONMKABB();
	}

	public override void GKFMJKAAJCA()
	{
		base.GKFMJKAAJCA();
		DOODNMJOHJB = 0;
	}
}
