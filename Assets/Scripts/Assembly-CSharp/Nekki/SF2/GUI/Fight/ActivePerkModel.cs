using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class ActivePerkModel : MonoBehaviour
	{
		public enum OKCGIBOLEKD
		{
			ActivePerkLeft = 0,
			ActivePerkRight = 1
		}

		[SerializeField]
		private OKCGIBOLEKD _align;

		[SerializeField]
		private GameObject _activePerkItemPrefab;

		[SerializeField]
		private GameObject _activePerkItemContainerPrefab;

		private Quaternion _containerRotation = new Quaternion(0f, 180f, 0f, 0f);

		private Vector2 _spacing = new Vector2(0f, 0f);

		private List<ActivePerkItem> _activePerks = new List<ActivePerkItem>();

		private List<ActivePerkItemContainer> _activePerksContainer = new List<ActivePerkItemContainer>();

		public void Init()
		{
			_spacing = PerkGUI.FEHBEIFACMG();
		}

		public void AddEffectPerk(PerksStage.ActionPerk CKOEFOCPMGK, PerksStage.ActionPerk IBODMPMJELJ)
		{
			PerkActionSetModEffect fBLKPCHKAHM = (PerkActionSetModEffect)IBODMPMJELJ.AMKJNPOCODK;
			PerkActionSetModEffect.COLPJOBKGEI cOLPJOBKGEI = fBLKPCHKAHM.CKEDENENELC();
			foreach (ActivePerkItem item in _activePerks)
			{
				PerksStage.ActionPerk action = item.get_Action();
				if (action == CKOEFOCPMGK && cOLPJOBKGEI == PerkActionSetModEffect.COLPJOBKGEI.EFFECT_PULSE)
				{
					item.set_PulseCount(item.get_PulseCount() + 1);
				}
			}
		}

		public void AddActivePerkItem(PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (IBODMPMJELJ != null)
			{
				MMIOFGHCNFC(IBODMPMJELJ);
			}
		}

		private void MMIOFGHCNFC(PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (_activePerkItemPrefab == null)
			{
				LLLOJBFMONN.Error("ActivePerkModel.CreateActivePerkItem: _activePerkItemPrefab is null");
				return;
			}
			ActivePerkItem component = Object.Instantiate(_activePerkItemPrefab).GetComponent<ActivePerkItem>();
			if (component == null)
			{
				LLLOJBFMONN.Error("ActivePerkModel.CreateActivePerkItem: item is null");
				return;
			}
			component.Init(IBODMPMJELJ);
			string FMHAGIPOIBJ = IBODMPMJELJ.GJONJADIAJM;
			ActivePerkItemContainer activePerkItemContainer = null;
			if (!FMHAGIPOIBJ.Equals(string.Empty))
			{
				activePerkItemContainer = _activePerksContainer.Find((ActivePerkItemContainer DHDMNHCIPEH) => DHDMNHCIPEH.get_Stack().Equals(FMHAGIPOIBJ));
			}
			if (activePerkItemContainer == null)
			{
				if (_activePerkItemContainerPrefab == null)
				{
					LLLOJBFMONN.Error("ActivePerkModel.CreateActivePerkItem: _activePerkItemContainerPrefab is null");
					return;
				}
				activePerkItemContainer = Object.Instantiate(_activePerkItemContainerPrefab).GetComponent<ActivePerkItemContainer>();
				if (activePerkItemContainer == null)
				{
					LLLOJBFMONN.Error("ActivePerkModel.CreateActivePerkItem: itemContainer is null");
					return;
				}
				if (_align == OKCGIBOLEKD.ActivePerkRight)
				{
					activePerkItemContainer.transform.localRotation = _containerRotation;
				}
				activePerkItemContainer.transform.SetParent(base.transform, false);
				activePerkItemContainer.Init(PerkGUI.IKONKNEHCPB(), PerkGUI.FKMDJBBMJFM());
				activePerkItemContainer.set_Stack(FMHAGIPOIBJ);
				_activePerksContainer.Add(activePerkItemContainer);
			}
			activePerkItemContainer.AddActivePerk(component);
			_activePerks.Add(component);
		}

		public void RemoveActivePerkItem(PerksStage.ActionPerk IBODMPMJELJ)
		{
			if (IBODMPMJELJ == null)
			{
				return;
			}
			foreach (ActivePerkItem item in _activePerks)
			{
				if (item.get_Action() == IBODMPMJELJ)
				{
					item.set_Show(false);
					break;
				}
			}
		}

		public void RemoveAllActivePerkItem()
		{
			_activePerks.ForEach((ActivePerkItem DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.set_Show(false);
			});
		}

		public void DestroyAllPerkItems()
		{
			_activePerks.ForEach((ActivePerkItem DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.Destroy();
			});
			_activePerks.Clear();
			_activePerksContainer.ForEach((ActivePerkItemContainer DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.Destroy();
			});
			_activePerksContainer.Clear();
		}

		private void AAFJILIOPBG()
		{
			float num = 0f;
			float x = _spacing.x;
			foreach (ActivePerkItemContainer item in _activePerksContainer)
			{
				item.set_FinishPosX(num);
				num += x;
				RectTransform rectTransform = item.transform as RectTransform;
				if (rectTransform != null)
				{
					num += rectTransform.rect.width * rectTransform.localScale.x * 0.5f;
				}
			}
		}

		public void Render()
		{
			for (int i = 0; i < _activePerksContainer.Count; i++)
			{
				ActivePerkItemContainer activePerkItemContainer = _activePerksContainer[i];
				activePerkItemContainer.Render();
				if (activePerkItemContainer.get_NeedDelete())
				{
					_activePerksContainer.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < _activePerks.Count; j++)
			{
				if (_activePerks[j].get_NeedDelete())
				{
					_activePerks.RemoveAt(j);
					j--;
				}
			}
			AAFJILIOPBG();
		}
	}
}
