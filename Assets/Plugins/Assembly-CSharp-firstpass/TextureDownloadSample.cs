using System;
using UnityEngine;

public sealed class TextureDownloadSample : MonoBehaviour
{
	private const string BaseURL = "http://besthttp.azurewebsites.net/Content/";

	private string[] Images = new string[9] { "One.png", "Two.png", "Three.png", "Four.png", "Five.png", "Six.png", "Seven.png", "Eight.png", "Nine.png" };

	private Texture2D[] Textures = new Texture2D[9];

	private bool allDownloadedFromLocalCache;

	private int finishedCount;

	private Vector2 scrollPos;

	private void Awake()
	{
		HTTPManager.set_MaxConnectionPerServer(1);
		for (int i = 0; i < Images.Length; i++)
		{
			Textures[i] = new Texture2D(100, 150);
		}
	}

	private void OnDestroy()
	{
		HTTPManager.set_MaxConnectionPerServer(4);
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.SelectionGrid(0, Textures, 3);
			if (finishedCount == Images.Length && allDownloadedFromLocalCache)
			{
				GUIHelper.GECFPNNDHHJ("All images loaded from the local cache!");
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Connection/Server: ", GUILayout.Width(150f));
			GUILayout.Label(HTTPManager.BDCIBFLAPJN().ToString(), GUILayout.Width(20f));
			HTTPManager.set_MaxConnectionPerServer((byte)GUILayout.HorizontalSlider((int)HTTPManager.BDCIBFLAPJN(), 1f, 10f));
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Start Download"))
			{
				HMKCMNNDNNC();
			}
			GUILayout.EndScrollView();
		});
	}

	private void HMKCMNNDNNC()
	{
		allDownloadedFromLocalCache = true;
		finishedCount = 0;
		for (int i = 0; i < Images.Length; i++)
		{
			Textures[i] = new Texture2D(100, 150);
			HTTPRequest iPLGNIDJDCF = new HTTPRequest(new Uri("http://besthttp.azurewebsites.net/Content/" + Images[i]), POOAMOOGNDL);
			iPLGNIDJDCF.set_Tag(Textures[i]);
			iPLGNIDJDCF.Send();
		}
	}

	private void POOAMOOGNDL(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		finishedCount++;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				Texture2D texture2D = CGOIOKHEGOE.LOIGCKFONHJ() as Texture2D;
				texture2D.LoadImage(BEIGFGCBICO.CHIGLEKCFFN());
				allDownloadedFromLocalCache = allDownloadedFromLocalCache && BEIGFGCBICO.LOHDBJLLKEE();
			}
			else
			{
				AdvLog.LOPHFKMOPAA(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB()));
			}
			break;
		case CFGBMHKCENK.Error:
			AdvLog.CCOFFJPPAKC("Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace)));
			break;
		case CFGBMHKCENK.Aborted:
			AdvLog.LOPHFKMOPAA("Request Aborted!");
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			AdvLog.CCOFFJPPAKC("Connection Timed Out!");
			break;
		case CFGBMHKCENK.TimedOut:
			AdvLog.CCOFFJPPAKC("Processing the request Timed Out!");
			break;
		}
	}
}
