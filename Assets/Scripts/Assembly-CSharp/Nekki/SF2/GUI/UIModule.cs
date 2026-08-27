using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	public class UIModule : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private static Action<UIModule> OnModuleActivated;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private static Action<UIModule> OnModuleDeactivated;

		private static List<UIModule> AINKGBEPGIN = new List<UIModule>();

		[SerializeField]
		private int _Order;

		private Canvas GGAPKCADNFJ;

		protected bool NDHHFHHBFEC;

		protected bool CPOMIKGDIEK;

		public bool DCHJDPCEODD
		{
			get
			{
				return get_IsActive();
			}
		}

		public static event Action<UIModule> PGCFMKIAFLP
		{
			add
			{
				add_OnModuleActivated(value);
			}
			remove
			{
				remove_OnModuleActivated(value);
			}
		}

		public static event Action<UIModule> MEEGEAAIBMI
		{
			add
			{
				add_OnModuleDeactivated(value);
			}
			remove
			{
				remove_OnModuleDeactivated(value);
			}
		}

		public static void add_OnModuleActivated(Action<UIModule> value)
		{
			Action<UIModule> action = OnModuleActivated;
			Action<UIModule> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnModuleActivated, (Action<UIModule>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void remove_OnModuleActivated(Action<UIModule> value)
		{
			Action<UIModule> action = OnModuleActivated;
			Action<UIModule> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnModuleActivated, (Action<UIModule>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void add_OnModuleDeactivated(Action<UIModule> value)
		{
			Action<UIModule> action = OnModuleDeactivated;
			Action<UIModule> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnModuleDeactivated, (Action<UIModule>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void remove_OnModuleDeactivated(Action<UIModule> value)
		{
			Action<UIModule> action = OnModuleDeactivated;
			Action<UIModule> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnModuleDeactivated, (Action<UIModule>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static UIModule MountModule(UIModule ILLLNBPALIO, Transform PKHKBAJOHHF, bool CMDIBEFNCOE)
		{
			if (ILLLNBPALIO == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(ILLLNBPALIO.gameObject);
			gameObject.name = ILLLNBPALIO.gameObject.name;
			gameObject.transform.SetParent(PKHKBAJOHHF, false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(false);
			UIModule component = gameObject.GetComponent<UIModule>();
			component.GGAPKCADNFJ = PKHKBAJOHHF.GetComponent<Canvas>();
			if (CMDIBEFNCOE)
			{
				component.Activate();
			}
			return component;
		}

		public static T GetModule<T>() where T : UIModule
		{
			for (int i = 0; i < AINKGBEPGIN.Count; i++)
			{
				if (AINKGBEPGIN[i] is T)
				{
					return AINKGBEPGIN[i] as T;
				}
			}
			return (T)null;
		}

		public static UIModule GetModuleByName(string JLEKBBJBLOE)
		{
			for (int i = 0; i < AINKGBEPGIN.Count; i++)
			{
				if (AINKGBEPGIN[i].name == JLEKBBJBLOE)
				{
					return AINKGBEPGIN[i];
				}
			}
			return null;
		}

		public void Activate()
		{
			base.gameObject.SetActive(true);
			GetComponent<RectTransform>().SetSiblingIndex(_Order);
			if (!NDHHFHHBFEC)
			{
				Init();
			}
			FKEGAGCFPNI();
			CoroutineManager.get_Current().StartCoroutine(EENKOHKBLOC(OnModuleActivated));
		}

		public void DeActivate()
		{
			base.gameObject.SetActive(false);
			FKJHCGLMGLF();
			CoroutineManager.get_Current().StartCoroutine(EENKOHKBLOC(OnModuleDeactivated));
		}

		private IEnumerator EENKOHKBLOC(Action<UIModule> p_event)
		{
			yield return new WaitForEndOfFrame();
			if (p_event != null)
			{
				p_event(this);
			}
		}

		public bool get_IsActive()
		{
			return base.gameObject.activeSelf;
		}

		public void MoveToSceneCanvas()
		{
			base.transform.SetParent(GGAPKCADNFJ.transform, false);
			Activate();
		}

		protected virtual void Init()
		{
			NDHHFHHBFEC = true;
		}

		protected virtual void PJNFHNFLNNO()
		{
			CPOMIKGDIEK = true;
		}

		protected virtual void FKEGAGCFPNI()
		{
		}

		protected virtual void FKJHCGLMGLF()
		{
		}

		private void Awake()
		{
			AINKGBEPGIN.Add(this);
		}

		private void OnDestroy()
		{
			if (!CPOMIKGDIEK)
			{
				PJNFHNFLNNO();
			}
			AINKGBEPGIN.Remove(this);
		}
	}
}
