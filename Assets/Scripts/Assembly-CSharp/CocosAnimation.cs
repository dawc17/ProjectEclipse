using UnityEngine;

public class CocosAnimation : MonoBehaviour
{
	private bool EEKENEDOAPH;

	private float LICAFNLFJHO;

	private float GCGNEEFNHFK = 0.03f;

	private CocosAnimationData _Animation;

	private GameObject CHDIDLJNAHI;

	private SpriteRenderer _SpriteRender;

	private int EBFEKKMDNIN;

	private int LDDLAKECNEG;

	private bool _IsWork;

	private int NHHKFIHBMKL = -1;

	public bool GDMEDOKBKJC
	{
		get
		{
			return get_Autoplay();
		}
		set
		{
			set_Autoplay(value);
		}
	}

	public float IEOOOFNGMBL
	{
		set
		{
			set_ChangeSpriteTime(value);
		}
	}

	public CocosAnimationData JKHHHCNJIJJ
	{
		get
		{
			return get_AnimationData();
		}
	}

	public int FLNLMIHEDCI
	{
		get
		{
			return get_TotalFrames();
		}
	}

	public bool OMJMJIBPGDN
	{
		get
		{
			return get_IsWork();
		}
	}

	public int BEJMHOGHCPA
	{
		set
		{
			set_Iterations(value);
		}
	}

	public bool get_Autoplay()
	{
		return EEKENEDOAPH;
	}

	public void set_Autoplay(bool value)
	{
		EEKENEDOAPH = value;
	}

	public void set_ChangeSpriteTime(float value)
	{
		GCGNEEFNHFK = value;
	}

	public CocosAnimationData get_AnimationData()
	{
		return _Animation;
	}

	public int get_TotalFrames()
	{
		return LDDLAKECNEG;
	}

	public bool get_IsWork()
	{
		return _IsWork;
	}

	public void set_Iterations(int value)
	{
		NHHKFIHBMKL = value;
	}

	public void SetSortingOrder(int value)
	{
		if (_SpriteRender != null)
			_SpriteRender.sortingOrder = value;
	}

	public bool Init(string ONEIGMLOGDC, bool MPMHHEMGHOJ)
	{
		_Animation = CocosAnimationData.Create(ONEIGMLOGDC + "_xml", MPMHHEMGHOJ);
		if (_Animation == null)
		{
			return false;
		}
		if (CHDIDLJNAHI == null)
		{
			CHDIDLJNAHI = new GameObject("Child");
			CHDIDLJNAHI.transform.SetParent(base.transform, false);
			_SpriteRender = CHDIDLJNAHI.AddComponent<SpriteRenderer>();
		}
		_Animation.AIFNJAPCCII();
		_Animation.JBPCHMAGDMI();
		LDDLAKECNEG = _Animation.BFJEFNHKPJI().Count;
		_IsWork = true;
		return true;
	}

	public void SetFirstFrame()
	{
		SetSpriteFrame(0);
	}

	private void Update()
	{
		if (EEKENEDOAPH)
		{
			Render(Time.deltaTime);
		}
	}

	public void Render(float PPOFNJGPHGP)
	{
		if (_IsWork && !(_SpriteRender == null))
		{
			LICAFNLFJHO += PPOFNJGPHGP;
			while (LICAFNLFJHO >= GCGNEEFNHFK)
			{
				CPBBGPPOOGL();
				LICAFNLFJHO -= GCGNEEFNHFK;
			}
		}
	}

	private void CPBBGPPOOGL()
	{
		SetSpriteFrame(EBFEKKMDNIN);
		EBFEKKMDNIN++;
		if (EBFEKKMDNIN < LDDLAKECNEG)
		{
			return;
		}
		EBFEKKMDNIN = 0;
		if (NHHKFIHBMKL != -1)
		{
			NHHKFIHBMKL--;
			if (NHHKFIHBMKL <= 0)
			{
				_IsWork = false;
			}
		}
	}

	private void SetSpriteFrame(int DCHCFFFFLLK)
	{
		if (DCHCFFFFLLK < LDDLAKECNEG)
		{
			CocosAnimationData.SpriteFrameCocos pBAHNJDFMBO = _Animation.BFJEFNHKPJI()[DCHCFFFFLLK];
			_SpriteRender.sprite = pBAHNJDFMBO.HJADPLOLOBH();
			CHDIDLJNAHI.transform.localEulerAngles = new Vector3(0f, 0f, pBAHNJDFMBO.KGFGOFBMCCG() ? 90 : 0);
			// Bundle extraction converted atlas frames to standalone sprites and
			// reconstructed their plist offsets with the opposite sign. Comparison
			// against the original 1536 metadata confirms this across the recovered
			// effect library. Convert back to the Cocos convention before applying
			// the frame offset; otherwise differently trimmed frames visibly shuffle.
			Vector2 recoveredOffset = pBAHNJDFMBO.LMJCBAFGAFL();
			CHDIDLJNAHI.transform.localPosition = new Vector3(-recoveredOffset.x, -recoveredOffset.y, 0f);
		}
	}
}
