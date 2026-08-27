using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;
using UnityEngine;

public class TutorialComponent : SFMonoBehaviour<object>
{
	[SerializeField]
	public bool IsActive;

	private void Start()
	{
	}

	private void Update()
	{
		if (TutorialCanvas.get_Instance().get_BlockOn() && IsActive && Input.GetMouseButtonUp(0))
		{
			Vector2 vector = Input.mousePosition;
			Vector3 position = base.transform.position;
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			Vector2 vector2 = Vector2.Scale(component.rect.size, component.lossyScale);
			float x = component.position.x + component.anchoredPosition.x;
			float y = (float)Screen.height - component.position.y - component.anchoredPosition.y;
			Rect rect = new Rect(x, y, vector2.x, vector2.y);
			if (vector.x >= position.x - rect.width / 2f && vector.x <= position.x + rect.width / 2f && vector.y >= position.y - rect.height / 2f && vector.y <= position.y + rect.height / 2f)
			{
				InvokeClick();
			}
		}
	}

	public virtual void InvokeClick()
	{
	}
}
