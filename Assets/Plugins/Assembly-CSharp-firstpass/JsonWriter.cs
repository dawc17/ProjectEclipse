using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class JsonWriter
{
	private static NumberFormatInfo number_format;

	private WriterContext PDCAHMPCPOC;

	private Stack<WriterContext> GANNNLHPFOF;

	private bool ENMNNCAJAKH;

	private char[] hex_seq;

	private int FCOACAMEHOE;

	private int OBDDDDDNAPB;

	private StringBuilder inst_string_builder;

	private bool PPEHFKKNOFP;

	private bool AKMKIMANJNB;

	private TextWriter writer;

	public int GPFDMIKIDPD
	{
		get
		{
			return FPECAKINHEH();
		}
		set
		{
			set_IndentValue(value);
		}
	}

	public bool MIDDOAJFLGK
	{
		get
		{
			return FJMNHDEIMOP();
		}
		set
		{
			PMMFMGBAOEC(value);
		}
	}

	public TextWriter JPKHJFKIBPE
	{
		get
		{
			return ONBOFGNMJEN();
		}
	}

	public bool FGCBJJKKILH
	{
		get
		{
			return EPCAKOLMCMC();
		}
		set
		{
			BHMCFLJJJNM(value);
		}
	}

	static JsonWriter()
	{
		number_format = NumberFormatInfo.InvariantInfo;
	}

	public JsonWriter()
	{
		inst_string_builder = new StringBuilder();
		writer = new StringWriter(inst_string_builder);
		Init();
	}

	public JsonWriter(StringBuilder NGPACMILENE)
		: this(new StringWriter(NGPACMILENE))
	{
	}

	public JsonWriter(TextWriter writer)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		this.writer = writer;
		Init();
	}

	public int FPECAKINHEH()
	{
		return OBDDDDDNAPB;
	}

	public void set_IndentValue(int value)
	{
		FCOACAMEHOE = FCOACAMEHOE / OBDDDDDNAPB * value;
		OBDDDDDNAPB = value;
	}

	public bool FJMNHDEIMOP()
	{
		return PPEHFKKNOFP;
	}

	public void PMMFMGBAOEC(bool value)
	{
		PPEHFKKNOFP = value;
	}

	public TextWriter ONBOFGNMJEN()
	{
		return writer;
	}

	public bool EPCAKOLMCMC()
	{
		return AKMKIMANJNB;
	}

	public void BHMCFLJJJNM(bool value)
	{
		AKMKIMANJNB = value;
	}

	private void KBPDDHBBNLA(KINIMNHPNLB AJEPDBPHNCM)
	{
		if (!PDCAHMPCPOC.DPMHKEGECAM)
		{
			PDCAHMPCPOC.Count++;
		}
		if (!AKMKIMANJNB)
		{
			return;
		}
		if (ENMNNCAJAKH)
		{
			throw new JsonException("A complete JSON symbol has already been written");
		}
		switch (AJEPDBPHNCM)
		{
		case KINIMNHPNLB.InArray:
			if (!PDCAHMPCPOC.HOILJAFJHLM)
			{
				throw new JsonException("Can't close an array here");
			}
			break;
		case KINIMNHPNLB.InObject:
			if (!PDCAHMPCPOC.MHABHHKLDFO || PDCAHMPCPOC.DPMHKEGECAM)
			{
				throw new JsonException("Can't close an object here");
			}
			break;
		case KINIMNHPNLB.NotAProperty:
			if (PDCAHMPCPOC.MHABHHKLDFO && !PDCAHMPCPOC.DPMHKEGECAM)
			{
				throw new JsonException("Expected a property");
			}
			break;
		case KINIMNHPNLB.Property:
			if (!PDCAHMPCPOC.MHABHHKLDFO || PDCAHMPCPOC.DPMHKEGECAM)
			{
				throw new JsonException("Can't add a property here");
			}
			break;
		case KINIMNHPNLB.Value:
			if (!PDCAHMPCPOC.HOILJAFJHLM && (!PDCAHMPCPOC.MHABHHKLDFO || !PDCAHMPCPOC.DPMHKEGECAM))
			{
				throw new JsonException("Can't add a value here");
			}
			break;
		}
	}

	private void Init()
	{
		ENMNNCAJAKH = false;
		hex_seq = new char[4];
		FCOACAMEHOE = 0;
		OBDDDDDNAPB = 4;
		PPEHFKKNOFP = false;
		AKMKIMANJNB = true;
		GANNNLHPFOF = new Stack<WriterContext>();
		PDCAHMPCPOC = new WriterContext();
		GANNNLHPFOF.Push(PDCAHMPCPOC);
	}

	private static void MCCNEABGNGB(int HDKKKCDKFEE, char[] IJGJLEJKMBJ)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = HDKKKCDKFEE % 16;
			if (num < 10)
			{
				IJGJLEJKMBJ[3 - i] = (char)(48 + num);
			}
			else
			{
				IJGJLEJKMBJ[3 - i] = (char)(65 + (num - 10));
			}
			HDKKKCDKFEE >>= 4;
		}
	}

	private void GOCGFMNIBAD()
	{
		if (PPEHFKKNOFP)
		{
			FCOACAMEHOE += OBDDDDDNAPB;
		}
	}

	private void CACLDGNEIFA(string IGGFGLLIGCG)
	{
		if (PPEHFKKNOFP && !PDCAHMPCPOC.DPMHKEGECAM)
		{
			for (int i = 0; i < FCOACAMEHOE; i++)
			{
				writer.Write(' ');
			}
		}
		writer.Write(IGGFGLLIGCG);
	}

	private void KLHPKPODIHH()
	{
		KLHPKPODIHH(true);
	}

	private void KLHPKPODIHH(bool GOLEKPDOAAP)
	{
		if (GOLEKPDOAAP && !PDCAHMPCPOC.DPMHKEGECAM && PDCAHMPCPOC.Count > 1)
		{
			writer.Write(',');
		}
		if (PPEHFKKNOFP && !PDCAHMPCPOC.DPMHKEGECAM)
		{
			writer.Write('\n');
		}
	}

	private void DIKLAKCMINM(string IGGFGLLIGCG)
	{
		CACLDGNEIFA(string.Empty);
		writer.Write('"');
		int length = IGGFGLLIGCG.Length;
		for (int i = 0; i < length; i++)
		{
			switch (IGGFGLLIGCG[i])
			{
			case '\n':
				writer.Write("\\n");
				continue;
			case '\r':
				writer.Write("\\r");
				continue;
			case '\t':
				writer.Write("\\t");
				continue;
			case '"':
			case '\\':
				writer.Write('\\');
				writer.Write(IGGFGLLIGCG[i]);
				continue;
			case '\f':
				writer.Write("\\f");
				continue;
			case '\b':
				writer.Write("\\b");
				continue;
			}
			if (IGGFGLLIGCG[i] >= ' ' && IGGFGLLIGCG[i] <= '~')
			{
				writer.Write(IGGFGLLIGCG[i]);
				continue;
			}
			MCCNEABGNGB(IGGFGLLIGCG[i], hex_seq);
			writer.Write("\\u");
			writer.Write(hex_seq);
		}
		writer.Write('"');
	}

	private void GBKGILPLBFE()
	{
		if (PPEHFKKNOFP)
		{
			FCOACAMEHOE -= OBDDDDDNAPB;
		}
	}

	public override string ToString()
	{
		if (inst_string_builder == null)
		{
			return string.Empty;
		}
		return inst_string_builder.ToString();
	}

	public void Reset()
	{
		ENMNNCAJAKH = false;
		GANNNLHPFOF.Clear();
		PDCAHMPCPOC = new WriterContext();
		GANNNLHPFOF.Push(PDCAHMPCPOC);
		if (inst_string_builder != null)
		{
			inst_string_builder.Remove(0, inst_string_builder.Length);
		}
	}

	public void Write(bool CIGMFMBICLJ)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		CACLDGNEIFA((!CIGMFMBICLJ) ? "false" : "true");
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(decimal number)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		CACLDGNEIFA(Convert.ToString(number, number_format));
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(double number)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		string text = Convert.ToString(number, number_format);
		CACLDGNEIFA(text);
		if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
		{
			writer.Write(".0");
		}
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(int number)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		CACLDGNEIFA(Convert.ToString(number, number_format));
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(long number)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		CACLDGNEIFA(Convert.ToString(number, number_format));
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(string IGGFGLLIGCG)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		if (IGGFGLLIGCG == null)
		{
			CACLDGNEIFA("null");
		}
		else
		{
			DIKLAKCMINM(IGGFGLLIGCG);
		}
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void Write(ulong number)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Value);
		KLHPKPODIHH();
		CACLDGNEIFA(Convert.ToString(number, number_format));
		PDCAHMPCPOC.DPMHKEGECAM = false;
	}

	public void FMIALOIGMFH()
	{
		KBPDDHBBNLA(KINIMNHPNLB.InArray);
		KLHPKPODIHH(false);
		GANNNLHPFOF.Pop();
		if (GANNNLHPFOF.Count == 1)
		{
			ENMNNCAJAKH = true;
		}
		else
		{
			PDCAHMPCPOC = GANNNLHPFOF.Peek();
			PDCAHMPCPOC.DPMHKEGECAM = false;
		}
		GBKGILPLBFE();
		CACLDGNEIFA("]");
	}

	public void AGGBIHCJOKF()
	{
		KBPDDHBBNLA(KINIMNHPNLB.NotAProperty);
		KLHPKPODIHH();
		CACLDGNEIFA("[");
		PDCAHMPCPOC = new WriterContext();
		PDCAHMPCPOC.HOILJAFJHLM = true;
		GANNNLHPFOF.Push(PDCAHMPCPOC);
		GOCGFMNIBAD();
	}

	public void KDAIDMBDFHB()
	{
		KBPDDHBBNLA(KINIMNHPNLB.InObject);
		KLHPKPODIHH(false);
		GANNNLHPFOF.Pop();
		if (GANNNLHPFOF.Count == 1)
		{
			ENMNNCAJAKH = true;
		}
		else
		{
			PDCAHMPCPOC = GANNNLHPFOF.Peek();
			PDCAHMPCPOC.DPMHKEGECAM = false;
		}
		GBKGILPLBFE();
		CACLDGNEIFA("}");
	}

	public void ACCDHGHBCHM()
	{
		KBPDDHBBNLA(KINIMNHPNLB.NotAProperty);
		KLHPKPODIHH();
		CACLDGNEIFA("{");
		PDCAHMPCPOC = new WriterContext();
		PDCAHMPCPOC.MHABHHKLDFO = true;
		GANNNLHPFOF.Push(PDCAHMPCPOC);
		GOCGFMNIBAD();
	}

	public void MPKEMEAPPJL(string MHJMMIJKOGH)
	{
		KBPDDHBBNLA(KINIMNHPNLB.Property);
		KLHPKPODIHH();
		DIKLAKCMINM(MHJMMIJKOGH);
		if (PPEHFKKNOFP)
		{
			if (MHJMMIJKOGH.Length > PDCAHMPCPOC.GAMMEFMGEFP)
			{
				PDCAHMPCPOC.GAMMEFMGEFP = MHJMMIJKOGH.Length;
			}
			for (int num = PDCAHMPCPOC.GAMMEFMGEFP - MHJMMIJKOGH.Length; num >= 0; num--)
			{
				writer.Write(' ');
			}
			writer.Write(": ");
		}
		else
		{
			writer.Write(':');
		}
		PDCAHMPCPOC.DPMHKEGECAM = true;
	}
}
