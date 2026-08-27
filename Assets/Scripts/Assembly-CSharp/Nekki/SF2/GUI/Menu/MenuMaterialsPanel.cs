using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuMaterialsPanel : MonoBehaviour
	{
		public enum LOECKBOPFGK
		{
			onMaterialsBtnClicked = 0
		}

		private enum NOBMHDGALLH
		{
			zContent = 0
		}

		[SerializeField]
		private GameObject MaterialPrefab;

		private List<MenuMaterSprite> OHPKMMJMLHE = new List<MenuMaterSprite>();

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init()
		{
			OFLLLFJNDCO();
		}

		private void CHILAIJNEHG()
		{
		}

		public void UpdateView()
		{
			for (int i = 0; i < OHPKMMJMLHE.Count; i++)
			{
				MenuMaterSprite menuMaterSprite = OHPKMMJMLHE[i];
				menuMaterSprite.UpdateView();
			}
		}

		private void OFLLLFJNDCO()
		{
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
			List<GameCurrency> list = GameUtils.AJDKHINLIDI.IIAPDCECFCN();
			if (list.Count == 0)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameCurrency cJJOFMHLFFM = list[i];
				if (cJJOFMHLFFM.NBIHGGLGMCN == GameCurrency.DEFOMBPHMBP.CURRENCY_GROUP_FORGE)
				{
					GameObject gameObject = Object.Instantiate(MaterialPrefab);
					MenuMaterSprite component = gameObject.GetComponent<MenuMaterSprite>();
					// RectTransform.parent preserves world-space scale and position, which
					// makes these UI entries oversized or offset under a scaled Canvas.
					component.transform.SetParent(base.transform, false);
					component.Init(cJJOFMHLFFM);
					OHPKMMJMLHE.Add(component);
				}
			}
		}
	}
}
