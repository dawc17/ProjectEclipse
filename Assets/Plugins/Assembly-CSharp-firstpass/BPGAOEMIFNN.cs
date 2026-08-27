using System.Runtime.InteropServices;
using UnityEngine;

public class BPGAOEMIFNN
{
	public static string FEBDLBNFNHD
	{
		get
		{
			return OBGMKPLOMJL();
		}
	}

	[DllImport("__Internal")]
	private static extern string _GetID();

	public static string OBGMKPLOMJL()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _GetID();
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.nekki.DeviceUniqueID"))
			{
				return androidJavaClass.CallStatic<string>("GetID", new object[0]);
			}
		}
		return null;
	}
}
