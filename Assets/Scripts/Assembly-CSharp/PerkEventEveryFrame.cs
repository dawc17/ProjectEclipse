using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkEventEveryFrame : PerkEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int PLFANBCJCLN;

	public int COEJGMIFLGD
	{
		get
		{
			return ONPHCNLGPBE();
		}
		protected set
		{
			set_Step(value);
		}
	}

	public PerkEventEveryFrame()
	{
	}

	public PerkEventEveryFrame(PerkEventEveryFrame NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Step(NOLFMPDGCOC.ONPHCNLGPBE());
	}

	public int ONPHCNLGPBE()
	{
		return PLFANBCJCLN;
	}

	protected void set_Step(int value)
	{
		PLFANBCJCLN = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Step(node.Attributes["Step"].ParseInt());
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (!base.IsEqual(EJMEALJNNIL) || EJMEALJNNIL == null || EJMEALJNNIL.Info == null)
		{
			return false;
		}
		if (ONPHCNLGPBE() != 0)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)EJMEALJNNIL.Info;
			if (dictionary != null)
			{
				long num = 0L;
				if (dictionary.ContainsKey("StepFrame"))
				{
					num = Convert.ToInt64(dictionary["StepFrame"]);
				}
				return 0 == num % ONPHCNLGPBE();
			}
		}
		return true;
	}
}
