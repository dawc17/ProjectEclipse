using UnityEngine.UI;

public class TutorialToggle : TutorialComponent
{
	public override void InvokeClick()
	{
		Toggle component = base.gameObject.GetComponent<Toggle>();
		component.onValueChanged.Invoke(false);
	}
}
