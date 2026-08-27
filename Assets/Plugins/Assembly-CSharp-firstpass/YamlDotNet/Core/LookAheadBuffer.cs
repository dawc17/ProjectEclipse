using System;
using System.IO;

namespace YamlDotNet.Core
{
	[Serializable]
	public class LookAheadBuffer : ILookAheadBuffer
	{
		private readonly TextReader input;

		private readonly char[] buffer;

		private int firstIndex;

		private int count;

		private bool endOfInput;

		public bool EndOfInput
		{
			get
			{
				return endOfInput && count == 0;
			}
		}

		public LookAheadBuffer(TextReader NILNDHEKNLJ, int LBGGELDABPF)
		{
			if (NILNDHEKNLJ == null)
			{
				throw new ArgumentNullException("input");
			}
			if (LBGGELDABPF < 1)
			{
				throw new ArgumentOutOfRangeException("capacity", "The capacity must be positive.");
			}
			input = NILNDHEKNLJ;
			buffer = new char[LBGGELDABPF];
		}

		private int GetIndexForOffset(int IPCOBJBKNAO)
		{
			int num = firstIndex + IPCOBJBKNAO;
			if (num >= buffer.Length)
			{
				num -= buffer.Length;
			}
			return num;
		}

		public char Peek(int IPCOBJBKNAO)
		{
			if (IPCOBJBKNAO < 0 || IPCOBJBKNAO >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "The offset must be betwwen zero and the capacity of the buffer.");
			}
			CGGPDODMKCF(IPCOBJBKNAO);
			if (IPCOBJBKNAO < count)
			{
				return buffer[GetIndexForOffset(IPCOBJBKNAO)];
			}
			return '\0';
		}

		public void CGGPDODMKCF(int BDBOAEGELMC)
		{
			while (BDBOAEGELMC >= count)
			{
				int num = input.Read();
				if (num >= 0)
				{
					int num2 = GetIndexForOffset(count);
					buffer[num2] = (char)num;
					count++;
					continue;
				}
				endOfInput = true;
				break;
			}
		}

		public void Skip(int BDBOAEGELMC)
		{
			if (BDBOAEGELMC < 1 || BDBOAEGELMC > count)
			{
				throw new ArgumentOutOfRangeException("length", "The length must be between 1 and the number of characters in the buffer. Use the Peek() and / or Cache() methods to fill the buffer.");
			}
			firstIndex = GetIndexForOffset(BDBOAEGELMC);
			count -= BDBOAEGELMC;
		}
	}
}
