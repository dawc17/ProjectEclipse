using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;

public class Emitter : NEKGJNOFOFN
{
	private class JOIPMMGCJDB
	{
		public string KOLNNNLOCFE;

		public bool LCPNKFDMFIA;
	}

	private class PMJADAOHGKL
	{
		public string FODGADCGDBH;

		public string NCFFAGOLJEC;
	}

	private class APEFHDKHMCA
	{
		public string value;

		public bool IKBPKEEMMCA;

		public bool KBJBLFNOFKI;

		public bool KGHHDKEBHJP;

		public bool EFJAPPGNNIJ;

		public bool KHNFEFCDPKM;

		public IBEOFCPMMJJ KIGNIBIMLKK;
	}

	private const int ILLDKDDPKAO = 4;

	private const int BOLCAPFKBHO = 9;

	private const int PIKKOLCJLDD = 128;

	private static readonly Regex uriReplacer = new Regex("[^0-9A-Za-z_\\-;?@=$~\\\\\\)\\]/:&+,\\.\\*\\(\\[!]", RegexOptions.Singleline);

	private readonly TextWriter output;

	private readonly bool LGDHGOGFFCJ;

	private readonly int EMOCJNOCJKM;

	private readonly int CEIKEMJHLKL;

	private PFIHGFCNOEG state;

	private readonly Stack<PFIHGFCNOEG> LNJBMLMFKDH = new Stack<PFIHGFCNOEG>();

	private readonly Queue<ParsingEvent> DNBFFLFBDOB = new Queue<ParsingEvent>();

	private readonly Stack<int> indents = new Stack<int>();

	private readonly TagDirectiveCollection FMCEHNBELJF = new TagDirectiveCollection();

	private int AIBHPFBFGNA;

	private int FLNOCBGGCPP;

	private bool MLJMBHGNABP;

	private bool LMHKCBPDCNH;

	private bool MIBEGFBMLEC;

	private int DLPJJBPDNDE;

	private bool JNNJNNGLDHF;

	private bool GAMIKMDGHLL;

	private bool MIBHHFPMBID;

	private bool FDLHHPENADJ;

	private readonly JOIPMMGCJDB MOIAINBHLBA = new JOIPMMGCJDB();

	private readonly PMJADAOHGKL KLIMKJBCHOC = new PMJADAOHGKL();

	private readonly APEFHDKHMCA AKIFIIJIHGI = new APEFHDKHMCA();

	public Emitter(TextWriter output)
		: this(output, 4)
	{
	}

	public Emitter(TextWriter output, int EMOCJNOCJKM)
		: this(output, EMOCJNOCJKM, int.MaxValue)
	{
	}

	public Emitter(TextWriter output, int EMOCJNOCJKM, int CEIKEMJHLKL)
		: this(output, EMOCJNOCJKM, CEIKEMJHLKL, false)
	{
	}

	public Emitter(TextWriter output, int EMOCJNOCJKM, int CEIKEMJHLKL, bool LGDHGOGFFCJ)
	{
		if (EMOCJNOCJKM < 4 || EMOCJNOCJKM > 9)
		{
			throw new ArgumentOutOfRangeException("bestIndent", string.Format(CultureInfo.InvariantCulture, "The bestIndent parameter must be between {0} and {1}.", 4, 9));
		}
		this.EMOCJNOCJKM = EMOCJNOCJKM;
		if (CEIKEMJHLKL <= EMOCJNOCJKM * 2)
		{
			throw new ArgumentOutOfRangeException("bestWidth", "The bestWidth parameter must be greater than bestIndent * 2.");
		}
		this.CEIKEMJHLKL = CEIKEMJHLKL;
		this.LGDHGOGFFCJ = LGDHGOGFFCJ;
		this.output = output;
	}

	public void Emit(ParsingEvent KEAJCHAAIEP)
	{
		DNBFFLFBDOB.Enqueue(KEAJCHAAIEP);
		while (!PDPBHNAGIGF())
		{
			ParsingEvent iILOLJJLLGH = DNBFFLFBDOB.Peek();
			try
			{
				NBOGMMDPKHO(iILOLJJLLGH);
				GIKHOKIFGLH(iILOLJJLLGH);
			}
			finally
			{
				DNBFFLFBDOB.Dequeue();
			}
		}
	}

	private bool PDPBHNAGIGF()
	{
		if (DNBFFLFBDOB.Count == 0)
		{
			return true;
		}
		int num;
		switch (DNBFFLFBDOB.Peek().get_Type())
		{
		case BHBPOHDAGPH.DocumentStart:
			num = 1;
			break;
		case BHBPOHDAGPH.SequenceStart:
			num = 2;
			break;
		case BHBPOHDAGPH.MappingStart:
			num = 3;
			break;
		default:
			return false;
		}
		if (DNBFFLFBDOB.Count > num)
		{
			return false;
		}
		int num2 = 0;
		foreach (ParsingEvent item in DNBFFLFBDOB)
		{
			switch (item.get_Type())
			{
			case BHBPOHDAGPH.DocumentStart:
			case BHBPOHDAGPH.SequenceStart:
			case BHBPOHDAGPH.MappingStart:
				num2++;
				break;
			case BHBPOHDAGPH.DocumentEnd:
			case BHBPOHDAGPH.SequenceEnd:
			case BHBPOHDAGPH.MappingEnd:
				num2--;
				break;
			}
			if (num2 == 0)
			{
				return false;
			}
		}
		return true;
	}

