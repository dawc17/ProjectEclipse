using System.Collections.Generic;
using UnityEngine;

public static class PreciseLocale
{
	private class OFGHLGPJEKM
	{
		private static AndroidJavaClass OGJIGPGJJDD = new AndroidJavaClass("com.kokosoft.preciselocale.PreciseLocale");

		public static string FBPILFMCNGJ()
		{
			return OGJIGPGJJDD.CallStatic<string>("getRegion", new object[0]);
		}

		public static string PBPAPAFAMJB()
		{
			return OGJIGPGJJDD.CallStatic<string>("getLanguage", new object[0]);
		}

		public static string BGMAJFGKCEB()
		{
			return OGJIGPGJJDD.CallStatic<string>("getLanguageID", new object[0]);
		}

		public static string OHHPBPBCFPL()
		{
			return OGJIGPGJJDD.CallStatic<string>("getCurrencyCode", new object[0]);
		}

		public static string HIMMFECDKCI()
		{
			return OGJIGPGJJDD.CallStatic<string>("getCurrencySymbol", new object[0]);
		}
	}

	private static readonly Dictionary<SystemLanguage, string> NAHDBCDHJHO = new Dictionary<SystemLanguage, string>
	{
		{
			SystemLanguage.Afrikaans,
			"af"
		},
		{
			SystemLanguage.Arabic,
			"ar"
		},
		{
			SystemLanguage.Basque,
			"eu"
		},
		{
			SystemLanguage.Belarusian,
			"be"
		},
		{
			SystemLanguage.Bulgarian,
			"bg"
		},
		{
			SystemLanguage.Catalan,
			"ca"
		},
		{
			SystemLanguage.Chinese,
			"zh"
		},
		{
			SystemLanguage.Czech,
			"cs"
		},
		{
			SystemLanguage.Danish,
			"da"
		},
		{
			SystemLanguage.Dutch,
			"nl"
		},
		{
			SystemLanguage.English,
			"en"
		},
		{
			SystemLanguage.Estonian,
			"et"
		},
		{
			SystemLanguage.Faroese,
			"fo"
		},
		{
			SystemLanguage.Finnish,
			"fi"
		},
		{
			SystemLanguage.French,
			"fr"
		},
		{
			SystemLanguage.German,
			"de"
		},
		{
			SystemLanguage.Greek,
			"el"
		},
		{
			SystemLanguage.Hebrew,
			"he"
		},
		{
			SystemLanguage.Hungarian,
			"hu"
		},
		{
			SystemLanguage.Icelandic,
			"is"
		},
		{
			SystemLanguage.Indonesian,
			"id"
		},
		{
			SystemLanguage.Italian,
			"it"
		},
		{
			SystemLanguage.Japanese,
			"ja"
		},
		{
			SystemLanguage.Korean,
			"ko"
		},
		{
			SystemLanguage.Latvian,
			"lv"
		},
		{
			SystemLanguage.Lithuanian,
			"lt"
		},
		{
			SystemLanguage.Norwegian,
			"no"
		},
		{
			SystemLanguage.Polish,
			"pl"
		},
		{
			SystemLanguage.Portuguese,
			"pt"
		},
		{
			SystemLanguage.Romanian,
			"ro"
		},
		{
			SystemLanguage.Russian,
			"ru"
		},
		{
			SystemLanguage.SerboCroatian,
			"sr"
		},
		{
			SystemLanguage.Slovak,
			"sk"
		},
		{
			SystemLanguage.Slovenian,
			"sl"
		},
		{
			SystemLanguage.Spanish,
			"es"
		},
		{
			SystemLanguage.Swedish,
			"sv"
		},
		{
			SystemLanguage.Thai,
			"th"
		},
		{
			SystemLanguage.Turkish,
			"tr"
		},
		{
			SystemLanguage.Ukrainian,
			"uk"
		},
		{
			SystemLanguage.Vietnamese,
			"vi"
		},
		{
			SystemLanguage.Unknown,
			"?"
		}
	};

	public static string FBPILFMCNGJ()
	{
		return OFGHLGPJEKM.FBPILFMCNGJ();
	}

	public static string BGMAJFGKCEB()
	{
		return OFGHLGPJEKM.BGMAJFGKCEB();
	}

	public static string PBPAPAFAMJB()
	{
		return OFGHLGPJEKM.PBPAPAFAMJB();
	}

	public static string OHHPBPBCFPL()
	{
		return OFGHLGPJEKM.OHHPBPBCFPL();
	}

	public static string HIMMFECDKCI()
	{
		return OFGHLGPJEKM.HIMMFECDKCI();
	}

	public static string ToLanguageCode(this SystemLanguage HBGOBBALPBP)
	{
		string value;
		if (NAHDBCDHJHO.TryGetValue(HBGOBBALPBP, out value))
		{
			return value;
		}
		return NAHDBCDHJHO[SystemLanguage.Unknown];
	}

	public static SystemLanguage FromLanguageCode(this string HBGOBBALPBP)
	{
		SystemLanguage result = SystemLanguage.Unknown;
		foreach (KeyValuePair<SystemLanguage, string> item in NAHDBCDHJHO)
		{
			if (HBGOBBALPBP == item.Value)
			{
				return item.Key;
			}
		}
		return result;
	}
}
