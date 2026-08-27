using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LargeFileDownloadSample : MonoBehaviour
{
	private const string URL = "http://ipv4.download.thinkbroadband.com/100MB.zip";

	private HTTPRequest ONOCIELLAPL;

	private string status = string.Empty;

	private float progress;

	private int fragmentSize = 4096;

	private void Awake()
	{
		if (PlayerPrefs.HasKey("DownloadLength"))
		{
			progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
		}
	}

	private void OnDestroy()
	{
		if (ONOCIELLAPL != null && ONOCIELLAPL.FLBBFDNHJAJ() < CFGBMHKCENK.Finished)
		{
			ONOCIELLAPL.OGLIKFCADME = null;
			ONOCIELLAPL.AFGFGHKDJJI(null);
			ONOCIELLAPL.AKLEEMEHBIC();
		}
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.Label("Request status: " + status);
			GUILayout.Space(5f);
			GUILayout.Label(string.Format("Progress: {0:P2} of {1:N0}Mb", progress, PlayerPrefs.GetInt("DownloadLength") / 1048576));
			GUILayout.HorizontalSlider(progress, 0f, 1f);
			GUILayout.Space(50f);
			if (ONOCIELLAPL == null)
			{
				GUILayout.Label(string.Format("Desired Fragment Size: {0:N} KBytes", (float)fragmentSize / 1024f));
				fragmentSize = (int)GUILayout.HorizontalSlider(fragmentSize, 4096f, 10485760f);
				GUILayout.Space(5f);
				string text = ((!PlayerPrefs.HasKey("DownloadProgress")) ? "Start Download" : "Continue Download");
				if (GUILayout.Button(text))
				{
					BMOGONHMAKN();
				}
			}
			else if (ONOCIELLAPL.FLBBFDNHJAJ() == CFGBMHKCENK.Processing && GUILayout.Button("Abort Download"))
			{
				ONOCIELLAPL.AKLEEMEHBIC();
			}
		});
	}

	private void BMOGONHMAKN()
	{
		ONOCIELLAPL = new HTTPRequest(new Uri("http://ipv4.download.thinkbroadband.com/100MB.zip"), (HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO) =>
		{
			switch (CGOIOKHEGOE.FLBBFDNHJAJ())
			{
			case CFGBMHKCENK.Processing:
				if (!PlayerPrefs.HasKey("DownloadLength"))
				{
					string text = BEIGFGCBICO.GetFirstHeaderValue("content-length");
					if (!string.IsNullOrEmpty(text))
					{
						PlayerPrefs.SetInt("DownloadLength", int.Parse(text));
					}
				}
				HNILLCOCGHA(BEIGFGCBICO.IOLFNBDPDDF());
				status = "Processing";
				break;
			case CFGBMHKCENK.Finished:
				if (BEIGFGCBICO.AICKPAMONBH())
				{
					HNILLCOCGHA(BEIGFGCBICO.IOLFNBDPDDF());
					if (BEIGFGCBICO.MJPPHHLMPEI())
					{
						status = "Streaming finished!";
						PlayerPrefs.DeleteKey("DownloadProgress");
						PlayerPrefs.Save();
						ONOCIELLAPL = null;
					}
					else
					{
						status = "Processing";
					}
				}
				else
				{
					status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
					AdvLog.LOPHFKMOPAA(status);
					ONOCIELLAPL = null;
				}
				break;
			case CFGBMHKCENK.Error:
				status = "Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
				AdvLog.CCOFFJPPAKC(status);
				ONOCIELLAPL = null;
				break;
			case CFGBMHKCENK.Aborted:
				status = "Request Aborted!";
				AdvLog.LOPHFKMOPAA(status);
				ONOCIELLAPL = null;
				break;
			case CFGBMHKCENK.ConnectionTimedOut:
				status = "Connection Timed Out!";
				AdvLog.CCOFFJPPAKC(status);
				ONOCIELLAPL = null;
				break;
			case CFGBMHKCENK.TimedOut:
				status = "Processing the request Timed Out!";
				AdvLog.CCOFFJPPAKC(status);
				ONOCIELLAPL = null;
				break;
			}
		});
		if (PlayerPrefs.HasKey("DownloadProgress"))
		{
			ONOCIELLAPL.SetRangeHeader(PlayerPrefs.GetInt("DownloadProgress"));
		}
		else
		{
			PlayerPrefs.SetInt("DownloadProgress", 0);
		}
		ONOCIELLAPL.JJCLPAGJEBJ(true);
		ONOCIELLAPL.DMHKNGKPHLJ(true);
		ONOCIELLAPL.LPALILOEHPE(fragmentSize);
		ONOCIELLAPL.Send();
	}

	private void HNILLCOCGHA(List<byte[]> DAGGODDBKDD)
	{
		if (DAGGODDBKDD != null && DAGGODDBKDD.Count > 0)
		{
			for (int i = 0; i < DAGGODDBKDD.Count; i++)
			{
				int value = PlayerPrefs.GetInt("DownloadProgress") + DAGGODDBKDD[i].Length;
				PlayerPrefs.SetInt("DownloadProgress", value);
			}
			PlayerPrefs.Save();
			progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
		}
	}
}
