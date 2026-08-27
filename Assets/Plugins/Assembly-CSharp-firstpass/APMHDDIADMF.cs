using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;
using Tokens = YamlDotNet.Core.Tokens;

public class APMHDDIADMF : IParser
{
	private readonly Stack<JMNFHMEBPFD> LNJBMLMFKDH = new Stack<JMNFHMEBPFD>();

	private readonly TagDirectiveCollection FMCEHNBELJF = new TagDirectiveCollection();

	private JMNFHMEBPFD state;

	private readonly Scanner scanner;

	private ParsingEvent OBLEMIHLFII;

	private Token currentToken;

	private readonly Queue<ParsingEvent> CIEDDLCIKOF = new Queue<ParsingEvent>();

	public ParsingEvent BLOOLFFMKFI
	{
		get
		{
			return AOJJOEHEPGM();
		}
	}

	public APMHDDIADMF(TextReader NILNDHEKNLJ)
		: this(new Scanner(NILNDHEKNLJ))
	{
	}

	public APMHDDIADMF(Scanner scanner)
	{
		this.scanner = scanner;
	}

	private Token NHGFHMEMCJL()
	{
		if (currentToken == null)
		{
			while (scanner.NCFHFMEKMFC())
			{
				currentToken = scanner.Current;
				Tokens.Comment comment = currentToken as Tokens.Comment;
				if (comment != null)
				{
					CIEDDLCIKOF.Enqueue(new Comment(comment.Value, comment.IsInline, comment.Start, comment.End));
					continue;
				}
				break;
			}
		}
		return currentToken;
	}

	public ParsingEvent AOJJOEHEPGM()
	{
		return OBLEMIHLFII;
	}

	public bool PCCMLADDNDG()
	{
		if (state == JMNFHMEBPFD.StreamEnd)
		{
			OBLEMIHLFII = null;
			return false;
		}
		if (CIEDDLCIKOF.Count == 0)
		{
			CIEDDLCIKOF.Enqueue(GIKHOKIFGLH());
		}
		OBLEMIHLFII = CIEDDLCIKOF.Dequeue();
		return true;
	}

	private ParsingEvent GIKHOKIFGLH()
	{
		switch (state)
		{
		case JMNFHMEBPFD.StreamStart:
			return HCBFPBNAKNH();
		case JMNFHMEBPFD.ImplicitDocumentStart:
			return CNBFMCIAGCE(true);
		case JMNFHMEBPFD.DocumentStart:
			return CNBFMCIAGCE(false);
		case JMNFHMEBPFD.DocumentContent:
			return IDNHLAABJIG();
		case JMNFHMEBPFD.DocumentEnd:
			return ABFIJEGPJMD();
		case JMNFHMEBPFD.BlockNode:
			return GLNMJNFLLIN(true, false);
		case JMNFHMEBPFD.BlockNodeOrIndentlessSequence:
			return GLNMJNFLLIN(true, true);
		case JMNFHMEBPFD.FlowNode:
			return GLNMJNFLLIN(false, false);
		case JMNFHMEBPFD.BlockSequenceFirstEntry:
			return FNFPNEKDLEC(true);
		case JMNFHMEBPFD.BlockSequenceEntry:
			return FNFPNEKDLEC(false);
		case JMNFHMEBPFD.IndentlessSequenceEntry:
			return IHNDAPPGHIH();
		case JMNFHMEBPFD.BlockMappingFirstKey:
			return JPOLLBAJILN(true);
		case JMNFHMEBPFD.BlockMappingKey:
			return JPOLLBAJILN(false);
		case JMNFHMEBPFD.BlockMappingValue:
			return KFDIGPBADNK();
		case JMNFHMEBPFD.FlowSequenceFirstEntry:
			return CJLJHDGCKOF(true);
		case JMNFHMEBPFD.FlowSequenceEntry:
			return CJLJHDGCKOF(false);
		case JMNFHMEBPFD.FlowSequenceEntryMappingKey:
			return EPLKGHJJGOK();
		case JMNFHMEBPFD.FlowSequenceEntryMappingValue:
			return JDBFBBMKHLE();
		case JMNFHMEBPFD.FlowSequenceEntryMappingEnd:
			return CNMGOEHOPBF();
		case JMNFHMEBPFD.FlowMappingFirstKey:
			return EDDOJPBKCBL(true);
		case JMNFHMEBPFD.FlowMappingKey:
			return EDDOJPBKCBL(false);
		case JMNFHMEBPFD.FlowMappingValue:
			return DDGOJCJPELG(false);
		case JMNFHMEBPFD.FlowMappingEmptyValue:
			return DDGOJCJPELG(true);
		default:
			throw new InvalidOperationException();
		}
	}

