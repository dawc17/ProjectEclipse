using System.Collections.Generic;
using System.Text;

public abstract class ConditionExtension
{
	public enum KDEAPAPEEAO
	{
		MATH_NONE = 0,
		MATH_SUM = 1,
		MATH_SUB = 2,
		MATH_MULTI = 3,
		MATH_DIVISION = 4,
		MATH_DIVISION_INT = 5,
		MATH_MOD = 6,
		MATH_RAND = 7
	}

	public enum FBKBGPPHALB
	{
		STRING_NONE = 0,
		STRING_CONCAT = 1,
		STRING_SLICE = 2
	}

	public enum HAEOKBKNCHE
	{
		QUEST_CONDITION_VARIABLE_NONE = 0,
		QUEST_CONDITION_VARIABLE_STRING = 1,
		QUEST_CONDITION_VARIABLE_NUMBER = 2
	}

	public class HLCPKKIIBFB
	{
		public string value = string.Empty;

		public string DCJLKCFKCOM = string.Empty;

		public bool Empty()
		{
			return DCJLKCFKCOM.Equals(string.Empty);
		}

		public void Reset()
		{
			DCJLKCFKCOM = string.Empty;
		}
	}

	public class QuestFunctions : HLCPKKIIBFB
	{
		public string FJLOLCPJACB = string.Empty;

		public string GKPKHOMIMMN = string.Empty;

		public string HBDLDIKHFEG = string.Empty;

		public List<HLCPKKIIBFB> arguments = new List<HLCPKKIIBFB>();

		public string OMHIDHHNPEF()
		{
			return (arguments.Count <= 0) ? string.Empty : arguments[0].DCJLKCFKCOM;
		}
	}

	public class CompareResult
	{
		public string resultSTR;

		public double resultNumber;

		public CompareResult()
		{
			resultSTR = string.Empty;
			resultNumber = 0.0;
		}

		public CompareResult(string value)
		{
			resultSTR = value;
			resultNumber = 0.0;
		}

		public CompareResult(float value)
		{
			resultSTR = string.Empty;
			resultNumber = value;
		}

		public CompareResult(string PGIDABLDOAM, float CCJOKGEFOFP)
		{
			resultSTR = PGIDABLDOAM;
			resultNumber = CCJOKGEFOFP;
		}

		public override string ToString()
		{
			if (INCOIAANDCO())
			{
				return resultNumber.ToString();
			}
			return resultSTR;
		}

		public bool INCOIAANDCO()
		{
			return resultSTR.Equals(string.Empty);
		}

		public void Clear()
		{
			resultSTR = string.Empty;
			resultNumber = 0.0;
		}
	}

	public ConditionExtension()
	{
	}

	public void MCPIOGALBMK(string value, CompareResult BMDEBHIHIAJ)
	{
		if (!value.Equals(string.Empty))
		{
			char c = value[0];
			if (c.Equals('_'))
			{
				SessionSettings(value, BMDEBHIHIAJ);
			}
			else if (c.Equals('?'))
			{
				CDNAPPKEJHA(value, BMDEBHIHIAJ);
				if (!BMDEBHIHIAJ.resultSTR.Equals(string.Empty))
				{
					string iBBAMMHHBFE = BMDEBHIHIAJ.resultSTR;
					BMDEBHIHIAJ.Clear();
					MCPIOGALBMK(iBBAMMHHBFE, BMDEBHIHIAJ);
				}
			}
			else
			{
				JLDFNJEALLB(value, BMDEBHIHIAJ);
			}
		}
		else
		{
			LLLOJBFMONN.Write("ConditionExtension::setValue - empty string");
		}
	}

	protected QuestFunctions JNMHPJGGPMI(string value)
	{
		if (!value.Equals(string.Empty))
		{
			return JMKKHEDOBDB(value);
		}
		LLLOJBFMONN.Write("ConditionExtension.parseFunctions - empty string");
		return null;
	}

