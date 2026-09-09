using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ContentClosed : ContentBase
	{
		[SerializeField]
		protected LabelAlias _lblDescription;

        public void InitText(string text)
        {
            _lblDescription.SetAlias(string.Empty);
            _lblDescription.set_text(text);
        }

		public void Init(string AJPALFBBGML)
		{
			_lblDescription.SetAlias(AJPALFBBGML);
		}
	}
}
