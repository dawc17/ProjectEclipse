using System.Collections.Generic;

public class Packs
{
	private List<JBKAOMLJCEL> OALOKFBBHOM = new List<JBKAOMLJCEL>();

	public List<JBKAOMLJCEL> KAPKIHOMADL
	{
		get
		{
			return ONJDMMIHCMG();
		}
	}

	public List<JBKAOMLJCEL> ONJDMMIHCMG()
	{
		return OALOKFBBHOM;
	}

	public void Reset()
	{
		OALOKFBBHOM.Clear();
	}

	public void DDKKLHDOFNG(string name, string BEPKJNKCKPH, string PEEOEOMEBFG, bool LCDCAKLKHMI, string HDPBNCNCMOH, bool AHDLCJFCJMJ)
	{
		JBKAOMLJCEL jBKAOMLJCEL = new JBKAOMLJCEL();
		jBKAOMLJCEL.Name = name;
		jBKAOMLJCEL.Url = BEPKJNKCKPH;
		jBKAOMLJCEL.Size = PEEOEOMEBFG;
		jBKAOMLJCEL.EFJLHFFGCIF = LCDCAKLKHMI;
		jBKAOMLJCEL.NDDHELJHHKI = HDPBNCNCMOH;
		jBKAOMLJCEL.NBEEINKJMPK = AHDLCJFCJMJ;
		int result = 0;
		if (int.TryParse(PEEOEOMEBFG, out result))
		{
			jBKAOMLJCEL.Size = ((float)result / 1000000f).ToString("#.#");
		}
		jBKAOMLJCEL.HKPOAABOLHN = result;
		OALOKFBBHOM.Add(jBKAOMLJCEL);
	}

	public JBKAOMLJCEL OCKOCHAINHG(string name)
	{
		return OALOKFBBHOM.Find((JBKAOMLJCEL DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
	}
}
