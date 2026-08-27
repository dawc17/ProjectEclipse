using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class HTTPFormBase
{
	private const int LongLength = 256;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<HTTPFieldData> CIIOOOFFJHD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool LIGGKJHGBKE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool LADEJCEHFCJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CNEGAADGHGH;

	public List<HTTPFieldData> JMKBOELCNPO
	{
		get
		{
			return CKOJIABCEBP();
		}
		set
		{
			EOJFFGAAGOA(value);
		}
	}

	public bool OOPMAAHJMCE
	{
		get
		{
			return DAIAOBAEDCB();
		}
	}

	public bool OIOLLLGHBPP
	{
		get
		{
			return JODDPBJOEJN();
		}
		protected set
		{
			AKIGPOBCEOC(value);
		}
	}

	public bool MGIABBEFIGL
	{
		get
		{
			return MNLGNEHBCJK();
		}
		protected set
		{
			NABPLGNDELE(value);
		}
	}

	public bool LEMLMNHBLLF
	{
		get
		{
			return FJPFFNEKKOL();
		}
		protected set
		{
			PNJDEHAHOJC(value);
		}
	}

	public List<HTTPFieldData> CKOJIABCEBP()
	{
		return CIIOOOFFJHD;
	}

	public void EOJFFGAAGOA(List<HTTPFieldData> value)
	{
		CIIOOOFFJHD = value;
	}

	public bool DAIAOBAEDCB()
	{
		return CKOJIABCEBP() == null || CKOJIABCEBP().Count == 0;
	}

	public bool JODDPBJOEJN()
	{
		return LIGGKJHGBKE;
	}

	protected void AKIGPOBCEOC(bool value)
	{
		LIGGKJHGBKE = value;
	}

	public bool MNLGNEHBCJK()
	{
		return LADEJCEHFCJ;
	}

	protected void NABPLGNDELE(bool value)
	{
		LADEJCEHFCJ = value;
	}

	public bool FJPFFNEKKOL()
	{
		return CNEGAADGHGH;
	}

	protected void PNJDEHAHOJC(bool value)
	{
		CNEGAADGHGH = value;
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME)
	{
		AddBinaryData(LKABGPANBMH, DMNBDBJNKME, null, null);
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME, string PMFEIPCHENB)
	{
		AddBinaryData(LKABGPANBMH, DMNBDBJNKME, PMFEIPCHENB, null);
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME, string PMFEIPCHENB, string KIDMMGJIEHJ)
	{
		if (CKOJIABCEBP() == null)
		{
			EOJFFGAAGOA(new List<HTTPFieldData>());
		}
		HTTPFieldData iIMHHCDGJOL = new HTTPFieldData();
		iIMHHCDGJOL.set_Name(LKABGPANBMH);
		if (PMFEIPCHENB == null)
		{
			iIMHHCDGJOL.IMMLGNKJPKA(LKABGPANBMH + ".dat");
		}
		else
		{
			iIMHHCDGJOL.IMMLGNKJPKA(PMFEIPCHENB);
		}
		if (KIDMMGJIEHJ == null)
		{
			iIMHHCDGJOL.KLHEONGIINC("application/octet-stream");
		}
		else
		{
			iIMHHCDGJOL.KLHEONGIINC(KIDMMGJIEHJ);
		}
		iIMHHCDGJOL.set_Binary(DMNBDBJNKME);
		CKOJIABCEBP().Add(iIMHHCDGJOL);
		bool bAINMLLIKOL = true;
		AKIGPOBCEOC(bAINMLLIKOL);
		NABPLGNDELE(bAINMLLIKOL);
	}

	public void AddField(string LKABGPANBMH, string value)
	{
		AddField(LKABGPANBMH, value, Encoding.UTF8);
	}

	public void AddField(string LKABGPANBMH, string value, Encoding FOPOKALJIIJ)
	{
		if (CKOJIABCEBP() == null)
		{
			EOJFFGAAGOA(new List<HTTPFieldData>());
		}
		HTTPFieldData iIMHHCDGJOL = new HTTPFieldData();
		iIMHHCDGJOL.set_Name(LKABGPANBMH);
		iIMHHCDGJOL.IMMLGNKJPKA(null);
		iIMHHCDGJOL.KLHEONGIINC("text/plain; charset=\"" + FOPOKALJIIJ.WebName + "\"");
		iIMHHCDGJOL.MHMDIMIEPLL(value);
		iIMHHCDGJOL.set_Encoding(FOPOKALJIIJ);
		CKOJIABCEBP().Add(iIMHHCDGJOL);
		AKIGPOBCEOC(true);
		PNJDEHAHOJC(FJPFFNEKKOL() | (value.Length > 256));
	}

	public virtual void CopyFrom(HTTPFormBase KHGIIFDIHHA)
	{
		EOJFFGAAGOA(new List<HTTPFieldData>(KHGIIFDIHHA.CKOJIABCEBP()));
		AKIGPOBCEOC(true);
		NABPLGNDELE(KHGIIFDIHHA.MNLGNEHBCJK());
		PNJDEHAHOJC(KHGIIFDIHHA.FJPFFNEKKOL());
	}

	public virtual void PrepareRequest(HTTPRequest ONOCIELLAPL)
	{
		throw new NotImplementedException();
	}

	public virtual byte[] GDENFGNLFKL()
	{
		throw new NotImplementedException();
	}
}
