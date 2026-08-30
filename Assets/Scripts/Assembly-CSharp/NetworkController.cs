using System;

public class NetworkController
{
	public Action<object> OnLoginComplete = delegate
	{
	};

	private static NetworkController _Instance;

	private bool CHAPOJPCOJI = true;

	public readonly GiveLogin LBDHOLEICEG = new GiveLogin();

	public readonly LedgerManager KDILDKDNIID = new LedgerManager();

	public static NetworkController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	private NetworkController()
	{
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
		// Complete the local session without config fetches, cloud saves, news,
		// licensing, or a fake successful server login.
		if (CHAPOJPCOJI)
		{
			ListSF.ELEBLBJKDBI().MAOPKFNKHOI();
			CHAPOJPCOJI = false;
		}
		ListSF.CCDKHLAMKKO().BIHELGAGPGO();
		AHPFEEAOFMD();
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

}
