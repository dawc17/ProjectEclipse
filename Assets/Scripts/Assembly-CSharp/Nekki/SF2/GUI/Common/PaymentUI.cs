using System;
using Nekki.SF2.GUI.Scripts;
using UnityEngine;
using UnityEngine.Events;
using SF2.Offline;

namespace Nekki.SF2.GUI.Common
{
	public class PaymentUI : UIModule
	{
		[SerializeField]
		private GameObject _Blocker;

		[SerializeField]
		private LoadingCircle _LoadingCircle;

		[SerializeField]
		private UnityEvent _OnProductsUpdateEvent;

		public UnityEvent OKHAFPANIDB
		{
			get
			{
				return get_OnProductsUpdateEvent();
			}
		}

		public UnityEvent get_OnProductsUpdateEvent()
		{
			return _OnProductsUpdateEvent;
		}

		protected override void Init()
		{
			base.Init();
			ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG.JILHGHCHDKN = (Action<string>)Delegate.Combine(aDEKACKLIJG.JILHGHCHDKN, new Action<string>(ABAEDAIOHDI));
			ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG2.JEAJAJMDPNL = (Action<string>)Delegate.Combine(aDEKACKLIJG2.JEAJAJMDPNL, new Action<string>(FBGKOONDOHL));
			ADEKACKLIJG aDEKACKLIJG3 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG3.ENCIAJBEOEA = (Action<string, PurchaseFailureReason>)Delegate.Combine(aDEKACKLIJG3.ENCIAJBEOEA, new Action<string, PurchaseFailureReason>(GJEBBAGEDKK));
			ADEKACKLIJG aDEKACKLIJG4 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG4.DFOLPLOOOHK = (Action<string>)Delegate.Combine(aDEKACKLIJG4.DFOLPLOOOHK, new Action<string>(AMBJFJMOKDI));
			ADEKACKLIJG aDEKACKLIJG5 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG5.OEIIAGKHMKN = (Action<string>)Delegate.Combine(aDEKACKLIJG5.OEIIAGKHMKN, new Action<string>(LBPCIPJANAE));
			ADEKACKLIJG aDEKACKLIJG6 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG6.GMKLFLAKKOJ = (Action)Delegate.Combine(aDEKACKLIJG6.GMKLFLAKKOJ, new Action(JLDACFNJGGE));
			ADEKACKLIJG aDEKACKLIJG7 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG7.MDBFKAJKPEH = (Action)Delegate.Combine(aDEKACKLIJG7.MDBFKAJKPEH, new Action(CEONLIFENPM));
			ADEKACKLIJG aDEKACKLIJG8 = ICFMIHIKGOD.OFFDIMCJOIC();
			aDEKACKLIJG8.CIIDFBBIICE = (Action)Delegate.Combine(aDEKACKLIJG8.CIIDFBBIICE, new Action(LCHCKOKGFHK));
			ICFMIHIKGOD.OFFDIMCJOIC().BKFGAIHBCHL();
			ICFMIHIKGOD.DCPEBKEGOHG();
		}

