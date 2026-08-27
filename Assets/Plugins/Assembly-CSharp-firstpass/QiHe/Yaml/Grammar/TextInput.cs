using System.Collections.Generic;

namespace QiHe.Yaml.Grammar
{
	public class TextInput : ParserInput<char>
	{
		private string InputText;

		private List<int> LineBreaks;

		public int Length
		{
			get
			{
				return InputText.Length;
			}
		}

		public TextInput(string text)
		{
			InputText = text;
			LineBreaks = new List<int>();
			LineBreaks.Add(0);
			for (int i = 0; i < InputText.Length; i++)
			{
				if (InputText[i] == '\n')
				{
					LineBreaks.Add(i + 1);
				}
			}
			LineBreaks.Add(InputText.Length);
		}

		public bool HasInput(int pos)
		{
			return pos < InputText.Length;
		}

		public char GetInputSymbol(int pos)
		{
			return InputText[pos];
		}

		public char[] GetSubSection(int position, int length)
		{
			return InputText.Substring(position, length).ToCharArray();
		}

		public string FormErrorMessage(int position, string message)
		{
			int line;
			int col;
			GetLineColumnNumber(position, out line, out col);
			string text = ((!HasInput(position)) ? null : ("'" + GetInputSymbol(position) + "'"));
			return string.Format("Line {0}, Col {1} {2}: {3}", line, col, text, message);
		}

		public void GetLineColumnNumber(int pos, out int line, out int col)
		{
			col = 1;
			for (line = 1; line < LineBreaks.Count; line++)
			{
				if (LineBreaks[line] > pos)
				{
					for (int i = LineBreaks[line - 1]; i < pos; i++)
					{
						if (InputText[i] == '\t')
						{
							col += 4;
						}
						else
						{
							col++;
						}
					}
					break;
				}
			}
		}

		public string GetSubString(int start, int length)
		{
			return InputText.Substring(start, length);
		}
	}
}
