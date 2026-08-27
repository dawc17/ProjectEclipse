using System.Diagnostics;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class ViewerFight : SFMonoBehaviour<object>
	{
		public enum PLGDCJPCLPN
		{
			ButtonPause = 0,
			ButtonPauseSurrender = 1,
			ButtonPausePlay = 2,
			ButtonCheatWinFight = 3,
			ButtonCheatWinRound = 4,
			ButtonCheatLoseFight = 5,
			ButtonCheatLoseRound = 6,
			ButtonCheatStartBenchmark = 7
		}

		public enum NPGEGIEDHDG
		{
			OnButtonClicked = 0
		}

		[SerializeField]
		private GameObject _pointsTablePrefab;

		[SerializeField]
		private Button btnPause;

		[SerializeField]
		private LabelAlias roundTimer;

		[SerializeField]
		private ScreenModel leftModel;

		[SerializeField]
		private ScreenModel rightModel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComboStatistic MFKEFBLNKNL;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComboStatistic JENIPIOPHAC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Round ICFBILLNEMJ;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool ABCAFNFBPBD;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool KLIBLBIFEHC;

		private PointsTable pointsTable;

		private ObscuredInt ENKHHGEMJCK = (ObscuredInt)(0);

		private ObscuredInt NFEMKPCLDDB = (ObscuredInt)(0);

		private Vector2 AKMAGAEENDB = new Vector2(-700f, 580f);

		private Vector2 JGKDIFJLHGO = new Vector2(700f, 580f);

		public ScreenModel FAGKJCDPLNC
		{
			get
			{
				return get_LeftModel();
			}
		}

		public ScreenModel CDMLHDLFONH
		{
			get
			{
				return get_RightModel();
			}
		}

		private ComboStatistic JKEGIADAKJG
		{
			get
			{
				return LMPCEACEPNB();
			}
			set
			{
				FIPBKELAEKH(value);
			}
		}

		private ComboStatistic KMDCOGOGKKD
		{
			get
			{
				return NMOOOABHGJD();
			}
			set
			{
				AHBGKOHAHFL(value);
			}
		}

		private bool EKEPPACCCPI
		{
			get
			{
				return NMEEPBDJHMG();
			}
			set
			{
				HEIGKEGAJAB(value);
			}
		}

		private bool KAJMPFJDMIF
		{
			get
			{
				return BGLPIGEPBKM();
			}
			set
			{
				CDGCDIJDODF(value);
			}
		}

		public int FNKJPCPJJLN
		{
			get
			{
				return get_RoundTimeTotal();
			}
		}

		public int MBPCDFMMJDJ
		{
			get
			{
				return get_RoundTimeTotalFrames();
			}
		}

		public ObscuredInt OLOOFCNJDKF
		{
			get
			{
				return get_TimeCount();
			}
		}

		public ObscuredInt KOMGBAOGMNF
		{
			get
			{
				return get_TimeSecond();
			}
		}

		public ScreenModel get_LeftModel()
		{
			return leftModel;
		}

		public ScreenModel get_RightModel()
		{
			return rightModel;
		}

		private ComboStatistic LMPCEACEPNB()
		{
			return MFKEFBLNKNL;
		}

		private void FIPBKELAEKH(ComboStatistic value)
		{
			MFKEFBLNKNL = value;
		}

		private ComboStatistic NMOOOABHGJD()
		{
			return JENIPIOPHAC;
		}

		private void AHBGKOHAHFL(ComboStatistic value)
		{
			JENIPIOPHAC = value;
		}

		private Round DKDGOOLAAKN()
		{
			return ICFBILLNEMJ;
		}

		private void set_Round(Round value)
		{
			ICFBILLNEMJ = value;
		}

		private bool NMEEPBDJHMG()
		{
			return ABCAFNFBPBD;
		}

		private void HEIGKEGAJAB(bool value)
		{
			ABCAFNFBPBD = value;
		}

		private bool BGLPIGEPBKM()
		{
			return KLIBLBIFEHC;
		}

		private void CDGCDIJDODF(bool value)
		{
			KLIBLBIFEHC = value;
		}

		public int get_RoundTimeTotal()
		{
			return (DKDGOOLAAKN() != null) ? DKDGOOLAAKN().timeTotal : 0;
		}

		public int get_RoundTimeTotalFrames()
		{
			return (DKDGOOLAAKN() != null) ? (DKDGOOLAAKN().timeTotal * 60) : 0;
		}

		public ObscuredInt get_TimeCount()
		{
			return ENKHHGEMJCK;
		}

		public ObscuredInt get_TimeSecond()
		{
			return NFEMKPCLDDB;
		}

		public void RandomizeObscuredVars()
		{
			ENKHHGEMJCK.GMCADPGOCHM();
			NFEMKPCLDDB.GMCADPGOCHM();
		}

		public void PreInit(ComboStatistic AIOMDIAFHGB, ComboStatistic MJOHDCPCCKB)
		{
			FIPBKELAEKH(AIOMDIAFHGB);
			AHBGKOHAHFL(MJOHDCPCCKB);
			set_Round(null);
			HEIGKEGAJAB(false);
			CDGCDIJDODF(true);
			ApplicationController.add_OnPause(ANJFIAHNKAD);
		}

		private void OnDestroy()
		{
			ApplicationController.remove_OnPause(ANJFIAHNKAD);
		}

		private void ANJFIAHNKAD(bool OIBJJLBCEHA)
		{
			if (OIBJJLBCEHA)
			{
				PausePress();
			}
		}

		public void Init(Round round, ModelParameters GKCDEPEKKEL, ModelParameters GJMOIENEDPB, bool ENCAKAAMEPN = true)
		{
			if (roundTimer != null)
			{
				// Preserve long raid rounds without clipping their third digit.
				roundTimer.horizontalOverflow = HorizontalWrapMode.Overflow;
				roundTimer.verticalOverflow = VerticalWrapMode.Overflow;
			}
			set_Round(round);
			JJDPIAPJOHO();
			CGEKLPLKIDC(leftModel, GKCDEPEKKEL, ENCAKAAMEPN, AKMAGAEENDB, "LeftModel");
			CGEKLPLKIDC(rightModel, GJMOIENEDPB, ENCAKAAMEPN, JGKDIFJLHGO, "RightModel");
			if (LMPCEACEPNB() != null && leftModel != null)
			{
				leftModel.Statistic = LMPCEACEPNB();
			}
			if (NMOOOABHGJD() != null && rightModel != null)
			{
				rightModel.Statistic = NMOOOABHGJD();
			}
			if (SystemProperties.DBBOCENKMGD())
			{
				FOLOMAHFFLG();
			}
			Reset();
		}

		private void JJDPIAPJOHO()
		{
			if (btnPause != null)
			{
				btnPause.interactable = false;
			}
		}

		private void FOLOMAHFFLG()
		{
			GameObject gameObject = new GameObject("BenchmarkButton");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.transform.SetParent(base.transform, false);
			rectTransform.sizeDelta = new Vector2(140f, 140f);
			if (roundTimer != null)
			{
				Vector3 localPosition = roundTimer.transform.localPosition;
				localPosition.y += 25f;
				rectTransform.localPosition = localPosition;
			}
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0f);
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(EAOAGOBOAFM);
		}

		private void CGEKLPLKIDC(ScreenModel ACENLMONNPA, ModelParameters KKNOCIPBIIK, bool ENCAKAAMEPN, Vector2 LCCLEFMKLPB, string name)
		{
			KKNOCIPBIIK.HJNOICKOFDL = DKDGOOLAAKN().roundTotal;
			ACENLMONNPA.Init(KKNOCIPBIIK, ENCAKAAMEPN);
			ACENLMONNPA.AddEventListener(2, OnClickCheat);
		}

		private void OnClickCheat(object data)
		{
			PLGDCJPCLPN pLGDCJPCLPN = (((ScreenModel.JEDPGMIGGKK)data != ScreenModel.JEDPGMIGGKK.TYPE_LEFT) ? PLGDCJPCLPN.ButtonCheatLoseFight : PLGDCJPCLPN.ButtonCheatWinFight);
			CallEvent(0, pLGDCJPCLPN);
		}

		private void EAOAGOBOAFM()
		{
			CallEvent(0, PLGDCJPCLPN.ButtonCheatStartBenchmark);
		}

		public void Render()
		{
			if (NMEEPBDJHMG())
			{
				if (DKDGOOLAAKN().processing)
				{
					EGKFCDIMBAB();
				}
				if (leftModel != null)
				{
					leftModel.Render(DKDGOOLAAKN().processing);
				}
				if (rightModel != null)
				{
					rightModel.Render(DKDGOOLAAKN().processing);
				}
			}
			if (leftModel != null)
			{
				leftModel.FEMAFNBEFAG();
			}
			if (rightModel != null)
			{
				rightModel.FEMAFNBEFAG();
			}
		}

		private void EGKFCDIMBAB()
		{
			ENKHHGEMJCK = (ObscuredInt)((ObscuredInt)(ENKHHGEMJCK) - 1);
			NFEMKPCLDDB = (ObscuredInt)((ObscuredInt)(ENKHHGEMJCK) / 60);
			if (roundTimer != null)
			{
				if ((ObscuredInt)(NFEMKPCLDDB) < 10)
				{
					roundTimer.set_text(string.Format("0{0}", Mathf.Max(0, (ObscuredInt)(NFEMKPCLDDB)).ToString()));
				}
				else
				{
					roundTimer.set_text(Mathf.Max(0, (ObscuredInt)(NFEMKPCLDDB)).ToString());
				}
			}
		}

		public void Reset()
		{
			HEIGKEGAJAB(false);
			ENKHHGEMJCK = (ObscuredInt)(DKDGOOLAAKN().timeTotal * 60 + 1);
			if (leftModel != null)
			{
				leftModel.Reset();
			}
			if (rightModel != null)
			{
				rightModel.Reset();
			}
			EGKFCDIMBAB();
			if (btnPause != null)
			{
				btnPause.interactable = false;
			}
		}

		public void RenderComboModel()
		{
			if (NMEEPBDJHMG())
			{
				if (leftModel != null)
				{
					leftModel.CPPACKAIGEK();
				}
				if (rightModel != null)
				{
					rightModel.CPPACKAIGEK();
				}
			}
		}

		public void Play()
		{
			DKDGOOLAAKN().processing = true;
			HEIGKEGAJAB(true);
			if (btnPause != null)
			{
				btnPause.interactable = true;
			}
		}

		public void Strike(InfoAnimation IFPDGKDKJOD, float CKKFKEIELCP, int LFLGCDNKNJI, bool isFirstStrike, bool FABADFPDLPG, bool OOGIBOBMGJA, bool OOCLHFGEPML, bool EPKEEMFHHFM)
		{
			if (leftModel == null || rightModel == null)
			{
				return;
			}
			ScreenModel screenModel = ((LFLGCDNKNJI != 0) ? leftModel : rightModel);
			ScreenModel screenModel2 = ((LFLGCDNKNJI != 0) ? rightModel : leftModel);
			if (EPKEEMFHHFM)
			{
				screenModel.DCFGPCHGHBC();
			}
			if (OOGIBOBMGJA)
			{
				screenModel.LFGCIFEHDMI();
			}
			if (!OOCLHFGEPML)
			{
				screenModel.CBJBDHGHJEB(IFPDGKDKJOD);
				if (isFirstStrike)
				{
					screenModel.ODLBDJKMDOJ();
				}
				if (FABADFPDLPG)
				{
					screenModel.MKHJLNAFLFN();
				}
				screenModel2.IsNoBlock = false;
			}
			else
			{
				screenModel.NJMJGDDBKOB();
			}
		}

		public void UpdateVictorys()
		{
			if (leftModel != null)
			{
				leftModel.GMFBMONNILL();
			}
			if (rightModel != null)
			{
				rightModel.GMFBMONNILL();
			}
		}

		public void SetVisible(bool value)
		{
			base.gameObject.SetActive(value);
			if (btnPause != null)
			{
				btnPause.gameObject.SetActive(BGLPIGEPBKM() && value);
			}
		}

		public void PauseVisible(bool value)
		{
			CDGCDIJDODF(value);
			if (btnPause != null)
			{
				btnPause.gameObject.SetActive(value);
			}
		}

		public void PausePress()
		{
			CallEvent(0, PLGDCJPCLPN.ButtonPause);
		}

		public void UpdateHotGroundTimer(int time, RuleAppliance EJPOJJKKICO)
		{
			switch (EJPOJJKKICO)
			{
			case RuleAppliance.ApplianceAll:
				if (leftModel != null)
				{
					leftModel.ShowHotGroundTimer(time);
				}
				if (rightModel != null)
				{
					rightModel.ShowHotGroundTimer(time);
				}
				break;
			case RuleAppliance.AppliancePlayer:
				if (leftModel != null)
				{
					leftModel.ShowHotGroundTimer(time);
				}
				break;
			case RuleAppliance.ApplianceOpponent:
				if (rightModel != null)
				{
					rightModel.ShowHotGroundTimer(time);
				}
				break;
			}
		}

		public void OnFightPause(bool value)
		{
			if (leftModel != null)
			{
				leftModel.JAIAMEKBNCE(value);
			}
			if (rightModel != null)
			{
				rightModel.JAIAMEKBNCE(value);
			}
		}

		public void SetHealthBarVisible(RuleAppliance EJPOJJKKICO, bool value)
		{
			switch (EJPOJJKKICO)
			{
			case RuleAppliance.AppliancePlayer:
				if (leftModel != null)
				{
					leftModel.JKPOGNMHDNK(value);
				}
				break;
			case RuleAppliance.ApplianceOpponent:
				if (rightModel != null)
				{
					rightModel.JKPOGNMHDNK(value);
				}
				break;
			case RuleAppliance.ApplianceAll:
				if (leftModel != null)
				{
					leftModel.JKPOGNMHDNK(value);
				}
				if (rightModel != null)
				{
					rightModel.JKPOGNMHDNK(value);
				}
				break;
			}
		}

		public ScreenModel GetScreenModel(int index)
		{
			return (index != 0) ? rightModel : leftModel;
		}

		public ComboStatistic GetStatistic(int index)
		{
			ScreenModel screenModel = GetScreenModel(index);
			return (screenModel == null) ? null : screenModel.Statistic;
		}

		public void CreatePointsTable(float FNDOOJNDJDC, float GBCONNBABLL, int CFMPJLLNCFF, PointsTableType GLBPKPEIOKE, int LOMKKEAMMIG)
		{
			if (pointsTable == null && _pointsTablePrefab != null)
			{
				pointsTable = Object.Instantiate(_pointsTablePrefab).GetComponent<PointsTable>();
				pointsTable.transform.SetParent(base.transform, false);
				pointsTable.Init(GLBPKPEIOKE, LOMKKEAMMIG, CFMPJLLNCFF);
			}
		}

		public void UpdatePointsTable(int BBNOPLBAOCF, int HBIKJBGFFBM)
		{
			if (pointsTable != null)
			{
				pointsTable.set_LeftScore(BBNOPLBAOCF);
				if (pointsTable.get_Type() == PointsTableType.POINTS_TABLE_CONTEST)
				{
					pointsTable.set_RightScore(HBIKJBGFFBM);
				}
			}
		}

		public void RemovePointsTable()
		{
			if (pointsTable != null)
			{
				pointsTable.gameObject.SetActive(false);
				Object.Destroy(pointsTable);
				pointsTable = null;
			}
		}

		public void SetLockLifeUpdate(bool EKBOGDKIHIH, bool value)
		{
			ScreenModel screenModel = ((!EKBOGDKIHIH) ? rightModel : leftModel);
			if (screenModel != null)
			{
				screenModel.LCFPHJKKDCG(value);
			}
		}

		public void UpdateCombo(bool EKBOGDKIHIH, int value, int HFMKKLJGPPN)
		{
			if (EKBOGDKIHIH)
			{
				if (leftModel != null)
				{
					leftModel.UpdateCombo(value, HFMKKLJGPPN);
				}
			}
			else if (rightModel != null)
			{
				rightModel.UpdateCombo(value, HFMKKLJGPPN);
			}
		}

		public void RemoveCombo()
		{
			if (leftModel != null)
			{
				leftModel.DGECGHDGPFO();
			}
			if (rightModel != null)
			{
				rightModel.DGECGHDGPFO();
			}
		}
	}
}
