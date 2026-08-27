using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class StyleBar : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImageSkew background;

		private List<StyleBarStrip> styleBarStrips = new List<StyleBarStrip>();

		private const float defaultStripValue = 1f;

		public virtual void Init()
		{
		}

		public void Render()
		{
			for (int i = 0; i < styleBarStrips.Count; i++)
			{
				styleBarStrips[i].Render();
			}
		}

		public void AddStrip(string KHPKDMGDMAB, string path = "", float LOGBIEIPCMB = 0f, string KFGFMPJMOAP = "")
		{
			if (KHPKDMGDMAB != null)
			{
				GameObject gameObject = new GameObject();
				if (!KFGFMPJMOAP.Equals(string.Empty))
				{
					gameObject.name = KFGFMPJMOAP;
				}
				StyleBarStrip styleBarStrip = gameObject.AddComponent<StyleBarStrip>();
				styleBarStrip.set_TexturePath(path);
				styleBarStrip.set_SpriteName(KHPKDMGDMAB);
				styleBarStrip.set_SkewAngle(LOGBIEIPCMB);
				styleBarStrip.SetNativeSize();
				AddStrip(styleBarStrip);
			}
		}

		public void AddStripRange(IEnumerable<StyleBarStrip> collection)
		{
			if (collection == null)
			{
				return;
			}
			foreach (StyleBarStrip item in collection)
			{
				AddStrip(item);
			}
		}

		public void AddStrip(StyleBarStrip AFAJEAMJFJH)
		{
			if (!(AFAJEAMJFJH == null))
			{
				AFAJEAMJFJH.transform.SetParent(base.transform, false);
				AFAJEAMJFJH.rectTransform.anchorMin = new Vector2(0f, 0.5f);
				AFAJEAMJFJH.rectTransform.anchorMax = new Vector2(1f, 0.5f);
				AFAJEAMJFJH.Init(1f);
				styleBarStrips.Add(AFAJEAMJFJH);
			}
		}

		public void SetSkewAngle(float LOGBIEIPCMB)
		{
			SetSkewBackground(LOGBIEIPCMB);
			styleBarStrips.ForEach((StyleBarStrip DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.set_SkewAngle(LOGBIEIPCMB);
			});
		}

		public void SetSkewAngle(float LOGBIEIPCMB, int BKCCOEBNFAA)
		{
			if (styleBarStrips.Count > BKCCOEBNFAA)
			{
				styleBarStrips[BKCCOEBNFAA].set_SkewAngle(LOGBIEIPCMB);
			}
		}

		public void SetSkewBackground(float LOGBIEIPCMB)
		{
			if (background != null)
			{
				background.set_SkewAngle(LOGBIEIPCMB);
			}
		}

		public void SetValue(float value, int frames)
		{
			styleBarStrips.ForEach((StyleBarStrip DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.SetValue(value, frames);
			});
		}

		public void SetValue(float value, int frames, int BKCCOEBNFAA)
		{
			if (styleBarStrips.Count > BKCCOEBNFAA)
			{
				styleBarStrips[BKCCOEBNFAA].SetValue(value, frames);
			}
		}

		public float GetValue(int BKCCOEBNFAA)
		{
			if (styleBarStrips.Count > BKCCOEBNFAA)
			{
				return styleBarStrips[BKCCOEBNFAA].fillAmount;
			}
			return 0f;
		}
	}
}
