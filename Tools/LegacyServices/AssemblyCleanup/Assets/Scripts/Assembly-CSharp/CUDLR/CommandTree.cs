using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CUDLR
{
	internal class CommandTree
	{
		private Dictionary<string, CommandTree> m_subcommands;

		private CommandAttribute m_command;

		private static string[] emptyArgs = new string[0];

		public CommandTree()
		{
			m_subcommands = new Dictionary<string, CommandTree>();
		}

		public void Add(CommandAttribute cmd)
		{
			_add(cmd.m_command.ToLower().Split(' '), 0, cmd);
		}

		private void _add(string[] commands, int command_index, CommandAttribute cmd)
		{
			if (commands.Length == command_index)
			{
				m_command = cmd;
				return;
			}
			string key = commands[command_index];
			if (!m_subcommands.ContainsKey(key))
			{
				m_subcommands[key] = new CommandTree();
			}
			m_subcommands[key]._add(commands, command_index + 1, cmd);
		}

		public string Complete(string partialCommand)
		{
			return _complete(partialCommand.Split(' '), 0, string.Empty);
		}

		public string _complete(string[] partialCommand, int index, string result)
		{
			if (partialCommand.Length == index && m_command != null)
			{
				return result;
			}
			if (partialCommand.Length == index)
			{
				Console.LogCommand(result);
				foreach (string key in m_subcommands.Keys)
				{
					Console.Log(result + " " + key);
				}
				return result + " ";
			}
			if (partialCommand.Length == index + 1)
			{
				string text = partialCommand[index];
				if (m_subcommands.ContainsKey(text))
				{
					result += text;
					return m_subcommands[text]._complete(partialCommand, index + 1, result);
				}
				List<string> list = new List<string>();
				foreach (string key2 in m_subcommands.Keys)
				{
					if (key2.StartsWith(text))
					{
						list.Add(key2);
					}
				}
				if (list.Count == 1)
				{
					return result + list[0] + " ";
				}
				if (list.Count > 1)
				{
					Console.LogCommand(result + text);
					foreach (string item in list)
					{
						Console.Log(result + item);
					}
				}
				return result + text;
			}
			string text2 = partialCommand[index];
			if (!m_subcommands.ContainsKey(text2))
			{
				return result;
			}
			result = result + text2 + " ";
			return m_subcommands[text2]._complete(partialCommand, index + 1, result);
		}

		public void Run(string commandStr)
		{
			Regex regex = new Regex("\\<[^}]+\\>|\\S+");
			MatchCollection matchCollection = regex.Matches(commandStr);
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = matchCollection[i].Value;
			}
			_run(array, 0);
		}

		private void _run(string[] commands, int index)
		{
			if (commands.Length == index)
			{
				RunCommand(emptyArgs);
				return;
			}
			string key = commands[index].ToLower();
			if (!m_subcommands.ContainsKey(key))
			{
				RunCommand(commands.Skip(index).ToArray());
			}
			else
			{
				m_subcommands[key]._run(commands, index + 1);
			}
		}

		private void RunCommand(string[] args)
		{
			if (m_command == null)
			{
				Console.Log("command not found");
			}
			else if (m_command.m_runOnMainThread)
			{
				Console.Queue(m_command, args);
			}
			else
			{
				m_command.m_callback(args);
			}
		}
	}
}
