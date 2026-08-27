using System;
using System.Collections.Generic;
using UnityEngine;

public class Render
{
	private enum HNGOJLFBLME
	{
		Z_DARKNESS = 0,
		Z_RINGOUT = 1,
		Z_HOTGROUND = 2,
		Z_HIT = 3
	}

	public const float CMDONJDIODG = 300f;

	public const string FKJCEEBDDJD = "Textures/fight/pointers/arrow";

	public const float APNMKDCNKOG = 10f;

	private SpriteRenderer EAJGDJLHJFD;

	private SpriteRenderer OEJKNMDNMAP;

	private int EMKDAHLGACK;

	private GameObject GKLOAPHPBPB;

	private LocationSelector EPKHDFIIFIL;

	private GameObject KPBKKJLLKIE;

	private CocosAnimation PHKBOGAICCI;

	private ChangingSprite LFPLGAMAGAC;

	private ChangingSprite MIBKDGNJKIE;

	private SpriteRenderer ALCGBGHPDCL;

	private SpriteRenderer IBDNNMHPOOA;

	private bool NEPJDGDCCFL;

	private float NIKDOKGPFOI;

	private float OGPCOIODJKI;

	private float KKICFAMLAAK;

	private float OEAGONAHCCA;

	private float BGKFFAGMIDE;

	private float JALEODAIDEO;

	private float KAGEIEBNGPO;

	public Location _location;

	public RenderContainer PFELMKLNBMC;

	private List<BloodEffect> FPLGNMICCPH = new List<BloodEffect>();

	private int OBCAJAIBJHP;

	private GameObject _UnityObject;

	public bool LDLFAJGLKBI
	{
		get
		{
			return CHGCKFIHOBG();
		}
		set
		{
			AOMKPKJNIKH(value);
		}
	}

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public ViewerModel KCBHCAMOEAK
	{
		get
		{
			return FPNKBJPKKGB();
		}
	}

	public EffectsContainer HKADBEAJECH
	{
		get
		{
			return GOCPBKNDKMC();
		}
	}

	public EffectsContainer HMLOLLBNBIL
	{
		get
		{
			return GDBMKMFFOCF();
		}
	}

	public float ALOKJEILMLK
	{
		get
		{
			return KMMOLDBJBIG();
		}
	}

	public float MGKJKJFOEHG
	{
		get
		{
			return KGCPMIDNKKI();
		}
	}

	public Render(GameObject PKHKBAJOHHF)
	{
		_UnityObject = new GameObject("Render");
		_UnityObject.transform.SetParent(PKHKBAJOHHF.transform, false);
		_location = null;
	}

	public bool CHGCKFIHOBG()
	{
		return NEPJDGDCCFL;
	}

