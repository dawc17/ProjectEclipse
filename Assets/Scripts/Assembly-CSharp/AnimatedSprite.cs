using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
	private List<Sprite> _Frames;

	private SpriteRenderer _SpriteRender;

	private float LICAFNLFJHO;

	private float GCGNEEFNHFK;

	private int EBFEKKMDNIN;

	private int LDDLAKECNEG;

	private bool _IsWork;

	private int NHHKFIHBMKL = -1;

	public List<Sprite> OCFKLCDIEBF
	{
		get
		{
			return get_Frames();
		}
	}

	public float IEOOOFNGMBL
	{
		set
		{
			set_ChangeSpriteTime(value);
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

	public List<Sprite> get_Frames()
	{
		return _Frames;
	}

	public void set_ChangeSpriteTime(float value)
	{
		GCGNEEFNHFK = value;
	}

	public bool get_IsWork()
	{
		return _IsWork;
	}

	public void set_Iterations(int value)
	{
		NHHKFIHBMKL = value;
	}

	public void SetFrames(Sprite[] DHOFFFHGIDL)
	{
		_Frames = new List<Sprite>(DHOFFFHGIDL);
		LDDLAKECNEG = _Frames.Count;
		_IsWork = true;
	}

	public void SetFirstFrame()
	{
		SetSpriteFrame(0);
	}

	private void Awake()
	{
		_SpriteRender = base.gameObject.AddComponent<SpriteRenderer>();
	}

	public void Render(float PPOFNJGPHGP)
	{
		if (_IsWork && !(_SpriteRender == null))
		{
			LICAFNLFJHO += PPOFNJGPHGP;
			if (LICAFNLFJHO >= GCGNEEFNHFK)
			{
				CPBBGPPOOGL();
				LICAFNLFJHO = 0f;
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
			_SpriteRender.sprite = _Frames[DCHCFFFFLLK];
		}
	}
}
