using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

public class RpnParser
{
	public enum BLELIIJLLEB
	{
		Operator = 0,
		Operand = 1
	}

	public enum NKMIEOOPJHP
	{
		None = 0,
		Constant = 1,
		Variable = 2
	}

	public delegate double POMKCMMOEGH(List<double> arguments);

	public delegate object PHNLIHEJEPK();

	public delegate object ParameterDelegate(List<object> BPLIHEIIBFP);

	public enum EGLAAMKIAHG
	{
		OperatorAddArgCount = 2,
		OperatorSubArgCount = OperatorAddArgCount,
		OperatorMultArgCount = OperatorAddArgCount,
		OperatorDivArgCount = OperatorAddArgCount,
		OperatorModArgCount = OperatorAddArgCount,
		OperatorPowArgCount = OperatorAddArgCount,
		OperatorRootArgCount = OperatorAddArgCount,
		OperatorBrackLArgCount = 0,
		OperatorBrackRArgCount = OperatorBrackLArgCount,
		OperatorSinArgCount = 1,
		OperatorCosArgCount = OperatorSinArgCount,
		OperatorMaxArgCount = OperatorAddArgCount,
		OperatorMinArgCount = OperatorAddArgCount,
		OperatorSqrtArgCount = OperatorSinArgCount,
		OperatorAbsArgCount = OperatorSinArgCount,
		OperatorExpArgCount = OperatorSinArgCount,
		OperatorLnArgCount = OperatorSinArgCount,
		OperatorLgArgCount = OperatorSinArgCount,
		OperatorLogArgCount = OperatorAddArgCount,
		OperatorComparisonArgCount = OperatorAddArgCount
	}

	public enum DMMCIDPIFMK
	{
		OperatorAddPrior = 1,
		OperatorSubPrior = OperatorAddPrior,
		OperatorMultPrior = 2,
		OperatorDivPrior = OperatorMultPrior,
		OperatorModPrior = OperatorMultPrior,
		OperatorPowPrior = 3,
		OperatorSqrtPrior = OperatorPowPrior,
		OperatorRootPrior = OperatorPowPrior,
		OperatorBrackLPrior = 0,
		OperatorBrackRPrior = OperatorBrackLPrior,
		OperatorSinPrior = 4,
		OperatorCosPrior = OperatorSinPrior,
		OperatorMaxPrior = OperatorSinPrior,
		OperatorMinPrior = OperatorSinPrior,
		OperatorAbsPrior = OperatorSinPrior,
		OperatorLnPrior = OperatorSinPrior,
		OperatorLgPrior = OperatorSinPrior,
		OperatorLogPrior = OperatorSinPrior,
		OperatorExpPrior = OperatorSinPrior,
		OperatorComparisonPrior = OperatorSinPrior
	}

	public enum DPEHBGCOPJM
	{
		DirectionRight = 0,
		DirectionLeft = 1
	}

	public enum IKHBAIDMOHC
	{
		OperatorAddDirect = 0,
		OperatorSubDirect = OperatorAddDirect,
		OperatorMultDirect = OperatorAddDirect,
		OperatorDivDirect = OperatorAddDirect,
		OperatorModDirect = OperatorAddDirect,
		OperatorPowDirect = 1,
		OperatorRootDirect = OperatorAddDirect,
		OperatorBrackLDirect = OperatorAddDirect,
		OperatorBrackRDirect = OperatorAddDirect,
		OperatorSinDirect = OperatorPowDirect,
		OperatorCosDirect = OperatorPowDirect,
		OperatorMaxDirect = OperatorPowDirect,
		OperatorMinDirect = OperatorPowDirect,
		OperatorAbsDirect = OperatorPowDirect,
		OperatorSqrtDirect = OperatorPowDirect,
		OperatorExpDirect = OperatorPowDirect,
		OperatorLnDirect = OperatorPowDirect,
		OperatorLgDirect = OperatorPowDirect,
		OperatorLogDirect = OperatorPowDirect,
		OperatorComparisonDirect = OperatorAddDirect
	}

	private class IMOIKMFBGDH
	{
		public DMMCIDPIFMK NNCOAODDCOD;

		public POMKCMMOEGH GDLNLMPIKMO;

		public EGLAAMKIAHG FABCLOGBCJM;