	public void AOMKPKJNIKH(bool value)
	{
		NEPJDGDCCFL = value;
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public void Init(Location LPJNEDFCBOI)
	{
		_location = LPJNEDFCBOI;
		NIKDOKGPFOI = 1f;
		OGPCOIODJKI = (float)SystemProperties.OACFGEDMCOD() / _location.FEIHFIPFNKF;
		OEAGONAHCCA = (float)SystemProperties.MCGOBLKFGHO() / OGPCOIODJKI;
		BGKFFAGMIDE = OEAGONAHCCA / _location.JMLAKAKDBBL;
		KKICFAMLAAK = (_location.FEIHFIPFNKF / 2f - _location.GBNPHCHGKDO) / 2f;
		MEHIFDPJGDO();
		PIENHGGANDI();
		CCEDPHBFFKK();
		_UnityObject.transform.localScale = new Vector3(1f, -1f, 1f);
	}

	public void GEDDKEKGCBI(Vector3f NAAPALOFBCI, Vector3f IHFFJPLMIAL, int count)
	{
		JBGIBKJIAPD();
		for (int i = 0; i < count; i++)
		{
			string oNEIGMLOGDC = "textures/misc/drop_blood";
			BloodEffect gIMFFDHKFIB = new BloodEffect(IHFFJPLMIAL);
			gIMFFDHKFIB.index = i;
			gIMFFDHKFIB.BIPGHENGCFI = 90;
			gIMFFDHKFIB.CreateSprite(oNEIGMLOGDC, _location.modelsColor);
			gIMFFDHKFIB.SetPosition(NAAPALOFBCI);
			gIMFFDHKFIB.SetScale(0.4f);
			gIMFFDHKFIB.SetParent(PFELMKLNBMC.FPNKBJPKKGB().MJNPBMOAFML());
			FPLGNMICCPH.Add(gIMFFDHKFIB);
		}
		OBCAJAIBJHP = 0;
	}

	public void OBNOJKGAJML()
	{
		if (FPLGNMICCPH.Count > 0)
		{
			for (int i = 0; i < FPLGNMICCPH.Count; i++)
			{
				FPLGNMICCPH[i].Render();
			}
			if (OBCAJAIBJHP > 90)
			{
				JBGIBKJIAPD();
			}
			OBCAJAIBJHP++;
		}
	}

	private void JBGIBKJIAPD()
	{
		for (int i = 0; i < FPLGNMICCPH.Count; i++)
		{
			FPLGNMICCPH[i].AGNODHKEJCJ();
		}
		FPLGNMICCPH.Clear();
	}

	public int OGICJPJDLNN(ModelObject ACENLMONNPA, Color color, bool IGGHECALMMP = true)
	{
		return PFELMKLNBMC.FPNKBJPKKGB().AddModel(ACENLMONNPA, color, IGGHECALMMP);
	}

	public void JACOKNMGNDF()
	{
		List<LocationSelector> hFBEDCGJHLJ = _location.layers;
		foreach (LocationSelector item in hFBEDCGJHLJ)
		{
			item.Render();
		}
		UpdateAdditionalDrawsLayer();
	}

	public void PCIGCDELJAL(float DHDMNHCIPEH, float BGEEALIPKCC)
	{
		if (EMKDAHLGACK > BasicGUI.HNJBADGLFEC())
		{
			EMKDAHLGACK = 0;
		}
		float num = 127.5f;
		Color color = EAJGDJLHJFD.color;
		color.a = (num + num * Mathf.Sin((float)Math.PI / (float)BasicGUI.HNJBADGLFEC() * (float)EMKDAHLGACK)) / 255f;
		EAJGDJLHJFD.color = color;
		EAJGDJLHJFD.transform.localPosition = new Vector3(DHDMNHCIPEH, BGEEALIPKCC, -40f);
		EMKDAHLGACK++;
	}

	public void BHOMOMIPKGC(Vector3f NAAPALOFBCI, Vector3f IHFFJPLMIAL, float time, bool HKNHLNGMOJC, string HJCIKLIPILA, float NOOOCHHKECH)
	{
		float num = Vector2f.GetAngle2DDegreeSigned(IHFFJPLMIAL, new Vector2f(1f));
		PHKBOGAICCI = KPBKKJLLKIE.GetComponent<CocosAnimation>();
		PHKBOGAICCI.Init("textures/effects/fight/" + HJCIKLIPILA, true);
		PHKBOGAICCI.set_Iterations(1);
		PHKBOGAICCI.set_ChangeSpriteTime(time);
		PHKBOGAICCI.set_Autoplay(true);
		KPBKKJLLKIE.transform.localPosition = new Vector3(NAAPALOFBCI.GILCBJJPKBK(), NAAPALOFBCI.OBIMBNIBEFG(), 0f);
		KPBKKJLLKIE.transform.localScale = new Vector3(NOOOCHHKECH, NOOOCHHKECH, NOOOCHHKECH);
		// The fight render root is mirrored vertically (localScale.y = -1).
		// A reflection reverses rotation handedness, so compensate here or an
		// upward strike (for example an uppercut) points the effect downward.
		KPBKKJLLKIE.transform.eulerAngles = new Vector3(0f, 0f, (!float.IsNaN(num)) ? (0f - num) : 0f);
		KPBKKJLLKIE.SetActive(true);
	}

	public void BHIMNPFDCDE(bool value)
	{
		PFELMKLNBMC.MJNPBMOAFML().SetActive(value);
		EAJGDJLHJFD.enabled = value;
	}

	public void UpdatePosition(Vector3f GJKIKGKCGIA, Vector3f JEBIHODAIKM, float DHDMNHCIPEH, float BGEEALIPKCC, float JPJGNKGEHPI = 0f)
	{
		JALEODAIDEO = _location.JMLAKAKDBBL / 2f - GJKIKGKCGIA.GILCBJJPKBK();
		NIKDOKGPFOI = ((!(JPJGNKGEHPI > 0f)) ? KMMOLDBJBIG() : JPJGNKGEHPI);
		float num = 1f;
		if (GameUtils.LEPANPKBBKI().IMHPAHJDAFP > 0f)
		{
			num = GameUtils.LEPANPKBBKI().IMHPAHJDAFP / _location.JMLAKAKDBBL;
		}
		float num2 = BGKFFAGMIDE / num;
		if (NIKDOKGPFOI < num2)
		{
			NIKDOKGPFOI = num2;
			float num3 = OEAGONAHCCA / NIKDOKGPFOI / 2f;
			float num4 = 0f - JALEODAIDEO;
			float num5 = DHDMNHCIPEH - _location.JMLAKAKDBBL / 2f;
			float pHKGOBGNDEC = GameUtils.LEPANPKBBKI().PHKGOBGNDEC;
			float num6 = num5 - num4;
			if (Mathf.Abs(num6) + pHKGOBGNDEC > num3)
			{
				int num7 = ((num6 > 0f) ? 1 : (-1));
				float num8 = (float)(-num7) * (Mathf.Abs(num6) - num3 + pHKGOBGNDEC);
				JALEODAIDEO += num8;
			}
		}
		float kKPKKIJFFMP = GameUtils.LEPANPKBBKI().KKPKKIJFFMP;
		NIKDOKGPFOI = ((!NEPJDGDCCFL) ? Mathf.Max(NIKDOKGPFOI, BGKFFAGMIDE) : BGKFFAGMIDE);
		float num9 = (_location.JMLAKAKDBBL - kKPKKIJFFMP) * NIKDOKGPFOI / 2f - OEAGONAHCCA / 2f;
		JALEODAIDEO *= NIKDOKGPFOI;
		if (Mathf.Abs(JALEODAIDEO) > num9)
		{
			JALEODAIDEO = ((!(JALEODAIDEO < 0f)) ? num9 : (0f - num9));
		}
		List<LocationSelector> hFBEDCGJHLJ = _location.layers;
		foreach (LocationSelector item in hFBEDCGJHLJ)
		{
			if (item.BBELALLBKHH() || item.OGBJCBMNJKC())
			{
				item.SetScale(NIKDOKGPFOI);
			}
			else
			{
				float lIAILCGJBDK = KKICFAMLAAK * (1f - NIKDOKGPFOI);
				item.SetPositionY(lIAILCGJBDK);
			}
			item.SetPositionX(JALEODAIDEO * item.JLBBJEELMGG());
		}
		float dHDMNHCIPEH = JALEODAIDEO - (_location.JMLAKAKDBBL / 2f - DHDMNHCIPEH) * NIKDOKGPFOI;
		float bGEEALIPKCC = _location.gameLayer.MJNPBMOAFML().transform.localPosition.y - 2f * KKICFAMLAAK * NIKDOKGPFOI - 10f;
		PCIGCDELJAL(dHDMNHCIPEH, bGEEALIPKCC);
	}

	public void PGJEGJKFHND(float FNDOOJNDJDC, float GBCONNBABLL)
	{
		_UnityObject.transform.localPosition = new Vector3(FNDOOJNDJDC, GBCONNBABLL);
	}

	public void BFLMJIEIIFM(float HIKKOEOGMEK, float NMMCJGHAJBB, float DKJCJBAGKIL, string AJBGJNMLMKE)
	{
		if (LFPLGAMAGAC == null)
		{
			ChangingSprite fEMGGEAGICG = new ChangingSprite(ChangingSprite.MHDKGPHKHIE.AtlasBased);
			fEMGGEAGICG.OMHFEGBJDHP(AJBGJNMLMKE, "Textures/fight/rules/ringout/", DKJCJBAGKIL, 0f, _location.JMLAKAKDBBL / 2f + HIKKOEOGMEK, 7f + 2f * _location.GBNPHCHGKDO);
			fEMGGEAGICG.SetPosition((0f - _location.JMLAKAKDBBL) / 4f + HIKKOEOGMEK / 2f, 7f + _location.GBNPHCHGKDO - _location.FEIHFIPFNKF / 2f);
			EPKHDFIIFIL.KGACPCKOHBC(fEMGGEAGICG, 1);
			LFPLGAMAGAC = fEMGGEAGICG;
		}
		if (MIBKDGNJKIE == null)
		{
			ChangingSprite fEMGGEAGICG2 = new ChangingSprite(ChangingSprite.MHDKGPHKHIE.AtlasBased);
			fEMGGEAGICG2.OMHFEGBJDHP(AJBGJNMLMKE, "Textures/fight/rules/ringout/", DKJCJBAGKIL, 0f, _location.JMLAKAKDBBL / 2f - NMMCJGHAJBB, 7f + 2f * _location.GBNPHCHGKDO);
			fEMGGEAGICG2.SetPosition(_location.JMLAKAKDBBL / 4f + NMMCJGHAJBB / 2f, 7f + _location.GBNPHCHGKDO - _location.FEIHFIPFNKF / 2f);
			EPKHDFIIFIL.KGACPCKOHBC(fEMGGEAGICG2, 1);
			MIBKDGNJKIE = fEMGGEAGICG2;
		}
	}

	public void DKLLNGOMCHN()
	{
		if (LFPLGAMAGAC != null)
		{
			EPKHDFIIFIL.AJAEMLEHCCH(LFPLGAMAGAC);
			LFPLGAMAGAC = null;
		}
		if (MIBKDGNJKIE != null)
		{
			EPKHDFIIFIL.AJAEMLEHCCH(MIBKDGNJKIE);
			MIBKDGNJKIE = null;
		}
	}

	public void CreatePerkActivationArea(float JMLAKAKDBBL, string KHPKDMGDMAB, string ADONPNOBBDE)
	{
		ALCGBGHPDCL = new GameObject("PerkActivationArea").AddComponent<SpriteRenderer>();
		ALCGBGHPDCL.sprite = ResourcesAndBundles.Load<Sprite>(KHPKDMGDMAB);
		ALCGBGHPDCL.transform.SetParent(EPKHDFIIFIL.MJNPBMOAFML().transform, false);
		ALCGBGHPDCL.gameObject.SetActive(false);
		RectTransform rectTransform = ALCGBGHPDCL.gameObject.AddComponent<RectTransform>();
		Vector2 sizeDelta = rectTransform.sizeDelta;
		rectTransform.localPosition = new Vector2(0f, sizeDelta.y - _location.GBNPHCHGKDO / 8f - _location.FEIHFIPFNKF / 2f);
		rectTransform.localScale = new Vector2(JMLAKAKDBBL / sizeDelta.x, _location.FEIHFIPFNKF * 2f / sizeDelta.y);
		if (!string.IsNullOrEmpty(ADONPNOBBDE))
		{
			IBDNNMHPOOA = new GameObject("PerkActivationAreaIcon").AddComponent<SpriteRenderer>();
			IBDNNMHPOOA.sprite = ResourcesAndBundles.Load<Sprite>(ADONPNOBBDE);
			IBDNNMHPOOA.transform.SetParent(ALCGBGHPDCL.transform, false);
			RectTransform rectTransform2 = IBDNNMHPOOA.gameObject.AddComponent<RectTransform>();
			Vector2 sizeDelta2 = rectTransform2.sizeDelta;
			rectTransform2.localPosition = new Vector2(0f, sizeDelta.x / 3f - _location.GBNPHCHGKDO / 8f - _location.FEIHFIPFNKF / 2f);
			float x = 0.75f * sizeDelta.x / JMLAKAKDBBL;
			float y = 0.75f * sizeDelta.y / (_location.FEIHFIPFNKF * 2f);
			rectTransform2.localScale = new Vector2(x, y);
		}
	}

	public void UpdatePerkActivationArea(float MGMMDGFPBLP, float KGJALFLDIBG)
	{
		KGJALFLDIBG /= 255f;
		if (ALCGBGHPDCL != null)
		{
			ALCGBGHPDCL.gameObject.SetActive(true);
			ALCGBGHPDCL.transform.localPosition = new Vector2(MGMMDGFPBLP, ALCGBGHPDCL.transform.localPosition.y);
			Color color = ALCGBGHPDCL.color;
			color.a = KGJALFLDIBG;
			ALCGBGHPDCL.color = color;
			color = IBDNNMHPOOA.color;
			color.a = KGJALFLDIBG;
			IBDNNMHPOOA.color = color;
		}
	}

	public void GOCNEMPBJIH(float ALCFJHNPDGL)
	{
		if (ALCGBGHPDCL != null)
		{
			float num = ALCGBGHPDCL.color.a - ALCFJHNPDGL / 255f;
			if (num < 0f)
			{
				num = 0f;
			}
			Color color = ALCGBGHPDCL.color;
			color.a = num;
			ALCGBGHPDCL.color = color;
			color = IBDNNMHPOOA.color;
			color.a = num;
			IBDNNMHPOOA.color = color;
		}
	}

	public void NPFHCPAAIFJ()
	{
		if (ALCGBGHPDCL != null)
		{
			UnityEngine.Object.Destroy(IBDNNMHPOOA.gameObject);
			UnityEngine.Object.Destroy(ALCGBGHPDCL.gameObject);
			IBDNNMHPOOA = null;
			ALCGBGHPDCL = null;
		}
	}

	public void DBIHABKLFHP(float KGJALFLDIBG)
	{
		if (OEJKNMDNMAP != null)
		{
			OEJKNMDNMAP.color = new Color(0f, 0f, 0f, KGJALFLDIBG / 255f);
		}
	}

	public void HKOMIIDELBC()
	{
		if (OEJKNMDNMAP == null)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, Color.black);
			texture2D.Apply();
			OEJKNMDNMAP = new GameObject("DarknessPicture").AddComponent<SpriteRenderer>();
			OEJKNMDNMAP.sprite = Sprite.Create(texture2D, Rect.MinMaxRect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
			OEJKNMDNMAP.color = new Color(1f, 1f, 1f, 0f);
			OEJKNMDNMAP.transform.localScale = new Vector2(_location.JMLAKAKDBBL * 1.5f, _location.FEIHFIPFNKF * 3f);
			OEJKNMDNMAP.transform.SetParent(EPKHDFIIFIL.MJNPBMOAFML().transform, false);
		}
	}

