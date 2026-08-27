using UnityEngine;

public class FightStatistics
{
	public enum EMKEIEJMONM
	{
		STYLE_TURTLE = 0,
		STYLE_HARD = 1,
		STYLE_BRUTAL = 2,
		STYLE_AGGRESSIVE = 3,
		STYLE_CRAZY = 4,
		STYLE_FANTASTIC = 5
	}

	private const int MIN_COMBO = 3;

	private int NPAIABEJHDI;

	private EMKEIEJMONM DFBCMLMHJKI;

	private int MLIIDKMAJLD;

	private int IDACPEKPPCA;

	private int IIENCMJNOLK;

	private int NLCGGNDICOK;

	private int NECLBAGGFCI;

	private bool _noStrikes = true;

	private int DPGNKMGLBMG;

	private int MFDOLKIPDHA;

	public EMKEIEJMONM HCJPMGKAAMN
	{
		get
		{
			return HALCJLMJDII();
		}
		set
		{
			KHFMMPCKMKE(value);
		}
	}

	public EMKEIEJMONM HALCJLMJDII()
	{
		return DFBCMLMHJKI;
	}

	public void KHFMMPCKMKE(EMKEIEJMONM value)
	{
		DFBCMLMHJKI = (EMKEIEJMONM)Mathf.Max((int)DFBCMLMHJKI, (int)value);
	}

	public void POPNNILNKAE(float JNNBEHPCCOB, float FEMPIBLJEAP)
	{
		if (JNNBEHPCCOB == FEMPIBLJEAP)
		{
			NPAIABEJHDI++;
		}
	}

	public void POPNNILNKAE()
	{
		NPAIABEJHDI++;
	}

	public void Draw()
	{
		if (DPGNKMGLBMG > 0)
		{
			DPGNKMGLBMG--;
			if (DPGNKMGLBMG == 0)
			{
				GBLAEAKGKLK();
			}
		}
	}

	public void Reset()
	{
		_noStrikes = true;
		DPGNKMGLBMG = 0;
		MFDOLKIPDHA = 0;
	}

	private bool NDOBPFBGKHG()
	{
		return IIENCMJNOLK >= 3;
	}

	private void GBLAEAKGKLK()
	{
		if (MFDOLKIPDHA > 0)
		{
			IDACPEKPPCA++;
			IIENCMJNOLK = Mathf.Max(IIENCMJNOLK, MFDOLKIPDHA);
			MFDOLKIPDHA = 0;
		}
	}
}
