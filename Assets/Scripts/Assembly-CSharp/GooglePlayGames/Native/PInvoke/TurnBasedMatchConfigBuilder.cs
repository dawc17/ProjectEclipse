using System;
using System.Runtime.InteropServices;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class TurnBasedMatchConfigBuilder : BaseReferenceHolder
	{
		private TurnBasedMatchConfigBuilder(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal TurnBasedMatchConfigBuilder PopulateFromUIResponse(PlayerSelectUIResponse response)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_PopulateFromPlayerSelectUIResponse(SelfPtr(), response.AsPointer());
			return this;
		}

		internal TurnBasedMatchConfigBuilder SetVariant(uint variant)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_SetVariant(SelfPtr(), variant);
			return this;
		}

		internal TurnBasedMatchConfigBuilder AddInvitedPlayer(string playerId)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_AddPlayerToInvite(SelfPtr(), playerId);
			return this;
		}

		internal TurnBasedMatchConfigBuilder SetExclusiveBitMask(ulong bitmask)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_SetExclusiveBitMask(SelfPtr(), bitmask);
			return this;
		}

		internal TurnBasedMatchConfigBuilder SetMinimumAutomatchingPlayers(uint minimum)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_SetMinimumAutomatchingPlayers(SelfPtr(), minimum);
			return this;
		}

		internal TurnBasedMatchConfigBuilder SetMaximumAutomatchingPlayers(uint maximum)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_SetMaximumAutomatchingPlayers(SelfPtr(), maximum);
			return this;
		}

		internal TurnBasedMatchConfig Build()
		{
			return new TurnBasedMatchConfig(GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_Create(SelfPtr()));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_Dispose(selfPointer);
		}

		internal static TurnBasedMatchConfigBuilder Create()
		{
			return new TurnBasedMatchConfigBuilder(GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfigBuilder.TurnBasedMatchConfig_Builder_Construct());
		}
	}
}
