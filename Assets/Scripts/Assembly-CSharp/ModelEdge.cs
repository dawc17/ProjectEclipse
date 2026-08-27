using Nekki.SF2.Core.Fights.Renders.Model;
using UnityEngine;

public class ModelEdge : Segment3D
{
	public EquationLine HENNAFMBEAG = new EquationLine();

	private string _Name;

	private string _BodyPart;

	private string DJAKNGMKAAL;

	private EdgeType KCIIELDOBOM;

	private EdgeSubType GPOHKJPLLGH;

	private ModelNode HAGFLLCANLE;

	private ModelNode NCCGHLOIJKI;

	private float NBPKNIADCFH;

	private float BOKNFPBHOLN;

	private float LEAKPNNFJJM;

	private float HGNFLOJMJNG;

	private int _Collisible;

	private bool KCJMHGNKFNG;

	private bool _IsShock;

	private Vector3f ELBIOKIIBGH = new Vector3f();

	private Vector3f GALMBGKDCFO = new Vector3f();

	public string EMMANKFGLLL
	{
		get
		{
			return ELHIBCEADCG();
		}
		set
		{
			MCDGHEAJGGP(value);
		}
	}

	public string GBOKABKLCFM
	{
		get
		{
			return NLLGDDMMJJN();
		}
		set
		{
			CFFCAJLFBEM(value);
		}
	}

	public EdgeSubType MPDIDJJJJGD
	{
		get
		{
			return DDFEOAHFFLO();
		}
		set
		{
			JIDPIOJGNBP(value);
		}
	}

	public ModelNode FJGONMIBEHC
	{
		get
		{
			return OGLAOHGLBHI();
		}
	}

	public ModelNode HGCKGAJDCDL
	{
		get
		{
			return KMHHBEKNHCJ();
		}
	}

	public float IHGONCCOKMK
	{
		get
		{
			return KLIOMCPELLF();
		}
		set
		{
			set_Length(value);
		}
	}

	public float AGODBAOHPJC
	{
		get
		{
			return OBGOAOELMDJ();
		}
		set
		{
			OIEJCNEODGC(value);
		}
	}

	public float FKKPDAIDLIM
	{
		get
		{
			return BCHMOKFJDLM();
		}
		set
		{
			LADPGJPABHO(value);
		}
	}

	public float JJDGKALDIAJ
	{
		get
		{
			return MHOICOCAPGD();
		}
		set
		{
			EJIOOIMBAEA(value);
		}
	}

	public int NIDJNCALPII
	{
		get
		{
			return NPMBEKDLAJO();
		}
		set
		{
			set_Collisible(value);
		}
	}

	public bool JOPBOBKMOCH
	{
		get
		{
			return KPEHIHNEKAF();
		}
		set
		{
			DIIBABHCHFP(value);
		}
	}

	public bool PFDCDIBODCL
	{
		get
		{
			return EDJFLMILEBA();
		}
		set
		{
			set_IsShock(value);
		}
	}

	public Vector3f CCMHKFHDFNM
	{
		get
		{
			return DOKBBJBFDCM();
		}
	}

	public Vector3f MBLICPBLEFC
	{
		get
		{
			return EBDICFAPOME();
		}
	}

	public new float LPDHNCDLFLO
	{
		get
		{
			return GLOLKEBFFEG();
		}
	}

	public Vector3f StartPosition
	{
		get
		{
			return FHGNPPBLIIL();
		}
	}

	public Vector3f FMMOHDJIABO
	{
		get
		{
			return FLCHIAEKIOO();
		}
	}

	public ModelEdge(ModelNode ILENLCMAMBH, ModelNode BFDAHEHCAGK)
	{
		EMGJLKDNNMM(ILENLCMAMBH);
		MCAEBMFHCIN(BFDAHEHCAGK);
	}

	public string get_Name()
	{
		return _Name;
	}

	public void set_Name(string value)
	{
		_Name = value;
	}

	public string ELHIBCEADCG()
	{
		return _BodyPart;
	}

