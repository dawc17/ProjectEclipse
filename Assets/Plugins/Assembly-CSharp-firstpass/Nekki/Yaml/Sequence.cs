using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Sequence : Node
	{
		private YamlSequenceNode _sequence;

		public List<Node> nodesInside { get; private set; }

		public Sequence(string HODKINDOEGD, YamlSequenceNode GJPGKAEFMPI)
		{
			base.typeNode = "Sequence";
			base.key = HODKINDOEGD;
			base.value = GJPGKAEFMPI;
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			foreach (YamlNode item in _sequence)
			{
				nodesInside.Add(Node.AGFNPDIMEDI(base.key, item));
			}
		}

		public Sequence(string HODKINDOEGD, Node node)
		{
			base.typeNode = "Sequence";
			base.key = HODKINDOEGD;
			base.value = new YamlSequenceNode(new YamlNode[0]);
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			HCPJPEGIJIK(node);
			foreach (YamlNode item in _sequence)
			{
				nodesInside.Add(Node.AGFNPDIMEDI(base.key, item));
			}
		}

		public Sequence(string HODKINDOEGD, Node[] AHBBBPNEMMM)
		{
			base.typeNode = "Sequence";
			base.key = HODKINDOEGD;
			base.value = new YamlSequenceNode(new YamlNode[0]);
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			AddNodes(AHBBBPNEMMM);
		}

		public Sequence(string HODKINDOEGD, List<Node> AHBBBPNEMMM)
			: this(HODKINDOEGD, AHBBBPNEMMM.ToArray())
		{
			base.typeNode = "Sequence";
		}

		public void HPLACOBNDLN(int index, Node PHEPOJIKDPN)
		{
			_sequence.UpdateNode(nodesInside[index].value, PHEPOJIKDPN.value);
			nodesInside[index] = PHEPOJIKDPN;
		}

		public void Replace(List<Node> HBGCCCIABFC)
		{
			foreach (Node item in nodesInside)
			{
				_sequence.Remove(item.value);
			}
			foreach (Node item2 in HBGCCCIABFC)
			{
				_sequence.Add(item2.value);
			}
			nodesInside = HBGCCCIABFC;
		}

		public void Remove(Node PHEPOJIKDPN)
		{
			_sequence.Remove(PHEPOJIKDPN.value);
			nodesInside.Remove(PHEPOJIKDPN);
		}

		public void HCPJPEGIJIK(Node PHEPOJIKDPN)
		{
			_sequence.Add(PHEPOJIKDPN.value);
			nodesInside.Add(PHEPOJIKDPN);
		}

		public void AddNodes(Node[] HBGCCCIABFC)
		{
			foreach (Node node in HBGCCCIABFC)
			{
				_sequence.Add(node.value);
				nodesInside.Add(node);
			}
		}

		public void AddNodes(List<Node> HBGCCCIABFC)
		{
			foreach (Node item in HBGCCCIABFC)
			{
				_sequence.Add(item.value);
				nodesInside.Add(item);
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

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}
	}
}
