using UnityEngine.UI;

public class TutorialButton : TutorialComponent
{
	public override void InvokeClick()
	{
		Button component = base.gameObject.GetComponent<Button>();
		component.onClick.Invoke();
	}
}