	public void MCDGHEAJGGP(string value)
	{
		_BodyPart = value;
	}

	public string NLLGDDMMJJN()
	{
		return DJAKNGMKAAL;
	}

	public void CFFCAJLFBEM(string value)
	{
		DJAKNGMKAAL = value;
	}

	public EdgeType get_Type()
	{
		return KCIIELDOBOM;
	}

	public void set_Type(EdgeType value)
	{
		KCIIELDOBOM = value;
	}

	public EdgeSubType DDFEOAHFFLO()
	{
		return GPOHKJPLLGH;
	}

	public void JIDPIOJGNBP(EdgeSubType value)
	{
		GPOHKJPLLGH = value;
	}

	public ModelNode OGLAOHGLBHI()
	{
		return HAGFLLCANLE;
	}

	public ModelNode KMHHBEKNHCJ()
	{
		return NCCGHLOIJKI;
	}

	public float KLIOMCPELLF()
	{
		return NBPKNIADCFH;
	}

	public void set_Length(float value)
	{
		NBPKNIADCFH = value;
	}

	public float OBGOAOELMDJ()
	{
		return BOKNFPBHOLN;
	}

	public void OIEJCNEODGC(float value)
	{
		BOKNFPBHOLN = value;
	}

	public void LADPGJPABHO(float value)
	{
		LEAKPNNFJJM = value;
	}

	public float BCHMOKFJDLM()
	{
		return LEAKPNNFJJM;
	}

	public float MHOICOCAPGD()
	{
		return HGNFLOJMJNG;
	}

	public void EJIOOIMBAEA(float value)
	{
		HGNFLOJMJNG = value;
	}

	public int NPMBEKDLAJO()
	{
		return _Collisible;
	}

	public void set_Collisible(int value)
	{
		_Collisible = value;
	}

	public bool KPEHIHNEKAF()
	{
		return KCJMHGNKFNG;
	}

	public void DIIBABHCHFP(bool value)
	{
		KCJMHGNKFNG = value;
	}

	public bool EDJFLMILEBA()
	{
		return _IsShock;
	}

	public void set_IsShock(bool value)
	{
		_IsShock = value;
	}

	public Vector3f DOKBBJBFDCM()
	{
		return ELBIOKIIBGH;
	}

	public Vector3f EBDICFAPOME()
	{
		return GALMBGKDCFO;
	}

	public EdgeRender CreateUI(Transform GLKEHHPBGKP)
	{
		GameObject gameObject = new GameObject(_Name);
		EdgeRender edgeRender = gameObject.AddComponent<EdgeRender>();
		edgeRender.set_Edge(this);
		gameObject.transform.SetParent(GLKEHHPBGKP, false);
		return edgeRender;
	}

	public void FCEPJPDNNCM(ModelNode node)
	{
		CGHDODPMAOD();
		EMGJLKDNNMM(node);
	}

	public void KBBPEMKDDGB(ModelNode node)
	{
		OACACMPBJFO();
		MCAEBMFHCIN(node);
	}

	public void Iterative(Vector3f MGMMDGFPBLP)
	{
		float num = HAGFLLCANLE.FJJFKAJOFNJ();
		float num2 = NCCGHLOIJKI.FJJFKAJOFNJ();
		Vector3f eMAFACPEPDK = HAGFLLCANLE.ICLEOFDKDIF();
		Vector3f eMAFACPEPDK2 = NCCGHLOIJKI.ICLEOFDKDIF();
		float num3 = NBPKNIADCFH / Vector3f.Distance(eMAFACPEPDK, eMAFACPEPDK2);
		float num4 = (1f - num3) / (num + num2);
		float num5 = num * num4;
		float num6 = num2 * num4;
		MGMMDGFPBLP.JPFALPBDBAP(MGMMDGFPBLP.GILCBJJPKBK() * num3 + eMAFACPEPDK.GILCBJJPKBK() * num5 + eMAFACPEPDK2.GILCBJJPKBK() * num6);
		MGMMDGFPBLP.IBNFLLGPOLD(MGMMDGFPBLP.OBIMBNIBEFG() * num3 + eMAFACPEPDK.OBIMBNIBEFG() * num5 + eMAFACPEPDK2.OBIMBNIBEFG() * num6);
		MGMMDGFPBLP.set_Z(MGMMDGFPBLP.KMFEKANLCFO() * num3 + eMAFACPEPDK.KMFEKANLCFO() * num5 + eMAFACPEPDK2.KMFEKANLCFO() * num6);
	}

