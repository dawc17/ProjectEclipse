using UnityEngine.Networking;

public class NekkiWebHandler : DownloadHandlerScript
{
	private const int preallocatedSize = 8192;

	private readonly NekkiUri JDNMLANMPPK;

	private bool OOGCKMCGAAH;

	private int HMJGFHBFNEI;

	private int LPMBLALPBGM;

	private bool GKFGGPHFEJG;

	public string Url
	{
		get
		{
			return KLMLKCKNNFD();
		}
	}

	public int NOPMKIAONBO
	{
		get
		{
			return GFLHMBOBICA();
		}
	}

	public int JJCKADKCDIF
	{
		get
		{
			return ECJPLFFAMJO();
		}
	}

	public bool ECBDGAJHOPF
	{
		get
		{
			return KPBEHFEBJJN();
		}
	}

	public NekkiWebHandler(NekkiUri IACLKBNEBDM)
		: base(new byte[8192])
	{
		JDNMLANMPPK = IACLKBNEBDM;
		OOGCKMCGAAH = false;
		HMJGFHBFNEI = 0;
		LPMBLALPBGM = 0;
		GKFGGPHFEJG = false;
	}

	public string KLMLKCKNNFD()
	{
		return JDNMLANMPPK.OriginalString;
	}

	public int GFLHMBOBICA()
	{
		return LPMBLALPBGM;
	}

	public int ECJPLFFAMJO()
	{
		return HMJGFHBFNEI;
	}

	public bool KPBEHFEBJJN()
	{
		return OOGCKMCGAAH;
	}

	public virtual void AKLEEMEHBIC()
	{
		GKFGGPHFEJG = true;
	}

	public virtual void GEJLNPIEDPF()
	{
		CompleteContent();
	}

	protected override void ReceiveContentLength(int HDIIBKGCCNB)
	{
		LPMBLALPBGM = HDIIBKGCCNB;
		NMJKPDGCEOK(LPMBLALPBGM);
	}

	protected override bool ReceiveData(byte[] data, int HIGBAHGOFIJ)
	{
		if (GKFGGPHFEJG || data == null || data.Length < 1)
		{
			return false;
		}
		LKECEJOMPGF(data, HMJGFHBFNEI, HIGBAHGOFIJ);
		HMJGFHBFNEI += HIGBAHGOFIJ;
		return true;
	}

	protected override void CompleteContent()
	{
		LPMBLALPBGM = HMJGFHBFNEI;
		OOGCKMCGAAH = true;
		HCNLJNFCBPA();
	}

	protected override float GetProgress()
	{
		return (HMJGFHBFNEI <= 0) ? 0f : ((float)GFLHMBOBICA() / (float)HMJGFHBFNEI);
	}

	protected virtual void NMJKPDGCEOK(int HDIIBKGCCNB)
	{
	}

	protected virtual void LKECEJOMPGF(byte[] data, int IAFIGGBIKOD, int HIGBAHGOFIJ)
	{
	}

	protected virtual void HCNLJNFCBPA()
	{
	}
}
