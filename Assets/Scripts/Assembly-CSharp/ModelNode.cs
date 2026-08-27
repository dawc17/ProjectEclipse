public class ModelNode
{
	public enum KOJNBGALAHM
	{
		Node = 0,
		MacroNode = 1
	}

	private ModelNode _PairNode;

	protected Vector3f _Current = new Vector3f();

	protected Vector3f BMCBFGEKMPP = new Vector3f();

	private string _Name;

	private KOJNBGALAHM KCIIELDOBOM;

	private int _Id;

	private float LLOFPDMCAPF;

	private float IPEIKBGLNKG;

	private bool MKCBIMJMGOE;

	private bool OBCDNOHNEEM;

	private bool KMKJFLHJDAF;

	private bool DIKFELJPJOB;

	private bool PALHLKDCAAC;

	private bool KPCBFAAPCNF;

	private bool IMAAMJJPIJB;

	private bool JLDCCMPMAAB;

	private bool _IsShock;

	private bool LHHBNHAAIND;

	private bool PJJPOAEPOKJ;

	protected bool BCIPCPOJJGN;

	private static Vector3f CANBEOHLBMH = new Vector3f();

	public ModelNode KOBMPGDHMIM
	{
		get
		{
			return PKOPJAHFNJG();
		}
		set
		{
			set_PairNode(value);
		}
	}

	public Vector3f HEMOJCBIJCE
	{
		get
		{
			return ICLEOFDKDIF();
		}
		set
		{
			AMPCKAIPIHH(value);
		}
	}

	public Vector3f EHHBGGDPJIM
	{
		get
		{
			return FOGHEPNAPLC();
		}
		set
		{
			LAHLFIKENPP(value);
		}
	}

	public int GJCOGFOJAEB
	{
		get
		{
			return ANAECCFDHMI();
		}
		set
		{
			set_ID(value);
		}
	}

	public float Weight
	{
		get
		{
			return FJJFKAJOFNJ();
		}
		set
		{
			NPKACGCHOLK(value);
		}
	}

	public float JJAMOMEPALM
	{
		get
		{
			return MMDLMJJHBJL();
		}
		set
		{
			BDFIDDLGDNM(value);
		}
	}

	public bool KKFBCOKMNDF
	{
		get
		{
			return MNFDCLJNFEJ();
		}
		set
		{
			OIBBADHKLNM(value);
		}
	}

	public bool MFGFJBPIECB
	{
		get
		{
			return DDBDJCHOKGJ();
		}
		set
		{
			MGPLABIFCAH(value);
		}
	}

	public bool FBKGDALBNDJ
	{
		get
		{
			return IDDNPDPEFOF();
		}
		set
		{
			CNNKFMNKDNE(value);
		}
	}

	public bool NCBPMBJCFBK
	{
		get
		{
			return NLHFJIEHKMM();
		}
	}

	public bool GAIIOCNEKEP
	{
		get
		{
			return BPJFABOAFJK();
		}
	}

	public bool BHIMNPFDCDE
	{
		get
		{
			return PKIFBKHKBPO();
		}
		set
		{
			NNHPOJFKEID(value);
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
			return EDJFLMILEBA();
		}
		set
		{
			set_IsShock(value);
		}
	}

	public bool NFDDEHDGAHP
	{
		get
		{
			return PENPLGPDNIF();
		}
		set
		{
			KMBHEMMJACN(value);
		}
	}

	public bool OIIFIGFEKKD
	{
		get
		{
			return FJIJJNLLDPM();
		}
		set
		{
			LBLPDPJGPHL(value);
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
		_Current.Set(PBOCEHNJDMI);
		BMCBFGEKMPP.Set(PBOCEHNJDMI);
		_Name = name;
		_Id = 0;
		LLOFPDMCAPF = 0f;
		IPEIKBGLNKG = 0f;
		_PairNode = null;
		OBCDNOHNEEM = true;
		KMKJFLHJDAF = false;
		IMAAMJJPIJB = false;
		JLDCCMPMAAB = false;
		PALHLKDCAAC = false;
		BCIPCPOJJGN = false;
		set_Type(KOJNBGALAHM.Node);
	}

	public ModelNode(ModelNode NPDJNAMFIKD)
	{
		_Name = NPDJNAMFIKD._Name;
		_Id = 0;
		LLOFPDMCAPF = 0f;
		IPEIKBGLNKG = 0f;
		_PairNode = null;
		OBCDNOHNEEM = true;
		KMKJFLHJDAF = false;
		IMAAMJJPIJB = false;
		JLDCCMPMAAB = false;
		PALHLKDCAAC = false;
		BCIPCPOJJGN = false;
		CopyFrom(NPDJNAMFIKD);
	}

	public ModelNode PKOPJAHFNJG()
	{
		return _PairNode;
	}

	public void set_PairNode(ModelNode value)
	{
		_PairNode = value;
	}

	public Vector3f ICLEOFDKDIF()
	{
		return _Current;
	}

	public void AMPCKAIPIHH(Vector3f value)
	{
		_Current.Set(value);
	}

	public Vector3f FOGHEPNAPLC()
	{
		return BMCBFGEKMPP;
	}

	public void LAHLFIKENPP(Vector3f value)
	{
		BMCBFGEKMPP.Set(value);
	}

	public string get_Name()
	{
		return _Name;
	}

	public KOJNBGALAHM get_Type()
	{
		return KCIIELDOBOM;
	}

	protected void set_Type(KOJNBGALAHM value)
	{
		KCIIELDOBOM = value;
		MKCBIMJMGOE = value == KOJNBGALAHM.Node;
		DIKFELJPJOB = KMKJFLHJDAF && MKCBIMJMGOE;
		KPCBFAAPCNF = OBCDNOHNEEM || !MKCBIMJMGOE;
	}

	public int ANAECCFDHMI()
	{
		return _Id;
	}

	public void set_ID(int value)
	{
		_Id = value;
	}

	public float FJJFKAJOFNJ()
	{
		return LLOFPDMCAPF;
	}

	public void NPKACGCHOLK(float value)
	{
		LLOFPDMCAPF = value;
	}

	public float MMDLMJJHBJL()
	{
		return IPEIKBGLNKG;
	}

	public void BDFIDDLGDNM(float value)
	{
		IPEIKBGLNKG = value;
	}

	public bool MNFDCLJNFEJ()
	{
		return MKCBIMJMGOE;
	}

	public void OIBBADHKLNM(bool value)
	{
		MKCBIMJMGOE = value;
	}

	public bool DDBDJCHOKGJ()
	{
		return OBCDNOHNEEM;
	}

	public void MGPLABIFCAH(bool value)
	{
		OBCDNOHNEEM = value;
		KPCBFAAPCNF = OBCDNOHNEEM || !MKCBIMJMGOE;
	}

	public bool IDDNPDPEFOF()
	{
		return KMKJFLHJDAF;
	}

	public void CNNKFMNKDNE(bool value)
	{
		KMKJFLHJDAF = value;
		if (value)
		{
			DIKFELJPJOB = KMKJFLHJDAF && MKCBIMJMGOE;
		}
		DIKFELJPJOB = KMKJFLHJDAF && MKCBIMJMGOE;
		PALHLKDCAAC = DIKFELJPJOB;
	}

	public bool NLHFJIEHKMM()
	{
		return DIKFELJPJOB;
	}

	public bool BPJFABOAFJK()
	{
		return KPCBFAAPCNF;
	}

	public bool PKIFBKHKBPO()
	{
		return IMAAMJJPIJB;
	}

	public void NNHPOJFKEID(bool value)
	{
		IMAAMJJPIJB = true;
	}

	public bool NEEJAPDCCMJ()
	{
		return JLDCCMPMAAB;
	}

	public void BGDMKGMEIDH(bool value)
	{
		JLDCCMPMAAB = value;
	}

	public bool EDJFLMILEBA()
	{
		return _IsShock;
	}

	public void set_IsShock(bool value)
	{
		_IsShock = value;
	}

	public bool PENPLGPDNIF()
	{
		return LHHBNHAAIND;
	}

	public void KMBHEMMJACN(bool value)
	{
		LHHBNHAAIND = value;
	}

	public bool FJIJJNLLDPM()
	{
		return PJJPOAEPOKJ;
	}

	public void LBLPDPJGPHL(bool value)
	{
		PJJPOAEPOKJ = value;
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
		_Current.Set(NPDJNAMFIKD._Current);
		BMCBFGEKMPP.Set(NPDJNAMFIKD.BMCBFGEKMPP);
		KCIIELDOBOM = NPDJNAMFIKD.KCIIELDOBOM;
		_Id = NPDJNAMFIKD._Id;
		LLOFPDMCAPF = NPDJNAMFIKD.LLOFPDMCAPF;
		IPEIKBGLNKG = NPDJNAMFIKD.IPEIKBGLNKG;
		MKCBIMJMGOE = NPDJNAMFIKD.MKCBIMJMGOE;
		OBCDNOHNEEM = NPDJNAMFIKD.OBCDNOHNEEM;
		KMKJFLHJDAF = NPDJNAMFIKD.KMKJFLHJDAF;
		DIKFELJPJOB = NPDJNAMFIKD.DIKFELJPJOB;
		KPCBFAAPCNF = NPDJNAMFIKD.KPCBFAAPCNF;
		IMAAMJJPIJB = NPDJNAMFIKD.IMAAMJJPIJB;
		JLDCCMPMAAB = NPDJNAMFIKD.JLDCCMPMAAB;
	}

	public void HBPBKNDPBMG()
	{
		if (MKCBIMJMGOE)
		{
			if (DIKFELJPJOB != PALHLKDCAAC)
			{
				int num = 0;
				num++;
			}
			DIKFELJPJOB = PALHLKDCAAC;
		}
	}

	public void KCDIAMOLAKB()
	{
		if (MKCBIMJMGOE)
		{
			DIKFELJPJOB = false;
		}
	}

	public void OIEPNGBEECN()
	{
		BMCBFGEKMPP.Set(_Current);
	}

	public void ChangeSpeed(float ELDDBMFEFIP)
	{
		Vector3f aKKEJFKBIHF = Vector3f.MJOKEBGPHKB(_Current, BMCBFGEKMPP);
		if (DIKFELJPJOB)
		{
		}
		BMCBFGEKMPP.Set(Vector3f.MJOKEBGPHKB(_Current, aKKEJFKBIHF));
	}

	public void TimeStep(float HCNEFANPGCK)
	{
		CANBEOHLBMH.Set(_Current);
		CANBEOHLBMH.EHGLHOGAIDI(BMCBFGEKMPP);
		if (DIKFELJPJOB)
		{
			CANBEOHLBMH.Multiply(1f - IPEIKBGLNKG);
		}
		CANBEOHLBMH.Add(_Current);
		Vector3f cANBEOHLBMH = CANBEOHLBMH;
		cANBEOHLBMH.IBNFLLGPOLD(cANBEOHLBMH.OBIMBNIBEFG() + HCNEFANPGCK);
		BMCBFGEKMPP.Set(_Current);
		_Current.Set(CANBEOHLBMH);
	}

	public override string ToString()
	{
		return string.Format("ModelNode(name = [{0}] c_pos= [{1}])", _Name, _Current);
	}
}
