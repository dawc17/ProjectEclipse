using System.Collections.Generic;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Fight;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

public class Camera : global::EventDispatcher<object>
{
	private class IODNDCNLGEL
	{
		public Vector3f NAAPALOFBCI = new Vector3f();

		public Vector3f IHFFJPLMIAL = new Vector3f();

		public int count;

		public bool isActive;
	}

	public enum KCEJHLIBPBL
	{
		onPauseFight = 0,
		onResumeFight = 1,
		onStopEffect = 2
	}

	private const float PNFKEBICIMM = 200f;

	private const float GKJLEIPCADD = 50f;

	private IODNDCNLGEL JOGMEKGACJL = new IODNDCNLGEL();

	private Render BMBGCIEFJGB;

	private GameUtils.HitEffect IONLHJIDACJ;

	private GameUtils.ZoomEffect OOFFFLEFKFA;

	private List<Model> _models = new List<Model>();

	private ModelNode NGOEHKEKBIL;

	private ModelNode JNBAHPMBLOL;

	private Location _location;

	private ModelNode CIJJBMDDAFL;

	private ModelNode BGFPBMFKFGJ;

	private PreFight _preFight;

	private bool PMNEFPDLPCC;

	private bool HLDMKKKKAMI;

	private float LLLNHELEKNF;

	private bool OHNBKMHOMJI;

	private float BIPHAGJDGOL;

	private float EPIPOLDCCHD;

	private bool NOLKMEPOJIE;

	private int showZoomEffectCount;

	private bool PMJCGFONEPA;

	public GameController OJINMMFLEEB;

	private GameObject _UnityObject;

	private readonly FightCameraInterpolation _RenderInterpolation = new FightCameraInterpolation();

	public Render DAAACGKPJAL
	{
		get
		{
			return KKFIJLOMOJI();
		}
	}

	public PreFight DJDMFJJBCEN
	{
		get
		{
			return MCLEFJNHJIK();
		}
	}

	public bool FPCPJGBCFAA
	{
		get
		{
			return LIPGMBDIPBB();
		}
		set
		{
			CLOBNBAHAHF(value);
		}
	}

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public Vector3f EDJMMFAMBOD
	{
		get
		{
			return HOKLGMEOMEI();
		}
	}

	public Vector3f CIFKJPOJHOL
	{
		get
		{
			return NPJHOCJIPDL();
		}
		set
		{
			MHKHHEMJFOK(value);
		}
	}

	public Camera(Transform PKHKBAJOHHF)
	{
		CreateUnityObject(PKHKBAJOHHF);
		NGOEHKEKBIL = new ModelNode("Camera");
		JNBAHPMBLOL = new ModelNode("Position");
		BMBGCIEFJGB = null;
		_preFight = null;
		_location = null;
		CIJJBMDDAFL = null;
		HLDMKKKKAMI = false;
		LLLNHELEKNF = 0f;
		OHNBKMHOMJI = false;
		BIPHAGJDGOL = 0f;
		EPIPOLDCCHD = 0f;
		IONLHJIDACJ = null;
		showZoomEffectCount = 0;
		PMNEFPDLPCC = false;
		PMJCGFONEPA = true;
	}

	public Render KKFIJLOMOJI()
	{
		return BMBGCIEFJGB;
	}

	public PreFight MCLEFJNHJIK()
	{
		return _preFight;
	}

	public bool LIPGMBDIPBB()
	{
		return PMJCGFONEPA;
	}