	private void NBOGMMDPKHO(ParsingEvent IILOLJJLLGH)
	{
		MOIAINBHLBA.KOLNNNLOCFE = null;
		KLIMKJBCHOC.FODGADCGDBH = null;
		KLIMKJBCHOC.NCFFAGOLJEC = null;
		AnchorAlias mBEGNNDMDKH = IILOLJJLLGH as AnchorAlias;
		if (mBEGNNDMDKH != null)
		{
			JPNMIPPONMB(mBEGNNDMDKH.OEAKCOHMIHH(), true);
			return;
		}
		NodeEvent dGMPGIHHKCN = IILOLJJLLGH as NodeEvent;
		if (dGMPGIHHKCN != null)
		{
			Scalar lEACOCDHICF = IILOLJJLLGH as Scalar;
			if (lEACOCDHICF != null)
			{
				GAJLLJBNNND(lEACOCDHICF.OEAKCOHMIHH());
			}
			JPNMIPPONMB(dGMPGIHHKCN.HCPOJDFJFMM(), false);
			if (!string.IsNullOrEmpty(dGMPGIHHKCN.LOIGCKFONHJ()) && (LGDHGOGFFCJ || dGMPGIHHKCN.DOHAHEHOCLN()))
			{
				POKPCBFFJIL(dGMPGIHHKCN.LOIGCKFONHJ());
			}
		}
	}

	private void JPNMIPPONMB(string KOLNNNLOCFE, bool LCPNKFDMFIA)
	{
		MOIAINBHLBA.KOLNNNLOCFE = KOLNNNLOCFE;
		MOIAINBHLBA.LCPNKFDMFIA = LCPNKFDMFIA;
	}

