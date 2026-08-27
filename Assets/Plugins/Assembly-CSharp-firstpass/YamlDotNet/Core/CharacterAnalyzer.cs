using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class CharacterAnalyzer<TBuffer> where TBuffer : ILookAheadBuffer
	{
		private readonly TBuffer buffer;

		public TBuffer Buffer
		{
			get
			{
				return buffer;
			}
		}

		public bool EndOfInput
		{
			get
			{
				return buffer.EndOfInput;
			}
		}

		public CharacterAnalyzer(TBuffer buffer)
		{
			buffer = buffer;
		}

		public char Peek(int IPCOBJBKNAO)
		{
			return buffer.Peek(IPCOBJBKNAO);
		}

		public void Skip(int BDBOAEGELMC)
		{
			buffer.Skip(BDBOAEGELMC);
		}

		public bool KJGBACCEGND(int IPCOBJBKNAO = 0)
		{
			char c = buffer.Peek(IPCOBJBKNAO);
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-';
		}

		public bool EAMJHPLDDLE(int IPCOBJBKNAO = 0)
		{
			return buffer.Peek(IPCOBJBKNAO) <= '\u007f';
		}

		public bool IGNGBDLCMGB(int IPCOBJBKNAO = 0)
		{
			char c = buffer.Peek(IPCOBJBKNAO);
			return c == '\t' || c == '\n' || c == '\r' || (c >= ' ' && c <= '~') || c == '\u0085' || (c >= '\u00a0' && c <= '\ud7ff') || (c >= '\ue000' && c <= '\ufffd');
		}

		public bool DDINBPOLPJP(int IPCOBJBKNAO = 0)
		{
			char c = buffer.Peek(IPCOBJBKNAO);
			return c >= '0' && c <= '9';
		}

		public int MDEJLGGFDCP(int IPCOBJBKNAO = 0)
		{
			return buffer.Peek(IPCOBJBKNAO) - 48;
		}

		public bool EMFKOPNCOFA(int IPCOBJBKNAO)
		{
			char c = buffer.Peek(IPCOBJBKNAO);
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
		}

		public int IGACACHGIGK(int IPCOBJBKNAO)
		{
			char c = buffer.Peek(IPCOBJBKNAO);
			if (c <= '9')
			{
				return c - 48;
			}
			if (c <= 'F')
			{
				return c - 65 + 10;
			}
			return c - 97 + 10;
		}

		public bool NBLLOLGNFGM(int IPCOBJBKNAO = 0)
		{
			return Check(' ', IPCOBJBKNAO);
		}

		public bool AJCHNKGPEJB(int IPCOBJBKNAO = 0)
		{
			return Check('\0', IPCOBJBKNAO);
		}

		public bool BPBEHGMHHGP(int IPCOBJBKNAO = 0)
		{
			return Check('\t', IPCOBJBKNAO);
		}

		public bool MIGPEDGKJEG(int IPCOBJBKNAO = 0)
		{
			return NBLLOLGNFGM(IPCOBJBKNAO) || BPBEHGMHHGP(IPCOBJBKNAO);
		}

		public bool JCPPGIPDMBK(int IPCOBJBKNAO = 0)
		{
			return Check("\r\n\u0085\u2028\u2029", IPCOBJBKNAO);
		}

		public bool DHPOAOIAGPE(int IPCOBJBKNAO = 0)
		{
			return Check('\r', IPCOBJBKNAO) && Check('\n', IPCOBJBKNAO + 1);
		}

		public bool PDOIBEFPDEB(int IPCOBJBKNAO = 0)
		{
			return JCPPGIPDMBK(IPCOBJBKNAO) || AJCHNKGPEJB(IPCOBJBKNAO);
		}

		public bool MKOKPKHBDMD(int IPCOBJBKNAO = 0)
		{
			return MIGPEDGKJEG(IPCOBJBKNAO) || PDOIBEFPDEB(IPCOBJBKNAO);
		}

		public bool Check(char EFCPGPEFNJI, int IPCOBJBKNAO = 0)
		{
			return buffer.Peek(IPCOBJBKNAO) == EFCPGPEFNJI;
		}

		public bool Check(string PAJEGDIJODA, int IPCOBJBKNAO = 0)
		{
			char value = buffer.Peek(IPCOBJBKNAO);
			return PAJEGDIJODA.IndexOf(value) != -1;
		}
	}
}
