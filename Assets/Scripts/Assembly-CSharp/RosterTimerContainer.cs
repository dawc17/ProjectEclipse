using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class RosterTimerContainer
{
	public enum NKFIIMIKPAO
	{
		TIMER_END = 0
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private XmlNode POFLFMGCMJA;

	private List<RosterTimer> ELALCENFCPJ = new List<RosterTimer>();

	public RosterTimerContainer(XmlNode node)
	{
		set_Node(node);
		foreach (XmlNode childNode in LIGMHKEOJBB().ChildNodes)
		{
			POEJBJOHFDP(childNode);
		}
	}

	public XmlNode LIGMHKEOJBB()
	{
		return POFLFMGCMJA;
	}

	protected void set_Node(XmlNode value)
	{
		POFLFMGCMJA = value;
	}

	public List<RosterTimer> NJCNKPFHNHB()
	{
		return ELALCENFCPJ;
	}

	public void POEJBJOHFDP(string name, long MCEDKIPLOMO)
	{
		RosterTimer nFFICPMLCFD = new RosterTimer(name, MCEDKIPLOMO);
		POEJBJOHFDP(nFFICPMLCFD);
	}

	public void POEJBJOHFDP(XmlNode node)
	{
		RosterTimer nFFICPMLCFD = new RosterTimer(node);
		POEJBJOHFDP(nFFICPMLCFD);
	}

	public void POEJBJOHFDP(RosterTimer NFFICPMLCFD)
	{
		RosterTimer fPNMILOHPMB = PPCMACMLHCA(NFFICPMLCFD.get_Name());
		if (fPNMILOHPMB == null)
		{
			ELALCENFCPJ.Add(NFFICPMLCFD);
			return;
		}
		IPKMLCMAINI(fPNMILOHPMB);
		POEJBJOHFDP(NFFICPMLCFD);
	}

	public RosterTimer PPCMACMLHCA(string name)
	{
		foreach (RosterTimer item in ELALCENFCPJ)
		{
			if (item.get_Name().Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public void IPKMLCMAINI(string name)
	{
		RosterTimer kIKOMNOGKDK = PPCMACMLHCA(name);
		IPKMLCMAINI(kIKOMNOGKDK);
	}

	public void IPKMLCMAINI(RosterTimer KIKOMNOGKDK)
	{
		if (KIKOMNOGKDK == null)
		{
			return;
		}
		for (int i = 0; i < ELALCENFCPJ.Count; i++)
		{
			RosterTimer fPNMILOHPMB = ELALCENFCPJ[i];
			if (fPNMILOHPMB == KIKOMNOGKDK)
			{
				LIGMHKEOJBB().RemoveChild(fPNMILOHPMB.LIGMHKEOJBB());
				ELALCENFCPJ.Remove(fPNMILOHPMB);
				break;
			}
		}
	}

	public void CHGALMBOHAH()
	{
		XmlNode parentNode = LIGMHKEOJBB().ParentNode;
		string name = LIGMHKEOJBB().Name;
		parentNode.RemoveChild(LIGMHKEOJBB());
		set_Node(parentNode.ACBPMPMPKJJ(name));
	}

	public void CheckTimers(long LBIGLJLMIDG)
	{
		List<RosterTimer> list = new List<RosterTimer>();
		foreach (RosterTimer item in ELALCENFCPJ)
		{
			if (item.CMIABOOJOEN() <= LBIGLJLMIDG)
			{
				list.Add(item);
				LIOBMNJPHFH(item);
			}
		}
		foreach (RosterTimer item2 in list)
		{
			IPKMLCMAINI(item2);
		}
		list.Clear();
	}

	public void LIOBMNJPHFH(RosterTimer timer)
	{
		LIOBMNJPHFH(timer.get_Name());
	}

	public void LIOBMNJPHFH(string name)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.NAMGBBCEEEI = name;
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_TIMER_END))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}

	public void BBGLKKEMOBF(string EBGIGEGKIBD)
	{
		IPKMLCMAINI(EBGIGEGKIBD);
	}
}
