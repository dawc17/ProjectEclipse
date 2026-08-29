using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;

namespace Nekki.SF2.Core.Fights.Controller
{
	public class GameController : SFMonoBehaviour<object>
	{
		public enum HDPJABCJEIC
		{
			TYPE_NONE = 0,
			TYPE_STICK = 1,
			TYPE_KEYBOARD = 2
		}

		public enum ELNFGDNMPDP
		{
			OnControlPressed = 0,
			OnControlReleased = 1
		}

		public struct NKHHIKLLJAC
		{
			private object data;

			private HDPJABCJEIC LFLGCDNKNJI;
		}

		private static GameController _Current;

		[SerializeField]
		private Stick _joystick;

		[SerializeField]
		private GameObject _leftContainer;

		private FightCID AGEAHBBKHMB;

		[SerializeField]
		private ActionButtons _actionButtons;

		private FHHECMPNKHC NBMONJPAMHI = new FHHECMPNKHC();

		private List<int> CCMBIEPECGN;

		private int LBNAOBIJIHF;

		private bool GOEHALKBLGK = true;

		private bool GCMMMFABBFC = true;

		private bool DGEIJHIPFIG = true;

		private bool JKDKBHNKCPH;

		private const float GamepadDeadZone = 0.35f;

		private FightCID _gamepadDirection = FightCID.QuadrantZero;

		private bool _gamepadPunchPressed;

		private bool _gamepadKickPressed;

		private bool _gamepadRangedPressed;

		private bool _gamepadMagicPressed;

		public static GameController BLOOLFFMKFI
		{
			get
			{
				return get_Current();
			}
		}

		public static GameController get_Current()
		{
			return _Current;
		}

		private void Awake()
		{
			_Current = this;
		}

		private void OnDestroy()
		{
			_Current = null;
		}

		private void Start()
		{
		}

		private void Update()
		{
			NBMONJPAMHI.Render();
			RenderGamepad();
		}

		public void Init(bool DFDCOMCCEEP = true, bool GJHOPBBMHDA = true, bool BIMHGOMADEJ = true)
		{
			_actionButtons.Init();
			GOEHALKBLGK = DFDCOMCCEEP;
			GCMMMFABBFC = GJHOPBBMHDA;
			DGEIJHIPFIG = BIMHGOMADEJ;
			InitController();
		}

		public void InitController()
		{
			if (!AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				NELFBBBKDEC(GOEHALKBLGK, GCMMMFABBFC);
				PDKHGNCLOIP();
			}
			CACNKNEFOCF();
			HMGCHHIOPEP();
			LBNAOBIJIHF = 0;
		}

		public Stick GetJoystick()
		{
			return _joystick;
		}

		public List<ProgressButton> GetButtons()
		{
			return _actionButtons.GetButtons();
		}

		public SFButton GetButtonKick()
		{
			return _actionButtons.GetButtonKick();
		}

		public SFButton GetButtonPunch()
		{
			return _actionButtons.GetButtonPunch();
		}

		public ActionButtons GetActionButtons()
		{
			return _actionButtons;
		}

		public void IsShowController(bool HHFKEDNEOIL)
		{
			base.gameObject.SetActive(HHFKEDNEOIL);
			_joystick.gameObject.SetActive(HHFKEDNEOIL);
			_actionButtons.gameObject.SetActive(HHFKEDNEOIL);
			if (!HHFKEDNEOIL)
			{
				if (!DGEIJHIPFIG)
				{
					_joystick.gameObject.SetActive(false);
				}
				if (!GOEHALKBLGK)
				{
					_actionButtons.GetButtonPunch().gameObject.SetActive(false);
				}
				if (!GCMMMFABBFC)
				{
					_actionButtons.GetButtonKick().gameObject.SetActive(false);
				}
			}
		}

		public void SetPunchEnabled(bool value)
		{
			_actionButtons.SetPunchEnabled(value);
			GOEHALKBLGK = value;
		}

		public void SetKickEnabled(bool value)
		{
			_actionButtons.SetKickEnabled(value);
			GCMMMFABBFC = value;
		}

