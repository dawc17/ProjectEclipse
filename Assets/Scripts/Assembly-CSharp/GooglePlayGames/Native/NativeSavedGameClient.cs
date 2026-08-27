using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class NativeSavedGameClient : ISavedGameClient
	{
		private class NativeConflictResolver : IConflictResolver
		{
			private readonly GooglePlayGames.Native.PInvoke.SnapshotManager mManager;

			private readonly string mConflictId;

			private readonly NativeSnapshotMetadata mOriginal;

			private readonly NativeSnapshotMetadata mUnmerged;

			private readonly Action<SavedGameRequestStatus, ISavedGameMetadata> mCompleteCallback;

			private readonly Action mRetryFileOpen;

			internal NativeConflictResolver(GooglePlayGames.Native.PInvoke.SnapshotManager manager, string conflictId, NativeSnapshotMetadata original, NativeSnapshotMetadata unmerged, Action<SavedGameRequestStatus, ISavedGameMetadata> completeCallback, Action retryOpen)
			{
				mManager = Misc.CheckNotNull(manager);
				mConflictId = Misc.CheckNotNull(conflictId);
				mOriginal = Misc.CheckNotNull(original);
				mUnmerged = Misc.CheckNotNull(unmerged);
				mCompleteCallback = Misc.CheckNotNull(completeCallback);
				mRetryFileOpen = Misc.CheckNotNull(retryOpen);
			}

			public void ChooseMetadata(ISavedGameMetadata chosenMetadata)
			{
				NativeSnapshotMetadata nativeSnapshotMetadata = chosenMetadata as NativeSnapshotMetadata;
				if (nativeSnapshotMetadata != mOriginal && nativeSnapshotMetadata != mUnmerged)
				{
					Logger.e("Caller attempted to choose a version of the metadata that was not part of the conflict");
					mCompleteCallback(SavedGameRequestStatus.BadInputError, null);
					return;
				}
				mManager.Resolve(nativeSnapshotMetadata, new NativeSnapshotMetadataChange.Builder().Build(), mConflictId, (GooglePlayGames.Native.PInvoke.SnapshotManager.CommitResponse response) =>
				{
					if (!response.RequestSucceeded())
					{
						mCompleteCallback(AsRequestStatus(response.ResponseStatus()), null);
					}
					else
					{
						mRetryFileOpen();
					}
				});
			}
		}

		private class Prefetcher
		{
			private readonly object mLock = new object();

			private bool mOriginalDataFetched;

			private byte[] mOriginalData;

			private bool mUnmergedDataFetched;

			private byte[] mUnmergedData;

			private Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback;

			private readonly Action<byte[], byte[]> mDataFetchedCallback;

			internal Prefetcher(Action<byte[], byte[]> dataFetchedCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
			{
				mDataFetchedCallback = Misc.CheckNotNull(dataFetchedCallback);
				this.completedCallback = Misc.CheckNotNull(completedCallback);
			}

			internal void OnOriginalDataRead(GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse readResponse)
			{
				lock (mLock)
				{
					if (!readResponse.RequestSucceeded())
					{
						Logger.e("Encountered error while prefetching original data.");
						completedCallback(AsRequestStatus(readResponse.ResponseStatus()), null);
						completedCallback = delegate
						{
						};
					}
					else
					{
						Logger.d("Successfully fetched original data");
						mOriginalDataFetched = true;
						mOriginalData = readResponse.Data();
						MaybeProceed();
					}
				}
			}

			internal void OnUnmergedDataRead(GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse readResponse)
			{
				lock (mLock)
				{
					if (!readResponse.RequestSucceeded())
					{
						Logger.e("Encountered error while prefetching unmerged data.");
						completedCallback(AsRequestStatus(readResponse.ResponseStatus()), null);
						completedCallback = delegate
						{
						};
					}
					else
					{
						Logger.d("Successfully fetched unmerged data");
						mUnmergedDataFetched = true;
						mUnmergedData = readResponse.Data();
						MaybeProceed();
					}
				}
			}

			private void MaybeProceed()
			{
				if (mOriginalDataFetched && mUnmergedDataFetched)
				{
					Logger.d("Fetched data for original and unmerged, proceeding");
					mDataFetchedCallback(mOriginalData, mUnmergedData);
					return;
				}
				Logger.d("Not all data fetched - original:" + mOriginalDataFetched + " unmerged:" + mUnmergedDataFetched);
			}
		}

		private static readonly Regex ValidFilenameRegex = new Regex("\\A[a-zA-Z0-9-._~]{1,100}\\Z");

		private readonly GooglePlayGames.Native.PInvoke.SnapshotManager mSnapshotManager;

		internal NativeSavedGameClient(GooglePlayGames.Native.PInvoke.SnapshotManager manager)
		{
			mSnapshotManager = Misc.CheckNotNull(manager);
		}

		public void OpenWithAutomaticConflictResolution(string filename, DataSource source, ConflictResolutionStrategy resolutionStrategy, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			Misc.CheckNotNull(filename);
			Misc.CheckNotNull(callback);
			callback = ToOnGameThread(callback);
			if (!IsValidFilename(filename))
			{
				Logger.e("Received invalid filename: " + filename);
				callback(SavedGameRequestStatus.BadInputError, null);
				return;
			}
			OpenWithManualConflictResolution(filename, source, false, (IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData) =>
			{
				switch (resolutionStrategy)
				{
				case ConflictResolutionStrategy.UseOriginal:
					resolver.ChooseMetadata(original);
					break;
				case ConflictResolutionStrategy.UseUnmerged:
					resolver.ChooseMetadata(unmerged);
					break;
				case ConflictResolutionStrategy.UseLongestPlaytime:
					if (original.TotalTimePlayed >= unmerged.TotalTimePlayed)
					{
						resolver.ChooseMetadata(original);
					}
					else
					{
						resolver.ChooseMetadata(unmerged);
					}
					break;
				default:
					Logger.e("Unhandled strategy " + resolutionStrategy);
					callback(SavedGameRequestStatus.InternalError, null);
					break;
				}
			}, callback);
		}

		private ConflictCallback ToOnGameThread(ConflictCallback conflictCallback)
		{
			return (IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData) =>
			{
				Logger.d("Invoking conflict callback");
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					conflictCallback(resolver, original, originalData, unmerged, unmergedData);
				});
			};
		}

		public void OpenWithManualConflictResolution(string filename, DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
		{
			Misc.CheckNotNull(filename);
			Misc.CheckNotNull(conflictCallback);
			Misc.CheckNotNull(completedCallback);
			conflictCallback = ToOnGameThread(conflictCallback);
			completedCallback = ToOnGameThread(completedCallback);
			if (!IsValidFilename(filename))
			{
				Logger.e("Received invalid filename: " + filename);
				completedCallback(SavedGameRequestStatus.BadInputError, null);
			}
			else
			{
				InternalManualOpen(filename, source, prefetchDataOnConflict, conflictCallback, completedCallback);
			}
		}

		private void InternalManualOpen(string filename, DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
		{
			mSnapshotManager.Open(filename, AsDataSource(source), Types.SnapshotConflictPolicy.MANUAL, (GooglePlayGames.Native.PInvoke.SnapshotManager.OpenResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					completedCallback(AsRequestStatus(response.ResponseStatus()), null);
				}
				else if (response.ResponseStatus() == CommonErrorStatus.SnapshotOpenStatus.VALID)
				{
					completedCallback(SavedGameRequestStatus.Success, response.Data());
				}
				else if (response.ResponseStatus() == CommonErrorStatus.SnapshotOpenStatus.VALID_WITH_CONFLICT)
				{
					NativeSnapshotMetadata original = response.ConflictOriginal();
					NativeSnapshotMetadata unmerged = response.ConflictUnmerged();
					NativeConflictResolver resolver = new NativeConflictResolver(mSnapshotManager, response.ConflictId(), original, unmerged, completedCallback, () =>
					{
						InternalManualOpen(filename, source, prefetchDataOnConflict, conflictCallback, completedCallback);
					});
					if (!prefetchDataOnConflict)
					{
						conflictCallback(resolver, original, null, unmerged, null);
					}
					else
					{
						Prefetcher prefetcher = new Prefetcher((byte[] originalData, byte[] unmergedData) =>
						{
							conflictCallback(resolver, original, originalData, unmerged, unmergedData);
						}, completedCallback);
						mSnapshotManager.Read(original, prefetcher.OnOriginalDataRead);
						mSnapshotManager.Read(unmerged, prefetcher.OnUnmergedDataRead);
					}
				}
				else
				{
					Logger.e("Unhandled response status");
					completedCallback(SavedGameRequestStatus.InternalError, null);
				}
			});
		}

		public void ReadBinaryData(ISavedGameMetadata metadata, Action<SavedGameRequestStatus, byte[]> completedCallback)
		{
			Misc.CheckNotNull(metadata);
			Misc.CheckNotNull(completedCallback);
			completedCallback = ToOnGameThread(completedCallback);
			NativeSnapshotMetadata nativeSnapshotMetadata = metadata as NativeSnapshotMetadata;
			if (nativeSnapshotMetadata == null)
			{
				Logger.e("Encountered metadata that was not generated by this ISavedGameClient");
				completedCallback(SavedGameRequestStatus.BadInputError, null);
				return;
			}
			if (!nativeSnapshotMetadata.IsOpen)
			{
				Logger.e("This method requires an open ISavedGameMetadata.");
				completedCallback(SavedGameRequestStatus.BadInputError, null);
				return;
			}
			mSnapshotManager.Read(nativeSnapshotMetadata, (GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					completedCallback(AsRequestStatus(response.ResponseStatus()), null);
				}
				else
				{
					completedCallback(SavedGameRequestStatus.Success, response.Data());
				}
			});
		}

		public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI, bool showDeleteSaveUI, Action<SelectUIStatus, ISavedGameMetadata> callback)
		{
			Misc.CheckNotNull(uiTitle);
			Misc.CheckNotNull(callback);
			callback = ToOnGameThread(callback);
			if (maxDisplayedSavedGames == 0)
			{
				Logger.e("maxDisplayedSavedGames must be greater than 0");
				callback(SelectUIStatus.BadInputError, null);
			}
			else
			{
				mSnapshotManager.SnapshotSelectUI(showCreateSaveUI, showDeleteSaveUI, maxDisplayedSavedGames, uiTitle, (GooglePlayGames.Native.PInvoke.SnapshotManager.SnapshotSelectUIResponse response) =>
				{
					callback(AsUIStatus(response.RequestStatus()), (!response.RequestSucceeded()) ? null : response.Data());
				});
			}
		}

		public void CommitUpdate(ISavedGameMetadata metadata, SavedGameMetadataUpdate updateForMetadata, byte[] updatedBinaryData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			Misc.CheckNotNull(metadata);
			Misc.CheckNotNull(updatedBinaryData);
			Misc.CheckNotNull(callback);
			callback = ToOnGameThread(callback);
			NativeSnapshotMetadata nativeSnapshotMetadata = metadata as NativeSnapshotMetadata;
			if (nativeSnapshotMetadata == null)
			{
				Logger.e("Encountered metadata that was not generated by this ISavedGameClient");
				callback(SavedGameRequestStatus.BadInputError, null);
				return;
			}
			if (!nativeSnapshotMetadata.IsOpen)
			{
				Logger.e("This method requires an open ISavedGameMetadata.");
				callback(SavedGameRequestStatus.BadInputError, null);
				return;
			}
			mSnapshotManager.Commit(nativeSnapshotMetadata, AsMetadataChange(updateForMetadata), updatedBinaryData, (GooglePlayGames.Native.PInvoke.SnapshotManager.CommitResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					callback(AsRequestStatus(response.ResponseStatus()), null);
				}
				else
				{
					callback(SavedGameRequestStatus.Success, response.Data());
				}
			});
		}

		public void FetchAllSavedGames(DataSource source, Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback)
		{
			Misc.CheckNotNull(callback);
			callback = ToOnGameThread(callback);
			mSnapshotManager.FetchAll(AsDataSource(source), (GooglePlayGames.Native.PInvoke.SnapshotManager.FetchAllResponse response) =>
			{
				if (!response.RequestSucceeded())
				{
					callback(AsRequestStatus(response.ResponseStatus()), new List<ISavedGameMetadata>());
				}
				else
				{
					callback(SavedGameRequestStatus.Success, response.Data().Cast<ISavedGameMetadata>().ToList());
				}
			});
		}

		public void Delete(ISavedGameMetadata metadata)
		{
			Misc.CheckNotNull(metadata);
			mSnapshotManager.Delete((NativeSnapshotMetadata)metadata);
		}

		internal static bool IsValidFilename(string filename)
		{
			if (filename == null)
			{
				return false;
			}
			return ValidFilenameRegex.IsMatch(filename);
		}

		private static Types.SnapshotConflictPolicy AsConflictPolicy(ConflictResolutionStrategy strategy)
		{
			switch (strategy)
			{
			case ConflictResolutionStrategy.UseLongestPlaytime:
				return Types.SnapshotConflictPolicy.LONGEST_PLAYTIME;
			case ConflictResolutionStrategy.UseOriginal:
				return Types.SnapshotConflictPolicy.LAST_KNOWN_GOOD;
			case ConflictResolutionStrategy.UseUnmerged:
				return Types.SnapshotConflictPolicy.MOST_RECENTLY_MODIFIED;
			default:
				throw new InvalidOperationException("Found unhandled strategy: " + strategy);
			}
		}

		private static SavedGameRequestStatus AsRequestStatus(CommonErrorStatus.SnapshotOpenStatus status)
		{
			switch (status)
			{
			case CommonErrorStatus.SnapshotOpenStatus.VALID:
				return SavedGameRequestStatus.Success;
			case CommonErrorStatus.SnapshotOpenStatus.ERROR_NOT_AUTHORIZED:
				return SavedGameRequestStatus.AuthenticationError;
			case CommonErrorStatus.SnapshotOpenStatus.ERROR_TIMEOUT:
				return SavedGameRequestStatus.TimeoutError;
			default:
				Logger.e("Encountered unknown status: " + status);
				return SavedGameRequestStatus.InternalError;
			}
		}

		private static Types.DataSource AsDataSource(DataSource source)
		{
			switch (source)
			{
			case DataSource.ReadCacheOrNetwork:
				return Types.DataSource.CACHE_OR_NETWORK;
			case DataSource.ReadNetworkOnly:
				return Types.DataSource.NETWORK_ONLY;
			default:
				throw new InvalidOperationException("Found unhandled DataSource: " + source);
			}
		}

		private static SavedGameRequestStatus AsRequestStatus(CommonErrorStatus.ResponseStatus status)
		{
			switch (status)
			{
			case CommonErrorStatus.ResponseStatus.ERROR_INTERNAL:
				return SavedGameRequestStatus.InternalError;
			case CommonErrorStatus.ResponseStatus.ERROR_LICENSE_CHECK_FAILED:
				Logger.e("User attempted to use the game without a valid license.");
				return SavedGameRequestStatus.AuthenticationError;
			case CommonErrorStatus.ResponseStatus.ERROR_NOT_AUTHORIZED:
				Logger.e("User was not authorized (they were probably not logged in).");
				return SavedGameRequestStatus.AuthenticationError;
			case CommonErrorStatus.ResponseStatus.ERROR_TIMEOUT:
				return SavedGameRequestStatus.TimeoutError;
			case CommonErrorStatus.ResponseStatus.VALID:
			case CommonErrorStatus.ResponseStatus.VALID_BUT_STALE:
				return SavedGameRequestStatus.Success;
			default:
				Logger.e("Unknown status: " + status);
				return SavedGameRequestStatus.InternalError;
			}
		}

		private static SelectUIStatus AsUIStatus(CommonErrorStatus.UIStatus uiStatus)
		{
			switch (uiStatus)
			{
			case CommonErrorStatus.UIStatus.VALID:
				return SelectUIStatus.SavedGameSelected;
			case CommonErrorStatus.UIStatus.ERROR_CANCELED:
				return SelectUIStatus.UserClosedUI;
			case CommonErrorStatus.UIStatus.ERROR_INTERNAL:
				return SelectUIStatus.InternalError;
			case CommonErrorStatus.UIStatus.ERROR_NOT_AUTHORIZED:
				return SelectUIStatus.AuthenticationError;
			case CommonErrorStatus.UIStatus.ERROR_TIMEOUT:
				return SelectUIStatus.TimeoutError;
			default:
				Logger.e("Encountered unknown UI Status: " + uiStatus);
				return SelectUIStatus.InternalError;
			}
		}

		private static NativeSnapshotMetadataChange AsMetadataChange(SavedGameMetadataUpdate update)
		{
			NativeSnapshotMetadataChange.Builder builder = new NativeSnapshotMetadataChange.Builder();
			if (update.IsCoverImageUpdated)
			{
				builder.SetCoverImageFromPngData(update.UpdatedPngCoverImage);
			}
			if (update.IsDescriptionUpdated)
			{
				builder.SetDescription(update.UpdatedDescription);
			}
			if (update.IsPlayedTimeUpdated)
			{
				builder.SetPlayedTime((ulong)update.UpdatedPlayedTime.Value.TotalMilliseconds);
			}
			return builder.Build();
		}

		private static Action<T1, T2> ToOnGameThread<T1, T2>(Action<T1, T2> toConvert)
		{
			return (T1 val1, T2 val2) =>
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					toConvert(val1, val2);
				});
			};
		}
	}
}
