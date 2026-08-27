using System;
using UnityEngine;

namespace Nekki.SF2.Core.Security
{
	public class SecurityManager : MonoBehaviour
	{
		public class AFDFFNCKPHM
		{
			public class UpgradeDataValueExist
			{
				public bool GJOHIPDEMOB;

				public bool JHNMCGGGGGO;

				public bool AIPAICPCNBE;

				public bool AJKBKHIGBJN;

				public bool OCGDKIOJGLP;

				public bool GBLMPEGLJLF;
			}

			public class KEFGAJNFIDP
			{
				public string OGKBLPIJPBK;

				public string FBFKEJEOELM;

				public bool ENNGMHNJDKH;

				public bool GIIGLIJLPPB;

				public bool IJPBIJAIHIO;

				public bool FKINJGAKICM = true;

				public bool ONJIELDFHMP;

				public bool DDDGNLJBHPK
				{
					get
					{
						return NKGJFHCEBLA();
					}
				}

				public bool FOIMMBJEKDE
				{
					get
					{
						return PDJAOJGPFEG();
					}
				}

				public bool NKGJFHCEBLA()
				{
					return !string.IsNullOrEmpty(OGKBLPIJPBK);
				}

				public bool PDJAOJGPFEG()
				{
					return !string.IsNullOrEmpty(FBFKEJEOELM);
				}
			}

			private UpgradeDataValueExist LHNICOECKEF = new UpgradeDataValueExist();

			private KEFGAJNFIDP DPPCHKOMEPL = new KEFGAJNFIDP();

			public UpgradeDataValueExist MEMOBFHEEBL
			{
				get
				{
					return LNEFNLLOGGH();
				}
			}

			public KEFGAJNFIDP KAPMNLLJLKN
			{
				get
				{
					return HLNECMJHMCP();
				}
			}

			public UpgradeDataValueExist LNEFNLLOGGH()
			{
				return LHNICOECKEF;
			}

			public KEFGAJNFIDP HLNECMJHMCP()
			{
				return DPPCHKOMEPL;
			}
		}

		private static Action<LicenseRequestResult> DCGFFIDPABB;

		private static LicenseFailReason CKHHDELAJDO;

		private static GooglePlayLicenseCheckError HHHBPIJDJOM;

		private static SecurityManager _Current;

		public static LicenseFailReason NNKMNAPHAEI
		{
			get
			{
				return get_FailReason();
			}
		}

