using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Mapping : Node
	{
		private YamlMappingNode _mapping;

		public List<Node> nodesInside { get; private set; }

		public Mapping(string HODKINDOEGD, Node[] KBFJEPICNNB)
		{
			base.typeNode = "Mapping";
			base.key = HODKINDOEGD;
			base.value = new YamlMappingNode(new YamlNode[0]);
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			foreach (Node node in KBFJEPICNNB)
			{
				_mapping.Add(node.key, node.value);
				nodesInside.Add(Node.AGFNPDIMEDI(node.key, node.value));
			}
		}

		public Mapping(string HODKINDOEGD, List<Node> KBFJEPICNNB)
			: this(HODKINDOEGD, KBFJEPICNNB.ToArray())
		{
			base.typeNode = "Mapping";
		}

		public Mapping(Mapping HELBCHMPMJP)
			: this(HELBCHMPMJP.key, (YamlMappingNode)HELBCHMPMJP.value)
		{
			base.typeNode = "Mapping";
		}

		public Mapping(string HODKINDOEGD, YamlMappingNode JPEFEBICPFI)
		{
			base.typeNode = "Mapping";
			base.key = HODKINDOEGD;
			base.value = JPEFEBICPFI;
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			foreach (KeyValuePair<YamlNode, YamlNode> item in _mapping)
			{
				nodesInside.Add(Node.AGFNPDIMEDI(item.Key.ToString(), item.Value));
			}
		}

		public int NMALBAPGAFM()
		{
			return nodesInside.Count;
		}

		public Node GetNodesByIndex(int index)
		{
			if (index < nodesInside.Count)
			{
				return nodesInside[index];
			}
			return null;
		}

		public List<Node> DBPKFJFFKPC()
		{
			return nodesInside;
		}

		public void Add(Node OABDMNKEMFL)
		{
			_mapping.Add(OABDMNKEMFL.key, OABDMNKEMFL.value);
			nodesInside.Add(OABDMNKEMFL);
		}

		public void AddNodes(Node[] HBGCCCIABFC)
		{
			foreach (Node node in HBGCCCIABFC)
			{
				_mapping.Add(node.key, node.value);
				nodesInside.Add(node);
			}
		}

		public void Remove(string KGBGENDIMBC, string value)
		{
			foreach (Node item in nodesInside)
			{
				if (item.key == KGBGENDIMBC && item.value.ToString() == value)
				{
					nodesInside.Remove(item);
					_mapping.Remove(KGBGENDIMBC, item.value);
					break;
				}
			}
		}

		public void Remove(Node HNIJHBJPEIA)
		{
			if (HNIJHBJPEIA != null)
			{
				Remove(HNIJHBJPEIA.key, HNIJHBJPEIA.value.ToString());
			}
		}

		public Mapping GetMapping(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode yamlNode = _mapping.GetNode(name);
			Type type = yamlNode.GetType();
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)yamlNode);
			}
			return null;
		}

		public Sequence GetSequence(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			Sequence result = null;
			foreach (Node item in nodesInside)
			{
				if (item.key.Equals(name) && item is Sequence)
				{
					result = (Sequence)item;
					break;
				}
			}
			return result;
		}

		public Scalar GetText(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode yamlNode = _mapping.GetNode(name);
			Type type = yamlNode.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)yamlNode);
			}
			return null;
		}

		public Node GetNode(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode yamlNode = _mapping.GetNode(name);
			Type type = yamlNode.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)yamlNode);
			}
			if (type == typeof(YamlSequenceNode))
			{
				return new Sequence(name, (YamlSequenceNode)yamlNode);
			}
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)yamlNode);
			}
			return null;
		}

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}
	}
}
