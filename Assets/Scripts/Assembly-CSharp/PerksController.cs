using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Profile;
using UnityEngine;

public class PerksController : ITableViewDataSource, ITableViewDelegate
{
	private List<ProfilePerkContainer> DJAFAHPFAHN = new List<ProfilePerkContainer>();

	private TableView FEFDHNFOJLF;

	public PerksController(TableView OIDFBEAABBA, GameObject CGLPIDAECLH)
	{
		FEFDHNFOJLF = OIDFBEAABBA;
		HAONJAPEKGB();
		OIDFBEAABBA.set_CellPrefab(CGLPIDAECLH);
		OIDFBEAABBA.Init(this, this);
	}

	public int NumberOfRowsInTableView(TableView OIDFBEAABBA)
	{
		return DJAFAHPFAHN.Count;
	}

	public float SizeForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		return 270f;
	}

	public TableViewCell CellForRowInTableView(TableView OIDFBEAABBA, int BIPGPCAHKIG)
	{
		TableViewCell tableViewCell = OIDFBEAABBA.ReusableCellForRow(BIPGPCAHKIG);
		PerkCell component = tableViewCell.GetComponent<PerkCell>();
		component.RemoveEventListener(0, IDCAGGPAKOB);
		component.AddEventListener(0, IDCAGGPAKOB);
		ProfilePerkContainer iFIEEAGMMMF = DJAFAHPFAHN[BIPGPCAHKIG];
		bool nMBEADHHHFH = BIPGPCAHKIG == 0;
		bool iBMGAPMHMOB = BIPGPCAHKIG + 1 == DJAFAHPFAHN.Count;
		component.Init(iFIEEAGMMMF, BIPGPCAHKIG, nMBEADHHHFH, iBMGAPMHMOB);
		return tableViewCell;
	}

	public void TableViewDidHighlightCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
	}

	public void TableViewDidSelectCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
	}

	private void HAONJAPEKGB()
	{
		DJAFAHPFAHN.Clear();
		DJAFAHPFAHN = PerkTree.GBPBIPFIOJH().KGKJCLDFIHA();
	}

	public void LAJJAAAGDLI(int BIPGPCAHKIG)
	{
		TableViewCell tableViewCell = FEFDHNFOJLF.get_visibleCells().GetCellAtIndex(BIPGPCAHKIG);
		PerkCell component = tableViewCell.GetComponent<PerkCell>();
		ProfilePerkContainer iFIEEAGMMMF = DJAFAHPFAHN[BIPGPCAHKIG];
		bool nMBEADHHHFH = BIPGPCAHKIG == 0;
		bool iBMGAPMHMOB = BIPGPCAHKIG + 1 == DJAFAHPFAHN.Count;
		component.Init(iFIEEAGMMMF, BIPGPCAHKIG, nMBEADHHHFH, iBMGAPMHMOB);
	}

	public void EENODCGBNHC(string name)
	{
		List<ProfilePerk> list = PerkTree.GBPBIPFIOJH().JGCHDCOOGII();
		int num = -1;
		foreach (ProfilePerk item in list)
		{
			if (name == item.KAMBOKLFBEE())
			{
				num = item.PINDEKDNCNL() - 2;
				break;
			}
		}
		if (num > -1)
		{
			FEFDHNFOJLF.ScrollToCell(num);
			PerkCell perkCell = (PerkCell)FEFDHNFOJLF.get_visibleCells().GetCellAtIndex(num);
			perkCell.ChoosePerkByName(name);
		}
	}

	public void IDCAGGPAKOB(object data)
	{
		int iBAKGENOEPH = (int)data;
		FEFDHNFOJLF.ScrollToCell(iBAKGENOEPH, 0.5f);
	}
}
