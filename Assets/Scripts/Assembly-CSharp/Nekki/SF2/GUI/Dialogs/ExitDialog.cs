using System;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class ExitDialog : BaseDialog
	{
		public const float CONTENT_PADDING = 40f;

		private bool MGEJCGPMJPF;

		private Action<object> AGEGHEGOGOI;

		private static bool _isOpened;

		[SerializeField]
		protected LabelAlias _text;

		public static bool DDKFMBKFGKJ
		{
			get
			{
				return get_IsOpened();
			}
		}

		public static bool get_IsOpened()
		{
			return _isOpened;
		}

		public override void Init(object data)
		{
			if (data != null)
			{
				GBAEHLPNDAC gBAEHLPNDAC = (GBAEHLPNDAC)data;
				MGEJCGPMJPF = gBAEHLPNDAC.BCIFHFCOCKI;
				AGEGHEGOGOI = gBAEHLPNDAC.Dlg;
				if (MGEJCGPMJPF)
				{
					IsPausing = false;
				}
			}
			base.Init((!MGEJCGPMJPF) ? "dlgExitTitle" : "dlgExitFightTitle", "dlgExitButton", "CANCEL", KBDHPMOMJLL.FOOTER_BOTH);
			_isOpened = true;
		}

		private void OnDestroy()
		{
			_isOpened = false;
		}

		protected override void HLJBLAPMDCB()
		{
			_text.alignment = TextAnchor.MiddleCenter;
			_text.transform.OKHPLHPBPKJ(0f);
			_text.transform.BGNJGIACJBG(0f);
			_text.set_LabelFontSize(103);
			_text.color = Constants.PJJIMHMJPAL;
			_text.set_Alias((!MGEJCGPMJPF) ? "dlgExitMessage" : "dlgExitFightMessage");
		}

		protected override void MAGOIKICKAH(KBDHPMOMJLL HJNAHNICGMH)
		{
			base.MAGOIKICKAH(HJNAHNICGMH);
			_btnOK.RemoveAllEventListener();
			_btnOK.AddEventListener(2, BGGEHPLONFP);
		}

		private void BGGEHPLONFP(object data)
		{
			if (MGEJCGPMJPF)
			{
				AGEGHEGOGOI(0);
			}
			else
			{
				GameUtils.PGLIKMEJBPK();
			}
			base.OnClose(data);
		}
	}
}
