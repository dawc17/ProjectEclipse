using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI_Nekki/LabelAliasLE")]
	[RequireComponent(typeof(LayoutElement))]
	public class LabelAliasLE : LabelAlias
	{
		[SerializeField]
		private LayoutElement _LayoutElement;

		public LayoutElement CMFIABIFDDD
		{
			get
			{
				return get_LayoutElement();
			}
			set
			{
				set_LayoutElement(value);
			}
		}

		public new string HCPNFPMHFCM
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

		public LayoutElement get_LayoutElement()
		{
			return _LayoutElement;
		}

		public void set_LayoutElement(LayoutElement value)
		{
			_LayoutElement = value;
		}

		public new string get_text()
		{
			return base.get_text();
		}

		public new void set_text(string value)
		{
			base.set_text(value);
			if (_LayoutElement == null)
			{
				_LayoutElement = base.gameObject.GetComponent<LayoutElement>();
			}
			if (_LayoutElement != null)
			{
				_LayoutElement.minWidth = CalculateLengthOfMessage();
			}
		}
	}
}
