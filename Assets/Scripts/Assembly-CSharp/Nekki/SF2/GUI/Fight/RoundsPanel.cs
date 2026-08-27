using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class RoundsPanel : MonoBehaviour
	{
		private List<ResolutionImage> _rounds = new List<ResolutionImage>();

		private const string HHEJJFJALHD = "FightUI.Round_Done";

		private const string BPHKOABEFBH = "FightUI.Round_Undone";

		public void Init(int NPLGIKNJBKD)
		{
			Vector2 sizeDelta = default(Vector2);
			for (int i = 0; i < NPLGIKNJBKD; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "Round";
				gameObject.transform.SetParent(base.transform, false);
				ResolutionImageLE resolutionImageLE = gameObject.AddComponent<ResolutionImageLE>();
				resolutionImageLE.set_SpriteName("FightUI.Round_Undone");
				resolutionImageLE.SetNativeSize();
				resolutionImageLE.transform.SetAsFirstSibling();
				_rounds.Add(resolutionImageLE);
				Vector2 sizeDelta2 = resolutionImageLE.rectTransform.sizeDelta;
				sizeDelta.x += sizeDelta2.x;
				if (sizeDelta.y < sizeDelta2.y)
				{
					sizeDelta.y = sizeDelta2.y;
				}
			}
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.sizeDelta = sizeDelta;
			}
		}

		public void UpdateVictories(int MGBIBMMNOJL)
		{
			int num = Mathf.Min(MGBIBMMNOJL, _rounds.Count);
			for (int i = 0; i < num; i++)
			{
				_rounds[i].set_SpriteName("FightUI.Round_Done");
			}
		}
	}
}
