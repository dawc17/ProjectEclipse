using System.Collections.Generic;

public partial class ModelMacroNode : ModelNode
{
	// best guess for name
	public List<global::Pair<string, float>> NamedWeights = new List<global::Pair<string, float>>();

	// best guess for name
	private List<global::Pair<ModelNode, float>> _nodeWeights = new List<global::Pair<ModelNode, float>>();

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
		_nodeWeights = new List<global::Pair<ModelNode, float>>(AHJOLBKABMC._nodeWeights);
		SetType(NodeType.MacroNode);
	}

	public List<global::Pair<ModelNode, float>> LDEBJOPLCKO()
	{
		return _nodeWeights;
	}

	public void DNCHNPNABFH(ModelNode BFEBLBKODLK, float EBIFKGEMHLK)
	{
		_nodeWeights.Add(new global::Pair<ModelNode, float>(BFEBLBKODLK, EBIFKGEMHLK));
	}

	public void FPKMHOMMFKB()
	{
		if (_skipMacroUpdate)
		{
			_skipMacroUpdate = false;
			return;
		}
		_End.Set(_Start);
		_Start.Reset();
		global::Pair<ModelNode, float> cCKLNOPEKHO = null;
		int count = _nodeWeights.Count;
		for (int i = 0; i < count; i++)
		{
			cCKLNOPEKHO = _nodeWeights[i];
			_Start.GLGNIMKANCA(cCKLNOPEKHO.First.GetStart(), cCKLNOPEKHO.Second);
		}
	}
}
