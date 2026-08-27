using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class HTTPResponse : IDisposable
{
	internal const byte DGDNNNCFKLL = 13;

	internal const byte LIGDEEPMLPF = 10;

	public const int MinBufferSize = 4096;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int KJPNOCBOGBL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int AJOOGBMJLHE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int GFNNKFEAKOF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AGEPKFMGHGA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FMACGENCLBL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HPKEIBAGFIO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool PNDNAMEJEGK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, List<string>> BOEIOCLGPDI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] JFKBADLJJBM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool BOEELGOJIJD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<Cookie> POHEINBEINN;

	protected string EPOPPOGJPMI;

	protected Texture2D texture;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CENCKNPEDFK;

	internal HTTPRequest KEEGKCNNPGM;

	protected Stream Stream;

	protected List<byte[]> streamedFragments;

	protected object SyncRoot = new object();

	protected byte[] GACBHEJGAHJ;

	protected int CGLDCBJFPKA;

	protected Stream LLBCFJNMEHJ;

	protected int EHIGJOACCNJ;

	public int IJONNHDDJLC
	{
		get
		{
			return NPPCNJOCKIM();
		}
		protected set
		{
			LPFNOIBIJFP(value);
		}
	}

	public int GMJIPPKIADJ
	{
		get
		{
			return FOOKHAFFFIF();
		}
		protected set
		{
			EHGCOEFJAHF(value);
		}
	}

	public int POPGAABILGI
	{
		get
		{
			return KNMDPGBPNED();
		}
		protected set
		{
			ADBHCKLPIFN(value);
		}
	}

	public bool BFIKOBDDHCD
	{
		get
		{
			return AICKPAMONBH();
		}
	}

	public bool COFIJEAHEOM
	{
		get
		{
			return HOKPOJABMPK();
		}
		protected set
		{
			HGGMCLNCADA(value);
		}
	}

	public bool KNPPNPBMKKH
	{
		get
		{
			return MJPPHHLMPEI();
		}
		internal set
		{
			OBGGJPGINGC(value);
		}
	}

	public bool OAHNBGENIPC
	{
		get
		{
			return LOHDBJLLKEE();
		}
		internal set
		{
			KBOCENDKCJO(value);
		}
	}

	public Dictionary<string, List<string>> CPNAPDCFCDL
	{
		get
		{
			return AJCCGKHBNML();
		}
		protected set
		{
			set_Headers(value);
		}
	}

	public bool BDDFFCAGFPE
	{
		get
		{
			return ODOHODEENIB();
		}
		protected set
		{
			GCDKHOCDONK(value);
		}
	}

	public List<Cookie> FPFLODAGEFD
	{
		get
		{
			return HNDADBHDOID();
		}
		internal set
		{
			PPLAPHMALFL(value);
		}
	}

	public string IDFMFCBBNGA
	{
		get
		{
			return DPBLPGKOEJB();
		}
	}

	public Texture2D OFKOFDLKGHG
	{
		get
		{
			return EKOAPMEHNAJ();
		}
	}

	public bool PKKDGGLBMCA
	{
		get
		{
			return MLPKGGIKCDF();
		}
		protected set
		{
			DFIAKBONHGB(value);
		}
	}

	internal HTTPResponse(HTTPRequest ONOCIELLAPL, Stream ABJIEFMMIEK, bool IBIIADCLKCH, bool PEAJIKCANHP)
	{
		KEEGKCNNPGM = ONOCIELLAPL;
		Stream = ABJIEFMMIEK;
		HGGMCLNCADA(IBIIADCLKCH);
		KBOCENDKCJO(PEAJIKCANHP);
		DFIAKBONHGB(false);
	}

	public int NPPCNJOCKIM()
	{
		return KJPNOCBOGBL;
	}

	protected void LPFNOIBIJFP(int value)
	{
		KJPNOCBOGBL = value;
	}

	public int FOOKHAFFFIF()
	{
		return AJOOGBMJLHE;
	}

	protected void EHGCOEFJAHF(int value)
	{
		AJOOGBMJLHE = value;
	}

	public int KNMDPGBPNED()
	{
		return GFNNKFEAKOF;
	}

	protected void ADBHCKLPIFN(int value)
	{
		GFNNKFEAKOF = value;
	}

	public bool AICKPAMONBH()
	{
		return (KNMDPGBPNED() >= 200 && KNMDPGBPNED() < 300) || KNMDPGBPNED() == 304;
	}

	public string DCKPMHKDLEJ()
	{
		return AGEPKFMGHGA;
	}

	protected void set_Message(string value)
	{
		AGEPKFMGHGA = value;
	}

	public bool HOKPOJABMPK()
	{
		return FMACGENCLBL;
	}

	protected void HGGMCLNCADA(bool value)
	{
		FMACGENCLBL = value;
	}

	public bool MJPPHHLMPEI()
	{
		return HPKEIBAGFIO;
	}

	internal void OBGGJPGINGC(bool value)
	{
		HPKEIBAGFIO = value;
	}

	public bool LOHDBJLLKEE()
	{
		return PNDNAMEJEGK;
	}

	internal void KBOCENDKCJO(bool value)
	{
		PNDNAMEJEGK = value;
	}

	public Dictionary<string, List<string>> AJCCGKHBNML()
	{
		return BOEIOCLGPDI;
	}

	protected void set_Headers(Dictionary<string, List<string>> value)
	{
		BOEIOCLGPDI = value;
	}

	public byte[] CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	internal void set_Data(byte[] value)
	{
		JFKBADLJJBM = value;
	}

	public bool ODOHODEENIB()
	{
		return BOEELGOJIJD;
	}

	protected void GCDKHOCDONK(bool value)
	{
		BOEELGOJIJD = value;
	}

	public List<Cookie> HNDADBHDOID()
	{
		return POHEINBEINN;
	}

	internal void PPLAPHMALFL(List<Cookie> value)
	{
		POHEINBEINN = value;
	}

	public string DPBLPGKOEJB()
	{
		if (CHIGLEKCFFN() == null)
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(EPOPPOGJPMI))
		{
			return EPOPPOGJPMI;
		}
		return EPOPPOGJPMI = Encoding.UTF8.GetString(CHIGLEKCFFN(), 0, CHIGLEKCFFN().Length);
	}

	public Texture2D EKOAPMEHNAJ()
	{
		if (CHIGLEKCFFN() == null)
		{
			return null;
		}
		if (texture != null)
		{
			return texture;
		}
		texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
		texture.LoadImage(CHIGLEKCFFN());
		return texture;
	}

	public bool MLPKGGIKCDF()
	{
		return CENCKNPEDFK;
	}

	protected void DFIAKBONHGB(bool value)
	{
		CENCKNPEDFK = value;
	}

	internal virtual bool Receive(int JHFPNBPNHEH = -1, bool NDCKHEGBAGO = true)
	{
		string empty = string.Empty;
		try
		{
			empty = JJFJFNEFOHK(Stream, 32);
		}
		catch
		{
			if (!KEEGKCNNPGM.CKLEKLGMEAG())
			{
				return false;
			}
			throw;
		}
		if (!KEEGKCNNPGM.CKLEKLGMEAG() && string.IsNullOrEmpty(empty))
		{
			return false;
		}
		string[] array = empty.Split('/', '.');
		LPFNOIBIJFP(int.Parse(array[1]));
		EHGCOEFJAHF(int.Parse(array[2]));
		string text = FOBCHHBKJDG(Stream, 32, 10);
		int result;
		if (KEEGKCNNPGM.CKLEKLGMEAG())
		{
			result = int.Parse(text);
		}
		else if (!int.TryParse(text, out result))
		{
			return false;
		}
		ADBHCKLPIFN(result);
		if (text.Length > 0 && (byte)text[text.Length - 1] != 10 && (byte)text[text.Length - 1] != 13)
		{
			set_Message(JJFJFNEFOHK(Stream, 10));
		}
		else
		{
			set_Message(string.Empty);
		}
		NEECNIHNFGI(Stream);
		GCDKHOCDONK(KNMDPGBPNED() == 101 && (HasHeaderWithValue("connection", "upgrade") || HasHeader("upgrade")));
		if (!NDCKHEGBAGO)
		{
			return true;
		}
		return ReadPayload(JHFPNBPNHEH);
	}

	protected bool ReadPayload(int JHFPNBPNHEH)
	{
		if (JHFPNBPNHEH != -1)
		{
			KBOCENDKCJO(true);
			ReadRaw(Stream, JHFPNBPNHEH);
			return true;
		}
		if ((KNMDPGBPNED() >= 100 && KNMDPGBPNED() < 200) || KNMDPGBPNED() == 204 || KNMDPGBPNED() == 304 || KEEGKCNNPGM.JCHNIGKBBMI() == LAAFHDKKJFL.Head)
		{
			return true;
		}
		if (HasHeaderWithValue("transfer-encoding", "chunked"))
		{
			ReadChunked(Stream);
		}
		else
		{
			List<string> list = GetHeaderValues("content-length");
			List<string> list2 = GetHeaderValues("content-range");
			if (list != null && list2 == null)
			{
				ReadRaw(Stream, int.Parse(list[0]));
			}
			else if (list2 != null)
			{
				HTTPRange jALPJGLIOFH = MFNPGKMKBMA();
				ReadRaw(Stream, jALPJGLIOFH.CCJEDKMCDHP() - jALPJGLIOFH.AHALHOCNCJK() + 1);
			}
			else
			{
				LLFGNKODCDG(Stream);
			}
		}
		return true;
	}

	protected void NEECNIHNFGI(Stream ABJIEFMMIEK)
	{
		string text = JJFJFNEFOHK(ABJIEFMMIEK, 58, 10).Trim();
		while (text != string.Empty)
		{
			string bAINMLLIKOL = JJFJFNEFOHK(ABJIEFMMIEK, 10);
			AddHeader(text, bAINMLLIKOL);
			text = JJFJFNEFOHK(ABJIEFMMIEK, 58, 10);
		}
	}

	protected void AddHeader(string name, string value)
	{
		name = name.ToLower();
		if (AJCCGKHBNML() == null)
		{
			set_Headers(new Dictionary<string, List<string>>());
		}
		List<string> list;
		if (!AJCCGKHBNML().TryGetValue(name, out list))
		{
			AJCCGKHBNML().Add(name, list = new List<string>(1));
		}
		list.Add(value);
	}

	public List<string> GetHeaderValues(string name)
	{
		if (AJCCGKHBNML() == null)
		{
			return null;
		}
		name = name.ToLower();
		List<string> value;
		if (!AJCCGKHBNML().TryGetValue(name, out value) || value.Count == 0)
		{
			return null;
		}
		return value;
	}

	public string GetFirstHeaderValue(string name)
	{
		if (AJCCGKHBNML() == null)
		{
			return null;
		}
		name = name.ToLower();
		List<string> value;
		if (!AJCCGKHBNML().TryGetValue(name, out value) || value.Count == 0)
		{
			return null;
		}
		return value[0];
	}

	public bool HasHeaderWithValue(string JOKIBHMEDAO, string value)
	{
		List<string> list = GetHeaderValues(JOKIBHMEDAO);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (string.Compare(list[i], value, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasHeader(string JOKIBHMEDAO)
	{
		List<string> list = GetHeaderValues(JOKIBHMEDAO);
		if (list == null)
		{
			return false;
		}
		return true;
	}

	public HTTPRange MFNPGKMKBMA()
	{
		List<string> list = GetHeaderValues("content-range");
		if (list == null)
		{
			return null;
		}
		string[] array = list[0].Split(new char[3] { ' ', '-', '/' }, StringSplitOptions.RemoveEmptyEntries);
		if (array[1] == "*")
		{
			return new HTTPRange(int.Parse(array[2]));
		}
		return new HTTPRange(int.Parse(array[1]), int.Parse(array[2]), (!(array[3] != "*")) ? (-1) : int.Parse(array[3]));
	}

	public static string JJFJFNEFOHK(Stream ABJIEFMMIEK, byte MOFIAGJPCNA)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int num = ABJIEFMMIEK.ReadByte();
			while (num != MOFIAGJPCNA && num != -1)
			{
				memoryStream.WriteByte((byte)num);
				num = ABJIEFMMIEK.ReadByte();
			}
			return memoryStream.ToArray().JBAOFMBHJND().Trim();
		}
	}

	public static string JJFJFNEFOHK(Stream ABJIEFMMIEK, byte ECEBLBGKFPF, byte NELKINPNIGD)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int num = ABJIEFMMIEK.ReadByte();
			while (num != ECEBLBGKFPF && num != NELKINPNIGD && num != -1)
			{
				memoryStream.WriteByte((byte)num);
				num = ABJIEFMMIEK.ReadByte();
			}
			return memoryStream.ToArray().JBAOFMBHJND().Trim();
		}
	}

	public static string FOBCHHBKJDG(Stream ABJIEFMMIEK, byte ECEBLBGKFPF, byte NELKINPNIGD)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int num = ABJIEFMMIEK.ReadByte();
			while (num != ECEBLBGKFPF && num != NELKINPNIGD && num != -1)
			{
				memoryStream.WriteByte((byte)num);
				num = ABJIEFMMIEK.ReadByte();
			}
			return memoryStream.ToArray().JBAOFMBHJND();
		}
	}

	protected int ReadChunkLength(Stream ABJIEFMMIEK)
	{
		string text = JJFJFNEFOHK(ABJIEFMMIEK, 10);
		string[] array = text.Split(';');
		string text2 = array[0];
		int result;
		if (int.TryParse(text2, NumberStyles.AllowHexSpecifier, null, out result))
		{
			return result;
		}
		throw new Exception(string.Format("Can't parse '{0}' as a hex number!", text2));
	}

	protected void ReadChunked(Stream ABJIEFMMIEK)
	{
		FJKBNCMIHAD();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int num = ReadChunkLength(ABJIEFMMIEK);
			byte[] array = new byte[num];
			int num2 = 0;
			KEEGKCNNPGM.HEEHALMDLPL(num);
			KEEGKCNNPGM.HNPAEADANKK(AICKPAMONBH() || LOHDBJLLKEE());
			while (num != 0)
			{
				if (array.Length < num)
				{
					Array.Resize(ref array, num);
				}
				int num3 = 0;
				CCKDHANEGHM();
				do
				{
					int num4 = ABJIEFMMIEK.Read(array, num3, num - num3);
					if (num4 == 0)
					{
						throw new Exception("The remote server closed the connection unexpectedly!");
					}
					num3 += num4;
				}
				while (num3 < num);
				if (KEEGKCNNPGM.MDEPOKKKKCL())
				{
					HGLEDODOADF(array, 0, num3);
				}
				else
				{
					memoryStream.Write(array, 0, num3);
				}
				JJFJFNEFOHK(ABJIEFMMIEK, 10);
				num2 += num3;
				num = ReadChunkLength(ABJIEFMMIEK);
				HTTPRequest kEEGKCNNPGM = KEEGKCNNPGM;
				kEEGKCNNPGM.HEEHALMDLPL(kEEGKCNNPGM.ELADIMFGGEO() + num);
				KEEGKCNNPGM.BHOHEPLCIOI(num2);
				KEEGKCNNPGM.HNPAEADANKK(AICKPAMONBH() || LOHDBJLLKEE());
			}
			if (KEEGKCNNPGM.MDEPOKKKKCL())
			{
				FBLEGPBMIFA();
			}
			NEECNIHNFGI(ABJIEFMMIEK);
			if (!KEEGKCNNPGM.MDEPOKKKKCL())
			{
				set_Data(DecodeStream(memoryStream));
			}
		}
	}

	internal void ReadRaw(Stream ABJIEFMMIEK, int HDIIBKGCCNB)
	{
		FJKBNCMIHAD();
		KEEGKCNNPGM.HEEHALMDLPL(HDIIBKGCCNB);
		KEEGKCNNPGM.HNPAEADANKK(AICKPAMONBH() || LOHDBJLLKEE());
		using (MemoryStream memoryStream = new MemoryStream((!KEEGKCNNPGM.MDEPOKKKKCL()) ? HDIIBKGCCNB : 0))
		{
			byte[] array = new byte[Math.Max(KEEGKCNNPGM.CKFPMFMHPGI(), 4096)];
			int num = 0;
			while (HDIIBKGCCNB > 0)
			{
				num = 0;
				CCKDHANEGHM();
				do
				{
					int num2 = ABJIEFMMIEK.Read(array, num, Math.Min(HDIIBKGCCNB, array.Length - num));
					if (num2 == 0)
					{
						throw new Exception("The remote server closed the connection unexpectedly!");
					}
					num += num2;
					HDIIBKGCCNB -= num2;
					HTTPRequest kEEGKCNNPGM = KEEGKCNNPGM;
					kEEGKCNNPGM.BHOHEPLCIOI(kEEGKCNNPGM.IBILKGBKKOI() + num2);
					KEEGKCNNPGM.HNPAEADANKK(AICKPAMONBH() || LOHDBJLLKEE());
				}
				while (num < array.Length && HDIIBKGCCNB > 0);
				if (KEEGKCNNPGM.MDEPOKKKKCL())
				{
					HGLEDODOADF(array, 0, num);
				}
				else
				{
					memoryStream.Write(array, 0, num);
				}
			}
			if (KEEGKCNNPGM.MDEPOKKKKCL())
			{
				FBLEGPBMIFA();
			}
			if (!KEEGKCNNPGM.MDEPOKKKKCL())
			{
				set_Data(DecodeStream(memoryStream));
			}
		}
	}

	protected void LLFGNKODCDG(Stream ABJIEFMMIEK)
	{
		NetworkStream networkStream = ABJIEFMMIEK as NetworkStream;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			byte[] array = new byte[Math.Max(KEEGKCNNPGM.CKFPMFMHPGI(), 4096)];
			int num = 0;
			int num2 = 0;
			do
			{
				num = 0;
				do
				{
					num2 = 0;
					if (networkStream != null)
					{
						for (int i = num; i < array.Length; i++)
						{
							if (!networkStream.DataAvailable)
							{
								break;
							}
							int num3 = ABJIEFMMIEK.ReadByte();
							if (num3 >= 0)
							{
								array[i] = (byte)num3;
								num2++;
								continue;
							}
							break;
						}
					}
					else
					{
						num2 = ABJIEFMMIEK.Read(array, num, array.Length - num);
					}
					num += num2;
					HTTPRequest kEEGKCNNPGM = KEEGKCNNPGM;
					kEEGKCNNPGM.BHOHEPLCIOI(kEEGKCNNPGM.IBILKGBKKOI() + num2);
					KEEGKCNNPGM.HEEHALMDLPL(KEEGKCNNPGM.IBILKGBKKOI());
					KEEGKCNNPGM.HNPAEADANKK(AICKPAMONBH() || LOHDBJLLKEE());
				}
				while (num < array.Length && num2 > 0);
				if (KEEGKCNNPGM.MDEPOKKKKCL())
				{
					HGLEDODOADF(array, 0, num);
				}
				else
				{
					memoryStream.Write(array, 0, num);
				}
			}
			while (num2 > 0);
			if (KEEGKCNNPGM.MDEPOKKKKCL())
			{
				FBLEGPBMIFA();
			}
			if (!KEEGKCNNPGM.MDEPOKKKKCL())
			{
				set_Data(DecodeStream(memoryStream));
			}
		}
	}

	protected byte[] DecodeStream(Stream JGFDPHDCFNL)
	{
		JGFDPHDCFNL.Seek(0L, SeekOrigin.Begin);
		List<string> list = ((!LOHDBJLLKEE()) ? GetHeaderValues("content-encoding") : null);
		Stream stream = null;
		if (list == null)
		{
			stream = JGFDPHDCFNL;
		}
		else
		{
			switch (list[0])
			{
			case "gzip":
				stream = new DMOMPOFCMJJ(JGFDPHDCFNL, KAOCBBMMFOG.Decompress);
				break;
			case "deflate":
				stream = new OPBDIMHHCMJ(JGFDPHDCFNL, KAOCBBMMFOG.Decompress);
				break;
			default:
				stream = JGFDPHDCFNL;
				break;
			}
		}
		using (MemoryStream memoryStream = new MemoryStream((int)JGFDPHDCFNL.Length))
		{
			byte[] array = new byte[1024];
			int num = 0;
			while ((num = stream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			return memoryStream.ToArray();
		}
	}

	protected void FJKBNCMIHAD()
	{
		if (!KEEGKCNNPGM.DCOLJJKGFGD() && KEEGKCNNPGM.MDEPOKKKKCL() && !LOHDBJLLKEE() && HTTPCacheService.HCCGCAKPOGB(KEEGKCNNPGM.DKAECMGPGOE(), KEEGKCNNPGM.JCHNIGKBBMI(), this))
		{
			LLBCFJNMEHJ = HTTPCacheService.KHGFDHOJOOG(KEEGKCNNPGM.DKAECMGPGOE(), this);
		}
		EHIGJOACCNJ = 0;
	}

	protected void HGLEDODOADF(byte[] buffer, int LCCLEFMKLPB, int BDBOAEGELMC)
	{
		if (GACBHEJGAHJ == null)
		{
			GACBHEJGAHJ = new byte[KEEGKCNNPGM.CKFPMFMHPGI()];
			CGLDCBJFPKA = 0;
		}
		if (CGLDCBJFPKA + BDBOAEGELMC <= KEEGKCNNPGM.CKFPMFMHPGI())
		{
			Array.Copy(buffer, LCCLEFMKLPB, GACBHEJGAHJ, CGLDCBJFPKA, BDBOAEGELMC);
			CGLDCBJFPKA += BDBOAEGELMC;
			if (CGLDCBJFPKA == KEEGKCNNPGM.CKFPMFMHPGI())
			{
				AddStreamedFragment(GACBHEJGAHJ);
				GACBHEJGAHJ = null;
				CGLDCBJFPKA = 0;
			}
		}
		else
		{
			int num = KEEGKCNNPGM.CKFPMFMHPGI() - CGLDCBJFPKA;
			HGLEDODOADF(buffer, LCCLEFMKLPB, num);
			HGLEDODOADF(buffer, LCCLEFMKLPB + num, BDBOAEGELMC - num);
		}
	}

	protected void FBLEGPBMIFA()
	{
		if (GACBHEJGAHJ != null)
		{
			Array.Resize(ref GACBHEJGAHJ, CGLDCBJFPKA);
			AddStreamedFragment(GACBHEJGAHJ);
			GACBHEJGAHJ = null;
			CGLDCBJFPKA = 0;
		}
		if (LLBCFJNMEHJ != null)
		{
			LLBCFJNMEHJ.Dispose();
			LLBCFJNMEHJ = null;
			HTTPCacheService.SetBodyLength(KEEGKCNNPGM.DKAECMGPGOE(), EHIGJOACCNJ);
		}
	}

	protected void AddStreamedFragment(byte[] buffer)
	{
		lock (SyncRoot)
		{
			if (streamedFragments == null)
			{
				streamedFragments = new List<byte[]>();
			}
			streamedFragments.Add(buffer);
			if (LLBCFJNMEHJ != null)
			{
				LLBCFJNMEHJ.Write(buffer, 0, buffer.Length);
				EHIGJOACCNJ += buffer.Length;
			}
		}
	}

	protected void CCKDHANEGHM()
	{
	}

	public List<byte[]> IOLFNBDPDDF()
	{
		lock (SyncRoot)
		{
			if (streamedFragments == null || streamedFragments.Count == 0)
			{
				return null;
			}
			List<byte[]> result = new List<byte[]>(streamedFragments);
			streamedFragments.Clear();
			return result;
		}
	}

	internal bool PNOCCDHAAHI()
	{
		lock (SyncRoot)
		{
			return streamedFragments != null && streamedFragments.Count > 0;
		}
	}

	internal void NOEMFDALAGD()
	{
		OBGGJPGINGC(true);
		Dispose();
	}

	public void Dispose()
	{
		if (LLBCFJNMEHJ != null)
		{
			LLBCFJNMEHJ.Dispose();
			LLBCFJNMEHJ = null;
		}
	}
}
