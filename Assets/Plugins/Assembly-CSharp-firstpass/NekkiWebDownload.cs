using System.IO;
using UnityEngine.Networking;

public class NekkiWebDownload : NekkiWebRequest
{
	private readonly string NHNPNIOBFPG;

	private readonly string CDIGBKFPBKE;

	public NekkiWebDownload(string path, float DGDKHFPEHOG = 5f)
		: base(DGDKHFPEHOG)
	{
		NHNPNIOBFPG = path;
		CDIGBKFPBKE = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_download.nekki";
		HCEPBIAOJKG.BKLIKICKDPH(NHNPNIOBFPG);
		HCEPBIAOJKG.BKLIKICKDPH(CDIGBKFPBKE);
	}

	public void Send(UnityWebRequest DLILAFJFLAI)
	{
		if (HCEPBIAOJKG.GFBMBNAIJEJ(CDIGBKFPBKE))
		{
			FileInfo fileInfo = new FileInfo(CDIGBKFPBKE);
			DLILAFJFLAI.SetRequestHeader("range-start", fileInfo.Length.ToString());
		}
		Send(DLILAFJFLAI);
	}

	protected override void JBEBPLHMDPF()
	{
		if (NekkiUtils.JDIKHMODKKF())
		{
			HCEPBIAOJKG.OOMKKNBMFDG(NHNPNIOBFPG, KJBFBPBCAOH());
			HCEPBIAOJKG.BKLIKICKDPH(CDIGBKFPBKE);
		}
		base.JBEBPLHMDPF();
	}

	protected override void SendError(bool BALCNGAKGKN = false)
	{
		HCEPBIAOJKG.BKLIKICKDPH(NHNPNIOBFPG);
		HCEPBIAOJKG.BKLIKICKDPH(CDIGBKFPBKE);
		base.SendError();
	}

	protected override NekkiWebHandler GBMIBHMCDHC(NekkiUri KJHNCLAJMLO)
	{
		return new NekkiWebHandlerDownload(KJHNCLAJMLO, NHNPNIOBFPG, CDIGBKFPBKE);
	}
}
