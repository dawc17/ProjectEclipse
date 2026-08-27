using System.Collections.Generic;
using UnityEngine;

public class WideScreenController : MonoBehaviour
{
	[SerializeField]
	private GameObject _LeftBorder;

	[SerializeField]
	private GameObject _RightBorder;

	[SerializeField]
	private bool _Flag;

	public void Run()
	{
		float num = 1920f;
		float num2 = 1080f;
		float num3 = num / num2;
		float num4 = (float)Screen.width / (float)Screen.height;
		if (num4 <= num3)
		{
			_LeftBorder.SetActive(false);
			_RightBorder.SetActive(false);
			return;
		}
		(base.transform as RectTransform).SetSiblingIndex(1000);
		RectTransform rectTransform = (RectTransform)base.gameObject.transform.parent;
		float num5 = 1536f / num2 * num;
		BEDKFGIICFL(num5);
		float pPOFNJGPHGP = (float)(int)((rectTransform.sizeDelta.x - num5) / 2f) + 1f;
		List<RectTransform> list = new List<RectTransform>();
		for (int i = 0; i < rectTransform.childCount; i++)
		{
			Transform child = rectTransform.GetChild(i);
			if (!(child == base.transform))
			{
				RectTransform component = child.GetComponent<RectTransform>();
				if (component != null)
				{
					list.Add(component);
					Run(component, pPOFNJGPHGP);
				}
			}
		}
	}

	private void Run(RectTransform PIDLNECOJBG, float PPOFNJGPHGP)
	{
		Vector2 offsetMax = PIDLNECOJBG.offsetMax;
		offsetMax.x = 0f - PPOFNJGPHGP;
		PIDLNECOJBG.offsetMax = offsetMax;
		Vector2 offsetMin = PIDLNECOJBG.offsetMin;
		offsetMin.x = PPOFNJGPHGP;
		PIDLNECOJBG.offsetMin = offsetMin;
	}

	private void BEDKFGIICFL(float DJFFDCFCNJM)
	{
		float num = (int)(DJFFDCFCNJM / 2f) - 1;
		Vector3 localPosition = _LeftBorder.transform.localPosition;
		localPosition.x = 0f - num;
		_LeftBorder.transform.localPosition = localPosition;
		localPosition = _RightBorder.transform.localPosition;
		localPosition.x = num;
		_RightBorder.transform.localPosition = localPosition;
	}

	private void Update()
	{
		if (_Flag)
		{
			_Flag = false;
			Run();
		}
	}
}
