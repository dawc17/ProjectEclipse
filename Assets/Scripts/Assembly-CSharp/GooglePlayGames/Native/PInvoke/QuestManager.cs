using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class QuestManager
	{
		internal class FetchResponse : BaseReferenceHolder
		{
			internal FetchResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchResponse_GetStatus(SelfPtr());
			}

			internal NativeQuest Data()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				return new NativeQuest(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchResponse_GetData(SelfPtr()));
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.ResponseStatus)0;
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchResponse_Dispose(selfPointer);
			}

			internal static FetchResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new FetchResponse(pointer);
			}
		}

		internal class FetchListResponse : BaseReferenceHolder
		{
			internal FetchListResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchListResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.ResponseStatus)0;
			}

			internal IEnumerable<NativeQuest> Data()
			{
				return PInvokeUtilities.ToEnumerable(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchListResponse_GetData_Length(SelfPtr()), (UIntPtr index) => new NativeQuest(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchListResponse_GetData_GetElement(SelfPtr(), index)));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchListResponse_Dispose(selfPointer);
			}

			internal static FetchListResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new FetchListResponse(pointer);
			}
		}

		internal class ClaimMilestoneResponse : BaseReferenceHolder
		{
			internal ClaimMilestoneResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.QuestClaimMilestoneStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ClaimMilestoneResponse_GetStatus(SelfPtr());
			}

			internal NativeQuest Quest()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				NativeQuest nativeQuest = new NativeQuest(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ClaimMilestoneResponse_GetQuest(SelfPtr()));
				if (nativeQuest.Valid())
				{
					return nativeQuest;
				}
				nativeQuest.Dispose();
				return null;
			}

			internal NativeQuestMilestone ClaimedMilestone()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				NativeQuestMilestone nativeQuestMilestone = new NativeQuestMilestone(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ClaimMilestoneResponse_GetClaimedMilestone(SelfPtr()));
				if (nativeQuestMilestone.Valid())
				{
					return nativeQuestMilestone;
				}
				nativeQuestMilestone.Dispose();
				return null;
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.QuestClaimMilestoneStatus)0;
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ClaimMilestoneResponse_Dispose(selfPointer);
			}

			internal static ClaimMilestoneResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new ClaimMilestoneResponse(pointer);
			}
		}

		internal class AcceptResponse : BaseReferenceHolder
		{
			internal AcceptResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.QuestAcceptStatus ResponseStatus()
			{
				return GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_AcceptResponse_GetStatus(SelfPtr());
			}

			internal NativeQuest AcceptedQuest()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				return new NativeQuest(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_AcceptResponse_GetAcceptedQuest(SelfPtr()));
			}

			internal bool RequestSucceeded()
			{
				return ResponseStatus() > (CommonErrorStatus.QuestAcceptStatus)0;
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_AcceptResponse_Dispose(selfPointer);
			}

			internal static AcceptResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new AcceptResponse(pointer);
			}
		}

		internal class QuestUIResponse : BaseReferenceHolder
		{
			internal QuestUIResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.UIStatus RequestStatus()
			{
				return GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_QuestUIResponse_GetStatus(SelfPtr());
			}

			internal bool RequestSucceeded()
			{
				return RequestStatus() > (CommonErrorStatus.UIStatus)0;
			}

			internal NativeQuest AcceptedQuest()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				NativeQuest nativeQuest = new NativeQuest(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_QuestUIResponse_GetAcceptedQuest(SelfPtr()));
				if (nativeQuest.Valid())
				{
					return nativeQuest;
				}
				nativeQuest.Dispose();
				return null;
			}

			internal NativeQuestMilestone MilestoneToClaim()
			{
				if (!RequestSucceeded())
				{
					return null;
				}
				NativeQuestMilestone nativeQuestMilestone = new NativeQuestMilestone(GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_QuestUIResponse_GetMilestoneToClaim(SelfPtr()));
				if (nativeQuestMilestone.Valid())
				{
					return nativeQuestMilestone;
				}
				nativeQuestMilestone.Dispose();
				return null;
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_QuestUIResponse_Dispose(selfPointer);
			}

			internal static QuestUIResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new QuestUIResponse(pointer);
			}
		}

		private readonly GameServices mServices;

		internal QuestManager(GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void Fetch(Types.DataSource source, string questId, Action<FetchResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_Fetch(mServices.AsHandle(), source, questId, InternalFetchCallback, Callbacks.ToIntPtr(callback, FetchResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.QuestManager.FetchCallback))]
		internal static void InternalFetchCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("QuestManager#FetchCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void FetchList(Types.DataSource source, int fetchFlags, Action<FetchListResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_FetchList(mServices.AsHandle(), source, fetchFlags, InternalFetchListCallback, Callbacks.ToIntPtr(callback, FetchListResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.QuestManager.FetchListCallback))]
		internal static void InternalFetchListCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("QuestManager#FetchListCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void ShowAllQuestUI(Action<QuestUIResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ShowAllUI(mServices.AsHandle(), InternalQuestUICallback, Callbacks.ToIntPtr(callback, QuestUIResponse.FromPointer));
		}

		internal void ShowQuestUI(NativeQuest quest, Action<QuestUIResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ShowUI(mServices.AsHandle(), quest.AsPointer(), InternalQuestUICallback, Callbacks.ToIntPtr(callback, QuestUIResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.QuestManager.QuestUICallback))]
		internal static void InternalQuestUICallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("QuestManager#QuestUICallback", Callbacks.Type.Temporary, response, data);
		}

		internal void Accept(NativeQuest quest, Action<AcceptResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_Accept(mServices.AsHandle(), quest.AsPointer(), InternalAcceptCallback, Callbacks.ToIntPtr(callback, AcceptResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.QuestManager.AcceptCallback))]
		internal static void InternalAcceptCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("QuestManager#AcceptCallback", Callbacks.Type.Temporary, response, data);
		}

		internal void ClaimMilestone(NativeQuestMilestone milestone, Action<ClaimMilestoneResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.QuestManager.QuestManager_ClaimMilestone(mServices.AsHandle(), milestone.AsPointer(), InternalClaimMilestoneCallback, Callbacks.ToIntPtr(callback, ClaimMilestoneResponse.FromPointer));
		}

		[AOT.MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.QuestManager.ClaimMilestoneCallback))]
		internal static void InternalClaimMilestoneCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("QuestManager#ClaimMilestoneCallback", Callbacks.Type.Temporary, response, data);
		}
	}
}
