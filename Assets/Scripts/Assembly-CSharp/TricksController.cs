using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Profile;
using UnityEngine;

public class TricksController : ITableViewDataSource, ITableViewDelegate
{
	private List<Trick> NDOEDIPFMDP = new List<Trick>();

	private TableView FEFDHNFOJLF;

	public TricksController(TableView OIDFBEAABBA, GameObject CGLPIDAECLH)
	{
		FEFDHNFOJLF = OIDFBEAABBA;
		PHNMANPDPKG();
		OIDFBEAABBA.set_CellPrefab(CGLPIDAECLH);
		OIDFBEAABBA.Init(this, this);
	}

	public int NumberOfRowsInTableView(TableView OIDFBEAABBA)
	{
		return NDOEDIPFMDP.Count;
	}

	public float SizeForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		return 268f;
	}

	public TableViewCell CellForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		Trick kPKPFFGEFGI = NDOEDIPFMDP[IBAKGENOEPH];
		TableViewCell tableViewCell = OIDFBEAABBA.ReusableCellForRow(IBAKGENOEPH);
		TrickCell component = tableViewCell.GetComponent<TrickCell>();
		component.Init(kPKPFFGEFGI, IBAKGENOEPH);
		return tableViewCell;
	}

	public void TableViewDidHighlightCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
	}

	public void TableViewDidSelectCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		FEFDHNFOJLF.ScrollToCell(IBAKGENOEPH, 0.5f);
	}

	private void PHNMANPDPKG()
	{
		NDOEDIPFMDP.Clear();
		NDOEDIPFMDP = GameUtils.KLLGJKHALGH(SceneTypes.SceneProfile);
		NDOEDIPFMDP.Sort((Trick KOOLDHKJHNH, Trick MHFCMOONCHB) => KOOLDHKJHNH.Rank.CompareTo(MHFCMOONCHB.Rank));
	}

	public void LLIMHAHIMML()
	{
		PHNMANPDPKG();
		FEFDHNFOJLF.ReloadData();
	}

	public void FEKDKAPJDCJ(string JGEKHJIHNMF)
	{
		int num = -1;
		int i = 0;
		for (int count = NDOEDIPFMDP.Count; i < count; i++)
		{
			string mENAJEAJJBE = NDOEDIPFMDP[i].Name;
			if (mENAJEAJJBE == JGEKHJIHNMF)
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			FEFDHNFOJLF.ScrollToCell(num);
			ProfileCell profileCell = (ProfileCell)FEFDHNFOJLF.get_visibleCells().GetCellAtIndex(num);
			profileCell.GetFirstIcon().Choose();
		}
	}

	public void ADDALEKEMCD()
	{
		List<string> list = ListSF.CCDKHLAMKKO().AMAELLHKNDJ();
		bool flag = false;
		int num = -1;
		for (int i = 0; i < NDOEDIPFMDP.Count; i++)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (NDOEDIPFMDP[i].Name == list[j])
				{
					num = i;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (num >= 0 && num < NDOEDIPFMDP.Count)
		{
			FEFDHNFOJLF.ScrollToCell(num, 0.5f);
		}
	}
}
