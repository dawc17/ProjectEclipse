using System;
using System.Collections.Generic;

public sealed class StreamFragment : IYamlSerializable
{
	private readonly List<ParsingEvent> DNBFFLFBDOB = new List<ParsingEvent>();

	public IList<ParsingEvent> AJCMBMJGJEG
	{
		get
		{
			return PHLLJJNCEIH();
		}
	}

	public IList<ParsingEvent> PHLLJJNCEIH()
	{
		return DNBFFLFBDOB;
	}

	void IYamlSerializable.ReadYaml(IParser BPGMNGAJMKK)
	{
		DNBFFLFBDOB.Clear();
		int num = 0;
		do
		{
			if (!BPGMNGAJMKK.PCCMLADDNDG())
			{
				throw new InvalidOperationException("The parser has reached the end before deserialization completed.");
			}
			DNBFFLFBDOB.Add(BPGMNGAJMKK.AOJJOEHEPGM());
			num += BPGMNGAJMKK.AOJJOEHEPGM().DPIMLJJFMCO();
		}
		while (num > 0);
	}

	void IYamlSerializable.WriteYaml(NEKGJNOFOFN NPIDIMCLNEM)
	{
		foreach (ParsingEvent item in DNBFFLFBDOB)
		{
			NPIDIMCLNEM.Emit(item);
		}
	}
}
