using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QiHe.CodeLib
{
	public class StringHelper
	{
		public static string[] SplitWithSeparator(string text, char sep)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string[] array = new string[2];
			int num = text.IndexOf(sep);
			if (num >= 0)
			{
				array[0] = text.Substring(0, num);
				array[1] = text.Substring(num + 1);
			}
			else
			{
				array[0] = text;
				array[1] = null;
			}
			return array;
		}

		public static string[] SplitWithSeparatorFromRight(string text, char sep)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string[] array = new string[2];
			int num = text.LastIndexOf(sep);
			if (num >= 0)
			{
				array[0] = text.Substring(0, num);
				array[1] = text.Substring(num + 1);
			}
			else
			{
				array[0] = text;
				array[1] = null;
			}
			return array;
		}

		public static string[] SplitWithSpaces(string text)
		{
			string pattern = "[^ ]+";
			Regex regex = new Regex(pattern);
			MatchCollection matchCollection = regex.Matches(text);
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = matchCollection[i].Value;
			}
			return array;
		}

		public static string[] SplitIntoLines(string text)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in text)
			{
				switch (c)
				{
				case '\n':
					list.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
					break;
				default:
					stringBuilder.Append(c);
					break;
				case '\r':
					break;
				}
			}
			if (stringBuilder.Length > 0)
			{
				list.Add(stringBuilder.ToString());
			}
			return list.ToArray();
		}

		public static string AddWithDelim(string s1, string delim, string s2)
		{
			if (string.IsNullOrEmpty(s1))
			{
				return s2;
			}
			return s1 + delim + s2;
		}

		public static string ContactWithDelim(string str1, string delim, string str2)
		{
			if (string.IsNullOrEmpty(str1))
			{
				return str2;
			}
			if (string.IsNullOrEmpty(str2))
			{
				return str1;
			}
			return str1 + delim + str2;
		}

		public static string ContactWithDelimSkipEmpty(IEnumerable<string> items, string delim)
		{
			return ContactWithDelimSkipSome(items, delim, string.Empty);
		}

		public static string ContactWithDelimSkipNull(IEnumerable<string> items, string delim)
		{
			return ContactWithDelimSkipSome(items, delim, null);
		}

		public static string ContactWithDelimSkipSome(IEnumerable<string> items, string delim, string skip)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string item in items)
			{
				if (!(item == skip))
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(delim);
					}
					stringBuilder.Append(item);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ContactWithDelim(IEnumerable<string> items, string delim)
		{
			return ContactWithDelim(items, delim, null, null);
		}

		public static string ContactWithDelim(IEnumerable<string> items, string delim, string initialValue)
		{
			return ContactWithDelim(items, delim, initialValue, null);
		}

		public static string ContactWithDelim(IEnumerable<string> items, string delim, string initialValue, string endValue)
		{
			StringBuilder stringBuilder = new StringBuilder(initialValue);
			bool flag = true;
			foreach (string item in items)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(delim);
				}
				stringBuilder.Append(item);
			}
			stringBuilder.Append(endValue);
			return stringBuilder.ToString();
		}

		public static string ContactWithDelim<T>(IEnumerable<T> items, string delim)
		{
			return ContactWithDelim(items, delim, null, null);
		}

		public static string ContactWithDelim<T>(IEnumerable<T> items, string delim, string initialValue, string endValue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(initialValue);
			bool flag = true;
			foreach (T item in items)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(delim);
				}
				stringBuilder.Append(item.ToString());
			}
			stringBuilder.Append(endValue);
			return stringBuilder.ToString();
		}

		public static string GetHeader(string text, int length)
		{
			if (text.Length <= length)
			{
				return text;
			}
			return text.Substring(0, length);
		}

		public static string GetSubStringBetween(string text, char bra, char ket)
		{
			if (text == null)
			{
				return null;
			}
			int num = text.IndexOf(bra);
			if (num > -1)
			{
				int num2 = text.IndexOf(ket);
				if (num2 > num)
				{
					return text.Substring(num + 1, num2 - num - 1);
				}
			}
			return string.Empty;
		}

		public static string GetLastSubStringBetween(string text, char bra, char ket)
		{
			if (text == null)
			{
				return null;
			}
			int num = text.LastIndexOf(bra);
			if (num > -1)
			{
				int num2 = text.LastIndexOf(ket);
				if (num2 > num)
				{
					return text.Substring(num + 1, num2 - num - 1);
				}
			}
			return string.Empty;
		}

		public static bool StartWithUpperCase(string name)
		{
			return name.Length >= 1 && char.IsUpper(name[0]);
		}

		public static string UpperCaseFirstChar(string name)
		{
			if (name.Length >= 1 && char.IsLower(name[0]))
			{
				char[] array = name.ToCharArray();
				array[0] = char.ToUpper(array[0]);
				return new string(array);
			}
			return name;
		}

		public static string LowerCaseFirstChar(string name)
		{
			if (name.Length >= 1 && char.IsUpper(name[0]))
			{
				char[] array = name.ToCharArray();
				array[0] = char.ToLower(array[0]);
				return new string(array);
			}
			return name;
		}

		public static string[] GetWords(string text)
		{
			List<string> list = new List<string>();
			List<char> list2 = new List<char>();
			foreach (char c in text)
			{
				switch (c)
				{
				case '\n':
				case '\r':
				case ' ':
					if (list2.Count > 0)
					{
						list.Add(new string(list2.ToArray()));
						list2.Clear();
					}
					break;
				default:
					list2.Add(c);
					break;
				}
			}
			if (list2.Count > 0)
			{
				list.Add(new string(list2.ToArray()));
			}
			return list.ToArray();
		}

		public static Dictionary<string, int> CountWordFrequency(string article, string delimiters)
		{
			if (article == null)
			{
				throw new ArgumentNullException("article");
			}
			if (delimiters == null)
			{
				throw new ArgumentNullException("delimiters");
			}
			List<string> list = new List<string>();
			List<char> list2 = new List<char>();
			foreach (char c in article)
			{
				if (delimiters.IndexOf(c) == -1)
				{
					list2.Add(c);
				}
				else if (list2.Count > 0)
				{
					list.Add(new string(list2.ToArray()));
					list2.Clear();
				}
			}
			if (list2.Count > 0)
			{
				list.Add(new string(list2.ToArray()));
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (string item in list)
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item]++;
				}
				else
				{
					dictionary.Add(item, 1);
				}
			}
			return dictionary;
		}
	}
}