	protected QuestFunctions JMKKHEDOBDB(string value)
	{
		if (value[0].Equals('?'))
		{
			// Newer gamedata uses ?Function[args] while this quest evaluator was
			// compiled for ?Function(args). Accept both at the parser boundary;
			// combat expressions are handled by the separate FunctionExtension.
			if (value.IndexOf('(') < 0 && value.IndexOf('[') >= 0)
				value = value.Replace('[', '(').Replace(']', ')');
			int num = value.IndexOf('(');
			int num2 = value.LastIndexOf(')');
			int num3 = value.LastIndexOf('.');
			if (num <= 1 || num2 < num)
			{
				LLLOJBFMONN.Error("ConditionExtension::parseFunctions - malformed function: {0}", value);
				return null;
			}
			QuestFunctions bECNHJBBOKO = new QuestFunctions();
			bECNHJBBOKO.GKPKHOMIMMN = value;
			bECNHJBBOKO.FJLOLCPJACB = value.Substring(1, num - 1);
			bECNHJBBOKO.GKPKHOMIMMN = value.Substring(num + 1, num2 - num - 1);
			bECNHJBBOKO.HBDLDIKHFEG = ((num3 <= num2) ? string.Empty : value.Substring(num3 + 1, value.Length - num3 - 1));
			FAKNFAMNNIL(bECNHJBBOKO);
			return bECNHJBBOKO;
		}
		return null;
	}

	protected void FAKNFAMNNIL(QuestFunctions KJFKPMCPIBH)
	{
		KJFKPMCPIBH.arguments.Clear();
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		string text = ClearGaps(KJFKPMCPIBH.GKPKHOMIMMN);
		int i = 0;
		for (int length = text.Length; i < length; i++)
		{
			char c = text[i];
			char c2 = ((i + 1 >= length) ? '*' : text[i + 1]);
			if (!flag && c == '?')
			{
				flag = true;
			}
			if (!flag)
			{
				if (!c.Equals(',') && !c.Equals('(') && !c.Equals(')') && !c.Equals('?'))
				{
					stringBuilder2.Append(c);
				}
				else if (stringBuilder2.Length > 0)
				{
					HLCPKKIIBFB hLCPKKIIBFB = new HLCPKKIIBFB();
					hLCPKKIIBFB.DCJLKCFKCOM = stringBuilder2.ToString();
					KJFKPMCPIBH.arguments.Add(hLCPKKIIBFB);
					stringBuilder2.Clear();
				}
			}
			if (flag && c.Equals('('))
			{
				num++;
			}
			else if ((flag && c.Equals(')')) || flag2)
			{
				if (c.Equals(')'))
				{
					num--;
				}
				if (num <= 0)
				{
					if (!flag2 && !c2.Equals('*') && !c2.Equals(',') && !c2.Equals(')'))
					{
						flag2 = true;
					}
					if (flag2 && (c2.Equals('*') || c2.Equals(',')))
					{
						flag2 = false;
					}
					if (!flag2)
					{
						flag = false;
						stringBuilder.Append(c);
						QuestFunctions item = JNMHPJGGPMI(stringBuilder.ToString());
						KJFKPMCPIBH.arguments.Add(item);
						stringBuilder.Clear();
					}
				}
			}
			if (flag)
			{
				stringBuilder.Append(c);
			}
		}
		if (stringBuilder2.Length > 0)
		{
			HLCPKKIIBFB hLCPKKIIBFB2 = new HLCPKKIIBFB();
			hLCPKKIIBFB2.DCJLKCFKCOM = stringBuilder2.ToString();
			KJFKPMCPIBH.arguments.Add(hLCPKKIIBFB2);
			stringBuilder2.Clear();
		}
	}

	protected void SessionSettings(string value, CompareResult BMDEBHIHIAJ)
	{
		IDHOFHMDIPL(value, BMDEBHIHIAJ);
	}

