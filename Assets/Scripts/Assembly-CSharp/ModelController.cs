using System.Collections.Generic;

public class ModelController : global::EventDispatcher<object>
{
	public enum FJBMMEFKCCD
	{
		onKeyPressed = 0,
		onKeyReleased = 1
	}

	private class KeyInfo
	{
		public int EDEEELJMHLG;

		public int Index;

		public bool Press;
	}

	private const float MCONTROLLER_COMBO_INTERVAL = 0.25f;

	private const int JDLMJGIBBMI = 30;

	private const int MHHLELDHCKA = 2;

	private const int GCGJJNMLPJM = 150;

	private List<KeyInfo> KNKFKGPKBPK = new List<KeyInfo>();

	private int LICAFNLFJHO;

	private KeyData AOFMFOHIKHF = new KeyData();

	private KeyData JKLJBOOOKOP = new KeyData();

	private int MDAFKNLGJBJ;

	public KeyData FONEJOKEIEN
	{
		get
		{
			return ANALKHBJKIO();
		}
		set
		{
			IPGLLIAHDPE(value);
		}
	}

	public ModelController()
	{
		LICAFNLFJHO = 0;
		MDAFKNLGJBJ = 0;
		KNKFKGPKBPK.Capacity = 150;
		for (int i = 0; i < 150; i++)
		{
			KeyInfo hFBBKFECOBD = new KeyInfo();
			hFBBKFECOBD.EDEEELJMHLG = (hFBBKFECOBD.Index = i + 1);
			hFBBKFECOBD.Press = false;
			KNKFKGPKBPK.Add(hFBBKFECOBD);
		}
	}

	public KeyData ANALKHBJKIO()
	{
		return AOFMFOHIKHF;
	}

	public void IPGLLIAHDPE(KeyData value)
	{
		AOFMFOHIKHF = value.Copy();
		JKLJBOOOKOP = value.Copy();
	}

	public void Render()
	{
		if (LICAFNLFJHO == 30)
		{
			AOFMFOHIKHF.Clear();
			LICAFNLFJHO = 0;
		}
		if ((float)MDAFKNLGJBJ >= 15f)
		{
			AOFMFOHIKHF.IGEEOAGOMEM.Clear();
			MDAFKNLGJBJ = 0;
		}
		SetAdditional();
		MDAFKNLGJBJ++;
		LICAFNLFJHO++;
	}

	public void Reset()
	{
		JKLJBOOOKOP.Clear();
		AOFMFOHIKHF.Clear();
		AOFMFOHIKHF.IGEEOAGOMEM.Clear();
		int i = 0;
		for (int count = KNKFKGPKBPK.Count; i < count; i++)
		{
			KNKFKGPKBPK[i].Press = false;
		}
	}

	public void OnPressAnyKey(int KJPGKHJNOMC)
	{
		KeyInfo hFBBKFECOBD = GetKey(KJPGKHJNOMC);
		if (hFBBKFECOBD != null && !hFBBKFECOBD.Press)
		{
			hFBBKFECOBD.Press = true;
			MDAFKNLGJBJ = 0;
			AOFMFOHIKHF.IGEEOAGOMEM.Add(hFBBKFECOBD.Index);
			while (AOFMFOHIKHF.IGEEOAGOMEM.Count > 2)
			{
				AOFMFOHIKHF.IGEEOAGOMEM.RemoveAt(0);
			}
			AOFMFOHIKHF.Clear();
			SetAdditional();
			AOFMFOHIKHF.HGPMABCJGGN = GetPressType(AOFMFOHIKHF.IGEEOAGOMEM, AOFMFOHIKHF.CEPODJDDLBF);
			CallKeyPressed();
		}
	}

	public void OnReleaseAnyKey(int KJPGKHJNOMC)
	{
		KeyInfo hFBBKFECOBD = GetKey(KJPGKHJNOMC);
		if (hFBBKFECOBD == null)
		{
			return;
		}
		hFBBKFECOBD.Press = false;
		if (!AOFMFOHIKHF.IGEEOAGOMEM.Contains(hFBBKFECOBD.Index))
		{
			int num = AOFMFOHIKHF.CEPODJDDLBF.IndexOf(hFBBKFECOBD.Index);
			if (AOFMFOHIKHF.CEPODJDDLBF.Contains(hFBBKFECOBD.Index))
			{
				AOFMFOHIKHF.HPEOJLAMIHC.Add(hFBBKFECOBD.Index);
				AOFMFOHIKHF.CEPODJDDLBF.Remove(hFBBKFECOBD.Index);
				CallKeyReleased();
			}
		}
	}

	public KeyData GetKeyDataBySign(int AOJJBKLCHJO)
	{
		JKLJBOOOKOP.Set(AOFMFOHIKHF);
		return JKLJBOOOKOP;
	}

	public void CallKeyPressed()
	{
		CallEvent(0, AOFMFOHIKHF);
	}

	public void CallKeyReleased()
	{
		CallEvent(1, AOFMFOHIKHF);
	}

	private KeyInfo GetKey(int HDKKKCDKFEE)
	{
		foreach (KeyInfo item in KNKFKGPKBPK)
		{
			if (item.EDEEELJMHLG == HDKKKCDKFEE)
			{
				return item;
			}
		}
		return null;
	}

	private void SetAdditional()
	{
		AOFMFOHIKHF.CEPODJDDLBF.Clear();
		foreach (KeyInfo item in KNKFKGPKBPK)
		{
			if (item.Press)
			{
				AOFMFOHIKHF.CEPODJDDLBF.Add(item.Index);
			}
		}
	}

	private KeyData.MCIDLLKHKDE GetPressType(List<int> AKFEAJDLIKF, List<int> LFKEMJBCMFL)
	{
		if (AKFEAJDLIKF.Count == 1)
		{
			return KeyData.MCIDLLKHKDE.BOTH;
		}
		foreach (int item in AKFEAJDLIKF)
		{
			foreach (int item2 in LFKEMJBCMFL)
			{
				if (item == item2)
				{
					return KeyData.MCIDLLKHKDE.BOTH;
				}
			}
		}
		return KeyData.MCIDLLKHKDE.SEQUENCE;
	}
}
