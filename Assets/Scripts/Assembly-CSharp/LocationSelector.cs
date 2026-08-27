using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationSelector
{
	public enum OPHMHHMDKOB
	{
		LayerBG = 1,
		LayerGame = 2,
		LayerStatic = 3
	}

	private List<ChangingSprite> KGPMIOGBFKM = new List<ChangingSprite>();

	private float OPIKNCNBCKJ;

	private float BNBMHDOMCCA;

	private bool BFPBLBEIKCI;

	private const float CLLKKNDDECD = -0.01f;

	private float BAOMCHDJHON;

	private GameObject _UnityObject;

	private bool Scaling;

	private int _type;

	private float IOGMPFJOCPE;

	private string Atlas;

	public int AOCHPHIHPIA;

	private CocosAnimationData DMKGOFGGFPJ;

	private List<GameObject> CDMAEGHDKCP = new List<GameObject>();

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public bool AFPAINNBOAI
	{
		get
		{
			return OGBJCBMNJKC();
		}
		set
		{
			NLJHHPCLMBI(value);
		}
	}

	public float Factor
	{
		get
		{
			return JLBBJEELMGG();
		}
		set
		{
			FPFLDAMPALH(value);
		}
	}

	public string LMABGLLMHKH
	{
		get
		{
			return EMNJEHHOBKG();
		}
		set
		{
			LHPOLNGGAFA(value);
		}
	}

	public CocosAnimationData CocosAnimationData
	{
		get
		{
			return ALLFLLFJIGC();
		}
		set
		{
			NJPBFGMGCFC(value);
		}
	}

	public bool EICGCNJOMMI
	{
		get
		{
			return BBELALLBKHH();
		}
	}

	public LocationSelector(int DFIDNHKKNMB)
	{
		AOCHPHIHPIA = DFIDNHKKNMB;
		_UnityObject = new GameObject("Layer");
		_UnityObject.transform.localPosition = new Vector3(0f, 0f, DFIDNHKKNMB);
		_UnityObject.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public bool OGBJCBMNJKC()
	{
		return Scaling;
	}

	public void NLJHHPCLMBI(bool value)
	{
		Scaling = value;
	}

	public int get_Type()
	{
		return _type;
	}

	public void set_Type(int value)
	{
		_type = value;
	}

	public float JLBBJEELMGG()
	{
		return IOGMPFJOCPE;
	}

	public void FPFLDAMPALH(float value)
	{
		IOGMPFJOCPE = value;
	}

	public string EMNJEHHOBKG()
	{
		return Atlas;
	}

	public void LHPOLNGGAFA(string value)
	{
		Atlas = value;
	}

	public CocosAnimationData ALLFLLFJIGC()
	{
		return DMKGOFGGFPJ;
	}

	public void NJPBFGMGCFC(CocosAnimationData value)
	{
		DMKGOFGGFPJ = value;
	}

	public bool BBELALLBKHH()
	{
		return _type == 2;
	}

	public void GDEDCJGMFDK(GameObject CJBKCEPFIAM, int EELGIMCJLAI)
	{
		CJBKCEPFIAM.transform.SetParent(_UnityObject.transform, false);
		Vector3 localPosition = CJBKCEPFIAM.transform.localPosition;
		localPosition.z = BAOMCHDJHON;
		CJBKCEPFIAM.transform.localPosition = localPosition;
		BAOMCHDJHON += -0.01f;
	}

	public void IFAMCLKHNMA(ChangingSprite CJBKCEPFIAM, int EELGIMCJLAI)
	{
		if (!(CJBKCEPFIAM.NJKCDEJGJLF == null))
		{
			CJBKCEPFIAM.NJKCDEJGJLF.transform.SetParent(_UnityObject.transform, false);
			KGPMIOGBFKM.Add(CJBKCEPFIAM);
			Vector3 localPosition = CJBKCEPFIAM.NJKCDEJGJLF.transform.localPosition;
			localPosition.z = BAOMCHDJHON;
			CJBKCEPFIAM.NJKCDEJGJLF.transform.localPosition = localPosition;
			BAOMCHDJHON += -0.01f;
		}
	}

	public void MHCEMJOAPCA(ChangingSprite DDMFNILHHMD, int EELGIMCJLAI)
	{
		DDMFNILHHMD.FOECAMJDAOI.transform.SetParent(_UnityObject.transform, false);
		KGPMIOGBFKM.Add(DDMFNILHHMD);
		Vector3 localPosition = DDMFNILHHMD.FOECAMJDAOI.transform.localPosition;
		localPosition.z = BAOMCHDJHON;
		DDMFNILHHMD.FOECAMJDAOI.transform.localPosition = localPosition;
		BAOMCHDJHON += -0.099999994f;
	}

	public void KGACPCKOHBC(ChangingSprite DDMFNILHHMD, int EELGIMCJLAI)
	{
		DDMFNILHHMD.NJKCDEJGJLF.transform.SetParent(_UnityObject.transform, false);
		KGPMIOGBFKM.Add(DDMFNILHHMD);
		Vector3 localPosition = DDMFNILHHMD.NJKCDEJGJLF.transform.localPosition;
		localPosition.z = BAOMCHDJHON;
		DDMFNILHHMD.NJKCDEJGJLF.transform.localPosition = localPosition;
		BAOMCHDJHON += -0.01f;
	}

	public void AJAEMLEHCCH(ChangingSprite DLGLPGBABHC)
	{
		if (DLGLPGBABHC != null)
		{
			KGPMIOGBFKM.Remove(DLGLPGBABHC);
			UnityEngine.Object.Destroy(DLGLPGBABHC.NJKCDEJGJLF.gameObject);
		}
	}

	public void Render()
	{
		int i = 0;
		for (int count = KGPMIOGBFKM.Count; i < count; i++)
		{
			KGPMIOGBFKM[i].Render(1f / (float)GameUtils.GGBABPJBGJB());
		}
	}

	public void SetScale(float ECDGDBADCKD)
	{
		_UnityObject.transform.localScale = new Vector3(ECDGDBADCKD, ECDGDBADCKD, 1f);
	}

	public void SetPositionX(float LIAILCGJBDK)
	{
		Vector3 localPosition = _UnityObject.transform.localPosition;
		localPosition.x = (float)Math.Round(LIAILCGJBDK, 2, MidpointRounding.AwayFromZero);
		_UnityObject.transform.localPosition = localPosition;
	}

	public void SetPositionY(float LIAILCGJBDK)
	{
		Vector3 localPosition = _UnityObject.transform.localPosition;
		localPosition.y = (float)Math.Round(LIAILCGJBDK, 2, MidpointRounding.AwayFromZero);
		_UnityObject.transform.localPosition = localPosition;
	}
}
