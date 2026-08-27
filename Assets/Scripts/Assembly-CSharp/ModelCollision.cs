using System.Collections.Generic;
using UnityEngine;

public class ModelCollision
{
	public class StrikeHit
	{
		public ModelEdge CMGLHHEJEBN;

		public ModelEdge ALIHGFIJEDN;

		private Vector3f BDPMMKHKFMN = new Vector3f();

		private Vector3f BLDIOPLELIK = new Vector3f();

		public Vector3f Point
		{
			get
			{
				return EGCPOJIDHKK();
			}
			set
			{
				CEDPGCAEKDD(value);
			}
		}

		public Vector3f AOFLADELDFB
		{
			get
			{
				return OJOMOLOIAOJ();
			}
			set
			{
				MNCLDJCFCEM(value);
			}
		}

		public Vector3f EGCPOJIDHKK()
		{
			return BDPMMKHKFMN;
		}

		public void CEDPGCAEKDD(Vector3f value)
		{
			BDPMMKHKFMN.Set(value);
		}

		public Vector3f OJOMOLOIAOJ()
		{
			return BLDIOPLELIK;
		}

		public void MNCLDJCFCEM(Vector3f value)
		{
			BLDIOPLELIK.Set(value);
		}
	}

	private IntervalAnimation ODIOOMNBAGF;

	private object _LastStrikePhase;

	private ModelObject _ModelObject;

	private bool _Render;

	public StrikeHit Strike;

	public object ILHMMOPKEIF
	{
		get
		{
			return GOPPGFPKLKP();
		}
		set
		{
			set_LastStrikePhase(value);
		}
	}

	public bool DAAACGKPJAL
	{
		get
		{
			return KKFIJLOMOJI();
		}
		set
		{
			set_render(value);
		}
	}

	public ModelCollision(ModelObject ACENLMONNPA)
	{
		Strike = new StrikeHit();
		_ModelObject = ACENLMONNPA;
		_LastStrikePhase = null;
		ODIOOMNBAGF = null;
		_Render = true;
	}

	public object GOPPGFPKLKP()
	{
		return _LastStrikePhase;
	}

	public void set_LastStrikePhase(object value)
	{
		_LastStrikePhase = value;
	}

	public bool KKFIJLOMOJI()
	{
		return _Render;
	}

	public void set_render(bool value)
	{
		_Render = value;
	}

	public bool Render(ModelObject HFGPAELCNMF, List<ModelEdge> BLJEFDAPKBH, object HIJDANGMJDM)
	{
		if (_Render && IsPhase(HIJDANGMJDM))
		{
			return false;
		}
		List<ModelEdge> lONAJAHCJGH = HFGPAELCNMF.ODDEMLAODPM();
		foreach (ModelEdge item in BLJEFDAPKBH)
		{
			if (CrossModel(lONAJAHCJGH, item))
			{
				_LastStrikePhase = HIJDANGMJDM;
				return true;
			}
		}
		return false;
	}

	public bool Render(ModelObject HFGPAELCNMF, List<ModelEdge> BLJEFDAPKBH, IntervalAnimation NOJNPFMOFLM)
	{
		if (ODIOOMNBAGF == NOJNPFMOFLM)
		{
			return false;
		}
		IntervalAttack hFIIPNLCIEE = NOJNPFMOFLM as IntervalAttack;
		if (!hFIIPNLCIEE.CFADPGIEKDN())
		{
			ODIOOMNBAGF = NOJNPFMOFLM;
			Strike.ALIHGFIJEDN = null;
			Strike.CMGLHHEJEBN = null;
			Strike.EGCPOJIDHKK().Reset();
			Strike.OJOMOLOIAOJ().Reset();
			return true;
		}
		List<ModelEdge> lONAJAHCJGH = HFGPAELCNMF.ODDEMLAODPM();
		foreach (ModelEdge item in BLJEFDAPKBH)
		{
			if (CrossModel(lONAJAHCJGH, item))
			{
				ODIOOMNBAGF = NOJNPFMOFLM;
				return true;
			}
		}
		return false;
	}

	public void ResetLastStrike()
	{
		_LastStrikePhase = null;
	}

	public bool IsPhase(object HIJDANGMJDM)
	{
		return _LastStrikePhase == HIJDANGMJDM;
	}

	public bool CrossModelByEdge(ModelObject HFGPAELCNMF, ModelEdge ADFIIAJCBHA)
	{
		List<ModelEdge> lONAJAHCJGH = HFGPAELCNMF.ODDEMLAODPM();
		return CrossModel(lONAJAHCJGH, ADFIIAJCBHA);
	}

