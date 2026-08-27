public class PerkEventAreaEnter : PerkEvent
{
	public PerkEventAreaEnter()
	{
	}

	public PerkEventAreaEnter(PerkEventAreaEnter NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (!base.IsEqual(EJMEALJNNIL) || EJMEALJNNIL == null)
		{
			return false;
		}
		return true;
	}
}
