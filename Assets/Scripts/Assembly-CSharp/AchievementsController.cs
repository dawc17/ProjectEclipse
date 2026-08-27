using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Profile;
using UnityEngine;

public class AchievementsController : ITableViewDataSource, ITableViewDelegate
{
	private TableView FEFDHNFOJLF;

	private List<global::Pair<Achievement, int>> GJEPILABGDO = new List<global::Pair<Achievement, int>>();

	public AchievementsController(TableView OIDFBEAABBA, GameObject CGLPIDAECLH)
	{
		FEFDHNFOJLF = OIDFBEAABBA;
		LCKFANCIHJB();
		OIDFBEAABBA.set_CellPrefab(CGLPIDAECLH);
		OIDFBEAABBA.Init(this, this);
	}

	public int NumberOfRowsInTableView(TableView OIDFBEAABBA)
	{
		return GJEPILABGDO.Count;
	}

	public float SizeForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		return 268f;
	}

	public TableViewCell CellForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		Achievement lLHEDBIEHAA = GJEPILABGDO[IBAKGENOEPH].First;
		TableViewCell tableViewCell = OIDFBEAABBA.ReusableCellForRow(IBAKGENOEPH);
		AchievementCell component = tableViewCell.GetComponent<AchievementCell>();
		component.Init(lLHEDBIEHAA, GJEPILABGDO[IBAKGENOEPH].Second, IBAKGENOEPH);
		return tableViewCell;
	}

	public void TableViewDidHighlightCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
	}

	public void TableViewDidSelectCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
	{
		FEFDHNFOJLF.ScrollToCell(IBAKGENOEPH, 0.5f);
	}

	private void LCKFANCIHJB()
	{
		GJEPILABGDO.Clear();
		List<AchievCounter> mDNKEAFGAOB = GameUtils.HHLEKNNJGMJ.MDNKEAFGAOB;
		List<RosterAchievCounter> list = new List<RosterAchievCounter>(ListSF.CCDKHLAMKKO().KJNPJKEHGLE().HOBHAAAEELG());
		List<RosterAchievement> eOJAMHMPKAJ = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().NOJKMMJJPHF();
		for (int i = 0; i < mDNKEAFGAOB.Count; i++)
		{
			string mENAJEAJJBE = mDNKEAFGAOB[i].Name;
			int ePJGLECOIBG = 0;
			List<Achievement> pGAGNLJABIE = new List<Achievement>(mDNKEAFGAOB[i].FOICCCGPCMJ);
			for (int j = 0; j < list.Count; j++)
			{
				RosterAchievCounter cKJBHGKBPPM = list[j];
				if (mENAJEAJJBE == cKJBHGKBPPM.get_Name())
				{
					ePJGLECOIBG = cKJBHGKBPPM.MCIPEJBLIDC();
					list.RemoveAt(j);
					break;
				}
			}
			DIKFALENELA(pGAGNLJABIE, eOJAMHMPKAJ, ePJGLECOIBG);
		}
	}

	private void DIKFALENELA(List<Achievement> PGAGNLJABIE, List<RosterAchievement> EOJAMHMPKAJ, int EPJGLECOIBG)
	{
		for (int i = 0; i < EOJAMHMPKAJ.Count; i++)
		{
			string text = EOJAMHMPKAJ[i].get_Name();
			for (int j = 0; j < PGAGNLJABIE.Count; j++)
			{
				Achievement jNPIOKEKMII = PGAGNLJABIE[j];
				if ((!jNPIOKEKMII.GDCBBAHKCIE || jNPIOKEKMII.HGMHEOGJDMM) && text == jNPIOKEKMII.Name)
				{
					GJEPILABGDO.Add(new global::Pair<Achievement, int>(jNPIOKEKMII, jNPIOKEKMII.EOGLBDCLMBM));
					PGAGNLJABIE.RemoveAt(j);
					break;
				}
			}
		}
		for (int k = 0; k < PGAGNLJABIE.Count; k++)
		{
			if (!PGAGNLJABIE[k].GDCBBAHKCIE || PGAGNLJABIE[k].HGMHEOGJDMM)
			{
				GJEPILABGDO.Add(new global::Pair<Achievement, int>(PGAGNLJABIE[k], EPJGLECOIBG));
				if (EPJGLECOIBG < PGAGNLJABIE[k].EOGLBDCLMBM)
				{
					break;
				}
			}
		}
	}

	public bool IGGFLBOBGLN()
	{
		for (int i = 0; i < GJEPILABGDO.Count; i++)
		{
			if (GJEPILABGDO[i].First.DBHJGAGOLOB())
			{
				return true;
			}
		}
		return false;
	}

	public void ICDEBPNMLFB(float _Duration = 0f)
	{
		int num = -1;
		for (int i = 0; i < GJEPILABGDO.Count; i++)
		{
			if (GJEPILABGDO[i].First.DBHJGAGOLOB())
			{
				num = i;
				break;
			}
		}
		if (num >= 0 && num < GJEPILABGDO.Count)
		{
			FEFDHNFOJLF.ScrollToCell(num, _Duration);
		}
	}

	public void BDHALBHODPG(string OGPJPGMBIHJ)
	{
		int num = -1;
		int i = 0;
		for (int count = GJEPILABGDO.Count; i < count; i++)
		{
			string mENAJEAJJBE = GJEPILABGDO[i].First.Name;
			if (mENAJEAJJBE == OGPJPGMBIHJ)
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
}
