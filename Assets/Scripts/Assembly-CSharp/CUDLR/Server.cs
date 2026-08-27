using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace CUDLR
{
	public class Server : MonoBehaviour
	{
		public enum ServerInitStatus
		{
			Error_Unknown = 0,
			Error_NoFreePorts = 1,
			Success = 2
		}

		public delegate void FileHandlerDelegate(RequestContext context, bool download);

		private const int _MaxPort = 65535;

		[SerializeField]
		public int Port = 55055;

		private static System.Threading.Thread mainThread;

		private static string fileRoot;

		private static HttpListener listener = new HttpListener();

		private static List<RouteAttribute> registeredRoutes;

		private static Queue<RequestContext> mainRequests = new Queue<RequestContext>();

		private ServerInitStatus _InitStatus;

		private static Dictionary<string, string> fileTypes = new Dictionary<string, string>
		{
			{ "js", "application/javascript" },
			{ "json", "application/json" },
			{ "jpg", "image/jpeg" },
			{ "jpeg", "image/jpeg" },
			{ "gif", "image/gif" },
			{ "png", "image/png" },
			{ "css", "text/css" },
			{ "htm", "text/html" },
			{ "html", "text/html" },
			{ "ico", "image/x-icon" }
		};

		public ServerInitStatus InitStatus
		{
			get
			{
				return _InitStatus;
			}
		}

		public Color32 ColorLogs
		{
			get
			{
				return Console.ColorLogs;
			}
			set
			{
				Console.ColorLogs = value;
			}
		}

		public Color32 ColorWarning
		{
			get
			{
				return Console.ColorWarning;
			}
			set
			{
				Console.ColorWarning = value;
			}
		}

		public Color32 ColorError
		{
			get
			{
				return Console.ColorErrors;
			}
			set
			{
				Console.ColorErrors = value;
			}
		}

		public static Server Init(int p_port)
		{
			GameObject gameObject = new GameObject("[CUDLR.Server]");
			Server server = gameObject.AddComponent<Server>();
			server.StartListener(p_port);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return server;
		}

		public void SetupEnabledLogTypes(HashSet<LogType> p_enabledLogTypes)
		{
			Console.EnabledLogTypes = p_enabledLogTypes;
		}

		private void StartListener(int p_port)
		{
			Port = GetFreePortInRange(p_port, 65535);
			if (Port == -1)
			{
				_InitStatus = ServerInitStatus.Error_NoFreePorts;
				return;
			}
			mainThread = System.Threading.Thread.CurrentThread;
			fileRoot = Path.Combine(Application.streamingAssetsPath, "CUDLR");
			RegisterRoutes();
			RegisterFileHandlers();
			listener.Prefixes.Add("http://*:" + Port + "/");
			listener.Start();
			listener.BeginGetContext(ListenerCallback, null);
			StartCoroutine(HandleRequests());
			_InitStatus = ServerInitStatus.Success;
		}

		private int GetFreePortInRange(int p_startPort, int p_endPort)
		{
			IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
			List<int> list = activeTcpListeners.Select((IPEndPoint p) => p.Port).ToList();
			IPEndPoint[] activeUdpListeners = iPGlobalProperties.GetActiveUdpListeners();
			List<int> list2 = activeUdpListeners.Select((IPEndPoint p) => p.Port).ToList();
			TcpConnectionInformation[] activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
			List<int> list3 = (from p in activeTcpConnections
				where p.State != TcpState.Closed
				select p.LocalEndPoint.Port).ToList();
			list3.AddRange(list.ToArray());
			list3.AddRange(list2.ToArray());
			int num = 0;
			for (num = p_startPort; num < p_endPort; num++)
			{
				if (!list3.Contains(num))
				{
					return num;
				}
			}
			return -1;
		}

		private void RegisterRoutes()
		{
			registeredRoutes = new List<RouteAttribute>();
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					RouteAttribute[] array = methodInfo.GetCustomAttributes(typeof(RouteAttribute), true) as RouteAttribute[];
					if (array.Length == 0)
					{
						continue;
					}
					RouteAttribute.Callback callback = Delegate.CreateDelegate(typeof(RouteAttribute.Callback), methodInfo, false) as RouteAttribute.Callback;
					if (callback == null)
					{
						Debug.LogError(string.Format("Method {0}.{1} takes the wrong arguments for a console route.", type, methodInfo.Name));
						continue;
					}
					RouteAttribute[] array2 = array;
					foreach (RouteAttribute routeAttribute in array2)
					{
						if (routeAttribute.m_route == null)
						{
							Debug.LogError(string.Format("Method {0}.{1} needs a valid route regexp.", type, methodInfo.Name));
							continue;
						}
						routeAttribute.m_callback = callback;
						registeredRoutes.Add(routeAttribute);
					}
				}
			}
		}

		private static void FindFileType(RequestContext context, bool download, out string path, out string type)
		{
			path = Path.Combine(fileRoot, context.match.Groups[1].Value);
			string key = Path.GetExtension(path).ToLower().TrimStart('.');
			if (download || !fileTypes.TryGetValue(key, out type))
			{
				type = "application/octet-stream";
			}
		}

		private static void WWWFileHandler(RequestContext context, bool download)
		{
			string path;
			string type;
			FindFileType(context, download, out path, out type);
			WWW wWW = new WWW(path);
			while (!wWW.isDone)
			{
				System.Threading.Thread.Sleep(0);
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				context.Response.ContentType = type;
				if (download)
				{
					context.Response.AddHeader("Content-disposition", string.Format("attachment; filename={0}", Path.GetFileName(path)));
				}
				context.Response.FJPANBOJJDI(wWW.bytes);
			}
			else if (wWW.error.StartsWith("Couldn't open file"))
			{
				context.pass = true;
			}
			else
			{
				context.Response.StatusCode = 500;
				context.Response.StatusDescription = string.Format("Fatal error:\n{0}", wWW.error);
			}
		}

		private static void FileHandler(RequestContext context, bool download)
		{
			string path;
			string type;
			FindFileType(context, download, out path, out type);
			if (File.Exists(path))
			{
				context.Response.CDAFFJPMLCG(path, type, download);
			}
			else
			{
				context.pass = true;
			}
		}

		private static void RegisterFileHandlers()
		{
			string arg = string.Format("({0})", string.Join("|", fileTypes.Select((KeyValuePair<string, string> x) => x.Key).ToArray()));
			RouteAttribute routeAttribute = new RouteAttribute(string.Format("^/download/(.*\\.{0})$", arg));
			RouteAttribute routeAttribute2 = new RouteAttribute(string.Format("^/(.*\\.{0})$", arg));
			bool flag = (routeAttribute2.m_runOnMainThread = (routeAttribute.m_runOnMainThread = fileRoot.Contains("://")));
			FileHandlerDelegate callback = FileHandler;
			if (flag)
			{
				callback = WWWFileHandler;
			}
			routeAttribute.m_callback = (RequestContext context) =>
			{
				callback(context, true);
			};
			routeAttribute2.m_callback = (RequestContext context) =>
			{
				callback(context, false);
			};
			registeredRoutes.Add(routeAttribute);
			registeredRoutes.Add(routeAttribute2);
		}

		private void OnEnable()
		{
			Application.logMessageReceived += Console.LogCallback;
		}

		private void OnDisable()
		{
			Application.logMessageReceived -= Console.LogCallback;
		}

		private void Update()
		{
			Console.Update();
		}

		private void ListenerCallback(IAsyncResult result)
		{
			RequestContext context = new RequestContext(listener.EndGetContext(result));
			HandleRequest(context);
			listener.BeginGetContext(ListenerCallback, null);
		}

		private void HandleRequest(RequestContext context)
		{
			try
			{
				bool flag = false;
				while (context.currentRoute < registeredRoutes.Count)
				{
					RouteAttribute routeAttribute = registeredRoutes[context.currentRoute];
					Match match = routeAttribute.m_route.Match(context.path);
					if (match.Success && routeAttribute.m_methods.IsMatch(context.Request.HttpMethod))
					{
						if (routeAttribute.m_runOnMainThread && System.Threading.Thread.CurrentThread != mainThread)
						{
							lock (mainRequests)
							{
								mainRequests.Enqueue(context);
								return;
							}
						}
						context.match = match;
						routeAttribute.m_callback(context);
						flag = !context.pass;
						if (flag)
						{
							break;
						}
					}
					context.currentRoute++;
				}
				if (!flag)
				{
					context.Response.StatusCode = 404;
					context.Response.StatusDescription = "Not Found";
				}
			}
			catch (Exception ex)
			{
				context.Response.StatusCode = 500;
				context.Response.StatusDescription = string.Format("Fatal error:\n{0}", ex);
				Debug.LogException(ex);
			}
			context.Response.OutputStream.Close();
		}

		private IEnumerator HandleRequests()
		{
			while (true)
			{
				if (mainRequests.Count == 0)
				{
					yield return new WaitForEndOfFrame();
					continue;
				}
				RequestContext context = null;
				lock (mainRequests)
				{
					context = mainRequests.Dequeue();
				}
				HandleRequest(context);
			}
		}
	}
}
