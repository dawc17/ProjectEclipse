using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class PreciseLocale
{
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
		RegionInfo region = GetLocalRegion();
		return region == null ? string.Empty : region.TwoLetterISORegionName;
	}

	public static string BGMAJFGKCEB()
	{
		string language = PBPAPAFAMJB();
		string region = FBPILFMCNGJ();
		return string.IsNullOrEmpty(region) ? language : language + "_" + region;
	}

	public static string PBPAPAFAMJB()
	{
		// Unity already reads the device language. No third-party Java plugin is
		// needed, and unsupported device languages must not block game startup.
		string language = Application.systemLanguage.ToLanguageCode();
		return language == "?" ? "en" : language;
	}

	public static string OHHPBPBCFPL()
	{
		RegionInfo region = GetLocalRegion();
		return region == null ? string.Empty : region.ISOCurrencySymbol;
	}

	public static string HIMMFECDKCI()
	{
		RegionInfo region = GetLocalRegion();
		return region == null ? string.Empty : region.CurrencySymbol;
	}

	private static RegionInfo GetLocalRegion()
	{
		// Some player runtimes only expose an invariant/default managed culture.
		// Use a region only when that culture agrees with Unity's device language;
		// otherwise retain the language-only locale instead of inventing a region.
		CultureInfo culture = CultureInfo.CurrentCulture;
		string language = PBPAPAFAMJB();
		string cultureLanguage = culture.TwoLetterISOLanguageName;
		bool matches = cultureLanguage == language || (language == "no" && (cultureLanguage == "nb" || cultureLanguage == "nn"));
		if (!matches || culture.IsNeutralCulture || string.IsNullOrEmpty(culture.Name)) return null;
		try
		{
			return new RegionInfo(culture.Name);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	public static string ToLanguageCode(this SystemLanguage HBGOBBALPBP)
	{
		if (HBGOBBALPBP == SystemLanguage.ChineseSimplified || HBGOBBALPBP == SystemLanguage.ChineseTraditional) return "zh";
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
