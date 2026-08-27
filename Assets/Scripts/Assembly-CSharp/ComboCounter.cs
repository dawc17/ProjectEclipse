public class ComboCounter : global::EventDispatcher<object>
{
	public enum ADPEHMMNJIA
	{
		ON_COMBO_CHANGE = 0
	}

	private int IHLGIDMENOP;

	private int PFGLKBODOPG;

	private int LMEPJHBGOAE;

	private int JLDAIMMOAMC;

	private bool LHJIAOJDHLB;

	public int GKAEJDCDMHC
	{
		get
		{
			return NPDOLGNNINO();
		}
	}

	public int AAKOCIPFDNM
	{
		get
		{
			return POKBOKHJJPL();
		}
	}

	public int DEENENNCBBC
	{
		get
		{
			return CLPDEPPPJFE();
		}
	}

	public bool BKLPNNIBJBE
	{
		get
		{
			return FPPKOMOPDJJ();
		}
	}

	public int NPDOLGNNINO()
	{
		return IHLGIDMENOP;
	}

	public int POKBOKHJJPL()
	{
		return PFGLKBODOPG;
	}

	public int CLPDEPPPJFE()
	{
		return JLDAIMMOAMC;
	}

	public bool FPPKOMOPDJJ()
	{
		return LHJIAOJDHLB;
	}

	public void HHHDLDIHKBJ()
	{
		if (!LHJIAOJDHLB)
		{
			return;
		}
		LMEPJHBGOAE++;
		if (LMEPJHBGOAE > GameUtils.KCBHAMHLGBC())
		{
			JLDAIMMOAMC = IHLGIDMENOP;
			Reset();
			if (JLDAIMMOAMC >= GameUtils.NPDOLGNNINO())
			{
				CallEvent(0, IHLGIDMENOP);
			}
		}
	}

	public void INNGMENHNEL()
	{
		LHJIAOJDHLB = true;
		LMEPJHBGOAE = 0;
		PFGLKBODOPG++;
		if (PFGLKBODOPG >= GameUtils.NPDOLGNNINO())
		{
			IHLGIDMENOP = PFGLKBODOPG;
			JLDAIMMOAMC = IHLGIDMENOP;
			CallEvent(0, IHLGIDMENOP);
		}
	}

	public void Reset()
	{
		LHJIAOJDHLB = false;
		LMEPJHBGOAE = 0;
		PFGLKBODOPG = 0;
		IHLGIDMENOP = 0;
	}
}
