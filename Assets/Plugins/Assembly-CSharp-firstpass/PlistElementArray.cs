using System.Collections.Generic;

public class PlistElementArray : PlistElement
{
	public List<PlistElement> AMMFNLMJJFM = new List<PlistElement>();

	public void AddString(string PKHDLOGJKAD)
	{
		AMMFNLMJJFM.Add(new PlistElementString(PKHDLOGJKAD));
	}

	public void AddInteger(int PKHDLOGJKAD)
	{
		AMMFNLMJJFM.Add(new PlistElementInteger(PKHDLOGJKAD));
	}

	public void AddBoolean(bool PKHDLOGJKAD)
	{
		AMMFNLMJJFM.Add(new PlistElementBoolean(PKHDLOGJKAD));
	}

	public PlistElementArray GGDILDGKEOA()
	{
		PlistElementArray gHFPDLCPEBH = new PlistElementArray();
		AMMFNLMJJFM.Add(gHFPDLCPEBH);
		return gHFPDLCPEBH;
	}

	public PlistElementDict IADDCFDGNEF()
	{
		PlistElementDict jDMGABPEDFI = new PlistElementDict();
		AMMFNLMJJFM.Add(jDMGABPEDFI);
		return jDMGABPEDFI;
	}
}
