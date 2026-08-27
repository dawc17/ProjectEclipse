using System.Collections.Generic;

public class UpgradeDataContainer
{
	public string Type = string.Empty;

	public List<UpgradeData> KPAPEBOAKIE = new List<UpgradeData>();

	public void RandomizeObscuredVars()
	{
		KPAPEBOAKIE.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}
}
