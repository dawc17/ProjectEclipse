using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using CodeStage.AntiCheat.Detectors;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using UnityEngine;

namespace Nekki.SF2.Core
{
	public class ApplicationController : MonoBehaviour
	{
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> OnPause = delegate
		{
		};

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnUpdate = () =>
		{
		};

		private bool _IsPaused;

		private static ApplicationController _Instance;

		public bool FPJLHEMGNNB
		{
			get
			{
				return get_IsPaused();
			}
			set
			{
				set_IsPaused(value);
			}
		}

		public static ApplicationController BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static event Action<bool> EIPNBNDFEFF
		{
			add
			{
				add_OnPause(value);
			}
			remove
			{
				remove_OnPause(value);
			}
		}

		public static event Action LCNANNAJNGG
		{
			add
			{
				add_OnUpdate(value);
			}
			remove
			{
				remove_OnUpdate(value);
			}
		}

		public static void add_OnPause(Action<bool> value)
		{
			Action<bool> action = OnPause;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPause, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void remove_OnPause(Action<bool> value)
		{
			Action<bool> action = OnPause;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPause, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void add_OnUpdate(Action value)
		{
			Action action = OnUpdate;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUpdate, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void remove_OnUpdate(Action value)
		{
			Action action = OnUpdate;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnUpdate, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public bool get_IsPaused()
		{
			return _IsPaused;
		}

		public void set_IsPaused(bool value)
		{
			_IsPaused = value;
		}

		public static ApplicationController get_Instance()
		{
			Init();
			return _Instance;
		}

		public static void Init()
		{
			if (_Instance == null)
			{
				GameObject gameObject = new GameObject("[ApplicationController]");
				_Instance = gameObject.AddComponent<ApplicationController>();
				ObscuredCheatingDetector.StartDetection(_Instance.GDCNEGNCLPG);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}

		private void Awake()
		{
			Application.targetFrameRate = 60;
			Application.runInBackground = !Application.isMobilePlatform;
			UnityEngine.Debug.LogFormat("Application.installMode={0}", Application.installMode);
			UnityEngine.Debug.LogFormat("Application.installerName={0}", Application.installerName);
		}

		private void OnApplicationPause(bool NBNEAOHDBBI)
		{
			UnityEngine.Debug.Log((!NBNEAOHDBBI) ? "ApplicationController.Resume" : "ApplicationController.Pause");
			_IsPaused = NBNEAOHDBBI;
			OnPause(_IsPaused);
			if (!NBNEAOHDBBI)
			{
				RandomizeObscuredVars();
			}
		}

		private void Update()
		{
			OnUpdate();
		}

		private void RandomizeObscuredVars()
		{
			if (ListSF.CCDKHLAMKKO() != null)
			{
				ListSF.CCDKHLAMKKO().RandomizeObscuredVars();
				ListSF.DJBOFEEKJMP().RandomizeObscuredVars();
				ListSF.ELEBLBJKDBI().RandomizeObscuredVars();
				if (Scene<FightScene>.get_Current() != null)
				{
					Scene<FightScene>.get_Current().RandomizeObscuredVars();
				}
			}
		}

		private void GDCNEGNCLPG()
		{
			Quit();
		}

		public static void Quit()
		{
			Application.Quit();
		}
	}
}
