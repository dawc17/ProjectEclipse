using System;

internal class CopyEncoder
{
	private const int PaddingSize = 5;

	private const int MaxUncompressedBlockSize = 65536;

	public void GetBlock(DeflateInput NILNDHEKNLJ, OutputBuffer output, bool JDHJLBBIKLM)
	{
		int num = 0;
		if (NILNDHEKNLJ != null)
		{
			num = Math.Min(NILNDHEKNLJ.OFOPFCJNEBL(), output.JBPBBAEEAFO() - 5 - output.DBBLKJPGAOO());
			if (num > 65531)
			{
				num = 65531;
			}
		}
		if (JDHJLBBIKLM)
		{
			output.EHFDJAJPOAO(3, 1u);
		}
		else
		{
			output.EHFDJAJPOAO(3, 0u);
		}
		output.NOOJGJGNLBL();
		HPGIOFEMBJM((ushort)num, output);
		if (NILNDHEKNLJ != null && num > 0)
		{
			output.FJPANBOJJDI(NILNDHEKNLJ.FAJIIIFCCPD(), NILNDHEKNLJ.JHGJIJNGNBO(), num);
			NILNDHEKNLJ.MBODOPCOFFE(num);
		}
	}

	private void HPGIOFEMBJM(ushort JCAJDBOMGOM, OutputBuffer output)
	{
		output.WriteUInt16(JCAJDBOMGOM);
		ushort bAINMLLIKOL = (ushort)(~JCAJDBOMGOM);
		output.WriteUInt16(bAINMLLIKOL);
	}
}
