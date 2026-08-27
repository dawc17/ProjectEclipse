using System;
using System.Collections;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public abstract class Node : IEnumerable
	{
		public string key { get; protected set; }

		public YamlNode value { get; protected set; }

		public string typeNode { get; protected set; }

		public override string ToString()
		{
			return value.ToString();
		}

		public string PLGAAMADMEE()
		{
			return typeNode;
		}

		public string GetKey()
		{
			return key;
		}

		public static Node AGFNPDIMEDI(string OEHJKBNMJPH, YamlNode DOBDLPLFMAC)
		{
			Type type = DOBDLPLFMAC.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(OEHJKBNMJPH, (YamlScalarNode)DOBDLPLFMAC);
			}
			if (type == typeof(YamlSequenceNode))
			{
				return new Sequence(OEHJKBNMJPH, (YamlSequenceNode)DOBDLPLFMAC);
			}
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(OEHJKBNMJPH, (YamlMappingNode)DOBDLPLFMAC);
			}
			return null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			yield return this;
		}
	}
}
