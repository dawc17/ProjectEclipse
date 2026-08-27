using System;
using System.Collections.Generic;
using System.IO;

public class JsonReader
{
	private static IDictionary<int, IDictionary<int, int[]>> parse_table;

	private Stack<int> automaton_stack;

	private int NHEIKKOGHHK;

	private int LNDNLHCPNCE;

	private bool HEAGLCOKJFC;

	private bool LGPODKLEMBP;

	private Lexer NFFOIEMPKNH;

	private bool PCKOPADDIEM;

	private bool DPDJCFNBHCF;

	private bool GBFEFPMEGNB;

	private TextReader reader;

	private bool LODIAFPNLBF;

	private bool LPHEJAENNGN;

	private object token_value;

	private GDDEBPANOCH JLFCBDKNAGP;

	public bool NIHDCFNOEMF
	{
		get
		{
			return CGHOOPPOBJO();
		}
		set
		{
			LEONKMNNHJC(value);
		}
	}

	public bool CBCDIFLPHAK
	{
		get
		{
			return MIPNPCEMOPG();
		}
		set
		{
			JKAFBNBJLCM(value);
		}
	}

	public bool GFODPGFAFDI
	{
		get
		{
			return AIGMJENINOM();
		}
		set
		{
			HJCGOJIMALO(value);
		}
	}

	public bool GEPCEOKMALO
	{
		get
		{
			return ELOPMJBDCEN();
		}
	}

	public bool JMFBDICDNFO
	{
		get
		{
			return LGNOGDFHEFA();
		}
	}

	public GDDEBPANOCH CJPJNFFJNGN
	{
		get
		{
			return EACDJONMMAP();
		}
	}

	public object Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	static JsonReader()
	{
		IMLJJDMLFAK();
	}

	public JsonReader(string HLCLNMCHIHP)
		: this(new StringReader(HLCLNMCHIHP), true)
	{
	}

	public JsonReader(TextReader reader)
		: this(reader, false)
	{
	}

