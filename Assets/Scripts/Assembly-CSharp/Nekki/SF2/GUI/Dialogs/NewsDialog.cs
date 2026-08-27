using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class NewsDialog : BaseDialog
	{
		public const int HEADER_OFFSET_Y = -25;

		public const int BUTTON_Y = -530;

		public const int BUTTON_OFFSET_X = -90;

		public const int SCROLL_WIDTH = 1680;

		private const int DENNBEEAPKP = 600;

		private const int HBHNDONLGGA = -640;

		protected string ONLLNCIAOFC = string.Empty;

		protected string KNPONKPJHFJ = string.Empty;

		[SerializeField]
		protected ResolutionImage _picture;

		[SerializeField]
		protected ResolutionImage _loadingPicture;

		private bool BKFINGKIGBE;

		protected List<NewsItem> FDAFJMFLKEP = new List<NewsItem>();

		public bool GoShopAfterClose;

		public string RedirectShopAfterClose = string.Empty;

		public bool BuyItemAfterClose;

		protected NewsButtonMaker JODALANHFPD = new NewsButtonMaker();

		public override void Init(object data)
		{
			NewsDialogInfo oFOJGCFHJKD = (NewsDialogInfo)data;
			if (oFOJGCFHJKD != null)
			{
				FDAFJMFLKEP = oFOJGCFHJKD.FNHPCBEDKFO;
			}
			IsQuestDialog = true;
			_picture.GetComponent<SFButton>().AddEventListener(0, EINIDMAILNM);
		}

		protected override void Start()
		{
			FLOHKIBCOKG();
			SetupHeader(ODLPOMFLOCP);
			MAGOIKICKAH(GBECKKCHAFI);
			HLJBLAPMDCB();
			if (AssemblyController.KMEOEAGGPBI())
			{
				BHLHODFNHHO();
			}
		}

		protected override void HLJBLAPMDCB()
		{
			_picture.gameObject.SetActive(FDAFJMFLKEP.Count > 0);
			if (FDAFJMFLKEP.Count > 0)
			{
				NewsItem pONDDFBMFOO = FDAFJMFLKEP[0];
				SetupHeader(pONDDFBMFOO.Title);
				ONLLNCIAOFC = pONDDFBMFOO.NHKMCLPOMFK;
				KNPONKPJHFJ = pONDDFBMFOO.Url;
				if (pONDDFBMFOO.EGBHELMJJKO)
				{
					GoShopAfterClose = true;
					RedirectShopAfterClose = pONDDFBMFOO.COIGFENOMJD;
				}
				Texture2D texture2D = null;
				if (File.Exists(pONDDFBMFOO.NHKMCLPOMFK))
				{
					byte[] data = File.ReadAllBytes(pONDDFBMFOO.NHKMCLPOMFK);
					texture2D = new Texture2D(2, 2);
					texture2D.LoadImage(data);
				}
				FDAFJMFLKEP.Remove(pONDDFBMFOO);
				if (texture2D != null)
				{
					_picture.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
				}
				else
				{
					EnableLoading(true);
				}
			}
		}

		protected override void FLOHKIBCOKG()
		{
			base.FLOHKIBCOKG();
			_topStripe.transform.BGNJGIACJBG(600f);
			_bottomStripe.transform.BGNJGIACJBG(-640f);
		}

		protected override void MAGOIKICKAH(KBDHPMOMJLL HJNAHNICGMH)
		{
			base.MAGOIKICKAH(HJNAHNICGMH);
			BOJNFFALDHH();
		}

		public override void OnClose(object data)
		{
			JODALANHFPD.EEFFNHNGDEH();
			if (FDAFJMFLKEP.Count == 0)
			{
				if ((!BuyItemAfterClose || !(RedirectShopAfterClose != string.Empty)) && GoShopAfterClose)
				{
					QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
					GameUtils.MKADBAEEMFA(GameUtils.NAMBCLFLNIN(hHKLFIIBIFF.OIKHBNOANPP), SliderType.SliderRuby);
					DelayedStrike dDFFCNPELBC = new DelayedStrike(SliderType.SliderRuby);
					if (RedirectShopAfterClose != string.Empty)
					{
						dDFFCNPELBC.DLKPBAJDHBO = ListSF.PGKBAEGCABK(RedirectShopAfterClose, 0L);
					}
					Module.DLOKJOHNDID(ScreenType.ModuleShop, dDFFCNPELBC);
				}
				base.OnClose(data);
			}
			else
			{
				BOJNFFALDHH();
				HLJBLAPMDCB();
			}
		}

		protected virtual void EnableLoading(bool value)
		{
			_loadingPicture.gameObject.SetActive(value);
			BKFINGKIGBE = value;
		}

		private void Update()
		{
			if (BKFINGKIGBE)
			{
				_loadingPicture.transform.Rotate(0f, 20f * Time.deltaTime, 0f);
			}
		}

		protected virtual void BOJNFFALDHH()
		{
			if (FDAFJMFLKEP.Count == 0)
			{
				return;
			}
			NewsItem pONDDFBMFOO = FDAFJMFLKEP[0];
			List<NewsButton> dHKDOHFKOOJ = pONDDFBMFOO.DHKDOHFKOOJ;
			int count = dHKDOHFKOOJ.Count;
			_btnOK.gameObject.SetActive(true);
			_btnOK.RemoveAllEventListener();
			_btnOK.AddEventListener(2, (object NPKMJMCLDAH) =>
			{
				OnClose(NPKMJMCLDAH);
				_btnOK.RemoveAllEventListener();
			});
			if (count <= 0)
			{
				return;
			}
			_btnOK.gameObject.SetActive(false);
			float gBCONNBABLL = -530f;
			float num = 1680 / (dHKDOHFKOOJ.Count + 1);
			float fNDOOJNDJDC = -840f + num;
			if (count == 1)
			{
				fNDOOJNDJDC = _btnOK.transform.localPosition.x;
			}
			JODALANHFPD.Init(fNDOOJNDJDC, gBCONNBABLL, num, base.gameObject, OnClose, this);
			foreach (NewsButton item in dHKDOHFKOOJ)
			{
				JODALANHFPD.IOCIJAODGKE(item);
			}
		}

		protected virtual void EINIDMAILNM(object data)
		{
			if (KNPONKPJHFJ != string.Empty)
			{
				Application.OpenURL(KNPONKPJHFJ);
			}
		}

		protected override void SetupHeader(string HCPNFPMHFCM)
		{
			_header.set_text((!(HCPNFPMHFCM == string.Empty)) ? HCPNFPMHFCM : LocalizationManager.GetString("dlgNewsTitle"));
			_header.transform.BGNJGIACJBG(-25f + _topStripe.transform.localPosition.y);
		}
	}
}
