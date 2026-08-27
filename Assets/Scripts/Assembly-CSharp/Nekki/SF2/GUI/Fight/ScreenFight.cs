using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Fight
{
	public class ScreenFight : MonoBehaviour
	{
		public struct PNCCDHPHKEM
		{
			public ModelParameters KMMJCHDKBDO;

			public List<ModelParameters> BDANFHBMIOF;

			public int BLOOLFFMKFI;
		}

		public class JGGPBICMICP : UnityEvent<ScreenFightType>
		{
		}

		public JGGPBICMICP OnStartScreen = new JGGPBICMICP();

		public JGGPBICMICP OnStopScreen = new JGGPBICMICP();

		private const float AMNDDAEIJAH = 1.666f;

		private const float JAOJKJJBPNL = 0.016f;

		private const float DGDFMKKHLPO = 1.166f;

		private const float ADPBOMNDCID = 5.833f;

		private const float JJPDKBGCCOM = 1.166f;

		private const float GJHANHLMFBI = 1.166f;

		private const float JDNECDOHBPD = 1.166f;

		private const float FEECKAGKHFK = 1.166f;

		private const float LDNCLBDABLG = 1.166f;

		private const string ILMIEAHBDCJ = "FightUI.perfect";

		private const string GOGGCBPFMML = "FightUI.great";

		private const string JMLBPGBNICP = "FightUI.fight";

		private const string AIHLFHKHGJJ = "FightUI.round";

		private const string LBEMDAFJMFN = "FightUI.timesup";

		private const string NOIOFMLHJBG = "FightUI.ringout";

		private const string DJPMFDKFCFA = "FightUI.youlose";

		private const string PFPAAJAEJPF = "FightUI.youwin";

		private PNCCDHPHKEM BBFBFDDDJMJ = default(PNCCDHPHKEM);

		public ScreenFightType Type;

		private int maxRounds;

		private string ruleDesc = string.Empty;

		private bool KCANPMPILKI;

		private bool IFMCDDIGOLD;

		private bool PPIJJHJCGGB;

		private bool GDLJMEJBGPO;

		private bool KJIAPGDFEIK;

		private float timer;

		[SerializeField]
		private GameObject vsScreenPrefab;

		[SerializeField]
		private GameObject enemiesScreenPrefab;

		[SerializeField]
		private ResolutionImage image;

		[SerializeField]
		private LabelAlias round;

		[SerializeField]
		private LabelAlias ruleDescription;

		private VsScreen vsScreen;

		private EnemiesScreen enemiesScreen;

		public bool get_Pause()
		{
			return KCANPMPILKI;
		}

		public void set_Pause(bool value)
		{
			KCANPMPILKI = value;
		}

		public void PreInit(FightList KGKDKENMAOA)
		{
			maxRounds = KGKDKENMAOA.BDBBNECNMBP * KGKDKENMAOA.PNHLGCBPFIG();
			ruleDesc = KGKDKENMAOA.GJOAJAIJHOE();
			set_Pause(false);
		}

		public void CreateVS(ModelParameters JCICKLIMBEF, List<ModelParameters> IDAAONBIBJM, int OBLEMIHLFII, bool PPIJJHJCGGB, bool GDLJMEJBGPO, bool IFMCDDIGOLD)
		{
			BBFBFDDDJMJ.KMMJCHDKBDO = JCICKLIMBEF;
			BBFBFDDDJMJ.BDANFHBMIOF = IDAAONBIBJM;
			BBFBFDDDJMJ.BLOOLFFMKFI = OBLEMIHLFII;
			this.PPIJJHJCGGB = PPIJJHJCGGB;
			this.GDLJMEJBGPO = GDLJMEJBGPO;
			if (IDAAONBIBJM.Count > 1 && GDLJMEJBGPO)
			{
				CreateEnemiesScreen(IDAAONBIBJM, OBLEMIHLFII, this.PPIJJHJCGGB);
				return;
			}
			if (IFMCDDIGOLD)
			{
				StartVS();
				return;
			}
			Type = ScreenFightType.TYPE_INFO_VS;
			StartScreen(0f);
		}

		public void CreateRound(int value, bool JMBAAPAPMGB = false)
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_ROUND;
			if (image != null)
			{
				image.set_SpriteName("FightUI.round");
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			if (round != null)
			{
				int num = ((!JMBAAPAPMGB) ? value : maxRounds);
				round.gameObject.SetActive(true);
				round.set_text(num.ToString());
			}
			StartScreen(1.666f);
		}

		public void CreateSkipRound()
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_SKIP_ROUND;
			StartScreen(0.016f);
		}

		public void CreateFight()
		{
			if (!base.gameObject.activeSelf || Type != ScreenFightType.TYPE_INFO_FIGHT)
			{
				ClearPictures();
				Type = ScreenFightType.TYPE_INFO_FIGHT;
				if (image != null)
				{
					image.set_SpriteName("FightUI.fight");
					image.SetNativeSize();
					image.gameObject.SetActive(true);
				}
				StartScreen(1.166f);
			}
		}

		public void CreateFightRule()
		{
			if (!ruleDesc.Equals(string.Empty))
			{
				ClearPictures();
				Type = ScreenFightType.TYPE_INFO_FIGHT_RULE;
				if (ruleDescription != null)
				{
					ruleDescription.gameObject.SetActive(true);
					ruleDescription.set_Alias(ruleDesc);
				}
				StartScreen(5.833f);
			}
		}

		public void CreateWinner(bool MBDILDFLMBL)
		{
			ClearPictures();
			Type = ((!MBDILDFLMBL) ? ScreenFightType.TYPE_INFO_COOL : ScreenFightType.TYPE_INFO_PERFECT);
			string spriteName = ((!MBDILDFLMBL) ? "FightUI.great" : "FightUI.perfect");
			if (image != null)
			{
				image.set_SpriteName(spriteName);
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			StartScreen(1.166f);
		}

		public void CreateTimesUp()
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_TIMESUP;
			if (image != null)
			{
				image.set_SpriteName("FightUI.timesup");
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			StartScreen(1.166f);
		}

		public void CreateRingOut()
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_RINGOUT;
			if (image != null)
			{
				image.set_SpriteName("FightUI.ringout");
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			StartScreen(1.166f);
		}

		public void CreateYouLose()
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_LOSE;
			if (image != null)
			{
				image.set_SpriteName("FightUI.youlose");
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			StartScreen(1.166f);
		}

		public void CreateYouWin()
		{
			ClearPictures();
			Type = ScreenFightType.TYPE_INFO_WIN;
			if (image != null)
			{
				image.set_SpriteName("FightUI.youwin");
				image.SetNativeSize();
				image.gameObject.SetActive(true);
			}
			StartScreen(1.166f);
		}

		public void Clear()
		{
			ClearPictures();
		}

		public void ClearPictures()
		{
			if (image != null)
			{
				image.gameObject.SetActive(false);
			}
			if (round != null)
			{
				round.gameObject.SetActive(false);
			}
			if (ruleDescription != null)
			{
				ruleDescription.gameObject.SetActive(false);
			}
			if (vsScreen != null)
			{
				vsScreen.gameObject.SetActive(false);
				Object.Destroy(vsScreen);
				vsScreen = null;
			}
			if (enemiesScreen != null)
			{
				enemiesScreen.gameObject.SetActive(false);
				Object.Destroy(enemiesScreen);
				enemiesScreen = null;
			}
		}

		public void CreateEnemiesScreen(List<ModelParameters> IDAAONBIBJM, int index, bool PPIJJHJCGGB)
		{
			Type = ScreenFightType.TYPE_INFO_ENEMIES;
			if (enemiesScreenPrefab == null)
			{
				StartScreen(0f);
				return;
			}
			ClearPictures();
			base.gameObject.SetActive(true);
			enemiesScreen = Object.Instantiate(enemiesScreenPrefab).GetComponent<EnemiesScreen>();
			enemiesScreen.transform.SetParent(base.transform, false);
			enemiesScreen.Init(IDAAONBIBJM, index, PPIJJHJCGGB);
			StartScreen(enemiesScreen.get_AnimationTime());
			OnStopScreen.AddListener(DKBOJEFOBCG);
		}

		private void DKBOJEFOBCG(ScreenFightType MPBIEONNLIJ)
		{
			if (MPBIEONNLIJ == ScreenFightType.TYPE_INFO_ENEMIES)
			{
				OnStopScreen.RemoveListener(DKBOJEFOBCG);
				StartVS();
			}
		}

		private void StartVS()
		{
			Type = ScreenFightType.TYPE_INFO_VS;
			if (vsScreenPrefab == null)
			{
				StartScreen(0f);
				return;
			}
			ClearPictures();
			vsScreen = Object.Instantiate(vsScreenPrefab).GetComponent<VsScreen>();
			vsScreen.transform.SetParent(base.transform, false);
			ModelParameters kIKOGDEPGHB = null;
			if (BBFBFDDDJMJ.BDANFHBMIOF.Count > BBFBFDDDJMJ.BLOOLFFMKFI)
			{
				kIKOGDEPGHB = BBFBFDDDJMJ.BDANFHBMIOF[BBFBFDDDJMJ.BLOOLFFMKFI];
			}
			else
			{
				if (BBFBFDDDJMJ.BDANFHBMIOF.Count <= 0)
				{
					StartScreen(0f);
					return;
				}
				kIKOGDEPGHB = BBFBFDDDJMJ.BDANFHBMIOF[0];
			}
			vsScreen.Init(BBFBFDDDJMJ.KMMJCHDKBDO, kIKOGDEPGHB);
			StartScreen(vsScreen.get_AnimationTime());
		}

		private void StartScreen(float LLIJBPJPHEL)
		{
			timer = LLIJBPJPHEL;
			Start();
		}

		public void Start()
		{
			base.gameObject.SetActive(true);
			KJIAPGDFEIK = true;
			OnStartScreen.Invoke(Type);
		}

		public void Stop()
		{
			base.gameObject.SetActive(false);
			KJIAPGDFEIK = false;
			OnStopScreen.Invoke(Type);
		}

		private void Update()
		{
			if (!KCANPMPILKI && KJIAPGDFEIK)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					Stop();
				}
			}
		}
	}
}
