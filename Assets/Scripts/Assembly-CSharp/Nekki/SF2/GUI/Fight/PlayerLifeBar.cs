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
			if (_remainingBars != HEGIABHIPHA.RemainingHealthBars)
			{
				// Refill the next segment without a false healing/damage tween.
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
