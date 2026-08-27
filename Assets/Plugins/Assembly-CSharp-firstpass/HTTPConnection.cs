using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using SocketEx;

internal sealed class HTTPConnection : IDisposable
{
	private enum AKALLIGHOHC
	{
		None = 0,
		Reconnect = 1,
		Authenticate = 2,
		ProxyAuthenticate = 3
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NFIMMCFCEKO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AHFEJIOPFGP MKHEFCIEOCA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPRequest BOLACMMLMLJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime OIFJPKPOAKI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime PIPBEMBBOKM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPProxy FGGPKCKKPNB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri DCMBFNKDCIF;

	private TcpClient Client;

	private Stream Stream;

	private DateTime LNDDFLHCGIM;

	internal string BICNDKPNCEP
	{
		get
		{
			return JHAJFMBPEDL();
		}
		private set
		{
			set_ServerAddress(value);
		}
	}

	internal AHFEJIOPFGP AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		private set
		{
			set_State(value);
		}
	}

	internal bool HACILDMBCMN
	{
		get
		{
			return PMOPEALOIKF();
		}
	}

	internal bool DCHJDPCEODD
	{
		get
		{
			return OPIAGHNCFAM();
		}
	}

	internal HTTPRequest ECKDGLMPIHP
	{
		get
		{
			return ONLLAFBCPIJ();
		}
		private set
		{
			NJHEPFCAFAK(value);
		}
	}

	internal bool DGDKKNCNGGE
	{
		get
		{
			return KKDOCCALBEG();
		}
	}

	internal DateTime PIOONMEEKHO
	{
		get
		{
			return MJFPCJODODA();
		}
		private set
		{
			OJDGGOOJNJM(value);
		}
	}

	internal DateTime HEDGMGEBLOA
	{
		get
		{
			return MFFGFLEIPOC();
		}
		private set
		{
			HCKHGGHLFAN(value);
		}
	}

	internal HTTPProxy DEFLNIGINCO
	{
		get
		{
			return FHGBKFBCGCO();
		}
		private set
		{
			PNGMAECJHID(value);
		}
	}

	internal bool JKNCBCEAILF
	{
		get
		{
			return AOPIGGFCGHC();
		}
	}

	internal Uri IHKGJONICKF
	{
		get
		{
			return PHMNCEBDLKP();
		}
		private set
		{
			set_LastProcessedUri(value);
		}
	}

	internal HTTPConnection(string FDFCPOOHGLE)
	{
		set_ServerAddress(FDFCPOOHGLE);
		set_State(AHFEJIOPFGP.Initial);
		LNDDFLHCGIM = DateTime.UtcNow;
	}

	internal string JHAJFMBPEDL()
	{
		return NFIMMCFCEKO;
	}

	private void set_ServerAddress(string value)
	{
		NFIMMCFCEKO = value;
	}

	internal AHFEJIOPFGP FLBBFDNHJAJ()
	{
		return MKHEFCIEOCA;
	}

	private void set_State(AHFEJIOPFGP value)
	{
		MKHEFCIEOCA = value;
	}

	internal bool PMOPEALOIKF()
	{
		return FLBBFDNHJAJ() == AHFEJIOPFGP.Initial || FLBBFDNHJAJ() == AHFEJIOPFGP.Free;
	}

	internal bool OPIAGHNCFAM()
	{
		return FLBBFDNHJAJ() > AHFEJIOPFGP.Initial && FLBBFDNHJAJ() < AHFEJIOPFGP.Free;
	}

	internal HTTPRequest ONLLAFBCPIJ()
	{
		return BOLACMMLMLJ;
	}

	private void NJHEPFCAFAK(HTTPRequest value)
	{
		BOLACMMLMLJ = value;
	}

	internal bool KKDOCCALBEG()
	{
		return PMOPEALOIKF() && DateTime.UtcNow - LNDDFLHCGIM > HTTPManager.AAKIPAJACAH();
	}

	internal DateTime MJFPCJODODA()
	{
		return OIFJPKPOAKI;
	}

	private void OJDGGOOJNJM(DateTime value)
	{
		OIFJPKPOAKI = value;
	}

