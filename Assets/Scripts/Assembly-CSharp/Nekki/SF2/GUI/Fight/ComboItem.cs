using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class ComboItem : MonoBehaviour
	{
		private const string IBGLKACJEHD = "FightUI.Critical";

		private const string JCFGHNHJOFP = "FightUI.First_Strike";

		private const string HMLOHEKPAGC = "FightUI.Head_Hit";

		private const string AIGAAPIEIPH = "FightUI.Combo";

		private const string NHCNONBPNJH = "FightUI.hot_ground";

		private const string MDIGBDBAKEM = "FightUI.shock";

		[SerializeField]
		private Color _labelColorHotground = new Color32(254, 253, 131, byte.MaxValue);

		[SerializeField]
		private int _labelFontSizeHotground = 120;

		[SerializeField]
		private ResolutionImage _image;

		[SerializeField]
		private LabelAlias _label;

		[SerializeField]
		private Shadow _labelShadow;

		[SerializeField]
		private HorizontalLayoutGroup _layout;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComboTypes MMEKOHBGPGG;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ScreenModel.JEDPGMIGGKK NMBECJLDELB;

		public ComboTypes GLFJFNICMIF
		{
			get
			{
				return get_ComboType();
			}
			private set
			{
				EIEKDJFOKAO(value);
			}
		}

		public ScreenModel.JEDPGMIGGKK DPLAGLNBMOM
		{
			get
			{
				return get_ModelType();
			}
			private set
			{
				JLBHBOKEAFD(value);
			}
		}

		public ComboTypes get_ComboType()
		{
			return MMEKOHBGPGG;
		}

		private void EIEKDJFOKAO(ComboTypes value)
		{
			MMEKOHBGPGG = value;
		}

		public ScreenModel.JEDPGMIGGKK get_ModelType()
		{
			return NMBECJLDELB;
		}

		private void JLBHBOKEAFD(ScreenModel.JEDPGMIGGKK value)
		{
			NMBECJLDELB = value;
		}

		public RectTransform get_rectTransform()
		{
			return base.transform as RectTransform;
		}

		public void Init(ComboTypes LFLGCDNKNJI, ScreenModel.JEDPGMIGGKK NPEAOKLDJHA)
		{
			JLBHBOKEAFD(NPEAOKLDJHA);
			EIEKDJFOKAO(LFLGCDNKNJI);
			if (_image != null)
			{
				_image.set_SpriteName(APNCKGLEIEE(LFLGCDNKNJI));
				_image.SetNativeSize();
				LayoutElement component = _image.GetComponent<LayoutElement>();
				component.minWidth = _image.rectTransform.rect.width;
				component.minHeight = _image.rectTransform.rect.height;
			}
			if (_labelShadow != null)
			{
				_labelShadow.gameObject.SetActive(LFLGCDNKNJI == ComboTypes.TypeHotGroundTimer);
			}
			if (_label != null)
			{
				if (LFLGCDNKNJI == ComboTypes.TypeHotGroundTimer)
				{
					_label.color = _labelColorHotground;
					_label.set_LabelFontSize(_labelFontSizeHotground);
				}
				if (NPEAOKLDJHA == ScreenModel.JEDPGMIGGKK.TYPE_LEFT)
				{
					_label.transform.SetAsLastSibling();
				}
				else
				{
					_label.transform.SetAsFirstSibling();
				}
			}
			UpdateSize();
		}

		private string APNCKGLEIEE(ComboTypes LFLGCDNKNJI)
		{
			switch (LFLGCDNKNJI)
			{
			case ComboTypes.TypeFirstStrike:
				return "FightUI.First_Strike";
			case ComboTypes.TypeHead:
				return "FightUI.Head_Hit";
			case ComboTypes.TypeCritical:
				return "FightUI.Critical";
			case ComboTypes.TypeCombo:
				return "FightUI.Combo";
			case ComboTypes.TypeHotGroundTimer:
				return "FightUI.hot_ground";
			case ComboTypes.TypeShock:
				return "FightUI.shock";
			default:
				return string.Empty;
			}
		}

		public void UpdateCount(int count)
		{
			_label.set_text(count.ToString());
			_label.gameObject.SetActive(true);
			UpdateSize();
		}

		public void UpdateSize()
		{
			if (_image != null && _label != null && _layout != null)
			{
				Vector2 sizeDelta = new Vector2(0f, 0f);
				sizeDelta.x = _image.rectTransform.rect.width;
				sizeDelta.y = _image.rectTransform.rect.height;
				if (!_label.get_text().Equals(string.Empty))
				{
					sizeDelta.x += _label.preferredWidth;
					sizeDelta.x += _layout.spacing;
				}
				get_rectTransform().sizeDelta = sizeDelta;
			}
		}
	}
}
