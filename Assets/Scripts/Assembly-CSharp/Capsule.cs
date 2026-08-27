using Nekki.SF2.Core.Fights.Renders.Model;
using UnityEngine;

public class Capsule : Segment3D
{
	private class AdditionalData
	{
		public string Name;

		public float IHHHADPCJPH;

		public float DPKIEPHDEFJ;

		public float GCCJLEGCLGN;

		public float BGJOLDMBEGI;

		public float NFOMECHPEOP;
	}

	public const float FOKLLBKBCGM = 1f;

	public const float BKADCDECIFC = 1f;

	public const float HDEKABMEKDI = 1f;

	public const float NKOPAKJBJHC = 1f;

	private AdditionalData DCFPONJAING = new AdditionalData();

	private CapsuleRender _CapsuleRender;

	public float NFOMECHPEOP
	{
		get
		{
			return IHEKOJKHPGP();
		}
		set
		{
			IJIGFKFDKGM(value);
		}
	}

	public float IHHHADPCJPH
	{
		get
		{
			return DOCOPPOLBMM();
		}
	}

	public float DPKIEPHDEFJ
	{
		get
		{
			return BAHOBNFOFCF();
		}
	}

	public float GCCJLEGCLGN
	{
		get
		{
			return JAEOCMCOEFE();
		}
	}

	public float BGJOLDMBEGI
	{
		get
		{
			return PLFEEBJMGAK();
		}
	}

	public Capsule(Segment3D LEFHAGAGOME)
	{
		LCFIDBHFBOO(LEFHAGAGOME.NDCACMDFLJN());
		PMGPGDDPOBB(LEFHAGAGOME.MINOGAHDDHA());
	}

	public CapsuleRender CreateUI(Transform GLKEHHPBGKP)
	{
		GameObject gameObject = new GameObject(DCFPONJAING.Name);
		CapsuleRender capsuleRender = gameObject.AddComponent<CapsuleRender>();
		capsuleRender.set_Base(this);
		gameObject.transform.SetParent(GLKEHHPBGKP, false);
		return capsuleRender;
	}

	public string get_Name()
	{
		return DCFPONJAING.Name;
	}

	public void set_Name(string value)
	{
		DCFPONJAING.Name = value;
	}

	public float IHEKOJKHPGP()
	{
		return DCFPONJAING.NFOMECHPEOP;
	}

	public void IJIGFKFDKGM(float value)
	{
		DCFPONJAING.NFOMECHPEOP = value;
	}

	public float DOCOPPOLBMM()
	{
		return DCFPONJAING.IHHHADPCJPH;
	}

	public float BAHOBNFOFCF()
	{
		return DCFPONJAING.DPKIEPHDEFJ;
	}

	public float JAEOCMCOEFE()
	{
		return DCFPONJAING.GCCJLEGCLGN;
	}

	public float PLFEEBJMGAK()
	{
		return DCFPONJAING.BGJOLDMBEGI;
	}

	public void CNEEGAJGBEI(float value = 1f)
	{
		DCFPONJAING.IHHHADPCJPH = value;
	}

	public void BLHHLPDEAKF(float value = 1f)
	{
		DCFPONJAING.DPKIEPHDEFJ = value;
	}

	public void GKBFHLAHCFG(float value = 1f)
	{
		DCFPONJAING.GCCJLEGCLGN = value;
	}

	public void HCCIGEIFEOF(float value = 1f)
	{
		DCFPONJAING.BGJOLDMBEGI = value;
	}

	private void JLCOIIBBKEF(Segment3D OEMALIFPGPO)
	{
		OEMALIFPGPO.LJPOALNMEOF(GetDivisionPoint3D(DCFPONJAING.GCCJLEGCLGN));
		OEMALIFPGPO.OCEHEINNABP(GetDivisionPoint3D(1f - DCFPONJAING.BGJOLDMBEGI));
	}
}
