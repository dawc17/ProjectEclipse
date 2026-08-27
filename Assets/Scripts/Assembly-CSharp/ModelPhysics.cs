using System.Collections.Generic;
using UnityEngine;

public class ModelPhysics
{
	public enum AEBCECIKDDD
	{
		onFalling = 0
	}

	private float JKKDLDOCOMM;

	private int KCJPJGLBKKP;

	private float EPHLNOOPFJL;

	private float OCAIKPKEDDN;

	private ModelObject _ModelObject;

	private bool _StartPhysics;

	private List<string> _Names = new List<string>();

	private int FOJHOGPLMNA;

	public bool ADIEJNHGCNH
	{
		get
		{
			return EGNOOKHNFLK();
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
			return PGOFHCBPLOE();
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
		_StartPhysics = false;
		FOJHOGPLMNA = 0;
		KCJPJGLBKKP = PhysicsController.HDEOPNEEMBJ();
		JKKDLDOCOMM = PhysicsController.ECOHOOEMDNH();
	}

	public bool EGNOOKHNFLK()
	{
		return _StartPhysics;
	}

	public List<string> IDAEPMLGFLG()
	{
		return _Names;
	}

	public int PGOFHCBPLOE()
	{
		return FOJHOGPLMNA;
	}

	public void Render()
	{
		TimeStep();
		IterativeProcess();
		if (_StartPhysics)
		{
			FOJHOGPLMNA++;
		}
	}

	public void SetWallShift(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		EPHLNOOPFJL = LHNCHOAEGEA;
		OCAIKPKEDDN = KAEPJHHLLPK;
	}

	public float DBJMBDLKOPM()
	{
		return PhysicsController.KKAJIHOJMPN() / (float)(GameUtils.GGBABPJBGJB() * GameUtils.GGBABPJBGJB());
	}

	public void Start(List<string> NIKHAICFGNM)
	{
		_StartPhysics = true;
		FOJHOGPLMNA = 0;
		_Names.Clear();
		if (NIKHAICFGNM != null)
		{
			_Names.AddRange(NIKHAICFGNM);
		}
		_ModelObject.FLPIFFOGDBF();
	}

	public void Stop()
	{
		_StartPhysics = false;
		FOJHOGPLMNA = 0;
	}

	public void IterativeProcess()
	{
		bool flag = _ModelObject.EDJFLMILEBA();
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		List<ModelEdge> list2 = _ModelObject.BKAPPJMGPKP();
		ModelNode lCDGOCIAIDK = null;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			lCDGOCIAIDK = list[i];
			bool bAINMLLIKOL = lCDGOCIAIDK.MNFDCLJNFEJ() && !lCDGOCIAIDK.BPJFABOAFJK() && (_StartPhysics || lCDGOCIAIDK.NLHFJIEHKMM() || (flag && lCDGOCIAIDK.EDJFLMILEBA()));
			lCDGOCIAIDK.BGDMKGMEIDH(bAINMLLIKOL);
		}
		for (int j = 0; j < KCJPJGLBKKP; j++)
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
			if (!item.BPJFABOAFJK() && (_StartPhysics || item.NLHFJIEHKMM() || (_ModelObject.EDJFLMILEBA() && item.EDJFLMILEBA())))
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
			if (!lCDGOCIAIDK.BPJFABOAFJK() && (_StartPhysics || lCDGOCIAIDK.NLHFJIEHKMM() || (_ModelObject.EDJFLMILEBA() && lCDGOCIAIDK.EDJFLMILEBA())))
			{
				lCDGOCIAIDK.TimeStep(DBJMBDLKOPM());
			}
		}
	}

	private void IterativeLine(ModelEdge ADFIIAJCBHA)
	{
		ModelNode lCDGOCIAIDK = ADFIIAJCBHA.OGLAOHGLBHI();
		ModelNode lCDGOCIAIDK2 = ADFIIAJCBHA.KMHHBEKNHCJ();
		if (lCDGOCIAIDK.NEEJAPDCCMJ())
		{
			IterativeNode(lCDGOCIAIDK);
			if (lCDGOCIAIDK2.NEEJAPDCCMJ())
			{
				IterativeNode(lCDGOCIAIDK2);
			}
			ADFIIAJCBHA.Iterative();
		}
		else if (lCDGOCIAIDK2.NEEJAPDCCMJ())
		{
			IterativeNode(lCDGOCIAIDK2);
			ADFIIAJCBHA.Iterative();
		}
	}

	private void IterativeNode(ModelNode node)
	{
		Vector3f eMAFACPEPDK = node.ICLEOFDKDIF();
		if (eMAFACPEPDK.OBIMBNIBEFG() >= 0f)
		{
			GetFrictionForce(node);
		}
		if (EPHLNOOPFJL != OCAIKPKEDDN)
		{
			if (eMAFACPEPDK.GILCBJJPKBK() < EPHLNOOPFJL)
			{
				eMAFACPEPDK.JPFALPBDBAP(EPHLNOOPFJL);
			}
			else if (OCAIKPKEDDN < eMAFACPEPDK.GILCBJJPKBK())
			{
				eMAFACPEPDK.JPFALPBDBAP(OCAIKPKEDDN);
			}
		}
	}

	private void GetFrictionForce(ModelNode node)
	{
		if (node.PENPLGPDNIF() && EPHLNOOPFJL != OCAIKPKEDDN)
		{
			Vector3f eMAFACPEPDK = node.FOGHEPNAPLC();
			Vector3f eMAFACPEPDK2 = node.ICLEOFDKDIF();
			float num = eMAFACPEPDK2.GILCBJJPKBK() - eMAFACPEPDK.GILCBJJPKBK();
			float num2 = eMAFACPEPDK2.KMFEKANLCFO() - eMAFACPEPDK.KMFEKANLCFO();
			float num3 = num * num + num2 * num2;
			float num4 = eMAFACPEPDK2.OBIMBNIBEFG() * JKKDLDOCOMM;
			eMAFACPEPDK2.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK());
			eMAFACPEPDK2.IBNFLLGPOLD(0f);
			eMAFACPEPDK2.set_Z(eMAFACPEPDK.KMFEKANLCFO());
			if (num4 * num4 < num3)
			{
				num4 = 1f - num4 / Mathf.Sqrt(num3);
				eMAFACPEPDK2.Add(num * num4, 0f, num2 * num4);
			}
		}
	}
}
