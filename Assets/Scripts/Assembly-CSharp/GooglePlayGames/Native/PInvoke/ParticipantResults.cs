using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class ParticipantResults : BaseReferenceHolder
	{
		internal ParticipantResults(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal bool HasResultsForParticipant(string participantId)
		{
			return GooglePlayGames.Native.Cwrapper.ParticipantResults.ParticipantResults_HasResultsForParticipant(SelfPtr(), participantId);
		}

		internal uint PlacingForParticipant(string participantId)
		{
			return GooglePlayGames.Native.Cwrapper.ParticipantResults.ParticipantResults_PlaceForParticipant(SelfPtr(), participantId);
		}

		internal Types.MatchResult ResultsForParticipant(string participantId)
		{
			return GooglePlayGames.Native.Cwrapper.ParticipantResults.ParticipantResults_MatchResultForParticipant(SelfPtr(), participantId);
		}

		internal ParticipantResults WithResult(string participantId, uint placing, Types.MatchResult result)
		{
			return new ParticipantResults(GooglePlayGames.Native.Cwrapper.ParticipantResults.ParticipantResults_WithResult(SelfPtr(), participantId, placing, result));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.ParticipantResults.ParticipantResults_Dispose(selfPointer);
		}
	}
}
