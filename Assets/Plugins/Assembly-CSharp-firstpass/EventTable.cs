using System.Collections.Generic;
using System.Diagnostics;

internal sealed class EventTable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Socket DLOJBJBNIOK;

	private Dictionary<string, List<EventDescriptor>> Table = new Dictionary<string, List<EventDescriptor>>();

	private Socket KNPLDJGCAKJ
	{
		get
		{
			return PDJFKOBODHH();
		}
		set
		{
			AHOICPCKAHI(value);
		}
	}

	public EventTable(Socket JLEACANCMJF)
	{
		AHOICPCKAHI(JLEACANCMJF);
	}

	private Socket PDJFKOBODHH()
	{
		return DLOJBJBNIOK;
	}

	private void AHOICPCKAHI(Socket value)
	{
		DLOJBJBNIOK = value;
	}

	public void DNKHCGPPBAE(string DOPHKKGNAEF, BLIMHGJLDLD callback, bool ONOLLCMDGBO, bool EJDLINOJJIF)
	{
		List<EventDescriptor> value;
		if (!Table.TryGetValue(DOPHKKGNAEF, out value))
		{
			Table.Add(DOPHKKGNAEF, value = new List<EventDescriptor>(1));
		}
		EventDescriptor lBIMLJMCENN = value.Find((EventDescriptor d) => d.BECMKPPKAJB() == ONOLLCMDGBO && d.CAACHPIAHIJ() == EJDLINOJJIF);
		if (lBIMLJMCENN == null)
		{
			value.Add(new EventDescriptor(ONOLLCMDGBO, EJDLINOJJIF, callback));
		}
		else
		{
			lBIMLJMCENN.PGBFAFNDGAA().Add(callback);
		}
	}

	public void Unregister(string DOPHKKGNAEF)
	{
		Table.Remove(DOPHKKGNAEF);
	}

	public void Unregister(string DOPHKKGNAEF, BLIMHGJLDLD callback)
	{
		List<EventDescriptor> value;
		if (Table.TryGetValue(DOPHKKGNAEF, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				value[i].PGBFAFNDGAA().Remove(callback);
			}
		}
	}

	public void Call(string DOPHKKGNAEF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
		{
			HTTPManager.MBBMPNDDPIH().JMHHKELODIO("EventTable", "Call - " + DOPHKKGNAEF);
		}
		List<EventDescriptor> value;
		if (Table.TryGetValue(DOPHKKGNAEF, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				value[i].Call(PDJFKOBODHH(), NPKADBPBKIG, LKIOKGCNKHE);
			}
		}
	}

	public void Call(Packet NPKADBPBKIG)
	{
		string text = NPKADBPBKIG.EFJKNHMALOL();
		string text2 = ((NPKADBPBKIG.CMEHGNCCCIN() == ECDAJBEFCAH.Unknown) ? EventNames.ICAIODPBKBO(NPKADBPBKIG.FFJBNPEOAHI()) : EventNames.ICAIODPBKBO(NPKADBPBKIG.CMEHGNCCCIN()));
		object[] lKIOKGCNKHE = null;
		if (JFEKPKEOGCL(text) || JFEKPKEOGCL(text2))
		{
			if (NPKADBPBKIG.FFJBNPEOAHI() == HJDLGPHLPNF.Message && (NPKADBPBKIG.CMEHGNCCCIN() == ECDAJBEFCAH.Event || NPKADBPBKIG.CMEHGNCCCIN() == ECDAJBEFCAH.BinaryEvent) && GICEOMDFBCK(text))
			{
				lKIOKGCNKHE = NPKADBPBKIG.Decode(PDJFKOBODHH().HLBNHJADOMP().KCMCCGKJGLE());
			}
			if (!string.IsNullOrEmpty(text))
			{
				Call(text, NPKADBPBKIG, lKIOKGCNKHE);
			}
			if (!NPKADBPBKIG.KJFDJLNHKJI() && GICEOMDFBCK(text2))
			{
				lKIOKGCNKHE = NPKADBPBKIG.Decode(PDJFKOBODHH().HLBNHJADOMP().KCMCCGKJGLE());
			}
			if (!string.IsNullOrEmpty(text2))
			{
				Call(text2, NPKADBPBKIG, lKIOKGCNKHE);
			}
		}
	}

	public void Clear()
	{
		Table.Clear();
	}

	private bool GICEOMDFBCK(string DOPHKKGNAEF)
	{
		List<EventDescriptor> value;
		if (Table.TryGetValue(DOPHKKGNAEF, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				if (value[i].CAACHPIAHIJ() && value[i].PGBFAFNDGAA().Count > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool JFEKPKEOGCL(string DOPHKKGNAEF)
	{
		return Table.ContainsKey(DOPHKKGNAEF);
	}
}
