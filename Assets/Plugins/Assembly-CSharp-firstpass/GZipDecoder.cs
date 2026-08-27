using System;
using Unity.IO.Compression;

internal class GZipDecoder : IFileFormatReader
{
	internal enum LKOONNNEIMK
	{
		ReadingID1 = 0,
		ReadingID2 = 1,
		ReadingCM = 2,
		ReadingFLG = 3,
		ReadingMMTime = 4,
		ReadingXFL = 5,
		ReadingOS = 6,
		ReadingXLen1 = 7,
		ReadingXLen2 = 8,
		ReadingXLenData = 9,
		ReadingFileName = 10,
		ReadingComment = 11,
		ReadingCRC16Part1 = 12,
		ReadingCRC16Part2 = 13,
		Done = 14,
		ReadingCRC = 15,
		ReadingFileSize = 16
	}

	[Flags]
	internal enum JKHEMHLDMFD
	{
		CRCFlag = 2,
		ExtraFieldsFlag = 4,
		FileNameFlag = 8,
		CommentFlag = 0x10
	}

	private LKOONNNEIMK LKFCDEHOOCP;

	private LKOONNNEIMK JNBHMAJIKLC;

	private int BHBDNLEACPM;

	private int OLNPJMKFACP;

	private uint GHMJIHKFBFJ;

	private uint FPPEOFLDECJ;

	private int DOLKGPHFKPJ;

	private uint NGBENFAENID;

	private long actualStreamSizeModulo;

	public GZipDecoder()
	{
		Reset();
	}

	public void Reset()
	{
		LKFCDEHOOCP = LKOONNNEIMK.ReadingID1;
		JNBHMAJIKLC = LKOONNNEIMK.ReadingCRC;
		GHMJIHKFBFJ = 0u;
		FPPEOFLDECJ = 0u;
	}

