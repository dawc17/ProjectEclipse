using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public class YamlScalarNode : YamlNode
	{
		public string Value { get; set; }

		public IBEOFCPMMJJ Style { get; set; }

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
			}
		}

		internal YamlScalarNode(EventReader DNBFFLFBDOB, DocumentLoadingState state)
		{
			Scalar lEACOCDHICF = DNBFFLFBDOB.DODGGCGJJLL<Scalar>();
			Load(lEACOCDHICF, state);
			Value = lEACOCDHICF.OEAKCOHMIHH();
			Style = lEACOCDHICF.HALCJLMJDII();
		}

		public YamlScalarNode()
		{
		}

		public YamlScalarNode(string value)
		{
			Value = value;
		}

		internal override void GPBMMFCHANP(DocumentLoadingState state)
		{
			throw new NotSupportedException("Resolving an alias on a scalar node does not make sense");
		}

		internal override void Emit(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state)
		{
			NPIDIMCLNEM.Emit(new Scalar(base.Anchor, base.Tag, Value, Style, true, false));
		}

		public override void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM)
		{
			NKECMANOOEM.Visit(this);
		}

		public override bool Equals(object NOLFMPDGCOC)
		{
			YamlScalarNode yamlScalarNode = NOLFMPDGCOC as YamlScalarNode;
			return yamlScalarNode != null && Equals(yamlScalarNode) && YamlNode.SafeEquals(Value, yamlScalarNode.Value);
		}

		public override int GetHashCode()
		{
			return YamlNode.CombineHashCodes(base.GetHashCode(), YamlNode.AOJHKEDINCA(Value));
		}

		[SpecialName]
		public static YamlScalarNode op_Implicit(string value)
		{
			return new YamlScalarNode(value);
		}

		[SpecialName]
		public static string op_Explicit(YamlScalarNode value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return Value;
		}

		public override YamlNode Clone()
		{
			return new YamlScalarNode(Value);
		}
	}
}
