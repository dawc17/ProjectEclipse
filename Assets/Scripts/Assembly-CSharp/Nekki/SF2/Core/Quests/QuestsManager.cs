using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;

namespace Nekki.SF2.Core.Quests
{
	public class QuestsManager : SFMonoBehaviour<object>
	{
		public enum ILFPBLIODJI
		{
			onComplete = 0
		}

		private static GameObject DJAPEKIHNOA;

		private static QuestsManager _instance;

		private List<QuestStage> DHKJBMDEODI = new List<QuestStage>();

		private List<QuestStage> APPKOJAAHDN = new List<QuestStage>();

		private List<QuestStage> FDCOIGNKJMH = new List<QuestStage>();

		private List<QuestStage> ENPCNOKGLNP = new List<QuestStage>();

		private List<QuestStage> PEEHEMLGEEK = new List<QuestStage>();

		private List<QuestStage> CLPNNBPLHNI = new List<QuestStage>();

		private List<QuestStage> GFLCIFMKNNE = new List<QuestStage>();

		private List<QuestStage> JBEEONLHBLE = new List<QuestStage>();

		private List<QuestStage> JOHAABFIGDA = new List<QuestStage>();

		private List<QuestStage> IHLEPPLBMHB = new List<QuestStage>();

		private List<QuestStage> JGPEACBFCNC = new List<QuestStage>();

		private List<QuestStage> CCHAEJKNFJD = new List<QuestStage>();

		private List<QuestStage> EBFANILOAHE = new List<QuestStage>();

		private List<QuestStage> IHFDKIPGLIJ = new List<QuestStage>();

		private List<QuestStage> IKHLHMEBBHD = new List<QuestStage>();

		private List<QuestStage> OJAMCDCPMPF = new List<QuestStage>();

		private List<QuestStage> NNKEABGPFEN = new List<QuestStage>();

		private List<QuestStage> FPCKFKACKCE = new List<QuestStage>();

		private List<QuestStage> MKOHPGEHDLO = new List<QuestStage>();

		private List<QuestStage> DBFJLACOBFF = new List<QuestStage>();

		private List<QuestStage> JGDOLDEFDJL = new List<QuestStage>();

		private List<QuestStage> NKIIJKMCLJK = new List<QuestStage>();

		private List<QuestStage> KNAMMEEBALL = new List<QuestStage>();

		private List<QuestStage> LGJMHMALDLN = new List<QuestStage>();

		private List<QuestStage> MAOFNCGEDMB = new List<QuestStage>();

		private List<QuestStage> NLFFOAAPKGN = new List<QuestStage>();

		private List<QuestStage> CMFHNLJBBEG = new List<QuestStage>();

		private List<QuestStage> PEGLADGDOBJ = new List<QuestStage>();

		private List<QuestStage> EBDMEPFMDFB = new List<QuestStage>();

		private List<QuestStage> EHMNHCPGEBJ = new List<QuestStage>();

		private List<QuestStage> OHILCIKMKEP = new List<QuestStage>();

		private List<QuestStage> DCJAGOMFFJM = new List<QuestStage>();

		private List<QuestStage> KMEAPFJKAGH = new List<QuestStage>();

		private List<QuestStage> JFDPFELNBKO = new List<QuestStage>();

		private List<QuestStage> HCDFLCAMNEH = new List<QuestStage>();

		private List<QuestStage> POIKGFBDFED = new List<QuestStage>();

		private List<QuestStage> IHOMBIFIAPP = new List<QuestStage>();

		private List<QuestStage> PEBGDMOIAPB = new List<QuestStage>();

		private List<QuestStage> PJGJCBIPONA = new List<QuestStage>();

		private List<QuestStage> KGBPFNGIEPM = new List<QuestStage>();

		private List<QuestStage> NOLCOAEHFLF = new List<QuestStage>();

		private List<QuestStage> EJBGELOBCGG = new List<QuestStage>();

		private List<QuestStage> AFMMMAFAHLI = new List<QuestStage>();

		private List<QuestStage> OEOLPAHAPBP = new List<QuestStage>();

		private List<QuestStage> MGDDJBKDGGA = new List<QuestStage>();

		private List<QuestStage> LKECNMACNMM = new List<QuestStage>();

