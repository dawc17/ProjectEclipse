using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core.Network;
using SF2.Offline;

public static class PurchaseController
{
	private static bool NDHHFHHBFEC;

	public static bool MMJAKDAEGDM
	{
		get
		{
			return KGEOCPBDJIF();
		}
	}

	public static bool KGEOCPBDJIF()
	{
		return NDHHFHHBFEC;
	}

	public static void Init()
	{
		NDHHFHHBFEC = true; // Inert store facade already exists; no keys or backend initialization.
	}

	private static void OINELJEKFJJ(string FDKNIPNGFNF)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Request_Start, lKIOKGCNKHE);
	}

	private static void ABAEDAIOHDI(string FDKNIPNGFNF)
	{
	}

	private static void FBGKOONDOHL(string FDKNIPNGFNF)
	{
		ItemInfo dJKEECEOCJB = ListSF.CKCMJAJAELO(FDKNIPNGFNF);
		ListSF.CDCHFKPDDFH(dJKEECEOCJB);
		float result = 0f;
		float.TryParse(dJKEECEOCJB.EGAJMELKANL, out result);
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("item", dJKEECEOCJB.Name);
		kEMMIFBFDPK.Add("price", result);
		kEMMIFBFDPK.Add("price_currency", dJKEECEOCJB.MIIJIMJDHFP);
		kEMMIFBFDPK.Add("money_changed", (ObscuredLong)(dJKEECEOCJB.HHIFKGOJFAC));
		kEMMIFBFDPK.Add("gems_paid_changed", (ObscuredLong)(dJKEECEOCJB.BBMLCBEFLGI));
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Payment, lKIOKGCNKHE);
		float HCHKFOJEEBK = 0f;
		if (GeneralConfig.IHHMHNHOLCB.LIKBNIAJHKA(dJKEECEOCJB.Name, out HCHKFOJEEBK))
		{
			FBController.LMBHFAHHDKI(HCHKFOJEEBK);
		}
	}

	private static void GJEBBAGEDKK(string FDKNIPNGFNF, PurchaseFailureReason ILDDNIBBANF)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Request_Fail, lKIOKGCNKHE);
	}

	private static void KJCGIEJEIII(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		kEMMIFBFDPK.Add("receipt", DNHKNDPBGNM);
		kEMMIFBFDPK.Add("status", StatisticsEvent.AICJEAMBGCE.Start);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Verification_Status_Change, lKIOKGCNKHE);
	}

	private static void EBPMIIEIBEE(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		kEMMIFBFDPK.Add("receipt", DNHKNDPBGNM);
		kEMMIFBFDPK.Add("status", StatisticsEvent.AICJEAMBGCE.Confirm_Start);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Verification_Status_Change, lKIOKGCNKHE);
	}

	private static void CIJDKLKMLKG(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		kEMMIFBFDPK.Add("receipt", DNHKNDPBGNM);
		kEMMIFBFDPK.Add("status", StatisticsEvent.AICJEAMBGCE.Finish);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Verification_Status_Change, lKIOKGCNKHE);
	}

	private static void FHODCLGKJIC(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		kEMMIFBFDPK.Add("receipt", DNHKNDPBGNM);
		kEMMIFBFDPK.Add("status", StatisticsEvent.AICJEAMBGCE.Confirm_Finish);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Verification_Status_Change, lKIOKGCNKHE);
	}

	private static void JOFLHEEPJIB(string FDKNIPNGFNF, string DNHKNDPBGNM)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK.Add("packageName", FDKNIPNGFNF);
		kEMMIFBFDPK.Add("receipt", DNHKNDPBGNM);
		ArgsDict lKIOKGCNKHE = kEMMIFBFDPK;
		StatisticsCollector.KBILEMGFDDC(StatisticsEvent.JDNFFHILFAF.Pay_Transaction_Finish, lKIOKGCNKHE);
	}

	private static void CIIDFBBIICE()
	{
		if (!ICFMIHIKGOD.LHGPKEFEHDH())
		{
			ListSF.DJBOFEEKJMP().HAHLCEBCPLJ(ICFMIHIKGOD.OFFDIMCJOIC().NABJBCEKEHK());
			ListSF.CCDKHLAMKKO().PNHPFNGCFGO();
		}
	}
}
