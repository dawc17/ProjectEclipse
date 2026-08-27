using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeSnapshotMetadataChange : BaseReferenceHolder
	{
		internal class Builder : BaseReferenceHolder
		{
			internal Builder()
				: base(SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_Construct())
			{
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_Dispose(selfPointer);
			}

			internal Builder SetDescription(string description)
			{
				SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_SetDescription(SelfPtr(), description);
				return this;
			}

			internal Builder SetPlayedTime(ulong playedTime)
			{
				SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_SetPlayedTime(SelfPtr(), playedTime);
				return this;
			}

			internal Builder SetCoverImageFromPngData(byte[] pngData)
			{
				Misc.CheckNotNull(pngData);
				SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_SetCoverImageFromPngData(SelfPtr(), pngData, new UIntPtr((ulong)pngData.LongLength));
				return this;
			}

			internal NativeSnapshotMetadataChange Build()
			{
				return FromPointer(SnapshotMetadataChangeBuilder.SnapshotMetadataChange_Builder_Create(SelfPtr()));
			}
		}

		internal NativeSnapshotMetadataChange(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			SnapshotMetadataChange.SnapshotMetadataChange_Dispose(selfPointer);
		}

		internal static NativeSnapshotMetadataChange FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeSnapshotMetadataChange(pointer);
		}
	}
}
