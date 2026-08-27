using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	[Serializable]
	public class ScreenModel : global::EventDispatcher<object>
	{
		public struct GCJMFLFFJAM
		{
			public ComboModel.KEDKBADCLOD Info;
		}

		public enum GHMNFKDJNAM
		{
			ON_STYLE_CHANGED = 0,
			ON_COMBO_UP = 1,
			ON_CLICK_CHEAT = 2
		}

		public enum JEDPGMIGGKK
		{
			TYPE_LEFT = 0,
			TYPE_RIGHT = 1
		}

		[SerializeField]
		private JEDPGMIGGKK _Type;

		[SerializeField]
		private ResolutionImageAvatar _Avatar;

		[SerializeField]
		private PlayerLifeBar _lifeBar;

		// Recovered Underworld raid UI: stacked boss health bars overlay.
		private RaidShieldBar _raidShields;

		[SerializeField]
		private StylePanel _stylePanel;

		[SerializeField]
		private ResolutionImage _styleName;

		[SerializeField]
		private RoundsPanel _roundsPanel;

		[SerializeField]
		private LabelAlias _name;

		[SerializeField]
		private ComboModel _comboModel;

		[SerializeField]
		private ActivePerkModel _activePerkModel;

		[SerializeField]
		private GameObject _WinBtn;

		private ModelParameters _parameters;

		private bool _showRounds = true;

		public bool IsNoBlock { get; set; }

		public ComboStatistic Statistic
		{
			get
			{
				return (!(_comboModel != null)) ? null : _comboModel.get_ComboStatistic();
			}
			set
			{
				if (_comboModel != null)
				{
					_comboModel.set_ComboStatistic(value);
				}
			}
		}

		public FightStatistics.EMKEIEJMONM MaxStyle
		{
			get
			{
				return (_stylePanel != null) ? _stylePanel.get_MaximumStyleStrip() : FightStatistics.EMKEIEJMONM.STYLE_TURTLE;
			}
		}

		public int CurrentStyleStrip
		{
			get
			{
				return (_stylePanel != null) ? _stylePanel.get_CurrentStyleStrip() : 0;
			}
		}

		public string CurrentStyleName
		{
			get
			{
				return (!(_stylePanel != null)) ? string.Empty : _stylePanel.get_CurrentStyleName();
			}
		}

		public float CurrentStyleValue
		{
			get
			{
				return (!(_stylePanel != null)) ? 0f : _stylePanel.GetStyleValue();
			}
		}

		public void Init(ModelParameters JCICKLIMBEF, bool ENCAKAAMEPN = true)
		{
			_parameters = JCICKLIMBEF;
			_showRounds = ENCAKAAMEPN;
			if (_roundsPanel != null)
				_roundsPanel.gameObject.SetActive(_showRounds);
			IsNoBlock = true;
			LOPIGAFLGDB();
			COHCIHCLGKE();
			ECHNJJALHJH();
			if (_showRounds)
			{
				PAOFIIBGPIJ();
			}
			APBGNJEHODB();
			JGKPPGIHFMN();
			DMCLPOFKHPP();
			IJDLNKFNHIG();
		}

		private void LOPIGAFLGDB()
		{
			_Avatar.set_TexturePath(SF2Paths.BHCPOOOJAAK());
			_Avatar.set_SpriteName(_parameters.HNKFHGOOKEG);
			_Avatar.SetNativeSize();
		}

		private void COHCIHCLGKE()
		{
			if (_raidShields != null)
			{
				UnityEngine.Object.Destroy(_raidShields.gameObject);
				_raidShields = null;
			}
			_lifeBar.Init(_parameters);
			bool raidBoss = _Type == JEDPGMIGGKK.TYPE_RIGHT && _parameters != null && _parameters.ShieldTotal > 0;
			_lifeBar.SetRaidStyle(raidBoss);
			if (raidBoss)
			{
				RectTransform lifeBarRect = _lifeBar.get_rectTransform();
				_raidShields = RaidShieldBar.Attach(lifeBarRect, _parameters, _name.font);
			}
		}

		private void ECHNJJALHJH()
		{
			_stylePanel.Init(_styleName);
		}

		private void PAOFIIBGPIJ()
		{
			if (_parameters != null)
			{
				_roundsPanel.Init(_parameters.HJNOICKOFDL);
			}
		}

		private void APBGNJEHODB()
		{
			if (!(_name == null))
			{
				if (_parameters.CHFEHBNIGKA != null && !_parameters.CHFEHBNIGKA.Equals(string.Empty))
				{
					_name.set_text(_parameters.CHFEHBNIGKA);
				}
				else
				{
					_name.SetAlias(_parameters.BMFLPBLAFLK);
				}
			}
		}

		private void JGKPPGIHFMN()
		{
			_comboModel.Init(_Type);
			_comboModel.AddEventListener(0, KPAPLCPCOBE);
		}

		private void IJDLNKFNHIG()
		{
			_WinBtn.SetActive(SystemProperties.DBBOCENKMGD());
			_WinBtn.GetComponent<Button>().onClick.AddListener(() =>
			{
				CallEvent(2, _Type);
			});
		}

		private void DMCLPOFKHPP()
		{
			_activePerkModel.Init();
		}

		public void CBJBDHGHJEB(InfoAnimation IFPDGKDKJOD)
		{
			if (_stylePanel != null)
			{
				_stylePanel.UpdateStyle(IFPDGKDKJOD);
				_comboModel.AddCrazyStyle(_stylePanel.get_CurrentStyleStrip());
				JAMCIPJEIFO(true);
			}
		}

		public void IncreaseStyleByValue(float value)
		{
			if (_stylePanel != null)
			{
				_stylePanel.IncreaseStyleStripByValue(value);
				_comboModel.AddCrazyStyle(_stylePanel.get_CurrentStyleStrip());
				JAMCIPJEIFO(true);
			}
		}

		public void GMFBMONNILL()
		{
			if (_showRounds && !(_roundsPanel == null))
			{
				_roundsPanel.UpdateVictories(_parameters.FCOALLOHJNP);
			}
		}

		public void JAIAMEKBNCE(bool value)
		{
			if (_comboModel != null)
			{
				_comboModel.OnFightPause(value);
			}
		}

		public void JKPOGNMHDNK(bool value)
		{
			if (_lifeBar != null)
			{
				_lifeBar.gameObject.SetActive(value);
			}
			if (_raidShields != null)
			{
				_raidShields.SetVisible(value);
			}
		}

		public void LCFPHJKKDCG(bool value)
		{
			if (_lifeBar != null)
			{
				_lifeBar.set_LockLifeUpdate(value);
			}
		}

		public void NJMJGDDBKOB()
		{
			JAMCIPJEIFO(true);
		}

		public void CPPACKAIGEK()
		{
			if (_comboModel != null)
			{
				_comboModel.Render();
			}
		}

		public void FEMAFNBEFAG()
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.Render();
			}
		}

		public void Render(bool DCAOOMFBFIO)
		{
			if (_lifeBar != null)
			{
				_lifeBar.Render();
			}
			if (_raidShields != null && _parameters != null)
			{
				_raidShields.UpdateBar((float)_parameters.KKMCHCNOHMB());
			}
			if (_stylePanel != null && DCAOOMFBFIO)
			{
				_stylePanel.Render();
				JAMCIPJEIFO(false);
			}
		}

		public void Reset()
		{
			if (_lifeBar != null)
			{
				_lifeBar.ResetLife();
			}
			if (_stylePanel != null)
			{
				_stylePanel.ResetStyle();
			}
			if (_comboModel != null)
			{
				_comboModel.ResetComboStrike();
			}
			IsNoBlock = true;
		}

		public void LFGCIFEHDMI()
		{
			if (_comboModel != null)
			{
				_comboModel.CreateCritical();
			}
		}

		public void ODLBDJKMDOJ()
		{
			if (_comboModel != null)
			{
				_comboModel.CreateFirstStrike();
			}
		}

		public void MKHJLNAFLFN()
		{
			if (_comboModel != null)
			{
				_comboModel.CreateHeadStrike();
			}
		}

		public void IGFGFICFKFH()
		{
			if (_comboModel != null)
			{
				_comboModel.AddPerfect();
			}
		}

		public void DCFGPCHGHBC()
		{
			if (_comboModel != null)
			{
				_comboModel.CreateShock();
			}
		}

		public void ShowHotGroundTimer(int time)
		{
			if (_comboModel != null)
			{
				_comboModel.UpdateHotGroundTimer(time);
			}
		}

		public void UpdateCombo(int value, int HFMKKLJGPPN)
		{
			if (_comboModel != null)
			{
				_comboModel.UpdateCombo(value, HFMKKLJGPPN);
			}
		}

		public void DGECGHDGPFO()
		{
			if (_comboModel != null)
			{
				_comboModel.RemoveAllCombo();
			}
		}

		public void PBCOANKNICH(PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.AddActivePerkItem(IBODMPMJELJ);
			}
		}

		public void DHHCHBNJDGH(PerksStage.ActionPerk CKOEFOCPMGK, PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.AddEffectPerk(CKOEFOCPMGK, IBODMPMJELJ);
			}
		}

		public void ADNAPNJMLBC(PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.RemoveActivePerkItem(IBODMPMJELJ);
			}
		}

		public void IHPHKJDODLG()
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.RemoveAllActivePerkItem();
			}
		}

		public void IBKPFLEMEAJ()
		{
			if (_activePerkModel != null)
			{
				_activePerkModel.DestroyAllPerkItems();
			}
		}

		private void JAMCIPJEIFO(bool GOAGDIANENH)
		{
			ModelStyleChange lONCJPNBHEA = new ModelStyleChange();
			lONCJPNBHEA.KJDFJPBIGJC = _Type;
			lONCJPNBHEA.StyleIndex = CurrentStyleStrip;
			lONCJPNBHEA.StyleName = CurrentStyleName;
			lONCJPNBHEA.StyleGain = CurrentStyleValue;
			lONCJPNBHEA.IsHit = GOAGDIANENH;
			CallEvent(0, lONCJPNBHEA);
		}

		private void KPAPLCPCOBE(ComboModel.KEDKBADCLOD EMBBNNBFODN)
		{
			CallEvent(1, new GCJMFLFFJAM
			{
				Info = EMBBNNBFODN
			});
		}
	}
}
