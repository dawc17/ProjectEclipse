using System;
using System.IO;
using JetBrains.Annotations;

public class NekkiUri : Uri
{
	private readonly string _fileName;

	private readonly string JOALONLENKO;

	private readonly string FPNPKCMKMOI;

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
	}

	public string INECMLIOKNJ
	{
		get
		{
			return CABNCDDFCNN();
		}
	}

	public string HBGKLIGDCKI
	{
		get
		{
			return GNHBNGDDOGG();
		}
	}

	public NekkiUri([NotNull] string GDJGOEDDJIJ)
		: base(GDJGOEDDJIJ)
	{
		_fileName = Path.GetFileNameWithoutExtension(GDJGOEDDJIJ);
		JOALONLENKO = Path.GetFileName(base.LocalPath);
		FPNPKCMKMOI = Path.GetExtension(base.LocalPath);
	}

	public string EPDMGFELIMC()
	{
		return _fileName;
	}

	public string CABNCDDFCNN()
	{
		return JOALONLENKO;
	}

	public string GNHBNGDDOGG()
	{
		return FPNPKCMKMOI;
	}

	public override string ToString()
	{
		return "NekkiUri [" + base.OriginalString + "]";
	}
}
