using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class MapButtonsPanel : MonoBehaviour
	{
		[SerializeField]
		private Transform _actionsPanel;

		[SerializeField]
		private GameObject _mapButtonPrefab;

		private List<MapButton> _buttons = new List<MapButton>();
		private bool _storyButtonsVisible = true;

		public void Init()
		{
			MapButtonController.ELEBLBJKDBI().AddEventListener(0, CFBPBNDGJGH);
			MapButtonController.ELEBLBJKDBI().AddEventListener(1, BJJMOMAKCGI);
			AddButtons();
		}

		private void OnDestroy()
		{
			MapButtonController.ELEBLBJKDBI().RemoveEventListener(0, CFBPBNDGJGH);
			MapButtonController.ELEBLBJKDBI().RemoveEventListener(1, BJJMOMAKCGI);
		}

		private void CFBPBNDGJGH(MapButtonInfo KLNKEPMAGKF)
		{
			if (KLNKEPMAGKF != null && MapButtonController.ELEBLBJKDBI().OHGCDIHFJIJ(KLNKEPMAGKF))
			{
				AddButton(KLNKEPMAGKF);
			}
		}

		private void BJJMOMAKCGI(MapButtonInfo KLNKEPMAGKF)
		{
			if (KLNKEPMAGKF != null)
			{
				RemoveButton(KLNKEPMAGKF);
			}
		}

		public void AddButtons()
		{
			List<MapButtonInfo> list = MapButtonController.ELEBLBJKDBI().MEPCBPIJLGB();
			foreach (MapButtonInfo item in list)
			{
				if (item != null)
				{
					AddButton(item);
				}
			}
		}

		public void AddButton(MapButtonInfo KLNKEPMAGKF)
		{
			Transform kPAICOOKACB = ((!KLNKEPMAGKF.NEOIMNAHLAN || !(_actionsPanel != null)) ? base.transform : _actionsPanel);
			AddButton(KLNKEPMAGKF, kPAICOOKACB);
		}

		public void AddButton(MapButtonInfo DJDNMAOEFBD, Transform KPAICOOKACB)
		{
			if (_mapButtonPrefab != null)
			{
				MapButton component = Object.Instantiate(_mapButtonPrefab).GetComponent<MapButton>();
				component.gameObject.SetActive(true);
				component.transform.SetParent(KPAICOOKACB, false);
				component.Init(DJDNMAOEFBD);
				component.gameObject.SetActive(_storyButtonsVisible || DJDNMAOEFBD.EDMILHNJFAA() != MapButtonInfo.HNEJAKIGDBA.Story);
				_buttons.Add(component);
			}
		}

		public void RemoveButtons()
		{
			foreach (MapButton item in _buttons)
			{
				RemoveButton(item);
			}
			_buttons.Clear();
		}

		public void SetStoryButtonsVisible(bool visible)
		{
			_storyButtonsVisible = visible;
			foreach (MapButton button in _buttons)
			{
				MapButtonInfo info = button.get_MapButtonInfo();
				bool storyOnly = info != null && info.EDMILHNJFAA() == MapButtonInfo.HNEJAKIGDBA.Story;
				button.gameObject.SetActive(visible || !storyOnly);
			}
		}

		public void RemoveButton(MapButtonInfo DJDNMAOEFBD)
		{
			MapButton mapButton = _buttons.Find((MapButton DHDMNHCIPEH) => DHDMNHCIPEH.get_MapButtonInfo() == DJDNMAOEFBD);
			if (mapButton != null)
			{
				RemoveButton(mapButton);
			}
		}

		public void RemoveButton(MapButton KLNKEPMAGKF)
		{
			KLNKEPMAGKF.gameObject.SetActive(false);
			Object.Destroy(KLNKEPMAGKF.gameObject);
		}
	}
}