	internal DateTime MFFGFLEIPOC()
	{
		return PIPBEMBBOKM;
	}

	private void HCKHGGHLFAN(DateTime value)
	{
		PIPBEMBBOKM = value;
	}

	internal HTTPProxy FHGBKFBCGCO()
	{
		return FGGPKCKKPNB;
	}

	private void PNGMAECJHID(HTTPProxy value)
	{
		FGGPKCKKPNB = value;
	}

	internal bool AOPIGGFCGHC()
	{
		return FHGBKFBCGCO() != null;
	}

	internal Uri PHMNCEBDLKP()
	{
		return DCMBFNKDCIF;
	}

	private void set_LastProcessedUri(Uri value)
	{
		DCMBFNKDCIF = value;
	}

	internal void HDEHLIKBKJG(HTTPRequest ONOCIELLAPL)
	{
		if (FLBBFDNHJAJ() == AHFEJIOPFGP.Processing)
		{
			throw new Exception("Connection already processing a request!");
		}
		OJDGGOOJNJM(DateTime.MaxValue);
		set_State(AHFEJIOPFGP.Processing);
		NJHEPFCAFAK(ONOCIELLAPL);
		new System.Threading.Thread(ThreadFunc).Start();
	}

	internal void FFKAKHDIBGD()
	{
		if (FLBBFDNHJAJ() == AHFEJIOPFGP.TimedOut)
		{
			LNDDFLHCGIM = DateTime.MinValue;
		}
		set_State(AHFEJIOPFGP.Free);
		NJHEPFCAFAK(null);
	}

