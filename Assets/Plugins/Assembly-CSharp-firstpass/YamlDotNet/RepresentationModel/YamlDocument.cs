using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlDocument
	{
		private class OMDIOFCAGOO : YamlVisitor
		{
			private readonly HashSet<string> existingAnchors = new HashSet<string>();

			private readonly Dictionary<YamlNode, bool> visitedNodes = new Dictionary<YamlNode, bool>(new YamlNodeIdentityEqualityComparer());

			public void AssignAnchors(YamlDocument DPMKHPJABAF)
			{
				existingAnchors.Clear();
				visitedNodes.Clear();
				DPMKHPJABAF.GPHIFFOGOGN(this);
				Random random = new Random();
				foreach (KeyValuePair<YamlNode, bool> item in visitedNodes)
				{
					if (item.Value)
					{
						string text;
						do
						{
							text = random.Next().ToString(CultureInfo.InvariantCulture);
						}
						while (existingAnchors.Contains(text));
						existingAnchors.Add(text);
						item.Key.Anchor = text;
					}
				}
			}

			private void VisitNode(YamlNode node)
			{
				if (string.IsNullOrEmpty(node.Anchor))
				{
					bool value;
					if (visitedNodes.TryGetValue(node, out value))
					{
						if (!value)
						{
							visitedNodes[node] = true;
						}
					}
					else
					{
						visitedNodes.Add(node, false);
					}
				}
				else
				{
					existingAnchors.Add(node.Anchor);
				}
			}

			protected override void Visit(YamlScalarNode ADDIBOMFCNH)
			{
				VisitNode(ADDIBOMFCNH);
			}

			protected override void Visit(YamlMappingNode JPEFEBICPFI)
			{
				VisitNode(JPEFEBICPFI);
			}

			protected override void Visit(YamlSequenceNode sequence)
			{
				VisitNode(sequence);
			}
		}

		public YamlNode RootNode { get; private set; }

		public IEnumerable<YamlNode> AllNodes
		{
			get
			{
				return RootNode.AllNodes;
			}
		}

		public YamlDocument(YamlNode FFMAKDIFLAN)
		{
			RootNode = FFMAKDIFLAN;
		}

		public YamlDocument(string FFMAKDIFLAN)
		{
			RootNode = new YamlScalarNode(FFMAKDIFLAN);
		}

		internal YamlDocument(EventReader DNBFFLFBDOB)
		{
			DocumentLoadingState jPGMAPEHLAB = new DocumentLoadingState();
			DNBFFLFBDOB.DODGGCGJJLL<DocumentStart>();
			while (!DNBFFLFBDOB.GPHIFFOGOGN<DocumentEnd>())
			{
				RootNode = YamlNode.GLNMJNFLLIN(DNBFFLFBDOB, jPGMAPEHLAB);
				if (RootNode is YamlAliasNode)
				{
					throw new YamlException();
				}
			}
			jPGMAPEHLAB.GPBMMFCHANP();
			DNBFFLFBDOB.DODGGCGJJLL<DocumentEnd>();
		}

		private void AssignAnchors()
		{
			OMDIOFCAGOO oMDIOFCAGOO = new OMDIOFCAGOO();
			oMDIOFCAGOO.AssignAnchors(this);
		}

		internal void Save(NEKGJNOFOFN NPIDIMCLNEM, bool EENMGCCBIHF = true)
		{
			if (EENMGCCBIHF)
			{
				AssignAnchors();
			}
			NPIDIMCLNEM.Emit(new DocumentStart());
			RootNode.Save(NPIDIMCLNEM, new EmitterState());
			NPIDIMCLNEM.Emit(new DocumentEnd(false));
		}

		public void GPHIFFOGOGN(IYamlVisitor NKECMANOOEM)
		{
			NKECMANOOEM.Visit(this);
		}
	}
}
