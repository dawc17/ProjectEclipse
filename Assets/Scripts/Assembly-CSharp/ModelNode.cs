public class ModelNode
{
	public enum NodeType
	{
		Node = 0,
		MacroNode = 1
	}

	private ModelNode _PairNode;

	protected Vector3f _Start = new Vector3f();

	protected Vector3f _End = new Vector3f();

	private string _Name;

	private NodeType _Type;

	private int _Id;

	private float _Weight;

	private float _Attenuation;

	private bool _IsNode;

	private bool _IsFixed;

	private bool _IsCloth;

	private bool _IsPhysics;

	private bool PALHLKDCAAC;

    // Evidence Points that this is a Fixed Boolean but OBCDNOHNEEM is directly set from the model loader as Fixed so I have no clue which one is which
	// so I temporarily named it from the conditional statement.
    private bool _IsFixedAndNotNode;

	private bool _Visible;

	// No Clue what this is...
	private bool JLDCCMPMAAB;

	private bool _IsShock;

	private bool _Collisible;

	private bool _Weak;

	// no clue what this is either
	protected bool BCIPCPOJJGN;

	private static Vector3f _TimeStepVector = new Vector3f();

	public ModelNode KOBMPGDHMIM
	{
		get
		{
			return PKOPJAHFNJG();
		}
		set
		{
			SetPairNode(value);
		}
	}

	public Vector3f HEMOJCBIJCE
	{
		get
		{
			return GetStart();
		}
		set
		{
			SetStart(value);
		}
	}

	public Vector3f EHHBGGDPJIM
	{
		get
		{
			return GetEnd();
		}
		set
		{
			SetEnd(value);
		}
	}

	public int GJCOGFOJAEB
	{
		get
		{
			return GetID();
		}
		set
		{
			SetID(value);
		}
	}

	public float Weight
	{
		get
		{
			return GetWeight();
		}
		set
		{
			SetMass(value);
		}
	}

	public float JJAMOMEPALM
	{
		get
		{
			return GetAttenuation();
		}
		set
		{
			SetAttenuation(value);
		}
	}

	public bool KKFBCOKMNDF
	{
		get
		{
			return IsNode();
		}
		set
		{
			SetIsNode(value);
		}
	}

	public bool MFGFJBPIECB
	{
		get
		{
			return IsFixed();
		}
		set
		{
			SetFixed(value);
		}
	}

	public bool FBKGDALBNDJ
	{
		get
		{
			return IsCloth();
		}
		set
		{
			SetCloth(value);
		}
	}

	public bool NCBPMBJCFBK
	{
		get
		{
			return IsPhysics();
		}
	}

	public bool GAIIOCNEKEP
	{
		get
		{
			return IsFixedAndIsNotNode();
		}
	}

	public bool BHIMNPFDCDE
	{
		get
		{
			return IsVisible();
		}
		set
		{
			SetVisible(value);
		}
	}

	public bool MOAOLGNKEPI
	{
		get
		{
			return NEEJAPDCCMJ();
		}
		set
		{
			BGDMKGMEIDH(value);
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
			SetIsShock(value);
		}
	}

	public bool NFDDEHDGAHP
	{
		get
		{
			return IsCollisible();
		}
		set
		{
			SetCollisible(value);
		}
	}

	public bool OIIFIGFEKKD
	{
		get
		{
			return IsWeak();
		}
		set
		{
			SetWeak(value);
		}
	}

	public bool AOFHEEAKBOM
	{
		get
		{
			return GGIDOLBCAMN();
		}
		set
		{
			OHMNDOKBGGA(value);
		}
	}

	public ModelNode(string name)
		: this(name, new Vector3f())
	{
	}

	public ModelNode(string name, Vector3f PBOCEHNJDMI)
	{
		_Start.Set(PBOCEHNJDMI);
		_End.Set(PBOCEHNJDMI);
		_Name = name;
		_Id = 0;
		_Weight = 0f;
		_Attenuation = 0f;
		_PairNode = null;
		_IsFixed = true;
		_IsCloth = false;
		_Visible = false;
		JLDCCMPMAAB = false;
		PALHLKDCAAC = false;
		BCIPCPOJJGN = false;
		SetType(NodeType.Node);
	}

	public ModelNode(ModelNode NPDJNAMFIKD)
	{
		_Name = NPDJNAMFIKD._Name;
		_Id = 0;
		_Weight = 0f;
		_Attenuation = 0f;
		_PairNode = null;
		_IsFixed = true;
		_IsCloth = false;
		_Visible = false;
		JLDCCMPMAAB = false;
		PALHLKDCAAC = false;
		BCIPCPOJJGN = false;
		CopyFrom(NPDJNAMFIKD);
	}

	public ModelNode PKOPJAHFNJG()
	{
		return _PairNode;
	}

	public void SetPairNode(ModelNode value)
	{
		_PairNode = value;
	}

	public Vector3f GetStart()
	{
		return _Start;
	}

	public void SetStart(Vector3f value)
	{
		_Start.Set(value);
	}

	public Vector3f GetEnd()
	{
		return _End;
	}

	public void SetEnd(Vector3f value)
	{
		_End.Set(value);
	}

	public string GetName()
	{
		return _Name;
	}

	public NodeType GetNodeType()
	{
		return _Type;
	}

	protected void SetType(NodeType value)
	{
		_Type = value;
		_IsNode = value == NodeType.Node;
		_IsPhysics = _IsCloth && _IsNode;
		_IsFixedAndNotNode = _IsFixed || !_IsNode;
	}

	public int GetID()
	{
		return _Id;
	}

	public void SetID(int value)
	{
		_Id = value;
	}

	public float GetWeight()
	{
		return _Weight;
	}

	public void SetMass(float value)
	{
		_Weight = value;
	}

	public float GetAttenuation()
	{
		return _Attenuation;
	}

	public void SetAttenuation(float value)
	{
		_Attenuation = value;
	}

	public bool IsNode()
	{
		return _IsNode;
	}

	public void SetIsNode(bool value)
	{
		_IsNode = value;
	}

	public bool IsFixed()
	{
		return _IsFixed;
	}

	public void SetFixed(bool value)
	{
		_IsFixed = value;
		_IsFixedAndNotNode = _IsFixed || !_IsNode;
	}

	public bool IsCloth()
	{
		return _IsCloth;
	}

	public void SetCloth(bool value)
	{
		_IsCloth = value;
		if (value)
		{
			_IsPhysics = _IsCloth && _IsNode;
		}
		_IsPhysics = _IsCloth && _IsNode;
		PALHLKDCAAC = _IsPhysics;
	}

	public bool IsPhysics()
	{
		return _IsPhysics;
	}

	public bool IsFixedAndIsNotNode()
	{
		return _IsFixedAndNotNode;
	}

	public bool IsVisible()
	{
		return _Visible;
	}

	public void SetVisible(bool value)
	{
		_Visible = true;
	}

	public bool NEEJAPDCCMJ()
	{
		return JLDCCMPMAAB;
	}

	public void BGDMKGMEIDH(bool value)
	{
		JLDCCMPMAAB = value;
	}

	public bool IsShock()
	{
		return _IsShock;
	}

	public void SetIsShock(bool value)
	{
		_IsShock = value;
	}

	public bool IsCollisible()
	{
		return _Collisible;
	}

	public void SetCollisible(bool value)
	{
		_Collisible = value;
	}

	public bool IsWeak()
	{
		return _Weak;
	}

	public void SetWeak(bool value)
	{
		_Weak = value;
	}

	public bool GGIDOLBCAMN()
	{
		return BCIPCPOJJGN;
	}

	public void OHMNDOKBGGA(bool value)
	{
		BCIPCPOJJGN = value;
	}

	public void CopyFrom(ModelNode NPDJNAMFIKD)
	{
		_Start.Set(NPDJNAMFIKD._Start);
		_End.Set(NPDJNAMFIKD._End);
		_Type = NPDJNAMFIKD._Type;
		_Id = NPDJNAMFIKD._Id;
		_Weight = NPDJNAMFIKD._Weight;
		_Attenuation = NPDJNAMFIKD._Attenuation;
		_IsNode = NPDJNAMFIKD._IsNode;
		_IsFixed = NPDJNAMFIKD._IsFixed;
		_IsCloth = NPDJNAMFIKD._IsCloth;
		_IsPhysics = NPDJNAMFIKD._IsPhysics;
		_IsFixedAndNotNode = NPDJNAMFIKD._IsFixedAndNotNode;
		_Visible = NPDJNAMFIKD._Visible;
		JLDCCMPMAAB = NPDJNAMFIKD.JLDCCMPMAAB;
	}

	public void HBPBKNDPBMG()
	{
		if (_IsNode)
		{
			if (_IsPhysics != PALHLKDCAAC)
			{
				int num = 0;
				num++;
			}
			_IsPhysics = PALHLKDCAAC;
		}
	}

	public void KCDIAMOLAKB()
	{
		if (_IsNode)
		{
			_IsPhysics = false;
		}
	}

	public void SetEnd()
	{
		_End.Set(_Start);
	}

	public void ChangeSpeed(float ELDDBMFEFIP)
	{
		Vector3f aKKEJFKBIHF = Vector3f.MJOKEBGPHKB(_Start, _End);
		if (_IsPhysics)
		{
		}
		_End.Set(Vector3f.MJOKEBGPHKB(_Start, aKKEJFKBIHF));
	}

	public void TimeStep(float gravity)
	{
		_TimeStepVector.Set(_Start);
		_TimeStepVector.Subtract(_End);
		if (_IsPhysics)
		{
			_TimeStepVector.Multiply(1f - _Attenuation);
		}
		_TimeStepVector.Add(_Start);
		Vector3f cANBEOHLBMH = _TimeStepVector;
		cANBEOHLBMH.SetY(cANBEOHLBMH.GetY() + gravity);
		_End.Set(_Start);
		_Start.Set(_TimeStepVector);
	}

	public override string ToString()
	{
		return string.Format("ModelNode(name = [{0}] c_pos= [{1}])", _Name, _Start);
	}
}