	public void ResetInterval()
	{
		ODIOOMNBAGF = null;
	}

	private bool CrossModel(List<ModelEdge> LONAJAHCJGH, ModelEdge PJMKFHFECLK)
	{
		Vector3f eMAFACPEPDK = new Vector3f();
		Vector3f eMAFACPEPDK2 = new Vector3f();
		float kLDFJGIKIHG = PJMKFHFECLK.OBGOAOELMDJ();
		Vector3f hICHONIJHKL = PJMKFHFECLK.DOKBBJBFDCM();
		Vector3f lNPFHLPCLOP = PJMKFHFECLK.EBDICFAPOME();
		EquationLine hENNAFMBEAG = PJMKFHFECLK.HENNAFMBEAG;
		foreach (ModelEdge item in LONAJAHCJGH)
		{
			float mGCKDDGGCBI = item.OBGOAOELMDJ();
			Vector3f nMAJNHKJJEM = item.DOKBBJBFDCM();
			Vector3f oNNJMGGPHEL = item.EBDICFAPOME();
			EquationLine hENNAFMBEAG2 = item.HENNAFMBEAG;
			if (Vector2f.FLHCKLEBDDK(hICHONIJHKL, lNPFHLPCLOP, kLDFJGIKIHG, nMAJNHKJJEM, oNNJMGGPHEL, mGCKDDGGCBI, eMAFACPEPDK, eMAFACPEPDK2, hENNAFMBEAG, hENNAFMBEAG2))
			{
				AddStrike(PJMKFHFECLK, item, eMAFACPEPDK, eMAFACPEPDK2);
				return true;
			}
		}
		return false;
	}

	private void AddStrike(ModelEdge PJMKFHFECLK, ModelEdge KPEGNDLGKFB, Vector3f NAAPALOFBCI, Vector3f GKCGDDBMHNJ)
	{
		Strike.ALIHGFIJEDN = PJMKFHFECLK;
		Strike.CMGLHHEJEBN = KPEGNDLGKFB;
		Strike.CEDPGCAEKDD(NAAPALOFBCI);
		Strike.MNCLDJCFCEM(GKCGDDBMHNJ);
	}

	private static bool IsDistanceStrike(float OIOMNNFMDOO, float JBLFLFOGDFI, EquationLine EGKHHBMCGMK, Vector3f NAAPALOFBCI, Vector3f _base, Vector3f ILENLCMAMBH, Vector3f PCLFFOBJJFO)
	{
		if (OIOMNNFMDOO < JBLFLFOGDFI)
		{
			_base.JPFALPBDBAP(NAAPALOFBCI.GILCBJJPKBK() - OIOMNNFMDOO * EGKHHBMCGMK.LHBNIMGFKIB);
			_base.IBNFLLGPOLD(NAAPALOFBCI.OBIMBNIBEFG() - OIOMNNFMDOO * EGKHHBMCGMK.AAOIAEJJINO);
			if (((_base.GILCBJJPKBK() <= ILENLCMAMBH.GILCBJJPKBK() && _base.GILCBJJPKBK() >= PCLFFOBJJFO.GILCBJJPKBK()) || (_base.GILCBJJPKBK() <= PCLFFOBJJFO.GILCBJJPKBK() && _base.GILCBJJPKBK() >= ILENLCMAMBH.GILCBJJPKBK())) && ((_base.OBIMBNIBEFG() <= ILENLCMAMBH.OBIMBNIBEFG() && _base.OBIMBNIBEFG() >= PCLFFOBJJFO.OBIMBNIBEFG()) || (_base.OBIMBNIBEFG() <= PCLFFOBJJFO.OBIMBNIBEFG() && _base.OBIMBNIBEFG() >= ILENLCMAMBH.OBIMBNIBEFG())))
			{
				return true;
			}
			if (Mathf.Pow(NAAPALOFBCI.GILCBJJPKBK() - ILENLCMAMBH.GILCBJJPKBK(), 2f) + Mathf.Pow(NAAPALOFBCI.OBIMBNIBEFG() - ILENLCMAMBH.OBIMBNIBEFG(), 2f) <= Mathf.Pow(JBLFLFOGDFI, 2f))
			{
				return true;
			}
			if (Mathf.Pow(NAAPALOFBCI.GILCBJJPKBK() - PCLFFOBJJFO.GILCBJJPKBK(), 2f) + Mathf.Pow(NAAPALOFBCI.OBIMBNIBEFG() - PCLFFOBJJFO.OBIMBNIBEFG(), 2f) <= Mathf.Pow(JBLFLFOGDFI, 2f))
			{
				return true;
			}
		}
		return false;
	}
}
