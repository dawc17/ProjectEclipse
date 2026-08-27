using System.Collections.Generic;
using System.Xml;

public class EventModExpires : EventAnimation
{
	private string GKJIAILGHMK;

	public string POLPHCDNLEL
	{
		get
		{
			return CMKKGFDBBJF();
		}
	}

	public EventModExpires()
		: base(EECEJKADLCK.EVENT_MOD_EXPIRES)
	{
	}

	public string CMKKGFDBBJF()
	{
		return GKJIAILGHMK;
	}

	protected override bool Compare(EventAnimation FOPOKALJIIJ)
	{
		bool flag = false;
		List<PerksStage.ActionPerk> fPFKABHOEHP = FOPOKALJIIJ.JIFAHHGNPFH.FPFKABHOEHP;
		for (int i = 0; i < fPFKABHOEHP.Count; i++)
		{
			if (fPFKABHOEHP[i].DDBPICENEJE() == GKJIAILGHMK)
			{
				flag = true;
			}
		}
		return (!IsNot) ? flag : (!flag);
	}

	protected override void Parse(XmlNode MEEAKLDGLDF)
	{
		GKJIAILGHMK = MEEAKLDGLDF.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}
}
