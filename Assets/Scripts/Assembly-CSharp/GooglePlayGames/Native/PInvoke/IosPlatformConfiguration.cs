using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal sealed class IosPlatformConfiguration : PlatformConfiguration
	{
		private IosPlatformConfiguration(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal void SetClientId(string clientId)
		{
			Misc.CheckNotNull(clientId);
			GooglePlayGames.Native.Cwrapper.IosPlatformConfiguration.IosPlatformConfiguration_SetClientID(SelfPtr(), clientId);
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.IosPlatformConfiguration.IosPlatformConfiguration_Dispose(selfPointer);
		}

		internal static IosPlatformConfiguration Create()
		{
			return new IosPlatformConfiguration(GooglePlayGames.Native.Cwrapper.IosPlatformConfiguration.IosPlatformConfiguration_Construct());
		}
	}
}
