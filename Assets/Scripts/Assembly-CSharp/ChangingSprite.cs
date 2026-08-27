using System;
using UnityEngine;

public class ChangingSprite
{
	public enum MHDKGPHKHIE
	{
		PictureBased = 0,
		AtlasBased = 1,
		ParticleBased = 2,
		None = 3
	}

	private bool JJDBFBGNALI;

	private float AHKMGNOGIJP;

	private float ALNFDKLOOIL;

	private MHDKGPHKHIE _type;

	private CocosAnimation ANHELIPPFLA;

	private Interpolator APCJDEEGPNM = new Interpolator();

	private Interpolator MCHBNCHNFKE = new Interpolator();

	private Interpolator LECGMOJJMGD = new Interpolator();

	private Interpolator NHBGIIHNIBA = new Interpolator();

	private ChanceAndFlag KGFDGKDKOBN = new ChanceAndFlag();

	private ChanceAndFlag JKCNCFJPOKI = new ChanceAndFlag();

	private float CEHGMLJILJH;

	private float GIFPDCPHLMB;

	private float KKMDAOEBJDJ;

	private float FLIBIFJKJOD;

	private float CLPCFGNDNIM;

	private float EMPIEIGBPKB;

	private float MFOJNBJIJIB;

	private float ELEHKLGPKDM;

	private float IIDGEEMOJJA;

	private float NNFMOFODOKF;

	public GameObject NJKCDEJGJLF;

	public ParticleSystem FOECAMJDAOI;

	public float JKEOLJBKDCD
	{
		set
		{
			PFLIOHNFLIM(value);
		}
	}

	public float NJDDPGAKPLJ
	{
		set
		{
			ICFECICLMOJ(value);
		}
	}

	public ChangingSprite(MHDKGPHKHIE LFLGCDNKNJI)
	{
		ALNFDKLOOIL = 0f;
		CEHGMLJILJH = 0f;
		GIFPDCPHLMB = 0f;
		KKMDAOEBJDJ = 0f;
		FLIBIFJKJOD = 0f;
		JJDBFBGNALI = true;
		CLPCFGNDNIM = 0f;
		EMPIEIGBPKB = 0f;
		MFOJNBJIJIB = 0f;
		ELEHKLGPKDM = 0f;
		IIDGEEMOJJA = 0f;
		NNFMOFODOKF = 0f;
		_type = LFLGCDNKNJI;
	}

	public void PFLIOHNFLIM(float value)
	{
		KKMDAOEBJDJ = value;
	}

	public void ICFECICLMOJ(float value)
	{
		FLIBIFJKJOD = value;
	}

	public virtual bool OMHFEGBJDHP(string PMFEIPCHENB, string path, float time, float ILENLCMAMBH, float JMLAKAKDBBL, float FEIHFIPFNKF)
	{
		if (_type != MHDKGPHKHIE.AtlasBased)
		{
			return false;
		}
		AHKMGNOGIJP = ILENLCMAMBH;
		NJKCDEJGJLF = new GameObject(PMFEIPCHENB);
		ANHELIPPFLA = NJKCDEJGJLF.AddComponent<CocosAnimation>();
		if (!ANHELIPPFLA.Init(path + PMFEIPCHENB, true))
		{
			LLLOJBFMONN.Write("Anim NO " + path + PMFEIPCHENB);
			return false;
		}
		ANHELIPPFLA.SetFirstFrame();
		ANHELIPPFLA.set_ChangeSpriteTime(time / 60f);
		EMPIEIGBPKB = time * (float)ANHELIPPFLA.get_TotalFrames() + 1f;
		float x = ANHELIPPFLA.get_AnimationData().BFJEFNHKPJI()[0].PFIECJPOFFB().x;
		float y = ANHELIPPFLA.get_AnimationData().BFJEFNHKPJI()[0].PFIECJPOFFB().y;
		Vector3 localScale = new Vector3(JMLAKAKDBBL / x, FEIHFIPFNKF / y, 1f);
		localScale.x = (float)Math.Round(localScale.x, 4, MidpointRounding.AwayFromZero);
		localScale.y = (float)Math.Round(localScale.y, 4, MidpointRounding.AwayFromZero);
		NJKCDEJGJLF.transform.localScale = localScale;
		return true;
	}

