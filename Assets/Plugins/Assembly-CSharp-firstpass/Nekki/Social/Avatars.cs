using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Social
{
	public class Avatars : MonoBehaviour
	{
		private class ALPOFHNCFOE
		{
			public UserInfo Info;

			public Action<string, Texture> OnDone;
		}

		private static Avatars EDAPJLKMFPC;

		private static readonly Dictionary<string, Texture> _avatars = new Dictionary<string, Texture>();

		private static readonly Dictionary<string, ALPOFHNCFOE> ECDIFOJMMEL = new Dictionary<string, ALPOFHNCFOE>();

		private static string _current;

		private bool _inProcess;

		private static void Init()
		{
			if (!EDAPJLKMFPC)
			{
				EDAPJLKMFPC = new GameObject("_avatar").AddComponent<Avatars>();
				UnityEngine.Object.DontDestroyOnLoad(EDAPJLKMFPC.gameObject);
			}
		}

		public static void GetAvatar(UserInfo EMBBNNBFODN, Action<string, Texture> onDone)
		{
			Init();
			if (_avatars.ContainsKey(EMBBNNBFODN.NDLJPNCIJIP()))
			{
				onDone(EMBBNNBFODN.NDLJPNCIJIP(), _avatars[EMBBNNBFODN.NDLJPNCIJIP()]);
				return;
			}
			ECDIFOJMMEL.Add(EMBBNNBFODN.NDLJPNCIJIP(), new ALPOFHNCFOE
			{
				Info = EMBBNNBFODN,
				OnDone = onDone
			});
		}

		private void Update()
		{
			if (!_inProcess && ECDIFOJMMEL.Count > 0)
			{
				StartCoroutine(Load());
			}
		}

		private IEnumerator Load()
		{
			ECDIFOJMMEL.Clear();
			_inProcess = false;
			yield break;
		}
	}
}
