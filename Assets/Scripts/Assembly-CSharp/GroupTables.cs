using System.Collections.Generic;

public class GroupTables
{
	public string GroupLabel;

	public List<TacticalTable> DOCMMNLEAMH;

	public TacticalTable GetTacticalTableByLabel(string ICBBNJMLDJH)
	{
		for (int i = 0; i < DOCMMNLEAMH.Count; i++)
		{
			if (DOCMMNLEAMH[i].Label == ICBBNJMLDJH)
			{
				return DOCMMNLEAMH[i];
			}
		}
		LLLOJBFMONN.Error("table for label {0} not found", ICBBNJMLDJH);
		return null;
	}
}
