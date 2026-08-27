using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Dialogs
{
	public class DialogCanvasController : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Background;

		[SerializeField]
		private GameObject _SettingsDialogPrefab;

		[SerializeField]
		private GameObject _SettingsAdvancedDialogPrefab;

		[SerializeField]
		private GameObject _StrangerDialogPrefab;

		[SerializeField]
		private GameObject _StoryDialogPrefab;

		[SerializeField]
		private GameObject _SimpleDialogPrefab;

		[SerializeField]
		private GameObject _TradeDialogPrefab;

		[SerializeField]
		private GameObject _NewsDialogPrefab;

		[SerializeField]
		private GameObject _ExitDialogPrefab;

		private Dictionary<Type, GameObject> FOICLKFONKA = new Dictionary<Type, GameObject>();

		private static DialogCanvasController _instance;

		private static Canvas JIGBFKIFFIB;

		public static DialogCanvasController BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static Canvas LHCJAFIHFEC
		{
			get
			{
				return get_DialogsCanvas();
			}
		}

		public static DialogCanvasController get_Instance()
		{
			if (_instance == null)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/Dialogs/DialogCanvas");
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				gameObject.name = "[DialogCanvas]";
				_instance = gameObject.GetComponent<DialogCanvasController>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}

		public static Canvas get_DialogsCanvas()
		{
			return JIGBFKIFFIB;
		}

		private void Awake()
		{
			JIGBFKIFFIB = GetComponent<Canvas>();
			_Background.gameObject.SetActive(false);
			LEBINHFPHKP();
		}

		private void LEBINHFPHKP()
		{
			FOICLKFONKA.Add(typeof(SettingsDialog), _SettingsDialogPrefab);
			FOICLKFONKA.Add(typeof(SettingsAdvancedDialog), _SettingsAdvancedDialogPrefab);
			FOICLKFONKA.Add(typeof(StrangerDialog), _StrangerDialogPrefab);
			FOICLKFONKA.Add(typeof(StoryDialog), _StoryDialogPrefab);
			FOICLKFONKA.Add(typeof(SimpleDialog), _SimpleDialogPrefab);
			FOICLKFONKA.Add(typeof(TradeDialog), _TradeDialogPrefab);
			FOICLKFONKA.Add(typeof(NewsDialog), _NewsDialogPrefab);
			FOICLKFONKA.Add(typeof(ExitDialog), _ExitDialogPrefab);
		}

		private void OnDestroy()
		{
			_instance = null;
			JIGBFKIFFIB = null;
		}

		public void BlockTouches()
		{
			GraphicRaycaster[] collection = UnityEngine.Object.FindObjectsOfType<GraphicRaycaster>();
			List<GraphicRaycaster> list = new List<GraphicRaycaster>(collection);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].enabled = false;
			}
		}

		public void BlockNotDialogTouches()
		{
			BlockTouches();
			base.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
		}

		public void UnBlockTouches()
		{
			GraphicRaycaster[] collection = UnityEngine.Object.FindObjectsOfType<GraphicRaycaster>();
			List<GraphicRaycaster> list = new List<GraphicRaycaster>(collection);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].enabled = true;
			}
		}

		public T CreateDialog<T>() where T : BaseDialog
		{
			Type typeFromHandle = typeof(T);
			if (LOHGCJOBGAE(typeFromHandle) == null)
			{
				LLLOJBFMONN.Error("Dialog prefab is empty! Name=" + typeFromHandle.ToString());
				return (T)null;
			}
			T component = UnityEngine.Object.Instantiate(LOHGCJOBGAE(typeFromHandle)).GetComponent<T>();
			component.transform.SetParent(base.transform, false);
			return component;
		}

		private GameObject LOHGCJOBGAE(Type IGABHEMGKKE)
		{
			if (FOICLKFONKA.ContainsKey(IGABHEMGKKE))
			{
				return FOICLKFONKA[IGABHEMGKKE];
			}
			return null;
		}
	}
}
