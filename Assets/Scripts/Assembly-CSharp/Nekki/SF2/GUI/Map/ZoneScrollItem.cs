using System;
using System.Collections.Generic;
using SF2DE.Underworld;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ZoneScrollItem : BaseScrollItem, global::IEventDispatcher<object>, IComparable<ZoneScrollItem>
	{
		public enum FMJIFOJEDEE
		{
			OnClickBattle = 0
		}

		private List<BattleButton> _buttons = new List<BattleButton>();

		private Zone CODCAENBFHK;

		private Battle MMDKAHMBPHH;

		private Color _maskColor = Color.white;

		private bool _raidPowerMode;

		public GameObject BattleBtnPrefab;

		private global::EventDispatcher<object> NBKJBIIPPNB = new global::EventDispatcher<object>();

		public Zone MBBLMHCLKNK
		{
			get
			{
				return get_Zone();
			}
		}

		public Battle DGBHBIGLBPB
		{
			get
			{
				return get_LastBattle();
			}
		}

		public Zone get_Zone()
		{
			return CODCAENBFHK;
		}

		public Battle get_LastBattle()
		{
			return MMDKAHMBPHH;
		}

		public int AddEventListener(int name, Action<object> ODDEOFKLIAG)
		{
			return NBKJBIIPPNB.AddEventListener(name, ODDEOFKLIAG);
		}

		public int CallEvent(int name, object EHCLMBADLKH)
		{
			return NBKJBIIPPNB.CallEvent(name, EHCLMBADLKH);
		}

		public int RemoveAllEventListener()
		{
			return NBKJBIIPPNB.RemoveAllEventListener();
		}

		public int RemoveEvent(int name)
		{
			return NBKJBIIPPNB.RemoveEvent(name);
		}

		public int RemoveEventListener(int name, Action<object> ODDEOFKLIAG)
		{
			return NBKJBIIPPNB.RemoveEventListener(name, ODDEOFKLIAG);
		}

		public void Init(Zone HLJKOKMKMLM)
		{
			CODCAENBFHK = HLJKOKMKMLM;
			PNDHHBFCLKM();
			MGGFEPDEGLO();
		}

		public BattleButton GetBattleButtonByBattleName(string _BattleName)
		{
			Battle cGJCGEBPCAF = get_Zone().MJINKOFNIAE(_BattleName);
			if (cGJCGEBPCAF != null)
			{
				return GetButtonByBattle(cGJCGEBPCAF);
			}
			return null;
		}

		public BattleButton GetButtonByBattle(Battle DPOOIONCEOA)
		{
			foreach (BattleButton item in _buttons)
			{
				if (item.get_Battle() == DPOOIONCEOA)
				{
					return item;
				}
			}
			return null;
		}

		public void SelectFirstBattle()
		{
			List<Battle> lGIIBNJFADA = CODCAENBFHK.LGIIBNJFADA;
			if (lGIIBNJFADA.Count <= 0)
			{
				return;
			}
			bool flag = false;
			int num = 0;
			foreach (Battle item in lGIIBNJFADA)
			{
				if (item.DCHJDPCEODD && IsBattleVisibleForRaidMode(item))
				{
					flag = true;
					break;
				}
				num++;
			}
			SetLastBattle(lGIIBNJFADA[flag ? num : 0]);
		}

		public void SelectBattle()
		{
			if (MMDKAHMBPHH == null || !MMDKAHMBPHH.DCHJDPCEODD || !IsBattleVisibleForRaidMode(MMDKAHMBPHH))
			{
				SelectFirstBattle();
			}
			GLOAHJGKDKG();
			CallEvent(0, MMDKAHMBPHH);
		}

		public void UpdateBattleHidden(Battle DPOOIONCEOA)
		{
			BattleButton buttonByBattle = GetButtonByBattle(DPOOIONCEOA);
			if (null != buttonByBattle)
			{
				buttonByBattle.set_Hidden(DPOOIONCEOA.KBPNDJPMCCG());
			}
			GLOAHJGKDKG();
		}

		public void SetLastBattle(string _BattleName)
		{
			Battle lastBattle = CODCAENBFHK.MJINKOFNIAE(_BattleName);
			SetLastBattle(lastBattle);
		}

		public void SetLastBattle(Battle DPOOIONCEOA)
		{
			MMDKAHMBPHH = DPOOIONCEOA;
		}

		public void UpdateBattleFocus()
		{
			if (MMDKAHMBPHH != null && MMDKAHMBPHH.KBPNDJPMCCG())
			{
				Battle lastBattle = EKONKLLPIDJ(MMDKAHMBPHH);
				SetLastBattle(lastBattle);
				SelectBattle();
			}
		}

		public void SetBackgroundColor(Color color)
		{
			_maskColor = color;
			ResolutionImage component = GetComponent<ResolutionImage>();
			component.color = _maskColor;
		}

		public void Enabled(bool value)
		{
			int i = 0;
			for (int count = _buttons.Count; i < count; i++)
			{
				Battle battle = _buttons[i].get_Battle();
				bool flag = value && !battle.KBPNDJPMCCG() && IsBattleVisibleForRaidMode(battle);
				_buttons[i].enabled = flag;
			}
		}

		public void SetRaidPowerMode(bool enabled)
		{
			_raidPowerMode = enabled;
			foreach (BattleButton button in _buttons)
			{
				Battle battle = button.get_Battle();
				button.gameObject.SetActive(battle.DCHJDPCEODD && !battle.KBPNDJPMCCG() &&
					IsBattleVisibleForRaidMode(battle));
			}
			if (MMDKAHMBPHH == null || !IsBattleVisibleForRaidMode(MMDKAHMBPHH))
			{
				SelectFirstBattle();
			}
		}

		private bool IsBattleVisibleForRaidMode(Battle battle)
		{
			if (battle == null || !UnderworldZonePolicy.IsRaidZone(CODCAENBFHK))
			{
				return true;
			}
			bool hardMode = battle.get_Name().EndsWith("_HARDMODE", StringComparison.OrdinalIgnoreCase);
			return hardMode == _raidPowerMode;
		}

		public void ActiveBattle(string GGNFBODEOMM, bool PEJELKNFEKJ, bool HCNBLJBAOHK = true, bool DPFMIACNGLL = false)
		{
			int num = 0;
			Battle cGJCGEBPCAF = null;
			List<Battle> lGIIBNJFADA = CODCAENBFHK.LGIIBNJFADA;
			foreach (Battle item in lGIIBNJFADA)
			{
				string text = item.get_Name();
				if (text == GGNFBODEOMM)
				{
					cGJCGEBPCAF = item;
					break;
				}
				if (item.get_Type() != BattleType.FightUnregister)
				{
					num++;
				}
			}
			if (cGJCGEBPCAF != null && num < lGIIBNJFADA.Count)
			{
				HHNBJKFHGIB(cGJCGEBPCAF, num, PEJELKNFEKJ, HCNBLJBAOHK, DPFMIACNGLL);
			}
			else
			{
				LLLOJBFMONN.Error("DisplayZone::activeBattle - cant find name " + GGNFBODEOMM);
			}
		}

		private void HHNBJKFHGIB(BattleType JBJHPJMJNNF, bool PEJELKNFEKJ, bool HCNBLJBAOHK = true, bool DPFMIACNGLL = false)
		{
			int num = 0;
			Battle hHMPCKCPOEA = null;
			List<Battle> lGIIBNJFADA = CODCAENBFHK.LGIIBNJFADA;
			foreach (Battle item in lGIIBNJFADA)
			{
				if (item.get_Type() == JBJHPJMJNNF)
				{
					hHMPCKCPOEA = item;
					break;
				}
				if (item.get_Type() != BattleType.FightUnregister)
				{
					num++;
				}
			}
			if (num < lGIIBNJFADA.Count)
			{
				HHNBJKFHGIB(hHMPCKCPOEA, num, PEJELKNFEKJ, HCNBLJBAOHK, DPFMIACNGLL);
			}
			else
			{
				LLLOJBFMONN.Error("DisplayZone::activeBattle - cant find type " + JBJHPJMJNNF);
			}
		}

		private void HHNBJKFHGIB(Battle HHMPCKCPOEA, int PHPDMMMAOIJ, bool PEJELKNFEKJ, bool HCNBLJBAOHK = true, bool DPFMIACNGLL = false)
		{
			if (HHMPCKCPOEA == null)
			{
				return;
			}
			if (PHPDMMMAOIJ < _buttons.Count)
			{
				BattleButton battleButton = _buttons[PHPDMMMAOIJ];
				if (battleButton.Locked != HHMPCKCPOEA.BACJPLBBCKL())
				{
					battleButton = HPCGCHGGAGC(HHMPCKCPOEA);
				}
				HHMPCKCPOEA.DCHJDPCEODD = PEJELKNFEKJ;
				if (PEJELKNFEKJ)
				{
					if (battleButton != null)
					{
						if (DPFMIACNGLL)
						{
							battleButton.gameObject.SetActive(!HHMPCKCPOEA.KBPNDJPMCCG());
						}
						else
						{
							battleButton.SetAlpha(0f);
							battleButton.gameObject.SetActive(true);
							battleButton.SetAlpha(1f, 0.5f);
						}
					}
					SetLastBattle(HHMPCKCPOEA);
				}
				else
				{
					if ((bool)battleButton)
					{
						battleButton.gameObject.SetActive(false);
					}
					if (HHMPCKCPOEA == MMDKAHMBPHH)
					{
						SelectFirstBattle();
					}
				}
				if (HCNBLJBAOHK)
				{
					SelectBattle();
				}
			}
			else
			{
				LLLOJBFMONN.Error("DisplayZone::activeBattle - no button " + PHPDMMMAOIJ);
			}
		}

		private BattleButton HPCGCHGGAGC(Battle DPOOIONCEOA)
		{
			BattleButton buttonByBattle = GetButtonByBattle(DPOOIONCEOA);
			if (buttonByBattle == null)
			{
				LLLOJBFMONN.Error("DisplayZone::recreateButtonByBattle ERROR - btn is NULL");
				return null;
			}
			int num = 0;
			foreach (BattleButton item in _buttons)
			{
				if (item == buttonByBattle)
				{
					break;
				}
				num++;
			}
			UnityEngine.Object.Destroy(buttonByBattle.gameObject);
			buttonByBattle = EEECOHBLFAO(DPOOIONCEOA);
			buttonByBattle.transform.localPosition = new Vector3(DPOOIONCEOA.ECJPLFFAMJO().x * 2f, DPOOIONCEOA.ECJPLFFAMJO().y * 2f, buttonByBattle.transform.position.z);
			buttonByBattle.set_Hidden(DPOOIONCEOA.KBPNDJPMCCG());
			_buttons[num] = buttonByBattle;
			return buttonByBattle;
		}

		private void GLOAHJGKDKG()
		{
			foreach (BattleButton item in _buttons)
			{
				bool flag = false;
				RosterBattle dDNLCGOPAGC = item.get_Battle().NNPNEABKHPP();
				if (dDNLCGOPAGC != null)
				{
					flag = dDNLCGOPAGC.KAPIELMDIIK();
				}
				if (item.get_Hidden() != flag)
				{
					item.set_Hidden(flag);
				}
				item.gameObject.SetActive(item.get_Battle().DCHJDPCEODD && !flag &&
					IsBattleVisibleForRaidMode(item.get_Battle()));
				bool activeBattle = item.get_Battle() == MMDKAHMBPHH;
				item.SetActiveBattle(activeBattle);
			}
		}

		private void PNDHHBFCLKM()
		{
			ResolutionImage component = GetComponent<ResolutionImage>();
			component.set_TexturePath("UI/zones/");
			string text = CODCAENBFHK.EPDMGFELIMC();
			if (text == "Map0.1")
			{
				text = "Map1.1";
			}
			else if (text == "Map3.7")
			{
				text = "7";
			}
			else if (text == "Raid1.1" || text == "Raid1.2" ||
				text == "Raid1.3" || text == "Raid2.1")
			{
				// The reference export suffixes the full-resolution raid crop with
				// _0; the unsuffixed duplicate points at the half-size low atlas.
				text += "_0";
			}
			component.set_SpriteName(text);
			component.color = _maskColor;
		}

		private void MGGFEPDEGLO()
		{
			List<Battle> lGIIBNJFADA = CODCAENBFHK.LGIIBNJFADA;
			foreach (Battle item in lGIIBNJFADA)
			{
				if (item.get_Type() != BattleType.FightUnregister)
				{
					BattleButton battleButton = EEECOHBLFAO(item);
					battleButton.transform.localPosition = new Vector3(item.ECJPLFFAMJO().x * 2f, item.ECJPLFFAMJO().y * 2f, battleButton.transform.localPosition.z);
					battleButton.CorrectLabel();
					_buttons.Add(battleButton);
				}
			}
		}

		private BattleButton EEECOHBLFAO(Battle DPOOIONCEOA)
		{
			RosterBattle dDNLCGOPAGC = DPOOIONCEOA.NNPNEABKHPP();
			bool flag = dDNLCGOPAGC != null && dDNLCGOPAGC.NLIJBCHAEBK();
			bool hidden = dDNLCGOPAGC != null && dDNLCGOPAGC.KAPIELMDIIK();
			string iconAtlas = DPOOIONCEOA.GetIconAtlas();
			if (string.IsNullOrEmpty(iconAtlas) && UnderworldZonePolicy.IsRaidZone(CODCAENBFHK))
			{
				iconAtlas = "BattleBtn_raid";
			}
			BattleButton battleButton = OHDFPIADEIG(DPOOIONCEOA.MIDPFGENBCF(), DPOOIONCEOA.CCALOKFBLMC(), DPOOIONCEOA.OAIJONICMKL(), DPOOIONCEOA.JCBOGEGKLKB(), DPOOIONCEOA.GMBFCAIINAD(), flag, iconAtlas);
			battleButton.onClick.AddListener(() =>
			{
				BDGBIIIKMEH(DPOOIONCEOA);
			});
			battleButton.Locked = flag;
			battleButton.set_Battle(DPOOIONCEOA);
			battleButton.set_Hidden(hidden);
			return battleButton;
		}

		private BattleButton OHDFPIADEIG(string OEICDGHJKMP, string KHPKDMGDMAB, string HNDCJIBKBML, string JMECOJDJMIA, string JIGDGEHJEDF, bool NIBIMBDBPMI, string iconAtlas)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(BattleBtnPrefab);
			BattleButton component = gameObject.GetComponent<BattleButton>();
			component.gameObject.transform.SetParent(base.gameObject.transform, false);
			if (component != null)
			{
				component.Init(KHPKDMGDMAB, HNDCJIBKBML, JMECOJDJMIA, JIGDGEHJEDF, NIBIMBDBPMI, iconAtlas);
				component.SetAlias(OEICDGHJKMP);
			}
			return component;
		}

		private void BDGBIIIKMEH(object data)
		{
			Battle cGJCGEBPCAF = (Battle)data;
			BattleButton buttonByBattle = GetButtonByBattle(cGJCGEBPCAF);
			SetLastBattle(cGJCGEBPCAF);
			SelectBattle();
		}

		private Battle EKONKLLPIDJ(Battle DPOOIONCEOA)
		{
			BattleButton buttonByBattle = GetButtonByBattle(DPOOIONCEOA);
			float x = buttonByBattle.transform.position.x;
			float y = buttonByBattle.transform.position.y;
			double num = 2147483647.0;
			BattleButton battleButton = null;
			foreach (BattleButton item in _buttons)
			{
				if (item.get_Battle().DCHJDPCEODD && !item.get_Battle().KBPNDJPMCCG())
				{
					float x2 = item.transform.position.x;
					float y2 = item.transform.position.y;
					double num2 = Math.Pow(x2 - x, 2.0) + Math.Pow(y2 - y, 2.0);
					if (num2 < num)
					{
						num = num2;
						battleButton = item;
					}
				}
			}
			if (null != battleButton)
			{
				return battleButton.get_Battle();
			}
			return null;
		}

		public int CompareTo(ZoneScrollItem NOLFMPDGCOC)
		{
			string text = ((CODCAENBFHK == null) ? string.Empty : CODCAENBFHK.get_Name());
			string strB = ((NOLFMPDGCOC.CODCAENBFHK == null) ? string.Empty : NOLFMPDGCOC.CODCAENBFHK.get_Name());
			return text.CompareTo(strB);
		}

		private new void OnDestroy()
		{
			for (int i = 0; i < _buttons.Count; i++)
			{
				_buttons[i].onClick = null;
			}
			NBKJBIIPPNB.RemoveAllEventListener();
		}
	}
}
