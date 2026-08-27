public class BaseEventLog : global::EventDispatcher<object>
{
	public enum MAPBOFEBBKD
	{
		NotLogging = 0,
		Logging = 1,
		Undecided = 2
	}

	private MAPBOFEBBKD PCFBFCODIED;

	public bool CFBGHLDIEOH
	{
		get
		{
			return DKKNEBIGLPJ();
		}
		set
		{
			AHPANGPNJPG(value);
		}
	}

	public bool DKKNEBIGLPJ()
	{
		return PCFBFCODIED != MAPBOFEBBKD.NotLogging;
	}

	public void AHPANGPNJPG(bool value)
	{
		if (value)
		{
			PCFBFCODIED = MAPBOFEBBKD.Logging;
			Send();
		}
		else
		{
			PCFBFCODIED = MAPBOFEBBKD.NotLogging;
		}
	}

	public void Send()
	{
	}
}
