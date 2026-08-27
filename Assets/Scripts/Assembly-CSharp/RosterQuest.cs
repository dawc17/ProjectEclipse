using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class RosterQuest
{
	public class NOKCOAHJIPB
	{
		public string Name = string.Empty;

		public string Value = string.Empty;

		public XmlNode Node;

		public NOKCOAHJIPB(string _name, string _value)
		{
			Name = _name;
			Value = _value;
		}

		public NOKCOAHJIPB(XmlNode PKHDLOGJKAD)
		{
			Node = PKHDLOGJKAD;
			Name = Node.Attributes["Name"].CIPOICEEIBK(string.Empty);
			Value = Node.Attributes["Value"].CIPOICEEIBK(string.Empty);
		}

		public void MCPIOGALBMK(string PKHDLOGJKAD)
		{
			Value = PKHDLOGJKAD;
			Node.Attributes["Value"].Value = Value;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ParametersQuest AIIJHKIPJDD;

	public string Name;

	public string FileName;

	public string Type;

	public XmlNode Node;

	public bool PLNNKKBPDJK;

	public List<NOKCOAHJIPB> FNGDAJJJJJD = new List<NOKCOAHJIPB>();

	public ParametersQuest KMMJCHDKBDO
	{
		get
		{
			return get_Parameters();
		}
		private set
		{
			HDIDFDHJENJ(value);
		}
	}

	public RosterQuest(XmlNode value)
	{
		Node = value;
		FileName = "quests.xml";
		bool flag = false;
		if (Node.Attributes["FileName"] != null)
		{
			string iFKJHHPJPLP = Node.Attributes["FileName"].CIPOICEEIBK(string.Empty);
			flag = DirectoryController.IsPathWithDrive(FileName);
			iFKJHHPJPLP = DirectoryController.BAANOCLBLKM(iFKJHHPJPLP);
			FileName = DirectoryController.BECKNKJNFJB(iFKJHHPJPLP);
		}
		if (flag)
		{
			SetFileName(FileName);
		}
		PLNNKKBPDJK = false;
		Name = Node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Type = Node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		HDIDFDHJENJ(null);
		NPICECAECDI();
	}

	public ParametersQuest get_Parameters()
	{
		return AIIJHKIPJDD;
	}

	private void HDIDFDHJENJ(ParametersQuest value)
	{
		AIIJHKIPJDD = value;
	}

	public void NPICECAECDI()
	{
		XmlNode xmlNode = Node["QuestParameters"];
		if (xmlNode != null)
		{
			HDIDFDHJENJ(new ParametersQuest(xmlNode));
		}
	}

	public void ECGFFBHMIIK(object data, int AAKAPLGDGNM, int ILNNINKHPOC)
	{
		if (get_Parameters() == null)
		{
			XmlNode pKHDLOGJKAD = Node.ACBPMPMPKJJ("QuestParameters");
			HDIDFDHJENJ(new ParametersQuest(pKHDLOGJKAD));
		}
		QuestParameters hHKLFIIBIFF = (QuestParameters)data;
		get_Parameters().IBMNACPGMLL(ILNNINKHPOC);
		get_Parameters().MNNPHOAEMII(AAKAPLGDGNM);
		get_Parameters().ELKOGHKIDOG((hHKLFIIBIFF.LBGOMJFFEPP() == null) ? string.Empty : hHKLFIIBIFF.LBGOMJFFEPP().BCKFACGMOKC.ToString());
		get_Parameters().AJBMLOLOFAN(hHKLFIIBIFF.HEIADONEACH);
		get_Parameters().CPONINMPIJL(hHKLFIIBIFF.AIEHNBBFNPF);
		get_Parameters().EFIFIPKDMIN(hHKLFIIBIFF.BJIDALJIKNC);
		get_Parameters().MPFIPAANJON(hHKLFIIBIFF.JNGFNNFAAGN);
		get_Parameters().set_FightAvgFPS(hHKLFIIBIFF.fightAvgFps);
	}

	public void SetFileName(string _fileName)
	{
		_fileName = DirectoryController.BAANOCLBLKM(_fileName);
		if (Node.Attributes["FileName"] == null)
		{
			Node.LLIKNHNLGJJ("FileName");
		}
		Node.Attributes["FileName"].Value = _fileName;
		FileName = _fileName;
	}

	public void LCIHKPPGNPF()
	{
		XmlElement xmlElement = Node["QuestParameters"];
		if (xmlElement != null)
		{
			Node.RemoveChild(xmlElement);
		}
		if (get_Parameters() != null)
		{
			HDIDFDHJENJ(null);
		}
	}

	public int ELBKKOPHLHK()
	{
		return (get_Parameters() != null) ? get_Parameters().EDADICNDCKK() : 0;
	}

	public int NGGNCHPDOOI()
	{
		return (get_Parameters() != null) ? get_Parameters().CIDMJEKCDMP() : 0;
	}
}
