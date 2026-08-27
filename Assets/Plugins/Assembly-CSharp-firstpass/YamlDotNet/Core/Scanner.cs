using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Scanner : IScanner
	{
		private const int MaxVersionNumberLength = 9;

		private const int MaxBufferLength = 8;

		private static readonly IDictionary<char, char> simpleEscapeCodes = new SortedDictionary<char, char>
		{
			{ '0', '\0' },
			{ 'a', '\a' },
			{ 'b', '\b' },
			{ 't', '\t' },
			{ '\t', '\t' },
			{ 'n', '\n' },
			{ 'v', '\v' },
			{ 'f', '\f' },
			{ 'r', '\r' },
			{ 'e', '\u001b' },
			{ ' ', ' ' },
			{ '"', '"' },
			{ '\'', '\'' },
			{ '\\', '\\' },
			{ 'N', '\u0085' },
			{ '_', '\u00a0' },
			{ 'L', '\u2028' },
			{ 'P', '\u2029' }
		};

		private readonly Stack<int> indents = new Stack<int>();

		private readonly InsertionQueue<Token> tokens = new InsertionQueue<Token>();

		private readonly Stack<SimpleKey> simpleKeys = new Stack<SimpleKey>();

		private readonly CharacterAnalyzer<LookAheadBuffer> analyzer;

		private Cursor cursor;

		private bool streamStartProduced;

		private bool streamEndProduced;

		private int indent = -1;

		private bool simpleKeyAllowed;

		private int flowLevel;

		private int tokensParsed;

		private bool tokenAvailable;

		private Token previous;

		public bool SkipComments { get; private set; }

		public Token Current { get; private set; }

		public Mark CurrentPosition
		{
			get
			{
				return cursor.BJKDANAAGHK();
			}
		}

		public Scanner(TextReader NILNDHEKNLJ, bool CGNHIACFHMM = true)
		{
			analyzer = new CharacterAnalyzer<LookAheadBuffer>(new LookAheadBuffer(NILNDHEKNLJ, 8));
			cursor = new Cursor();
			SkipComments = CGNHIACFHMM;
		}

		public bool PCCMLADDNDG()
		{
			if (Current != null)
			{
				KPFPDDBILAE();
			}
			return NCFHFMEKMFC();
		}

		internal bool NCFHFMEKMFC()
		{
			if (!tokenAvailable && !streamEndProduced)
			{
				FKGHEPHAOAP();
			}
			if (tokens.Count > 0)
			{
				Current = tokens.HBPLGGGBDAB();
				tokenAvailable = false;
				return true;
			}
			Current = null;
			return false;
		}

		internal void KPFPDDBILAE()
		{
			tokensParsed++;
			tokenAvailable = false;
			previous = Current;
			Current = null;
		}

		private char HKACDMEFHIB()
		{
			char result = analyzer.Peek(0);
			Skip();
			return result;
		}

		private char OGNODHFDLNF()
		{
			if (analyzer.Check("\r\n\u0085"))
			{
				BBLGADKMIPD();
				return '\n';
			}
			char result = analyzer.Peek(0);
			BBLGADKMIPD();
			return result;
		}

		private void FKGHEPHAOAP()
		{
			while (true)
			{
				bool flag = false;
				if (tokens.Count == 0)
				{
					flag = true;
				}
				else
				{
					EJKKMIGJHKC();
					foreach (SimpleKey simpleKey in simpleKeys)
					{
						if (simpleKey.IsPossible && simpleKey.TokenNumber == tokensParsed)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				PBNCMCKIPLN();
			}
			tokenAvailable = true;
		}

		private static bool BKAHPGIAGED(StringBuilder BMKNHNOGIHO, char ILENLCMAMBH)
		{
			return BMKNHNOGIHO.Length > 0 && BMKNHNOGIHO[0] == ILENLCMAMBH;
		}

		private void EJKKMIGJHKC()
		{
			foreach (SimpleKey simpleKey in simpleKeys)
			{
				if (simpleKey.IsPossible && (simpleKey.Line < cursor.Line || simpleKey.Index + 1024 < cursor.Index))
				{
					if (simpleKey.IsRequired)
					{
						Mark mark = cursor.BJKDANAAGHK();
						throw new SyntaxErrorException(mark, mark, "While scanning a simple key, could not find expected ':'.");
					}
					simpleKey.IsPossible = false;
				}
			}
		}

		private void PBNCMCKIPLN()
		{
			if (!streamStartProduced)
			{
				JCHFJFAJLMP();
				return;
			}
			KGOIEEBEKFA();
			EJKKMIGJHKC();
			UnrollIndent(cursor.LineOffset);
			analyzer.Buffer.CGGPDODMKCF(4);
			if (analyzer.Buffer.EndOfInput)
			{
				FIPLOALOAMJ();
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('%'))
			{
				HMINGFLCBEB();
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('-') && analyzer.Check('-', 1) && analyzer.Check('-', 2) && analyzer.MKOKPKHBDMD(3))
			{
				BIIAPHMNDJK(true);
				return;
			}
			if (cursor.LineOffset == 0 && analyzer.Check('.') && analyzer.Check('.', 1) && analyzer.Check('.', 2) && analyzer.MKOKPKHBDMD(3))
			{
				BIIAPHMNDJK(false);
				return;
			}
			if (analyzer.Check('['))
			{
				CCDGCBGHAEB(true);
				return;
			}
			if (analyzer.Check('{'))
			{
				CCDGCBGHAEB(false);
				return;
			}
			if (analyzer.Check(']'))
			{
				KNECKBMGPEC(true);
				return;
			}
			if (analyzer.Check('}'))
			{
				KNECKBMGPEC(false);
				return;
			}
			if (analyzer.Check(','))
			{
				JNIGJKOKPIH();
				return;
			}
			if (analyzer.Check('-') && analyzer.MKOKPKHBDMD(1))
			{
				LMKPEPCKKMP();
				return;
			}
			if (analyzer.Check('?') && (flowLevel > 0 || analyzer.MKOKPKHBDMD(1)))
			{
				MHOMBHMODGE();
				return;
			}
			if (analyzer.Check(':') && (flowLevel > 0 || analyzer.MKOKPKHBDMD(1)))
			{
				HBGPDPAPEJB();
				return;
			}
			if (analyzer.Check('*'))
			{
				BKPNOKPHDCE(true);
				return;
			}
			if (analyzer.Check('&'))
			{
				BKPNOKPHDCE(false);
				return;
			}
			if (analyzer.Check('!'))
			{
				MHDNFCAILLG();
				return;
			}
			if (analyzer.Check('|') && flowLevel == 0)
			{
				FLCGFAGILPB(true);
				return;
			}
			if (analyzer.Check('>') && flowLevel == 0)
			{
				FLCGFAGILPB(false);
				return;
			}
			if (analyzer.Check('\''))
			{
				DJLOGNJKFNK(true);
				return;
			}
			if (analyzer.Check('"'))
			{
				DJLOGNJKFNK(false);
				return;
			}
			if ((!analyzer.MKOKPKHBDMD() && !analyzer.Check("-?:,[]{}#&*!|>'\"%@`")) || (analyzer.Check('-') && !analyzer.MIGPEDGKJEG(1)) || (flowLevel == 0 && analyzer.Check("?:") && !analyzer.MKOKPKHBDMD(1)))
			{
				JDJODGJEDOF();
				return;
			}
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			Mark pCLFFOBJJFO = cursor.BJKDANAAGHK();
			throw new SyntaxErrorException(iLENLCMAMBH, pCLFFOBJJFO, "While scanning for the next token, find character that cannot start any token.");
		}

		private bool CJCNCHCJAOM()
		{
			return analyzer.Check(' ') || ((flowLevel > 0 || !simpleKeyAllowed) && analyzer.Check('\t'));
		}

		private bool OPEFOACCBGC()
		{
			if (cursor.LineOffset == 0 && analyzer.MKOKPKHBDMD(3))
			{
				bool flag = analyzer.Check('-') && analyzer.Check('-', 1) && analyzer.Check('-', 2);
				bool flag2 = analyzer.Check('.') && analyzer.Check('.', 1) && analyzer.Check('.', 2);
				return flag || flag2;
			}
			return false;
		}

		private void Skip()
		{
			cursor.Skip();
			analyzer.Buffer.Skip(1);
		}

		private void BBLGADKMIPD()
		{
			if (analyzer.DHPOAOIAGPE())
			{
				cursor.AIGOMGCEJJD(2);
				analyzer.Buffer.Skip(2);
			}
			else if (analyzer.JCPPGIPDMBK())
			{
				cursor.AIGOMGCEJJD(1);
				analyzer.Buffer.Skip(1);
			}
			else if (!analyzer.AJCHNKGPEJB())
			{
				throw new InvalidOperationException("Not at a break.");
			}
		}

		private void KGOIEEBEKFA()
		{
			while (true)
			{
				if (CJCNCHCJAOM())
				{
					Skip();
					continue;
				}
				LHNPOFBJJMN();
				if (analyzer.JCPPGIPDMBK())
				{
					BBLGADKMIPD();
					if (flowLevel == 0)
					{
						simpleKeyAllowed = true;
					}
					continue;
				}
				break;
			}
		}

		private void LHNPOFBJJMN()
		{
			if (analyzer.Check('#'))
			{
				Mark mark = cursor.BJKDANAAGHK();
				Skip();
				while (analyzer.NBLLOLGNFGM())
				{
					Skip();
				}
				StringBuilder stringBuilder = new StringBuilder();
				while (!analyzer.PDOIBEFPDEB())
				{
					stringBuilder.Append(HKACDMEFHIB());
				}
				if (!SkipComments)
				{
					bool eKOKIGANOMO = previous != null && previous.End.Line == mark.Line && !(previous is StreamStart);
					tokens.JFGNCJCOCJA(new Tokens.Comment(stringBuilder.ToString(), eKOKIGANOMO, mark, cursor.BJKDANAAGHK()));
				}
			}
		}

		private void JCHFJFAJLMP()
		{
			simpleKeys.Push(new SimpleKey());
			simpleKeyAllowed = true;
			streamStartProduced = true;
			Mark mark = cursor.BJKDANAAGHK();
			tokens.JFGNCJCOCJA(new Tokens.StreamStart(mark, mark));
		}

		private void UnrollIndent(int DLPJJBPDNDE)
		{
			if (flowLevel == 0)
			{
				while (indent > DLPJJBPDNDE)
				{
					Mark mark = cursor.BJKDANAAGHK();
					tokens.JFGNCJCOCJA(new BlockEnd(mark, mark));
					indent = indents.Pop();
				}
			}
		}

		private void FIPLOALOAMJ()
		{
			cursor.JFJBGABDLJM();
			UnrollIndent(-1);
			OIDDFFJBIKK();
			simpleKeyAllowed = false;
			streamEndProduced = true;
			Mark mark = cursor.BJKDANAAGHK();
			tokens.JFGNCJCOCJA(new StreamEnd(mark, mark));
		}

		private void HMINGFLCBEB()
		{
			UnrollIndent(-1);
			OIDDFFJBIKK();
			simpleKeyAllowed = false;
			Token mBIJKDIEFIF = GCBFEKEJMJM();
			tokens.JFGNCJCOCJA(mBIJKDIEFIF);
		}

		private Token GCBFEKEJMJM()
		{
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			Token result;
			switch (ScanDirectiveName(iLENLCMAMBH))
			{
			case "YAML":
				result = HNBHLKOPJGL(iLENLCMAMBH);
				break;
			case "TAG":
				result = GKOGPOOEBGL(iLENLCMAMBH);
				break;
			default:
				throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a directive, find uknown directive name.");
			}
			while (analyzer.MIGPEDGKJEG())
			{
				Skip();
			}
			LHNPOFBJJMN();
			if (!analyzer.PDOIBEFPDEB())
			{
				throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a directive, did not find expected comment or line break.");
			}
			if (analyzer.JCPPGIPDMBK())
			{
				BBLGADKMIPD();
			}
			return result;
		}

		private void BIIAPHMNDJK(bool EFNNCDEPIFB)
		{
			UnrollIndent(-1);
			OIDDFFJBIKK();
			simpleKeyAllowed = false;
			Mark mark = cursor.BJKDANAAGHK();
			Skip();
			Skip();
			Skip();
			Token mBIJKDIEFIF = ((!EFNNCDEPIFB) ? ((Token)new Tokens.DocumentEnd(mark, mark)) : ((Token)new Tokens.DocumentStart(mark, cursor.BJKDANAAGHK())));
			tokens.JFGNCJCOCJA(mBIJKDIEFIF);
		}

		private void CCDGCBGHAEB(bool AHKFNJKIBCN)
		{
			GBHAEIBOPNO();
			ECMNMOMCMGB();
			simpleKeyAllowed = true;
			Mark mark = cursor.BJKDANAAGHK();
			Skip();
			Token mBIJKDIEFIF = ((!AHKFNJKIBCN) ? ((Token)new FlowMappingStart(mark, mark)) : ((Token)new FlowSequenceStart(mark, mark)));
			tokens.JFGNCJCOCJA(mBIJKDIEFIF);
		}

		private void ECMNMOMCMGB()
		{
			simpleKeys.Push(new SimpleKey());
			flowLevel++;
		}

		private void KNECKBMGPEC(bool AHKFNJKIBCN)
		{
			OIDDFFJBIKK();
			BMJNHPLKCEA();
			simpleKeyAllowed = false;
			Mark mark = cursor.BJKDANAAGHK();
			Skip();
			Token mBIJKDIEFIF = ((!AHKFNJKIBCN) ? ((Token)new FlowMappingEnd(mark, mark)) : ((Token)new FlowSequenceEnd(mark, mark)));
			tokens.JFGNCJCOCJA(mBIJKDIEFIF);
		}

		private void BMJNHPLKCEA()
		{
			if (flowLevel > 0)
			{
				flowLevel--;
				simpleKeys.Pop();
			}
		}

		private void JNIGJKOKPIH()
		{
			OIDDFFJBIKK();
			simpleKeyAllowed = true;
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			tokens.JFGNCJCOCJA(new FlowEntry(iLENLCMAMBH, cursor.BJKDANAAGHK()));
		}

		private void LMKPEPCKKMP()
		{
			if (flowLevel == 0)
			{
				if (!simpleKeyAllowed)
				{
					Mark mark = cursor.BJKDANAAGHK();
					throw new SyntaxErrorException(mark, mark, "Block sequence entries are not allowed in this context.");
				}
				RollIndent(cursor.LineOffset, -1, true, cursor.BJKDANAAGHK());
			}
			OIDDFFJBIKK();
			simpleKeyAllowed = true;
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			tokens.JFGNCJCOCJA(new BlockEntry(iLENLCMAMBH, cursor.BJKDANAAGHK()));
		}

		private void MHOMBHMODGE()
		{
			if (flowLevel == 0)
			{
				if (!simpleKeyAllowed)
				{
					Mark mark = cursor.BJKDANAAGHK();
					throw new SyntaxErrorException(mark, mark, "Mapping keys are not allowed in this context.");
				}
				RollIndent(cursor.LineOffset, -1, false, cursor.BJKDANAAGHK());
			}
			OIDDFFJBIKK();
			simpleKeyAllowed = flowLevel == 0;
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			tokens.JFGNCJCOCJA(new Key(iLENLCMAMBH, cursor.BJKDANAAGHK()));
		}

		private void HBGPDPAPEJB()
		{
			SimpleKey simpleKey = simpleKeys.Peek();
			if (simpleKey.IsPossible)
			{
				tokens.Insert(simpleKey.TokenNumber - tokensParsed, new Key(simpleKey.Mark, simpleKey.Mark));
				RollIndent(simpleKey.LineOffset, simpleKey.TokenNumber, false, simpleKey.Mark);
				simpleKey.IsPossible = false;
				simpleKeyAllowed = false;
			}
			else
			{
				if (flowLevel == 0)
				{
					if (!simpleKeyAllowed)
					{
						Mark mark = cursor.BJKDANAAGHK();
						throw new SyntaxErrorException(mark, mark, "Mapping values are not allowed in this context.");
					}
					RollIndent(cursor.LineOffset, -1, false, cursor.BJKDANAAGHK());
				}
				simpleKeyAllowed = flowLevel == 0;
			}
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			tokens.JFGNCJCOCJA(new Value(iLENLCMAMBH, cursor.BJKDANAAGHK()));
		}

		private void RollIndent(int DLPJJBPDNDE, int number, bool ANLMKAJLJIJ, Mark MGMMDGFPBLP)
		{
			if (flowLevel <= 0 && indent < DLPJJBPDNDE)
			{
				indents.Push(indent);
				indent = DLPJJBPDNDE;
				Token mBIJKDIEFIF = ((!ANLMKAJLJIJ) ? ((Token)new BlockMappingStart(MGMMDGFPBLP, MGMMDGFPBLP)) : ((Token)new BlockSequenceStart(MGMMDGFPBLP, MGMMDGFPBLP)));
				if (number == -1)
				{
					tokens.JFGNCJCOCJA(mBIJKDIEFIF);
				}
				else
				{
					tokens.Insert(number - tokensParsed, mBIJKDIEFIF);
				}
			}
		}

		private void BKPNOKPHDCE(bool LCPNKFDMFIA)
		{
			GBHAEIBOPNO();
			simpleKeyAllowed = false;
			tokens.JFGNCJCOCJA(ALHHDIPHHAJ(LCPNKFDMFIA));
		}

		private Token ALHHDIPHHAJ(bool LCPNKFDMFIA)
		{
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			StringBuilder stringBuilder = new StringBuilder();
			while (analyzer.KJGBACCEGND())
			{
				stringBuilder.Append(HKACDMEFHIB());
			}
			if (stringBuilder.Length == 0 || (!analyzer.MKOKPKHBDMD() && !analyzer.Check("?:,]}%@`")))
			{
				throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning an anchor or alias, did not find expected alphabetic or numeric character.");
			}
			if (LCPNKFDMFIA)
			{
				return new Tokens.AnchorAlias(stringBuilder.ToString(), iLENLCMAMBH, cursor.BJKDANAAGHK());
			}
			return new Anchor(stringBuilder.ToString(), iLENLCMAMBH, cursor.BJKDANAAGHK());
		}

		private void MHDNFCAILLG()
		{
			GBHAEIBOPNO();
			simpleKeyAllowed = false;
			tokens.JFGNCJCOCJA(PAAPFNEIACF());
		}

		private Token PAAPFNEIACF()
		{
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			string text;
			string text2;
			if (analyzer.Check('<', 1))
			{
				text = string.Empty;
				Skip();
				Skip();
				text2 = ScanTagUri(null, iLENLCMAMBH);
				if (!analyzer.Check('>'))
				{
					throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a tag, did not find the expected '>'.");
				}
				Skip();
			}
			else
			{
				string text3 = ScanTagHandle(false, iLENLCMAMBH);
				if (text3.Length > 1 && text3[0] == '!' && text3[text3.Length - 1] == '!')
				{
					text = text3;
					text2 = ScanTagUri(null, iLENLCMAMBH);
				}
				else
				{
					text2 = ScanTagUri(text3, iLENLCMAMBH);
					text = "!";
					if (text2.Length == 0)
					{
						text2 = text;
						text = string.Empty;
					}
				}
			}
			if (!analyzer.MKOKPKHBDMD())
			{
				throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a tag, did not find expected whitespace or line break.");
			}
			return new Tag(text, text2, iLENLCMAMBH, cursor.BJKDANAAGHK());
		}

		private void FLCGFAGILPB(bool HLIHDHJFPJP)
		{
			OIDDFFJBIKK();
			simpleKeyAllowed = true;
			tokens.JFGNCJCOCJA(BEJNDKGCICB(HLIHDHJFPJP));
		}

		private Token BEJNDKGCICB(bool HLIHDHJFPJP)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int nAPIKMHPLFP = 0;
			bool flag = false;
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			if (analyzer.Check("+-"))
			{
				num = (analyzer.Check('+') ? 1 : (-1));
				Skip();
				if (analyzer.DDINBPOLPJP())
				{
					if (analyzer.Check('0'))
					{
						throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a block scalar, find an intendation indicator equal to 0.");
					}
					num2 = analyzer.MDEJLGGFDCP();
					Skip();
				}
			}
			else if (analyzer.DDINBPOLPJP())
			{
				if (analyzer.Check('0'))
				{
					throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a block scalar, find an intendation indicator equal to 0.");
				}
				num2 = analyzer.MDEJLGGFDCP();
				Skip();
				if (analyzer.Check("+-"))
				{
					num = (analyzer.Check('+') ? 1 : (-1));
					Skip();
				}
			}
			while (analyzer.MIGPEDGKJEG())
			{
				Skip();
			}
			LHNPOFBJJMN();
			if (!analyzer.PDOIBEFPDEB())
			{
				throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a block scalar, did not find expected comment or line break.");
			}
			if (analyzer.JCPPGIPDMBK())
			{
				BBLGADKMIPD();
			}
			Mark PCLFFOBJJFO = cursor.BJKDANAAGHK();
			if (num2 != 0)
			{
				nAPIKMHPLFP = ((indent < 0) ? num2 : (indent + num2));
			}
			nAPIKMHPLFP = ScanBlockScalarBreaks(nAPIKMHPLFP, stringBuilder3, iLENLCMAMBH, ref PCLFFOBJJFO);
			while (cursor.LineOffset == nAPIKMHPLFP && !analyzer.AJCHNKGPEJB())
			{
				bool flag2 = analyzer.MIGPEDGKJEG();
				if (!HLIHDHJFPJP && BKAHPGIAGED(stringBuilder2, '\n') && !flag && !flag2)
				{
					if (stringBuilder3.Length == 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder2.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
				stringBuilder.Append(stringBuilder3.ToString());
				stringBuilder3.Length = 0;
				flag = analyzer.MIGPEDGKJEG();
				while (!analyzer.PDOIBEFPDEB())
				{
					stringBuilder.Append(HKACDMEFHIB());
				}
				stringBuilder2.Append(OGNODHFDLNF());
				nAPIKMHPLFP = ScanBlockScalarBreaks(nAPIKMHPLFP, stringBuilder3, iLENLCMAMBH, ref PCLFFOBJJFO);
			}
			if (num != -1)
			{
				stringBuilder.Append(stringBuilder2);
			}
			if (num == 1)
			{
				stringBuilder.Append(stringBuilder3);
			}
			IBEOFCPMMJJ kIGNIBIMLKK = ((!HLIHDHJFPJP) ? IBEOFCPMMJJ.Folded : IBEOFCPMMJJ.Literal);
			return new Tokens.Scalar(stringBuilder.ToString(), kIGNIBIMLKK, iLENLCMAMBH, PCLFFOBJJFO);
		}

		private int ScanBlockScalarBreaks(int NAPIKMHPLFP, StringBuilder IIAFKNDBKLN, Mark ILENLCMAMBH, ref Mark PCLFFOBJJFO)
		{
			int num = 0;
			PCLFFOBJJFO = cursor.BJKDANAAGHK();
			while (true)
			{
				if ((NAPIKMHPLFP == 0 || cursor.LineOffset < NAPIKMHPLFP) && analyzer.NBLLOLGNFGM())
				{
					Skip();
					continue;
				}
				if (cursor.LineOffset > num)
				{
					num = cursor.LineOffset;
				}
				if ((NAPIKMHPLFP == 0 || cursor.LineOffset < NAPIKMHPLFP) && analyzer.BPBEHGMHHGP())
				{
					throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a block scalar, find a tab character where an intendation space is expected.");
				}
				if (!analyzer.JCPPGIPDMBK())
				{
					break;
				}
				IIAFKNDBKLN.Append(OGNODHFDLNF());
				PCLFFOBJJFO = cursor.BJKDANAAGHK();
			}
			if (NAPIKMHPLFP == 0)
			{
				NAPIKMHPLFP = Math.Max(num, Math.Max(indent + 1, 1));
			}
			return NAPIKMHPLFP;
		}

		private void DJLOGNJKFNK(bool JNEECJAOIHK)
		{
			GBHAEIBOPNO();
			simpleKeyAllowed = false;
			tokens.JFGNCJCOCJA(JJAKHACHPAF(JNEECJAOIHK));
		}

		private Token JJAKHACHPAF(bool JNEECJAOIHK)
		{
			Mark iLENLCMAMBH = cursor.BJKDANAAGHK();
			Skip();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			while (true)
			{
				if (OPEFOACCBGC())
				{
					throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a quoted scalar, find unexpected document indicator.");
				}
				if (analyzer.AJCHNKGPEJB())
				{
					throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a quoted scalar, find unexpected end of stream.");
				}
				bool flag = false;
				while (!analyzer.MKOKPKHBDMD())
				{
					if (JNEECJAOIHK && analyzer.Check('\'') && analyzer.Check('\'', 1))
					{
						stringBuilder.Append('\'');
						Skip();
						Skip();
						continue;
					}
					if (analyzer.Check((!JNEECJAOIHK) ? '"' : '\''))
					{
						break;
					}
					if (!JNEECJAOIHK && analyzer.Check('\\') && analyzer.JCPPGIPDMBK(1))
					{
						Skip();
						BBLGADKMIPD();
						flag = true;
						break;
					}
					if (!JNEECJAOIHK && analyzer.Check('\\'))
					{
						int num = 0;
						char c = analyzer.Peek(1);
						switch (c)
						{
						case 'x':
							num = 2;
							break;
						case 'u':
							num = 4;
							break;
						case 'U':
							num = 8;
							break;
						default:
						{
							char value;
							if (simpleEscapeCodes.TryGetValue(c, out value))
							{
								stringBuilder.Append(value);
								break;
							}
							throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a quoted scalar, find unknown escape character.");
						}
						}
						Skip();
						Skip();
						if (num <= 0)
						{
							continue;
						}
						uint num2 = 0u;
						for (int i = 0; i < num; i++)
						{
							if (!analyzer.EMFKOPNCOFA(i))
							{
								throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a quoted scalar, did not find expected hexdecimal number.");
							}
							num2 = (uint)((num2 << 4) + analyzer.IGACACHGIGK(i));
						}
						if ((num2 >= 55296 && num2 <= 57343) || num2 > 1114111)
						{
							throw new SyntaxErrorException(iLENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a quoted scalar, find invalid Unicode character escape code.");
						}
						stringBuilder.Append((char)num2);
						for (int j = 0; j < num; j++)
						{
							Skip();
						}
					}
					else
					{
						stringBuilder.Append(HKACDMEFHIB());
					}
				}
				if (analyzer.Check((!JNEECJAOIHK) ? '"' : '\''))
				{
					break;
				}
				while (analyzer.MIGPEDGKJEG() || analyzer.JCPPGIPDMBK())
				{
					if (analyzer.MIGPEDGKJEG())
					{
						if (!flag)
						{
							stringBuilder2.Append(HKACDMEFHIB());
						}
						else
						{
							Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(OGNODHFDLNF());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(OGNODHFDLNF());
					}
				}
				if (flag)
				{
					if (BKAHPGIAGED(stringBuilder3, '\n'))
					{
						if (stringBuilder4.Length == 0)
						{
							stringBuilder.Append(' ');
						}
						else
						{
							stringBuilder.Append(stringBuilder4.ToString());
						}
					}
					else
					{
						stringBuilder.Append(stringBuilder3.ToString());
						stringBuilder.Append(stringBuilder4.ToString());
					}
					stringBuilder3.Length = 0;
					stringBuilder4.Length = 0;
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder2.Length = 0;
				}
			}
			Skip();
			return new Tokens.Scalar(stringBuilder.ToString(), (!JNEECJAOIHK) ? IBEOFCPMMJJ.DoubleQuoted : IBEOFCPMMJJ.SingleQuoted);
		}

		private void JDJODGJEDOF()
		{
			GBHAEIBOPNO();
			simpleKeyAllowed = false;
			tokens.JFGNCJCOCJA(GIEBOAFKIGE());
		}

		private Token GIEBOAFKIGE()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			bool flag = false;
			int num = indent + 1;
			Mark mark = cursor.BJKDANAAGHK();
			Mark pCLFFOBJJFO = mark;
			while (!OPEFOACCBGC() && !analyzer.Check('#'))
			{
				while (!analyzer.MKOKPKHBDMD())
				{
					if (flowLevel > 0 && analyzer.Check(':') && !analyzer.MKOKPKHBDMD(1))
					{
						throw new SyntaxErrorException(mark, cursor.BJKDANAAGHK(), "While scanning a plain scalar, find unexpected ':'.");
					}
					if ((analyzer.Check(':') && analyzer.MKOKPKHBDMD(1)) || (flowLevel > 0 && analyzer.Check(",:?[]{}")))
					{
						break;
					}
					if (flag || stringBuilder2.Length > 0)
					{
						if (flag)
						{
							if (BKAHPGIAGED(stringBuilder3, '\n'))
							{
								if (stringBuilder4.Length == 0)
								{
									stringBuilder.Append(' ');
								}
								else
								{
									stringBuilder.Append(stringBuilder4);
								}
							}
							else
							{
								stringBuilder.Append(stringBuilder3);
								stringBuilder.Append(stringBuilder4);
							}
							stringBuilder3.Length = 0;
							stringBuilder4.Length = 0;
							flag = false;
						}
						else
						{
							stringBuilder.Append(stringBuilder2);
							stringBuilder2.Length = 0;
						}
					}
					stringBuilder.Append(HKACDMEFHIB());
					pCLFFOBJJFO = cursor.BJKDANAAGHK();
				}
				if (!analyzer.MIGPEDGKJEG() && !analyzer.JCPPGIPDMBK())
				{
					break;
				}
				while (analyzer.MIGPEDGKJEG() || analyzer.JCPPGIPDMBK())
				{
					if (analyzer.MIGPEDGKJEG())
					{
						if (flag && cursor.LineOffset < num && analyzer.BPBEHGMHHGP())
						{
							throw new SyntaxErrorException(mark, cursor.BJKDANAAGHK(), "While scanning a plain scalar, find a tab character that violate intendation.");
						}
						if (!flag)
						{
							stringBuilder2.Append(HKACDMEFHIB());
						}
						else
						{
							Skip();
						}
					}
					else if (!flag)
					{
						stringBuilder2.Length = 0;
						stringBuilder3.Append(OGNODHFDLNF());
						flag = true;
					}
					else
					{
						stringBuilder4.Append(OGNODHFDLNF());
					}
				}
				if (flowLevel == 0 && cursor.LineOffset < num)
				{
					break;
				}
			}
			if (flag)
			{
				simpleKeyAllowed = true;
			}
			return new Tokens.Scalar(stringBuilder.ToString(), IBEOFCPMMJJ.Plain, mark, pCLFFOBJJFO);
		}

		private void OIDDFFJBIKK()
		{
			SimpleKey simpleKey = simpleKeys.Peek();
			if (simpleKey.IsPossible && simpleKey.IsRequired)
			{
				throw new SyntaxErrorException(simpleKey.Mark, simpleKey.Mark, "While scanning a simple key, could not find expected ':'.");
			}
			simpleKey.IsPossible = false;
		}

		private string ScanDirectiveName(Mark ILENLCMAMBH)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (analyzer.KJGBACCEGND())
			{
				stringBuilder.Append(HKACDMEFHIB());
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a directive, could not find expected directive name.");
			}
			if (!analyzer.MKOKPKHBDMD())
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a directive, find unexpected non-alphabetical character.");
			}
			return stringBuilder.ToString();
		}

		private void ANNKALGJLJH()
		{
			while (analyzer.MIGPEDGKJEG())
			{
				Skip();
			}
		}

		private Token HNBHLKOPJGL(Mark ILENLCMAMBH)
		{
			ANNKALGJLJH();
			int iBGMIGIFNJM = ScanVersionDirectiveNumber(ILENLCMAMBH);
			if (!analyzer.Check('.'))
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a %YAML directive, did not find expected digit or '.' character.");
			}
			Skip();
			int lDKAECLLDNG = ScanVersionDirectiveNumber(ILENLCMAMBH);
			return new VersionDirective(new Version(iBGMIGIFNJM, lDKAECLLDNG), ILENLCMAMBH, ILENLCMAMBH);
		}

		private Token GKOGPOOEBGL(Mark ILENLCMAMBH)
		{
			ANNKALGJLJH();
			string fODGADCGDBH = ScanTagHandle(true, ILENLCMAMBH);
			if (!analyzer.MIGPEDGKJEG())
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a %TAG directive, did not find expected whitespace.");
			}
			ANNKALGJLJH();
			string jMOHMLIGHHD = ScanTagUri(null, ILENLCMAMBH);
			if (!analyzer.MKOKPKHBDMD())
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a %TAG directive, did not find expected whitespace or line break.");
			}
			return new TagDirective(fODGADCGDBH, jMOHMLIGHHD, ILENLCMAMBH, ILENLCMAMBH);
		}

		private string ScanTagUri(string POLFAHOJJCN, Mark ILENLCMAMBH)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (POLFAHOJJCN != null && POLFAHOJJCN.Length > 1)
			{
				stringBuilder.Append(POLFAHOJJCN.Substring(1));
			}
			while (analyzer.KJGBACCEGND() || analyzer.Check(";/?:@&=+$,.!~*'()[]%"))
			{
				if (analyzer.Check('%'))
				{
					stringBuilder.Append(ScanUriEscapes(ILENLCMAMBH));
				}
				else
				{
					stringBuilder.Append(HKACDMEFHIB());
				}
			}
			if (stringBuilder.Length == 0)
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag, did not find expected tag URI.");
			}
			return stringBuilder.ToString();
		}

		private char ScanUriEscapes(Mark ILENLCMAMBH)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			do
			{
				if (!analyzer.Check('%') || !analyzer.EMFKOPNCOFA(1) || !analyzer.EMFKOPNCOFA(2))
				{
					throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag, did not find URI escaped octet.");
				}
				int num2 = (analyzer.IGACACHGIGK(1) << 4) + analyzer.IGACACHGIGK(2);
				if (num == 0)
				{
					num = (((num2 & 0x80) == 0) ? 1 : (((num2 & 0xE0) == 192) ? 2 : (((num2 & 0xF0) == 224) ? 3 : (((num2 & 0xF8) == 240) ? 4 : 0))));
					if (num == 0)
					{
						throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag, find an incorrect leading UTF-8 octet.");
					}
				}
				else if ((num2 & 0xC0) != 128)
				{
					throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag, find an incorrect trailing UTF-8 octet.");
				}
				list.Add((byte)num2);
				Skip();
				Skip();
				Skip();
			}
			while (--num > 0);
			char[] chars = Encoding.UTF8.GetChars(list.ToArray());
			if (chars.Length != 1)
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag, find an incorrect UTF-8 sequence.");
			}
			return chars[0];
		}

		private string ScanTagHandle(bool NDFFLMEDCFH, Mark ILENLCMAMBH)
		{
			if (!analyzer.Check('!'))
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a tag, did not find expected '!'.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(HKACDMEFHIB());
			while (analyzer.KJGBACCEGND())
			{
				stringBuilder.Append(HKACDMEFHIB());
			}
			if (analyzer.Check('!'))
			{
				stringBuilder.Append(HKACDMEFHIB());
			}
			else if (NDFFLMEDCFH && (stringBuilder.Length != 1 || stringBuilder[0] != '!'))
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While parsing a tag directive, did not find expected '!'.");
			}
			return stringBuilder.ToString();
		}

		private int ScanVersionDirectiveNumber(Mark ILENLCMAMBH)
		{
			int num = 0;
			int num2 = 0;
			while (analyzer.DDINBPOLPJP())
			{
				if (++num2 > 9)
				{
					throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a %YAML directive, find extremely long version number.");
				}
				num = num * 10 + analyzer.MDEJLGGFDCP();
				Skip();
			}
			if (num2 == 0)
			{
				throw new SyntaxErrorException(ILENLCMAMBH, cursor.BJKDANAAGHK(), "While scanning a %YAML directive, did not find expected version number.");
			}
			return num;
		}

		private void GBHAEIBOPNO()
		{
			bool mMIJJJMNNND = flowLevel == 0 && indent == cursor.LineOffset;
			if (simpleKeyAllowed)
			{
				SimpleKey t = new SimpleKey(true, mMIJJJMNNND, tokensParsed + tokens.Count, cursor);
				OIDDFFJBIKK();
				simpleKeys.Pop();
				simpleKeys.Push(t);
			}
		}
	}
}
