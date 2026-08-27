using System.Collections.Generic;

public class AttributesAlign
{
	public float Factor;

	public float Shift;

	public int Priority;

	public ModelParameters.IHFKGJLIPGH KONCHIPGFGO;

	public AttributesAlign()
	{
		Factor = 0f;
		Shift = 0f;
		Priority = 0;
		KONCHIPGFGO = ModelParameters.IHFKGJLIPGH.DFBoth;
	}

	public AttributesAlign(AttributesAlign NBMGOEMJJAF)
	{
		Factor = NBMGOEMJJAF.Factor;
		Shift = NBMGOEMJJAF.Shift;
		Priority = NBMGOEMJJAF.Priority;
		KONCHIPGFGO = NBMGOEMJJAF.KONCHIPGFGO;
	}

	public static int HDNMKKBMKLN(List<AttributesAlign> JPJIIDGEODE)
	{
		int num = int.MinValue;
		for (int i = 0; i < JPJIIDGEODE.Count; i++)
		{
			if (num < JPJIIDGEODE[i].Priority)
			{
				num = JPJIIDGEODE[i].Priority;
			}
		}
		return num;
	}

	public static int GGBAGGMLFHE(List<AttributesAlign> MHDPIEJEKIP, List<AttributesAlign> PNKJPOHEOJB)
	{
		int count = PNKJPOHEOJB.Count;
		int num = HDNMKKBMKLN(MHDPIEJEKIP);
		for (int i = 0; i < MHDPIEJEKIP.Count; i++)
		{
			if (MHDPIEJEKIP[i].Priority == num)
			{
				PNKJPOHEOJB.Add(MHDPIEJEKIP[i]);
			}
		}
		return PNKJPOHEOJB.Count - count;
	}
}
