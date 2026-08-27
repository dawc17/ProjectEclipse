using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class SnapshotManager
	{
		internal class OpenResponse : BaseReferenceHolder
		{
			internal OpenResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.SnapshotOpenStatus)0;
			}

			internal CommonErrorStatus.SnapshotOpenStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_GetStatus(SelfPtr());
			}

			internal string ConflictId()
			{
				if (ResponseStatus() != CommonErrorStatus.SnapshotOpenStatus.VALID_WITH_CONFLICT)
				{
					throw new InvalidOperationException("OpenResponse did not have a conflict");
				}
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_GetConflictId(SelfPtr(), out_string, out_size));
			}

			internal NativeSnapshotMetadata Data()
			{
				if (ResponseStatus() != CommonErrorStatus.SnapshotOpenStatus.VALID)
				{
					throw new InvalidOperationException("OpenResponse had a conflict");
				}
				return new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_GetData(SelfPtr()));
			}

			internal NativeSnapshotMetadata ConflictOriginal()
			{
				if (ResponseStatus() != CommonErrorStatus.SnapshotOpenStatus.VALID_WITH_CONFLICT)
				{
					throw new InvalidOperationException("OpenResponse did not have a conflict");
				}
				return new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_GetConflictOriginal(SelfPtr()));
			}

			internal NativeSnapshotMetadata ConflictUnmerged()
			{
				if (ResponseStatus() != CommonErrorStatus.SnapshotOpenStatus.VALID_WITH_CONFLICT)
				{
					throw new InvalidOperationException("OpenResponse did not have a conflict");
				}
				return new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_GetConflictUnmerged(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_OpenResponse_Dispose(selfPointer);
			}

			internal static OpenResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new OpenResponse(pointer);
			}
		}

		internal class FetchAllResponse : BaseReferenceHolder
		{
			internal FetchAllResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_FetchAllResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.ResponseStatus)0;
			}

			internal IEnumerable<NativeSnapshotMetadata> Data()
			{
				return PInvokeUtilities.ToEnumerable(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_FetchAllResponse_GetData_Length(SelfPtr()), (UIntPtr index) => new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_FetchAllResponse_GetData_GetElement(SelfPtr(), index)));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_FetchAllResponse_Dispose(selfPointer);
			}

			internal static FetchAllResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new FetchAllResponse(pointer);
			}
		}

		internal class CommitResponse : BaseReferenceHolder
		{
			internal CommitResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_CommitResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.ResponseStatus)0;
			}

			internal NativeSnapshotMetadata Data()
			{
				if (!RequestSucceeded())
				{
					throw new InvalidOperationException("Request did not succeed");
				}
				return new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_CommitResponse_GetData(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_CommitResponse_Dispose(selfPointer);
			}

			internal static CommitResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new CommitResponse(pointer);
			}
		}

		internal class ReadResponse : BaseReferenceHolder
		{
			internal ReadResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_CommitResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.ResponseStatus)0;
			}

			internal byte[] Data()
			{
				if (!RequestSucceeded())
				{
					throw new InvalidOperationException("Request did not succeed");
				}
				return PInvokeUtilities.OutParamsToArray((byte[] out_bytes, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_ReadResponse_GetData(SelfPtr(), out_bytes, out_size));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_ReadResponse_Dispose(selfPointer);
			}

			internal static ReadResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new ReadResponse(pointer);
			}
		}

		internal class SnapshotSelectUIResponse : BaseReferenceHolder
		{
			internal SnapshotSelectUIResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.UIStatus RequestStatus()
			{
				return GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_SnapshotSelectUIResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return RequestStatus() > (CommonErrorStatus.UIStatus)0;
			}

			internal NativeSnapshotMetadata Data()
			{
				if (!RequestSucceeded())
				{
					throw new InvalidOperationException("Request did not succeed");
				}
				return new NativeSnapshotMetadata(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_SnapshotSelectUIResponse_GetData(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_SnapshotSelectUIResponse_Dispose(selfPointer);
			}

			internal static SnapshotSelectUIResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new SnapshotSelectUIResponse(pointer);
			}
		}

		private readonly GameServices mServices;

		internal SnapshotManager(GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void FetchAll(Types.DataSource source, Action<FetchAllResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_FetchAll(mServices.AsHandle(), source, InternalFetchAllCallback, Callbacks.ToIntPtr(callback, FetchAllResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.SnapshotManager.FetchAllCallback))]
		internal static void InternalFetchAllCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("SnapshotManager#FetchAllCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void SnapshotSelectUI(bool allowCreate, bool allowDelete, uint maxSnapshots, string uiTitle, Action<SnapshotSelectUIResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_ShowSelectUIOperation(mServices.AsHandle(), allowCreate, allowDelete, maxSnapshots, uiTitle, InternalSnapshotSelectUICallback, Callbacks.ToIntPtr(callback, SnapshotSelectUIResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotSelectUICallback))]
		internal static void InternalSnapshotSelectUICallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("SnapshotManager#SnapshotSelectUICallback", Callbacks.Type.Temporary, response, data);
		}

		internal void Open(string fileName, Types.DataSource source, Types.SnapshotConflictPolicy conflictPolicy, Action<OpenResponse> callback)
		{
			Misc.CheckNotNull(fileName);
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_Open(mServices.AsHandle(), source, fileName, conflictPolicy, InternalOpenCallback, Callbacks.ToIntPtr(callback, OpenResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.SnapshotManager.OpenCallback))]
		internal static void InternalOpenCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("SnapshotManager#OpenCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void Commit(NativeSnapshotMetadata metadata, NativeSnapshotMetadataChange metadataChange, byte[] updatedData, Action<CommitResponse> callback)
		{
			Misc.CheckNotNull(metadata);
			Misc.CheckNotNull(metadataChange);
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_Commit(mServices.AsHandle(), metadata.AsPointer(), metadataChange.AsPointer(), updatedData, new UIntPtr((ulong)updatedData.Length), InternalCommitCallback, Callbacks.ToIntPtr(callback, CommitResponse.FromPointer));
		}

		internal void Resolve(NativeSnapshotMetadata metadata, NativeSnapshotMetadataChange metadataChange, string conflictId, Action<CommitResponse> callback)
		{
			Misc.CheckNotNull(metadata);
			Misc.CheckNotNull(metadataChange);
			Misc.CheckNotNull(conflictId);
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_ResolveConflict(mServices.AsHandle(), metadata.AsPointer(), metadataChange.AsPointer(), conflictId, InternalCommitCallback, Callbacks.ToIntPtr(callback, CommitResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.SnapshotManager.CommitCallback))]
		internal static void InternalCommitCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("SnapshotManager#CommitCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void Delete(NativeSnapshotMetadata metadata)
		{
			Misc.CheckNotNull(metadata);
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_Delete(mServices.AsHandle(), metadata.AsPointer());
		}

		internal void Read(NativeSnapshotMetadata metadata, Action<ReadResponse> callback)
		{
			Misc.CheckNotNull(metadata);
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.SnapshotManager.SnapshotManager_Read(mServices.AsHandle(), metadata.AsPointer(), InternalReadCallback, Callbacks.ToIntPtr(callback, ReadResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.SnapshotManager.ReadCallback))]
		internal static void InternalReadCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("SnapshotManager#ReadCallback", Callbacks.Type.Temporary, response, data);
		}
	}
}