		private List<QuestStage> DLFEDMGKGDA = new List<QuestStage>();

		private List<QuestStage> DevXmlSceneLoaded = new List<QuestStage>();

		private List<QuestStage> DevXmlShopButtonPress = new List<QuestStage>();

		private List<QuestStage> NCJBGIFHMDK = new List<QuestStage>();

		public QuestParameters QuestParameters = new QuestParameters();

		private int currentIndex;

		[SerializeField]
		private bool _isRunActions;

		[SerializeField]
		public string CurrentQuestName = string.Empty;

		[SerializeField]
		public string CurrentActionName = string.Empty;

		[SerializeField]
		public int QuestsEndedMaxCount = 30;

		[SerializeField]
		public List<string> QuestsEnded = new List<string>();

		[SerializeField]
		public List<string> QuestsInQueue = new List<string>();

		private bool GPBPGAPMAMN;

		public static QuestsManager BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public int NHKHOJNCABG
		{
			set
			{
				set_QuestsAllCapacity(value);
			}
		}

		public bool HNGPPEOMHHB
		{
			get
			{
				return get_IsRunActions();
			}
		}

		public static QuestsManager get_Instance()
		{
			if (_instance == null)
			{
				DJAPEKIHNOA = new GameObject("QuestsManager");
				_instance = DJAPEKIHNOA.AddComponent<QuestsManager>();
				Object.DontDestroyOnLoad(DJAPEKIHNOA);
			}
			return _instance;
		}

		public static void Reset()
		{
			_instance = null;
			Object.Destroy(DJAPEKIHNOA);
			DJAPEKIHNOA = null;
		}

		public void set_QuestsAllCapacity(int value)
		{
			NCJBGIFHMDK.Capacity += value;
		}

		public bool get_IsRunActions()
		{
			return _isRunActions;
		}

		private bool FJADEODAOFO(QuestStage DOKAIKMLLDK)
		{
			if (DOKAIKMLLDK.allowDoubles)
			{
				return false;
			}
			string text = DOKAIKMLLDK.get_Name();
			foreach (QuestStage item in DHKJBMDEODI)
			{
				if (text.Equals(item.get_Name()))
				{
					return true;
				}
			}
			return false;
		}

		private void OnQuestComplete(object data)
		{
			QuestStage mLLKDGBEGJI = data as QuestStage;
			if (mLLKDGBEGJI != null)
			{
				mLLKDGBEGJI.RemoveEventListener(0, OnQuestComplete);
				DHKJBMDEODI.RemoveAt(mLLKDGBEGJI.index);
				DOEOMEJPHNF(mLLKDGBEGJI.index);
				NMIHEICJDEP(mLLKDGBEGJI.get_Name());
				DNLKMNIEHLM();
				if (DHKJBMDEODI.Count > 0)
				{
					GPBPGAPMAMN = true;
					return;
				}
				ClearActions();
				CallEvent(0, 0);
			}
		}

