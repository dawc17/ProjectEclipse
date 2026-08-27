using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Nekki.SF2.Core.Network;
using Nekki.Utils;
using SimpleJSON;
using UnityEngine;

public class NetworkController
{
	public Action<object> OnLoginComplete = delegate
	{
	};

	private static NetworkController _Instance;

	private bool GLPIGAOBNOP;

	private bool CHAPOJPCOJI = true;

	private int _NewsCounter;

	public readonly GiveLogin LBDHOLEICEG = new GiveLogin();

	public readonly LedgerManager KDILDKDNIID = new LedgerManager();

	public readonly DumpController DumpController = new DumpController();

	public static NetworkController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	private NetworkController()
	{
		CDPMAEPEMEF();
	}

	public static NetworkController ELEBLBJKDBI()
	{
		if (_Instance == null)
		{
			_Instance = new NetworkController();
		}
		return _Instance;
	}

	public void IFFDOFMDABC()
	{
		GLPIGAOBNOP = true;
		DOJCMIFHJKM();
	}

	private void DOJCMIFHJKM()
	{
		string iFKJHHPJPLP = InternetController.PPPALDPCFPL();
		int hCCLKJOCHGP = AssemblyController.NJBBJGCJBAE();
		int iNGCPFFHBOG = AssemblyController.FODKMFKJDAJ();
		GeneralConfig.ELEBLBJKDBI().DOJCMIFHJKM(iFKJHHPJPLP, OnLoadConfig, null, null, hCCLKJOCHGP, iNGCPFFHBOG);
	}

