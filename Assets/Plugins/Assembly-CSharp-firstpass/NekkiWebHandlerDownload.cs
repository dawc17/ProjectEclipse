using System.IO;
using UnityEngine;

public class NekkiWebHandlerDownload : NekkiWebHandler
{
	private readonly string NHNPNIOBFPG;

	private readonly string CDIGBKFPBKE;

	private readonly FileMode _fileMode;

	private readonly FileStream _file;

	public NekkiWebHandlerDownload(NekkiUri IACLKBNEBDM, string NDAOKPCHGJP, string IAOMDDJCIPC)
		: base(IACLKBNEBDM)
	{
		NHNPNIOBFPG = NDAOKPCHGJP;
		CDIGBKFPBKE = IAOMDDJCIPC;
		_file = HCEPBIAOJKG.BNIOOOBANEN(CDIGBKFPBKE);
	}

	protected override void LKECEJOMPGF(byte[] data, int IAFIGGBIKOD, int HIGBAHGOFIJ)
	{
		_file.Write(data, 0, HIGBAHGOFIJ);
	}

	public override void AKLEEMEHBIC()
	{
		HEFLNKLKHKC();
		base.AKLEEMEHBIC();
	}

	protected override void HCNLJNFCBPA()
	{
		HEFLNKLKHKC();
		if (HCEPBIAOJKG.GFBMBNAIJEJ(NHNPNIOBFPG) && HCEPBIAOJKG.BKACCHENJPK(NHNPNIOBFPG))
		{
			Debug.LogError(string.Format("This file is locked: {0} User may have this file open (as a folder or 7z), or there is a bug in the code still holding an open file handler", NHNPNIOBFPG));
			return;
		}
		HCEPBIAOJKG.BKLIKICKDPH(NHNPNIOBFPG);
		HCEPBIAOJKG.JALLPPJHCEA(CDIGBKFPBKE, NHNPNIOBFPG);
	}

	private void HEFLNKLKHKC()
	{
		_file.Close();
	}
}
