using System.Globalization;
using System.IO;
using YamlDotNet.Core;

public class EventReader
{
	private readonly IParser BPGMNGAJMKK;

	private bool endOfStream;

	public IParser BDCBNKDELDD
	{
		get
		{
			return OAPMECPBPKJ();
		}
	}

	public EventReader(IParser BPGMNGAJMKK)
	{
		this.BPGMNGAJMKK = BPGMNGAJMKK;
		PCCMLADDNDG();
	}

	public IParser OAPMECPBPKJ()
	{
		return BPGMNGAJMKK;
	}

	public T DODGGCGJJLL<T>() where T : ParsingEvent
	{
		T val = GNNPKHDPGLN<T>();
		if (val == null)
		{
			ParsingEvent jMKLCDAKEOG = BPGMNGAJMKK.AOJJOEHEPGM();
			throw new YamlException(jMKLCDAKEOG.OGPHJPFHBJL(), jMKLCDAKEOG.GDJHIJHFPHA(), string.Format(CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", typeof(T).Name, jMKLCDAKEOG.GetType().Name, jMKLCDAKEOG.OGPHJPFHBJL()));
		}
		return val;
	}

	public bool GPHIFFOGOGN<T>() where T : ParsingEvent
	{
		BPHMOEBOHLN();
		return BPGMNGAJMKK.AOJJOEHEPGM() is T;
	}

	private void BPHMOEBOHLN()
	{
		if (endOfStream)
		{
			throw new EndOfStreamException();
		}
	}

	public T GNNPKHDPGLN<T>() where T : ParsingEvent
	{
		if (!GPHIFFOGOGN<T>())
		{
			return (T)null;
		}
		T result = (T)BPGMNGAJMKK.AOJJOEHEPGM();
		PCCMLADDNDG();
		return result;
	}

	public T Peek<T>() where T : ParsingEvent
	{
		if (!GPHIFFOGOGN<T>())
		{
			return (T)null;
		}
		return (T)BPGMNGAJMKK.AOJJOEHEPGM();
	}

	public void FHCPPKNIOKB()
	{
		int num = 0;
		do
		{
			num += Peek<ParsingEvent>().DPIMLJJFMCO();
			PCCMLADDNDG();
		}
		while (num > 0);
	}

	private void PCCMLADDNDG()
	{
		endOfStream = !BPGMNGAJMKK.PCCMLADDNDG();
	}
}
