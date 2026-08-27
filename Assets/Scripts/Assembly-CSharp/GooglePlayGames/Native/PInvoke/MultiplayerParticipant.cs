using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class MultiplayerParticipant : BaseReferenceHolder
	{
		private static readonly Dictionary<Types.ParticipantStatus, Participant.ParticipantStatus> StatusConversion = new Dictionary<Types.ParticipantStatus, Participant.ParticipantStatus>
		{
			{
				Types.ParticipantStatus.INVITED,
				Participant.ParticipantStatus.Invited
			},
			{
				Types.ParticipantStatus.JOINED,
				Participant.ParticipantStatus.Joined
			},
			{
				Types.ParticipantStatus.DECLINED,
				Participant.ParticipantStatus.Declined
			},
			{
				Types.ParticipantStatus.LEFT,
				Participant.ParticipantStatus.Left
			},
			{
				Types.ParticipantStatus.NOT_INVITED_YET,
				Participant.ParticipantStatus.NotInvitedYet
			},
			{
				Types.ParticipantStatus.FINISHED,
				Participant.ParticipantStatus.Finished
			},
			{
				Types.ParticipantStatus.UNRESPONSIVE,
				Participant.ParticipantStatus.Unresponsive
			}
		};

		internal MultiplayerParticipant(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal Types.ParticipantStatus Status()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_Status(SelfPtr());
		}

		internal bool IsConnectedToRoom()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_IsConnectedToRoom(SelfPtr()) || Status() == Types.ParticipantStatus.JOINED;
		}

		internal string DisplayName()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr size) => GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_DisplayName(SelfPtr(), out_string, size));
		}

		internal NativePlayer Player()
		{
			if (!GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_HasPlayer(SelfPtr()))
			{
				return null;
			}
			return new NativePlayer(GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_Player(SelfPtr()));
		}

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr size) => GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_Id(SelfPtr(), out_string, size));
		}

		internal bool Valid()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_Valid(SelfPtr());
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.MultiplayerParticipant.MultiplayerParticipant_Dispose(selfPointer);
		}

		internal Participant AsParticipant()
		{
			NativePlayer nativePlayer = Player();
			return new Participant(DisplayName(), Id(), StatusConversion[Status()], (nativePlayer != null) ? nativePlayer.AsPlayer() : null, IsConnectedToRoom());
		}

		internal static MultiplayerParticipant FromPointer(IntPtr pointer)
		{
			if (PInvokeUtilities.IsNull(pointer))
			{
				return null;
			}
			return new MultiplayerParticipant(pointer);
		}

		internal static MultiplayerParticipant AutomatchingSentinel()
		{
			return new MultiplayerParticipant(Sentinels.Sentinels_AutomatchingParticipant());
		}
	}
}
