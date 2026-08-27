using System.Xml;

public class ConditionDirection : ConditionAnimation
{
	public InfoAnimation.MoveInside.Direction HFOEHJIMGPI;

	public ConditionDirection(XmlNode node)
		: base(DGAGKLODADD.DIRECTION)
	{
		HFOEHJIMGPI = MovesParser.JOLJIHDPADK(node);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		int num = HFOEHJIMGPI.IMLFCBLAJGA(conditions);
		int num2 = 1;
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			num2 = conditions.GFHOIKMBNHF;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			num2 = conditions.OLNDCCIPJAE;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			num2 = conditions.CDPEPJDJIPK;
			break;
		default:
			LLLOJBFMONN.Error("ConditionDirection::isEqual ERROR - unsupported model type: {0}", OOFFOILONLO);
			break;
		}
		bool flag = num == num2;
		return (!IsNot) ? flag : (!flag);
	}

	public void KJHPCLOFDJB(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, ModelNode AECCPADGGPG, bool PHADJMAONJG, ModelObject MJCGOJBGFIE = null)
	{
		HFOEHJIMGPI.CLCFLPDNBNL.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
		HFOEHJIMGPI.KAEAKHIEIHH.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
	}

	public void ABNCNNHMLII()
	{
		HFOEHJIMGPI.CLCFLPDNBNL.GPGKANDFLNB();
		HFOEHJIMGPI.KAEAKHIEIHH.GPGKANDFLNB();
	}

	public override Model DKDAKGDMHAL(Model BPBMKGHEEBI, ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
		return BPBMKGHEEBI;
	}

	public override void MJFKNEHGNMB(ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
	}
}
