using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class StylePanel : MonoBehaviour
	{
		[SerializeField]
		private StyleBar _styleBar;

		private ResolutionImage _styleName;

		private Dictionary<InfoAnimation, int> DNPLAENEMAK = new Dictionary<InfoAnimation, int>();

		private int DIEPFHAGGOD;

		private int HOGDDINKHFK;

		private const float NIOJCGLAKHL = 25f;

		private const float HEOMLLOIFDE = 0f;

		public FightStatistics.EMKEIEJMONM JANNOFIHHEL
		{
			get
			{
				return get_MaximumStyleStrip();
			}
		}

		public int AANBELKNKJJ
		{
			get
			{
				return get_CurrentStyleStrip();
			}
		}

		public string LDLLJHEDCPD
		{
			get
			{
				return get_CurrentStyleName();
			}
		}

		public ResolutionImage get_StyleName()
		{
			return _styleName;
		}

		public RectTransform get_rectTransform()
		{
			return (RectTransform)base.transform;
		}

		public FightStatistics.EMKEIEJMONM get_MaximumStyleStrip()
		{
			return (FightStatistics.EMKEIEJMONM)DIEPFHAGGOD;
		}

		public int get_CurrentStyleStrip()
		{
			return HOGDDINKHFK;
		}

		public string get_CurrentStyleName()
		{
			Style mHOJFHKHIIL = GameUtils.NIPBIAGMAOD.BPDFOLFPBHO(HOGDDINKHFK);
			return (mHOJFHKHIIL == null) ? string.Empty : mHOJFHKHIIL.Name;
		}

		public void Init(ResolutionImage ECJDAIHCDBA)
		{
			_styleName = ECJDAIHCDBA;
			_styleBar.Init();
			GameUtils.NIPBIAGMAOD.Styles.ForEach((Style DHDMNHCIPEH) =>
			{
				_styleBar.AddStrip(DHDMNHCIPEH.MJGNPJMBNFK, string.Empty, 25f, DHDMNHCIPEH.Name);
			});
			_styleBar.SetValue(0f, 0);
		}

		public void UpdateStyleLabel()
		{
			if (!(_styleName == null))
			{
				Style mHOJFHKHIIL = GameUtils.NIPBIAGMAOD.BPDFOLFPBHO(HOGDDINKHFK);
				if (mHOJFHKHIIL != null && !mHOJFHKHIIL.PDJFODICKBP.Equals(string.Empty))
				{
					_styleName.gameObject.SetActive(true);
					_styleName.set_SpriteName(mHOJFHKHIIL.PDJFODICKBP);
					_styleName.SetNativeSize();
				}
			}
		}

		public void SetCurrentStyleStrip(int index)
		{
			HOGDDINKHFK = index;
			DIEPFHAGGOD = Mathf.Max(DIEPFHAGGOD, HOGDDINKHFK);
			UpdateStyleLabel();
		}

		public float GetStyleIncrease(InfoAnimation IFPDGKDKJOD)
		{
			if (IFPDGKDKJOD == null)
			{
				return 0f;
			}
			int num = 0;
			if (DNPLAENEMAK.ContainsKey(IFPDGKDKJOD))
			{
				num = ++DNPLAENEMAK[IFPDGKDKJOD];
			}
			if (num == 0)
			{
				DNPLAENEMAK[IFPDGKDKJOD] = num;
			}
			float eNKMAPMCMCM = GameUtils.NIPBIAGMAOD.ENKMAPMCMCM;
			float num2 = GameUtils.NIPBIAGMAOD.GetStyleMultiplier(HOGDDINKHFK);
			float pKOFNMPOMKM = GameUtils.NIPBIAGMAOD.PKOFNMPOMKM;
			return eNKMAPMCMCM * num2 * Mathf.Pow(pKOFNMPOMKM, -num) * IFPDGKDKJOD.DCLGDANCGHC;
		}

		public void UpdateStyle(InfoAnimation IFPDGKDKJOD)
		{
			float styleIncrease = GetStyleIncrease(IFPDGKDKJOD);
			float value = _styleBar.GetValue(HOGDDINKHFK);
			float bAINMLLIKOL = value + styleIncrease;
			IncreaseStyleStripByValue(bAINMLLIKOL);
		}

		public void IncreaseStyleStripByValue(float value)
		{
			int num = HOGDDINKHFK + (int)value;
			value %= 1f;
			if (num >= GameUtils.NIPBIAGMAOD.Styles.Count)
			{
				num = GameUtils.NIPBIAGMAOD.Styles.Count - 1;
				value = 1f;
			}
			if (num != HOGDDINKHFK)
			{
				for (int num2 = num - 1; num2 >= 0; num2--)
				{
					_styleBar.SetValue(1f, 0, num2);
				}
				SetCurrentStyleStrip(num);
			}
			SetStyleValue(value);
		}

		public void SetStyleValue(float value)
		{
			_styleBar.SetValue(value, 0, HOGDDINKHFK);
		}

		public void SetAllStyleValue(float value)
		{
			_styleBar.SetValue(value, 0);
		}

		public float GetStyleValue(int KNECPEAFMIM)
		{
			return _styleBar.GetValue(KNECPEAFMIM);
		}

		public float GetStyleValue()
		{
			return _styleBar.GetValue(HOGDDINKHFK);
		}

		public void ClearStyleIncrease()
		{
			DNPLAENEMAK.Clear();
		}

		public void ResetStyle()
		{
			SetCurrentStyleStrip(0);
			SetAllStyleValue(0f);
			ClearStyleIncrease();
			if (_styleName != null)
			{
				_styleName.gameObject.SetActive(false);
			}
		}

		public void Render()
		{
			float num = GameUtils.NIPBIAGMAOD.JOIJKPLCJAN / 60f;
			num /= (float)GameUtils.GGBABPJBGJB();
			float styleValue = Mathf.Max(0f, _styleBar.GetValue(HOGDDINKHFK) - num);
			SetStyleValue(styleValue);
		}
	}
}
