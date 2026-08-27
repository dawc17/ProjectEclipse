using System;
using UnityEngine;

namespace Nekki.SF2.Core.Permissions
{
	public class PermissionsManager : MonoBehaviour
	{
		private static string[] _Permissions;

		private static Action<string[], PermissionRequestResult> DCGFFIDPABB;

		private static PermissionsManager _Current;

		public static PermissionsManager BLOOLFFMKFI
		{
			get
			{
				return get_Current();
			}
		}

		private static string CJEBLHDIJGP()
		{
			return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
			{
				100, 34, 209, 163, 88, 249, 11, 238, 36, 48,
				5, 74, 218, 140, 52, 48, 136, 26, 39, 132,
				201, 167, 192, 69, 47, 144, 181, 222, 10, 234,
				26, 22, 102, 140, 242, 50, 247, 55, 102, 45,
				134, 79, 28, 132, 252, 98, 16, 68, 179, 155,
				41, 18, 45, 59, 42, 201, 236, 88, 57, 187,
				9, 157, 52, 156, 253, 138, 251, 139, 59, 17,
				153, 113, 52, 20, 83, 37, 110, 215, 40, 152,
				39, 70, 252, 75, 178, 192, 224, 7, 81, 54,
				81, 96, 7, 206, 143, 16, 221, 93, 134, 139,
				211, 150, 70, 88, 216, 107, 112, 136, 208, 104,
				22, 238, 48, 94, 26, 81, 136, 99, 128, 125,
				239, 94, 245, 95, 133, 158, 229, 63
			}, false);
		}

		public static PermissionsManager get_Current()
		{
			if (_Current == null)
			{
				_Current = new GameObject("[PermissionsManager]").AddComponent<PermissionsManager>();
				UnityEngine.Object.DontDestroyOnLoad(_Current.gameObject);
			}
			return _Current;
		}

		public bool CheckPermissions(string[] OHELAJEAOLC)
		{
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
			{
				bool flag = false;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
				{
					return androidJavaClass.CallStatic<bool>("CheckPermissions", new object[1] { HIOIONCFPEP(OHELAJEAOLC) });
				}
			}
			return true;
		}

		public bool IsShouldShowRequestPermissionsRationale(string[] OHELAJEAOLC)
		{
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
			{
				bool flag = false;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
				{
					return androidJavaClass.CallStatic<bool>("IsShouldShowRequestPermissionsRationale", new object[1] { HIOIONCFPEP(OHELAJEAOLC) });
				}
			}
			return true;
		}

		public void RequestPermissions(string[] OHELAJEAOLC, Action<string[], PermissionRequestResult> PLFCFPHMKJM, PermissionDialog ECHGBOPIDEP)
		{
			_Permissions = OHELAJEAOLC;
			DCGFFIDPABB = PLFCFPHMKJM;
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
				{
					androidJavaClass.CallStatic("RequestPermissions", HIOIONCFPEP(OHELAJEAOLC), ECHGBOPIDEP.IDLDKFEPJLI(), ECHGBOPIDEP.ILMJJEMPKCN(), ECHGBOPIDEP.EDPLBKPHCMN()[0], ECHGBOPIDEP.EDPLBKPHCMN()[1], ECHGBOPIDEP.EDPLBKPHCMN()[2]);
				}
			}
		}

		public void RequestPermissionsWithExplanation(string[] OHELAJEAOLC, Action<string[], PermissionRequestResult> PLFCFPHMKJM, PermissionDialog CLLNCCOKMHK, PermissionDialog ECHGBOPIDEP)
		{
			_Permissions = OHELAJEAOLC;
			DCGFFIDPABB = PLFCFPHMKJM;
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
				{
					androidJavaClass.CallStatic("RequestPermissionsWithExplanation", HIOIONCFPEP(OHELAJEAOLC), CLLNCCOKMHK.IDLDKFEPJLI(), CLLNCCOKMHK.ILMJJEMPKCN(), CLLNCCOKMHK.EDPLBKPHCMN()[0], ECHGBOPIDEP.IDLDKFEPJLI(), ECHGBOPIDEP.ILMJJEMPKCN(), ECHGBOPIDEP.EDPLBKPHCMN()[0], ECHGBOPIDEP.EDPLBKPHCMN()[1], ECHGBOPIDEP.EDPLBKPHCMN()[2]);
				}
			}
		}

		private static AndroidJavaObject HIOIONCFPEP(string[] AALGCAPHOED)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.reflect.Array");
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("newInstance", new object[2]
			{
				new AndroidJavaClass("java.lang.String"),
				AALGCAPHOED.Length
			});
			int i = 0;
			for (int num = AALGCAPHOED.Length; i < num; i++)
			{
				androidJavaClass.CallStatic("set", androidJavaObject, i, new AndroidJavaObject("java.lang.String", AALGCAPHOED[i]));
			}
			return androidJavaObject;
		}

		private void OnGranted(string p_message)
		{
			Debug.Log("Permissions: " + p_message + " - GRANDED");
			NBBJCMPFHKM(_Permissions, PermissionRequestResult.Granded);
		}

		private void OnDenied(string p_message)
		{
			Debug.Log("Permissions: " + p_message + " - DENIED");
			NBBJCMPFHKM(_Permissions, PermissionRequestResult.Denied);
		}

		private void OnUserSkip(string p_message)
		{
			Debug.Log("Permissions: " + p_message + "- USER_SKIP");
			NBBJCMPFHKM(_Permissions, PermissionRequestResult.UserSkip);
		}

		private static void NBBJCMPFHKM(string[] OHELAJEAOLC, PermissionRequestResult AMKKLMOONEP)
		{
			if (DCGFFIDPABB != null)
			{
				DCGFFIDPABB(OHELAJEAOLC, AMKKLMOONEP);
			}
			DCGFFIDPABB = null;
		}
	}
}
