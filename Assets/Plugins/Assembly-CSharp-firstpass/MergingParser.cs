using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.Core;

public sealed class MergingParser : IParser
{
	private class JLKAJHOJBDF : IParsingEventVisitor
	{
		private ParsingEvent ACADPLINCAN;

		public ParsingEvent Clone(ParsingEvent FOPOKALJIIJ)
		{
			FOPOKALJIIJ.GPHIFFOGOGN(this);
			return ACADPLINCAN;
		}

		void IParsingEventVisitor.Visit(AnchorAlias FOPOKALJIIJ)
		{
			ACADPLINCAN = new AnchorAlias(FOPOKALJIIJ.OEAKCOHMIHH(), FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(StreamStart FOPOKALJIIJ)
		{
			throw new NotSupportedException();
		}

		void IParsingEventVisitor.Visit(HNKFEGCMBJB FOPOKALJIIJ)
		{
			throw new NotSupportedException();
		}

		void IParsingEventVisitor.Visit(DocumentStart FOPOKALJIIJ)
		{
			throw new NotSupportedException();
		}

		void IParsingEventVisitor.Visit(DocumentEnd FOPOKALJIIJ)
		{
			throw new NotSupportedException();
		}

		void IParsingEventVisitor.Visit(Scalar FOPOKALJIIJ)
		{
			ACADPLINCAN = new Scalar(null, FOPOKALJIIJ.LOIGCKFONHJ(), FOPOKALJIIJ.OEAKCOHMIHH(), FOPOKALJIIJ.HALCJLMJDII(), FOPOKALJIIJ.BIDLJMEAFMI(), FOPOKALJIIJ.NIENIKOPKOG(), FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(JODGINIKFJF FOPOKALJIIJ)
		{
			ACADPLINCAN = new JODGINIKFJF(null, FOPOKALJIIJ.LOIGCKFONHJ(), FOPOKALJIIJ.BBBGHODAEIN(), FOPOKALJIIJ.HALCJLMJDII(), FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(AKMKLAINLOL FOPOKALJIIJ)
		{
			ACADPLINCAN = new AKMKLAINLOL(FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(MappingStart FOPOKALJIIJ)
		{
			ACADPLINCAN = new MappingStart(null, FOPOKALJIIJ.LOIGCKFONHJ(), FOPOKALJIIJ.BBBGHODAEIN(), FOPOKALJIIJ.HALCJLMJDII(), FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(BLFPJCPALDH FOPOKALJIIJ)
		{
			ACADPLINCAN = new BLFPJCPALDH(FOPOKALJIIJ.OGPHJPFHBJL(), FOPOKALJIIJ.GDJHIJHFPHA());
		}

		void IParsingEventVisitor.Visit(Comment FOPOKALJIIJ)
		{
			throw new NotSupportedException();
		}
	}

	private readonly List<ParsingEvent> CHKBHFONNGG = new List<ParsingEvent>();

	private readonly IParser ICOLPJPILCG;

	private int _currentIndex = -1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ParsingEvent EADAACFGGGM;

	public ParsingEvent BLOOLFFMKFI
	{
		get
		{
			return AOJJOEHEPGM();
		}
		private set
		{
			GAKMJOBBBAD(value);
		}
	}

	public MergingParser(IParser FIMPGLKJDKK)
	{
		ICOLPJPILCG = FIMPGLKJDKK;
	}

	public ParsingEvent AOJJOEHEPGM()
	{
		return EADAACFGGGM;
	}

	private void GAKMJOBBBAD(ParsingEvent value)
	{
		EADAACFGGGM = value;
	}

	public bool PCCMLADDNDG()
	{
		if (_currentIndex < 0)
		{
			while (ICOLPJPILCG.PCCMLADDNDG())
			{
				CHKBHFONNGG.Add(ICOLPJPILCG.AOJJOEHEPGM());
			}
			for (int num = CHKBHFONNGG.Count - 2; num >= 0; num--)
			{
				Scalar lEACOCDHICF = CHKBHFONNGG[num] as Scalar;
				if (lEACOCDHICF == null || !(lEACOCDHICF.OEAKCOHMIHH() == "<<"))
				{
					continue;
				}
				AnchorAlias mBEGNNDMDKH = CHKBHFONNGG[num + 1] as AnchorAlias;
				if (mBEGNNDMDKH != null)
				{
					IEnumerable<ParsingEvent> collection = LCHALMNNAGA(mBEGNNDMDKH.OEAKCOHMIHH());
					CHKBHFONNGG.RemoveRange(num, 2);
					CHKBHFONNGG.InsertRange(num, collection);
					continue;
				}
				JODGINIKFJF jODGINIKFJF = CHKBHFONNGG[num + 1] as JODGINIKFJF;
				if (jODGINIKFJF != null)
				{
					List<IEnumerable<ParsingEvent>> list = new List<IEnumerable<ParsingEvent>>();
					bool flag = false;
					for (int i = num + 2; i < CHKBHFONNGG.Count; i++)
					{
						mBEGNNDMDKH = CHKBHFONNGG[i] as AnchorAlias;
						if (mBEGNNDMDKH != null)
						{
							list.Add(LCHALMNNAGA(mBEGNNDMDKH.OEAKCOHMIHH()));
						}
						else if (CHKBHFONNGG[i] is AKMKLAINLOL)
						{
							CHKBHFONNGG.RemoveRange(num, i - num + 1);
							CHKBHFONNGG.InsertRange(num, list.SelectMany((IEnumerable<ParsingEvent> FOPOKALJIIJ) => FOPOKALJIIJ));
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				throw new SemanticErrorException(lEACOCDHICF.OGPHJPFHBJL(), lEACOCDHICF.GDJHIJHFPHA(), "Unrecognized merge key pattern");
			}
		}
		int num2 = _currentIndex + 1;
		if (num2 < CHKBHFONNGG.Count)
		{
			GAKMJOBBBAD(CHKBHFONNGG[num2]);
			_currentIndex = num2;
			return true;
		}
		return false;
	}

	private IEnumerable<ParsingEvent> LCHALMNNAGA(string mappingAlias)
	{
		JLKAJHOJBDF PFCAJIFNHMC = new JLKAJHOJBDF();
		int nesting = 0;
		return (from FOPOKALJIIJ in CHKBHFONNGG.SkipWhile((ParsingEvent FOPOKALJIIJ) =>
			{
				MappingStart oGMPNFCPPDH = FOPOKALJIIJ as MappingStart;
				return oGMPNFCPPDH == null || oGMPNFCPPDH.HCPOJDFJFMM() != mappingAlias;
			}).Skip(1).TakeWhile((ParsingEvent FOPOKALJIIJ) => (nesting += FOPOKALJIIJ.DPIMLJJFMCO()) >= 0)
			select PFCAJIFNHMC.Clone(FOPOKALJIIJ)).ToList();
	}
}
