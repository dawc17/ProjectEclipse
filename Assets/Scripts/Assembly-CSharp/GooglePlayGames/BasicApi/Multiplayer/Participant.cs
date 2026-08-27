using System;

namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class Participant : IComparable<Participant>
	{
		public enum ParticipantStatus
		{
			NotInvitedYet = 0,
			Invited = 1,
			Joined = 2,
			Declined = 3,
			Left = 4,
			Finished = 5,
			Unresponsive = 6,
			Unknown = 7
		}

		private string mDisplayName = string.Empty;

		private string mParticipantId = string.Empty;

		private ParticipantStatus mStatus = ParticipantStatus.Unknown;

		private Player mPlayer;

		private bool mIsConnectedToRoom;

		public string DisplayName
		{
			get
			{
				return mDisplayName;
			}
		}

		public string ParticipantId
		{
			get
			{
				return mParticipantId;
			}
		}

		public ParticipantStatus Status
		{
			get
			{
				return mStatus;
			}
		}

		public Player Player
		{
			get
			{
				return mPlayer;
			}
		}

		public bool IsConnectedToRoom
		{
			get
			{
				return mIsConnectedToRoom;
			}
		}

		public bool IsAutomatch
		{
			get
			{
				return mPlayer == null;
			}
		}

		internal Participant(string displayName, string participantId, ParticipantStatus status, Player player, bool connectedToRoom)
		{
			mDisplayName = displayName;
			mParticipantId = participantId;
			mStatus = status;
			mPlayer = player;
			mIsConnectedToRoom = connectedToRoom;
		}

		public override string ToString()
		{
			return string.Format("[Participant: '{0}' (id {1}), status={2}, player={3}, connected={4}]", mDisplayName, mParticipantId, mStatus.ToString(), (mPlayer != null) ? mPlayer.ToString() : "NULL", mIsConnectedToRoom);
		}

		public int CompareTo(Participant other)
		{
			return mParticipantId.CompareTo(other.mParticipantId);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(Participant))
			{
				return false;
			}
			Participant participant = (Participant)obj;
			return mParticipantId.Equals(participant.mParticipantId);
		}

		public override int GetHashCode()
		{
			return (mParticipantId != null) ? mParticipantId.GetHashCode() : 0;
		}
	}
}
