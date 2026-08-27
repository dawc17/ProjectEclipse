using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class InternalSettings
{
	private static readonly InternalSettings Instance;

	public const string FileConfig = "Configs/internalSettings";

	public Dictionary<string, string> ExternalPaths = new Dictionary<string, string>();

	public LocalInternalSettings LocalSettings;

	public JObject ServerSettings;

	public static LocalInternalSettings Local
	{
		get
		{
			return Instance.LocalSettings;
		}
	}

	public static string NoImageTexture
	{
		get
		{
			return Local.NoImageTexture;
		}
	}

	public static bool IsDebug
	{
		get
		{
			return Local.Debug;
		}
	}

	public static string BundlesPath
	{
		get
		{
			return Local.BundlesPath;
		}
	}

	static InternalSettings()
	{
		string value = GlobalLoad.GetLoadText("Configs/internalSettings");
		Instance = JsonConvert.DeserializeObject<InternalSettings>(value);
		Instance.IKFGBJDBNBP(GlobalPath.PathToLoaderFolder);
	}

	public static T MDOHDKAKHGH<T>(string name) where T : class
	{
		JToken jToken = Instance.ServerSettings[name];
		if (jToken != null)
		{
			return jToken.ToObject<T>();
		}
		return (T)null;
	}

	public static string HFGMHHDBHMH(string name)
	{
		if (Instance != null && Instance.ExternalPaths.ContainsKey(name))
		{
			return Instance.ExternalPaths[name];
		}
		return null;
	}

	private void IKFGBJDBNBP(string OGGDCFJAIMH)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> externalPath in ExternalPaths)
		{
			dictionary.Add(externalPath.Key, externalPath.Value.Replace("EXTERNAL_PATH", OGGDCFJAIMH));
		}
		ExternalPaths = dictionary;
		string noImageTexture = LocalSettings.NoImageTexture.Replace("EXTERNAL_PATH", OGGDCFJAIMH);
		LocalSettings.NoImageTexture = noImageTexture;
	}

	public static string GetString()
	{
		return Instance.ToString();
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}
}
