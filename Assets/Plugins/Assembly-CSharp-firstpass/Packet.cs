using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public sealed class Packet
{
	private enum LHHBMJEGIOF : byte
	{
		Textual = 0,
		Binary = 1
	}

	private const string Placeholder = "_placeholder";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HJDLGPHLPNF MHIBOFNNPIM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ECDAJBEFCAH BMNMAGPDJMO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int EHPCKDIJKIB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int DGDIGIMFGMI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HCHALPNMNMK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BALMFJPGGLO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KADNOPPLDGD;

	private List<byte[]> attachments;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool JIEAFKJJKKF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object[] MECKGDHKLCA;

	public HJDLGPHLPNF CFKMFAGNEGL
	{
		get
		{
			return FFJBNPEOAHI();
		}
		private set
		{
			EMGACMELDFH(value);
		}
	}

	public ECDAJBEFCAH NEBKEFEEEPI
	{
		get
		{
			return CMEHGNCCCIN();
		}
		private set
		{
			MLNKJLNGJPD(value);
		}
	}

	public int ELIDDCBKCMA
	{
		get
		{
			return AIHPGOGLBCE();
		}
		private set
		{
			NEMGKBJKDGO(value);
		}
	}

	public int Id
	{
		get
		{
			return IMMIJJCLPBO();
		}
		private set
		{
			MKAMABIPHEN(value);
		}
	}

	public string LBAMJPCNCNK
	{
		get
		{
			return NLHGDFGNIHB();
		}
		private set
		{
			ODECIOLOGDP(value);
		}
	}

	public string PEKKADLJCLG
	{
		get
		{
			return EJPGPOEEPFD();
		}
		private set
		{
			HIJPAHJJIIF(value);
		}
	}

	public List<byte[]> ECGBGPOFLFD
	{
		get
		{
			return BINAPGLGAGE();
		}
		set
		{
			set_Attachments(value);
		}
	}

	public bool AIONADLFOLM
	{
		get
		{
			return DGDMLLFEAME();
		}
	}

	public bool EMABMILBMFO
	{
		get
		{
			return KJFDJLNHKJI();
		}
		private set
		{
			set_IsDecoded(value);
		}
	}

	public object[] OJNPBCNPKDN
	{
		get
		{
			return BHLHOEIDGME();
		}
		private set
		{
			set_DecodedArgs(value);
		}
	}

	internal Packet()
	{
		EMGACMELDFH(HJDLGPHLPNF.Unknown);
		MLNKJLNGJPD(ECDAJBEFCAH.Unknown);
		ODECIOLOGDP(string.Empty);
	}

	internal Packet(string IOFHCAAOELD)
	{
		Parse(IOFHCAAOELD);
	}

	internal Packet(HJDLGPHLPNF JODJHEDGFDK, ECDAJBEFCAH MANDEJPGHBK, string JBALIKEKHGL, string OIINBGMDJKE, int MGOCOLCDIMG = 0, int OKNNNLIPODI = 0)
	{
		EMGACMELDFH(JODJHEDGFDK);
		MLNKJLNGJPD(MANDEJPGHBK);
		set_Namespace(JBALIKEKHGL);
		ODECIOLOGDP(OIINBGMDJKE);
		NEMGKBJKDGO(MGOCOLCDIMG);
		MKAMABIPHEN(OKNNNLIPODI);
	}

	public HJDLGPHLPNF FFJBNPEOAHI()
	{
		return MHIBOFNNPIM;
	}

	private void EMGACMELDFH(HJDLGPHLPNF value)
	{
		MHIBOFNNPIM = value;
	}

	public ECDAJBEFCAH CMEHGNCCCIN()
	{
		return BMNMAGPDJMO;
	}

	private void MLNKJLNGJPD(ECDAJBEFCAH value)
	{
		BMNMAGPDJMO = value;
	}

	public int AIHPGOGLBCE()
	{
		return EHPCKDIJKIB;
	}

	private void NEMGKBJKDGO(int value)
	{
		EHPCKDIJKIB = value;
	}

	public int IMMIJJCLPBO()
	{
		return DGDIGIMFGMI;
	}

	private void MKAMABIPHEN(int value)
	{
		DGDIGIMFGMI = value;
	}

	public string IONIEDIPEGB()
	{
		return HCHALPNMNMK;
	}

	private void set_Namespace(string value)
	{
		HCHALPNMNMK = value;
	}

	public string NLHGDFGNIHB()
	{
		return BALMFJPGGLO;
	}

	private void ODECIOLOGDP(string value)
	{
		BALMFJPGGLO = value;
	}

	public string EJPGPOEEPFD()
	{
		return KADNOPPLDGD;
	}

	private void HIJPAHJJIIF(string value)
	{
		KADNOPPLDGD = value;
	}

	public List<byte[]> BINAPGLGAGE()
	{
		return attachments;
	}

	public void set_Attachments(List<byte[]> value)
	{
		attachments = value;
		NEMGKBJKDGO((attachments != null) ? attachments.Count : 0);
	}

	public bool DGDMLLFEAME()
	{
		return BINAPGLGAGE() != null && BINAPGLGAGE().Count == AIHPGOGLBCE();
	}

	public bool KJFDJLNHKJI()
	{
		return JIEAFKJJKKF;
	}

	private void set_IsDecoded(bool value)
	{
		JIEAFKJJKKF = value;
	}

	public object[] BHLHOEIDGME()
	{
		return MECKGDHKLCA;
	}

	private void set_DecodedArgs(object[] value)
	{
		MECKGDHKLCA = value;
	}

	public object[] Decode(OOINGNLNJGM GLOJHMAIFOK)
	{
		if (KJFDJLNHKJI() || GLOJHMAIFOK == null)
		{
			return BHLHOEIDGME();
		}
		set_IsDecoded(true);
		if (string.IsNullOrEmpty(NLHGDFGNIHB()))
		{
			return BHLHOEIDGME();
		}
		List<object> list = GLOJHMAIFOK.Decode(NLHGDFGNIHB());
		if (list != null && list.Count > 0)
		{
			if (CMEHGNCCCIN() == ECDAJBEFCAH.Ack || CMEHGNCCCIN() == ECDAJBEFCAH.BinaryAck)
			{
				set_DecodedArgs(list.ToArray());
			}
			else
			{
				list.RemoveAt(0);
				set_DecodedArgs(list.ToArray());
			}
		}
		return BHLHOEIDGME();
	}

	public string EFJKNHMALOL()
	{
		if (!string.IsNullOrEmpty(EJPGPOEEPFD()))
		{
			return EJPGPOEEPFD();
		}
		if (string.IsNullOrEmpty(NLHGDFGNIHB()))
		{
			return string.Empty;
		}
		if (NLHGDFGNIHB()[0] != '[')
		{
			return string.Empty;
		}
		int i;
		for (i = 1; NLHGDFGNIHB().Length > i && NLHGDFGNIHB()[i] != '"' && NLHGDFGNIHB()[i] != '\''; i++)
		{
		}
		if (NLHGDFGNIHB().Length <= i)
		{
			return string.Empty;
		}
		int num = ++i;
		for (; NLHGDFGNIHB().Length > i && NLHGDFGNIHB()[i] != '"' && NLHGDFGNIHB()[i] != '\''; i++)
		{
		}
		if (NLHGDFGNIHB().Length <= i)
		{
			return string.Empty;
		}
		string text = NLHGDFGNIHB().Substring(num, i - num);
		HIJPAHJJIIF(text);
		return text;
	}

	public string RemoveEventName(bool KGECPDKNJNN)
	{
		if (string.IsNullOrEmpty(NLHGDFGNIHB()))
		{
			return string.Empty;
		}
		if (NLHGDFGNIHB()[0] != '[')
		{
			return string.Empty;
		}
		int i;
		for (i = 1; NLHGDFGNIHB().Length > i && NLHGDFGNIHB()[i] != '"' && NLHGDFGNIHB()[i] != '\''; i++)
		{
		}
		if (NLHGDFGNIHB().Length <= i)
		{
			return string.Empty;
		}
		int num = i;
		for (; NLHGDFGNIHB().Length > i && NLHGDFGNIHB()[i] != ',' && NLHGDFGNIHB()[i] != ']'; i++)
		{
		}
		if (NLHGDFGNIHB().Length <= ++i)
		{
			return string.Empty;
		}
		string text = NLHGDFGNIHB().Remove(num, i - num);
		if (KGECPDKNJNN)
		{
			text = text.Substring(1, text.Length - 2);
		}
		return text;
	}

	public bool FBBPHOCINHN()
	{
		return PlaceholderReplacer((string EMDHMHOKGFP, Dictionary<string, object> AOMLCBHAJJH) =>
		{
			int num = Convert.ToInt32(AOMLCBHAJJH["num"]);
			ODECIOLOGDP(NLHGDFGNIHB().Replace(EMDHMHOKGFP, num.ToString()));
			set_IsDecoded(false);
		});
	}

	public bool NDMDFFCGINF()
	{
		if (!DGDMLLFEAME())
		{
			return false;
		}
		return PlaceholderReplacer((string EMDHMHOKGFP, Dictionary<string, object> AOMLCBHAJJH) =>
		{
			int index = Convert.ToInt32(AOMLCBHAJJH["num"]);
			ODECIOLOGDP(NLHGDFGNIHB().Replace(EMDHMHOKGFP, string.Format("\"{0}\"", Convert.ToBase64String(BINAPGLGAGE()[index]))));
			set_IsDecoded(false);
		});
	}

	internal void Parse(string IOFHCAAOELD)
	{
		int i = 0;
		EMGACMELDFH((HJDLGPHLPNF)char.GetNumericValue(IOFHCAAOELD, i++));
		if (IOFHCAAOELD.Length > i && char.GetNumericValue(IOFHCAAOELD, i) >= 0.0)
		{
			MLNKJLNGJPD((ECDAJBEFCAH)char.GetNumericValue(IOFHCAAOELD, i++));
		}
		else
		{
			MLNKJLNGJPD(ECDAJBEFCAH.Unknown);
		}
		if (CMEHGNCCCIN() == ECDAJBEFCAH.BinaryEvent || CMEHGNCCCIN() == ECDAJBEFCAH.BinaryAck)
		{
			int num = IOFHCAAOELD.IndexOf('-', i);
			if (num == -1)
			{
				num = IOFHCAAOELD.Length;
			}
			int result = 0;
			int.TryParse(IOFHCAAOELD.Substring(i, num - i), out result);
			NEMGKBJKDGO(result);
			i = num + 1;
		}
		if (IOFHCAAOELD.Length > i && IOFHCAAOELD[i] == '/')
		{
			int num2 = IOFHCAAOELD.IndexOf(',', i);
			if (num2 == -1)
			{
				num2 = IOFHCAAOELD.Length;
			}
			set_Namespace(IOFHCAAOELD.Substring(i, num2 - i));
			i = num2 + 1;
		}
		else
		{
			set_Namespace("/");
		}
		if (IOFHCAAOELD.Length > i && char.GetNumericValue(IOFHCAAOELD[i]) >= 0.0)
		{
			int num3 = i++;
			for (; IOFHCAAOELD.Length > i && char.GetNumericValue(IOFHCAAOELD[i]) >= 0.0; i++)
			{
			}
			int result2 = 0;
			int.TryParse(IOFHCAAOELD.Substring(num3, i - num3), out result2);
			MKAMABIPHEN(result2);
		}
		if (IOFHCAAOELD.Length > i)
		{
			ODECIOLOGDP(IOFHCAAOELD.Substring(i));
		}
		else
		{
			ODECIOLOGDP(string.Empty);
		}
	}

	internal string Encode()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (FFJBNPEOAHI() == HJDLGPHLPNF.Unknown && AIHPGOGLBCE() > 0)
		{
			EMGACMELDFH(HJDLGPHLPNF.Message);
		}
		if (FFJBNPEOAHI() != HJDLGPHLPNF.Unknown)
		{
			stringBuilder.Append(((int)FFJBNPEOAHI()/*cast due to constrained. prefix*/).ToString());
		}
		if (CMEHGNCCCIN() == ECDAJBEFCAH.Unknown && AIHPGOGLBCE() > 0)
		{
			MLNKJLNGJPD(ECDAJBEFCAH.BinaryEvent);
		}
		if (CMEHGNCCCIN() != ECDAJBEFCAH.Unknown)
		{
			stringBuilder.Append(((int)CMEHGNCCCIN()/*cast due to constrained. prefix*/).ToString());
		}
		if (CMEHGNCCCIN() == ECDAJBEFCAH.BinaryEvent || CMEHGNCCCIN() == ECDAJBEFCAH.BinaryAck)
		{
			stringBuilder.Append(AIHPGOGLBCE().ToString());
			stringBuilder.Append("-");
		}
		bool flag = false;
		if (IONIEDIPEGB() != "/")
		{
			stringBuilder.Append(IONIEDIPEGB());
			flag = true;
		}
		if (IMMIJJCLPBO() != 0)
		{
			if (flag)
			{
				stringBuilder.Append(",");
				flag = false;
			}
			stringBuilder.Append(IMMIJJCLPBO().ToString());
		}
		if (!string.IsNullOrEmpty(NLHGDFGNIHB()))
		{
			if (flag)
			{
				stringBuilder.Append(",");
				flag = false;
			}
			stringBuilder.Append(NLHGDFGNIHB());
		}
		return stringBuilder.ToString();
	}

	internal byte[] KOLJHOEKHLI()
	{
		if (AIHPGOGLBCE() != 0 || (BINAPGLGAGE() != null && BINAPGLGAGE().Count != 0))
		{
			if (BINAPGLGAGE() == null)
			{
				throw new ArgumentException("packet.Attachments are null!");
			}
			if (AIHPGOGLBCE() != BINAPGLGAGE().Count)
			{
				throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
			}
		}
		string s = Encode();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] array = MHFBCMFOBPN(bytes, LHHBMJEGIOF.Textual, null);
		if (AIHPGOGLBCE() != 0)
		{
			int num = array.Length;
			List<byte[]> list = new List<byte[]>(AIHPGOGLBCE());
			int num2 = 0;
			for (int i = 0; i < AIHPGOGLBCE(); i++)
			{
				byte[] array2 = MHFBCMFOBPN(BINAPGLGAGE()[i], LHHBMJEGIOF.Binary, new byte[1] { 4 });
				list.Add(array2);
				num2 += array2.Length;
			}
			Array.Resize(ref array, array.Length + num2);
			for (int j = 0; j < AIHPGOGLBCE(); j++)
			{
				byte[] array3 = list[j];
				Array.Copy(array3, 0, array, num, array3.Length);
				num += array3.Length;
			}
		}
		return array;
	}

	internal void AddAttachmentFromServer(byte[] data, bool FEEFOCCJIML)
	{
		if (data != null && data.Length != 0)
		{
			if (attachments == null)
			{
				attachments = new List<byte[]>(AIHPGOGLBCE());
			}
			if (FEEFOCCJIML)
			{
				BINAPGLGAGE().Add(data);
				return;
			}
			byte[] array = new byte[data.Length - 1];
			Array.Copy(data, 1, array, 0, data.Length - 1);
			BINAPGLGAGE().Add(array);
		}
	}

	private byte[] MHFBCMFOBPN(byte[] data, LHHBMJEGIOF LFLGCDNKNJI, byte[] BGFJJKCBOJI)
	{
		int num = ((BGFJJKCBOJI != null) ? BGFJJKCBOJI.Length : 0);
		string text = (data.Length + num).ToString();
		byte[] array = new byte[text.Length];
		for (int i = 0; i < text.Length; i++)
		{
			array[i] = (byte)char.GetNumericValue(text[i]);
		}
		byte[] array2 = new byte[data.Length + array.Length + 2 + num];
		array2[0] = (byte)LFLGCDNKNJI;
		for (int j = 0; j < array.Length; j++)
		{
			array2[1 + j] = array[j];
		}
		int num2 = 1 + array.Length;
		array2[num2++] = byte.MaxValue;
		if (BGFJJKCBOJI != null && BGFJJKCBOJI.Length > 0)
		{
			Array.Copy(BGFJJKCBOJI, 0, array2, num2, BGFJJKCBOJI.Length);
			num2 += BGFJJKCBOJI.Length;
		}
		Array.Copy(data, 0, array2, num2, data.Length);
		return array2;
	}

	private bool PlaceholderReplacer(Action<string, Dictionary<string, object>> FOACGDMKGNH)
	{
		if (string.IsNullOrEmpty(NLHGDFGNIHB()))
		{
			return false;
		}
		for (int num = NLHGDFGNIHB().IndexOf("_placeholder"); num >= 0; num = NLHGDFGNIHB().IndexOf("_placeholder"))
		{
			int num2 = num;
			while (NLHGDFGNIHB()[num2] != '{')
			{
				num2--;
			}
			int i;
			for (i = num; NLHGDFGNIHB().Length > i && NLHGDFGNIHB()[i] != '}'; i++)
			{
			}
			if (NLHGDFGNIHB().Length <= i)
			{
				return false;
			}
			string text = NLHGDFGNIHB().Substring(num2, i - num2 + 1);
			bool IBFAPIMOMBA = false;
			Dictionary<string, object> dictionary = Json.Decode(text, ref IBFAPIMOMBA) as Dictionary<string, object>;
			if (!IBFAPIMOMBA)
			{
				return false;
			}
			object value;
			if (!dictionary.TryGetValue("_placeholder", out value) || !(bool)value)
			{
				return false;
			}
			if (!dictionary.TryGetValue("num", out value))
			{
				return false;
			}
			FOACGDMKGNH(text, dictionary);
		}
		return true;
	}

	public override string ToString()
	{
		return NLHGDFGNIHB();
	}

	internal Packet Clone()
	{
		Packet cMPKPLIGKLC = new Packet(FFJBNPEOAHI(), CMEHGNCCCIN(), IONIEDIPEGB(), NLHGDFGNIHB(), 0, IMMIJJCLPBO());
		cMPKPLIGKLC.HIJPAHJJIIF(EJPGPOEEPFD());
		cMPKPLIGKLC.NEMGKBJKDGO(AIHPGOGLBCE());
		cMPKPLIGKLC.attachments = attachments;
		return cMPKPLIGKLC;
	}
}
