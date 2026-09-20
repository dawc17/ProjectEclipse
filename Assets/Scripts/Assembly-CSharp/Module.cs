using System;
using Nekki.SF2.GUI;
using UnityEngine.SceneManagement;

public class Module : global::EventDispatcher<object>
{
	public enum FKHIMIAOCJL
	{
		OnPrepareOpenScene = 0,
		OnOpenScene = 1,
		OnEnterFrame = 2,
		OnCloseScene = 3,
		OnSceneCreated = 4,
		OnStartShake = 5,
		OnStopShake = 6
	}

	private static Module instance;

	public ScreenInfo DMCJGOMOJEF = new ScreenInfo();

	private ModuleHolder FFICJOEBPAK;

	// best guess for name
	private bool _inputLocked;

	// best guess for name
	private bool _visibleInputLock;

	private bool INGHJOIPMGH;

	private bool FGGGPEGPDIK;
    private int _presentationLocks;
    public IDisposable AcquirePresentationLock()
    {
        bool visible = _inputLocked && _visibleInputLock;
        _presentationLocks++;
        RefreshInputLock(visible);
        return new PresentationLock(this);
    }
    private sealed class PresentationLock : IDisposable
    {
        private Module owner;
        public PresentationLock(Module owner) { this.owner = owner; }
        public void Dispose()
        {
            var current = owner; owner = null;
            if (current == null) return;
            current._presentationLocks--;
            current.RefreshInputLock(current._visibleInputLock);
        }
    }

	private bool HNDJIOBDPLN;

	public static Module BPCBBHAKFDM
	{
		get
		{
			return GetInstance();
		}
	}

	public ModuleHolder BDIEFLEJCNE
	{
		get
		{
			return BOHBCFMJPCA();
		}
	}

	private Module()
	{
	}

	// best guess for name
	public static Module GetInstance()
	{
		if (instance == null)
		{
			instance = new Module();
		}
		return instance;
	}

	public ModuleHolder BOHBCFMJPCA()
	{
		return FFICJOEBPAK;
	}

	public static void Reset()
	{
		instance = null;
	}

	public static bool DLOKJOHNDID(string HBGBPDEGKFE, object data = null, Action<object> ODDEOFKLIAG = null, bool EOIDGPINLAH = true)
	{
		ScreenType hBGBPDEGKFE = DFDEMKONNKK(HBGBPDEGKFE);
		return DLOKJOHNDID(hBGBPDEGKFE, data, ODDEOFKLIAG, EOIDGPINLAH);
	}

	public static bool DLOKJOHNDID(ScreenType HBGBPDEGKFE, object data = null, Action<object> ODDEOFKLIAG = null, bool EOIDGPINLAH = true)
	{
		Module jLINNJGCFOG = GetInstance();
		QuestParameters hHKLFIIBIFF = ListSF.GetInstance().BNMLDPNCMLB();
		hHKLFIIBIFF.GAEPENBCCPB = hHKLFIIBIFF.BPPAPLLPBIJ;
		hHKLFIIBIFF.GMDFCHJBJGO = INIOOEKJIDI(HBGBPDEGKFE);
		SliderType oFEMKBGPNBH = GameUtils.NAMBCLFLNIN(hHKLFIIBIFF.OIKHBNOANPP);
		SliderType cFDMHKKBGIN = PDLBAGNMFIN(HBGBPDEGKFE, data);
		if (EOIDGPINLAH && GameUtils.OIGPBEKELCP(HBGBPDEGKFE))
		{
			return false;
		}
		if (EOIDGPINLAH && GameUtils.MKADBAEEMFA(oFEMKBGPNBH, cFDMHKKBGIN))
		{
			return false;
		}
		string bPPAPLLPBIJ = hHKLFIIBIFF.BPPAPLLPBIJ;
		string text = INIOOEKJIDI(ScreenType.ModuleShop);
		if (bPPAPLLPBIJ == text)
		{
			MenuController.BGFJOFOLGDH(false);
		}
		MenuController.BEMOBLOBCHN();
		jLINNJGCFOG.DMCJGOMOJEF.HKJFKDEEIDJ = jLINNJGCFOG.DMCJGOMOJEF.ScreenType;
		jLINNJGCFOG.DMCJGOMOJEF.ScreenType = HBGBPDEGKFE;
		jLINNJGCFOG.DMCJGOMOJEF.Data = data;
		jLINNJGCFOG.DMCJGOMOJEF.Dlg = ODDEOFKLIAG;
		jLINNJGCFOG.OAAFAINKKMI();
		jLINNJGCFOG.CallEvent(0, jLINNJGCFOG.DMCJGOMOJEF);
		return true;
	}

