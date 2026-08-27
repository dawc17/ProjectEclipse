using System;
using System.Xml;

public class Trick : IComparable<Trick>
{
	public string NHKMCLPOMFK;

	public string Name;

	public string COJPEGLPGDF;

	public string HIAMFGEIGDP;

	public int Rank;

	public bool IsNew;

	public InfoAnimation KJHMOGGECBN;

	public Trick(XmlNode BHBHAOJHABE, InfoAnimation KJHGIKMFJOB)
	{
		NHKMCLPOMFK = BHBHAOJHABE.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		Rank = BHBHAOJHABE.Attributes["Rank"].ParseInt();
		COJPEGLPGDF = BHBHAOJHABE.Attributes["KeysDescription"].CIPOICEEIBK(string.Empty);
		HIAMFGEIGDP = BHBHAOJHABE.Attributes["EffectDescription"].CIPOICEEIBK(string.Empty);
		Name = KJHGIKMFJOB.Name;
		KJHMOGGECBN = KJHGIKMFJOB;
		IsNew = false;
	}

	public Trick(string NCKCDCODNHA, string _name, InfoAnimation KJHGIKMFJOB, int HEIBENBPNLN, string PHDCIEGEKBC, string LDKAELDNKGH)
	{
		NHKMCLPOMFK = NCKCDCODNHA;
		Name = _name;
		KJHMOGGECBN = KJHGIKMFJOB;
		Rank = HEIBENBPNLN;
		COJPEGLPGDF = PHDCIEGEKBC;
		HIAMFGEIGDP = LDKAELDNKGH;
		IsNew = false;
	}

	public static bool Compare(Trick KOOLDHKJHNH, Trick MHFCMOONCHB)
	{
		return KOOLDHKJHNH.Rank < MHFCMOONCHB.Rank;
	}

	public int CompareTo(Trick NOLFMPDGCOC)
	{
		return Rank.CompareTo(NOLFMPDGCOC.Rank);
	}
}
