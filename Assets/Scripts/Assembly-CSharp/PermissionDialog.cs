public class PermissionDialog
{
	private string IBEDAJAAAKL;

	private string GDAEGLJODKA;

	private string[] _Buttons;

	public string Title
	{
		get
		{
			return IDLDKFEPJLI();
		}
		set
		{
			BFEHCFMFCGP(value);
		}
	}

	public string GGDJIPKMKFC
	{
		get
		{
			return ILMJJEMPKCN();
		}
		set
		{
			MHMDIMIEPLL(value);
		}
	}

	public string[] DHKDOHFKOOJ
	{
		get
		{
			return EDPLBKPHCMN();
		}
		set
		{
			set_Buttons(value);
		}
	}

	public PermissionDialog(string DKPOMENFJDA, string NGEPNAJJHCD, params string[] NHLLHLLCJJP)
	{
		IBEDAJAAAKL = DKPOMENFJDA;
		GDAEGLJODKA = NGEPNAJJHCD;
		_Buttons = NHLLHLLCJJP;
	}

	public PermissionDialog()
	{
	}

	public string IDLDKFEPJLI()
	{
		return IBEDAJAAAKL;
	}

	public void BFEHCFMFCGP(string value)
	{
		IBEDAJAAAKL = value;
	}

	public string ILMJJEMPKCN()
	{
		return GDAEGLJODKA;
	}

	public void MHMDIMIEPLL(string value)
	{
		GDAEGLJODKA = value;
	}

	public string[] EDPLBKPHCMN()
	{
		return _Buttons;
	}

	public void set_Buttons(string[] value)
	{
		_Buttons = value;
	}

	public static PermissionDialog JJLLCGBEJLF(string DKPOMENFJDA, string NGEPNAJJHCD, string JNDOLCKKGIE)
	{
		return new PermissionDialog(DKPOMENFJDA, NGEPNAJJHCD, JNDOLCKKGIE);
	}

	public static PermissionDialog HMCLCLAJHGC(string DKPOMENFJDA, string NGEPNAJJHCD, string JNDOLCKKGIE, string PDPPGLDMPGH, string ENBBEFMEILD)
	{
		return new PermissionDialog(DKPOMENFJDA, NGEPNAJJHCD, JNDOLCKKGIE, PDPPGLDMPGH, ENBBEFMEILD);
	}
}
