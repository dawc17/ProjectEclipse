using System;
using System.Collections.Generic;

public sealed class TagNodeTypeResolver : INodeTypeResolver
{
	private readonly IDictionary<string, Type> NKEHCGOLJDA;

	public TagNodeTypeResolver(IDictionary<string, Type> NKEHCGOLJDA)
	{
		if (NKEHCGOLJDA == null)
		{
			throw new ArgumentNullException("tagMappings");
		}
		this.NKEHCGOLJDA = NKEHCGOLJDA;
	}

	bool INodeTypeResolver.Resolve(NodeEvent ABOEBNGCALL, ref Type PHOBEGPKAKH)
	{
		Type value;
		if (!string.IsNullOrEmpty(ABOEBNGCALL.LOIGCKFONHJ()) && NKEHCGOLJDA.TryGetValue(ABOEBNGCALL.LOIGCKFONHJ(), out value))
		{
			PHOBEGPKAKH = value;
			return true;
		}
		return false;
	}
}
