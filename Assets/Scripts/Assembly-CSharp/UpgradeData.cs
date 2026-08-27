using System;
using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class UpgradeData : IComparable<UpgradeData>
{
	public struct ONOBKNJPKIP
	{
		public bool EHKNIKHPGDN;

		public bool KLHOKKPALOK;

		public bool KFIAHDNGHMI;

		public bool MDAAJFBENON;

		public bool FMHECGHHKGB;

		public bool Level;

		public bool AKKLOMFOLNO;

		public bool ICDIEHCJBGA;
	}

	public struct AGKOBJMBAEC
	{
		public Attributes IBLHIAHECLK;

		public ObscuredLong KLHOKKPALOK;

		public ObscuredLong FMHECGHHKGB;

		public ObscuredLong MDAAJFBENON;

		public string KFIAHDNGHMI;

		public long EHKNIKHPGDN;

		public int Level;

		public int AKKLOMFOLNO;

		public int ICDIEHCJBGA;
	}

	public AGKOBJMBAEC OGLHOJNMEBD;

	public ONOBKNJPKIP EBHOFBFKNMB;

	public int UpgradeIndex;

	public UpgradeData(XmlNode node, string LFLGCDNKNJI)
	{
		EBHOFBFKNMB.KLHOKKPALOK = false;
		EBHOFBFKNMB.FMHECGHHKGB = false;
		EBHOFBFKNMB.EHKNIKHPGDN = false;
		EBHOFBFKNMB.Level = false;
		EBHOFBFKNMB.ICDIEHCJBGA = false;
		EBHOFBFKNMB.MDAAJFBENON = false;
		EBHOFBFKNMB.AKKLOMFOLNO = false;
		EBHOFBFKNMB.KFIAHDNGHMI = false;
		OGLHOJNMEBD.KLHOKKPALOK = (ObscuredLong)(0L);
		OGLHOJNMEBD.FMHECGHHKGB = (ObscuredLong)(0L);
		OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(0L);
		OGLHOJNMEBD.EHKNIKHPGDN = 0L;
		OGLHOJNMEBD.Level = 0;
		OGLHOJNMEBD.ICDIEHCJBGA = 0;
		OGLHOJNMEBD.AKKLOMFOLNO = 0;
		OGLHOJNMEBD.KFIAHDNGHMI = LFLGCDNKNJI;
		OGLHOJNMEBD.IBLHIAHECLK = new Attributes();
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			XmlAttribute xmlAttribute = node.Attributes[item.get_Name()];
			if (xmlAttribute != null)
			{
				OGLHOJNMEBD.IBLHIAHECLK.Set(item.get_Name(), xmlAttribute.ParseInt());
			}
		}
		XmlAttribute xmlAttribute2 = node.Attributes["DeliveryTime"];
		if (xmlAttribute2 != null)
		{
			EBHOFBFKNMB.EHKNIKHPGDN = true;
			OGLHOJNMEBD.EHKNIKHPGDN = xmlAttribute2.ParseLong(0L);
		}
		XmlAttribute xmlAttribute3 = node.Attributes["BonusDeliveryPrice"];
		if (xmlAttribute3 != null)
		{
			EBHOFBFKNMB.KLHOKKPALOK = true;
			OGLHOJNMEBD.KLHOKKPALOK = (ObscuredLong)(xmlAttribute3.ParseLong(0L));
		}
		EBHOFBFKNMB.KFIAHDNGHMI = true;
		OGLHOJNMEBD.KFIAHDNGHMI = LFLGCDNKNJI;
		XmlAttribute xmlAttribute4 = node.Attributes["Price"];
		if (xmlAttribute4 != null)
		{
			EBHOFBFKNMB.MDAAJFBENON = true;
			OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(xmlAttribute4.ParseLong(0L));
		}
		XmlAttribute xmlAttribute5 = node.Attributes["BonusPrice"];
		if (xmlAttribute5 != null)
		{
			EBHOFBFKNMB.FMHECGHHKGB = true;
			OGLHOJNMEBD.FMHECGHHKGB = (ObscuredLong)(xmlAttribute5.ParseLong(0L));
		}
		XmlAttribute xmlAttribute6 = node.Attributes["Level"];
		if (xmlAttribute6 != null)
		{
			EBHOFBFKNMB.Level = true;
			OGLHOJNMEBD.Level = xmlAttribute6.ParseInt();
		}
		XmlAttribute xmlAttribute7 = node.Attributes["UpgradeLevel"];
		if (xmlAttribute7 != null)
		{
			EBHOFBFKNMB.AKKLOMFOLNO = true;
			OGLHOJNMEBD.AKKLOMFOLNO = xmlAttribute7.ParseInt();
		}
		XmlAttribute xmlAttribute8 = node.Attributes["Milestone"];
		if (xmlAttribute8 != null)
		{
			EBHOFBFKNMB.ICDIEHCJBGA = true;
			OGLHOJNMEBD.ICDIEHCJBGA = xmlAttribute8.ParseInt();
		}
	}

	public UpgradeData(UpgradeData NOLFMPDGCOC)
	{
		EBHOFBFKNMB.KLHOKKPALOK = NOLFMPDGCOC.EBHOFBFKNMB.KLHOKKPALOK;
		EBHOFBFKNMB.FMHECGHHKGB = NOLFMPDGCOC.EBHOFBFKNMB.FMHECGHHKGB;
		EBHOFBFKNMB.EHKNIKHPGDN = NOLFMPDGCOC.EBHOFBFKNMB.EHKNIKHPGDN;
		EBHOFBFKNMB.Level = NOLFMPDGCOC.EBHOFBFKNMB.Level;
		EBHOFBFKNMB.ICDIEHCJBGA = NOLFMPDGCOC.EBHOFBFKNMB.ICDIEHCJBGA;
		EBHOFBFKNMB.MDAAJFBENON = NOLFMPDGCOC.EBHOFBFKNMB.MDAAJFBENON;
		EBHOFBFKNMB.AKKLOMFOLNO = NOLFMPDGCOC.EBHOFBFKNMB.AKKLOMFOLNO;
		EBHOFBFKNMB.KFIAHDNGHMI = NOLFMPDGCOC.EBHOFBFKNMB.KFIAHDNGHMI;
		OGLHOJNMEBD.IBLHIAHECLK = NOLFMPDGCOC.OGLHOJNMEBD.IBLHIAHECLK;
		OGLHOJNMEBD.KLHOKKPALOK = NOLFMPDGCOC.OGLHOJNMEBD.KLHOKKPALOK;
		OGLHOJNMEBD.FMHECGHHKGB = NOLFMPDGCOC.OGLHOJNMEBD.FMHECGHHKGB;
		OGLHOJNMEBD.EHKNIKHPGDN = NOLFMPDGCOC.OGLHOJNMEBD.EHKNIKHPGDN;
		OGLHOJNMEBD.Level = NOLFMPDGCOC.OGLHOJNMEBD.Level;
		OGLHOJNMEBD.ICDIEHCJBGA = NOLFMPDGCOC.OGLHOJNMEBD.ICDIEHCJBGA;
		OGLHOJNMEBD.MDAAJFBENON = NOLFMPDGCOC.OGLHOJNMEBD.MDAAJFBENON;
		OGLHOJNMEBD.AKKLOMFOLNO = NOLFMPDGCOC.OGLHOJNMEBD.AKKLOMFOLNO;
		OGLHOJNMEBD.KFIAHDNGHMI = NOLFMPDGCOC.OGLHOJNMEBD.KFIAHDNGHMI;
	}

	public int CompareTo(UpgradeData NOLFMPDGCOC)
	{
		return (OGLHOJNMEBD.AKKLOMFOLNO >= NOLFMPDGCOC.OGLHOJNMEBD.AKKLOMFOLNO) ? 1 : (-1);
	}

	public void RandomizeObscuredVars()
	{
		OGLHOJNMEBD.KLHOKKPALOK.GMCADPGOCHM();
		OGLHOJNMEBD.FMHECGHHKGB.GMCADPGOCHM();
		OGLHOJNMEBD.MDAAJFBENON.GMCADPGOCHM();
	}
}
