using System.Collections.Generic;
using System.Diagnostics;
using Nekki.SF2.GUI;
using UnityEngine.SocialPlatforms;

public class VisibleCells
{
	public Range IndexesRange;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<int, TableViewCell> JMEPBPMMOKE;

	public Dictionary<int, TableViewCell> MMINMBOKMEO
	{
		get
		{
			return BFNFADJMAPC();
		}
		private set
		{
			set_cells(value);
		}
	}

	public int Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
	}

	public VisibleCells()
	{
		IndexesRange = new Range(0, 0);
		set_cells(new Dictionary<int, TableViewCell>());
	}

	public Dictionary<int, TableViewCell> BFNFADJMAPC()
	{
		return JMEPBPMMOKE;
	}

	private void set_cells(Dictionary<int, TableViewCell> value)
	{
		JMEPBPMMOKE = value;
	}

	public int OFOPFCJNEBL()
	{
		return BFNFADJMAPC().Count;
	}

	public TableViewCell GetCellAtIndex(int index)
	{
		TableViewCell value = null;
		BFNFADJMAPC().TryGetValue(index, out value);
		return value;
	}

	public void KLKJONFEGHM(int index, TableViewCell HJCPCBLCJJN)
	{
		BFNFADJMAPC()[index] = HJCPCBLCJJN;
	}

	public void RemoveCellAtIndex(int index)
	{
		BFNFADJMAPC().Remove(index);
	}
}
