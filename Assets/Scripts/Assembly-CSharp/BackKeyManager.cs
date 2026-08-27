using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using UnityEngine;

public class BackKeyManager : SFMonoBehaviour<object>
{
	private static BackKeyManager _instance;

	private List<BackKeyController> BOFDPBGOPEI = new List<BackKeyController>();

	public static BackKeyManager BPCBBHAKFDM
	{
		get
		{
			return get_Instance();
		}
	}

	public static BackKeyManager get_Instance()
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject("[BackKeyManager]");
			_instance = gameObject.AddComponent<BackKeyManager>();
			Object.DontDestroyOnLoad(gameObject);
		}
		return _instance;
	}

	private void OnDestroy()
	{
		RemoveAllEventListener();
		_instance = null;
	}

	public void AddBackKeyController(BackKeyController OJINMMFLEEB)
	{
		BOFDPBGOPEI.AddIfNotExist(OJINMMFLEEB);
	}

	public void RemoveBackKeyController(BackKeyController OJINMMFLEEB)
	{
		BOFDPBGOPEI.Remove(OJINMMFLEEB);
	}

	public void Clear()
	{
		BOFDPBGOPEI.Clear();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackKeyClicked();
		}
	}

	public void OnBackKeyClicked()
	{
		if (BOFDPBGOPEI.Count > 0)
		{
			BOFDPBGOPEI[BOFDPBGOPEI.Count - 1].OnBackKeyClicked(0);
			return;
		}
		switch (SceneManagerSF.EKFBDMBCDMB())
		{
		case ScreenType.ModuleFight:
		{
			FightScene current = Scene<FightScene>.get_Current();
			if (current != null && current.Fight != null)
			{
				current.Fight.HIIGDMMGBBD(true);
			}
			break;
		}
		case ScreenType.ModuleDojo:
			DialogsOpener.PMMOGEADGNL();
			break;
		case ScreenType.ModuleShop:
		case ScreenType.ModuleMap:
		case ScreenType.ModuleProfile:
			Module.DLOKJOHNDID(ScreenType.ModuleDojo);
			break;
		}
	}
}
