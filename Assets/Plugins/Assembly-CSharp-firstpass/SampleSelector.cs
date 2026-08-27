using System.Collections.Generic;
using UnityEngine;

public class SampleSelector : MonoBehaviour
{
	public const int statisticsHeight = 160;

	private List<SampleDescriptor> POKCHMGCOOB = new List<SampleDescriptor>();

	public static SampleDescriptor SelectedSample;

	private Vector2 scrollPos;

	private void Awake()
	{
		HTTPManager.MBBMPNDDPIH().DLDMOHEGENM(BFNKPHDJNII.All);
		List<SampleDescriptor> pOKCHMGCOOB = POKCHMGCOOB;
		SampleDescriptor oBEFFHFLFKL = new SampleDescriptor(null, "HTTP Samples", string.Empty, string.Empty);
		oBEFFHFLFKL.FPEMDEEFJEL(true);
		pOKCHMGCOOB.Add(oBEFFHFLFKL);
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(TextureDownloadSample), "Texture Download", "With HTTPManager.MaxConnectionPerServer you can control how many requests can be processed per server parallel.\n\nFeatures demoed in this example:\n-Parallel requests to the same server\n-Controlling the parallelization\n-Automatic Caching\n-Create a Texture2D from the downloaded data", CodeBlocks.EFBJAIJLOBG));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(AssetBundleSample), "AssetBundle Download", "A small example that shows a possible way to download an AssetBundle and load a resource from it.\n\nFeatures demoed in this example:\n-Using HTTPRequest without a callback\n-Using HTTPRequest in a Coroutine\n-Loading an AssetBundle from the downloaded bytes\n-Automatic Caching", CodeBlocks.AKFJAIAHIGP));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(LargeFileDownloadSample), "Large File Download", "This example demonstrates how you can download a (large) file and continue the download after the connection is aborted.\n\nFeatures demoed in this example:\n-Setting up a streamed download\n-How to access the downloaded data while the download is in progress\n-Setting the HTTPRequest's StreamFragmentSize to controll the frequency and size of the fragments\n-How to use the SetRangeHeader to continue a previously disconnected download\n-How to disable the local, automatic caching", CodeBlocks.LEDJHKPLGGE));
		List<SampleDescriptor> pOKCHMGCOOB2 = POKCHMGCOOB;
		oBEFFHFLFKL = new SampleDescriptor(null, "WebSocket Samples", string.Empty, string.Empty);
		oBEFFHFLFKL.FPEMDEEFJEL(true);
		pOKCHMGCOOB2.Add(oBEFFHFLFKL);
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(WebSocketSample), "Echo", "A WebSocket demonstration that connects to a WebSocket echo service.\n\nFeatures demoed in this example:\n-Basic useage of the WebSocket class", CodeBlocks.KDMBKNIAHIL));
		List<SampleDescriptor> pOKCHMGCOOB3 = POKCHMGCOOB;
		oBEFFHFLFKL = new SampleDescriptor(null, "Socket.IO Samples", string.Empty, string.Empty);
		oBEFFHFLFKL.FPEMDEEFJEL(true);
		pOKCHMGCOOB3.Add(oBEFFHFLFKL);
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(SocketIOChatSample), "Chat", "This example uses the Socket.IO implementation to connect to the official Chat demo server(http://chat.socket.io/).\n\nFeatures demoed in this example:\n-Instantiating and setting up a SocketManager to connect to a Socket.IO server\n-Changing SocketOptions property\n-Subscribing to Socket.IO events\n-Sending custom events to the server", CodeBlocks.CBDBFJDCDBM));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(SocketIOWePlaySample), "WePlay", "This example uses the Socket.IO implementation to connect to the official WePlay demo server(http://weplay.io/).\n\nFeatures demoed in this example:\n-Instantiating and setting up a SocketManager to connect to a Socket.IO server\n-Subscribing to Socket.IO events\n-Receiving binary data\n-How to load a texture from the received binary data\n-How to disable payload decoding for fine tune for some speed\n-Sending custom events to the server", CodeBlocks.BLMNBHAHINM));
		List<SampleDescriptor> pOKCHMGCOOB4 = POKCHMGCOOB;
		oBEFFHFLFKL = new SampleDescriptor(null, "SignalR Samples", string.Empty, string.Empty);
		oBEFFHFLFKL.FPEMDEEFJEL(true);
		pOKCHMGCOOB4.Add(oBEFFHFLFKL);
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(SimpleStreamingSample), "Simple Streaming", "A very simple example of a background thread that broadcasts the server time to all connected clients every two seconds.\n\nFeatures demoed in this example:\n-Subscribing and handling non-hub messages", CodeBlocks.MHPGHMJFHNK));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(ConnectionAPISample), "Connection API", "Demonstrates all features of the lower-level connection API including starting and stopping, sending and receiving messages, and managing groups.\n\nFeatures demoed in this example:\n-Instantiating and setting up a SignalR Connection to connect to a SignalR server\n-Changing the default Json encoder\n-Subscribing to state changes\n-Receiving and handling of non-hub messages\n-Sending non-hub messages\n-Managing groups", CodeBlocks.BIHAOHNMHIE));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(ConnectionStatusSample), "Connection Status", "Demonstrates how to handle the events that are raised when connections connect, reconnect and disconnect from the Hub API.\n\nFeatures demoed in this example:\n-Connecting to a Hub\n-Setting up a callback for Hub events\n-Handling server-sent method call requests\n-Calling a Hub-method on the server-side\n-Opening and closing the SignalR Connection", CodeBlocks.COMJLIDBFCC));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(DemoHubSample), "Demo Hub", "A contrived example that exploits every feature of the Hub API.\n\nFeatures demoed in this example:\n-Creating and using wrapper Hub classes to encapsulate hub functions and events\n-Handling long running server-side functions by handling progress messages\n-Groups\n-Handling server-side functions with return value\n-Handling server-side functions throwing Exceptions\n-Calling server-side functions with complex type parameters\n-Calling server-side functions with array parameters\n-Calling overloaded server-side functions\n-Changing Hub states\n-Receiving and handling hub state changes\n-Calling server-side functions implemented in VB .NET", CodeBlocks.BNEFAGACJLN));
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(AuthenticationSample), "Authentication", "Demonstrates how to use the authorization features of the Hub API to restrict certain Hubs and methods to specific users.\n\nFeatures demoed in this example:\n-Creating and using wrapper Hub classes to encapsulate hub functions and events\n-Create and use a Header-based authenticator to access protected APIs\n-SignalR over HTTPS", CodeBlocks.NGINLOLGPEC));
		List<SampleDescriptor> pOKCHMGCOOB5 = POKCHMGCOOB;
		oBEFFHFLFKL = new SampleDescriptor(null, "Plugin Samples", string.Empty, string.Empty);
		oBEFFHFLFKL.FPEMDEEFJEL(true);
		pOKCHMGCOOB5.Add(oBEFFHFLFKL);
		POKCHMGCOOB.Add(new SampleDescriptor(typeof(CacheMaintenanceSample), "Cache Maintenance", "With this demo you can see how you can use the HTTPCacheService's BeginMaintainence function to delete too old cached entities and keep the cache size under a specified value.\n\nFeatures demoed in this example:\n-How to set up a HTTPCacheMaintananceParams\n-How to call the BeginMaintainence function", CodeBlocks.EKIPMACDDBK));
		SelectedSample = POKCHMGCOOB[1];
	}

	private void Start()
	{
		GUIHelper.ClientArea = new Rect(0f, 165f, Screen.width, Screen.height - 160 - 50);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (SelectedSample != null && SelectedSample.NMACGEJHPDN())
			{
				SelectedSample.EHDDIIAKFGI();
			}
			else
			{
				Application.Quit();
			}
		}
		if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && SelectedSample != null && !SelectedSample.NMACGEJHPDN())
		{
			SelectedSample.CreateUnityObject();
		}
	}

	private void OnGUI()
	{
		GeneralStatistics GBOBPGDJMFI = HTTPManager.CBGIGIBGBLD(StatisticsQueryFlags.All);
		GUIHelper.ECMOBPFHNPN(new Rect(0f, 0f, Screen.width / 3, 160f), false, () =>
		{
			GUIHelper.GECFPNNDHHJ("Connections");
			GUILayout.Space(5f);
			GUIHelper.IDPAKMFLODB("Sum:", GBOBPGDJMFI.GKLCMJOHCBJ.ToString());
			GUIHelper.IDPAKMFLODB("Active:", GBOBPGDJMFI.BPIDLPMODDC.ToString());
			GUIHelper.IDPAKMFLODB("Free:", GBOBPGDJMFI.HHGJJICJJOJ.ToString());
			GUIHelper.IDPAKMFLODB("Recycled:", GBOBPGDJMFI.OBMKJIOLKNI.ToString());
			GUIHelper.IDPAKMFLODB("Requests in queue:", GBOBPGDJMFI.AOIDCCECOIE.ToString());
		});
		GUIHelper.ECMOBPFHNPN(new Rect(Screen.width / 3, 0f, Screen.width / 3, 160f), false, () =>
		{
			GUIHelper.GECFPNNDHHJ("Cache");
			if (!HTTPCacheService.EPACOIFEICA())
			{
				GUI.color = Color.yellow;
				GUIHelper.GECFPNNDHHJ("Disabled in WebPlayer & Samsung Smart TV Builds!");
				GUI.color = Color.white;
			}
			GUILayout.Space(5f);
			GUIHelper.IDPAKMFLODB("Cached entities:", GBOBPGDJMFI.LMEFONBEGEN.ToString());
			GUIHelper.IDPAKMFLODB("Sum Size (bytes): ", GBOBPGDJMFI.CacheSize.ToString("N0"));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear Cache"))
			{
				HTTPCacheService.ACMOEBGCPML();
			}
			GUILayout.EndVertical();
		});
		GUIHelper.ECMOBPFHNPN(new Rect(Screen.width / 3 * 2, 0f, Screen.width / 3, 160f), false, () =>
		{
			GUIHelper.GECFPNNDHHJ("Cookies");
			if (!CookieJar.NOMOAENPKCP())
			{
				GUI.color = Color.yellow;
				GUIHelper.GECFPNNDHHJ("Saving and loading from disk is disabled in WebPlayer & Samsung Smart TV Builds!");
				GUI.color = Color.white;
			}
			GUILayout.Space(5f);
			GUIHelper.IDPAKMFLODB("Cookies:", GBOBPGDJMFI.GPBFMKPPIAL.ToString());
			GUIHelper.IDPAKMFLODB("Estimated size (bytes):", GBOBPGDJMFI.CookieJarSize.ToString("N0"));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear Cookies"))
			{
				CookieJar.Clear();
			}
			GUILayout.EndVertical();
		});
		if (SelectedSample == null || (SelectedSample != null && !SelectedSample.NMACGEJHPDN()))
		{
			GUIHelper.ECMOBPFHNPN(new Rect(0f, 165f, (SelectedSample != null) ? (Screen.width / 3) : Screen.width, Screen.height - 160 - 5), false, () =>
			{
				scrollPos = GUILayout.BeginScrollView(scrollPos);
				for (int i = 0; i < POKCHMGCOOB.Count; i++)
				{
					NCOFADLOBME(POKCHMGCOOB[i]);
				}
				GUILayout.EndScrollView();
			});
			if (SelectedSample != null)
			{
				LJDGOOGHEKC(SelectedSample);
			}
		}
		else if (SelectedSample != null && SelectedSample.NMACGEJHPDN())
		{
			GUILayout.BeginArea(new Rect(0f, Screen.height - 50, Screen.width, 50f), string.Empty);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Back", GUILayout.MinWidth(100f)))
			{
				SelectedSample.EHDDIIAKFGI();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	private void NCOFADLOBME(SampleDescriptor CINILOKJAGN)
	{
		if (CINILOKJAGN.AFDFPBKGIIL())
		{
			GUILayout.Space(15f);
			GUIHelper.GECFPNNDHHJ(CINILOKJAGN.IFBOMKBDANN());
			GUILayout.Space(5f);
		}
		else if (GUILayout.Button(CINILOKJAGN.IFBOMKBDANN()))
		{
			CINILOKJAGN.OBNFCPCDNEJ(true);
			if (SelectedSample != null)
			{
				SelectedSample.OBNFCPCDNEJ(false);
			}
			SelectedSample = CINILOKJAGN;
		}
	}

	private void LJDGOOGHEKC(SampleDescriptor CINILOKJAGN)
	{
		Rect rect = new Rect(Screen.width / 3, 165f, Screen.width / 3 * 2, Screen.height - 160 - 5);
		GUI.Box(rect, string.Empty);
		GUILayout.BeginArea(rect);
		GUILayout.BeginVertical();
		GUIHelper.GECFPNNDHHJ(CINILOKJAGN.IFBOMKBDANN());
		GUILayout.Space(5f);
		GUILayout.Label(CINILOKJAGN.GJOAJAIJHOE());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start Sample"))
		{
			CINILOKJAGN.CreateUnityObject();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
