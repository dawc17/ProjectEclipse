using System.Collections.Generic;
using QiHe.CodeLib;

namespace QiHe.Yaml.Grammar
{
	public class Mapping : DataItem
	{
		public List<MappingEntry> Entries = new List<MappingEntry>();

		public MappingEntry GetNode(string name)
		{
			return Entries.Find((MappingEntry x) => x.Key is Scalar && ((Scalar)x.Key).Text == name);
		}

		public string GetText(string name)
		{
			MappingEntry node = GetNode(name);
			if (node != null)
			{
				return ((Scalar)node.Value).Text;
			}
			return null;
		}

		public Mapping GetMapping(string name)
		{
			MappingEntry node = GetNode(name);
			if (node != null)
			{
				return (Mapping)node.Value;
			}
			return null;
		}

		public Sequence GetSequence(string name)
		{
			MappingEntry node = GetNode(name);
			if (node != null)
			{
				return (Sequence)node.Value;
			}
			return null;
		}

		public void AddNode(MappingEntry node)
		{
			Entries.Add(node);
		}

		public void AddNode(string name, DataItem value)
		{
			MappingEntry mappingEntry = new MappingEntry();
			Scalar scalar = new Scalar();
			scalar.Text = name;
			mappingEntry.Key = scalar;
			mappingEntry.Value = value;
			AddNode(mappingEntry);
		}

		public void AddNode(string name, string value)
		{
			Scalar scalar = new Scalar();
			scalar.Text = value;
			AddNode(name, scalar);
		}

		public void SetNode(string name, DataItem value)
		{
			MappingEntry node = GetNode(name);
			if (node != null)
			{
				node.Value = value;
			}
			else
			{
				AddNode(name, value);
			}
		}

		public void SetNode(string name, string value)
		{
			Scalar scalar = new Scalar();
			scalar.Text = value;
			SetNode(name, scalar);
		}

		public override string ToString()
		{
			return " ( " + StringHelper.ContactWithDelim(Entries, ",") + " ) ";
		}
	}
}
