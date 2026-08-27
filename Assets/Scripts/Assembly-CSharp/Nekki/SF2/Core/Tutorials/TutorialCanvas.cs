using Nekki.SF2.GUI;
using UnityEngine;

namespace Nekki.SF2.Core.Tutorials
{
	public class TutorialCanvas : SFMonoBehaviour<object>
	{
		private static TutorialCanvas _instance;

		[SerializeField]
		private ResolutionImage _Background;

		private bool _BlockOn;

		public static TutorialCanvas BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public bool IOGKDIPPLMB
		{
			get
			{
				return get_BlockOn();
			}
			set
			{
				set_BlockOn(value);
			}
		}

		public static TutorialCanvas get_Instance()
		{
			if (_instance == null)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/TutorialCanvas"));
				gameObject.name = "[TutorialCanvas]";
				_instance = gameObject.GetComponent<TutorialCanvas>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}

		public bool get_BlockOn()
		{
			return _BlockOn;
		}

		public void set_BlockOn(bool value)
		{
			_BlockOn = value;
			if (_BlockOn)
			{
				JKCGIMBHDPE();
			}
			else
			{
				DBFJLLNBOKE();
			}
		}

		private void Awake()
		{
			_Background.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		private void JKCGIMBHDPE()
		{
			_Background.gameObject.SetActive(true);
		}

		private void DBFJLLNBOKE()
		{
			_Background.gameObject.SetActive(false);
		}
	}
}
