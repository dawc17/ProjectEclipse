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
			return IsShock();
		}
		set
		{
			SetShock(value);
		}
	}

	public Model KJDFJPBIGJC
	{
		get
		{
			return GetModel();
		}
		set
		{
			SetModel(value);
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

	public bool IsShock()
	{
		return _IsShock;
	}

	public void SetShock(bool value)
	{
		_IsShock = value;
	}

	public Model GetModel()
	{
		return _Model;
	}

	public void SetModel(Model value)
	{
		_Model = value;
	}

	public Vector3f PLBNCDCFPML()
	{
		return HOFFDCFEBGA().GetStart();
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
		return Vector3f.Middle(LHBNIMGFKIB.GetStart(), AAOIAEJJINO.GetStart());
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
			Vector3f lHBNIMGFKIB = item.GetStart();
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
		if (GetModel() != null && GetModel().NJDJHGDMCIJ() != null)
		{
			lCDGOCIAIDK = GetModel().NJDJHGDMCIJ().CLDMEJKGLBA().EGHIDHMENEF(name);
		}
		return lCDGOCIAIDK;
	}

	// best guess for name
	public ModelNode FindNodeOrParent(string name)
	{
		return KLAPIGGACMM(name);
	}

	public int GetNodeIDByName(string name)
	{
		for (int i = 0; i < CEHJGIHMKFF.OEAFIMFONDL.Count; i++)
		{
			if (name == CEHJGIHMKFF.OEAFIMFONDL[i].GetName())
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
			_ModelWeight += item.GetWeight();
		}
	}

	public void NDDMFBCIHPC()
	{
		Vector3f eMAFACPEPDK = new Vector3f();
		Vector3f eMAFACPEPDK2 = new Vector3f();
		Vector3f bAINMLLIKOL = new Vector3f(CNCJJDEJBNK.GetStart());
		List<ModelNode> list = JEGHCCFLAIF();
		CNCJJDEJBNK.GetStart().Reset();
		foreach (ModelNode item in list)
		{
			eMAFACPEPDK2.Set(item.GetStart());
			eMAFACPEPDK2.Multiply(item.GetWeight());
			eMAFACPEPDK.Add(eMAFACPEPDK2);
		}
		eMAFACPEPDK.Multiply(1f / _ModelWeight);
		CNCJJDEJBNK.SetStart(eMAFACPEPDK);
		CNCJJDEJBNK.SetEnd(bAINMLLIKOL);
	}

	public Vector3f BEFMLJFBPGN()
	{
		return CNCJJDEJBNK.GetEnd();
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
		Vector3f bEHOPOPCJGB = new Vector3f(Vector3f.MJOKEBGPHKB(MGMMDGFPBLP, NFADOLIKJEA.GetStart()));
		Vector3f bEHOPOPCJGB2 = new Vector3f(Vector3f.MJOKEBGPHKB(MGMMDGFPBLP, NFADOLIKJEA.GetEnd()));
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.GetStart().Add(bEHOPOPCJGB);
			item.GetEnd().Add(bEHOPOPCJGB2);
		}
	}

	public void MNHAGALCNFB(List<Vector3f> KPLANIHPMED)
	{
		int index = GetNodeIDByName(_PivotName);
		Vector3f nBMEGFBPGFE = CJELIBMCCMA().GetStart();
		Vector3f eMAFACPEPDK = Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, KPLANIHPMED[index]);
		eMAFACPEPDK.SetY(0f);
		int i = 0;
		for (int count = KPLANIHPMED.Count; i < count; i++)
		{
			CEHJGIHMKFF.OEAFIMFONDL[i].SetStart(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
			CEHJGIHMKFF.OEAFIMFONDL[i].SetEnd(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
		}
	}

	public void MNHAGALCNFB(List<Vector3f> KPLANIHPMED, ModelNode AECCPADGGPG)
	{
		int index = AECCPADGGPG.GetID();
		Vector3f nBMEGFBPGFE = AECCPADGGPG.GetStart();
		Vector3f eMAFACPEPDK = Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, KPLANIHPMED[index]);
		eMAFACPEPDK.SetY(0f);
		int i = 0;
		for (int count = KPLANIHPMED.Count; i < count; i++)
		{
			CEHJGIHMKFF.OEAFIMFONDL[i].SetStart(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
			CEHJGIHMKFF.OEAFIMFONDL[i].SetEnd(Vector3f.PHEFFKMOOCM(KPLANIHPMED[i], eMAFACPEPDK));
		}
	}

	public void JBHFODLCNIA(Vector3f OPNPKNEOALJ)
	{
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.GetStart().Add(OPNPKNEOALJ);
			item.GetEnd().Add(OPNPKNEOALJ);
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
				LLLOJBFMONN.Error("Nodes '{0}' for macronode '{1}' was not found", item.First, gDNAJOODAGP.GetName());
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
		SetShock(false);
		ModelReloader.NPMIHDFCBBH(this, DCFPONJAING.FileNames);
	}

	public void OBFONONKIAN()
	{
		foreach (ModelNode item in CEHJGIHMKFF.OEAFIMFONDL)
		{
			item.SetEnd();
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
			string text = item.GetName();
			string text2 = text.Substring(0, text.Length - 2);
			string text3 = text.Substring(text.Length - 2, 2);
			if (!(text3 == "_1"))
			{
				continue;
			}
			string text4 = text2 + "_2";
			foreach (ModelNode item2 in nodes)
			{
				string text5 = item2.GetName();
				if (text5 == text4)
				{
					int gBCLEDJAOBM = item.GetID();
					int pOFHDGJAFMP = item2.GetID();
					OEMALIFPGPO.Add(new global::Pair<int, int>(gBCLEDJAOBM, pOFHDGJAFMP));
					item.SetPairNode(item2);
					item2.SetPairNode(item);
					break;
				}
			}
		}
	}
}
