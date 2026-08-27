using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.Core.Fights.Controller
{
	public class ActionButtons : SFMonoBehaviour<object>
	{
		public enum GMOKFCLLDDI
		{
			OnButtonClick = 0,
			OnButtonPress = 1,
			OnButtonRelease = 2
		}

		[SerializeField]
		private Text _lblRaidChargeCount;

		[SerializeField]
		private ProgressButton _btnKick;

		[SerializeField]
		private ProgressButton _btnPunch;

		[SerializeField]
		private ProgressButton _btnMissile;

		[SerializeField]
		private ProgressButton _btnMagic;

		[SerializeField]
		private ProgressButton _btnRaidCharge;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init()
		{
			_btnKick.Init();
			_btnKick.ButtonId = 10;
			_btnKick.AddEventListener(2, ButtonClick);
			_btnKick.AddEventListener(0, ButtonPress);
			_btnKick.AddEventListener(1, ButtonRelease);
			_btnPunch.Init();
			_btnPunch.ButtonId = 9;
			_btnPunch.AddEventListener(2, ButtonClick);
			_btnPunch.AddEventListener(0, ButtonPress);
			_btnPunch.AddEventListener(1, ButtonRelease);
			_btnMissile.Init();
			_btnMissile.ButtonId = 11;
			_btnMissile.AddEventListener(2, ButtonClick);
			_btnMissile.AddEventListener(0, ButtonPress);
			_btnMissile.AddEventListener(1, ButtonRelease);
			_btnMagic.Init();
			_btnMagic.ButtonId = 12;
			_btnMagic.AddEventListener(2, ButtonClick);
			_btnMagic.AddEventListener(0, ButtonPress);
			_btnMagic.AddEventListener(1, ButtonRelease);
			_btnRaidCharge.Init();
			_btnRaidCharge.ButtonId = 13;
			_btnRaidCharge.AddEventListener(2, ButtonClick);
			_btnRaidCharge.AddEventListener(0, ButtonPress);
			_btnRaidCharge.AddEventListener(1, ButtonRelease);
			if (!AssemblyController.PGFJMOGKEID())
			{
				_btnMissile.transform.position = _btnKick.transform.position;
				_btnMagic.transform.position = _btnPunch.transform.position;
				_btnKick.gameObject.SetActive(false);
				_btnPunch.gameObject.SetActive(false);
				_btnRaidCharge.gameObject.SetActive(false);
			}
			_btnMagic.SetPercentage(100f, 1f);
		}

		public List<ProgressButton> GetButtons()
		{
			List<ProgressButton> list = new List<ProgressButton>();
			list.Add(_btnKick);
			list.Add(_btnPunch);
			return list;
		}

		public ProgressButton GetButtonKick()
		{
			return _btnKick;
		}

		public ProgressButton GetButtonPunch()
		{
			return _btnPunch;
		}

		public ProgressButton GetButtonRaidCharge()
		{
			return _btnRaidCharge;
		}

		public void SetButtonRaidChargePos(float DHDMNHCIPEH, float BGEEALIPKCC)
		{
			_btnRaidCharge.transform.position = new Vector2(DHDMNHCIPEH, BGEEALIPKCC);
			_lblRaidChargeCount.transform.position = _btnRaidCharge.transform.position;
		}

		public void SetPunchEnabled(bool value)
		{
			_btnPunch.gameObject.SetActive(value);
		}

		public void SetKickEnabled(bool value)
		{
			_btnKick.gameObject.SetActive(value);
		}

		public void ShowMagic(bool HFIIEPMEMFF)
		{
			_btnMagic.gameObject.SetActive(HFIIEPMEMFF);
		}

		public void ShowRanged(bool GKGKKCLPGBB)
		{
			_btnMissile.gameObject.SetActive(GKGKKCLPGBB);
		}

		public void ShowRaidCharge(bool OPPBHOOBHOE)
		{
			_btnRaidCharge.gameObject.SetActive(OPPBHOOBHOE);
			_lblRaidChargeCount.gameObject.SetActive(OPPBHOOBHOE);
		}

		public void SetNeededPercentageToActBtn(FightCID DGECPBJDPNL, float NDFGBDLLMGB, float _Duration = 0.5f)
		{
			switch (DGECPBJDPNL)
			{
			case FightCID.MagicButton:
				_btnMagic.SetPercentage(NDFGBDLLMGB, _Duration);
				break;
			case FightCID.MissileButton:
				_btnMissile.SetPercentage(NDFGBDLLMGB, _Duration);
				break;
			case FightCID.RaidChargeButton:
				_btnRaidCharge.SetPercentage(NDFGBDLLMGB, _Duration);
				break;
			case FightCID.Punch:
				_btnPunch.SetPercentage(NDFGBDLLMGB, _Duration);
				break;
			case FightCID.Kick:
				_btnKick.SetPercentage(NDFGBDLLMGB, _Duration);
				break;
			}
		}

		public void SetBulletsCountToActBtn(FightCID DGECPBJDPNL, int HFBOCMEDCOA)
		{
			if (DGECPBJDPNL == FightCID.RaidChargeButton)
			{
				_lblRaidChargeCount.text = HFBOCMEDCOA.ToString();
			}
		}

		public void ResetMagicButton()
		{
			_btnMagic.ResetPercentage();
		}

		public void ResetRaidChargeButton()
		{
			_btnRaidCharge.ResetPercentage();
		}

		public virtual void SetVisible(bool HHFKEDNEOIL)
		{
			base.gameObject.SetActive(HHFKEDNEOIL);
		}

		public void ButtonClick(object data)
		{
			FightCID kJPGKHJNOMC = (FightCID)data;
			FCKDDEIIPEN(GMOKFCLLDDI.OnButtonClick, kJPGKHJNOMC);
		}

		public void ButtonPress(object data)
		{
			FightCID kJPGKHJNOMC = (FightCID)data;
			FCKDDEIIPEN(GMOKFCLLDDI.OnButtonPress, kJPGKHJNOMC);
		}

		public void ButtonRelease(object data)
		{
			FightCID kJPGKHJNOMC = (FightCID)data;
			FCKDDEIIPEN(GMOKFCLLDDI.OnButtonRelease, kJPGKHJNOMC);
		}

		private void FCKDDEIIPEN(GMOKFCLLDDI DOPHKKGNAEF, FightCID KJPGKHJNOMC)
		{
			CBBEIGACPPD cBBEIGACPPD = new CBBEIGACPPD();
			cBBEIGACPPD.Index = 0;
			cBBEIGACPPD.KMOPCKPBHIA = KJPGKHJNOMC;
			CallEvent((int)DOPHKKGNAEF, cBBEIGACPPD);
		}

		private float MCCCGGDKNPO(int count)
		{
			float num = 0.125f;
			float num2 = num * 360f / (float)count;
			if (num2 < 1.7f)
			{
				num2 = 0f;
			}
			return num2;
		}
	}
}
