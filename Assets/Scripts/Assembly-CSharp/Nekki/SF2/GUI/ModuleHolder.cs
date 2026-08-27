using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	public class ModuleHolder : MonoBehaviour
	{
		[SerializeField]
		private List<UIModule> _MountOnStart = new List<UIModule>();

		[SerializeField]
		private List<UIModule> _MountOnLater = new List<UIModule>();

		public List<UIModule> IHMDPCLNOEK
		{
			get
			{
				return get_MountOnStart();
			}
		}

		public List<UIModule> CBMFBELIEPI
		{
			get
			{
				return get_MountOnLater();
			}
		}

		public List<UIModule> get_MountOnStart()
		{
			return _MountOnStart;
		}

		public List<UIModule> get_MountOnLater()
		{
			return _MountOnLater;
		}

		public Canvas GetCanvas()
		{
			return base.gameObject.GetComponent<Canvas>();
		}

		protected virtual void IJDCAJHLJEJ()
		{
		}

		protected virtual bool AAEAEIJJGHA()
		{
			return true;
		}

		protected virtual void Awake()
		{
			IJDCAJHLJEJ();
			KIEDCKJMDLK();
		}

		private void KIEDCKJMDLK()
		{
			foreach (UIModule item in _MountOnStart)
			{
				UIModule.MountModule(item, base.transform, true);
			}
			foreach (UIModule item2 in _MountOnLater)
			{
				UIModule.MountModule(item2, base.transform, false);
			}
		}

		protected virtual void OnDestroy()
		{
		}

		protected T IMDHIBMOAIG<T>() where T : UIModule
		{
			return UIModule.GetModule<T>();
		}

		public UIModule GetModuleByName(string JLEKBBJBLOE)
		{
			return UIModule.GetModuleByName(JLEKBBJBLOE);
		}

		public List<UIModule> ActiveModule()
		{
			List<UIModule> list = new List<UIModule>();
			PODGOONOGHK(_MountOnStart, list);
			PODGOONOGHK(_MountOnLater, list);
			return list;
		}

		protected static void PODGOONOGHK(List<UIModule> NGGBNMCECLM, List<UIModule> AMKKLMOONEP)
		{
			for (int i = 0; i < NGGBNMCECLM.Count; i++)
			{
				if (NGGBNMCECLM[i].gameObject.activeSelf)
				{
					AMKKLMOONEP.Add(NGGBNMCECLM[i]);
				}
			}
		}
	}
}
