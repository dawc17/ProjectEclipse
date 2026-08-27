using System.Collections.Generic;

namespace QiHe.Yaml.Grammar
{
	public class ReservedDirective : Directive
	{
		public string Name;

		public List<string> Parameters = new List<string>();
	}
}
