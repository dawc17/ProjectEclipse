using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DistancePoint
{
	public enum JJIAEPLMBFF
	{
		OBJECT_NULL = 0,
		OBJECT_NODES = 1,
		OBJECT_PIVOT = 2,
		OBJECT_WALL = 3,
		OBJECT_FLOOR = 4,
		OBJECT_COM = 5
	}

	public enum PCEPIHHGDJC
	{
		DISTANCE_FRAME_NULL = 0,
		DISTANCE_FRAME_CURRENT = 1,
		DISTANCE_FRAME_PREVIOUS = 2
	}

	public enum FKIAPHGNLKC
	{
		IMPULSE_NONE = 0,
		IMPULSE_NOT_REVERSE = 1,
		IMPULSE_REVERSE = 2
	}

	public class PointNode
	{
		public ModelNode Node;

		public ModelNode CHEKEGGJDBL;
	}

	public class ChildPoint
	{
		public PointNode Point = new PointNode();

		public ModelObject BIAONEGEHLB;

		public ChildPoint(ModelObject ACENLMONNPA)
		{
			BIAONEGEHLB = ACENLMONNPA;
		}
	}

	public ModelType.KEIDBIOIFGA OOFFOILONLO;

	public JJIAEPLMBFF HLGJJGHDEAP;

	public PCEPIHHGDJC Frame;

	public string Part;

	public bool IsBackWall;

	public float GEHDIPOGEOL;

	public float KLGJPPOOBFF;

	private List<ChildPoint> BJIPGGAOPDB = new List<ChildPoint>();

	private List<ChildPoint> OKJEDLODFCP = new List<ChildPoint>();

	private PointNode ABDKBCLJAME = new PointNode();

	private PointNode IHJJBIDMEMB = new PointNode();

	public DistancePoint()
	{
		IsBackWall = false;
		OOFFOILONLO = ModelType.KEIDBIOIFGA.MODEL_NULL;
		HLGJJGHDEAP = JJIAEPLMBFF.OBJECT_NULL;
		Frame = PCEPIHHGDJC.DISTANCE_FRAME_NULL;
		GEHDIPOGEOL = (KLGJPPOOBFF = 0f);
	}

	public DistancePoint(XmlNode node)
	{
		IsBackWall = false;
		OOFFOILONLO = ModelType.KEIDBIOIFGA.MODEL_NULL;
		HLGJJGHDEAP = JJIAEPLMBFF.OBJECT_NULL;
		Frame = PCEPIHHGDJC.DISTANCE_FRAME_NULL;
		GEHDIPOGEOL = (KLGJPPOOBFF = 0f);
		Create(node);
	}

	public virtual void Create(XmlNode node)
	{
		OOFFOILONLO = ModelType.EHFNOBFLAHI((node == null) ? "Null" : node.Attributes["Player"].CIPOICEEIBK("Null"));
		XmlAttribute cJBEMNNNHDM = ((node == null) ? null : node.Attributes["Object"]);
		string bAINMLLIKOL = cJBEMNNNHDM.CIPOICEEIBK(string.Empty);
		HLGJJGHDEAP = (JJIAEPLMBFF)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.DISTANCE_OBJECT_TYPE, bAINMLLIKOL);
		cJBEMNNNHDM = ((node == null) ? null : node.Attributes["Part"]);
		Part = cJBEMNNNHDM.CIPOICEEIBK(string.Empty);
		if (HLGJJGHDEAP == JJIAEPLMBFF.OBJECT_WALL)
		{
			IsBackWall = Part == "Back";
		}
		Frame = PCEPIHHGDJC.DISTANCE_FRAME_CURRENT;
		cJBEMNNNHDM = ((node == null) ? null : node.Attributes["Frame"]);
		if (cJBEMNNNHDM != null && cJBEMNNNHDM.CIPOICEEIBK(string.Empty) == "Previous")
		{
			Frame = PCEPIHHGDJC.DISTANCE_FRAME_PREVIOUS;
		}
		GEHDIPOGEOL = ((node == null) ? 0f : node.Attributes["ShiftX"].ParseFloat());
		KLGJPPOOBFF = ((node == null) ? 0f : node.Attributes["ShiftY"].ParseFloat());
	}

	public void Create(string ENAEDFEDNGI, string HIPONJCKJEH, string BOLAFILGINF)
	{
		OOFFOILONLO = ModelType.EHFNOBFLAHI(ENAEDFEDNGI);
		HLGJJGHDEAP = (JJIAEPLMBFF)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.DISTANCE_OBJECT_TYPE, HIPONJCKJEH);
		if (HLGJJGHDEAP == JJIAEPLMBFF.OBJECT_WALL)
		{
			IsBackWall = BOLAFILGINF == "Back";
		}
		Frame = PCEPIHHGDJC.DISTANCE_FRAME_CURRENT;
		string empty = string.Empty;
		if (empty == "Previous")
		{
			Frame = PCEPIHHGDJC.DISTANCE_FRAME_PREVIOUS;
		}
		GEHDIPOGEOL = 0f;
		KLGJPPOOBFF = 0f;
	}

	public virtual float ILIKNABGPNK(ModelConditions conditions)
	{
		return EMGKDOAMBOH(conditions).x;
	}

	public virtual float MJPKHPNIJGK(ModelConditions conditions)
	{
		return EMGKDOAMBOH(conditions).y * -1f;
	}

	public virtual float CHBKDOCBKFJ(ModelConditions conditions)
	{
		return EMGKDOAMBOH(conditions).z;
	}

	public virtual Vector3 EMGKDOAMBOH(ModelConditions conditions)
	{
		ModelConditions.ModelPositions dFKJGDBENAL = MJFBOLMAEGG(conditions);
		ModelNode lCDGOCIAIDK = null;
		Vector3 result = default(Vector3);
		switch (HLGJJGHDEAP)
		{
		case JJIAEPLMBFF.OBJECT_NODES:
		{
			lCDGOCIAIDK = GetNode(conditions);
			if (lCDGOCIAIDK != null)
			{
				result = Vector3f.op_Implicit(KEFNOMIKGEN(lCDGOCIAIDK));
			}
			float x = result.x + GEHDIPOGEOL * (float)conditions.PCAOCHAIBJC;
			float y = result.y - KLGJPPOOBFF;
			return new Vector3(x, y);
		}
		case JJIAEPLMBFF.OBJECT_PIVOT:
			lCDGOCIAIDK = EJKAMJPJKMF(conditions);
			if (lCDGOCIAIDK != null)
			{
				result = Vector3f.op_Implicit(KEFNOMIKGEN(lCDGOCIAIDK));
			}
			result.x += GEHDIPOGEOL * (float)conditions.PCAOCHAIBJC;
			result.y -= KLGJPPOOBFF;
			return result;
		case JJIAEPLMBFF.OBJECT_WALL:
		{
			Vector2 vector = NONAHPKMDMA(conditions, dFKJGDBENAL);
			vector.x += GEHDIPOGEOL * (float)conditions.GFHOIKMBNHF;
			vector.y -= KLGJPPOOBFF;
			return Vector3f.op_Implicit(new Vector3f(vector));
		}
		case JJIAEPLMBFF.OBJECT_FLOOR:
			return Vector3f.op_Implicit(new Vector3f(GEHDIPOGEOL, 0f - KLGJPPOOBFF));
		case JJIAEPLMBFF.OBJECT_COM:
			lCDGOCIAIDK = dFKJGDBENAL.CBAECAAKAIA.HOFFDCFEBGA();
			result = Vector3f.op_Implicit(KEFNOMIKGEN(lCDGOCIAIDK));
			result.x += GEHDIPOGEOL * (float)conditions.GFHOIKMBNHF;
			result.y -= KLGJPPOOBFF;
			return result;
		default:
			LLLOJBFMONN.Write("ERROR: unknown object type: %i", HLGJJGHDEAP);
			return default(Vector3);
		}
	}

	public void UpdateNode(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, ModelNode AECCPADGGPG, bool PHADJMAONJG, ModelObject MJCGOJBGFIE)
	{
		PointNode bKHJJICJODB = null;
		if (PHADJMAONJG)
		{
			bool flag = false;
			List<ChildPoint> list = ((!EKBOGDKIHIH) ? OKJEDLODFCP : BJIPGGAOPDB);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].BIAONEGEHLB == MJCGOJBGFIE)
				{
					bKHJJICJODB = list[i].Point;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ChildPoint fKKHKDFDLJI = new ChildPoint(MJCGOJBGFIE);
				list.Add(fKKHKDFDLJI);
				bKHJJICJODB = fKKHKDFDLJI.Point;
			}
		}
		else
		{
			bKHJJICJODB = ((!EKBOGDKIHIH) ? ABDKBCLJAME : IHJJBIDMEMB);
		}
		if (HLGJJGHDEAP == JJIAEPLMBFF.OBJECT_NODES)
		{
			bKHJJICJODB.Node = OECPEDPMKCD.EGHIDHMENEF(Part);
		}
		bKHJJICJODB.CHEKEGGJDBL = AECCPADGGPG;
	}

	protected Vector2 NONAHPKMDMA(ModelConditions conditions, ModelConditions.ModelPositions MDBELBGHDFP)
	{
		int num = 0;
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			num = conditions.PCAOCHAIBJC;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
		case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
			num = conditions.OLNDCCIPJAE;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			num = conditions.CDPEPJDJIPK;
			break;
		default:
			LLLOJBFMONN.Error("ERROR: DistancePoint::getPosition - unknown model: {0}", OOFFOILONLO);
			break;
		}
		return (num > 0 != IsBackWall) ? MDBELBGHDFP.PCIBKEOCFAO : MDBELBGHDFP.BOGHNBAKCEL;
	}

	protected ModelNode EJKAMJPJKMF(ModelConditions conditions)
	{
		ModelNode lCDGOCIAIDK = MHIDGNCKHON(conditions).CHEKEGGJDBL;
		if (lCDGOCIAIDK != null)
		{
			ModelNode lCDGOCIAIDK2 = lCDGOCIAIDK.PKOPJAHFNJG();
			if (lCDGOCIAIDK2 != null)
			{
				int pCAOCHAIBJC = conditions.PCAOCHAIBJC;
				int fOIHIKCEBJF = conditions.FOIHIKCEBJF;
				float num = lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() * (float)pCAOCHAIBJC;
				float num2 = lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK() * (float)pCAOCHAIBJC;
				bool flag = num > num2;
				if ((fOIHIKCEBJF == 1 && !flag) || (fOIHIKCEBJF == 2 && flag))
				{
					lCDGOCIAIDK = lCDGOCIAIDK2;
				}
			}
		}
		else
		{
			switch (OOFFOILONLO)
			{
			case ModelType.KEIDBIOIFGA.MODEL_NULL:
			case ModelType.KEIDBIOIFGA.MODEL_THIS:
				lCDGOCIAIDK = conditions.AFLPHBDFMGA;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
				lCDGOCIAIDK = conditions.EJFOAKCDPHH;
				break;
			case ModelType.KEIDBIOIFGA.MODEL_PARENT:
				lCDGOCIAIDK = conditions.CJELGCJHMHI;
				break;
			default:
				LLLOJBFMONN.Error("DistancePoint: getNode - wrong type: {1}", OOFFOILONLO);
				break;
			}
		}
		return lCDGOCIAIDK;
	}

	private ModelNode GetNode(ModelConditions conditions)
	{
		return MHIDGNCKHON(conditions).Node;
	}

	protected ModelConditions.ModelPositions MJFBOLMAEGG(ModelConditions conditions)
	{
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return conditions.IHJJBIDMEMB;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
		case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
			return conditions.GAIBPAGPEGK;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return conditions.JBNPEMEEMLK;
		default:
			LLLOJBFMONN.Error("ERROR: DistancePoint.DistanceObject.getPosition - unknown model: {0}", OOFFOILONLO);
			return conditions.IHJJBIDMEMB;
		}
	}

	protected Vector3f KEFNOMIKGEN(ModelNode node)
	{
		switch (Frame)
		{
		case PCEPIHHGDJC.DISTANCE_FRAME_CURRENT:
			return node.ICLEOFDKDIF();
		case PCEPIHHGDJC.DISTANCE_FRAME_PREVIOUS:
			return node.FOGHEPNAPLC();
		default:
			LLLOJBFMONN.Error("DistancePoint: getPositionFrame - unknown frame: {0}", Frame);
			return node.ICLEOFDKDIF();
		}
	}

	protected PointNode MHIDGNCKHON(ModelConditions conditions)
	{
		if (conditions.FDELMAHAAJD && OOFFOILONLO != ModelType.KEIDBIOIFGA.MODEL_OTHER &&
			OOFFOILONLO != ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD)
		{
			return PAPCNMHMBOO(conditions);
		}
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return (!conditions.IsPlayer) ? ABDKBCLJAME : IHJJBIDMEMB;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
		case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
			return (!conditions.IsPlayer) ? IHJJBIDMEMB : ABDKBCLJAME;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			LLLOJBFMONN.Error("DistancePoint: getPointNode - taking parent from base model");
			break;
		}
		LLLOJBFMONN.Error("DistancePoint: getPointNode - wrong type: {0}", OOFFOILONLO);
		return null;
	}

	private PointNode PAPCNMHMBOO(ModelConditions conditions)
	{
		ModelObject cBAECAAKAIA = conditions.IHJJBIDMEMB.CBAECAAKAIA;
		List<ChildPoint> list = null;
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			list = ((!conditions.IsPlayer) ? OKJEDLODFCP : BJIPGGAOPDB);
			break;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
		case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
			list = ((!conditions.IsPlayer) ? BJIPGGAOPDB : OKJEDLODFCP);
			break;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return (!conditions.IsPlayer) ? ABDKBCLJAME : IHJJBIDMEMB;
		default:
			LLLOJBFMONN.Error("DistancePoint: getChildPointNode - wrong type: {0}", OOFFOILONLO);
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].BIAONEGEHLB == cBAECAAKAIA)
			{
				return list[i].Point;
			}
		}
		LLLOJBFMONN.Error("DistancePoint: getChildPointNode - no child found");
		return null;
	}

	public void GPGKANDFLNB()
	{
		BJIPGGAOPDB.Clear();
		OKJEDLODFCP.Clear();
	}
}
