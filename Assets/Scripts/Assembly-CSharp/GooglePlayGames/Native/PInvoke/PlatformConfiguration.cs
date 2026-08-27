using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.PInvoke
{
	internal abstract class PlatformConfiguration : BaseReferenceHolder
	{
		protected PlatformConfiguration(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal HandleRef AsHandle()
		{
			return SelfPtr();
		}
	}
}
