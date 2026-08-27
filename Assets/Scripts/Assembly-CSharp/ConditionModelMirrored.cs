using System.Xml;

public class ConditionModelMirrored : ConditionAnimation
{
	public ConditionModelMirrored(XmlNode node)
		: base(DGAGKLODADD.MIRROR)
	{
	}

	public override bool IsEqual(Model ACENLMONNPA, InfoAnimation DBOLBEOCEME)
	{
		bool flag = GMEKJPICMEJ(ACENLMONNPA, DBOLBEOCEME);
		return (!IsNot) ? flag : (!flag);
	}

	private bool GMEKJPICMEJ(Model ACENLMONNPA, InfoAnimation DBOLBEOCEME)
	{
		ModelType.KEIDBIOIFGA oOFFOILONLO = OOFFOILONLO;
		if (oOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_THIS)
		{
			return ModelAnimation.CalcIsMirror(ACENLMONNPA.CLDMEJKGLBA(), DBOLBEOCEME.ECCLELFHNHE().FJANLLCDPCP(), ACENLMONNPA.OCPMJKIEPIG().KFCNPADAMHA(), DBOLBEOCEME.BGHLLHNKFEM(), false);
		}
		LLLOJBFMONN.Error("ConditionModelMirrored: getMirror - wrong type: {0}", OOFFOILONLO);
		return false;
	}
}
