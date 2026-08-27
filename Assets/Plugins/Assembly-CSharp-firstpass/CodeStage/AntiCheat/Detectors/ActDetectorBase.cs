using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string KCAEFCJMBPA = "Anti-Cheat Toolkit Detectors";

		protected const string EEENHFBCDJE = "Code Stage/Anti-Cheat Toolkit/";

		protected const string LEOHBKLGNNH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		protected static GameObject detectorsContainer;

		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		[SerializeField]
		protected UnityEvent detectionEvent;

		protected UnityAction detectionAction;

		[SerializeField]
		protected bool detectionEventHasListener;

		protected bool EKDNCONELMD;

		protected bool AKFEAJDLIKF;

		private void Start()
		{
			if (detectorsContainer == null && base.gameObject.name == "Anti-Cheat Toolkit Detectors")
			{
				detectorsContainer = base.gameObject;
			}
			if (autoStart && !AKFEAJDLIKF)
			{
				LICPBNOFNOB();
			}
		}

		private void OnEnable()
		{
			if (AKFEAJDLIKF && (detectionEventHasListener || detectionAction != null))
			{
				KLJNEJIEMCN();
			}
		}

		private void OnDisable()
		{
			if (AKFEAJDLIKF)
			{
				HEGJDFPFMII();
			}
		}

		private void OnApplicationQuit()
		{
			HIEIKJFAIJE();
		}

		protected virtual void OnDestroy()
		{
			DJEBEEIELBB();
			if (base.transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				Object.Destroy(base.gameObject);
			}
			else if (base.name == "Anti-Cheat Toolkit Detectors" && GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				Object.Destroy(base.gameObject);
			}
		}

		protected virtual bool Init(ActDetectorBase instance, string JKIBCHLBCBC)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				Debug.LogWarning("[ACTk] " + base.name + ": self-destroying, other instance already exists & only one instance allowed!", base.gameObject);
				Object.Destroy(this);
				return false;
			}
			Object.DontDestroyOnLoad(base.gameObject);
			return true;
		}

		protected virtual void HIEIKJFAIJE()
		{
			Object.Destroy(this);
		}

		internal virtual void MCDANNDOEIK()
		{
			if (detectionAction != null)
			{
				detectionAction();
			}
			if (detectionEventHasListener)
			{
				detectionEvent.Invoke();
			}
			if (autoDispose)
			{
				HIEIKJFAIJE();
			}
			else
			{
				DJEBEEIELBB();
			}
		}

		protected abstract void LICPBNOFNOB();

		protected abstract void DJEBEEIELBB();

		protected abstract void HEGJDFPFMII();

		protected abstract void KLJNEJIEMCN();
	}
}
