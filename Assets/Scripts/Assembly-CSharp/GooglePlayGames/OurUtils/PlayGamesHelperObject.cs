using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GooglePlayGames.OurUtils
{
	public class PlayGamesHelperObject : MonoBehaviour
	{
		private static PlayGamesHelperObject instance = null;

		private static bool sIsDummy = false;

		private static List<Action> sQueue = new List<Action>();

		private List<Action> localQueue = new List<Action>();

		private static volatile bool sQueueEmpty = true;

		private static List<Action<bool>> sPauseCallbackList = new List<Action<bool>>();

		private static List<Action<bool>> sFocusCallbackList = new List<Action<bool>>();

		public static void CreateObject()
		{
			if (!(instance != null))
			{
				if (Application.isPlaying)
				{
					GameObject gameObject = new GameObject("PlayGames_QueueRunner");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					instance = gameObject.AddComponent<PlayGamesHelperObject>();
				}
				else
				{
					instance = new PlayGamesHelperObject();
					sIsDummy = true;
				}
			}
		}

		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void OnDisable()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public static void RunCoroutine(IEnumerator action)
		{
			if (instance != null)
			{
				RunOnGameThread(() =>
				{
					instance.StartCoroutine(action);
				});
			}
		}

		public static void RunOnGameThread(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (sIsDummy)
			{
				return;
			}
			lock (sQueue)
			{
				sQueue.Add(action);
				sQueueEmpty = false;
			}
		}

		public void Update()
		{
			if (!sIsDummy && !sQueueEmpty)
			{
				localQueue.Clear();
				lock (sQueue)
				{
					localQueue.AddRange(sQueue);
					sQueue.Clear();
					sQueueEmpty = true;
				}
				for (int i = 0; i < localQueue.Count; i++)
				{
					localQueue[i]();
				}
			}
		}

		public void OnApplicationFocus(bool focused)
		{
			foreach (Action<bool> sFocusCallback in sFocusCallbackList)
			{
				try
				{
					sFocusCallback(focused);
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnApplicationFocus:" + ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		public void OnApplicationPause(bool paused)
		{
			foreach (Action<bool> sPauseCallback in sPauseCallbackList)
			{
				try
				{
					sPauseCallback(paused);
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in OnApplicationPause:" + ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		public static void AddFocusCallback(Action<bool> callback)
		{
			if (!sFocusCallbackList.Contains(callback))
			{
				sFocusCallbackList.Add(callback);
			}
		}

		public static bool RemoveFocusCallback(Action<bool> callback)
		{
			return sFocusCallbackList.Remove(callback);
		}

		public static void AddPauseCallback(Action<bool> callback)
		{
			if (!sPauseCallbackList.Contains(callback))
			{
				sPauseCallbackList.Add(callback);
			}
		}

		public static bool RemovePauseCallback(Action<bool> callback)
		{
			return sPauseCallbackList.Remove(callback);
		}
	}
}
