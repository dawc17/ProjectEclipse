using System.Collections.Generic;
using System.Diagnostics;

public class KeyValuePairList
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<KeyValuePair> HEHOKIJGDGI;

	public List<KeyValuePair> OGLHOJNMEBD
	{
		get
		{
			return CCEDNLIDAND();
		}
		protected set
		{
			CAAHOEMAAAL(value);
		}
	}

	public List<KeyValuePair> CCEDNLIDAND()
	{
		return HEHOKIJGDGI;
	}

	protected void CAAHOEMAAAL(List<KeyValuePair> value)
	{
		HEHOKIJGDGI = value;
	}

	public bool KJFEPAOCNGO(string value, out KeyValuePair KKNOCIPBIIK)
	{
		KKNOCIPBIIK = null;
		for (int i = 0; i < CCEDNLIDAND().Count; i++)
		{
			if (string.CompareOrdinal(CCEDNLIDAND()[i].AENLBNDAEKB(), value) == 0)
			{
				KKNOCIPBIIK = CCEDNLIDAND()[i];
				return true;
			}
		}
		return false;
	}

	public bool HasAny(string OIPHDFDAOFN, string DBALKBDCIKJ = "")
	{
		for (int i = 0; i < CCEDNLIDAND().Count; i++)
		{
			if (string.CompareOrdinal(CCEDNLIDAND()[i].AENLBNDAEKB(), OIPHDFDAOFN) == 0 || string.CompareOrdinal(CCEDNLIDAND()[i].AENLBNDAEKB(), DBALKBDCIKJ) == 0)
			{
				return true;
			}
		}
		return false;
	}
}