	public void OBICGGFDMLN()
	{
		if (OEJKNMDNMAP != null)
		{
			UnityEngine.Object.Destroy(OEJKNMDNMAP.gameObject);
			OEJKNMDNMAP = null;
		}
	}

	public ViewerModel FPNKBJPKKGB()
	{
		return PFELMKLNBMC.FPNKBJPKKGB();
	}

	public EffectsContainer GOCPBKNDKMC()
	{
		return PFELMKLNBMC.GOCPBKNDKMC();
	}

	public EffectsContainer GDBMKMFFOCF()
	{
		return PFELMKLNBMC.GDBMKMFFOCF();
	}

	public void UpdateAdditionalDrawsLayer()
	{
		EPKHDFIIFIL.MJNPBMOAFML().transform.localScale = _location.gameLayer.MJNPBMOAFML().transform.localScale;
		Vector3 localPosition = _location.gameLayer.MJNPBMOAFML().transform.localPosition;
		localPosition.z = EPKHDFIIFIL.AOCHPHIHPIA;
		EPKHDFIIFIL.MJNPBMOAFML().transform.localPosition = localPosition;
		EPKHDFIIFIL.Render();
	}

	public void CDDKOOMODHG(Model ACENLMONNPA)
	{
		PFELMKLNBMC.CDDKOOMODHG(ACENLMONNPA);
	}

