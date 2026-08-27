using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public interface ITableView
{
	ITableViewDataSource JFDKGBHEEOF { get; set; }

	ITableViewDelegate Delegate { get; set; }

	GameObject AKMFLOHDJJG { get; set; }

	Range NKGGFPOGKJC { get; }

	float ELNOAHEFGBL { get; }

	float JJCKADKCDIF { get; }

	ITableViewDataSource get_DataSource();

	void set_DataSource(ITableViewDataSource value);



	GameObject get_CellPrefab();

	void set_CellPrefab(GameObject value);

	Range get_VisibleRange();

	float get_ContentSize();

	float get_Position();

	TableViewCell ReusableCellForRow(int IBAKGENOEPH);

	TableViewCell CellForRow(int IBAKGENOEPH);

	float PositionForRow(int IBAKGENOEPH);

	void ReloadData();

	void SetPosition(float FFMJGKPCBNK, float time);
}
