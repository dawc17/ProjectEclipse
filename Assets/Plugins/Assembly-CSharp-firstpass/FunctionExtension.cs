using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class FunctionExtension : global::EventDispatcher<object>
{
	public enum AFILEBFICDF
	{
		VARIABLE_NONE = 0,
		VARIABLE_STRING = 1,
		VARIABLE_NUMBER = 2
	}

	public enum DLLJOIFFBPL
	{
		COMPARE_NONE = 0,
		COMPARE_EQUAL = 1,
		COMPARE_GREATER = 2,
		COMPARE_GREATER_EQUAL = 3,
		COMPARE_LESS = 4,
		COMPARE_LESS_EQUAL = 5
	}

	public enum PLGKHFNOBCB
	{
		TYPE_VALUE = 0,
		TYPE_FUNCTION = 1,
		TYPE_VARIABLE = 2,
		TYPE_SEPARATOR = 3
	}

	public class CCDGFNHLMCG
	{
		public string name = string.Empty;

		public string value = string.Empty;

		public Action<object> callback;
	}

	public class CallbackResult
	{
		public object target;

		public object data;

		public FunctionResult NAGGNMIFFGK;
	}

	public class FunctionObject
	{
		public PLGKHFNOBCB LFLGCDNKNJI;

		public string body = string.Empty;

		public FunctionResult DCJLKCFKCOM = new FunctionResult();
	}

	public class GLBAFLLMOOH : FunctionObject
	{
		public string FJLOLCPJACB = string.Empty;

		public string arguments = string.Empty;

		public string HBDLDIKHFEG = string.Empty;

		public string name = string.Empty;

		public List<FunctionObject> EIALKNELNMB = new List<FunctionObject>();

		public List<FunctionObject> EJMMIOKEPHC = new List<FunctionObject>();

		public GLBAFLLMOOH()
		{
			LFLGCDNKNJI = PLGKHFNOBCB.TYPE_FUNCTION;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<CallbackResult> DEGCIDCPOBA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<CallbackResult> GLPCLEMBAJJ;

	private List<CCDGFNHLMCG> LOEIIBBBOCF = new List<CCDGFNHLMCG>();

	private List<FunctionObject> AHGOKKLEGBP = new List<FunctionObject>();

	private List<FunctionObject> FMBAPDNOOGG = new List<FunctionObject>();

	private int countFuncName;

	private GLBAFLLMOOH GJBGEHPMBCN = new GLBAFLLMOOH();

	private FunctionResult NAGGNMIFFGK = new FunctionResult();

	private object target;

	public Action<CallbackResult> ILFGMIMMCOF
	{
		get
		{
			return GOPDKBFNDPJ();
		}
		set
		{
			DMPCFMACDJM(value);
		}
	}

	public Action<CallbackResult> BLGIIGFKFPN
	{
		get
		{
			return KAJPFGAIDCE();
		}
		set
		{
			PBPBNENGLPA(value);
		}
	}

	public Action<CallbackResult> GOPDKBFNDPJ()
	{
		return DEGCIDCPOBA;
	}

	public void DMPCFMACDJM(Action<CallbackResult> value)
	{
		DEGCIDCPOBA = value;
	}

	public Action<CallbackResult> KAJPFGAIDCE()
	{
		return GLPCLEMBAJJ;
	}

	public void PBPBNENGLPA(Action<CallbackResult> value)
	{
		GLPCLEMBAJJ = value;
	}

	public object IDPLAGOELKE()
	{
		return target;
	}

	public void set_Target(object value)
	{
		target = value;
	}

	public void Parse(string target)
	{
		string value = ClearGaps(target);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("?Root[");
		stringBuilder.Append(value);
		stringBuilder.Append("]");
		GJBGEHPMBCN = JCJHEBOMKIC(stringBuilder.ToString());
	}

	public static DLLJOIFFBPL MHKNIEBONKD(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Equal":
			return DLLJOIFFBPL.COMPARE_EQUAL;
		case "Greater":
			return DLLJOIFFBPL.COMPARE_GREATER;
		case "GreaterEqual":
			return DLLJOIFFBPL.COMPARE_GREATER_EQUAL;
		case "Less":
			return DLLJOIFFBPL.COMPARE_LESS;
		case "LessEqual":
			return DLLJOIFFBPL.COMPARE_LESS_EQUAL;
		default:
			return DLLJOIFFBPL.COMPARE_NONE;
		}
	}

	public static AFILEBFICDF FANCHMOGIEI(string value)
	{
		if (value.Length == 0)
		{
			LLLOJBFMONN.Error("Value of QuestConditionVariable is empty");
		}
		Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
		Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
		RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
		RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(value);
		if (lANLKOHCGEJ.OJEHEKMJJBL() == 0)
		{
			return AFILEBFICDF.VARIABLE_NUMBER;
		}
		return AFILEBFICDF.VARIABLE_STRING;
	}

	public void SetVariable(string name, string value)
	{
		CCDGFNHLMCG cCDGFNHLMCG = new CCDGFNHLMCG();
		cCDGFNHLMCG.name = name;
		cCDGFNHLMCG.value = value;
		LOEIIBBBOCF.Add(cCDGFNHLMCG);
	}

	public CCDGFNHLMCG GBNDBKOIEJA(string name)
	{
		foreach (CCDGFNHLMCG item in LOEIIBBBOCF)
		{
			if (item.name.Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public List<CCDGFNHLMCG> ICGHBLMJENJ()
	{
		return LOEIIBBBOCF;
	}

	public List<FunctionObject> COGDGCDPOBJ()
	{
		return AHGOKKLEGBP;
	}

	public FunctionResult IBCPKBBAFNH()
	{
		NAGGNMIFFGK.DCJLKCFKCOM = null;
		GHBKAIFOPPC(GJBGEHPMBCN, NAGGNMIFFGK);
		NAGGNMIFFGK.DCJLKCFKCOM = CalculateResult(NAGGNMIFFGK.DCJLKCFKCOM);
		return NAGGNMIFFGK;
	}

	public FunctionResult DIHPNAJEENI()
	{
		return NAGGNMIFFGK;
	}

	public bool ENCHAAJAMIM()
	{
		return NAGGNMIFFGK.DCJLKCFKCOM.Equals(string.Empty);
	}

	public string CalculateResult(string target)
	{
		List<string> list = new List<string>();
		string[] collection = target.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		list.AddRange(collection);
		if (list.Count > 1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.Append(CalculateResult(list[i]));
				if (i < list.Count - 1)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}
		if (target.Equals(string.Empty))
		{
			return target;
		}
		Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
		Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
		RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
		RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(target);
		if (lANLKOHCGEJ.OJEHEKMJJBL() == 0)
		{
			object obj = lANLKOHCGEJ.ODHJHHMEEOI();
			string text = ((obj == null) ? string.Empty : obj.ToString());
			double result;
			if (double.TryParse(text, out result) && result >= 0.0)
			{
				return text;
			}
			return string.Format("({0})", text);
		}
		return target;
	}

	private void GHBKAIFOPPC(GLBAFLLMOOH KJFKPMCPIBH, FunctionResult DCJLKCFKCOM)
	{
		foreach (FunctionObject item in KJFKPMCPIBH.EJMMIOKEPHC)
		{
			if (item.LFLGCDNKNJI == PLGKHFNOBCB.TYPE_FUNCTION)
			{
				GLBAFLLMOOH gLBAFLLMOOH = item as GLBAFLLMOOH;
				if (gLBAFLLMOOH != null)
				{
					GHBKAIFOPPC(gLBAFLLMOOH, item.DCJLKCFKCOM);
				}
			}
			else if (item.LFLGCDNKNJI == PLGKHFNOBCB.TYPE_VARIABLE)
			{
				KKODDGMCDBC(item.body, item.DCJLKCFKCOM);
			}
			else
			{
				item.DCJLKCFKCOM.DCJLKCFKCOM = item.body;
			}
		}
		CBBHKMCGFKC(KJFKPMCPIBH, DCJLKCFKCOM);
	}

	private void CBBHKMCGFKC(GLBAFLLMOOH KJFKPMCPIBH, FunctionResult DCJLKCFKCOM)
	{
		CBMFNEPCOCP(KJFKPMCPIBH, ref DCJLKCFKCOM);
		FAKNFAMNNIL(KJFKPMCPIBH, DCJLKCFKCOM);
		DCJLKCFKCOM.DCJLKCFKCOM = CalculateResult(DCJLKCFKCOM.DCJLKCFKCOM);
		if (KAJPFGAIDCE() != null)
		{
			CallbackResult oMJHHJNIJOL = new CallbackResult();
			oMJHHJNIJOL.data = KJFKPMCPIBH;
			oMJHHJNIJOL.NAGGNMIFFGK = DCJLKCFKCOM;
			oMJHHJNIJOL.target = target;
			KAJPFGAIDCE()(oMJHHJNIJOL);
		}
		KJFKPMCPIBH.EIALKNELNMB.Clear();
	}

	private void FAKNFAMNNIL(GLBAFLLMOOH KJFKPMCPIBH, FunctionResult DCJLKCFKCOM)
	{
		KJFKPMCPIBH.EIALKNELNMB.Clear();
		StringBuilder stringBuilder = new StringBuilder();
		PLGKHFNOBCB pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
		char c = ',';
		string dCJLKCFKCOM = DCJLKCFKCOM.DCJLKCFKCOM;
		int i = 0;
		for (int length = dCJLKCFKCOM.Length; i < length; i++)
		{
			char c2 = dCJLKCFKCOM[i];
			bool flag = c2 == ',';
			bool flag2 = c == ',';
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_VALUE && flag)
			{
				if (!stringBuilder.Equals(string.Empty))
				{
					FunctionObject pENDFCHBHIB = new FunctionObject();
					Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
					Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
					RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
					RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(stringBuilder.ToString());
					if (lANLKOHCGEJ.OJEHEKMJJBL() == 0)
					{
						pENDFCHBHIB.body = lANLKOHCGEJ.ODHJHHMEEOI().ToString();
					}
					else
					{
						pENDFCHBHIB.body = stringBuilder.ToString();
					}
					pENDFCHBHIB.LFLGCDNKNJI = PLGKHFNOBCB.TYPE_VALUE;
					KJFKPMCPIBH.EIALKNELNMB.Add(pENDFCHBHIB);
					stringBuilder.Clear();
				}
				pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
			}
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_SEPARATOR && !flag)
			{
				if (!stringBuilder.Equals(string.Empty))
				{
					stringBuilder.Clear();
				}
				pLGKHFNOBCB = PLGKHFNOBCB.TYPE_VALUE;
			}
			c = c2;
			stringBuilder.Append(c2);
		}
		string text = stringBuilder.ToString();
		if (!text.Equals(string.Empty))
		{
			FunctionObject pENDFCHBHIB2 = new FunctionObject();
			Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI2 = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
			Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO2 = new Dictionary<string, RpnParser.ParameterDelegate>();
			RpnParser.init(pPEABEJMCPI2, gIOGAJGIGMO2);
			RpnParser.Formula lANLKOHCGEJ2 = new RpnParser.Formula(text);
			if (lANLKOHCGEJ2.OJEHEKMJJBL() == 0)
			{
				pENDFCHBHIB2.body = lANLKOHCGEJ2.ODHJHHMEEOI().ToString();
			}
			else
			{
				pENDFCHBHIB2.body = stringBuilder.ToString();
			}
			pENDFCHBHIB2.LFLGCDNKNJI = PLGKHFNOBCB.TYPE_VALUE;
			KJFKPMCPIBH.EIALKNELNMB.Add(pENDFCHBHIB2);
		}
	}

	private void KKODDGMCDBC(string body, FunctionResult DCJLKCFKCOM)
	{
		if (((body.Length <= 1) ? '_' : body[1]).Equals('$'))
		{
			if (GOPDKBFNDPJ() != null)
			{
				CallbackResult oMJHHJNIJOL = new CallbackResult();
				oMJHHJNIJOL.data = body;
				oMJHHJNIJOL.NAGGNMIFFGK = DCJLKCFKCOM;
				oMJHHJNIJOL.target = target;
				GOPDKBFNDPJ()(oMJHHJNIJOL);
			}
		}
		else
		{
			string text = null;
			text = ((body.Length <= 0 || !body[0].Equals('_')) ? body : body.Substring(1));
			CCDGFNHLMCG cCDGFNHLMCG = GBNDBKOIEJA(text);
			if (cCDGFNHLMCG != null)
			{
				DCJLKCFKCOM.DCJLKCFKCOM = cCDGFNHLMCG.value;
			}
		}
	}

	private void CBMFNEPCOCP(GLBAFLLMOOH KJFKPMCPIBH, ref FunctionResult DCJLKCFKCOM)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(KJFKPMCPIBH.arguments);
		foreach (FunctionObject item in KJFKPMCPIBH.EJMMIOKEPHC)
		{
			stringBuilder.Replace(item.body, item.DCJLKCFKCOM.DCJLKCFKCOM);
		}
		DCJLKCFKCOM.DCJLKCFKCOM = stringBuilder.ToString();
	}

	private string ClearGaps(string target)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in target)
		{
			if (!char.IsWhiteSpace(c))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private GLBAFLLMOOH JCJHEBOMKIC(string target)
	{
		int num = target.IndexOf('[');
		int num2 = target.LastIndexOf(']');
		int num3 = target.LastIndexOf('.');
		GLBAFLLMOOH gLBAFLLMOOH = new GLBAFLLMOOH();
		gLBAFLLMOOH.FJLOLCPJACB = target.Substring(1, num - 1);
		gLBAFLLMOOH.arguments = target.Substring(num + 1, num2 - num - 1);
		gLBAFLLMOOH.HBDLDIKHFEG = ((num3 <= num2) ? string.Empty : target.Substring(num3 + 1, target.Length - num3 - 1));
		gLBAFLLMOOH.body = target;
		gLBAFLLMOOH.name = "Function";
		gLBAFLLMOOH.name += countFuncName;
		countFuncName++;
		HDDGCAHGNIB(gLBAFLLMOOH);
		return gLBAFLLMOOH;
	}

	private void HDDGCAHGNIB(GLBAFLLMOOH KJFKPMCPIBH)
	{
		StringBuilder stringBuilder = new StringBuilder();
		PLGKHFNOBCB pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
		int num = 0;
		bool flag = false;
		char c = ',';
		string mAABDFKMACJ = KJFKPMCPIBH.arguments;
		int i = 0;
		for (int length = mAABDFKMACJ.Length; i < length; i++)
		{
			char c2 = mAABDFKMACJ[i];
			bool flag2 = (RpnParser.MHPHPJEMDNH(c2) || c2 == ',') && c2 != '?' && c2 != '_';
			bool flag3 = RpnParser.MHPHPJEMDNH(c) || c == ',';
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_FUNCTION)
			{
				switch (c2)
				{
				case '[':
					num++;
					flag = true;
					break;
				case ']':
					num--;
					if (num < 0)
					{
						LLLOJBFMONN.Error("FunctionExtension::parseObjects error! Brackets not valid. {0}", mAABDFKMACJ);
					}
					break;
				default:
					if (flag2 && flag && num == 0)
					{
						if (!stringBuilder.Equals(string.Empty))
						{
							GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
							stringBuilder.Clear();
						}
						pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
					}
					break;
				}
			}
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_VARIABLE && flag2)
			{
				if (!stringBuilder.Equals(string.Empty))
				{
					GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
					stringBuilder.Clear();
				}
				pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
			}
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_VALUE && flag2)
			{
				if (!stringBuilder.Equals(string.Empty))
				{
					GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
					stringBuilder.Clear();
				}
				pLGKHFNOBCB = PLGKHFNOBCB.TYPE_SEPARATOR;
			}
			if (pLGKHFNOBCB == PLGKHFNOBCB.TYPE_SEPARATOR)
			{
				switch (c2)
				{
				case '?':
					if (!stringBuilder.Equals(string.Empty))
					{
						GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
						stringBuilder.Clear();
					}
					pLGKHFNOBCB = PLGKHFNOBCB.TYPE_FUNCTION;
					num = 0;
					flag = false;
					break;
				case '_':
					if (!stringBuilder.Equals(string.Empty))
					{
						GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
						stringBuilder.Clear();
					}
					pLGKHFNOBCB = PLGKHFNOBCB.TYPE_VARIABLE;
					break;
				default:
					if (!flag2)
					{
						if (!stringBuilder.Equals(string.Empty))
						{
							GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
							stringBuilder.Clear();
						}
						pLGKHFNOBCB = PLGKHFNOBCB.TYPE_VALUE;
					}
					break;
				}
			}
			c = c2;
			stringBuilder.Append(c2);
		}
		if (!stringBuilder.Equals(string.Empty))
		{
			GLJMJOACEIP(KJFKPMCPIBH, stringBuilder.ToString(), pLGKHFNOBCB);
		}
	}

	private void GLJMJOACEIP(GLBAFLLMOOH KJFKPMCPIBH, string HGFADEKMPAK, PLGKHFNOBCB LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case PLGKHFNOBCB.TYPE_FUNCTION:
			DMEMAJBIOKE(KJFKPMCPIBH, HGFADEKMPAK);
			break;
		case PLGKHFNOBCB.TYPE_VARIABLE:
			DEGGADHPIPO(KJFKPMCPIBH, HGFADEKMPAK, true);
			break;
		case PLGKHFNOBCB.TYPE_VALUE:
			DEGGADHPIPO(KJFKPMCPIBH, HGFADEKMPAK, false);
			break;
		case PLGKHFNOBCB.TYPE_SEPARATOR:
			AMDGNEHFPOK(KJFKPMCPIBH, HGFADEKMPAK);
			break;
		}
	}

	private void DMEMAJBIOKE(GLBAFLLMOOH KJFKPMCPIBH, string target)
	{
		if (!target.Equals(string.Empty))
		{
			GLBAFLLMOOH gLBAFLLMOOH = JCJHEBOMKIC(target);
			gLBAFLLMOOH.LFLGCDNKNJI = PLGKHFNOBCB.TYPE_FUNCTION;
			KJFKPMCPIBH.EJMMIOKEPHC.Add(gLBAFLLMOOH);
			target = string.Empty;
			FMBAPDNOOGG.Add(gLBAFLLMOOH);
		}
	}

	private void DEGGADHPIPO(GLBAFLLMOOH KJFKPMCPIBH, string target, bool HHEKDBGADGC)
	{
		if (!target.Equals(string.Empty))
		{
			FunctionObject pENDFCHBHIB = new FunctionObject();
			pENDFCHBHIB.body = target;
			pENDFCHBHIB.LFLGCDNKNJI = (HHEKDBGADGC ? PLGKHFNOBCB.TYPE_VARIABLE : PLGKHFNOBCB.TYPE_VALUE);
			KJFKPMCPIBH.EJMMIOKEPHC.Add(pENDFCHBHIB);
			target = string.Empty;
			AHGOKKLEGBP.Add(pENDFCHBHIB);
		}
	}

	private void AMDGNEHFPOK(GLBAFLLMOOH KJFKPMCPIBH, string target)
	{
		if (!target.Equals(string.Empty))
		{
			FunctionObject pENDFCHBHIB = new FunctionObject();
			pENDFCHBHIB.body = target;
			pENDFCHBHIB.LFLGCDNKNJI = PLGKHFNOBCB.TYPE_SEPARATOR;
			KJFKPMCPIBH.EJMMIOKEPHC.Add(pENDFCHBHIB);
			target = string.Empty;
			AHGOKKLEGBP.Add(pENDFCHBHIB);
		}
	}

	public static bool NumberCompare(float KONPFNHLPJG, float BABJGGEOCBG, DLLJOIFFBPL LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case DLLJOIFFBPL.COMPARE_EQUAL:
			return KONPFNHLPJG == BABJGGEOCBG;
		case DLLJOIFFBPL.COMPARE_GREATER:
			return KONPFNHLPJG > BABJGGEOCBG;
		case DLLJOIFFBPL.COMPARE_GREATER_EQUAL:
			return KONPFNHLPJG >= BABJGGEOCBG;
		case DLLJOIFFBPL.COMPARE_LESS:
			return KONPFNHLPJG < BABJGGEOCBG;
		case DLLJOIFFBPL.COMPARE_LESS_EQUAL:
			return KONPFNHLPJG <= BABJGGEOCBG;
		default:
			return false;
		}
	}

	public static bool IsCompare(string value)
	{
		string[] collection = value.Split(',');
		List<string> list = new List<string>(collection);
		if (list.Count > 2)
		{
			AFILEBFICDF aFILEBFICDF = FANCHMOGIEI(list[0]);
			AFILEBFICDF aFILEBFICDF2 = FANCHMOGIEI(list[1]);
			DLLJOIFFBPL lFLGCDNKNJI = MHKNIEBONKD(list[2]);
			if (aFILEBFICDF == aFILEBFICDF2)
			{
				switch (aFILEBFICDF)
				{
				case AFILEBFICDF.VARIABLE_NUMBER:
				{
					Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
					Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
					RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
					RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(list[0]);
					RpnParser.Formula lANLKOHCGEJ2 = new RpnParser.Formula(list[1]);
					float result;
					if (!float.TryParse(lANLKOHCGEJ.ODHJHHMEEOI().ToString(), out result))
					{
						result = 0f;
					}
					float result2;
					if (!float.TryParse(lANLKOHCGEJ2.ODHJHHMEEOI().ToString(), out result2))
					{
						result2 = 0f;
					}
					return NumberCompare(result, result2, lFLGCDNKNJI);
				}
				case AFILEBFICDF.VARIABLE_STRING:
					return list[0].Equals(list[1]);
				}
			}
		}
		return false;
	}
}
