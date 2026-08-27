using System;
using UnityEngine;

public sealed class ConnectionAPISample : MonoBehaviour
{
	private enum AGFILDNOCMD
	{
		Send = 0,
		Broadcast = 1,
		Join = 2,
		PrivateMessage = 3,
		AddToGroup = 4,
		RemoveFromGroup = 5,
		SendToGroup = 6,
		BroadcastExceptMe = 7
	}

	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/raw-connection/");

	private Connection FJGOJHMELAH;

	private string GOKALAMDGAL = string.Empty;

	private string AJHINGDCGLO = string.Empty;

	private string AIDLJIEDHKI = string.Empty;

	private string KPKDNFNLLHH = string.Empty;

	private GUIMessageList messages = new GUIMessageList();

	private void Start()
	{
		if (PlayerPrefs.HasKey("userName"))
		{
			CookieJar.Set(URI, new Cookie("user", PlayerPrefs.GetString("userName")));
		}
		FJGOJHMELAH = new Connection(URI);
		FJGOJHMELAH.LPEPILDNMNE(new PEELJCOAGOH());
		FJGOJHMELAH.FADMHEJNPJO(DLJDCFEGCOE);
		FJGOJHMELAH.EHOAGKMPCJH(ECDBJNFKOKI);
		FJGOJHMELAH.LAJCMNNNIIM();
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.BeginVertical();
			GUILayout.Label("To Everybody");
			GUILayout.BeginHorizontal();
			GOKALAMDGAL = GUILayout.TextField(GOKALAMDGAL, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Broadcast"))
			{
				DADCDNNAHIE(GOKALAMDGAL);
			}
			if (GUILayout.Button("Broadcast (All Except Me)"))
			{
				NDIOIGGMBEE(GOKALAMDGAL);
			}
			if (GUILayout.Button("Enter Name"))
			{
				AMLMKONNCMJ(GOKALAMDGAL);
			}
			if (GUILayout.Button("Join Group"))
			{
				DEDGJIFLHLH(GOKALAMDGAL);
			}
			if (GUILayout.Button("Leave Group"))
			{
				OGJBPKPJCIL(GOKALAMDGAL);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("To Me");
			GUILayout.BeginHorizontal();
			AJHINGDCGLO = GUILayout.TextField(AJHINGDCGLO, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Send to me"))
			{
				KNHFPLLADPB(AJHINGDCGLO);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Private Message");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Message:");
			AIDLJIEDHKI = GUILayout.TextField(AIDLJIEDHKI, GUILayout.MinWidth(100f));
			GUILayout.Label("User or Group name:");
			KPKDNFNLLHH = GUILayout.TextField(KPKDNFNLLHH, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Send to user"))
			{
				PNHJAFFODNO(KPKDNFNLLHH, AIDLJIEDHKI);
			}
			if (GUILayout.Button("Send to group"))
			{
				ALFMFLDILFE(KPKDNFNLLHH, AIDLJIEDHKI);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			if (FJGOJHMELAH.FLBBFDNHJAJ() == OHLFKFFAOMF.Closed)
			{
				if (GUILayout.Button("Start Connection"))
				{
					FJGOJHMELAH.LAJCMNNNIIM();
				}
			}
			else if (GUILayout.Button("Stop Connection"))
			{
				FJGOJHMELAH.Close();
			}
			GUILayout.Space(20f);
			GUILayout.Label("Messages");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			messages.MCAIPGEPMDE(Screen.width - 20, 0f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		});
	}

	private void OnDestroy()
	{
		FJGOJHMELAH.Close();
	}

	private void ECDBJNFKOKI(Connection BJGMPDIKEJC, object data)
	{
		string text = Json.Encode(data);
		messages.Add("[Server Message] " + text);
	}

	private void DLJDCFEGCOE(Connection BJGMPDIKEJC, OHLFKFFAOMF JOBAGBFMMFP, OHLFKFFAOMF MPJEMGJIBBD)
	{
		messages.Add(string.Format("[State Change] {0} => {1}", JOBAGBFMMFP.ToString(), MPJEMGJIBBD.ToString()));
	}

	private void DADCDNNAHIE(string HCPNFPMHFCM)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.Broadcast,
			Value = HCPNFPMHFCM
		});
	}

	private void NDIOIGGMBEE(string HCPNFPMHFCM)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.BroadcastExceptMe,
			Value = HCPNFPMHFCM
		});
	}

	private void AMLMKONNCMJ(string name)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.Join,
			Value = name
		});
	}

	private void DEDGJIFLHLH(string LKLJOLILPCJ)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.AddToGroup,
			Value = LKLJOLILPCJ
		});
	}

	private void OGJBPKPJCIL(string LKLJOLILPCJ)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.RemoveFromGroup,
			Value = LKLJOLILPCJ
		});
	}

	private void KNHFPLLADPB(string HCPNFPMHFCM)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.Send,
			Value = HCPNFPMHFCM
		});
	}

	private void PNHJAFFODNO(string HIMLMCMHHGJ, string HCPNFPMHFCM)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.PrivateMessage,
			Value = string.Format("{0}|{1}", HIMLMCMHHGJ, HCPNFPMHFCM)
		});
	}

	private void ALFMFLDILFE(string HIMLMCMHHGJ, string HCPNFPMHFCM)
	{
		FJGOJHMELAH.Send(new
		{
			Type = AGFILDNOCMD.SendToGroup,
			Value = string.Format("{0}|{1}", HIMLMCMHHGJ, HCPNFPMHFCM)
		});
	}
}