		public IKHBAIDMOHC JBBJEBCAAEG;

		public IMOIKMFBGDH(DMMCIDPIFMK DBNEBOIBILM, POMKCMMOEGH NGOJAJIKFBA, EGLAAMKIAHG MPOEHCOADGE, IKHBAIDMOHC PCBCFHJBODO)
		{
			NNCOAODDCOD = DBNEBOIBILM;
			GDLNLMPIKMO = NGOJAJIKFBA;
			FABCLOGBCJM = MPOEHCOADGE;
			JBBJEBCAAEG = PCBCFHJBODO;
		}

		public static double ICPPFFICMJH(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] + arguments[1];
		}

		public static double BPBNMAPGJAE(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] - arguments[1];
		}

		public static double BEOCEMLPKDD(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return arguments[0] * arguments[1];
		}

		public static double JONIACGMPPF(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			if (arguments[1] != 0.0)
			{
				return arguments[0] / arguments[1];
			}
			return 0.0;
		}

		public static double EEMILHABJID(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (int)arguments[0] % (int)arguments[1];
		}

		public static double IPCEIGHPABJ(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Pow(arguments[0], arguments[1]);
		}

		public static double AIJAPKDMBFD(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Convert.ToDouble(Convert.ToBoolean(arguments[0]) | Convert.ToBoolean(arguments[1]));
		}

		public static double CDANHLLHEMG(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Sin(arguments[0]);
		}

		public static double PBMKAPABCFN(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Cos(arguments[0]);
		}

		public static double ANEAGFECEFM(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Max(arguments[0], arguments[1]);
		}

		public static double NKHACLLAANI(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Min(arguments[0], arguments[1]);
		}

		public static double JCPIFEGOAFN(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Sqrt(arguments[0]);
		}

		public static double PDEFPKGHIMD(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Abs(arguments[0]);
		}

		public static double IIAKACBAOIB(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Log(arguments[0], Math.E);
		}

		public static double JHEBIJIIFHE(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Log10(arguments[0]);
		}

		public static double CIDBJOJNPIO(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return Math.Log(arguments[0], arguments[1]);
		}

		public static double DOJHPLNIEKJ(List<double> arguments)
		{
			if (arguments.Count != 1)
			{
				return 0.0;
			}
			return Math.Exp(arguments[0]);
		}

		public static double GKDIKKHOAPH(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] > arguments[1]) ? 1 : 0;
		}

		public static double ODGIFGPIGEK(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] < arguments[1]) ? 1 : 0;
		}

		public static double PBCFPODKOLL(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] >= arguments[1]) ? 1 : 0;
		}

		public static double JPNDCHMCBAL(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] <= arguments[1]) ? 1 : 0;
		}

		public static double LLLCNFLNBIO(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] == arguments[1]) ? 1 : 0;
		}

		public static double LGIIDGAEAGN(List<double> arguments)
		{
			if (arguments.Count != 2)
			{
				return 0.0;
			}
			return (arguments[0] != arguments[1]) ? 1 : 0;
		}
	}

	private class FAOBBBMHEBL
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private NKMIEOOPJHP KAHHEBMBCFA;

		public FAOBBBMHEBL()
		{
			set_Type(NKMIEOOPJHP.None);
		}

		public NKMIEOOPJHP get_Type()
		{
			return KAHHEBMBCFA;
		}

		public void set_Type(NKMIEOOPJHP value)
		{
			KAHHEBMBCFA = value;
		}

		public virtual object OAGPELOHACM()
		{
			return null;
		}
	}

	private class ANCMPDAEGDN : FAOBBBMHEBL
	{
		private object _value;

		public ANCMPDAEGDN(object value)
		{
			_value = value;
			set_Type(NKMIEOOPJHP.Constant);
		}

		public override object OAGPELOHACM()
		{
			return _value;
		}
	}

	private class AAGFMFHOGDN : FAOBBBMHEBL
	{
		private PHNLIHEJEPK CDIHDPDEOOF;

		public AAGFMFHOGDN(PHNLIHEJEPK NFDJONMIEFL)
		{
			CDIHDPDEOOF = NFDJONMIEFL;
			set_Type(NKMIEOOPJHP.Variable);
		}

		public override object OAGPELOHACM()
		{
			return CDIHDPDEOOF();
		}
	}

	private class KPOPDKMFMNK : FAOBBBMHEBL
	{
		private ParameterDelegate KCNIKBNOJKN;

		private List<FAOBBBMHEBL> DLNJFPNNBKO;

		public KPOPDKMFMNK(List<FAOBBBMHEBL> arguments, ParameterDelegate JKAELOIBLFJ)
		{
			DLNJFPNNBKO = arguments;
			KCNIKBNOJKN = JKAELOIBLFJ;
		}

		public override object OAGPELOHACM()
		{
			List<object> list = new List<object>(1);
			foreach (FAOBBBMHEBL item in DLNJFPNNBKO)
			{
				list.Add(item.OAGPELOHACM());
			}
			return KCNIKBNOJKN(list);
		}
	}

	private class BAELOMEILMK
	{
		public BLELIIJLLEB LFLGCDNKNJI;

		public IMOIKMFBGDH IMGAKPOBGBP;

		public FAOBBBMHEBL LMENIMNKNHP;
	}

	public class Formula
	{
		private List<BAELOMEILMK> _items = new List<BAELOMEILMK>();

		public int DOOJLEBPJCK
		{
			get
			{
				return OJEHEKMJJBL();
			}
		}

		public Formula(string HBICLHKEIEI)
		{
			if (!_isInited)
			{
				throw new Exception("RpnParser is not inited!");
			}
			_items = OCJPEOLCPCC(HBICLHKEIEI);
		}

		public int OJEHEKMJJBL()
		{
			return _items.FindAll((BAELOMEILMK DHDMNHCIPEH) => DHDMNHCIPEH.LMENIMNKNHP != null && DHDMNHCIPEH.LMENIMNKNHP.get_Type() == NKMIEOOPJHP.Variable).Count;
		}

		public object ODHJHHMEEOI()
		{
			return RpnParser.ODHJHHMEEOI(_items);
		}
	}

	private static Dictionary<string, IMOIKMFBGDH> JHNBIMFAIMP;

	private static Dictionary<string, PHNLIHEJEPK> FNBONMPOPKH;

	private static Dictionary<string, ParameterDelegate> JNNPIBLANAD;

	private static bool _isInited;

	private const string HEDFGBBGEBL = "+-*^()|/&#";

	public const char MOCCDHDIKPB = '?';

	public const char MOIFICCGIMN = '.';

	public const char BAMMMBAOMFA = '$';

	public const char GEFMPKBNGJA = ',';

	public static void init(Dictionary<string, PHNLIHEJEPK> PPEABEJMCPI, Dictionary<string, ParameterDelegate> GIOGAJGIGMO)
	{
		if (!_isInited)
		{
			GOALNBNMFKH();
			FNBONMPOPKH = PPEABEJMCPI;
			JNNPIBLANAD = GIOGAJGIGMO;
			_isInited = true;
		}
	}

	private static void GOALNBNMFKH()
	{
		if (JHNBIMFAIMP == null)
		{
			JHNBIMFAIMP = new Dictionary<string, IMOIKMFBGDH>(18);
			JHNBIMFAIMP["+"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorAddPrior, IMOIKMFBGDH.ICPPFFICMJH, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["-"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorAddPrior, IMOIKMFBGDH.BPBNMAPGJAE, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["*"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorMultPrior, IMOIKMFBGDH.BEOCEMLPKDD, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["/"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorMultPrior, IMOIKMFBGDH.JONIACGMPPF, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["%"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorMultPrior, IMOIKMFBGDH.EEMILHABJID, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["^"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorPowPrior, IMOIKMFBGDH.IPCEIGHPABJ, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["|"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorPowPrior, IMOIKMFBGDH.AIJAPKDMBFD, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["("] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorBrackLPrior, null, EGLAAMKIAHG.OperatorBrackLArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP[")"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorBrackLPrior, null, EGLAAMKIAHG.OperatorBrackLArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["sin"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.CDANHLLHEMG, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["cos"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.PBMKAPABCFN, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["max"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.ANEAGFECEFM, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["min"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.NKHACLLAANI, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["pow"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorPowPrior, IMOIKMFBGDH.IPCEIGHPABJ, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["sqrt"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorPowPrior, IMOIKMFBGDH.JCPIFEGOAFN, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["abs"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.PDEFPKGHIMD, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["ln"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.IIAKACBAOIB, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["lg"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.JHEBIJIIFHE, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["log"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.CIDBJOJNPIO, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP["exp"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.DOJHPLNIEKJ, EGLAAMKIAHG.OperatorSinArgCount, IKHBAIDMOHC.OperatorPowDirect);
			JHNBIMFAIMP[">"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.GKDIKKHOAPH, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["<"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.ODGIFGPIGEK, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["=>"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.PBCFPODKOLL, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["=<"] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.JPNDCHMCBAL, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["=="] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.LLLCNFLNBIO, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
			JHNBIMFAIMP["!="] = new IMOIKMFBGDH(DMMCIDPIFMK.OperatorSinPrior, IMOIKMFBGDH.LGIIDGAEAGN, EGLAAMKIAHG.OperatorAddArgCount, IKHBAIDMOHC.OperatorAddDirect);
		}
	}

	private static List<BAELOMEILMK> OCJPEOLCPCC(string DPABILBDPFF)
	{
		List<BAELOMEILMK> list = new List<BAELOMEILMK>();
		if (DPABILBDPFF == string.Empty)
		{
			throw new Exception("Formula can not be empty");
		}
		DPABILBDPFF = DPCKBFHACAC(DPABILBDPFF);
		DPABILBDPFF = OAFPFEKLOFO(DPABILBDPFF);
		List<BAELOMEILMK> list2 = LMGMFLGHNGJ(DPABILBDPFF);
		if (!KDHHNEOBBCA(list2))
		{
			throw new Exception("Formula can not be empty");
		}
		list = CNOJALPFLLC(list2);
		if (list.Count == 0)
		{
			throw new Exception("Formula has no items");
		}
		return list;
	}

	private static object ODHJHHMEEOI(List<BAELOMEILMK> HELFDCAIJNE)
	{
		if (HELFDCAIJNE == null || HELFDCAIJNE.Count == 0)
		{
			// A newer perk can resolve entirely to unsupported runtime parameters.
			// Treat that expression as zero instead of breaking combat every hit.
			return 0.0;
		}
		if (HELFDCAIJNE.Count == 1 && HELFDCAIJNE[0].LFLGCDNKNJI == BLELIIJLLEB.Operand)
		{
			return HELFDCAIJNE[0].LMENIMNKNHP.OAGPELOHACM();
		}
		List<object> list = new List<object>(2);
		for (int i = 0; i != HELFDCAIJNE.Count; i++)
		{
			if (HELFDCAIJNE[i].LFLGCDNKNJI == BLELIIJLLEB.Operand)
			{
				list.Add(HELFDCAIJNE[i].LMENIMNKNHP.OAGPELOHACM());
			}
			else
			{
				if (HELFDCAIJNE[i].LFLGCDNKNJI != BLELIIJLLEB.Operator)
				{
					continue;
				}
				List<double> list2 = new List<double>();
				int num = (int)(list.Count - HELFDCAIJNE[i].IMGAKPOBGBP.FABCLOGBCJM);
				if (num < 0)
				{
					return 0.0;
				}
				for (int j = num; j < list.Count; j++)
				{
					double result = 0.0;
					if (!double.TryParse(list[j].ToString(), out result))
					{
						throw new Exception("Double expected because there are math operators");
					}
					list2.Add(result);
				}
				double num2 = HELFDCAIJNE[i].IMGAKPOBGBP.GDLNLMPIKMO(list2);
				list.RemoveRange(num, (int)HELFDCAIJNE[i].IMGAKPOBGBP.FABCLOGBCJM);
				list.Add(num2);
			}
		}
		return ((list.Count == 0) ? 0.0 : list[0]);
	}

	private static string OAFPFEKLOFO(string DPABILBDPFF)
	{
		for (int i = 0; i < DPABILBDPFF.Length; i++)
		{
			if ((DPABILBDPFF[i] == '-' || DPABILBDPFF[i] == '+') && (i == 0 || DPABILBDPFF[i - 1] == '('))
			{
				DPABILBDPFF = DPABILBDPFF.Insert(i, "0");
			}
		}
		return DPABILBDPFF;
	}

	private static string DPCKBFHACAC(string DPABILBDPFF)
	{
		return DPABILBDPFF.Replace(" ", string.Empty);
	}

	public static bool MHPHPJEMDNH(string symbol)
	{
		if (!"+-*^()|/&#".Contains(symbol))
		{
			return false;
		}
		return true;
	}

	public static bool MHPHPJEMDNH(char symbol)
	{
		return "+-*^()|/&#".IndexOf(symbol) != -1;
	}

	private static bool EEEMOIBDBEA(string IGGFGLLIGCG)
	{
		string key = IGGFGLLIGCG.ToLower();
		return JHNBIMFAIMP.ContainsKey(key);
	}

	private static bool MBEKDNOJCOP(string IGGFGLLIGCG)
	{
		if (IGGFGLLIGCG[0] == '?')
		{
			return true;
		}
		foreach (char c in IGGFGLLIGCG)
		{
			if (c == '.' || c == '[' || c == ']')
			{
				return true;
			}
		}
		return false;
	}

	private static bool BNDLKPGCJBM(string IGGFGLLIGCG)
	{
		if (IGGFGLLIGCG[0] == '$')
		{
			return true;
		}
		return false;
	}

	private static List<BAELOMEILMK> LMGMFLGHNGJ(string DPABILBDPFF)
	{
		List<BAELOMEILMK> list = new List<BAELOMEILMK>();
		int i = 0;
		int length = DPABILBDPFF.Length;
		while (i < length)
		{
			BAELOMEILMK bAELOMEILMK = new BAELOMEILMK();
			if (char.IsDigit(DPABILBDPFF[i]))
			{
				string text = string.Empty;
				char? c = null;
				for (; i < length; i++)
				{
					char c2 = DPABILBDPFF[i];
					if (!char.IsDigit(c2))
					{
						bool flag = c2 != '.';
						bool flag2 = c2 != 'e' && c2 != 'E';
						bool flag3 = !c.HasValue || (((!c.HasValue) ? ((int?)null) : new int?(c.Value)) != 101 && ((!c.HasValue) ? ((int?)null) : new int?(c.Value)) != 69) || c2 != '-';
						if (flag && flag2 && flag3)
						{
							break;
						}
					}
					c = c2;
					text += c2;
				}
				double num = Convert.ToDouble(text, CultureInfo.InvariantCulture);
				bAELOMEILMK.LFLGCDNKNJI = BLELIIJLLEB.Operand;
				bAELOMEILMK.LMENIMNKNHP = new ANCMPDAEGDN(num);
			}
			else if (!char.IsDigit(DPABILBDPFF[i]))
			{
				if (DPABILBDPFF[i] == ',')
				{
					i++;
					continue;
				}
				string text2 = string.Empty;
				while (i < length && (!MHPHPJEMDNH(DPABILBDPFF[i].ToString()) || !CPLOMAEDDEK(text2) || text2.Length == 0))
				{
					text2 += DPABILBDPFF[i];
					i++;
					if (EEEMOIBDBEA(text2))
					{
						break;
					}
				}
				if (EEEMOIBDBEA(text2))
				{
					bAELOMEILMK.LFLGCDNKNJI = BLELIIJLLEB.Operator;
					string key = text2.ToLower();
					bAELOMEILMK.IMGAKPOBGBP = JHNBIMFAIMP[key];
				}
				else
				{
					bAELOMEILMK.LFLGCDNKNJI = BLELIIJLLEB.Operand;
					bAELOMEILMK.LMENIMNKNHP = KDEOLIEBNKA(text2);
				}
			}
			list.Add(bAELOMEILMK);
		}
		return list;
	}

	private static bool CPLOMAEDDEK(string IGGFGLLIGCG)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < IGGFGLLIGCG.Length; i++)
		{
			switch (IGGFGLLIGCG[i])
			{
			case '[':
				num++;
				break;
			case ']':
				num2++;
				break;
			}
		}
		return num == num2;
	}

	private static KPOPDKMFMNK AFJIEEOFIAA(string IGGFGLLIGCG)
	{
		int num = IGGFGLLIGCG.IndexOf("[");
		string text = IGGFGLLIGCG.Substring(0, num);
		string dJIONFCICFC = IGGFGLLIGCG.Substring(num + 1, IGGFGLLIGCG.Length - num - 2);
		string text2 = text.Substring(1, text.Length - 1);
		if (!JNNPIBLANAD.ContainsKey(text2))
		{
			throw new Exception("Unknown function name " + text2);
		}
		List<FAOBBBMHEBL> mAABDFKMACJ = FNMPBFKFALB(dJIONFCICFC);
		return new KPOPDKMFMNK(mAABDFKMACJ, JNNPIBLANAD[text2]);
	}

	private static FAOBBBMHEBL KDEOLIEBNKA(string EBDLDPIBIEO)
	{
		FAOBBBMHEBL fAOBBBMHEBL = null;
		if (MBEKDNOJCOP(EBDLDPIBIEO))
		{
			EBDLDPIBIEO = NABNCPFHCLB(EBDLDPIBIEO);
			fAOBBBMHEBL = AFJIEEOFIAA(EBDLDPIBIEO);
		}
		else if (char.IsDigit(EBDLDPIBIEO[0]))
		{
			double num = Convert.ToDouble(EBDLDPIBIEO);
			fAOBBBMHEBL = new ANCMPDAEGDN(num);
		}
		else if (BNDLKPGCJBM(EBDLDPIBIEO))
		{
			string text = EBDLDPIBIEO.Substring(1, EBDLDPIBIEO.Length - 1);
			if (!FNBONMPOPKH.ContainsKey(text))
			{
				throw new Exception("Unknown variable " + text);
			}
			fAOBBBMHEBL = new AAGFMFHOGDN(FNBONMPOPKH[text]);
		}
		else
		{
			fAOBBBMHEBL = new ANCMPDAEGDN(EBDLDPIBIEO);
			fAOBBBMHEBL.set_Type(NKMIEOOPJHP.Variable);
		}
		return fAOBBBMHEBL;
	}

	private static List<FAOBBBMHEBL> FNMPBFKFALB(string DJIONFCICFC)
	{
		if (DJIONFCICFC.Length == 0)
		{
			List<FAOBBBMHEBL> list = new List<FAOBBBMHEBL>();
			list.Add(new FAOBBBMHEBL());
			return list;
		}
		List<FAOBBBMHEBL> list2 = new List<FAOBBBMHEBL>();
		DJIONFCICFC = DJIONFCICFC.Trim();
		string[] array = KGBAOFMDKOL(DJIONFCICFC, ',');
		string[] array2 = array;
		foreach (string eBDLDPIBIEO in array2)
		{
			FAOBBBMHEBL item = KDEOLIEBNKA(eBDLDPIBIEO);
			list2.Add(item);
		}
		return list2;
	}

	private static string[] KGBAOFMDKOL(string CGJGACJABDF, char EPJDMLMAOII)
	{
		List<string> list = new List<string>();
		int num = 0;
		do
		{
			int num2;
			if (CGJGACJABDF[num] == '?')
			{
				num2 = GetEndOfFunc(CGJGACJABDF, num) + 1;
			}
			else
			{
				num2 = CGJGACJABDF.IndexOf(',', num);
				if (num2 == -1)
				{
					list.Add(CGJGACJABDF.Substring(num));
					break;
				}
			}
			list.Add(CGJGACJABDF.Substring(num, num2 - num));
			num = num2 + 1;
		}
		while (num < CGJGACJABDF.Length);
		return list.ToArray();
	}

	private static int GetEndOfFunc(string CGJGACJABDF, int CAILGDNIKJD)
	{
		int num = CGJGACJABDF.IndexOf('[', CAILGDNIKJD);
		if (num < 0)
		{
			return -1;
		}
		num++;
		int num2 = 1;
		for (; num < CGJGACJABDF.Length; num++)
		{
			if (num2 == 0)
			{
				break;
			}
			if (CGJGACJABDF[num] == '[')
			{
				num2++;
			}
			else if (CGJGACJABDF[num] == ']')
			{
				num2--;
			}
		}
		return num - 1;
	}

	private static string NABNCPFHCLB(string IGGFGLLIGCG)
	{
		while (IGGFGLLIGCG.Contains("."))
		{
			IGGFGLLIGCG = BOENCMHMLGA(IGGFGLLIGCG);
		}
		return IGGFGLLIGCG;
	}

	private static string BOENCMHMLGA(string IGGFGLLIGCG)
	{
		int num = IGGFGLLIGCG.IndexOf('.');
		string text = IGGFGLLIGCG.Substring(0, num);
		string text2 = "?" + IGGFGLLIGCG.Substring(num + 1, IGGFGLLIGCG.Length - num - 1);
		IGGFGLLIGCG = text2 + "[" + text + "]";
		return IGGFGLLIGCG;
	}

	private static bool KDHHNEOBBCA(List<BAELOMEILMK> HELFDCAIJNE)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i != HELFDCAIJNE.Count; i++)
		{
			if (HELFDCAIJNE[i].LFLGCDNKNJI == BLELIIJLLEB.Operator)
			{
				if (HELFDCAIJNE[i].IMGAKPOBGBP == JHNBIMFAIMP["("])
				{
					num++;
				}
				else if (HELFDCAIJNE[i].IMGAKPOBGBP == JHNBIMFAIMP[")"])
				{
					num2++;
				}
			}
		}
		return num == num2;
	}

	private static List<BAELOMEILMK> CNOJALPFLLC(List<BAELOMEILMK> JEFEGDICJJC)
	{
		List<BAELOMEILMK> list = new List<BAELOMEILMK>();
		List<BAELOMEILMK> list2 = new List<BAELOMEILMK>();
		int num = 0;
		while (num != JEFEGDICJJC.Count)
		{
			if (JEFEGDICJJC[num].LFLGCDNKNJI == BLELIIJLLEB.Operand)
			{
				list.Add(JEFEGDICJJC[num]);
				num++;
			}
			else if (JEFEGDICJJC[num].IMGAKPOBGBP == JHNBIMFAIMP["("])
			{
				list2.Add(JEFEGDICJJC[num]);
				num++;
			}
			else if (JEFEGDICJJC[num].IMGAKPOBGBP == JHNBIMFAIMP[")"])
			{
				while (list2.Count != 0 && list2[list2.Count - 1].IMGAKPOBGBP != JHNBIMFAIMP["("])
				{
					list.Add(list2[list2.Count - 1]);
					list2.RemoveAt(list2.Count - 1);
				}
				if (list2.Count != 0 && list2[list2.Count - 1].IMGAKPOBGBP == JHNBIMFAIMP["("])
				{
					list2.RemoveAt(list2.Count - 1);
					if (list2.Count != 0 && list2[list2.Count - 1].IMGAKPOBGBP.JBBJEBCAAEG == IKHBAIDMOHC.OperatorPowDirect)
					{
						list.Add(list2[list2.Count - 1]);
						list2.RemoveAt(list2.Count - 1);
					}
				}
				num++;
			}
			else if (list2.Count == 0)
			{
				list2.Add(JEFEGDICJJC[num]);
				num++;
			}
			else if ((JEFEGDICJJC[num].IMGAKPOBGBP.JBBJEBCAAEG == IKHBAIDMOHC.OperatorAddDirect && list2[list2.Count - 1].IMGAKPOBGBP.NNCOAODDCOD < JEFEGDICJJC[num].IMGAKPOBGBP.NNCOAODDCOD) || (JEFEGDICJJC[num].IMGAKPOBGBP.JBBJEBCAAEG == IKHBAIDMOHC.OperatorPowDirect && list2[list2.Count - 1].IMGAKPOBGBP.NNCOAODDCOD <= JEFEGDICJJC[num].IMGAKPOBGBP.NNCOAODDCOD))
			{
				list2.Add(JEFEGDICJJC[num]);
				num++;
			}
			else if ((JEFEGDICJJC[num].IMGAKPOBGBP.JBBJEBCAAEG == IKHBAIDMOHC.OperatorAddDirect && list2[list2.Count - 1].IMGAKPOBGBP.NNCOAODDCOD >= JEFEGDICJJC[num].IMGAKPOBGBP.NNCOAODDCOD) || (JEFEGDICJJC[num].IMGAKPOBGBP.JBBJEBCAAEG == IKHBAIDMOHC.OperatorPowDirect && list2[list2.Count - 1].IMGAKPOBGBP.NNCOAODDCOD > JEFEGDICJJC[num].IMGAKPOBGBP.NNCOAODDCOD))
			{
				list.Add(list2[list2.Count - 1]);
				list2.RemoveAt(list2.Count - 1);
			}
		}
		while (list2.Count != 0)
		{
			list.Add(list2[list2.Count - 1]);
			list2.RemoveAt(list2.Count - 1);
		}
		return list;
	}
}
