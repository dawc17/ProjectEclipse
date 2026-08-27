using System;
using System.Collections.Generic;

public sealed class TagMappings
{
	private readonly IDictionary<string, Type> INONLMCLKPG;

	public TagMappings()
	{
		INONLMCLKPG = new Dictionary<string, Type>();
	}

	public TagMappings(IDictionary<string, Type> INONLMCLKPG)
	{
		this.INONLMCLKPG = new Dictionary<string, Type>(INONLMCLKPG);
	}

	public void Add(string EDLADAAKMDF, Type JPEFEBICPFI)
	{
		INONLMCLKPG.Add(EDLADAAKMDF, JPEFEBICPFI);
	}

	internal Type GetMapping(string EDLADAAKMDF)
	{
		Type value;
		if (INONLMCLKPG.TryGetValue(EDLADAAKMDF, out value))
		{
			return value;
		}
		return null;
	}
}
