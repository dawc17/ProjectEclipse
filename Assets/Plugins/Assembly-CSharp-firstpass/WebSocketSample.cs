using System;
using UnityEngine;

public class WebSocketSample : MonoBehaviour
{
	private string IKHEAOEKLHL = "ws://echo.websocket.org";

	private string MDOGLPLKBJO = "Hello World!";

	private string GGDJIPKMKFC = string.Empty;

	private WebSocket ILNFPNFEOCL;

	private Vector2 scrollPos;

	private void OnDestroy()
	{
		if (ILNFPNFEOCL != null)
		{
			ILNFPNFEOCL.Close();
		}
	}

	private void OnGUI()
	{
		GUIHelper.ECMOBPFHNPN(GUIHelper.ClientArea, true, () =>
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(GGDJIPKMKFC);
			GUILayout.EndScrollView();
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			IKHEAOEKLHL = GUILayout.TextField(IKHEAOEKLHL);
			if (ILNFPNFEOCL == null && GUILayout.Button("Open Web Socket"))
			{
				ILNFPNFEOCL = new WebSocket(new Uri(IKHEAOEKLHL));
				if (HTTPManager.FHGBKFBCGCO() != null)
				{
					ILNFPNFEOCL.KGBEGJJPCKC().PNGMAECJHID(new HTTPProxy(HTTPManager.FHGBKFBCGCO().DNIJHGFINDG(), HTTPManager.FHGBKFBCGCO().HPKPFEOBIOC(), false));
				}
				WebSocket iLNFPNFEOCL = ILNFPNFEOCL;
				iLNFPNFEOCL.HKBKFMIBCED = (BNIEFDKHAJN)Delegate.Combine(iLNFPNFEOCL.HKBKFMIBCED, new BNIEFDKHAJN(HKBKFMIBCED));
				WebSocket iLNFPNFEOCL2 = ILNFPNFEOCL;
				iLNFPNFEOCL2.OnMessage = (KCEBOGOANEH)Delegate.Combine(iLNFPNFEOCL2.OnMessage, new KCEBOGOANEH(GKPFJAIFHMC));
				WebSocket iLNFPNFEOCL3 = ILNFPNFEOCL;
				iLNFPNFEOCL3.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(iLNFPNFEOCL3.OnClosed, new OnWebSocketClosedDelegate(OnClosed));
				WebSocket iLNFPNFEOCL4 = ILNFPNFEOCL;
				iLNFPNFEOCL4.OnError = (OnWebSocketErrorDelegate)Delegate.Combine(iLNFPNFEOCL4.OnError, new OnWebSocketErrorDelegate(OnError));
				ILNFPNFEOCL.LAJCMNNNIIM();
				GGDJIPKMKFC += "Opening Web Socket...\n";
			}
			if (ILNFPNFEOCL != null && ILNFPNFEOCL.DJKKJPNLOAE())
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				MDOGLPLKBJO = GUILayout.TextField(MDOGLPLKBJO);
				if (GUILayout.Button("Send", GUILayout.MaxWidth(70f)))
				{
					GGDJIPKMKFC += "Sending message...\n";
					ILNFPNFEOCL.Send(MDOGLPLKBJO);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (GUILayout.Button("Close"))
				{
					ILNFPNFEOCL.Close(1000, "Bye!");
				}
			}
		});
	}

	private void HKBKFMIBCED(WebSocket IIBIPJJLEGJ)
	{
		GGDJIPKMKFC += string.Format("-WebSocket Open!\n");
	}

	private void GKPFJAIFHMC(WebSocket IIBIPJJLEGJ, string LIOGIBJBHAH)
	{
		GGDJIPKMKFC += string.Format("-Message received: {0}\n", LIOGIBJBHAH);
	}

	private void OnClosed(WebSocket IIBIPJJLEGJ, ushort KJPGKHJNOMC, string LIOGIBJBHAH)
	{
		GGDJIPKMKFC += string.Format("-WebSocket closed! Code: {0} Message: {1}\n", KJPGKHJNOMC, LIOGIBJBHAH);
		ILNFPNFEOCL = null;
	}

	private void OnError(WebSocket IIBIPJJLEGJ, Exception MPFFFAOGBJE)
	{
		string text = string.Empty;
		if (IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG() != null)
		{
			text = string.Format("Status Code from Server: {0} and Message: {1}", IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().KNMDPGBPNED(), IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().DCKPMHKDLEJ());
		}
		GGDJIPKMKFC += string.Format("-An error occured: {0}\n", (MPFFFAOGBJE == null) ? ("Unknown Error " + text) : MPFFFAOGBJE.Message);
		ILNFPNFEOCL = null;
	}
}
