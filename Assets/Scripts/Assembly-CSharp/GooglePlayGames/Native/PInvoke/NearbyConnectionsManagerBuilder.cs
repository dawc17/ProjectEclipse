using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NearbyConnectionsManagerBuilder : BaseReferenceHolder
	{
		internal NearbyConnectionsManagerBuilder()
			: base(NearbyConnectionsBuilder.NearbyConnections_Builder_Construct())
		{
		}

		internal NearbyConnectionsManagerBuilder SetOnInitializationFinished(Action<NearbyConnectionsStatus.InitializationStatus> callback)
		{
			NearbyConnectionsBuilder.NearbyConnections_Builder_SetOnInitializationFinished(SelfPtr(), InternalOnInitializationFinishedCallback, Callbacks.ToIntPtr(callback));
			return this;
		}

		[AOT.MonoPInvokeCallback(typeof(NearbyConnectionsBuilder.OnInitializationFinishedCallback))]
		private static void InternalOnInitializationFinishedCallback(NearbyConnectionsStatus.InitializationStatus status, IntPtr userData)
		{
			Action<NearbyConnectionsStatus.InitializationStatus> action = Callbacks.IntPtrToPermanentCallback<Action<NearbyConnectionsStatus.InitializationStatus>>(userData);
			if (action == null)
			{
				Logger.w("Callback for Initialization is null. Received status: " + status);
				return;
			}
			try
			{
				action(status);
			}
			catch (Exception ex)
			{
				Logger.e("Error encountered executing NearbyConnectionsManagerBuilder#InternalOnInitializationFinishedCallback. Smothering exception: " + ex);
			}
		}

		internal NearbyConnectionsManagerBuilder SetLocalClientId(long localClientId)
		{
			NearbyConnectionsBuilder.NearbyConnections_Builder_SetClientId(SelfPtr(), localClientId);
			return this;
		}

		internal NearbyConnectionsManagerBuilder SetDefaultLogLevel(Types.LogLevel minLevel)
		{
			NearbyConnectionsBuilder.NearbyConnections_Builder_SetDefaultOnLog(SelfPtr(), minLevel);
			return this;
		}

		internal NearbyConnectionsManager Build(PlatformConfiguration configuration)
		{
			return new NearbyConnectionsManager(NearbyConnectionsBuilder.NearbyConnections_Builder_Create(SelfPtr(), configuration.AsPointer()));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionsBuilder.NearbyConnections_Builder_Dispose(selfPointer);
		}
	}
}
