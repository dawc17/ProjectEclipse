using System.Globalization;
using UnityEngine;

public class Vector3D
{
	public static Vector3 GLPCCFLBEPG(Vector3 LHBNIMGFKIB, Vector3 AAOIAEJJINO, float ratio)
	{
		return new Vector3(LHBNIMGFKIB.x + (AAOIAEJJINO.x - LHBNIMGFKIB.x) * ratio, LHBNIMGFKIB.y + (AAOIAEJJINO.y - LHBNIMGFKIB.y) * ratio, LHBNIMGFKIB.z + (AAOIAEJJINO.z - LHBNIMGFKIB.z) * ratio);
	}

	public static Vector3 HOCIAKOFPON(string NDPLLLKGKGO)
	{
		string[] array = NDPLLLKGKGO.Split(' ');
		if (array.Length < 3)
		{
			AdvLog.CCOFFJPPAKC("Wrong Vector3 string!!! - " + NDPLLLKGKGO);
			return Vector3.zero;
		}
		return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
	}
}
