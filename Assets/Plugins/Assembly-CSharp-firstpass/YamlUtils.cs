using Nekki.Yaml;
using UnityEngine;

public class YamlUtils
{
	public static Vector2 GPKICIBEBJD(Sequence KKMBOLHOLLP)
	{
		Vector2 vector = default(Vector2);
		vector = Vector2.zero;
		if (KKMBOLHOLLP != null)
		{
			AdvLog.Log(string.Format("<{0}>", KKMBOLHOLLP.GetType()));
			vector.x = float.Parse(((Nekki.Yaml.Scalar)KKMBOLHOLLP.nodesInside[0]).text);
			vector.y = float.Parse(((Nekki.Yaml.Scalar)KKMBOLHOLLP.nodesInside[1]).text);
		}
		return vector;
	}
}
