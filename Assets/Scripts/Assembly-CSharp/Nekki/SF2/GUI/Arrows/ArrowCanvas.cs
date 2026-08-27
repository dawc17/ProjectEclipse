using UnityEngine;

namespace Nekki.SF2.GUI.Arrows
{
	public class ArrowCanvas : SFMonoBehaviour<object>
	{
		private static ArrowCanvas _instance;

		[SerializeField]
		private Arrow _arrow;

		public static ArrowCanvas BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static ArrowCanvas get_Instance()
		{
			if (_instance == null)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ArrowCanvas"));
				gameObject.name = "[ArrowCanvas]";
				_instance = gameObject.GetComponent<ArrowCanvas>();
				_instance.Init();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}

		public void Init()
		{
			_arrow.Init();
			HideArrow();
		}

		public void ShowArrow(Vector3 MGMMDGFPBLP)
		{
			_arrow.gameObject.SetActive(true);
			_arrow.transform.position = MGMMDGFPBLP;
		}

		public void HideArrow()
		{
			_arrow.gameObject.SetActive(false);
		}
	}
}
