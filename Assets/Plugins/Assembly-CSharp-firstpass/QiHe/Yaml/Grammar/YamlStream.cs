using System.Collections.Generic;

namespace QiHe.Yaml.Grammar
{
	public class YamlStream
	{
		public List<YamlDocument> Documents = new List<YamlDocument>();

		public Mapping GetRoot(int docNum = 0)
		{
			return (Mapping)Documents[docNum].Root;
		}
	}
}
