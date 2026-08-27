using System;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

public class PressButton : Button
{
	[Serializable]
	public enum TransitionType
	{
		SHOW_ONE_IMAGE = 0,
		SHOW_BOTH_IMAGE = 1
	}

	[SerializeField]
	private TransitionType transitionType;

	[SerializeField]
	private ResolutionImage normalImage;

	[SerializeField]
	private ResolutionImage pressImage;

	[SerializeField]
	private LayoutElement layoutElement;

	private RectTransform rectTransform;

	private RectTransform DIBDBBCPEGN
	{
		get
		{
			return CNPGGHONDII();
		}
	}

	private RectTransform CNPGGHONDII()
	{
		if (rectTransform == null)
		{
			rectTransform = (RectTransform)base.transform;
		}
		return rectTransform;
	}

	public void SetImages(string AOIODPAABDM, string OMDLMOMCPLG, string texturePath = "")
	{
		if (AOIODPAABDM == null)
		{
			LLLOJBFMONN.Error("PressButton.Init normalImage is null");
			return;
		}
		if (OMDLMOMCPLG == null)
		{
			LLLOJBFMONN.Error("PressButton.Init pressImage is null");
			return;
		}
		if (layoutElement == null)
		{
			LLLOJBFMONN.Error("PressButton.Init layoutElement is null");
			return;
		}
		if (texturePath != string.Empty)
		{
			normalImage.set_TexturePath(texturePath);
			pressImage.set_TexturePath(texturePath);
		}
		normalImage.set_SpriteName(AOIODPAABDM);
		pressImage.set_SpriteName(OMDLMOMCPLG);
		normalImage.SetNativeSize();
		pressImage.SetNativeSize();
		Vector2 size = normalImage.rectTransform.rect.size;
		CNPGGHONDII().sizeDelta = size;
		layoutElement.minWidth = size.x;
		layoutElement.minHeight = size.y;
	}

	protected override void DoStateTransition(SelectionState state, bool PJHFBFHIGNN)
	{
		base.DoStateTransition(state, PJHFBFHIGNN);
		if (state == SelectionState.Pressed)
		{
			NOHEFAKEHEF();
		}
		else
		{
			PHOJODFJOOI();
		}
	}

	private void NOHEFAKEHEF()
	{
		switch (transitionType)
		{
		case TransitionType.SHOW_BOTH_IMAGE:
			if (pressImage != null)
			{
				pressImage.gameObject.SetActive(true);
			}
			return;
		}
		if (pressImage != null)
		{
			pressImage.gameObject.SetActive(true);
		}
		if (normalImage != null)
		{
			normalImage.gameObject.SetActive(false);
		}
	}

	private void PHOJODFJOOI()
	{
		if (pressImage != null)
		{
			pressImage.gameObject.SetActive(false);
		}
		if (normalImage != null)
		{
			normalImage.gameObject.SetActive(true);
		}
	}
}