	public virtual void OGBLGCKOCLL(float GCMMAPEFEBG)
	{
		ELEHKLGPKDM = GCMMAPEFEBG;
	}

	public virtual void LDEAPJCKFMP(string GPNPNHFACPO, string ODMCNMJPHFJ, string GAKBMMOOGDB, CocosAnimationData.SpriteFrameCocos PIDBGGLFBCO, float JMLAKAKDBBL, float FEIHFIPFNKF)
	{
		if (_type != MHDKGPHKHIE.PictureBased)
		{
			return;
		}
		Sprite sprite = LocationSpriteCache.PPBEKKDIJKC(GPNPNHFACPO, ODMCNMJPHFJ, GAKBMMOOGDB);
		if (sprite == null)
		{
			LLLOJBFMONN.Write("Pic: {0}", ODMCNMJPHFJ);
			return;
		}
		NJKCDEJGJLF = new GameObject(ODMCNMJPHFJ);
		SpriteRenderer spriteRenderer = NJKCDEJGJLF.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		float num = sprite.rect.size.x;
		float num2 = sprite.rect.size.y;
		IIDGEEMOJJA = 0f;
		NNFMOFODOKF = 0f;
		bool flag = false;
		if (PIDBGGLFBCO != null)
		{
			flag = PIDBGGLFBCO.KGFGOFBMCCG();
			IIDGEEMOJJA = PIDBGGLFBCO.LMJCBAFGAFL().x;
			NNFMOFODOKF = PIDBGGLFBCO.LMJCBAFGAFL().y;
			if (flag)
			{
				float num3 = num;
				num = num2;
				num2 = num3;
			}
			num = ((!(PIDBGGLFBCO.PFIECJPOFFB().x < num)) ? PIDBGGLFBCO.PFIECJPOFFB().x : num);
			num2 = ((!(PIDBGGLFBCO.PFIECJPOFFB().y < num2)) ? PIDBGGLFBCO.PFIECJPOFFB().y : num2);
		}
		Vector3 localPosition = NJKCDEJGJLF.transform.localPosition;
		NJKCDEJGJLF.transform.localPosition = new Vector3(IIDGEEMOJJA, NNFMOFODOKF, localPosition.z);
		Vector3 vector = default(Vector3);
		if (flag)
		{
			NJKCDEJGJLF.transform.Rotate(0f, 0f, 90f);
			vector = new Vector3(FEIHFIPFNKF / num2, JMLAKAKDBBL / num, 1f);
		}
		else
		{
			vector = new Vector3(JMLAKAKDBBL / num, FEIHFIPFNKF / num2, 1f);
		}
		NJKCDEJGJLF.transform.localScale = vector;
	}

	public virtual bool AFPMFHFIBBO(string JIPAAPBPNJM, float FNDOOJNDJDC, float GBCONNBABLL)
	{
		GameObject gameObject = Resources.Load<GameObject>(JIPAAPBPNJM);
		if (gameObject != null && gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.transform.localPosition = new Vector3(FNDOOJNDJDC, GBCONNBABLL, -0.1f);
			FOECAMJDAOI = gameObject2.GetComponent<ParticleSystem>();
			FOECAMJDAOI.Pause();
			return true;
		}
		LLLOJBFMONN.Write("Particles NO " + JIPAAPBPNJM);
		GameObject gameObject3 = new GameObject("NO " + JIPAAPBPNJM);
		FOECAMJDAOI = gameObject3.AddComponent<ParticleSystem>();
		gameObject3.transform.localPosition = new Vector3(FNDOOJNDJDC, GBCONNBABLL, -0.1f);
		return true;
	}

	public virtual bool FJPPLGAABLM(string JIPAAPBPNJM, float FNDOOJNDJDC, float GBCONNBABLL, int HJAHHPHOMDO = 0, int JAJICKINNCP = 0, int FKFCKIDMFCP = 24)
	{
		return true;
	}

	public virtual void SetPosition(float DHDMNHCIPEH, float BGEEALIPKCC)
	{
		KKMDAOEBJDJ = DHDMNHCIPEH;
		FLIBIFJKJOD = BGEEALIPKCC;
	}