	private void OnLoadConfig(bool DCJLKCFKCOM)
	{
		LLLOJBFMONN.INNGABABJPC("Login sequence: NetworkController.OnLoadConfig");
		if (GeneralConfig.ELEBLBJKDBI().IHGDCIFNAOA())
		{
			GameUtils.ECBGGDNBKJC(GeneralConfig.ELEBLBJKDBI().CHAPIILIEPK() && BFEPBPFAEML());
			IMDCOKBHAMB();
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Session_End);
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.User);
			if (!GameCenterController.GEJNIMAILDA())
			{
				OIPNOLBHNKF();
			}
			else
			{
				ServerProvider.get_Instance().StartCoroutine(BILKIHELPBO());
			}
		}
	}

	private IEnumerator BILKIHELPBO()
	{
		yield return new WaitForSeconds(0.1f);
		OIPNOLBHNKF();
	}

	private void OIPNOLBHNKF()
	{
		if (CHAPOJPCOJI)
		{
			ListSF.ELEBLBJKDBI().MAOPKFNKHOI();
			CHAPOJPCOJI = false;
		}
		LLLOJBFMONN.INNGABABJPC("Login sequence: NetworkController.LoadLogin()");
		GLPIGAOBNOP = true;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (AssemblyController.AIOBIKGPGHA() && SystemProperties.DBBOCENKMGD())
		{
			dictionary["abGroups"] = ListSF.CCDKHLAMKKO().KNGJJEOLFHF();
		}
		ServerProvider.get_Instance().Login(GameCenterController.CONEABALMEJ(), IGCLHHGPNCJ, dictionary);
	}

	private void IGCLHHGPNCJ(ServerProvider.LoginData IDLIDOGIJHO)
	{
		LLLOJBFMONN.INNGABABJPC("Login sequence: NetworkController.LoginResponse");
		if (!GLPIGAOBNOP)
		{
			return;
		}
		bool flag = false;
		if (IDLIDOGIJHO != null)
		{
			if (IDLIDOGIJHO.EOKFDJIIKEA)
			{
				ListSF.CCDKHLAMKKO().HCMLOIDALKC(IDLIDOGIJHO.UserID);
				LBDHOLEICEG.Parse(IDLIDOGIJHO.Json);
				CGMBMKJDKND(IDLIDOGIJHO.Json);
				OELLEBJEBLI(IDLIDOGIJHO.Json);
			}
		}
		else
		{
			LLLOJBFMONN.Error("loginData is null");
		}
		KIBJJDMNPOA();
	}

	private void KIBJJDMNPOA()
	{
		ListSF.CCDKHLAMKKO().BIHELGAGPGO();
		KPBNAACNBLP();
	}

	private void CDPMAEPEMEF()
	{
		List<string> list = new List<string>();
		list.Add(string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ));
		DumpController.Init(list);
		DumpController.ACEIJEGLJCD(BFCOGFMINJA);
	}

	private void BFCOGFMINJA()
	{
		NGFHFEAGIFI();
	}

	private void KPBNAACNBLP()
	{
		DumpController.PAHKDLLDCDP();
	}

	private void NGFHFEAGIFI()
	{
		if (Directory.Exists(SF2Paths.GJFFDOJLHGK()))
		{
			Directory.Delete(SF2Paths.GJFFDOJLHGK(), true);
		}
		Directory.CreateDirectory(SF2Paths.GJFFDOJLHGK());
		_NewsCounter = 0;
		DLNNPADBCCM();
	}

	private void DLNNPADBCCM()
	{
		if (_NewsCounter < GeneralConfig.FNHPCBEDKFO.MEFNHIALOED().Count)
		{
			NewsItem pONDDFBMFOO = GeneralConfig.FNHPCBEDKFO.MEFNHIALOED()[_NewsCounter];
			ServerProvider.get_Instance().DownloadFile(pONDDFBMFOO.MDDOAGNHAHE, OPABBMCKEPF);
		}
		else
		{
			JOIGJOFNIKI();
		}
	}

	private void OPABBMCKEPF(byte[] data, string JDONBAPIJCG, string BEPKJNKCKPH)
	{
		if (string.IsNullOrEmpty(JDONBAPIJCG))
		{
			NewsItem pONDDFBMFOO = GeneralConfig.FNHPCBEDKFO.MEFNHIALOED()[_NewsCounter];
			string text = string.Format("{0}/{1}", SF2Paths.GJFFDOJLHGK(), pONDDFBMFOO.Name);
			File.WriteAllBytes(text, data);
			pONDDFBMFOO.NHKMCLPOMFK = text;
			pONDDFBMFOO.GAHGCJNGDMH = true;
		}
		_NewsCounter++;
		DLNNPADBCCM();
	}

	private void JOIGJOFNIKI()
	{
		if (SystemProperties.NDEPIDFFOBF())
		{
			if (RemoteLicenseCache.GGLBNKFOKKH())
			{
				GFMIBIHAEOH(RemoteLicenseCache.DJBEPKDFIAI());
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer
					&& string.IsNullOrEmpty((ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>() != null)
						? ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>().ANHBIPONDNE()
						: null) && !DialogsOpener.MOAEBPJBDCD())
			{
				DialogsOpener.FEAHBJGCNLC(() =>
				{
					RemoteLicenseChecker.JOIGJOFNIKI(ServerProvider.get_Instance(), GFMIBIHAEOH, OPNFMIBACNN, BNKDOCHPCGD);
				});
			}
			else
			{
				RemoteLicenseChecker.JOIGJOFNIKI(ServerProvider.get_Instance(), GFMIBIHAEOH, OPNFMIBACNN, BNKDOCHPCGD);
			}
		}
		else
		{
			AHPFEEAOFMD();
		}
	}

	private void BNKDOCHPCGD()
	{
		DialogsOpener.FEAHBJGCNLC(() =>
		{
			RemoteLicenseChecker.JOIGJOFNIKI(ServerProvider.get_Instance(), GFMIBIHAEOH, OPNFMIBACNN, BNKDOCHPCGD);
		});
	}

	private void OPNFMIBACNN()
	{
		DialogsOpener.OFIOGLOLIJP(JOIGJOFNIKI);
	}

	private void GFMIBIHAEOH(bool NOGBHHLJECH)
	{
		if (!RemoteLicenseCache.GGLBNKFOKKH() || (RemoteLicenseCache.GGLBNKFOKKH() && !RemoteLicenseCache.LHJJMGKDKKM()))
		{
			RemoteLicenseCache.FNNBCANLNKE(NOGBHHLJECH);
		}
		if (NOGBHHLJECH)
		{
			AHPFEEAOFMD();
		}
		else
		{
			DialogsOpener.FPCPKGBNEPD();
		}
	}

	private void AHPFEEAOFMD()
	{
		LLLOJBFMONN.INNGABABJPC("Login sequence: NetworkController.LoginComplete");
		LBDHOLEICEG.PGAJKMOPDIJ();
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		if (hHKLFIIBIFF.LBGOMJFFEPP() == null)
		{
			hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
			hHKLFIIBIFF.HEIADONEACH = string.Empty;
		}
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_END))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
		OnLoginComplete(null);
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SESSION))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}

	private void CGMBMKJDKND(JSONNode value)
	{
		JSONNode jSONNode = null;
		JSONNode jSONNode2 = value["data"];
		if (jSONNode2 != null && jSONNode2.Value.Equals("user"))
		{
			JSONNode mEEAKLDGLDF = value["value"];
			jSONNode = mEEAKLDGLDF.GetNode("spendertypeid");
		}
		if (!(jSONNode != null))
		{
		}
	}

	private void OELLEBJEBLI(JSONNode value)
	{
	}

	private void IMDCOKBHAMB()
	{
		string kGBGENDIMBC = "HR7mvh4vb/V_mY,khW4R/!}g=q]qm.We HNb#2$$][RJ`._ egg+U-A]?f7ew(u8";
		string kGBGENDIMBC2 = "E+wf5]#2>pI|O7yS>s64N.+36&3YM}~6p:=9ZU:@s$g$^P_xo2VH4fGJ%6vk.c1E";
		if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() || AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
		{
			ServerProvider.Init(kGBGENDIMBC2, AssemblyController.BAFKGAHBAAJ(), AssemblyController.LMMKKKPPPAD());
		}
		else
		{
			ServerProvider.Init(kGBGENDIMBC, AssemblyController.BAFKGAHBAAJ(), AssemblyController.LMMKKKPPPAD());
		}
		GlobalTimer.Init();
	}

	public bool BFEPBPFAEML()
	{
		return !AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() && !AssemblyController.JONCCPLEIBE().OPCBKOOFMAK();
	}
}
