namespace UnityEngine.Purchasing
{
	[AddComponentMenu("")]
	public class DemoInventory : MonoBehaviour
	{
		public void Fulfill(string EEHFHCFPCEM)
		{
			if (EEHFHCFPCEM != null && EEHFHCFPCEM == "100.gold.coins")
			{
				Debug.Log("You Got Money!");
			}
			else
			{
				Debug.Log(string.Format("Unrecognized productId \"{0}\"", EEHFHCFPCEM));
			}
		}
	}
}
