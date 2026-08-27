using System;
using System.Collections.Generic;
using System.Linq;

public class CommandLineReader
{
	private const string CUSTOM_ARGS_PREFIX = "-CustomArgs:";

	private const char CUSTOM_ARGS_SEPARATOR = ';';

	public static string[] BCFGNHKHDIC()
	{
		return Environment.GetCommandLineArgs();
	}

	public static string FFAPOOKOGDJ()
	{
		string[] array = BCFGNHKHDIC();
		if (array.Length > 0)
		{
			return string.Join(" ", array);
		}
		AdvLog.CCOFFJPPAKC("CommandLineReader.cs - GetCommandLine() - Can't find any command line arguments!");
		return string.Empty;
	}

	public static Dictionary<string, string> MGCECOEGDFG()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = BCFGNHKHDIC();
		string empty = string.Empty;
		try
		{
			empty = array.Where((string IBAKGENOEPH) => IBAKGENOEPH.Contains("-CustomArgs:")).Single();
		}
		catch (Exception ex)
		{
			AdvLog.CCOFFJPPAKC(string.Concat("CommandLineReader.cs - GetCustomArguments() - Can't retrieve any custom arguments in the command line [", array, "]. Exception: ", ex));
			return dictionary;
		}
		empty = empty.Replace("-CustomArgs:", string.Empty);
		string[] array2 = empty.Split(';');
		string[] array3 = array2;
		foreach (string text in array3)
		{
			string[] array4 = text.Split('=');
			if (array4.Length == 2)
			{
				dictionary.Add(array4[0], array4[1]);
			}
			else
			{
				AdvLog.LOPHFKMOPAA("CommandLineReader.cs - GetCustomArguments() - The custom argument [" + text + "] seem to be malformed.");
			}
		}
		return dictionary;
	}

	public static string GetCustomArgument(string CDCEKJEPOAK)
	{
		Dictionary<string, string> dictionary = MGCECOEGDFG();
		if (dictionary.ContainsKey(CDCEKJEPOAK))
		{
			return dictionary[CDCEKJEPOAK];
		}
		AdvLog.CCOFFJPPAKC("CommandLineReader.cs - GetCustomArgument() - Can't retrieve any custom argument named [" + CDCEKJEPOAK + "] in the command line [" + FFAPOOKOGDJ() + "].");
		return string.Empty;
	}
}
