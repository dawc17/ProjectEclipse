using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Shop;
using UnityEngine;

public class SealsController : ITableViewDataSource, ITableViewDelegate
{
	private const int ILLBMINOHCF = 4;

	private TableView FEFDHNFOJLF;

	private List<UserItem> ICNPIFCMLLC = new List<UserItem>();

	public SealsController(TableView OIDFBEAABBA, GameObject CGLPIDAECLH)
	{
		FEFDHNFOJLF = OIDFBEAABBA;
		NFDKBNIMBOF();
		OIDFBEAABBA.set_CellPrefab(CGLPIDAECLH);
		OIDFBEAABBA.Init(this, this);
	}

	public int NumberOfRowsInTableView(TableView OIDFBEAABBA)
	{
		return ICNPIFCMLLC.Count;
	}

	public float SizeForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		return 632f;
	}

	public TableViewCell CellForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		TableViewCell tableViewCell = OIDFBEAABBA.ReusableCellForRow(IBAKGENOEPH);
		ShopTableViewCell component = tableViewCell.GetComponent<ShopTableViewCell>();
		component.set_BaseSize(Constants.SEAL_SIZE);
		component.set_IconPanelActive(false);
		ItemInfo itemInfo = ICNPIFCMLLC[IBAKGENOEPH].BHKHOJPANHE();
		component.SetItemInfo(itemInfo);
		return tableViewCell;
	}

	public void TableViewDidHighlightCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
	}

	public void TableViewDidSelectCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		FEFDHNFOJLF.ScrollToCell(IBAKGENOEPH, 0.5f);
	}

	private void NFDKBNIMBOF()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		List<UserItem> list = nKGLHEGIKKP.KHCNHPCPFII().HOPBBLJLHOB("Seal", string.Empty);
		foreach (UserItem item in list)
		{
			if (item.OFOPFCJNEBL() != 0)
			{
				ICNPIFCMLLC.Add(item);
			}
		}
	}

	public void KCAAFPNBEGL(string EOIDIMBBLFB)
	{
		int num = -1;
		int i = 0;
		for (int count = ICNPIFCMLLC.Count; i < count; i++)
		{
			string text = ICNPIFCMLLC[i].get_Name();
			if (text == EOIDIMBBLFB)
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			FEFDHNFOJLF.ScrollToCell(num);
		}
	}
}
