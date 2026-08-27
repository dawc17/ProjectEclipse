namespace GooglePlayGames.BasicApi.SavedGame
{
	public interface IConflictResolver
	{
		void ChooseMetadata(ISavedGameMetadata chosenMetadata);
	}
}