	public void NOHGIBJKJNC(float GKIHFPFHKCI, float value, float JENJFNNFGLD)
	{
		APCJDEEGPNM.EIOGKOBGBFK(GKIHFPFHKCI, value, JENJFNNFGLD);
	}

	public void PBDEFHJGBML(float IPCOBJBKNAO)
	{
		APCJDEEGPNM.HJGPLENNFCK(IPCOBJBKNAO);
	}

	public void HMLBMLMDLOP(float GKIHFPFHKCI, float value, float JENJFNNFGLD)
	{
		MCHBNCHNFKE.EIOGKOBGBFK(GKIHFPFHKCI, value, JENJFNNFGLD);
	}

	public void INPLHCAAJKP(float IPCOBJBKNAO)
	{
	}

	public virtual void KEOBIGPEGEO(float GKIHFPFHKCI, float value, float JENJFNNFGLD)
	{
		NHBGIIHNIBA.EIOGKOBGBFK(GKIHFPFHKCI, value, JENJFNNFGLD);
	}

	public virtual void MBGHNIKNNPJ(float IPCOBJBKNAO)
	{
		NHBGIIHNIBA.HJGPLENNFCK(IPCOBJBKNAO);
	}

	public virtual void JNLCGHHDBBE(float LKMBEJFMCHJ, float IAKJEEBPDBE)
	{
		CEHGMLJILJH = LKMBEJFMCHJ;
		GIFPDCPHLMB = IAKJEEBPDBE;
	}

	public virtual void KGJGDKNJPJH(float GKIHFPFHKCI, float value, float JENJFNNFGLD)
	{
		value = Mathf.Max(0f, value);
		value = Mathf.Min(100f, value);
		LECGMOJJMGD.EIOGKOBGBFK(GKIHFPFHKCI, value, JENJFNNFGLD);
	}

	public virtual void CNECHMNCAHM(float IPCOBJBKNAO)
	{
		LECGMOJJMGD.HJGPLENNFCK(IPCOBJBKNAO);
	}

	public virtual void FDMODLLENAE(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		KGFDGKDKOBN.state = true;
		KGFDGKDKOBN.LHNCHOAEGEA = LHNCHOAEGEA;
		KGFDGKDKOBN.KAEPJHHLLPK = KAEPJHHLLPK;
	}

	public virtual void FNPELDEJFGN(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		JKCNCFJPOKI.state = true;
		JKCNCFJPOKI.LHNCHOAEGEA = LHNCHOAEGEA;
		JKCNCFJPOKI.KAEPJHHLLPK = KAEPJHHLLPK;
	}

