using System.Collections.Generic;

public class SFSocial : global::EventDispatcher<object>
{
	public class DDANIONNBLJ
	{
		public string name;

		public int progress;

		public bool complete;

		public DDANIONNBLJ()
		{
			name = string.Empty;
			progress = 0;
			complete = false;
		}

		public DDANIONNBLJ(string _name, int JLHFNCKLMDI)
		{
			name = _name;
			progress = JLHFNCKLMDI;
			complete = false;
		}
	}

	public enum NKFIPJOGFEK
	{
		EVENT_AUTHORIZE_FINISH = 0,
		EVENT_AUTHORIZE_CANCEL = 1,
		EVENT_POST_ACHIEVEMENT_FAILED = 2,
		EVENT_REQUEST_VERIFICATION_DATA_SUCCSESS = 3,
		EVENT_REQUEST_VERIFICATION_DATA_FAILED = 4
	}

	public enum JMPNJFMJJAI
	{
		SOCIAL_NONE = 0,
		SOCIAL_NEKKI = 1,
		SOCIAL_GOOGLE = 2,
		SOCIAL_GAME_CENTER = 3
	}

	private static SFSocial EDAPJLKMFPC;

	private static SFSocial FOJDAIHBAJP;

	public static void OFEBIPBOBPC(SFSocial ENMMMPLLLCD)
	{
		EDAPJLKMFPC = ENMMMPLLLCD;
	}

	public static void PENFENANIMC(SFSocial ENMMMPLLLCD)
	{
		FOJDAIHBAJP = ENMMMPLLLCD;
	}

	public static SFSocial GBPBIPFIOJH()
	{
		EDAPJLKMFPC = new SFSocial();
		return EDAPJLKMFPC;
	}

	public virtual void DIKPCDIONOJ()
	{
		CallEvent(0, 0);
	}

	public virtual void DNEAALKFIPC()
	{
		DIKPCDIONOJ();
	}

	public virtual string HBPJFLOFIJO()
	{
		return string.Empty;
	}

	public virtual string EOBBEMNEIOA()
	{
		if (FOJDAIHBAJP != null)
		{
			return FOJDAIHBAJP.HBPJFLOFIJO();
		}
		return string.Empty;
	}

	public virtual void FLJILJDHNLJ(List<DDANIONNBLJ> CIMGCGDDKCE)
	{
	}

	public virtual void MMGHEKOEHDB()
	{
	}

	public virtual bool CMOOANCABOG()
	{
		return true;
	}

	public virtual void BEBEFJHBBFL()
	{
	}

	public virtual void FJJDHDEGJGE()
	{
	}

	public virtual bool GEJNIMAILDA()
	{
		return false;
	}

	public virtual string APJHLLAHHHP()
	{
		return string.Empty;
	}

	public virtual void HMLPAADACAM()
	{
	}

	protected void CFNPOHHLNKM(DDANIONNBLJ PGAGNLJABIE)
	{
		CallEvent(2, PGAGNLJABIE);
	}
}
