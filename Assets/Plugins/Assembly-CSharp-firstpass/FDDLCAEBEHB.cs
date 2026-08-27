using System;
using System.Collections.Generic;

public sealed class FDDLCAEBEHB : INodeTypeResolver
{
	bool INodeTypeResolver.Resolve(NodeEvent ABOEBNGCALL, ref Type PHOBEGPKAKH)
	{
		if (PHOBEGPKAKH == typeof(object))
		{
			if (ABOEBNGCALL is JODGINIKFJF)
			{
				PHOBEGPKAKH = typeof(List<object>);
				return true;
			}
			if (ABOEBNGCALL is MappingStart)
			{
				PHOBEGPKAKH = typeof(Dictionary<object, object>);
				return true;
			}
		}
		return false;
	}
}
