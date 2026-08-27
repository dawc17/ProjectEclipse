using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

public sealed class YamlNodeIdentityEqualityComparer : IEqualityComparer<YamlNode>
{
	public bool Equals(YamlNode DHDMNHCIPEH, YamlNode BGEEALIPKCC)
	{
		return object.ReferenceEquals(DHDMNHCIPEH, BGEEALIPKCC);
	}

	public int GetHashCode(YamlNode AOMLCBHAJJH)
	{
		return AOMLCBHAJJH.GetHashCode();
	}
}
