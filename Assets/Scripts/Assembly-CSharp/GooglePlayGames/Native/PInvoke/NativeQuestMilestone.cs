using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeQuestMilestone : BaseReferenceHolder, IQuestMilestone
	{
		public string Id
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => QuestMilestone.QuestMilestone_Id(SelfPtr(), out_string, out_size));
			}
		}

		public string EventId
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => QuestMilestone.QuestMilestone_EventId(SelfPtr(), out_string, out_size));
			}
		}

		public string QuestId
		{
			get
			{
				return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => QuestMilestone.QuestMilestone_QuestId(SelfPtr(), out_string, out_size));
			}
		}

		public ulong CurrentCount
		{
			get
			{
				return QuestMilestone.QuestMilestone_CurrentCount(SelfPtr());
			}
		}

		public ulong TargetCount
		{
			get
			{
				return QuestMilestone.QuestMilestone_TargetCount(SelfPtr());
			}
		}

		public byte[] CompletionRewardData
		{
			get
			{
				return PInvokeUtilities.OutParamsToArray((byte[] out_bytes, UIntPtr out_size) => QuestMilestone.QuestMilestone_CompletionRewardData(SelfPtr(), out_bytes, out_size));
			}
		}

		public MilestoneState State
		{
			get
			{
				Types.QuestMilestoneState questMilestoneState = QuestMilestone.QuestMilestone_State(SelfPtr());
				switch (questMilestoneState)
				{
				case Types.QuestMilestoneState.CLAIMED:
					return MilestoneState.Claimed;
				case Types.QuestMilestoneState.COMPLETED_NOT_CLAIMED:
					return MilestoneState.CompletedNotClaimed;
				case Types.QuestMilestoneState.NOT_COMPLETED:
					return MilestoneState.NotCompleted;
				case Types.QuestMilestoneState.NOT_STARTED:
					return MilestoneState.NotStarted;
				default:
					throw new InvalidOperationException("Unknown state: " + questMilestoneState);
				}
			}
		}

		internal NativeQuestMilestone(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal bool Valid()
		{
			return QuestMilestone.QuestMilestone_Valid(SelfPtr());
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			QuestMilestone.QuestMilestone_Dispose(selfPointer);
		}

		public override string ToString()
		{
			return string.Format("[NativeQuestMilestone: Id={0}, EventId={1}, QuestId={2}, CurrentCount={3}, TargetCount={4}, State={5}]", Id, EventId, QuestId, CurrentCount, TargetCount, State);
		}

		internal static NativeQuestMilestone FromPointer(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return null;
			}
			return new NativeQuestMilestone(pointer);
		}
	}
}
