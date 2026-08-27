using System;
using System.Collections.Generic;
using Com.Google.Android.Gms.Common.Api;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidTokenClient : TokenClient
	{
		private const string TokenFragmentClass = "com.google.games.bridge.TokenFragment";

		private const string FetchTokenSignature = "(Landroid/app/Activity;ZZZLjava/lang/String;Z[Ljava/lang/String;ZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";

		private const string FetchTokenMethod = "fetchToken";

		private bool requestEmail;

		private bool requestAuthCode;

		private bool requestIdToken;

		private List<string> oauthScopes;

		private string webClientId;

		private bool forceRefresh;

		private bool hidePopups;

		private string accountName;

		private string email;

		private string authCode;

		private string idToken;

		public static AndroidJavaObject GetActivity()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}

		public void SetRequestAuthCode(bool flag, bool forceRefresh)
		{
			requestAuthCode = flag;
			this.forceRefresh = forceRefresh;
		}

		public void SetRequestEmail(bool flag)
		{
			requestEmail = flag;
		}

		public void SetRequestIdToken(bool flag)
		{
			requestIdToken = flag;
		}

		public void SetWebClientId(string webClientId)
		{
			this.webClientId = webClientId;
		}

		public void SetHidePopups(bool flag)
		{
			hidePopups = flag;
		}

		public void SetAccountName(string accountName)
		{
			this.accountName = accountName;
		}

		public void AddOauthScopes(string[] scopes)
		{
			if (scopes != null)
			{
				if (oauthScopes == null)
				{
					oauthScopes = new List<string>();
				}
				oauthScopes.AddRange(scopes);
			}
		}

		public void Signout()
		{
			authCode = null;
			email = null;
			idToken = null;
			PlayGamesHelperObject.RunOnGameThread(() =>
			{
				Debug.Log("Calling Signout in token client");
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.TokenFragment");
				androidJavaClass.CallStatic("signOut", GetActivity());
			});
		}

		public bool NeedsToRun()
		{
			return requestAuthCode || requestEmail || requestIdToken;
		}

		public void FetchTokens(Action<int> callback)
		{
			PlayGamesHelperObject.RunOnGameThread(() =>
			{
				DoFetchToken(callback);
			});
		}

		internal void DoFetchToken(Action<int> callback)
		{
			object[] args = new object[9];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.TokenFragment"))
				{
					using (AndroidJavaObject androidJavaObject = GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "fetchToken", "(Landroid/app/Activity;ZZZLjava/lang/String;Z[Ljava/lang/String;ZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].z = requestAuthCode;
						array[2].z = requestEmail;
						array[3].z = requestIdToken;
						array[4].l = AndroidJNI.NewStringUTF(webClientId);
						array[5].z = forceRefresh;
						array[6].l = AndroidJNIHelper.ConvertToJNIArray(oauthScopes.ToArray());
						array[7].z = hidePopups;
						array[8].l = AndroidJNI.NewStringUTF(accountName);
						IntPtr ptr = AndroidJNI.CallStaticObjectMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
						PendingResult<TokenResult> pendingResult = new PendingResult<TokenResult>(ptr);
						pendingResult.setResultCallback(new TokenResultCallback((int rc, string authCode, string email, string idToken) =>
						{
							this.authCode = authCode;
							this.email = email;
							this.idToken = idToken;
							callback(rc);
						}));
					}
				}
			}
			catch (Exception ex)
			{
				GooglePlayGames.OurUtils.Logger.e("Exception launching token request: " + ex.Message);
				GooglePlayGames.OurUtils.Logger.e(ex.ToString());
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public string GetEmail()
		{
			return email;
		}

		public string GetAuthCode()
		{
			return authCode;
		}

		public string GetIdToken()
		{
			return idToken;
		}
	}
}
