using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Nekki.SF2.GUI
{
	public class TableViewScroll : SFScrollRect
	{
		private TableViewOrientation NDIOMHCLPEP;

		[SerializeField]
		public UnityEvent onDragBegin = new UnityEvent();

		[SerializeField]
		public UnityEvent onDragEnd = new UnityEvent();

		public float AFAJHIFFOGP
		{
			get
			{
				return get_SizeDelta();
			}
			set
			{
				set_SizeDelta(value);
			}
		}

		public float get_Size()
		{
			if (NDIOMHCLPEP == TableViewOrientation.Horizontal)
			{
				return get_content().rect.width;
			}
			return get_content().rect.height;
		}

		public float get_SizeDelta()
		{
			if (NDIOMHCLPEP == TableViewOrientation.Horizontal)
			{
				return get_content().sizeDelta.y;
			}
			return get_content().sizeDelta.x;
		}

		public void set_SizeDelta(float value)
		{
			if (NDIOMHCLPEP == TableViewOrientation.Horizontal)
			{
				get_content().sizeDelta = new Vector2(value, get_content().sizeDelta.y);
			}
			else
			{
				get_content().sizeDelta = new Vector2(get_content().sizeDelta.x, value);
			}
		}

		public void Init()
		{
			OKPMNKIOOOE();
		}

		public void SetOrientation(TableViewOrientation LJHFAPHFGIC)
		{
			NDIOMHCLPEP = LJHFAPHFGIC;
			if (NDIOMHCLPEP == TableViewOrientation.Horizontal)
			{
				get_content().anchorMin = new Vector2(0f, 0f);
				get_content().anchorMax = new Vector2(0f, 1f);
				get_content().pivot = new Vector2(0f, 0.5f);
			}
			else
			{
				get_content().anchorMin = new Vector2(0f, 1f);
				get_content().anchorMax = new Vector2(1f, 1f);
				get_content().pivot = new Vector2(0.5f, 1f);
			}
			set_horizontal(NDIOMHCLPEP == TableViewOrientation.Horizontal);
			set_vertical(!get_horizontal());
		}

		public void SetNormalizedPosition(float HOLFOPDJLFL)
		{
			if (NDIOMHCLPEP == TableViewOrientation.Horizontal)
			{
				set_horizontalNormalizedPosition(HOLFOPDJLFL);
			}
			else
			{
				set_verticalNormalizedPosition(HOLFOPDJLFL);
			}
		}

		private void OKPMNKIOOOE()
		{
			set_content(new GameObject("Table View Content", typeof(RectTransform)).GetComponent<RectTransform>());
			get_content().SetParent(base.gameObject.GetComponent<RectTransform>(), false);
			get_content().offsetMin = Vector2.zero;
			get_content().offsetMax = Vector2.zero;
			get_content().gameObject.AddComponent<NonDrawingGraphic>();
		}

		public override void OnBeginDrag(PointerEventData BHOLFGOGPCP)
		{
			base.OnBeginDrag(BHOLFGOGPCP);
			onDragBegin.Invoke();
		}

		public override void OnEndDrag(PointerEventData BHOLFGOGPCP)
		{
			base.OnEndDrag(BHOLFGOGPCP);
			onDragEnd.Invoke();
		}
	}
}
