public class Triangle
{
	private string _name;

	private ModelNode[] CFPIOKDFJCH = new ModelNode[3];

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
		CFPIOKDFJCH[0] = new ModelNode("tmp");
		CFPIOKDFJCH[1] = new ModelNode("tmp");
		CFPIOKDFJCH[2] = new ModelNode("tmp");
	}

	public Triangle(ModelNode NOLAMPHAAII, ModelNode BIPPDOPJCOI, ModelNode LJOMMHPDFCI, string name)
	{
		CFPIOKDFJCH[0] = NOLAMPHAAII;
		CFPIOKDFJCH[1] = BIPPDOPJCOI;
		CFPIOKDFJCH[2] = LJOMMHPDFCI;
		_name = name;
	}

	public Triangle(Triangle EDANJNHMLBC)
	{
		CFPIOKDFJCH[0] = EDANJNHMLBC.CFPIOKDFJCH[0];
		CFPIOKDFJCH[1] = EDANJNHMLBC.CFPIOKDFJCH[1];
		CFPIOKDFJCH[2] = EDANJNHMLBC.CFPIOKDFJCH[2];
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
		return CFPIOKDFJCH[0];
	}

	public void DGNONLPFKKL(ModelNode value)
	{
		CFPIOKDFJCH[0] = value;
	}

	public ModelNode BGDMIKIODPC()
	{
		return CFPIOKDFJCH[1];
	}

	public void FCMMFAEBDDL(ModelNode value)
	{
		CFPIOKDFJCH[1] = value;
	}

	public ModelNode DBOJFAAGEKB()
	{
		return CFPIOKDFJCH[2];
	}

	public void DADGEIJKBMM(ModelNode value)
	{
		CFPIOKDFJCH[2] = value;
	}

	public void CopyFrom(Triangle CJAGCDNBEPA)
	{
		CFPIOKDFJCH[0].CopyFrom(CJAGCDNBEPA.LACAPAAKHGF());
		CFPIOKDFJCH[1].CopyFrom(CJAGCDNBEPA.BGDMIKIODPC());
		CFPIOKDFJCH[2].CopyFrom(CJAGCDNBEPA.DBOJFAAGEKB());
	}

	public void GOKCABDNIKF()
	{
	}

	private void GBKLNGHEICJ()
	{
	}
}
