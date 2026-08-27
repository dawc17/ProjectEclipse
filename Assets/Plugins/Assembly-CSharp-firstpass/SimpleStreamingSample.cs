using System;
using UnityEngine;

internal sealed class SimpleStreamingSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/streaming-connection");

	private Connection FJGOJHMELAH;

	private GUIMessageList messages = new GUIMessageList();

	private void Start()
	{
		FJGOJHMELAH = new Connection(URI);
		FJGOJHMELAH.EHOAGKMPCJH(ECBGAEFNPBA);
		FJGOJHMELAH.FADMHEJNPJO(DLJDCFEGCOE);
		FJGOJHMELAH.BJDMHEHILEO(KGKBELBAKJO);
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
			GUILayout.Label("Messages");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			messages.MCAIPGEPMDE(Screen.width - 20, 0f);
			GUILayout.EndHorizontal();
		});
	}

	private void ECBGAEFNPBA(Connection MDGFGCDPGFI, object data)
	{
		messages.Add("[Server Message] " + data.ToString());
	}

	private void DLJDCFEGCOE(Connection MDGFGCDPGFI, OHLFKFFAOMF JOBAGBFMMFP, OHLFKFFAOMF MPJEMGJIBBD)
	{
		messages.Add(string.Format("[State Change] {0} => {1}", JOBAGBFMMFP, MPJEMGJIBBD));
	}

	private void KGKBELBAKJO(Connection MDGFGCDPGFI, string JDONBAPIJCG)
	{
		messages.Add("[Error] " + JDONBAPIJCG);
	}
}
