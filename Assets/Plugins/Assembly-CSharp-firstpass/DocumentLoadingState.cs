using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

internal class DocumentLoadingState
{
	private readonly IDictionary<string, YamlNode> anchors = new Dictionary<string, YamlNode>();

	private readonly IList<YamlNode> nodesWithUnresolvedAliases = new List<YamlNode>();

	public void BOKLCAFFHOD(YamlNode node)
	{
		if (node.Anchor == null)
		{
			throw new ArgumentException("The specified node does not have an anchor");
		}
		if (anchors.ContainsKey(node.Anchor))
		{
			throw new DuplicateAnchorException(node.Start, node.End, string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' already exists", node.Anchor));
		}
		anchors.Add(node.Anchor, node);
	}

	public YamlNode GetNode(string KOLNNNLOCFE, bool MIPDMNEJOCI, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
	{
		YamlNode value;
		if (anchors.TryGetValue(KOLNNNLOCFE, out value))
		{
			return value;
		}
		if (MIPDMNEJOCI)
		{
			throw new AnchorNotFoundException(ILENLCMAMBH, PCLFFOBJJFO, string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", KOLNNNLOCFE));
		}
		return null;
	}

	public void GOGDMGMHFOK(YamlNode node)
	{
		nodesWithUnresolvedAliases.Add(node);
	}

	public void GPBMMFCHANP()
	{
		foreach (YamlNode item in nodesWithUnresolvedAliases)
		{
			item.GPBMMFCHANP(this);
		}
	}
}
