using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	internal class YamlAliasNode : YamlNode
	{
		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
			}
		}

		internal YamlAliasNode(string KOLNNNLOCFE)
		{
			base.Anchor = KOLNNNLOCFE;
		}

		internal override void GPBMMFCHANP(DocumentLoadingState state)
		{
			throw new NotSupportedException("Resolving an alias on an alias node does not make sense");
		}

		internal override void Emit(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state)
		{
			throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be saved.");
		}

		public override void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM)
		{
			throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be visited.");
		}

		public override bool Equals(object NOLFMPDGCOC)
		{
			YamlAliasNode yamlAliasNode = NOLFMPDGCOC as YamlAliasNode;
			return yamlAliasNode != null && Equals(yamlAliasNode) && YamlNode.SafeEquals(base.Anchor, yamlAliasNode.Anchor);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return "*" + base.Anchor;
		}

		public override YamlNode Clone()
		{
			return new YamlAliasNode(base.Anchor);
		}
	}
}
