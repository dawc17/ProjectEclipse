using System.Collections.Generic;
using UnityEngine;

public class ViewerModel
{
	private GameObject _UnityObject;

	private List<ModelObject> INNLAFHKJNI = new List<ModelObject>();

	private ModelObject PHJPLPPEPJN;

	private ModelObject JMHBCFGBHIP;

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public ModelObject EEHFCJHOHNH
	{
		get
		{
			return KBMBCHDBMML();
		}
	}

	public ModelObject IDIFJAHBLIG
	{
		get
		{
			return BNGBCPKIHPD();
		}
	}

	public float DHOHNCJMOBB
	{
		get
		{
			return LGGKNLPOCIH();
		}
	}

	public ViewerModel()
	{
		_UnityObject = new GameObject("ViewerModel");
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public ModelObject KBMBCHDBMML()
	{
		return PHJPLPPEPJN;
	}

	public ModelObject BNGBCPKIHPD()
	{
		return JMHBCFGBHIP;
	}

	public void Clear()
	{
		PHJPLPPEPJN = null;
		JMHBCFGBHIP = null;
		INNLAFHKJNI.Clear();
	}

	public void Init(float GBNPHCHGKDO)
	{
	}

	public int AddModel(ModelObject ACENLMONNPA, Color color, bool IGGHECALMMP)
	{
		if (IGGHECALMMP)
		{
			if (PHJPLPPEPJN == null)
			{
				PHJPLPPEPJN = ACENLMONNPA;
			}
			else
			{
				JMHBCFGBHIP = ACENLMONNPA;
			}
		}
		ACENLMONNPA.get_Model().MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		ACENLMONNPA.get_Model().set_color(color);
		INNLAFHKJNI.Add(ACENLMONNPA);
		return 0;
	}

	public void RemoveModel(int index)
	{
		ModelObject oIEODIEHJMH = INNLAFHKJNI[index];
		if (oIEODIEHJMH == PHJPLPPEPJN)
		{
			PHJPLPPEPJN = null;
		}
		else if (oIEODIEHJMH == JMHBCFGBHIP)
		{
			JMHBCFGBHIP = null;
		}
		INNLAFHKJNI.RemoveAt(index);
	}

	public void NGPIALAGGBI(ModelObject ACENLMONNPA, bool value)
	{
		foreach (ModelObject item in INNLAFHKJNI)
		{
			if (item == ACENLMONNPA)
			{
				item.get_Model().MJNPBMOAFML().SetActive(value);
				break;
			}
		}
	}

	public float LGGKNLPOCIH()
	{
		if (PHJPLPPEPJN != null && JMHBCFGBHIP != null)
		{
			return Vector2f.JOIHAKCICMP(PHJPLPPEPJN.PLBNCDCFPML(), JMHBCFGBHIP.PLBNCDCFPML());
		}
		return 0f;
	}
}
