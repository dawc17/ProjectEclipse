using System.Collections.Generic;
using Nekki.SF2.GUI;

public class ReusableCellsContainer
{
	public LinkedList<TableViewCell> IGKHHJKCPIJ;

	public void Init()
	{
		IGKHHJKCPIJ = new LinkedList<TableViewCell>();
	}

	public void FCGLFBFIPON(TableViewCell HJCPCBLCJJN)
	{
		IGKHHJKCPIJ.AddLast(HJCPCBLCJJN);
		HJCPCBLCJJN.gameObject.SetActive(false);
	}

	public TableViewCell CBLMJDCPLCD()
	{
		if (IGKHHJKCPIJ.Count == 0)
		{
			return null;
		}
		TableViewCell value = IGKHHJKCPIJ.First.Value;
		value.gameObject.SetActive(true);
		IGKHHJKCPIJ.RemoveFirst();
		return value;
	}
}