	public void Iterative()
	{
		if (HAGFLLCANLE.NEEJAPDCCMJ() || NCCGHLOIJKI.NEEJAPDCCMJ())
		{
			float num = HAGFLLCANLE.FJJFKAJOFNJ();
			float num2 = NCCGHLOIJKI.FJJFKAJOFNJ();
			Vector3f eMAFACPEPDK = HAGFLLCANLE.ICLEOFDKDIF();
			Vector3f eMAFACPEPDK2 = NCCGHLOIJKI.ICLEOFDKDIF();
			float num3 = NBPKNIADCFH / Vector3f.Distance(eMAFACPEPDK, eMAFACPEPDK2);
			float num4 = (1f - num3) / (num + num2);
			float num5 = num * num4;
			float num6 = num2 * num4;
			float lHNJJFDIJKK = eMAFACPEPDK.GILCBJJPKBK() * num5 + eMAFACPEPDK2.GILCBJJPKBK() * num6;
			float fFFHIOALHGM = eMAFACPEPDK.OBIMBNIBEFG() * num5 + eMAFACPEPDK2.OBIMBNIBEFG() * num6;
			float pDCENMEKIAP = eMAFACPEPDK.KMFEKANLCFO() * num5 + eMAFACPEPDK2.KMFEKANLCFO() * num6;
			if (HAGFLLCANLE.NEEJAPDCCMJ())
			{
				eMAFACPEPDK.Multiply(num3);
				eMAFACPEPDK.Add(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
			}
			if (NCCGHLOIJKI.NEEJAPDCCMJ())
			{
				eMAFACPEPDK2.Multiply(num3);
				eMAFACPEPDK2.Add(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
			}
		}
	}

	public new float GLOLKEBFFEG()
	{
		return Vector3f.Distance(HAGFLLCANLE.ICLEOFDKDIF(), NCCGHLOIJKI.ICLEOFDKDIF());
	}

	public void AGMHEHLBFCG()
	{
		OCGGJGCMNCH();
		Vector2f.JBLEOOBOCND(FHGNPPBLIIL(), FLCHIAEKIOO(), HENNAFMBEAG);
	}

	public void OCGGJGCMNCH()
	{
		Vector3f lHBNIMGFKIB = FHGNPPBLIIL();
		Vector3f aAOIAEJJINO = FLCHIAEKIOO();
		Vector3f.GetDivisionPoint3D(lHBNIMGFKIB, aAOIAEJJINO, LEAKPNNFJJM, ELBIOKIIBGH);
		Vector3f.GetDivisionPoint3D(lHBNIMGFKIB, aAOIAEJJINO, 1f - HGNFLOJMJNG, GALMBGKDCFO);
	}

	public Vector3f FHGNPPBLIIL()
	{
		return HAGFLLCANLE.ICLEOFDKDIF();
	}

	public Vector3f FLCHIAEKIOO()
	{
		return NCCGHLOIJKI.ICLEOFDKDIF();
	}

	private void EMGJLKDNNMM(ModelNode node)
	{
		HAGFLLCANLE = node;
		LCFIDBHFBOO(node.ICLEOFDKDIF());
	}

	private void MCAEBMFHCIN(ModelNode node)
	{
		NCCGHLOIJKI = node;
		PMGPGDDPOBB(node.ICLEOFDKDIF());
	}

	private void CGHDODPMAOD()
	{
		HAGFLLCANLE = null;
	}

	private void OACACMPBJFO()
	{
		NCCGHLOIJKI = null;
	}
}
