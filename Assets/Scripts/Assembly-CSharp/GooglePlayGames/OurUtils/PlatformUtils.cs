using System;
using UnityEngine;

namespace GooglePlayGames.OurUtils
{
	public static class PlatformUtils
	{
		public static bool Supported
		{
			get
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]);
				AndroidJavaObject androidJavaObject3 = null;
				try
				{
					androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1] { "com.google.android.play.games" });
				}
				catch (Exception)
				{
					return false;
				}
				return androidJavaObject3 != null;
			}
		}
	}
}