	public bool DJJBPAJHJFI(InputBuffer NILNDHEKNLJ)
	{
		switch (LKFCDEHOOCP)
		{
		case LKOONNNEIMK.ReadingID1:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			if (num != 31)
			{
				throw new InvalidDataException(SR.GetString("Corrupted gzip header"));
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingID2;
			goto case LKOONNNEIMK.ReadingID2;
		}
		case LKOONNNEIMK.ReadingID2:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			if (num != 139)
			{
				throw new InvalidDataException(SR.GetString("Corrupted gzip header"));
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingCM;
			goto case LKOONNNEIMK.ReadingCM;
		}
		case LKOONNNEIMK.ReadingCM:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			if (num != 8)
			{
				throw new InvalidDataException(SR.GetString("Unknown compression mode"));
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingFLG;
			goto case LKOONNNEIMK.ReadingFLG;
		}
		case LKOONNNEIMK.ReadingFLG:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			BHBDNLEACPM = num;
			LKFCDEHOOCP = LKOONNNEIMK.ReadingMMTime;
			DOLKGPHFKPJ = 0;
			goto case LKOONNNEIMK.ReadingMMTime;
		}
		case LKOONNNEIMK.ReadingMMTime:
		{
			int num = 0;
			while (DOLKGPHFKPJ < 4)
			{
				num = NILNDHEKNLJ.GetBits(8);
				if (num < 0)
				{
					return false;
				}
				DOLKGPHFKPJ++;
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingXFL;
			DOLKGPHFKPJ = 0;
			goto case LKOONNNEIMK.ReadingXFL;
		}
		case LKOONNNEIMK.ReadingXFL:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingOS;
			goto case LKOONNNEIMK.ReadingOS;
		}
		case LKOONNNEIMK.ReadingOS:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingXLen1;
			goto case LKOONNNEIMK.ReadingXLen1;
		}
		case LKOONNNEIMK.ReadingXLen1:
		{
			if ((BHBDNLEACPM & 4) == 0)
			{
				goto case LKOONNNEIMK.ReadingFileName;
			}
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			OLNPJMKFACP = num;
			LKFCDEHOOCP = LKOONNNEIMK.ReadingXLen2;
			goto case LKOONNNEIMK.ReadingXLen2;
		}
		case LKOONNNEIMK.ReadingXLen2:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			OLNPJMKFACP |= num << 8;
			LKFCDEHOOCP = LKOONNNEIMK.ReadingXLenData;
			DOLKGPHFKPJ = 0;
			goto case LKOONNNEIMK.ReadingXLenData;
		}
		case LKOONNNEIMK.ReadingXLenData:
		{
			int num = 0;
			while (DOLKGPHFKPJ < OLNPJMKFACP)
			{
				num = NILNDHEKNLJ.GetBits(8);
				if (num < 0)
				{
					return false;
				}
				DOLKGPHFKPJ++;
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingFileName;
			DOLKGPHFKPJ = 0;
			goto case LKOONNNEIMK.ReadingFileName;
		}
		case LKOONNNEIMK.ReadingFileName:
			if ((BHBDNLEACPM & 8) == 0)
			{
				LKFCDEHOOCP = LKOONNNEIMK.ReadingComment;
			}
			else
			{
				int num;
				do
				{
					num = NILNDHEKNLJ.GetBits(8);
					if (num < 0)
					{
						return false;
					}
				}
				while (num != 0);
				LKFCDEHOOCP = LKOONNNEIMK.ReadingComment;
			}
			goto case LKOONNNEIMK.ReadingComment;
		case LKOONNNEIMK.ReadingComment:
			if ((BHBDNLEACPM & 0x10) == 0)
			{
				LKFCDEHOOCP = LKOONNNEIMK.ReadingCRC16Part1;
			}
			else
			{
				int num;
				do
				{
					num = NILNDHEKNLJ.GetBits(8);
					if (num < 0)
					{
						return false;
					}
				}
				while (num != 0);
				LKFCDEHOOCP = LKOONNNEIMK.ReadingCRC16Part1;
			}
			goto case LKOONNNEIMK.ReadingCRC16Part1;
		case LKOONNNEIMK.ReadingCRC16Part1:
		{
			if ((BHBDNLEACPM & 2) == 0)
			{
				LKFCDEHOOCP = LKOONNNEIMK.Done;
				goto case LKOONNNEIMK.Done;
			}
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			LKFCDEHOOCP = LKOONNNEIMK.ReadingCRC16Part2;
			goto case LKOONNNEIMK.ReadingCRC16Part2;
		}
		case LKOONNNEIMK.ReadingCRC16Part2:
		{
			int num = NILNDHEKNLJ.GetBits(8);
			if (num < 0)
			{
				return false;
			}
			LKFCDEHOOCP = LKOONNNEIMK.Done;
			goto case LKOONNNEIMK.Done;
		}
		case LKOONNNEIMK.Done:
			return true;
		default:
			throw new InvalidDataException(SR.GetString("Unknown state"));
		}
	}

	public bool BEPMEBNFAEL(InputBuffer NILNDHEKNLJ)
	{
		NILNDHEKNLJ.KHMFPEJHFHC();
		if (JNBHMAJIKLC == LKOONNNEIMK.ReadingCRC)
		{
			while (DOLKGPHFKPJ < 4)
			{
				int num = NILNDHEKNLJ.GetBits(8);
				if (num < 0)
				{
					return false;
				}
				GHMJIHKFBFJ |= (uint)(num << 8 * DOLKGPHFKPJ);
				DOLKGPHFKPJ++;
			}
			JNBHMAJIKLC = LKOONNNEIMK.ReadingFileSize;
			DOLKGPHFKPJ = 0;
		}
		if (JNBHMAJIKLC == LKOONNNEIMK.ReadingFileSize)
		{
			if (DOLKGPHFKPJ == 0)
			{
				FPPEOFLDECJ = 0u;
			}
			while (DOLKGPHFKPJ < 4)
			{
				int num2 = NILNDHEKNLJ.GetBits(8);
				if (num2 < 0)
				{
					return false;
				}
				FPPEOFLDECJ |= (uint)(num2 << 8 * DOLKGPHFKPJ);
				DOLKGPHFKPJ++;
			}
		}
		return true;
	}

	public void UpdateWithBytesRead(byte[] buffer, int IPCOBJBKNAO, int KKBGGFLOLMB)
	{
		NGBENFAENID = Crc32Helper.JDBNFCAIBHC(NGBENFAENID, buffer, IPCOBJBKNAO, KKBGGFLOLMB);
		long num = actualStreamSizeModulo + (uint)KKBGGFLOLMB;
		if (num >= 4294967296L)
		{
			num %= 4294967296L;
		}
		actualStreamSizeModulo = num;
	}

	public void FGCBJJKKILH()
	{
		if (GHMJIHKFBFJ != NGBENFAENID)
		{
			throw new InvalidDataException(SR.GetString("Invalid CRC"));
		}
		if (actualStreamSizeModulo != FPPEOFLDECJ)
		{
			throw new InvalidDataException(SR.GetString("Invalid stream size"));
		}
	}
}
