using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Obscured Cheating Detector")]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		internal const string JCAOMBMKNDE = "Obscured Cheating Detector";

		internal const string MGAMICFMIJK = "[ACTk] Obscured Cheating Detector: ";

		private static int instancesInScene;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.")]
		public float floatEpsilon = 0.0001f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.")]
		public float vector2Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.")]
		public float vector3Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.")]
		public float quaternionEpsilon = 0.1f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ObscuredCheatingDetector OGKMDFDNIEN;

		public static ObscuredCheatingDetector BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
			private set
			{
				set_Instance(value);
			}
		}

		private static ObscuredCheatingDetector MCEPJKHJPIJ
		{
			get
			{
				return NNMHGMJELIL();
			}
		}

		internal static bool OEDPHHDKECI
		{
			get
			{
				return NMACGEJHPDN();
			}
		}

		private ObscuredCheatingDetector()
		{
		}

		public static void StartDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().FCJDKBEGPEF(null);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] Obscured Cheating Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			NNMHGMJELIL().FCJDKBEGPEF(callback);
		}

		public static void StopDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().DJEBEEIELBB();
			}
		}

		public static void Dispose()
		{
			if (get_Instance() != null)
			{
				get_Instance().HIEIKJFAIJE();
			}
		}

		public static ObscuredCheatingDetector get_Instance()
		{
			return OGKMDFDNIEN;
		}

		private static void set_Instance(ObscuredCheatingDetector value)
		{
			OGKMDFDNIEN = value;
		}

		private static ObscuredCheatingDetector NNMHGMJELIL()
		{
			if (get_Instance() != null)
			{
				return get_Instance();
			}
			if (ActDetectorBase.detectorsContainer == null)
			{
				ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
			}
			set_Instance(ActDetectorBase.detectorsContainer.AddComponent<ObscuredCheatingDetector>());
			return get_Instance();
		}

		internal static bool NMACGEJHPDN()
		{
			return (object)get_Instance() != null && get_Instance().EKDNCONELMD;
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(get_Instance(), "Obscured Cheating Detector"))
			{
				set_Instance(this);
			}
			SceneManager.sceneLoaded += FOFIOMHDCOM;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

		private void FOFIOMHDCOM(Scene MHOCFOODLLL, LoadSceneMode NMMPBADCFHK)
		{
			KJCKJOKLPLL();
		}

		private void KJCKJOKLPLL()
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					HIEIKJFAIJE();
				}
			}
			else if (!keepAlive && get_Instance() != this)
			{
				HIEIKJFAIJE();
			}
		}

		private void FCJDKBEGPEF(UnityAction callback)
		{
			if (EKDNCONELMD)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Obscured Cheating Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
			}
			else
			{
				detectionAction = callback;
				AKFEAJDLIKF = true;
				EKDNCONELMD = true;
			}
		}

		protected override void LICPBNOFNOB()
		{
			FCJDKBEGPEF(null);
		}

		protected override void HEGJDFPFMII()
		{
			EKDNCONELMD = false;
		}

		protected override void KLJNEJIEMCN()
		{
			if (detectionAction != null || detectionEventHasListener)
			{
				EKDNCONELMD = true;
			}
		}

		protected override void DJEBEEIELBB()
		{
			if (AKFEAJDLIKF)
			{
				detectionAction = null;
				AKFEAJDLIKF = false;
				EKDNCONELMD = false;
			}
		}

		protected override void HIEIKJFAIJE()
		{
			base.HIEIKJFAIJE();
			if (get_Instance() == this)
			{
				set_Instance(null);
			}
		}
	}
}