	private void ThreadFunc(object KKNOCIPBIIK)
	{
		bool flag = false;
		bool flag2 = false;
		AKALLIGHOHC aKALLIGHOHC = AKALLIGHOHC.None;
		try
		{
			if (!AOPIGGFCGHC() && ONLLAFBCPIJ().AOPIGGFCGHC())
			{
				PNGMAECJHID(ONLLAFBCPIJ().FHGBKFBCGCO());
			}
			if (DECKPAHHIDJ())
			{
				return;
			}
			if (Client != null && !Client.IsConnected())
			{
				Close();
			}
			do
			{
				if (aKALLIGHOHC == AKALLIGHOHC.Reconnect)
				{
					Close();
					System.Threading.Thread.Sleep(100);
				}
				set_LastProcessedUri(ONLLAFBCPIJ().DKAECMGPGOE());
				aKALLIGHOHC = AKALLIGHOHC.None;
				NDCILHIAPIK();
				if (FLBBFDNHJAJ() == AHFEJIOPFGP.AbortRequested)
				{
					throw new Exception("AbortRequested");
				}
				if (!ONLLAFBCPIJ().DCOLJJKGFGD())
				{
					HTTPCacheService.JGLDNKPBBGC(ONLLAFBCPIJ());
				}
				bool flag3 = false;
				try
				{
					ONLLAFBCPIJ().SendOutTo(Stream);
					flag3 = true;
				}
				catch (Exception ex)
				{
					Close();
					if (FLBBFDNHJAJ() == AHFEJIOPFGP.TimedOut)
					{
						throw new Exception("AbortRequested");
					}
					if (flag || ONLLAFBCPIJ().CKLEKLGMEAG())
					{
						throw ex;
					}
					flag = true;
					aKALLIGHOHC = AKALLIGHOHC.Reconnect;
				}
				if (!flag3)
				{
					continue;
				}
				bool flag4 = Receive();
				if (FLBBFDNHJAJ() == AHFEJIOPFGP.TimedOut)
				{
					throw new Exception("AbortRequested");
				}
				if (!flag4 && !flag && !ONLLAFBCPIJ().CKLEKLGMEAG())
				{
					flag = true;
					aKALLIGHOHC = AKALLIGHOHC.Reconnect;
				}
				if (ONLLAFBCPIJ().POGDKNCHIBG() == null)
				{
					continue;
				}
				switch (ONLLAFBCPIJ().POGDKNCHIBG().KNMDPGBPNED())
				{
				case 401:
				{
					string text3 = DigestStore.FindBest(ONLLAFBCPIJ().POGDKNCHIBG().GetHeaderValues("www-authenticate"));
					if (!string.IsNullOrEmpty(text3))
					{
						Digest kHNAPCOOAEF2 = DigestStore.NLJEDHBBPKK(ONLLAFBCPIJ().DKAECMGPGOE());
						kHNAPCOOAEF2.CKNNIILGPNN(text3);
						if (ONLLAFBCPIJ().HPKPFEOBIOC() != null && kHNAPCOOAEF2.IsUriProtected(ONLLAFBCPIJ().DKAECMGPGOE()) && (!ONLLAFBCPIJ().HasHeader("Authorization") || kHNAPCOOAEF2.OCBMLPLDMOO()))
						{
							aKALLIGHOHC = AKALLIGHOHC.Authenticate;
						}
					}
					break;
				}
				case 407:
				{
					if (!ONLLAFBCPIJ().AOPIGGFCGHC())
					{
						break;
					}
					string text2 = DigestStore.FindBest(ONLLAFBCPIJ().POGDKNCHIBG().GetHeaderValues("proxy-authenticate"));
					if (!string.IsNullOrEmpty(text2))
					{
						Digest kHNAPCOOAEF = DigestStore.NLJEDHBBPKK(ONLLAFBCPIJ().FHGBKFBCGCO().DNIJHGFINDG());
						kHNAPCOOAEF.CKNNIILGPNN(text2);
						if (ONLLAFBCPIJ().FHGBKFBCGCO().HPKPFEOBIOC() != null && kHNAPCOOAEF.IsUriProtected(ONLLAFBCPIJ().FHGBKFBCGCO().DNIJHGFINDG()) && (!ONLLAFBCPIJ().HasHeader("Proxy-Authorization") || kHNAPCOOAEF.OCBMLPLDMOO()))
						{
							aKALLIGHOHC = AKALLIGHOHC.ProxyAuthenticate;
						}
					}
					break;
				}
				case 301:
				case 302:
				case 307:
				case 308:
					if (ONLLAFBCPIJ().FJNLLEMJKDC() < ONLLAFBCPIJ().MNBNOBNFOJH())
					{
						HTTPRequest iPLGNIDJDCF = ONLLAFBCPIJ();
						iPLGNIDJDCF.NDCFOHCFKHE(iPLGNIDJDCF.FJNLLEMJKDC() + 1);
						string text = ONLLAFBCPIJ().POGDKNCHIBG().GetFirstHeaderValue("location");
						if (string.IsNullOrEmpty(text))
						{
							throw new MissingFieldException(string.Format("Got redirect status({0}) without 'location' header!", ONLLAFBCPIJ().POGDKNCHIBG().KNMDPGBPNED().ToString()));
						}
						Uri uri = GetRedirectUri(text);
						if (!ONLLAFBCPIJ().CallOnBeforeRedirection(uri))
						{
							HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HTTPConnection", "OnBeforeRedirection returned False");
							break;
						}
						ONLLAFBCPIJ().KKCENCBJJIJ("Host");
						ONLLAFBCPIJ().MMPFBNNMGED("Referer", ONLLAFBCPIJ().DKAECMGPGOE().ToString());
						ONLLAFBCPIJ().NPFNLBPENAC(uri);
						ONLLAFBCPIJ().AOMLIJAIJHE(null);
						bool flag5 = true;
						ONLLAFBCPIJ().MAKPGAOFDOD(flag5);
						flag2 = flag5;
					}
					break;
				}
				if (ONLLAFBCPIJ().IJJCLBHKMDJ())
				{
					CookieJar.Set(ONLLAFBCPIJ().POGDKNCHIBG());
				}
				ANGJGJLIODI();
				if (ONLLAFBCPIJ().POGDKNCHIBG() == null || (!ONLLAFBCPIJ().POGDKNCHIBG().MLPKGGIKCDF() && ONLLAFBCPIJ().POGDKNCHIBG().HasHeaderWithValue("connection", "close")))
				{
					Close();
				}
			}
			while (aKALLIGHOHC != AKALLIGHOHC.None);
		}
		catch (TimeoutException bAINMLLIKOL)
		{
			ONLLAFBCPIJ().AOMLIJAIJHE(null);
			ONLLAFBCPIJ().set_Exception(bAINMLLIKOL);
			ONLLAFBCPIJ().set_State(CFGBMHKCENK.ConnectionTimedOut);
			Close();
		}
		catch (Exception bAINMLLIKOL2)
		{
			if (ONLLAFBCPIJ() != null)
			{
				if (ONLLAFBCPIJ().MDEPOKKKKCL())
				{
					HTTPCacheService.DeleteEntity(ONLLAFBCPIJ().DKAECMGPGOE());
				}
				ONLLAFBCPIJ().AOMLIJAIJHE(null);
				switch (FLBBFDNHJAJ())
				{
				case AHFEJIOPFGP.AbortRequested:
					ONLLAFBCPIJ().set_State(CFGBMHKCENK.Aborted);
					break;
				case AHFEJIOPFGP.TimedOut:
					ONLLAFBCPIJ().set_State(CFGBMHKCENK.TimedOut);
					break;
				default:
					ONLLAFBCPIJ().set_Exception(bAINMLLIKOL2);
					ONLLAFBCPIJ().set_State(CFGBMHKCENK.Error);
					break;
				}
			}
			Close();
		}
		finally
		{
			if (ONLLAFBCPIJ() != null)
			{
				lock (HTTPManager.Locker)
				{
					if (ONLLAFBCPIJ() != null && ONLLAFBCPIJ().POGDKNCHIBG() != null && ONLLAFBCPIJ().POGDKNCHIBG().ODOHODEENIB())
					{
						set_State(AHFEJIOPFGP.Upgraded);
					}
					else
					{
						set_State(flag2 ? AHFEJIOPFGP.Redirected : ((Client != null) ? AHFEJIOPFGP.WaitForRecycle : AHFEJIOPFGP.Closed));
					}
					if (ONLLAFBCPIJ().FLBBFDNHJAJ() == CFGBMHKCENK.Processing && (FLBBFDNHJAJ() == AHFEJIOPFGP.Closed || FLBBFDNHJAJ() == AHFEJIOPFGP.WaitForRecycle))
					{
						if (ONLLAFBCPIJ().POGDKNCHIBG() != null)
						{
							ONLLAFBCPIJ().set_State(CFGBMHKCENK.Finished);
						}
						else
						{
							ONLLAFBCPIJ().set_State(CFGBMHKCENK.Error);
						}
					}
					if (ONLLAFBCPIJ().FLBBFDNHJAJ() == CFGBMHKCENK.ConnectionTimedOut)
					{
						set_State(AHFEJIOPFGP.Closed);
					}
					LNDDFLHCGIM = DateTime.UtcNow;
				}
				HTTPCacheService.FIMLABMLKJF();
				CookieJar.AENFMDELLBM();
			}
		}
	}

