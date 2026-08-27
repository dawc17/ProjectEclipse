using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Speed Hack Detector")]
	public class SpeedHackDetector : ActDetectorBase
	{
		internal const string JCAOMBMKNDE = "Speed Hack Detector";

		internal const string MGAMICFMIJK = "[ACTk] Speed Hack Detector: ";

		private const long TICKS_PER_SECOND = 10000000L;

		private const int ILOKOOHJKFN = 5000000;

		private static int instancesInScene;

		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;

		private byte ALDILCFNLBM;

		private int CDMJCHICMAP;

		private long BIJLGFLDLAJ;

		private long EGENECDHMIC;

		private long CNFAGHGOMGO;

		private long KJFLOICFFEN;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static SpeedHackDetector OGKMDFDNIEN;

		public static SpeedHackDetector BPCBBHAKFDM
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

		private static SpeedHackDetector MCEPJKHJPIJ
		{
			get
			{
				return NNMHGMJELIL();
			}
		}

		private SpeedHackDetector()
		{
		}

		public static void StartDetection()
		{
			if (get_Instance() != null)
			{
				get_Instance().FCJDKBEGPEF(null, get_Instance().interval, get_Instance().maxFalsePositives, get_Instance().coolDown);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] Speed Hack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			StartDetection(callback, NNMHGMJELIL().interval);
		}

		public static void StartDetection(UnityAction callback, float CHCGJBLDPML)
		{
			StartDetection(callback, CHCGJBLDPML, NNMHGMJELIL().maxFalsePositives);
		}

		public static void StartDetection(UnityAction callback, float CHCGJBLDPML, byte JKBEIPOFGCI)
		{
			StartDetection(callback, CHCGJBLDPML, JKBEIPOFGCI, NNMHGMJELIL().coolDown);
		}

		public static void StartDetection(UnityAction callback, float CHCGJBLDPML, byte JKBEIPOFGCI, int CCCBHICMMJP)
		{
			NNMHGMJELIL().FCJDKBEGPEF(callback, CHCGJBLDPML, JKBEIPOFGCI, CCCBHICMMJP);
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

		public static SpeedHackDetector get_Instance()
		{
			return OGKMDFDNIEN;
		}

		private static void set_Instance(SpeedHackDetector value)
		{
			OGKMDFDNIEN = value;
		}

		private static SpeedHackDetector NNMHGMJELIL()
		{
			if (get_Instance() != null)
			{
				return get_Instance();
			}
			if (ActDetectorBase.detectorsContainer == null)
			{
				ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
			}
			set_Instance(ActDetectorBase.detectorsContainer.AddComponent<SpeedHackDetector>());
			return get_Instance();
		}

		private void Awake()
		{
			instancesInScene++;
			if (Init(get_Instance(), "Speed Hack Detector"))
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

		private void OnApplicationPause(bool KCANPMPILKI)
		{
			if (!KCANPMPILKI)
			{
				FPOIECJBFGG();
			}
		}

		private void Update()
		{
			if (!EKDNCONELMD)
			{
				return;
			}
			long ticks = DateTime.UtcNow.Ticks;
			long num = ticks - CNFAGHGOMGO;
			if (num < 0 || num > 10000000)
			{
				FPOIECJBFGG();
				return;
			}
			CNFAGHGOMGO = ticks;
			long num2 = (long)(interval * 10000000f);
			if (ticks - KJFLOICFFEN < num2)
			{
				return;
			}
			long num3 = (long)Environment.TickCount * 10000L;
			if (Mathf.Abs(num3 - EGENECDHMIC - (ticks - BIJLGFLDLAJ)) > 5000000f)
			{
				ALDILCFNLBM++;
				if (ALDILCFNLBM > maxFalsePositives)
				{
					MCDANNDOEIK();
				}
				else
				{
					CDMJCHICMAP = 0;
					FPOIECJBFGG();
				}
			}
			else if (ALDILCFNLBM > 0 && coolDown > 0)
			{
				CDMJCHICMAP++;
				if (CDMJCHICMAP >= coolDown)
				{
					ALDILCFNLBM = 0;
				}
			}
			KJFLOICFFEN = ticks;
		}

		private void FCJDKBEGPEF(UnityAction callback, float DHMGICLCNNA, byte BKMJNLEIGGG, int KBLLGDNEAMO)
		{
			if (EKDNCONELMD)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] Speed Hack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			detectionAction = callback;
			interval = DHMGICLCNNA;
			maxFalsePositives = BKMJNLEIGGG;
			coolDown = KBLLGDNEAMO;
			FPOIECJBFGG();
			ALDILCFNLBM = 0;
			CDMJCHICMAP = 0;
			AKFEAJDLIKF = true;
			EKDNCONELMD = true;
		}

		protected override void LICPBNOFNOB()
		{
			FCJDKBEGPEF(null, interval, maxFalsePositives, coolDown);
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

		private void FPOIECJBFGG()
		{
			BIJLGFLDLAJ = DateTime.UtcNow.Ticks;
			EGENECDHMIC = (long)Environment.TickCount * 10000L;
			CNFAGHGOMGO = BIJLGFLDLAJ;
			KJFLOICFFEN = BIJLGFLDLAJ;
		}
	}
}
