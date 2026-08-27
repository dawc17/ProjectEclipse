using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SocketIOWePlaySample : MonoBehaviour
{
	private enum ODEGBLBHENN
	{
		Connecting = 0,
		WaitForNick = 1,
		Joined = 2
	}

	private string[] controls = new string[8] { "left", "right", "a", "b", "up", "down", "select", "start" };

	private const float ratio = 1.5f;

	private int DNJCIJMNIAP = 50;

	private ODEGBLBHENN AFINHOBCHMC;

	private Socket KNPLDJGCAKJ;

	private string PLNHHIOPCMM = string.Empty;

	private string IPNDLNBHBED = string.Empty;

	private int PHINJLLLGOK;

	private List<string> messages = new List<string>();

	private Vector2 scrollPos;

	private Texture2D FrameTexture;

	private void Start()
	{
		SocketOptions pGHMKLAAHKP = new SocketOptions();
		pGHMKLAAHKP.AHGIJFEGONK(false);
		SocketManager mFANOMMMCFG = new SocketManager(new Uri("http://io.weplay.io/socket.io/"), pGHMKLAAHKP);
		KNPLDJGCAKJ = mFANOMMMCFG.PDJFKOBODHH();
		KNPLDJGCAKJ.JPJAFMLNALO(ECDAJBEFCAH.Connect, PIGDCLOPNKJ);
		KNPLDJGCAKJ.JPJAFMLNALO("joined", DPIKEOICPHA);
		KNPLDJGCAKJ.JPJAFMLNALO("connections", PFJGKIABBEA);
		KNPLDJGCAKJ.JPJAFMLNALO("join", DEEFHPDIEEA);
		KNPLDJGCAKJ.JPJAFMLNALO("move", DLGGMAJKJNO);
		KNPLDJGCAKJ.JPJAFMLNALO("message", OnMessage);
		KNPLDJGCAKJ.JPJAFMLNALO("reload", GFOOJBJJICD);
		KNPLDJGCAKJ.JPJAFMLNALO("frame", FJMAEKAMKCP, false);
		KNPLDJGCAKJ.JPJAFMLNALO(ECDAJBEFCAH.Error, OnError);
		mFANOMMMCFG.Open();
		AFINHOBCHMC = ODEGBLBHENN.Connecting;
	}

	private void OnDestroy()
	{
		KNPLDJGCAKJ.HLBNHJADOMP().Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.EHDDIIAKFGI();
		}
	}

	private void OnGUI()
	{
		switch (AFINHOBCHMC)
		{
		case ODEGBLBHENN.Connecting:
			GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUIHelper.GECFPNNDHHJ("Connecting to the server...");
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			});
			break;
		case ODEGBLBHENN.WaitForNick:
			GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
			{
				CDFMAJDHEIL();
			});
			break;
		case ODEGBLBHENN.Joined:
			GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
			{
				if (FrameTexture != null)
				{
					GUILayout.Box(FrameTexture);
				}
				DBGDPNOGLMG();
				DrawChat();
			});
			break;
		}
	}

	private void CDFMAJDHEIL()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUIHelper.GECFPNNDHHJ("What's your nickname?");
		PLNHHIOPCMM = GUILayout.TextField(PLNHHIOPCMM);
		if (GUILayout.Button("Join"))
		{
			Join();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void DBGDPNOGLMG()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Controls:");
		for (int i = 0; i < controls.Length; i++)
		{
			if (GUILayout.Button(controls[i]))
			{
				KNPLDJGCAKJ.Emit("move", controls[i]);
			}
		}
		GUILayout.Label(" Connections: " + PHINJLLLGOK);
		GUILayout.EndHorizontal();
	}

	private void DrawChat(bool JBAGKKKEBGK = true)
	{
		GUILayout.BeginVertical();
		scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
		for (int i = 0; i < messages.Count; i++)
		{
			GUILayout.Label(messages[i], GUILayout.MinWidth(Screen.width));
		}
		GUILayout.EndScrollView();
		if (JBAGKKKEBGK)
		{
			GUILayout.Label("Your message: ");
			GUILayout.BeginHorizontal();
			IPNDLNBHBED = GUILayout.TextField(IPNDLNBHBED);
			if (GUILayout.Button("Send", GUILayout.MaxWidth(100f)))
			{
				CGJFNMPOGCO();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}

	private void AddMessage(string CKEHOEGLMBM)
	{
		messages.Insert(0, CKEHOEGLMBM);
		if (messages.Count > DNJCIJMNIAP)
		{
			messages.RemoveRange(DNJCIJMNIAP, messages.Count - DNJCIJMNIAP);
		}
	}

	private void CGJFNMPOGCO()
	{
		if (!string.IsNullOrEmpty(IPNDLNBHBED))
		{
			KNPLDJGCAKJ.Emit("message", IPNDLNBHBED);
			AddMessage(string.Format("{0}: {1}", PLNHHIOPCMM, IPNDLNBHBED));
			IPNDLNBHBED = string.Empty;
		}
	}

	private void Join()
	{
		PlayerPrefs.SetString("Nick", PLNHHIOPCMM);
		KNPLDJGCAKJ.Emit("join", PLNHHIOPCMM);
	}

	private void NPMIHDFCBBH()
	{
		FrameTexture = null;
		if (KNPLDJGCAKJ != null)
		{
			KNPLDJGCAKJ.HLBNHJADOMP().Close();
			KNPLDJGCAKJ = null;
			Start();
		}
	}

	private void PIGDCLOPNKJ(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (PlayerPrefs.HasKey("Nick"))
		{
			PLNHHIOPCMM = PlayerPrefs.GetString("Nick", "NickName");
			Join();
		}
		else
		{
			AFINHOBCHMC = ODEGBLBHENN.WaitForNick;
		}
		AddMessage("connected");
	}

	private void DPIKEOICPHA(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		AFINHOBCHMC = ODEGBLBHENN.Joined;
	}

	private void GFOOJBJJICD(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		NPMIHDFCBBH();
	}

	private void OnMessage(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (LKIOKGCNKHE.Length == 1)
		{
			AddMessage(LKIOKGCNKHE[0] as string);
		}
		else
		{
			AddMessage(string.Format("{0}: {1}", LKIOKGCNKHE[1], LKIOKGCNKHE[0]));
		}
	}

	private void DLGGMAJKJNO(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		AddMessage(string.Format("{0} pressed {1}", LKIOKGCNKHE[1], LKIOKGCNKHE[0]));
	}

	private void DEEFHPDIEEA(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		string arg = ((LKIOKGCNKHE.Length <= 1) ? string.Empty : string.Format("({0})", LKIOKGCNKHE[1]));
		AddMessage(string.Format("{0} joined {1}", LKIOKGCNKHE[0], arg));
	}

	private void PFJGKIABBEA(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		PHINJLLLGOK = Convert.ToInt32(LKIOKGCNKHE[0]);
	}

	private void FJMAEKAMKCP(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (AFINHOBCHMC == ODEGBLBHENN.Joined)
		{
			if (FrameTexture == null)
			{
				FrameTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
				FrameTexture.filterMode = FilterMode.Point;
			}
			byte[] data = NPKADBPBKIG.BINAPGLGAGE()[0];
			FrameTexture.LoadImage(data);
		}
	}

	private void OnError(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		AddMessage(string.Format("--ERROR - {0}", LKIOKGCNKHE[0].ToString()));
	}
}
