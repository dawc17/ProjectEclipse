using System;
using System.Collections.Generic;
using YamlDotNet.Core;

public sealed class NodeValueDeserializer : FFBEMOKFDNL
{
	private readonly IList<INodeDeserializer> JJAHDOOOGFH;

	private readonly IList<INodeTypeResolver> FFEOGMPHPBI;

	public NodeValueDeserializer(IList<INodeDeserializer> JJAHDOOOGFH, IList<INodeTypeResolver> FFEOGMPHPBI)
	{
		if (JJAHDOOOGFH == null)
		{
			throw new ArgumentNullException("deserializers");
		}
		this.JJAHDOOOGFH = JJAHDOOOGFH;
		if (FFEOGMPHPBI == null)
		{
			throw new ArgumentNullException("typeResolvers");
		}
		this.FFEOGMPHPBI = FFEOGMPHPBI;
	}

	public object BBNMBCMJOFM(EventReader reader, Type MBLGNMBFHBI, SerializerState state, FFBEMOKFDNL IJBAEAEDMCC)
	{
		NodeEvent dGMPGIHHKCN = reader.Peek<NodeEvent>();
		Type mBLGNMBFHBI = KFGNLMDILOC(dGMPGIHHKCN, MBLGNMBFHBI);
		try
		{
			foreach (INodeDeserializer item in JJAHDOOOGFH)
			{
				object value;
				if (item.Deserialize(reader, mBLGNMBFHBI, (EventReader BOPODEAIEBJ, Type GNAONAPDDLD) => IJBAEAEDMCC.BBNMBCMJOFM(BOPODEAIEBJ, GNAONAPDDLD, state, IJBAEAEDMCC), out value))
				{
					return value;
				}
			}
		}
		catch (YamlException)
		{
			throw;
		}
		catch (Exception oLABPFGLNFC)
		{
			throw new YamlException(dGMPGIHHKCN.OGPHJPFHBJL(), dGMPGIHHKCN.GDJHIJHFPHA(), "Exception during deserialization", oLABPFGLNFC);
		}
		throw new YamlException(dGMPGIHHKCN.OGPHJPFHBJL(), dGMPGIHHKCN.GDJHIJHFPHA(), string.Format("No node deserializer was able to deserialize the node into type {0}", MBLGNMBFHBI.AssemblyQualifiedName));
	}

	private Type KFGNLMDILOC(NodeEvent ABOEBNGCALL, Type PHOBEGPKAKH)
	{
		foreach (INodeTypeResolver item in FFEOGMPHPBI)
		{
			if (item.Resolve(ABOEBNGCALL, ref PHOBEGPKAKH))
			{
				break;
			}
		}
		return PHOBEGPKAKH;
	}
}
