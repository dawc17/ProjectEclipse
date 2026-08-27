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
			_inProcess = true;
			ALPOFHNCFOE aLPOFHNCFOE = null;
			foreach (KeyValuePair<string, ALPOFHNCFOE> item in ECDIFOJMMEL)
			{
				aLPOFHNCFOE = item.Value;
			}
			if (aLPOFHNCFOE != null)
			{
				WWW wWW = new WWW(aLPOFHNCFOE.Info.CIHLLDHJLON());
				yield return wWW;
				if (string.IsNullOrEmpty(wWW.error))
				{
					if (_avatars.ContainsKey(aLPOFHNCFOE.Info.NDLJPNCIJIP()))
					{
						_avatars[aLPOFHNCFOE.Info.NDLJPNCIJIP()] = wWW.texture;
					}
					else
					{
						_avatars.Add(aLPOFHNCFOE.Info.NDLJPNCIJIP(), wWW.texture);
					}
					aLPOFHNCFOE.OnDone(aLPOFHNCFOE.Info.NDLJPNCIJIP(), _avatars[aLPOFHNCFOE.Info.NDLJPNCIJIP()]);
				}
				ECDIFOJMMEL.Remove(aLPOFHNCFOE.Info.NDLJPNCIJIP());
			}
			_inProcess = false;
		}
	}
}
