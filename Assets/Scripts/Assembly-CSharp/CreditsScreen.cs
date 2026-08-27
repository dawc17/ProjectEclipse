using System;
using System.Xml;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsScreen : SFMonoBehaviour<object>, IPointerClickHandler, IEventSystemHandler, BackKeyController
{
	public Transform content;

	public CreditsScreenElement elementPrefab;

	public ScrollRect scrollRect;

	public float autoScrollSpeed = 0.01f;

	private Action PIKDIOFIJDK;

	public static CreditsScreen Create(Action OCLNBMKHLMH = null)
	{
		CreditsScreen original = Resources.Load<CreditsScreen>("Prefabs/Credits/CreditsScreen");
		original = UnityEngine.Object.Instantiate(original);
		original.PIKDIOFIJDK = OCLNBMKHLMH;
		return original;
	}

	public void OnPointerClick(PointerEventData BHOLFGOGPCP)
	{
		Hide();
	}

	private void NGHDCFJJKKI()
	{
		string text = SF2Paths.KKIDGPBOBNI() + "/credits/";
		text += ((!(LocalizationManager.ILAJKOBCHFH.name == "rus")) ? "eng.xml" : "rus.xml");
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(text, string.Empty);
		XmlNode xmlNode = xmlDocument["Credits"];
		foreach (XmlNode item in xmlNode)
		{
			string kNNEDNHONBJ = item.Attributes["Name"].CIPOICEEIBK();
			string innerText = item.InnerText;
			CreditsScreenElement creditsScreenElement = UnityEngine.Object.Instantiate(elementPrefab, content, false);
			creditsScreenElement.Init(kNNEDNHONBJ, innerText);
		}
	}

	public void Hide()
	{
		if (PIKDIOFIJDK != null)
		{
			PIKDIOFIJDK();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		NGHDCFJJKKI();
		BackKeyManager.get_Instance().AddBackKeyController(this);
	}

	private void OnDestroy()
	{
		BackKeyManager.get_Instance().RemoveBackKeyController(this);
	}

	private void Update()
	{
		scrollRect.verticalNormalizedPosition += autoScrollSpeed * Time.deltaTime;
		if (scrollRect.verticalNormalizedPosition <= 0f)
		{
			Hide();
		}
	}

	public void OnBackKeyClicked(object GHDPPHAAPCA)
	{
		Hide();
	}
}
