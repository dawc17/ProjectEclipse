using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
	[SerializeField]
	private Texture2D _spriteSheet;

	private int _currentFrame;

	private Sprite[] _sprites;

	private bool[] _spritesIsRotated;

	private Vector2 _startSize;

	private float NJOBMGDCIMP = 0.1f;

	private float PBDGCPDCAKJ;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init()
	{
		_startSize = GetComponent<Image>().rectTransform.rect.size;
		StartCoroutine("UpdateFrame");
	}

	private IEnumerator UpdateFrame()
	{
		while (true)
		{
			GetComponent<Image>().sprite = _sprites[_currentFrame];
			if (!_spritesIsRotated[_currentFrame])
			{
				GetComponent<Image>().rectTransform.sizeDelta = new Vector2(_startSize.x, _startSize.y);
				base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			else
			{
				GetComponent<Image>().rectTransform.sizeDelta = new Vector2(_startSize.y, _startSize.x);
				base.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
			}
			if (_currentFrame != _sprites.Length - 1)
			{
				yield return new WaitForSeconds(NJOBMGDCIMP);
			}
			else
			{
				yield return new WaitForSeconds(PBDGCPDCAKJ);
			}
			_currentFrame++;
			if (_currentFrame >= _sprites.Length)
			{
				_currentFrame = 0;
			}
		}
	}

	public void SetAnimationTime(float NKNMHPLMFND)
	{
		if (!(NKNMHPLMFND <= 0f))
		{
			NJOBMGDCIMP = NKNMHPLMFND / (float)_sprites.Length;
		}
	}

	public void SetPauseAfterLoop(float AJANNMFPEMN)
	{
		if (!(AJANNMFPEMN <= 0f))
		{
			PBDGCPDCAKJ = AJANNMFPEMN;
		}
	}
}
