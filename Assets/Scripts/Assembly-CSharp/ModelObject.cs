using System.Collections.Generic;

public class ModelObject
{
	private class ModelNodes
	{
		public ModelNode CHEKEGGJDBL;

		public List<ModelNode> PIDBGGHBJCG = new List<ModelNode>();

		public List<ModelMacroNode> IAMDHKKBBOE = new List<ModelMacroNode>();

		public List<ModelNode> OEAFIMFONDL = new List<ModelNode>();

		public Dictionary<string, ModelNode> PLBNGPFCCEG = new Dictionary<string, ModelNode>();

		public List<ModelNode> NBJCHIJDDNN = new List<ModelNode>();
	}

	private class ModelEdges
	{
		public List<ModelEdge> PIDBGGHBJCG = new List<ModelEdge>();

		public List<ModelEdge> HKNNDIEDNDN = new List<ModelEdge>();

		public List<ModelEdge> ECNOHFFDIFG = new List<ModelEdge>();

		public List<ModelEdge> OEAFIMFONDL = new List<ModelEdge>();
	}

	private class Figures
	{
		public List<Capsule> NFGOBHMMJEB = new List<Capsule>();

		public List<Triangle> Triangles = new List<Triangle>();
	}

	private class AdditionalData
	{
		public List<global::Pair<int, int>> IANPBPEOKBH = new List<global::Pair<int, int>>();

		public List<string> FileNames = new List<string>();
	}

	private ModelNodes CEHJGIHMKFF = new ModelNodes();

	private ModelEdges LCDOKKAKODE = new ModelEdges();

	private Figures CFKNNINIIEA = new Figures();

	private AdditionalData DCFPONJAING = new AdditionalData();

	private ModelNode CNCJJDEJBNK;

	private float _ModelWeight;

	private int _NodesCount;

	private bool _IsShock;

	private Model _Model;

	private string _PivotName;

	public ModelNode DOEILCFFCDN
	{
		get
		{
			return HOFFDCFEBGA();
		}
	}

	public float ELFFFMCCGAJ
	{
		get
		{
			return PAJLIKBIAPA();
		}
	}

