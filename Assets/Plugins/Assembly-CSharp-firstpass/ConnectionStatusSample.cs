using System;
using UnityEngine;

internal sealed class ConnectionStatusSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/signalr");

	private Connection FJGOJHMELAH;

	private GUIMessageList messages = new GUIMessageList();

	private void Start()
	{
		FJGOJHMELAH = new Connection(URI, "StatusHub");
		FJGOJHMELAH.EHOAGKMPCJH(ECBGAEFNPBA);
		FJGOJHMELAH.BJDMHEHILEO(KGKBELBAKJO);
		FJGOJHMELAH.FADMHEJNPJO(DLJDCFEGCOE);
		FJGOJHMELAH.get_Item("StatusHub").OPHFDPDINKG(HAOGIBHGBNP);
		FJGOJHMELAH.LAJCMNNNIIM();
	}

	private void OnDestroy()
	{
		FJGOJHMELAH.Close();
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("START") && FJGOJHMELAH.FLBBFDNHJAJ() != OHLFKFFAOMF.Connected)
			{
				FJGOJHMELAH.LAJCMNNNIIM();
			}
			if (GUILayout.Button("STOP") && FJGOJHMELAH.FLBBFDNHJAJ() == OHLFKFFAOMF.Connected)
			{
				FJGOJHMELAH.Close();
				messages.Clear();
			}
			if (GUILayout.Button("PING") && FJGOJHMELAH.FLBBFDNHJAJ() == OHLFKFFAOMF.Connected)
			{
				FJGOJHMELAH.get_Item("StatusHub").Call("Ping");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.Label("Connection Status Messages");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			messages.MCAIPGEPMDE(Screen.width - 20, 0f);
			GUILayout.EndHorizontal();
		});
	}

	private void ECBGAEFNPBA(Connection BJGMPDIKEJC, object data)
	{
		messages.Add("[Server Message] " + data.ToString());
	}

	private void DLJDCFEGCOE(Connection BJGMPDIKEJC, OHLFKFFAOMF JOBAGBFMMFP, OHLFKFFAOMF MPJEMGJIBBD)
	{
		messages.Add(string.Format("[State Change] {0} => {1}", JOBAGBFMMFP, MPJEMGJIBBD));
	}

	private void KGKBELBAKJO(Connection BJGMPDIKEJC, string JDONBAPIJCG)
	{
		messages.Add("[Error] " + JDONBAPIJCG);
	}

	private void HAOGIBHGBNP(Hub CGFIJCNNCKP, string FJLOLCPJACB, params object[] LKIOKGCNKHE)
	{
		string arg = ((LKIOKGCNKHE.Length <= 0) ? string.Empty : (LKIOKGCNKHE[0] as string));
		string arg2 = ((LKIOKGCNKHE.Length <= 1) ? string.Empty : LKIOKGCNKHE[1].ToString());
		switch (FJLOLCPJACB)
		{
		case "joined":
			messages.Add(string.Format("[{0}] {1} joined at {2}", CGFIJCNNCKP.get_Name(), arg, arg2));
			break;
		case "rejoined":
			messages.Add(string.Format("[{0}] {1} reconnected at {2}", CGFIJCNNCKP.get_Name(), arg, arg2));
			break;
		case "leave":
			messages.Add(string.Format("[{0}] {1} leaved at {2}", CGFIJCNNCKP.get_Name(), arg, arg2));
			break;
		default:
			messages.Add(string.Format("[{0}] {1}", CGFIJCNNCKP.get_Name(), FJLOLCPJACB));
			break;
		}
	}
}
