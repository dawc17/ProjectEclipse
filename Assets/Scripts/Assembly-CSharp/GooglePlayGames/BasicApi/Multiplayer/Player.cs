namespace GooglePlayGames.BasicApi.Multiplayer
{
	public class Player : PlayGamesUserProfile
	{
		internal Player(string displayName, string playerId, string avatarUrl)
			: base(displayName, playerId, avatarUrl)
		{
		}
	}
}
