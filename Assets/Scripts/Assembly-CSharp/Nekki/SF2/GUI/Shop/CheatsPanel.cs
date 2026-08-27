using Nekki.SF2.GUI.Menu;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Shop
{
	public class CheatsPanel : MonoBehaviour
	{
		[SerializeField]
		private int addMoneyCount = 1000000;

		[SerializeField]
		private int addBonusCount = 1000000;

		[SerializeField]
		private int addCurrencyCount = 1000000;

		[SerializeField]
		private GameObject cheatsButtons;

		public UnityEvent OnShowCheats = new UnityEvent();

		public UnityEvent OnHideCheats = new UnityEvent();

		public UnityEvent OnAddLevel = new UnityEvent();

		public void ShowCheats()
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				if (cheatsButtons != null)
				{
					cheatsButtons.SetActive(true);
				}
				OnShowCheats.Invoke();
			}
		}

		public void HideCheats()
		{
			if (cheatsButtons != null)
			{
				cheatsButtons.SetActive(false);
			}
			OnHideCheats.Invoke();
		}

		public void AddMoney()
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				ListSF.GCPJADIMNKI(addMoneyCount);
				if (MainMenu.get_Instance() != null)
				{
					MainMenu.get_Instance().UpdateMoney();
				}
			}
		}

		public void AddBonus()
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				ListSF.FPIJEOMBFJN(addBonusCount, Roster.HPOIJPGPOCF.CHANGE_CHEAT);
				if (MainMenu.get_Instance() != null)
				{
					MainMenu.get_Instance().UpdateMoney();
				}
			}
		}

		public void AddLevel()
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				uint bAINMLLIKOL = ListSF.CCDKHLAMKKO().HEOHJNFGEDH();
				ListSF.CCDKHLAMKKO().DBPBGBNHAIP(bAINMLLIKOL);
				if (MainMenu.get_Instance() != null)
				{
					MainMenu.get_Instance().UpdateLevel();
				}
				OnAddLevel.Invoke();
			}
		}

		public void AddCurrency()
		{
		}

		public void OpenItems()
		{
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_2", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_3", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_4", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_5", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_6", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_IM", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_1", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_2", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_3", true);
		}

		public void ResetProgress()
		{
			GameCenterController.MMGHEKOEHDB();
			ListSF.CELGPFFHLIM();
		}
	}
}
