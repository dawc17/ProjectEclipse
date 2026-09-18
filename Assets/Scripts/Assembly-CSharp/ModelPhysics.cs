using System.Collections.Generic;
using UnityEngine;

public class ModelPhysics
{
	public enum AEBCECIKDDD
	{
		onFalling = 0
	}

	private float _FrictionForce;

	private int _Iterative;

	private float EPHLNOOPFJL;

	private float OCAIKPKEDDN;

	private ModelObject _ModelObject;

	private bool _IsPhysics;

	private List<string> _Names = new List<string>();

	private int _Frame;

	public bool ADIEJNHGCNH
	{
		get
		{
			return IsPhysics();
		}
	}

	public List<string> Names
	{
		get
		{
			return IDAEPMLGFLG();
		}
	}

	public int NFKBFGIACOP
	{
		get
		{
			return GetFrame();
		}
	}

	public float ILJNBIKEDDP
	{
		get
		{
			return DBJMBDLKOPM();
		}
	}

	public ModelPhysics(ModelObject OECPEDPMKCD)
	{
		EPHLNOOPFJL = 0f;
		OCAIKPKEDDN = 0f;
		_ModelObject = OECPEDPMKCD;
		_IsPhysics = false;
		_Frame = 0;
		_Iterative = PhysicsController.GetIterativeProcess();
		_FrictionForce = PhysicsController.GetFrictionForce();
	}

	public bool IsPhysics()
	{
		return _IsPhysics;
	}

	public List<string> IDAEPMLGFLG()
	{
		return _Names;
	}

	public int GetFrame()
	{
		return _Frame;
	}

	public void Render()
	{
		TimeStep();
		IterativeProcess();
		if (_IsPhysics)
		{
			_Frame++;
		}
	}

	public void SetWallShift(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		EPHLNOOPFJL = LHNCHOAEGEA;
		OCAIKPKEDDN = KAEPJHHLLPK;
	}

	public float DBJMBDLKOPM()
	{
		return PhysicsController.GetGravity() / (float)(GameUtils.GGBABPJBGJB() * GameUtils.GGBABPJBGJB());
	}

	public void Start(List<string> NIKHAICFGNM)
	{
		_IsPhysics = true;
		_Frame = 0;
		_Names.Clear();
		if (NIKHAICFGNM != null)
		{
			_Names.AddRange(NIKHAICFGNM);
		}
		_ModelObject.FLPIFFOGDBF();
	}

	public void Stop()
	{
		_IsPhysics = false;
		_Frame = 0;
	}

	public void IterativeProcess()
	{
		bool flag = _ModelObject.IsShock();
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		List<ModelEdge> list2 = _ModelObject.BKAPPJMGPKP();
		ModelNode lCDGOCIAIDK = null;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			lCDGOCIAIDK = list[i];
			bool bAINMLLIKOL = lCDGOCIAIDK.IsNode() && !lCDGOCIAIDK.IsFixedAndIsNotNode() && (_IsPhysics || lCDGOCIAIDK.IsPhysics() || (flag && lCDGOCIAIDK.IsShock()));
			lCDGOCIAIDK.BGDMKGMEIDH(bAINMLLIKOL);
		}
		for (int j = 0; j < _Iterative; j++)
		{
			ModelEdge nAKBKCDKEHF = null;
			count = list2.Count;
			for (int k = 0; k < count; k++)
			{
				nAKBKCDKEHF = list2[k];
				if (!flag || !nAKBKCDKEHF.EDJFLMILEBA())
				{
					IterativeLine(nAKBKCDKEHF);
				}
			}
		}
	}

	public void ChangeSpeed(float ELDDBMFEFIP)
	{
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			if (!item.IsFixedAndIsNotNode() && (_IsPhysics || item.IsPhysics() || (_ModelObject.IsShock() && item.IsShock())))
			{
				item.ChangeSpeed(ELDDBMFEFIP);
			}
		}
	}

	private void TimeStep()
	{
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		ModelNode lCDGOCIAIDK = null;
		for (int i = 0; i < list.Count; i++)
		{
			lCDGOCIAIDK = list[i];
			if (!lCDGOCIAIDK.IsFixedAndIsNotNode() && (_IsPhysics || lCDGOCIAIDK.IsPhysics() || (_ModelObject.IsShock() && lCDGOCIAIDK.IsShock())))
			{
				lCDGOCIAIDK.TimeStep(DBJMBDLKOPM());
			}
		}
	}

	private void IterativeLine(ModelEdge Edge)
	{
		ModelNode lCDGOCIAIDK = Edge.GetStartNode();
		ModelNode lCDGOCIAIDK2 = Edge.GetEndNode();
		if (lCDGOCIAIDK.NEEJAPDCCMJ())
		{
			IterativeNode(lCDGOCIAIDK);
			if (lCDGOCIAIDK2.NEEJAPDCCMJ())
			{
				IterativeNode(lCDGOCIAIDK2);
			}
			Edge.Iterative();
		}
		else if (lCDGOCIAIDK2.NEEJAPDCCMJ())
		{
			IterativeNode(lCDGOCIAIDK2);
			Edge.Iterative();
		}
	}

	private void IterativeNode(ModelNode node)
	{
		Vector3f eMAFACPEPDK = node.GetStart();
		if (eMAFACPEPDK.GetY() >= 0f)
		{
			GetFrictionForce(node);
		}
		if (EPHLNOOPFJL != OCAIKPKEDDN)
		{
			if (eMAFACPEPDK.GetX() < EPHLNOOPFJL)
			{
				eMAFACPEPDK.SetX(EPHLNOOPFJL);
			}
			else if (OCAIKPKEDDN < eMAFACPEPDK.GetX())
			{
				eMAFACPEPDK.SetX(OCAIKPKEDDN);
			}
		}
	}

	private void GetFrictionForce(ModelNode node)
	{
		if (node.IsCollisible() && EPHLNOOPFJL != OCAIKPKEDDN)
		{
			Vector3f eMAFACPEPDK = node.GetEnd();
			Vector3f eMAFACPEPDK2 = node.GetStart();
			float num = eMAFACPEPDK2.GetX() - eMAFACPEPDK.GetX();
			float num2 = eMAFACPEPDK2.GetZ() - eMAFACPEPDK.GetZ();
			float num3 = num * num + num2 * num2;
			float num4 = eMAFACPEPDK2.GetY() * _FrictionForce;
			eMAFACPEPDK2.SetX(eMAFACPEPDK.GetX());
			eMAFACPEPDK2.SetY(0f);
			eMAFACPEPDK2.SetZ(eMAFACPEPDK.GetZ());
			if (num4 * num4 < num3)
			{
				num4 = 1f - num4 / Mathf.Sqrt(num3);
				eMAFACPEPDK2.Add(num * num4, 0f, num2 * num4);
			}
		}
	}
}
