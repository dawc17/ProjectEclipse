using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CUDLR
{
	public class Console
	{
		public struct ConsoleMessage
		{
			public string message;

			public Color32 color;

			public ConsoleMessage(string message)
			{
				this.message = message;
				color = new Color32(240, 240, 240, byte.MaxValue);
			}

			public ConsoleMessage(string message, Color32 color)
			{
				this.message = message;
				this.color = color;
			}
		}

		private const int MAX_LINES = 1000;

		private const int MAX_HISTORY = 50;

		private const string COMMAND_OUTPUT_PREFIX = "> ";

		public static Color32 ColorLogs = new Color32(240, 240, 240, byte.MaxValue);

		public static Color32 ColorWarning = new Color32(254, 239, 179, byte.MaxValue);

		public static Color32 ColorErrors = new Color32(byte.MaxValue, 186, 186, byte.MaxValue);

		public static HashSet<LogType> EnabledLogTypes = new HashSet<LogType>
		{
			LogType.Log,
			LogType.Warning,
			LogType.Exception,
			LogType.Assert,
			LogType.Error
		};

		private static Console instance;

		private CommandTree m_commands;

		private List<ConsoleMessage> m_output;

		private List<string> m_history;

		private string m_help;

		private Queue<QueuedCommand> m_commandQueue;

		public List<ConsoleMessage> ConsoleOutput
		{
			get
			{
				return m_output;
			}
		}

		public string HelpData
		{
			get
			{
				return m_help;
			}
		}

		public static Console Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Console();
				}
				return instance;
			}
		}

		private Console()
		{
			m_commands = new CommandTree();
			m_output = new List<ConsoleMessage>();
			m_history = new List<string>();
			m_commandQueue = new Queue<QueuedCommand>();
			RegisterAttributes();
		}

		public static void Update()
		{
			while (Instance.m_commandQueue.Count > 0)
			{
				QueuedCommand queuedCommand = Instance.m_commandQueue.Dequeue();
				queuedCommand.command.m_callback(queuedCommand.args);
			}
		}

		public static void Queue(CommandAttribute command, string[] args)
		{
			QueuedCommand item = new QueuedCommand
			{
				command = command,
				args = args
			};
			Instance.m_commandQueue.Enqueue(item);
		}

		public static void Run(string str)
		{
			if (str.Length > 0)
			{
				LogCommand(str);
				Instance.RecordCommand(str);
				Instance.m_commands.Run(str);
			}
		}

		public static string Complete(string partialCommand)
		{
			return Instance.m_commands.Complete(partialCommand);
		}

		public static void LogCommand(string cmd)
		{
			Log("> " + cmd);
		}

		public static void Log(string str)
		{
			Log(str, ColorLogs);
		}

		public static void Log(string str, Color32 color)
		{
			Instance.m_output.Add(new ConsoleMessage(str, color));
			if (Instance.m_output.Count > 1000)
			{
				Instance.m_output.RemoveAt(0);
			}
		}

		public static void LogCallback(string logString, string stackTrace, LogType type)
		{
			if (EnabledLogTypes.Contains(type))
			{
				Log(logString, GetLogColor(type));
				if (type != LogType.Log)
				{
					Log(stackTrace);
				}
			}
		}

		private static Color32 GetLogColor(LogType p_type)
		{
			switch (p_type)
			{
			case LogType.Warning:
				return ColorWarning;
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				return ColorErrors;
			default:
				return ColorLogs;
			}
		}

		public static string Output()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ConsoleMessage item in Instance.m_output)
			{
				string value = HtmlEncode(item.message);
				stringBuilder.Append("<div style='color:#" + item.color.r.ToString("X2") + item.color.g.ToString("X2") + item.color.b.ToString("X2") + ";'>");
				stringBuilder.Append(value);
				stringBuilder.Append("</div>");
			}
			return stringBuilder.ToString();
		}

		private static string HtmlEncode(string text)
		{
			if (text == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '<':
					stringBuilder.Append("&lt;");
					break;
				case '>':
					stringBuilder.Append("&gt;");
					break;
				case '"':
					stringBuilder.Append("&quot;");
					break;
				case '&':
					stringBuilder.Append("&amp;");
					break;
				default:
					stringBuilder.Append(text[i]);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		public static void RegisterCommand(string command, string desc, CommandAttribute.Callback callback, bool runOnMainThread = true)
		{
			if (command == null || command.Length == 0)
			{
				throw new Exception("Command String cannot be empty");
			}
			CommandAttribute commandAttribute = new CommandAttribute(command, desc, runOnMainThread);
			commandAttribute.m_callback = callback;
			Instance.m_commands.Add(commandAttribute);
			Instance.m_help += string.Format("\n{0}", desc);
		}

		private void RegisterAttributes()
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					CommandAttribute[] array = methodInfo.GetCustomAttributes(typeof(CommandAttribute), true) as CommandAttribute[];
					if (array.Length == 0)
					{
						continue;
					}
					CommandAttribute.Callback callback = Delegate.CreateDelegate(typeof(CommandAttribute.Callback), methodInfo, false) as CommandAttribute.Callback;
					if (callback == null)
					{
						CommandAttribute.CallbackSimple cbs = Delegate.CreateDelegate(typeof(CommandAttribute.CallbackSimple), methodInfo, false) as CommandAttribute.CallbackSimple;
						if (cbs != null)
						{
							callback = (string[] args) =>
							{
								cbs();
							};
						}
					}
					if (callback == null)
					{
						Debug.LogError(string.Format("Method {0}.{1} takes the wrong arguments for a console command.", type, methodInfo.Name));
						continue;
					}
					CommandAttribute[] array2 = array;
					foreach (CommandAttribute commandAttribute in array2)
					{
						if (string.IsNullOrEmpty(commandAttribute.m_command))
						{
							Debug.LogError(string.Format("Method {0}.{1} needs a valid command name.", type, methodInfo.Name));
							continue;
						}
						commandAttribute.m_callback = callback;
						m_commands.Add(commandAttribute);
						m_help += string.Format("\n{0}", commandAttribute.m_help);
					}
				}
			}
		}

		public static string PreviousCommand(int index)
		{
			return (index < 0 || index >= Instance.m_history.Count) ? null : Instance.m_history[index];
		}

		private void RecordCommand(string command)
		{
			m_history.Insert(0, command);
			if (m_history.Count > 50)
			{
				m_history.RemoveAt(m_history.Count - 1);
			}
		}

		[Route("^/console/out$", "(GET|HEAD)", true)]
		public static void Output(RequestContext context)
		{
			context.Response.IHOOAEHGMFO(Output());
		}

		[Route("^/console/run$", "(GET|HEAD)", true)]
		public static void Run(RequestContext context)
		{
			string text = Uri.UnescapeDataString(context.Request.QueryString.Get("command"));
			if (!string.IsNullOrEmpty(text))
			{
				Run(text);
			}
			context.Response.StatusCode = 200;
			context.Response.StatusDescription = "OK";
		}

		[Route("^/console/commandHistory$", "(GET|HEAD)", true)]
		public static void History(RequestContext context)
		{
			string text = context.Request.QueryString.Get("index");
			string nILNDHEKNLJ = null;
			if (!string.IsNullOrEmpty(text))
			{
				nILNDHEKNLJ = PreviousCommand(int.Parse(text));
			}
			context.Response.IHOOAEHGMFO(nILNDHEKNLJ);
		}

		[Route("^/console/complete$", "(GET|HEAD)", true)]
		public static void Complete(RequestContext context)
		{
			string text = context.Request.QueryString.Get("command");
			string nILNDHEKNLJ = null;
			if (text != null)
			{
				nILNDHEKNLJ = Complete(text);
			}
			context.Response.IHOOAEHGMFO(nILNDHEKNLJ);
		}
	}
}
