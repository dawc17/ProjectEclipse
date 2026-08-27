using System;

public sealed class CKPADCLAMFF : INodeTypeResolver
{
	bool INodeTypeResolver.Resolve(NodeEvent ABOEBNGCALL, ref Type PHOBEGPKAKH)
	{
		if (!string.IsNullOrEmpty(ABOEBNGCALL.LOIGCKFONHJ()))
		{
			PHOBEGPKAKH = Type.GetType(ABOEBNGCALL.LOIGCKFONHJ().Substring(1), true);
			return true;
		}
		return false;
	}
}
