using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nekki.SF2.Core;
using Nekki.SF2.Core.Network;
using Nekki.Utils;
using SimpleJSON;
using UnityEngine;

public class StatisticsCollector
{
	private enum NBGNCCPMCCD
	{
		NotLogging = 0,
		Logging = 1,
		Undecided = 2
	}

	public enum CNCDMFJLMFH
	{
		Money = 0,
		Bonus = 1
	}

	private const int IMADHLKIDBG = 200;

	private static StatisticsCollector _Current;

	private const string MGDGCFPHODB = "full_events.json";

	private const string PBABHJGLMCO = "events.json";

	private const string LDKDCMJENGM = "pays.json";

	private const string JPOAKGGNJPO = "events_data.xml";

	private const int KJHKCGNIINE = 2000000;

	private int CBJGPFNDAOC;

	private int FNHNEBKMHFA;

	private int PHONHMEKDDE;

	private int CFAIGEIHNLK;

	private int JEBHJOPAIGC;

	private int BLLDCBKNIJA;

	private long BPGFOAHGAHE;

	private long EKFPAJCKJLB;

	private int FIGDGGKLFDG;

	private long ONLODELNKPO;

	private uint _EndXp;

	private int JBNACLHHBPA;

	private int MGHBCNLHCID;

	private string LCNJJLAKEHK;

	private string ABOPCADKEJI;

	private int KAFIDINNDIP;

	private long HKJHIACNECB;

	private long LNIMCIMMJJG;

	private NBGNCCPMCCD COLCBNPNLPG = NBGNCCPMCCD.Undecided;

	private NBGNCCPMCCD ACNHHFKLKID = NBGNCCPMCCD.Undecided;

	private StringBuilder DCFPONJAING = new StringBuilder();

	private StringBuilder PHGGNFPBOKD = new StringBuilder();

	private long PFGKFHJMMFH;

	private const int _DeltaSendTime = 900000;

	private long DEBIFEKGFKD;

	private static string CEIDLBLDKBF
	{
		get
		{
			return ANOGPNAIFLI();
		}
	}

	private static string CPCHJPKJMAN
	{
		get
		{
			return GCHJLCEFNMK();
		}
	}

	private static string HDFIGEJDAMC
	{
		get
		{
			return EFILFAHEAKB();
		}
	}

	private static string MFGCCAEIJEB
	{
		get
		{
			return JNJGGIGNFBE();
		}
	}

	public static StatisticsCollector BLOOLFFMKFI
	{
		get
		{
			return AOJJOEHEPGM();
		}
	}

	public static int CGLECNFLHBI
	{
		get
		{
			return MONNGMBGLHH();
		}
	}

	public static int HHMNPBFGGJF
	{
		get
		{
			return POAAIJHJFEG();
		}
	}

	public static int ECCNPKGHOFF
	{
		get
		{
			return JGMAONJINDK();
		}
		set
		{
			HIMFCMGOMGI(value);
		}
	}

	public static int MOGCPPOKEJC
	{
		get
		{
			return MHKHJJIJKCD();
		}
		set
		{
			JNGMOOOPCBN(value);
		}
	}

	public static int JLDNNCAPPHC
	{
		get
		{
			return ODJEHEPNNHH();
		}
		set
		{
			JKHLAAKKHDI(value);
		}
	}

	public static long CMDLONKFNOK
	{
		get
		{
			return GOBHGMIFLAA();
		}
	}

	public static long EndDate
	{
		get
		{
			return EJNLDIPJOOI();
		}
	}

	public static int HAHNNKPGAAM
	{
		get
		{
			return FJNKLLJAPNL();
		}
	}

	public static long DOOGNLBBALO
	{
		get
		{
			return OBKJFLENCBC();
		}
	}

	public static uint NNDDNBCPELF
	{
		get
		{
			return BFKLBDEEKLN();
		}
	}

	public static int FDHKOEHKNOC
	{
		get
		{
			return CGDFOCLOHEB();
		}
	}

