using System.Collections.Generic;
using UnityEngine;

public class EffectsRunning
{
	private const string _Path = "Textures/Effects/Magic/";

	private GameObject _UnityObject;

	private List<CurrentEffect> IGLOMLIOOBM = new List<CurrentEffect>();

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public EffectsRunning()
	{
		_UnityObject = new GameObject("EffectsRunning");
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	private void CNBFDLLLJOF(string name, Model ACENLMONNPA)
	{
		int i = 0;
		for (int count = IGLOMLIOOBM.Count; i < count; i++)
		{
			CurrentEffect bNGLFPIBAIM = IGLOMLIOOBM[i];
			if (ACENLMONNPA == bNGLFPIBAIM.ACENLMONNPA && (name == bNGLFPIBAIM.LLOLBKJMKNC.get_Name() || name == string.Empty))
			{
				CNBFDLLLJOF(bNGLFPIBAIM, i);
				break;
			}
		}
	}

	private void CNBFDLLLJOF(CurrentEffect LLOLBKJMKNC, int index)
	{
		Object.Destroy(LLOLBKJMKNC.EGJHGBCEPHO);
		if (LLOLBKJMKNC.ACENLMONNPA != null)
		{
			LLOLBKJMKNC.ACENLMONNPA.MICMDHGOCAN(LLOLBKJMKNC);
		}
		IGLOMLIOOBM.RemoveAt(index);
	}

	private void stopFollowEffect(string name, Model ACENLMONNPA)
	{
		int i = 0;
		for (int count = IGLOMLIOOBM.Count; i < count; i++)
		{
			CurrentEffect bNGLFPIBAIM = IGLOMLIOOBM[i];
			if (ACENLMONNPA == bNGLFPIBAIM.ACENLMONNPA && name == bNGLFPIBAIM.LLOLBKJMKNC.get_Name())
			{
				bNGLFPIBAIM.stopFollowEffect = true;
				break;
			}
		}
	}

	public void BGDJDFABJFD(ActionEffect IBODMPMJELJ, Model ACENLMONNPA)
	{
		ModelConditions dGJJDPIAEAO = ACENLMONNPA.EBABHGHPLFK();
		dGJJDPIAEAO.PCAOCHAIBJC = ACENLMONNPA.OCPMJKIEPIG().KFCNPADAMHA();
		Vector3f eMAFACPEPDK = Vector3f.op_Implicit(IBODMPMJELJ.ECJPLFFAMJO().EMGKDOAMBOH(dGJJDPIAEAO));
		GameObject gameObject = new GameObject(IBODMPMJELJ.get_Name());
		gameObject.transform.localPosition = new Vector3(eMAFACPEPDK.GetX(), eMAFACPEPDK.GetY(), eMAFACPEPDK.GetZ());
		Quaternion attachmentRotation = Quaternion.identity;
		if (IBODMPMJELJ.Attachment != null)
		{
			Vector3 attachmentPosition;
			if (!IBODMPMJELJ.Attachment.TryGetTransform(ACENLMONNPA, out attachmentPosition, out attachmentRotation))
			{
				Debug.LogWarning("[EffectAttach] Missing live anchor for " + IBODMPMJELJ.get_Name() + " on " + ACENLMONNPA.get_Name());
				Object.Destroy(gameObject);
				return;
			}
			gameObject.transform.localPosition = attachmentPosition;
		}
		if (IBODMPMJELJ.JNAALMFCPCN())
			gameObject.transform.localPosition += new Vector3(0f, 0f, 0.1f);
		Vector3 localScale = new Vector3((float)dGJJDPIAEAO.PCAOCHAIBJC * IBODMPMJELJ.GetScaleX(), 0f - IBODMPMJELJ.GetScaleY(), 1f);
		gameObject.transform.localScale = localScale;
		gameObject.transform.localRotation = IBODMPMJELJ.Attachment == null
			? Quaternion.Euler(0f, 0f, IBODMPMJELJ.GetStartRotation()) : attachmentRotation;
		gameObject.transform.SetParent(_UnityObject.transform, false);
		float changeSpriteTime = IBODMPMJELJ.EHJCPFIELAN() / 60f;
		string oNNKJLOGHGH = "Textures/Effects/Magic/" + IBODMPMJELJ.EPDMGFELIMC();
		CocosAnimation cocosAnimation = gameObject.AddComponent<CocosAnimation>();
		bool effectLoaded = cocosAnimation.Init(oNNKJLOGHGH, true);
		// The recovered -10 background order puts effects behind every location
		// sprite in Unity. Keep their order with the arena and use model depth.
		cocosAnimation.SetSortingOrder(IBODMPMJELJ.JNAALMFCPCN() ? 0 : IBODMPMJELJ.GetPriority());
		if (!effectLoaded)
		{
			LLLOJBFMONN.Write("Effect NO " + oNNKJLOGHGH);
		}
		if (IBODMPMJELJ.NCEKKNIMHAG())
		{
			cocosAnimation.set_Iterations(-1);
		}
		else
		{
			cocosAnimation.set_Iterations(1);
		}
		cocosAnimation.SetFirstFrame();
		cocosAnimation.set_ChangeSpriteTime(changeSpriteTime);
		CurrentEffect bNGLFPIBAIM = new CurrentEffect(ACENLMONNPA, IBODMPMJELJ, gameObject, cocosAnimation);
		IGLOMLIOOBM.Add(bNGLFPIBAIM);
		ACENLMONNPA.CMBHIBKEAJH(bNGLFPIBAIM);
	}

	public void DHOMHKADCFG()
	{
		float num = 1f / (float)GameUtils.GGBABPJBGJB();
		int i = 0;
		for (int num2 = IGLOMLIOOBM.Count; i < num2; i++)
		{
			CurrentEffect bNGLFPIBAIM = IGLOMLIOOBM[i];
			if (!bNGLFPIBAIM.BHHCMELOEJF.get_IsWork() || (bNGLFPIBAIM.LLOLBKJMKNC.DIGCODDLDAD() && bNGLFPIBAIM.ACENLMONNPA == null && !bNGLFPIBAIM.stopFollowEffect))
			{
				CNBFDLLLJOF(bNGLFPIBAIM.LLOLBKJMKNC.get_Name(), bNGLFPIBAIM.ACENLMONNPA);
				num2--;
				i--;
				continue;
			}
			if (bNGLFPIBAIM.LLOLBKJMKNC.DIGCODDLDAD() && !bNGLFPIBAIM.stopFollowEffect)
			{
				bNGLFPIBAIM.HJGPLENNFCK();
			}
			bNGLFPIBAIM.BHHCMELOEJF.Render(1f / 60f * num);
		}
	}

	public void CNBFDLLLJOF(ActionStopEffect IBODMPMJELJ, Model ACENLMONNPA)
	{
		CNBFDLLLJOF(IBODMPMJELJ.get_Name(), ACENLMONNPA);
	}

	public void stopFollowEffect(ActionStopFollowEffect IBODMPMJELJ, Model ACENLMONNPA)
	{
		stopFollowEffect(IBODMPMJELJ.get_Name(), ACENLMONNPA);
	}

	public void PMAFCJNKFLF()
	{
		while (IGLOMLIOOBM.Count > 0)
		{
			CNBFDLLLJOF(IGLOMLIOOBM[0], 0);
		}
	}
}
