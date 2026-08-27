using System.Collections.Generic;

namespace QiHe.Yaml.Grammar
{
	public class YamlDocument
	{
		public Dictionary<string, DataItem> AnchoredItems = new Dictionary<string, DataItem>();

		public DataItem Root;

		public List<Directive> Directives = new List<Directive>();
	}
}
