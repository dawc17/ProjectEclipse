using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using QiHe.CodeLib;
using UnityEngine;

namespace QiHe.Yaml.Grammar
{
	public class YamlParser
	{
		private YamlDocument currentDocument;

		private int currentIndent = -1;

		private bool detectIndent;

		private Stack<int> Indents = new Stack<int>();

		private ChompingMethod CurrentChompingMethod;

		private int position;

		private ParserInput<char> Input;

		public List<Pair<int, string>> Errors = new List<Pair<int, string>>();

		private Stack<int> ErrorStatck = new Stack<int>();

		public int Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public static YamlStream Load(string file)
		{
			string text = File.ReadAllText(file);
			TextInput input = new TextInput(text);
			YamlParser yamlParser = new YamlParser();
			bool success;
			YamlStream result = yamlParser.ParseYamlStream(input, out success);
			if (success)
			{
				return result;
			}
			string eorrorMessages = yamlParser.GetEorrorMessages();
			throw new Exception(eorrorMessages);
		}

		public static YamlStream FromContent(string content)
		{
			TextInput input = new TextInput(content);
			YamlParser yamlParser = new YamlParser();
			bool success;
			YamlStream result = yamlParser.ParseYamlStream(input, out success);
			if (success)
			{
				return result;
			}
			string eorrorMessages = yamlParser.GetEorrorMessages();
			throw new Exception(eorrorMessages);
		}

		public static YamlStream Load(TextAsset content)
		{
			string text = content.text;
			TextInput input = new TextInput(text);
			YamlParser yamlParser = new YamlParser();
			bool success;
			YamlStream result = yamlParser.ParseYamlStream(input, out success);
			if (success)
			{
				return result;
			}
			string eorrorMessages = yamlParser.GetEorrorMessages();
			throw new Exception(eorrorMessages);
		}

		private void SetDataItemProperty(DataItem dataItem, NodeProperty property)
		{
			if (property.Anchor != null)
			{
				currentDocument.AnchoredItems[property.Anchor] = dataItem;
			}
			dataItem.Property = property;
		}

		private DataItem GetAnchoredDataItem(string name)
		{
			if (currentDocument.AnchoredItems.ContainsKey(name))
			{
				return currentDocument.AnchoredItems[name];
			}
			Error(name + " is not anchored.");
			return null;
		}

		private bool ParseIndent()
		{
			bool success;
			for (int i = 0; i < currentIndent; i++)
			{
				MatchTerminal(' ', out success);
				if (!success)
				{
					position -= i;
					return false;
				}
			}
			if (detectIndent)
			{
				int num = 0;
				while (true)
				{
					MatchTerminal(' ', out success);
					if (success)
					{
						num++;
						continue;
					}
					break;
				}
				currentIndent += num;
				detectIndent = false;
			}
			return true;
		}

		private void IncreaseIndentIfZero()
		{
			Indents.Push(currentIndent);
			if (currentIndent == 0)
			{
				currentIndent++;
			}
			detectIndent = true;
		}

		private void IncreaseIndent()
		{
			Indents.Push(currentIndent);
			currentIndent++;
			detectIndent = true;
		}

		private void DecreaseIndent()
		{
			currentIndent = Indents.Pop();
		}

		private void RememberIndent()
		{
			Indents.Push(currentIndent);
		}

		private void RestoreIndent()
		{
			currentIndent = Indents.Pop();
		}

		private void AddIndent(BlockScalarModifier modifier, bool success)
		{
			if (success)
			{
				Indents.Push(currentIndent);
				currentIndent += modifier.GetIndent();
				detectIndent = true;
			}
			else
			{
				IncreaseIndentIfZero();
			}
			CurrentChompingMethod = modifier.GetChompingMethod();
		}

		private string Chomp(string linebreaks)
		{
			switch (CurrentChompingMethod)
			{
			case ChompingMethod.Strip:
				return string.Empty;
			case ChompingMethod.Keep:
				return linebreaks;
			default:
				if (linebreaks.StartsWith("\r\n"))
				{
					return "\r\n";
				}
				if (linebreaks.Length == 0)
				{
					return Environment.NewLine;
				}
				return linebreaks.Substring(0, 1);
			}
		}

		private void SetInput(ParserInput<char> input)
		{
			Input = input;
			position = 0;
		}

		private bool TerminalMatch(char terminal)
		{
			if (Input.HasInput(position))
			{
				char inputSymbol = Input.GetInputSymbol(position);
				return terminal == inputSymbol;
			}
			return false;
		}

		private bool TerminalMatch(char terminal, int pos)
		{
			if (Input.HasInput(pos))
			{
				char inputSymbol = Input.GetInputSymbol(pos);
				return terminal == inputSymbol;
			}
			return false;
		}

		private char MatchTerminal(char terminal, out bool success)
		{
			success = false;
			if (Input.HasInput(position))
			{
				char inputSymbol = Input.GetInputSymbol(position);
				if (terminal == inputSymbol)
				{
					position++;
					success = true;
				}
				return inputSymbol;
			}
			return '\0';
		}

		private char MatchTerminalRange(char start, char end, out bool success)
		{
			success = false;
			if (Input.HasInput(position))
			{
				char inputSymbol = Input.GetInputSymbol(position);
				if (start <= inputSymbol && inputSymbol <= end)
				{
					position++;
					success = true;
				}
				return inputSymbol;
			}
			return '\0';
		}

		private char MatchTerminalSet(string terminalSet, bool isComplement, out bool success)
		{
			success = false;
			if (Input.HasInput(position))
			{
				char inputSymbol = Input.GetInputSymbol(position);
				if ((!isComplement) ? (terminalSet.IndexOf(inputSymbol) > -1) : (terminalSet.IndexOf(inputSymbol) == -1))
				{
					position++;
					success = true;
				}
				return inputSymbol;
			}
			return '\0';
		}

		private string MatchTerminalString(string terminalString, out bool success)
		{
			int num = position;
			foreach (char terminal in terminalString)
			{
				MatchTerminal(terminal, out success);
				if (!success)
				{
					position = num;
					return null;
				}
			}
			success = true;
			return terminalString;
		}

		private int Error(string message)
		{
			Errors.Add(new Pair<int, string>(position, message));
			return Errors.Count;
		}

		private void ClearError(int count)
		{
			Errors.RemoveRange(count, Errors.Count - count);
		}

