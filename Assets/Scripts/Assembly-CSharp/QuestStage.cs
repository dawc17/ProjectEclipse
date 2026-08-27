using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

public class QuestStage : global::EventDispatcher<object>, IComparable<QuestStage>
{
	public enum HPOLGFKCOOE
	{
		QUEST_UNCOMPLETE = 0,
		QUEST_ACTIONS = 1,
		QUEST_COMPLETE = 2
	}

	public enum KPNDBFINCMM
	{
		OnComplete = 0,
		OnCompleteQuest = 1
	}

	private List<QuestEvent> DNBFFLFBDOB = new List<QuestEvent>();

	private List<QuestCondition> conditions = new List<QuestCondition>();

	private List<QuestActionCheckPoint> KKEDDLHBJHA = new List<QuestActionCheckPoint>();

	private List<string> FAKEEEIEBFI = new List<string>();

	private List<string> FBDKJJBICOK = new List<string>();

	private QuestActionsSequence AFENHJFICNN = new QuestActionsSequence();

	private QuestActionCheckPoint JAPJJHBDLKB;

	private QuestParameters NFIKJCJGMBB;

	private int NCIGHJBBMJK;

	public int DEFHBAPNPHI;

	public int index;

	public int ABBODBKGCCL;

	public bool allowDoubles;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NICJKIEBEOP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private RosterQuest CGOHBJELBLF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HPOLGFKCOOE DDBFPPBBDLL;

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
		private set
		{
			IMMLGNKJPKA(value);
		}
	}

	public RosterQuest NAHAGPHNGBG
	{
		get
		{
			return LBIPHHIJEFP();
		}
		private set
		{
			IOFIGEAFDEI(value);
		}
	}

	public HPOLGFKCOOE GMMGAEANIAH
	{
		get
		{
			return MHFPGCBLGIP();
		}
		private set
		{
			GKJHJHMAGLE(value);
		}
	}

	public QuestStage(XmlNode node, string PMFEIPCHENB)
	{
		IMMLGNKJPKA(PMFEIPCHENB);
		set_Name(XmlUtils.ParseString(node.Attributes["Name"], string.Empty));
		FBDKJJBICOK.Add(get_Name());
		string text = XmlUtils.ParseString(node.Attributes["Group"], string.Empty);
		FBDKJJBICOK.AddRange(text.Split('|'));
		GKJHJHMAGLE(HPOLGFKCOOE.QUEST_UNCOMPLETE);
		DEFHBAPNPHI = XmlUtils.ParseInt(node.Attributes["Priority"]);
		ABBODBKGCCL = XmlUtils.ParseInt(node.Attributes["Unresumable"]);
		allowDoubles = XmlUtils.ParseBool(node.Attributes["AllowDoubles"]);
		JBNILFIHMMK(node["Events"], DNBFFLFBDOB, this);
		DKPIKJMJPPH(node["Conditions"], conditions, this);
		EFJHONIPBOC(node["Actions"], AFENHJFICNN, this);
		ParseMarks(node["Marks"], FAKEEEIEBFI, this);
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public string EPDMGFELIMC()
	{
		return NICJKIEBEOP;
	}

	private void IMMLGNKJPKA(string value)
	{
		NICJKIEBEOP = value;
	}

	public RosterQuest LBIPHHIJEFP()
	{
		return CGOHBJELBLF;
	}

	private void IOFIGEAFDEI(RosterQuest value)
	{
		CGOHBJELBLF = value;
	}

	public HPOLGFKCOOE MHFPGCBLGIP()
	{
		return DDBFPPBBDLL;
	}

	private void GKJHJHMAGLE(HPOLGFKCOOE value)
	{
		DDBFPPBBDLL = value;
	}

	private void ParseMarks(XmlNode CDFJOIJHJDA, List<string> GENLBPMKENI, QuestStage PJEAMPLHPOH)
	{
		if (CDFJOIJHJDA == null)
		{
			return;
		}
		XmlNodeList childNodes = CDFJOIJHJDA.ChildNodes;
		foreach (XmlNode item2 in childNodes)
		{
			string item = XmlUtils.ParseString(item2.Attributes["Name"]);
			GENLBPMKENI.Add(item);
		}
	}

	private void JBNILFIHMMK(XmlNode MKFADLKDEJM, List<QuestEvent> GENLBPMKENI, QuestStage PJEAMPLHPOH = null)
	{
		if (MKFADLKDEJM == null)
		{
			return;
		}
		XmlNodeList childNodes = MKFADLKDEJM.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			QuestEvent hKFNABCMDCB = new QuestEvent();
			hKFNABCMDCB.Parse(item);
			GENLBPMKENI.Add(hKFNABCMDCB);
		}
	}

	private void DKPIKJMJPPH(XmlNode IPDGDBMMHEP, List<QuestCondition> GENLBPMKENI, QuestStage PJEAMPLHPOH = null)
	{
		if (IPDGDBMMHEP == null)
		{
			return;
		}
		XmlNodeList childNodes = IPDGDBMMHEP.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			QuestCondition kKDGLNECFHA = new QuestCondition();
			kKDGLNECFHA.Parse(item);
			if (kKDGLNECFHA.LFLGCDNKNJI == QuestCondition.NFFNINLIPJJ.QUEST_CONDITION_OPERATOR)
			{
				DKPIKJMJPPH(item, kKDGLNECFHA.conditions);
			}
			GENLBPMKENI.Add(kKDGLNECFHA);
		}
	}

	private void EFJHONIPBOC(XmlNode EPKLCPOEELO, QuestActionsSequence GENLBPMKENI, QuestStage PJEAMPLHPOH = null)
	{
		if (EPKLCPOEELO == null)
		{
			return;
		}
		string bAINMLLIKOL = XmlUtils.ParseString(EPKLCPOEELO.Attributes["Place"], "Map");
		NCIGHJBBMJK = NMCNDOPKFJD(bAINMLLIKOL);
		int num = 0;
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			if (num == 0)
			{
				JAPJJHBDLKB = new QuestActionCheckPoint();
				JAPJJHBDLKB.ONGHPGEIJEN = get_Name();
				JAPJJHBDLKB.AEHNKDOJALB = EPDMGFELIMC();
				JAPJJHBDLKB.Index = 0;
				JAPJJHBDLKB.AMIMGEOENPL = NCIGHJBBMJK;
				JAPJJHBDLKB.Parse(childNode);
				JAPJJHBDLKB.EPFCAILHDII(this);
				JAPJJHBDLKB.AddEventListener(2, DAEMPBALNGM);
				KKEDDLHBJHA.Add(JAPJJHBDLKB);
			}
			FGAEEJBEGEJ(childNode.Name, childNode, GENLBPMKENI, num);
			num++;
		}
		IOFIGEAFDEI(ListSF.CCDKHLAMKKO().OOMJEHAKOBA(get_Name()));
		GENLBPMKENI.AddEventListener(1, OnActionComplete);
	}

	private void OnActionComplete(object data)
	{
		MFGLIALECAM();
	}

	private void DAEMPBALNGM(object data)
	{
		if (data != null)
		{
			IOFIGEAFDEI((RosterQuest)data);
		}
	}

	private void FGAEEJBEGEJ(string LJICOHPCPKO, XmlNode node, QuestActionsSequence GENLBPMKENI, int index)
	{
		QuestAction mBAAKHELFKL = QuestAction.GetClassActionByName(LJICOHPCPKO);
		mBAAKHELFKL.ONGHPGEIJEN = get_Name();
		mBAAKHELFKL.AEHNKDOJALB = EPDMGFELIMC();
		mBAAKHELFKL.EPFCAILHDII(this);
		mBAAKHELFKL.Index = index;
		mBAAKHELFKL.AMIMGEOENPL = NCIGHJBBMJK;
		mBAAKHELFKL.Parse(node);
		GENLBPMKENI.NLJLHHNPCAO(mBAAKHELFKL);
	}

	public static int NMCNDOPKFJD(string value)
	{
		if (value.Equals("Fight"))
		{
			return 5;
		}
		if (value.Equals("Dojo"))
		{
			return 2;
		}
		if (value.Equals("Map"))
		{
			return 4;
		}
		return -1;
	}

	public void MFGLIALECAM()
	{
		if (AGJGEBBLFGA())
		{
			if (GameUtils.IEJDNMPFLPP.GLHICPIHDKA)
			{
				string text = "Quest ";
				text += get_Name();
				text += " completed";
				LLLOJBFMONN.INNGABABJPC(text);
			}
			CallEvent(0, this);
		}
		CallEvent(1, this);
	}

	public QuestEvent HAANFOGOEHM(string MCGHIOHACBJ)
	{
		QuestEvent.PMDPDMFLCIJ mCGHIOHACBJ = QuestEvent.HDPFFPAGOPE(MCGHIOHACBJ);
		return HAANFOGOEHM(mCGHIOHACBJ);
	}

	public QuestEvent HAANFOGOEHM(QuestEvent.PMDPDMFLCIJ MCGHIOHACBJ)
	{
		foreach (QuestEvent item in DNBFFLFBDOB)
		{
			if (item.IsEvent(MCGHIOHACBJ))
			{
				return item;
			}
		}
		return null;
	}

	public bool IsEvent(string MCGHIOHACBJ)
	{
		QuestEvent.PMDPDMFLCIJ mCGHIOHACBJ = QuestEvent.HDPFFPAGOPE(MCGHIOHACBJ);
		return IsEvent(mCGHIOHACBJ);
	}

	public bool IsEvent(QuestEvent.PMDPDMFLCIJ MCGHIOHACBJ)
	{
		QuestEvent hKFNABCMDCB = HAANFOGOEHM(MCGHIOHACBJ);
		return hKFNABCMDCB != null;
	}

	public bool IsGroup(List<string> FBDKJJBICOK)
	{
		foreach (string item in FBDKJJBICOK)
		{
			foreach (string item2 in this.FBDKJJBICOK)
			{
				if (item.Equals(item2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool Compare(QuestParameters GFIHPBCEEOB)
	{
		foreach (QuestCondition item in conditions)
		{
			if (!item.Compare(GFIHPBCEEOB, LBIPHHIJEFP()))
			{
				return false;
			}
		}
		return true;
	}

	public void MHNEBBGMOLA(QuestParameters GFIHPBCEEOB)
	{
		if (JAPJJHBDLKB != null)
		{
			JAPJJHBDLKB.OIPDKFAJILO(GFIHPBCEEOB);
		}
	}

	public void MHHNIPBJNAD(QuestParameters GFIHPBCEEOB, bool MKBPLLIHMPE)
	{
		if (LogRules.ELEBLBJKDBI().MDKADLMMJLD())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Quest ");
			stringBuilder.Append(get_Name());
			stringBuilder.Append(" started");
			LLLOJBFMONN.INNGABABJPC(stringBuilder.ToString());
		}
		GKJHJHMAGLE(HPOLGFKCOOE.QUEST_ACTIONS);
		AFENHJFICNN.JJIHOMLLAOL = ((MKBPLLIHMPE && LBIPHHIJEFP() != null) ? LBIPHHIJEFP().NGGNCHPDOOI() : 0);
		AFENHJFICNN.DEJMHFMLKIC(GFIHPBCEEOB);
	}

	public string OCMHJBKFABM(HPOLGFKCOOE value)
	{
		switch (value)
		{
		case HPOLGFKCOOE.QUEST_UNCOMPLETE:
			return "UNCOMPLETE";
		case HPOLGFKCOOE.QUEST_ACTIONS:
			return "ACTIONS";
		case HPOLGFKCOOE.QUEST_COMPLETE:
			return "COMPLETE";
		default:
			return string.Empty;
		}
	}

	public HPOLGFKCOOE LNOHCOCPGJL(string value)
	{
		switch (value)
		{
		case "UNCOMPLETE":
			return HPOLGFKCOOE.QUEST_UNCOMPLETE;
		case "ACTIONS":
			return HPOLGFKCOOE.QUEST_ACTIONS;
		case "COMPLETE":
			return HPOLGFKCOOE.QUEST_COMPLETE;
		default:
			return HPOLGFKCOOE.QUEST_UNCOMPLETE;
		}
	}

	public bool AGJGEBBLFGA()
	{
		GKJHJHMAGLE(HPOLGFKCOOE.QUEST_COMPLETE);
		if (LBIPHHIJEFP() != null)
		{
			LBIPHHIJEFP().LCIHKPPGNPF();
			ListSF.ELEBLBJKDBI().EJANJEEGOOE();
			return true;
		}
		return false;
	}

	public bool IDGAAJAFCHC()
	{
		return ABBODBKGCCL > 0;
	}

	public QuestParameters AFALCHHKLFP()
	{
		if (LBIPHHIJEFP() != null)
		{
			return JMHGHCAGFDI(LBIPHHIJEFP().get_Parameters());
		}
		return new QuestParameters();
	}

	public QuestParameters JMHGHCAGFDI(ParametersQuest KKNOCIPBIIK)
	{
		QuestParameters hHKLFIIBIFF = new QuestParameters();
		if (KKNOCIPBIIK != null)
		{
			FightList jDIPBIHBGPF = ListSF.ELEBLBJKDBI().AOEPHEPGLAK(KKNOCIPBIIK.HPELIEHPJCI());
			hHKLFIIBIFF.JLGLBLDPAAF = ((jDIPBIHBGPF == null) ? FightIDS.Empty() : jDIPBIHBGPF.BCKFACGMOKC);
			hHKLFIIBIFF.HEIADONEACH = KKNOCIPBIIK.LIPMCBHCLKN();
			hHKLFIIBIFF.AIEHNBBFNPF = KKNOCIPBIIK.JOLAAOAFNFF();
			hHKLFIIBIFF.BJIDALJIKNC = KKNOCIPBIIK.OGIPFNNJOPK();
			hHKLFIIBIFF.JNGFNNFAAGN = KKNOCIPBIIK.NHKMGNPADKI();
		}
		return hHKLFIIBIFF;
	}

	public int CompareTo(QuestStage NOLFMPDGCOC)
	{
		if (MHFPGCBLGIP() == HPOLGFKCOOE.QUEST_ACTIONS && NOLFMPDGCOC.MHFPGCBLGIP() == HPOLGFKCOOE.QUEST_ACTIONS)
		{
			return 1;
		}
		if (MHFPGCBLGIP() != HPOLGFKCOOE.QUEST_ACTIONS && NOLFMPDGCOC.MHFPGCBLGIP() == HPOLGFKCOOE.QUEST_ACTIONS)
		{
			return -1;
		}
		return NOLFMPDGCOC.DEFHBAPNPHI.CompareTo(DEFHBAPNPHI);
	}
}
