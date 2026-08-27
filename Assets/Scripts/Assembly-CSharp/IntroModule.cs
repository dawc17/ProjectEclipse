using Nekki.SF2.GUI.Scenes;
using UnityEngine;
using UnityEngine.Video;

public class IntroModule : LoadingModule
{
	private VideoClip NDPCKCLAFAK;

	private GameObject FECENALPJDH;

	private VideoPlayerController NOOBPIDLFNH;

	private GameObject _logo;

	public IntroModule(GameLoaderScene FCDFLMFEJGI)
	{
		NDPCKCLAFAK = FCDFLMFEJGI.get_IntroClip();
		_logo = FCDFLMFEJGI.get_Logo();
	}

	public static bool Disabled = true;

	public override void Start()
	{
		base.Start();
		if (Disabled || !AssemblyController.CPJFGBLMHFH())
		{
			if (_logo != null)
			{
				_logo.SetActive(true);
			}
			CHIHBINEGFL = true;
			return;
		}
		if (AssemblyController.CPJFGBLMHFH())
		{
			FECENALPJDH = (GameObject)Object.Instantiate(Resources.Load("Prefabs/VideoScreen"));
			NOOBPIDLFNH = FECENALPJDH.GetComponent<VideoPlayerController>();
			NOOBPIDLFNH.Init();
			NOOBPIDLFNH.add_ShowCompleted(HLKOKIDAPGO);
			NOOBPIDLFNH.Play(NDPCKCLAFAK);
		}
		else
		{
			_logo.SetActive(true);
			CHIHBINEGFL = true;
		}
	}

	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL && Input.anyKeyDown)
		{
			HLKOKIDAPGO();
		}
	}

	private void HLKOKIDAPGO()
	{
		NOOBPIDLFNH.remove_ShowCompleted(HLKOKIDAPGO);
		Object.Destroy(NOOBPIDLFNH, 0.5f);
		Object.Destroy(FECENALPJDH, 0.5f);
		FECENALPJDH = null;
		NDPCKCLAFAK = null;
		NOOBPIDLFNH = null;
		_logo.SetActive(true);
		CHIHBINEGFL = true;
	}
}