	public void CLOBNBAHAHF(bool value)
	{
		PMJCGFONEPA = value;
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	private float FGLAIPPLINB()
	{
		float result = 1f;
		if (!GraphicsController.LargeControlsEnabled())
		{
			result = 25f / 33f;
		}
		return result;
	}

	private void UpdateCameraPosition()
	{
		NGOEHKEKBIL.SetEnd();
		ModelObject oIEODIEHJMH = BMBGCIEFJGB.FPNKBJPKKGB().KBMBCHDBMML();
		ModelObject oIEODIEHJMH2 = BMBGCIEFJGB.FPNKBJPKKGB().BNGBCPKIHPD();
		if (oIEODIEHJMH != null && oIEODIEHJMH2 != null)
		{
			NGOEHKEKBIL.SetStart(Model.MHFFCMKNIKM(oIEODIEHJMH, oIEODIEHJMH2));
		}
	}

	private void CameraUpdate()
	{
		if (PMJCGFONEPA)
		{
			JNBAHPMBLOL.TimeStep(0f);
			Vector3f eMAFACPEPDK = new Vector3f(NGOEHKEKBIL.GetEnd());
			Vector3f eMAFACPEPDK2 = new Vector3f(NGOEHKEKBIL.GetStart());
			Vector3f eMAFACPEPDK3 = new Vector3f(JNBAHPMBLOL.GetEnd());
			Vector3f eMAFACPEPDK4 = new Vector3f(JNBAHPMBLOL.GetStart());
			float num = 0f;
			eMAFACPEPDK4.SetZ(num);
			num = num;
			eMAFACPEPDK3.SetZ(num);
			num = num;
			eMAFACPEPDK2.SetZ(num);
			eMAFACPEPDK.SetZ(num);
			Vector3f aKKEJFKBIHF = Vector3f.MJOKEBGPHKB(eMAFACPEPDK2, eMAFACPEPDK);
			Vector3f nBMEGFBPGFE = Vector3f.PHEFFKMOOCM(eMAFACPEPDK3, aKKEJFKBIHF);
			Vector3f nBMEGFBPGFE2 = Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, eMAFACPEPDK4);
			Vector3f eMAFACPEPDK5 = Vector3f.MJOKEBGPHKB(eMAFACPEPDK2, eMAFACPEPDK4);
			eMAFACPEPDK5.Multiply(0.15f);
			Vector3f eMAFACPEPDK6 = Vector3f.PHEFFKMOOCM(nBMEGFBPGFE2, eMAFACPEPDK5);
			if (eMAFACPEPDK6.IGJNMAOKEKK() > 200f)
			{
				eMAFACPEPDK6.NBDMEIKNJBG();
				eMAFACPEPDK6.Multiply(200f);
			}
			eMAFACPEPDK4.Add(eMAFACPEPDK6);
			Vector3f eMAFACPEPDK7 = Vector3f.MJOKEBGPHKB(eMAFACPEPDK4, eMAFACPEPDK3);
			float num2 = eMAFACPEPDK7.IGJNMAOKEKK();
			if (num2 > 50f)
			{
				eMAFACPEPDK7.Multiply(50f / num2);
				eMAFACPEPDK4 = Vector3f.PHEFFKMOOCM(eMAFACPEPDK3, eMAFACPEPDK7);
			}
			JNBAHPMBLOL.SetStart(eMAFACPEPDK4);
		}
	}

	private void DrawPosition()
	{
		Vector3f jEBIHODAIKM = NGOEHKEKBIL.GetStart();
		Vector3f eMAFACPEPDK = CIJJBMDDAFL.GetStart();
		if (NOLKMEPOJIE)
		{
			BMBGCIEFJGB.UpdatePosition(JNBAHPMBLOL.GetStart(), jEBIHODAIKM, eMAFACPEPDK.GetX(), eMAFACPEPDK.GetY(), OOFFFLEFKFA.ALOKJEILMLK);
		}
		else
		{
			BMBGCIEFJGB.UpdatePosition(JNBAHPMBLOL.GetStart(), jEBIHODAIKM, eMAFACPEPDK.GetX(), eMAFACPEPDK.GetY());
		}
	}