	private void Skip()
	{
		if (currentToken != null)
		{
			currentToken = null;
			scanner.KPFPDDBILAE();
		}
	}

	private ParsingEvent HCBFPBNAKNH()
	{
		Tokens.StreamStart streamStart = NHGFHMEMCJL() as Tokens.StreamStart;
		if (streamStart == null)
		{
			Token token = NHGFHMEMCJL();
			throw new SemanticErrorException(token.Start, token.End, "Did not find expected <stream-start>.");
		}
		Skip();
		state = JMNFHMEBPFD.ImplicitDocumentStart;
		return new StreamStart(streamStart.Start, streamStart.End);
	}

	private ParsingEvent CNBFMCIAGCE(bool isImplicit)
	{
		if (!isImplicit)
		{
			while (NHGFHMEMCJL() is DocumentEnd)
			{
				Skip();
			}
		}
		if (isImplicit && !(NHGFHMEMCJL() is VersionDirective) && !(NHGFHMEMCJL() is TagDirective) && !(NHGFHMEMCJL() is DocumentStart) && !(NHGFHMEMCJL() is StreamEnd))
		{
			TagDirectiveCollection cPAIGLNDIOK = new TagDirectiveCollection();
			FEDDEBIJABD(cPAIGLNDIOK);
			LNJBMLMFKDH.Push(JMNFHMEBPFD.DocumentEnd);
			state = JMNFHMEBPFD.BlockNode;
			return new DocumentStart(null, cPAIGLNDIOK, true, NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
		}
		if (!(NHGFHMEMCJL() is StreamEnd))
		{
			Mark start = NHGFHMEMCJL().Start;
			TagDirectiveCollection cPAIGLNDIOK2 = new TagDirectiveCollection();
			VersionDirective aHLPODLKBEP = FEDDEBIJABD(cPAIGLNDIOK2);
			Token token = NHGFHMEMCJL();
			if (!(token is DocumentStart))
			{
				throw new SemanticErrorException(token.Start, token.End, "Did not find expected <document start>.");
			}
			LNJBMLMFKDH.Push(JMNFHMEBPFD.DocumentEnd);
			state = JMNFHMEBPFD.DocumentContent;
			ParsingEvent result = new DocumentStart(aHLPODLKBEP, cPAIGLNDIOK2, false, start, token.End);
			Skip();
			return result;
		}
		state = JMNFHMEBPFD.StreamEnd;
		ParsingEvent result2 = new HNKFEGCMBJB(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
		if (scanner.NCFHFMEKMFC())
		{
			throw new InvalidOperationException("The scanner should contain no more tokens.");
		}
		return result2;
	}

	private VersionDirective FEDDEBIJABD(TagDirectiveCollection CPAIGLNDIOK)
	{
		VersionDirective versionDirective = null;
		while (true)
		{
			VersionDirective versionDirective2;
			if ((versionDirective2 = NHGFHMEMCJL() as VersionDirective) != null)
			{
				if (versionDirective != null)
				{
					throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found duplicate %YAML directive.");
				}
				if (versionDirective2.Version.Major != 1 || versionDirective2.Version.Minor != 1)
				{
					throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found incompatible YAML document.");
				}
				versionDirective = versionDirective2;
			}
			else
			{
				TagDirective tagDirective;
				if ((tagDirective = NHGFHMEMCJL() as TagDirective) == null)
				{
					break;
				}
				if (FMCEHNBELJF.Contains(tagDirective.Handle))
				{
					throw new SemanticErrorException(tagDirective.Start, tagDirective.End, "Found duplicate %TAG directive.");
				}
				FMCEHNBELJF.Add(tagDirective);
				if (CPAIGLNDIOK != null)
				{
					CPAIGLNDIOK.Add(tagDirective);
				}
			}
			Skip();
		}
		if (CPAIGLNDIOK != null)
		{
			EMGDBLHBDMH(CPAIGLNDIOK);
		}
		EMGDBLHBDMH(FMCEHNBELJF);
		return versionDirective;
	}

	private static void EMGDBLHBDMH(TagDirectiveCollection AJGCJGFNFIP)
	{
		TagDirective[] gNPKLFKPLCM = CHOAMHPCPFL.DefaultTagDirectives;
		foreach (TagDirective tagDirective in gNPKLFKPLCM)
		{
			if (!AJGCJGFNFIP.Contains(tagDirective))
			{
				AJGCJGFNFIP.Add(tagDirective);
			}
		}
	}

	private ParsingEvent IDNHLAABJIG()
	{
		if (NHGFHMEMCJL() is VersionDirective || NHGFHMEMCJL() is TagDirective || NHGFHMEMCJL() is DocumentStart || NHGFHMEMCJL() is DocumentEnd || NHGFHMEMCJL() is StreamEnd)
		{
			state = LNJBMLMFKDH.Pop();
			return MCFMBPLNMDE(scanner.CurrentPosition);
		}
		return GLNMJNFLLIN(true, false);
	}

	private static ParsingEvent MCFMBPLNMDE(Mark MGMMDGFPBLP)
	{
		return new Scalar(null, null, string.Empty, IBEOFCPMMJJ.Plain, true, false, MGMMDGFPBLP, MGMMDGFPBLP);
	}

	private ParsingEvent GLNMJNFLLIN(bool OOCLHFGEPML, bool GGHAMOFCLMP)
	{
		Tokens.AnchorAlias anchorAlias = NHGFHMEMCJL() as Tokens.AnchorAlias;
		if (anchorAlias != null)
		{
			state = LNJBMLMFKDH.Pop();
			ParsingEvent result = new AnchorAlias(anchorAlias.Value, anchorAlias.Start, anchorAlias.End);
			Skip();
			return result;
		}
		Mark start = NHGFHMEMCJL().Start;
		Anchor anchor = null;
		Tag tag = null;
		while (true)
		{
			if (anchor == null && (anchor = NHGFHMEMCJL() as Anchor) != null)
			{
				Skip();
				continue;
			}
			if (tag == null && (tag = NHGFHMEMCJL() as Tag) != null)
			{
				Skip();
				continue;
			}
			break;
		}
		string text = null;
		if (tag != null)
		{
			if (string.IsNullOrEmpty(tag.Handle))
			{
				text = tag.Suffix;
			}
			else
			{
				if (!FMCEHNBELJF.Contains(tag.Handle))
				{
					throw new SemanticErrorException(tag.Start, tag.End, "While parsing a node, find undefined tag handle.");
				}
				text = FMCEHNBELJF[tag.Handle].Prefix + tag.Suffix;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = null;
		}
		string text2 = ((anchor == null) ? null : ((!string.IsNullOrEmpty(anchor.Value)) ? anchor.Value : null));
		bool flag = string.IsNullOrEmpty(text);
		if (GGHAMOFCLMP && NHGFHMEMCJL() is BlockEntry)
		{
			state = JMNFHMEBPFD.IndentlessSequenceEntry;
			return new JODGINIKFJF(text2, text, flag, NBCBGEPFIKG.Block, start, NHGFHMEMCJL().End);
		}
		Tokens.Scalar scalar = NHGFHMEMCJL() as Tokens.Scalar;
		if (scalar != null)
		{
			bool oCBIEJBMFJN = false;
			bool fAKBCOKEHGP = false;
			if ((scalar.Style == IBEOFCPMMJJ.Plain && text == null) || text == "!")
			{
				oCBIEJBMFJN = true;
			}
			else if (text == null)
			{
				fAKBCOKEHGP = true;
			}
			state = LNJBMLMFKDH.Pop();
			ParsingEvent result2 = new Scalar(text2, text, scalar.Value, scalar.Style, oCBIEJBMFJN, fAKBCOKEHGP, start, scalar.End);
			Skip();
			return result2;
		}
		FlowSequenceStart flowSequenceStart = NHGFHMEMCJL() as FlowSequenceStart;
		if (flowSequenceStart != null)
		{
			state = JMNFHMEBPFD.FlowSequenceFirstEntry;
			return new JODGINIKFJF(text2, text, flag, NBCBGEPFIKG.Flow, start, flowSequenceStart.End);
		}
		FlowMappingStart flowMappingStart = NHGFHMEMCJL() as FlowMappingStart;
		if (flowMappingStart != null)
		{
			state = JMNFHMEBPFD.FlowMappingFirstKey;
			return new MappingStart(text2, text, flag, FGDKNBEFPFN.Flow, start, flowMappingStart.End);
		}
		if (OOCLHFGEPML)
		{
			BlockSequenceStart blockSequenceStart = NHGFHMEMCJL() as BlockSequenceStart;
			if (blockSequenceStart != null)
			{
				state = JMNFHMEBPFD.BlockSequenceFirstEntry;
				return new JODGINIKFJF(text2, text, flag, NBCBGEPFIKG.Block, start, blockSequenceStart.End);
			}
			BlockMappingStart blockMappingStart = NHGFHMEMCJL() as BlockMappingStart;
			if (blockMappingStart != null)
			{
				state = JMNFHMEBPFD.BlockMappingFirstKey;
				return new MappingStart(text2, text, flag, FGDKNBEFPFN.Block, start, NHGFHMEMCJL().End);
			}
		}
		if (text2 != null || tag != null)
		{
			state = LNJBMLMFKDH.Pop();
			return new Scalar(text2, text, string.Empty, IBEOFCPMMJJ.Plain, flag, false, start, NHGFHMEMCJL().End);
		}
		Token token = NHGFHMEMCJL();
		throw new SemanticErrorException(token.Start, token.End, "While parsing a node, did not find expected node content.");
	}

	private ParsingEvent ABFIJEGPJMD()
	{
		bool fFDGFENKBKH = true;
		Mark start = NHGFHMEMCJL().Start;
		Mark pCLFFOBJJFO = start;
		if (NHGFHMEMCJL() is DocumentEnd)
		{
			pCLFFOBJJFO = NHGFHMEMCJL().End;
			Skip();
			fFDGFENKBKH = false;
		}
		FMCEHNBELJF.Clear();
		state = JMNFHMEBPFD.DocumentStart;
		return new DocumentEnd(fFDGFENKBKH, start, pCLFFOBJJFO);
	}

	private ParsingEvent FNFPNEKDLEC(bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			NHGFHMEMCJL();
			Skip();
		}
		if (NHGFHMEMCJL() is BlockEntry)
		{
			Mark end = NHGFHMEMCJL().End;
			Skip();
			if (!(NHGFHMEMCJL() is BlockEntry) && !(NHGFHMEMCJL() is BlockEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.BlockSequenceEntry);
				return GLNMJNFLLIN(true, false);
			}
			state = JMNFHMEBPFD.BlockSequenceEntry;
			return MCFMBPLNMDE(end);
		}
		if (NHGFHMEMCJL() is BlockEnd)
		{
			state = LNJBMLMFKDH.Pop();
			ParsingEvent result = new AKMKLAINLOL(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
			Skip();
			return result;
		}
		Token token = NHGFHMEMCJL();
		throw new SemanticErrorException(token.Start, token.End, "While parsing a block collection, did not find expected '-' indicator.");
	}

	private ParsingEvent IHNDAPPGHIH()
	{
		if (NHGFHMEMCJL() is BlockEntry)
		{
			Mark end = NHGFHMEMCJL().End;
			Skip();
			if (!(NHGFHMEMCJL() is BlockEntry) && !(NHGFHMEMCJL() is Key) && !(NHGFHMEMCJL() is Value) && !(NHGFHMEMCJL() is BlockEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.IndentlessSequenceEntry);
				return GLNMJNFLLIN(true, false);
			}
			state = JMNFHMEBPFD.IndentlessSequenceEntry;
			return MCFMBPLNMDE(end);
		}
		state = LNJBMLMFKDH.Pop();
		return new AKMKLAINLOL(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
	}

	private ParsingEvent JPOLLBAJILN(bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			NHGFHMEMCJL();
			Skip();
		}
		if (NHGFHMEMCJL() is Key)
		{
			Mark end = NHGFHMEMCJL().End;
			Skip();
			if (!(NHGFHMEMCJL() is Key) && !(NHGFHMEMCJL() is Value) && !(NHGFHMEMCJL() is BlockEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.BlockMappingValue);
				return GLNMJNFLLIN(true, true);
			}
			state = JMNFHMEBPFD.BlockMappingValue;
			return MCFMBPLNMDE(end);
		}
		if (NHGFHMEMCJL() is BlockEnd)
		{
			state = LNJBMLMFKDH.Pop();
			ParsingEvent result = new BLFPJCPALDH(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
			Skip();
			return result;
		}
		Token token = NHGFHMEMCJL();
		throw new SemanticErrorException(token.Start, token.End, "While parsing a block mapping, did not find expected key.");
	}

	private ParsingEvent KFDIGPBADNK()
	{
		if (NHGFHMEMCJL() is Value)
		{
			Mark end = NHGFHMEMCJL().End;
			Skip();
			if (!(NHGFHMEMCJL() is Key) && !(NHGFHMEMCJL() is Value) && !(NHGFHMEMCJL() is BlockEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.BlockMappingKey);
				return GLNMJNFLLIN(true, true);
			}
			state = JMNFHMEBPFD.BlockMappingKey;
			return MCFMBPLNMDE(end);
		}
		state = JMNFHMEBPFD.BlockMappingKey;
		return MCFMBPLNMDE(NHGFHMEMCJL().Start);
	}

	private ParsingEvent CJLJHDGCKOF(bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			NHGFHMEMCJL();
			Skip();
		}
		ParsingEvent result;
		if (!(NHGFHMEMCJL() is FlowSequenceEnd))
		{
			if (!IKNHLPGLLKB)
			{
				if (!(NHGFHMEMCJL() is FlowEntry))
				{
					Token token = NHGFHMEMCJL();
					throw new SemanticErrorException(token.Start, token.End, "While parsing a flow sequence, did not find expected ',' or ']'.");
				}
				Skip();
			}
			if (NHGFHMEMCJL() is Key)
			{
				state = JMNFHMEBPFD.FlowSequenceEntryMappingKey;
				result = new MappingStart(null, null, true, FGDKNBEFPFN.Flow);
				Skip();
				return result;
			}
			if (!(NHGFHMEMCJL() is FlowSequenceEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowSequenceEntry);
				return GLNMJNFLLIN(false, false);
			}
		}
		state = LNJBMLMFKDH.Pop();
		result = new AKMKLAINLOL(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
		Skip();
		return result;
	}

	private ParsingEvent EPLKGHJJGOK()
	{
		if (!(NHGFHMEMCJL() is Value) && !(NHGFHMEMCJL() is FlowEntry) && !(NHGFHMEMCJL() is FlowSequenceEnd))
		{
			LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowSequenceEntryMappingValue);
			return GLNMJNFLLIN(false, false);
		}
		Mark end = NHGFHMEMCJL().End;
		Skip();
		state = JMNFHMEBPFD.FlowSequenceEntryMappingValue;
		return MCFMBPLNMDE(end);
	}

	private ParsingEvent JDBFBBMKHLE()
	{
		if (NHGFHMEMCJL() is Value)
		{
			Skip();
			if (!(NHGFHMEMCJL() is FlowEntry) && !(NHGFHMEMCJL() is FlowSequenceEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowSequenceEntryMappingEnd);
				return GLNMJNFLLIN(false, false);
			}
		}
		state = JMNFHMEBPFD.FlowSequenceEntryMappingEnd;
		return MCFMBPLNMDE(NHGFHMEMCJL().Start);
	}

	private ParsingEvent CNMGOEHOPBF()
	{
		state = JMNFHMEBPFD.FlowSequenceEntry;
		return new BLFPJCPALDH(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
	}

	private ParsingEvent EDDOJPBKCBL(bool IKNHLPGLLKB)
	{
		if (IKNHLPGLLKB)
		{
			NHGFHMEMCJL();
			Skip();
		}
		if (!(NHGFHMEMCJL() is FlowMappingEnd))
		{
			if (!IKNHLPGLLKB)
			{
				if (!(NHGFHMEMCJL() is FlowEntry))
				{
					Token token = NHGFHMEMCJL();
					throw new SemanticErrorException(token.Start, token.End, "While parsing a flow mapping,  did not find expected ',' or '}'.");
				}
				Skip();
			}
			if (NHGFHMEMCJL() is Key)
			{
				Skip();
				if (!(NHGFHMEMCJL() is Value) && !(NHGFHMEMCJL() is FlowEntry) && !(NHGFHMEMCJL() is FlowMappingEnd))
				{
					LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowMappingValue);
					return GLNMJNFLLIN(false, false);
				}
				state = JMNFHMEBPFD.FlowMappingValue;
				return MCFMBPLNMDE(NHGFHMEMCJL().Start);
			}
			if (!(NHGFHMEMCJL() is FlowMappingEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowMappingEmptyValue);
				return GLNMJNFLLIN(false, false);
			}
		}
		state = LNJBMLMFKDH.Pop();
		ParsingEvent result = new BLFPJCPALDH(NHGFHMEMCJL().Start, NHGFHMEMCJL().End);
		Skip();
		return result;
	}

	private ParsingEvent DDGOJCJPELG(bool LPGLCGMMPHN)
	{
		if (LPGLCGMMPHN)
		{
			state = JMNFHMEBPFD.FlowMappingKey;
			return MCFMBPLNMDE(NHGFHMEMCJL().Start);
		}
		if (NHGFHMEMCJL() is Value)
		{
			Skip();
			if (!(NHGFHMEMCJL() is FlowEntry) && !(NHGFHMEMCJL() is FlowMappingEnd))
			{
				LNJBMLMFKDH.Push(JMNFHMEBPFD.FlowMappingKey);
				return GLNMJNFLLIN(false, false);
			}
		}
		state = JMNFHMEBPFD.FlowMappingKey;
		return MCFMBPLNMDE(NHGFHMEMCJL().Start);
	}
}
