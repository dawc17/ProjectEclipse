namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class Invitation
	{
		public enum InvType
		{
			RealTime = 0,
			TurnBased = 1,
			Unknown = 2
		}

		private InvType mInvitationType;

		private string mInvitationId;

		private Participant mInviter;

		private int mVariant;

		public InvType InvitationType
		{
			get
			{
				return mInvitationType;
			}
		}

		public string InvitationId
		{
			get
			{
				return mInvitationId;
			}
		}

		public Participant Inviter
		{
			get
			{
				return mInviter;
			}
		}

		public int Variant
		{
			get
			{
				return mVariant;
			}
		}

		internal Invitation(InvType invType, string invId, Participant inviter, int variant)
		{
			mInvitationType = invType;
			mInvitationId = invId;
			mInviter = inviter;
			mVariant = variant;
		}

		public override string ToString()
		{
			return string.Format("[Invitation: InvitationType={0}, InvitationId={1}, Inviter={2}, Variant={3}]", InvitationType, InvitationId, Inviter, Variant);
		}
	}
}
