using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

public class InfoPerk
{
	public enum HDDACJNDLEE
	{
		EVENT_MOD_EXPIRES = 0
	}

	public PerkData DCMHONAFOGI;

	private List<PerksStage.ActionPerk> NIDKKJFBNHO = new List<PerksStage.ActionPerk>();

	private List<PerksStage.ActionPerk> NBFBBDHELEJ = new List<PerksStage.ActionPerk>();

	private List<string> PCOPAMLECKI = new List<string>();

	private List<string> IEDBEDCKAIE = new List<string>();

	public List<PerksStage.ActionPerk> JMIIJAFLAEF
	{
		get
		{
			return MNLNLKOJPHO();
		}
	}

	public List<PerksStage.ActionPerk> DJBAIAKOIHM
	{
		get
		{
			return HIPOGANEPMI();
		}
	}

	public List<string> MBMBFONBKPE
	{
		get
		{
			return BFKDLIMHGFA();
		}
	}

	public List<string> HOKIPGPFMCM
	{
		get
		{
			return BKIMFEIMHCF();
		}
	}

	public bool LEBLFMFDKAA
	{
		get
		{
			return IHAHGIHPNIG();
		}
	}

	public List<PerksStage.ActionPerk> MNLNLKOJPHO()
	{
		return NIDKKJFBNHO;
	}

	public List<PerksStage.ActionPerk> HIPOGANEPMI()
	{
		return NBFBBDHELEJ;
	}

	public List<string> BFKDLIMHGFA()
	{
		return PCOPAMLECKI;
	}

	public List<string> BKIMFEIMHCF()
	{
		return IEDBEDCKAIE;
	}

	public bool IHAHGIHPNIG()
	{
		if (DCMHONAFOGI != null && DCMHONAFOGI.MBDDKGIOOGD != null && DCMHONAFOGI.MBDDKGIOOGD.ELPJBGIPEIB() != null)
		{
			return DCMHONAFOGI.MBDDKGIOOGD.ELPJBGIPEIB().EPCNJLEHJCB();
		}
		return false;
	}

	public void Render()
	{
		int num = 0;
		int count = NBFBBDHELEJ.Count;
		while (num < NBFBBDHELEJ.Count)
		{
			count = NBFBBDHELEJ.Count;
			PerksStage.ActionPerk oAJGINIDKJD = NBFBBDHELEJ[num];
			CAIPNAAJICO(oAJGINIDKJD);
			if (oAJGINIDKJD.FLNLMIHEDCI > 0)
			{
				if (oAJGINIDKJD.KGNDJOLBBJF >= oAJGINIDKJD.FLNLMIHEDCI || oAJGINIDKJD.PLNNKKBPDJK)
				{
					ACKKGAAPLDG(oAJGINIDKJD);
				}
				oAJGINIDKJD.KGNDJOLBBJF++;
			}
			if (count == NBFBBDHELEJ.Count)
			{
				num++;
			}
		}
	}

	private void CAIPNAAJICO(PerksStage.ActionPerk IBODMPMJELJ)
	{
		if (IBODMPMJELJ.AMKJNPOCODK.get_Type() == ActionType.ACTION_MOD_HEALTH_CHANGE)
		{
			DDOGCEKKDMK(IBODMPMJELJ);
		}
	}

	public void Run()
	{
		if (NIDKKJFBNHO.Count > 0)
		{
			MHHNIPBJNAD(NIDKKJFBNHO);
			NIDKKJFBNHO.Clear();
		}
	}

