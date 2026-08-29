using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using Range = UnityEngine.SocialPlatforms.Range;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	public class TableView : MonoBehaviour, ITableView
	{
		[Serializable]
		public class SelectCellEvent : UnityEvent<TableViewCell>
		{
		}

		[SerializeField]
		private TableViewOrientation tableViewOrientation;

		[SerializeField]
		private float _spacing;

		private float HAHLEDMDEOD;

		private float MINGJPDNIIK;

		[SerializeField]
		private RectOffset padding;

		[SerializeField]
		private bool inertia = true;

		[SerializeField]
		private float elasticity = 0.1f;

		[SerializeField]
		private float scrollSensitivity = 1f;

		[SerializeField]
		private float decelerationRate = 0.135f;

		[SerializeField]
		private bool scrollToHighlighted = true;

		private ITableViewDataSource AAOCIPABPOF;

		private ITableViewDelegate EMMPEMOLNLG;

		private GameObject _cellPrefab;

		private float _currentPosition;

		private float MEKJIGGNMMK;

		private CellSizes BKODOALFJKO;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private VisibleCells HLNOEMFPJEI;

		private ReusableCellsContainer ENLAAAJMNKF;

		private TableViewScroll tableViewScroll;

		private bool LPGLCGMMPHN;

		private bool GIOKDACNHOM;

		private bool LCPBMPKHPOF;

		private Tween _tween;

		private bool BKJCHFPNIIB;

		[SerializeField]
		public SelectCellEvent onSelectCell = new SelectCellEvent();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TableViewCell DFOKDELLBKM;

		[SerializeField]
		private float _MinScrollVelocity;

		public float EPDFGFIACAF
		{
			get
			{
				return get_Spacing();
			}
			set
			{
				set_Spacing(value);
			}
		}

		public ITableViewDataSource JFDKGBHEEOF
		{
			get
			{
				return get_DataSource();
			}
			set
			{
				set_DataSource(value);
			}
		}

		public GameObject AKMFLOHDJJG
		{
			get
			{
				return get_CellPrefab();
			}
			set
			{
				set_CellPrefab(value);
			}
		}

		public Range NKGGFPOGKJC
		{
			get
			{
				return get_VisibleRange();
			}
		}

		public float ELNOAHEFGBL
		{
			get
			{
				return get_ContentSize();
			}
		}

		public float JJCKADKCDIF
		{
			get
			{
				return get_Position();
			}
		}

		public VisibleCells EEDJOBJKDAE
		{
			get
			{
				return get_visibleCells();
			}
			private set
			{
				CPIKBGBHKBG(value);
			}
		}

		public TableViewScroll DCIEBGPFPEB
		{
			get
			{
				return get_Scroll();
			}
		}

		private bool FMLODLPJBJJ
		{
			get
			{
				return BPFHDMABJJM();
			}
		}

		private float EAFCNBIGKJM
		{
			get
			{
				return OKIBKGLCGMG();
			}
		}

		public TableViewCell PLFNODGBFKB
		{
			get
			{
				return get_SelectedCell();
			}
			protected set
			{
				set_SelectedCell(value);
			}
		}

		public float CNBIHHDLDGE
		{
			get
			{
				return get_MinScrollVelocity();
			}
			set
			{
				set_MinScrollVelocity(value);
			}
		}

		public float get_Spacing()
		{
			return _spacing;
		}

		public void set_Spacing(float value)
		{
			_spacing = value;
			if (BKODOALFJKO != null)
			{
				BKODOALFJKO.set_Spacing(_spacing);
			}
		}

		public ITableViewDataSource get_DataSource()
		{
			return AAOCIPABPOF;
		}

		public void set_DataSource(ITableViewDataSource value)
		{
			AAOCIPABPOF = value;
			GIOKDACNHOM = true;
		}

		public ITableViewDelegate Delegate
		{
			get
			{
				return EMMPEMOLNLG;
			}
			set
			{
				EMMPEMOLNLG = value;
			}
		}

		public GameObject get_CellPrefab()
		{
			return _cellPrefab;
		}

		public void set_CellPrefab(GameObject value)
		{
			_cellPrefab = value;
		}

		public Range get_VisibleRange()
		{
			return get_visibleCells().IndexesRange;
		}

		public float get_ContentSize()
		{
			return tableViewScroll.get_Size() - OKIBKGLCGMG();
		}

		public float get_Position()
		{
			return _currentPosition;
		}

		public VisibleCells get_visibleCells()
		{
			return HLNOEMFPJEI;
		}

		private void CPIKBGBHKBG(VisibleCells value)
		{
			HLNOEMFPJEI = value;
		}

		public TableViewScroll get_Scroll()
		{
			return tableViewScroll;
		}

		private bool BPFHDMABJJM()
		{
			return tableViewOrientation == TableViewOrientation.Vertical;
		}

		private float OKIBKGLCGMG()
		{
			Rect rect = (base.transform as RectTransform).rect;
			return (!BPFHDMABJJM()) ? rect.width : rect.height;
		}

		public void Init(ITableViewDataSource PHPCFCPCOAG, ITableViewDelegate CGPBNFFLLDK)
		{
			AAOCIPABPOF = PHPCFCPCOAG;
			EMMPEMOLNLG = CGPBNFFLLDK;
			LPGLCGMMPHN = true;
			BKODOALFJKO = new CellSizes();
			BKODOALFJKO.set_Spacing(_spacing);
			CPIKBGBHKBG(new VisibleCells());
			ENLAAAJMNKF = new ReusableCellsContainer();
			ENLAAAJMNKF.Init();
			tableViewScroll = base.gameObject.AddComponent<TableViewScroll>();
			tableViewScroll.Init();
			tableViewScroll.SetOrientation(tableViewOrientation);
			tableViewScroll.set_elasticity(elasticity);
			tableViewScroll.set_movementType(SFScrollRect.MDMLKCMBBPA.SF2);
			tableViewScroll.set_inertia(inertia);
			tableViewScroll.set_decelerationRate(decelerationRate);
			tableViewScroll.set_scrollSensitivity(scrollSensitivity);
			tableViewScroll.get_onValueChanged().AddListener(ScrollViewValueChanged);
			tableViewScroll.onDragBegin.AddListener(CDILOAACHKK);
			tableViewScroll.onDragEnd.AddListener(CPEGCBHNHLH);
			HAHLEDMDEOD = (int)(OKIBKGLCGMG() / 2f);
			MINGJPDNIIK = (int)(OKIBKGLCGMG() / 2f);
			base.gameObject.AddComponent<RectMask2D>();
			base.gameObject.AddComponent<CanvasRenderer>();
			ReloadData();
		}

		private void Update()
		{
			if (GIOKDACNHOM)
			{
				ReloadData();
			}
			FKCENJCHLBK();
		}

		private void LateUpdate()
		{
			if (LCPBMPKHPOF)
			{
				DMEPFNGHIMN();
			}
		}

		public TableViewCell ReusableCellForRow(int IBAKGENOEPH)
		{
			TableViewCell tableViewCell = ENLAAAJMNKF.CBLMJDCPLCD();
			if (tableViewCell == null)
			{
				tableViewCell = BLMPEOPPMMI(IBAKGENOEPH);
			}
			return tableViewCell;
		}

		public TableViewCell CellForRow(int IBAKGENOEPH)
		{
			return get_visibleCells().GetCellAtIndex(IBAKGENOEPH);
		}

		public float PositionForRow(int IBAKGENOEPH)
		{
			if (IBAKGENOEPH < 0 || IBAKGENOEPH > NumberOfRows() - 1)
			{
				return 0f;
			}
			return BKODOALFJKO.PCKBCFLHKLO(IBAKGENOEPH) - BKODOALFJKO.IEMKAEEOMIH(IBAKGENOEPH) / 2f + HAHLEDMDEOD;
		}

		public void ReloadData()
		{
			MGKPBIEJENL();
			set_SelectedCell(null);
			int num = NumberOfRows();
			BKODOALFJKO.SetRowsCount(num);
			LPGLCGMMPHN = num == 0;
			if (!LPGLCGMMPHN)
			{
				for (int i = 0; i < num; i++)
				{
					float pEEOEOMEBFG = AAOCIPABPOF.SizeForRowInTableView(this, i);
					BKODOALFJKO.KJPFDBAIKAH(pEEOEOMEBFG, i);
				}
				tableViewScroll.set_SizeDelta(HAHLEDMDEOD + BKODOALFJKO.PCKBCFLHKLO(num - 1) + MINGJPDNIIK);
				ELEIODCNFKD();
				GIOKDACNHOM = false;
			}
		}

		public void ScrollToCell(int IBAKGENOEPH, float time = 0f)
		{
			float fFMJGKPCBNK = PositionForRow(IBAKGENOEPH);
			SetPosition(fFMJGKPCBNK, time);
		}

		public void SetPosition(float FFMJGKPCBNK, float time = 0f)
		{
			KillTween();
			if (BKJCHFPNIIB)
			{
				return;
			}
			if (!base.gameObject.activeSelf || time <= 0f)
			{
				SetPosition(FFMJGKPCBNK);
				return;
			}
			_tween = DOTween.To(() => _currentPosition, (float DHDMNHCIPEH) =>
			{
				SetPosition(DHDMNHCIPEH);
			}, FFMJGKPCBNK, time);
		}

		private void SetPosition(float FFMJGKPCBNK)
		{
			if (!LPGLCGMMPHN)
			{
				FFMJGKPCBNK = Mathf.Clamp(FFMJGKPCBNK, PositionForRow(0), PositionForRow(BKODOALFJKO.HGHCEDEOMHA() - 1));
				if (_currentPosition != FFMJGKPCBNK)
				{
					LCPBMPKHPOF = true;
					_currentPosition = FFMJGKPCBNK;
					float num = FFMJGKPCBNK - OKIBKGLCGMG() / 2f;
					float num2 = num / get_ContentSize();
					float num3 = 0f;
					num3 = ((!BPFHDMABJJM()) ? num2 : (1f - num2));
					tableViewScroll.SetNormalizedPosition(num3);
				}
			}
		}

		private void KillTween()
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}
		}

		private TableViewCell BLMPEOPPMMI(int IBAKGENOEPH)
		{
			if (get_CellPrefab() == null)
			{
				return null;
			}
			TableViewCell component = UnityEngine.Object.Instantiate(get_CellPrefab(), tableViewScroll.get_content(), false).GetComponent<TableViewCell>();
			component.set_RowNumber(IBAKGENOEPH);
			return ConfigureCellWithRowAtEnd(component, IBAKGENOEPH, true);
		}

		private void ScrollViewValueChanged(Vector2 PHONDPFNNGF)
		{
			float num = 0f;
			num = ((!BPFHDMABJJM()) ? PHONDPFNNGF.x : (1f - PHONDPFNNGF.y));
			_currentPosition = num * get_ContentSize() + OKIBKGLCGMG() / 2f;
			LCPBMPKHPOF = true;
		}

		private void ELEIODCNFKD()
		{
			MGKPBIEJENL();
			NBFIIJFJJID();
		}

		private void MGKPBIEJENL()
		{
			while (get_visibleCells().OFOPFCJNEBL() > 0)
			{
				MoveCellToReusable(false);
			}
			get_visibleCells().IndexesRange = new Range(0, 0);
		}

		private void LNKCBPPHJCO()
		{
			if (ENLAAAJMNKF != null)
			{
				foreach (TableViewCell item in ENLAAAJMNKF.IGKHHJKCPIJ)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
				ENLAAAJMNKF.IGKHHJKCPIJ.Clear();
			}
			if (get_visibleCells() == null)
			{
				return;
			}
			foreach (KeyValuePair<int, TableViewCell> item2 in get_visibleCells().BFNFADJMAPC())
			{
				UnityEngine.Object.Destroy(item2.Value.gameObject);
			}
			get_visibleCells().IndexesRange = new Range(0, 0);
			get_visibleCells().BFNFADJMAPC().Clear();
		}

		private Range FMLLLBDMIFI()
		{
			float mGMMDGFPBLP = Math.Max(_currentPosition - OKIBKGLCGMG() * 1.5f, PositionForRow(0));
			float mGMMDGFPBLP2 = Math.Min(_currentPosition + OKIBKGLCGMG() * 1.5f, PositionForRow(BKODOALFJKO.HGHCEDEOMHA() - 1));
			int num = FindIndexOfRowAtPosition(mGMMDGFPBLP);
			int num2 = FindIndexOfRowAtPosition(mGMMDGFPBLP2);
			int valueCount = num2 - num + 1;
			return new Range(num, valueCount);
		}

		public int FindIndexOfRowAtPosition(float MGMMDGFPBLP)
		{
			return FindIndexOfRowAtPosition(MGMMDGFPBLP, 0, BKODOALFJKO.HGHCEDEOMHA() - 1);
		}

		public int FindIndexOfRowAtPosition(float MGMMDGFPBLP, int CAILGDNIKJD, int FBGEOOKNPCF)
		{
			if (CAILGDNIKJD >= FBGEOOKNPCF)
			{
				return CAILGDNIKJD;
			}
			if (FBGEOOKNPCF - CAILGDNIKJD == 1)
			{
				float num = Mathf.Abs(MGMMDGFPBLP - PositionForRow(CAILGDNIKJD));
				float num2 = Mathf.Abs(MGMMDGFPBLP - PositionForRow(FBGEOOKNPCF));
				if (num <= num2)
				{
					return CAILGDNIKJD;
				}
				return FBGEOOKNPCF;
			}
			int num3 = (CAILGDNIKJD + FBGEOOKNPCF) / 2;
			float num4 = PositionForRow(num3);
			if (num4 >= MGMMDGFPBLP)
			{
				return FindIndexOfRowAtPosition(MGMMDGFPBLP, CAILGDNIKJD, num3);
			}
			return FindIndexOfRowAtPosition(MGMMDGFPBLP, num3, FBGEOOKNPCF);
		}

		private void NBFIIJFJJID()
		{
			Range bIGCGGHIPIK = FMLLLBDMIFI();
			for (int i = 0; i < bIGCGGHIPIK.count; i++)
			{
				CreateCell(bIGCGGHIPIK.from + i, true);
			}
			get_visibleCells().IndexesRange = bIGCGGHIPIK;
		}

		private void DMEPFNGHIMN()
		{
			LCPBMPKHPOF = false;
			if (!LPGLCGMMPHN && !(Mathf.Abs(_currentPosition - MEKJIGGNMMK) < BKODOALFJKO.IEMKAEEOMIH(0) / 2f + _spacing / 2f))
			{
				MEKJIGGNMMK = _currentPosition;
				Range bIGCGGHIPIK = get_visibleCells().IndexesRange;
				Range range = FMLLLBDMIFI();
				if (range.from > bIGCGGHIPIK.GEMHMCFOIMJ() || range.GEMHMCFOIMJ() < bIGCGGHIPIK.from)
				{
					ELEIODCNFKD();
				}
				else if (!bIGCGGHIPIK.Equals(range))
				{
					PKGMJPNBPIE(bIGCGGHIPIK, range);
					AKDMLIDONIM(bIGCGGHIPIK, range);
					get_visibleCells().IndexesRange = range;
				}
			}
		}

		private void PKGMJPNBPIE(Range IKJKAMKCCMB, Range MHEKHCKHNLG)
		{
			for (int i = IKJKAMKCCMB.from; i < MHEKHCKHNLG.from; i++)
			{
				MoveCellToReusable(false);
			}
			for (int j = MHEKHCKHNLG.GEMHMCFOIMJ(); j < IKJKAMKCCMB.GEMHMCFOIMJ(); j++)
			{
				MoveCellToReusable(true);
			}
		}

		private void AKDMLIDONIM(Range IKJKAMKCCMB, Range MHEKHCKHNLG)
		{
			for (int num = IKJKAMKCCMB.from - 1; num >= MHEKHCKHNLG.from; num--)
			{
				CreateCell(num, false);
			}
			for (int i = IKJKAMKCCMB.GEMHMCFOIMJ() + 1; i <= MHEKHCKHNLG.GEMHMCFOIMJ(); i++)
			{
				CreateCell(i, true);
			}
		}

		private void CreateCell(int IBAKGENOEPH, bool HJIIHCLNCGH)
		{
			TableViewCell hJCPCBLCJJN = AAOCIPABPOF.CellForRowInTableView(this, IBAKGENOEPH);
			hJCPCBLCJJN = ConfigureCellWithRowAtEnd(hJCPCBLCJJN, IBAKGENOEPH, HJIIHCLNCGH);
		}

		private TableViewCell ConfigureCellWithRowAtEnd(TableViewCell HJCPCBLCJJN, int IBAKGENOEPH, bool HJIIHCLNCGH)
		{
			HJCPCBLCJJN.set_RowNumber(IBAKGENOEPH);
			HJCPCBLCJJN.DidHighlightEvent.RemoveListener(KKBDALNMIAB);
			HJCPCBLCJJN.DidHighlightEvent.AddListener(KKBDALNMIAB);
			HJCPCBLCJJN.DidSelectEvent.RemoveListener(LHEJGKCNBAC);
			HJCPCBLCJJN.DidSelectEvent.AddListener(LHEJGKCNBAC);
			get_visibleCells().KLKJONFEGHM(IBAKGENOEPH, HJCPCBLCJJN);
			if (HJIIHCLNCGH)
			{
				HJCPCBLCJJN.transform.SetSiblingIndex(tableViewScroll.get_content().childCount - 1);
			}
			else
			{
				HJCPCBLCJJN.transform.SetSiblingIndex(0);
			}
			if (!BPFHDMABJJM())
			{
				HJCPCBLCJJN.transform.OKHPLHPBPKJ(0f - PositionForRow(IBAKGENOEPH));
			}
			else
			{
				HJCPCBLCJJN.transform.BGNJGIACJBG(0f - PositionForRow(IBAKGENOEPH));
			}
			return HJCPCBLCJJN;
		}

		private void MoveCellToReusable(bool IBMGAPMHMOB)
		{
			int num = ((!IBMGAPMHMOB) ? get_visibleCells().IndexesRange.from : get_visibleCells().IndexesRange.GEMHMCFOIMJ());
			TableViewCell tableViewCell = get_visibleCells().GetCellAtIndex(num);
			tableViewCell.DidHighlightEvent.RemoveAllListeners();
			tableViewCell.DidSelectEvent.RemoveAllListeners();
			ENLAAAJMNKF.FCGLFBFIPON(tableViewCell);
			get_visibleCells().RemoveCellAtIndex(num);
			get_visibleCells().IndexesRange.count--;
			if (!IBMGAPMHMOB)
			{
				get_visibleCells().IndexesRange.from++;
			}
			if (!BPFHDMABJJM())
			{
				tableViewCell.transform.OKHPLHPBPKJ(BKODOALFJKO.IEMKAEEOMIH(num));
			}
			else
			{
				tableViewCell.transform.BGNJGIACJBG(BKODOALFJKO.IEMKAEEOMIH(num));
			}
		}

		private void KKBDALNMIAB(int IBAKGENOEPH)
		{
			if (EMMPEMOLNLG != null)
			{
				EMMPEMOLNLG.TableViewDidHighlightCellForRow(this, IBAKGENOEPH);
			}
			if (!scrollToHighlighted)
			{
			}
		}

		private void LHEJGKCNBAC(int IBAKGENOEPH)
		{
			if (EMMPEMOLNLG != null)
			{
				EMMPEMOLNLG.TableViewDidSelectCellForRow(this, IBAKGENOEPH);
			}
		}

		public TableViewCell get_SelectedCell()
		{
			return DFOKDELLBKM;
		}

		protected void set_SelectedCell(TableViewCell value)
		{
			DFOKDELLBKM = value;
		}

		public float get_MinScrollVelocity()
		{
			return _MinScrollVelocity;
		}

		public void set_MinScrollVelocity(float value)
		{
			_MinScrollVelocity = value;
		}

		private void FKCENJCHLBK()
		{
			if (get_SelectedCell() != null)
			{
				float num = Mathf.Abs(_currentPosition - PositionForRow(get_SelectedCell().get_RowNumber()));
				if (num <= BKODOALFJKO.IEMKAEEOMIH(get_SelectedCell().get_RowNumber()) / 2f + _spacing / 2f)
				{
					return;
				}
			}
			TableViewCell tableViewCell = DLMJIOLOKAI();
			if (tableViewCell != get_SelectedCell())
			{
				set_SelectedCell(tableViewCell);
				onSelectCell.Invoke(get_SelectedCell());
			}
		}

		public int GetCurrentCellRow()
		{
			if (get_SelectedCell() != null)
			{
				return get_SelectedCell().get_RowNumber();
			}
			return 0;
		}

		private TableViewCell DLMJIOLOKAI()
		{
			TableViewCell result = null;
			float num = float.MaxValue;
			foreach (KeyValuePair<int, TableViewCell> item in get_visibleCells().BFNFADJMAPC())
			{
				float num2 = Mathf.Abs(_currentPosition - PositionForRow(item.Key));
				if (num2 < num)
				{
					num = num2;
					result = item.Value;
				}
			}
			return result;
		}

		public int GetNearestCellRow(float GHGLPGGMDNP)
		{
			TableViewCell selectedCell = get_SelectedCell();
			int rowNumber = selectedCell.get_RowNumber();
			if (GHGLPGGMDNP == 0f)
			{
				return rowNumber;
			}
			float num = _currentPosition + GHGLPGGMDNP;
			int result = rowNumber;
			float num2 = PositionForRow(rowNumber);
			if (GHGLPGGMDNP > 0f)
			{
				if (rowNumber == NumberOfRows() - 1)
				{
					return rowNumber;
				}
				for (int i = rowNumber + 1; i < NumberOfRows(); i++)
				{
					float num3 = PositionForRow(i);
					if (Mathf.Abs(num - num3) < Mathf.Abs(num - num2))
					{
						num2 = num3;
						result = i;
						continue;
					}
					break;
				}
			}
			else
			{
				if (rowNumber == 0)
				{
					return rowNumber;
				}
				int num4 = rowNumber - 1;
				while (num4 >= 0)
				{
					float num5 = PositionForRow(num4);
					if (Mathf.Abs(num - num5) < Mathf.Abs(num - num2))
					{
						num2 = num5;
						result = num4;
						num4--;
						continue;
					}
					break;
				}
			}
			return result;
		}

		public int NumberOfRows()
		{
			return AAOCIPABPOF.NumberOfRowsInTableView(this);
		}

		private void CDILOAACHKK()
		{
			BKJCHFPNIIB = true;
			KillTween();
		}

		private void CPEGCBHNHLH()
		{
			BKJCHFPNIIB = false;
			if (Mathf.Abs(tableViewScroll.get_velocity().magnitude) != 0f)
			{
				float num = 0f;
				num = ((!BPFHDMABJJM()) ? (tableViewScroll.get_velocity().x / 2f) : (tableViewScroll.get_velocity().y / 2f));
				int iBAKGENOEPH = get_SelectedCell().get_RowNumber();
				if (Math.Abs(num) >= Math.Abs(get_MinScrollVelocity() / 2f))
				{
					iBAKGENOEPH = GetNearestCellRow(num);
				}
				tableViewScroll.set_velocity(default(Vector2));
				float aFHNFJLOGIC = Mathf.Min(0.5f, Mathf.Abs(Mathf.Ceil(num / tableViewScroll.get_velocity().magnitude)));
				ScrollToCell(iBAKGENOEPH, aFHNFJLOGIC);
			}
		}
	}
}
