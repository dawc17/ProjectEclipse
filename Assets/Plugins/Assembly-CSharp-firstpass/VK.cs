using Nekki.Social;

public class VK : ISocialNetwork
{
	private SocialWrapper _wrap;

	public bool PHKJIMLOFBA
	{
		get
		{
			return CMBNMEACMMK();
		}
	}

	public bool OJNBOLKNEPO
	{
		get
		{
			return CEELEFHIJKK();
		}
	}

	public DFIPCKIEILP FAAHDOEMDCJ
	{
		get
		{
			return CCOHIOKHFKI();
		}
	}

	public void Init(SocialWrapper JEMGDGKGMAJ)
	{
		_wrap = JEMGDGKGMAJ;
	}

	public void PGAMICBKPMF(string AOKMNKOIMHI)
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
			return;
		}
		_wrap.RequestUsersInfo(new string[1] { AOKMNKOIMHI });
	}

	public void GetUsers(string[] JAIEEFOCDAA)
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.RequestUsersInfo(JAIEEFOCDAA);
		}
	}

	public void LHCBELEDOEP()
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.KCBMPAILEIN();
		}
	}

	public void OPDGBGPEEEE()
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.NKCLMBADENN();
		}
	}

	public void PAFIGDDLACE()
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.RequestBookmark(false);
		}
	}

	public void DNNNAMJBEPE()
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.RequestBookmark(true);
		}
	}

	public void CLALKCGLFFM()
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.CNKJLMJAFNL();
		}
	}

	public void DNCCPGDMLON(string AOKMNKOIMHI, string LIOGIBJBHAH, string DMNBDBJNKME)
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.MGFOEBCEKNB(AOKMNKOIMHI, LIOGIBJBHAH, DMNBDBJNKME);
		}
	}

	public void Buy(int item)
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.Buy(item.ToString());
		}
	}

	public void Buy(string item)
	{
		if (!_wrap.get_Initialized())
		{
			AdvLog.LOPHFKMOPAA("you must call Social.Init(..) method first");
		}
		else
		{
			_wrap.Buy(item);
		}
	}

	public bool CMBNMEACMMK()
	{
		return true;
	}

	public bool CEELEFHIJKK()
	{
		return true;
	}

	public DFIPCKIEILP CCOHIOKHFKI()
	{
		return DFIPCKIEILP.VKontakte;
	}
}
