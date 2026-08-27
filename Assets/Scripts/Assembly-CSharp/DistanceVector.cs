using System.Xml;
using UnityEngine;

public class DistanceVector
{
	private bool _Exists;

	private DistancePointFollow DLKOKBMKAJE = new DistancePointFollow();

	private DistancePointFollow KPBKMOCDIBF = new DistancePointFollow();

	private Vector2f BMKAHMEJOJI;

	public DistanceVector()
	{
		_Exists = false;
	}

	public DistanceVector(XmlNode node)
	{
		Parse(node);
	}

	public void Parse(XmlNode node)
	{
		DLKOKBMKAJE.Create(node["From"]);
		KPBKMOCDIBF.Create(node["To"]);
		_Exists = true;
	}

	public Vector2f HLBBNCBJHGB(ModelConditions conditions)
	{
		if (_Exists)
		{
			Vector3 vector = DLKOKBMKAJE.EMGKDOAMBOH(conditions);
			Vector3 vector2 = KPBKMOCDIBF.EMGKDOAMBOH(conditions);
			BMKAHMEJOJI = new Vector2f(vector2.x - vector.x, vector2.y - vector.y);
		}
		else
		{
			BMKAHMEJOJI = new Vector2f();
		}
		return BMKAHMEJOJI;
	}

	public void KJHPCLOFDJB(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, ModelNode AECCPADGGPG, bool PHADJMAONJG, ModelObject MJCGOJBGFIE = null)
	{
		DLKOKBMKAJE.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
		KPBKMOCDIBF.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
	}

	public void JKDMJGNOKCA()
	{
		DLKOKBMKAJE.GPGKANDFLNB();
		KPBKMOCDIBF.GPGKANDFLNB();
	}
}
