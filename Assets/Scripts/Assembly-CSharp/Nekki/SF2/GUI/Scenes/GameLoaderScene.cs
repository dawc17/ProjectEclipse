using Nekki.SF2.Core.Exceptions;
using Nekki.SF2.Core.Network;
using UnityEngine;
using UnityEngine.Video;

namespace Nekki.SF2.GUI.Scenes
{
	public class GameLoaderScene : Scene<GameLoaderScene>
	{
		[SerializeField]
		private VideoClip introClip;

		[SerializeField]
		private GameObject logo;

		private LoadingModule FCFFELHCEEA = new LoadingModule();

		private bool CDCHAOBEMKH;

		private bool HOCNNFGOMHL;

		private bool EOKFGFADIIH;

		[SerializeField]
		private LockScreen _lockScreenPrefab;

		private static bool PIHEPFHMJHJ;

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public VideoClip ENNDIAJLJOH
		{
			get
			{
				return get_IntroClip();
			}
		}

		public GameObject DODLJPFPIOB
		{
			get
			{
				return get_Logo();
			}
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.ModulePreloader;
		}

		public VideoClip get_IntroClip()
		{
			return introClip;
		}

		public GameObject get_Logo()
		{
			return logo;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			Application.runInBackground = true;
			CDCHAOBEMKH = true;
			HOCNNFGOMHL = PIHEPFHMJHJ;
			EOKFGFADIIH = false;
			PIHEPFHMJHJ = true;
			get_Logo().SetActive(false);
			if (_lockScreenPrefab != null && LockScreen.get_Instance() == null)
			{
				Object.Instantiate(_lockScreenPrefab);
			}
		}

		private void Update()
		{
			if (EOKFGFADIIH)
			{
				Clear();
			}
			if (HOCNNFGOMHL)
			{
				Stop();
				HOCNNFGOMHL = false;
			}
			if (CDCHAOBEMKH)
			{
				Start();
				CDCHAOBEMKH = false;
			}
			if (!FCFFELHCEEA.JPDPHACFBFB())
			{
				FCFFELHCEEA.Start();
			}
			if (!FCFFELHCEEA.GCHANFIHDGH())
			{
				if (!GameUtils.LJOJHDOIFLN)
				{
					try
					{
						FCFFELHCEEA.JLPMOKPFECK();
					}
					catch (HackDetectedException ex)
					{
						GameUtils.LJOJHDOIFLN = true;
						ListSF.ELEBLBJKDBI().LCFENEAGDDG(ex.Message);
					}
				}
			}
			else if (FCFFELHCEEA.GCHANFIHDGH() && !FCFFELHCEEA.OOPMAAHJMCE())
			{
				EOKFGFADIIH = true;
			}
		}

		public void Restart()
		{
			HOCNNFGOMHL = true;
			CDCHAOBEMKH = true;
			SoundController.IsBackgroundMusicIntro = false;
			Sound.FAJONFGJBPD();
			Sound.GKMINHHAMAK();
		}

		public void Start()
		{
			Clear();
			FCFFELHCEEA.Stop();
			FCFFELHCEEA.AddModule(new PreInitializationModule());
			if (SystemProperties.FHHPHDIBEFM())
			{
				FCFFELHCEEA.AddModule(new SecurityModule());
			}
			if (SystemProperties.IPJFCBAGMJJ())
			{
				FCFFELHCEEA.AddModule(new PermissionsModule());
			}
			FCFFELHCEEA.AddModule(new AntichitingModule());
			FCFFELHCEEA.AddModule(new AttachFileModule());
			FCFFELHCEEA.AddModule(new InitializationModule());
			FCFFELHCEEA.AddModule(new IntroModule(this));
			FCFFELHCEEA.AddModule(new ParseModule());
			FCFFELHCEEA.AddModule(new LoginModule());
		}

		public static void Stop()
		{
			PIHEPFHMJHJ = false;
			GameUtils.OBJEKOBDMOE = false;
			GameUtils.GCDIGFODNFO = true;
			Module.Reset();
			ListSF.Reset();
			ServerProvider.Reset();
			AnimationData.BCILLFEBJHK();
			AiData.ClearAll();
		}

		private void Clear()
		{
			FCFFELHCEEA.ClearModules(true);
			EOKFGFADIIH = false;
		}
	}
}
