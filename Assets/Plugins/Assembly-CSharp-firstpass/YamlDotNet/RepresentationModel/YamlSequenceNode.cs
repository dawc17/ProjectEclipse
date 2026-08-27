using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	[DebuggerDisplay("Count = {children.Count}")]
	public class YamlSequenceNode : YamlNode, IEnumerable<YamlNode>, IEnumerable
	{
		private readonly IList<YamlNode> children = new List<YamlNode>();

		public IList<YamlNode> Children
		{
			get
			{
				return children;
			}
		}

		public NBCBGEPFIKG Style { get; set; }

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
				foreach (YamlNode child in children)
				{
					foreach (YamlNode allNode in child.AllNodes)
					{
						yield return allNode;
					}
				}
			}
		}

		internal YamlSequenceNode(EventReader DNBFFLFBDOB, DocumentLoadingState state)
		{
			JODGINIKFJF cAJDINOLOJH = DNBFFLFBDOB.DODGGCGJJLL<JODGINIKFJF>();
			Load(cAJDINOLOJH, state);
			bool flag = false;
			while (!DNBFFLFBDOB.GPHIFFOGOGN<AKMKLAINLOL>())
			{
				YamlNode yamlNode = YamlNode.GLNMJNFLLIN(DNBFFLFBDOB, state);
				children.Add(yamlNode);
				flag |= yamlNode is YamlAliasNode;
			}
			if (flag)
			{
				state.GOGDMGMHFOK(this);
			}
			DNBFFLFBDOB.DODGGCGJJLL<AKMKLAINLOL>();
		}

		public YamlSequenceNode()
		{
		}

		public YamlSequenceNode(params YamlNode[] IPCFHFNBMIC)
			: this((IEnumerable<YamlNode>)IPCFHFNBMIC)
		{
		}

		public YamlSequenceNode(IEnumerable<YamlNode> IPCFHFNBMIC)
		{
			foreach (YamlNode item in IPCFHFNBMIC)
			{
				children.Add(item);
			}
		}

		public void Add(YamlNode BFEBLBKODLK)
		{
			children.Add(BFEBLBKODLK);
		}

		public void Add(string BFEBLBKODLK)
		{
			children.Add(new YamlScalarNode(BFEBLBKODLK));
		}

		public void Remove(YamlNode BFEBLBKODLK)
		{
			foreach (YamlNode child in children)
			{
				if (child == BFEBLBKODLK)
				{
					children.Remove(child);
					break;
				}
			}
		}

		public void Replace(YamlNode LAAGOLBMEKP, Predicate<YamlNode> DFIECHGNEPK)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (DFIECHGNEPK(children[i]))
				{
					children[i] = LAAGOLBMEKP;
					break;
				}
			}
		}

		public void UpdateNode(YamlNode KLDACECBHOJ, YamlNode BFEBLBKODLK)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] == KLDACECBHOJ)
				{
					children[i] = BFEBLBKODLK;
					break;
				}
			}
		}

		internal override void GPBMMFCHANP(DocumentLoadingState state)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] is YamlAliasNode)
				{
					children[i] = state.GetNode(children[i].Anchor, true, children[i].Start, children[i].End);
				}
			}
		}

		internal override void Emit(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state)
		{
			NPIDIMCLNEM.Emit(new JODGINIKFJF(base.Anchor, base.Tag, true, Style));
			foreach (YamlNode child in children)
			{
				child.Save(NPIDIMCLNEM, state);
			}
			NPIDIMCLNEM.Emit(new AKMKLAINLOL());
		}

		public override void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM)
		{
			NKECMANOOEM.Visit(this);
		}

		public override bool Equals(object NOLFMPDGCOC)
		{
			YamlSequenceNode yamlSequenceNode = NOLFMPDGCOC as YamlSequenceNode;
			if (yamlSequenceNode == null || !Equals(yamlSequenceNode) || children.Count != yamlSequenceNode.children.Count)
			{
				return false;
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (!YamlNode.SafeEquals(children[i], yamlSequenceNode.children[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (YamlNode child in children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.AOJHKEDINCA(child));
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			foreach (YamlNode child in children)
			{
				stringBuilder.Append(child);
				stringBuilder.Append(", ");
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public IEnumerator<YamlNode> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override YamlNode Clone()
		{
			YamlSequenceNode yamlSequenceNode = new YamlSequenceNode();
			foreach (YamlNode child in Children)
			{
				yamlSequenceNode.Add(child.Clone());
			}
			return yamlSequenceNode;
		}
	}
}
