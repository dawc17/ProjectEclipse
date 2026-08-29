using System;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;

namespace SF2DE.UI.Settings
{
	public sealed class DesktopRenderSettingsControls
	{
		public delegate void ConfigureButton(ResolutionButton button, string normalSprite, string selectedSprite,
			float x, float y, SettingsDialog.AHDEAELNGBD buttonId);

		private static readonly int[] FrameRateOptions = { 0, 60, 120, 144, 165, 240, 360 };

		private readonly Transform _content;
		private readonly ResolutionButton _interpolationButton;
		private readonly LabelAlias _interpolationLabel;
		private readonly ResolutionButton _controllerButton;
		private readonly ResolutionButton _musicButton;
		private readonly ResolutionButton _soundButton;
		private readonly ConfigureButton _configureButton;
		private readonly Action _configureMusicButton;
		private readonly Action _configureSoundButton;

		private ResolutionButton _frameRateButton;
		private LabelAlias _frameRateLabel;
		private ResolutionButton _motionBlurButton;
		private LabelAlias _motionBlurLabel;

		public ResolutionButton FrameRateButton
		{
			get { return _frameRateButton; }
		}

		public ResolutionButton MotionBlurButton
		{
			get { return _motionBlurButton; }
		}

		public DesktopRenderSettingsControls(Transform content, ResolutionButton interpolationButton,
			LabelAlias interpolationLabel, ResolutionButton controllerButton, ResolutionButton musicButton,
			ResolutionButton soundButton, ConfigureButton configureButton, Action configureMusicButton,
			Action configureSoundButton)
		{
			_content = content;
			_interpolationButton = interpolationButton;
			_interpolationLabel = interpolationLabel;
			_controllerButton = controllerButton;
			_musicButton = musicButton;
			_soundButton = soundButton;
			_configureButton = configureButton;
			_configureMusicButton = configureMusicButton;
			_configureSoundButton = configureSoundButton;
		}

		public void Setup()
		{
			if (_interpolationButton == null || _content == null)
			{
				return;
			}

			Transform frameRateRow = _content.Find("btnLocationRes");
			if (frameRateRow != null)
			{
				_frameRateButton = frameRateRow.GetComponent<ResolutionButton>();
				_frameRateLabel = frameRateRow.GetComponentInChildren<LabelAlias>(true);
			}

			if (_motionBlurButton == null)
			{
				GameObject motionBlurRow = UnityEngine.Object.Instantiate(_interpolationButton.gameObject, _content, false);
				motionBlurRow.name = "btnMotionBlur";
				_motionBlurButton = motionBlurRow.GetComponent<ResolutionButton>();
				_motionBlurLabel = motionBlurRow.GetComponentInChildren<LabelAlias>(true);
			}

			_configureButton(_interpolationButton, "SettingsButtons.graphics", "SettingsButtons.graphics_selected",
				-620f, 325f, SettingsDialog.AHDEAELNGBD.BTN_RENDER_INTERPOLATION);
			_configureButton(_frameRateButton, "SettingsButtons.graphics", "SettingsButtons.graphics_selected",
				-620f, 185f, SettingsDialog.AHDEAELNGBD.BTN_MAX_FRAME_RATE);
			_configureButton(_motionBlurButton, "SettingsButtons.graphics", "SettingsButtons.graphics_selected",
				-620f, 45f, SettingsDialog.AHDEAELNGBD.BTN_MOTION_BLUR);
			_configureButton(_controllerButton, "SettingsButtons.controller", "SettingsButtons.controller_selected",
				-620f, -95f, SettingsDialog.AHDEAELNGBD.BTN_CONTROLLER);

			_configureMusicButton();
			_configureSoundButton();
			SetLocalY(_musicButton, -235f);
			SetLocalY(_soundButton, -375f);
			UpdateLabels();
		}

		public bool HandleClick(SettingsDialog.AHDEAELNGBD buttonId)
		{
			switch (buttonId)
			{
			case SettingsDialog.AHDEAELNGBD.BTN_RENDER_INTERPOLATION:
				SF2DisplayFrameRate.ToggleInterpolation();
				UpdateLabels();
				return true;
			case SettingsDialog.AHDEAELNGBD.BTN_MAX_FRAME_RATE:
				CycleMaxFrameRate();
				UpdateLabels();
				return true;
			case SettingsDialog.AHDEAELNGBD.BTN_MOTION_BLUR:
				SF2DisplayFrameRate.ToggleMotionBlur();
				UpdateLabels();
				return true;
			default:
				return false;
			}
		}

		public void UpdateLabels()
		{
			SetLabel(_interpolationLabel,
				"Frame interpolation: " + (SF2DisplayFrameRate.InterpolationEnabled ? "On" : "Off"));
			string frameRate = SF2DisplayFrameRate.MaxFrameRate <= 0
				? "Display / VSync"
				: SF2DisplayFrameRate.MaxFrameRate + " FPS";
			SetLabel(_frameRateLabel, "Max frame rate: " + frameRate);
			SetLabel(_motionBlurLabel,
				"Motion blur: " + (SF2DisplayFrameRate.MotionBlurEnabled ? "On" : "Off"));
		}

		private static void SetLabel(LabelAlias label, string text)
		{
			if (label == null)
			{
				return;
			}
			label.gameObject.SetActive(true);
			label.set_Alias(string.Empty);
			label.alignment = TextAnchor.MiddleLeft;
			label.set_LabelFontSize(101);
			label.color = Constants.PJJIMHMJPAL;
			label.set_text(text);
		}

		private static void CycleMaxFrameRate()
		{
			int current = SF2DisplayFrameRate.MaxFrameRate;
			for (int i = 0; i < FrameRateOptions.Length; i++)
			{
				if (FrameRateOptions[i] == current)
				{
					SF2DisplayFrameRate.SetMaxFrameRate(FrameRateOptions[(i + 1) % FrameRateOptions.Length]);
					return;
				}
			}
			SF2DisplayFrameRate.SetMaxFrameRate(0);
		}

		private static void SetLocalY(Component component, float y)
		{
			if (component == null)
			{
				return;
			}
			Transform transform = component.transform;
			Vector3 position = transform.localPosition;
			position.y = y;
			transform.localPosition = position;
		}
	}
}