	public void NAKJKHLEAEB(Model ACENLMONNPA)
	{
		PFELMKLNBMC.NAKJKHLEAEB(ACENLMONNPA);
	}

	public void IFDHBLGKEHN()
	{
		if (null != PHKBOGAICCI && !PHKBOGAICCI.get_IsWork())
		{
			KPBKKJLLKIE.SetActive(false);
			PHKBOGAICCI = null;
		}
	}

	public void JPPGJBHLAGC()
	{
		if (PFELMKLNBMC != null)
		{
			PFELMKLNBMC.JPPGJBHLAGC();
		}
	}

	public float KMMOLDBJBIG()
	{
		return Mathf.Min(OEAGONAHCCA / (PFELMKLNBMC.FPNKBJPKKGB().LGGKNLPOCIH() + 300f), 1f);
	}

	public float KGCPMIDNKKI()
	{
		return 1f;
	}

	private void PIENHGGANDI()
	{
		EMKDAHLGACK = 0;
		EAJGDJLHJFD = new GameObject("Arrow").AddComponent<SpriteRenderer>();
		EAJGDJLHJFD.flipY = true;
		EAJGDJLHJFD.sprite = ResourcesAndBundles.Load<Sprite>("Textures/fight/pointers/arrow");
		EAJGDJLHJFD.transform.localScale = new Vector3(0.5f, -0.5f, 1f);
		EAJGDJLHJFD.transform.SetParent(_UnityObject.transform, false);
	}

