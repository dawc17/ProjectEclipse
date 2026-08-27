public class Segment3D
{
	private Vector3f LCLJKFJNPLG;

	private Vector3f LNBCDDCNAHB;

	public Vector3f LLMNFABDADF
	{
		get
		{
			return NDCACMDFLJN();
		}
	}

	public Vector3f MKCGJNMMCCM
	{
		get
		{
			return MINOGAHDDHA();
		}
	}

	public float LPDHNCDLFLO
	{
		get
		{
			return GLOLKEBFFEG();
		}
	}

	public float NGKODECNPFK
	{
		get
		{
			return LDKFFINHBOH();
		}
	}

	public Vector3f KBNMOFDDLOM
	{
		get
		{
			return GJLBOKGJMHJ();
		}
	}

	public Vector2f BONEAMLIHOM
	{
		get
		{
			return NFOEEOBPIKO();
		}
	}

	public Vector3f FJALJCNEPLN
	{
		get
		{
			return MLDFIEBFJDI();
		}
	}

	public Vector2f DIHDLKHPHKG
	{
		get
		{
			return AHOIAJKLABL();
		}
	}

	public Vector3f FILILMIGAHE
	{
		get
		{
			return IEFEGPLODCM();
		}
	}

	public Segment3D()
	{
	}

	public Segment3D(Vector3f ILENLCMAMBH, Vector3f BFDAHEHCAGK)
	{
		LCLJKFJNPLG.Set(ILENLCMAMBH);
		LNBCDDCNAHB.Set(BFDAHEHCAGK);
	}

	public Vector3f NDCACMDFLJN()
	{
		return LCLJKFJNPLG;
	}

	public Vector3f MINOGAHDDHA()
	{
		return LNBCDDCNAHB;
	}

	public void SetSegment3D(Segment3D LEFHAGAGOME)
	{
		LJPOALNMEOF(LEFHAGAGOME.LCLJKFJNPLG);
		OCEHEINNABP(LEFHAGAGOME.LNBCDDCNAHB);
	}

	public float GLOLKEBFFEG()
	{
		return Vector3f.Distance(LCLJKFJNPLG, LNBCDDCNAHB);
	}

	public float LDKFFINHBOH()
	{
		return Vector2f.JOIHAKCICMP(LCLJKFJNPLG, LNBCDDCNAHB);
	}

	public Vector3f GJLBOKGJMHJ()
	{
		return Vector3f.KBNMOFDDLOM(LCLJKFJNPLG, LNBCDDCNAHB);
	}

	public Vector2f NFOEEOBPIKO()
	{
		return new Vector2f(LNBCDDCNAHB.GILCBJJPKBK() - LCLJKFJNPLG.GILCBJJPKBK(), LNBCDDCNAHB.OBIMBNIBEFG() - LCLJKFJNPLG.OBIMBNIBEFG());
	}

	public Vector3f MLDFIEBFJDI()
	{
		return Vector3f.MJOKEBGPHKB(LNBCDDCNAHB, LCLJKFJNPLG);
	}

	public Vector2f AHOIAJKLABL()
	{
		return PGNJEIBFCMJ(0.5f);
	}

	public Vector3f IEFEGPLODCM()
	{
		return GetDivisionPoint3D(0.5f);
	}

	public Vector2f PGNJEIBFCMJ(float ratio)
	{
		return new Vector2f(LCLJKFJNPLG.GILCBJJPKBK() + (LNBCDDCNAHB.GILCBJJPKBK() - LCLJKFJNPLG.GILCBJJPKBK()) * ratio, LCLJKFJNPLG.OBIMBNIBEFG() + (LNBCDDCNAHB.OBIMBNIBEFG() - LCLJKFJNPLG.OBIMBNIBEFG()) * ratio);
	}

	public Vector3f GetDivisionPoint3D(float ratio)
	{
		return Vector3f.GetDivisionPoint3D(LCLJKFJNPLG, LNBCDDCNAHB, ratio);
	}

	public void GetDivisionPoint3D(Vector3f OEMALIFPGPO, float ratio)
	{
		OEMALIFPGPO.Set(LCLJKFJNPLG.GILCBJJPKBK() + (LNBCDDCNAHB.GILCBJJPKBK() - LCLJKFJNPLG.GILCBJJPKBK()) * ratio, LCLJKFJNPLG.OBIMBNIBEFG() + (LNBCDDCNAHB.OBIMBNIBEFG() - LCLJKFJNPLG.OBIMBNIBEFG()) * ratio, LCLJKFJNPLG.KMFEKANLCFO() + (LNBCDDCNAHB.KMFEKANLCFO() - LCLJKFJNPLG.KMFEKANLCFO()) * ratio);
	}

	public Vector2f BCFPOPFLLGJ(Vector2f NAAPALOFBCI)
	{
		Vector2f hEJKLMNOLLG = new Vector2f(NAAPALOFBCI);
		Vector2f hEJKLMNOLLG2 = new Vector2f(LCLJKFJNPLG);
		Vector2f hEJKLMNOLLG3 = NFOEEOBPIKO();
		hEJKLMNOLLG.EHGLHOGAIDI(hEJKLMNOLLG2);
		float num = hEJKLMNOLLG.DotProduct(hEJKLMNOLLG3);
		float num2 = hEJKLMNOLLG3.DotProduct(hEJKLMNOLLG3);
		float lIAILCGJBDK = ((num2 == 0f) ? 0f : (num / num2));
		hEJKLMNOLLG3.Multiply(lIAILCGJBDK);
		hEJKLMNOLLG2.Add(hEJKLMNOLLG3);
		return hEJKLMNOLLG2;
	}

	public float GDNPPAEGJPF(Vector2f NAAPALOFBCI)
	{
		float num = Vector2f.JOIHAKCICMP(LCLJKFJNPLG, LNBCDDCNAHB);
		if (num != 0f)
		{
			return Vector2f.JOIHAKCICMP(LNBCDDCNAHB, NAAPALOFBCI) / num;
		}
		return 0f;
	}

	public static bool FLCEACOFEKB(Segment3D JLIFFKIFOKM, Segment3D BCNKAGOKLCL, Vector3f ONGFADJKIBB)
	{
		return Vector2f.AOFPHLDNOIL(JLIFFKIFOKM.LCLJKFJNPLG, JLIFFKIFOKM.LNBCDDCNAHB, BCNKAGOKLCL.LCLJKFJNPLG, BCNKAGOKLCL.LNBCDDCNAHB, ONGFADJKIBB);
	}

	public void LJPOALNMEOF(Vector3f value)
	{
		LCLJKFJNPLG.Set(value);
	}

	public void LCFIDBHFBOO(Vector3f value)
	{
		LCLJKFJNPLG = value;
	}

	public void OCEHEINNABP(Vector3f value)
	{
		LNBCDDCNAHB.Set(value);
	}

	public void PMGPGDDPOBB(Vector3f value)
	{
		LNBCDDCNAHB = value;
	}
}
