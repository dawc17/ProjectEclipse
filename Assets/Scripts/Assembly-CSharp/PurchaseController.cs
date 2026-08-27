using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core.Network;
using UnityEngine.Purchasing;

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
		if (!NDHHFHHBFEC)
		{
			NDHHFHHBFEC = true;
			ICFMIHIKGOD.Init(Roster.PKACFPCOHJH(), ServerProvider.get_Instance(), ListSF.DJBOFEEKJMP().FOEGEPKLGJN(), new Dictionary<string, object> { 
			{
				"AndroidPublicKey",
				Constants.DPMDLBCBJJD()
			} });
			ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG.PGICGKMBACN = (Action<string>)Delegate.Combine(aDEKACKLIJG.PGICGKMBACN, new Action<string>(OINELJEKFJJ));
			ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG2.JILHGHCHDKN = (Action<string>)Delegate.Combine(aDEKACKLIJG2.JILHGHCHDKN, new Action<string>(ABAEDAIOHDI));
			ADEKACKLIJG aDEKACKLIJG3 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG3.JEAJAJMDPNL = (Action<string>)Delegate.Combine(aDEKACKLIJG3.JEAJAJMDPNL, new Action<string>(FBGKOONDOHL));
			ADEKACKLIJG aDEKACKLIJG4 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG4.ENCIAJBEOEA = (Action<string, PurchaseFailureReason>)Delegate.Combine(aDEKACKLIJG4.ENCIAJBEOEA, new Action<string, PurchaseFailureReason>(GJEBBAGEDKK));
			ADEKACKLIJG aDEKACKLIJG5 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG5.JOFLHEEPJIB = (Action<string, string>)Delegate.Combine(aDEKACKLIJG5.JOFLHEEPJIB, new Action<string, string>(JOFLHEEPJIB));
			ADEKACKLIJG aDEKACKLIJG6 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG6.AFBIACPALAJ = (Action<string, string>)Delegate.Combine(aDEKACKLIJG6.AFBIACPALAJ, new Action<string, string>(KJCGIEJEIII));
			ADEKACKLIJG aDEKACKLIJG7 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG7.CFBBLIBNILI = (Action<string, string>)Delegate.Combine(aDEKACKLIJG7.CFBBLIBNILI, new Action<string, string>(EBPMIIEIBEE));
			ADEKACKLIJG aDEKACKLIJG8 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG8.GDFEPPLAKBP = (Action<string, string>)Delegate.Combine(aDEKACKLIJG8.GDFEPPLAKBP, new Action<string, string>(CIJDKLKMLKG));
			ADEKACKLIJG aDEKACKLIJG9 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG9.PDAKLFLLIKK = (Action<string, string>)Delegate.Combine(aDEKACKLIJG9.PDAKLFLLIKK, new Action<string, string>(FHODCLGKJIC));
			ADEKACKLIJG aDEKACKLIJG10 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG10.CIIDFBBIICE = (Action)Delegate.Combine(aDEKACKLIJG10.CIIDFBBIICE, new Action(CIIDFBBIICE));
		}
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
