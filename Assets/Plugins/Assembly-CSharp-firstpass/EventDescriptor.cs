using System;
using System.Collections.Generic;
using System.Diagnostics;

internal sealed class EventDescriptor
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<BLIMHGJLDLD> OAKKGBDGKCM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FOJBGCENLHM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool BABMNMGKNMB;

	private BLIMHGJLDLD[] OKOBFIFILCH;

	public List<BLIMHGJLDLD> JOKFMPDBGNL
	{
		get
		{
			return PGBFAFNDGAA();
		}
		private set
		{
			NCODIDAGBAE(value);
		}
	}

	public bool DFCOFEGKOMP
	{
		get
		{
			return BECMKPPKAJB();
		}
		private set
		{
			LBOEBPFPHMJ(value);
		}
	}

	public bool KCIILHEGDAG
	{
		get
		{
			return CAACHPIAHIJ();
		}
		private set
		{
			FEDKJGINJID(value);
		}
	}

	public EventDescriptor(bool ONOLLCMDGBO, bool EJDLINOJJIF, BLIMHGJLDLD callback)
	{
		LBOEBPFPHMJ(ONOLLCMDGBO);
		FEDKJGINJID(EJDLINOJJIF);
		NCODIDAGBAE(new List<BLIMHGJLDLD>(1));
		if (callback != null)
		{
			PGBFAFNDGAA().Add(callback);
		}
	}

	public List<BLIMHGJLDLD> PGBFAFNDGAA()
	{
		return OAKKGBDGKCM;
	}

	private void NCODIDAGBAE(List<BLIMHGJLDLD> value)
	{
		OAKKGBDGKCM = value;
	}

	public bool BECMKPPKAJB()
	{
		return FOJBGCENLHM;
	}

	private void LBOEBPFPHMJ(bool value)
	{
		FOJBGCENLHM = value;
	}

	public bool CAACHPIAHIJ()
	{
		return BABMNMGKNMB;
	}

	private void FEDKJGINJID(bool value)
	{
		BABMNMGKNMB = value;
	}

	public void Call(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (OKOBFIFILCH == null || OKOBFIFILCH.Length < PGBFAFNDGAA().Count)
		{
			Array.Resize(ref OKOBFIFILCH, PGBFAFNDGAA().Count);
		}
		PGBFAFNDGAA().CopyTo(OKOBFIFILCH);
		for (int i = 0; i < OKOBFIFILCH.Length; i++)
		{
			try
			{
				OKOBFIFILCH[i](JLEACANCMJF, NPKADBPBKIG, LKIOKGCNKHE);
			}
			catch (Exception ex)
			{
				((ISocket)JLEACANCMJF).EmitError(CCCOMMIFIMB.User, ex.Message + " " + ex.StackTrace);
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventDescriptor", "Call", ex);
			}
			if (BECMKPPKAJB())
			{
				PGBFAFNDGAA().Remove(OKOBFIFILCH[i]);
			}
			OKOBFIFILCH[i] = null;
		}
	}
}
