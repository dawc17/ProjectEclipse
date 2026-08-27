using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Nekki.Audio
{
	public class AudioManager : MonoBehaviour
	{
		private const int BMFLCMGODMO = 0;

		private const int FJIPIONMGBM = 1;

		private static AudioManager EDAPJLKMFPC;

		private static readonly Dictionary<int, Chanel> NCAHAPGPHDM = new Dictionary<int, Chanel>();

		private static List<int> _musicChanels = new List<int>();

		private static Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

		private static Dictionary<string, float> _volumesByClips = new Dictionary<string, float>();

		private static AudioSettings HPOJCCHMKOP;

		public static void Init(string ALHKHJOJECK, int[] DOBMHKNFHCA, int[] HLGLHKIOPDE)
		{
			if ((bool)EDAPJLKMFPC)
			{
				AdvLog.LOPHFKMOPAA("AudioManager already exists!");
				return;
			}
			_musicChanels = new List<int>(DOBMHKNFHCA);
			EDAPJLKMFPC = new GameObject("_audioManager").AddComponent<AudioManager>();
			UnityEngine.Object.DontDestroyOnLoad(EDAPJLKMFPC.gameObject);
			Load(ALHKHJOJECK);
			OverallUnitPool.Init(EDAPJLKMFPC);
			HPOJCCHMKOP = new AudioSettings();
		}

		public static void Init(string ALHKHJOJECK, int CEDJBBELDLH, int[] HLGLHKIOPDE)
		{
			Init(ALHKHJOJECK, new int[1] { CEDJBBELDLH }, HLGLHKIOPDE);
		}

		public static void Init(string ALHKHJOJECK, int CEDJBBELDLH, int DCMFMCGMMKG)
		{
			Init(ALHKHJOJECK, new int[1] { CEDJBBELDLH }, new int[1] { DCMFMCGMMKG });
		}

		public static void Init(string ALHKHJOJECK)
		{
			Init(ALHKHJOJECK, new int[1], new int[1] { 1 });
		}

		private static void Load(string ALHKHJOJECK)
		{
			if (Directory.Exists(ALHKHJOJECK))
			{
				string[] directories = Directory.GetDirectories(ALHKHJOJECK);
				for (int i = 0; i < directories.Length; i++)
				{
					Load(directories[i]);
				}
				List<string> list = new List<string>(Directory.GetFiles(ALHKHJOJECK, "*.xml"));
				for (int j = 0; j < list.Count; j++)
				{
					IPLDPCAAEGK(list[j], ALHKHJOJECK);
				}
			}
		}

		private static void IPLDPCAAEGK(string HIOFDADIEME, string ALHKHJOJECK)
		{
			if (!File.Exists(HIOFDADIEME))
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(File.ReadAllText(HIOFDADIEME));
			}
			catch (Exception ex)
			{
				AdvLog.LOPHFKMOPAA("wrong xml: " + ex.Message);
				return;
			}
			XmlElement xmlElement = xmlDocument["Sounds"];
			if (xmlElement == null)
			{
				return;
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.Attributes == null)
				{
					continue;
				}
				string value = childNode.Attributes["Name"].Value;
				string text = childNode.Attributes["File"].Value.Replace("\\", "/");
				float value2 = ((childNode.Attributes["Volume"] != null) ? float.Parse(childNode.Attributes["Volume"].Value) : 1f);
				if (!string.IsNullOrEmpty(text))
				{
					string aKGGCMGELKH = ALHKHJOJECK + "/" + text;
					KDJAABFBHEL(value, aKGGCMGELKH);
					if (_volumesByClips.ContainsKey(value))
					{
						_volumesByClips[value] = value2;
					}
					else
					{
						_volumesByClips.Add(value, value2);
					}
				}
			}
		}

		private static void KDJAABFBHEL(string LGLFOBEIPKB, string AKGGCMGELKH)
		{
			if (!File.Exists(AKGGCMGELKH))
			{
				AdvLog.Log("No" + AKGGCMGELKH);
				return;
			}
			AudioClip audioClip = GeAudioClip(AKGGCMGELKH);
			if ((bool)audioClip)
			{
				if (!_clips.ContainsKey(LGLFOBEIPKB))
				{
					_clips.Add(LGLFOBEIPKB, audioClip);
				}
				else
				{
					_clips[LGLFOBEIPKB] = audioClip;
				}
			}
		}

		private static AudioClip GeAudioClip(string path)
		{
			WWW wWW = new WWW(string.Format("file:///{0}", path));
			while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
			{
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				return wWW.GetAudioClip();
			}
			AdvLog.CCOFFJPPAKC(wWW.error);
			return null;
		}

		public static void AddAudio(AudioClip PIKHEAGHOKB, string name, float JIJAJFEJJHK)
		{
			if (_clips.ContainsKey(name))
			{
				_clips[name] = PIKHEAGHOKB;
			}
			else
			{
				_clips.Add(name, PIKHEAGHOKB);
			}
			if (_volumesByClips.ContainsKey(name))
			{
				_volumesByClips[name] = JIJAJFEJJHK;
			}
			else
			{
				_volumesByClips.Add(name, JIJAJFEJJHK);
			}
		}

		public static void UnloadAudio(string name)
		{
			if (_clips.ContainsKey(name))
			{
				_clips.Remove(name);
			}
			if (_volumesByClips.ContainsKey(name))
			{
				_volumesByClips.Remove(name);
			}
		}

		public static void Play(int ADNDLGKIJJK, string LGLFOBEIPKB, bool KKHJAJFEPPA, bool ENNOPELJKPB, float JIJAJFEJJHK = 1f)
		{
			if (_clips.ContainsKey(LGLFOBEIPKB))
			{
				if (!NCAHAPGPHDM.ContainsKey(ADNDLGKIJJK))
				{
					NCAHAPGPHDM.Add(ADNDLGKIJJK, new Chanel(ADNDLGKIJJK, OIAEAINHKBJ(ADNDLGKIJJK), _clips));
				}
				PlayCommand iPHFFPCPLDP = new PlayCommand(ADNDLGKIJJK, LGLFOBEIPKB, KKHJAJFEPPA, ENNOPELJKPB, JIJAJFEJJHK * _volumesByClips[LGLFOBEIPKB]);
				iPHFFPCPLDP.JJOFEEGNEDM(HPOJCCHMKOP);
				NCAHAPGPHDM[ADNDLGKIJJK].EACCANOGCFL(iPHFFPCPLDP);
			}
		}

		public static void Play(PlayCommand LEKEGLMDAHA)
		{
			if (!EDAPJLKMFPC)
			{
				AdvLog.LOPHFKMOPAA("you must init AudioManager first!");
				return;
			}
			LEKEGLMDAHA.JJOFEEGNEDM(HPOJCCHMKOP);
			if (NCAHAPGPHDM.ContainsKey(LEKEGLMDAHA.OKFNIMIANKK()))
			{
				NCAHAPGPHDM.Add(LEKEGLMDAHA.OKFNIMIANKK(), new Chanel(LEKEGLMDAHA.OKFNIMIANKK(), OIAEAINHKBJ(LEKEGLMDAHA.OKFNIMIANKK()), _clips));
			}
			NCAHAPGPHDM[LEKEGLMDAHA.OKFNIMIANKK()].EACCANOGCFL(LEKEGLMDAHA);
		}

		public static void Mute(int ADNDLGKIJJK)
		{
			if (!NCAHAPGPHDM.ContainsKey(ADNDLGKIJJK))
			{
				NCAHAPGPHDM.Add(ADNDLGKIJJK, new Chanel(ADNDLGKIJJK, OIAEAINHKBJ(ADNDLGKIJJK), _clips));
			}
			NCAHAPGPHDM[ADNDLGKIJJK].LKLAFKJFNIP();
		}

		public static void UnMute(int ADNDLGKIJJK)
		{
			if (!NCAHAPGPHDM.ContainsKey(ADNDLGKIJJK))
			{
				NCAHAPGPHDM.Add(ADNDLGKIJJK, new Chanel(ADNDLGKIJJK, OIAEAINHKBJ(ADNDLGKIJJK), _clips));
			}
			NCAHAPGPHDM[ADNDLGKIJJK].PNNNNJBKONA();
		}

		private static void Pause(bool KCANPMPILKI, int ADNDLGKIJJK, string DPBKBKDCIOI)
		{
			if (NCAHAPGPHDM.ContainsKey(ADNDLGKIJJK))
			{
				NCAHAPGPHDM[ADNDLGKIJJK].Pause(KCANPMPILKI, DPBKBKDCIOI);
			}
		}

		public static void Pause(bool KCANPMPILKI, int ADNDLGKIJJK)
		{
			if (NCAHAPGPHDM.ContainsKey(ADNDLGKIJJK))
			{
				NCAHAPGPHDM[ADNDLGKIJJK].Pause(KCANPMPILKI);
			}
		}

		private static void Pause(bool KCANPMPILKI)
		{
			foreach (Chanel value in NCAHAPGPHDM.Values)
			{
				value.Pause(KCANPMPILKI);
			}
		}

		public static void Stop(int AHCPPDFEDNJ, bool BJIOMMPCLEA = false)
		{
			if (NCAHAPGPHDM.ContainsKey(AHCPPDFEDNJ))
			{
				NCAHAPGPHDM[AHCPPDFEDNJ].IEHPNJOOPCG(BJIOMMPCLEA);
			}
		}

		public static void SetVolume(float JIJAJFEJJHK, int AHCPPDFEDNJ)
		{
			if (!NCAHAPGPHDM.ContainsKey(AHCPPDFEDNJ))
			{
				NCAHAPGPHDM.Add(AHCPPDFEDNJ, new Chanel(AHCPPDFEDNJ, OIAEAINHKBJ(AHCPPDFEDNJ), _clips));
			}
			NCAHAPGPHDM[AHCPPDFEDNJ].set_MasterVolume(JIJAJFEJJHK);
		}

		public static float GetVolume(int AHCPPDFEDNJ)
		{
			if (NCAHAPGPHDM.ContainsKey(AHCPPDFEDNJ))
			{
				return NCAHAPGPHDM[AHCPPDFEDNJ].LFDFKPHKEGJ();
			}
			return 1f;
		}

		internal void Start()
		{
			if ((bool)EDAPJLKMFPC && EDAPJLKMFPC != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private static bool OIAEAINHKBJ(int LIAILCGJBDK)
		{
			return _musicChanels.Contains(LIAILCGJBDK);
		}

		public static bool IsPlaying(int AHCPPDFEDNJ)
		{
			if (NCAHAPGPHDM.ContainsKey(AHCPPDFEDNJ))
			{
				return NCAHAPGPHDM[AHCPPDFEDNJ].EGCDMGAFFEE();
			}
			return false;
		}

		public static bool CheckAudioLoaded(string LGLFOBEIPKB)
		{
			return _clips.ContainsKey(LGLFOBEIPKB);
		}
	}
}
