using System;

public class NekkiWebHelper
{
	public static NekkiWebRequest EMANDFAOCNO(string BEPKJNKCKPH, string path, Action<NekkiWebRequest> LMKFJLKEILL, Action<NekkiWebRequest> onError, Action<NekkiWebRequest> LFAIENNBBMK = null, object data = null, float DGDKHFPEHOG = 5f, bool GHIGJJCMEDI = true)
	{
		NekkiWebDownload iOGFNGLOCHL = new NekkiWebDownload(path, DGDKHFPEHOG);
		iOGFNGLOCHL.FGJPDDAPFME(LMKFJLKEILL);
		iOGFNGLOCHL.BJDMHEHILEO(onError);
		iOGFNGLOCHL.JIMPDHAOECM(LFAIENNBBMK);
		iOGFNGLOCHL.SetExternalData(data);
		iOGFNGLOCHL.Send(BEPKJNKCKPH, GHIGJJCMEDI);
		return iOGFNGLOCHL;
	}

	public static NekkiWebRequest EMPGOCGHMBI(string BEPKJNKCKPH, Action<NekkiWebRequest> LMKFJLKEILL, Action<NekkiWebRequest> onError, Action<NekkiWebRequest> LFAIENNBBMK = null, object data = null, float DGDKHFPEHOG = 5f, bool GHIGJJCMEDI = true)
	{
		NekkiWebRequest aHEFDBHFHOM = new NekkiWebRequest(DGDKHFPEHOG);
		aHEFDBHFHOM.FGJPDDAPFME(LMKFJLKEILL);
		aHEFDBHFHOM.BJDMHEHILEO(onError);
		aHEFDBHFHOM.JIMPDHAOECM(LFAIENNBBMK);
		aHEFDBHFHOM.SetExternalData(data);
		aHEFDBHFHOM.Send(BEPKJNKCKPH, GHIGJJCMEDI);
		return aHEFDBHFHOM;
	}
}
