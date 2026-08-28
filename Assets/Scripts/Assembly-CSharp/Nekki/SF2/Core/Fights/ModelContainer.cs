using System.Collections.Generic;
using System.Diagnostics;
using Nekki.SF2.GUI;
using UnityEngine;

namespace Nekki.SF2.Core.Fights
{
	public class ModelContainer : SFMonoBehaviour<object>
	{
		public enum LKJKNILAFIO
		{
			EventAnimationEnd = 0,
			EventTryOnEnd = 1
		}

		[SerializeField]
		private Vector2 _modelPosition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float DDLANLBOIIJ;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float INKIIOKAJBE;

		private string _currentScene;

		private bool FFDNOHEDBKB;

		private bool COENNLCACDI;

		private bool ODEHNPJKBIA;

		private StageType.FDBBPEGEGMK PGHPNADNACH;

		private ModelParameters HEGIABHIPHA;

		private Model _playerModel;

		private RenderContainer PFELMKLNBMC;

		private Location _location = new Location();

		private EquippedItemsStruct OCEIGMAPCHK = new EquippedItemsStruct();

		private SelectAnimation _selectAnimation = new SelectAnimation();

		private List<Model> _models = new List<Model>();

		private List<Model> HCPGFOCGDAA = new List<Model>();

		private List<Model> JLEFIKJODGG = new List<Model>();

		private Color _colorModel = new Color32(40, 20, 9, byte.MaxValue);

		private bool OGKFKJFGOIE = true;

		public float KBGFAKKBMCN
		{
			get
			{
				return get_Width();
			}
			protected set
			{
				KIBFMGKHMLI(value);
			}
		}

		public float LOAKJAJAJJC
		{
			get
			{
				return get_Height();
			}
			protected set
			{
				NDKNCMACEBA(value);
			}
		}

		public StageType.FDBBPEGEGMK ONOBNMHGABO
		{
			get
			{
				return get__StageType();
			}
		}

		public float get_Width()
		{
			return DDLANLBOIIJ;
		}

		protected void KIBFMGKHMLI(float value)
		{
			DDLANLBOIIJ = value;
		}

		public float get_Height()
		{
			return INKIIOKAJBE;
		}

		protected void NDKNCMACEBA(float value)
		{
			INKIIOKAJBE = value;
		}

		public StageType.FDBBPEGEGMK get__StageType()
		{
			return PGHPNADNACH;
		}

		public void Init(float JMLAKAKDBBL = 0f, float FEIHFIPFNKF = 0f, float LOJLAFEALJO = 0f, float ILLMIAIFBKL = 0f)
		{
			KIBFMGKHMLI((JMLAKAKDBBL != 0f) ? JMLAKAKDBBL : ((float)Screen.width));
			NDKNCMACEBA(FEIHFIPFNKF);
			_location.gameLayer = new LocationSelector(0);
			_location.gameLayer.MJNPBMOAFML().transform.SetParent(base.transform, false);
			NFFPENNBCMB();
			HEGIABHIPHA = GameUtils.LBMPHBNJMGG();
			HEGIABHIPHA.EEGMBGBLLIF = false;
			HEGIABHIPHA.ABAPAIEBNGK = false;
		}

		private void OnDestroy()
		{
			if (_playerModel != null)
			{
				_playerModel.RemoveEventListener(3, OnAnimationEnd);
				_playerModel.RemoveEventListener(6, HMAGHCEBOPK);
				_playerModel.RemoveEventListener(5, KAJHBALIMOE);
				_playerModel.RemoveEventListener(14, NIFELGIKECC);
				GPGGHKLFAGC();
				_models.Remove(_playerModel);
				_playerModel.IMFOFFFLGOM();
				_playerModel = null;
				_models.ForEach((Model DHDMNHCIPEH) =>
				{
					DHDMNHCIPEH.IMFOFFFLGOM();
				});
				_models.Clear();
			}
		}

		private void NFFPENNBCMB()
		{
			if (PFELMKLNBMC != null)
			{
				Object.Destroy(PFELMKLNBMC.MJNPBMOAFML());
				PFELMKLNBMC = null;
			}
			PFELMKLNBMC = new RenderContainer();
			PFELMKLNBMC.Init(_location);
			PFELMKLNBMC.MJNPBMOAFML().SetActive(false);
		}

