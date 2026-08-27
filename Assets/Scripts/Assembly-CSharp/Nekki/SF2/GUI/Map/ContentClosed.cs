using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ContentClosed : ContentBase
	{
		[SerializeField]
		protected LabelAlias _lblDescription;

		public void Init(string AJPALFBBGML)
		{
			_lblDescription.SetAlias(AJPALFBBGML);
		}
	}
}
