using System;
using Com.Google.Android.Gms.Common.Api;
using Com.Google.Android.Gms.Games;
using Com.Google.Android.Gms.Games.Stats;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidClient : IClientImpl
	{
		private class StatsResultCallback : ResultCallbackProxy<Stats_LoadPlayerStatsResultObject>
		{
			private Action<int, Com.Google.Android.Gms.Games.Stats.PlayerStats> callback;

			public StatsResultCallback(Action<int, Com.Google.Android.Gms.Games.Stats.PlayerStats> callback)
			{
				this.callback = callback;
			}

			public override void OnResult(Stats_LoadPlayerStatsResultObject arg_Result_1)
			{
				callback(arg_Result_1.getStatus().getStatusCode(), arg_Result_1.getPlayerStats());
			}
		}

		internal const string BridgeActivityClass = "com.google.games.bridge.NativeBridgeActivity";

		private const string LaunchBridgeMethod = "launchBridgeIntent";

		private const string LaunchBridgeSignature = "(Landroid/app/Activity;Landroid/content/Intent;)V";

		private TokenClient tokenClient;

		private static AndroidJavaObject invisible;

		public PlatformConfiguration CreatePlatformConfiguration(PlayGamesClientConfiguration clientConfig)
		{
			AndroidPlatformConfiguration androidPlatformConfiguration = AndroidPlatformConfiguration.Create();
			using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
			{
				androidPlatformConfiguration.SetActivity(androidJavaObject.GetRawObject());
				androidPlatformConfiguration.SetOptionalIntentHandlerForUI((IntPtr intent) =>
				{
					IntPtr intentRef = AndroidJNI.NewGlobalRef(intent);
					PlayGamesHelperObject.RunOnGameThread(() =>
					{
						try
						{
							LaunchBridgeIntent(intentRef);
						}
						finally
						{
							AndroidJNI.DeleteGlobalRef(intentRef);
						}
					});
				});
				if (clientConfig.IsHidingPopups)
				{
					androidPlatformConfiguration.SetOptionalViewForPopups(CreateHiddenView(androidJavaObject.GetRawObject()));
				}
			}
			return androidPlatformConfiguration;
		}

		public TokenClient CreateTokenClient(bool reset)
		{
			if (tokenClient == null)
			{
				tokenClient = new AndroidTokenClient();
			}
			else if (reset)
			{
				tokenClient.Signout();
			}
			return tokenClient;
		}

		private IntPtr CreateHiddenView(IntPtr activity)
		{
			if (invisible == null || invisible.GetRawObject() == IntPtr.Zero)
			{
				invisible = new AndroidJavaObject("android.view.View", activity);
				invisible.Call("setVisibility", 4);
				invisible.Call("setClickable", false);
			}
			return invisible.GetRawObject();
		}

		private static void LaunchBridgeIntent(IntPtr bridgedIntent)
		{
			object[] args = new object[2];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.NativeBridgeActivity"))
				{
					using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "launchBridgeIntent", "(Landroid/app/Activity;Landroid/content/Intent;)V");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].l = bridgedIntent;
						AndroidJNI.CallStaticVoidMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
					}
				}
			}
			catch (Exception ex)
			{
				GooglePlayGames.OurUtils.Logger.e("Exception launching bridge intent: " + ex.Message);
				GooglePlayGames.OurUtils.Logger.e(ex.ToString());
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public void Signout()
		{
			if (tokenClient != null)
			{
				tokenClient.Signout();
			}
		}

		public void GetPlayerStats(IntPtr apiClient, Action<CommonStatusCodes, GooglePlayGames.BasicApi.PlayerStats> callback)
		{
			GoogleApiClient arg_GoogleApiClient_ = new GoogleApiClient(apiClient);
			StatsResultCallback resultCallback;
			try
			{
				resultCallback = new StatsResultCallback((int result, Com.Google.Android.Gms.Games.Stats.PlayerStats stats) =>
				{
					Debug.Log("Result for getStats: " + result);
					GooglePlayGames.BasicApi.PlayerStats arg = null;
					if (stats != null)
					{
						arg = new GooglePlayGames.BasicApi.PlayerStats
						{
							AvgSessonLength = stats.getAverageSessionLength(),
							DaysSinceLastPlayed = stats.getDaysSinceLastPlayed(),
							NumberOfPurchases = stats.getNumberOfPurchases(),
							NumberOfSessions = stats.getNumberOfSessions(),
							SessPercentile = stats.getSessionPercentile(),
							SpendPercentile = stats.getSpendPercentile(),
							ChurnProbability = stats.getChurnProbability(),
							SpendProbability = stats.getSpendProbability(),
							HighSpenderProbability = stats.getHighSpenderProbability(),
							TotalSpendNext28Days = stats.getTotalSpendNext28Days()
						};
					}
					callback((CommonStatusCodes)result, arg);
				});
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				callback(CommonStatusCodes.DeveloperError, null);
				return;
			}
			PendingResult<Stats_LoadPlayerStatsResultObject> pendingResult = Games.Stats.loadPlayerStats(arg_GoogleApiClient_, true);
			pendingResult.setResultCallback(resultCallback);
		}

		public void SetGravityForPopups(IntPtr apiClient, Gravity gravity)
		{
			GoogleApiClient arg_GoogleApiClient_ = new GoogleApiClient(apiClient);
			Games.setGravityForPopups(arg_GoogleApiClient_, (int)(gravity | Gravity.CENTER_HORIZONTAL));
		}
	}
}
