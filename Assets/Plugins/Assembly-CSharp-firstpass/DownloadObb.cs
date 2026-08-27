using UnityEngine;

public class DownloadObb : MonoBehaviour
{
	private void Awake()
	{
		if (!FGIMNEGFGPM.FEDMOFPKEEL())
		{
			KJBHHFGDCJE("not android");
			return;
		}
		string text = FGIMNEGFGPM.AFKEFCHKEOP();
		if (string.IsNullOrEmpty(text))
		{
			KJBHHFGDCJE("no obb file");
			return;
		}
		string text2 = FGIMNEGFGPM.CKKGPFLGBEJ(text);
		string text3 = FGIMNEGFGPM.OOMFLCBNPID(text);
		if (text2 == null || text3 == null)
		{
			FGIMNEGFGPM.GPGEEIADAJI();
			KJBHHFGDCJE("all done");
		}
	}

	private void KJBHHFGDCJE(string NEPOLDCKNJL)
	{
		AdvLog.LOPHFKMOPAA(NEPOLDCKNJL);
		Application.LoadLevel(1);
	}
}