	public static int OJHDENMGCNE
	{
		get
		{
			return FFFEOOMOJIL();
		}
	}

	public static string PEFHKPHNJII
	{
		get
		{
			return JLFGCIGNHNC();
		}
	}

	public static string Info
	{
		get
		{
			return ADAMFIJFGBE();
		}
	}

	public static int EOGLBDCLMBM
	{
		get
		{
			return MCIPEJBLIDC();
		}
	}

	private StatisticsCollector()
	{
		ENMPJEFHECD();
		GlobalTimer.get_Instance().addEventListener(0, OnTimerTick);
		ApplicationController.add_OnPause(FFMHKLENGLP);
		Send();
	}

	private static string ANOGPNAIFLI()
	{
		return SF2Paths.LCDBGFFDKJB() + "/full_events.json";
	}

	private static string GCHJLCEFNMK()
	{
		return SF2Paths.LCDBGFFDKJB() + "/events.json";
	}

	private static string EFILFAHEAKB()
	{
		return SF2Paths.LCDBGFFDKJB() + "/pays.json";
	}

	private static string JNJGGIGNFBE()
	{
		return SF2Paths.LCDBGFFDKJB() + "/events_data.xml";
	}

	public static StatisticsCollector AOJJOEHEPGM()
	{
		if (_Current == null)
		{
			_Current = new StatisticsCollector();
		}
		return _Current;
	}

	public static int MONNGMBGLHH()
	{
		AOJJOEHEPGM().CBJGPFNDAOC++;
		_Current.AHOPPPNPOHB();
		return _Current.CBJGPFNDAOC;
	}

	public static int POAAIJHJFEG()
	{
		AOJJOEHEPGM().FNHNEBKMHFA++;
		_Current.AHOPPPNPOHB();
		return _Current.FNHNEBKMHFA;
	}

	public static int JGMAONJINDK()
	{
		return AOJJOEHEPGM().CFAIGEIHNLK;
	}

	public static void HIMFCMGOMGI(int value)
	{
		AOJJOEHEPGM().CFAIGEIHNLK = value;
		_Current.AHOPPPNPOHB();
	}

	public static int MHKHJJIJKCD()
	{
		return AOJJOEHEPGM().JEBHJOPAIGC;
	}

	public static void JNGMOOOPCBN(int value)
	{
		AOJJOEHEPGM().JEBHJOPAIGC = value;
		_Current.AHOPPPNPOHB();
	}

	public static int ODJEHEPNNHH()
	{
		return AOJJOEHEPGM().BLLDCBKNIJA;
	}

	public static void JKHLAAKKHDI(int value)
	{
		AOJJOEHEPGM().BLLDCBKNIJA = value;
		_Current.AHOPPPNPOHB();
	}

	public static long GOBHGMIFLAA()
	{
		return AOJJOEHEPGM().BPGFOAHGAHE;
	}

	public static long EJNLDIPJOOI()
	{
		return AOJJOEHEPGM().EKFPAJCKJLB;
	}

	public static int FJNKLLJAPNL()
	{
		return AOJJOEHEPGM().FIGDGGKLFDG;
	}

	public static long OBKJFLENCBC()
	{
		return AOJJOEHEPGM().ONLODELNKPO;
	}

	public static uint BFKLBDEEKLN()
	{
		return AOJJOEHEPGM()._EndXp;
	}

	public static int CGDFOCLOHEB()
	{
		return AOJJOEHEPGM().JBNACLHHBPA;
	}

	public static int FFFEOOMOJIL()
	{
		return AOJJOEHEPGM().MGHBCNLHCID;
	}

	public static string JLFGCIGNHNC()
	{
		return AOJJOEHEPGM().LCNJJLAKEHK;
	}

	public static string ADAMFIJFGBE()
	{
		return AOJJOEHEPGM().ABOPCADKEJI;
	}

	public static int MCIPEJBLIDC()
	{
		return AOJJOEHEPGM().KAFIDINNDIP;
	}

