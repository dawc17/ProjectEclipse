using System;
using System.Collections;
using UnityEngine;

public sealed class AssetBundleSample : MonoBehaviour
{
	private const string URL = "http://besthttp.azurewebsites.net/Content/AssetBundle.html";

	private string status = "Waiting for user interaction";

	private AssetBundle cachedBundle;

	private Texture2D texture;

	private bool downloading;

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.Label("Status: " + status);
			if (texture != null)
			{
				GUILayout.Box(texture, GUILayout.MaxHeight(256f));
			}
			if (!downloading && GUILayout.Button("Start Download"))
			{
				KLFHODHCNIP();
				StartCoroutine(MMCCPDJCJMP());
			}
		});
	}

	private void OnDestroy()
	{
		KLFHODHCNIP();
	}

	private IEnumerator MMCCPDJCJMP()
	{
		downloading = true;
		HTTPRequest iPLGNIDJDCF = new HTTPRequest(new Uri("http://besthttp.azurewebsites.net/Content/AssetBundle.html")).Send();
		status = "Download started";
		while (iPLGNIDJDCF.FLBBFDNHJAJ() < CFGBMHKCENK.Finished)
		{
			yield return new WaitForSeconds(0.1f);
			status += ".";
		}
		switch (iPLGNIDJDCF.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (iPLGNIDJDCF.POGDKNCHIBG().AICKPAMONBH())
			{
				status = string.Format("AssetBundle downloaded! Loaded from local cache: {0}", iPLGNIDJDCF.POGDKNCHIBG().LOHDBJLLKEE().ToString());
				AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(iPLGNIDJDCF.POGDKNCHIBG().CHIGLEKCFFN());
				yield return assetBundleCreateRequest;
				yield return StartCoroutine(EKBJIEECGLA(assetBundleCreateRequest.assetBundle));
			}
			else
			{
				status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", iPLGNIDJDCF.POGDKNCHIBG().KNMDPGBPNED(), iPLGNIDJDCF.POGDKNCHIBG().DCKPMHKDLEJ(), iPLGNIDJDCF.POGDKNCHIBG().DPBLPGKOEJB());
				AdvLog.LOPHFKMOPAA(status);
			}
			break;
		case CFGBMHKCENK.Error:
			status = "Request Finished with Error! " + ((iPLGNIDJDCF.IEFGFKFHNMD() == null) ? "No Exception" : (iPLGNIDJDCF.IEFGFKFHNMD().Message + "\n" + iPLGNIDJDCF.IEFGFKFHNMD().StackTrace));
			AdvLog.CCOFFJPPAKC(status);
			break;
		case CFGBMHKCENK.Aborted:
			status = "Request Aborted!";
			AdvLog.LOPHFKMOPAA(status);
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			status = "Connection Timed Out!";
			AdvLog.CCOFFJPPAKC(status);
			break;
		case CFGBMHKCENK.TimedOut:
			status = "Processing the request Timed Out!";
			AdvLog.CCOFFJPPAKC(status);
			break;
		}
		downloading = false;
	}

	private IEnumerator EKBJIEECGLA(AssetBundle bundle)
	{
		if (!(bundle == null))
		{
			cachedBundle = bundle;
			AssetBundleRequest assetBundleRequest = cachedBundle.LoadAssetAsync("9443182_orig", typeof(Texture2D));
			yield return assetBundleRequest;
			texture = assetBundleRequest.asset as Texture2D;
		}
	}

	private void KLFHODHCNIP()
	{
		if (cachedBundle != null)
		{
			cachedBundle.Unload(true);
			cachedBundle = null;
		}
	}
}