	private void ALBIODLFMAK(PerksStage.ActionPerk IBODMPMJELJ, bool PENNHKHFEOM)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("PERK ----- ");
		stringBuilder.Append((!PENNHKHFEOM) ? "ModDestruction " : "ModStart ");
		stringBuilder.Append("PerkName: {0} ModName: {1} ModXML: {2}");
		if (LogRules.ELEBLBJKDBI().DKHBLILFCOA())
		{
			LLLOJBFMONN.INNGABABJPC(stringBuilder.ToString(), IBODMPMJELJ.AMKJNPOCODK.GNDAFILBLIB().JMDLAMHAJLN().Name, IBODMPMJELJ.AMKJNPOCODK.get_Name(), IBODMPMJELJ.AMKJNPOCODK.FDEKGNPKJFL());
		}
	}

	public void MHHNIPBJNAD(List<PerksStage.ActionPerk> AFENHJFICNN)
	{
		foreach (PerksStage.ActionPerk item in AFENHJFICNN)
		{
			PerksStage.ActionPerk oAJGINIDKJD = ((!item.AMKJNPOCODK.NKAEEFNNBEN()) ? item : new PerksStage.ActionPerk(item));
			switch (item.AMKJNPOCODK.get_Type())
			{
			case ActionType.ACTION_SHOW_ICONS:
				MBKLEKPDGOA(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_MOD_HEALTH_CHANGE:
				IEDBKHEFKDE(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_SET_ATTRIBUTES:
				NMIGELMNBDF(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_INVISIBILITY:
				PCCAPNKPOKB(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_DISABLE_INTERVAL:
				LICINJMMICM(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_HIT:
				LHNCAIDDJIJ(oAJGINIDKJD);
				break;
			case ActionType.ACTION_LIFE_STEAL:
				GKLCDJLBBAM(oAJGINIDKJD);
				break;
			case ActionType.ACTION_ADD_BULLETS:
				KGFBIAOGHFF(oAJGINIDKJD);
				break;
			case ActionType.ACTION_ADD_MAGIC:
				DLMEDFNIEHI(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_MOD_FRAMES:
				AGPDKNAEDPB(oAJGINIDKJD);
				break;
			case ActionType.ACTION_MOD_EFFECT:
				HNAIFDHOMPL(oAJGINIDKJD);
				break;
			case ActionType.ACTION_PROVOKE:
				CPKHOBHFJDN(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_TACTICS:
				DGNKIJEICCJ(oAJGINIDKJD);
				break;
			case ActionType.ACTION_CLEAR_ACTION:
				OLEBPFBJCII(oAJGINIDKJD);
				break;
			case ActionType.ACTION_VARIABLE:
				KKODDGMCDBC(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_VARIABLE:
				MFKFMPAPHDG(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_COOLDOWN:
				LIPMLGCPAJG(oAJGINIDKJD);
				break;
			case ActionType.ACTION_CHANGE_IMPULSE:
				APMFPHOALEO(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_CHANGE_HIT_EFFECT_SCALE:
				IMJCCNPMHKC(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_CHANGE_ADD_DAMAGE_VALUE:
				BFBGNIICAHE(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_CHANGE_MODEL_COLOR:
				NPNJDBJABMG(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_SLOW_MODEL:
				KCELDPMGNMI(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_TURN_OFF_COLLISION:
				FHDDBMFJBJJ(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_SWITCH:
				JFNHLKEEJNC(oAJGINIDKJD);
				break;
			case ActionType.ACTION_MARK_PERK_USED:
				PerksStage.ANPAFFMJMNG(DCMHONAFOGI.MBDDKGIOOGD.Name);
				break;
			case ActionType.ACTION_PERK_AREA:
				EAFKPBMOMKI(oAJGINIDKJD, false);
				break;
			case ActionType.ACTION_MOVE_MODEL:
				NEKDJLPGMAH(oAJGINIDKJD);
				break;
			case ActionType.ACTION_SET_MOVES_VARIABLE:
				DHPEJIFPLCF(oAJGINIDKJD);
				break;
			case ActionType.ACTION_STEAL_MAGIC:
				PHHLFMLOPEK(oAJGINIDKJD, false);
				break;
			}
			if (item.AMKJNPOCODK.NKAEEFNNBEN())
			{
				NBFBBDHELEJ.Add(oAJGINIDKJD);
				PCOPAMLECKI.Add(oAJGINIDKJD.AMKJNPOCODK.get_Name());
				PerkActionModificator cKCICHAIMFL = (PerkActionModificator)oAJGINIDKJD.AMKJNPOCODK;
				if (cKCICHAIMFL.IONIEDIPEGB() != string.Empty)
				{
					PerksStage.HKMMGCLNJCN(oAJGINIDKJD);
				}
			}
			ALBIODLFMAK(item, true);
		}
		AFENHJFICNN.Clear();
		ClearActions();
	}

	private void MBKLEKPDGOA(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		if (!CCBEDPIHKAD)
		{
			ACBNLJBJGDF();
		}
		PerkActionShowIcon fMJDHMBCMKL = (PerkActionShowIcon)IBODMPMJELJ.AMKJNPOCODK;
		if (fMJDHMBCMKL.AJAEJNGLKOK() != string.Empty)
		{
			IBODMPMJELJ.NHKMCLPOMFK = string.Format("{0}{1}", SF2Paths.KLIDILIHOFF(), fMJDHMBCMKL.AJAEJNGLKOK());
		}
		else
		{
			IBODMPMJELJ.NHKMCLPOMFK = string.Format("{0}{1}", SF2Paths.KLIDILIHOFF(), DCMHONAFOGI.MBDDKGIOOGD.NHKMCLPOMFK);
		}
		IBODMPMJELJ.FLNCPBKBJBL = fMJDHMBCMKL.ECKEHGCGBBP();
		IBODMPMJELJ.MGDCIODPHCH = fMJDHMBCMKL.NKHNFHIKGIG();
		IBODMPMJELJ.KJDFJPBIGJC.CKCCBJKIGIO(IBODMPMJELJ, CCBEDPIHKAD);
	}

	private void IEDBKHEFKDE(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
	}

	private void NMIGELMNBDF(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		int num = ((!CCBEDPIHKAD) ? 1 : (-1));
		PerkActionSetAttributes aHFKENAALLF = (PerkActionSetAttributes)IBODMPMJELJ.AMKJNPOCODK;
		foreach (KeyValuePair<string, FunctionExtension> item in aHFKENAALLF.NNBFJDJAAGI())
		{
			string key = item.Key;
			FunctionResult dEIHAOLOPLC = item.Value.IBCPKBBAFNH();
			Attributes cCODCLGOHKB = new Attributes();
			cCODCLGOHKB.Set(key, dEIHAOLOPLC.ToInt());
			int OEMALIFPGPO = 0;
			bool flag = cCODCLGOHKB.Get(key, ref OEMALIFPGPO);
			int OEMALIFPGPO2 = 0;
			IBODMPMJELJ.KJDFJPBIGJC.KMMJCHDKBDO.IBLHIAHECLK.Get(key, ref OEMALIFPGPO2, false, true);
			IBODMPMJELJ.KJDFJPBIGJC.KMMJCHDKBDO.IBLHIAHECLK.Set(key, OEMALIFPGPO2 + OEMALIFPGPO * num, true);
			if (key == "DamageFactor" && !CCBEDPIHKAD && IHAHGIHPNIG())
			{
				Model.StrikeResult gHHCDAFIKJE = IBODMPMJELJ.BIKLKJMNGKP.GHHCDAFIKJE;
				gHHCDAFIKJE.GGENIBPJPAG(DCMHONAFOGI.MBDDKGIOOGD.Id);
			}
		}
	}

	private void APMFPHOALEO(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		PerkActionChangeImpulse nKPJIECMIJB = (PerkActionChangeImpulse)IBODMPMJELJ.AMKJNPOCODK;
		float dHDMNHCIPEH = nKPJIECMIJB.NBECOMENIEH();
		float bGEEALIPKCC = nKPJIECMIJB.LEAGBJCDLLA();
		float lKPCKJOLJDO = nKPJIECMIJB.HODMHJNNFFG();
		if (CCBEDPIHKAD)
		{
			IBODMPMJELJ.KJDFJPBIGJC.MGNOBDLOINP();
		}
		else
		{
			IBODMPMJELJ.KJDFJPBIGJC.SetImpulseFactor(dHDMNHCIPEH, bGEEALIPKCC, lKPCKJOLJDO);
		}
	}

	private void IMJCCNPMHKC(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		PerkActionChangeHitEffectScale aCEHLJCDLKB = (PerkActionChangeHitEffectScale)IBODMPMJELJ.AMKJNPOCODK;
		float bAINMLLIKOL = aCEHLJCDLKB.DNOILFCGCGD();
		if (CCBEDPIHKAD)
		{
			IBODMPMJELJ.KJDFJPBIGJC.CKOEEMFHCFK();
		}
		else
		{
			IBODMPMJELJ.KJDFJPBIGJC.set_HitEffectScale(bAINMLLIKOL);
		}
	}

	private void BFBGNIICAHE(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		PerkActionChangeAdditionalDamageValue dMPBHHGACBP = (PerkActionChangeAdditionalDamageValue)IBODMPMJELJ.AMKJNPOCODK;
		float bAINMLLIKOL = dMPBHHGACBP.JKEKBCJHANF();
		if (CCBEDPIHKAD)
		{
			IBODMPMJELJ.KJDFJPBIGJC.LNBCEJDJPAH();
			return;
		}
		IBODMPMJELJ.KJDFJPBIGJC.set_AdditionalDamageValue(bAINMLLIKOL);
		if (IHAHGIHPNIG())
		{
			Model.StrikeResult gHHCDAFIKJE = IBODMPMJELJ.BIKLKJMNGKP.GHHCDAFIKJE;
			gHHCDAFIKJE.GGENIBPJPAG(DCMHONAFOGI.MBDDKGIOOGD.Id);
		}
	}

	private void NPNJDBJABMG(PerksStage.ActionPerk action, bool remove)
	{
		PerkActionChangeModelColor color = (PerkActionChangeModelColor)action.AMKJNPOCODK;
		action.KJDFJPBIGJC.set_color(remove ? UnityEngine.Color.white : color.Color);
	}

	private void KCELDPMGNMI(PerksStage.ActionPerk action, bool remove)
	{
		PerkActionSlowModel slow = (PerkActionSlowModel)action.AMKJNPOCODK;
		action.KJDFJPBIGJC.SetPerkSlowFactor(remove ? 1 : slow.Speed);
	}

	private void FHDDBMFJBJJ(PerksStage.ActionPerk action, bool remove)
	{
		action.KJDFJPBIGJC.SetPerkCollisionDisabled(!remove);
	}

	private void JFNHLKEEJNC(PerksStage.ActionPerk action)
	{
		PerkActionSwitch switchAction = (PerkActionSwitch)action.AMKJNPOCODK;
		List<PerksStage.ActionPerk> selected = new List<PerksStage.ActionPerk>();
		foreach (PerkAction nested in switchAction.SelectActions())
		{
			PerksStage.ActionPerk nestedAction = new PerksStage.ActionPerk();
			nestedAction.AMKJNPOCODK = nested;
			nestedAction.BIKLKJMNGKP = action.BIKLKJMNGKP;
			nestedAction.KJDFJPBIGJC = nested.NKLMKGFAGFG(nested.JMDLAMHAJLN().ELPJBGIPEIB());
			if (nestedAction.KJDFJPBIGJC != null)
				selected.Add(nestedAction);
		}
		if (selected.Count != 0)
			MHHNIPBJNAD(selected);
	}

	private void EAFKPBMOMKI(PerksStage.ActionPerk action, bool remove)
	{
		Fight fight = Fight.OHNKFOHIAKG();
		if (fight == null)
			return;
		if (remove)
		{
			fight.NPFHCPAAIFJ();
			return;
		}
		PerkActionArea area = (PerkActionArea)action.AMKJNPOCODK;
		float x = area.PositionX.IBCPKBBAFNH().ToFloat();
		fight.CreatePerkActivationArea(area.Width, area.FileName, area.get_Name());
		fight.UpdatePerkActivationArea(x, area.ShiftY, true);
	}

	private void NEKDJLPGMAH(PerksStage.ActionPerk action)
	{
		PerkActionMoveModel move = (PerkActionMoveModel)action.AMKJNPOCODK;
		float offset = UnityEngine.Mathf.Abs(move.OffsetX.IBCPKBBAFNH().ToFloat());
		Model target = action.KJDFJPBIGJC;
		if (target == null)
			return;
		Model enemy = target.EGGEACCDAEK();
		if (enemy == null)
			return;
		Vector3f position = new Vector3f(target.PLBNCDCFPML());
		float direction = enemy.PLBNCDCFPML().GILCBJJPKBK() >= position.GILCBJJPKBK() ? 1f : -1f;
		position.JPFALPBDBAP(position.GILCBJJPKBK() + direction * offset);
		target.SetModelPosition(position);
	}

	private void DHPEJIFPLCF(PerksStage.ActionPerk action)
	{
		PerkActionSetMovesVariable variable = (PerkActionSetMovesVariable)action.AMKJNPOCODK;
		FunctionResult result = variable.Value.IBCPKBBAFNH();
		float number;
		if (float.TryParse(result.DCJLKCFKCOM, out number))
			action.KJDFJPBIGJC.EBABHGHPLFK().PerkVariables[variable.get_Name()] = number;
		else
			action.KJDFJPBIGJC.EBABHGHPLFK().PerkStringVariables[variable.get_Name()] = result.DCJLKCFKCOM;
	}

	private void PHHLFMLOPEK(PerksStage.ActionPerk action, bool remove)
	{
		if (remove)
		{
			if (action.PreviousMagic != null)
				action.KJDFJPBIGJC.SwapPerkItem(action.PreviousMagic);
			return;
		}
		PerkActionStealMagic steal = (PerkActionStealMagic)action.AMKJNPOCODK;
		string magicName = steal.MagicName.IBCPKBBAFNH().DCJLKCFKCOM;
		ItemInfo magic = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(magicName);
		if (magic != null)
		{
			action.PreviousMagic = action.KJDFJPBIGJC.KMMJCHDKBDO.ADBKGIBBNHJ;
			action.KJDFJPBIGJC.SwapPerkItem(magic);
		}
	}

	private void PCCAPNKPOKB(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA != null)
		{
			gDBOMJODDEA.PHNCLBJKCOE(IBODMPMJELJ.KJDFJPBIGJC, CCBEDPIHKAD);
		}
	}

	private void LICINJMMICM(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionDisableInterval dDALHNPFHAO = (PerkActionDisableInterval)IBODMPMJELJ.AMKJNPOCODK;
		if (dDALHNPFHAO.KFDPPOKFMPI() != string.Empty)
		{
			IntervalAnimation.NGAJJDIEDGF lFLGCDNKNJI = IntervalAnimation.LAJMDAFFPJE(dDALHNPFHAO.KFDPPOKFMPI());
			IBODMPMJELJ.KJDFJPBIGJC.RemoveInterval(lFLGCDNKNJI);
		}
		else if (dDALHNPFHAO.BIIIIDOCMEK() != string.Empty)
		{
			IBODMPMJELJ.KJDFJPBIGJC.RemoveInterval(dDALHNPFHAO.BIIIIDOCMEK());
		}
	}

	private void GKLCDJLBBAM(PerksStage.ActionPerk IBODMPMJELJ)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA != null)
		{
			PerkActionLifesteal gGFKBGKDALP = (PerkActionLifesteal)IBODMPMJELJ.AMKJNPOCODK;
			Model.StrikeResult gHHCDAFIKJE = IBODMPMJELJ.BIKLKJMNGKP.GHHCDAFIKJE;
			float num = (ObscuredFloat)(IBODMPMJELJ.KJDFJPBIGJC.KMMJCHDKBDO.KKMCHCNOHMB());
			float aACBFABMADJ = gGFKBGKDALP.NIBCOALEIDN() * gHHCDAFIKJE.EEDJBBOCFNL * (IBODMPMJELJ.KJDFJPBIGJC.EGGEACCDAEK().LJCFIOPBNKD() / gHHCDAFIKJE.KJDFJPBIGJC.LJCFIOPBNKD());
			gDBOMJODDEA.UpdateLife(IBODMPMJELJ.KJDFJPBIGJC, aACBFABMADJ);
		}
	}

	private void LHNCAIDDJIJ(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionSetHit jLKGFCFBJGE = (PerkActionSetHit)IBODMPMJELJ.AMKJNPOCODK;
		Model bIKLKJMNGKP = IBODMPMJELJ.BIKLKJMNGKP;
		Model.StrikeResult gHHCDAFIKJE = bIKLKJMNGKP.GHHCDAFIKJE;
		bool flag = true;
		if (jLKGFCFBJGE.LFJCOGGNFHL() > -1)
		{
			gHHCDAFIKJE.DNGKOMPMPCD = jLKGFCFBJGE.LFJCOGGNFHL() > 0;
			if (!gHHCDAFIKJE.DNGKOMPMPCD)
			{
				flag = false;
			}
		}
		if (jLKGFCFBJGE.JEIAJBMLIBP() > -1)
		{
			gHHCDAFIKJE.APCAKCCOMLO = jLKGFCFBJGE.JEIAJBMLIBP() > 0;
		}
		if (jLKGFCFBJGE.NALPADHBLNH() > -1)
		{
			gHHCDAFIKJE.NIKPBGPPFEP = jLKGFCFBJGE.NALPADHBLNH() > 0;
		}
		if (jLKGFCFBJGE.IOAHLEKLBLE() > -1)
		{
			gHHCDAFIKJE.DFOHNJEBDED = jLKGFCFBJGE.IOAHLEKLBLE() > 0;
		}
		if (jLKGFCFBJGE.GHGGNMBCMNM() != null)
		{
			FunctionResult dEIHAOLOPLC = jLKGFCFBJGE.GHGGNMBCMNM().IBCPKBBAFNH();
			gHHCDAFIKJE.NPDHOJEHPDM = dEIHAOLOPLC.ToFloat();
			gHHCDAFIKJE.EEDJBBOCFNL = dEIHAOLOPLC.ToFloat();
		}
		if (IHAHGIHPNIG() && flag)
		{
			gHHCDAFIKJE.GGENIBPJPAG(DCMHONAFOGI.MBDDKGIOOGD.Id);
		}
	}

	private void DGNKIJEICCJ(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionSetTactics fBDAHEODOGP = (PerkActionSetTactics)IBODMPMJELJ.AMKJNPOCODK;
		IBODMPMJELJ.KJDFJPBIGJC.CIFKBIPDCHK(fBDAHEODOGP.NLCLHLIPFFH());
	}

	private void KGFBIAOGHFF(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionAddBullets cLBEGGLEHMB = (PerkActionAddBullets)IBODMPMJELJ.AMKJNPOCODK;
		FunctionResult dEIHAOLOPLC = cLBEGGLEHMB.OEAKCOHMIHH().IBCPKBBAFNH();
		int fOIPKLDNGDL = dEIHAOLOPLC.ToInt();
		if (cLBEGGLEHMB.MPGDOMBCAAF() == "MagicBullet")
		{
			IBODMPMJELJ.KJDFJPBIGJC.IPGBFKOCOCK(fOIPKLDNGDL);
			IBODMPMJELJ.KJDFJPBIGJC.BFBFNKMLOJA();
		}
	}

	private void DLMEDFNIEHI(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionAddMagicCharge aMOILMJLADC = (PerkActionAddMagicCharge)IBODMPMJELJ.AMKJNPOCODK;
		FunctionResult dEIHAOLOPLC = aMOILMJLADC.OEAKCOHMIHH().IBCPKBBAFNH();
		float fOIPKLDNGDL = dEIHAOLOPLC.ToFloat();
		IBODMPMJELJ.KJDFJPBIGJC.JJHLOKBPBLD(fOIPKLDNGDL);
		IBODMPMJELJ.KJDFJPBIGJC.BFBFNKMLOJA();
	}

	private void AGPDKNAEDPB(PerksStage.ActionPerk IBODMPMJELJ)
	{
		ACBNLJBJGDF();
		PerkActionSetModFrames iGDDHFCDELM = (PerkActionSetModFrames)IBODMPMJELJ.AMKJNPOCODK;
		iGDDHFCDELM.NFPODDJPNEL().IBCPKBBAFNH();
		FunctionResult dEIHAOLOPLC = iGDDHFCDELM.NFPODDJPNEL().IBCPKBBAFNH();
		int fLNLMIHEDCI = dEIHAOLOPLC.ToInt();
		foreach (PerksStage.ActionPerk item in NBFBBDHELEJ)
		{
			if (iGDDHFCDELM.CMKKGFDBBJF() == item.AMKJNPOCODK.get_Name())
			{
				item.FLNLMIHEDCI = fLNLMIHEDCI;
				item.KGNDJOLBBJF = 0;
			}
		}
		if (iGDDHFCDELM.IONIEDIPEGB() == null || !(iGDDHFCDELM.IONIEDIPEGB() != string.Empty))
		{
			return;
		}
		List<PerksStage.ActionPerk> list = PerksStage.DOAECFNPKIO(iGDDHFCDELM.IONIEDIPEGB());
		if (list == null)
		{
			return;
		}
		foreach (PerksStage.ActionPerk item2 in list)
		{
			if (iGDDHFCDELM.CMKKGFDBBJF().Equals(item2.AMKJNPOCODK.get_Name()))
			{
				item2.FLNLMIHEDCI = fLNLMIHEDCI;
				item2.KGNDJOLBBJF = 0;
			}
		}
	}

	private void HNAIFDHOMPL(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionSetModEffect fBLKPCHKAHM = (PerkActionSetModEffect)IBODMPMJELJ.AMKJNPOCODK;
		string text = fBLKPCHKAHM.IONIEDIPEGB();
		if (text != null && text != string.Empty)
		{
			PerksStage.ActionPerk oAJGINIDKJD = PerksStage.AFAGHKFHHIF(fBLKPCHKAHM.CMKKGFDBBJF(), text);
			if (oAJGINIDKJD != null)
			{
				oAJGINIDKJD.KJDFJPBIGJC.GICAFBABMGA(oAJGINIDKJD, IBODMPMJELJ);
			}
			return;
		}
		foreach (PerksStage.ActionPerk item in NBFBBDHELEJ)
		{
			if (fBLKPCHKAHM.CMKKGFDBBJF() == item.AMKJNPOCODK.get_Name())
			{
				item.KJDFJPBIGJC.GICAFBABMGA(item, IBODMPMJELJ);
			}
		}
	}

	private void CPKHOBHFJDN(PerksStage.ActionPerk IBODMPMJELJ)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA == null)
		{
			return;
		}
		PerkActionProvoke bLAIFJHNJIO = (PerkActionProvoke)IBODMPMJELJ.AMKJNPOCODK;
		PerkInfoItem aCONCDFDNJH = bLAIFJHNJIO.GNDAFILBLIB().JMDLAMHAJLN();
		InfoPerk bPDFFLADJMJ = gDBOMJODDEA.IEEGPNLEKHH().BELALEGDCDM(IBODMPMJELJ.KJDFJPBIGJC, aCONCDFDNJH);
		if (bPDFFLADJMJ != null)
		{
			List<string> list = bPDFFLADJMJ.BFKDLIMHGFA();
		}
		List<PerkTrigger> list2 = new List<PerkTrigger>();
		foreach (PerkTrigger item in aCONCDFDNJH.NOJEIGNOPII())
		{
			if (item.get_Name() == bLAIFJHNJIO.FFLBCPJJKEJ() && item.IPFOGLIBLLB(IBODMPMJELJ.KJDFJPBIGJC, BFKDLIMHGFA()))
			{
				list2.Add(item);
			}
		}
		foreach (PerkTrigger item2 in list2)
		{
			gDBOMJODDEA.IEEGPNLEKHH().MHHNIPBJNAD(IBODMPMJELJ.KJDFJPBIGJC, item2, true);
		}
	}

	private void OLEBPFBJCII(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionClearAction hHMDDFCJDEO = (PerkActionClearAction)IBODMPMJELJ.AMKJNPOCODK;
		foreach (PerksStage.ActionPerk item in NBFBBDHELEJ)
		{
			if (hHMDDFCJDEO.DDBPICENEJE() == string.Empty || hHMDDFCJDEO.DDBPICENEJE() == item.AMKJNPOCODK.get_Name())
			{
				item.PLNNKKBPDJK = true;
			}
		}
		if (hHMDDFCJDEO.IONIEDIPEGB() == null || !(hHMDDFCJDEO.IONIEDIPEGB() != string.Empty))
		{
			return;
		}
		List<PerksStage.ActionPerk> list = PerksStage.DOAECFNPKIO(hHMDDFCJDEO.IONIEDIPEGB());
		if (list == null)
		{
			return;
		}
		foreach (PerksStage.ActionPerk item2 in list)
		{
			if (hHMDDFCJDEO.DDBPICENEJE() == null || hHMDDFCJDEO.DDBPICENEJE().Equals(string.Empty) || hHMDDFCJDEO.DDBPICENEJE().Equals(item2.AMKJNPOCODK.get_Name()))
			{
				item2.PLNNKKBPDJK = true;
			}
		}
	}

	private void DDOGCEKKDMK(PerksStage.ActionPerk IBODMPMJELJ)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA != null)
		{
			ModHealthChange eFIMNMBMCIJ = (ModHealthChange)IBODMPMJELJ.AMKJNPOCODK;
			gDBOMJODDEA.UpdateLife(IBODMPMJELJ.KJDFJPBIGJC, eFIMNMBMCIJ.JMPIBKKAHJP());
		}
	}

	private void ACBNLJBJGDF()
	{
		if (IHAHGIHPNIG())
		{
			Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
			if (gDBOMJODDEA != null)
			{
				PerksStage.ANPAFFMJMNG(DCMHONAFOGI.MBDDKGIOOGD.Name);
			}
			else
			{
				LLLOJBFMONN.Error("Error: No fight on perk start");
			}
		}
	}

	private void ACKKGAAPLDG(PerksStage.ActionPerk IBODMPMJELJ)
	{
		IBODMPMJELJ.FLNLMIHEDCI = 0;
		if (IBODMPMJELJ.AMKJNPOCODK.BFJEFNHKPJI() != null)
		{
			FunctionResult dEIHAOLOPLC = IBODMPMJELJ.AMKJNPOCODK.BFJEFNHKPJI().IBCPKBBAFNH();
			IBODMPMJELJ.FLNLMIHEDCI = dEIHAOLOPLC.ToInt();
		}
		switch (IBODMPMJELJ.AMKJNPOCODK.get_Type())
		{
		case ActionType.ACTION_SHOW_ICONS:
			MBKLEKPDGOA(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_SET_ATTRIBUTES:
			NMIGELMNBDF(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_CHANGE_IMPULSE:
			APMFPHOALEO(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_CHANGE_HIT_EFFECT_SCALE:
			IMJCCNPMHKC(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_CHANGE_ADD_DAMAGE_VALUE:
			BFBGNIICAHE(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_CHANGE_MODEL_COLOR:
			NPNJDBJABMG(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_SLOW_MODEL:
			KCELDPMGNMI(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_TURN_OFF_COLLISION:
			FHDDBMFJBJJ(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_PERK_AREA:
			EAFKPBMOMKI(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_STEAL_MAGIC:
			PHHLFMLOPEK(IBODMPMJELJ, true);
			break;
		case ActionType.ACTION_INVISIBILITY:
			PCCAPNKPOKB(IBODMPMJELJ, true);
			break;
		}
		ALBIODLFMAK(IBODMPMJELJ, false);
		bool flag = IBODMPMJELJ.AMKJNPOCODK.NKAEEFNNBEN();
		string value = IBODMPMJELJ.AMKJNPOCODK.get_Name();
		Model kJDFJPBIGJC = IBODMPMJELJ.KJDFJPBIGJC;
		BBEMBELMEGP(IBODMPMJELJ);
		if (!flag)
		{
			return;
		}
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA != null)
		{
			PerkActionModificator cKCICHAIMFL = (PerkActionModificator)IBODMPMJELJ.AMKJNPOCODK;
			if (cKCICHAIMFL.IONIEDIPEGB() != null && cKCICHAIMFL.IONIEDIPEGB() != string.Empty)
			{
				PerksStage.AEMBNMFGDBN(IBODMPMJELJ);
			}
			string text = (string)gDBOMJODDEA.IEEGPNLEKHH().OFKIKABKDFD()["ModExpires"];
			if (text != null)
			{
				BKIMFEIMHCF().Add(text);
			}
			gDBOMJODDEA.IEEGPNLEKHH().OFKIKABKDFD()["ModExpires"] = value;
			gDBOMJODDEA.IEEGPNLEKHH().OFKIKABKDFD()["Namespace"] = cKCICHAIMFL.IONIEDIPEGB();
			gDBOMJODDEA.IEEGPNLEKHH().OFKIKABKDFD()["ParentPerk"] = cKCICHAIMFL.JMDLAMHAJLN();
			gDBOMJODDEA.IEEGPNLEKHH().JALOHCICLGN(kJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_MOD_EXPIRES, true);
			gDBOMJODDEA.IEEGPNLEKHH().CLBPEANCNOA(IBODMPMJELJ);
		}
	}

	private void BBEMBELMEGP(PerksStage.ActionPerk DIMEFLGFIME)
	{
		foreach (PerksStage.ActionPerk item in NBFBBDHELEJ)
		{
			if (DIMEFLGFIME == item)
			{
				NBFBBDHELEJ.Remove(item);
				PCOPAMLECKI.Remove(item.AMKJNPOCODK.get_Name());
				break;
			}
		}
	}

	private void KKODDGMCDBC(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionVariable nMCKMGOCCBO = (PerkActionVariable)IBODMPMJELJ.AMKJNPOCODK;
		string key = nMCKMGOCCBO.get_Name();
		FunctionResult dEIHAOLOPLC = nMCKMGOCCBO.OEAKCOHMIHH().IBCPKBBAFNH();
		float value;
		if (float.TryParse(dEIHAOLOPLC.DCJLKCFKCOM, out value))
		{
			IBODMPMJELJ.KJDFJPBIGJC.EBABHGHPLFK().PerkVariables[key] = value;
			IBODMPMJELJ.KJDFJPBIGJC.EBABHGHPLFK().PerkStringVariables.Remove(key);
		}
		else
		{
			IBODMPMJELJ.KJDFJPBIGJC.EBABHGHPLFK().PerkStringVariables[key] =
				dEIHAOLOPLC.DCJLKCFKCOM ?? string.Empty;
		}
	}

	private void MFKFMPAPHDG(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionSetVariable lBGDPLDCKFJ = (PerkActionSetVariable)IBODMPMJELJ.AMKJNPOCODK;
		string text = lBGDPLDCKFJ.get_Name();
		FunctionResult dEIHAOLOPLC = lBGDPLDCKFJ.OEAKCOHMIHH().IBCPKBBAFNH();
		float num = dEIHAOLOPLC.ToFloat();
		if (lBGDPLDCKFJ.CPDLBMAKCEK())
		{
			FunctionResult dEIHAOLOPLC2 = lBGDPLDCKFJ.MCOHCDPJHAK().IBCPKBBAFNH();
			float num2 = dEIHAOLOPLC2.ToFloat();
			if (num < num2)
			{
				num = num2;
			}
		}
		if (lBGDPLDCKFJ.OMLFBFOFJDD())
		{
			FunctionResult dEIHAOLOPLC3 = lBGDPLDCKFJ.BHIGOIHJBDK().IBCPKBBAFNH();
			float num3 = dEIHAOLOPLC3.ToFloat();
			if (num > num3)
			{
				num = num3;
			}
		}
		IBODMPMJELJ.KJDFJPBIGJC.EBABHGHPLFK().PerkVariables[text] = num;
		if (SystemProperties.DBBOCENKMGD())
		{
			LLLOJBFMONN.INNGABABJPC("SetVariable {0} = {1}", text, num);
		}
	}

	private void LIPMLGCPAJG(PerksStage.ActionPerk IBODMPMJELJ)
	{
		PerkActionSetCooldown bHPLOIHAPFP = (PerkActionSetCooldown)IBODMPMJELJ.AMKJNPOCODK;
		int num = bHPLOIHAPFP.BFJEFNHKPJI();
		string bAINMLLIKOL = bHPLOIHAPFP.GHHAKGGLBCN();
		FightCID dDNBGEJJGMG = (FightCID)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.KEY_TYPE, bAINMLLIKOL);
		IBODMPMJELJ.KJDFJPBIGJC.OBJCCBMMDJH(dDNBGEJJGMG, 0);
		IBODMPMJELJ.KJDFJPBIGJC.PJGPCDPPOHA(dDNBGEJJGMG, num);
		if (SystemProperties.DBBOCENKMGD())
		{
			LLLOJBFMONN.INNGABABJPC("SetCooldown button = {0}, frames = {1}", bHPLOIHAPFP.GHHAKGGLBCN(), num);
		}
	}

	public void ClearActions(bool GIBIGPCELOB = false)
	{
		if (NBFBBDHELEJ.Count <= 0)
		{
			return;
		}
		int num = 0;
		int count = NBFBBDHELEJ.Count;
		while (num < NBFBBDHELEJ.Count)
		{
			count = NBFBBDHELEJ.Count;
			PerksStage.ActionPerk oAJGINIDKJD = NBFBBDHELEJ[num];
			if (GIBIGPCELOB || oAJGINIDKJD.PLNNKKBPDJK)
			{
				ACKKGAAPLDG(oAJGINIDKJD);
			}
			if (count == NBFBBDHELEJ.Count)
			{
				num++;
			}
		}
	}

	public void PANKENFPNPN()
	{
		IEDBEDCKAIE.Clear();
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA != null)
		{
			gDBOMJODDEA.IEEGPNLEKHH().OFKIKABKDFD()["ModExpires"] = null;
		}
	}
}
