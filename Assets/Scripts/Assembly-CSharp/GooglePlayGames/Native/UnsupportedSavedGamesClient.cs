using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class UnsupportedSavedGamesClient : ISavedGameClient
	{
		private readonly string mMessage;

		public UnsupportedSavedGamesClient(string message)
		{
			mMessage = Misc.CheckNotNull(message);
		}

		public void OpenWithAutomaticConflictResolution(string filename, DataSource source, ConflictResolutionStrategy resolutionStrategy, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void OpenWithManualConflictResolution(string filename, DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void ReadBinaryData(ISavedGameMetadata metadata, Action<SavedGameRequestStatus, byte[]> completedCallback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI, bool showDeleteSaveUI, Action<SelectUIStatus, ISavedGameMetadata> callback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void CommitUpdate(ISavedGameMetadata metadata, SavedGameMetadataUpdate updateForMetadata, byte[] updatedBinaryData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void FetchAllSavedGames(DataSource source, Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback)
		{
			throw new NotImplementedException(mMessage);
		}

		public void Delete(ISavedGameMetadata metadata)
		{
			throw new NotImplementedException(mMessage);
		}
	}
}
