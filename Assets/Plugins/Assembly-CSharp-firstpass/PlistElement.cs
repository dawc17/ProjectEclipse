using System.Reflection;

[DefaultMember("Item")]
public class PlistElement
{
	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public PlistElement get_DLKPBAJDHBO(string KGBGENDIMBC)
	{
		return get_Item(KGBGENDIMBC);
	}

	public void set_DLKPBAJDHBO(string KGBGENDIMBC, PlistElement value)
	{
		AGGAMCGBFAF(KGBGENDIMBC, value);
	}

	protected PlistElement()
	{
	}

	public string CIPOICEEIBK()
	{
		return ((PlistElementString)this).value;
	}

	public int HJJGDHGJFEG()
	{
		return ((PlistElementInteger)this).value;
	}

	public bool MHAKAEEDBIJ()
	{
		return ((PlistElementBoolean)this).value;
	}

	public PlistElementArray GKDJFCGPACC()
	{
		return (PlistElementArray)this;
	}

	public PlistElementDict MKLDLPEGCDE()
	{
		return (PlistElementDict)this;
	}

	public PlistElement get_Item(string KGBGENDIMBC)
	{
		return MKLDLPEGCDE().get_Item(KGBGENDIMBC);
	}

	public void AGGAMCGBFAF(string KGBGENDIMBC, PlistElement value)
	{
		MKLDLPEGCDE().AGGAMCGBFAF(KGBGENDIMBC, value);
	}
}
