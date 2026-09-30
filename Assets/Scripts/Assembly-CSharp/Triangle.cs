public class Triangle
{
	private string _name;

	// best guess for name
	private ModelNode[] _nodes = new ModelNode[3];

	// Snapshot codecs preserve the array identity as well as its node references.
	internal ModelNode[] Nodes { get => _nodes; set => _nodes = value; }

	public ModelNode IFKIMCJKHPF
	{
		get
		{
			return LACAPAAKHGF();
		}
		set
		{
			DGNONLPFKKL(value);
		}
	}

	public ModelNode IHGCBKLIDJF
	{
		get
		{
			return BGDMIKIODPC();
		}
		set
		{
			FCMMFAEBDDL(value);
		}
	}

	public ModelNode JBPNNHDCBJD
	{
		get
		{
			return DBOJFAAGEKB();
		}
		set
		{
			DADGEIJKBMM(value);
		}
	}

	public Triangle()
	{
		_nodes[0] = new ModelNode("tmp");
		_nodes[1] = new ModelNode("tmp");
		_nodes[2] = new ModelNode("tmp");
	}

	public Triangle(ModelNode NOLAMPHAAII, ModelNode BIPPDOPJCOI, ModelNode LJOMMHPDFCI, string name)
	{
		_nodes[0] = NOLAMPHAAII;
		_nodes[1] = BIPPDOPJCOI;
		_nodes[2] = LJOMMHPDFCI;
		_name = name;
	}

	public Triangle(Triangle EDANJNHMLBC)
	{
		_nodes[0] = EDANJNHMLBC._nodes[0];
		_nodes[1] = EDANJNHMLBC._nodes[1];
		_nodes[2] = EDANJNHMLBC._nodes[2];
		_name = EDANJNHMLBC._name;
	}

	public string get_Name()
	{
		return _name;
	}

	public void set_Name(string value)
	{
		_name = value;
	}

	public ModelNode LACAPAAKHGF()
	{
		return _nodes[0];
	}

	public void DGNONLPFKKL(ModelNode value)
	{
		_nodes[0] = value;
	}

	public ModelNode BGDMIKIODPC()
	{
		return _nodes[1];
	}

	public void FCMMFAEBDDL(ModelNode value)
	{
		_nodes[1] = value;
	}

	public ModelNode DBOJFAAGEKB()
	{
		return _nodes[2];
	}

	public void DADGEIJKBMM(ModelNode value)
	{
		_nodes[2] = value;
	}

	public void CopyFrom(Triangle CJAGCDNBEPA)
	{
		_nodes[0].CopyFrom(CJAGCDNBEPA.LACAPAAKHGF());
		_nodes[1].CopyFrom(CJAGCDNBEPA.BGDMIKIODPC());
		_nodes[2].CopyFrom(CJAGCDNBEPA.DBOJFAAGEKB());
	}

	public void GOKCABDNIKF()
	{
	}

	private void GBKLNGHEICJ()
	{
	}
}
