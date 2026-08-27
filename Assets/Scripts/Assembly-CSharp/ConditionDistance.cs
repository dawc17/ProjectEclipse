using System.Xml;
using UnityEngine;

public class ConditionDistance : ConditionAnimation
{
	private enum HENHJEAEGLG
	{
		LENGTH_X = 0,
		LENGTH_Y = 1,
		LENGTH_FULL = 2
	}

	public const float nonlimit = 1000000f;

	private float LHNCHOAEGEA;

	private float KAEPJHHLLPK;

	private HENHJEAEGLG MKNFOOEOMAO;

	private DistancePoint PNFEMBMAEGA = new DistancePoint();

	private DistancePoint LEAEOECEOPG = new DistancePoint();

	public ConditionDistance(XmlNode node)
		: base(DGAGKLODADD.DISTANCE)
	{
		XmlAttribute xmlAttribute = node.Attributes["Axis"];
		MKNFOOEOMAO = ((xmlAttribute == null) ? HENHJEAEGLG.LENGTH_FULL : ((!(xmlAttribute.Value == "X")) ? HENHJEAEGLG.LENGTH_Y : HENHJEAEGLG.LENGTH_X));
		LHNCHOAEGEA = node.Attributes["Min"].ParseFloat(-1000000f);
		KAEPJHHLLPK = node.Attributes["Max"].ParseFloat(1000000f);
		PNFEMBMAEGA.Create(node["From"]);
		LEAEOECEOPG.Create(node["To"]);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		float num = 0f;
		switch (MKNFOOEOMAO)
		{
		case HENHJEAEGLG.LENGTH_X:
			num = LEAEOECEOPG.ILIKNABGPNK(conditions) - PNFEMBMAEGA.ILIKNABGPNK(conditions);
			num *= (float)conditions.PCAOCHAIBJC;
			break;
		case HENHJEAEGLG.LENGTH_Y:
			num = LEAEOECEOPG.MJPKHPNIJGK(conditions) - PNFEMBMAEGA.MJPKHPNIJGK(conditions);
			break;
		case HENHJEAEGLG.LENGTH_FULL:
		{
			Vector3f eMAFACPEPDK = Vector3f.op_Implicit(LEAEOECEOPG.EMGKDOAMBOH(conditions));
			Vector3f eMAFACPEPDK2 = Vector3f.op_Implicit(PNFEMBMAEGA.EMGKDOAMBOH(conditions));
			num = Mathf.Sqrt((eMAFACPEPDK.GILCBJJPKBK() - eMAFACPEPDK2.GILCBJJPKBK()) * (eMAFACPEPDK.GILCBJJPKBK() - eMAFACPEPDK2.GILCBJJPKBK()) + (eMAFACPEPDK.OBIMBNIBEFG() - eMAFACPEPDK2.OBIMBNIBEFG()) * (eMAFACPEPDK.OBIMBNIBEFG() - eMAFACPEPDK2.OBIMBNIBEFG()));
			break;
		}
		}
		bool flag = LHNCHOAEGEA <= num && num <= KAEPJHHLLPK;
		return (!IsNot) ? flag : (!flag);
	}

	public void KJHPCLOFDJB(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, ModelNode AECCPADGGPG, bool PHADJMAONJG, ModelObject MJCGOJBGFIE = null)
	{
		PNFEMBMAEGA.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
		LEAEOECEOPG.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
	}

	public void ABNCNNHMLII()
	{
		PNFEMBMAEGA.GPGKANDFLNB();
		LEAEOECEOPG.GPGKANDFLNB();
	}
}
