using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuMaterSprite : MonoBehaviour
	{
		private GameCurrency JJPFBOKGIEF;

		private int _value;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private Text _valueLbl;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(GameCurrency MDDNHLBDJBN)
		{
			JJPFBOKGIEF = MDDNHLBDJBN;
			_value = ListSF.CCDKHLAMKKO().GetCurrencyCount(JJPFBOKGIEF);
			string mJBPMLCLMFN = JJPFBOKGIEF.MJBPMLCLMFN;
			if (_icon != null)
			{
				_icon.sprite = Nekki.SF2.GUI.ResolutionImage.GetSprite("UI/Atlases/", mJBPMLCLMFN);
			}
			if (_valueLbl != null)
			{
				_valueLbl.text = _value.ToString();
			}
		}

		public void UpdateView()
		{
			int num = ListSF.CCDKHLAMKKO().GetCurrencyCount(JJPFBOKGIEF);
			if (_value != num)
			{
				_value = num;
				_valueLbl.text = _value.ToString();
			}
		}
	}
}