	private JsonReader(TextReader reader, bool MMDCAOBCJDE)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		PCKOPADDIEM = false;
		DPDJCFNBHCF = false;
		GBFEFPMEGNB = false;
		automaton_stack = new Stack<int>();
		automaton_stack.Push(65553);
		automaton_stack.Push(65543);
		NFFOIEMPKNH = new Lexer(reader);
		LGPODKLEMBP = false;
		HEAGLCOKJFC = false;
		LPHEJAENNGN = true;
		this.reader = reader;
		LODIAFPNLBF = MMDCAOBCJDE;
	}

	public bool CGHOOPPOBJO()
	{
		return NFFOIEMPKNH.CGHOOPPOBJO();
	}

	public void LEONKMNNHJC(bool value)
	{
		NFFOIEMPKNH.LEONKMNNHJC(value);
	}

	public bool MIPNPCEMOPG()
	{
		return NFFOIEMPKNH.MIPNPCEMOPG();
	}

	public void JKAFBNBJLCM(bool value)
	{
		NFFOIEMPKNH.JKAFBNBJLCM(value);
	}

	public bool AIGMJENINOM()
	{
		return LPHEJAENNGN;
	}

	public void HJCGOJIMALO(bool value)
	{
		LPHEJAENNGN = value;
	}

	public bool ELOPMJBDCEN()
	{
		return LGPODKLEMBP;
	}

	public bool LGNOGDFHEFA()
	{
		return HEAGLCOKJFC;
	}

	public GDDEBPANOCH EACDJONMMAP()
	{
		return JLFCBDKNAGP;
	}

	public object OEAKCOHMIHH()
	{
		return token_value;
	}

	private static void IMLJJDMLFAK()
	{
		parse_table = new Dictionary<int, IDictionary<int, int[]>>();
		JLKFPJMCALC(ParserToken.Array);
		IDFBOMCCDOI(ParserToken.Array, 91, 91, 65549);
		JLKFPJMCALC(ParserToken.ArrayPrime);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 34, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 91, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 93, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 123, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 65537, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 65538, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 65539, 65550, 65551, 93);
		IDFBOMCCDOI(ParserToken.ArrayPrime, 65540, 65550, 65551, 93);
		JLKFPJMCALC(ParserToken.Object);
		IDFBOMCCDOI(ParserToken.Object, 123, 123, 65545);
		JLKFPJMCALC(ParserToken.ObjectPrime);
		IDFBOMCCDOI(ParserToken.ObjectPrime, 34, 65546, 65547, 125);
		IDFBOMCCDOI(ParserToken.ObjectPrime, 125, 125);
		JLKFPJMCALC(ParserToken.Pair);
		IDFBOMCCDOI(ParserToken.Pair, 34, 65552, 58, 65550);
		JLKFPJMCALC(ParserToken.PairRest);
		IDFBOMCCDOI(ParserToken.PairRest, 44, 44, 65546, 65547);
		IDFBOMCCDOI(ParserToken.PairRest, 125, 65554);
		JLKFPJMCALC(ParserToken.String);
		IDFBOMCCDOI(ParserToken.String, 34, 34, 65541, 34);
		JLKFPJMCALC(ParserToken.Text);
		IDFBOMCCDOI(ParserToken.Text, 91, 65548);
		IDFBOMCCDOI(ParserToken.Text, 123, 65544);
		JLKFPJMCALC(ParserToken.Value);
		IDFBOMCCDOI(ParserToken.Value, 34, 65552);
		IDFBOMCCDOI(ParserToken.Value, 91, 65548);
		IDFBOMCCDOI(ParserToken.Value, 123, 65544);
		IDFBOMCCDOI(ParserToken.Value, 65537, 65537);
		IDFBOMCCDOI(ParserToken.Value, 65538, 65538);
		IDFBOMCCDOI(ParserToken.Value, 65539, 65539);
		IDFBOMCCDOI(ParserToken.Value, 65540, 65540);
		JLKFPJMCALC(ParserToken.ValueRest);
		IDFBOMCCDOI(ParserToken.ValueRest, 44, 44, 65550, 65551);
		IDFBOMCCDOI(ParserToken.ValueRest, 93, 65554);
	}

	private static void IDFBOMCCDOI(ParserToken IBAKGENOEPH, int JNCFMKPIAHB, params int[] HGDAGCFFKNJ)
	{
		parse_table[(int)IBAKGENOEPH].Add(JNCFMKPIAHB, HGDAGCFFKNJ);
	}

	private static void JLKFPJMCALC(ParserToken HNBFMAKFJAM)
	{
		parse_table.Add((int)HNBFMAKFJAM, new Dictionary<int, int[]>());
	}

	private void ProcessNumber(string number)
	{
		double result;
		int result2;
		long result3;
		if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, out result))
		{
			JLFCBDKNAGP = GDDEBPANOCH.Double;
			token_value = result;
		}
		else if (int.TryParse(number, out result2))
		{
			JLFCBDKNAGP = GDDEBPANOCH.Int;
			token_value = result2;
		}
		else if (long.TryParse(number, out result3))
		{
			JLFCBDKNAGP = GDDEBPANOCH.Long;
			token_value = result3;
		}
		else
		{
			JLFCBDKNAGP = GDDEBPANOCH.Int;
			token_value = 0;
		}
	}

	private void PBIKFHDLCLG()
	{
		if (LNDNLHCPNCE == 91)
		{
			JLFCBDKNAGP = GDDEBPANOCH.ArrayStart;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 93)
		{
			JLFCBDKNAGP = GDDEBPANOCH.ArrayEnd;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 123)
		{
			JLFCBDKNAGP = GDDEBPANOCH.ObjectStart;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 125)
		{
			JLFCBDKNAGP = GDDEBPANOCH.ObjectEnd;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 34)
		{
			if (PCKOPADDIEM)
			{
				PCKOPADDIEM = false;
				DPDJCFNBHCF = true;
				return;
			}
			if (JLFCBDKNAGP == GDDEBPANOCH.None)
			{
				JLFCBDKNAGP = GDDEBPANOCH.String;
			}
			PCKOPADDIEM = true;
		}
		else if (LNDNLHCPNCE == 65541)
		{
			token_value = NFFOIEMPKNH.EODMEFCBIOM();
		}
		else if (LNDNLHCPNCE == 65539)
		{
			JLFCBDKNAGP = GDDEBPANOCH.Boolean;
			token_value = false;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 65540)
		{
			JLFCBDKNAGP = GDDEBPANOCH.Null;
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 65537)
		{
			ProcessNumber(NFFOIEMPKNH.EODMEFCBIOM());
			DPDJCFNBHCF = true;
		}
		else if (LNDNLHCPNCE == 65546)
		{
			JLFCBDKNAGP = GDDEBPANOCH.PropertyName;
		}
		else if (LNDNLHCPNCE == 65538)
		{
			JLFCBDKNAGP = GDDEBPANOCH.Boolean;
			token_value = true;
			DPDJCFNBHCF = true;
		}
	}

	private bool IFLJEOBDMCD()
	{
		if (LGPODKLEMBP)
		{
			return false;
		}
		NFFOIEMPKNH.NextToken();
		if (NFFOIEMPKNH.ELOPMJBDCEN())
		{
			Close();
			return false;
		}
		NHEIKKOGHHK = NFFOIEMPKNH.EACDJONMMAP();
		return true;
	}

	public void Close()
	{
		if (!LGPODKLEMBP)
		{
			LGPODKLEMBP = true;
			HEAGLCOKJFC = true;
			if (LODIAFPNLBF)
			{
				reader.Dispose();
			}
			reader = null;
		}
	}

	public bool Read()
	{
		if (LGPODKLEMBP)
		{
			return false;
		}
		if (HEAGLCOKJFC)
		{
			HEAGLCOKJFC = false;
			automaton_stack.Clear();
			automaton_stack.Push(65553);
			automaton_stack.Push(65543);
		}
		PCKOPADDIEM = false;
		DPDJCFNBHCF = false;
		JLFCBDKNAGP = GDDEBPANOCH.None;
		token_value = null;
		if (!GBFEFPMEGNB)
		{
			GBFEFPMEGNB = true;
			if (!IFLJEOBDMCD())
			{
				return false;
			}
		}
		while (true)
		{
			if (DPDJCFNBHCF)
			{
				if (automaton_stack.Peek() == 65553)
				{
					HEAGLCOKJFC = true;
				}
				return true;
			}
			LNDNLHCPNCE = automaton_stack.Pop();
			PBIKFHDLCLG();
			if (LNDNLHCPNCE == NHEIKKOGHHK)
			{
				if (!IFLJEOBDMCD())
				{
					break;
				}
				continue;
			}
			int[] array;
			try
			{
				array = parse_table[LNDNLHCPNCE][NHEIKKOGHHK];
			}
			catch (KeyNotFoundException iADJLHGKHGL)
			{
				throw new JsonException((ParserToken)NHEIKKOGHHK, iADJLHGKHGL);
			}
			if (array[0] != 65554)
			{
				for (int num = array.Length - 1; num >= 0; num--)
				{
					automaton_stack.Push(array[num]);
				}
			}
		}
		if (automaton_stack.Peek() != 65553)
		{
			throw new JsonException("Input doesn't evaluate to proper JSON text");
		}
		if (DPDJCFNBHCF)
		{
			return true;
		}
		return false;
	}
}
