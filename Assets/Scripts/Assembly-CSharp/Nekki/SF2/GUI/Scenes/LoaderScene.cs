using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Scenes
{
	public class LoaderScene : Scene<LoaderScene>
	{
		private static ScreenType JOLOLCEHFBO = ScreenType.ModuleNone;

		private static ScreenType LMGJJNACLFG = ScreenType.ModuleNone;

		[SerializeField]
		private GameObject _LoaderType1;

		[SerializeField]
		private GameObject _LoaderType2;

		private Image OJCNKNALKEO;

		private Text KNIMLOGHNFN;

		private bool PJCDILABGPP;

		public static ScreenType GGJHDCMLGOL
		{
			get
			{
				return get_PrevScene();
			}
			set
			{
				set_PrevScene(value);
			}
		}

		public static ScreenType HENJLOPMEHC
		{
			set
			{
				set_NextScene(value);
			}
		}

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public static ScreenType get_PrevScene()
		{
			return JOLOLCEHFBO;
		}

		public static void set_PrevScene(ScreenType value)
		{
			JOLOLCEHFBO = value;
		}

		public static void set_NextScene(ScreenType value)
		{
			LMGJJNACLFG = value;
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.Loader;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			if (LMGJJNACLFG != ScreenType.ModulePreloader)
			{
			}
			AtlasCache.Clear();
			// The Single-mode LoadSceneAsync below already unloads unused assets.
			// An extra sweep here scans the same objects twice, followed by a forced
			// full collection, producing long stalls even when nothing is reclaimed.
			if (LMGJJNACLFG == ScreenType.ModuleFight && Eclipse.Multiplayer.LocalVersusMenu.VersusSplashVisible)
			{
				// Multiplayer's persistent VS introduction owns this transition.
				_LoaderType1.SetActive(false);
				_LoaderType2.SetActive(false);
			}
			else if (get_PrevScene() == ScreenType.ModulePreloader)
			{
				_LoaderType1.SetActive(true);
				_LoaderType2.SetActive(false);
			}
			else
			{
				_LoaderType1.SetActive(false);
				_LoaderType2.SetActive(true);
				Eclipse.UI.LoaderArt.ApplyMenuSplash(_LoaderType2, get_PrevScene(), LMGJJNACLFG);
			}
			StartCoroutine(OBHAPHKNGFE());
		}

		private IEnumerator OBHAPHKNGFE()
		{
			yield return SceneManager.LoadSceneAsync((int)LMGJJNACLFG);
		}

		public override void UpdateScene(object data)
		{
		}

		public void UpdateLocalization()
		{
			IJEODNCIJLK();
			ONJLCKGEHMD();
		}

		public void ChangePicture()
		{
			PJCDILABGPP = true;
		}

		public void ScalePicture()
		{
			float nHIDNIPGCPC = SystemProperties.NHIDNIPGCPC;
		}

		public void AddLogo()
		{
		}

		public void RemoveLogo()
		{
		}

		public void AddLoadingPic()
		{
			if (OJCNKNALKEO == null)
			{
				float nHIDNIPGCPC = SystemProperties.NHIDNIPGCPC;
			}
		}

		public void RemoveLoadingPic()
		{
		}

		private void ONJLCKGEHMD()
		{
		}

		protected virtual void Draw()
		{
		}

		private void IJEODNCIJLK()
		{
		}
	}
}
