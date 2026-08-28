using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class InternetUtils
{
	private enum JEJBBIEMKGO
	{
		UNDEFINED = 0,
		AVAILABLE = 1,
		REDIRECTED = 2,
		NOT_AVAILABLE = 3
	}

	public class DownloadFileResult
	{
		private WWW DFLDEDJIPEJ;
		private string _offlineUrl;

		public string Url
		{
			get
			{
				return KLMLKCKNNFD();
			}
		}

		public byte[] Data
		{
			get
			{
				return CHIGLEKCFFN();
			}
		}

		public string KJKDFBJFPEH
		{
			get
			{
				return FCJBMLGHAME();
			}
		}

		public bool PLIPMIJDPEM
		{
			get
			{
				return ANGCJOIMCCB();
			}
		}

		public bool CNLODKJLNBG
		{
			get
			{
				return POOFHLGEOIA();
			}
		}

		public bool JKICJCBONHO
		{
			get
			{
				return GCFAACCPOPF();
			}
		}

		public DownloadFileResult(WWW OKFCHMDJIAL)
		{
			DFLDEDJIPEJ = OKFCHMDJIAL;
		}

		public DownloadFileResult(string offlineUrl)
		{
			_offlineUrl = offlineUrl;
		}

		public string KLMLKCKNNFD()
		{
			return DFLDEDJIPEJ == null ? _offlineUrl : DFLDEDJIPEJ.url;
		}

		public byte[] CHIGLEKCFFN()
		{
			return DFLDEDJIPEJ == null ? null : DFLDEDJIPEJ.bytes;
		}

		public string FCJBMLGHAME()
		{
			return DFLDEDJIPEJ == null ? OfflineServices.Unavailable : DFLDEDJIPEJ.error;
		}

		public bool ANGCJOIMCCB()
		{
			return !string.IsNullOrEmpty(FCJBMLGHAME()) || CHIGLEKCFFN() == null;
		}

		public bool POOFHLGEOIA()
		{
			if (!ANGCJOIMCCB())
			{
				return false;
			}
			string text = FCJBMLGHAME().ToLower();
			return text.Contains("404") || text.Contains("not found");
		}

		public bool GCFAACCPOPF()
		{
			if (DFLDEDJIPEJ == null) return false;
			bool result = false;
			try
			{
				AssetBundle assetBundle = DFLDEDJIPEJ.assetBundle;
				if (assetBundle != null)
				{
					result = true;
					assetBundle.Unload(false);
				}
			}
			catch
			{
			}
			return result;
		}
	}

	private static JEJBBIEMKGO MEBAMKIMHCP;

	private const int _RequestTimeout = 5000;

	public static bool JLBPKAFHNNN()
	{
		if (MEBAMKIMHCP != JEJBBIEMKGO.UNDEFINED)
		{
			return MEBAMKIMHCP == JEJBBIEMKGO.AVAILABLE;
		}
		return FCJPEABOFAA();
	}

	public static bool FCJPEABOFAA()
	{
		//Discarded unreachable code: IL_002f, IL_003b, IL_0072, IL_00f3, IL_00fe, IL_0113, IL_0123, IL_0138, IL_0148
		return false;
	}

	public static DownloadFileResult EMANDFAOCNO(string p_url)
	{
		if (!OfflineServices.IsLocalContent(p_url)) return new DownloadFileResult(p_url);
		WWW wWW = new WWW(p_url);
		while (!wWW.isDone)
		{
		}
		return new DownloadFileResult(wWW);
	}

	public static long GetContentLength(string p_url)
	{
		if (!OfflineServices.IsLocalContent(p_url)) return 0L;
		UnityWebRequest unityWebRequest = UnityWebRequest.Head(p_url);
		unityWebRequest.Send();
		while (!unityWebRequest.isDone && Application.internetReachability != NetworkReachability.NotReachable)
		{
		}
		try
		{
			Dictionary<string, string> responseHeaders = unityWebRequest.GetResponseHeaders();
			return long.Parse(responseHeaders["Content-Length"]);
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error("[InternetUtils]: GetContentLength error - " + ex.Message);
			return 0L;
		}
	}
}
