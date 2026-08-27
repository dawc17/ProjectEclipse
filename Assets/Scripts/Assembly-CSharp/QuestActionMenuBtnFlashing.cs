using System.Xml;
using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI.Arrows;
using Nekki.SF2.GUI.Dialogs;
using Nekki.SF2.GUI.Menu;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using UnityEngine.UI;

public class QuestActionMenuBtnFlashing : QuestAction
{
	private string _btnName = string.Empty;

	private string resolvedScreenName = string.Empty;

	private SectionButton targetButton;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_btnName = EPKLCPOEELO.Attributes["BtnName"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		// Keep the quest parameters alive until the scroll finishes opening.  The
		// destination is usually a roster variable (for example _NextScene), and
		// resolving it later without base initialization turns it into numeric 0.
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		resolvedScreenName = ResolveScreenName(GFIHPBCEEOB);
		MainMenu.get_Instance().CloseMenu();
		TutorialCanvas.get_Instance().set_BlockOn(true);
		Button scrollBtn = MainMenu.get_Instance().GetScrollBtn();
		TutorialComponent component = scrollBtn.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = true;
		Button skipBtn = MainMenu.get_Instance().GetSkipBtn();
		TutorialComponent tutorialComponent = ((!(skipBtn != null)) ? null : skipBtn.gameObject.GetComponent<TutorialComponent>());
		if (tutorialComponent != null)
		{
			tutorialComponent.IsActive = true;
			skipBtn.onClick.AddListener(JPMFAFMCCLP);
		}
		ArrowCanvas.get_Instance().ShowArrow(new Vector3(scrollBtn.transform.position.x, scrollBtn.transform.position.y - 50f, 0f));
		MainMenu.get_Instance().Scroll.AddEventListener(3, MGCEBAICAJG);
		MainMenu.get_Instance().Scroll.AddEventListener(0, JDMLJBADBBI);
	}

	private void JPMFAFMCCLP()
	{
		Button skipBtn = MainMenu.get_Instance().GetSkipBtn();
		skipBtn.onClick.RemoveListener(JPMFAFMCCLP);
		MGCEBAICAJG(0);
		JDMLJBADBBI(0);
		OAJKIJGCLMJ();
	}

	private void MGCEBAICAJG(object data)
	{
		MainMenu.get_Instance().Scroll.RemoveEventListener(3, MGCEBAICAJG);
		ArrowCanvas.get_Instance().HideArrow();
	}

	private void JDMLJBADBBI(object data)
	{
		MainMenu.get_Instance().Scroll.RemoveEventListener(0, JDMLJBADBBI);
		NotificationsGame.CloseNotifications();
		Button scrollBtn = MainMenu.get_Instance().GetScrollBtn();
		TutorialComponent component = scrollBtn.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = false;
		string screenName = resolvedScreenName;
		ScreenType cCGJDFLIKFN;
		switch (screenName)
		{
		case "Dojo":
		case "Map":
		case "Shop":
		case "Profile":
			cCGJDFLIKFN = Module.DFDEMKONNKK(screenName);
			break;
		default:
			Debug.LogWarning("[Tutorial] MenuBtnFlashing could not resolve target '" + _btnName + "' (value '" + screenName + "'); releasing tutorial lock.");
			OAJKIJGCLMJ();
			return;
		}
		targetButton = MainMenu.get_Instance().GetButtonFromScreen(cCGJDFLIKFN);
		if (targetButton == null)
		{
			Debug.LogWarning("[Tutorial] Menu button is unavailable for " + screenName + "; releasing tutorial lock.");
			OAJKIJGCLMJ();
			return;
		}
		targetButton.set_IsFlashing(true);
		targetButton.onClick.AddListener(OAJKIJGCLMJ);
		TutorialComponent component2 = targetButton.gameObject.GetComponent<TutorialComponent>();
		if (component2 == null)
		{
			Debug.LogWarning("[Tutorial] Menu button for " + screenName + " has no TutorialComponent; releasing tutorial lock.");
			OAJKIJGCLMJ();
			return;
		}
		component2.IsActive = true;
	}

	private string ResolveScreenName(QuestParameters parameters)
	{
		ConditionExtension.CompareResult result = new ConditionExtension.CompareResult();
		QuestCondition condition = new QuestCondition();
		condition.LIMHBJBEEIA(parameters);
		condition.MCPIOGALBMK(_btnName, result);
		string screenName = result.ToString();
		if (IsMenuScreen(screenName))
		{
			return screenName;
		}
		// The migrated tutorial stores its destination in this roster variable.
		// Read it directly as a second source so an evaluator regression cannot
		// silently turn a string destination into zero again.
		if (_btnName == "_NextScene")
		{
			RosterQuest.NOKCOAHJIPB variable = ListSF.CCDKHLAMKKO().PFMIBOCGGPC(_btnName);
			if (variable != null && IsMenuScreen(variable.Value))
			{
				return variable.Value;
			}
			string tutorialStep = ListSF.CCDKHLAMKKO().BKBHIMEEDBG().JILGHNPIHME();
			switch (tutorialStep)
			{
			case "STEP_BUY_ITEM":
				return "Shop";
			case "MAP":
			case "END":
				return "Map";
			case "SHOW_DOUBLE_SWEEP":
				return "Dojo";
			case "SHOW_BLOCK":
				return "Profile";
			}
		}
		return screenName;
	}

	private static bool IsMenuScreen(string screenName)
	{
		return screenName == "Dojo" || screenName == "Map" || screenName == "Shop" || screenName == "Profile";
	}

	private void OAJKIJGCLMJ()
	{
		if (targetButton != null)
		{
			targetButton.onClick.RemoveListener(OAJKIJGCLMJ);
			targetButton.set_IsFlashing(false);
			TutorialComponent component = targetButton.gameObject.GetComponent<TutorialComponent>();
			if (component != null)
			{
				component.IsActive = false;
			}
			targetButton = null;
		}
		TutorialCanvas.get_Instance().set_BlockOn(false);
		OGIJONMKABB();
	}
}
