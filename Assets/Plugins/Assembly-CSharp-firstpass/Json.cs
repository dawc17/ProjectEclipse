using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class Json
{
	private const int CGPBDIPPGAM = 0;

	private const int PCJCEOFFCCP = 1;

	private const int OPJLGINFAMB = 2;

	private const int PBEKCMMCPPD = 3;

	private const int MJIFIAINJGI = 4;

	private const int OAEAEEMMBHF = 5;

	private const int OPKIMEIIBEN = 6;

	private const int HGHKDLLNDLJ = 7;

	private const int MDOJALGHBGN = 8;

	private const int EIHBGKAOGAK = 9;

	private const int JMKAIOGMEHO = 10;

	private const int GCKKHHGCOFO = 11;

	private const int HNPKHPLBEGG = 2000;

	public static object Decode(string EMDHMHOKGFP)
	{
		bool IBFAPIMOMBA = true;
		return Decode(EMDHMHOKGFP, ref IBFAPIMOMBA);
	}

	public static object Decode(string EMDHMHOKGFP, ref bool IBFAPIMOMBA)
	{
		IBFAPIMOMBA = true;
		if (EMDHMHOKGFP != null)
		{
			char[] eMDHMHOKGFP = EMDHMHOKGFP.ToCharArray();
			int index = 0;
			return ParseValue(eMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
		}
		return null;
	}

	public static string Encode(object EMDHMHOKGFP)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		return (!SerializeValue(EMDHMHOKGFP, stringBuilder)) ? null : stringBuilder.ToString();
	}

	protected static Dictionary<string, object> ParseObject(char[] EMDHMHOKGFP, ref int index, ref bool IBFAPIMOMBA)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		NextToken(EMDHMHOKGFP, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (IHEOHKNJHIG(EMDHMHOKGFP, index))
			{
			case 0:
				IBFAPIMOMBA = false;
				return null;
			case 6:
				NextToken(EMDHMHOKGFP, ref index);
				continue;
			case 2:
				NextToken(EMDHMHOKGFP, ref index);
				return dictionary;
			}
			string key = ParseString(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
			if (!IBFAPIMOMBA)
			{
				IBFAPIMOMBA = false;
				return null;
			}
			int num = NextToken(EMDHMHOKGFP, ref index);
			if (num != 5)
			{
				IBFAPIMOMBA = false;
				return null;
			}
			object value = ParseValue(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
			if (!IBFAPIMOMBA)
			{
				IBFAPIMOMBA = false;
				return null;
			}
			dictionary[key] = value;
		}
		return dictionary;
	}

	protected static List<object> ParseArray(char[] EMDHMHOKGFP, ref int index, ref bool IBFAPIMOMBA)
	{
		List<object> list = new List<object>();
		NextToken(EMDHMHOKGFP, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (IHEOHKNJHIG(EMDHMHOKGFP, index))
			{
			case 0:
				IBFAPIMOMBA = false;
				return null;
			case 6:
				NextToken(EMDHMHOKGFP, ref index);
				continue;
			case 4:
				break;
			default:
			{
				object item = ParseValue(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
				if (!IBFAPIMOMBA)
				{
					return null;
				}
				list.Add(item);
				continue;
			}
			}
			NextToken(EMDHMHOKGFP, ref index);
			break;
		}
		return list;
	}

	protected static object ParseValue(char[] EMDHMHOKGFP, ref int index, ref bool IBFAPIMOMBA)
	{
		switch (IHEOHKNJHIG(EMDHMHOKGFP, index))
		{
		case 7:
			return ParseString(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
		case 8:
			return ParseNumber(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
		case 1:
			return ParseObject(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
		case 3:
			return ParseArray(EMDHMHOKGFP, ref index, ref IBFAPIMOMBA);
		case 9:
			NextToken(EMDHMHOKGFP, ref index);
			return true;
		case 10:
			NextToken(EMDHMHOKGFP, ref index);
			return false;
		case 11:
			NextToken(EMDHMHOKGFP, ref index);
			return null;
		default:
			IBFAPIMOMBA = false;
			return null;
		}
	}

	protected static string ParseString(char[] EMDHMHOKGFP, ref int index, ref bool IBFAPIMOMBA)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		EatWhitespace(EMDHMHOKGFP, ref index);
		char c = EMDHMHOKGFP[index++];
		bool flag = false;
		while (!flag && index != EMDHMHOKGFP.Length)
		{
			c = EMDHMHOKGFP[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
			{
				if (index == EMDHMHOKGFP.Length)
				{
					break;
				}
				switch (EMDHMHOKGFP[index++])
				{
				case '"':
					stringBuilder.Append('"');
					continue;
				case '\\':
					stringBuilder.Append('\\');
					continue;
				case '/':
					stringBuilder.Append('/');
					continue;
				case 'b':
					stringBuilder.Append('\b');
					continue;
				case 'f':
					stringBuilder.Append('\f');
					continue;
				case 'n':
					stringBuilder.Append('\n');
					continue;
				case 'r':
					stringBuilder.Append('\r');
					continue;
				case 't':
					stringBuilder.Append('\t');
					continue;
				case 'u':
					break;
				default:
					continue;
				}
				int num = EMDHMHOKGFP.Length - index;
				if (num < 4)
				{
					break;
				}
				uint result;
				if (!(IBFAPIMOMBA = uint.TryParse(new string(EMDHMHOKGFP, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result)))
				{
					return string.Empty;
				}
				stringBuilder.Append(char.ConvertFromUtf32((int)result));
				index += 4;
				continue;
			}
			default:
				stringBuilder.Append(c);
				continue;
			}
			break;
		}
		if (!flag)
		{
			IBFAPIMOMBA = false;
			return null;
		}
		return stringBuilder.ToString();
	}

	protected static double ParseNumber(char[] EMDHMHOKGFP, ref int index, ref bool IBFAPIMOMBA)
	{
		EatWhitespace(EMDHMHOKGFP, ref index);
		int num = JHFHJJGFENL(EMDHMHOKGFP, index);
		int length = num - index + 1;
		double result;
		IBFAPIMOMBA = double.TryParse(new string(EMDHMHOKGFP, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		index = num + 1;
		return result;
	}

	protected static int JHFHJJGFENL(char[] EMDHMHOKGFP, int index)
	{
		int i;
		for (i = index; i < EMDHMHOKGFP.Length && "0123456789+-.eE".IndexOf(EMDHMHOKGFP[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	protected static void EatWhitespace(char[] EMDHMHOKGFP, ref int index)
	{
		while (index < EMDHMHOKGFP.Length && " \t\n\r".IndexOf(EMDHMHOKGFP[index]) != -1)
		{
			index++;
		}
	}

	protected static int IHEOHKNJHIG(char[] EMDHMHOKGFP, int index)
	{
		int IHPMGHJPLBP2 = index;
		return NextToken(EMDHMHOKGFP, ref IHPMGHJPLBP2);
	}

	protected static int NextToken(char[] EMDHMHOKGFP, ref int index)
	{
		EatWhitespace(EMDHMHOKGFP, ref index);
		if (index == EMDHMHOKGFP.Length)
		{
			return 0;
		}
		char c = EMDHMHOKGFP[index];
		index++;
		switch (c)
		{
		case '{':
			return 1;
		case '}':
			return 2;
		case '[':
			return 3;
		case ']':
			return 4;
		case ',':
			return 6;
		case '"':
			return 7;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return 8;
		case ':':
			return 5;
		default:
		{
			index--;
			int num = EMDHMHOKGFP.Length - index;
			if (num >= 5 && EMDHMHOKGFP[index] == 'f' && EMDHMHOKGFP[index + 1] == 'a' && EMDHMHOKGFP[index + 2] == 'l' && EMDHMHOKGFP[index + 3] == 's' && EMDHMHOKGFP[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && EMDHMHOKGFP[index] == 't' && EMDHMHOKGFP[index + 1] == 'r' && EMDHMHOKGFP[index + 2] == 'u' && EMDHMHOKGFP[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && EMDHMHOKGFP[index] == 'n' && EMDHMHOKGFP[index + 1] == 'u' && EMDHMHOKGFP[index + 2] == 'l' && EMDHMHOKGFP[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}
		}
	}

	protected static bool SerializeValue(object value, StringBuilder builder)
	{
		bool result = true;
		if (value is string)
		{
			result = SerializeString((string)value, builder);
		}
		else if (value is IDictionary)
		{
			result = SerializeObject((IDictionary)value, builder);
		}
		else if (value is IList)
		{
			result = SerializeArray(value as IList, builder);
		}
		else if (value is bool && (bool)value)
		{
			builder.Append("true");
		}
		else if (value is bool && !(bool)value)
		{
			builder.Append("false");
		}
		else if (value is ValueType)
		{
			result = SerializeNumber(Convert.ToDouble(value), builder);
		}
		else if (value == null)
		{
			builder.Append("null");
		}
		else
		{
			result = false;
		}
		return result;
	}

	protected static bool SerializeObject(IDictionary LJAFGJIEFDK, StringBuilder builder)
	{
		builder.Append("{");
		IDictionaryEnumerator enumerator = LJAFGJIEFDK.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			string jMOLGHDKNME = enumerator.Key.ToString();
			object value = enumerator.Value;
			if (!flag)
			{
				builder.Append(", ");
			}
			SerializeString(jMOLGHDKNME, builder);
			builder.Append(":");
			if (!SerializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool SerializeArray(IList FGPLKJMKKBP, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		for (int i = 0; i < FGPLKJMKKBP.Count; i++)
		{
			object bAINMLLIKOL = FGPLKJMKKBP[i];
			if (!flag)
			{
				builder.Append(", ");
			}
			if (!SerializeValue(bAINMLLIKOL, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	protected static bool SerializeString(string JMOLGHDKNME, StringBuilder builder)
	{
		builder.Append("\"");
		char[] array = JMOLGHDKNME.ToCharArray();
		foreach (char c in array)
		{
			switch (c)
			{
			case '"':
				builder.Append("\\\"");
				continue;
			case '\\':
				builder.Append("\\\\");
				continue;
			case '\b':
				builder.Append("\\b");
				continue;
			case '\f':
				builder.Append("\\f");
				continue;
			case '\n':
				builder.Append("\\n");
				continue;
			case '\r':
				builder.Append("\\r");
				continue;
			case '\t':
				builder.Append("\\t");
				continue;
			}
			int num = Convert.ToInt32(c);
			if (num >= 32 && num <= 126)
			{
				builder.Append(c);
			}
			else
			{
				builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
			}
		}
		builder.Append("\"");
		return true;
	}

	protected static bool SerializeNumber(double number, StringBuilder builder)
	{
		builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
		return true;
	}
}
