using System.Collections.Generic;
using System.Reflection;

[DefaultMember("Item")]
public class PlistElementDict : PlistElement
{
	private SortedDictionary<string, PlistElement> BIHJIEHMAFC = new SortedDictionary<string, PlistElement>();

	public IDictionary<string, PlistElement> AMMFNLMJJFM
	{
		get
		{
			return NGEGAPEEGPN();
		}
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public PlistElement get_DLKPBAJDHBO(string KGBGENDIMBC)
	{
		return get_Item(KGBGENDIMBC);
	}

	public void set_DLKPBAJDHBO(string KGBGENDIMBC, PlistElement value)
	{
		AGGAMCGBFAF(KGBGENDIMBC, value);
	}

	public IDictionary<string, PlistElement> NGEGAPEEGPN()
	{
		return BIHJIEHMAFC;
	}

	public new PlistElement get_Item(string KGBGENDIMBC)
	{
		if (NGEGAPEEGPN().ContainsKey(KGBGENDIMBC))
		{
			return NGEGAPEEGPN()[KGBGENDIMBC];
		}
		return null;
	}

	public new void AGGAMCGBFAF(string KGBGENDIMBC, PlistElement value)
	{
		NGEGAPEEGPN()[KGBGENDIMBC] = value;
	}

	public void SetInteger(string KGBGENDIMBC, int PKHDLOGJKAD)
	{
		NGEGAPEEGPN()[KGBGENDIMBC] = new PlistElementInteger(PKHDLOGJKAD);
	}

	public void SetString(string KGBGENDIMBC, string PKHDLOGJKAD)
	{
		NGEGAPEEGPN()[KGBGENDIMBC] = new PlistElementString(PKHDLOGJKAD);
	}

	public void SetBoolean(string KGBGENDIMBC, bool PKHDLOGJKAD)
	{
		NGEGAPEEGPN()[KGBGENDIMBC] = new PlistElementBoolean(PKHDLOGJKAD);
	}

	public PlistElementArray IKMCPPMBAMN(string KGBGENDIMBC)
	{
		PlistElementArray gHFPDLCPEBH = new PlistElementArray();
		NGEGAPEEGPN()[KGBGENDIMBC] = gHFPDLCPEBH;
		return gHFPDLCPEBH;
	}

	public PlistElementDict CreateDict(string KGBGENDIMBC)
	{
		PlistElementDict jDMGABPEDFI = new PlistElementDict();
		NGEGAPEEGPN()[KGBGENDIMBC] = jDMGABPEDFI;
		return jDMGABPEDFI;
	}
}