	private void DrawInterpolatedPosition(float alpha)
	{
		if (CIJJBMDDAFL == null)
		{
			return;
		}
		_RenderInterpolation.SamplePositions(JNBAHPMBLOL, NGOEHKEKBIL, CIJJBMDDAFL, alpha);
		if (NOLKMEPOJIE)
		{
			float zoomScale = _RenderInterpolation.SampleZoomScale(alpha);
			BMBGCIEFJGB.UpdatePosition(
				_RenderInterpolation.CameraPosition,
				_RenderInterpolation.CameraTarget,
				_RenderInterpolation.FocusPosition.GetX(),
				_RenderInterpolation.FocusPosition.GetY(),
				zoomScale);
		}
		else
		{
			BMBGCIEFJGB.UpdatePosition(
				_RenderInterpolation.CameraPosition,
				_RenderInterpolation.CameraTarget,
				_RenderInterpolation.FocusPosition.GetX(),
				_RenderInterpolation.FocusPosition.GetY());
		}
	}

	private void DrawQuakeEffect()
	{
		if (OHNBKMHOMJI && IONLHJIDACJ != null)
		{
			float ePIPOLDCCHD = EPIPOLDCCHD;
			float num = EPIPOLDCCHD - BIPHAGJDGOL;
			float strength = IONLHJIDACJ.Type == "CriticalHit" ? Eclipse.UI.AccessibilitySettings.CriticalShake : 1f;
			float fMICELIGLPG = IONLHJIDACJ.FMICELIGLPG * strength;
			float pPKAMOILNLN = IONLHJIDACJ.PPKAMOILNLN * strength;
			float kFEMKHHANDC = IONLHJIDACJ.KFEMKHHANDC;
			float gGJBPLHAHFH = IONLHJIDACJ.GGJBPLHAHFH;
			float num2 = Mathf.Sin(kFEMKHHANDC * num) * fMICELIGLPG * (ePIPOLDCCHD - num) / ePIPOLDCCHD;
			float num3 = Mathf.Sin(gGJBPLHAHFH * num) * pPKAMOILNLN * (ePIPOLDCCHD - num) / ePIPOLDCCHD;
			num2 *= SystemProperties.NHIDNIPGCPC;
			num3 *= SystemProperties.NHIDNIPGCPC;
			BMBGCIEFJGB.PGJEGJKFHND(num2, num3);
		}
	}

	private void DrawInterpolatedQuakeEffect(float alpha)
	{
		if (OHNBKMHOMJI && IONLHJIDACJ != null)
		{
			float duration = EPIPOLDCCHD;
			float elapsed = Mathf.Max(0f, EPIPOLDCCHD - BIPHAGJDGOL - (1f - alpha));
			float x = Mathf.Sin(IONLHJIDACJ.KFEMKHHANDC * elapsed) * IONLHJIDACJ.FMICELIGLPG * (duration - elapsed) / duration;
			float y = Mathf.Sin(IONLHJIDACJ.GGJBPLHAHFH * elapsed) * IONLHJIDACJ.PPKAMOILNLN * (duration - elapsed) / duration;
			float strength = IONLHJIDACJ.Type == "CriticalHit" ? Eclipse.UI.AccessibilitySettings.CriticalShake : 1f;
			BMBGCIEFJGB.PGJEGJKFHND(x * SystemProperties.NHIDNIPGCPC * strength, y * SystemProperties.NHIDNIPGCPC * strength);
		}
	}

	private void DrawZoomEffect()
	{
		if (!NOLKMEPOJIE)
		{
			return;
		}
		float num = Mathf.Abs(OOFFFLEFKFA.JCNPAOMNJCL - OOFFFLEFKFA.AFBPPNDBMEC) / ((float)OOFFFLEFKFA.OFJCKMNLAEP / 2f);
		if (OOFFFLEFKFA.BJDFMKOCNBN <= OOFFFLEFKFA.OFJCKMNLAEP / 2)
		{
			OOFFFLEFKFA.ALOKJEILMLK -= num;
			if (OOFFFLEFKFA.ALOKJEILMLK < OOFFFLEFKFA.JCNPAOMNJCL)
			{
				OOFFFLEFKFA.ALOKJEILMLK = OOFFFLEFKFA.JCNPAOMNJCL;
			}
		}
		else
		{
			OOFFFLEFKFA.ALOKJEILMLK += num;
			if (OOFFFLEFKFA.ALOKJEILMLK > OOFFFLEFKFA.AFBPPNDBMEC)
			{
				OOFFFLEFKFA.ALOKJEILMLK = OOFFFLEFKFA.AFBPPNDBMEC;
			}
		}
		OOFFFLEFKFA.BJDFMKOCNBN++;
		_RenderInterpolation.PushZoomScale(OOFFFLEFKFA.ALOKJEILMLK);
	}

