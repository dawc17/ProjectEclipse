using System.Collections.Generic;
using UnityEngine;

public class EffectsContainer
{
	private GameObject _UnityObject;

	private List<Model> _models = new List<Model>();

	private EffectsRunning IGLOMLIOOBM;

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public EffectsRunning EOPFGIDLHKP
	{
		get
		{
			return NFCBNLKLPBK();
		}
	}

	public EffectsContainer()
	{
		IGLOMLIOOBM = null;
		_UnityObject = new GameObject("EffectsContainer");
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public EffectsRunning NFCBNLKLPBK()
	{
		return IGLOMLIOOBM;
	}

	public void init(float GBNPHCHGKDO)
	{
		GOPCPIFMEKO();
	}

	public void GOPCPIFMEKO()
	{
		if (IGLOMLIOOBM == null)
		{
			IGLOMLIOOBM = new EffectsRunning();
			IGLOMLIOOBM.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		}
	}

	public void CEPKAHEAADL()
	{
		_models.Clear();
	}

	public void DHOMHKADCFG()
	{
		IGLOMLIOOBM.DHOMHKADCFG();
	}

	public void NAOIDALEENG(ActionEffect IBODMPMJELJ)
	{
		IGLOMLIOOBM.BGDJDFABJFD(IBODMPMJELJ, IBODMPMJELJ.get_Model());
	}

	public void HNCAGBNDBMA(ActionStopEffect IBODMPMJELJ)
	{
		IGLOMLIOOBM.CNBFDLLLJOF(IBODMPMJELJ, IBODMPMJELJ.get_Model());
	}

	public void PCOEBNDLHKP(ActionStopFollowEffect IBODMPMJELJ)
	{
		IGLOMLIOOBM.stopFollowEffect(IBODMPMJELJ, IBODMPMJELJ.get_Model());
	}

	public void PMAFCJNKFLF()
	{
		IGLOMLIOOBM.PMAFCJNKFLF();
	}
}