		public static SecurityManager BLOOLFFMKFI
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
				33, 125, 24, 72, 57, 79, 214, 231, 50, 27,
				200, 211, 131, 178, 105, 19, 113, 104, 45, 23,
				96, 127, 4, 14, 69, 239, 180, 163, 119, 71,
				29, 207, 141, 122, 187, 77, 224, 34, 21, 147,
				130, 48, 172, 7, 57, 215, 201, 150, 48, 11,
				2, 90, 165, 188, 93, 154, 247, 187, 104, 156,
				240, 7, 29, 85, 168, 185, 123, 179, 132, 130,
				188, 138, 54, 199, 59, 34, 251, 207, 134, 8,
				237, 59, 84, 176, 146, 19, 75, 239, 227, 208,
				63, 86, 110, 124, 44, 9, 132, 166, 75, 10,
				172, 251, 130, 182, 83, 114, 133, 39, 58, 190,
				155, 223, 150, 73, 2, 50, 188, 205, 185, 141,
				5, 193, 157, 103, 123, 97, 80, 27
			}, false);
		}

		public static LicenseFailReason get_FailReason()
		{
			return CKHHDELAJDO;
		}

		public static GooglePlayLicenseCheckError get_Error()
		{
			return HHHBPIJDJOM;
		}

		public static SecurityManager get_Current()
		{
			if (_Current == null)
			{
				_Current = new GameObject("[SecurityManager]").AddComponent<SecurityManager>();
				UnityEngine.Object.DontDestroyOnLoad(_Current.gameObject);
			}
			return _Current;
		}

		public void CheckLicense(Action<LicenseRequestResult> PLFCFPHMKJM, AFDFFNCKPHM PCJAKPJMKGN)
		{
			DCGFFIDPABB = PLFCFPHMKJM;
			CKHHDELAJDO = LicenseFailReason.Unknown;
			HHHBPIJDJOM = GooglePlayLicenseCheckError.Unknown;
			FIPIAKMLEOH(PCJAKPJMKGN);
		}

		private static void FIPIAKMLEOH(AFDFFNCKPHM PCJAKPJMKGN)
		{
			NBBJCMPFHKM(LicenseRequestResult.Success);
		}

		private static void OFOENCFPCOK(AFDFFNCKPHM PCJAKPJMKGN)
		{
			try
			{
				LLJPGGEJCPK.JOIGJOFNIKI(PCJAKPJMKGN.LNEFNLLOGGH().GJOHIPDEMOB, PCJAKPJMKGN.LNEFNLLOGGH().JHNMCGGGGGO, PCJAKPJMKGN.LNEFNLLOGGH().AIPAICPCNBE, PCJAKPJMKGN.LNEFNLLOGGH().AJKBKHIGBJN, PCJAKPJMKGN.LNEFNLLOGGH().OCGDKIOJGLP, PCJAKPJMKGN.LNEFNLLOGGH().GJOHIPDEMOB);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				CKHHDELAJDO = LicenseFailReason.LicenseCheckerCorrupted;
				NBBJCMPFHKM(LicenseRequestResult.Failed);
			}
		}

		private static void PEPPKCELDGC(AFDFFNCKPHM PCJAKPJMKGN)
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
				{
					if (PCJAKPJMKGN.HLNECMJHMCP().NKGJFHCEBLA())
					{
						androidJavaClass.CallStatic("EnableGooglePlayLicenseCheck", PCJAKPJMKGN.HLNECMJHMCP().OGKBLPIJPBK);
					}
					if (PCJAKPJMKGN.HLNECMJHMCP().PDJAOJGPFEG())
					{
						androidJavaClass.CallStatic("EnableSigningCertificateCheck", PCJAKPJMKGN.HLNECMJHMCP().FBFKEJEOELM);
					}
					if (PCJAKPJMKGN.HLNECMJHMCP().ENNGMHNJDKH)
					{
						androidJavaClass.CallStatic("EnableInstallIdCheck");
					}
					if (PCJAKPJMKGN.HLNECMJHMCP().GIIGLIJLPPB)
					{
						androidJavaClass.CallStatic("EnableDebugCheck");
					}
					if (PCJAKPJMKGN.HLNECMJHMCP().IJPBIJAIHIO)
					{
						androidJavaClass.CallStatic("EnableEmulatorCheck", PCJAKPJMKGN.HLNECMJHMCP().FKINJGAKICM);
					}
					if (PCJAKPJMKGN.HLNECMJHMCP().ONJIELDFHMP)
					{
						androidJavaClass.CallStatic("EnableUnauthorizedAppsCheck");
					}
					androidJavaClass.CallStatic("StartCheckLicense");
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				CKHHDELAJDO = LicenseFailReason.LicenseCheckerCorrupted;
				NBBJCMPFHKM(LicenseRequestResult.Failed);
			}
		}

		private static void EEKMOKDIJHN(AFDFFNCKPHM PCJAKPJMKGN)
		{
			CKHHDELAJDO = LicenseFailReason.UndefinedPlatform;
			NBBJCMPFHKM(LicenseRequestResult.Failed);
		}

		public string GetSignature()
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
					string result = string.Empty;
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
					{
						result = androidJavaClass.CallStatic<string>("GetSignature", new object[0]);
					}
					return result;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return string.Empty;
				}
			}
			return string.Empty;
		}

		public string GetInstallerId()
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
					string result = string.Empty;
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
					{
						result = androidJavaClass.CallStatic<string>("GetInstallerId", new object[0]);
					}
					return result;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return string.Empty;
				}
			}
			return string.Empty;
		}

		public static GooglePlayLicenseServerResponse GetGooglePlayLicenseServerResponse()
		{
			if (Application.isEditor)
			{
				return null;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
					string mGDHJCDGOLJ = string.Empty;
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP()))
					{
						mGDHJCDGOLJ = androidJavaClass.CallStatic<string>("GetLicenseServerResponse", new object[0]);
					}
					return GooglePlayLicenseServerResponse.Parse(mGDHJCDGOLJ);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return null;
				}
			}
			return null;
		}

		private void OnSuccess(string p_message)
		{
			NBBJCMPFHKM(LicenseRequestResult.Success);
		}

		private void OnFailed(string p_message)
		{
			int result;
			if (!int.TryParse(p_message, out result))
			{
				CKHHDELAJDO = LicenseFailReason.Unknown;
			}
			else
			{
				CKHHDELAJDO = (LicenseFailReason)result;
			}
			NBBJCMPFHKM(LicenseRequestResult.Failed);
		}

		private void OnError(string p_message)
		{
			int result;
			if (!int.TryParse(p_message, out result))
			{
				HHHBPIJDJOM = GooglePlayLicenseCheckError.Unknown;
			}
			else
			{
				HHHBPIJDJOM = (GooglePlayLicenseCheckError)result;
			}
			NBBJCMPFHKM(LicenseRequestResult.Error);
		}

		private static void NBBJCMPFHKM(LicenseRequestResult AMKKLMOONEP)
		{
			switch (AMKKLMOONEP)
			{
			case LicenseRequestResult.Success:
				Debug.Log("[SecurityManager] CheckLicense - SUCCESS");
				break;
			case LicenseRequestResult.Error:
				Debug.LogFormat("[SecurityManager] CheckLicense - ERROR ({0})", HHHBPIJDJOM);
				break;
			default:
				Debug.LogFormat("[SecurityManager] CheckLicense - FAILED ({0})", CKHHDELAJDO);
				break;
			}
			if (DCGFFIDPABB != null)
			{
				DCGFFIDPABB(AMKKLMOONEP);
			}
			DCGFFIDPABB = null;
		}
	}
}
