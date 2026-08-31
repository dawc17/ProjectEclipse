using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI_Nekki/LabelAlias")]
	public class LabelAlias : TextPic
	{
		public enum LGEOOHJJOPP
		{
			Content = 0,
			Title = 1,
			Button = 2
		}

		[Tooltip("If true target will use font from localization settings")]
		public bool UseLocalizationFont = true;

		private static readonly Regex BPNAJICGHEH = new Regex("<quad name=(.+?) size=(\\d*\\.?\\d+%?) width=(\\d*\\.?\\d+%?) />", RegexOptions.Singleline);

		[Tooltip("Type of font. Choose font from languages.yaml")]
		public LGEOOHJJOPP FontType;

		[Tooltip("Flag for turn on/off using custom font size.")]
		public bool UseLabelFontSize = true;

		[Tooltip("Size who used for calculating result font size.")]
		[SerializeField]
		private int _labelFontSize;

		[Tooltip("Flag for turn on/off using custom line spacing.")]
		public bool UseLabelLineSpacing;

		[SerializeField]
		[Tooltip("Size who used for calculating result line spacing.")]
		private float _labelLineSpacing;

		[Tooltip("Text template with aliases")]
		[SerializeField]
		[TextArea(4, 10)]
		private string _Alias = string.Empty;

		[SerializeField]
		private bool _ToUpperCase;

		public int LFBOJOLEJED
		{
			get
			{
				return get_LabelFontSize();
			}
			set
			{
				set_LabelFontSize(value);
			}
		}

		public float KHKIIEDIEBL
		{
			get
			{
				return get_LabelLineSpacing();
			}
			set
			{
				set_LabelLineSpacing(value);
			}
		}

		public string HBCNKNFPAIM
		{
			get
			{
				return get_Alias();
			}
			set
			{
				set_Alias(value);
			}
		}

		public bool GKEJOPJGFLM
		{
			get
			{
				return get_ToUpperCase();
			}
			set
			{
				set_ToUpperCase(value);
			}
		}

		public string HCPNFPMHFCM
		{
			get
			{
				return get_text();
			}
			set
			{
				set_text(value);
			}
		}

		private string DEEKEDAKOBM
		{
			get
			{
				return KHDALILJDCM();
			}
		}

		private Font NOIMBKHLOLN
		{
			get
			{
				return PIBHLLOJEKK();
			}
		}

		public int get_LabelFontSize()
		{
			return _labelFontSize;
		}

		public void set_LabelFontSize(int value)
		{
			_labelFontSize = value;
			UpdateLabelFontSize();
		}

		public float get_LabelLineSpacing()
		{
			return _labelLineSpacing;
		}

		public void set_LabelLineSpacing(float value)
		{
			_labelLineSpacing = value;
			UpdateLabelLineSpacing();
		}

		public string get_Alias()
		{
			return _Alias;
		}

		public void set_Alias(string value)
		{
			SetAlias(value);
		}

		public bool get_ToUpperCase()
		{
			return _ToUpperCase;
		}

		public void set_ToUpperCase(bool value)
		{
			_ToUpperCase = value;
		}

		public string get_text()
		{
			return base.text;
		}

		public void set_text(string value)
		{
			string text = value;
			foreach (Match item in BPNAJICGHEH.Matches(text))
			{
				string valueText = item.Groups[1].Value;
				Sprite sprite = ResolutionImage.GetSprite(string.Empty, valueText);
				string newValue = Regex.Replace(item.ToString(), "size=(\\d*\\.?\\d+%?)", "size=" + sprite.rect.width);
				text = text.Replace(item.ToString(), newValue);
			}
			base.text = text;
			if (UseLocalizationFont)
			{
				base.font = PIBHLLOJEKK();
			}
		}

		private string KHDALILJDCM()
		{
			return LocalizationManager.GetString(_Alias);
		}

		private Font PIBHLLOJEKK()
		{
			switch (FontType)
			{
			case LGEOOHJJOPP.Content:
				return LocalizationManager.MBPJIKFOEBJ();
			case LGEOOHJJOPP.Title:
				return LocalizationManager.GNIENOIHLNO();
			case LGEOOHJJOPP.Button:
				return LocalizationManager.DIJFGLJHDBI();
			default:
				return LocalizationManager.MBPJIKFOEBJ();
			}
		}

		private List<string> BCFAGNEMKPJ()
		{
			List<string> list = new List<string>();
			string text = null;
			bool flag = false;
			if (_Alias != null)
			{
				string alias = _Alias;
				foreach (char c in alias)
				{
					if (flag)
					{
						if (c == '^')
						{
							list.Add(text);
							flag = false;
						}
						else
						{
							text += c;
						}
					}
					else if (c == '^')
					{
						text = string.Empty;
						flag = true;
					}
				}
				return list;
			}
			return null;
		}

		protected override void Awake()
		{
			base.Awake();
			// Counters have no alias and are often updated through Text.text, which
			// bypasses set_text. Apply their configured font independently of text.
			if (UseLocalizationFont)
			{
				Font localizedFont = PIBHLLOJEKK();
				if (localizedFont != null)
				{
					base.font = localizedFont;
				}
			}
			UpdateLabelFontSize();
			UpdateLabelLineSpacing();
			UpdateVerticalOverflow();
			LocalizationManager.LKFNMDCLMCD(OCLBJLPOKLB);
			if (LocalizationManager.KGEOCPBDJIF() && _Alias != null && _Alias.Length != 0)
			{
				OCLBJLPOKLB();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LocalizationManager.FFIJPHDLPCF(OCLBJLPOKLB);
		}

		private void OCLBJLPOKLB()
		{
			string text = KHDALILJDCM().TrimStart(' ');
			if (_ToUpperCase)
			{
				text = text.ToUpper();
			}
			if (get_Alias() != string.Empty)
			{
				set_text(text);
				if (get_text() == "%ERROR%")
				{
					color = Color.red;
				}
			}
			if (UseLocalizationFont)
			{
				base.font = PIBHLLOJEKK();
			}
			UpdateLabelFontSize();
			UpdateLabelLineSpacing();
			UpdateVerticalOverflow();
		}

		public void SetAlias(string HCPNFPMHFCM)
		{
			_Alias = HCPNFPMHFCM;
			OCLBJLPOKLB();
		}

		public int CalculateLengthOfMessage()
		{
			int num = 0;
			CharacterInfo info = default(CharacterInfo);
			char[] array = get_text().ToCharArray();
			char[] array2 = array;
			foreach (char ch in array2)
			{
				base.font.GetCharacterInfo(ch, out info, base.fontSize);
				num += info.advance;
			}
			return num;
		}

		public void UpdateLabelFontSize()
		{
			if (UseLabelFontSize)
			{
				int num = (int)((float)_labelFontSize * LocalizationManager.GCBEBEGKAOE());
				if (base.fontSize != num)
				{
					base.fontSize = num;
					base.resizeTextMaxSize = num;
				}
			}
		}

		public void UpdateLabelLineSpacing()
		{
			float num = 0f;
			num = ((!UseLabelLineSpacing) ? LocalizationManager.DLGKFIICJMG() : (_labelLineSpacing * LocalizationManager.OKIIEMCLAHH()));
			if (base.lineSpacing != num)
			{
				base.lineSpacing = num;
			}
		}

		public void UpdateVerticalOverflow()
		{
			if (LocalizationManager.ILAJKOBCHFH != null && LocalizationManager.ILAJKOBCHFH.MINNJBMGKLL)
			{
				base.verticalOverflow = VerticalWrapMode.Overflow;
			}
		}
	}
}
