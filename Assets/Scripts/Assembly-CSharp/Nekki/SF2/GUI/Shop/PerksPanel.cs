using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class PerksPanel : MonoBehaviour
	{
		public class KLDEKKBHMNL : UnityEvent<PerkInfoItem, Vector2, Vector2, GameObject>
		{
		}

		public KLDEKKBHMNL onPerksClick = new KLDEKKBHMNL();

		protected string LMABGLLMHKH = "Enchantments.";

		protected Vector2 PDMOLDKOACF = new Vector2(0f, -25f);

		private RectTransform IOAEAJAEOFK;

		public RectTransform DIBDBBCPEGN
		{
			get
			{
				return get_RectTransform();
			}
		}

		public RectTransform get_RectTransform()
		{
			if (IOAEAJAEOFK == null)
			{
				IOAEAJAEOFK = base.transform as RectTransform;
			}
			return IOAEAJAEOFK;
		}

		public virtual void Clear()
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(false);
				list.Add(item.gameObject);
			}
			base.transform.DetachChildren();
			list.ForEach(Object.Destroy);
		}

		public virtual void SetPerks(List<PerkInfoItem> JOGBKOJCINM)
		{
			Clear();
			if (JOGBKOJCINM != null)
			{
				JOGBKOJCINM.ForEach(CreatePerkItem);
			}
		}

		public void CreatePerkItem(PerkInfoItem CBINHDDCIEA)
		{
			if (CBINHDDCIEA != null && CBINHDDCIEA.NHKMCLPOMFK != null && !CBINHDDCIEA.NHKMCLPOMFK.Equals(string.Empty))
			{
				GameObject AOMLCBHAJJH = new GameObject(CBINHDDCIEA.Name);
				ResolutionImage resolutionImage = AOMLCBHAJJH.AddComponent<ResolutionImage>();
				TouchHandler touchHandler = AOMLCBHAJJH.AddComponent<TouchHandler>();
				LayoutElement layoutElement = AOMLCBHAJJH.AddComponent<LayoutElement>();
				touchHandler.transition = Selectable.Transition.None;
				touchHandler.get_OnTouch().AddListener(() =>
				{
					Vector3 position = AOMLCBHAJJH.transform.position;
					onPerksClick.Invoke(CBINHDDCIEA, position, PDMOLDKOACF, AOMLCBHAJJH);
				});
				resolutionImage.set_SpriteName(LMABGLLMHKH + CBINHDDCIEA.NHKMCLPOMFK);
				layoutElement.minHeight = resolutionImage.rectTransform.rect.height;
				layoutElement.minWidth = resolutionImage.rectTransform.rect.width;
				AOMLCBHAJJH.transform.SetParent(base.gameObject.transform, false);
			}
		}

		private void OnDestroy()
		{
			onPerksClick.RemoveAllListeners();
		}
	}
}
