using System;
using UnityEngine;

internal class AuthenticationSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection FJGOJHMELAH;

	private string IFCOOFDKDGL = string.Empty;

	private string BOOICHNBHBL = string.Empty;

	private Vector2 scrollPos;

	private void Start()
	{
		FJGOJHMELAH = new Connection(URI, new BaseHub("noauthhub", "Messages"), new BaseHub("invokeauthhub", "Messages Invoked By Admin or Invoker"), new BaseHub("authhub", "Messages Requiring Authentication to Send or Receive"), new BaseHub("inheritauthhub", "Messages Requiring Authentication to Send or Receive Because of Inheritance"), new BaseHub("incomingauthhub", "Messages Requiring Authentication to Send"), new BaseHub("adminauthhub", "Messages Requiring Admin Membership to Send or Receive"), new BaseHub("userandroleauthhub", "Messages Requiring Name to be \"User\" and Role to be \"Admin\" to Send or Receive"));
		if (!string.IsNullOrEmpty(IFCOOFDKDGL) && !string.IsNullOrEmpty(BOOICHNBHBL))
		{
			FJGOJHMELAH.FBFLBJGPEGA(new HeaderAuthenticator(IFCOOFDKDGL, BOOICHNBHBL));
		}
		FJGOJHMELAH.FJBEHFPIAHI(INACOFIJGKE);
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
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
			GUILayout.BeginVertical();
			if (FJGOJHMELAH.DLKDCNNCKCL() == null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Username (Enter 'User'):");
				IFCOOFDKDGL = GUILayout.TextField(IFCOOFDKDGL, GUILayout.MinWidth(100f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Roles (Enter 'Invoker' or 'Admin'):");
				BOOICHNBHBL = GUILayout.TextField(BOOICHNBHBL, GUILayout.MinWidth(100f));
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Log in"))
				{
					GEBDDPAFKCH();
				}
			}
			for (int i = 0; i < FJGOJHMELAH.LINDGKFKGND().Length; i++)
			{
				(FJGOJHMELAH.LINDGKFKGND()[i] as BaseHub).MCAIPGEPMDE();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}

	private void INACOFIJGKE(Connection BJGMPDIKEJC)
	{
		for (int i = 0; i < FJGOJHMELAH.LINDGKFKGND().Length; i++)
		{
			(FJGOJHMELAH.LINDGKFKGND()[i] as BaseHub).IJOCHDFBMJN();
		}
	}

	private void GEBDDPAFKCH()
	{
		FJGOJHMELAH.LCIOENIELOA(INACOFIJGKE);
		FJGOJHMELAH.Close();
		FJGOJHMELAH = null;
		Start();
	}
}
