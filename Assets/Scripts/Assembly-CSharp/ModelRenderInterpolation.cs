using UnityEngine;

// Render-only interpolation for the custom fixed-step fight model. Simulation,
// collision, AI and attack timing continue to read ModelNode's current pose.
public static class ModelRenderInterpolation
{
	public static float CalculateAlpha(double renderTime, double fixedTime, float fixedDeltaTime)
	{
		if (fixedDeltaTime <= 0f || double.IsNaN(renderTime) || double.IsNaN(fixedTime))
		{
			return 1f;
		}
		return Mathf.Clamp01((float)((renderTime - fixedTime) / fixedDeltaTime));
	}

	public static void GetPosition(ModelNode node, out float x, out float y, out float z)
	{
		Vector3f current = node.ICLEOFDKDIF();
		Vector3f previous = node.FOGHEPNAPLC();
		float alpha = CalculateAlpha(Time.timeAsDouble, Time.fixedTimeAsDouble, Time.fixedDeltaTime);
		x = Mathf.Lerp(previous.GILCBJJPKBK(), current.GILCBJJPKBK(), alpha);
		y = Mathf.Lerp(previous.OBIMBNIBEFG(), current.OBIMBNIBEFG(), alpha);
		z = Mathf.Lerp(previous.KMFEKANLCFO(), current.KMFEKANLCFO(), alpha);
	}
}
