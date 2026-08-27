using System.Collections.Generic;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;

public class DialogsManager : global::EventDispatcher<object>
{
	public enum OEKBGPDCCEK
	{
		OnStopDialog = 0
	}

	private static DialogsManager _instance;

	private static List<BaseDialog> ICAMFDCLGDD = new List<BaseDialog>();

	private static DialogType AKJJGFIGFMJ;

	private static BaseDialog OALIPPPOHCL = null;

	[SerializeField]
	private static GameObject _SettingsDialogPrefab;

	public static DialogsManager BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public static DialogsManager ELEBLBJKDBI()
	{
		if (_instance == null)
		{
			_instance = new DialogsManager();
		}
		return _instance;
	}

	public static BaseDialog LAEGPJHIGAM(DialogType FPEKKMJIKBG, object data)
	{
		AKJJGFIGFMJ = FPEKKMJIKBG;
		DABAPEPMIHP(data);
		return OALIPPPOHCL;
	}

	public static void DABAPEPMIHP(object data)
	{
		OALIPPPOHCL = NHBCKGFBGMN(AKJJGFIGFMJ);
		if (!(OALIPPPOHCL == null))
		{
			OALIPPPOHCL.Init(data);
			if (NotificationsGame.get_IsOpen())
			{
				NotificationsGame.CloseNotifications();
			}
			DialogCanvasController.get_Instance().BlockNotDialogTouches();
			if (OALIPPPOHCL.TopMenuIsActive)
			{
			}
			ICAMFDCLGDD.Add(OALIPPPOHCL);
			if (!OALIPPPOHCL.IsPausing)
			{
			}
		}
	}

	public void StopDialog(BaseDialog MDOHPMBJFIL)
	{
		// Dialog prefabs live on a DontDestroyOnLoad canvas.  Scene changes and
		// reconstructed quest flows can therefore leave destroyed or inactive
		// entries in this static stack.  Treating those entries as open keeps all
		// scene GraphicRaycasters disabled after a buy/upgrade until Escape is
		// pressed.  Remove the closing dialog, duplicates, and stale entries in one
		// backwards pass before deciding whether input should remain blocked.
		int staleCount = 0;
		for (int i = ICAMFDCLGDD.Count - 1; i >= 0; i--)
		{
			BaseDialog baseDialog = ICAMFDCLGDD[i];
			if (baseDialog == null || baseDialog == MDOHPMBJFIL || !baseDialog.gameObject.activeInHierarchy)
			{
				ICAMFDCLGDD.RemoveAt(i);
				if (baseDialog != MDOHPMBJFIL)
				{
					staleCount++;
				}
			}
		}
		if (staleCount > 0)
		{
			Debug.LogWarning("[Dialogs] Removed " + staleCount + " stale dialog blocker(s).");
		}
		if (ICAMFDCLGDD.Count > 0)
		{
			OALIPPPOHCL = ICAMFDCLGDD[ICAMFDCLGDD.Count - 1];
			DialogCanvasController.get_Instance().BlockNotDialogTouches();
			for (int j = 0; j < ICAMFDCLGDD.Count; j++)
			{
			}
		}
		else
		{
			OALIPPPOHCL = null;
			DialogCanvasController.get_Instance().UnBlockTouches();
		}
		if (MDOHPMBJFIL.IsPausing)
		{
		}
		CallEvent(0, MDOHPMBJFIL);
	}

	public static void HNEGECPBALO()
	{
		BaseDialog baseDialog = ((!(OALIPPPOHCL != null)) ? null : OALIPPPOHCL);
		bool flag = baseDialog != null && !baseDialog.IsQuestDialog;
		while (flag)
		{
			OALIPPPOHCL.Close(0);
			baseDialog = ((!(OALIPPPOHCL != null)) ? null : OALIPPPOHCL);
			flag = baseDialog != null && !baseDialog.IsQuestDialog;
		}
	}

	public static BaseDialog NHBCKGFBGMN(DialogType IOCONKEEGKL)
	{
		switch (IOCONKEEGKL)
		{
		case DialogType.DialogSimple:
			return DialogCanvasController.get_Instance().CreateDialog<SimpleDialog>();
		case DialogType.DialogExit:
			return DialogCanvasController.get_Instance().CreateDialog<ExitDialog>();
		case DialogType.DialogBuy:
			return DialogCanvasController.get_Instance().CreateDialog<TradeDialog>();
		case DialogType.DialogImpossible:
			return DialogCanvasController.get_Instance().CreateDialog<ImpossibleDialog>();
		case DialogType.DialogStory:
			return DialogCanvasController.get_Instance().CreateDialog<StoryDialog>();
		case DialogType.DialogSettings:
			return DialogCanvasController.get_Instance().CreateDialog<SettingsDialog>();
		case DialogType.DialogSettingsAdvenced:
			return DialogCanvasController.get_Instance().CreateDialog<SettingsAdvancedDialog>();
		case DialogType.DialogStranger:
			return DialogCanvasController.get_Instance().CreateDialog<StrangerDialog>();
		case DialogType.DialogNews:
			return DialogCanvasController.get_Instance().CreateDialog<NewsDialog>();
		default:
			LLLOJBFMONN.Write("ERROR: getDialog - unknown dialog type: " + IOCONKEEGKL);
			return null;
		}
	}
}
