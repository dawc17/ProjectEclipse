using System.Collections.Generic;

public class ModelMacroNode : ModelNode
{
	public List<global::Pair<string, float>> LMPPCKACMNB = new List<global::Pair<string, float>>();

	private List<global::Pair<ModelNode, float>> LKBADGFHJHK = new List<global::Pair<ModelNode, float>>();

	public List<global::Pair<ModelNode, float>> AAHADKFKDPN
	{
		get
		{
			return LDEBJOPLCKO();
		}
	}

	public ModelMacroNode(string name, Vector3f OBLEMIHLFII)
		: base(name, OBLEMIHLFII)
	{
		SetType(NodeType.MacroNode);
	}

	public ModelMacroNode(ModelMacroNode AHJOLBKABMC)
		: base(AHJOLBKABMC)
	{
		LKBADGFHJHK = new List<global::Pair<ModelNode, float>>(AHJOLBKABMC.LKBADGFHJHK);
		SetType(NodeType.MacroNode);
	}

	public List<global::Pair<ModelNode, float>> LDEBJOPLCKO()
	{
		return LKBADGFHJHK;
	}

	public void DNCHNPNABFH(ModelNode BFEBLBKODLK, float EBIFKGEMHLK)
	{
		LKBADGFHJHK.Add(new global::Pair<ModelNode, float>(BFEBLBKODLK, EBIFKGEMHLK));
	}

	public void FPKMHOMMFKB()
	{
		if (BCIPCPOJJGN)
		{
			BCIPCPOJJGN = false;
			return;
		}
		_End.Set(_Start);
		_Start.Reset();
		global::Pair<ModelNode, float> cCKLNOPEKHO = null;
		int count = LKBADGFHJHK.Count;
		for (int i = 0; i < count; i++)
		{
			cCKLNOPEKHO = LKBADGFHJHK[i];
			_Start.GLGNIMKANCA(cCKLNOPEKHO.First.GetStart(), cCKLNOPEKHO.Second);
		}
	}
}
