using System.Xml;
using UnityEngine;

public class DistancePointFollow : DistancePoint
{
	private bool MJPLGHIKGPG;

	private bool EIBLPPEGAMJ;

	private Vector3 _ComputedPosition;

	public DistancePointFollow()
	{
		EIBLPPEGAMJ = false;
	}

	public DistancePointFollow(XmlNode node)
		: base(node)
	{
		EIBLPPEGAMJ = false;
	}

	public override void Create(XmlNode node)
	{
		base.Create(node);
		MJPLGHIKGPG = node.Attributes["Follow"].ParseBool();
	}

	public override float ILIKNABGPNK(ModelConditions conditions)
	{
		EJDNAPALIDH(conditions);
		if (!conditions.FAHHBNIFAMB)
		{
			return 0f;
		}
		return _ComputedPosition.x;
	}

	public override float MJPKHPNIJGK(ModelConditions conditions)
	{
		EJDNAPALIDH(conditions);
		if (!conditions.FAHHBNIFAMB)
		{
			return 0f;
		}
		return -1f * _ComputedPosition.y;
	}

	public override Vector3 EMGKDOAMBOH(ModelConditions conditions)
	{
		EJDNAPALIDH(conditions);
		return _ComputedPosition;
	}

	private void EJDNAPALIDH(ModelConditions conditions)
	{
		if (MJPLGHIKGPG || !EIBLPPEGAMJ)
		{
			_ComputedPosition = base.EMGKDOAMBOH(conditions);
			EIBLPPEGAMJ = true;
		}
	}
}
