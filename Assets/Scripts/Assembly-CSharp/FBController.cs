using System;
using Facebook.Unity;

public class FBController
{
	private static FBController _Instance;

	private static bool IGMALNGPGAA
	{
		get
		{
			return HENFPEIBLAJ();
		}
	}

	public static FBController BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	private static bool HENFPEIBLAJ()
	{
		return SystemProperties.MEBGOGMJFLM() || SystemProperties.IPJFCBAGMJJ();
	}

	public static FBController ELEBLBJKDBI()
	{
		if (_Instance == null)
		{
			_Instance = new FBController();
		}
		return _Instance;
	}

	public static void Init()
	{
		if (HENFPEIBLAJ())
		{
			ELEBLBJKDBI().JBKKFFPFFJH();
		}
	}

	public void JBKKFFPFFJH()
	{
		if (!FB.IsInitialized)
		{
			try
			{
				FB.Init(CMNKIJGFKGI, (bool LKGKEIDBFEJ) =>
				{
				});
				return;
			}
			catch (Exception ex)
			{
				LLLOJBFMONN.Error(ex.ToString());
				return;
			}
		}
		CCJONONHCGA();
	}

	public static void LMBHFAHHDKI(float NICNMHCJIBJ, string MDDNHLBDJBN = "USD")
	{
		if (HENFPEIBLAJ())
		{
			ELEBLBJKDBI().BAPOHDGEIGD(NICNMHCJIBJ, MDDNHLBDJBN);
		}
	}

	public void BAPOHDGEIGD(float NICNMHCJIBJ, string MDDNHLBDJBN = "USD")
	{
		if (FB.IsInitialized)
		{
			try
			{
				FB.LogPurchase(NICNMHCJIBJ, MDDNHLBDJBN);
				return;
			}
			catch (Exception ex)
			{
				LLLOJBFMONN.Error(ex.ToString());
				return;
			}
		}
		LLLOJBFMONN.Error("Failed to Initialize the Facebook SDK");
	}

	private void CMNKIJGFKGI()
	{
		if (FB.IsInitialized)
		{
			CCJONONHCGA();
		}
		else
		{
			LLLOJBFMONN.Error("Failed to Initialize the Facebook SDK");
		}
	}

	private void CCJONONHCGA()
	{
		try
		{
			FB.ActivateApp();
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.ToString());
		}
	}
}
