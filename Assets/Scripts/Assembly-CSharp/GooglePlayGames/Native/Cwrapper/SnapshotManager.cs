using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class SnapshotManager
	{
		internal delegate void FetchAllCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void OpenCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void CommitCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void ReadCallback(IntPtr arg0, IntPtr arg1);

		internal delegate void SnapshotSelectUICallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_FetchAll(HandleRef self, Types.DataSource data_source, FetchAllCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_ShowSelectUIOperation(HandleRef self, [MarshalAs(UnmanagedType.I1)] bool allow_create, [MarshalAs(UnmanagedType.I1)] bool allow_delete, uint max_snapshots, string title, SnapshotSelectUICallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_Read(HandleRef self, IntPtr snapshot_metadata, ReadCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_Commit(HandleRef self, IntPtr snapshot_metadata, IntPtr metadata_change, byte[] data, UIntPtr data_size, CommitCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_Open(HandleRef self, Types.DataSource data_source, string file_name, Types.SnapshotConflictPolicy conflict_policy, OpenCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_ResolveConflict(HandleRef self, IntPtr snapshot_metadata, IntPtr metadata_change, string conflict_id, CommitCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_Delete(HandleRef self, IntPtr snapshot_metadata);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_FetchAllResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus SnapshotManager_FetchAllResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr SnapshotManager_FetchAllResponse_GetData_Length(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_FetchAllResponse_GetData_GetElement(HandleRef self, UIntPtr index);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_OpenResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.SnapshotOpenStatus SnapshotManager_OpenResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_OpenResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr SnapshotManager_OpenResponse_GetConflictId(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_OpenResponse_GetConflictOriginal(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_OpenResponse_GetConflictUnmerged(HandleRef self);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_CommitResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus SnapshotManager_CommitResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_CommitResponse_GetData(HandleRef self);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_ReadResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus SnapshotManager_ReadResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr SnapshotManager_ReadResponse_GetData(HandleRef self, [In][Out] byte[] out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void SnapshotManager_SnapshotSelectUIResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.UIStatus SnapshotManager_SnapshotSelectUIResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr SnapshotManager_SnapshotSelectUIResponse_GetData(HandleRef self);
	}
}