	public virtual void Render(float KBBLAECAAFG = 1f)
	{
		float num = 1f / 60f * KBBLAECAAFG;
		if (JJDBFBGNALI)
		{
			if (_type == MHDKGPHKHIE.ParticleBased && FOECAMJDAOI != null)
			{
				for (int i = 0; i < 150; i++)
				{
					FOECAMJDAOI.Simulate(Mathf.Max(0f, num * 2f));
				}
			}
			JJDBFBGNALI = false;
		}
		if (_type == MHDKGPHKHIE.ParticleBased && FOECAMJDAOI != null)
		{
			FOECAMJDAOI.Simulate(Mathf.Max(0f, num), true, false);
			return;
		}
		ALNFDKLOOIL += KBBLAECAAFG;
		if (_type == MHDKGPHKHIE.AtlasBased)
		{
			float num2 = UpdateAnimation(KBBLAECAAFG);
			if (num2 > 0f)
			{
				ANHELIPPFLA.Render(1f / 60f * num2);
			}
		}
		KKMDAOEBJDJ += CEHGMLJILJH;
		float kKMDAOEBJDJ = KKMDAOEBJDJ;
		APCJDEEGPNM.HJGPLENNFCK(num);
		kKMDAOEBJDJ += APCJDEEGPNM.OAGPELOHACM();
		FLIBIFJKJOD += GIFPDCPHLMB;
		float fLIBIFJKJOD = FLIBIFJKJOD;
		MCHBNCHNFKE.HJGPLENNFCK(num);
		fLIBIFJKJOD += MCHBNCHNFKE.OAGPELOHACM();
		Vector3 localPosition = NJKCDEJGJLF.transform.localPosition;
		NJKCDEJGJLF.transform.localPosition = new Vector3(kKMDAOEBJDJ + IIDGEEMOJJA, fLIBIFJKJOD + NNFMOFODOKF, localPosition.z);
		if (NHBGIIHNIBA.HNJDHGDLLPD())
		{
			NHBGIIHNIBA.HJGPLENNFCK(num);
			float z = NHBGIIHNIBA.OAGPELOHACM();
			NJKCDEJGJLF.transform.eulerAngles = new Vector3(0f, 0f, z);
		}
		if (LECGMOJJMGD.HNJDHGDLLPD())
		{
			LECGMOJJMGD.HJGPLENNFCK(num);
			SpriteRenderer component = NJKCDEJGJLF.GetComponent<SpriteRenderer>();
			Color color = component.color;
			color.a = 2.55f * LECGMOJJMGD.OAGPELOHACM() / 255f;
			component.color = color;
		}
		if (KGFDGKDKOBN.state)
		{
			if (kKMDAOEBJDJ > KGFDGKDKOBN.KAEPJHHLLPK)
			{
				KKMDAOEBJDJ = KGFDGKDKOBN.LHNCHOAEGEA + (kKMDAOEBJDJ - KGFDGKDKOBN.KAEPJHHLLPK);
			}
			if (kKMDAOEBJDJ < KGFDGKDKOBN.LHNCHOAEGEA)
			{
				KKMDAOEBJDJ = KGFDGKDKOBN.KAEPJHHLLPK - (KGFDGKDKOBN.LHNCHOAEGEA - kKMDAOEBJDJ);
			}
		}
		if (JKCNCFJPOKI.state)
		{
			if (fLIBIFJKJOD > JKCNCFJPOKI.KAEPJHHLLPK)
			{
				FLIBIFJKJOD = JKCNCFJPOKI.LHNCHOAEGEA + (fLIBIFJKJOD - JKCNCFJPOKI.KAEPJHHLLPK);
			}
			if (fLIBIFJKJOD < JKCNCFJPOKI.LHNCHOAEGEA)
			{
				FLIBIFJKJOD = JKCNCFJPOKI.KAEPJHHLLPK - (JKCNCFJPOKI.LHNCHOAEGEA - fLIBIFJKJOD);
			}
		}
	}

	public void BJNAECMLBHL()
	{
	}

	private float UpdateAnimation(float HDJFIPHOLMP)
	{
		if (HDJFIPHOLMP > EMPIEIGBPKB + ELEHKLGPKDM)
		{
			HDJFIPHOLMP -= (float)(int)(HDJFIPHOLMP / (EMPIEIGBPKB + ELEHKLGPKDM)) * (EMPIEIGBPKB + ELEHKLGPKDM);
		}
		if (MFOJNBJIJIB == 0f)
		{
			CLPCFGNDNIM += HDJFIPHOLMP;
			if (CLPCFGNDNIM < EMPIEIGBPKB)
			{
				return HDJFIPHOLMP;
			}
			MFOJNBJIJIB += CLPCFGNDNIM - EMPIEIGBPKB;
			if (MFOJNBJIJIB >= ELEHKLGPKDM)
			{
				float hDJFIPHOLMP = MFOJNBJIJIB - ELEHKLGPKDM;
				CLPCFGNDNIM = 0f;
				MFOJNBJIJIB = 0f;
				return UpdateAnimation(hDJFIPHOLMP);
			}
			return EMPIEIGBPKB - (CLPCFGNDNIM - HDJFIPHOLMP + 1E-05f);
		}
		MFOJNBJIJIB += HDJFIPHOLMP;
		if (MFOJNBJIJIB < ELEHKLGPKDM)
		{
			return 0f;
		}
		float mFOJNBJIJIB = MFOJNBJIJIB;
		CLPCFGNDNIM = 0f;
		MFOJNBJIJIB = 0f;
		return UpdateAnimation(mFOJNBJIJIB - ELEHKLGPKDM);
	}
}
