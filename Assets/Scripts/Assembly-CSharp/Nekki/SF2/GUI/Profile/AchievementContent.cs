using System;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class AchievementContent : Content
	{
		public enum FICFDHMCGAP
		{
			zText = 0
		}

		private const int BFGKMIDOEFC = 88;

		private const int CKJIDMCICGK = 70;

		private const int ODGMAFLNGME = -294;

		private const int IBJPMNIGOMJ = 83;

		[SerializeField]
		protected LabelAlias _textLabel;

		[SerializeField]
		protected LabelAlias _rewardLabel;

		[SerializeField]
		protected SFButton _takeButton;

		protected Action<object> FNOECGMEKGL;

		protected string _text;

		protected int PIGOGFLFMMH;

		protected int ALGBFOBNPFO;

		protected bool BODCOGFGHAD;

		protected bool DPJOPMHPGKG;

		protected bool OBGMJMBNMME;

		protected float ECCMHGEEFLE;

		private void Start()
		{
			_takeButton.AddEventListener(2, BFJAKDPGLIB);
		}

		public void Init(string HCPNFPMHFCM, int PABLCLGLPBB = 0, int GNDNEONJDKG = 0, Action<object> ODDEOFKLIAG = null, bool DJGOCCEOAKD = false, bool NNEHNDILGDP = false)
		{
			_takeButton.gameObject.SetActive(false);
			_text = HCPNFPMHFCM;
			PIGOGFLFMMH = PABLCLGLPBB;
			ALGBFOBNPFO = GNDNEONJDKG;
			FNOECGMEKGL = ODDEOFKLIAG;
			BODCOGFGHAD = DJGOCCEOAKD;
			DPJOPMHPGKG = NNEHNDILGDP;
			HeaderFontSize = 88;
			AJNMAKEIDMH();
			EADJKKIOIHA();
			IBIKJHNOFDH();
			BNMLMGGOMGN();
		}

		public override void SetUpBorder(float BGEEALIPKCC)
		{
			ECCMHGEEFLE = BGEEALIPKCC;
			OBGMJMBNMME = true;
			BNMLMGGOMGN();
		}

		protected void AJNMAKEIDMH()
		{
			_textLabel.color = Constants.PJJIMHMJPAL;
			_textLabel.set_LabelFontSize(70);
			_textLabel.set_Alias(_text);
		}

		protected void EADJKKIOIHA()
		{
			_rewardLabel.gameObject.SetActive(false);
			if (BODCOGFGHAD)
			{
				_rewardLabel.gameObject.SetActive(true);
				_rewardLabel.transform.BGNJGIACJBG(-294f);
				_rewardLabel.color = Constants.PJJIMHMJPAL;
				_rewardLabel.set_LabelFontSize(83);
				string text = ((PIGOGFLFMMH <= 0) ? "MiscSprites.ruby" : ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
				int num = ((PIGOGFLFMMH <= 0) ? ALGBFOBNPFO : PIGOGFLFMMH);
				string text2 = LocalizationManager.GetString("achievementReward") + "<quad name=" + text + " size=88 width=1 /> " + num;
				_rewardLabel.set_text(text2);
				bool flag = PIGOGFLFMMH > 0 || ALGBFOBNPFO > 0;
				_rewardLabel.gameObject.SetActive(flag && BODCOGFGHAD);
			}
		}

		protected void IBIKJHNOFDH()
		{
			bool flag = PIGOGFLFMMH > 0 || ALGBFOBNPFO > 0;
			if (DPJOPMHPGKG && BODCOGFGHAD && flag)
			{
				float y = _rewardLabel.transform.transform.localPosition.y;
				_takeButton.transform.BGNJGIACJBG(y - 20f);
				_rewardLabel.transform.BGNJGIACJBG(y + 80f);
				_takeButton.gameObject.SetActive(BODCOGFGHAD);
			}
		}

		protected void BFJAKDPGLIB(object data)
		{
			if (FNOECGMEKGL != null)
			{
				FNOECGMEKGL(data);
			}
		}

		protected void BNMLMGGOMGN()
		{
			if (OBGMJMBNMME && _rewardLabel != null && _textLabel != null)
			{
				float num = _rewardLabel.transform.localPosition.y + _rewardLabel.rectTransform.rect.height / 2f;
				float bAINMLLIKOL = ECCMHGEEFLE - (ECCMHGEEFLE - num) / 2f;
				_textLabel.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}
	}
}