	private void RenderEffect()
	{
		if (HLDMKKKKAMI)
		{
			if (LLLNHELEKNF <= 0f)
			{
				HLDMKKKKAMI = false;
				HOGCLFMOHLE();
			}
			LLLNHELEKNF--;
		}
		if (OHNBKMHOMJI)
		{
			if (BIPHAGJDGOL <= 0f)
			{
				OHNBKMHOMJI = false;
				LJIJAPMBLFM();
			}
			BIPHAGJDGOL--;
		}
		if (NOLKMEPOJIE)
		{
			if (showZoomEffectCount <= 0)
			{
				NOLKMEPOJIE = false;
			}
			showZoomEffectCount--;
		}
	}

	private void HOGCLFMOHLE()
	{
		CallEvent(1, null);
		if (JOGMEKGACJL.isActive)
		{
			BMBGCIEFJGB.GEDDKEKGCBI(JOGMEKGACJL.NAAPALOFBCI, JOGMEKGACJL.IHFFJPLMIAL, JOGMEKGACJL.count);
			JOGMEKGACJL.isActive = false;
		}
	}

	private void LJIJAPMBLFM()
	{
		EPIPOLDCCHD = 0f;
		CallEvent(2, null);
	}

	private void CreateUnityObject(Transform PKHKBAJOHHF)
	{
		_UnityObject = new GameObject("Camera");
		_UnityObject.transform.SetParent(PKHKBAJOHHF, false);
		_UnityObject.transform.localPosition = new Vector3(0f, 0f);
		_UnityObject.AddComponent<CameraRenderInterpolationDriver>().Init(this);
	}

	public void Clear()
	{
		BMBGCIEFJGB.Clear();
		BMBGCIEFJGB = null;
	}

	public virtual void Init(Location LPJNEDFCBOI)
	{
		BMBGCIEFJGB = new Render(_UnityObject);
		JNBAHPMBLOL.SetAttenuation(0f);
		NGOEHKEKBIL.SetAttenuation(0f);
		_location = LPJNEDFCBOI;
		BMBGCIEFJGB.Init(_location);
		Vector3f bAINMLLIKOL = _location.GOEOFEIOAPC();
		JNBAHPMBLOL.SetStart(bAINMLLIKOL);
		JNBAHPMBLOL.SetEnd(bAINMLLIKOL);
		NGOEHKEKBIL.SetStart(bAINMLLIKOL);
		NGOEHKEKBIL.SetEnd(bAINMLLIKOL);
		BIPHAGJDGOL = 0f;
		OHNBKMHOMJI = false;
		LLLNHELEKNF = 0f;
		HLDMKKKKAMI = false;
		showZoomEffectCount = 0;
		NOLKMEPOJIE = false;
		_RenderInterpolation.ResetZoomScale(0f);
	}

	public void Render()
	{
		if (PMNEFPDLPCC)
		{
			bool flag = true;
			foreach (Model item in _models)
			{
				if (item.LPFPGDJALED() == -1)
				{
					flag = false;
				}
			}
			if (flag)
			{
				CCLHMAFDAPI();
				PMNEFPDLPCC = false;
			}
		}
		if (PMJCGFONEPA)
		{
			RenderEffect();
			UpdateCameraPosition();
			CameraUpdate();
			DrawPosition();
			DrawQuakeEffect();
			DrawZoomEffect();
		}
	}