		public void SetStickEnabled(bool value)
		{
			_joystick.gameObject.SetActive(value);
			DGEIJHIPFIG = value;
		}

		public bool GetPunchEnabled()
		{
			return GOEHALKBLGK;
		}

		public bool GetKickEnabled()
		{
			return GCMMMFABBFC;
		}

		public bool GetStickEnabled()
		{
			return DGEIJHIPFIG;
		}

		public void ResetController()
		{
			NBMONJPAMHI.Clear();
			NBMONJPAMHI.RemoveEventListener(0, ANEHCIIOHOK);
			NBMONJPAMHI.RemoveEventListener(1, EEJDBDNNABN);
			_joystick.RemoveEventListener(0, KJJNFLFLNHB);
			_joystick.RemoveEventListener(1, KJJNFLFLNHB);
			_joystick.RemoveEventListener(2, CICPKENEGNI);
			_actionButtons.RemoveEventListener(1, KJJNFLFLNHB);
			_actionButtons.RemoveEventListener(2, CICPKENEGNI);
			StopController();
		}

		public void AddKeysModels()
		{
			if (!AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.W, FightCID.QuadrantUp);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.S, FightCID.QuadrantDown);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.A, FightCID.QuadrantBack);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.D, FightCID.QuadrantForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.E, FightCID.QuadrantUpForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.C, FightCID.QuadrantDownForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Z, FightCID.QuadrantDownBack);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Q, FightCID.QuadrantUpBack);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad8, FightCID.QuadrantUp);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad9, FightCID.QuadrantUpForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad6, FightCID.QuadrantForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad3, FightCID.QuadrantDownForward);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad2, FightCID.QuadrantDown);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad1, FightCID.QuadrantDownBack);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad4, FightCID.QuadrantBack);
				NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad7, FightCID.QuadrantUpBack);
			}
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.O, FightCID.Punch);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.P, FightCID.Kick);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.K, FightCID.MissileButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.L, FightCID.MagicButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.J, FightCID.RaidChargeButton);
		}

		public bool IsQuadrantEnabled(FightCID KGBGENDIMBC)
		{
			if (!GOEHALKBLGK && KGBGENDIMBC == FightCID.Punch)
			{
				return false;
			}
			if (!GCMMMFABBFC && KGBGENDIMBC == FightCID.Kick)
			{
				return false;
			}
			if (!DGEIJHIPFIG && FCCPDEBPMCH(KGBGENDIMBC))
			{
				return false;
			}
			return true;
		}

		public void StartController()
		{
			BFMNHIPMDMG(true);
		}

		public void StopController()
		{
			BFMNHIPMDMG(false);
		}

		public void ClearButtonsAppearance()
		{
			if (AssemblyController.PGFJMOGKEID())
			{
				SetKickEnabled(true);
				SetPunchEnabled(true);
			}
		}

		public static bool IsDirectionQuadrant(FightCID DFOLKDCLLLN)
		{
			return DFOLKDCLLLN >= FightCID.QuadrantZero && DFOLKDCLLLN <= FightCID.QuadrantUpBack;
		}

		private void PDKHGNCLOIP()
		{
			_joystick.RemoveEventListener(0, KJJNFLFLNHB);
			_joystick.AddEventListener(0, KJJNFLFLNHB);
			_joystick.RemoveEventListener(1, KJJNFLFLNHB);
			_joystick.AddEventListener(1, KJJNFLFLNHB);
			_joystick.RemoveEventListener(2, CICPKENEGNI);
			_joystick.AddEventListener(2, CICPKENEGNI);
		}

		private void NELFBBBKDEC(bool DFDCOMCCEEP = true, bool GJHOPBBMHDA = true)
		{
			_actionButtons.RemoveEventListener(1, KJJNFLFLNHB);
			_actionButtons.AddEventListener(1, KJJNFLFLNHB);
			_actionButtons.RemoveEventListener(2, CICPKENEGNI);
			_actionButtons.AddEventListener(2, CICPKENEGNI);
			_actionButtons.SetPunchEnabled(DFDCOMCCEEP);
			_actionButtons.SetKickEnabled(GJHOPBBMHDA);
		}

		private void CACNKNEFOCF()
		{
			NBMONJPAMHI.Init();
			NBMONJPAMHI.RemoveEventListener(0, ANEHCIIOHOK);
			NBMONJPAMHI.RemoveEventListener(1, EEJDBDNNABN);
			NBMONJPAMHI.AddEventListener(0, ANEHCIIOHOK);
			NBMONJPAMHI.AddEventListener(1, EEJDBDNNABN);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad0, FightCID.QuadrantZero);
			AddKeysModels();
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.RightArrow, FightCID.NextFrameButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad1, FightCID.WinRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad2, FightCID.WinFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad3, FightCID.ResetRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad4, FightCID.ResetFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad5, FightCID.LossRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad6, FightCID.LossFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad7, FightCID.RechargeMagic);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad8, FightCID.IncreaseComboHit);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad9, FightCID.IncreaseStyle);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Keypad0, FightCID.SetPlayerAllHitsCritical);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha1, FightCID.WinRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha2, FightCID.WinFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha3, FightCID.ResetRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha4, FightCID.ResetFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha5, FightCID.LossRoundButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha6, FightCID.LossFightButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha7, FightCID.RechargeMagic);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha8, FightCID.IncreaseComboHit);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha9, FightCID.IncreaseStyle);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.Alpha0, FightCID.SetPlayerAllHitsCritical);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F1, FightCID.SlowModeKey);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F3, FightCID.ShowEdgesButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F4, FightCID.PauseButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F5, FightCID.ShowDebugPerksButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F6, FightCID.SetPlayerImmortality);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F7, FightCID.SetBotImmortality);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F10, FightCID.FullscreenMode);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F11, FightCID.EnableMinScale);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.F12, FightCID.TestTactic);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.M, FightCID.SoundMuteButton);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.B, FightCID.StartBenchmarkKey);
			NBMONJPAMHI.NGHDGMNEPJB(KeyCode.U, FightCID.StartSuper);
		}

		private void HMGCHHIOPEP()
		{
			_gamepadDirection = FightCID.QuadrantZero;
			_gamepadPunchPressed = false;
			_gamepadKickPressed = false;
			_gamepadRangedPressed = false;
			_gamepadMagicPressed = false;
		}

		private void BFMNHIPMDMG(bool value)
		{
			if (!value && JKDKBHNKCPH)
			{
				ReleaseGamepadControls();
			}
			JKDKBHNKCPH = value;
			NBMONJPAMHI.DCHJDPCEODD = value;
		}

		private void RenderGamepad()
		{
			if (!JKDKBHNKCPH)
			{
				return;
			}

			Vector2 dpad = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.Dpad, GamePad.GGAKHLLMPMM.One, true);
			Vector2 leftStick = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.LeftStick, GamePad.GGAKHLLMPMM.One, true);
			Vector2 movement = dpad.sqrMagnitude >= GamepadDeadZone * GamepadDeadZone ? dpad : leftStick;
			SetGamepadDirection(GetGamepadDirection(movement));

			SetGamepadButton(ref _gamepadPunchPressed, GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.X, GamePad.GGAKHLLMPMM.One), FightCID.Punch);
			SetGamepadButton(ref _gamepadKickPressed, GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.A, GamePad.GGAKHLLMPMM.One), FightCID.Kick);
			SetGamepadButton(ref _gamepadRangedPressed, GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.B, GamePad.GGAKHLLMPMM.One), FightCID.MissileButton);
			SetGamepadButton(ref _gamepadMagicPressed, GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.Y, GamePad.GGAKHLLMPMM.One), FightCID.MagicButton);
		}

		private static FightCID GetGamepadDirection(Vector2 direction)
		{
			if (direction.sqrMagnitude < GamepadDeadZone * GamepadDeadZone)
			{
				return FightCID.QuadrantZero;
			}

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			if (angle < 0f)
			{
				angle += 360f;
			}

			if (angle < 27.5f || angle >= 332.5f)
			{
				return FightCID.QuadrantForward;
			}
			if (angle < 62.5f)
			{
				return FightCID.QuadrantUpForward;
			}
			if (angle < 117.5f)
			{
				return FightCID.QuadrantUp;
			}
			if (angle < 152.5f)
			{
				return FightCID.QuadrantUpBack;
			}
			if (angle < 207.5f)
			{
				return FightCID.QuadrantBack;
			}
			if (angle < 242.5f)
			{
				return FightCID.QuadrantDownBack;
			}
			if (angle < 297.5f)
			{
				return FightCID.QuadrantDown;
			}
			return FightCID.QuadrantDownForward;
		}

		private void SetGamepadDirection(FightCID direction)
		{
			if (!DGEIJHIPFIG)
			{
				direction = FightCID.QuadrantZero;
			}
			if (_gamepadDirection == direction)
			{
				return;
			}

			if (_gamepadDirection != FightCID.QuadrantZero)
			{
				SendGamepadControlEvent(1, _gamepadDirection);
			}
			_gamepadDirection = direction;
			if (_gamepadDirection != FightCID.QuadrantZero)
			{
				SendGamepadControlEvent(0, _gamepadDirection);
			}
		}

		private void SetGamepadButton(ref bool state, bool pressed, FightCID control)
		{
			bool value = pressed && IsQuadrantEnabled(control);
			if (state == value)
			{
				return;
			}
			state = value;
			SendGamepadControlEvent(value ? 0 : 1, control);
		}

		private void ReleaseGamepadControls()
		{
			SetGamepadDirection(FightCID.QuadrantZero);
			ReleaseGamepadButton(ref _gamepadPunchPressed, FightCID.Punch);
			ReleaseGamepadButton(ref _gamepadKickPressed, FightCID.Kick);
			ReleaseGamepadButton(ref _gamepadRangedPressed, FightCID.MissileButton);
			ReleaseGamepadButton(ref _gamepadMagicPressed, FightCID.MagicButton);
		}

		private void ReleaseGamepadButton(ref bool state, FightCID control)
		{
			if (!state)
			{
				return;
			}
			state = false;
			SendGamepadControlEvent(1, control);
		}

		private void SendGamepadControlEvent(int eventType, FightCID control)
		{
			CBBEIGACPPD cBBEIGACPPD = new CBBEIGACPPD();
			cBBEIGACPPD.Index = 0;
			cBBEIGACPPD.KMOPCKPBHIA = control;
			CallEvent(eventType, cBBEIGACPPD);
		}

		private void KJJNFLFLNHB(object data)
		{
			CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
			if (cBBEIGACPPD.KMOPCKPBHIA != FightCID.QuadrantZero && IsQuadrantEnabled(cBBEIGACPPD.KMOPCKPBHIA))
			{
				CallEvent(0, cBBEIGACPPD);
			}
		}

		private void CICPKENEGNI(object data)
		{
			CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
			if (IsQuadrantEnabled(cBBEIGACPPD.KMOPCKPBHIA))
			{
				CallEvent(1, cBBEIGACPPD);
			}
		}

		private void ANEHCIIOHOK(object data)
		{
			CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
			if (!AssemblyController.JONCCPLEIBE().OKALPNOADLJ() || !IsDirectionQuadrant(cBBEIGACPPD.KMOPCKPBHIA))
			{
				if (cBBEIGACPPD.KMOPCKPBHIA != FightCID.QuadrantZero && IsQuadrantEnabled(cBBEIGACPPD.KMOPCKPBHIA))
				{
					CallEvent(0, cBBEIGACPPD);
				}
			}
			else
			{
				CGMOFEOMPCB(cBBEIGACPPD);
			}
		}

		private void EEJDBDNNABN(object data)
		{
			CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
			if (!AssemblyController.JONCCPLEIBE().OKALPNOADLJ() || !IsDirectionQuadrant(cBBEIGACPPD.KMOPCKPBHIA))
			{
				if (IsQuadrantEnabled(cBBEIGACPPD.KMOPCKPBHIA))
				{
					CallEvent(1, cBBEIGACPPD);
				}
			}
			else
			{
				CGMOFEOMPCB(cBBEIGACPPD);
			}
		}

		private bool FCCPDEBPMCH(FightCID KGBGENDIMBC)
		{
			return KGBGENDIMBC > FightCID.QuadrantZero && KGBGENDIMBC <= FightCID.QuadrantUpBack;
		}

		private void CGMOFEOMPCB(CBBEIGACPPD DFIBLGKFAHN)
		{
			List<FightCID> list = new List<FightCID>();
			int i = 0;
			for (int count = NBMONJPAMHI.BFEBNHGFIHB.Count; i < count; i++)
			{
				foreach (CBBEIGACPPD.GIPHMILLKGA item in NBMONJPAMHI.BFEBNHGFIHB[i])
				{
					if (Input.GetKeyDown(item.EDEEELJMHLG) || Input.GetKey(item.EDEEELJMHLG))
					{
						if (!item.isActive)
						{
							item.isActive = true;
						}
						list.Add(item.Index);
					}
					else if (item.isActive)
					{
						item.isActive = false;
						DFIBLGKFAHN.KMOPCKPBHIA = item.Index;
						CallEvent(1, DFIBLGKFAHN);
					}
				}
			}
			FightCID eCHINOPKGGI = FightCID.QuadrantZero;
			if (list.Count == 1)
			{
				eCHINOPKGGI = list[0];
			}
			else if (list.Count > 1)
			{
				FightCID eCHINOPKGGI2 = list[0];
				FightCID eCHINOPKGGI3 = list[1];
				switch (eCHINOPKGGI2)
				{
				case FightCID.QuadrantUp:
					switch (eCHINOPKGGI3)
					{
					case FightCID.QuadrantForward:
						eCHINOPKGGI = FightCID.QuadrantUpForward;
						break;
					case FightCID.QuadrantBack:
						eCHINOPKGGI = FightCID.QuadrantUpBack;
						break;
					}
					break;
				case FightCID.QuadrantForward:
					switch (eCHINOPKGGI3)
					{
					case FightCID.QuadrantUp:
						eCHINOPKGGI = FightCID.QuadrantUpForward;
						break;
					case FightCID.QuadrantDown:
						eCHINOPKGGI = FightCID.QuadrantDownForward;
						break;
					}
					break;
				case FightCID.QuadrantDown:
					switch (eCHINOPKGGI3)
					{
					case FightCID.QuadrantForward:
						eCHINOPKGGI = FightCID.QuadrantDownForward;
						break;
					case FightCID.QuadrantBack:
						eCHINOPKGGI = FightCID.QuadrantDownBack;
						break;
					}
					break;
				case FightCID.QuadrantBack:
					switch (eCHINOPKGGI3)
					{
					case FightCID.QuadrantUp:
						eCHINOPKGGI = FightCID.QuadrantUpBack;
						break;
					case FightCID.QuadrantDown:
						eCHINOPKGGI = FightCID.QuadrantDownBack;
						break;
					}
					break;
				}
			}
			if (AGEAHBBKHMB != eCHINOPKGGI)
			{
				if (AGEAHBBKHMB != FightCID.QuadrantZero)
				{
					DFIBLGKFAHN.KMOPCKPBHIA = AGEAHBBKHMB;
					CallEvent(1, DFIBLGKFAHN);
				}
				AGEAHBBKHMB = eCHINOPKGGI;
				if (eCHINOPKGGI != FightCID.QuadrantZero)
				{
					DFIBLGKFAHN.KMOPCKPBHIA = eCHINOPKGGI;
					CallEvent(0, DFIBLGKFAHN);
				}
			}
		}

		public void SetScale(float BDEMEIHKADI)
		{
			_leftContainer.transform.localScale = new Vector3(BDEMEIHKADI, BDEMEIHKADI);
			_actionButtons.transform.localScale = new Vector3(BDEMEIHKADI, BDEMEIHKADI);
		}
	}
}
