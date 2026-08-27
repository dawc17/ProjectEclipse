using UnityEngine;

public class WideController : MonoBehaviour
{
	[SerializeField]
	private GameObject _Wide;

	[SerializeField]
	private GameObject _NotWide;

	private void Awake()
	{
		float num = 1.7753906f;
		float num2 = Screen.width / Screen.height;
		if (num2 < num)
		{
			_NotWide.SetActive(true);
		}
		else
		{
			_Wide.SetActive(true);
		}
	}
}
