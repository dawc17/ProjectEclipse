using System;
using System.Diagnostics;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class ActivePerkItem : MonoBehaviour, IComparable<ActivePerkItem>
	{
		[SerializeField]
		private ResolutionImage _icon;

		[SerializeField]
		private ResolutionImage _expiration;

		private const float NAKDPGBCFCD = 1f;

		private const float AIGOJMDDPDE = 0f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string HKGHEJDKCPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PerksStage.ActionPerk PMCIKHJONNM;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool NCGLMKOGBCB;

		private bool MGIIAEFNAIM;

		private bool IAOPDCPAELB;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool GPNIIJGOMKC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float FEALIIMKDPN;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float LNAHKGCNGLB;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool IMLKFJDOOOM;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int IGAPOPAFKON;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int APIJKCAIHED;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool JHLCLPEGHKP;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool MIKNOJCLEIM;

		public PerksStage.ActionPerk AMKJNPOCODK
		{
			get
			{
				return get_Action();
			}
			private set
			{
				AHBNPODMIOD(value);
			}
		}

		public bool IEEIFCFIGAD
		{
			get
			{
				return get_NeedDelete();
			}
			private set
			{
				set_NeedDelete(value);
			}
		}

		public int KGNDJOLBBJF
		{
			get
			{
				return get_CurrentFrames();
			}
		}

		public int FLNLMIHEDCI
		{
			get
			{
				return get_TotalFrames();
			}
		}

		public bool OIHFMOHEBLC
		{
			get
			{
				return get_Show();
			}
			set
			{
				set_Show(value);
			}
		}

		public bool FLNCPBKBJBL
		{
			get
			{
				return get_ShowExpiration();
			}
			set
			{
				set_ShowExpiration(value);
			}
		}

		public bool FPEPBOBIMAD
		{
			get
			{
				return get_ChangeOpacity();
			}
			private set
			{
				DIGBNJOGMOH(value);
			}
		}

		public float CIAFHNKKCCG
		{
			get
			{
				return get_CurrentIconOpacity();
			}
			private set
			{
				OBPMKMGNABJ(value);
			}
		}

		private float CFFPHNNEPAI
		{
			get
			{
				return OOAHAJJKHKI();
			}
			set
			{
				LKHJKOOFAAN(value);
			}
		}

		private bool NOEDAJCKMIB
		{
			get
			{
				return IAGEBCJCFEF();
			}
			set
			{
				CJLDJACPNLO(value);
			}
		}

		private int NJKAGHEFHLF
		{
			get
			{
				return ACMGBPEBGMD();
			}
			set
			{
				LLCEMCONIBK(value);
			}
		}

		public int HNICOKEANPK
		{
			get
			{
				return get_PulseCount();
			}
			set
			{
				set_PulseCount(value);
			}
		}

		public bool IHBPKFEPJIJ
		{
			get
			{
				return get_DeleteRequested();
			}
			private set
			{
				GFKHNLGGGJH(value);
			}
		}

		private bool HCLJGEPIEJN
		{
			get
			{
				return HKKGNLIFPDD();
			}
			set
			{
				LGACCOFOCNL(value);
			}
		}

		public int HKEICGDHLNH
		{
			get
			{
				return get_FramesToEnd();
			}
		}

		public string get_Name()
		{
			return HKGHEJDKCPI;
		}

		private void set_Name(string value)
		{
			HKGHEJDKCPI = value;
		}

		public PerksStage.ActionPerk get_Action()
		{
			return PMCIKHJONNM;
		}

		private void AHBNPODMIOD(PerksStage.ActionPerk value)
		{
			PMCIKHJONNM = value;
		}

		public bool get_NeedDelete()
		{
			return NCGLMKOGBCB;
		}

		private void set_NeedDelete(bool value)
		{
			NCGLMKOGBCB = value;
		}

		public int get_CurrentFrames()
		{
			return (get_Action() != null) ? get_Action().KGNDJOLBBJF : 0;
		}

		public int get_TotalFrames()
		{
			return (get_Action() != null) ? get_Action().FLNLMIHEDCI : 0;
		}

		public bool get_Show()
		{
			return MGIIAEFNAIM;
		}

		public void set_Show(bool value)
		{
			MGIIAEFNAIM = value;
			DIGBNJOGMOH(true);
		}

		public bool get_ShowExpiration()
		{
			return IAOPDCPAELB;
		}

		public void set_ShowExpiration(bool value)
		{
			IAOPDCPAELB = value;
			if (_expiration != null)
			{
				_expiration.gameObject.SetActive(IAOPDCPAELB);
			}
		}

		public bool get_ChangeOpacity()
		{
			return GPNIIJGOMKC;
		}

		private void DIGBNJOGMOH(bool value)
		{
			GPNIIJGOMKC = value;
		}

		public float get_CurrentIconOpacity()
		{
			return FEALIIMKDPN;
		}

		private void OBPMKMGNABJ(float value)
		{
			FEALIIMKDPN = value;
		}

		private float OOAHAJJKHKI()
		{
			return LNAHKGCNGLB;
		}

		private void LKHJKOOFAAN(float value)
		{
			LNAHKGCNGLB = value;
		}

		private bool IAGEBCJCFEF()
		{
			return IMLKFJDOOOM;
		}

		private void CJLDJACPNLO(bool value)
		{
			IMLKFJDOOOM = value;
		}

		private int ACMGBPEBGMD()
		{
			return IGAPOPAFKON;
		}

		private void LLCEMCONIBK(int value)
		{
			IGAPOPAFKON = value;
		}

		public int get_PulseCount()
		{
			return APIJKCAIHED;
		}

		public void set_PulseCount(int value)
		{
			APIJKCAIHED = value;
		}

		public bool get_DeleteRequested()
		{
			return JHLCLPEGHKP;
		}

		private void GFKHNLGGGJH(bool value)
		{
			JHLCLPEGHKP = value;
		}

		private bool HKKGNLIFPDD()
		{
			return MIKNOJCLEIM;
		}

		private void LGACCOFOCNL(bool value)
		{
			MIKNOJCLEIM = value;
		}

		public int get_FramesToEnd()
		{
			return get_TotalFrames() - get_CurrentFrames();
		}

		public void Init(PerksStage.ActionPerk IBODMPMJELJ)
		{
			AHBNPODMIOD(IBODMPMJELJ);
			set_Name(IBODMPMJELJ.NHKMCLPOMFK);
			set_Show(true);
			DIGBNJOGMOH(true);
			LGACCOFOCNL(false);
			set_NeedDelete(false);
			OBPMKMGNABJ(0f);
			LKHJKOOFAAN(PerkGUI.OOAHAJJKHKI());
			set_ShowExpiration(IBODMPMJELJ.FLNCPBKBJBL);
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform == null)
			{
				rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			if (_icon != null)
			{
				_icon.set_SpriteName(IBODMPMJELJ.NHKMCLPOMFK);
				_icon.SetNativeSize();
				_icon.set_Alpha(0f);
				rectTransform.sizeDelta = _icon.rectTransform.sizeDelta;
			}
			if (_expiration != null)
			{
				_expiration.set_Alpha(OOAHAJJKHKI());
				_expiration.fillAmount = 0f;
			}
		}

		private void BBEMBELMEGP()
		{
			AHBNPODMIOD(null);
		}

		private void NNLGELLDBKN()
		{
			if (!get_Show() && _icon != null)
			{
				_icon.set_Alpha(1f);
				OBPMKMGNABJ(1f);
				set_Show(true);
			}
		}

		private void OFIPILHODFF()
		{
			if (get_CurrentIconOpacity() < 1f)
			{
				float num = 1f / PerkGUI.PDNIHJMHKBI().x;
				OBPMKMGNABJ(get_CurrentIconOpacity() + num);
				if (get_CurrentIconOpacity() >= 1f)
				{
					OBPMKMGNABJ(1f);
					DIGBNJOGMOH(false);
				}
				if (_icon != null)
				{
					_icon.set_Alpha(get_CurrentIconOpacity());
				}
			}
		}

		private void KMGECIDGBPA()
		{
			if (get_CurrentIconOpacity() > 0f)
			{
				float num = 1f / PerkGUI.PDNIHJMHKBI().y;
				OBPMKMGNABJ(get_CurrentIconOpacity() - num);
				if (get_CurrentIconOpacity() <= 0f)
				{
					OBPMKMGNABJ(0f);
					DIGBNJOGMOH(false);
				}
				if (_icon != null)
				{
					_icon.set_Alpha(get_CurrentIconOpacity());
				}
			}
		}

		public void Render()
		{
			BHAKGKHLAKK();
			if (IAOPDCPAELB && _expiration != null)
			{
				float fillAmount = ((get_TotalFrames() == 0) ? 0f : ((float)get_CurrentFrames() / (float)get_TotalFrames()));
				_expiration.fillAmount = fillAmount;
			}
			if (get_ChangeOpacity())
			{
				if (get_Show())
				{
					OFIPILHODFF();
				}
				else
				{
					KMGECIDGBPA();
				}
				if (get_ShowExpiration() && _expiration != null)
				{
					_expiration.set_Alpha(get_CurrentIconOpacity() * (OOAHAJJKHKI() / 1f));
				}
				if (get_CurrentIconOpacity() == 0f)
				{
					Destroy();
				}
			}
		}

		private void BHAKGKHLAKK()
		{
			if (get_PulseCount() <= 0)
			{
				return;
			}
			float num = PerkGUI.CPPMFFCKHJI();
			float x = PerkGUI.PELLCOKIJMM().x;
			float y = PerkGUI.PELLCOKIJMM().y;
			float x2 = PerkGUI.HGHGEAIOHJA().x;
			float y2 = PerkGUI.HGHGEAIOHJA().y;
			int num2 = (int)((!IAGEBCJCFEF()) ? y2 : x2);
			float num3 = (IAGEBCJCFEF() ? 1f : num);
			if (ACMGBPEBGMD() > num2)
			{
				CJLDJACPNLO(!IAGEBCJCFEF());
				LLCEMCONIBK(0);
			}
			if (ACMGBPEBGMD() <= num2)
			{
				if (IAGEBCJCFEF())
				{
					num3 = 1f + (num - 1f) / x2 * (x * Mathf.Pow(ACMGBPEBGMD(), 2f) / x2 + (1f - x) * (float)ACMGBPEBGMD());
				}
				else
				{
					num3 = num - (num - 1f) / y2 * (y * Mathf.Pow(ACMGBPEBGMD(), 2f) / y2 + (1f - y) * (float)ACMGBPEBGMD());
					if (num3 <= 1f)
					{
						set_PulseCount(get_PulseCount() - 1);
					}
				}
			}
			Vector2 vector = base.transform.localScale;
			vector.x = num3;
			vector.y = num3;
			base.transform.localScale = vector;
			LLCEMCONIBK(ACMGBPEBGMD() + 1);
		}

		public void Destroy()
		{
			set_NeedDelete(true);
			DIGBNJOGMOH(false);
			base.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public int CompareTo(ActivePerkItem NOLFMPDGCOC)
		{
			if (NOLFMPDGCOC == null)
			{
				return 1;
			}
			return get_FramesToEnd().CompareTo(NOLFMPDGCOC.get_FramesToEnd());
		}
	}
}
