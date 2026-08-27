using System.Collections.Generic;

internal static class YamlTypeConverters
{
	private static readonly IEnumerable<IYamlTypeConverter> PGMCBEMHNFO = new IYamlTypeConverter[1]
	{
		new GuidConverter()
	};

	public static IEnumerable<IYamlTypeConverter> BDPEAGJCHHH
	{
		get
		{
			return LGAMHDFHDHG();
		}
	}

	public static IEnumerable<IYamlTypeConverter> LGAMHDFHDHG()
	{
		return PGMCBEMHNFO;
	}
}
