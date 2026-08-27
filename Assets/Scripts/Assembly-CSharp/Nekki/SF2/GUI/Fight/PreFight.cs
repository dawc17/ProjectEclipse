using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Fight
{
	public class PreFight : MonoBehaviour
	{
		public class LDFICONDCGC : UnityEvent<ViewerFight.PLGDCJPCLPN>
		{
		}

		public LDFICONDCGC OnButtonClick = new LDFICONDCGC();

		public ScreenFight.JGGPBICMICP OnStopScreen = new ScreenFight.JGGPBICMICP();

		public UnityEvent OnAchievementMessageHide = new UnityEvent();

		[SerializeField]
		private GameObject viewerFightPrefab;

		[SerializeField]
		private GameObject screenFightPrefab;

		[SerializeField]
		private GameObject pauseScreenPrefab;

		[SerializeField]
		private GameObject endFightScreenPrefab;

		[SerializeField]
		private GameObject achievementMessagePrefab;

		private FightList KGKDKENMAOA;

		private ViewerFight viewerFight;

		private ScreenFight screenFight;

		private PauseScreen BMIDCILCFNK;

		private EndFightScreen JJGINCAHMMP;

		private AchievementMessage achievementMessage;

		public ViewerFight NLMBMKAABLF
		{
			get
			{
				return get_ViewerFight();
			}
		}

		public int CLDABPBDDGB
		{
			get
			{
				return get_TimeLeft();
			}
		}

		public int APOMCBJKMLJ
		{
			get
			{
				return get_TimeLeftFrames();
			}
		}

		public int BLOMEBACGDF
		{
			get
			{
				return get_TimePassedFrames();
			}
		}

		public int PJPKOGPLCBH
		{
			get
			{
				return get_TimeTotalRoundFrames();
			}
		}

		public ViewerFight get_ViewerFight()
		{
			return viewerFight;
		}

		public int get_TimeLeft()
		{
			if (viewerFight != null)
			{
				return (ObscuredInt)(viewerFight.get_TimeSecond());
			}
			return 0;
		}

		public int get_TimeLeftFrames()
		{
			if (viewerFight != null)
			{
				return (ObscuredInt)(viewerFight.get_TimeCount());
			}
			return 0;
		}

		public int get_TimePassedFrames()
		{
			if (viewerFight != null)
			{
				return viewerFight.get_RoundTimeTotalFrames() - (ObscuredInt)(viewerFight.get_TimeCount());
			}
			return 0;
		}

		public int get_TimeTotalRoundFrames()
		{
			if (viewerFight != null)
			{
				return viewerFight.get_RoundTimeTotalFrames();
			}
			return 0;
		}

		public ScreenFightType get_Type()
		{
			if (screenFight != null)
			{
				return screenFight.Type;
			}
			return ScreenFightType.TYPE_INFO_NONE;
		}

		public void Init(FightList KGKDKENMAOA)
		{
			this.KGKDKENMAOA = KGKDKENMAOA;
			InitPreFight();
		}

		public void InitPreFight(ComboStatistic AIOMDIAFHGB = null, ComboStatistic MOJHPBGGNAH = null)
		{
			if (viewerFight == null)
			{
				viewerFight = Object.Instantiate(viewerFightPrefab).GetComponent<ViewerFight>();
				viewerFight.transform.SetParent(base.transform, false);
				viewerFight.AddEventListener(0, OnButtonClicked);
				viewerFight.PreInit(AIOMDIAFHGB, MOJHPBGGNAH);
			}
			if (screenFight == null)
			{
				screenFight = Object.Instantiate(screenFightPrefab).GetComponent<ScreenFight>();
				screenFight.transform.SetParent(base.transform, false);
				screenFight.gameObject.SetActive(false);
				screenFight.OnStopScreen.AddListener(OnScreenStop);
				screenFight.PreInit(KGKDKENMAOA);
			}
			VisibleViewer(false);
		}

		public void OpenPauseScreen()
		{
			if (pauseScreenPrefab != null && BMIDCILCFNK == null)
			{
				BMIDCILCFNK = Object.Instantiate(pauseScreenPrefab).GetComponent<PauseScreen>();
				BMIDCILCFNK.gameObject.SetActive(true);
				BMIDCILCFNK.transform.SetParent(base.transform, false);
				BMIDCILCFNK.transform.SetAsLastSibling();
				BMIDCILCFNK.OnSurrender.AddListener(HNHMFDIFEML);
				BMIDCILCFNK.OnPlay.AddListener(CHJNNHEJFKO);
				BMIDCILCFNK.Init();
			}
		}

		public void ClosePauseScreen()
		{
			if (BMIDCILCFNK != null)
			{
				BMIDCILCFNK.gameObject.SetActive(false);
				Object.Destroy(BMIDCILCFNK.gameObject);
				BMIDCILCFNK = null;
			}
		}

		public void OpenEndFightScreen(FightResult DCJLKCFKCOM)
		{
			if (endFightScreenPrefab != null)
			{
				JJGINCAHMMP = Object.Instantiate(endFightScreenPrefab).GetComponent<EndFightScreen>();
				JJGINCAHMMP.gameObject.SetActive(true);
				Transform parent = ((!(base.transform.parent != null)) ? base.transform : base.transform.parent);
				JJGINCAHMMP.transform.SetParent(parent, false);
				JJGINCAHMMP.transform.SetAsLastSibling();
				JJGINCAHMMP.Init(DCJLKCFKCOM);
			}
		}

		public void Render()
		{
			if (viewerFight != null)
			{
				viewerFight.Render();
			}
		}

		public void RenderComboModel()
		{
			if (viewerFight != null)
			{
				viewerFight.RenderComboModel();
			}
		}

		public void ViewerInit(Round round, ModelParameters GKCDEPEKKEL, ModelParameters GJMOIENEDPB, bool ENCAKAAMEPN = true)
		{
			if (viewerFight != null)
			{
				viewerFight.Init(round, GKCDEPEKKEL, GJMOIENEDPB, ENCAKAAMEPN);
			}
		}

		public void ViewerPlay()
		{
			if (viewerFight != null)
			{
				viewerFight.Play();
			}
		}

		public void ViewerStrike(InfoAnimation IFPDGKDKJOD, float CKKFKEIELCP, int LBIOCDCPAGO, bool isFirstStrike, bool FABADFPDLPG, bool OOGIBOBMGJA, bool OOCLHFGEPML, bool EPKEEMFHHFM)
		{
			if (viewerFight != null)
			{
				viewerFight.Strike(IFPDGKDKJOD, CKKFKEIELCP, LBIOCDCPAGO, isFirstStrike, FABADFPDLPG, OOGIBOBMGJA, OOCLHFGEPML, EPKEEMFHHFM);
			}
		}

		public void ViewerUpdateVictorys()
		{
			if (viewerFight != null)
			{
				viewerFight.UpdateVictorys();
			}
		}

		public void ViewerUpdateHotGroundTimer(int time, RuleAppliance EJPOJJKKICO)
		{
			if (viewerFight != null)
			{
				viewerFight.UpdateHotGroundTimer(time, EJPOJJKKICO);
			}
		}

		public void VisibleViewer(bool value)
		{
			if (viewerFight != null)
			{
				viewerFight.SetVisible(value);
			}
		}

		public void ViewerPauseVisible(bool value)
		{
			if (viewerFight != null)
			{
				viewerFight.PauseVisible(value);
			}
		}

		public void Reset()
		{
			if (screenFight != null)
			{
				screenFight.OnStopScreen.RemoveListener(OnScreenStop);
			}
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(false);
				Object.Destroy(item.gameObject);
			}
			viewerFight = null;
			screenFight = null;
			BMIDCILCFNK = null;
		}

		public void CreateVS(ModelParameters JCICKLIMBEF, List<ModelParameters> IDAAONBIBJM, int OBLEMIHLFII, bool BBBNBKIMHJC, bool GDLJMEJBGPO, bool IFMCDDIGOLD)
		{
			if (screenFight != null)
			{
				screenFight.CreateVS(JCICKLIMBEF, IDAAONBIBJM, OBLEMIHLFII, BBBNBKIMHJC, GDLJMEJBGPO, IFMCDDIGOLD);
			}
		}

		public void CreateRound(int value, bool JMBAAPAPMGB)
		{
			if (screenFight != null)
			{
				screenFight.CreateRound(value, JMBAAPAPMGB);
			}
			if (viewerFight != null)
			{
				viewerFight.Reset();
			}
		}

		public void CreateSkipRound()
		{
			if (screenFight != null)
			{
				screenFight.CreateSkipRound();
			}
			if (viewerFight != null)
			{
				viewerFight.Reset();
			}
		}

		public void CreateFight()
		{
			if (screenFight != null)
			{
				screenFight.CreateFight();
			}
		}

		public void CreateFightRule()
		{
			if (screenFight != null)
			{
				screenFight.CreateFightRule();
			}
		}

		public void CreateWinner(bool MBDILDFLMBL)
		{
			if (screenFight != null)
			{
				screenFight.CreateWinner(MBDILDFLMBL);
			}
			if (MBDILDFLMBL && viewerFight != null)
			{
				viewerFight.GetScreenModel(0).IGFGFICFKFH();
			}
		}

		public void CreateTimesUp()
		{
			if (screenFight != null)
			{
				screenFight.CreateTimesUp();
			}
		}

		public void CreateRingOut()
		{
			if (screenFight != null)
			{
				screenFight.CreateRingOut();
			}
		}

		public void CreateYouLose()
		{
			if (screenFight != null)
			{
				screenFight.CreateYouLose();
			}
		}

		public void CreateYouWin()
		{
			if (screenFight != null)
			{
				screenFight.CreateYouWin();
			}
		}

		public void ShowAchievementMessage(Achievement NCCHENOEPNF)
		{
			if (achievementMessagePrefab == null)
			{
				OnAchievementMessageHide.Invoke();
			}
			else if (achievementMessage == null)
			{
				achievementMessage = Object.Instantiate(achievementMessagePrefab).GetComponent<AchievementMessage>();
				achievementMessage.gameObject.SetActive(true);
				achievementMessage.transform.SetParent(base.transform, false);
				achievementMessage.OnHide.AddListener(OnAchievementMessageAnimationEnd);
				achievementMessage.Init(NCCHENOEPNF);
				achievementMessage.StartAnimation();
			}
		}

		public void OnAchievementMessageAnimationEnd()
		{
			if (achievementMessage != null)
			{
				achievementMessage.gameObject.SetActive(false);
				Object.Destroy(achievementMessage.gameObject);
				achievementMessage = null;
			}
			OnAchievementMessageHide.Invoke();
		}

		public void OnScreenStop(ScreenFightType LFLGCDNKNJI)
		{
			if (screenFight != null)
			{
				screenFight.Clear();
			}
			OnStopScreen.Invoke(LFLGCDNKNJI);
		}

		public void OnFightPause(bool value)
		{
			if (viewerFight != null)
			{
				viewerFight.OnFightPause(value);
			}
		}

		public void SetHealthBarVisible(RuleAppliance EJPOJJKKICO, bool value)
		{
			if (viewerFight != null)
			{
				viewerFight.SetHealthBarVisible(EJPOJJKKICO, value);
			}
		}

		public bool IsTimeOut()
		{
			if (viewerFight != null)
			{
				return (ObscuredInt)(viewerFight.get_TimeSecond()) <= 0;
			}
			return false;
		}

		public ComboStatistic GetStatistic(int value)
		{
			if (viewerFight != null)
			{
				return viewerFight.GetStatistic(value);
			}
			return null;
		}

		public void SetPause(bool value)
		{
			if (screenFight != null)
			{
				screenFight.set_Pause(value);
			}
		}

		public void ClearInscription()
		{
			if (screenFight != null)
			{
				screenFight.Clear();
			}
		}

		public void CreatePointsTable(float FNDOOJNDJDC, float GBCONNBABLL, int CFMPJLLNCFF, PointsTableType NOPJGLHKJPG, int LOMKKEAMMIG)
		{
			if (viewerFight != null)
			{
				viewerFight.CreatePointsTable(FNDOOJNDJDC, GBCONNBABLL, CFMPJLLNCFF, NOPJGLHKJPG, LOMKKEAMMIG);
			}
		}

		public void UpdatePointsTable(int BBNOPLBAOCF, int HBIKJBGFFBM)
		{
			if (viewerFight != null)
			{
				viewerFight.UpdatePointsTable(BBNOPLBAOCF, HBIKJBGFFBM);
			}
		}

		public void RemovePointsTable()
		{
			if (viewerFight != null)
			{
				viewerFight.RemovePointsTable();
			}
		}

		private void HNHMFDIFEML()
		{
			OnButtonClick.Invoke(ViewerFight.PLGDCJPCLPN.ButtonPauseSurrender);
		}

		private void CHJNNHEJFKO()
		{
			OnButtonClick.Invoke(ViewerFight.PLGDCJPCLPN.ButtonPausePlay);
		}

		private void OnButtonClicked(object data)
		{
			ViewerFight.PLGDCJPCLPN arg = (ViewerFight.PLGDCJPCLPN)data;
			OnButtonClick.Invoke(arg);
		}
	}
}
