using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI/Scroll Rect", 37)]
	[SelectionBase]
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class SFScrollRect : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup, ILayoutController
	{
		public enum MDMLKCMBBPA
		{
			Unrestricted = 0,
			Elastic = 1,
			Clamped = 2,
			SF2 = 3
		}

		public enum JJDKHMPDLNC
		{
			Permanent = 0,
			AutoHide = 1,
			AutoHideAndExpandViewport = 2
		}

		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}

		[SerializeField]
		private RectTransform m_Content;

		[SerializeField]
		private bool m_Horizontal = true;

		[SerializeField]
		private bool m_Vertical = true;

		[SerializeField]
		private MDMLKCMBBPA m_MovementType = MDMLKCMBBPA.Elastic;

		[SerializeField]
		private float m_Elasticity = 0.1f;

		[SerializeField]
		private bool m_Inertia = true;

		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		[SerializeField]
		private RectTransform m_Viewport;

		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		[SerializeField]
		private JJDKHMPDLNC m_HorizontalScrollbarVisibility;

		[SerializeField]
		private JJDKHMPDLNC m_VerticalScrollbarVisibility;

		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		[SerializeField]
		private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();

		private Vector2 KPFNKIFLDPL = Vector2.zero;

		private Vector2 IDMLKGACFNO = Vector2.zero;

		private RectTransform KMNHEEBLKGA;

		private Bounds GOIIEBHDLOH;

		private Bounds ALIFOBAFPOB;

		private Vector2 BKHKAOENMIG;

		private bool HDDEPEAELAM;

		private Vector2 JEMEHMDEEHA = Vector2.zero;

		private Bounds GMBMMKGKCJO;

		private Bounds AGCBMBCFDPE;

		[NonSerialized]
		private bool OHJMNCLNPFD;

		private bool HPLLDNIDIML;

		private bool FPOANPLBGGC;

		private float LPOOHGINOEF;

		private float NAFHJBACPDE;

		[NonSerialized]
		private RectTransform HMJHIFIEKOD;

		private RectTransform PNMDBLOPDHA;

		private RectTransform KPFFEDDJNOE;

		private DrivenRectTransformTracker m_Tracker;

		[SerializeField]
		private float m_ScrollFactor = 1f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float CEEACGHNLAE;

		private readonly Vector3[] m_Corners = new Vector3[4];

		public RectTransform DMNBDBJNKME
		{
			get
			{
				return get_content();
			}
			set
			{
				set_content(value);
			}
		}

		public bool NNLLKCNOADD
		{
			get
			{
				return get_horizontal();
			}
			set
			{
				set_horizontal(value);
			}
		}

		public bool BFAPABIBDFB
		{
			get
			{
				return get_vertical();
			}
			set
			{
				set_vertical(value);
			}
		}

		public MDMLKCMBBPA DICDMCPHKMO
		{
			get
			{
				return get_movementType();
			}
			set
			{
				set_movementType(value);
			}
		}

		public float JCIOHDIOAGP
		{
			get
			{
				return get_elasticity();
			}
			set
			{
				set_elasticity(value);
			}
		}

		public bool JCIMNPMFFBK
		{
			get
			{
				return get_inertia();
			}
			set
			{
				set_inertia(value);
			}
		}

		public float CGDBGPDOODH
		{
			get
			{
				return get_decelerationRate();
			}
			set
			{
				set_decelerationRate(value);
			}
		}

		public float CFHDPILGGGO
		{
			get
			{
				return get_scrollSensitivity();
			}
			set
			{
				set_scrollSensitivity(value);
			}
		}

		public RectTransform NOPKELAGEJK
		{
			get
			{
				return get_viewport();
			}
			set
			{
				set_viewport(value);
			}
		}

		public Scrollbar LBOAGOLPGPM
		{
			get
			{
				return get_horizontalScrollbar();
			}
			set
			{
				set_horizontalScrollbar(value);
			}
		}

		public Scrollbar DAJECJJGKIL
		{
			get
			{
				return get_verticalScrollbar();
			}
			set
			{
				set_verticalScrollbar(value);
			}
		}

		public JJDKHMPDLNC LBENGNMGLMC
		{
			get
			{
				return get_horizontalScrollbarVisibility();
			}
			set
			{
				set_horizontalScrollbarVisibility(value);
			}
		}

		public JJDKHMPDLNC GBBALIKILDM
		{
			get
			{
				return get_verticalScrollbarVisibility();
			}
			set
			{
				set_verticalScrollbarVisibility(value);
			}
		}

		public float MNOAFNGEGFN
		{
			get
			{
				return get_horizontalScrollbarSpacing();
			}
			set
			{
				set_horizontalScrollbarSpacing(value);
			}
		}

		public float HNBGEEFNPPK
		{
			get
			{
				return get_verticalScrollbarSpacing();
			}
			set
			{
				set_verticalScrollbarSpacing(value);
			}
		}

		public ScrollRectEvent PIKCOEPBAJN
		{
			get
			{
				return get_onValueChanged();
			}
			set
			{
				set_onValueChanged(value);
			}
		}

		protected RectTransform GPPCFOEEGEL
		{
			get
			{
				return BIOPKLNFEJI();
			}
		}

		public Vector2 BLPIMOCGMKJ
		{
			get
			{
				return get_velocity();
			}
			set
			{
				set_velocity(value);
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				return FDHIFJPGOIC();
			}
		}

		public float OLMPCHENNHJ
		{
			get
			{
				return get_scrollFactor();
			}
			set
			{
				set_scrollFactor(value);
			}
		}

		public Vector2 HOLFOPDJLFL
		{
			get
			{
				return get_normalizedPosition();
			}
			set
			{
				set_normalizedPosition(value);
			}
		}

		public float AFDHGNNLHHC
		{
			get
			{
				return get_horizontalNormalizedPosition();
			}
			set
			{
				set_horizontalNormalizedPosition(value);
			}
		}

		public float JJNGHDNMAKL
		{
			get
			{
				return get_verticalNormalizedPosition();
			}
			set
			{
				set_verticalNormalizedPosition(value);
			}
		}

		private bool CDMDGDPHMMK
		{
			get
			{
				return PGFAIAPPGJA();
			}
		}

		private bool HIHFMPDCMLL
		{
			get
			{
				return KEHLLKKMOLB();
			}
		}

		public virtual float IIMDMHKPJJN
		{
			get
			{
				return minWidth;
			}
		}

		public virtual float KFKBHBDJBLK
		{
			get
			{
				return preferredWidth;
			}
		}

		public virtual float FOLGLLLJPCP
		{
			get
			{
				return flexibleWidth;
			}
			private set
			{
				EFFGGDHEDCM(value);
			}
		}

		public virtual float JKKFHOLODHB
		{
			get
			{
				return minHeight;
			}
		}

		public virtual float AMCFFDNMFFG
		{
			get
			{
				return preferredHeight;
			}
		}

		public virtual float IPPGGFEPJMA
		{
			get
			{
				return flexibleHeight;
			}
		}

		public virtual int BIGNCGFHNAK
		{
			get
			{
				return layoutPriority;
			}
		}

		Transform ICanvasElement.transform
		{
			get
			{
				return base.transform;
			}
		}

		protected SFScrollRect()
		{
			EFFGGDHEDCM(-1f);
		}

		public RectTransform get_content()
		{
			return m_Content;
		}

		public void set_content(RectTransform value)
		{
			m_Content = value;
		}

		public bool get_horizontal()
		{
			return m_Horizontal;
		}

		public void set_horizontal(bool value)
		{
			m_Horizontal = value;
		}

		public bool get_vertical()
		{
			return m_Vertical;
		}

		public void set_vertical(bool value)
		{
			m_Vertical = value;
		}

		public MDMLKCMBBPA get_movementType()
		{
			return m_MovementType;
		}

		public void set_movementType(MDMLKCMBBPA value)
		{
			m_MovementType = value;
		}

		public float get_elasticity()
		{
			return m_Elasticity;
		}

		public void set_elasticity(float value)
		{
			m_Elasticity = value;
		}

		public bool get_inertia()
		{
			return m_Inertia;
		}

		public void set_inertia(bool value)
		{
			m_Inertia = value;
		}

		public float get_decelerationRate()
		{
			return m_DecelerationRate;
		}

		public void set_decelerationRate(float value)
		{
			m_DecelerationRate = value;
		}

		public float get_scrollSensitivity()
		{
			return m_ScrollSensitivity;
		}

		public void set_scrollSensitivity(float value)
		{
			m_ScrollSensitivity = value;
		}

		public RectTransform get_viewport()
		{
			return m_Viewport;
		}

		public void set_viewport(RectTransform value)
		{
			m_Viewport = value;
			KKMKEKGJLMJ();
		}

		public Scrollbar get_horizontalScrollbar()
		{
			return m_HorizontalScrollbar;
		}

		public void set_horizontalScrollbar(Scrollbar value)
		{
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.RemoveListener(PGGKPMBDFHH);
			}
			m_HorizontalScrollbar = value;
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.AddListener(PGGKPMBDFHH);
			}
			KKMKEKGJLMJ();
		}

		public Scrollbar get_verticalScrollbar()
		{
			return m_VerticalScrollbar;
		}

		public void set_verticalScrollbar(Scrollbar value)
		{
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.RemoveListener(GGONALKGFCG);
			}
			m_VerticalScrollbar = value;
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.AddListener(GGONALKGFCG);
			}
			KKMKEKGJLMJ();
		}

		public JJDKHMPDLNC get_horizontalScrollbarVisibility()
		{
			return m_HorizontalScrollbarVisibility;
		}

		public void set_horizontalScrollbarVisibility(JJDKHMPDLNC value)
		{
			m_HorizontalScrollbarVisibility = value;
			KKMKEKGJLMJ();
		}

		public JJDKHMPDLNC get_verticalScrollbarVisibility()
		{
			return m_VerticalScrollbarVisibility;
		}

		public void set_verticalScrollbarVisibility(JJDKHMPDLNC value)
		{
			m_VerticalScrollbarVisibility = value;
			KKMKEKGJLMJ();
		}

		public float get_horizontalScrollbarSpacing()
		{
			return m_HorizontalScrollbarSpacing;
		}

		public void set_horizontalScrollbarSpacing(float value)
		{
			m_HorizontalScrollbarSpacing = value;
			FIBKLPHOCFC();
		}

		public float get_verticalScrollbarSpacing()
		{
			return m_VerticalScrollbarSpacing;
		}

		public void set_verticalScrollbarSpacing(float value)
		{
			m_VerticalScrollbarSpacing = value;
			FIBKLPHOCFC();
		}

		public ScrollRectEvent get_onValueChanged()
		{
			return m_OnValueChanged;
		}

		public void set_onValueChanged(ScrollRectEvent value)
		{
			m_OnValueChanged = value;
		}

		protected RectTransform BIOPKLNFEJI()
		{
			if (KMNHEEBLKGA == null)
			{
				KMNHEEBLKGA = m_Viewport;
			}
			if (KMNHEEBLKGA == null)
			{
				KMNHEEBLKGA = (RectTransform)base.transform;
			}
			return KMNHEEBLKGA;
		}

		public Vector2 get_velocity()
		{
			return BKHKAOENMIG;
		}

		public void set_velocity(Vector2 value)
		{
			BKHKAOENMIG = value;
		}

		private RectTransform FDHIFJPGOIC()
		{
			if (HMJHIFIEKOD == null)
			{
				HMJHIFIEKOD = GetComponent<RectTransform>();
			}
			return HMJHIFIEKOD;
		}

		public float get_scrollFactor()
		{
			return m_ScrollFactor;
		}

		public void set_scrollFactor(float value)
		{
			m_ScrollFactor = value;
		}

		public virtual void Rebuild(CanvasUpdate FLAKOEEDOAF)
		{
			if (FLAKOEEDOAF == CanvasUpdate.Prelayout)
			{
				KOAEDAIOJIM();
			}
			if (FLAKOEEDOAF == CanvasUpdate.PostLayout)
			{
				JKFDJGELEID();
				LOGFHHOFJAH(Vector2.zero);
				PPBLONJPJEA();
				OHJMNCLNPFD = true;
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void KOAEDAIOJIM()
		{
			Transform transform = base.transform;
			PNMDBLOPDHA = ((!(m_HorizontalScrollbar == null)) ? (m_HorizontalScrollbar.transform as RectTransform) : null);
			KPFFEDDJNOE = ((!(m_VerticalScrollbar == null)) ? (m_VerticalScrollbar.transform as RectTransform) : null);
			bool flag = BIOPKLNFEJI().parent == transform;
			bool flag2 = !PNMDBLOPDHA || PNMDBLOPDHA.parent == transform;
			bool flag3 = !KPFFEDDJNOE || KPFFEDDJNOE.parent == transform;
			bool flag4 = flag && flag2 && flag3;
			HPLLDNIDIML = flag4 && (bool)PNMDBLOPDHA && get_horizontalScrollbarVisibility() == JJDKHMPDLNC.AutoHideAndExpandViewport;
			FPOANPLBGGC = flag4 && (bool)KPFFEDDJNOE && get_verticalScrollbarVisibility() == JJDKHMPDLNC.AutoHideAndExpandViewport;
			LPOOHGINOEF = ((!(PNMDBLOPDHA == null)) ? PNMDBLOPDHA.rect.height : 0f);
			NAFHJBACPDE = ((!(KPFFEDDJNOE == null)) ? KPFFEDDJNOE.rect.width : 0f);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.AddListener(PGGKPMBDFHH);
			}
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.AddListener(GGONALKGFCG);
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		protected override void OnDisable()
		{
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if ((bool)m_HorizontalScrollbar)
			{
				m_HorizontalScrollbar.onValueChanged.RemoveListener(PGGKPMBDFHH);
			}
			if ((bool)m_VerticalScrollbar)
			{
				m_VerticalScrollbar.onValueChanged.RemoveListener(GGONALKGFCG);
			}
			OHJMNCLNPFD = false;
			m_Tracker.Clear();
			BKHKAOENMIG = Vector2.zero;
			LayoutRebuilder.MarkLayoutForRebuild(FDHIFJPGOIC());
			base.OnDisable();
		}

		public override bool IsActive()
		{
			return base.IsActive() && m_Content != null;
		}

		private void DKAMBKOGFML()
		{
			if (!OHJMNCLNPFD && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		public virtual void StopMovement()
		{
			BKHKAOENMIG = Vector2.zero;
		}

		public virtual void OnScroll(PointerEventData data)
		{
			if (!IsActive())
			{
				return;
			}
			DKAMBKOGFML();
			JKFDJGELEID();
			Vector2 scrollDelta = data.scrollDelta;
			scrollDelta.y *= -1f;
			if (get_vertical() && !get_horizontal())
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (get_horizontal() && !get_vertical())
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 anchoredPosition = m_Content.anchoredPosition;
			anchoredPosition += scrollDelta * m_ScrollSensitivity;
			if (m_MovementType == MDMLKCMBBPA.Clamped)
			{
				anchoredPosition += CalculateOffset(anchoredPosition - m_Content.anchoredPosition);
			}
			IKIMIDOGICB(anchoredPosition);
			JKFDJGELEID();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData BHOLFGOGPCP)
		{
			if (BHOLFGOGPCP.button == PointerEventData.InputButton.Left)
			{
				BKHKAOENMIG = Vector2.zero;
			}
		}

		public virtual void OnBeginDrag(PointerEventData BHOLFGOGPCP)
		{
			if (BHOLFGOGPCP.button == PointerEventData.InputButton.Left && IsActive())
			{
				JKFDJGELEID();
				KPFNKIFLDPL = Vector2.zero;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(BIOPKLNFEJI(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out KPFNKIFLDPL);
				IDMLKGACFNO = m_Content.anchoredPosition;
				HDDEPEAELAM = true;
			}
		}

		public virtual void OnEndDrag(PointerEventData BHOLFGOGPCP)
		{
			if (BHOLFGOGPCP.button == PointerEventData.InputButton.Left)
			{
				HDDEPEAELAM = false;
			}
		}

		public virtual void OnDrag(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			if (BHOLFGOGPCP.button != PointerEventData.InputButton.Left || !IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(BIOPKLNFEJI(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint))
			{
				return;
			}
			JKFDJGELEID();
			Vector2 vector = localPoint - KPFNKIFLDPL;
			vector *= m_ScrollFactor;
			Vector2 vector2 = IDMLKGACFNO + vector;
			Vector2 vector3 = CalculateOffset(vector2 - m_Content.anchoredPosition);
			vector2 += vector3;
			if (m_MovementType == MDMLKCMBBPA.Elastic)
			{
				if (vector3.x != 0f)
				{
					vector2.x -= IIIMENBNAKB(vector3.x, ALIFOBAFPOB.size.x);
				}
				if (vector3.y != 0f)
				{
					vector2.y -= IIIMENBNAKB(vector3.y, ALIFOBAFPOB.size.y);
				}
			}
			IKIMIDOGICB(vector2);
		}

		protected virtual void IKIMIDOGICB(Vector2 MGMMDGFPBLP)
		{
			if (!m_Horizontal)
			{
				MGMMDGFPBLP.x = m_Content.anchoredPosition.x;
			}
			if (!m_Vertical)
			{
				MGMMDGFPBLP.y = m_Content.anchoredPosition.y;
			}
			if (MGMMDGFPBLP != m_Content.anchoredPosition)
			{
				m_Content.anchoredPosition = MGMMDGFPBLP;
				JKFDJGELEID();
			}
		}

		protected virtual void LateUpdate()
		{
			if (!m_Content)
			{
				return;
			}
			DKAMBKOGFML();
			NFOKADBDILL();
			JKFDJGELEID();
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Vector2 vector = CalculateOffset(Vector2.zero);
			if (!HDDEPEAELAM && (vector != Vector2.zero || BKHKAOENMIG != Vector2.zero) && m_MovementType != MDMLKCMBBPA.SF2)
			{
				Vector2 anchoredPosition = m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (m_MovementType == MDMLKCMBBPA.Elastic && vector[i] != 0f)
					{
						float currentVelocity = BKHKAOENMIG[i];
						anchoredPosition[i] = Mathf.SmoothDamp(m_Content.anchoredPosition[i], m_Content.anchoredPosition[i] + vector[i], ref currentVelocity, m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
						BKHKAOENMIG[i] = currentVelocity;
					}
					else if (m_Inertia)
					{
						BKHKAOENMIG[i] *= Mathf.Pow(m_DecelerationRate, unscaledDeltaTime);
						if (Mathf.Abs(BKHKAOENMIG[i]) < 1f)
						{
							BKHKAOENMIG[i] = 0f;
						}
						anchoredPosition[i] += BKHKAOENMIG[i] * unscaledDeltaTime;
					}
					else
					{
						BKHKAOENMIG[i] = 0f;
					}
				}
				if (BKHKAOENMIG != Vector2.zero)
				{
					if (m_MovementType == MDMLKCMBBPA.Clamped)
					{
						vector = CalculateOffset(anchoredPosition - m_Content.anchoredPosition);
						anchoredPosition += vector;
					}
					IKIMIDOGICB(anchoredPosition);
				}
			}
			if (HDDEPEAELAM && m_Inertia)
			{
				Vector3 b = (m_Content.anchoredPosition - JEMEHMDEEHA) / unscaledDeltaTime;
				BKHKAOENMIG = Vector3.Lerp(BKHKAOENMIG, b, unscaledDeltaTime * 10f);
			}
			if (ALIFOBAFPOB != AGCBMBCFDPE || GOIIEBHDLOH != GMBMMKGKCJO || m_Content.anchoredPosition != JEMEHMDEEHA)
			{
				LOGFHHOFJAH(vector);
				m_OnValueChanged.Invoke(get_normalizedPosition());
				PPBLONJPJEA();
			}
		}

		private void PPBLONJPJEA()
		{
			if (m_Content == null)
			{
				JEMEHMDEEHA = Vector2.zero;
			}
			else
			{
				JEMEHMDEEHA = m_Content.anchoredPosition;
			}
			AGCBMBCFDPE = ALIFOBAFPOB;
			GMBMMKGKCJO = GOIIEBHDLOH;
		}

		private void LOGFHHOFJAH(Vector2 IPCOBJBKNAO)
		{
			if ((bool)m_HorizontalScrollbar)
			{
				if (GOIIEBHDLOH.size.x > 0f)
				{
					m_HorizontalScrollbar.size = Mathf.Clamp01((ALIFOBAFPOB.size.x - Mathf.Abs(IPCOBJBKNAO.x)) / GOIIEBHDLOH.size.x);
				}
				else
				{
					m_HorizontalScrollbar.size = 1f;
				}
				m_HorizontalScrollbar.value = get_horizontalNormalizedPosition();
			}
			if ((bool)m_VerticalScrollbar)
			{
				if (GOIIEBHDLOH.size.y > 0f)
				{
					m_VerticalScrollbar.size = Mathf.Clamp01((ALIFOBAFPOB.size.y - Mathf.Abs(IPCOBJBKNAO.y)) / GOIIEBHDLOH.size.y);
				}
				else
				{
					m_VerticalScrollbar.size = 1f;
				}
				m_VerticalScrollbar.value = get_verticalNormalizedPosition();
			}
		}

		public Vector2 get_normalizedPosition()
		{
			return new Vector2(get_horizontalNormalizedPosition(), get_verticalNormalizedPosition());
		}

		public void set_normalizedPosition(Vector2 value)
		{
			NNBELLIECIA(value.x, 0);
			NNBELLIECIA(value.y, 1);
		}

		public float get_horizontalNormalizedPosition()
		{
			JKFDJGELEID();
			if (GOIIEBHDLOH.size.x <= ALIFOBAFPOB.size.x)
			{
				return (ALIFOBAFPOB.min.x > GOIIEBHDLOH.min.x) ? 1 : 0;
			}
			return (ALIFOBAFPOB.min.x - GOIIEBHDLOH.min.x) / (GOIIEBHDLOH.size.x - ALIFOBAFPOB.size.x);
		}

		public void set_horizontalNormalizedPosition(float value)
		{
			NNBELLIECIA(value, 0);
		}

		public float get_verticalNormalizedPosition()
		{
			JKFDJGELEID();
			if (GOIIEBHDLOH.size.y <= ALIFOBAFPOB.size.y)
			{
				return (ALIFOBAFPOB.min.y > GOIIEBHDLOH.min.y) ? 1 : 0;
			}
			return (ALIFOBAFPOB.min.y - GOIIEBHDLOH.min.y) / (GOIIEBHDLOH.size.y - ALIFOBAFPOB.size.y);
		}

		public void set_verticalNormalizedPosition(float value)
		{
			NNBELLIECIA(value, 1);
		}

		private void PGGKPMBDFHH(float value)
		{
			NNBELLIECIA(value, 0);
		}

		private void GGONALKGFCG(float value)
		{
			NNBELLIECIA(value, 1);
		}

		private void NNBELLIECIA(float value, int NMADGDHJBGB)
		{
			DKAMBKOGFML();
			JKFDJGELEID();
			float num = GOIIEBHDLOH.size[NMADGDHJBGB] - ALIFOBAFPOB.size[NMADGDHJBGB];
			float num2 = ALIFOBAFPOB.min[NMADGDHJBGB] - value * num;
			float num3 = m_Content.localPosition[NMADGDHJBGB] + num2 - GOIIEBHDLOH.min[NMADGDHJBGB];
			Vector3 localPosition = m_Content.localPosition;
			if (Mathf.Abs(localPosition[NMADGDHJBGB] - num3) > 0.01f)
			{
				localPosition[NMADGDHJBGB] = num3;
				m_Content.localPosition = localPosition;
				BKHKAOENMIG[NMADGDHJBGB] = 0f;
				JKFDJGELEID();
			}
		}

		private static float IIIMENBNAKB(float LLKABJBFHKJ, float HHOGGNHEFEG)
		{
			return (1f - 1f / (Mathf.Abs(LLKABJBFHKJ) * 0.55f / HHOGGNHEFEG + 1f)) * HHOGGNHEFEG * Mathf.Sign(LLKABJBFHKJ);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			FIBKLPHOCFC();
		}

		private bool PGFAIAPPGJA()
		{
			if (Application.isPlaying)
			{
				return GOIIEBHDLOH.size.x > ALIFOBAFPOB.size.x + 0.01f;
			}
			return true;
		}

		private bool KEHLLKKMOLB()
		{
			if (Application.isPlaying)
			{
				return GOIIEBHDLOH.size.y > ALIFOBAFPOB.size.y + 0.01f;
			}
			return true;
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual float minWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return CEEACGHNLAE;
			}
		}

		private void EFFGGDHEDCM(float value)
		{
			CEEACGHNLAE = value;
		}

		public virtual float minHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return -1;
			}
		}

		public virtual void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			if (HPLLDNIDIML || FPOANPLBGGC)
			{
				m_Tracker.Add(this, BIOPKLNFEJI(), DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);
				BIOPKLNFEJI().anchorMin = Vector2.zero;
				BIOPKLNFEJI().anchorMax = Vector2.one;
				BIOPKLNFEJI().sizeDelta = Vector2.zero;
				BIOPKLNFEJI().anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(get_content());
				ALIFOBAFPOB = new Bounds(BIOPKLNFEJI().rect.center, BIOPKLNFEJI().rect.size);
				GOIIEBHDLOH = BFJHGFIGKAJ();
			}
			if (FPOANPLBGGC && KEHLLKKMOLB())
			{
				BIOPKLNFEJI().sizeDelta = new Vector2(0f - (NAFHJBACPDE + m_VerticalScrollbarSpacing), BIOPKLNFEJI().sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(get_content());
				ALIFOBAFPOB = new Bounds(BIOPKLNFEJI().rect.center, BIOPKLNFEJI().rect.size);
				GOIIEBHDLOH = BFJHGFIGKAJ();
			}
			if (HPLLDNIDIML && PGFAIAPPGJA())
			{
				BIOPKLNFEJI().sizeDelta = new Vector2(BIOPKLNFEJI().sizeDelta.x, 0f - (LPOOHGINOEF + m_HorizontalScrollbarSpacing));
				ALIFOBAFPOB = new Bounds(BIOPKLNFEJI().rect.center, BIOPKLNFEJI().rect.size);
				GOIIEBHDLOH = BFJHGFIGKAJ();
			}
			if (FPOANPLBGGC && KEHLLKKMOLB() && BIOPKLNFEJI().sizeDelta.x == 0f && BIOPKLNFEJI().sizeDelta.y < 0f)
			{
				BIOPKLNFEJI().sizeDelta = new Vector2(0f - (NAFHJBACPDE + m_VerticalScrollbarSpacing), BIOPKLNFEJI().sizeDelta.y);
			}
		}

		public virtual void SetLayoutVertical()
		{
			DFNHCAAKAKO();
			ALIFOBAFPOB = new Bounds(BIOPKLNFEJI().rect.center, BIOPKLNFEJI().rect.size);
			GOIIEBHDLOH = BFJHGFIGKAJ();
		}

		private void NFOKADBDILL()
		{
			if ((bool)m_VerticalScrollbar && m_VerticalScrollbarVisibility != JJDKHMPDLNC.Permanent && m_VerticalScrollbar.gameObject.activeSelf != KEHLLKKMOLB())
			{
				m_VerticalScrollbar.gameObject.SetActive(KEHLLKKMOLB());
			}
			if ((bool)m_HorizontalScrollbar && m_HorizontalScrollbarVisibility != JJDKHMPDLNC.Permanent && m_HorizontalScrollbar.gameObject.activeSelf != PGFAIAPPGJA())
			{
				m_HorizontalScrollbar.gameObject.SetActive(PGFAIAPPGJA());
			}
		}

		private void DFNHCAAKAKO()
		{
			if (FPOANPLBGGC && (bool)m_HorizontalScrollbar)
			{
				m_Tracker.Add(this, PNMDBLOPDHA, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				PNMDBLOPDHA.anchorMin = new Vector2(0f, PNMDBLOPDHA.anchorMin.y);
				PNMDBLOPDHA.anchorMax = new Vector2(1f, PNMDBLOPDHA.anchorMax.y);
				PNMDBLOPDHA.anchoredPosition = new Vector2(0f, PNMDBLOPDHA.anchoredPosition.y);
				if (KEHLLKKMOLB())
				{
					PNMDBLOPDHA.sizeDelta = new Vector2(0f - (NAFHJBACPDE + m_VerticalScrollbarSpacing), PNMDBLOPDHA.sizeDelta.y);
				}
				else
				{
					PNMDBLOPDHA.sizeDelta = new Vector2(0f, PNMDBLOPDHA.sizeDelta.y);
				}
			}
			if (HPLLDNIDIML && (bool)m_VerticalScrollbar)
			{
				m_Tracker.Add(this, KPFFEDDJNOE, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				KPFFEDDJNOE.anchorMin = new Vector2(KPFFEDDJNOE.anchorMin.x, 0f);
				KPFFEDDJNOE.anchorMax = new Vector2(KPFFEDDJNOE.anchorMax.x, 1f);
				KPFFEDDJNOE.anchoredPosition = new Vector2(KPFFEDDJNOE.anchoredPosition.x, 0f);
				if (PGFAIAPPGJA())
				{
					KPFFEDDJNOE.sizeDelta = new Vector2(KPFFEDDJNOE.sizeDelta.x, 0f - (LPOOHGINOEF + m_HorizontalScrollbarSpacing));
				}
				else
				{
					KPFFEDDJNOE.sizeDelta = new Vector2(KPFFEDDJNOE.sizeDelta.x, 0f);
				}
			}
		}

		private void JKFDJGELEID()
		{
			ALIFOBAFPOB = new Bounds(BIOPKLNFEJI().rect.center, BIOPKLNFEJI().rect.size);
			GOIIEBHDLOH = BFJHGFIGKAJ();
			if (!(m_Content == null))
			{
				Vector3 size = GOIIEBHDLOH.size;
				Vector3 center = GOIIEBHDLOH.center;
				Vector3 vector = ALIFOBAFPOB.size - size;
				if (vector.x > 0f)
				{
					center.x -= vector.x * (m_Content.pivot.x - 0.5f);
					size.x = ALIFOBAFPOB.size.x;
				}
				if (vector.y > 0f)
				{
					center.y -= vector.y * (m_Content.pivot.y - 0.5f);
					size.y = ALIFOBAFPOB.size.y;
				}
				GOIIEBHDLOH.size = size;
				GOIIEBHDLOH.center = center;
			}
		}

		private Bounds BFJHGFIGKAJ()
		{
			if (m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = BIOPKLNFEJI().worldToLocalMatrix;
			m_Content.GetWorldCorners(m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		private Vector2 CalculateOffset(Vector2 FOIPKLDNGDL)
		{
			Vector2 zero = Vector2.zero;
			if (m_MovementType == MDMLKCMBBPA.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = GOIIEBHDLOH.min;
			Vector2 vector2 = GOIIEBHDLOH.max;
			if (m_Horizontal)
			{
				vector.x += FOIPKLDNGDL.x;
				vector2.x += FOIPKLDNGDL.x;
				if (vector.x > ALIFOBAFPOB.min.x)
				{
					zero.x = ALIFOBAFPOB.min.x - vector.x;
				}
				else if (vector2.x < ALIFOBAFPOB.max.x)
				{
					zero.x = ALIFOBAFPOB.max.x - vector2.x;
				}
			}
			if (m_Vertical)
			{
				vector.y += FOIPKLDNGDL.y;
				vector2.y += FOIPKLDNGDL.y;
				if (vector2.y < ALIFOBAFPOB.max.y)
				{
					zero.y = ALIFOBAFPOB.max.y - vector2.y;
				}
				else if (vector.y > ALIFOBAFPOB.min.y)
				{
					zero.y = ALIFOBAFPOB.min.y - vector.y;
				}
			}
			return zero;
		}

		protected void FIBKLPHOCFC()
		{
			if (IsActive())
			{
				LayoutRebuilder.MarkLayoutForRebuild(FDHIFJPGOIC());
			}
		}

		protected void KKMKEKGJLMJ()
		{
			if (IsActive())
			{
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
				LayoutRebuilder.MarkLayoutForRebuild(FDHIFJPGOIC());
			}
		}

		bool ICanvasElement.IsDestroyed()
		{
			return IsDestroyed();
		}
	}
}