	private void CCEDPHBFFKK()
	{
		string text = "textures/effects/fight/hit_blade";
		KPBKKJLLKIE = new GameObject("ConteynerHit");
		KPBKKJLLKIE.AddComponent<CocosAnimation>();
		KPBKKJLLKIE.transform.localScale = new Vector3(0.7f, 1f, 1f);
		GKLOAPHPBPB = new GameObject("HitLayer");
		GKLOAPHPBPB.transform.localPosition = PFELMKLNBMC.MJNPBMOAFML().transform.localPosition;
		GKLOAPHPBPB.transform.localScale = PFELMKLNBMC.MJNPBMOAFML().transform.localScale;
		GKLOAPHPBPB.transform.SetParent(EPKHDFIIFIL.MJNPBMOAFML().transform, false);
		KPBKKJLLKIE.transform.SetParent(GKLOAPHPBPB.transform, false);
	}

	private void MEHIFDPJGDO()
	{
		PFELMKLNBMC = new RenderContainer();
		PFELMKLNBMC.Init(_location);
		foreach (LocationSelector item in _location.layers)
		{
			item.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		}
		EPKHDFIIFIL = new LocationSelector(_location.layers[_location.layers.Count - 1].AOCHPHIHPIA + -3);
		EPKHDFIIFIL.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		UpdateAdditionalDrawsLayer();
	}

	public void Clear()
	{
		PFELMKLNBMC.Clear();
	}
}
