using Nekki.SF2.GUI;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	[SerializeField]
	private ResolutionImage _arrowImg;

	private const float EJDJBNBIMCO = 24f;

	private const float AGANNALIGCA = 0.8f;

	protected float PPHDNBOHHOP;

	protected float IAAJJHIGNDK;

	protected int MNLLCAPJFPF;

	protected int PEEOHAPHGIK;

	protected bool _animationUp;

	public void Init(float AONLJLDPMEE = 24f)
	{
		PPHDNBOHHOP = AONLJLDPMEE;
		IAAJJHIGNDK = 0.8f;
		MNLLCAPJFPF = (int)(PPHDNBOHHOP / 0.8f);
	}

	private void Update()
	{
		if (_arrowImg == null)
		{
			return;
		}
		_arrowImg.transform.BGNJGIACJBG(_arrowImg.transform.localPosition.y - ((!_animationUp) ? (0f - IAAJJHIGNDK) : IAAJJHIGNDK));
		if (_animationUp)
		{
			if (PEEOHAPHGIK < MNLLCAPJFPF)
			{
				PEEOHAPHGIK++;
			}
			else
			{
				_animationUp = false;
			}
		}
		else if (PEEOHAPHGIK > 0)
		{
			PEEOHAPHGIK--;
		}
		else
		{
			_animationUp = true;
		}
	}
}
