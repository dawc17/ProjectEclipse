using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativePlayer : BaseReferenceHolder
	{
		internal NativePlayer(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Player.Player_Id(SelfPtr(), out_string, out_size));
		}

		internal string Name()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Player.Player_Name(SelfPtr(), out_string, out_size));
		}

		internal string AvatarURL()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Player.Player_AvatarUrl(SelfPtr(), Types.ImageResolution.ICON, out_string, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.Player.Player_Dispose(selfPointer);
		}

		internal GooglePlayGames.BasicApi.Multiplayer.Player AsPlayer()
		{
			return new GooglePlayGames.BasicApi.Multiplayer.Player(Name(), Id(), AvatarURL());
		}
	}
}
