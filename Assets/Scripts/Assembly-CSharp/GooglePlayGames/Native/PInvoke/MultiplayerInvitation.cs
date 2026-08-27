using System;
using System.Runtime.InteropServices;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class MultiplayerInvitation : BaseReferenceHolder
	{
		internal MultiplayerInvitation(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal MultiplayerParticipant Inviter()
		{
			MultiplayerParticipant multiplayerParticipant = new MultiplayerParticipant(GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_InvitingParticipant(SelfPtr()));
			if (!multiplayerParticipant.Valid())
			{
				multiplayerParticipant.Dispose();
				return null;
			}
			return multiplayerParticipant;
		}

		internal uint Variant()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Variant(SelfPtr());
		}

		internal Types.MultiplayerInvitationType Type()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Type(SelfPtr());
		}

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr size) => GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Id(SelfPtr(), out_string, size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Dispose(selfPointer);
		}

		internal uint AutomatchingSlots()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_AutomatchingSlotsAvailable(SelfPtr());
		}

		internal uint ParticipantCount()
		{
			return GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Participants_Length(SelfPtr()).ToUInt32();
		}

		private static Invitation.InvType ToInvType(Types.MultiplayerInvitationType invitationType)
		{
			switch (invitationType)
			{
			case Types.MultiplayerInvitationType.REAL_TIME:
				return Invitation.InvType.RealTime;
			case Types.MultiplayerInvitationType.TURN_BASED:
				return Invitation.InvType.TurnBased;
			default:
				Logger.d("Found unknown invitation type: " + invitationType);
				return Invitation.InvType.Unknown;
			}
		}

		internal Invitation AsInvitation()
		{
			Invitation.InvType invType = ToInvType(Type());
			string invId = Id();
			int variant = (int)Variant();
			Participant inviter;
			using (MultiplayerParticipant multiplayerParticipant = Inviter())
			{
				inviter = ((multiplayerParticipant != null) ? multiplayerParticipant.AsParticipant() : null);
			}
			return new Invitation(invType, invId, inviter, variant);
		}

		internal static MultiplayerInvitation FromPointer(IntPtr selfPointer)
		{
			if (PInvokeUtilities.IsNull(selfPointer))
			{
				return null;
			}
			return new MultiplayerInvitation(selfPointer);
		}
	}
}