		protected override void PJNFHNFLNNO()
		{
			base.PJNFHNFLNNO();
			if (ICFMIHIKGOD.OFFDIMCJOIC() != null)
			{
				ADEKACKLIJG aDEKACKLIJG = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG.JILHGHCHDKN = (Action<string>)Delegate.Remove(aDEKACKLIJG.JILHGHCHDKN, new Action<string>(ABAEDAIOHDI));
				ADEKACKLIJG aDEKACKLIJG2 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG2.JEAJAJMDPNL = (Action<string>)Delegate.Remove(aDEKACKLIJG2.JEAJAJMDPNL, new Action<string>(FBGKOONDOHL));
				ADEKACKLIJG aDEKACKLIJG3 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG3.ENCIAJBEOEA = (Action<string, PurchaseFailureReason>)Delegate.Remove(aDEKACKLIJG3.ENCIAJBEOEA, new Action<string, PurchaseFailureReason>(GJEBBAGEDKK));
				ADEKACKLIJG aDEKACKLIJG4 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG4.DFOLPLOOOHK = (Action<string>)Delegate.Remove(aDEKACKLIJG4.DFOLPLOOOHK, new Action<string>(AMBJFJMOKDI));
				ADEKACKLIJG aDEKACKLIJG5 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG5.OEIIAGKHMKN = (Action<string>)Delegate.Remove(aDEKACKLIJG5.OEIIAGKHMKN, new Action<string>(LBPCIPJANAE));
				ADEKACKLIJG aDEKACKLIJG6 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG6.GMKLFLAKKOJ = (Action)Delegate.Remove(aDEKACKLIJG6.GMKLFLAKKOJ, new Action(JLDACFNJGGE));
				ADEKACKLIJG aDEKACKLIJG7 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG7.MDBFKAJKPEH = (Action)Delegate.Remove(aDEKACKLIJG7.MDBFKAJKPEH, new Action(CEONLIFENPM));
				ADEKACKLIJG aDEKACKLIJG8 = ICFMIHIKGOD.OFFDIMCJOIC();
				aDEKACKLIJG8.CIIDFBBIICE = (Action)Delegate.Remove(aDEKACKLIJG8.CIIDFBBIICE, new Action(LCHCKOKGFHK));
			}
		}

		public void MakePurchase(ItemInfo FAKOMBAIFPP)
		{
			DFOHNJEBDED();
			ICFMIHIKGOD.OFFDIMCJOIC().BDAAKHOLPOF(FAKOMBAIFPP.JLDEALIEEJI());
		}

		public void RestorePurchases()
		{
			DFOHNJEBDED();
			ICFMIHIKGOD.OFFDIMCJOIC().JDMELMJCKMN();
		}

		private void ABAEDAIOHDI(string FDKNIPNGFNF)
		{
			OBFDGDEAAIH();
		}

		private void FBGKOONDOHL(string FDKNIPNGFNF)
		{
			ItemInfo fAKOMBAIFPP = ListSF.CKCMJAJAELO(FDKNIPNGFNF);
			CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE, fAKOMBAIFPP);
			OBFDGDEAAIH();
		}

		private void GJEBBAGEDKK(string FDKNIPNGFNF, PurchaseFailureReason ILDDNIBBANF)
		{
			if (ILDDNIBBANF != PurchaseFailureReason.UserCancelled)
			{
				CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL, null, "Connection");
			}
			OBFDGDEAAIH();
		}

		private void AMBJFJMOKDI(string FDKNIPNGFNF)
		{
			CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL, null);
			OBFDGDEAAIH();
		}

		private void LBPCIPJANAE(string FDKNIPNGFNF)
		{
			CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL, null, "ServerNoResponse");
			OBFDGDEAAIH();
		}

		private void JLDACFNJGGE()
		{
			OBFDGDEAAIH();
		}

		private void CEONLIFENPM()
		{
			CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL, null, "Connection");
			OBFDGDEAAIH();
		}

		private void LCHCKOKGFHK()
		{
			_OnProductsUpdateEvent.Invoke();
		}

		private void CNIEJAKAIFG(QuestEvent.PMDPDMFLCIJ p_event, ItemInfo FAKOMBAIFPP, string HEMPKKHDINJ = null)
		{
			QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
			FightIDS jLGLBLDPAAF = hHKLFIIBIFF.JLGLBLDPAAF;
			hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
			hHKLFIIBIFF.HEIADONEACH = string.Empty;
			if (FAKOMBAIFPP != null)
			{
				hHKLFIIBIFF.DLKPBAJDHBO = FAKOMBAIFPP;
			}
			if (!string.IsNullOrEmpty(HEMPKKHDINJ))
			{
				hHKLFIIBIFF.OOFHDANMCJB = HEMPKKHDINJ;
			}
			if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(p_event))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			}
			hHKLFIIBIFF.JLGLBLDPAAF = jLGLBLDPAAF;
		}

		private void DFOHNJEBDED()
		{
			_Blocker.SetActive(true);
			_LoadingCircle.Play();
		}

		private void OBFDGDEAAIH()
		{
			_LoadingCircle.Stop();
			_Blocker.SetActive(false);
		}
	}
}
