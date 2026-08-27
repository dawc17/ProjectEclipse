using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlMappingNode : YamlNode, IEnumerable<KeyValuePair<YamlNode, YamlNode>>, IEnumerable
	{
		[Serializable]
		public class BoxEqVolume : EqualityComparer<YamlNode>
		{
			public override int GetHashCode(YamlNode FLFFACBBLNM)
			{
				return FLFFACBBLNM.GetHashCode();
			}

			public override bool Equals(YamlNode NMAJNHKJJEM, YamlNode ONNJMGGPHEL)
			{
				return NMAJNHKJJEM.Equals(ONNJMGGPHEL);
			}
		}

		private IDictionary<YamlNode, YamlNode> children = new Dictionary<YamlNode, YamlNode>(new BoxEqVolume());

		public IDictionary<YamlNode, YamlNode> Children
		{
			get
			{
				return children;
			}
		}

		public FGDKNBEFPFN Style { get; set; }

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
				foreach (KeyValuePair<YamlNode, YamlNode> child in children)
				{
					foreach (YamlNode allNode in child.Key.AllNodes)
					{
						yield return allNode;
					}
					foreach (YamlNode allNode2 in child.Value.AllNodes)
					{
						yield return allNode2;
					}
				}
			}
		}

		internal YamlMappingNode(EventReader DNBFFLFBDOB, DocumentLoadingState state)
		{
			MappingStart cAJDINOLOJH = DNBFFLFBDOB.DODGGCGJJLL<MappingStart>();
			Load(cAJDINOLOJH, state);
			bool flag = false;
			while (!DNBFFLFBDOB.GPHIFFOGOGN<BLFPJCPALDH>())
			{
				YamlNode yamlNode = YamlNode.GLNMJNFLLIN(DNBFFLFBDOB, state);
				YamlNode yamlNode2 = YamlNode.GLNMJNFLLIN(DNBFFLFBDOB, state);
				try
				{
					children.Add(yamlNode, yamlNode2);
				}
				catch (ArgumentException oLABPFGLNFC)
				{
					throw new YamlException(yamlNode.Start, yamlNode.End, "Duplicate key", oLABPFGLNFC);
				}
				flag |= yamlNode is YamlAliasNode || yamlNode2 is YamlAliasNode;
			}
			if (flag)
			{
				state.GOGDMGMHFOK(this);
			}
			DNBFFLFBDOB.DODGGCGJJLL<BLFPJCPALDH>();
		}

		public YamlMappingNode()
		{
		}

		public YamlMappingNode(params KeyValuePair<YamlNode, YamlNode>[] IPCFHFNBMIC)
			: this((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)IPCFHFNBMIC)
		{
		}

		public YamlMappingNode(IEnumerable<KeyValuePair<YamlNode, YamlNode>> IPCFHFNBMIC)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> item in IPCFHFNBMIC)
			{
				children.Add(item);
			}
		}

		public YamlMappingNode(params YamlNode[] IPCFHFNBMIC)
			: this((IEnumerable<YamlNode>)IPCFHFNBMIC)
		{
		}

		public YamlMappingNode(IEnumerable<YamlNode> IPCFHFNBMIC)
		{
			using (IEnumerator<YamlNode> enumerator = IPCFHFNBMIC.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					YamlNode current = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						throw new ArgumentException("When constructing a mapping node with a sequence, the number of elements of the sequence must be even.");
					}
					Add(current, enumerator.Current);
				}
			}
		}

		public void Remove(string KGBGENDIMBC, YamlNode value)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (child.Key.ToString() == KGBGENDIMBC && child.Value == value)
				{
					children.Remove(child.Key);
					break;
				}
			}
		}

		public void Add(YamlNode KGBGENDIMBC, YamlNode value)
		{
			children.Add(KGBGENDIMBC, value);
		}

		public void Add(string KGBGENDIMBC, YamlNode value)
		{
			children.Add(new YamlScalarNode(KGBGENDIMBC), value);
		}

		public void Add(YamlNode KGBGENDIMBC, string value)
		{
			children.Add(KGBGENDIMBC, new YamlScalarNode(value));
		}

		public void Add(string KGBGENDIMBC, string value)
		{
			children.Add(new YamlScalarNode(KGBGENDIMBC), new YamlScalarNode(value));
		}

		public bool HasKey(string KGBGENDIMBC)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (((YamlScalarNode)child.Key).Value == KGBGENDIMBC)
				{
					return true;
				}
			}
			return false;
		}

		public YamlNode GetNode(string LLCIAOMCJBG)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (((YamlScalarNode)child.Key).Value == LLCIAOMCJBG)
				{
					return child.Value;
				}
			}
			return null;
		}

		internal override void GPBMMFCHANP(DocumentLoadingState state)
		{
			Dictionary<YamlNode, YamlNode> dictionary = null;
			Dictionary<YamlNode, YamlNode> dictionary2 = null;
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (child.Key is YamlAliasNode)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary.Add(child.Key, state.GetNode(child.Key.Anchor, true, child.Key.Start, child.Key.End));
				}
				if (child.Value is YamlAliasNode)
				{
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary2.Add(child.Key, state.GetNode(child.Value.Anchor, true, child.Value.Start, child.Value.End));
				}
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<YamlNode, YamlNode> item in dictionary2)
				{
					children[item.Key] = item.Value;
				}
			}
			if (dictionary == null)
			{
				return;
			}
			foreach (KeyValuePair<YamlNode, YamlNode> item2 in dictionary)
			{
				YamlNode value = children[item2.Key];
				children.Remove(item2.Key);
				children.Add(item2.Value, value);
			}
		}

		internal override void Emit(NEKGJNOFOFN NPIDIMCLNEM, EmitterState state)
		{
			NPIDIMCLNEM.Emit(new MappingStart(base.Anchor, base.Tag, true, Style));
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				child.Key.Save(NPIDIMCLNEM, state);
				child.Value.Save(NPIDIMCLNEM, state);
			}
			NPIDIMCLNEM.Emit(new BLFPJCPALDH());
		}

		public override void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM)
		{
			NKECMANOOEM.Visit(this);
		}

		public override bool Equals(object NOLFMPDGCOC)
		{
			YamlMappingNode yamlMappingNode = NOLFMPDGCOC as YamlMappingNode;
			if (yamlMappingNode == null || !Equals(yamlMappingNode) || children.Count != yamlMappingNode.children.Count)
			{
				return false;
			}
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				YamlNode value;
				if (!yamlMappingNode.children.TryGetValue(child.Key, out value) || !YamlNode.SafeEquals(child.Value, value))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.AOJHKEDINCA(child.Key));
				num = YamlNode.CombineHashCodes(num, YamlNode.AOJHKEDINCA(child.Value));
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(child.Key).Append(": ").Append(child.Value);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public IEnumerator<KeyValuePair<YamlNode, YamlNode>> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override YamlNode Clone()
		{
			YamlMappingNode yamlMappingNode = new YamlMappingNode(new YamlNode[0]);
			foreach (KeyValuePair<YamlNode, YamlNode> child in Children)
			{
				yamlMappingNode.Add(child.Key.ToString(), child.Value.Clone());
			}
			return yamlMappingNode;
		}
	}
}