	public static void BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF IGABHEMGKKE, ArgsDict LKIOKGCNKHE = null)
	{
		if (AOJJOEHEPGM().DCFPONJAING.Length != 0)
		{
			_Current.DCFPONJAING.Append("\n");
		}
		AOJJOEHEPGM().DCFPONJAING.Append(StatisticsEvent.PCGEAIIJICB(IGABHEMGKKE, LKIOKGCNKHE));
		if (StatisticsEvent.KLOICDEDMEB(IGABHEMGKKE))
		{
			_Current.GGGEHAGCLGC();
		}
		if (StatisticsEvent.IFDADIENEKC(IGABHEMGKKE))
		{
			_Current.Send();
		}
	}

	public static void KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF IGABHEMGKKE, ArgsDict LKIOKGCNKHE = null)
	{
		if (AOJJOEHEPGM().PHGGNFPBOKD.Length != 0)
		{
			_Current.PHGGNFPBOKD.Append("\n");
		}
		AOJJOEHEPGM().PHGGNFPBOKD.Append(StatisticsEvent.PCGEAIIJICB(IGABHEMGKKE, LKIOKGCNKHE));
		if (StatisticsEvent.KLOICDEDMEB(IGABHEMGKKE))
		{
			_Current.AGNMPDEFABN();
		}
		if (StatisticsEvent.IFDADIENEKC(IGABHEMGKKE))
		{
			_Current.OPGFPNFAOLH();
		}
	}

	public void GLKJABEOHDF(bool MAACIEHOLML)
	{
		if (MAACIEHOLML)
		{
			COLCBNPNLPG = NBGNCCPMCCD.Logging;
			Send();
		}
		else
		{
			COLCBNPNLPG = NBGNCCPMCCD.NotLogging;
		}
	}

	public void IOGFHNKNGHJ(bool MAACIEHOLML)
	{
		if (MAACIEHOLML)
		{
			ACNHHFKLKID = NBGNCCPMCCD.Logging;
			OPGFPNFAOLH();
		}
		else
		{
			ACNHHFKLKID = NBGNCCPMCCD.NotLogging;
		}
	}

	private void ENMPJEFHECD()
	{
		if (!File.Exists(JNJGGIGNFBE()))
		{
			CBJGPFNDAOC = 0;
			FNHNEBKMHFA = 0;
			PHONHMEKDDE = 0;
			CFAIGEIHNLK = 0;
			JEBHJOPAIGC = 0;
			BLLDCBKNIJA = 0;
			BPGFOAHGAHE = 0L;
			EKFPAJCKJLB = 0L;
			FIGDGGKLFDG = 0;
			ONLODELNKPO = 0L;
			_EndXp = 0u;
			JBNACLHHBPA = 0;
			MGHBCNLHCID = 0;
			LCNJJLAKEHK = "0";
			ABOPCADKEJI = string.Empty;
			KAFIDINNDIP = 0;
		}
		else
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(JNJGGIGNFBE(), string.Empty, XmlUtils.EBLFEPIOMOL.ForcedExternal);
			XmlNode xmlNode = xmlDocument["Data"];
			CBJGPFNDAOC = XmlUtils.ParseInt(xmlNode["EventID"].Attribute("Value"));
			FNHNEBKMHFA = XmlUtils.ParseInt(xmlNode["PayEventID"].Attribute("Value"));
			PHONHMEKDDE = XmlUtils.ParseInt(xmlNode["RunCount"].Attribute("Value"));
			CFAIGEIHNLK = XmlUtils.ParseInt(xmlNode["SessionID"].Attribute("Value"));
			JEBHJOPAIGC = XmlUtils.ParseInt(xmlNode["FightAmount"].Attribute("Value"));
			BLLDCBKNIJA = XmlUtils.ParseInt(xmlNode["Length"].Attribute("Value"));
			BPGFOAHGAHE = XmlUtils.ParseLong(xmlNode["EndBonus"].Attribute("Value"), 0L);
			EKFPAJCKJLB = XmlUtils.ParseInt(xmlNode["EndDate"].Attribute("Value"));
			FIGDGGKLFDG = XmlUtils.ParseInt(xmlNode["EndLevel"].Attribute("Value"));
			ONLODELNKPO = XmlUtils.ParseLong(xmlNode["EndMoney"].Attribute("Value"), 0L);
			_EndXp = XmlUtils.ParseUint(xmlNode["EndXp"].Attribute("Value"));
			JBNACLHHBPA = XmlUtils.ParseInt(xmlNode["EndEnergy"].Attribute("Value"));
			MGHBCNLHCID = XmlUtils.ParseInt(xmlNode["Payments"].Attribute("Value"));
			LCNJJLAKEHK = XmlUtils.ParseString(xmlNode["CheatId"].Attribute("Value"), "0");
			ABOPCADKEJI = XmlUtils.ParseString(xmlNode["Info"].Attribute("Value"), string.Empty);
			KAFIDINNDIP = XmlUtils.ParseInt(xmlNode["Counter"].Attribute("Value"));
			HKJHIACNECB = XmlUtils.ParseInt(xmlNode["FilePosition"].Attribute("Value"));
			LNIMCIMMJJG = XmlUtils.ParseInt(xmlNode["PayLogPosition"].Attribute("Value"));
		}
	}

	private void AHOPPPNPOHB()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
		XmlElement xmlElement = xmlDocument.CreateElement("Data");
		xmlDocument.AppendChild(xmlElement);
		XmlElement xmlElement2 = xmlDocument.CreateElement("EventID");
		xmlElement.AppendChild(xmlElement2);
		xmlElement2.SetAttribute("Value", CBJGPFNDAOC.ToString());
		XmlElement newChild = xmlDocument.CreateElement("PayEventID");
		xmlElement.AppendChild(newChild);
		xmlElement2.SetAttribute("Value", FNHNEBKMHFA.ToString());
		XmlElement xmlElement3 = xmlDocument.CreateElement("RunCount");
		xmlElement.AppendChild(xmlElement3);
		xmlElement3.SetAttribute("Value", PHONHMEKDDE.ToString());
		XmlElement xmlElement4 = xmlDocument.CreateElement("SessionID");
		xmlElement.AppendChild(xmlElement4);
		xmlElement4.SetAttribute("Value", CFAIGEIHNLK.ToString());
		XmlElement xmlElement5 = xmlDocument.CreateElement("FightAmount");
		xmlElement.AppendChild(xmlElement5);
		xmlElement5.SetAttribute("Value", JEBHJOPAIGC.ToString());
		XmlElement xmlElement6 = xmlDocument.CreateElement("TimeLength");
		xmlElement.AppendChild(xmlElement6);
		xmlElement6.SetAttribute("Value", BLLDCBKNIJA.ToString());
		XmlElement xmlElement7 = xmlDocument.CreateElement("EndBonus");
		xmlElement.AppendChild(xmlElement7);
		xmlElement7.SetAttribute("Value", BPGFOAHGAHE.ToString());
		XmlElement xmlElement8 = xmlDocument.CreateElement("EndDate");
		xmlElement.AppendChild(xmlElement8);
		xmlElement8.SetAttribute("Value", EKFPAJCKJLB.ToString());
		XmlElement xmlElement9 = xmlDocument.CreateElement("EndLevel");
		xmlElement.AppendChild(xmlElement9);
		xmlElement9.SetAttribute("Value", FIGDGGKLFDG.ToString());
		XmlElement xmlElement10 = xmlDocument.CreateElement("EndMoney");
		xmlElement.AppendChild(xmlElement10);
		xmlElement10.SetAttribute("Value", ONLODELNKPO.ToString());
		XmlElement xmlElement11 = xmlDocument.CreateElement("EndXp");
		xmlElement.AppendChild(xmlElement11);
		xmlElement11.SetAttribute("Value", _EndXp.ToString());
		XmlElement xmlElement12 = xmlDocument.CreateElement("EndEnergy");
		xmlElement.AppendChild(xmlElement12);
		xmlElement12.SetAttribute("Value", JBNACLHHBPA.ToString());
		XmlElement xmlElement13 = xmlDocument.CreateElement("Payments");
		xmlElement.AppendChild(xmlElement13);
		xmlElement13.SetAttribute("Value", MGHBCNLHCID.ToString());
		XmlElement xmlElement14 = xmlDocument.CreateElement("CheatId");
		xmlElement.AppendChild(xmlElement14);
		xmlElement14.SetAttribute("Value", LCNJJLAKEHK.ToString());
		XmlElement xmlElement15 = xmlDocument.CreateElement("Info");
		xmlElement.AppendChild(xmlElement15);
		xmlElement15.SetAttribute("Value", ABOPCADKEJI.ToString());
		XmlElement xmlElement16 = xmlDocument.CreateElement("Counter");
		xmlElement.AppendChild(xmlElement16);
		xmlElement16.SetAttribute("Value", KAFIDINNDIP.ToString());
		XmlElement xmlElement17 = xmlDocument.CreateElement("FilePosition");
		xmlElement.AppendChild(xmlElement17);
		xmlElement17.SetAttribute("Value", HKJHIACNECB.ToString());
		XmlElement xmlElement18 = xmlDocument.CreateElement("PayLogPosition");
		xmlElement.AppendChild(xmlElement18);
		xmlElement18.SetAttribute("Value", LNIMCIMMJJG.ToString());
		if (!Directory.Exists(SF2Paths.LCDBGFFDKJB()))
		{
			Directory.CreateDirectory(SF2Paths.LCDBGFFDKJB());
		}
		xmlDocument.Save(JNJGGIGNFBE());
	}

	private void SaveFullLog()
	{
		try
		{
			FileInfo fileInfo = new FileInfo(ANOGPNAIFLI());
			if (!fileInfo.Exists)
			{
				fileInfo.Create().Close();
			}
			FileInfo fileInfo2 = new FileInfo(GCHJLCEFNMK());
			if (fileInfo2.Exists)
			{
				StreamWriter streamWriter = fileInfo.AppendText();
				StreamReader streamReader = fileInfo2.OpenText();
				while (!streamReader.EndOfStream)
				{
					streamWriter.WriteLine(streamReader.ReadLine());
				}
				streamWriter.Close();
				streamReader.Close();
				fileInfo2.Create().Close();
			}
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.Message);
		}
	}

	private void SaveFullLog(string MMEHKDAMJBF)
	{
		try
		{
			FileInfo fileInfo = new FileInfo(ANOGPNAIFLI());
			if (!fileInfo.Exists)
			{
				fileInfo.Create().Close();
			}
			using (StreamWriter streamWriter = fileInfo.AppendText())
			{
				streamWriter.Write(MMEHKDAMJBF);
			}
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.Message);
		}
	}

	private void GGGEHAGCLGC()
	{
		GGGEHAGCLGC(DCFPONJAING, GCHJLCEFNMK());
	}

	private void AGNMPDEFABN()
	{
		GGGEHAGCLGC(PHGGNFPBOKD, EFILFAHEAKB());
	}

	private void GGGEHAGCLGC(StringBuilder Data, string PDLAFCOODMM)
	{
		if (Data.Length == 0)
		{
			return;
		}
		string value = Data.ToString();
		Data.Length = 0;
		try
		{
			if (!Directory.Exists(SF2Paths.LCDBGFFDKJB()))
			{
				Directory.CreateDirectory(SF2Paths.LCDBGFFDKJB());
			}
			FileInfo fileInfo = new FileInfo(PDLAFCOODMM);
			if (!fileInfo.Exists)
			{
				FileStream fileStream = fileInfo.Create();
				fileStream.Close();
			}
			StreamWriter streamWriter = fileInfo.AppendText();
			if (fileInfo.Exists && fileInfo.Length != 0)
			{
				streamWriter.Write("\n");
			}
			streamWriter.Write(value);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.Message);
		}
	}

	public void LENBEPODJPC()
	{
		int pCOENEHCGNI = 2000000;
		GGGEHAGCLGC();
		SaveFullLog();
		Send(ANOGPNAIFLI(), ref DEBIFEKGFKD, (bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string MDIJEPEOAJH) =>
		{
			GBNKIIILEGI(AMKKLMOONEP, GHDPPHAAPCA, JHJDJOFPHPH, MDIJEPEOAJH);
		}, pCOENEHCGNI, false, "save_full_json_log");
	}

	public void GBNKIIILEGI(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string CCNACAJIIGA)
	{
		if (AMKKLMOONEP)
		{
			JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
			if (jSONNode != null && jSONNode["data"] != null && jSONNode["data"].Value == "ok")
			{
				SendOnNextFrame(LENBEPODJPC);
			}
		}
	}

	private void OPGFPNFAOLH()
	{
		if (ACNHHFKLKID != NBGNCCPMCCD.NotLogging && ACNHHFKLKID != NBGNCCPMCCD.Undecided)
		{
			int pCOENEHCGNI = 2000000;
			Send(EFILFAHEAKB(), ref LNIMCIMMJJG, (bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string MDIJEPEOAJH) =>
			{
				ADFDKDNHONL(AMKKLMOONEP, GHDPPHAAPCA, JHJDJOFPHPH, MDIJEPEOAJH);
			}, pCOENEHCGNI, false, "save_pay_log");
		}
	}

	public void ADFDKDNHONL(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string CCNACAJIIGA)
	{
		if (AMKKLMOONEP)
		{
			JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
			if (jSONNode != null && jSONNode["data"] != null && jSONNode["data"].Value == "ok")
			{
				LJBLDJPLMPL();
				SendOnNextFrame(OPGFPNFAOLH);
			}
		}
	}

	private void LJBLDJPLMPL()
	{
		AGNMPDEFABN();
		FileInfo fileInfo = new FileInfo(EFILFAHEAKB());
		if (fileInfo.Exists)
		{
			bool flag = false;
			StreamReader streamReader = fileInfo.OpenText();
			flag = LNIMCIMMJJG >= streamReader.BaseStream.Length;
			streamReader.Close();
			if (flag)
			{
				fileInfo.Create().Close();
				LNIMCIMMJJG = 0L;
				AHOPPPNPOHB();
			}
		}
	}

	private void Send()
	{
		int pCOENEHCGNI = 2000000;
		if (COLCBNPNLPG != NBGNCCPMCCD.NotLogging && COLCBNPNLPG != NBGNCCPMCCD.Undecided)
		{
			PFGKFHJMMFH = FAEJIAODPEA.IKJMBCFLHMC();
			GGGEHAGCLGC();
			Send(GCHJLCEFNMK(), ref HKJHIACNECB, (bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string MDIJEPEOAJH) =>
			{
				EBPNOCPEOFN(AMKKLMOONEP, GHDPPHAAPCA, JHJDJOFPHPH, MDIJEPEOAJH);
			}, pCOENEHCGNI);
		}
	}

	private void Send(string EFGLOMANJHN, ref long DNGJNMNHIOB, Action<bool, string, object, string> p_delegate, int PCOENEHCGNI = 2000000, bool AOOKEDHEDHJ = true, string IBODMPMJELJ = "save_json_log")
	{
		FileInfo fileInfo = new FileInfo(EFGLOMANJHN);
		if (!fileInfo.Exists)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			StreamReader streamReader = fileInfo.OpenText();
			streamReader.BaseStream.Seek(DNGJNMNHIOB, SeekOrigin.Begin);
			while (stringBuilder.Length < PCOENEHCGNI && !streamReader.EndOfStream)
			{
				stringBuilder.Append(streamReader.ReadLine());
				stringBuilder.Append("\n");
			}
			DNGJNMNHIOB = streamReader.BaseStream.Position;
			streamReader.Close();
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.Message);
		}
		if (AOOKEDHEDHJ)
		{
			AHOPPPNPOHB();
		}
		if (stringBuilder.Length == 0)
		{
			p_delegate(false, null, null, null);
			return;
		}
		string logResultStr = stringBuilder.ToString();
		ServerProvider.get_Instance().SaveJsonLogAction(logResultStr, (bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH) =>
		{
			p_delegate(AMKKLMOONEP, GHDPPHAAPCA, JHJDJOFPHPH, logResultStr);
		}, IBODMPMJELJ);
	}

	public void EBPNOCPEOFN(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH, string CCNACAJIIGA)
	{
		if (AMKKLMOONEP)
		{
			JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
			if (jSONNode != null && jSONNode["data"] != null && jSONNode["data"].Value == "ok")
			{
				SaveFullLog(CCNACAJIIGA);
				FHOBBAHNNCL();
			}
		}
	}

	private void FHOBBAHNNCL()
	{
		GGGEHAGCLGC();
		FileInfo fileInfo = new FileInfo(GCHJLCEFNMK());
		if (fileInfo.Exists)
		{
			bool flag = false;
			StreamReader streamReader = fileInfo.OpenText();
			flag = HKJHIACNECB >= streamReader.BaseStream.Length;
			streamReader.Close();
			if (flag)
			{
				fileInfo.Create().Close();
				HKJHIACNECB = 0L;
				AHOPPPNPOHB();
			}
			else
			{
				SendOnNextFrame(Send);
			}
		}
	}

	private void SendOnNextFrame(Action IBODMPMJELJ)
	{
		ServerProvider.get_Instance().StartCoroutine(SendOnNextFrameCorutine(IBODMPMJELJ));
	}

	private IEnumerator SendOnNextFrameCorutine(Action IBODMPMJELJ)
	{
		yield return new WaitForEndOfFrame();
		IBODMPMJELJ();
	}

	private void NLMGNONKMBN()
	{
		if (!File.Exists(GCHJLCEFNMK()))
		{
			return;
		}
		string[] array = File.ReadAllLines(GCHJLCEFNMK());
		if (array.Length <= 200)
		{
			return;
		}
		int count = array.Length - 200;
		IEnumerable<string> enumerable = array.Skip(count);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in enumerable)
		{
			stringBuilder.AppendLine(item);
		}
		stringBuilder.Length--;
		File.WriteAllText(GCHJLCEFNMK(), stringBuilder.ToString());
		PFGKFHJMMFH = FAEJIAODPEA.IKJMBCFLHMC();
	}

	public void OnTimerTick(object data)
	{
		BLLDCBKNIJA++;
	}

	public void FFMHKLENGLP(bool FILCEHABKLK)
	{
		if (!FILCEHABKLK && FAEJIAODPEA.IKJMBCFLHMC() - PFGKFHJMMFH > 900000)
		{
			Send();
		}
		if (FILCEHABKLK)
		{
			CHHJENPJGDP();
		}
	}

	private void CHHJENPJGDP()
	{
		EKFPAJCKJLB = GlobalTimer.get_LocalTimeUTC();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
			BPGFOAHGAHE = ListSF.CCDKHLAMKKO().EHFJHFDACMP();
			FIGDGGKLFDG = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			ONLODELNKPO = ListSF.CCDKHLAMKKO().BFBOEGMAMNF();
			_EndXp = ListSF.CCDKHLAMKKO().EOKLELGLHJJ();
			JBNACLHHBPA = ListSF.CCDKHLAMKKO().NHKMGNPADKI();
		}
		LCNJJLAKEHK = "0";
		ABOPCADKEJI = string.Empty;
		KAFIDINNDIP = 0;
		AHOPPPNPOHB();
	}
}
