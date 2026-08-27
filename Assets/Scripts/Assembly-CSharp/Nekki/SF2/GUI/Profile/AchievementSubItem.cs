using System;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class AchievementSubItem : SubItem
	{
		public enum ODADOKNBOCB
		{
			onRewardTake = 12
		}

		private const int EJBPLIKLAJO = 43;

		private const int CPILOBPDOBM = 40;

		private const int HKIAPHBIIID = 102;

		private const int CNCDAIBGMAA = 350;

		[SerializeField]
		protected ProgressBar _progress;

		[SerializeField]
		protected LabelAlias _progressLabel;

		protected float CALHMPICJPL;

		protected float HFJCEMJLLAJ;

		protected Achievement JJGCLBIGIPL;

		protected AchievementInfo HOGJCGGBEGP;

		protected Action<object> _dlg;

		public void Init(string KHPKDMGDMAB, string HHAAFADDOJB, string HCPNFPMHFCM, float AKIOCHEKNPE, float NPILBMKDDGN, int OKNNNLIPODI, Achievement NCCHENOEPNF = null)
		{
			Init(OKNNNLIPODI);
			CALHMPICJPL = AKIOCHEKNPE;
			HFJCEMJLLAJ = NPILBMKDDGN;
			JJGCLBIGIPL = NCCHENOEPNF;
			_texturePath = "UI/Achievements/";
			GJPJJHACOJJ = KHPKDMGDMAB;
			BHKAAODJMJF = ProfileGUI.OJEAKFALOGE.EBDBPJNBHGI;
			CDNOKAKOLMP = ProfileGUI.OJEAKFALOGE.DPGMCKCDMBC;
			int mJBFFBPLAGC = ((NCCHENOEPNF != null) ? NCCHENOEPNF.ANCDKCFLHOL : 0);
			int bDONIKLHFLJ = ((NCCHENOEPNF != null) ? NCCHENOEPNF.LBJFKGAHBBG : 0);
			bool bODCOGFGHAD = NCCHENOEPNF != null && !NCCHENOEPNF.NMCBAKACIGK;
			bool dPJOPMHPGKG = HFJCEMJLLAJ >= CALHMPICJPL;
			_dlg = OBENPGJNOIO;
			HOGJCGGBEGP = new AchievementInfo(HHAAFADDOJB, HCPNFPMHFCM, mJBFFBPLAGC, bDONIKLHFLJ, _dlg, bODCOGFGHAD, dPJOPMHPGKG);
			Data = HOGJCGGBEGP;
			UpdateIcon();
			SetActive(true);
			PIECOEPBLFL();
			UpdateProgress(HFJCEMJLLAJ);
			UpdatePositions();
		}

		public void UpdateProgress(float value)
		{
			HFJCEMJLLAJ = value;
			_progress.SetValue(value);
		}

		public void ResetOpacity()
		{
			if ((bool)_icon)
			{
				UIExtensions.HNIHBGAOAIH(_icon, BHKAAODJMJF);
			}
		}

		public virtual void UpdatePositions()
		{
			float num = 0f;
			num += _icon.transform.localPosition.x + _icon.rectTransform.rect.width / 2f;
			num += 43f;
			num += _progress.GetComponent<RectTransform>().rect.width / 2f;
			_progress.transform.OKHPLHPBPKJ(num);
		}

		public override void Choose()
		{
			if (JJGCLBIGIPL != null)
			{
				((AchievementInfo)Data).DJGOCCEOAKD = !JJGCLBIGIPL.NMCBAKACIGK;
			}
			base.Choose();
		}

		public Achievement GetAchievement()
		{
			return JJGCLBIGIPL;
		}

		protected void OBENPGJNOIO(object data)
		{
			CallEvent(12, this);
		}

		protected override void FGICHADOEHF()
		{
			base.FGICHADOEHF();
			if (JJGCLBIGIPL != null && JJGCLBIGIPL.DBHJGAGOLOB())
			{
				AJGODMIMDDP();
			}
		}

		protected void PIECOEPBLFL()
		{
			_progress.SetValueBorders(0f, CALHMPICJPL);
			_progress.transform.BGNJGIACJBG(40f);
			int num = (int)((!(HFJCEMJLLAJ <= CALHMPICJPL)) ? CALHMPICJPL : HFJCEMJLLAJ);
			string text = ((!((float)num < CALHMPICJPL)) ? LocalizationManager.GetString("achievement_Completed") : (num + "/" + CALHMPICJPL));
			_progressLabel.color = Constants.PJJIMHMJPAL;
			_progressLabel.set_LabelFontSize(102);
			_progressLabel.set_text(text);
			_progressLabel.transform.OKHPLHPBPKJ(350f);
			_progressLabel.transform.BGNJGIACJBG(-40f);
		}
	}
}
