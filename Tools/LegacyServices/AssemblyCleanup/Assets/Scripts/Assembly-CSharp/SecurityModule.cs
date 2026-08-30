using System.Collections;
using Nekki.SF2.Core;
using Nekki.SF2.Core.Security;
using UnityEngine;

public class SecurityModule : LoadingModule
{
	private bool _CheckStarted;

	private static string DJBOLGEPGDB()
	{
		return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			149, 40, 141, 2, 252, 248, 142, 24, 189, 22,
			119, 12, 18, 4, 63, 31, 127, 108, 240, 62,
			226, 167, 155, 15, 121, 184, 67, 48, 208, 41,
			80, 241, 145, 50, 53, 236, 129, 250, 198, 191,
			169, 127, 49, 124, 40, 229, 102, 134, 237, 61,
			127, 251, 155, 63, 53, 202, 136, 86, 180, 20,
			168, 251, 25, 90, 189, 91, 163, 89, 207, 254,
			234, 120, 188, 222, 14, 93, 220, 165, 164, 128,
			128, 83, 194, 111, 92, 40, 224, 14, 9, 32,
			101, 83, 184, 190, 150, 92, 252, 181, 156, 157,
			24, 229, 5, 59, 108, 89, 79, 149, 54, 125,
			226, 241, 21, 101, 59, 42, 231, 213, 237, 133,
			88, 149, 143, 145, 209, 3, 248, 232
		}, false);
	}

	private static string DHCNLCMKLAK()
	{
		return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			94, 6, 135, 45, 196, 60, 209, 5, 171, 11,
			30, 255, 187, 124, 249, 175, 19, 188, 76, 198,
			19, 14, 254, 194, 96, 39, 202, 225, 33, 199,
			21, 10, 103, 221, 158, 176, 13, 250, 82, 184,
			79, 139, 193, 112, 167, 114, 82, 160, 43, 224,
			195, 170, 161, 169, 0, 155, 183, 163, 152, 85,
			161, 90, 105, 60, 225, 1, 14, 88, 190, 222,
			138, 250, 33, 206, 16, 19, 94, 161, 148, 132,
			11, 142, 198, 120, 194, 1, 222, 105, 120, 85,
			195, 111, 36, 104, 91, 143, 251, 55, 242, 228,
			26, 5, 102, 101, 61, 183, 137, 177, 66, 136,
			156, 67, 156, 54, 11, 89, 9, 113, 43, 97,
			254, 86, 169, 132, 45, 76, 58, 56
		}, false);
	}

	private static string MHKIGCACHMM()
	{
		return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			6, 77, 174, 41, 39, 109, 68, 3, 128, 42,
			96, 122, 196, 240, 141, 234, 165, 178, 104, 190,
			130, 134, 75, 134, 40, 2, 151, 195, 224, 5,
			129, 187, 95, 121, 35, 201, 43, 78, 60, 49,
			99, 150, 232, 117, 66, 154, 177, 96, 183, 154,
			192, 26, 238, 108, 62, 134, 214, 169, 60, 195,
			170, 143, 14, 73, 247, 247, 168, 127, 102, 170,
			156, 5, 156, 102, 217, 174, 164, 214, 77, 190,
			90, 74, 61, 103, 157, 128, 74, 150, 159, 224,
			252, 28, 119, 16, 171, 112, 48, 1, 189, 66,
			190, 99, 15, 209, 25, 109, 9, 205, 231, 193,
			10, 220, 60, 220, 183, 51, 58, 247, 132, 76,
			13, 179, 245, 151, 162, 16, 251, 181
		}, false);
	}

	private static string GJJECAMAJDN()
	{
		return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			136, 135, 18, 52, 211, 101, 239, 166, 239, 130,
			119, 146, 56, 178, 104, 172, 124, 207, 30, 117,
			132, 233, 68, 180, 85, 1, 144, 106, 177, 180,
			74, 208, 248, 90, 16, 160, 63, 114, 103, 51,
			167, 114, 84, 250, 134, 107, 112, 105, 31, 6,
			183, 241, 140, 172, 123, 165, 23, 233, 126, 167,
			109, 78, 84, 107, 46, 52, 205, 2, 11, 84,
			149, 62, 253, 247, 207, 134, 196, 222, 228, 155,
			34, 238, 0, 117, 6, 99, 143, 65, 35, 73,
			181, 65, 78, 214, 146, 230, 170, 177, 175, 100,
			211, 85, 250, 15, 228, 6, 201, 111, 227, 92,
			162, 168, 210, 200, 236, 245, 214, 44, 213, 39,
			94, 175, 166, 76, 152, 110, 211, 49
		}, false);
	}

	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL && !_CheckStarted)
		{
			_CheckStarted = true;
			Debug.Log("SecurityModule.CheckStarted");
			SecurityManager.AFDFFNCKPHM aFDFFNCKPHM = new SecurityManager.AFDFFNCKPHM();
			aFDFFNCKPHM.LNEFNLLOGGH().GJOHIPDEMOB = true;
			aFDFFNCKPHM.LNEFNLLOGGH().JHNMCGGGGGO = true;
			aFDFFNCKPHM.LNEFNLLOGGH().AIPAICPCNBE = true;
			aFDFFNCKPHM.LNEFNLLOGGH().AJKBKHIGBJN = true;
			aFDFFNCKPHM.LNEFNLLOGGH().OCGDKIOJGLP = true;
			aFDFFNCKPHM.LNEFNLLOGGH().GBLMPEGLJLF = true;
			aFDFFNCKPHM.HLNECMJHMCP().FBFKEJEOELM = Constants.JFCAJKAOPLL();
			aFDFFNCKPHM.HLNECMJHMCP().ENNGMHNJDKH = true;
			aFDFFNCKPHM.HLNECMJHMCP().GIIGLIJLPPB = true;
			aFDFFNCKPHM.HLNECMJHMCP().IJPBIJAIHIO = true;
			aFDFFNCKPHM.HLNECMJHMCP().FKINJGAKICM = true;
			aFDFFNCKPHM.HLNECMJHMCP().ONJIELDFHMP = true;
			SecurityManager.get_Current().CheckLicense(KHLDJJLIOLI, aFDFFNCKPHM);
		}
	}

	private void KHLDJJLIOLI(LicenseRequestResult AMKKLMOONEP)
	{
		switch (AMKKLMOONEP)
		{
		case LicenseRequestResult.Success:
			CHIHBINEGFL = true;
			return;
		case LicenseRequestResult.Error:
			switch (SecurityManager.get_Error())
			{
			case GooglePlayLicenseCheckError.InvalidPackageName:
			case GooglePlayLicenseCheckError.NonMathcingUID:
			case GooglePlayLicenseCheckError.NonMarketManaged:
			case GooglePlayLicenseCheckError.InvalidPublicKey:
			case GooglePlayLicenseCheckError.MissingPermission:
				JFNNKPGOOOA(DHCNLCMKLAK());
				break;
			default:
				CHIHBINEGFL = true;
				break;
			}
			return;
		}
		switch (SecurityManager.get_FailReason())
		{
		case LicenseFailReason.iOS_JB:
			JFNNKPGOOOA(MHKIGCACHMM());
			break;
		case LicenseFailReason.Android_UnauthorizedApps:
			JFNNKPGOOOA(GJJECAMAJDN());
			break;
		default:
			JFNNKPGOOOA(DHCNLCMKLAK());
			break;
		}
	}

	private static void JFNNKPGOOOA(string LBDBAEJMDDC)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CoroutineManager.get_Current().StartRoutine(POPBLHFELGO());
		}
		DialogsOpener.OpenLocalAlertDialog(DJBOLGEPGDB(), LBDBAEJMDDC, "OK", () =>
		{
			ApplicationController.Quit();
		});
	}

	private static IEnumerator POPBLHFELGO()
	{
		yield return new WaitForSeconds(10f);
		ApplicationController.Quit();
	}
}
