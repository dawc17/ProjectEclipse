using UnityEngine.Purchasing;

public static class KNCKFKIOPHE
{
	public static FIKEKJBAKBO HMBLNIANJDN(this Product KDOEGOIJKLG)
	{
		return string.IsNullOrEmpty(KDOEGOIJKLG.receipt) ? null : new FIKEKJBAKBO(KDOEGOIJKLG);
	}

	public static string Log(this ProductDefinition PANEMFIIOGB)
	{
		return string.Format("[ProductDefinition: id={0}, storeSpecificId={1}, type={2}]", PANEMFIIOGB.id, PANEMFIIOGB.storeSpecificId, PANEMFIIOGB.type);
	}
}
