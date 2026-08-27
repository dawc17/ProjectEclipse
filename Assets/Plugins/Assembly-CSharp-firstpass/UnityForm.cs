using System.Diagnostics;
using UnityEngine;

public sealed class UnityForm : HTTPFormBase
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private WWWForm LHIKIINENLF;

	public WWWForm ABBKKOEEELJ
	{
		get
		{
			return OCEAMIDPFAJ();
		}
		set
		{
			set_Form(value);
		}
	}

	public UnityForm()
	{
	}

	public UnityForm(WWWForm HOELLMLEBAK)
	{
		set_Form(HOELLMLEBAK);
	}

	public WWWForm OCEAMIDPFAJ()
	{
		return LHIKIINENLF;
	}

	public void set_Form(WWWForm value)
	{
		LHIKIINENLF = value;
	}

	public override void CopyFrom(HTTPFormBase KHGIIFDIHHA)
	{
		EOJFFGAAGOA(KHGIIFDIHHA.CKOJIABCEBP());
		AKIGPOBCEOC(true);
		if (OCEAMIDPFAJ() != null)
		{
			return;
		}
		set_Form(new WWWForm());
		if (CKOJIABCEBP() == null)
		{
			return;
		}
		for (int i = 0; i < CKOJIABCEBP().Count; i++)
		{
			HTTPFieldData iIMHHCDGJOL = CKOJIABCEBP()[i];
			if (string.IsNullOrEmpty(iIMHHCDGJOL.ILMJJEMPKCN()) && iIMHHCDGJOL.CLBEEBOFBMA() != null)
			{
				OCEAMIDPFAJ().AddBinaryData(iIMHHCDGJOL.get_Name(), iIMHHCDGJOL.CLBEEBOFBMA(), iIMHHCDGJOL.EPDMGFELIMC(), iIMHHCDGJOL.DIHKMAKOHGN());
			}
			else
			{
				OCEAMIDPFAJ().AddField(iIMHHCDGJOL.get_Name(), iIMHHCDGJOL.ILMJJEMPKCN(), iIMHHCDGJOL.PGBGEOMJDJK());
			}
		}
	}

	public override void PrepareRequest(HTTPRequest ONOCIELLAPL)
	{
		if (OCEAMIDPFAJ().headers.ContainsKey("Content-Type"))
		{
			ONOCIELLAPL.MMPFBNNMGED("Content-Type", OCEAMIDPFAJ().headers["Content-Type"]);
		}
		else
		{
			ONOCIELLAPL.MMPFBNNMGED("Content-Type", "application/x-www-form-urlencoded");
		}
	}

	public override byte[] GDENFGNLFKL()
	{
		return OCEAMIDPFAJ().data;
	}
}