	public int OKDGCCPGLMC
	{
		get
		{
			return DFKIHADCFKG();
		}
		set
		{
			set_NodesCount(value);
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

	public Model KJDFJPBIGJC
	{
		get
		{
			return get_Model();
		}
		set
		{
			set_Model(value);
		}
	}

	public Vector3f BPPINEHFOBB
	{
		get
		{
			return PLBNCDCFPML();
		}
	}

	public ModelNode AFLPHBDFMGA
	{
		get
		{
			return CJELIBMCCMA();
		}
	}

	public List<ModelNode> CBAECAAKAIA
	{
		get
		{
			return LMBNDIPLBJA();
		}
	}

	public List<ModelMacroNode> IPJJCFJLBKE
	{
		get
		{
			return BLFJJAEFKKP();
		}
	}

	public List<ModelNode> OGPDJLAOEEO
	{
		get
		{
			return NAMKCLGOPDD();
		}
	}

	public Dictionary<string, ModelNode> KANEKAAJJGA
	{
		get
		{
			return HKCFFKKFFFE();
		}
	}

	private List<ModelNode> DLIKKPPEBEC
	{
		get
		{
			return JEGHCCFLAIF();
		}
	}

	public List<ModelEdge> OOFMOAHJEJF
	{
		get
		{
			return ODDEMLAODPM();
		}
	}

	public List<ModelEdge> DJNCBGONICH
	{
		get
		{
			return HABIIJGLCMA();
		}
	}

	public List<ModelEdge> HKNNDIEDNDN
	{
		get
		{
			return EKOGCJAAKDN();
		}
	}

	public List<ModelEdge> NNLHIICJNOG
	{
		get
		{
			return BKAPPJMGPKP();
		}
	}

	public List<Capsule> NFGOBHMMJEB
	{
		get
		{
			return DPIFMDIKDBC();
		}
	}

	public List<Triangle> Triangles
	{
		get
		{
			return ELOGKMHEBGA();
		}
	}

	public List<global::Pair<int, int>> ANKDHFEFEFF
	{
		get
		{
			return DJNNIKHGGFO();
		}
	}

	public Vector3f DMNHMMMGIMI
	{
		get
		{
			return BEFMLJFBPGN();
		}
	}

	public ModelObject()
	{
		CNCJJDEJBNK = new ModelNode("_CenterOfMass_");
		_ModelWeight = 0f;
		_NodesCount = 0;
		_IsShock = false;
		_Model = null;
		CEHJGIHMKFF.CHEKEGGJDBL = null;
		_PivotName = GameUtils.KEFHKHCNBOK;
	}

	public ModelNode HOFFDCFEBGA()
	{
		return CNCJJDEJBNK;
	}

	public float PAJLIKBIAPA()
	{
		return _ModelWeight;
	}

	public int DFKIHADCFKG()
	{
		return _NodesCount;
	}

	public void set_NodesCount(int value)
	{
		_NodesCount = value;
	}

	public bool EDJFLMILEBA()
	{
		return _IsShock;
	}

	public void set_IsShock(bool value)
	{
		_IsShock = value;
	}

	public Model get_Model()
	{
		return _Model;
	}

	public void set_Model(Model value)
	{
		_Model = value;
	}

	public Vector3f PLBNCDCFPML()
	{
		return HOFFDCFEBGA().ICLEOFDKDIF();
	}

	public ModelNode CJELIBMCCMA()
	{
		return CEHJGIHMKFF.CHEKEGGJDBL;
	}

	public List<ModelNode> LMBNDIPLBJA()
	{
		return CEHJGIHMKFF.PIDBGGHBJCG;
	}

	public List<ModelMacroNode> BLFJJAEFKKP()
	{
		return CEHJGIHMKFF.IAMDHKKBBOE;
	}

	public List<ModelNode> NAMKCLGOPDD()
	{
		return CEHJGIHMKFF.OEAFIMFONDL;
	}

	public Dictionary<string, ModelNode> HKCFFKKFFFE()
	{
		return CEHJGIHMKFF.PLBNGPFCCEG;
	}

	private List<ModelNode> JEGHCCFLAIF()
	{
		return (CEHJGIHMKFF.NBJCHIJDDNN.Count == 0) ? CEHJGIHMKFF.OEAFIMFONDL : CEHJGIHMKFF.NBJCHIJDDNN;
	}

	public List<ModelEdge> ODDEMLAODPM()
	{
		return LCDOKKAKODE.ECNOHFFDIFG;
	}

	public List<ModelEdge> HABIIJGLCMA()
	{
		return LCDOKKAKODE.PIDBGGHBJCG;
	}

	public List<ModelEdge> EKOGCJAAKDN()
	{
		return LCDOKKAKODE.HKNNDIEDNDN;
	}

	public List<ModelEdge> BKAPPJMGPKP()
	{
		return LCDOKKAKODE.OEAFIMFONDL;
	}

	public List<Capsule> DPIFMDIKDBC()
	{
		return CFKNNINIIEA.NFGOBHMMJEB;
	}

	public List<Triangle> ELOGKMHEBGA()
	{
		return CFKNNINIIEA.Triangles;
	}

	public List<global::Pair<int, int>> DJNNIKHGGFO()
	{
		return DCFPONJAING.IANPBPEOKBH;
	}

	public static Vector3f MHFFCMKNIKM(ModelNode LHBNIMGFKIB, ModelNode AAOIAEJJINO)
	{
		return Vector3f.Middle(LHBNIMGFKIB.ICLEOFDKDIF(), AAOIAEJJINO.ICLEOFDKDIF());
	}

	public int GetNodeIDByPairName(int index)
	{
		List<global::Pair<int, int>> list = DJNNIKHGGFO();
		foreach (global::Pair<int, int> item in list)
		{
			if (index == item.First)
			{
				return item.Second;
			}
			if (index == item.Second)
			{
				return item.First;
			}
		}
		return -1;
	}

	public ModelNode PHKIOHJBFGH(Vector2f MGMMDGFPBLP, float PKFOEAEPOAF)
	{
		ModelNode result = null;
		PKFOEAEPOAF *= PKFOEAEPOAF;
		List<ModelNode> list = NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			Vector3f lHBNIMGFKIB = item.ICLEOFDKDIF();
			if (Vector2f.LDKCDLFIDHL(lHBNIMGFKIB, MGMMDGFPBLP) < PKFOEAEPOAF)
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public ModelNode EGHIDHMENEF(string name)
	{
		if (CEHJGIHMKFF.CHEKEGGJDBL != null && name == _PivotName)
		{
			return CEHJGIHMKFF.CHEKEGGJDBL;
		}
		ModelNode value = null;
		if (CEHJGIHMKFF.PLBNGPFCCEG.TryGetValue(name, out value))
		{
			return value;
		}
		return null;
	}

	public ModelNode KLAPIGGACMM(string name)
	{
		if (CEHJGIHMKFF.CHEKEGGJDBL != null && name == _PivotName)
		{
			return CEHJGIHMKFF.CHEKEGGJDBL;
		}
		ModelNode lCDGOCIAIDK = EGHIDHMENEF(name);
		if (lCDGOCIAIDK != null)
		{
			return lCDGOCIAIDK;
		}
		if (get_Model() != null && get_Model().NJDJHGDMCIJ() != null)
		{
			lCDGOCIAIDK = get_Model().NJDJHGDMCIJ().CLDMEJKGLBA().EGHIDHMENEF(name);
		}
		return lCDGOCIAIDK;
	}

	public int GetNodeIDByName(string name)
	{
		for (int i = 0; i < CEHJGIHMKFF.OEAFIMFONDL.Count; i++)
		{
			if (name == CEHJGIHMKFF.OEAFIMFONDL[i].get_Name())
			{
				return i;
			}
		}
		return -1;
	}

	public ModelEdge CLBHEMEAAEN(string name)
	{
		foreach (ModelEdge item in LCDOKKAKODE.OEAFIMFONDL)
		{
			if (name == item.get_Name())
			{
				return item;
			}
		}
		return null;
	}

	public void GINBBKBGMDC()
	{
		_ModelWeight = 0f;
		List<ModelNode> list = JEGHCCFLAIF();
		foreach (ModelNode item in list)
		{
			_ModelWeight += item.FJJFKAJOFNJ();
		}
	}

	public void NDDMFBCIHPC()
	{
		Vector3f eMAFACPEPDK = new Vector3f();
		Vector3f eMAFACPEPDK2 = new Vector3f();
		Vector3f bAINMLLIKOL = new Vector3f(CNCJJDEJBNK.ICLEOFDKDIF());
		List<ModelNode> list = JEGHCCFLAIF();
		CNCJJDEJBNK.ICLEOFDKDIF().Reset();
		foreach (ModelNode item in list)
		{
			eMAFACPEPDK2.Set(item.ICLEOFDKDIF());
			eMAFACPEPDK2.Multiply(item.FJJFKAJOFNJ());
			eMAFACPEPDK.Add(eMAFACPEPDK2);
		}
		eMAFACPEPDK.Multiply(1f / _ModelWeight);
		CNCJJDEJBNK.AMPCKAIPIHH(eMAFACPEPDK);
		CNCJJDEJBNK.LAHLFIKENPP(bAINMLLIKOL);
	}

	public Vector3f BEFMLJFBPGN()
	{
		return CNCJJDEJBNK.FOGHEPNAPLC();
	}

	public void SetModelPosition(Vector3f MGMMDGFPBLP, ModelNode NFADOLIKJEA = null)
	{
		if (NFADOLIKJEA == null)
		{
			NFADOLIKJEA = EGHIDHMENEF(_PivotName);
		}
		if (NFADOLIKJEA == null || 0 >= CEHJGIHMKFF.OEAFIMFONDL.Count)
		{
			return;
		}
		Vector3f bEHOPOPCJGB = new Vector3f(Vector3f.MJOKEBGPHKB(MGMMDGFPBLP, NFADOLIKJEA.ICLEOFDKDIF()));
		Vector3f bEHOPOPCJGB2 = new Vector3f(Vector3f.MJOKEBGPHKB(MGMMDGFPBLP, NFADOLIKJEA.FOGHEPNAPLC()));
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.ICLEOFDKDIF().Add(bEHOPOPCJGB);
			item.FOGHEPNAPLC().Add(bEHOPOPCJGB2);
		}
	}

	public void MNHAGALCNFB(List<Vector3f> KPLANIHPMED)
	{
		int index = GetNodeIDByName(_PivotName);
		Vector3f nBMEGFBPGFE = CJELIBMCCMA().ICLEOFDKDIF();
		Vector3f eMAFACPEPDK = Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, KPLANIHPMED[index]);
		eMAFACPEPDK.IBNFLLGPOLD(0f);
		int i = 0;
		for (int count = KPLANIHPMED.Count; i < count; i++)
		{
			CEHJGIHMKFF.OEAFIMFONDL[i].AMPCKAIPIHH(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
			CEHJGIHMKFF.OEAFIMFONDL[i].LAHLFIKENPP(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
		}
	}

	public void MNHAGALCNFB(List<Vector3f> KPLANIHPMED, ModelNode AECCPADGGPG)
	{
		int index = AECCPADGGPG.ANAECCFDHMI();
		Vector3f nBMEGFBPGFE = AECCPADGGPG.ICLEOFDKDIF();
		Vector3f eMAFACPEPDK = Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, KPLANIHPMED[index]);
		eMAFACPEPDK.IBNFLLGPOLD(0f);
		int i = 0;
		for (int count = KPLANIHPMED.Count; i < count; i++)
		{
			CEHJGIHMKFF.OEAFIMFONDL[i].AMPCKAIPIHH(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
			CEHJGIHMKFF.OEAFIMFONDL[i].LAHLFIKENPP(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
		}
	}

	public void JBHFODLCNIA(Vector3f OPNPKNEOALJ)
	{
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.ICLEOFDKDIF().Add(OPNPKNEOALJ);
			item.FOGHEPNAPLC().Add(OPNPKNEOALJ);
		}
		NDDMFBCIHPC();
	}

	public void LKFBKGPOHPI()
	{
		ModelNode lCDGOCIAIDK = EGHIDHMENEF(_PivotName);
		if (lCDGOCIAIDK != null)
		{
			CEHJGIHMKFF.CHEKEGGJDBL = lCDGOCIAIDK;
		}
	}

	public void SetFileNames(List<string> CBHAEPCLDFG)
	{
		DCFPONJAING.FileNames.AddRange(CBHAEPCLDFG);
	}

	public void KJIEPFHIIKM()
	{
		int count = CEHJGIHMKFF.IAMDHKKBBOE.Count;
		for (int i = 0; i < count; i++)
		{
			ModelMacroNode gDNAJOODAGP = CEHJGIHMKFF.IAMDHKKBBOE[i];
			List<global::Pair<string, float>> lMPPCKACMNB = gDNAJOODAGP.LMPPCKACMNB;
			if (lMPPCKACMNB == null)
			{
				continue;
			}
			foreach (global::Pair<string, float> item in lMPPCKACMNB)
			{
				ModelNode lCDGOCIAIDK = EGHIDHMENEF(item.First);
				if (lCDGOCIAIDK != null)
				{
					gDNAJOODAGP.DNCHNPNABFH(lCDGOCIAIDK, item.Second);
					continue;
				}
				LLLOJBFMONN.Error("Nodes '{0}' for macronode '{1}' was not found", item.First, gDNAJOODAGP.get_Name());
			}
			gDNAJOODAGP.LMPPCKACMNB = null;
		}
	}

	public void JANOFOIKIAP()
	{
		int count = CEHJGIHMKFF.IAMDHKKBBOE.Count;
		for (int i = 0; i < count; i++)
		{
			CEHJGIHMKFF.IAMDHKKBBOE[i].FPKMHOMMFKB();
		}
	}

	public void Clear()
	{
		LCDOKKAKODE.PIDBGGHBJCG.Clear();
		LCDOKKAKODE.HKNNDIEDNDN.Clear();
		LCDOKKAKODE.ECNOHFFDIFG.Clear();
		LCDOKKAKODE.OEAFIMFONDL.Clear();
		CFKNNINIIEA.NFGOBHMMJEB.Clear();
		CFKNNINIIEA.Triangles.Clear();
		DCFPONJAING.IANPBPEOKBH.Clear();
		CEHJGIHMKFF.PIDBGGHBJCG.Clear();
		CEHJGIHMKFF.IAMDHKKBBOE.Clear();
		if (CEHJGIHMKFF.CHEKEGGJDBL != null)
		{
			CEHJGIHMKFF.CHEKEGGJDBL = null;
		}
		CEHJGIHMKFF.OEAFIMFONDL.Clear();
		_Model = null;
	}

	public void Reset()
	{
		set_IsShock(false);
		ModelReloader.NPMIHDFCBBH(this, DCFPONJAING.FileNames);
	}

	public void OBFONONKIAN()
	{
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.OIEPNGBEECN();
		}
	}

	public void MDDBGGPHNLF()
	{
		EFDOLECDDHI(NAMKCLGOPDD(), DCFPONJAING.IANPBPEOKBH);
	}

	public void FLPIFFOGDBF()
	{
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.HBPBKNDPBMG();
		}
	}

	public void LEOMLPGGLNA(List<global::Pair<string, float>> MFIEGKAMKNJ)
	{
		foreach (global::Pair<string, float> item in MFIEGKAMKNJ)
		{
			ModelNode lCDGOCIAIDK = EGHIDHMENEF(item.First);
			if (lCDGOCIAIDK == null)
			{
				LLLOJBFMONN.Error("ModelObject::addComNodes - no node with name: {0}", item.First);
			}
			CEHJGIHMKFF.NBJCHIJDDNN.Add(lCDGOCIAIDK);
		}
	}

	private void EFDOLECDDHI(List<ModelNode> nodes, List<global::Pair<int, int>> OEMALIFPGPO)
	{
		OEMALIFPGPO.Clear();
		foreach (ModelNode item in nodes)
		{
			string text = item.get_Name();
			string text2 = text.Substring(0, text.Length - 2);
			string text3 = text.Substring(text.Length - 2, 2);
			if (!(text3 == "_1"))
			{
				continue;
			}
			string text4 = text2 + "_2";
			foreach (ModelNode item2 in nodes)
			{
				string text5 = item2.get_Name();
				if (text5 == text4)
				{
					int gBCLEDJAOBM = item.ANAECCFDHMI();
					int pOFHDGJAFMP = item2.ANAECCFDHMI();
					OEMALIFPGPO.Add(new global::Pair<int, int>(gBCLEDJAOBM, pOFHDGJAFMP));
					item.set_PairNode(item2);
					item2.set_PairNode(item);
					break;
				}
			}
		}
	}
}
