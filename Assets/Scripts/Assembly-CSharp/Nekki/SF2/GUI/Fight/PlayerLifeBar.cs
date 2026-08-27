using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class PlayerLifeBar : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImageSkew _background;

		[SerializeField]
		private ResolutionImageSkew _healthBar;

		[SerializeField]
		private ResolutionImageSkew _hitBar;

		private ModelParameters HEGIABHIPHA;

		private float CNNFPAMBLCN;

		private float PHNAGDGNKCN;

		private float GDJBAIIDKDE;

		private float GJBDOHGGIAA;

		private int HMPIKELHBMG;

		private int CDJAOLBGLJK;

		private float FBFNFLBPACL;

		private bool _lockLifeUpdate;

		private int _remainingBars;

		private enum RaidHealthBarTransition
		{
			None,
			DrainingSegment,
			FillingReplacement
		}

		// Raid life is represented one segment at a time. Keep the visual
		// handoff separate from the model so an exhausted segment can finish
		// draining before the next one appears.
		private RaidHealthBarTransition _raidHealthBarTransition;

		private int _pendingSegmentsToDrain;

		private float _pendingHealthBarFraction;

		private bool _capturedNormalStyle;
		private string _normalHealthSprite;
		private string _normalBackgroundSprite;
		private Color _normalBackgroundColor;
		private Color _normalHealthColor;

		public void SetRaidStyle(bool raidBoss)
		{
			if (_healthBar == null || _background == null)
				return;
			if (!_capturedNormalStyle)
			{
				_normalHealthSprite = _healthBar.get_SpriteName();
				_normalBackgroundSprite = _background.get_SpriteName();
				_normalBackgroundColor = _background.color;
				_normalHealthColor = _healthBar.color;
				_capturedNormalStyle = true;
			}
			// Use the recovered blue gradient, not a tint of the red/orange bar.
			// All layers retain the prefab's fixed width, skew and fill direction.
			// The ordinary gold hit layer remains the delayed damage indicator.
			_healthBar.set_SpriteName(raidBoss ? "FightUI.Raid_HealthBar_Full" : _normalHealthSprite);
			_background.set_SpriteName(raidBoss ? "FightUI.Raid_HealthBar_Full" : _normalBackgroundSprite);
			_healthBar.color = raidBoss ? Color.white : _normalHealthColor;
			_background.color = raidBoss ? new Color(0.25f, 0.44f, 0.38f, 1f) : _normalBackgroundColor;
		}

		private const int CNPPCMJEHIF = 60;

		private const int OMDLEFBIGKK = 10;

		private const int COBDMAGFJDB = 30;

		public bool IILLNODMAMI
		{
			get
			{
				return get_LockLifeUpdate();
			}
			set
			{
				set_LockLifeUpdate(value);
			}
		}

		public bool get_LockLifeUpdate()
		{
			return _lockLifeUpdate;
		}

		public void set_LockLifeUpdate(bool value)
		{
			_lockLifeUpdate = value;
		}

		public RectTransform get_rectTransform()
		{
			return (RectTransform)base.transform;
		}

		public void Init(ModelParameters JCICKLIMBEF)
		{
			HEGIABHIPHA = JCICKLIMBEF;
			JEBDBEIMPLK();
		}

		private void JEBDBEIMPLK()
		{
			FBFNFLBPACL = 0f;
			_lockLifeUpdate = false;
			GDJBAIIDKDE = 0f;
			HMPIKELHBMG = 0;
			GJBDOHGGIAA = 0f;
			CDJAOLBGLJK = 0;
			CNNFPAMBLCN = 0f;
			PHNAGDGNKCN = 0f;
			ResetLife();
		}

		public void ResetLife()
		{
			_raidHealthBarTransition = RaidHealthBarTransition.None;
			_pendingSegmentsToDrain = 0;
			_pendingHealthBarFraction = 0f;
			_remainingBars = HEGIABHIPHA.RemainingHealthBars;
			FBFNFLBPACL = 0f;
			SetValBarValue(KIPMKKDPEKH());
			SetHitBarValue(KIPMKKDPEKH());
		}

		public virtual void Render()
		{
			if (!_lockLifeUpdate)
			{
				UpdateLife();
			}
			IBJGPLNHBHM();
			OFGDKCEBAPN();
			float num = 1f / (float)GameUtils.GGBABPJBGJB();
			if (FBFNFLBPACL > 0f)
			{
				FBFNFLBPACL -= num;
				if (FBFNFLBPACL <= 0f)
				{
					SetHitBarValue(CNNFPAMBLCN, 30);
				}
			}
		}

		private void IBJGPLNHBHM()
		{
			if (GDJBAIIDKDE != CNNFPAMBLCN)
			{
				float num = 0f;
				if (HMPIKELHBMG < 1)
				{
					num = CNNFPAMBLCN;
				}
				else
				{
					float num2 = GDJBAIIDKDE - CNNFPAMBLCN;
					float num3 = num2 / (float)HMPIKELHBMG;
					num = GDJBAIIDKDE - num3;
				}
				JEEHFMNKFFH(num);
				HMPIKELHBMG--;
			}
		}

		private void OFGDKCEBAPN()
		{
			if (GJBDOHGGIAA != PHNAGDGNKCN)
			{
				float num = 0f;
				if (CDJAOLBGLJK < 1)
				{
					num = PHNAGDGNKCN;
				}
				else
				{
					float num2 = GJBDOHGGIAA - PHNAGDGNKCN;
					float num3 = num2 / (float)CDJAOLBGLJK;
					num = GJBDOHGGIAA - num3;
				}
				EKAFFANKJFB(num);
				CDJAOLBGLJK--;
			}
		}

		private void UpdateLife()
		{
			float num = KIPMKKDPEKH();
			int remainingHealthBars = HEGIABHIPHA.RemainingHealthBars;
			if (_raidHealthBarTransition != RaidHealthBarTransition.None)
			{
				UpdateRaidHealthBarTransition(num, remainingHealthBars);
				return;
			}
			if (_remainingBars != remainingHealthBars)
			{
				if (HEGIABHIPHA.HealthBarCount > 1 && remainingHealthBars < _remainingBars)
				{
					StartRaidHealthBarTransition(num, remainingHealthBars);
					return;
				}
				ResetLife();
				return;
			}
			float num2 = num - CNNFPAMBLCN;
			if (num2 != 0f)
			{
				if (num2 > 0f)
				{
					SetValBarValue(num, 10);
					SetHitBarValue(num, 30);
				}
				else
				{
					FBFNFLBPACL = ((Mathf.Abs(num2) < 0.01f) ? 1 : 60);
					SetValBarValue(num, 10);
				}
			}
		}

		private void StartRaidHealthBarTransition(float targetFraction, int remainingHealthBars)
		{
			_pendingSegmentsToDrain = Mathf.Max(1, _remainingBars - remainingHealthBars);
			_pendingHealthBarFraction = targetFraction;
			_remainingBars = remainingHealthBars;
			BeginDrainingRaidHealthBarSegment();
		}

		private void UpdateRaidHealthBarTransition(float targetFraction, int remainingHealthBars)
		{
			// A hit can arrive while the HUD is still handing off the previous
			// segment. Fold its carry-over damage into this transition instead of
			// snapping the fill to the latest model value.
			if (remainingHealthBars > _remainingBars)
			{
				// Cross-segment healing is rare and must remain truthful; cancel the
				// damage-only handoff rather than animating it in the wrong direction.
				ResetLife();
				return;
			}
			if (remainingHealthBars < _remainingBars)
			{
				_pendingSegmentsToDrain += _remainingBars - remainingHealthBars;
				_remainingBars = remainingHealthBars;
			}
			_pendingHealthBarFraction = targetFraction;
			if (_raidHealthBarTransition == RaidHealthBarTransition.DrainingSegment)
			{
				if (GDJBAIIDKDE != 0f || GJBDOHGGIAA != 0f)
				{
					return;
				}
				if (_remainingBars <= 0)
				{
					CompleteRaidHealthBarTransition();
					return;
				}
				BeginFillingReplacementRaidHealthBarSegment();
				return;
			}
			if (GDJBAIIDKDE != 1f || GJBDOHGGIAA != 1f)
			{
				return;
			}
			if (_pendingSegmentsToDrain > 1)
			{
				_pendingSegmentsToDrain--;
				BeginDrainingRaidHealthBarSegment();
				return;
			}
			CompleteRaidHealthBarTransition();
		}

		private void BeginDrainingRaidHealthBarSegment()
		{
			FBFNFLBPACL = 0f;
			_raidHealthBarTransition = RaidHealthBarTransition.DrainingSegment;
			SetValBarValue(0f, OMDLEFBIGKK);
			SetHitBarValue(0f, COBDMAGFJDB);
		}

		private void BeginFillingReplacementRaidHealthBarSegment()
		{
			_raidHealthBarTransition = RaidHealthBarTransition.FillingReplacement;
			SetValBarValue(1f, OMDLEFBIGKK);
			SetHitBarValue(1f, OMDLEFBIGKK);
		}

		private void CompleteRaidHealthBarTransition()
		{
			_raidHealthBarTransition = RaidHealthBarTransition.None;
			_pendingSegmentsToDrain = 0;
			if (_remainingBars <= 0 || _pendingHealthBarFraction >= 1f)
			{
				return;
			}
			float num = _pendingHealthBarFraction - CNNFPAMBLCN;
			if (num >= 0f)
			{
				return;
			}
			FBFNFLBPACL = ((Mathf.Abs(num) < 0.01f) ? 1 : CNPPCMJEHIF);
			SetValBarValue(_pendingHealthBarFraction, OMDLEFBIGKK);
		}

		public void SetValBarValue(float value, int frames = 0)
		{
			CNNFPAMBLCN = value;
			HMPIKELHBMG = frames;
			if (HMPIKELHBMG == 0)
			{
				JEEHFMNKFFH(CNNFPAMBLCN);
			}
		}

		public void SetHitBarValue(float value, int frames = 0)
		{
			PHNAGDGNKCN = value;
			CDJAOLBGLJK = frames;
			if (CDJAOLBGLJK == 0)
			{
				EKAFFANKJFB(PHNAGDGNKCN);
			}
		}

		private void JEEHFMNKFFH(float value)
		{
			if (_healthBar != null && _healthBar.fillAmount != value)
			{
				_healthBar.fillAmount = value;
			}
			GDJBAIIDKDE = value;
		}

		private void EKAFFANKJFB(float value)
		{
			if (_hitBar != null && _hitBar.fillAmount != value)
			{
				_hitBar.fillAmount = value;
			}
			GJBDOHGGIAA = value;
		}

		private float KIPMKKDPEKH()
		{
			float num = HEGIABHIPHA == null ? 0f : HEGIABHIPHA.CurrentHealthBarFraction;
			if (num > 0f && num < FightGUI.JIBDDCOHPCC())
			{
				num = FightGUI.JIBDDCOHPCC();
			}
			return num;
		}
	}
}
