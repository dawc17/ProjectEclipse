using System;
using Nekki.SF2.Core.Security;
using SimpleJSON;
using UnityEngine;

public static class RemoteLicenseChecker
{
	private const string BBFNNDFMHPA = "iOS";

	private const string IHJHAOAFDCO = "Android";

	private const int HDMBLBMAPAM = 0;

	private const int IEBLNGMMCHO = 1;

	private const int NHOLPNBDIPI = 2;

	private static ILicenseVerificationSender AMJAHBBALKI;

	private static Action<bool> DIHOBBNFJIN;

	private static Action KGCPIBJKJKA;

	private static Action KGMJONGJLMD;

	private static int GBNDFPILBDN;

	private static object DBCCBNMFLOJ;

	public static void JOIGJOFNIKI(ILicenseVerificationSender FBMHPLELAPP, Action<bool> OFDKLMPDBEC, Action PEDLPMNMIBC, Action IMMJOLGIANN)
	{
		AMJAHBBALKI = FBMHPLELAPP;
		DIHOBBNFJIN = OFDKLMPDBEC;
		KGCPIBJKJKA = PEDLPMNMIBC;
		KGMJONGJLMD = IMMJOLGIANN;
		GBNDFPILBDN = 0;
		ILMGIBMDICH();
	}

	private static void ILMGIBMDICH()
	{
		FBJONAABIKE();
	}

	private static void FBJONAABIKE()
	{
		NBBJCMPFHKM(true);
	}

	private static void PPLICHKBOKC()
	{
		var appleExt = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();
		string text = (appleExt != null) ? appleExt.ANHBIPONDNE() : null;
		if (!string.IsNullOrEmpty(text))
		{
			AMJAHBBALKI.VerifyLicenseAction(text, "iOS", ALCGLODGGJP);
		}
		else
		{
			LLMMLMLKEEO();
		}
	}

	private static void BCBGPBDNCPH()
	{
		GooglePlayLicenseServerResponse googlePlayLicenseServerResponse = SecurityManager.GetGooglePlayLicenseServerResponse();
		if (googlePlayLicenseServerResponse != null && SecurityManager.get_Error() != GooglePlayLicenseCheckError.ErrorContactingServer)
		{
			AMJAHBBALKI.VerifyLicenseAction(googlePlayLicenseServerResponse, "Android", ALCGLODGGJP);
		}
		else
		{
			EJOMFKCLLIN();
		}
	}

	private static void GPMLJAGNOBL()
	{
		NBBJCMPFHKM(false);
	}

	private static void LLMMLMLKEEO()
	{
		Debug.Log("[RemoteLicenseChecker] RefreshAppReceipt");
		if (!InternetUtils.FCJPEABOFAA())
		{
			HGCEIPPOJCL();
			return;
		}
		ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG.KGPLNLCHNAO = (Action<string>)Delegate.Combine(aDEKACKLIJG.KGPLNLCHNAO, new Action<string>(ICOHKENIJII));
		ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG2.GIJNGGEFCHF = (Action)Delegate.Combine(aDEKACKLIJG2.GIJNGGEFCHF, new Action(NFHDCGEAKIM));
		var appleExt2 = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();
		if (appleExt2 != null)
		{
			appleExt2.KJGFLKHCEJM();
		}
	}

	private static void ICOHKENIJII(string DNHKNDPBGNM)
	{
		ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG.KGPLNLCHNAO = (Action<string>)Delegate.Remove(aDEKACKLIJG.KGPLNLCHNAO, new Action<string>(ICOHKENIJII));
		ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG2.GIJNGGEFCHF = (Action)Delegate.Remove(aDEKACKLIJG2.GIJNGGEFCHF, new Action(NFHDCGEAKIM));
		AMJAHBBALKI.VerifyLicenseAction(DNHKNDPBGNM, "iOS", ALCGLODGGJP);
	}

	private static void NFHDCGEAKIM()
	{
		ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG.KGPLNLCHNAO = (Action<string>)Delegate.Remove(aDEKACKLIJG.KGPLNLCHNAO, new Action<string>(ICOHKENIJII));
		ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
		aDEKACKLIJG2.GIJNGGEFCHF = (Action)Delegate.Remove(aDEKACKLIJG2.GIJNGGEFCHF, new Action(NFHDCGEAKIM));
		AMKEKPHBBCL();
	}

	private static void EJOMFKCLLIN()
	{
		Debug.Log("[RemoteLicenseChecker] RefreshLicenseServerResponse");
		SecurityManager.AFDFFNCKPHM aFDFFNCKPHM = new SecurityManager.AFDFFNCKPHM();
		aFDFFNCKPHM.HLNECMJHMCP().OGKBLPIJPBK = Constants.DPMDLBCBJJD();
		SecurityManager.get_Current().CheckLicense(KHLDJJLIOLI, aFDFFNCKPHM);
	}

	private static void KHLDJJLIOLI(LicenseRequestResult AMKKLMOONEP)
	{
		if (AMKKLMOONEP == LicenseRequestResult.Error && (SecurityManager.get_Error() == GooglePlayLicenseCheckError.ErrorContactingServer || SecurityManager.get_Error() == GooglePlayLicenseCheckError.CheckInProgress))
		{
			HGCEIPPOJCL();
		}
		else
		{
			AMJAHBBALKI.VerifyLicenseAction(SecurityManager.GetGooglePlayLicenseServerResponse(), "Android", ALCGLODGGJP);
		}
	}

	private static void ALCGLODGGJP(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH)
	{
		Debug.Log("[RemoteLicenseChecker] VerifyLicense_Response");
		Debug.Log((!AMKKLMOONEP) ? "Result fail" : "Result ok!");
		Debug.Log(GHDPPHAAPCA);
		if (!AMKKLMOONEP)
		{
			if (GBNDFPILBDN <= 2)
			{
				GBNDFPILBDN++;
				ILMGIBMDICH();
			}
			else
			{
				HGCEIPPOJCL();
			}
		}
		else
		{
			JSONNode jSONNode = JSONNode.Parse(GHDPPHAAPCA);
			int asInt = jSONNode["status"].AsInt;
			NBBJCMPFHKM(asInt == 0);
		}
	}

	private static void NBBJCMPFHKM(bool AMKKLMOONEP)
	{
		if (AMKKLMOONEP)
		{
			Debug.Log("[RemoteLicenseChecker] CheckLicense - SUCCESS");
		}
		else
		{
			Debug.LogFormat("[RemoteLicenseChecker] CheckLicense - FAILED)");
		}
		if (DIHOBBNFJIN != null)
		{
			DIHOBBNFJIN(AMKKLMOONEP);
		}
		DIHOBBNFJIN = null;
		KGCPIBJKJKA = null;
		KGMJONGJLMD = null;
	}

	private static void HGCEIPPOJCL()
	{
		Debug.Log("[RemoteLicenseChecker] CheckLicense - ERROR CONNECTION");
		if (KGCPIBJKJKA != null)
		{
			KGCPIBJKJKA();
		}
		DIHOBBNFJIN = null;
		KGCPIBJKJKA = null;
		KGMJONGJLMD = null;
	}

	private static void AMKEKPHBBCL()
	{
		Debug.Log("[RemoteLicenseChecker] CheckLicense - ERROR REFRESH RECEIPT");
		if (KGMJONGJLMD != null)
		{
			KGMJONGJLMD();
		}
		DIHOBBNFJIN = null;
		KGCPIBJKJKA = null;
		KGMJONGJLMD = null;
	}
}
