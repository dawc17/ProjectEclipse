using System;
using System.IO;
using UnityEngine;

public class FGIMNEGFGPM
{
	private static string JEJNOAGFHNB;

	private static AndroidJavaClass OHIKDMAKFKD;

	private static AndroidJavaClass HGLNMLAFGFF;

	private const string LICMLEFNLMB = "mounted";

	private static string BICNNFDLKJK;

	private static int JBKKPPKKPDN;

	static FGIMNEGFGPM()
	{
		JEJNOAGFHNB = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3zkp9e2MZBwiUQmvX29cC5aOOmeGctk8HZMGqx+1xMKrflQQ3t4u5DU6UFjFynF58lbRAt4DCfaKUsAJYLL/zbXiKe4UMXytboJONqdDAG3T72+APBCKfE1IvCxCLFSok7VQpce/u2pTbmGVBMTBOCwbmxi9wNF/5IzBSKMhRTU70WiPiPECMyGc41m5fAEUbu3bPtmMCB+ltur/EphZf5R7lkxUl3Tl66qgfq1IgEIU6pqiZ7Xymh0roc7LLfpw3ossEtr5jHETSwwr7KKNIdh1ixMZRhA/sdLtH9fGe8cvyykUIC20tXGEATQuwzt1+1/APGzl6P1jW/UGk/zPxwIDAQAB";
		if (!FEDMOFPKEEL())
		{
			return;
		}
		HGLNMLAFGFF = new AndroidJavaClass("android.os.Environment");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
		{
			androidJavaClass.SetStatic("BASE64_PUBLIC_KEY", JEJNOAGFHNB);
			androidJavaClass.SetStatic("SALT", new byte[20]
			{
				1, 43, 244, 255, 54, 98, 156, 244, 43, 2,
				248, 252, 9, 5, 150, 148, 223, 45, 255, 84
			});
		}
	}

	public static bool FEDMOFPKEEL()
	{
		if (OHIKDMAKFKD == null)
		{
			OHIKDMAKFKD = new AndroidJavaClass("android.os.Build");
		}
		return OHIKDMAKFKD.GetRawClass() != IntPtr.Zero;
	}

	public static string AFKEFCHKEOP()
	{
		EPDAMDOEGMG();
		if (HGLNMLAFGFF.CallStatic<string>("getExternalStorageState", new object[0]) != "mounted")
		{
			return null;
		}
		using (AndroidJavaObject androidJavaObject = HGLNMLAFGFF.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]))
		{
			string arg = androidJavaObject.Call<string>("getPath", new object[0]);
			return string.Format("{0}/{1}/{2}", arg, "Android/obb", BICNNFDLKJK);
		}
	}

	public static string CKKGPFLGBEJ(string CIEFMEOOPDE)
	{
		EPDAMDOEGMG();
		if (CIEFMEOOPDE == null)
		{
			return null;
		}
		string text = string.Format("{0}/main.{1}.{2}.obb", CIEFMEOOPDE, JBKKPPKKPDN, BICNNFDLKJK);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static string OOMFLCBNPID(string CIEFMEOOPDE)
	{
		EPDAMDOEGMG();
		if (CIEFMEOOPDE == null)
		{
			return null;
		}
		string text = string.Format("{0}/patch.{1}.{2}.obb", CIEFMEOOPDE, JBKKPPKKPDN, BICNNFDLKJK);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static void GPGEEIADAJI()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent", androidJavaObject, new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));
			int num = 65536;
			androidJavaObject2.Call<AndroidJavaObject>("addFlags", new object[1] { num });
			androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				"unityplayer.Activity",
				androidJavaObject.Call<AndroidJavaObject>("getClass", new object[0]).Call<string>("getName", new object[0])
			});
			androidJavaObject.Call("startActivity", androidJavaObject2);
			if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
			{
				Debug.LogError("Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
				AndroidJNI.ExceptionDescribe();
				AndroidJNI.ExceptionClear();
			}
		}
	}

	private static void EPDAMDOEGMG()
	{
		if (JBKKPPKKPDN != 0)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			BICNNFDLKJK = androidJavaObject.Call<string>("getPackageName", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[2] { BICNNFDLKJK, 0 });
			JBKKPPKKPDN = androidJavaObject2.Get<int>("versionCode");
		}
	}
}