	public void RenderInterpolatedPresentation()
	{
		if (!PMJCGFONEPA || BMBGCIEFJGB == null)
		{
			return;
		}
		float alpha = FightInterpolation.CameraAlpha;
		BMBGCIEFJGB.PresentationPass = true;
		try
		{
			DrawInterpolatedPosition(alpha);
			DrawInterpolatedQuakeEffect(alpha);
			BMBGCIEFJGB.RefreshLightInTheDarkness();
		}
		finally { BMBGCIEFJGB.PresentationPass = false; }
		BMBGCIEFJGB.SyncAdditionalDrawsLayerTransform();
	}

	public void PHGNIPMBJEH(Vector3f NAAPALOFBCI, Vector3f KKIKIDNALOL, float time, bool HKNHLNGMOJC, string HJCIKLIPILA, float NOOOCHHKECH)
	{
		BMBGCIEFJGB.BHOMOMIPKGC(NAAPALOFBCI, KKIKIDNALOL, time, HKNHLNGMOJC, HJCIKLIPILA, NOOOCHHKECH);
	}

	public void LCBPCEHILJD(Vector3f NAAPALOFBCI, Vector3f IHFFJPLMIAL, int count = 4)
	{
		JOGMEKGACJL.NAAPALOFBCI.Set(NAAPALOFBCI);
		JOGMEKGACJL.IHFFJPLMIAL.Set(IHFFJPLMIAL);
		JOGMEKGACJL.count = count;
		JOGMEKGACJL.isActive = true;
	}

	public void HDFAOMAONJI(GameController value)
	{
		OJINMMFLEEB = value;
		OJINMMFLEEB.SetScale(FGLAIPPLINB());
		OJINMMFLEEB.InitController();
	}

	public void DFKKNMDAFDC(bool value)
	{
		if (_preFight != null)
		{
			_preFight.VisibleViewer(value);
		}
		OJINMMFLEEB.gameObject.SetActive(value);
		if (value)
		{
			PMNEFPDLPCC = value;
		}
		else
		{
			BMBGCIEFJGB.BHIMNPFDCDE(value);
		}
	}

	public void RemoveObjectByIndex(int index)
	{
		BMBGCIEFJGB.FPNKBJPKKGB().RemoveModel(index);
		BMBGCIEFJGB.NAKJKHLEAEB(_models[index]);
		_models.RemoveAt(index);
	}

	public void RemoveObject(Model ACENLMONNPA)
	{
		int num = _models.IndexOf(ACENLMONNPA);
		if (num != -1)
		{
			RemoveObjectByIndex(num);
		}
	}

	public int AddModel(Model ACENLMONNPA, bool EKBOGDKIHIH, bool IGGHECALMMP)
	{
		int result = -1;
		_models.Add(ACENLMONNPA);
		BMBGCIEFJGB.CDDKOOMODHG(ACENLMONNPA);
		if (EKBOGDKIHIH)
		{
			CIJJBMDDAFL = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(GameUtils.LEPANPKBBKI().MNDFNOCCOKI);
			BGFPBMFKFGJ = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(GameUtils.LEPANPKBBKI().MEIHGLKHLFC);
		}
		if (BMBGCIEFJGB != null)
		{
			result = BMBGCIEFJGB.OGICJPJDLNN(ACENLMONNPA.CLDMEJKGLBA(), _location.modelsColor, IGGHECALMMP);
		}
		return result;
	}

    internal bool ReplaceModel(Model expected, Model replacement, bool player)
    {
        if (expected == null || replacement == null || BMBGCIEFJGB == null || _models.Contains(replacement)) return false;
        int index = _models.IndexOf(expected);
        if (index < 0 || replacement.CLDMEJKGLBA() == null) return false;
        ModelNode focus = null, secondaryFocus = null;
        if (player)
        {
            focus = replacement.CLDMEJKGLBA().EGHIDHMENEF(GameUtils.LEPANPKBBKI().MNDFNOCCOKI);
            secondaryFocus = replacement.CLDMEJKGLBA().EGHIDHMENEF(GameUtils.LEPANPKBBKI().MEIHGLKHLFC);
            if (focus == null || secondaryFocus == null) return false;
        }
        BMBGCIEFJGB.CDDKOOMODHG(replacement);
        bool replaced = false;
        try
        {
            replaced = BMBGCIEFJGB.FPNKBJPKKGB().ReplaceModel(index, expected.CLDMEJKGLBA(),
                replacement.CLDMEJKGLBA(), _location.modelsColor);
            if (!replaced) return false;
        }
        finally
        {
            if (!replaced) BMBGCIEFJGB.NAKJKHLEAEB(replacement);
        }
        BMBGCIEFJGB.NAKJKHLEAEB(expected);
        _models[index] = replacement;
        replacement.Index = expected.Index;
        if (player) { CIJJBMDDAFL = focus; BGFPBMFKFGJ = secondaryFocus; }
        return true;
    }

