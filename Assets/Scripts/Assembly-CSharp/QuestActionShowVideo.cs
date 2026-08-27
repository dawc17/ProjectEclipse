using System.IO;
using System.Xml;
using UnityEngine;

public class QuestActionShowVideo : QuestAction
{
	private GameObject FECENALPJDH;

	private VideoPlayerController NOOBPIDLFNH;

	private string PIANEEJIGBH;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		PIANEEJIGBH = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		// The mobile intro is over 100 seconds long and the original player only
		// accepted touch input.  In the Editor that presents as an unskippable
		// black loader screen, so continue the first-launch quest immediately.
		if (Application.isEditor && string.Equals(PIANEEJIGBH, "intro.mp4", System.StringComparison.OrdinalIgnoreCase))
		{
			Debug.Log("[Video] Skipping mobile intro during Editor play.");
			OGIJONMKABB();
			return;
		}
		FECENALPJDH = (GameObject)Object.Instantiate(Resources.Load("Prefabs/VideoScreen"));
		NOOBPIDLFNH = FECENALPJDH.GetComponent<VideoPlayerController>();
		NOOBPIDLFNH.Init();
		NOOBPIDLFNH.add_ShowCompleted(IKBCACMMLHE);
		string text = string.Format("{0}/{1}", SF2Paths.MEKBAHBKMNB(), PIANEEJIGBH);
		if (File.Exists(text))
		{
			NOOBPIDLFNH.Play(text);
		}
		else
		{
			NOOBPIDLFNH.Play(ResourceManager.DEKCGMCMGKK(PIANEEJIGBH));
		}
	}

	private void IKBCACMMLHE()
	{
		NOOBPIDLFNH.remove_ShowCompleted(IKBCACMMLHE);
		Object.Destroy(FECENALPJDH, 1f);
		OGIJONMKABB();
	}
}
