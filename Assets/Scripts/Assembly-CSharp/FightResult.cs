using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class FightResult
{
	public class LJFFIBFBGID
	{
		public ItemInfo DLKPBAJDHBO;

		public RewardItem NAIEGGHELIH;

		public bool IDGKPLBKDIB;
	}

	public class NCHCEPNIDGO
	{
		public MoneyStruct EOBGMFMAMOK;

		public bool IDGKPLBKDIB;
	}

	public class NFBOLAJJIAD
	{
		public CurrencyStruct NAKKNKPJNHB;

		public bool IDGKPLBKDIB;
	}

	public class OLJIFHLGHNM
	{
		public ResistanceStruct JIDLBLPFAAE;

		public bool IDGKPLBKDIB;
	}

	public class ResultPrizeStruct
	{
		public long GBGNFPNCGED;

		public long PNDAIFALIKF;

		public uint exp;

		public List<LJFFIBFBGID> HELFDCAIJNE = new List<LJFFIBFBGID>();

		public List<NFBOLAJJIAD> KIMJGOHCCPO = new List<NFBOLAJJIAD>();

		public List<OLJIFHLGHNM> KBMDJACLAOH = new List<OLJIFHLGHNM>();

		public RewardLottery FAPDEKOMOGH;

		public void Clear()
		{
			GBGNFPNCGED = 0L;
			PNDAIFALIKF = 0L;
			exp = 0u;
			HELFDCAIJNE.Clear();
			KIMJGOHCCPO.Clear();
			KBMDJACLAOH.Clear();
			FAPDEKOMOGH = null;
		}

		public void KFJABAMAKOD(Rewardable POHFOGPKMMK)
		{
			if (POHFOGPKMMK != null)
			{
				switch (POHFOGPKMMK.CLOGJMBMMPI)
				{
				case Rewardable.GADCOGHCGDP.REWARD_ITEM:
				{
					RewardItem jJBPBGKBEED = (RewardItem)POHFOGPKMMK;
					KFJABAMAKOD(jJBPBGKBEED);
					break;
				}
				case Rewardable.GADCOGHCGDP.REWARD_MONEY:
				{
					RewardMoney mNEDNJMBHMF = (RewardMoney)POHFOGPKMMK;
					KFJABAMAKOD(mNEDNJMBHMF);
					break;
				}
				case Rewardable.GADCOGHCGDP.REWARD_CURRENCY:
				{
					RewardCurrency oIPIAAJCEOO = (RewardCurrency)POHFOGPKMMK;
					KFJABAMAKOD(oIPIAAJCEOO);
					break;
				}
				case Rewardable.GADCOGHCGDP.REWARD_RESISTANCE:
				{
					RewardResistance gBKBCEGJNLA = (RewardResistance)POHFOGPKMMK;
					KFJABAMAKOD(gBKBCEGJNLA);
					break;
				}
				case Rewardable.GADCOGHCGDP.REWARD_LOTTERY:
				{
					RewardLottery mIPHAMDMKJB = (RewardLottery)POHFOGPKMMK;
					KFJABAMAKOD(mIPHAMDMKJB);
					break;
				}
				}
			}
		}

		public void KFJABAMAKOD(RewardMoney MNEDNJMBHMF)
		{
			GBGNFPNCGED += MNEDNJMBHMF.BANPBCOOFMB();
		}

		public void KFJABAMAKOD(RewardCurrency OIPIAAJCEOO)
		{
			if (OIPIAAJCEOO == null)
			{
				return;
			}
			GameCurrency cJJOFMHLFFM = GameUtils.AJDKHINLIDI.ICFINJLNCPM(OIPIAAJCEOO.Name);
			if (cJJOFMHLFFM == null)
			{
				return;
			}
			foreach (NFBOLAJJIAD item in KIMJGOHCCPO)
			{
				if (item.NAKKNKPJNHB.BKDEAGGPNAO == cJJOFMHLFFM)
				{
					int num = (ObscuredInt)(item.NAKKNKPJNHB.Count);
					num += OIPIAAJCEOO.NAHFILGJAPC();
					item.NAKKNKPJNHB.Count = (ObscuredInt)(num);
					return;
				}
			}
			int num2 = OIPIAAJCEOO.NAHFILGJAPC();
			if (num2 > 0)
			{
				CurrencyStruct nAKKNKPJNHB = new CurrencyStruct(cJJOFMHLFFM, num2);
				NFBOLAJJIAD nFBOLAJJIAD = new NFBOLAJJIAD();
				nFBOLAJJIAD.NAKKNKPJNHB = nAKKNKPJNHB;
				nFBOLAJJIAD.IDGKPLBKDIB = OIPIAAJCEOO.IDGKPLBKDIB;
				KIMJGOHCCPO.Add(nFBOLAJJIAD);
			}
		}

		public void KFJABAMAKOD(RewardResistance GBKBCEGJNLA)
		{
			if (GBKBCEGJNLA == null)
			{
				return;
			}
			GameResistance oOJJEOFENBJ = GameUtils.JNIMKHKGPHE.NDMEGBEFBPJ(GBKBCEGJNLA.Name);
			if (oOJJEOFENBJ == null)
			{
				return;
			}
			foreach (OLJIFHLGHNM item in KBMDJACLAOH)
			{
				if (item.JIDLBLPFAAE.PIFOHOOFJDE == oOJJEOFENBJ)
				{
					int num = (ObscuredInt)(item.JIDLBLPFAAE.Count);
					num += GBKBCEGJNLA.Value;
					item.JIDLBLPFAAE.Count = (ObscuredInt)(num);
					return;
				}
			}
			int iOHAOMLJECE = GBKBCEGJNLA.Value;
			if (iOHAOMLJECE > 0)
			{
				ResistanceStruct jIDLBLPFAAE = new ResistanceStruct(oOJJEOFENBJ, iOHAOMLJECE);
				OLJIFHLGHNM oLJIFHLGHNM = new OLJIFHLGHNM();
				oLJIFHLGHNM.JIDLBLPFAAE = jIDLBLPFAAE;
				oLJIFHLGHNM.IDGKPLBKDIB = GBKBCEGJNLA.IDGKPLBKDIB;
				KBMDJACLAOH.Add(oLJIFHLGHNM);
			}
		}

		public void KFJABAMAKOD(RewardLottery MIPHAMDMKJB)
		{
			if (MIPHAMDMKJB != null)
			{
				if (FAPDEKOMOGH == null)
				{
					FAPDEKOMOGH = MIPHAMDMKJB;
				}
				else
				{
					FAPDEKOMOGH.EDCOGMLOEHE.AddRange(MIPHAMDMKJB.EDCOGMLOEHE);
				}
			}
		}

		public void KFJABAMAKOD(RewardItem JJBPBGKBEED)
		{
			if (JJBPBGKBEED == null)
			{
				return;
			}
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(JJBPBGKBEED.Name);
			if (dKCHDHMLKHN != null)
			{
				return;
			}
			ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(JJBPBGKBEED.Name);
			if (dJKEECEOCJB == null)
			{
				return;
			}
			int num = ((JJBPBGKBEED.CMEFKONFDKN() <= 0) ? ListSF.CCDKHLAMKKO().PINDEKDNCNL() : JJBPBGKBEED.CMEFKONFDKN());
			ItemInfo dJKEECEOCJB2 = null;
			if (dJKEECEOCJB.MHGODOLNDLE == num)
			{
				dJKEECEOCJB2 = dJKEECEOCJB;
			}
			else
			{
				ItemInfo dJKEECEOCJB3 = dJKEECEOCJB.GetUpdateItemByLevel(num, false);
				dJKEECEOCJB2 = ((dJKEECEOCJB3 == null) ? dJKEECEOCJB : dJKEECEOCJB3);
			}
			if (JJBPBGKBEED.UpgradeNumber != 0)
			{
				List<UpgradeData> list = dJKEECEOCJB2.DNFDAGFAANJ(true, dJKEECEOCJB2.MHGODOLNDLE);
				uint count = (uint)list.Count;
				if (count != 0)
				{
					uint num2 = JJBPBGKBEED.UpgradeNumber;
					if (count - 1 < num2)
					{
						num2 = count - 1;
					}
					dJKEECEOCJB2 = dJKEECEOCJB.MPADIPJLMLH(list[(int)num2]);
				}
			}
			LJFFIBFBGID lJFFIBFBGID = new LJFFIBFBGID();
			lJFFIBFBGID.DLKPBAJDHBO = dJKEECEOCJB2;
			lJFFIBFBGID.NAIEGGHELIH = JJBPBGKBEED;
			lJFFIBFBGID.IDGKPLBKDIB = JJBPBGKBEED.IDGKPLBKDIB;
			HELFDCAIJNE.Add(lJFFIBFBGID);
		}

		public List<ItemInfo> PJNJIJIODHE(bool NLDNIHHPEFI = false)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (LJFFIBFBGID item in HELFDCAIJNE)
			{
				if (!NLDNIHHPEFI || item.IDGKPLBKDIB)
				{
					list.Add(item.DLKPBAJDHBO);
				}
			}
			return list;
		}

		public List<CurrencyStruct> JGJLJMHKJBM(bool NLDNIHHPEFI = false)
		{
			List<CurrencyStruct> list = new List<CurrencyStruct>();
			foreach (NFBOLAJJIAD item in KIMJGOHCCPO)
			{
				if (!NLDNIHHPEFI || item.IDGKPLBKDIB)
				{
					list.Add(item.NAKKNKPJNHB);
				}
			}
			return list;
		}

		public List<ResistanceStruct> IHLPFEPHBPG(bool NLDNIHHPEFI = false)
		{
			List<ResistanceStruct> list = new List<ResistanceStruct>();
			foreach (OLJIFHLGHNM item in KBMDJACLAOH)
			{
				if (!NLDNIHHPEFI || item.IDGKPLBKDIB)
				{
					list.Add(item.JIDLBLPFAAE);
				}
			}
			return list;
		}

		public RewardLottery LNKIDFKKABB()
		{
			return FAPDEKOMOGH;
		}
	}

	public GameOverTypes MHNEKAEGNBO = GameOverTypes.GAME_OVER_NONE;

	public ResultPrizeStruct PMIHPJFAJIO = new ResultPrizeStruct();

	public BattleType LFLGCDNKNJI;

	public int OKNNNLIPODI;

	public FightIDS DIAIIPCBMFL;

	public float NJNKGLJNNDH;

	public ComboStatistic AIOMDIAFHGB;

	public ComboStatistic MOJHPBGGNAH;

	public FightStatistics GBGNFPNCGED;

	public int HNDHMPKKPJF;

	public int IMIEKGEIOLN;

	public int PNDAIFALIKF;

	public List<ItemInfo> CBAHALBKMHC = new List<ItemInfo>();

	public ModelParameters ABKBEJBICOA;

	public ModelParameters LEBLJJCFKOP;

	public FightList KGKDKENMAOA;

	public void FMNLAFLKFOO(long BLOOFMGLMHP, long GICNLBOICGP, float KNDKJANLIDI, float BHGNKHIKGOG, float FKHKEHICPAH, float IFCOPPPDOCD, float LMKJOMKPOAM, float OJIPBDBMLLO, List<float> JGANMCPMMLN)
	{
		if (AIOMDIAFHGB != null)
		{
			AIOMDIAFHGB.EAOGOCDLLBD(BLOOFMGLMHP, GICNLBOICGP, (long)KNDKJANLIDI, BHGNKHIKGOG, FKHKEHICPAH, IFCOPPPDOCD, LMKJOMKPOAM, OJIPBDBMLLO, JGANMCPMMLN);
		}
	}

	public long KMGLLBMIDHJ()
	{
		return PMIHPJFAJIO.GBGNFPNCGED;
	}

	public long BNILCODHHKC()
	{
		return PMIHPJFAJIO.PNDAIFALIKF;
	}

	public float JGPIEDFAKLC()
	{
		return PMIHPJFAJIO.exp;
	}

	public bool IsWinner()
	{
		return MHNEKAEGNBO == GameOverTypes.GAME_OVER_WIN;
	}

	public bool EKBAHCGBNEM()
	{
		return MHNEKAEGNBO == GameOverTypes.GAME_OVER_RAID_ROUND_TIMEOUT;
	}

	public void FCKFOPMNFOF(RewardStruct LGDIIADDFLH, ComboStatistic AIOMDIAFHGB, ComboStatistic MOJHPBGGNAH, FightList KGKDKENMAOA)
	{
		if (LGDIIADDFLH == null)
		{
			this.AIOMDIAFHGB = AIOMDIAFHGB;
			this.MOJHPBGGNAH = MOJHPBGGNAH;
			return;
		}
		PMIHPJFAJIO.Clear();
		Reward lOELDGJGPIF = ((!ListSF.CCDKHLAMKKO().JPMPIDFGCJL()) ? LGDIIADDFLH.FMOGFMIGLNP : LGDIIADDFLH.LJLIFMOIAJJ);
		bool bLBDMKNOJEJ = true;
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if ((ObscuredFloat)(LGDIIADDFLH.BBCCBPIIELF.KOBOIFJNPMO(nKGLHEGIKKP.PINDEKDNCNL()).prizeBase) > 0f || (lOELDGJGPIF != null && (ObscuredFloat)(lOELDGJGPIF.KOBOIFJNPMO(nKGLHEGIKKP.PINDEKDNCNL()).prizeBase) > 0f) || KGKDKENMAOA.MOPEDKMDLFA > 0f)
		{
			bLBDMKNOJEJ = false;
		}
		BDLLAEPPAKL(LGDIIADDFLH.BBCCBPIIELF, KGKDKENMAOA.MOPEDKMDLFA, AIOMDIAFHGB, MOJHPBGGNAH, bLBDMKNOJEJ);
		BDLLAEPPAKL(lOELDGJGPIF, KGKDKENMAOA.MOPEDKMDLFA, AIOMDIAFHGB, MOJHPBGGNAH, bLBDMKNOJEJ);
	}

	public void BDLLAEPPAKL(Reward POHFOGPKMMK, float prizeBase, ComboStatistic ODOJIOOGLJM, ComboStatistic IHNEOCGCCJO, bool BLBDMKNOJEJ)
	{
		if (POHFOGPKMMK == null)
		{
			return;
		}
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		RewardPrize cMHHEHILIIH = POHFOGPKMMK.KOBOIFJNPMO(nKGLHEGIKKP.PINDEKDNCNL());
		float num = (ObscuredUInt)((POHFOGPKMMK == null) ? (ObscuredUInt)(0u) : cMHHEHILIIH.exp);
		NJNKGLJNNDH += num;
		AIOMDIAFHGB = ODOJIOOGLJM;
		MOJHPBGGNAH = IHNEOCGCCJO;
		long num2 = (ObscuredLong)((POHFOGPKMMK == null) ? (ObscuredLong)(0L) : cMHHEHILIIH.GBGNFPNCGED);
		float kNDKJANLIDI = (ObscuredLong)((POHFOGPKMMK == null) ? (ObscuredLong)(0L) : cMHHEHILIIH.PNDAIFALIKF);
		float num3 = 0f;
		num3 = ((POHFOGPKMMK != null && (ObscuredFloat)(cMHHEHILIIH.prizeBase) > 0f) ? (float)(ObscuredFloat)(cMHHEHILIIH.prizeBase) : ((prizeBase >= 0f) ? prizeBase : ((!BLBDMKNOJEJ) ? 0f : Mathf.Ceil((float)num2 * GameUtils.AAKJKANGFMJ.PMPDAOIGCLP))));
		FMNLAFLKFOO((long)num3, num2, kNDKJANLIDI, GameUtils.AAKJKANGFMJ.NJAIKCKFMNN, GameUtils.AAKJKANGFMJ.LOONMILKCFK, GameUtils.AAKJKANGFMJ.MLNBGDHDKLL, GameUtils.AAKJKANGFMJ.GKAEJDCDMHC, GameUtils.AAKJKANGFMJ.APCAKCCOMLO, GameUtils.AAKJKANGFMJ.Styles);
		PMIHPJFAJIO.GBGNFPNCGED = ENKOCJBKOMF();
		PMIHPJFAJIO.PNDAIFALIKF = AIJNEGMOOML();
		PMIHPJFAJIO.exp += (ObscuredUInt)(cMHHEHILIIH.exp);
		foreach (RewardMoney item in cMHHEHILIIH.MDJFGLELOBA)
		{
			PMIHPJFAJIO.KFJABAMAKOD(item);
		}
		foreach (RewardCurrency item2 in cMHHEHILIIH.KIMJGOHCCPO)
		{
			PMIHPJFAJIO.KFJABAMAKOD(item2);
		}
		foreach (RewardResistance item3 in cMHHEHILIIH.KBMDJACLAOH)
		{
			PMIHPJFAJIO.KFJABAMAKOD(item3);
		}
		if (cMHHEHILIIH.FAPDEKOMOGH != null)
		{
			PMIHPJFAJIO.KFJABAMAKOD(cMHHEHILIIH.FAPDEKOMOGH);
		}
		foreach (RewardItem item4 in cMHHEHILIIH.HELFDCAIJNE)
		{
			PMIHPJFAJIO.KFJABAMAKOD(item4);
		}
		foreach (RewardChoice item5 in cMHHEHILIIH.PNFMKMLLFHK)
		{
			PMIHPJFAJIO.KFJABAMAKOD(item5.OOOBLJIHBEP());
		}
	}

	private long AIJNEGMOOML()
	{
		if (AIOMDIAFHGB != null)
		{
			return AIOMDIAFHGB.ECOOCLMNFJM.AMFFCKOAAED;
		}
		return 0L;
	}

	private long ENKOCJBKOMF()
	{
		if (AIOMDIAFHGB != null)
		{
			return AIOMDIAFHGB.ECOOCLMNFJM.POPNFGNAOJD;
		}
		return 0L;
	}
}
