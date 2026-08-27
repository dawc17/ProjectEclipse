using System;
using System.Collections;
using GooglePlayGames.OurUtils;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesUserProfile : IUserProfile
	{
		private string mDisplayName;

		private string mPlayerId;

		private string mAvatarUrl;

		private volatile bool mImageLoading;

		private Texture2D mImage;

		public string userName
		{
			get
			{
				return mDisplayName;
			}
		}

		public string id
		{
			get
			{
				return mPlayerId;
			}
		}

		public bool isFriend
		{
			get
			{
				return true;
			}
		}

		public UserState state
		{
			get
			{
				return UserState.Online;
			}
		}

		public Texture2D image
		{
			get
			{
				if (!mImageLoading && mImage == null && !string.IsNullOrEmpty(AvatarURL))
				{
					Debug.Log("Starting to load image: " + AvatarURL);
					mImageLoading = true;
					PlayGamesHelperObject.RunCoroutine(LoadImage());
				}
				return mImage;
			}
		}

		public string AvatarURL
		{
			get
			{
				return mAvatarUrl;
			}
		}

		internal PlayGamesUserProfile(string displayName, string playerId, string avatarUrl)
		{
			mDisplayName = displayName;
			mPlayerId = playerId;
			mAvatarUrl = avatarUrl;
			mImageLoading = false;
		}

		protected void ResetIdentity(string displayName, string playerId, string avatarUrl)
		{
			mDisplayName = displayName;
			mPlayerId = playerId;
			if (mAvatarUrl != avatarUrl)
			{
				mImage = null;
				mAvatarUrl = avatarUrl;
			}
			mImageLoading = false;
		}

		internal IEnumerator LoadImage()
		{
			if (!string.IsNullOrEmpty(AvatarURL))
			{
				WWW www = new WWW(AvatarURL);
				while (!www.isDone)
				{
					yield return null;
				}
				if (www.error == null)
				{
					mImage = www.texture;
				}
				else
				{
					mImage = Texture2D.blackTexture;
					Debug.Log("Error downloading image: " + www.error);
				}
				mImageLoading = false;
			}
			else
			{
				Debug.Log("No URL found.");
				mImage = Texture2D.blackTexture;
				mImageLoading = false;
			}
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
			PlayGamesUserProfile playGamesUserProfile = obj as PlayGamesUserProfile;
			if (playGamesUserProfile == null)
			{
				return false;
			}
			return StringComparer.Ordinal.Equals(mPlayerId, playGamesUserProfile.mPlayerId);
		}

		public override int GetHashCode()
		{
			return typeof(PlayGamesUserProfile).GetHashCode() ^ mPlayerId.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[Player: '{0}' (id {1})]", mDisplayName, mPlayerId);
		}
	}
}