	protected void JLDFNJEALLB(string value, CompareResult BMDEBHIHIAJ)
	{
		// An unset session/roster variable is a valid state while a quest waits
		// for its first event.  Empty strings used to be classified as numbers
		// and fed to double.Parse, spamming FormatException and interrupting the
		// tutorial action sequence (notably the first Shop button press).
		if (string.IsNullOrEmpty(value))
		{
			BMDEBHIHIAJ.resultSTR = string.Empty;
			return;
		}
		switch (FANCHMOGIEI(value))
		{
		case HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_STRING:
			BMDEBHIHIAJ.resultSTR = value;
			break;
		case HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_NUMBER:
			double result;
			if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result) ||
				double.TryParse(value, out result))
			{
				BMDEBHIHIAJ.resultNumber = result;
			}
			else
			{
				BMDEBHIHIAJ.resultSTR = value;
			}
			break;
		}
	}

	protected HAEOKBKNCHE FANCHMOGIEI(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_STRING;
		}
		foreach (int num in value)
		{
			if (num < 43 || num > 57)
			{
				return HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_STRING;
			}
		}
		return HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_NUMBER;
	}

	protected void CDNAPPKEJHA(string value, CompareResult BMDEBHIHIAJ)
	{
		QuestFunctions kJFKPMCPIBH = JNMHPJGGPMI(value);
		CDNAPPKEJHA(kJFKPMCPIBH, BMDEBHIHIAJ);
		FELJIDPAKPI(kJFKPMCPIBH);
	}

	protected void CDNAPPKEJHA(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		if (KJFKPMCPIBH == null)
		{
			return;
		}
		foreach (HLCPKKIIBFB item in KJFKPMCPIBH.arguments)
		{
			QuestFunctions bECNHJBBOKO = item as QuestFunctions;
			if (bECNHJBBOKO != null)
			{
				if (bECNHJBBOKO.Empty())
				{
					CompareResult lNIDLHOIHIM = new CompareResult();
					CDNAPPKEJHA(bECNHJBBOKO, lNIDLHOIHIM);
					bECNHJBBOKO.DCJLKCFKCOM = lNIDLHOIHIM.ToString();
				}
			}
			else
			{
				CompareResult lNIDLHOIHIM2 = new CompareResult();
				MCPIOGALBMK(item.DCJLKCFKCOM, lNIDLHOIHIM2);
				item.DCJLKCFKCOM = lNIDLHOIHIM2.ToString();
			}
		}
		AIFNPKLNPEE(KJFKPMCPIBH, BMDEBHIHIAJ);
	}

	protected abstract void AIFNPKLNPEE(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ);

	protected abstract void IDHOFHMDIPL(string value, CompareResult BMDEBHIHIAJ);

	protected void FELJIDPAKPI(QuestFunctions KJFKPMCPIBH)
	{
		KJFKPMCPIBH.Reset();
		foreach (HLCPKKIIBFB item in KJFKPMCPIBH.arguments)
		{
			QuestFunctions bECNHJBBOKO = item as QuestFunctions;
			if (bECNHJBBOKO != null)
			{
				FELJIDPAKPI(bECNHJBBOKO);
			}
		}
	}

	protected void MDENBJJAPMH(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ, KDEAPAPEEAO LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case KDEAPAPEEAO.MATH_SUM:
		case KDEAPAPEEAO.MATH_SUB:
		case KDEAPAPEEAO.MATH_MULTI:
		case KDEAPAPEEAO.MATH_DIVISION:
		{
			int num5 = 0;
			double num6 = 0.0;
			foreach (HLCPKKIIBFB item in KJFKPMCPIBH.arguments)
			{
				double result = 0.0;
				double.TryParse(item.DCJLKCFKCOM, out result);
				if (num5 == 0)
				{
					num6 = result;
				}
				else
				{
					switch (LFLGCDNKNJI)
					{
					case KDEAPAPEEAO.MATH_SUM:
						num6 += result;
						break;
					case KDEAPAPEEAO.MATH_SUB:
						num6 -= result;
						break;
					case KDEAPAPEEAO.MATH_MULTI:
						num6 *= result;
						break;
					case KDEAPAPEEAO.MATH_DIVISION:
						if (result == 0.0)
						{
							LLLOJBFMONN.Error("ConditionExtension::MathFunction - wrong arguments division 0");
						}
						num6 /= result;
						break;
					default:
						LLLOJBFMONN.Error(string.Format("{0},{1}", "ConditionExtension::mathFunction - unknown type: ", LFLGCDNKNJI));
						break;
					}
				}
				num5++;
			}
			BMDEBHIHIAJ.resultNumber = num6;
			break;
		}
		case KDEAPAPEEAO.MATH_DIVISION_INT:
			if (KJFKPMCPIBH.arguments.Count == 2)
			{
				int num3 = int.Parse(KJFKPMCPIBH.arguments[0].DCJLKCFKCOM);
				int num4 = int.Parse(KJFKPMCPIBH.arguments[1].DCJLKCFKCOM);
				BMDEBHIHIAJ.resultNumber = num3 / num4;
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "ConditionExtension::MathFunction - wrong arguments count ", KJFKPMCPIBH.arguments.Count));
			}
			break;
		case KDEAPAPEEAO.MATH_MOD:
			if (KJFKPMCPIBH.arguments.Count == 2)
			{
				int num7 = int.Parse(KJFKPMCPIBH.arguments[0].DCJLKCFKCOM);
				int num8 = int.Parse(KJFKPMCPIBH.arguments[1].DCJLKCFKCOM);
				BMDEBHIHIAJ.resultNumber = num7 % num8;
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "ConditionExtension::MathFunction - wrong arguments count ", KJFKPMCPIBH.arguments.Count));
			}
			break;
		case KDEAPAPEEAO.MATH_RAND:
			if (KJFKPMCPIBH.arguments.Count == 2)
			{
				float num = float.Parse(KJFKPMCPIBH.arguments[0].DCJLKCFKCOM);
				float num2 = float.Parse(KJFKPMCPIBH.arguments[1].DCJLKCFKCOM);
				BMDEBHIHIAJ.resultNumber = NekkiMath.randomInt((int)num, (int)num2 + 1);
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "ConditionExtension::MathFunction - wrong arguments count ", KJFKPMCPIBH.arguments.Count));
			}
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},{1}", "ConditionExtension::mathFunction - unknown type: ", LFLGCDNKNJI));
			break;
		}
	}

	protected void MLPIEBOJBNM(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ, FBKBGPPHALB LFLGCDNKNJI)
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = new List<string>();
		foreach (HLCPKKIIBFB item in KJFKPMCPIBH.arguments)
		{
			string dCJLKCFKCOM = item.DCJLKCFKCOM;
			list.Add(dCJLKCFKCOM);
			if (num == 0)
			{
				stringBuilder.Clear();
				stringBuilder.Append(dCJLKCFKCOM);
			}
			else if (LFLGCDNKNJI == FBKBGPPHALB.STRING_CONCAT)
			{
				stringBuilder.Append(dCJLKCFKCOM);
			}
			num++;
		}
		if (list.Count >= 3 && LFLGCDNKNJI == FBKBGPPHALB.STRING_SLICE)
		{
			string iGGFGLLIGCG = list[0];
			int iOFHCAAOELD = int.Parse(list[1]);
			int iPMPAMAHLJG = int.Parse(list[2]);
			string value = StringFunctionSlice(iGGFGLLIGCG, iOFHCAAOELD, iPMPAMAHLJG);
			stringBuilder.Clear();
			stringBuilder.Append(value);
		}
		BMDEBHIHIAJ.resultSTR = stringBuilder.ToString();
	}

	protected string ClearGaps(string value)
	{
		if (value == null)
		{
			return string.Empty;
		}
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int length = value.Length; i < length; i++)
		{
			char c = value[i];
			bool flag2 = c.Equals('\'');
			if (!c.Equals(' '))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	protected string StringFunctionSlice(string IGGFGLLIGCG, int IOFHCAAOELD, int IPMPAMAHLJG)
	{
		if (IOFHCAAOELD > IGGFGLLIGCG.Length || IPMPAMAHLJG < IOFHCAAOELD || IOFHCAAOELD < 0 || IPMPAMAHLJG < 0)
		{
			return string.Empty;
		}
		int length = 1 + IPMPAMAHLJG - IOFHCAAOELD;
		if (1 + IPMPAMAHLJG > IGGFGLLIGCG.Length)
		{
			length = IGGFGLLIGCG.Length - IOFHCAAOELD;
		}
		return IGGFGLLIGCG.Substring(IOFHCAAOELD, length);
	}
}
