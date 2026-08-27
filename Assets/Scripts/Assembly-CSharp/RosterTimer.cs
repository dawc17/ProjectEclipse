using System.Diagnostics;
using System.Xml;

public class RosterTimer
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long MFBDGFAPFNI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private XmlNode POFLFMGCMJA;

	public long LICABBHACHO
	{
		get
		{
			return CMIABOOJOEN();
		}
		set
		{
			set_EndTimeSeconds(value);
		}
	}

	public RosterTimer(string name, long MCEDKIPLOMO)
	{
		set_Name(name);
		set_EndTimeSeconds(MCEDKIPLOMO);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		RosterTimerContainer kCMICMHCEBB = nKGLHEGIKKP.AEMFLPNDDKL();
		XmlNode mEEAKLDGLDF = kCMICMHCEBB.LIGMHKEOJBB();
		set_Node(mEEAKLDGLDF.ACBPMPMPKJJ("Timer"));
		LIGMHKEOJBB().LLIKNHNLGJJ("Name").Value = get_Name();
		LIGMHKEOJBB().LLIKNHNLGJJ("EndTime").Value = ((ulong)CMIABOOJOEN()/*cast due to constrained. prefix*/).ToString();
	}

	public RosterTimer(XmlNode node)
	{
		set_Node(node);
		if (LIGMHKEOJBB().Attributes["Name"].Empty())
		{
			LIGMHKEOJBB().LLIKNHNLGJJ("Name").Value = string.Empty;
		}
		if (LIGMHKEOJBB().Attributes["EndTime"].Empty())
		{
			LIGMHKEOJBB().LLIKNHNLGJJ("EndTime").Value = "0";
		}
		set_Name(LIGMHKEOJBB().Attributes["Name"].CIPOICEEIBK(string.Empty));
		set_EndTimeSeconds(LIGMHKEOJBB().Attributes["EndTime"].ParseLong(0L));
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public long CMIABOOJOEN()
	{
		return MFBDGFAPFNI;
	}

	public void set_EndTimeSeconds(long value)
	{
		MFBDGFAPFNI = value;
	}

	public XmlNode LIGMHKEOJBB()
	{
		return POFLFMGCMJA;
	}

	private void set_Node(XmlNode value)
	{
		POFLFMGCMJA = value;
	}
}
