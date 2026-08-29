using CodeStage.AntiCheat.ObscuredTypes;
using SF2DE.Underworld.UI;
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

		private readonly UnderworldRaidLifeBarTransition _raidTransition =
			new UnderworldRaidLifeBarTransition();

		private readonly UnderworldRaidLifeBarStyle _raidStyle =
			new UnderworldRaidLifeBarStyle();

		public void SetRaidStyle(bool raidBoss)
		{
			_raidStyle.Apply(_healthBar, _background, raidBoss);
		}

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
			_raidTransition.Reset(HEGIABHIPHA.RemainingHealthBars);
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
			UnderworldRaidLifeBarUpdate raidUpdate = _raidTransition.Update(
				num,
				HEGIABHIPHA.RemainingHealthBars,
				HEGIABHIPHA.HealthBarCount,
				GDJBAIIDKDE,
				GJBDOHGGIAA,
				CNNFPAMBLCN);
			if (raidUpdate.Handled)
			{
				ApplyRaidLifeBarUpdate(raidUpdate);
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

		private void ApplyRaidLifeBarUpdate(UnderworldRaidLifeBarUpdate update)
		{
			if (update.ResetLife)
			{
				ResetLife();
				return;
			}
			if (update.SetHitDelay)
			{
				FBFNFLBPACL = update.HitDelay;
			}
			if (update.SetHealthBar)
			{
				SetValBarValue(update.HealthBarValue, update.HealthBarFrames);
			}
			if (update.SetHitBar)
			{
				SetHitBarValue(update.HitBarValue, update.HitBarFrames);
			}
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
