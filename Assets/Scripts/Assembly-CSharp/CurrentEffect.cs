using SF2DE.Rendering.Interpolation;
using UnityEngine;

public class CurrentEffect
{
	public Model ACENLMONNPA;

	public ActionEffect LLOLBKJMKNC;

	public GameObject EGJHGBCEPHO;

	public CocosAnimation BHHCMELOEJF;

	public bool stopFollowEffect;

	private FightTransformInterpolation _Interpolation;

	private Vector3 _PreviousAnchor;

	private bool _HasPreviousAnchor;

	private int _LoggedAnchorJumps;

	private int _PreviousDirection;

	public CurrentEffect(Model GIAMLEDNFJD, ActionEffect FNNOHPEMKMB, GameObject GHDAPMGLICD, CocosAnimation EDMCLHEOJGD)
	{
		ACENLMONNPA = GIAMLEDNFJD;
		LLOLBKJMKNC = FNNOHPEMKMB;
		EGJHGBCEPHO = GHDAPMGLICD;
		BHHCMELOEJF = EDMCLHEOJGD;
		stopFollowEffect = false;
		if (LLOLBKJMKNC.DIGCODDLDAD())
		{
			_Interpolation = EGJHGBCEPHO.GetComponent<FightTransformInterpolation>();
			if (_Interpolation == null)
			{
				_Interpolation = EGJHGBCEPHO.AddComponent<FightTransformInterpolation>();
			}
			_Interpolation.Snap(EGJHGBCEPHO.transform.localPosition, EGJHGBCEPHO.transform.localRotation);
		}
		_HasPreviousAnchor = false;
		_LoggedAnchorJumps = 0;
		_PreviousDirection = 0;
	}

	public void HJGPLENNFCK()
	{
		int num = ACENLMONNPA.KFCNPADAMHA();
		ModelConditions kDOGKKGDOBK = ACENLMONNPA.EBABHGHPLFK();
		Vector3f eMAFACPEPDK = Vector3f.op_Implicit(LLOLBKJMKNC.ECJPLFFAMJO().EMGKDOAMBOH(kDOGKKGDOBK));
		Vector3 anchor = new Vector3(eMAFACPEPDK.GILCBJJPKBK(), eMAFACPEPDK.OBIMBNIBEFG(), eMAFACPEPDK.KMFEKANLCFO());
		if (_HasPreviousAnchor)
		{
			float anchorStep = Vector3.Distance(_PreviousAnchor, anchor);
			if ((anchorStep > 40f || (_PreviousDirection != 0 && _PreviousDirection != num)) && _LoggedAnchorJumps < 5)
			{
				Debug.LogWarning("[EffectTransform] follow-jump actor=" + ACENLMONNPA.get_Name() +
					" action=" + LLOLBKJMKNC.get_Name() +
					" sequence=" + LLOLBKJMKNC.EPDMGFELIMC() +
					" step=" + anchorStep +
					" from=" + _PreviousAnchor.x + "," + _PreviousAnchor.y +
					" to=" + anchor.x + "," + anchor.y +
					" direction=" + _PreviousDirection + "->" + num +
					" frame=" + Time.frameCount);
				_LoggedAnchorJumps++;
			}
		}
		_PreviousAnchor = anchor;
		_PreviousDirection = num;
		_HasPreviousAnchor = true;
		Quaternion rotation = _Interpolation.CurrentRotation;
		Vector2f hEJKLMNOLLG = LLOLBKJMKNC.MABFDDNEOGO().HLBBNCBJHGB(kDOGKKGDOBK);
		if (hEJKLMNOLLG.GILCBJJPKBK() != 0f || hEJKLMNOLLG.OBIMBNIBEFG() != 0f)
		{
			hEJKLMNOLLG.JPFALPBDBAP(hEJKLMNOLLG.GILCBJJPKBK() * (float)num);
			hEJKLMNOLLG.IBNFLLGPOLD(hEJKLMNOLLG.OBIMBNIBEFG() * (float)num);
			float z = Vector2f.GetAngle2DDegreeSigned(hEJKLMNOLLG, new Vector2f(1f));
			rotation = Quaternion.Euler(0f, 0f, z);
		}
		_Interpolation.Push(anchor, rotation);
	}
}