	private void GAJLLJBNNND(string value)
	{
		AKIFIIJIHGI.value = value;
		if (value.Length == 0)
		{
			AKIFIIJIHGI.IKBPKEEMMCA = false;
			AKIFIIJIHGI.KBJBLFNOFKI = false;
			AKIFIIJIHGI.KGHHDKEBHJP = true;
			AKIFIIJIHGI.EFJAPPGNNIJ = true;
			AKIFIIJIHGI.KHNFEFCDPKM = false;
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (value.StartsWith("---", StringComparison.Ordinal) || value.StartsWith("...", StringComparison.Ordinal))
		{
			flag = true;
			flag2 = true;
		}
		CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
		bool flag3 = true;
		bool flag4 = characterAnalyzer.MKOKPKHBDMD(1);
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = false;
		bool flag11 = false;
		bool flag12 = false;
		bool flag13 = false;
		bool flag14 = false;
		bool flag15 = true;
		while (!characterAnalyzer.EndOfInput)
		{
			if (flag15)
			{
				if (characterAnalyzer.Check("#,[]{}&*!|>\\\"%@`"))
				{
					flag = true;
					flag2 = true;
				}
				if (characterAnalyzer.Check("?:"))
				{
					flag = true;
					if (flag4)
					{
						flag2 = true;
					}
				}
				if (characterAnalyzer.Check('-') && flag4)
				{
					flag = true;
					flag2 = true;
				}
			}
			else
			{
				if (characterAnalyzer.Check(",?[]{}"))
				{
					flag = true;
				}
				if (characterAnalyzer.Check(':'))
				{
					flag = true;
					if (flag4)
					{
						flag2 = true;
					}
				}
				if (characterAnalyzer.Check('#') && flag3)
				{
					flag = true;
					flag2 = true;
				}
			}
			if (!characterAnalyzer.IGNGBDLCMGB() || (!characterAnalyzer.EAMJHPLDDLE() && !IsUnicode(output.Encoding)))
			{
				flag14 = true;
			}
			if (characterAnalyzer.JCPPGIPDMBK())
			{
				flag13 = true;
			}
			if (characterAnalyzer.NBLLOLGNFGM())
			{
				if (flag15)
				{
					flag5 = true;
				}
				if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
				{
					flag7 = true;
				}
				if (flag12)
				{
					flag9 = true;
				}
				flag11 = true;
				flag12 = false;
			}
			else if (characterAnalyzer.JCPPGIPDMBK())
			{
				if (flag15)
				{
					flag6 = true;
				}
				if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
				{
					flag8 = true;
				}
				if (flag11)
				{
					flag10 = true;
				}
				flag11 = false;
				flag12 = true;
			}
			else
			{
				flag11 = false;
				flag12 = false;
			}
			flag3 = characterAnalyzer.MKOKPKHBDMD();
			characterAnalyzer.Skip(1);
			if (!characterAnalyzer.EndOfInput)
			{
				flag4 = characterAnalyzer.MKOKPKHBDMD(1);
			}
			flag15 = false;
		}
		AKIFIIJIHGI.KBJBLFNOFKI = true;
		AKIFIIJIHGI.KGHHDKEBHJP = true;
		AKIFIIJIHGI.EFJAPPGNNIJ = true;
		AKIFIIJIHGI.KHNFEFCDPKM = true;
		if (flag5 || flag6 || flag7 || flag8)
		{
			AKIFIIJIHGI.KBJBLFNOFKI = false;
			AKIFIIJIHGI.KGHHDKEBHJP = false;
		}
		if (flag7)
		{
			AKIFIIJIHGI.KHNFEFCDPKM = false;
		}
		if (flag9)
		{
			AKIFIIJIHGI.KBJBLFNOFKI = false;
			AKIFIIJIHGI.KGHHDKEBHJP = false;
			AKIFIIJIHGI.EFJAPPGNNIJ = false;
		}
		if (flag10 || flag14)
		{
			AKIFIIJIHGI.KBJBLFNOFKI = false;
			AKIFIIJIHGI.KGHHDKEBHJP = false;
			AKIFIIJIHGI.EFJAPPGNNIJ = false;
			AKIFIIJIHGI.KHNFEFCDPKM = false;
		}
		AKIFIIJIHGI.IKBPKEEMMCA = flag13;
		if (flag13)
		{
			AKIFIIJIHGI.KBJBLFNOFKI = false;
			AKIFIIJIHGI.KGHHDKEBHJP = false;
		}
		if (flag)
		{
			AKIFIIJIHGI.KBJBLFNOFKI = false;
		}
		if (flag2)
		{
			AKIFIIJIHGI.KGHHDKEBHJP = false;
		}
	}

	private bool IsUnicode(Encoding JIBCJOMMFCO)
	{
		return JIBCJOMMFCO.Equals(Encoding.UTF8) || JIBCJOMMFCO.Equals(Encoding.Unicode) || JIBCJOMMFCO.Equals(Encoding.BigEndianUnicode) || JIBCJOMMFCO.Equals(Encoding.UTF7) || JIBCJOMMFCO.Equals(Encoding.UTF32);
	}

	private void POKPCBFFJIL(string EDLADAAKMDF)
	{
		KLIMKJBCHOC.FODGADCGDBH = EDLADAAKMDF;
		foreach (TagDirective item in FMCEHNBELJF)
		{
			if (EDLADAAKMDF.StartsWith(item.Prefix, StringComparison.Ordinal))
			{
				KLIMKJBCHOC.FODGADCGDBH = item.Handle;
				KLIMKJBCHOC.NCFFAGOLJEC = EDLADAAKMDF.Substring(item.Prefix.Length);
				break;
			}
		}
	}

	private void GIKHOKIFGLH(ParsingEvent IILOLJJLLGH)
	{
		Comment mGMGDDOIHAJ = IILOLJJLLGH as Comment;
		if (mGMGDDOIHAJ != null)
		{
			CBLCGKALPNC(mGMGDDOIHAJ);
			return;
		}
		switch (state)
		{
		case PFIHGFCNOEG.StreamStart:
			AHIGMLMJDMO(IILOLJJLLGH);
			break;
		case PFIHGFCNOEG.FirstDocumentStart:
			IDIOKNDCNBH(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.DocumentStart:
			IDIOKNDCNBH(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.DocumentContent:
			OFOGFHMBDJF(IILOLJJLLGH);
			break;
		case PFIHGFCNOEG.DocumentEnd:
			IOPKCDJBPFJ(IILOLJJLLGH);
			break;
		case PFIHGFCNOEG.FlowSequenceFirstItem:
			MOGAEAKBOGN(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.FlowSequenceItem:
			MOGAEAKBOGN(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.FlowMappingFirstKey:
			JGPMFBFHPLG(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.FlowMappingKey:
			JGPMFBFHPLG(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.FlowMappingSimpleValue:
			LKMHLDHKDAM(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.FlowMappingValue:
			LKMHLDHKDAM(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.BlockSequenceFirstItem:
			BNNKMPFDJGP(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.BlockSequenceItem:
			BNNKMPFDJGP(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.BlockMappingFirstKey:
			OPAIFNHGJGA(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.BlockMappingKey:
			OPAIFNHGJGA(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.BlockMappingSimpleValue:
			PAGLCKDAPMK(IILOLJJLLGH, true);
			break;
		case PFIHGFCNOEG.BlockMappingValue:
			PAGLCKDAPMK(IILOLJJLLGH, false);
			break;
		case PFIHGFCNOEG.StreamEnd:
			throw new YamlException("Expected nothing after STREAM-END");
		default:
			throw new InvalidOperationException();
		}
	}

	private void CBLCGKALPNC(Comment MPMFGPGDGDN)
	{
		if (MPMFGPGDGDN.IGLENNPMPDJ())
		{
			Write(' ');
		}
		else
		{
			NMGCFFFIFPJ();
		}
		Write("# ");
		Write(MPMFGPGDGDN.OEAKCOHMIHH());
		GAMIKMDGHLL = true;
	}

	private void AHIGMLMJDMO(ParsingEvent IILOLJJLLGH)
	{
		if (!(IILOLJJLLGH is StreamStart))
		{
			throw new ArgumentException("Expected STREAM-START.", "evt");
		}
		AIBHPFBFGNA = -1;
		DLPJJBPDNDE = 0;
		JNNJNNGLDHF = true;
		GAMIKMDGHLL = true;
		state = PFIHGFCNOEG.FirstDocumentStart;
	}

	private void IDIOKNDCNBH(ParsingEvent IILOLJJLLGH, bool IKNHLPGLLKB)
	{
		DocumentStart aOGNBDOIKPE = IILOLJJLLGH as DocumentStart;
		if (aOGNBDOIKPE != null)
		{
			bool flag = aOGNBDOIKPE.BBBGHODAEIN() && IKNHLPGLLKB && !LGDHGOGFFCJ;
			TagDirectiveCollection iDHIKALFADG = GCFGDGHKLEI(aOGNBDOIKPE.FNNKPBJDMDF());
			if (!IKNHLPGLLKB && !FDLHHPENADJ && (aOGNBDOIKPE.KCJMMIEBLHL() != null || iDHIKALFADG.Count > 0))
			{
				FDLHHPENADJ = false;
				WriteIndicator("...", true, false, false);
				MDBMBMENEBP();
			}
			if (aOGNBDOIKPE.KCJMMIEBLHL() != null)
			{
				AnalyzeVersionDirective(aOGNBDOIKPE.KCJMMIEBLHL());
				flag = false;
				WriteIndicator("%YAML", true, false, false);
				WriteIndicator(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", 1, 1), true, false, false);
				MDBMBMENEBP();
			}
			foreach (TagDirective item in iDHIKALFADG)
			{
				PPFIAICBBBI(item, false, FMCEHNBELJF);
			}
			TagDirective[] gNPKLFKPLCM = CHOAMHPCPFL.DefaultTagDirectives;
			foreach (TagDirective bAINMLLIKOL in gNPKLFKPLCM)
			{
				PPFIAICBBBI(bAINMLLIKOL, true, FMCEHNBELJF);
			}
			if (iDHIKALFADG.Count > 0)
			{
				flag = false;
				TagDirective[] gNPKLFKPLCM2 = CHOAMHPCPFL.DefaultTagDirectives;
				foreach (TagDirective bAINMLLIKOL2 in gNPKLFKPLCM2)
				{
					PPFIAICBBBI(bAINMLLIKOL2, true, iDHIKALFADG);
				}
				foreach (TagDirective item2 in iDHIKALFADG)
				{
					WriteIndicator("%TAG", true, false, false);
					IMEIEEFKHCD(item2.Handle);
					DMCBHMNGJIP(item2.Prefix, true);
					MDBMBMENEBP();
				}
			}
			if (NOIMGKCEPOH())
			{
				flag = false;
			}
			if (!flag)
			{
				MDBMBMENEBP();
				WriteIndicator("---", true, false, false);
				if (LGDHGOGFFCJ)
				{
					MDBMBMENEBP();
				}
			}
			state = PFIHGFCNOEG.DocumentContent;
		}
		else
		{
			if (!(IILOLJJLLGH is HNKFEGCMBJB))
			{
				throw new YamlException("Expected DOCUMENT-START or STREAM-END");
			}
			if (MIBHHFPMBID)
			{
				WriteIndicator("...", true, false, false);
				MDBMBMENEBP();
			}
			state = PFIHGFCNOEG.StreamEnd;
		}
	}

	private TagDirectiveCollection GCFGDGHKLEI(IEnumerable<TagDirective> FIMJCFLNJIK)
	{
		TagDirectiveCollection iDHIKALFADG = new TagDirectiveCollection();
		if (FIMJCFLNJIK == null)
		{
			return iDHIKALFADG;
		}
		foreach (TagDirective item2 in FIMJCFLNJIK)
		{
			PPFIAICBBBI(item2, false, iDHIKALFADG);
		}
		TagDirective[] gNPKLFKPLCM = CHOAMHPCPFL.DefaultTagDirectives;
		foreach (TagDirective item in gNPKLFKPLCM)
		{
			iDHIKALFADG.Remove(item);
		}
		return iDHIKALFADG;
	}

	private void AnalyzeVersionDirective(VersionDirective JMCEGKIENKI)
	{
		if (JMCEGKIENKI.Version.Major != 1 || JMCEGKIENKI.Version.Minor != 1)
		{
			throw new YamlException("Incompatible %YAML directive");
		}
	}

	private void PPFIAICBBBI(TagDirective value, bool KBLBEMDBNGB, TagDirectiveCollection FMCEHNBELJF)
	{
		if (FMCEHNBELJF.Contains(value))
		{
			if (!KBLBEMDBNGB)
			{
				throw new YamlException("Duplicate %TAG directive.");
			}
		}
		else
		{
			FMCEHNBELJF.Add(value);
		}
	}

	private void OFOGFHMBDJF(ParsingEvent IILOLJJLLGH)
	{
		LNJBMLMFKDH.Push(PFIHGFCNOEG.DocumentEnd);
		MIACPENCIPM(IILOLJJLLGH, true, false, false);
	}

	private void MIACPENCIPM(ParsingEvent IILOLJJLLGH, bool OHJNFDICPDH, bool CLHNCJFJJKN, bool MJHMCMNBBAA)
	{
		MIBEGFBMLEC = OHJNFDICPDH;
		MLJMBHGNABP = CLHNCJFJJKN;
		LMHKCBPDCNH = MJHMCMNBBAA;
		switch (IILOLJJLLGH.get_Type())
		{
		case BHBPOHDAGPH.Alias:
			CECABEPJIDO();
			break;
		case BHBPOHDAGPH.Scalar:
			JMBGOAAKBCA(IILOLJJLLGH);
			break;
		case BHBPOHDAGPH.SequenceStart:
			NILGFBAEKHE(IILOLJJLLGH);
			break;
		case BHBPOHDAGPH.MappingStart:
			MHADHMBBLNP(IILOLJJLLGH);
			break;
		default:
			throw new YamlException(string.Format("Expected SCALAR, SEQUENCE-START, MAPPING-START, or ALIAS, got {0}", IILOLJJLLGH.get_Type()));
		}
	}

	private void CECABEPJIDO()
	{
		OEMEDIPPEMM();
		state = LNJBMLMFKDH.Pop();
	}

	private void JMBGOAAKBCA(ParsingEvent IILOLJJLLGH)
	{
		PGBINBADAEG(IILOLJJLLGH);
		OEMEDIPPEMM();
		DFIHPHNPHAO();
		IncreaseIndent(true, false);
		KHMHNMAFFGO();
		AIBHPFBFGNA = indents.Pop();
		state = LNJBMLMFKDH.Pop();
	}

	private void PGBINBADAEG(ParsingEvent IILOLJJLLGH)
	{
		Scalar lEACOCDHICF = (Scalar)IILOLJJLLGH;
		IBEOFCPMMJJ iBEOFCPMMJJ = lEACOCDHICF.HALCJLMJDII();
		bool flag = KLIMKJBCHOC.FODGADCGDBH == null && KLIMKJBCHOC.NCFFAGOLJEC == null;
		if (flag && !lEACOCDHICF.BIDLJMEAFMI() && !lEACOCDHICF.NIENIKOPKOG())
		{
			throw new YamlException("Neither tag nor isImplicit flags are specified.");
		}
		if (iBEOFCPMMJJ == IBEOFCPMMJJ.Any)
		{
			iBEOFCPMMJJ = ((!AKIFIIJIHGI.IKBPKEEMMCA) ? IBEOFCPMMJJ.Plain : IBEOFCPMMJJ.Folded);
		}
		if (LGDHGOGFFCJ)
		{
			iBEOFCPMMJJ = IBEOFCPMMJJ.DoubleQuoted;
		}
		if (LMHKCBPDCNH && AKIFIIJIHGI.IKBPKEEMMCA)
		{
			iBEOFCPMMJJ = IBEOFCPMMJJ.DoubleQuoted;
		}
		if (iBEOFCPMMJJ == IBEOFCPMMJJ.Plain)
		{
			if ((FLNOCBGGCPP != 0 && !AKIFIIJIHGI.KBJBLFNOFKI) || (FLNOCBGGCPP == 0 && !AKIFIIJIHGI.KGHHDKEBHJP))
			{
				iBEOFCPMMJJ = IBEOFCPMMJJ.SingleQuoted;
			}
			if (string.IsNullOrEmpty(AKIFIIJIHGI.value) && (FLNOCBGGCPP != 0 || LMHKCBPDCNH))
			{
				iBEOFCPMMJJ = IBEOFCPMMJJ.SingleQuoted;
			}
			if (flag && !lEACOCDHICF.BIDLJMEAFMI())
			{
				iBEOFCPMMJJ = IBEOFCPMMJJ.SingleQuoted;
			}
		}
		if (iBEOFCPMMJJ == IBEOFCPMMJJ.SingleQuoted && !AKIFIIJIHGI.EFJAPPGNNIJ)
		{
			iBEOFCPMMJJ = IBEOFCPMMJJ.DoubleQuoted;
		}
		if ((iBEOFCPMMJJ == IBEOFCPMMJJ.Literal || iBEOFCPMMJJ == IBEOFCPMMJJ.Folded) && (!AKIFIIJIHGI.KHNFEFCDPKM || FLNOCBGGCPP != 0 || LMHKCBPDCNH))
		{
			iBEOFCPMMJJ = IBEOFCPMMJJ.DoubleQuoted;
		}
		AKIFIIJIHGI.KIGNIBIMLKK = iBEOFCPMMJJ;
	}

	private void KHMHNMAFFGO()
	{
		switch (AKIFIIJIHGI.KIGNIBIMLKK)
		{
		case IBEOFCPMMJJ.Plain:
			MBNHMNOJKNI(AKIFIIJIHGI.value, !LMHKCBPDCNH);
			break;
		case IBEOFCPMMJJ.SingleQuoted:
			IMIJLBOINNE(AKIFIIJIHGI.value, !LMHKCBPDCNH);
			break;
		case IBEOFCPMMJJ.DoubleQuoted:
			GOCAOKBNGOB(AKIFIIJIHGI.value, !LMHKCBPDCNH);
			break;
		case IBEOFCPMMJJ.Literal:
			PCMFBAHBEBN(AKIFIIJIHGI.value);
			break;
		case IBEOFCPMMJJ.Folded:
			IDBGPLELKHA(AKIFIIJIHGI.value);
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	private void MBNHMNOJKNI(string value, bool AEMLFBEACGF)
	{
		if (!JNNJNNGLDHF)
		{
			Write(' ');
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (NBLLOLGNFGM(c))
			{
				if (AEMLFBEACGF && !flag && DLPJJBPDNDE > CEIKEMJHLKL && i + 1 < value.Length && value[i + 1] != ' ')
				{
					MDBMBMENEBP();
				}
				else
				{
					Write(c);
				}
				flag = true;
				continue;
			}
			if (JCPPGIPDMBK(c))
			{
				if (!flag2 && c == '\n')
				{
					NMGCFFFIFPJ();
				}
				NMGCFFFIFPJ();
				GAMIKMDGHLL = true;
				flag2 = true;
				continue;
			}
			if (flag2)
			{
				MDBMBMENEBP();
			}
			Write(c);
			GAMIKMDGHLL = false;
			flag = false;
			flag2 = false;
		}
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
		if (MIBEGFBMLEC)
		{
			MIBHHFPMBID = true;
		}
	}

	private void IMIJLBOINNE(string value, bool AEMLFBEACGF)
	{
		WriteIndicator("'", true, false, false);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (c == ' ')
			{
				if (AEMLFBEACGF && !flag && DLPJJBPDNDE > CEIKEMJHLKL && i != 0 && i + 1 < value.Length && value[i + 1] != ' ')
				{
					MDBMBMENEBP();
				}
				else
				{
					Write(c);
				}
				flag = true;
				continue;
			}
			if (JCPPGIPDMBK(c))
			{
				if (!flag2 && c == '\n')
				{
					NMGCFFFIFPJ();
				}
				NMGCFFFIFPJ();
				GAMIKMDGHLL = true;
				flag2 = true;
				continue;
			}
			if (flag2)
			{
				MDBMBMENEBP();
			}
			if (c == '\'')
			{
				Write(c);
			}
			Write(c);
			GAMIKMDGHLL = false;
			flag = false;
			flag2 = false;
		}
		WriteIndicator("'", false, false, false);
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
	}

	private void GOCAOKBNGOB(string value, bool AEMLFBEACGF)
	{
		WriteIndicator("\"", true, false, false);
		bool flag = false;
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (IGNGBDLCMGB(c) && !JCPPGIPDMBK(c))
			{
				switch (c)
				{
				case '"':
				case '\\':
					break;
				case ' ':
					if (AEMLFBEACGF && !flag && DLPJJBPDNDE > CEIKEMJHLKL && i > 0 && i + 1 < value.Length)
					{
						MDBMBMENEBP();
						if (value[i + 1] == ' ')
						{
							Write('\\');
						}
					}
					else
					{
						Write(c);
					}
					flag = true;
					continue;
				default:
					Write(c);
					flag = false;
					continue;
				}
			}
			Write('\\');
			switch (c)
			{
			case '\0':
				Write('0');
				break;
			case '\a':
				Write('a');
				break;
			case '\b':
				Write('b');
				break;
			case '\t':
				Write('t');
				break;
			case '\n':
				Write('n');
				break;
			case '\v':
				Write('v');
				break;
			case '\f':
				Write('f');
				break;
			case '\r':
				Write('r');
				break;
			case '\u001b':
				Write('e');
				break;
			case '"':
				Write('"');
				break;
			case '\\':
				Write('\\');
				break;
			case '\u0085':
				Write('N');
				break;
			case '\u00a0':
				Write('_');
				break;
			case '\u2028':
				Write('L');
				break;
			case '\u2029':
				Write('P');
				break;
			default:
			{
				short num = (short)c;
				if (num <= 255)
				{
					Write('x');
					Write(num.ToString("X02", CultureInfo.InvariantCulture));
				}
				else
				{
					Write('u');
					Write(num.ToString("X04", CultureInfo.InvariantCulture));
				}
				break;
			}
			}
			flag = false;
		}
		WriteIndicator("\"", false, false, false);
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
	}

	private void PCMFBAHBEBN(string value)
	{
		bool flag = true;
		WriteIndicator("|", true, false, false);
		GDBPECOIHAP(value);
		NMGCFFFIFPJ();
		GAMIKMDGHLL = true;
		JNNJNNGLDHF = true;
		foreach (char c in value)
		{
			if (JCPPGIPDMBK(c))
			{
				NMGCFFFIFPJ();
				GAMIKMDGHLL = true;
				flag = true;
				continue;
			}
			if (flag)
			{
				MDBMBMENEBP();
			}
			Write(c);
			GAMIKMDGHLL = false;
			flag = false;
		}
	}

	private void IDBGPLELKHA(string value)
	{
		bool flag = true;
		bool flag2 = true;
		WriteIndicator(">", true, false, false);
		GDBPECOIHAP(value);
		NMGCFFFIFPJ();
		GAMIKMDGHLL = true;
		JNNJNNGLDHF = true;
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (JCPPGIPDMBK(c))
			{
				if (!flag && !flag2 && c == '\n')
				{
					int j;
					for (j = 0; i + j < value.Length && JCPPGIPDMBK(value[i + j]); j++)
					{
					}
					if (i + j < value.Length && !LLGAGKDMPPL(value[i + j]) && !JCPPGIPDMBK(value[i + j]))
					{
						NMGCFFFIFPJ();
					}
				}
				NMGCFFFIFPJ();
				GAMIKMDGHLL = true;
				flag = true;
			}
			else
			{
				if (flag)
				{
					MDBMBMENEBP();
					flag2 = LLGAGKDMPPL(c);
				}
				if (!flag && c == ' ' && i + 1 < value.Length && value[i + 1] != ' ' && DLPJJBPDNDE > CEIKEMJHLKL)
				{
					MDBMBMENEBP();
				}
				else
				{
					Write(c);
				}
				GAMIKMDGHLL = false;
				flag = false;
			}
		}
	}

	private static bool NBLLOLGNFGM(char KGDPNIINCJH)
	{
		return KGDPNIINCJH == ' ';
	}

	private static bool JCPPGIPDMBK(char KGDPNIINCJH)
	{
		return KGDPNIINCJH == '\r' || KGDPNIINCJH == '\n' || KGDPNIINCJH == '\u0085' || KGDPNIINCJH == '\u2028' || KGDPNIINCJH == '\u2029';
	}

	private static bool LLGAGKDMPPL(char KGDPNIINCJH)
	{
		return KGDPNIINCJH == ' ' || KGDPNIINCJH == '\t';
	}

	private static bool IGNGBDLCMGB(char KGDPNIINCJH)
	{
		return KGDPNIINCJH == '\t' || KGDPNIINCJH == '\n' || KGDPNIINCJH == '\r' || (KGDPNIINCJH >= ' ' && KGDPNIINCJH <= '~') || KGDPNIINCJH == '\u0085' || (KGDPNIINCJH >= '\u00a0' && KGDPNIINCJH <= '\ud7ff') || (KGDPNIINCJH >= '\ue000' && KGDPNIINCJH <= '\ufffd');
	}

	private void NILGFBAEKHE(ParsingEvent IILOLJJLLGH)
	{
		OEMEDIPPEMM();
		DFIHPHNPHAO();
		JODGINIKFJF jODGINIKFJF = (JODGINIKFJF)IILOLJJLLGH;
		if (FLNOCBGGCPP != 0 || LGDHGOGFFCJ || jODGINIKFJF.HALCJLMJDII() == NBCBGEPFIKG.Flow || JIIDKCHJIBP())
		{
			state = PFIHGFCNOEG.FlowSequenceFirstItem;
		}
		else
		{
			state = PFIHGFCNOEG.BlockSequenceFirstItem;
		}
	}

	private void MHADHMBBLNP(ParsingEvent IILOLJJLLGH)
	{
		OEMEDIPPEMM();
		DFIHPHNPHAO();
		MappingStart oGMPNFCPPDH = (MappingStart)IILOLJJLLGH;
		if (FLNOCBGGCPP != 0 || LGDHGOGFFCJ || oGMPNFCPPDH.HALCJLMJDII() == FGDKNBEFPFN.Flow || FDHCLHDIAAM())
		{
			state = PFIHGFCNOEG.FlowMappingFirstKey;
		}
		else
		{
			state = PFIHGFCNOEG.BlockMappingFirstKey;
		}
	}

	private void OEMEDIPPEMM()
	{
		if (MOIAINBHLBA.KOLNNNLOCFE != null)
		{
			WriteIndicator((!MOIAINBHLBA.LCPNKFDMFIA) ? "&" : "*", true, false, false);
			EFKJGICEBDI(MOIAINBHLBA.KOLNNNLOCFE);
		}
	}

	private void DFIHPHNPHAO()
	{
		if (KLIMKJBCHOC.FODGADCGDBH == null && KLIMKJBCHOC.NCFFAGOLJEC == null)
		{
			return;
		}
		if (KLIMKJBCHOC.FODGADCGDBH != null)
		{
			IMEIEEFKHCD(KLIMKJBCHOC.FODGADCGDBH);
			if (KLIMKJBCHOC.NCFFAGOLJEC != null)
			{
				DMCBHMNGJIP(KLIMKJBCHOC.NCFFAGOLJEC, false);
			}
		}
		else
		{
			WriteIndicator("!<", true, false, false);
			DMCBHMNGJIP(KLIMKJBCHOC.NCFFAGOLJEC, false);
			WriteIndicator(">", false, false, false);
		}
	}

	private void IOPKCDJBPFJ(ParsingEvent IILOLJJLLGH)
	{
		DocumentEnd nKCBFAMCLMO = IILOLJJLLGH as DocumentEnd;
		if (nKCBFAMCLMO != null)
		{
			MDBMBMENEBP();
			if (!nKCBFAMCLMO.BBBGHODAEIN())
			{
				WriteIndicator("...", true, false, false);
				MDBMBMENEBP();
				FDLHHPENADJ = true;
			}
			state = PFIHGFCNOEG.DocumentStart;
			FMCEHNBELJF.Clear();
			return;
		}
		throw new YamlException("Expected DOCUMENT-END.");
	}

	private void MOGAEAKBOGN(ParsingEvent IILOLJJLLGH, bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			WriteIndicator("[", true, true, false);
			IncreaseIndent(true, false);
			FLNOCBGGCPP++;
		}
		if (IILOLJJLLGH is AKMKLAINLOL)
		{
			FLNOCBGGCPP--;
			AIBHPFBFGNA = indents.Pop();
			if (LGDHGOGFFCJ && !IKNHLPGLLKB)
			{
				WriteIndicator(",", false, false, false);
				MDBMBMENEBP();
			}
			WriteIndicator("]", false, false, false);
			state = LNJBMLMFKDH.Pop();
		}
		else
		{
			if (!IKNHLPGLLKB)
			{
				WriteIndicator(",", false, false, false);
			}
			if (LGDHGOGFFCJ || DLPJJBPDNDE > CEIKEMJHLKL)
			{
				MDBMBMENEBP();
			}
			LNJBMLMFKDH.Push(PFIHGFCNOEG.FlowSequenceItem);
			MIACPENCIPM(IILOLJJLLGH, false, false, false);
		}
	}

	private void JGPMFBFHPLG(ParsingEvent IILOLJJLLGH, bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			WriteIndicator("{", true, true, false);
			IncreaseIndent(true, false);
			FLNOCBGGCPP++;
		}
		if (IILOLJJLLGH is BLFPJCPALDH)
		{
			FLNOCBGGCPP--;
			AIBHPFBFGNA = indents.Pop();
			if (LGDHGOGFFCJ && !IKNHLPGLLKB)
			{
				WriteIndicator(",", false, false, false);
				MDBMBMENEBP();
			}
			WriteIndicator("}", false, false, false);
			state = LNJBMLMFKDH.Pop();
			return;
		}
		if (!IKNHLPGLLKB)
		{
			WriteIndicator(",", false, false, false);
		}
		if (LGDHGOGFFCJ || DLPJJBPDNDE > CEIKEMJHLKL)
		{
			MDBMBMENEBP();
		}
		if (!LGDHGOGFFCJ && FHCDNJHMBFN())
		{
			LNJBMLMFKDH.Push(PFIHGFCNOEG.FlowMappingSimpleValue);
			MIACPENCIPM(IILOLJJLLGH, false, true, true);
		}
		else
		{
			WriteIndicator("?", true, false, false);
			LNJBMLMFKDH.Push(PFIHGFCNOEG.FlowMappingValue);
			MIACPENCIPM(IILOLJJLLGH, false, true, false);
		}
	}

	private void LKMHLDHKDAM(ParsingEvent IILOLJJLLGH, bool FBFEFFJCLBE)
	{
		if (FBFEFFJCLBE)
		{
			WriteIndicator(":", false, false, false);
		}
		else
		{
			if (LGDHGOGFFCJ || DLPJJBPDNDE > CEIKEMJHLKL)
			{
				MDBMBMENEBP();
			}
			WriteIndicator(":", true, false, false);
		}
		LNJBMLMFKDH.Push(PFIHGFCNOEG.FlowMappingKey);
		MIACPENCIPM(IILOLJJLLGH, false, true, false);
	}

	private void BNNKMPFDJGP(ParsingEvent IILOLJJLLGH, bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			IncreaseIndent(false, MLJMBHGNABP && !GAMIKMDGHLL);
		}
		if (IILOLJJLLGH is AKMKLAINLOL)
		{
			AIBHPFBFGNA = indents.Pop();
			state = LNJBMLMFKDH.Pop();
			return;
		}
		MDBMBMENEBP();
		WriteIndicator("  -", true, false, true);
		LNJBMLMFKDH.Push(PFIHGFCNOEG.BlockSequenceItem);
		MIACPENCIPM(IILOLJJLLGH, false, false, false);
	}

	private void OPAIFNHGJGA(ParsingEvent IILOLJJLLGH, bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			IncreaseIndent(false, false);
		}
		if (IILOLJJLLGH is BLFPJCPALDH)
		{
			AIBHPFBFGNA = indents.Pop();
			state = LNJBMLMFKDH.Pop();
			return;
		}
		MDBMBMENEBP();
		if (FHCDNJHMBFN())
		{
			LNJBMLMFKDH.Push(PFIHGFCNOEG.BlockMappingSimpleValue);
			MIACPENCIPM(IILOLJJLLGH, false, true, true);
		}
		else
		{
			WriteIndicator("?", true, false, true);
			LNJBMLMFKDH.Push(PFIHGFCNOEG.BlockMappingValue);
			MIACPENCIPM(IILOLJJLLGH, false, true, false);
		}
	}

	private void PAGLCKDAPMK(ParsingEvent IILOLJJLLGH, bool FBFEFFJCLBE)
	{
		if (FBFEFFJCLBE)
		{
			WriteIndicator(":", false, false, false);
		}
		else
		{
			MDBMBMENEBP();
			WriteIndicator(":", true, false, true);
		}
		LNJBMLMFKDH.Push(PFIHGFCNOEG.BlockMappingKey);
		MIACPENCIPM(IILOLJJLLGH, false, true, false);
	}

	private void IncreaseIndent(bool LMEEIAPIDIJ, bool BHHEHBPGKIO)
	{
		indents.Push(AIBHPFBFGNA);
		if (AIBHPFBFGNA < 0)
		{
			AIBHPFBFGNA = (LMEEIAPIDIJ ? EMOCJNOCJKM : 0);
		}
		else if (!BHHEHBPGKIO)
		{
			AIBHPFBFGNA += EMOCJNOCJKM;
		}
	}

	private bool NOIMGKCEPOH()
	{
		int num = 0;
		foreach (ParsingEvent item in DNBFFLFBDOB)
		{
			num++;
			if (num == 2)
			{
				Scalar lEACOCDHICF = item as Scalar;
				if (lEACOCDHICF != null)
				{
					return string.IsNullOrEmpty(lEACOCDHICF.OEAKCOHMIHH());
				}
				break;
			}
		}
		return false;
	}

	private bool FHCDNJHMBFN()
	{
		if (DNBFFLFBDOB.Count < 1)
		{
			return false;
		}
		int num;
		switch (DNBFFLFBDOB.Peek().get_Type())
		{
		case BHBPOHDAGPH.Alias:
			num = SafeStringLength(MOIAINBHLBA.KOLNNNLOCFE);
			break;
		case BHBPOHDAGPH.Scalar:
			if (AKIFIIJIHGI.IKBPKEEMMCA)
			{
				return false;
			}
			num = SafeStringLength(MOIAINBHLBA.KOLNNNLOCFE) + SafeStringLength(KLIMKJBCHOC.FODGADCGDBH) + SafeStringLength(KLIMKJBCHOC.NCFFAGOLJEC) + SafeStringLength(AKIFIIJIHGI.value);
			break;
		case BHBPOHDAGPH.SequenceStart:
			if (!JIIDKCHJIBP())
			{
				return false;
			}
			num = SafeStringLength(MOIAINBHLBA.KOLNNNLOCFE) + SafeStringLength(KLIMKJBCHOC.FODGADCGDBH) + SafeStringLength(KLIMKJBCHOC.NCFFAGOLJEC);
			break;
		case BHBPOHDAGPH.MappingStart:
			if (!JIIDKCHJIBP())
			{
				return false;
			}
			num = SafeStringLength(MOIAINBHLBA.KOLNNNLOCFE) + SafeStringLength(KLIMKJBCHOC.FODGADCGDBH) + SafeStringLength(KLIMKJBCHOC.NCFFAGOLJEC);
			break;
		default:
			return false;
		}
		return num <= 128;
	}

	private int SafeStringLength(string value)
	{
		return (value != null) ? value.Length : 0;
	}

	private bool JIIDKCHJIBP()
	{
		if (DNBFFLFBDOB.Count < 2)
		{
			return false;
		}
		global::FakeList<ParsingEvent> aGIJCJFMLNN = new global::FakeList<ParsingEvent>(DNBFFLFBDOB);
		return aGIJCJFMLNN.get_Item(0) is JODGINIKFJF && aGIJCJFMLNN.get_Item(1) is AKMKLAINLOL;
	}

	private bool FDHCLHDIAAM()
	{
		if (DNBFFLFBDOB.Count < 2)
		{
			return false;
		}
		global::FakeList<ParsingEvent> aGIJCJFMLNN = new global::FakeList<ParsingEvent>(DNBFFLFBDOB);
		return aGIJCJFMLNN.get_Item(0) is MappingStart && aGIJCJFMLNN.get_Item(1) is BLFPJCPALDH;
	}

	private void GDBPECOIHAP(string value)
	{
		CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
		if (characterAnalyzer.NBLLOLGNFGM() || characterAnalyzer.JCPPGIPDMBK())
		{
			string gPKBINAOGDC = string.Format(CultureInfo.InvariantCulture, "{0}\0", EMOCJNOCJKM);
			WriteIndicator(gPKBINAOGDC, false, false, false);
		}
		MIBHHFPMBID = false;
		string text = null;
		if (value.Length == 0 || !characterAnalyzer.JCPPGIPDMBK(value.Length - 1))
		{
			text = "-";
		}
		else if (value.Length >= 2 && characterAnalyzer.JCPPGIPDMBK(value.Length - 2))
		{
			text = "+";
			MIBHHFPMBID = true;
		}
		if (text != null)
		{
			WriteIndicator(text, false, false, false);
		}
	}

	private void WriteIndicator(string GPKBINAOGDC, bool EMBMHCGJHDL, bool KCCMOOJPCBM, bool FCOACAMEHOE)
	{
		if (EMBMHCGJHDL && !JNNJNNGLDHF)
		{
			Write(' ');
		}
		Write(GPKBINAOGDC);
		JNNJNNGLDHF = KCCMOOJPCBM;
		GAMIKMDGHLL &= FCOACAMEHOE;
		MIBHHFPMBID = false;
	}

	private void MDBMBMENEBP()
	{
		int num = Math.Max(AIBHPFBFGNA, 0);
		if (!GAMIKMDGHLL || DLPJJBPDNDE > num || (DLPJJBPDNDE == num && !JNNJNNGLDHF))
		{
			NMGCFFFIFPJ();
		}
		while (DLPJJBPDNDE < num)
		{
			Write(' ');
		}
		JNNJNNGLDHF = true;
		GAMIKMDGHLL = true;
	}

	private void EFKJGICEBDI(string value)
	{
		Write(value);
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
	}

	private void IMEIEEFKHCD(string value)
	{
		if (!JNNJNNGLDHF)
		{
			Write(' ');
		}
		Write(value);
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
	}

	private void DMCBHMNGJIP(string value, bool BFPMMILLOHL)
	{
		if (BFPMMILLOHL && !JNNJNNGLDHF)
		{
			Write(' ');
		}
		Write(UrlEncode(value));
		JNNJNNGLDHF = false;
		GAMIKMDGHLL = false;
	}

	private string UrlEncode(string HCPNFPMHFCM)
	{
		return uriReplacer.Replace(HCPNFPMHFCM, (System.Text.RegularExpressions.Match MLPEJKLNAKF) =>
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] bytes = Encoding.UTF8.GetBytes(MLPEJKLNAKF.Value);
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("%{0:X02}", b);
			}
			return stringBuilder.ToString();
		});
	}

	private void Write(char value)
	{
		output.Write(value);
		DLPJJBPDNDE++;
	}

	private void Write(string value)
	{
		output.Write(value);
		DLPJJBPDNDE += value.Length;
	}

	private void NMGCFFFIFPJ()
	{
		output.WriteLine();
		DLPJJBPDNDE = 0;
	}
}