		public string GetEorrorMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pair<int, string> error in Errors)
			{
				stringBuilder.Append(Input.FormErrorMessage(error.Left, error.Right));
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public YamlStream ParseYamlStream(ParserInput<char> input, out bool success)
		{
			SetInput(input);
			YamlStream result = ParseYamlStream(out success);
			if (Position < input.Length)
			{
				success = false;
				Error("Failed to parse remained input.");
			}
			return result;
		}

		private YamlStream ParseYamlStream(out bool success)
		{
			int count = Errors.Count;
			YamlStream yamlStream = new YamlStream();
			int num = position;
			do
			{
				ParseComment(out success);
			}
			while (success);
			success = true;
			YamlDocument item = ParseImplicitDocument(out success);
			if (success)
			{
				yamlStream.Documents.Add(item);
			}
			success = true;
			while (true)
			{
				item = ParseExplicitDocument(out success);
				if (success)
				{
					yamlStream.Documents.Add(item);
					continue;
				}
				break;
			}
			success = true;
			success = !Input.HasInput(position);
			if (!success)
			{
				Error("Failed to parse end of YamlStream.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return yamlStream;
		}

		private YamlDocument ParseImplicitDocument(out bool success)
		{
			YamlDocument yamlDocument = new YamlDocument();
			int num = position;
			currentDocument = yamlDocument;
			currentIndent = -1;
			yamlDocument.Root = ParseIndentedBlockNode(out success);
			if (!success)
			{
				Error("Failed to parse Root of ImplicitDocument.");
				position = num;
				return yamlDocument;
			}
			ParseEndOfDocument(out success);
			success = true;
			return yamlDocument;
		}

		private YamlDocument ParseExplicitDocument(out bool success)
		{
			YamlDocument yamlDocument = new YamlDocument();
			int num = position;
			currentDocument = yamlDocument;
			currentIndent = -1;
			while (true)
			{
				Directive item = ParseDirective(out success);
				if (success)
				{
					yamlDocument.Directives.Add(item);
					continue;
				}
				break;
			}
			success = true;
			MatchTerminalString("---", out success);
			if (!success)
			{
				Error("Failed to parse '---' of ExplicitDocument.");
				position = num;
				return yamlDocument;
			}
			yamlDocument.Root = ParseSeparatedBlockNode(out success);
			if (!success)
			{
				Error("Failed to parse Root of ExplicitDocument.");
				position = num;
				return yamlDocument;
			}
			ParseEndOfDocument(out success);
			success = true;
			return yamlDocument;
		}

		private void ParseEndOfDocument(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			MatchTerminalString("...", out success);
			if (!success)
			{
				Error("Failed to parse '...' of EndOfDocument.");
				position = num;
				return;
			}
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of EndOfDocument.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
		}

		private Directive ParseDirective(out bool success)
		{
			int count = Errors.Count;
			Directive directive = null;
			directive = ParseYamlDirective(out success);
			if (success)
			{
				ClearError(count);
				return directive;
			}
			directive = ParseTagDirective(out success);
			if (success)
			{
				ClearError(count);
				return directive;
			}
			directive = ParseReservedDirective(out success);
			if (success)
			{
				ClearError(count);
				return directive;
			}
			return directive;
		}

		private ReservedDirective ParseReservedDirective(out bool success)
		{
			int count = Errors.Count;
			ReservedDirective reservedDirective = new ReservedDirective();
			int num = position;
			MatchTerminal('%', out success);
			if (!success)
			{
				Error("Failed to parse '%' of ReservedDirective.");
				position = num;
				return reservedDirective;
			}
			reservedDirective.Name = ParseDirectiveName(out success);
			if (!success)
			{
				Error("Failed to parse Name of ReservedDirective.");
				position = num;
				return reservedDirective;
			}
			do
			{
				int num2 = position;
				ParseSeparationSpace(out success);
				if (!success)
				{
					Error("Failed to parse SeparationSpace of ReservedDirective.");
					continue;
				}
				string item = ParseDirectiveParameter(out success);
				if (success)
				{
					reservedDirective.Parameters.Add(item);
					continue;
				}
				Error("Failed to parse DirectiveParameter of ReservedDirective.");
				position = num2;
			}
			while (success);
			success = true;
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of ReservedDirective.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return reservedDirective;
		}

		private string ParseDirectiveName(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (true)
			{
				char value = ParseNonSpaceChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse (NonSpaceChar)+ of DirectiveName.");
			}
			return stringBuilder.ToString();
		}

		private string ParseDirectiveParameter(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (true)
			{
				char value = ParseNonSpaceChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse (NonSpaceChar)+ of DirectiveParameter.");
			}
			return stringBuilder.ToString();
		}

		private YamlDirective ParseYamlDirective(out bool success)
		{
			int count = Errors.Count;
			YamlDirective yamlDirective = new YamlDirective();
			int num = position;
			MatchTerminalString("YAML", out success);
			if (!success)
			{
				Error("Failed to parse 'YAML' of YamlDirective.");
				position = num;
				return yamlDirective;
			}
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of YamlDirective.");
				position = num;
				return yamlDirective;
			}
			yamlDirective.Version = ParseYamlVersion(out success);
			if (!success)
			{
				Error("Failed to parse Version of YamlDirective.");
				position = num;
				return yamlDirective;
			}
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of YamlDirective.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return yamlDirective;
		}

		private YamlVersion ParseYamlVersion(out bool success)
		{
			int count = Errors.Count;
			YamlVersion yamlVersion = new YamlVersion();
			int num = position;
			yamlVersion.Major = ParseInteger(out success);
			if (!success)
			{
				Error("Failed to parse Major of YamlVersion.");
				position = num;
				return yamlVersion;
			}
			MatchTerminal('.', out success);
			if (!success)
			{
				Error("Failed to parse '.' of YamlVersion.");
				position = num;
				return yamlVersion;
			}
			yamlVersion.Minor = ParseInteger(out success);
			if (!success)
			{
				Error("Failed to parse Minor of YamlVersion.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return yamlVersion;
		}

		private TagDirective ParseTagDirective(out bool success)
		{
			int count = Errors.Count;
			TagDirective tagDirective = new TagDirective();
			int num = position;
			MatchTerminalString("TAG", out success);
			if (!success)
			{
				Error("Failed to parse 'TAG' of TagDirective.");
				position = num;
				return tagDirective;
			}
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of TagDirective.");
				position = num;
				return tagDirective;
			}
			tagDirective.Handle = ParseTagHandle(out success);
			if (!success)
			{
				Error("Failed to parse Handle of TagDirective.");
				position = num;
				return tagDirective;
			}
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of TagDirective.");
				position = num;
				return tagDirective;
			}
			tagDirective.Prefix = ParseTagPrefix(out success);
			if (!success)
			{
				Error("Failed to parse Prefix of TagDirective.");
				position = num;
				return tagDirective;
			}
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of TagDirective.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return tagDirective;
		}

		private TagHandle ParseTagHandle(out bool success)
		{
			int count = Errors.Count;
			TagHandle tagHandle = null;
			tagHandle = ParseNamedTagHandle(out success);
			if (success)
			{
				ClearError(count);
				return tagHandle;
			}
			tagHandle = ParseSecondaryTagHandle(out success);
			if (success)
			{
				ClearError(count);
				return tagHandle;
			}
			tagHandle = ParsePrimaryTagHandle(out success);
			if (success)
			{
				ClearError(count);
				return tagHandle;
			}
			return tagHandle;
		}

		private PrimaryTagHandle ParsePrimaryTagHandle(out bool success)
		{
			int count = Errors.Count;
			PrimaryTagHandle result = new PrimaryTagHandle();
			MatchTerminal('!', out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse '!' of PrimaryTagHandle.");
			}
			return result;
		}

		private SecondaryTagHandle ParseSecondaryTagHandle(out bool success)
		{
			int count = Errors.Count;
			SecondaryTagHandle result = new SecondaryTagHandle();
			MatchTerminalString("!!", out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse '!!' of SecondaryTagHandle.");
			}
			return result;
		}

		private NamedTagHandle ParseNamedTagHandle(out bool success)
		{
			int count = Errors.Count;
			NamedTagHandle namedTagHandle = new NamedTagHandle();
			int num = position;
			MatchTerminal('!', out success);
			if (!success)
			{
				Error("Failed to parse '!' of NamedTagHandle.");
				position = num;
				return namedTagHandle;
			}
			int num2 = 0;
			while (true)
			{
				char item = ParseWordChar(out success);
				if (success)
				{
					namedTagHandle.Name.Add(item);
					num2++;
					continue;
				}
				break;
			}
			if (num2 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse Name of NamedTagHandle.");
				position = num;
				return namedTagHandle;
			}
			MatchTerminal('!', out success);
			if (!success)
			{
				Error("Failed to parse '!' of NamedTagHandle.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return namedTagHandle;
		}

		private TagPrefix ParseTagPrefix(out bool success)
		{
			int count = Errors.Count;
			TagPrefix tagPrefix = null;
			tagPrefix = ParseLocalTagPrefix(out success);
			if (success)
			{
				ClearError(count);
				return tagPrefix;
			}
			tagPrefix = ParseGlobalTagPrefix(out success);
			if (success)
			{
				ClearError(count);
				return tagPrefix;
			}
			return tagPrefix;
		}

		private LocalTagPrefix ParseLocalTagPrefix(out bool success)
		{
			LocalTagPrefix localTagPrefix = new LocalTagPrefix();
			int num = position;
			MatchTerminal('!', out success);
			if (!success)
			{
				Error("Failed to parse '!' of LocalTagPrefix.");
				position = num;
				return localTagPrefix;
			}
			while (true)
			{
				char item = ParseUriChar(out success);
				if (success)
				{
					localTagPrefix.Prefix.Add(item);
					continue;
				}
				break;
			}
			success = true;
			return localTagPrefix;
		}

		private GlobalTagPrefix ParseGlobalTagPrefix(out bool success)
		{
			int count = Errors.Count;
			GlobalTagPrefix globalTagPrefix = new GlobalTagPrefix();
			int num = 0;
			while (true)
			{
				char item = ParseUriChar(out success);
				if (success)
				{
					globalTagPrefix.Prefix.Add(item);
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse Prefix of GlobalTagPrefix.");
			}
			return globalTagPrefix;
		}

		private DataItem ParseDataItem(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = new DataItem();
			int num = position;
			int num2 = position;
			dataItem.Property = ParseNodeProperty(out success);
			if (!success)
			{
				Error("Failed to parse Property of DataItem.");
			}
			else
			{
				ParseSeparationLines(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLines of DataItem.");
					position = num2;
				}
			}
			success = true;
			ErrorStatck.Push(count);
			count = Errors.Count;
			dataItem = ParseScalar(out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				dataItem = ParseSequence(out success);
				if (success)
				{
					ClearError(count);
				}
				else
				{
					dataItem = ParseMapping(out success);
					if (success)
					{
						ClearError(count);
					}
				}
			}
			count = ErrorStatck.Pop();
			if (!success)
			{
				Error("Failed to parse (Scalar / Sequence / Mapping) of DataItem.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return dataItem;
		}

		private Scalar ParseScalar(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = null;
			scalar = ParseFlowScalarInBlock(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar = ParseFlowScalarInFlow(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar = ParseBlockScalar(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private Sequence ParseSequence(out bool success)
		{
			int count = Errors.Count;
			Sequence sequence = null;
			sequence = ParseFlowSequence(out success);
			if (success)
			{
				ClearError(count);
				return sequence;
			}
			sequence = ParseBlockSequence(out success);
			if (success)
			{
				ClearError(count);
				return sequence;
			}
			return sequence;
		}

		private Mapping ParseMapping(out bool success)
		{
			int count = Errors.Count;
			Mapping mapping = null;
			mapping = ParseFlowMapping(out success);
			if (success)
			{
				ClearError(count);
				return mapping;
			}
			mapping = ParseBlockMapping(out success);
			if (success)
			{
				ClearError(count);
				return mapping;
			}
			return mapping;
		}

		private DataItem ParseIndentedBlockNode(out bool success)
		{
			int count = Errors.Count;
			IncreaseIndent();
			DataItem result = ParseIndentedBlock(out success);
			DecreaseIndent();
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse IndentedBlock of IndentedBlockNode.");
			}
			return result;
		}

		private DataItem ParseSeparatedBlockNode(out bool success)
		{
			int count = Errors.Count;
			IncreaseIndent();
			DataItem result = ParseSeparatedBlock(out success);
			DecreaseIndent();
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse SeparatedBlock of SeparatedBlockNode.");
			}
			return result;
		}

		private DataItem ParseIndentedBlock(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseIndentedContent(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			int num = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of IndentedBlock.");
			}
			else
			{
				dataItem = ParseAliasNode(out success);
				if (!success)
				{
					Error("Failed to parse AliasNode of IndentedBlock.");
					position = num;
				}
				else
				{
					ParseInlineComments(out success);
					if (!success)
					{
						Error("Failed to parse InlineComments of IndentedBlock.");
						position = num;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of IndentedBlock.");
			}
			else
			{
				NodeProperty property = ParseNodeProperty(out success);
				if (!success)
				{
					Error("Failed to parse property of IndentedBlock.");
					position = num2;
				}
				else
				{
					dataItem = ParseSeparatedContent(out success);
					if (success)
					{
						SetDataItemProperty(dataItem, property);
					}
					success = true;
				}
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseSeparatedBlock(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseSeparatedContent(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			int num = position;
			ParseSeparationLines(out success);
			if (!success)
			{
				Error("Failed to parse SeparationLines of SeparatedBlock.");
			}
			else
			{
				dataItem = ParseAliasNode(out success);
				if (!success)
				{
					Error("Failed to parse AliasNode of SeparatedBlock.");
					position = num;
				}
				else
				{
					ParseInlineComments(out success);
					if (!success)
					{
						Error("Failed to parse InlineComments of SeparatedBlock.");
						position = num;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			int num2 = position;
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of SeparatedBlock.");
			}
			else
			{
				NodeProperty property = ParseNodeProperty(out success);
				if (!success)
				{
					Error("Failed to parse property of SeparatedBlock.");
					position = num2;
				}
				else
				{
					dataItem = ParseSeparatedContent(out success);
					if (success)
					{
						SetDataItemProperty(dataItem, property);
					}
					success = true;
				}
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseEmptyBlock(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseIndentedContent(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of IndentedContent.");
			}
			else
			{
				result = ParseBlockContent(out success);
				if (!success)
				{
					Error("Failed to parse BlockContent of IndentedContent.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of IndentedContent.");
			}
			else
			{
				result = ParseFlowContentInBlock(out success);
				if (!success)
				{
					Error("Failed to parse FlowContentInBlock of IndentedContent.");
					position = num2;
				}
				else
				{
					ParseInlineComments(out success);
					if (!success)
					{
						Error("Failed to parse InlineComments of IndentedContent.");
						position = num2;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private DataItem ParseSeparatedContent(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of SeparatedContent.");
			}
			else
			{
				result = ParseIndentedContent(out success);
				if (!success)
				{
					Error("Failed to parse IndentedContent of SeparatedContent.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num2 = position;
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of SeparatedContent.");
			}
			else
			{
				result = ParseBlockScalar(out success);
				if (!success)
				{
					Error("Failed to parse BlockScalar of SeparatedContent.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num3 = position;
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of SeparatedContent.");
			}
			else
			{
				result = ParseFlowContentInBlock(out success);
				if (!success)
				{
					Error("Failed to parse FlowContentInBlock of SeparatedContent.");
					position = num3;
				}
				else
				{
					ParseInlineComments(out success);
					if (!success)
					{
						Error("Failed to parse InlineComments of SeparatedContent.");
						position = num3;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private DataItem ParseBlockCollectionEntry(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			IncreaseIndent();
			int num = position;
			ParseSeparationSpaceAsIndent(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpaceAsIndent of BlockCollectionEntry.");
			}
			else
			{
				result = ParseBlockCollection(out success);
				if (!success)
				{
					Error("Failed to parse BlockCollection of BlockCollectionEntry.");
					position = num;
				}
			}
			DecreaseIndent();
			if (success)
			{
				ClearError(count);
				return result;
			}
			result = ParseSeparatedBlockNode(out success);
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private DataItem ParseBlockCollectionEntryOptionalIndent(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			RememberIndent();
			int num = position;
			ParseSeparationSpaceAsIndent(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpaceAsIndent of BlockCollectionEntryOptionalIndent.");
			}
			else
			{
				result = ParseBlockCollection(out success);
				if (!success)
				{
					Error("Failed to parse BlockCollection of BlockCollectionEntryOptionalIndent.");
					position = num;
				}
			}
			RestoreIndent();
			if (success)
			{
				ClearError(count);
				return result;
			}
			RememberIndent();
			result = ParseSeparatedBlock(out success);
			RestoreIndent();
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private DataItem ParseFlowNodeInFlow(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseAliasNode(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseFlowContentInFlow(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			NodeProperty property = ParseNodeProperty(out success);
			if (success)
			{
				dataItem = new Scalar();
				int num = position;
				ParseSeparationLinesInFlow(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLinesInFlow of FlowNodeInFlow.");
				}
				else
				{
					dataItem = ParseFlowContentInFlow(out success);
					if (!success)
					{
						Error("Failed to parse FlowContentInFlow of FlowNodeInFlow.");
						position = num;
					}
				}
				if (success)
				{
					SetDataItemProperty(dataItem, property);
				}
				success = true;
			}
			else
			{
				Error("Failed to parse property of FlowNodeInFlow.");
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseAliasNode(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			MatchTerminal('*', out success);
			if (!success)
			{
				Error("Failed to parse '*' of AliasNode.");
				position = num;
				return result;
			}
			string name = ParseAnchorName(out success);
			if (success)
			{
				return GetAnchoredDataItem(name);
			}
			Error("Failed to parse name of AliasNode.");
			position = num;
			if (success)
			{
				ClearError(count);
			}
			return result;
		}

		private DataItem ParseFlowContentInBlock(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseFlowScalarInBlock(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseFlowSequence(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseFlowMapping(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseFlowContentInFlow(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseFlowScalarInFlow(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseFlowSequence(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseFlowMapping(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseBlockContent(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseBlockScalar(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseBlockSequence(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseBlockMapping(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseBlockCollection(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseBlockSequence(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			dataItem = ParseBlockMapping(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private DataItem ParseEmptyFlow(out bool success)
		{
			DataItem result = new DataItem();
			success = true;
			if (success)
			{
				return new Scalar();
			}
			return result;
		}

		private DataItem ParseEmptyBlock(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			int num = position;
			dataItem = ParseEmptyFlow(out success);
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of EmptyBlock.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return dataItem;
		}

		private NodeProperty ParseNodeProperty(out bool success)
		{
			int count = Errors.Count;
			NodeProperty nodeProperty = new NodeProperty();
			nodeProperty.Tag = ParseTag(out success);
			if (!success)
			{
				Error("Failed to parse Tag of NodeProperty.");
			}
			else
			{
				int num = position;
				ParseSeparationLines(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLines of NodeProperty.");
				}
				else
				{
					nodeProperty.Anchor = ParseAnchor(out success);
					if (!success)
					{
						Error("Failed to parse Anchor of NodeProperty.");
						position = num;
					}
				}
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return nodeProperty;
			}
			nodeProperty.Anchor = ParseAnchor(out success);
			if (!success)
			{
				Error("Failed to parse Anchor of NodeProperty.");
			}
			else
			{
				int num2 = position;
				ParseSeparationLines(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLines of NodeProperty.");
				}
				else
				{
					nodeProperty.Tag = ParseTag(out success);
					if (!success)
					{
						Error("Failed to parse Tag of NodeProperty.");
						position = num2;
					}
				}
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return nodeProperty;
			}
			return nodeProperty;
		}

		private string ParseAnchor(out bool success)
		{
			int count = Errors.Count;
			string result = null;
			int num = position;
			MatchTerminal('&', out success);
			if (!success)
			{
				Error("Failed to parse '&' of Anchor.");
				position = num;
				return result;
			}
			result = ParseAnchorName(out success);
			if (!success)
			{
				Error("Failed to parse AnchorName of Anchor.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return result;
		}

		private string ParseAnchorName(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (true)
			{
				char value = ParseNonSpaceChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse (NonSpaceChar)+ of AnchorName.");
			}
			return stringBuilder.ToString();
		}

		private Tag ParseTag(out bool success)
		{
			int count = Errors.Count;
			Tag tag = null;
			tag = ParseVerbatimTag(out success);
			if (success)
			{
				ClearError(count);
				return tag;
			}
			tag = ParseShorthandTag(out success);
			if (success)
			{
				ClearError(count);
				return tag;
			}
			tag = ParseNonSpecificTag(out success);
			if (success)
			{
				ClearError(count);
				return tag;
			}
			return tag;
		}

		private NonSpecificTag ParseNonSpecificTag(out bool success)
		{
			int count = Errors.Count;
			NonSpecificTag result = new NonSpecificTag();
			MatchTerminal('!', out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse '!' of NonSpecificTag.");
			}
			return result;
		}

		private VerbatimTag ParseVerbatimTag(out bool success)
		{
			int count = Errors.Count;
			VerbatimTag verbatimTag = new VerbatimTag();
			int num = position;
			MatchTerminal('!', out success);
			if (!success)
			{
				Error("Failed to parse '!' of VerbatimTag.");
				position = num;
				return verbatimTag;
			}
			MatchTerminal('<', out success);
			if (!success)
			{
				Error("Failed to parse '<' of VerbatimTag.");
				position = num;
				return verbatimTag;
			}
			int num2 = 0;
			while (true)
			{
				char item = ParseUriChar(out success);
				if (success)
				{
					verbatimTag.Chars.Add(item);
					num2++;
					continue;
				}
				break;
			}
			if (num2 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse Chars of VerbatimTag.");
				position = num;
				return verbatimTag;
			}
			MatchTerminal('>', out success);
			if (!success)
			{
				Error("Failed to parse '>' of VerbatimTag.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return verbatimTag;
		}

		private ShorthandTag ParseShorthandTag(out bool success)
		{
			int count = Errors.Count;
			ShorthandTag shorthandTag = new ShorthandTag();
			int num = position;
			ParseNamedTagHandle(out success);
			if (!success)
			{
				Error("Failed to parse NamedTagHandle of ShorthandTag.");
			}
			else
			{
				int num2 = 0;
				while (true)
				{
					char item = ParseTagChar(out success);
					if (success)
					{
						shorthandTag.Chars.Add(item);
						num2++;
						continue;
					}
					break;
				}
				if (num2 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse Chars of ShorthandTag.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return shorthandTag;
			}
			int num3 = position;
			ParseSecondaryTagHandle(out success);
			if (!success)
			{
				Error("Failed to parse SecondaryTagHandle of ShorthandTag.");
			}
			else
			{
				int num4 = 0;
				while (true)
				{
					char item2 = ParseTagChar(out success);
					if (success)
					{
						shorthandTag.Chars.Add(item2);
						num4++;
						continue;
					}
					break;
				}
				if (num4 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse Chars of ShorthandTag.");
					position = num3;
				}
			}
			if (success)
			{
				ClearError(count);
				return shorthandTag;
			}
			int num5 = position;
			ParsePrimaryTagHandle(out success);
			if (!success)
			{
				Error("Failed to parse PrimaryTagHandle of ShorthandTag.");
			}
			else
			{
				int num6 = 0;
				while (true)
				{
					char item3 = ParseTagChar(out success);
					if (success)
					{
						shorthandTag.Chars.Add(item3);
						num6++;
						continue;
					}
					break;
				}
				if (num6 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse Chars of ShorthandTag.");
					position = num5;
				}
			}
			if (success)
			{
				ClearError(count);
				return shorthandTag;
			}
			return shorthandTag;
		}

		private Scalar ParseFlowScalarInBlock(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = new Scalar();
			scalar.Text = ParsePlainTextMultiLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseSingleQuotedText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseDoubleQuotedText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private Scalar ParseFlowScalarInFlow(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = new Scalar();
			scalar.Text = ParsePlainTextInFlow(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseSingleQuotedText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseDoubleQuotedText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private Scalar ParseBlockScalar(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = new Scalar();
			scalar.Text = ParseLiteralText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseFoldedText(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private string ParsePlainText(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			text = ParsePlainTextMultiLine(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			text = ParsePlainTextInFlow(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			return text;
		}

		private string ParsePlainTextMultiLine(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParsePlainTextSingleLine(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParsePlainTextMoreLine(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse PlainTextSingleLine of PlainTextMultiLine.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextSingleLine(out bool success)
		{
			int t = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			int num2 = position;
			ParseDocumentMarker(out success);
			position = num2;
			success = !success;
			if (!success)
			{
				Error("Failed to parse !(DocumentMarker) of PlainTextSingleLine.");
				position = num;
				return stringBuilder.ToString();
			}
			string value = ParsePlainTextFirstChar(out success);
			if (success)
			{
				stringBuilder.Append(value);
				do
				{
					ErrorStatck.Push(t);
					t = Errors.Count;
					value = ParsePlainTextChar(out success);
					if (success)
					{
						ClearError(t);
						stringBuilder.Append(value);
					}
					else
					{
						value = ParseSpacedPlainTextChar(out success);
						if (success)
						{
							ClearError(t);
							stringBuilder.Append(value);
						}
					}
					t = ErrorStatck.Pop();
				}
				while (success);
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse PlainTextFirstChar of PlainTextSingleLine.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextMoreLine(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIgnoredBlank(out success);
			string value = ParseLineFolding(out success);
			if (success)
			{
				stringBuilder.Append(value);
				ParseIndent(out success);
				if (!success)
				{
					Error("Failed to parse Indent of PlainTextMoreLine.");
					position = num2;
					return stringBuilder.ToString();
				}
				ParseIgnoredSpace(out success);
				int num3 = 0;
				while (true)
				{
					ErrorStatck.Push(num);
					num = Errors.Count;
					value = ParsePlainTextChar(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						value = ParseSpacedPlainTextChar(out success);
						if (success)
						{
							ClearError(num);
							stringBuilder.Append(value);
						}
					}
					num = ErrorStatck.Pop();
					if (!success)
					{
						break;
					}
					num3++;
				}
				if (num3 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse ((PlainTextChar / SpacedPlainTextChar))+ of PlainTextMoreLine.");
					position = num2;
				}
				if (success)
				{
					ClearError(num);
				}
				return stringBuilder.ToString();
			}
			Error("Failed to parse LineFolding of PlainTextMoreLine.");
			position = num2;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextInFlow(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParsePlainTextInFlowSingleLine(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParsePlainTextInFlowMoreLine(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse PlainTextInFlowSingleLine of PlainTextInFlow.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextInFlowSingleLine(out bool success)
		{
			int t = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			int num2 = position;
			ParseDocumentMarker(out success);
			position = num2;
			success = !success;
			if (!success)
			{
				Error("Failed to parse !(DocumentMarker) of PlainTextInFlowSingleLine.");
				position = num;
				return stringBuilder.ToString();
			}
			string value = ParsePlainTextFirstCharInFlow(out success);
			if (success)
			{
				stringBuilder.Append(value);
				do
				{
					ErrorStatck.Push(t);
					t = Errors.Count;
					value = ParsePlainTextCharInFlow(out success);
					if (success)
					{
						ClearError(t);
						stringBuilder.Append(value);
					}
					else
					{
						value = ParseSpacedPlainTextCharInFlow(out success);
						if (success)
						{
							ClearError(t);
							stringBuilder.Append(value);
						}
					}
					t = ErrorStatck.Pop();
				}
				while (success);
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse PlainTextFirstCharInFlow of PlainTextInFlowSingleLine.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextInFlowMoreLine(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIgnoredBlank(out success);
			string value = ParseLineFolding(out success);
			if (success)
			{
				stringBuilder.Append(value);
				ParseIndent(out success);
				if (!success)
				{
					Error("Failed to parse Indent of PlainTextInFlowMoreLine.");
					position = num2;
					return stringBuilder.ToString();
				}
				ParseIgnoredSpace(out success);
				int num3 = 0;
				while (true)
				{
					ErrorStatck.Push(num);
					num = Errors.Count;
					value = ParsePlainTextCharInFlow(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						value = ParseSpacedPlainTextCharInFlow(out success);
						if (success)
						{
							ClearError(num);
							stringBuilder.Append(value);
						}
					}
					num = ErrorStatck.Pop();
					if (!success)
					{
						break;
					}
					num3++;
				}
				if (num3 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse ((PlainTextCharInFlow / SpacedPlainTextCharInFlow))+ of PlainTextInFlowMoreLine.");
					position = num2;
				}
				if (success)
				{
					ClearError(num);
				}
				return stringBuilder.ToString();
			}
			Error("Failed to parse LineFolding of PlainTextInFlowMoreLine.");
			position = num2;
			return stringBuilder.ToString();
		}

		private string ParsePlainTextFirstChar(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			char value = MatchTerminalSet("\r\n\t -?:,[]{}#&*!|>'\"%@`", true, out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value);
				return stringBuilder.ToString();
			}
			int num = position;
			value = MatchTerminalSet("-?:", false, out success);
			if (success)
			{
				stringBuilder.Append(value);
				value = ParseNonSpaceChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse NonSpaceChar of PlainTextFirstChar.");
					position = num;
				}
			}
			else
			{
				Error("Failed to parse \"-?:\" of PlainTextFirstChar.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString();
		}

		private string ParsePlainTextChar(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			char value = MatchTerminal(':', out success);
			if (success)
			{
				stringBuilder.Append(value);
				value = ParseNonSpaceChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse NonSpaceChar of PlainTextChar.");
					position = num;
				}
			}
			else
			{
				Error("Failed to parse ':' of PlainTextChar.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			int num2 = position;
			char value2 = ParseNonSpaceChar(out success);
			if (success)
			{
				stringBuilder.Append(value2);
				int num3 = 0;
				while (true)
				{
					value2 = MatchTerminal('#', out success);
					if (success)
					{
						stringBuilder.Append(value2);
						num3++;
						continue;
					}
					break;
				}
				if (num3 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse ('#')+ of PlainTextChar.");
					position = num2;
				}
			}
			else
			{
				Error("Failed to parse NonSpaceChar of PlainTextChar.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			stringBuilder.Length = 0;
			char value3 = MatchTerminalSet("\r\n\t :#", true, out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value3);
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString();
		}

		private string ParseSpacedPlainTextChar(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			int num2 = 0;
			while (true)
			{
				char value = MatchTerminal(' ', out success);
				if (success)
				{
					stringBuilder.Append(value);
					num2++;
					continue;
				}
				break;
			}
			if (num2 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse (' ')+ of SpacedPlainTextChar.");
				position = num;
				return stringBuilder.ToString();
			}
			string value2 = ParsePlainTextChar(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse PlainTextChar of SpacedPlainTextChar.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return stringBuilder.ToString();
		}

		private string ParsePlainTextFirstCharInFlow(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			char value = MatchTerminalSet("\r\n\t -?:,[]{}#&*!|>'\"%@`", true, out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value);
				return stringBuilder.ToString();
			}
			int num = position;
			value = MatchTerminalSet("-?:", false, out success);
			if (success)
			{
				stringBuilder.Append(value);
				value = ParseNonSpaceSep(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse NonSpaceSep of PlainTextFirstCharInFlow.");
					position = num;
				}
			}
			else
			{
				Error("Failed to parse \"-?:\" of PlainTextFirstCharInFlow.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString();
		}

		private string ParsePlainTextCharInFlow(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			char value = MatchTerminalSet(":", false, out success);
			if (success)
			{
				stringBuilder.Append(value);
				value = ParseNonSpaceSep(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse NonSpaceSep of PlainTextCharInFlow.");
					position = num;
				}
			}
			else
			{
				Error("Failed to parse \":\" of PlainTextCharInFlow.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			int num2 = position;
			char value2 = ParseNonSpaceSep(out success);
			if (success)
			{
				stringBuilder.Append(value2);
				value2 = MatchTerminal('#', out success);
				if (success)
				{
					stringBuilder.Append(value2);
				}
				else
				{
					Error("Failed to parse '#' of PlainTextCharInFlow.");
					position = num2;
				}
			}
			else
			{
				Error("Failed to parse NonSpaceSep of PlainTextCharInFlow.");
			}
			if (success)
			{
				ClearError(count);
				return stringBuilder.ToString();
			}
			stringBuilder.Length = 0;
			char value3 = MatchTerminalSet("\r\n\t :#,[]{}", true, out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value3);
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString();
		}

		private string ParseSpacedPlainTextCharInFlow(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			int num2 = 0;
			while (true)
			{
				char value = MatchTerminal(' ', out success);
				if (success)
				{
					stringBuilder.Append(value);
					num2++;
					continue;
				}
				break;
			}
			if (num2 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse (' ')+ of SpacedPlainTextCharInFlow.");
				position = num;
				return stringBuilder.ToString();
			}
			string value2 = ParsePlainTextCharInFlow(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse PlainTextCharInFlow of SpacedPlainTextCharInFlow.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return stringBuilder.ToString();
		}

		private void ParseDocumentMarker(out bool success)
		{
			int num = Errors.Count;
			int num2 = position;
			success = position == 0 || TerminalMatch('\n', position - 1);
			if (!success)
			{
				Error("Failed to parse sol of DocumentMarker.");
			}
			else
			{
				MatchTerminalString("---", out success);
				if (!success)
				{
					Error("Failed to parse '---' of DocumentMarker.");
					position = num2;
				}
				else
				{
					ErrorStatck.Push(num);
					num = Errors.Count;
					ParseSpace(out success);
					if (success)
					{
						ClearError(num);
					}
					else
					{
						ParseLineBreak(out success);
						if (success)
						{
							ClearError(num);
						}
					}
					num = ErrorStatck.Pop();
					if (!success)
					{
						Error("Failed to parse (Space / LineBreak) of DocumentMarker.");
						position = num2;
					}
				}
			}
			if (success)
			{
				ClearError(num);
				return;
			}
			int num3 = position;
			success = position == 0 || TerminalMatch('\n', position - 1);
			if (!success)
			{
				Error("Failed to parse sol of DocumentMarker.");
			}
			else
			{
				MatchTerminalString("...", out success);
				if (!success)
				{
					Error("Failed to parse '...' of DocumentMarker.");
					position = num3;
				}
				else
				{
					ErrorStatck.Push(num);
					num = Errors.Count;
					ParseSpace(out success);
					if (success)
					{
						ClearError(num);
					}
					else
					{
						ParseLineBreak(out success);
						if (success)
						{
							ClearError(num);
						}
					}
					num = ErrorStatck.Pop();
					if (!success)
					{
						Error("Failed to parse (Space / LineBreak) of DocumentMarker.");
						position = num3;
					}
				}
			}
			if (success)
			{
				ClearError(num);
			}
		}

		private string ParseDoubleQuotedText(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			text = ParseDoubleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			text = ParseDoubleQuotedMultiLine(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			return text;
		}

		private string ParseDoubleQuotedSingleLine(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			MatchTerminal('"', out success);
			if (!success)
			{
				Error("Failed to parse '\\\"' of DoubleQuotedSingleLine.");
				position = num2;
				return stringBuilder.ToString();
			}
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet("\"\\\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapeSequence(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			MatchTerminal('"', out success);
			if (!success)
			{
				Error("Failed to parse '\\\"' of DoubleQuotedSingleLine.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseDoubleQuotedMultiLine(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseDoubleQuotedMultiLineFist(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParseDoubleQuotedMultiLineInner(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				value = ParseDoubleQuotedMultiLineLast(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse DoubleQuotedMultiLineLast of DoubleQuotedMultiLine.");
					position = num;
				}
				if (success)
				{
					ClearError(count);
				}
				return stringBuilder.ToString();
			}
			Error("Failed to parse DoubleQuotedMultiLineFist of DoubleQuotedMultiLine.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParseDoubleQuotedMultiLineFist(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			MatchTerminal('"', out success);
			if (!success)
			{
				Error("Failed to parse '\\\"' of DoubleQuotedMultiLineFist.");
				position = num2;
				return stringBuilder.ToString();
			}
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet(" \"\\\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapeSequence(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						int num3 = position;
						value = MatchTerminal(' ', out success);
						if (success)
						{
							stringBuilder.Append(value);
							int num4 = position;
							ParseIgnoredBlank(out success);
							ParseLineBreak(out success);
							position = num4;
							success = !success;
							if (!success)
							{
								Error("Failed to parse !((IgnoredBlank LineBreak)) of DoubleQuotedMultiLineFist.");
								position = num3;
							}
						}
						else
						{
							Error("Failed to parse ' ' of DoubleQuotedMultiLineFist.");
						}
						if (success)
						{
							ClearError(num);
						}
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			ParseIgnoredBlank(out success);
			string value2 = ParseDoubleQuotedMultiLineBreak(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse DoubleQuotedMultiLineBreak of DoubleQuotedMultiLineFist.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseDoubleQuotedMultiLineInner(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of DoubleQuotedMultiLineInner.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			int num3 = 0;
			while (true)
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet(" \"\\\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapeSequence(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						int num4 = position;
						value = MatchTerminal(' ', out success);
						if (success)
						{
							stringBuilder.Append(value);
							int num5 = position;
							ParseIgnoredBlank(out success);
							ParseLineBreak(out success);
							position = num5;
							success = !success;
							if (!success)
							{
								Error("Failed to parse !((IgnoredBlank LineBreak)) of DoubleQuotedMultiLineInner.");
								position = num4;
							}
						}
						else
						{
							Error("Failed to parse ' ' of DoubleQuotedMultiLineInner.");
						}
						if (success)
						{
							ClearError(num);
						}
					}
				}
				num = ErrorStatck.Pop();
				if (!success)
				{
					break;
				}
				num3++;
			}
			if (num3 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse ((-\" \"\\\r\n\" / EscapeSequence / ' ' !((IgnoredBlank LineBreak))))+ of DoubleQuotedMultiLineInner.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			string value2 = ParseDoubleQuotedMultiLineBreak(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse DoubleQuotedMultiLineBreak of DoubleQuotedMultiLineInner.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseDoubleQuotedMultiLineLast(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of DoubleQuotedMultiLineLast.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet("\"\\\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapeSequence(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			MatchTerminal('"', out success);
			if (!success)
			{
				Error("Failed to parse '\\\"' of DoubleQuotedMultiLineLast.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseDoubleQuotedMultiLineBreak(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			text = ParseLineFolding(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			ParseEscapedLineBreak(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			return text;
		}

		private string ParseSingleQuotedText(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			text = ParseSingleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			text = ParseSingleQuotedMultiLine(out success);
			if (success)
			{
				ClearError(count);
				return text;
			}
			return text;
		}

		private string ParseSingleQuotedSingleLine(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			MatchTerminal('\'', out success);
			if (!success)
			{
				Error("Failed to parse ''' of SingleQuotedSingleLine.");
				position = num2;
				return stringBuilder.ToString();
			}
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet("'\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapedSingleQuote(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			MatchTerminal('\'', out success);
			if (!success)
			{
				Error("Failed to parse ''' of SingleQuotedSingleLine.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseSingleQuotedMultiLine(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseSingleQuotedMultiLineFist(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParseSingleQuotedMultiLineInner(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				value = ParseSingleQuotedMultiLineLast(out success);
				if (success)
				{
					stringBuilder.Append(value);
				}
				else
				{
					Error("Failed to parse SingleQuotedMultiLineLast of SingleQuotedMultiLine.");
					position = num;
				}
				if (success)
				{
					ClearError(count);
				}
				return stringBuilder.ToString();
			}
			Error("Failed to parse SingleQuotedMultiLineFist of SingleQuotedMultiLine.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParseSingleQuotedMultiLineFist(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			MatchTerminal('\'', out success);
			if (!success)
			{
				Error("Failed to parse ''' of SingleQuotedMultiLineFist.");
				position = num2;
				return stringBuilder.ToString();
			}
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet(" '\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapedSingleQuote(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						int num3 = position;
						value = MatchTerminal(' ', out success);
						if (success)
						{
							stringBuilder.Append(value);
							int num4 = position;
							ParseIgnoredBlank(out success);
							ParseLineBreak(out success);
							position = num4;
							success = !success;
							if (!success)
							{
								Error("Failed to parse !((IgnoredBlank LineBreak)) of SingleQuotedMultiLineFist.");
								position = num3;
							}
						}
						else
						{
							Error("Failed to parse ' ' of SingleQuotedMultiLineFist.");
						}
						if (success)
						{
							ClearError(num);
						}
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			ParseIgnoredBlank(out success);
			string value2 = ParseLineFolding(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse fold of SingleQuotedMultiLineFist.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseSingleQuotedMultiLineInner(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of SingleQuotedMultiLineInner.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			int num3 = 0;
			while (true)
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet(" '\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapedSingleQuote(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
					else
					{
						int num4 = position;
						value = MatchTerminal(' ', out success);
						if (success)
						{
							stringBuilder.Append(value);
							int num5 = position;
							ParseIgnoredBlank(out success);
							ParseLineBreak(out success);
							position = num5;
							success = !success;
							if (!success)
							{
								Error("Failed to parse !((IgnoredBlank LineBreak)) of SingleQuotedMultiLineInner.");
								position = num4;
							}
						}
						else
						{
							Error("Failed to parse ' ' of SingleQuotedMultiLineInner.");
						}
						if (success)
						{
							ClearError(num);
						}
					}
				}
				num = ErrorStatck.Pop();
				if (!success)
				{
					break;
				}
				num3++;
			}
			if (num3 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse ((-\" '\r\n\" / EscapedSingleQuote / ' ' !((IgnoredBlank LineBreak))))+ of SingleQuotedMultiLineInner.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			string value2 = ParseLineFolding(out success);
			if (success)
			{
				stringBuilder.Append(value2);
			}
			else
			{
				Error("Failed to parse fold of SingleQuotedMultiLineInner.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseSingleQuotedMultiLineLast(out bool success)
		{
			int num = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of SingleQuotedMultiLineLast.");
				position = num2;
				return stringBuilder.ToString();
			}
			ParseIgnoredBlank(out success);
			do
			{
				ErrorStatck.Push(num);
				num = Errors.Count;
				char value = MatchTerminalSet("'\r\n", true, out success);
				if (success)
				{
					ClearError(num);
					stringBuilder.Append(value);
				}
				else
				{
					value = ParseEscapedSingleQuote(out success);
					if (success)
					{
						ClearError(num);
						stringBuilder.Append(value);
					}
				}
				num = ErrorStatck.Pop();
			}
			while (success);
			success = true;
			MatchTerminal('\'', out success);
			if (!success)
			{
				Error("Failed to parse ''' of SingleQuotedMultiLineLast.");
				position = num2;
			}
			if (success)
			{
				ClearError(num);
			}
			return stringBuilder.ToString();
		}

		private string ParseLineFolding(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			int num = position;
			text = ParseReservedLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse ReservedLineBreak of LineFolding.");
			}
			else
			{
				int num2 = 0;
				while (true)
				{
					int num3 = position;
					ParseIgnoredBlank(out success);
					ParseLineBreak(out success);
					if (!success)
					{
						Error("Failed to parse LineBreak of LineFolding.");
						position = num3;
					}
					if (!success)
					{
						break;
					}
					num2++;
				}
				if (num2 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse ((IgnoredBlank LineBreak))+ of LineFolding.");
					position = num;
				}
				else
				{
					int num4 = position;
					ParseIndent(out success);
					position = num4;
					if (!success)
					{
						Error("Failed to parse &(Indent) of LineFolding.");
						position = num;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return text;
			}
			int num5 = position;
			ParseLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse LineBreak of LineFolding.");
			}
			else
			{
				int num6 = position;
				ParseIndent(out success);
				position = num6;
				if (success)
				{
					return " ";
				}
				Error("Failed to parse &(Indent) of LineFolding.");
				position = num5;
			}
			if (success)
			{
				ClearError(count);
				return text;
			}
			return text;
		}

		private char ParseEscapedSingleQuote(out bool success)
		{
			int count = Errors.Count;
			char result = '\0';
			MatchTerminalString("''", out success);
			if (success)
			{
				ClearError(count);
				return '\'';
			}
			Error("Failed to parse '''' of EscapedSingleQuote.");
			return result;
		}

		private void ParseEscapedLineBreak(out bool success)
		{
			int num = position;
			MatchTerminal('\\', out success);
			if (!success)
			{
				Error("Failed to parse '\\\\' of EscapedLineBreak.");
				position = num;
				return;
			}
			ParseLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse LineBreak of EscapedLineBreak.");
				position = num;
				return;
			}
			do
			{
				int num2 = position;
				ParseIgnoredBlank(out success);
				ParseLineBreak(out success);
				if (!success)
				{
					Error("Failed to parse LineBreak of EscapedLineBreak.");
					position = num2;
				}
			}
			while (success);
			success = true;
		}

		private string ParseLiteralText(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			MatchTerminal('|', out success);
			if (!success)
			{
				Error("Failed to parse '|' of LiteralText.");
				position = num;
				return stringBuilder.ToString();
			}
			BlockScalarModifier modifier = ParseBlockScalarModifier(out success);
			AddIndent(modifier, success);
			success = true;
			ParseInlineComment(out success);
			if (!success)
			{
				Error("Failed to parse InlineComment of LiteralText.");
				position = num;
				return stringBuilder.ToString();
			}
			string value = ParseLiteralContent(out success);
			DecreaseIndent();
			if (success)
			{
				stringBuilder.Append(value);
			}
			success = true;
			return stringBuilder.ToString();
		}

		private string ParseFoldedText(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			MatchTerminal('>', out success);
			if (!success)
			{
				Error("Failed to parse '>' of FoldedText.");
				position = num;
				return stringBuilder.ToString();
			}
			BlockScalarModifier modifier = ParseBlockScalarModifier(out success);
			AddIndent(modifier, success);
			success = true;
			ParseInlineComment(out success);
			if (!success)
			{
				Error("Failed to parse InlineComment of FoldedText.");
				position = num;
				return stringBuilder.ToString();
			}
			int num2 = position;
			while (true)
			{
				string value = ParseEmptyLineBlock(out success);
				if (success)
				{
					stringBuilder.Append(value);
					continue;
				}
				break;
			}
			success = true;
			string value2 = ParseFoldedLines(out success);
			if (success)
			{
				stringBuilder.Append(value2);
				value2 = ParseChompedLineBreak(out success);
				if (success)
				{
					stringBuilder.Append(value2);
					ParseComments(out success);
					success = true;
				}
				else
				{
					Error("Failed to parse ChompedLineBreak of FoldedText.");
					position = num2;
				}
			}
			else
			{
				Error("Failed to parse FoldedLines of FoldedText.");
				position = num2;
			}
			DecreaseIndent();
			if (!success)
			{
				Error("Failed to parse (((EmptyLineBlock))* FoldedLines ChompedLineBreak (Comments)?) of FoldedText.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return stringBuilder.ToString();
		}

		private BlockScalarModifier ParseBlockScalarModifier(out bool success)
		{
			int count = Errors.Count;
			BlockScalarModifier blockScalarModifier = new BlockScalarModifier();
			blockScalarModifier.Indent = ParseIndentIndicator(out success);
			if (!success)
			{
				Error("Failed to parse Indent of BlockScalarModifier.");
			}
			else
			{
				blockScalarModifier.Chomp = ParseChompingIndicator(out success);
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return blockScalarModifier;
			}
			blockScalarModifier.Chomp = ParseChompingIndicator(out success);
			if (!success)
			{
				Error("Failed to parse Chomp of BlockScalarModifier.");
			}
			else
			{
				blockScalarModifier.Indent = ParseIndentIndicator(out success);
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return blockScalarModifier;
			}
			return blockScalarModifier;
		}

		private string ParseLiteralContent(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseLiteralFirst(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParseLiteralInner(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				string value2 = ParseChompedLineBreak(out success);
				if (success)
				{
					stringBuilder.Append(value2);
					ParseComments(out success);
					success = true;
					return stringBuilder.ToString();
				}
				Error("Failed to parse str2 of LiteralContent.");
				position = num;
				return stringBuilder.ToString();
			}
			Error("Failed to parse LiteralFirst of LiteralContent.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParseLiteralFirst(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			while (true)
			{
				string value = ParseEmptyLineBlock(out success);
				if (success)
				{
					stringBuilder.Append(value);
					continue;
				}
				break;
			}
			success = true;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of LiteralFirst.");
				position = num;
				return stringBuilder.ToString();
			}
			int num2 = 0;
			while (true)
			{
				char value2 = ParseNonBreakChar(out success);
				if (success)
				{
					stringBuilder.Append(value2);
					num2++;
					continue;
				}
				break;
			}
			if (num2 > 0)
			{
				success = true;
			}
			if (!success)
			{
				Error("Failed to parse (NonBreakChar)+ of LiteralFirst.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return stringBuilder.ToString();
		}

		private string ParseLiteralInner(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseReservedLineBreak(out success);
			if (success)
			{
				stringBuilder.Append(value);
				while (true)
				{
					value = ParseEmptyLineBlock(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					break;
				}
				success = true;
				ParseIndent(out success);
				if (!success)
				{
					Error("Failed to parse Indent of LiteralInner.");
					position = num;
					return stringBuilder.ToString();
				}
				int num2 = 0;
				while (true)
				{
					char value2 = ParseNonBreakChar(out success);
					if (success)
					{
						stringBuilder.Append(value2);
						num2++;
						continue;
					}
					break;
				}
				if (num2 > 0)
				{
					success = true;
				}
				if (!success)
				{
					Error("Failed to parse (NonBreakChar)+ of LiteralInner.");
					position = num;
				}
				if (success)
				{
					ClearError(count);
				}
				return stringBuilder.ToString();
			}
			Error("Failed to parse ReservedLineBreak of LiteralInner.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParseFoldedLine(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of FoldedLine.");
				position = num;
				return stringBuilder.ToString();
			}
			while (true)
			{
				char value = ParseNonBreakChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
					continue;
				}
				break;
			}
			success = true;
			return stringBuilder.ToString();
		}

		private string ParseFoldedLines(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseFoldedLine(out success);
			if (success)
			{
				stringBuilder.Append(value);
				do
				{
					int num2 = position;
					string value2 = ParseLineFolding(out success);
					if (success)
					{
						stringBuilder.Append(value2);
						value2 = ParseFoldedLine(out success);
						if (success)
						{
							stringBuilder.Append(value2);
							continue;
						}
						Error("Failed to parse FoldedLine of FoldedLines.");
						position = num2;
					}
					else
					{
						Error("Failed to parse LineFolding of FoldedLines.");
					}
				}
				while (success);
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse str2 of FoldedLines.");
			position = num;
			return stringBuilder.ToString();
		}

		private string ParseSpacedLine(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of SpacedLine.");
				position = num;
				return stringBuilder.ToString();
			}
			ParseBlank(out success);
			if (!success)
			{
				Error("Failed to parse Blank of SpacedLine.");
				position = num;
				return stringBuilder.ToString();
			}
			while (true)
			{
				char value = ParseNonBreakChar(out success);
				if (success)
				{
					stringBuilder.Append(value);
					continue;
				}
				break;
			}
			success = true;
			return stringBuilder.ToString();
		}

		private string ParseSpacedLines(out bool success)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			string value = ParseSpacedLine(out success);
			if (success)
			{
				stringBuilder.Append(value);
				do
				{
					int num2 = position;
					ParseLineBreak(out success);
					if (!success)
					{
						Error("Failed to parse LineBreak of SpacedLines.");
						continue;
					}
					string value2 = ParseSpacedLine(out success);
					if (success)
					{
						stringBuilder.Append(value2);
						continue;
					}
					Error("Failed to parse SpacedLine of SpacedLines.");
					position = num2;
				}
				while (success);
				success = true;
				return stringBuilder.ToString();
			}
			Error("Failed to parse str2 of SpacedLines.");
			position = num;
			return stringBuilder.ToString();
		}

		private char ParseIndentIndicator(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalRange('1', '9', out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse '1'...'9' of IndentIndicator.");
			}
			return result;
		}

		private char ParseChompingIndicator(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = MatchTerminal('-', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = MatchTerminal('+', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private Sequence ParseFlowSequence(out bool success)
		{
			int count = Errors.Count;
			Sequence sequence = new Sequence();
			int num = position;
			MatchTerminal('[', out success);
			if (!success)
			{
				Error("Failed to parse '[' of FlowSequence.");
				position = num;
				return sequence;
			}
			ParseSeparationLinesInFlow(out success);
			success = true;
			DataItem item = ParseFlowSequenceEntry(out success);
			if (success)
			{
				sequence.Entries.Add(item);
				do
				{
					int num2 = position;
					MatchTerminal(',', out success);
					if (!success)
					{
						Error("Failed to parse ',' of FlowSequence.");
						continue;
					}
					ParseSeparationLinesInFlow(out success);
					success = true;
					item = ParseFlowSequenceEntry(out success);
					if (success)
					{
						sequence.Entries.Add(item);
						continue;
					}
					Error("Failed to parse FlowSequenceEntry of FlowSequence.");
					position = num2;
				}
				while (success);
				success = true;
			}
			else
			{
				Error("Failed to parse FlowSequenceEntry of FlowSequence.");
			}
			if (!success)
			{
				Error("Failed to parse Entries of FlowSequence.");
				position = num;
				return sequence;
			}
			MatchTerminal(']', out success);
			if (!success)
			{
				Error("Failed to parse ']' of FlowSequence.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return sequence;
		}

		private DataItem ParseFlowSequenceEntry(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			dataItem = ParseFlowNodeInFlow(out success);
			if (!success)
			{
				Error("Failed to parse FlowNodeInFlow of FlowSequenceEntry.");
			}
			else
			{
				ParseSeparationLinesInFlow(out success);
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			ParseFlowSingPair(out success);
			if (success)
			{
				ClearError(count);
				return dataItem;
			}
			return dataItem;
		}

		private Sequence ParseBlockSequence(out bool success)
		{
			int count = Errors.Count;
			Sequence sequence = new Sequence();
			DataItem item = ParseBlockSequenceEntry(out success);
			if (success)
			{
				sequence.Entries.Add(item);
				do
				{
					int num = position;
					ParseIndent(out success);
					if (!success)
					{
						Error("Failed to parse Indent of BlockSequence.");
						continue;
					}
					item = ParseBlockSequenceEntry(out success);
					if (success)
					{
						sequence.Entries.Add(item);
						continue;
					}
					Error("Failed to parse BlockSequenceEntry of BlockSequence.");
					position = num;
				}
				while (success);
				success = true;
			}
			else
			{
				Error("Failed to parse BlockSequenceEntry of BlockSequence.");
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse Entries of BlockSequence.");
			}
			return sequence;
		}

		private DataItem ParseBlockSequenceEntry(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			MatchTerminal('-', out success);
			if (!success)
			{
				Error("Failed to parse '-' of BlockSequenceEntry.");
				position = num;
				return result;
			}
			result = ParseBlockCollectionEntry(out success);
			if (!success)
			{
				Error("Failed to parse BlockCollectionEntry of BlockSequenceEntry.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return result;
		}

		private Mapping ParseFlowMapping(out bool success)
		{
			int count = Errors.Count;
			Mapping mapping = new Mapping();
			int num = position;
			MatchTerminal('{', out success);
			if (!success)
			{
				Error("Failed to parse '{' of FlowMapping.");
				position = num;
				return mapping;
			}
			ParseSeparationLinesInFlow(out success);
			success = true;
			MappingEntry item = ParseFlowMappingEntry(out success);
			if (success)
			{
				mapping.Entries.Add(item);
				do
				{
					int num2 = position;
					MatchTerminal(',', out success);
					if (!success)
					{
						Error("Failed to parse ',' of FlowMapping.");
						continue;
					}
					ParseSeparationLinesInFlow(out success);
					success = true;
					item = ParseFlowMappingEntry(out success);
					if (success)
					{
						mapping.Entries.Add(item);
						continue;
					}
					Error("Failed to parse FlowMappingEntry of FlowMapping.");
					position = num2;
				}
				while (success);
				success = true;
			}
			else
			{
				Error("Failed to parse FlowMappingEntry of FlowMapping.");
			}
			if (!success)
			{
				Error("Failed to parse Entries of FlowMapping.");
				position = num;
				return mapping;
			}
			MatchTerminal('}', out success);
			if (!success)
			{
				Error("Failed to parse '}' of FlowMapping.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return mapping;
		}

		private MappingEntry ParseFlowMappingEntry(out bool success)
		{
			int count = Errors.Count;
			MappingEntry mappingEntry = new MappingEntry();
			int num = position;
			mappingEntry.Key = ParseExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseExplicitValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of FlowMappingEntry.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			mappingEntry.Key = ParseExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseEmptyFlow(out success);
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			int num2 = position;
			mappingEntry.Key = ParseSimpleKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseExplicitValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of FlowMappingEntry.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			mappingEntry.Key = ParseSimpleKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseEmptyFlow(out success);
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			return mappingEntry;
		}

		private DataItem ParseExplicitKey(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			MatchTerminal('?', out success);
			if (!success)
			{
				Error("Failed to parse '?' of ExplicitKey.");
			}
			else
			{
				ParseSeparationLinesInFlow(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLinesInFlow of ExplicitKey.");
					position = num;
				}
				else
				{
					result = ParseFlowNodeInFlow(out success);
					if (!success)
					{
						Error("Failed to parse FlowNodeInFlow of ExplicitKey.");
						position = num;
					}
					else
					{
						ParseSeparationLinesInFlow(out success);
						success = true;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num2 = position;
			MatchTerminal('?', out success);
			if (!success)
			{
				Error("Failed to parse '?' of ExplicitKey.");
			}
			else
			{
				result = ParseEmptyFlow(out success);
				ParseSeparationLinesInFlow(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLinesInFlow of ExplicitKey.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private DataItem ParseSimpleKey(out bool success)
		{
			DataItem dataItem = null;
			int num = position;
			dataItem = ParseFlowKey(out success);
			if (!success)
			{
				Error("Failed to parse FlowKey of SimpleKey.");
				position = num;
				return dataItem;
			}
			ParseSeparationLinesInFlow(out success);
			success = true;
			return dataItem;
		}

		private Scalar ParseFlowKey(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = new Scalar();
			scalar.Text = ParsePlainTextInFlowSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseDoubleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseSingleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private Scalar ParseBlockKey(out bool success)
		{
			int count = Errors.Count;
			Scalar scalar = new Scalar();
			scalar.Text = ParsePlainTextSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseDoubleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			scalar.Text = ParseSingleQuotedSingleLine(out success);
			if (success)
			{
				ClearError(count);
				return scalar;
			}
			return scalar;
		}

		private DataItem ParseExplicitValue(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			MatchTerminal(':', out success);
			if (!success)
			{
				Error("Failed to parse ':' of ExplicitValue.");
			}
			else
			{
				ParseSeparationLinesInFlow(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLinesInFlow of ExplicitValue.");
					position = num;
				}
				else
				{
					result = ParseFlowNodeInFlow(out success);
					if (!success)
					{
						Error("Failed to parse FlowNodeInFlow of ExplicitValue.");
						position = num;
					}
					else
					{
						ParseSeparationLinesInFlow(out success);
						success = true;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num2 = position;
			MatchTerminal(':', out success);
			if (!success)
			{
				Error("Failed to parse ':' of ExplicitValue.");
			}
			else
			{
				result = ParseEmptyFlow(out success);
				ParseSeparationLinesInFlow(out success);
				if (!success)
				{
					Error("Failed to parse SeparationLinesInFlow of ExplicitValue.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}

		private MappingEntry ParseFlowSingPair(out bool success)
		{
			int count = Errors.Count;
			MappingEntry mappingEntry = new MappingEntry();
			int num = position;
			mappingEntry.Key = ParseExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowSingPair.");
			}
			else
			{
				mappingEntry.Value = ParseExplicitValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of FlowSingPair.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			mappingEntry.Key = ParseExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowSingPair.");
			}
			else
			{
				mappingEntry.Value = ParseEmptyFlow(out success);
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			int num2 = position;
			mappingEntry.Key = ParseSimpleKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of FlowSingPair.");
			}
			else
			{
				mappingEntry.Value = ParseExplicitValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of FlowSingPair.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			return mappingEntry;
		}

		private Mapping ParseBlockMapping(out bool success)
		{
			int count = Errors.Count;
			Mapping mapping = new Mapping();
			MappingEntry item = ParseBlockMappingEntry(out success);
			if (success)
			{
				mapping.Entries.Add(item);
				do
				{
					int num = position;
					ParseIndent(out success);
					if (!success)
					{
						Error("Failed to parse Indent of BlockMapping.");
						continue;
					}
					item = ParseBlockMappingEntry(out success);
					if (success)
					{
						mapping.Entries.Add(item);
						continue;
					}
					Error("Failed to parse BlockMappingEntry of BlockMapping.");
					position = num;
				}
				while (success);
				success = true;
			}
			else
			{
				Error("Failed to parse BlockMappingEntry of BlockMapping.");
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse Entries of BlockMapping.");
			}
			return mapping;
		}

		private MappingEntry ParseBlockMappingEntry(out bool success)
		{
			int count = Errors.Count;
			MappingEntry mappingEntry = new MappingEntry();
			int num = position;
			mappingEntry.Key = ParseBlockExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of BlockMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseBlockExplicitValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of BlockMappingEntry.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			mappingEntry.Key = ParseBlockExplicitKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of BlockMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseEmptyFlow(out success);
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			int num2 = position;
			mappingEntry.Key = ParseBlockSimpleKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of BlockMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseBlockSimpleValue(out success);
				if (!success)
				{
					Error("Failed to parse Value of BlockMappingEntry.");
					position = num2;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			int num3 = position;
			mappingEntry.Key = ParseBlockSimpleKey(out success);
			if (!success)
			{
				Error("Failed to parse Key of BlockMappingEntry.");
			}
			else
			{
				mappingEntry.Value = ParseEmptyBlock(out success);
				if (!success)
				{
					Error("Failed to parse Value of BlockMappingEntry.");
					position = num3;
				}
			}
			if (success)
			{
				ClearError(count);
				return mappingEntry;
			}
			return mappingEntry;
		}

		private DataItem ParseBlockExplicitKey(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			MatchTerminal('?', out success);
			if (!success)
			{
				Error("Failed to parse '?' of BlockExplicitKey.");
				position = num;
				return result;
			}
			result = ParseBlockCollectionEntry(out success);
			if (!success)
			{
				Error("Failed to parse BlockCollectionEntry of BlockExplicitKey.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return result;
		}

		private DataItem ParseBlockExplicitValue(out bool success)
		{
			int count = Errors.Count;
			DataItem result = null;
			int num = position;
			ParseIndent(out success);
			if (!success)
			{
				Error("Failed to parse Indent of BlockExplicitValue.");
				position = num;
				return result;
			}
			MatchTerminal(':', out success);
			if (!success)
			{
				Error("Failed to parse ':' of BlockExplicitValue.");
				position = num;
				return result;
			}
			result = ParseBlockCollectionEntry(out success);
			if (!success)
			{
				Error("Failed to parse BlockCollectionEntry of BlockExplicitValue.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return result;
		}

		private DataItem ParseBlockSimpleKey(out bool success)
		{
			int count = Errors.Count;
			DataItem dataItem = null;
			int num = position;
			dataItem = ParseBlockKey(out success);
			if (!success)
			{
				Error("Failed to parse BlockKey of BlockSimpleKey.");
				position = num;
				return dataItem;
			}
			ParseSeparationLines(out success);
			success = true;
			MatchTerminal(':', out success);
			if (!success)
			{
				Error("Failed to parse ':' of BlockSimpleKey.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return dataItem;
		}

		private DataItem ParseBlockSimpleValue(out bool success)
		{
			int count = Errors.Count;
			DataItem result = ParseBlockCollectionEntry(out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse BlockCollectionEntry of BlockSimpleValue.");
			}
			return result;
		}

		private void ParseComment(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			int num2 = position;
			success = !Input.HasInput(position);
			position = num2;
			success = !success;
			if (!success)
			{
				Error("Failed to parse !(eof) of Comment.");
				position = num;
				return;
			}
			ParseIgnoredSpace(out success);
			MatchTerminal('#', out success);
			if (!success)
			{
				Error("Failed to parse '#' of Comment.");
			}
			else
			{
				do
				{
					ParseNonBreakChar(out success);
				}
				while (success);
				success = true;
			}
			success = true;
			ErrorStatck.Push(count);
			count = Errors.Count;
			ParseLineBreak(out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				success = !Input.HasInput(position);
				if (success)
				{
					ClearError(count);
				}
			}
			count = ErrorStatck.Pop();
			if (!success)
			{
				Error("Failed to parse (LineBreak / eof) of Comment.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
		}

		private void ParseInlineComment(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			ParseSeparationSpace(out success);
			if (!success)
			{
				Error("Failed to parse SeparationSpace of InlineComment.");
			}
			else
			{
				MatchTerminal('#', out success);
				if (!success)
				{
					Error("Failed to parse '#' of InlineComment.");
				}
				else
				{
					do
					{
						ParseNonBreakChar(out success);
					}
					while (success);
					success = true;
				}
				success = true;
			}
			success = true;
			ErrorStatck.Push(count);
			count = Errors.Count;
			ParseLineBreak(out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				success = !Input.HasInput(position);
				if (success)
				{
					ClearError(count);
				}
			}
			count = ErrorStatck.Pop();
			if (!success)
			{
				Error("Failed to parse (LineBreak / eof) of InlineComment.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
		}

		private void ParseComments(out bool success)
		{
			int count = Errors.Count;
			int num = 0;
			while (true)
			{
				ParseComment(out success);
				if (!success)
				{
					break;
				}
				num++;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse (Comment)+ of Comments.");
			}
		}

		private void ParseInlineComments(out bool success)
		{
			int num = position;
			ParseInlineComment(out success);
			if (!success)
			{
				Error("Failed to parse InlineComment of InlineComments.");
				position = num;
				return;
			}
			do
			{
				ParseComment(out success);
			}
			while (success);
			success = true;
		}

		private string ParseInteger(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			List<char> list = new List<char>();
			int num = 0;
			while (true)
			{
				char item = ParseDigit(out success);
				if (success)
				{
					list.Add(item);
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
				return new string(list.ToArray());
			}
			Error("Failed to parse chars of Integer.");
			return stringBuilder.ToString();
		}

		private char ParseWordChar(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = ParseLetter(out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = ParseDigit(out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = MatchTerminal('-', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private char ParseLetter(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = MatchTerminalRange('a', 'z', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = MatchTerminalRange('A', 'Z', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private char ParseDigit(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalRange('0', '9', out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse '0'...'9' of Digit.");
			}
			return result;
		}

		private char ParseHexDigit(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = MatchTerminalRange('0', '9', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = MatchTerminalRange('A', 'F', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			c = MatchTerminalRange('a', 'f', out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private char ParseUriChar(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = ParseWordChar(out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			int num = position;
			MatchTerminal('%', out success);
			if (!success)
			{
				Error("Failed to parse '%' of UriChar.");
			}
			else
			{
				char c2 = ParseHexDigit(out success);
				if (!success)
				{
					Error("Failed to parse char1 of UriChar.");
					position = num;
				}
				else
				{
					char c3 = ParseHexDigit(out success);
					if (success)
					{
						c = Convert.ToChar(int.Parse(string.Format("{0}{1}", c2, c3), NumberStyles.HexNumber));
					}
					else
					{
						Error("Failed to parse char2 of UriChar.");
						position = num;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return c;
			}
			MatchTerminalSet(";/?:@&=+$,_.!~*'()[]", false, out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private char ParseTagChar(out bool success)
		{
			int count = Errors.Count;
			char c = '\0';
			c = ParseWordChar(out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			int num = position;
			MatchTerminal('%', out success);
			if (!success)
			{
				Error("Failed to parse '%' of TagChar.");
			}
			else
			{
				char c2 = ParseHexDigit(out success);
				if (!success)
				{
					Error("Failed to parse char1 of TagChar.");
					position = num;
				}
				else
				{
					char c3 = ParseHexDigit(out success);
					if (success)
					{
						c = Convert.ToChar(int.Parse(string.Format("{0}{1}", c2, c3), NumberStyles.HexNumber));
					}
					else
					{
						Error("Failed to parse char2 of TagChar.");
						position = num;
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return c;
			}
			MatchTerminalSet(";/?:@&=+$,_.~*'()[]", false, out success);
			if (success)
			{
				ClearError(count);
				return c;
			}
			return c;
		}

		private void ParseEmptyLinePlain(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			ParseIgnoredSpace(out success);
			ParseNormalizedLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse NormalizedLineBreak of EmptyLinePlain.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
		}

		private void ParseEmptyLineQuoted(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			ParseIgnoredBlank(out success);
			ParseNormalizedLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse NormalizedLineBreak of EmptyLineQuoted.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
		}

		private string ParseEmptyLineBlock(out bool success)
		{
			int count = Errors.Count;
			string text = null;
			int num = position;
			ParseIgnoredSpace(out success);
			text = ParseReservedLineBreak(out success);
			if (!success)
			{
				Error("Failed to parse ReservedLineBreak of EmptyLineBlock.");
				position = num;
			}
			if (success)
			{
				ClearError(count);
			}
			return text;
		}

		private char ParseNonSpaceChar(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalSet(" \t\r\n", true, out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse -\" \t\r\n\" of NonSpaceChar.");
			}
			return result;
		}

		private char ParseNonSpaceSep(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalSet("\r\n\t ,[]{}", true, out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse -\"\r\n\t ,[]{}\" of NonSpaceSep.");
			}
			return result;
		}

		private char ParseNonBreakChar(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalSet("\r\n", true, out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse -\"\r\n\" of NonBreakChar.");
			}
			return result;
		}

		private void ParseIgnoredSpace(out bool success)
		{
			do
			{
				MatchTerminal(' ', out success);
			}
			while (success);
			success = true;
		}

		private void ParseIgnoredBlank(out bool success)
		{
			do
			{
				MatchTerminalSet(" \t", false, out success);
			}
			while (success);
			success = true;
		}

		private void ParseSeparationSpace(out bool success)
		{
			int count = Errors.Count;
			int num = 0;
			while (true)
			{
				MatchTerminal(' ', out success);
				if (!success)
				{
					break;
				}
				num++;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse (' ')+ of SeparationSpace.");
			}
		}

		private void ParseSeparationLines(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			ParseInlineComments(out success);
			if (!success)
			{
				Error("Failed to parse InlineComments of SeparationLines.");
			}
			else
			{
				ParseIndent(out success);
				if (!success)
				{
					Error("Failed to parse Indent of SeparationLines.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return;
			}
			ParseSeparationSpace(out success);
			if (success)
			{
				ClearError(count);
			}
		}

		private void ParseSeparationLinesInFlow(out bool success)
		{
			int count = Errors.Count;
			int num = position;
			ParseInlineComments(out success);
			if (success)
			{
				detectIndent = false;
				ParseIndent(out success);
				if (!success)
				{
					Error("Failed to parse Indent of SeparationLinesInFlow.");
					position = num;
				}
				else
				{
					ParseIgnoredSpace(out success);
				}
			}
			else
			{
				Error("Failed to parse InlineComments of SeparationLinesInFlow.");
			}
			if (success)
			{
				ClearError(count);
				return;
			}
			ParseSeparationSpace(out success);
			if (success)
			{
				ClearError(count);
			}
		}

		private void ParseSeparationSpaceAsIndent(out bool success)
		{
			int count = Errors.Count;
			int num = 0;
			while (true)
			{
				MatchTerminal(' ', out success);
				if (success)
				{
					currentIndent++;
					num++;
					continue;
				}
				break;
			}
			if (num > 0)
			{
				success = true;
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse ((' '))+ of SeparationSpaceAsIndent.");
			}
		}

		private void ParseIndent(out bool success)
		{
			success = ParseIndent();
		}

		private char ParseSpace(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminal(' ', out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse ' ' of Space.");
			}
			return result;
		}

		private char ParseBlank(out bool success)
		{
			int count = Errors.Count;
			char result = MatchTerminalSet(" \t", false, out success);
			if (success)
			{
				ClearError(count);
			}
			else
			{
				Error("Failed to parse \" \t\" of Blank.");
			}
			return result;
		}

		private void ParseLineBreak(out bool success)
		{
			int count = Errors.Count;
			MatchTerminalString("\r\n", out success);
			if (success)
			{
				ClearError(count);
				return;
			}
			MatchTerminal('\r', out success);
			if (success)
			{
				ClearError(count);
				return;
			}
			MatchTerminal('\n', out success);
			if (success)
			{
				ClearError(count);
			}
		}

		private string ParseReservedLineBreak(out bool success)
		{
			int count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			string value = MatchTerminalString("\r\n", out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value);
				return stringBuilder.ToString();
			}
			char value2 = MatchTerminal('\r', out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value2);
				return stringBuilder.ToString();
			}
			value2 = MatchTerminal('\n', out success);
			if (success)
			{
				ClearError(count);
				stringBuilder.Append(value2);
				return stringBuilder.ToString();
			}
			return stringBuilder.ToString();
		}

		private string ParseChompedLineBreak(out bool success)
		{
			int count = Errors.Count;
			ErrorStatck.Push(count);
			count = Errors.Count;
			StringBuilder stringBuilder = new StringBuilder();
			string value = ParseReservedLineBreak(out success);
			if (success)
			{
				stringBuilder.Append(value);
				do
				{
					int num = position;
					ParseIgnoredSpace(out success);
					value = ParseReservedLineBreak(out success);
					if (success)
					{
						stringBuilder.Append(value);
						continue;
					}
					Error("Failed to parse ReservedLineBreak of ChompedLineBreak.");
					position = num;
				}
				while (success);
				success = true;
			}
			else
			{
				Error("Failed to parse ReservedLineBreak of ChompedLineBreak.");
			}
			if (success)
			{
				ClearError(count);
			}
			else
			{
				success = !Input.HasInput(position);
				if (success)
				{
					ClearError(count);
				}
			}
			count = ErrorStatck.Pop();
			return Chomp(stringBuilder.ToString());
		}

		private char ParseNormalizedLineBreak(out bool success)
		{
			int count = Errors.Count;
			char result = '\0';
			ParseLineBreak(out success);
			if (success)
			{
				ClearError(count);
				return '\n';
			}
			Error("Failed to parse LineBreak of NormalizedLineBreak.");
			return result;
		}

		private char ParseEscapeSequence(out bool success)
		{
			int count = Errors.Count;
			char result = '\0';
			MatchTerminalString("\\\\", out success);
			if (success)
			{
				return '\\';
			}
			MatchTerminalString("\\'", out success);
			if (success)
			{
				return '\'';
			}
			MatchTerminalString("\\\"", out success);
			if (success)
			{
				return '"';
			}
			MatchTerminalString("\\r", out success);
			if (success)
			{
				return '\r';
			}
			MatchTerminalString("\\n", out success);
			if (success)
			{
				return '\n';
			}
			MatchTerminalString("\\t", out success);
			if (success)
			{
				return '\t';
			}
			MatchTerminalString("\\v", out success);
			if (success)
			{
				return '\v';
			}
			MatchTerminalString("\\a", out success);
			if (success)
			{
				return '\a';
			}
			MatchTerminalString("\\b", out success);
			if (success)
			{
				return '\b';
			}
			MatchTerminalString("\\f", out success);
			if (success)
			{
				return '\f';
			}
			MatchTerminalString("\\0", out success);
			if (success)
			{
				return '\0';
			}
			MatchTerminalString("\\/", out success);
			if (success)
			{
				return '/';
			}
			MatchTerminalString("\\ ", out success);
			if (success)
			{
				return ' ';
			}
			MatchTerminalString("\\\t", out success);
			if (success)
			{
				return '\t';
			}
			MatchTerminalString("\\_", out success);
			if (success)
			{
				return '\u00a0';
			}
			MatchTerminalString("\\e", out success);
			if (success)
			{
				return '\u001b';
			}
			MatchTerminalString("\\N", out success);
			if (success)
			{
				return '\u0085';
			}
			MatchTerminalString("\\L", out success);
			if (success)
			{
				return '\u2028';
			}
			MatchTerminalString("\\P", out success);
			if (success)
			{
				return '\u2029';
			}
			int num = position;
			MatchTerminalString("\\x", out success);
			if (!success)
			{
				Error("Failed to parse '\\\\x' of EscapeSequence.");
			}
			else
			{
				char c = ParseHexDigit(out success);
				if (!success)
				{
					Error("Failed to parse char1 of EscapeSequence.");
					position = num;
				}
				else
				{
					char c2 = ParseHexDigit(out success);
					if (success)
					{
						return Convert.ToChar(int.Parse(string.Format("{0}{1}", c, c2), NumberStyles.HexNumber));
					}
					Error("Failed to parse char2 of EscapeSequence.");
					position = num;
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			int num2 = position;
			MatchTerminalString("\\u", out success);
			if (!success)
			{
				Error("Failed to parse '\\\\u' of EscapeSequence.");
			}
			else
			{
				char c3 = ParseHexDigit(out success);
				if (!success)
				{
					Error("Failed to parse char1 of EscapeSequence.");
					position = num2;
				}
				else
				{
					char c4 = ParseHexDigit(out success);
					if (!success)
					{
						Error("Failed to parse char2 of EscapeSequence.");
						position = num2;
					}
					else
					{
						char c5 = ParseHexDigit(out success);
						if (!success)
						{
							Error("Failed to parse char3 of EscapeSequence.");
							position = num2;
						}
						else
						{
							char c6 = ParseHexDigit(out success);
							if (success)
							{
								return Convert.ToChar(int.Parse(string.Format("{0}{1}{2}{3}", c3, c4, c5, c6), NumberStyles.HexNumber));
							}
							Error("Failed to parse char4 of EscapeSequence.");
							position = num2;
						}
					}
				}
			}
			if (success)
			{
				ClearError(count);
				return result;
			}
			return result;
		}
	}
}
