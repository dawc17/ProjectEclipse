using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SocketIOChatSample : MonoBehaviour
{
	private enum BCLDBFMAKBM
	{
		Login = 0,
		Chat = 1
	}

	private readonly TimeSpan TYPING_TIMER_LENGTH = TimeSpan.FromMilliseconds(700.0);

	private SocketManager CPOHGNDIBJD;

	private BCLDBFMAKBM AFINHOBCHMC;

	private string IFCOOFDKDGL = string.Empty;

	private string LIOGIBJBHAH = string.Empty;

	private string HJIGDBEJLGJ = string.Empty;

	private Vector2 scrollPos;

	private bool typing;

	private DateTime lastTypingTime = DateTime.MinValue;

	private List<string> typingUsers = new List<string>();

	private void Start()
	{
		AFINHOBCHMC = BCLDBFMAKBM.Login;
		SocketOptions pGHMKLAAHKP = new SocketOptions();
		pGHMKLAAHKP.AHGIJFEGONK(false);
		CPOHGNDIBJD = new SocketManager(new Uri("http://chat.socket.io/socket.io/"), pGHMKLAAHKP);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("login", BKHJLIEAHOO);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("new message", MOGAEBMBPHN);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("user joined", CFAPMDGPGCA);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("user left", HPJHEHHKCJD);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("typing", IJHAEGHAIEM);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO("stop typing", HNOBHBKOHDA);
		CPOHGNDIBJD.PDJFKOBODHH().JPJAFMLNALO(ECDAJBEFCAH.Error, (Socket JLEACANCMJF, Packet NPKADBPBKIG, object[] LKIOKGCNKHE) =>
		{
			AdvLog.CCOFFJPPAKC(string.Format("Error: {0}", LKIOKGCNKHE[0].ToString()));
		});
		CPOHGNDIBJD.Open();
	}

	private void OnDestroy()
	{
		CPOHGNDIBJD.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.EHDDIIAKFGI();
		}
		if (typing)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - lastTypingTime;
			if (timeSpan >= TYPING_TIMER_LENGTH)
			{
				CPOHGNDIBJD.PDJFKOBODHH().Emit("stop typing");
				typing = false;
			}
		}
	}

	private void OnGUI()
	{
		switch (AFINHOBCHMC)
		{
		case BCLDBFMAKBM.Login:
			CDFMAJDHEIL();
			break;
		case BCLDBFMAKBM.Chat:
			HMHNKACNGAI();
			break;
		}
	}

	private void CDFMAJDHEIL()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUIHelper.GECFPNNDHHJ("What's your nickname?");
			IFCOOFDKDGL = GUILayout.TextField(IFCOOFDKDGL);
			if (GUILayout.Button("Join"))
			{
				BDEIGNGJHOF();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		});
	}

	private void HMHNKACNGAI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.BeginVertical();
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(HJIGDBEJLGJ, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
			string text = string.Empty;
			if (typingUsers.Count > 0)
			{
				text += string.Format("{0}", typingUsers[0]);
				for (int i = 1; i < typingUsers.Count; i++)
				{
					text += string.Format(", {0}", typingUsers[i]);
				}
				text = ((typingUsers.Count != 1) ? (text + " are typing!") : (text + " is typing!"));
			}
			GUILayout.Label(text);
			GUILayout.Label("Type here:");
			GUILayout.BeginHorizontal();
			LIOGIBJBHAH = GUILayout.TextField(LIOGIBJBHAH);
			if (GUILayout.Button("Send", GUILayout.MaxWidth(100f)))
			{
				CGJFNMPOGCO();
			}
			GUILayout.EndHorizontal();
			if (GUI.changed)
			{
				MMNHJJNCEJM();
			}
			GUILayout.EndVertical();
		});
	}

	private void BDEIGNGJHOF()
	{
		if (!string.IsNullOrEmpty(IFCOOFDKDGL))
		{
			AFINHOBCHMC = BCLDBFMAKBM.Chat;
			CPOHGNDIBJD.PDJFKOBODHH().Emit("add user", IFCOOFDKDGL);
		}
	}

	private void CGJFNMPOGCO()
	{
		if (!string.IsNullOrEmpty(LIOGIBJBHAH))
		{
			CPOHGNDIBJD.PDJFKOBODHH().Emit("new message", LIOGIBJBHAH);
			HJIGDBEJLGJ += string.Format("{0}: {1}\n", IFCOOFDKDGL, LIOGIBJBHAH);
			LIOGIBJBHAH = string.Empty;
		}
	}

	private void MMNHJJNCEJM()
	{
		if (!typing)
		{
			typing = true;
			CPOHGNDIBJD.PDJFKOBODHH().Emit("typing");
		}
		lastTypingTime = DateTime.UtcNow;
	}

	private void DNFPAPNBKIF(Dictionary<string, object> data)
	{
		int num = Convert.ToInt32(data["numUsers"]);
		if (num == 1)
		{
			HJIGDBEJLGJ += "there's 1 participant\n";
			return;
		}
		string hJIGDBEJLGJ = HJIGDBEJLGJ;
		HJIGDBEJLGJ = hJIGDBEJLGJ + "there are " + num + " participants\n";
	}

	private void PNGFLLOBENH(Dictionary<string, object> data)
	{
		string arg = data["username"] as string;
		string arg2 = data["message"] as string;
		HJIGDBEJLGJ += string.Format("{0}: {1}\n", arg, arg2);
	}

	private void KADOMLKIKJP(Dictionary<string, object> data)
	{
		string item = data["username"] as string;
		typingUsers.Add(item);
	}

	private void FFPDLNIBCNH(Dictionary<string, object> data)
	{
		string HPCGFILEHPH = data["username"] as string;
		int num = typingUsers.FindIndex((string name) => name.Equals(HPCGFILEHPH));
		if (num != -1)
		{
			typingUsers.RemoveAt(num);
		}
	}

	private void BKHJLIEAHOO(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		HJIGDBEJLGJ = "Welcome to Socket.IO Chat — \n";
		DNFPAPNBKIF(LKIOKGCNKHE[0] as Dictionary<string, object>);
	}

	private void MOGAEBMBPHN(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		PNGFLLOBENH(LKIOKGCNKHE[0] as Dictionary<string, object>);
	}

	private void CFAPMDGPGCA(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		Dictionary<string, object> dictionary = LKIOKGCNKHE[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		HJIGDBEJLGJ += string.Format("{0} joined\n", arg);
		DNFPAPNBKIF(dictionary);
	}

	private void HPJHEHHKCJD(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		Dictionary<string, object> dictionary = LKIOKGCNKHE[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		HJIGDBEJLGJ += string.Format("{0} left\n", arg);
		DNFPAPNBKIF(dictionary);
	}

	private void IJHAEGHAIEM(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		KADOMLKIKJP(LKIOKGCNKHE[0] as Dictionary<string, object>);
	}

	private void HNOBHBKOHDA(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		FFPDLNIBCNH(LKIOKGCNKHE[0] as Dictionary<string, object>);
	}
}