		private void KBFJLHJMCDO(string IEEAOCEJHGK)
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				QuestsInQueue.Add(IEEAOCEJHGK);
			}
		}

		private void DOEOMEJPHNF(int index)
		{
			if (SystemProperties.DBBOCENKMGD() && index < QuestsInQueue.Count)
			{
				QuestsInQueue.RemoveAt(index);
			}
		}

		private void NMIHEICJDEP(string IEEAOCEJHGK)
		{
			if (SystemProperties.DBBOCENKMGD())
			{
				QuestsEnded.Add(IEEAOCEJHGK);
				if (QuestsEnded.Count > QuestsEndedMaxCount)
				{
					QuestsEnded.RemoveAt(0);
				}
			}
		}

		private void GPFBBOBHHPD()
		{
			DHKJBMDEODI.Sort();
			DNLKMNIEHLM();
		}

		private void DNLKMNIEHLM()
		{
			int num = 0;
			foreach (QuestStage item in DHKJBMDEODI)
			{
				item.index = num;
				num++;
			}
		}

		private void Add(QuestStage DOKAIKMLLDK)
		{
			if (DOKAIKMLLDK != null)
			{
				NGOFBFGBICM.ELEBLBJKDBI().HIHDEKHLHKP(DOKAIKMLLDK.get_Name());
				DHKJBMDEODI.Add(DOKAIKMLLDK);
				KBFJLHJMCDO(DOKAIKMLLDK.get_Name());
				ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
				if (!GPBPGAPMAMN && !_isRunActions && iPKNDMINFMJ != ScreenType.ModuleFight)
				{
					GPBPGAPMAMN = true;
				}
			}
		}

		public void AddQuest(QuestStage PJEAMPLHPOH)
		{
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_ENTER))
			{
				APPKOJAAHDN.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_END))
			{
				FDCOIGNKJMH.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_ENTER))
			{
				ENPCNOKGLNP.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_END))
			{
				PEEHEMLGEEK.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LEVEL_UP))
			{
				CLPNNBPLHNI.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_GOT_ITEM))
			{
				GFLCIFMKNNE.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DIALOG))
			{
				JBEEONLHBLE.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SESSION))
			{
				JOHAABFIGDA.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ACTIVATE))
			{
				IHLEPPLBMHB.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE))
			{
				JGPEACBFCNC.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PREPURCHASE))
			{
				CCHAEJKNFJD.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_FB))
			{
				EBFANILOAHE.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DELIVERY))
			{
				IHFDKIPGLIJ.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENERGY))
			{
				IKHLHMEBBHD.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SERVER_CURRENCY))
			{
				OJAMCDCPMPF.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LANGUAGE_SWITCH))
			{
				NNKEABGPFEN.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FREE_SECTION_BUTTON))
			{
				FPCKFKACKCE.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_START_APPLICATION))
			{
				MKOHPGEHDLO.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_CHANGE_TAB))
			{
				DBFJLACOBFF.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL))
			{
				JGDOLDEFDJL.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_STARTER_PACK_PRESS))
			{
				NKIIJKMCLJK.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENERGY_BAR_PRESS))
			{
				KNAMMEEBALL.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_VIDEO_BUTTON_PRESS))
			{
				LGJMHMALDLN.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_TIMER_END))
			{
				MAOFNCGEDMB.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_MAP_BUTTON_PRESS))
			{
				NLFFOAAPKGN.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_MAP_BUTTON_PRESS))
			{
				CMFHNLJBBEG.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT))
			{
				PEGLADGDOBJ.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT_UNSUCCESSFUL))
			{
				EBDMEPFMDFB.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ACTIVATE_PERK))
			{
				EHMNHCPGEBJ.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DIACTIVATE_PERK))
			{
				OHILCIKMKEP.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_BUY_SPIN_GEMS))
			{
				DCJAGOMFFJM.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RESET_ASCENSION))
			{
				KMEAPFJKAGH.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SET_ITEM_ACQUIRED))
			{
				JFDPFELNBKO.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DUEL_UNLOCKED))
			{
				HCDFLCAMNEH.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_OPEN))
			{
				POIKGFBDFED.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_ENTER))
			{
				IHOMBIFIAPP.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_END))
			{
				PEBGDMOIAPB.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_BOSS_SHIELD_DESTR))
			{
				PJGJCBIPONA.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_LOGIN))
			{
				KGBPFNGIEPM.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SEAS_START_WITH_REST))
			{
				NOLCOAEHFLF.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SEAS_START_WITHOUT_REST))
			{
				EJBGELOBCGG.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DAILY_WINDOW_OPEN))
			{
				AFMMMAFAHLI.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LEADERBOARD_TAP))
			{
				OEOLPAHAPBP.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOW_REWARDED_VIDEO))
			{
				MGDDJBKDGGA.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_END))
			{
				LKECNMACNMM.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_ENTER))
			{
				DLFEDMGKGDA.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SCENE_LOADED))
			{
				DevXmlSceneLoaded.Add(PJEAMPLHPOH);
			}
			if (PJEAMPLHPOH.IsEvent(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_BUTTON_PRESS))
			{
				DevXmlShopButtonPress.Add(PJEAMPLHPOH);
			}
			NCJBGIFHMDK.Add(PJEAMPLHPOH);
		}

		public bool ActionQuest(QuestEvent.PMDPDMFLCIJ MCGHIOHACBJ)
		{
			List<QuestStage> list = null;
			switch (MCGHIOHACBJ)
			{
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_ENTER:
				list = APPKOJAAHDN;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_END:
				list = FDCOIGNKJMH;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_ENTER:
				list = ENPCNOKGLNP;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_END:
				list = PEEHEMLGEEK;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LEVEL_UP:
				list = CLPNNBPLHNI;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_GOT_ITEM:
				list = GFLCIFMKNNE;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DIALOG:
				list = JBEEONLHBLE;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SESSION:
				list = JOHAABFIGDA;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ACTIVATE:
				list = IHLEPPLBMHB;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE:
				list = JGPEACBFCNC;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PREPURCHASE:
				list = CCHAEJKNFJD;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_FB:
				list = EBFANILOAHE;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DELIVERY:
				list = IHFDKIPGLIJ;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENERGY:
				list = IKHLHMEBBHD;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SERVER_CURRENCY:
				list = OJAMCDCPMPF;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LANGUAGE_SWITCH:
				list = NNKEABGPFEN;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FREE_SECTION_BUTTON:
				list = FPCKFKACKCE;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_START_APPLICATION:
				list = MKOHPGEHDLO;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_CHANGE_TAB:
				list = DBFJLACOBFF;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL:
				list = JGDOLDEFDJL;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_STARTER_PACK_PRESS:
				list = NKIIJKMCLJK;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENERGY_BAR_PRESS:
				list = KNAMMEEBALL;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_VIDEO_BUTTON_PRESS:
				list = LGJMHMALDLN;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_TIMER_END:
				list = MAOFNCGEDMB;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_MAP_BUTTON_PRESS:
				list = NLFFOAAPKGN;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_MAP_BUTTON_PRESS:
				list = CMFHNLJBBEG;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT:
				list = PEGLADGDOBJ;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT_UNSUCCESSFUL:
				list = EBDMEPFMDFB;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ACTIVATE_PERK:
				list = EHMNHCPGEBJ;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DIACTIVATE_PERK:
				list = OHILCIKMKEP;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_BUY_SPIN_GEMS:
				list = DCJAGOMFFJM;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RESET_ASCENSION:
				list = KMEAPFJKAGH;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SET_ITEM_ACQUIRED:
				list = JFDPFELNBKO;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DUEL_UNLOCKED:
				list = HCDFLCAMNEH;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_OPEN:
				list = POIKGFBDFED;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_ENTER:
				list = IHOMBIFIAPP;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_END:
				list = PEBGDMOIAPB;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_BOSS_SHIELD_DESTR:
				list = PJGJCBIPONA;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_LOGIN:
				list = KGBPFNGIEPM;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SEAS_START_WITH_REST:
				list = NOLCOAEHFLF;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SEAS_START_WITHOUT_REST:
				list = EJBGELOBCGG;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DAILY_WINDOW_OPEN:
				list = AFMMMAFAHLI;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LEADERBOARD_TAP:
				list = OEOLPAHAPBP;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOW_REWARDED_VIDEO:
				list = MGDDJBKDGGA;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_END:
				list = LKECNMACNMM;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_ENTER:
				list = DLFEDMGKGDA;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SCENE_LOADED:
				list = DevXmlSceneLoaded;
				break;
			case QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_BUTTON_PRESS:
				list = DevXmlShopButtonPress;
				break;
			default:
				LLLOJBFMONN.Error(string.Format("{0},{1}", "Quest::actionQuest - unknown type: ", MCGHIOHACBJ));
				break;
			}
			bool flag = false;
			if (list != null)
			{
				foreach (QuestStage item in list)
				{
					if (!FJADEODAOFO(item))
					{
						if (item.Compare(QuestParameters))
						{
							AddQuestToStek(item);
							flag = true;
						}
						if (flag)
						{
							GPFBBOBHHPD();
						}
					}
				}
			}
			return flag;
		}

		public bool AddQuestToStek(string name, bool OBJGGIPDKDF)
		{
			QuestStage questByName = GetQuestByName(name);
			if (questByName != null)
			{
				return AddQuestToStek(questByName, OBJGGIPDKDF);
			}
			return false;
		}

		public bool AddQuestToStek(QuestStage DOKAIKMLLDK, bool OBJGGIPDKDF = false)
		{
			DOKAIKMLLDK.MHNEBBGMOLA(QuestParameters);
			DOKAIKMLLDK.index = DHKJBMDEODI.Count;
			Add(DOKAIKMLLDK);
			if (OBJGGIPDKDF)
			{
				GPFBBOBHHPD();
			}
			return true;
		}

		public QuestStage GetQuestByName(string name)
		{
			foreach (QuestStage item in NCJBGIFHMDK)
			{
				if (item.get_Name().Equals(name))
				{
					return item;
				}
			}
			return null;
		}

		public void Update()
		{
			if (GPBPGAPMAMN)
			{
				MHHNIPBJNAD();
			}
		}

		private void MHHNIPBJNAD()
		{
			int count = DHKJBMDEODI.Count;
			if (count <= 0)
			{
				return;
			}
			_isRunActions = true;
			GPBPGAPMAMN = false;
			bool flag = false;
			while (!flag && 0 < DHKJBMDEODI.Count)
			{
				QuestStage mLLKDGBEGJI = DHKJBMDEODI[0];
				if (mLLKDGBEGJI != null)
				{
					mLLKDGBEGJI.AddEventListener(0, OnQuestComplete);
					mLLKDGBEGJI.MHHNIPBJNAD(QuestParameters, false);
					flag = true;
					CurrentQuestName = mLLKDGBEGJI.get_Name();
				}
				else
				{
					DHKJBMDEODI.RemoveAt(0);
					DOEOMEJPHNF(0);
				}
			}
			if (DHKJBMDEODI.Count == 0)
			{
				_isRunActions = false;
				CurrentQuestName = string.Empty;
				CallEvent(0, 0);
			}
		}

		public void RunActionsAll()
		{
			if (!_isRunActions)
			{
				MHHNIPBJNAD();
			}
		}

		public bool AddActionQuest(List<QuestStage> NKNMCOEBMNG)
		{
			ParametersQuest dHLPMNEKHKD = ((NKNMCOEBMNG[0].LBIPHHIJEFP() == null) ? null : NKNMCOEBMNG[0].LBIPHHIJEFP().get_Parameters());
			if (dHLPMNEKHKD != null)
			{
				QuestParameters = NKNMCOEBMNG[0].JMHGHCAGFDI(dHLPMNEKHKD);
			}
			foreach (QuestStage item in NKNMCOEBMNG)
			{
				if (item.LBIPHHIJEFP() != null && !FJADEODAOFO(item))
				{
					item.MHNEBBGMOLA(QuestParameters);
					item.index = DHKJBMDEODI.Count;
					Add(item);
				}
			}
			GPFBBOBHHPD();
			return true;
		}

		public void ClearActions()
		{
			GPBPGAPMAMN = false;
			_isRunActions = false;
			CurrentQuestName = string.Empty;
			DHKJBMDEODI.Clear();
			QuestsInQueue.Clear();
		}

		public bool HaveCompareQuests()
		{
			return DHKJBMDEODI.Count > 0;
		}

		public void ClearStack(List<string> NIKHAICFGNM = null)
		{
			RemoveAllButThis(NIKHAICFGNM);
		}

		public void RemoveAllButThis(List<string> NIKHAICFGNM = null)
		{
			for (int i = 0; i < DHKJBMDEODI.Count; i++)
			{
				QuestStage mLLKDGBEGJI = DHKJBMDEODI[i];
				if (mLLKDGBEGJI.MHFPGCBLGIP() != QuestStage.HPOLGFKCOOE.QUEST_ACTIONS && (NIKHAICFGNM == null || NIKHAICFGNM.Count == 0 || mLLKDGBEGJI.IsGroup(NIKHAICFGNM)))
				{
					if (mLLKDGBEGJI.LBIPHHIJEFP() != null)
					{
						mLLKDGBEGJI.LBIPHHIJEFP().LCIHKPPGNPF();
					}
					mLLKDGBEGJI.RemoveEventListener(0, OnQuestComplete);
					DHKJBMDEODI.RemoveAt(i);
					DOEOMEJPHNF(i);
					i--;
				}
			}
			DNLKMNIEHLM();
			ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		}
	}
}
