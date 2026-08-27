using System;
using System.Collections.Generic;
using System.Linq;
using Nekki.SF2.Core.Network;
using SimpleJSON;
using UnityEngine;

public class LedgerManager
{
	private List<int> unconfirmedIds;

	public void Check()
	{
		string bEPKJNKCKPH = GeneralConfig.ELEBLBJKDBI().IMOKGIDCANG().KLMLKCKNNFD() + "clist";
		ServerProvider.get_Instance().CheckLedger(bEPKJNKCKPH, ACPAMDHCFFE);
	}

	private void ACPAMDHCFFE(bool DCJLKCFKCOM, string data, object IEHMCKBJCAK)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			Debug.LogError("Roster not created error");
		}
		else
		{
			if (!DCJLKCFKCOM)
			{
				return;
			}
			JSONNode jSONNode = JSON.Parse(data);
			unconfirmedIds = SystemProperties.LFICEOIFOMI().ToList();
			for (int i = 0; i < jSONNode.Count; i++)
			{
				JSONNode jSONNode2 = jSONNode[i];
				int asInt = jSONNode2["rid"].AsInt;
				if (!unconfirmedIds.Contains(asInt))
				{
					unconfirmedIds.Add(asInt);
					string lFLGCDNKNJI = jSONNode2["cur"].CIPOICEEIBK();
					string fDGOFODPGPH = jSONNode2["ini"].CIPOICEEIBK();
					GiveReward(lFLGCDNKNJI, jSONNode2["cnt"], fDGOFODPGPH);
				}
			}
			FMONIHMEGBF();
			SystemProperties.set_UnconfirmedLedgerIDs(unconfirmedIds.ToArray());
			LIIBANIDLLI();
		}
	}

	private void GiveReward(string LFLGCDNKNJI, JSONNode NICNMHCJIBJ, string FDGOFODPGPH)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (FDGOFODPGPH == "admin")
		{
			switch (LFLGCDNKNJI)
			{
			case "GEMS":
				EBAIKGHPPOG(NICNMHCJIBJ.ParseInt());
				DialogsOpener.PEDJMOMBJJI("dlgAlertTitle", string.Concat("dlgGotGift{img::MiscSprites.ruby}{", NICNMHCJIBJ, "}"), "dlgStoryBtnTake", string.Empty, null, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
				break;
			case "COINS":
				GPEHMMKAOAL(NICNMHCJIBJ.ParseInt());
				DialogsOpener.PEDJMOMBJJI("dlgAlertTitle", string.Concat("dlgGotGift{img::MiscSprites.gold}{", NICNMHCJIBJ, "}"), "dlgStoryBtnTake", string.Empty, null, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
				break;
			default:
				Debug.LogError("LedgerManagerSF::giveReward Unknown currency type");
				break;
			}
		}
		else if (!(LFLGCDNKNJI == "GEMS"))
		{
			if (LFLGCDNKNJI == "AscensionTicket")
			{
				nKGLHEGIKKP.AddCurrencyCount("AscensionTicket", NICNMHCJIBJ.ParseInt());
			}
			else
			{
				Debug.LogError("LedgerManager.GiveReward() Unknown currency type");
			}
		}
		MenuController.IAMGKKOINFC();
	}

	private static void GPEHMMKAOAL(int NICNMHCJIBJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.OIOOMAKNIOB(Math.Max(0L, nKGLHEGIKKP.BFBOEGMAMNF() + NICNMHCJIBJ));
	}

	private static void EBAIKGHPPOG(int NICNMHCJIBJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.LLNELLFMMBB(Math.Max(0L, nKGLHEGIKKP.EHFJHFDACMP() + NICNMHCJIBJ), Roster.HPOIJPGPOCF.CHANGE_LEDGER);
	}

	private void FMONIHMEGBF()
	{
	}

	private void LIIBANIDLLI()
	{
		if (unconfirmedIds.Count > 0)
		{
			string bEPKJNKCKPH = GeneralConfig.ELEBLBJKDBI().IMOKGIDCANG().KLMLKCKNNFD() + "caccept";
			string dIAIIPCBMFL = string.Join(",", unconfirmedIds.Select((int OKNNNLIPODI) => OKNNNLIPODI.ToString()).ToArray());
			ServerProvider.get_Instance().ConfirmLedger(bEPKJNKCKPH, KDLADLABMGE, dIAIIPCBMFL);
		}
	}

	private void KDLADLABMGE(bool DCJLKCFKCOM, string data, object IEHMCKBJCAK)
	{
		if (DCJLKCFKCOM)
		{
			JSONNode jSONNode = JSON.Parse(data);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				unconfirmedIds.Remove(jSONNode[i].AsInt);
			}
			SystemProperties.set_UnconfirmedLedgerIDs(unconfirmedIds.ToArray());
		}
	}
}
