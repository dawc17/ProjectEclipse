using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI/Extensions/TextPic")]
	[ExecuteInEditMode]
	public class TextPic : Text, IPointerClickHandler, IEventSystemHandler, ISelectHandler, IPointerExitHandler, IPointerEnterHandler
	{
		[Serializable]
		public struct IconName
		{
			public string name;

			public Sprite sprite;
		}

		[Serializable]
		public class HrefClickEvent : UnityEvent<string>
		{
		}

		private class LLHOOOEJICC
		{
			public int CAILGDNIKJD;

			public int FBGEOOKNPCF;

			public string name;

			public readonly List<Rect> EGOEJCBNDIJ = new List<Rect>();
		}

		private readonly List<ResolutionImage> FFCOKBMOANO = new List<ResolutionImage>();

		private readonly List<GameObject> ECKINFOPDPD = new List<GameObject>();

		private bool JJLEJDCGDNL;

		private UnityEngine.Object IDADFIBEOIA = new UnityEngine.Object();

		private readonly List<int> PHPKKMKLNOA = new List<int>();

		private static readonly Regex BPNAJICGHEH = new Regex("<quad name=(.+?) size=(\\d*\\.?\\d+%?) width=(\\d*\\.?\\d+%?) />", RegexOptions.Singleline);

		private string HBGCAGPIGIJ;

		private string AIBLJIIKGIG;

		public IconName[] inspectorIconList;

		private Dictionary<string, Sprite> BJMDJPDPEDH = new Dictionary<string, Sprite>();

		public float ImageScalingFactor = 0.5f;

		public string hyperlinkColor = "blue";

		[SerializeField]
		public Vector2 imageOffset = Vector2.zero;

		private Button KLNKEPMAGKF;

		private List<Vector2> MDBELBGHDFP = new List<Vector2>();

		private string IKDLGPNGKOO = string.Empty;

		public bool isCreating_m_HrefInfos = true;

		private readonly List<LLHOOOEJICC> GGAGBFDGNNP = new List<LLHOOOEJICC>();

		private static readonly StringBuilder GCCFOHDGODJ = new StringBuilder();

		private static readonly Regex LCHAAEKKODP = new Regex("<a href=([^>\\n\\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

		[SerializeField]
		private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

		public HrefClickEvent GGBFIMGFIAG
		{
			get
			{
				return get_onHrefClick();
			}
			set
			{
				set_onHrefClick(value);
			}
		}

		public override void SetVerticesDirty()
		{
			base.SetVerticesDirty();
			EOODLGNHCEB();
		}

		private new void Start()
		{
			KLNKEPMAGKF = GetComponent<Button>();
			if (inspectorIconList != null && inspectorIconList.Length > 0)
			{
				IconName[] array = inspectorIconList;
				for (int i = 0; i < array.Length; i++)
				{
					IconName iconName = array[i];
					BJMDJPDPEDH.Add(iconName.name, iconName.sprite);
				}
			}
			MNMOJPJEFFB();
		}

		protected void EOODLGNHCEB()
		{
			AIBLJIIKGIG = OABONMHJBEE();
			PHPKKMKLNOA.Clear();
			foreach (Match item2 in BPNAJICGHEH.Matches(AIBLJIIKGIG))
			{
				int index = item2.Index;
				int item = index * 4 + 3;
				PHPKKMKLNOA.Add(item);
				FFCOKBMOANO.RemoveAll((ResolutionImage KHPKDMGDMAB) => KHPKDMGDMAB == null);
				if (FFCOKBMOANO.Count == 0)
				{
					GetComponentsInChildren(FFCOKBMOANO);
				}
				if (PHPKKMKLNOA.Count > FFCOKBMOANO.Count)
				{
					GameObject gameObject = new GameObject("ResolutionImage");
					ResolutionImage resolutionImage = gameObject.AddComponent<ResolutionImage>();
					resolutionImage.raycastTarget = false;
					gameObject.layer = base.gameObject.layer;
					gameObject.layer = base.gameObject.layer;
					RectTransform rectTransform = gameObject.transform as RectTransform;
					if ((bool)rectTransform)
					{
						rectTransform.SetParent(base.rectTransform);
						rectTransform.localPosition = Vector3.zero;
						rectTransform.localRotation = Quaternion.identity;
						rectTransform.localScale = Vector3.one;
						rectTransform.pivot = Vector2.zero;
					}
					FFCOKBMOANO.Add(resolutionImage);
				}
				string value = item2.Groups[1].Value;
				float num = float.Parse(item2.Groups[2].Value);
				ResolutionImage resolutionImage2 = FFCOKBMOANO[PHPKKMKLNOA.Count - 1];
				if (resolutionImage2.sprite == null || resolutionImage2.sprite.name != value)
				{
					resolutionImage2.set_SpriteName(value);
				}
				resolutionImage2.rectTransform.sizeDelta = new Vector2(num, num * resolutionImage2.sprite.rect.height / resolutionImage2.sprite.rect.width);
				resolutionImage2.enabled = true;
				if (MDBELBGHDFP.Count == FFCOKBMOANO.Count)
				{
					resolutionImage2.transform.OKHPLHPBPKJ(MDBELBGHDFP[PHPKKMKLNOA.Count - 1].x);
					resolutionImage2.transform.BGNJGIACJBG(MDBELBGHDFP[PHPKKMKLNOA.Count - 1].y - resolutionImage2.rectTransform.rect.height / 2f + (float)(base.fontSize / 4));
				}
			}
			for (int num2 = PHPKKMKLNOA.Count; num2 < FFCOKBMOANO.Count; num2++)
			{
				if ((bool)FFCOKBMOANO[num2])
				{
					FFCOKBMOANO[num2].gameObject.SetActive(false);
					FFCOKBMOANO[num2].gameObject.hideFlags = HideFlags.HideAndDontSave;
					ECKINFOPDPD.Add(FFCOKBMOANO[num2].gameObject);
					FFCOKBMOANO.Remove(FFCOKBMOANO[num2]);
				}
			}
			if (ECKINFOPDPD.Count > 1)
			{
				JJLEJDCGDNL = true;
			}
		}

		protected override void OnPopulateMesh(VertexHelper EMOHIIMOAAL)
		{
			string text = m_Text;
			m_Text = AIBLJIIKGIG;
			base.OnPopulateMesh(EMOHIIMOAAL);
			m_Text = text;
			MDBELBGHDFP.Clear();
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < PHPKKMKLNOA.Count; i++)
			{
				int num = PHPKKMKLNOA[i];
				RectTransform rectTransform = FFCOKBMOANO[i].rectTransform;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				if (num < EMOHIIMOAAL.currentVertCount)
				{
					EMOHIIMOAAL.PopulateUIVertex(ref vertex, num);
					MDBELBGHDFP.Add(vertex.position);
					EMOHIIMOAAL.PopulateUIVertex(ref vertex, num - 3);
					Vector3 position = vertex.position;
					int num2 = num;
					int num3 = num - 3;
					while (num2 > num3)
					{
						EMOHIIMOAAL.PopulateUIVertex(ref vertex, num);
						vertex.position = position;
						EMOHIIMOAAL.SetUIVertex(vertex, num2);
						num2--;
					}
				}
			}
			if (PHPKKMKLNOA.Count != 0)
			{
				PHPKKMKLNOA.Clear();
			}
			foreach (LLHOOOEJICC item in GGAGBFDGNNP)
			{
				item.EGOEJCBNDIJ.Clear();
				if (item.CAILGDNIKJD >= EMOHIIMOAAL.currentVertCount)
				{
					continue;
				}
				EMOHIIMOAAL.PopulateUIVertex(ref vertex, item.CAILGDNIKJD);
				Vector3 position2 = vertex.position;
				Bounds bounds = new Bounds(position2, Vector3.zero);
				int j = item.CAILGDNIKJD;
				for (int fBGEOOKNPCF = item.FBGEOOKNPCF; j < fBGEOOKNPCF && j < EMOHIIMOAAL.currentVertCount; j++)
				{
					EMOHIIMOAAL.PopulateUIVertex(ref vertex, j);
					position2 = vertex.position;
					if (position2.x < bounds.min.x)
					{
						item.EGOEJCBNDIJ.Add(new Rect(bounds.min, bounds.size));
						bounds = new Bounds(position2, Vector3.zero);
					}
					else
					{
						bounds.Encapsulate(position2);
					}
				}
				item.EGOEJCBNDIJ.Add(new Rect(bounds.min, bounds.size));
			}
			EOODLGNHCEB();
		}

		public HrefClickEvent get_onHrefClick()
		{
			return m_OnHrefClick;
		}

		public void set_onHrefClick(HrefClickEvent value)
		{
			m_OnHrefClick = value;
		}

		protected string OABONMHJBEE()
		{
			GCCFOHDGODJ.Length = 0;
			int num = 0;
			HBGCAGPIGIJ = text;
			if (inspectorIconList != null && inspectorIconList.Length > 0)
			{
				IconName[] array = inspectorIconList;
				for (int i = 0; i < array.Length; i++)
				{
					IconName iconName = array[i];
					if (iconName.name != null && iconName.name != string.Empty)
					{
						HBGCAGPIGIJ = HBGCAGPIGIJ.Replace(iconName.name, "<quad name=" + iconName.name + " size=" + base.fontSize + " width=1 />");
					}
				}
			}
			int num2 = 0;
			foreach (Match item2 in LCHAAEKKODP.Matches(HBGCAGPIGIJ))
			{
				GCCFOHDGODJ.Append(HBGCAGPIGIJ.Substring(num, item2.Index - num));
				GCCFOHDGODJ.Append("<color=" + hyperlinkColor + ">");
				Group obj = item2.Groups[1];
				if (isCreating_m_HrefInfos)
				{
					LLHOOOEJICC lLHOOOEJICC = new LLHOOOEJICC();
					lLHOOOEJICC.CAILGDNIKJD = GCCFOHDGODJ.Length * 4;
					lLHOOOEJICC.FBGEOOKNPCF = (GCCFOHDGODJ.Length + item2.Groups[2].Length - 1) * 4 + 3;
					lLHOOOEJICC.name = obj.Value;
					LLHOOOEJICC item = lLHOOOEJICC;
					GGAGBFDGNNP.Add(item);
				}
				else if (GGAGBFDGNNP.Count > 0)
				{
					GGAGBFDGNNP[num2].CAILGDNIKJD = GCCFOHDGODJ.Length * 4;
					GGAGBFDGNNP[num2].FBGEOOKNPCF = (GCCFOHDGODJ.Length + item2.Groups[2].Length - 1) * 4 + 3;
					num2++;
				}
				GCCFOHDGODJ.Append(item2.Groups[2].Value);
				GCCFOHDGODJ.Append("</color>");
				num = item2.Index + item2.Length;
			}
			if (isCreating_m_HrefInfos)
			{
				isCreating_m_HrefInfos = false;
			}
			GCCFOHDGODJ.Append(HBGCAGPIGIJ.Substring(num, HBGCAGPIGIJ.Length - num));
			return GCCFOHDGODJ.ToString();
		}

		public void OnPointerClick(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint);
			foreach (LLHOOOEJICC item in GGAGBFDGNNP)
			{
				List<Rect> eGOEJCBNDIJ = item.EGOEJCBNDIJ;
				for (int i = 0; i < eGOEJCBNDIJ.Count; i++)
				{
					if (eGOEJCBNDIJ[i].Contains(localPoint))
					{
						m_OnHrefClick.Invoke(item.name);
						return;
					}
				}
			}
		}

		public void OnPointerEnter(PointerEventData BHOLFGOGPCP)
		{
			if (FFCOKBMOANO.Count < 1)
			{
				return;
			}
			foreach (ResolutionImage item in FFCOKBMOANO)
			{
				if (KLNKEPMAGKF != null && !KLNKEPMAGKF.isActiveAndEnabled)
				{
				}
			}
		}

		public void OnPointerExit(PointerEventData BHOLFGOGPCP)
		{
			if (FFCOKBMOANO.Count < 1)
			{
				return;
			}
			foreach (ResolutionImage item in FFCOKBMOANO)
			{
				if (KLNKEPMAGKF != null && !KLNKEPMAGKF.isActiveAndEnabled)
				{
				}
			}
		}

		public void OnSelect(BaseEventData BHOLFGOGPCP)
		{
			if (FFCOKBMOANO.Count < 1)
			{
				return;
			}
			foreach (ResolutionImage item in FFCOKBMOANO)
			{
				if (KLNKEPMAGKF != null && !KLNKEPMAGKF.isActiveAndEnabled)
				{
				}
			}
		}

		private void Update()
		{
			lock (IDADFIBEOIA)
			{
				if (JJLEJDCGDNL)
				{
					for (int i = 0; i < ECKINFOPDPD.Count; i++)
					{
						UnityEngine.Object.DestroyImmediate(ECKINFOPDPD[i]);
					}
					ECKINFOPDPD.Clear();
					JJLEJDCGDNL = false;
				}
			}
			if (IKDLGPNGKOO != text)
			{
				MNMOJPJEFFB();
			}
		}

		private void MNMOJPJEFFB()
		{
			IKDLGPNGKOO = text;
			GGAGBFDGNNP.Clear();
			isCreating_m_HrefInfos = true;
		}
	}
}