    internal void OpenLocalVersus(Eclipse.Multiplayer.LocalVersusMatch match)
    {
        if (match == null || !Eclipse.Multiplayer.LocalVersusSession.IsActive)
            throw new InvalidOperationException("Local versus must own the scene transition.");
        DMCJGOMOJEF.HKJFKDEEIDJ = DMCJGOMOJEF.ScreenType;
        DMCJGOMOJEF.ScreenType = ScreenType.ModuleFight;
        DMCJGOMOJEF.Data = match;
        DMCJGOMOJEF.Dlg = null;
        CallEvent(3, DMCJGOMOJEF.HKJFKDEEIDJ);
        DialogsManager.HNEGECPBALO();
        SceneManagerSF.Load(ScreenType.ModuleFight);
        CallEvent(4, ScreenType.ModuleFight);
        CallEvent(0, DMCJGOMOJEF);
    }

	public void OAAFAINKKMI()
	{
		CallEvent(3, DMCJGOMOJEF.HKJFKDEEIDJ);
		DialogsManager.HNEGECPBALO();
		SceneManagerSF.Load(DMCJGOMOJEF.ScreenType);
		CallEvent(4, DMCJGOMOJEF.ScreenType);
		QuestParameters hHKLFIIBIFF = ListSF.GetInstance().BNMLDPNCMLB();
		hHKLFIIBIFF.BPPAPLLPBIJ = INIOOEKJIDI(DMCJGOMOJEF.ScreenType);
		if (ListSF.GetInstance().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SCENE_LOADED))
		{
			ListSF.GetInstance().MHHNIPBJNAD();
		}
	}

	public ScreenType NMCNDOPKFJD()
	{
		return DMCJGOMOJEF.ScreenType;
	}

	public Scene HMGDPCPPEFC()
	{
		return SceneManagerSF.GAFDMIPPIAL();
	}

	public static ScreenType DFDEMKONNKK(string PHJPOKBOJOG)
	{
		switch (PHJPOKBOJOG)
		{
		case "Dojo":
			return ScreenType.ModuleDojo;
		case "Map":
			return ScreenType.ModuleMap;
		case "Shop":
			return ScreenType.ModuleShop;
		case "Profile":
			return ScreenType.ModuleProfile;
		case "Loader":
			return ScreenType.ModulePreloader;
		default:
			LLLOJBFMONN.Error("Module::getScreenTypeFromString - screen: %s", PHJPOKBOJOG);
			return ScreenType.ModuleFight;
		}
	}

	public static string INIOOEKJIDI(ScreenType HBGBPDEGKFE)
	{
		string result = string.Empty;
		switch (HBGBPDEGKFE)
		{
		case ScreenType.ModulePreloader:
			result = "Loader";
			break;
		case ScreenType.ModuleFight:
			result = "Fight";
			break;
		case ScreenType.ModuleShop:
			result = "Shop";
			break;
		case ScreenType.ModuleMap:
			result = "Map";
			break;
		case ScreenType.ModuleProfile:
			result = "Profile";
			break;
		case ScreenType.ModuleCreditsScreen:
			result = "Credits";
			break;
		case ScreenType.ModuleDojo:
			result = "Dojo";
			break;
		default:
			LLLOJBFMONN.Error("Module::getScreenNameFromType - screen: " + HBGBPDEGKFE);
			break;
		}
		return result;
	}

	public static SliderType PDLBAGNMFIN(ScreenType HBGBPDEGKFE, object data)
	{
		SliderType lBFKFBALMGA = SliderType.SliderNone;
		switch (HBGBPDEGKFE)
		{
		case ScreenType.ModuleShop:
		{
			DelayedStrike dDFFCNPELBC = ((data == null) ? null : ((DelayedStrike)data));
			if (dDFFCNPELBC != null)
			{
				return dDFFCNPELBC.SliderType;
			}
			return SliderType.SliderWeapon;
		}
		case ScreenType.ModuleProfile:
			return SliderType.SliderPerks;
		case ScreenType.ModuleMap:
			return SliderType.SliderStoryMap;
		default:
			return SliderType.SliderNone;
		}
	}

	public void OJFNMDGIDJN()
	{
		if (Eclipse.Multiplayer.LocalVersusSession.IsActive)
		{
			CallEvent(1, DMCJGOMOJEF.ScreenType);
			CallEvent(2, 0);
			return;
		}
		if (DMCJGOMOJEF.ScreenType != ScreenType.ModulePreloader)
		{
			if (DMCJGOMOJEF.Dlg != null)
			{
				DMCJGOMOJEF.Dlg(null);
				DMCJGOMOJEF.Dlg = null;
			}
			else
			{
				ListSF.GetInstance().MHHNIPBJNAD();
			}
		}
		CallEvent(1, DMCJGOMOJEF.ScreenType);
		Eclipse.Modding.ModRuntime.ShowPendingBattleLottery();
		CallEvent(2, 0);
	}

	public void NFEBHLDPHHI(ModuleHolder MHOCFOODLLL)
	{
		FFICJOEBPAK = MHOCFOODLLL;
		OJFNMDGIDJN();
	}

	public void JOCFBBAAPBE(ModuleHolder MHOCFOODLLL)
	{
		FFICJOEBPAK = null;
		BackKeyManager.get_Instance().Clear();
	}

	public bool OMDLOOFIJDF()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			return true;
		}
		return nKGLHEGIKKP.BKBHIMEEDBG().JBPHIAEPHAH();
	}

	public void JJFFNDJDNAJ(bool NKCGCFGFNDL, bool LLOLBKJMKNC = true)
	{
		INGHJOIPMGH = NKCGCFGFNDL;
		RefreshInputLock(LLOLBKJMKNC);
	}

	public void DIDFMBMPEAF(bool FHFEIGOAJHO, bool LLOLBKJMKNC = true)
	{
		FGGGPEGPDIK = FHFEIGOAJHO;
		RefreshInputLock(LLOLBKJMKNC);
	}

    // best guess for name
	public void RefreshInputLock(bool LLOLBKJMKNC)
	{
		bool flag = INGHJOIPMGH || FGGGPEGPDIK || HNDJIOBDPLN || _presentationLocks > 0;
		if (_inputLocked != flag)
		{
			_inputLocked = flag;
			_visibleInputLock = LLOLBKJMKNC;
			if (_inputLocked)
			{
				MMHIKEIDDNB(_visibleInputLock);
			}
			else
			{
				FFIEHHJMLKJ();
			}
		}
		else if (_inputLocked && _inputLocked == flag && _visibleInputLock != LLOLBKJMKNC)
		{
			_visibleInputLock = LLOLBKJMKNC;
			FFIEHHJMLKJ();
			MMHIKEIDDNB(_visibleInputLock);
		}
	}

	private void EKMKBPNAKCN(bool value, bool LLOLBKJMKNC = true)
	{
		LockScreen.Lock(value, LLOLBKJMKNC);
	}

	private void MMHIKEIDDNB(bool LLOLBKJMKNC)
	{
		EKMKBPNAKCN(_inputLocked, LLOLBKJMKNC);
	}

	private void FFIEHHJMLKJ()
	{
		EKMKBPNAKCN(_inputLocked);
	}

	public void NPMIHDFCBBH()
	{
		ScreenType hBGBPDEGKFE = NMCNDOPKFJD();
		DLOKJOHNDID(hBGBPDEGKFE);
	}
}
