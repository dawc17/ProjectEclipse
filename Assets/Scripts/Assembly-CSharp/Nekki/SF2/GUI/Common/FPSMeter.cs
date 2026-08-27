using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Common
{
	public class FPSMeter : UIModule
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static float AFCLFLDJMML;

		[SerializeField]
		private float _UpdateInterval = 0.2f;

		[SerializeField]
		private Text _Label;

		private float FLDEMCMMMCI;

		private int _LastFramesCount;

		private float IBALEADCBMA;

		public static float OPOJHHMNNGH
		{
			get
			{
				return get_FPS();
			}
			private set
			{
				PFOLPDOJNKI(value);
			}
		}

		public static float get_FPS()
		{
			return AFCLFLDJMML;
		}

		private static void PFOLPDOJNKI(float value)
		{
			AFCLFLDJMML = value;
		}

		protected override void Init()
		{
			base.Init();
			SetTime();
		}

		protected override void PJNFHNFLNNO()
		{
			base.PJNFHNFLNNO();
		}

		private void Update()
		{
			FLDEMCMMMCI -= Time.deltaTime;
			if (FLDEMCMMMCI <= 1E-06f)
			{
				HMOOAHEBKAM();
			}
		}

		private void SetTime()
		{
			FLDEMCMMMCI = _UpdateInterval;
			_LastFramesCount = Time.frameCount;
			IBALEADCBMA = Time.realtimeSinceStartup;
		}

		private void HMOOAHEBKAM()
		{
			int num = Time.frameCount - _LastFramesCount;
			float num2 = Time.realtimeSinceStartup - IBALEADCBMA;
			SetTime();
			PFOLPDOJNKI((float)num / num2);
			_Label.text = string.Format("FPS: {0:F1}", get_FPS());
		}
	}
}
