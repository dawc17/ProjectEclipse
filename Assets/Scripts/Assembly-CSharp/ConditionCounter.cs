using System.Xml;

public class ConditionCounter
{
	public enum FELOFIAKFCO
	{
		NONE = 0,
		BATTLE = 1,
		OPERATOR = 2
	}

	protected FELOFIAKFCO OPKOJKOCIDJ;

	protected bool _isNot;

	public ConditionCounter(FELOFIAKFCO LFLGCDNKNJI)
	{
		OPKOJKOCIDJ = LFLGCDNKNJI;
	}

	public virtual bool IsEqual(CounterConditions conditions)
	{
		LLLOJBFMONN.Error("ERROR: Unknown condition type checked: %i", OPKOJKOCIDJ);
		return false;
	}

	public bool IsNotCompare(bool DCJLKCFKCOM)
	{
		return _isNot ? (!DCJLKCFKCOM) : DCJLKCFKCOM;
	}

	public virtual void AEPHNNABOEK()
	{
	}

	protected virtual void Parse(XmlNode BGPKIKNPIKP)
	{
		_isNot = BGPKIKNPIKP.Attributes["Not"].ParseBool();
	}
}