		public void UpdateModel(ItemInfo item, StageType.FDBBPEGEGMK LGPIFNMFPAN, string MHOCFOODLLL)
		{
			PGHPNADNACH = LGPIFNMFPAN;
			NFFPENNBCMB();
			if (_playerModel != null)
			{
				_playerModel.RemoveEventListener(3, OnAnimationEnd);
				_playerModel.RemoveEventListener(6, HMAGHCEBOPK);
				_playerModel.RemoveEventListener(5, KAJHBALIMOE);
				_playerModel.RemoveEventListener(14, NIFELGIKECC);
				GPGGHKLFAGC();
				_models.Remove(_playerModel);
				_playerModel.IMFOFFFLGOM();
				_playerModel = null;
				_models.ForEach((Model DHDMNHCIPEH) =>
				{
					DHDMNHCIPEH.IMFOFFFLGOM();
				});
				_models.Clear();
			}
			HEGIABHIPHA = new ModelParameters(GameUtils.LBMPHBNJMGG());
			HEGIABHIPHA.JJCKADKCDIF = new Vector3f(_modelPosition);
			HEGIABHIPHA.EEGMBGBLLIF = false;
			HEGIABHIPHA.ABAPAIEBNGK = false;
			HEGIABHIPHA.IBBALIJOJMC = BMGDMKHAPEC(MHOCFOODLLL);
			_currentScene = MHOCFOODLLL;
			ItemInfo dJKEECEOCJB = null;
			if (item != null)
			{
				if (item.Type.Equals("Weapon"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.JGMLKIPCFII;
					HEGIABHIPHA.JGMLKIPCFII = item;
				}
				else if (item.Type.Equals("Armor"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.LKKFNMBCCDB;
					HEGIABHIPHA.LKKFNMBCCDB = item;
				}
				else if (item.Type.Equals("Helm"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.FKMOLBBLKDA;
					HEGIABHIPHA.FKMOLBBLKDA = item;
				}
				else if (item.Type.Equals("Ranged"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.LGHMILECPLA;
					HEGIABHIPHA.LGHMILECPLA = item;
				}
				else if (item.Type.Equals("Magic"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.ADBKGIBBNHJ;
					HEGIABHIPHA.ADBKGIBBNHJ = item;
				}
				else if (item.Type.Equals("RaidConsumable") && item.MDPPNGIEJGD.Equals("RaidCharge"))
				{
					dJKEECEOCJB = OCEIGMAPCHK.LMIBBJIKLNO;
					KAOPLEPILDH kAOPLEPILDH = HEGIABHIPHA as KAOPLEPILDH;
					if (kAOPLEPILDH != null)
					{
						kAOPLEPILDH.LMIBBJIKLNO = item;
					}
				}
				HEGIABHIPHA.PPFDLIBLNDG();
			}
			else
			{
				dJKEECEOCJB = CNIMJKICMBG();
			}
			HEGIABHIPHA.ALBOCOGOBCN(OCEIGMAPCHK);
			KCDFCHGDJBJ(HEGIABHIPHA, MHOCFOODLLL);
			if (FFDNOHEDBKB)
			{
				FFDNOHEDBKB = false;
				HEGIABHIPHA.ALBOCOGOBCN(OCEIGMAPCHK);
			}
			COENNLCACDI = false;
			_playerModel = new Model(HEGIABHIPHA);
			_playerModel.CGEKLPLKIDC();
			SetModelOnListening(_playerModel);
			_selectAnimation.FDBHLFMBECM();
			_selectAnimation.AddModel(_playerModel);
			_models.Add(_playerModel);
			JJLIGFGHLKA();
			Render();
			OPHJJEPKGPO();
		}

		private void JJLIGFGHLKA()
		{
			_playerModel.KDAHHIMLJGG.Data = PGHPNADNACH;
			_playerModel.JMHJDHLBHLK = (int)PGHPNADNACH;
			UpdateAnimationParameters(_playerModel);
			_selectAnimation.CheckEvent(EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE, _playerModel.KDAHHIMLJGG);
		}

		private void OPHJJEPKGPO()
		{
			PFELMKLNBMC.FPNKBJPKKGB().AddModel(_playerModel.CLDMEJKGLBA(), _colorModel, true);
			PFELMKLNBMC.CDDKOOMODHG(_playerModel);
			ODEHNPJKBIA = true;
		}

		public void PlayAnimation(string name, int AOJJBKLCHJO = 1)
		{
			TryPlayAnimation(name);
		}

		public bool TryPlayAnimation(string name)
		{
			InfoAnimation pJAHIOELGGD = AnimationData.BCIFKBJAFEC(name);
			if (pJAHIOELGGD != null && _playerModel != null)
			{
				_playerModel.PlayAnimationDelay(pJAHIOELGGD);
				return true;
			}
			return false;
		}

		private void HMAGHCEBOPK(object data)
		{
			Model fGCODGKLHED = (Model)data;
			HCPGFOCGDAA.Add(fGCODGKLHED);
			SetModelOnListening(fGCODGKLHED);
			PFELMKLNBMC.FPNKBJPKKGB().AddModel(fGCODGKLHED.CLDMEJKGLBA(), _colorModel, true);
			PFELMKLNBMC.CDDKOOMODHG(fGCODGKLHED);
			UpdateAnimationParameters(fGCODGKLHED);
		}

		private void KAJHBALIMOE(object data)
		{
			Model fGCODGKLHED = (Model)data;
			int num = 0;
			foreach (Model item in _models)
			{
				if (item == fGCODGKLHED)
				{
					break;
				}
				num++;
			}
			JLEFIKJODGG.AddIfNotExist(fGCODGKLHED);
		}

		private void UpdateAnimationParameters(Model CNAAFEHFGKD)
		{
			ModelObject bBGCMFGFMCL = CNAAFEHFGKD.CLDMEJKGLBA();
			bool dPKOKLCJEHI = CNAAFEHFGKD.EPCNJLEHJCB();
			bool eMGNKKHPGCJ = CNAAFEHFGKD.NJDJHGDMCIJ() != null;
			List<InfoAnimation> lNKFKJKLCKP = CNAAFEHFGKD.MCFPDHOLNGB();
			foreach (Model item in _models)
			{
				KMKOHGBJNBK(item, lNKFKJKLCKP, bBGCMFGFMCL, dPKOKLCJEHI, eMGNKKHPGCJ);
			}
			foreach (Model item2 in HCPGFOCGDAA)
			{
				KMKOHGBJNBK(item2, lNKFKJKLCKP, bBGCMFGFMCL, dPKOKLCJEHI, eMGNKKHPGCJ);
			}
		}

		private void KMKOHGBJNBK(Model ACENLMONNPA, List<InfoAnimation> LNKFKJKLCKP, ModelObject BBGCMFGFMCL, bool DPKOKLCJEHI, bool EMGNKKHPGCJ)
		{
			List<InfoAnimation> list = ACENLMONNPA.MCFPDHOLNGB();
			foreach (InfoAnimation item in list)
			{
				item.BPHNHFJCFCD(BBGCMFGFMCL, DPKOKLCJEHI, EMGNKKHPGCJ, BBGCMFGFMCL);
			}
			ModelObject oIEODIEHJMH = ACENLMONNPA.CLDMEJKGLBA();
			bool eKBOGDKIHIH = ACENLMONNPA.EPCNJLEHJCB();
			bool pHADJMAONJG = ACENLMONNPA.NJDJHGDMCIJ() != null;
			foreach (InfoAnimation item2 in LNKFKJKLCKP)
			{
				item2.BPHNHFJCFCD(oIEODIEHJMH, eKBOGDKIHIH, pHADJMAONJG, oIEODIEHJMH);
			}
		}

		private bool BKBMDPBINNO(ItemInfo CHJGFBKFKKD, ItemInfo BGCMDCGMPPL)
		{
			if (CHJGFBKFKKD != null && BGCMDCGMPPL != null && !CHJGFBKFKKD.Name.Equals(BGCMDCGMPPL.Name))
			{
				return true;
			}
			return false;
		}

		public bool IsItemDiffer(ModelParameters JCICKLIMBEF)
		{
			if (BKBMDPBINNO(OCEIGMAPCHK.LKKFNMBCCDB, JCICKLIMBEF.LKKFNMBCCDB))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.FKMOLBBLKDA, JCICKLIMBEF.FKMOLBBLKDA))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.PILJCAOFAED, JCICKLIMBEF.PILJCAOFAED))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.KKJJONOBHKI, JCICKLIMBEF.KKJJONOBHKI))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.JGMLKIPCFII, JCICKLIMBEF.JGMLKIPCFII))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.ADBKGIBBNHJ, JCICKLIMBEF.ADBKGIBBNHJ))
			{
				return true;
			}
			if (BKBMDPBINNO(OCEIGMAPCHK.LGHMILECPLA, JCICKLIMBEF.LGHMILECPLA))
			{
				return true;
			}
			KAOPLEPILDH kAOPLEPILDH = JCICKLIMBEF as KAOPLEPILDH;
			if (kAOPLEPILDH != null && BKBMDPBINNO(OCEIGMAPCHK.LMIBBJIKLNO, kAOPLEPILDH.LMIBBJIKLNO))
			{
				return true;
			}
			return false;
		}

		private ItemInfo CNIMJKICMBG()
		{
			if (OCEIGMAPCHK.LKKFNMBCCDB != HEGIABHIPHA.LKKFNMBCCDB)
			{
				return OCEIGMAPCHK.LKKFNMBCCDB;
			}
			if (OCEIGMAPCHK.FKMOLBBLKDA != HEGIABHIPHA.FKMOLBBLKDA)
			{
				return OCEIGMAPCHK.FKMOLBBLKDA;
			}
			if (OCEIGMAPCHK.PILJCAOFAED != HEGIABHIPHA.PILJCAOFAED)
			{
				return OCEIGMAPCHK.PILJCAOFAED;
			}
			if (OCEIGMAPCHK.KKJJONOBHKI != HEGIABHIPHA.KKJJONOBHKI)
			{
				return OCEIGMAPCHK.KKJJONOBHKI;
			}
			if (OCEIGMAPCHK.JGMLKIPCFII != HEGIABHIPHA.JGMLKIPCFII)
			{
				return OCEIGMAPCHK.JGMLKIPCFII;
			}
			if (OCEIGMAPCHK.ADBKGIBBNHJ != HEGIABHIPHA.ADBKGIBBNHJ)
			{
				return OCEIGMAPCHK.ADBKGIBBNHJ;
			}
			if (OCEIGMAPCHK.LGHMILECPLA != HEGIABHIPHA.LGHMILECPLA)
			{
				return OCEIGMAPCHK.LGHMILECPLA;
			}
			KAOPLEPILDH kAOPLEPILDH = HEGIABHIPHA as KAOPLEPILDH;
			if (kAOPLEPILDH != null && OCEIGMAPCHK.LMIBBJIKLNO != kAOPLEPILDH.LMIBBJIKLNO)
			{
				return OCEIGMAPCHK.LMIBBJIKLNO;
			}
			return null;
		}

		private void BELLAEIMEAB()
		{
			foreach (Model item in JLEFIKJODGG)
			{
				int num = 0;
				foreach (Model item2 in _models)
				{
					if (item2 == item)
					{
						break;
					}
					num++;
				}
				RemoveModelByIndex(num, item);
			}
			JLEFIKJODGG.Clear();
		}

		private void RemoveModelByIndex(int index, Model LEKHCMIFJAO)
		{
			int count = _models.Count;
			Model fGCODGKLHED = null;
			if (count == 0 || index < 0 || count - 1 < index)
			{
				fGCODGKLHED = LEKHCMIFJAO;
			}
			else
			{
				fGCODGKLHED = _models[index];
				PFELMKLNBMC.FPNKBJPKKGB().RemoveModel(index);
				PFELMKLNBMC.NAKJKHLEAEB(fGCODGKLHED);
				_models.Remove(fGCODGKLHED);
			}
			RemoveModel(fGCODGKLHED);
		}

		private void RemoveModel(Model ACENLMONNPA)
		{
			if (ACENLMONNPA == null)
			{
				LLLOJBFMONN.Error("Fight::removeModel - cant find model");
				return;
			}
			Model fGCODGKLHED = ACENLMONNPA.NJDJHGDMCIJ();
			if (fGCODGKLHED != null)
			{
				fGCODGKLHED.MGGBIBAHDEE((WeaponModel)ACENLMONNPA);
			}
			foreach (Model item in _models)
			{
				item.CNIAJPBJHIM(ACENLMONNPA);
				item.SetNearestEnemy();
			}
			_selectAnimation.RemoveModel(ACENLMONNPA);
			ACENLMONNPA.FKIBECCHIJC();
			ACENLMONNPA.IMFOFFFLGOM();
		}

		private void GPGGHKLFAGC()
		{
		}

		private SceneTypes BMGDMKHAPEC(string LFLGCDNKNJI)
		{
			switch (LFLGCDNKNJI)
			{
			case "Weapon":
				return SceneTypes.SceneShopWeapon;
			case "Armor":
				return SceneTypes.SceneShopArmor;
			case "Helm":
				return SceneTypes.SceneShopHelm;
			case "Ranged":
				return SceneTypes.SceneShopMissile;
			case "Magic":
				return SceneTypes.SceneShopMagic;
			case "RealMoneyItem":
				return SceneTypes.SceneShopRuby;
			case "Consumable":
				return SceneTypes.SceneShopRuby;
			case "Free":
				return SceneTypes.SceneShopFree;
			case "RaidItemPack":
				return SceneTypes.SceneShopRaidItemPack;
			case "RaidConsumable":
				return SceneTypes.SceneShopRaidItemPack;
			case "Profile":
				return SceneTypes.SceneProfile;
			default:
				return SceneTypes.SceneNone;
			}
		}

		public void ResetModel()
		{
			_selectAnimation.Reset();
			ResetModelPosition();
			JJLIGFGHLKA();
		}

		public void ResetModelPosition()
		{
			if (_playerModel != null)
			{
				Vector3f mGMMDGFPBLP = new Vector3f(_modelPosition);
				_playerModel.SetModelPosition(mGMMDGFPBLP);
			}
		}

		private void KCDFCHGDJBJ(ModelParameters JCICKLIMBEF, string NFNJJIGAKNN)
		{
			ShopOverride jHJPEFFBMFM = GameUtils.JNDLCLLIMMM.GetOverrideByScreen(NFNJJIGAKNN);
			if (jHJPEFFBMFM != null)
			{
				ItemInfo mBIJKDIEFIF = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(jHJPEFFBMFM.DAOMBPLCBMN);
				HEGIABHIPHA.OLLNIKFPMKE(jHJPEFFBMFM.Type, mBIJKDIEFIF);
				HEGIABHIPHA.PPFDLIBLNDG();
			}
		}

		private void SetModelOnListening(Model ACENLMONNPA)
		{
			float nGHJOCKCCHH = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x - base.transform.position.x;
			float kCNCLAANGGJ = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0f)).x - base.transform.position.x;
			ACENLMONNPA.SetWalls(nGHJOCKCCHH, kCNCLAANGGJ, 0, 0);
			ACENLMONNPA.AddEventListener(3, OnAnimationEnd);
			ACENLMONNPA.AddEventListener(6, HMAGHCEBOPK);
			ACENLMONNPA.AddEventListener(5, KAJHBALIMOE);
			ACENLMONNPA.AddEventListener(14, NIFELGIKECC);
		}

		private void OnAnimationEnd(object data)
		{
			CallEvent(0, data);
		}

		private void NIFELGIKECC(object data)
		{
			CallEvent(1, data);
		}

		private void FixedUpdate()
		{
			if (OGKFKJFGOIE || Input.GetKeyDown(KeyCode.Equals))
			{
				Render();
			}
			if (Input.GetKeyDown(KeyCode.Minus))
			{
				OGKFKJFGOIE = !OGKFKJFGOIE;
			}
		}

		private void Render()
		{
			if (!ODEHNPJKBIA)
			{
				return;
			}
			foreach (Model item in _models)
			{
				if (!COENNLCACDI && item.LPFPGDJALED() != -1)
				{
					COENNLCACDI = true;
					PFELMKLNBMC.MJNPBMOAFML().SetActive(true);
				}
				item.Render();
			}
			if (HCPGFOCGDAA.Count > 0)
			{
				foreach (Model item2 in HCPGFOCGDAA)
				{
					item2.Render();
					_models.Add(item2);
				}
				HCPGFOCGDAA.Clear();
			}
			_selectAnimation.Render();
			PFELMKLNBMC.GOCPBKNDKMC().DHOMHKADCFG();
			PFELMKLNBMC.GDBMKMFFOCF().DHOMHKADCFG();
			BELLAEIMEAB();
		}
	}
}