	public void FIEBIONJCCI(GameUtils.HitEffect HJLADIDMFOM)
	{
		if (HJLADIDMFOM != null)
		{
			IONLHJIDACJ = HJLADIDMFOM;
			// Mod-configured impact effect; critical hits respect the shake slider.
			Eclipse.Modding.ModVisuals.TriggerImpact(IONLHJIDACJ.Type,
				IONLHJIDACJ.Type == "CriticalHit" ? Eclipse.UI.AccessibilitySettings.CriticalShake : 1f);
			bool wasPaused = HLDMKKKKAMI;
			LLLNHELEKNF = IONLHJIDACJ.NHKPODHHDPF * (IONLHJIDACJ.Type == "CriticalHit" ? Eclipse.UI.AccessibilitySettings.CriticalPause : 1f);
			OHNBKMHOMJI = true;
			BIPHAGJDGOL = IONLHJIDACJ.OFJCKMNLAEP;
			EPIPOLDCCHD = IONLHJIDACJ.OFJCKMNLAEP;
            HLDMKKKKAMI = LLLNHELEKNF > 0f;
            if (HLDMKKKKAMI) CallEvent(0, null);
            else if (wasPaused) HOGCLFMOHLE();
		}
	}

	public void FFIAMGHGPPA(GameUtils.ZoomEffect DCLANCDBJLM)
	{
		if (DCLANCDBJLM != null)
		{
			NOLKMEPOJIE = true;
			OOFFFLEFKFA = DCLANCDBJLM;
			float num = BMBGCIEFJGB.KGCPMIDNKKI();
			if (OOFFFLEFKFA.JCNPAOMNJCL < num)
			{
				OOFFFLEFKFA.JCNPAOMNJCL = num;
			}
			OOFFFLEFKFA.AFBPPNDBMEC = BMBGCIEFJGB.KMMOLDBJBIG();
			OOFFFLEFKFA.ALOKJEILMLK = OOFFFLEFKFA.AFBPPNDBMEC;
			_RenderInterpolation.ResetZoomScale(OOFFFLEFKFA.ALOKJEILMLK);
			OOFFFLEFKFA.BJDFMKOCNBN = 0;
			showZoomEffectCount = OOFFFLEFKFA.OFJCKMNLAEP;
		}
	}

	public Vector3f HOKLGMEOMEI()
	{
		return CIJJBMDDAFL.GetStart();
	}

	public void OMPFAMELAII()
	{
		BMBGCIEFJGB.OBNOJKGAJML();
	}

	public void GDOPCJEGPFL()
	{
		BMBGCIEFJGB.JACOKNMGNDF();
	}

	public void AddPreFight(PreFight value)
	{
		_preFight = value;
	}

	public void CCLHMAFDAPI()
	{
		BMBGCIEFJGB.BHIMNPFDCDE(true);
	}

	public void NPFMKCHKGND()
	{
		OJINMMFLEEB.SetScale(FGLAIPPLINB());
	}

	public Vector3f NPJHOCJIPDL()
	{
		return NGOEHKEKBIL.GetStart();
	}

	public void MHKHHEMJFOK(Vector3f value)
	{
		NGOEHKEKBIL.SetStart(value);
	}

	public void JMGBMIDNCFP()
	{
		BMBGCIEFJGB.AOMKPKJNIKH(!BMBGCIEFJGB.CHGCKFIHOBG());
	}
}
