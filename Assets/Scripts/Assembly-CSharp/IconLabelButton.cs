using Nekki.SF2.GUI;
using UnityEngine;

[RequireComponent(typeof(ResolutionImage))]
public class IconLabelButton : LabelButton
{
	[SerializeField]
	private ResolutionImage _icon;

	public ResolutionImage MJBPMLCLMFN
	{
		get
		{
			return get_Icon();
		}
	}

	public ResolutionImage get_Icon()
	{
		return _icon;
	}

	public override void SetOpacity(float KGJALFLDIBG)
	{
		if (_icon != null)
		{
			Color color = _icon.color;
			color.a = KGJALFLDIBG;
			_icon.color = color;
		}
		base.SetOpacity(KGJALFLDIBG);
	}
}
