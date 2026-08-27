using UnityEngine;

public class RenderContainer
{
	public enum OLBEPOHIHGA
	{
		Z_EFFECTS_CONTAINER_BACKGROUND = 0,
		Z_VIEWER_MODEL = 1,
		Z_EFFECTS_CONTAINER_FRONT = 2
	}

	private GameObject _UnityObject;

	private ViewerModel MELCPENMECL;

	private EffectsContainer NKGMEJFMGAP;

	private EffectsContainer FBBJHFBEALC;

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

	public RenderContainer()
	{
		_UnityObject = new GameObject("RenderContainer");
		MELCPENMECL = new ViewerModel();
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public ViewerModel FPNKBJPKKGB()
	{
		return MELCPENMECL;
	}

	public EffectsContainer GOCPBKNDKMC()
	{
		return NKGMEJFMGAP;
	}

	public EffectsContainer GDBMKMFFOCF()
	{
		return FBBJHFBEALC;
	}

	private void JCDIJHDFHFN(object data)
	{
		ActionEffect jFJGGMEJDPG = (ActionEffect)data;
		if (jFJGGMEJDPG.JNAALMFCPCN())
		{
			NKGMEJFMGAP.NAOIDALEENG(jFJGGMEJDPG);
		}
		else
		{
			FBBJHFBEALC.NAOIDALEENG(jFJGGMEJDPG);
		}
	}

	private void DGMPPBLAGFN(object data)
	{
		ActionStopEffect iBODMPMJELJ = (ActionStopEffect)data;
		NKGMEJFMGAP.HNCAGBNDBMA(iBODMPMJELJ);
		FBBJHFBEALC.HNCAGBNDBMA(iBODMPMJELJ);
	}

	private void FMIILPBJGAC(object data)
	{
		ActionStopFollowEffect iBODMPMJELJ = (ActionStopFollowEffect)data;
		NKGMEJFMGAP.PCOEBNDLHKP(iBODMPMJELJ);
		FBBJHFBEALC.PCOEBNDLHKP(iBODMPMJELJ);
	}

	public void Init(Location LPJNEDFCBOI)
	{
		FJHEAKJJFIC(LPJNEDFCBOI.GBNPHCHGKDO);
		KCPOPBDIBMM(LPJNEDFCBOI.GBNPHCHGKDO);
		_UnityObject.transform.localPosition = new Vector3((0f - LPJNEDFCBOI.JMLAKAKDBBL) / 2f, (0f - LPJNEDFCBOI.FEIHFIPFNKF) / 2f + LPJNEDFCBOI.GBNPHCHGKDO, 0f);
		_UnityObject.transform.SetParent(LPJNEDFCBOI.gameLayer.MJNPBMOAFML().transform, false);
		Vector3 localScale = _UnityObject.transform.localScale;
		_UnityObject.transform.localScale = new Vector3(localScale.x, localScale.y * -1f, localScale.z);
	}

	public void FJHEAKJJFIC(float GBNPHCHGKDO)
	{
		MELCPENMECL.Init(GBNPHCHGKDO);
		MELCPENMECL.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		MELCPENMECL.MJNPBMOAFML().transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	public void KCPOPBDIBMM(float GBNPHCHGKDO)
	{
		NKGMEJFMGAP = new EffectsContainer();
		NKGMEJFMGAP.init(GBNPHCHGKDO);
		NKGMEJFMGAP.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		NKGMEJFMGAP.MJNPBMOAFML().transform.localPosition = new Vector3(0f, 0f, 0.01f);
		FBBJHFBEALC = new EffectsContainer();
		FBBJHFBEALC.init(GBNPHCHGKDO);
		FBBJHFBEALC.MJNPBMOAFML().transform.SetParent(_UnityObject.transform, false);
		FBBJHFBEALC.MJNPBMOAFML().transform.localPosition = new Vector3(0f, 0f, -0.01f);
	}

	public void Clear()
	{
		MELCPENMECL.Clear();
		NKGMEJFMGAP.CEPKAHEAADL();
		FBBJHFBEALC.CEPKAHEAADL();
	}

	public void CDDKOOMODHG(Model ACENLMONNPA)
	{
		ACENLMONNPA.AddEventListener(7, JCDIJHDFHFN);
		ACENLMONNPA.AddEventListener(8, DGMPPBLAGFN);
		ACENLMONNPA.AddEventListener(9, FMIILPBJGAC);
	}

	public void NAKJKHLEAEB(Model ACENLMONNPA)
	{
		ACENLMONNPA.RemoveEventListener(7, JCDIJHDFHFN);
		ACENLMONNPA.RemoveEventListener(8, DGMPPBLAGFN);
		ACENLMONNPA.RemoveEventListener(9, FMIILPBJGAC);
	}

	public void JPPGJBHLAGC()
	{
		NKGMEJFMGAP.PMAFCJNKFLF();
		FBBJHFBEALC.PMAFCJNKFLF();
	}
}