	private void NDCILHIAPIK()
	{
		Uri uri = ((!ONLLAFBCPIJ().AOPIGGFCGHC()) ? ONLLAFBCPIJ().DKAECMGPGOE() : ONLLAFBCPIJ().FHGBKFBCGCO().DNIJHGFINDG());
		if (Client == null)
		{
			Client = new TcpClient();
		}
		if (!Client.Connected)
		{
			Client.ConnectTimeout = ONLLAFBCPIJ().DGHOJLHDGPB();
			Client.Connect(uri.Host, uri.Port);
			if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.Information)
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HTTPConnection", "Connected to " + uri.Host + ":" + uri.Port);
			}
		}
		else if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.Information)
		{
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HTTPConnection", "Already connected to " + uri.Host + ":" + uri.Port);
		}
		lock (HTTPManager.Locker)
		{
			OJDGGOOJNJM(DateTime.UtcNow);
		}
		if (Stream != null)
		{
			return;
		}
		bool flag = HTTPProtocolFactory.IsSecureProtocol(ONLLAFBCPIJ().DKAECMGPGOE());
		if (AOPIGGFCGHC() && (!FHGBKFBCGCO().JDBFAABAEIL() || (flag && FHGBKFBCGCO().OHCGKBPPMEN())))
		{
			Stream = Client.GetStream();
			BinaryWriter binaryWriter = new BinaryWriter(Stream);
			bool flag2;
			do
			{
				flag2 = false;
				binaryWriter.SendAsASCII(string.Format("CONNECT {0}:{1} HTTP/1.1", ONLLAFBCPIJ().DKAECMGPGOE().Host, ONLLAFBCPIJ().DKAECMGPGOE().Port));
				binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
				binaryWriter.SendAsASCII("Proxy-Connection: Keep-Alive");
				binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
				binaryWriter.SendAsASCII("Connection: Keep-Alive");
				binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
				binaryWriter.SendAsASCII(string.Format("Host: {0}:{1}", ONLLAFBCPIJ().DKAECMGPGOE().Host, ONLLAFBCPIJ().DKAECMGPGOE().Port));
				binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
				if (AOPIGGFCGHC() && FHGBKFBCGCO().HPKPFEOBIOC() != null)
				{
					switch (FHGBKFBCGCO().HPKPFEOBIOC().get_Type())
					{
					case BMBGFBGIAPL.Basic:
						binaryWriter.Write(string.Format("Proxy-Authorization: {0}", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(FHGBKFBCGCO().HPKPFEOBIOC().BFFCEKDPNAM() + ":" + FHGBKFBCGCO().HPKPFEOBIOC().LDEFEGOBBGO()))).GetASCIIBytes());
						binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
						break;
					case BMBGFBGIAPL.Unknown:
					case BMBGFBGIAPL.Digest:
					{
						Digest kHNAPCOOAEF = DigestStore.Get(FHGBKFBCGCO().DNIJHGFINDG());
						if (kHNAPCOOAEF != null)
						{
							string text = kHNAPCOOAEF.CIIGLAEHAOJ(ONLLAFBCPIJ(), FHGBKFBCGCO().HPKPFEOBIOC());
							if (!string.IsNullOrEmpty(text))
							{
								binaryWriter.Write(string.Format("Proxy-Authorization: {0}", text).GetASCIIBytes());
								binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
							}
						}
						break;
					}
					}
				}
				binaryWriter.Write(HTTPRequest.HGBANJPCEPF);
				binaryWriter.Flush();
				ONLLAFBCPIJ().FFBIEJDBKIL(new HTTPResponse(ONLLAFBCPIJ(), Stream, false, false));
				if (!ONLLAFBCPIJ().MJKNMBDFBID().Receive())
				{
					throw new Exception("Connection to the Proxy Server failed!");
				}
				if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.Information)
				{
					HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HTTPConnection", "Proxy returned - status code: " + ONLLAFBCPIJ().MJKNMBDFBID().KNMDPGBPNED() + " message: " + ONLLAFBCPIJ().MJKNMBDFBID().DCKPMHKDLEJ());
				}
				int num = ONLLAFBCPIJ().MJKNMBDFBID().KNMDPGBPNED();
				if (num == 407)
				{
					string text2 = DigestStore.FindBest(ONLLAFBCPIJ().MJKNMBDFBID().GetHeaderValues("proxy-authenticate"));
					if (!string.IsNullOrEmpty(text2))
					{
						Digest kHNAPCOOAEF2 = DigestStore.NLJEDHBBPKK(FHGBKFBCGCO().DNIJHGFINDG());
						kHNAPCOOAEF2.CKNNIILGPNN(text2);
						if (FHGBKFBCGCO().HPKPFEOBIOC() != null && kHNAPCOOAEF2.IsUriProtected(FHGBKFBCGCO().DNIJHGFINDG()) && (!ONLLAFBCPIJ().HasHeader("Proxy-Authorization") || kHNAPCOOAEF2.OCBMLPLDMOO()))
						{
							flag2 = true;
						}
					}
				}
				else if (!ONLLAFBCPIJ().MJKNMBDFBID().AICKPAMONBH())
				{
					throw new Exception(string.Format("Proxy returned Status Code: \"{0}\", Message: \"{1}\" and Response: {2}", ONLLAFBCPIJ().MJKNMBDFBID().KNMDPGBPNED(), ONLLAFBCPIJ().MJKNMBDFBID().DCKPMHKDLEJ(), ONLLAFBCPIJ().MJKNMBDFBID().DPBLPGKOEJB()));
				}
			}
			while (flag2);
		}
		if (flag)
		{
			if (ONLLAFBCPIJ().KMOEMMLAJNC())
			{
				TlsClientProtocol tlsClientProtocol = new TlsClientProtocol(Client.GetStream(), new SecureRandom());
				List<string> list = new List<string>(1);
				list.Add(ONLLAFBCPIJ().DKAECMGPGOE().Host);
				tlsClientProtocol.Connect(new LegacyTlsClient(ONLLAFBCPIJ().DKAECMGPGOE(), (ONLLAFBCPIJ().KNFEJHLHPDO() != null) ? ONLLAFBCPIJ().KNFEJHLHPDO() : new AlwaysValidVerifyer(), null, list));
				Stream = tlsClientProtocol.Stream;
				return;
			}
			SslStream sslStream = new SslStream(Client.GetStream(), false, (object ABONPDBPJBA, X509Certificate DBCFDLIJOBD, X509Chain GCONPBMJDFL, SslPolicyErrors FKDNIHKLCGP) => ONLLAFBCPIJ().IMMANGELKAN(DBCFDLIJOBD, GCONPBMJDFL));
			if (!sslStream.IsAuthenticated)
			{
				sslStream.AuthenticateAsClient(ONLLAFBCPIJ().DKAECMGPGOE().Host);
			}
			Stream = sslStream;
		}
		else
		{
			Stream = Client.GetStream();
		}
	}

	private bool Receive()
	{
		OBBKIBFJEMI eNLHAIGCCBO = ((ONLLAFBCPIJ().BEKFCACGBLL() != OBBKIBFJEMI.Unknown) ? ONLLAFBCPIJ().BEKFCACGBLL() : HTTPProtocolFactory.AOMOKHPFJFA(ONLLAFBCPIJ().DKAECMGPGOE()));
		ONLLAFBCPIJ().AOMLIJAIJHE(HTTPProtocolFactory.Get(eNLHAIGCCBO, ONLLAFBCPIJ(), Stream, ONLLAFBCPIJ().MDEPOKKKKCL(), false));
		if (!ONLLAFBCPIJ().POGDKNCHIBG().Receive())
		{
			ONLLAFBCPIJ().AOMLIJAIJHE(null);
			return false;
		}
		if (ONLLAFBCPIJ().POGDKNCHIBG().KNMDPGBPNED() == 304)
		{
			int BDBOAEGELMC;
			using (Stream aBJIEFMMIEK = HTTPCacheService.GetBody(ONLLAFBCPIJ().DKAECMGPGOE(), out BDBOAEGELMC))
			{
				if (!ONLLAFBCPIJ().POGDKNCHIBG().HasHeader("content-length"))
				{
					ONLLAFBCPIJ().POGDKNCHIBG().AJCCGKHBNML().Add("content-length", new List<string>(1) { BDBOAEGELMC.ToString() });
				}
				ONLLAFBCPIJ().POGDKNCHIBG().KBOCENDKCJO(true);
				ONLLAFBCPIJ().POGDKNCHIBG().ReadRaw(aBJIEFMMIEK, BDBOAEGELMC);
			}
		}
		return true;
	}

	private bool DECKPAHHIDJ()
	{
		if (ONLLAFBCPIJ().DCOLJJKGFGD() || !HTTPCacheService.EPACOIFEICA())
		{
			return false;
		}
		try
		{
			if (HTTPCacheService.BEBFIMACMEK(ONLLAFBCPIJ()))
			{
				ONLLAFBCPIJ().AOMLIJAIJHE(HTTPCacheService.HLLKJACMILI(ONLLAFBCPIJ()));
				if (ONLLAFBCPIJ().POGDKNCHIBG() != null)
				{
					return true;
				}
			}
		}
		catch
		{
			HTTPCacheService.DeleteEntity(ONLLAFBCPIJ().DKAECMGPGOE());
		}
		return false;
	}

	private void ANGJGJLIODI()
	{
		if (!ONLLAFBCPIJ().MDEPOKKKKCL() && !ONLLAFBCPIJ().DCOLJJKGFGD() && ONLLAFBCPIJ().POGDKNCHIBG() != null && HTTPCacheService.EPACOIFEICA() && HTTPCacheService.HCCGCAKPOGB(ONLLAFBCPIJ().DKAECMGPGOE(), ONLLAFBCPIJ().JCHNIGKBBMI(), ONLLAFBCPIJ().POGDKNCHIBG()))
		{
			HTTPCacheService.LDFKMIOPLKA(ONLLAFBCPIJ().DKAECMGPGOE(), ONLLAFBCPIJ().JCHNIGKBBMI(), ONLLAFBCPIJ().POGDKNCHIBG());
		}
	}

	private Uri GetRedirectUri(string LPJNEDFCBOI)
	{
		Uri uri = null;
		try
		{
			return new Uri(LPJNEDFCBOI);
		}
		catch (UriFormatException)
		{
			Uri uri2 = ONLLAFBCPIJ().OJBDMGBGJMA();
			UriBuilder uriBuilder = new UriBuilder(uri2.Scheme, uri2.Host, uri2.Port, LPJNEDFCBOI);
			return uriBuilder.Uri;
		}
	}

	internal void PNCNLDHGDLP()
	{
		if (ONLLAFBCPIJ().OGLIKFCADME != null && ONLLAFBCPIJ().BOABEDJEDDC())
		{
			try
			{
				ONLLAFBCPIJ().OGLIKFCADME(ONLLAFBCPIJ(), ONLLAFBCPIJ().IBILKGBKKOI(), ONLLAFBCPIJ().ELADIMFGGEO());
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPManager", "HandleProgressCallback - OnProgress", mPFFFAOGBJE);
			}
			ONLLAFBCPIJ().HNPAEADANKK(false);
		}
		if (ONLLAFBCPIJ().EEPGPFILKFI != null && ONLLAFBCPIJ().LCECFOLDKHH())
		{
			try
			{
				ONLLAFBCPIJ().EEPGPFILKFI(ONLLAFBCPIJ(), ONLLAFBCPIJ().MJBCCNEIBDA(), ONLLAFBCPIJ().LKHMFMMBAHL());
			}
			catch (Exception mPFFFAOGBJE2)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPManager", "HandleProgressCallback - OnUploadProgress", mPFFFAOGBJE2);
			}
			ONLLAFBCPIJ().MBNHNNCHJAG(false);
		}
	}

	internal void ICGOKIADHNK()
	{
		try
		{
			PNCNLDHGDLP();
			if (FLBBFDNHJAJ() == AHFEJIOPFGP.Upgraded)
			{
				if (ONLLAFBCPIJ() != null && ONLLAFBCPIJ().POGDKNCHIBG() != null && ONLLAFBCPIJ().POGDKNCHIBG().ODOHODEENIB())
				{
					ONLLAFBCPIJ().PPNNNGLBPFD();
				}
				set_State(AHFEJIOPFGP.WaitForProtocolShutdown);
			}
			else
			{
				ONLLAFBCPIJ().FLNDBIJDGMH();
			}
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPManager", "HandleCallback", mPFFFAOGBJE);
		}
	}

	internal void AKLEEMEHBIC(AHFEJIOPFGP MPJEMGJIBBD)
	{
		set_State(MPJEMGJIBBD);
		AHFEJIOPFGP aHFEJIOPFGP = FLBBFDNHJAJ();
		if (aHFEJIOPFGP == AHFEJIOPFGP.TimedOut)
		{
			HCKHGGHLFAN(DateTime.UtcNow);
		}
		if (Stream != null)
		{
			Stream.Dispose();
		}
	}

	private void Close()
	{
		set_LastProcessedUri(null);
		if (Client == null)
		{
			return;
		}
		try
		{
			Client.Close();
		}
		catch
		{
		}
		finally
		{
			Stream = null;
			Client = null;
		}
	}

	public void Dispose()
	{
		Close();
	}
}
