using System;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFButton : Button, global::IEventDispatcher<object>
{
	public enum HGNNIDPFKCN
	{
		OnPress = 0,
		OnRelease = 1,
		OnClick = 2,
		OnDoubleClick = 3,
		OnActiveOpacity = 4,
		OnTouchBegin = 5
	}

	private global::EventDispatcher<object> NBKJBIIPPNB = new global::EventDispatcher<object>();

	[SerializeField]
	public ResolutionImage FlashingImage;

	private bool MGNLBNLCDAI;

	private int HCMOIDIJNMD;

	private int DMEAFBMAGDH = 10;

	private bool CANIGBPEKFA;

	public bool IsOneShot;

	public int ButtonId = -1;

	public bool BLNHFKLOPBF
	{
		get
		{
			return get_IsFlashing();
		}
		set
		{
			set_IsFlashing(value);
		}
	}

	public bool get_IsFlashing()
	{
		return CANIGBPEKFA;
	}

	public void set_IsFlashing(bool value)
	{
		CANIGBPEKFA = value;
		if (FlashingImage != null)
		{
			FlashingImage.gameObject.SetActive(CANIGBPEKFA);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.onClick.AddListener(() =>
		{
			if (IsOneShot)
			{
				base.interactable = false;
			}
		});
	}

	private new void OnDestroy()
	{
		if (base.onClick != null)
		{
			base.onClick.RemoveAllListeners();
		}
	}

	public int AddEventListener(int name, Action<object> ODDEOFKLIAG)
	{
		return NBKJBIIPPNB.AddEventListener(name, ODDEOFKLIAG);
	}

	public int CallEvent(int name, object EHCLMBADLKH)
	{
		return (!base.interactable) ? 1 : NBKJBIIPPNB.CallEvent(name, EHCLMBADLKH);
	}

	public int RemoveAllEventListener()
	{
		return NBKJBIIPPNB.RemoveAllEventListener();
	}

	public int RemoveEvent(int name)
	{
		return NBKJBIIPPNB.RemoveEvent(name);
	}

	public int RemoveEventListener(int name, Action<object> ODDEOFKLIAG)
	{
		return NBKJBIIPPNB.RemoveEventListener(name, ODDEOFKLIAG);
	}

	public override void OnPointerDown(PointerEventData BHOLFGOGPCP)
	{
		base.OnPointerDown(BHOLFGOGPCP);
		CallEvent(0, ButtonId);
	}

	public override void OnPointerUp(PointerEventData BHOLFGOGPCP)
	{
		base.OnPointerUp(BHOLFGOGPCP);
		CallEvent(1, ButtonId);
	}

	public override void OnPointerClick(PointerEventData BHOLFGOGPCP)
	{
		base.OnPointerClick(BHOLFGOGPCP);
		CallEvent(2, ButtonId);
	}

	private void Update()
	{
		if (!CANIGBPEKFA || !(FlashingImage != null))
		{
			return;
		}
		FlashingImage.color = new Color(FlashingImage.color.r, FlashingImage.color.g, FlashingImage.color.b, (float)HCMOIDIJNMD / 255f);
		if (MGNLBNLCDAI)
		{
			if (HCMOIDIJNMD < 250)
			{
				HCMOIDIJNMD += DMEAFBMAGDH;
				return;
			}
			MGNLBNLCDAI = false;
			if (HCMOIDIJNMD > 250)
			{
				HCMOIDIJNMD = 250;
			}
		}
		else if (HCMOIDIJNMD > 0)
		{
			HCMOIDIJNMD -= DMEAFBMAGDH;
		}
		else
		{
			MGNLBNLCDAI = true;
			if (HCMOIDIJNMD < 0)
			{
				HCMOIDIJNMD = 0;
			}
		}
	}

	public void AddFlashImage(string JGIGOMLGLPN)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "FlashingImage";
		gameObject.transform.SetParent(base.transform, false);
		FlashingImage = gameObject.AddComponent<ResolutionImage>();
		FlashingImage.set_SpriteName(JGIGOMLGLPN);
		FlashingImage.SetNativeSize();
	}
}
