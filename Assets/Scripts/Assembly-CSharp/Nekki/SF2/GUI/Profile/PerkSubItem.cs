using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nekki.SF2.GUI.Profile
{
	public class PerkSubItem : SubItem
	{
		public enum PPAOEMJNACM
		{
			onPerkImprove = 12
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProfilePerk AAJEBIINPNL;

		protected Action<object> _dlg;

		protected InfoAnimation BJONHDGCNFE;

		[SerializeField]
		private ResolutionImage _upgradeLevelIcon;

		protected KAHIFHMHDAF JLANLOEGGEP;

		protected bool EODJNKFPEHH;

		protected bool DCHFLKPBOBB;

		public ProfilePerk MBDDKGIOOGD
		{
			get
			{
				return get_Perk();
			}
			private set
			{
				NOLDHAFMOLF(value);
			}
		}

		public ProfilePerk get_Perk()
		{
			return AAJEBIINPNL;
		}

		private void NOLDHAFMOLF(ProfilePerk value)
		{
			AAJEBIINPNL = value;
		}

		public void Init(ProfilePerk AEFFHJGMNFI, int OKNNNLIPODI)
		{
			Init(OKNNNLIPODI);
			_upgradeLevelIcon.gameObject.SetActive(false);
			Clear();
			NOLDHAFMOLF(AEFFHJGMNFI);
			if (get_Perk() != null)
			{
				get_Perk().AddEventListener(0, NBJIGFCBEAL);
				get_Perk().AddEventListener(1, DILLOCHIAOL);
				get_Perk().AddEventListener(2, IKNEDPAJELH);
				get_Perk().AddEventListener(3, BAKFAGENODP);
			}
			_dlg = ADLHFBEOJPB;
			BJONHDGCNFE = MGJFIMFODOM();
			if (get_Perk() != null)
			{
				JLANLOEGGEP = new KAHIFHMHDAF(get_Perk().KAMBOKLFBEE(), get_Perk().GJOAJAIJHOE(), get_Perk().FLBBFDNHJAJ(), _dlg, BJONHDGCNFE);
				Data = JLANLOEGGEP;
			}
			GJPJJHACOJJ = ((get_Perk() == null) ? string.Empty : get_Perk().OPIOIHAPMDG());
			DCHFLKPBOBB = false;
			EODJNKFPEHH = false;
			BHKAAODJMJF = ProfileGUI.OJEAKFALOGE.EBDBPJNBHGI / 255f;
			CDNOKAKOLMP = ProfileGUI.OJEAKFALOGE.DPGMCKCDMBC / 255f;
			FOPPGHBAKHJ(true);
			UpdateIcon();
			UpdateState();
			EJFGMHPJHGI();
			if (ListSF.CCDKHLAMKKO().PINDEKDNCNL() < AEFFHJGMNFI.PINDEKDNCNL())
			{
				SetLock(true);
			}
		}

		private new void OnDestroy()
		{
			Clear();
			RemoveAllEventListener();
		}

		public void Clear()
		{
			if (get_Perk() != null)
			{
				get_Perk().RemoveEventListener(0, NBJIGFCBEAL);
				get_Perk().RemoveEventListener(1, DILLOCHIAOL);
				get_Perk().RemoveEventListener(2, IKNEDPAJELH);
				get_Perk().RemoveEventListener(3, BAKFAGENODP);
			}
		}

		public override void SetLock(bool AJPDLMOHKEN)
		{
			EODJNKFPEHH = AJPDLMOHKEN;
			base.SetLock(EODJNKFPEHH || (get_Perk() != null && get_Perk().FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_LOCK));
		}

		public override bool GetLock()
		{
			return EODJNKFPEHH || get_Perk() == null || get_Perk().FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_LOCK;
		}

		public override void Choose()
		{
			if (get_Perk() != null)
			{
				((KAHIFHMHDAF)Data).state = get_Perk().FLBBFDNHJAJ();
			}
			base.Choose();
		}

		public override void UpdateState()
		{
			if (get_Perk() == null)
			{
				return;
			}
			switch (get_Perk().FLBBFDNHJAJ())
			{
			case ProfilePerk.KMHBPKKCNPP.PERK_AVAILABLE:
				DCHFLKPBOBB = false;
				SetActive(true);
				break;
			case ProfilePerk.KMHBPKKCNPP.PERK_UNAVAILABLE:
				DCHFLKPBOBB = false;
				if ((bool)_icon)
				{
					UIExtensions.HNIHBGAOAIH(_icon, BHKAAODJMJF);
				}
				SetActive(false);
				break;
			case ProfilePerk.KMHBPKKCNPP.PERK_SELECTED:
				DCHFLKPBOBB = true;
				if ((bool)_icon)
				{
					UIExtensions.HNIHBGAOAIH(_icon, BHKAAODJMJF);
				}
				SetActive(true);
				break;
			case ProfilePerk.KMHBPKKCNPP.PERK_LOCK:
				SetActive(true);
				SetSelected(false);
				break;
			}
			SetLock(EODJNKFPEHH);
			if (!IIPJNGBMJJP)
			{
			}
		}

		public bool IsInfoAnimation()
		{
			return BJONHDGCNFE != null;
		}

		public override void OnPointerDown(PointerEventData BHOLFGOGPCP)
		{
			if (!GetLock())
			{
				base.OnPointerDown(BHOLFGOGPCP);
			}
		}

		protected override void FGICHADOEHF()
		{
			base.FGICHADOEHF();
			if (get_Perk() != null && get_Perk().FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_AVAILABLE)
			{
				AJGODMIMDDP();
			}
		}

		protected InfoAnimation MGJFIMFODOM()
		{
			if (get_Perk() == null)
			{
				return null;
			}
			List<Trick> list = AnimationData.BFNFDDLNHPA();
			for (int i = 0; i < list.Count; i++)
			{
				if (get_Perk().DFOELJAEEGG() != null && list[i].Name == get_Perk().DFOELJAEEGG().JNBECGKCNBB)
				{
					return list[i].KJHMOGGECBN;
				}
			}
			return null;
		}

		protected bool FNALAKIPMBN()
		{
			return get_Perk() != null && get_Perk().FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_SELECTED;
		}

		protected bool IMNGDPMMNMJ()
		{
			return get_Perk() != null && get_Perk().FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_UNAVAILABLE;
		}

		protected string BGDKCEBADDL()
		{
			int num = ((get_Perk() == null) ? 1 : get_Perk().LMGGMMFEODJ());
			string text = "ProfilePieces.level";
			return text + num;
		}

		protected void EJFGMHPJHGI()
		{
			if (get_Perk() != null)
			{
				bool flag = get_Perk().LMGGMMFEODJ() <= 0;
				ProfilePerk.JHDKDOPHGOO jHDKDOPHGOO = get_Perk().get_Type();
				bool flag2 = get_Perk().LMGGMMFEODJ() == 1 && jHDKDOPHGOO != ProfilePerk.JHDKDOPHGOO.TYPE_UPGRADE && jHDKDOPHGOO != ProfilePerk.JHDKDOPHGOO.TYPE_PERK_SELETED;
				if (!flag && !flag2)
				{
					_upgradeLevelIcon.gameObject.SetActive(true);
					string spriteName = BGDKCEBADDL();
					_upgradeLevelIcon.set_SpriteName(spriteName);
					float num = 95f;
					float num2 = -34f;
					_upgradeLevelIcon.transform.OKHPLHPBPKJ(num + num2);
					_upgradeLevelIcon.transform.BGNJGIACJBG(0f - (num + num2));
				}
			}
		}

		protected void BMDLJNPHPGF()
		{
			BJONHDGCNFE = MGJFIMFODOM();
			if (JLANLOEGGEP != null && get_Perk() != null)
			{
				JLANLOEGGEP.name = get_Perk().KAMBOKLFBEE();
				JLANLOEGGEP.EMDJGBHIAIA = get_Perk().GJOAJAIJHOE();
				JLANLOEGGEP.state = get_Perk().FLBBFDNHJAJ();
				JLANLOEGGEP.HCBDNEOKGNK = BJONHDGCNFE;
			}
		}

		private void ADLHFBEOJPB(object data)
		{
			CallEvent(12, this);
		}

		private void NBJIGFCBEAL(object data)
		{
			UpdateState();
		}

		private void DILLOCHIAOL(object data)
		{
			NOLDHAFMOLF(null);
		}

		private void IKNEDPAJELH(object data)
		{
			UpdateIcon();
			UpdateState();
			BMDLJNPHPGF();
			EJFGMHPJHGI();
		}

		private void BAKFAGENODP(object data)
		{
			BMDLJNPHPGF();
			EJFGMHPJHGI();
		}
	}
}
