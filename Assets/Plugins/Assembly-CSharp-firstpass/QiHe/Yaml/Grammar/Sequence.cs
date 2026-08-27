using System.Collections.Generic;
using QiHe.CodeLib;

namespace QiHe.Yaml.Grammar
{
	public class Sequence : DataItem
	{
		public List<DataItem> Entries = new List<DataItem>();

		public void AddNode(DataItem node)
		{
			Entries.Add(node);
		}

		public void AddNode(string value)
		{
			Scalar scalar = new Scalar();
			scalar.Text = value;
			AddNode(scalar);
		}

		public override string ToString()
		{
			return " [ " + StringHelper.ContactWithDelim(Entries, ",") + " ] ";
		}
	}
}
