using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core.Fights.Controller;
using UnityEngine;

public class RulesInspector : global::EventDispatcher<object>
{
	public const float SCORE_TABLE_X = 0f;

	public const float SCORE_TABLE_Y = -220f;

	private Fight _fight;

	private List<PerkInfoItem> _playerPerks = new List<PerkInfoItem>();

	private List<PerkInfoItem> _enemyPerks = new List<PerkInfoItem>();

	private List<NoPerksRule> _playerNoPerksRules = new List<NoPerksRule>();

	private List<NoPerksRule> _enemyNoPerksRules = new List<NoPerksRule>();

	private List<InFightRule> _renderRules = new List<InFightRule>();

	private List<InFightRule> _collisionRules = new List<InFightRule>();

	private List<InFightRule> _hitRules = new List<InFightRule>();

	private List<InFightRule> _strikeRules = new List<InFightRule>();

	private List<InFightRule> _animationRules = new List<InFightRule>();

	private List<InFightRule> _physicsRules = new List<InFightRule>();

	private List<InFightRule> _crazyRules = new List<InFightRule>();

	private List<InFightRule> _timeoutRules = new List<InFightRule>();

	private List<InFightRule> _damageRules = new List<InFightRule>();

	private List<InFightRule> _comboRules = new List<InFightRule>();

	private List<InFightRule> _resistanceRules = new List<InFightRule>();

	private List<InFightRule> _inFightRules = new List<InFightRule>();

	private List<ItemRule> _itemRules = new List<ItemRule>();

	private List<ItemRule> _playerItemRules = new List<ItemRule>();

	private List<ItemRule> _enemyItemRules = new List<ItemRule>();

	private List<NoButtonRule> _noButtonRules = new List<NoButtonRule>();

	private List<NoAnimationRule> _noAnimationRules = new List<NoAnimationRule>();

	private List<ChangeFightRule> _changeFightRules = new List<ChangeFightRule>();

	private List<RandomRule> _randomRules = new List<RandomRule>();

	private List<AvatarRule> _avatarRules = new List<AvatarRule>();

	private List<NameRule> _nameRules = new List<NameRule>();

	private List<Rule> _rules = new List<Rule>();

	public bool RulesActive;

	public int CurrentRound;

	private int _randomRuleSeed;

	private bool _hasRandomSeed;

	public RulesInspector(Fight fight, FightList KGKDKENMAOA)
	{
		_fight = null;
		CurrentRound = 1;
		RulesActive = false;
		_hasRandomSeed = false;
		_randomRuleSeed = 0;
		Init(fight, KGKDKENMAOA);
	}

	public void Init(Fight fight, FightList KGKDKENMAOA)
	{
		_fight = fight;
		RulesActive = false;
		if (KGKDKENMAOA.FLKFFDLLBKA() != null && KGKDKENMAOA.FLKFFDLLBKA().HasRandomSeeds)
		{
			SetRandomRuleSeed(KGKDKENMAOA.FLKFFDLLBKA().BKDOAOCGJLJ());
			SetHasRandomSeed(true);
		}
		List<Rule> list = KGKDKENMAOA.BONNMLEJBJH();
		foreach (Rule item in list)
		{
			PutRule(item);
		}
	}

	public void CheckEvent(FightEvent KOJNCHKPLLN, RuleAppliance EJPOJJKKICO, object data)
	{
		if (!RulesActive)
		{
			return;
		}
		bool flag = false;
		List<InFightRule> list = new List<InFightRule>();
		switch (KOJNCHKPLLN)
		{
		case FightEvent.RenderEvent:
			list = _renderRules;
			flag = true;
			break;
		case FightEvent.CollisionEvent:
			list = _collisionRules;
			break;
		case FightEvent.HitEvent:
			list = _hitRules;
			break;
		case FightEvent.StrikeEvent:
			list = _strikeRules;
			break;
		case FightEvent.AnimationStartEvent:
			list = _animationRules;
			break;
		case FightEvent.PhysicsStartEvent:
			list = _physicsRules;
			break;
		case FightEvent.CrazyEvent:
			list = _crazyRules;
			break;
		case FightEvent.TimeoutEvent:
			list = _timeoutRules;
			break;
		case FightEvent.DamageCheckEvent:
			list = _damageRules;
			break;
		case FightEvent.ComboEvent:
			list = _comboRules;
			break;
		case FightEvent.ResistanceCheckEvent:
			list = _resistanceRules;
			break;
		default:
			LLLOJBFMONN.Error("Error - RulesInspector::checkEvent - unknown event %i", KOJNCHKPLLN);
			return;
		}
		foreach (InFightRule item in list)
		{
			if (item.HHHPGLLBPMF() && (item.EDAKADCHOLE() == EJPOJJKKICO || item.EDAKADCHOLE() == RuleAppliance.ApplianceAll || EJPOJJKKICO == RuleAppliance.ApplianceAll) && item.Compare(data))
			{
				RulePassed(item);
			}
		}
		if (flag)
		{
			CheckRulesRender();
		}
	}

	public void CheckPreDraws()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		foreach (InFightRule item in _inFightRules)
		{
			if (!item.HHHPGLLBPMF())
			{
				continue;
			}
			switch (item.get_Type())
			{
			case Rule.BCBLLMPAMLP.RuleRingout:
				if (!flag)
				{
					RingOutRule iKKBOBLOPDI = (RingOutRule)item;
					_fight.CreateRingout(iKKBOBLOPDI.EJHLFJBJHAN(), iKKBOBLOPDI.JFBOKNFDFDO(), iKKBOBLOPDI.IOCBNKAFHKL(), iKKBOBLOPDI.OCJHHNFNHMK());
					flag = true;
				}
				break;
			case Rule.BCBLLMPAMLP.RuleHotGround:
				if (!flag5)
				{
					HotGroundRule gCNCEGFIOKG = (HotGroundRule)item;
					if (gCNCEGFIOKG.BFOJOGLCIBB())
					{
						_fight.CreateHotGround(gCNCEGFIOKG.OCJHHNFNHMK(), gCNCEGFIOKG.APDIONCLEDH());
					}
					flag5 = true;
				}
				break;
			case Rule.BCBLLMPAMLP.RuleNoHealthBar:
				_fight.JKPOGNMHDNK(item.EDAKADCHOLE(), false);
				break;
			case Rule.BCBLLMPAMLP.RuleDarkness:
				if (!flag2)
				{
					_fight.HKOMIIDELBC();
					flag2 = true;
				}
				break;
			case Rule.BCBLLMPAMLP.RulePoints:
				if (!flag3)
				{
					PointsTableType nOPJGLHKJPG = ((PointsRule)item).GCKBDFJKPDC();
					int lOMKKEAMMIG = ((PointsRule)item).OEDHHGKAMID();
					_fight.ANAOBOCPCON(0f, -220f, nOPJGLHKJPG, lOMKKEAMMIG);
					flag3 = true;
				}
				break;
			case Rule.BCBLLMPAMLP.RuleRandomArea:
				if (!flag4)
				{
					_fight.CreatePerkActivationArea(((RandomAreaRule)item).HFDJFADIAEP(), ((RandomAreaRule)item).BPMABAFDFJK(), ((RandomAreaRule)item).AJIAFONPDKE());
				}
				break;
			}
		}
	}

	public void InitRules(object data)
	{
		foreach (InFightRule item in _inFightRules)
		{
			if (item.HHHPGLLBPMF())
			{
				item.InitRule(data);
				switch (item.get_Type())
				{
				case Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound:
					_fight.IFFANEPCAJB(item.EDAKADCHOLE());
					break;
				case Rule.BCBLLMPAMLP.RuleTactic:
					_fight.SetBotTactic(((TacticRule)item).ICIKNGANCGK());
					break;
				case Rule.BCBLLMPAMLP.RuleInvertJoystick:
					_fight.OHEIDPMLNDE(true);
					break;
				}
			}
		}
		CheckDamageRules(RuleAppliance.ApplianceAll);
		CheckResistanceRules();
	}

	public void ClearRules()
	{
		_playerPerks.Clear();
		_enemyPerks.Clear();
		foreach (InFightRule item in _inFightRules)
		{
			item.Clear();
		}
	}

	public void StopRules()
	{
		foreach (InFightRule item in _inFightRules)
		{
			if (item.HHHPGLLBPMF())
			{
				item.Stop();
				Rule.BCBLLMPAMLP bCBLLMPAMLP = item.get_Type();
				if (bCBLLMPAMLP == Rule.BCBLLMPAMLP.RuleDarkness)
				{
					_fight.DBIHABKLFHP(0f);
				}
			}
		}
	}

	public void SetRulesActivity()
	{
		bool flag = false;
		foreach (InFightRule item in _inFightRules)
		{
			flag = item.HAKHBAOJBON(CurrentRound) && item.CHDEIEMINPF();
			item.SetActive(flag);
		}
		foreach (NoButtonRule item2 in _noButtonRules)
		{
			flag = item2.HAKHBAOJBON(CurrentRound) && item2.CHDEIEMINPF();
			item2.SetActive(flag);
		}
		foreach (NoAnimationRule item3 in _noAnimationRules)
		{
			flag = item3.HAKHBAOJBON(CurrentRound) && item3.CHDEIEMINPF();
			item3.SetActive(flag);
		}
		foreach (ItemRule item4 in _itemRules)
		{
			flag = item4.HAKHBAOJBON(CurrentRound) && item4.CHDEIEMINPF();
			item4.SetActive(flag);
		}
		foreach (RandomRule item5 in _randomRules)
		{
			flag = item5.HAKHBAOJBON(CurrentRound) && item5.CHDEIEMINPF();
			item5.SetActive(flag);
		}
	}

	public void ApplyNoPerksRules(ModelParameters IHEFAMAFBIA, List<NoPerksRule> GOMIMEDNKHH)
	{
		if (IHEFAMAFBIA == null)
		{
			LLLOJBFMONN.Error("RulesInspector::applyNoPerksRules ERROR - modelParameters is NULL");
		}
		IHEFAMAFBIA.KOELCOMEJMI.Clear();
		foreach (NoPerksRule item2 in GOMIMEDNKHH)
		{
			string item = item2.DMEDLGGNAIK();
			IHEFAMAFBIA.KOELCOMEJMI.Add(item);
		}
		IHEFAMAFBIA.JEJPEJFLDJC(IHEFAMAFBIA.NHBIJEEKALC, IHEFAMAFBIA.KOELCOMEJMI);
	}

	public void ApplyNoAnimationRules(ModelParameters IHEFAMAFBIA)
	{
		if (IHEFAMAFBIA == null)
		{
			LLLOJBFMONN.Error("RulesInspector::applyNoAnimationRules ERROR - modelParameters is NULL");
		}
		IHEFAMAFBIA.DANNKMJOOOH.Clear();
		foreach (NoAnimationRule item in _noAnimationRules)
		{
			if (item.HHHPGLLBPMF())
			{
				IHEFAMAFBIA.DANNKMJOOOH.Add(item.DPKNMJMPEDM());
			}
		}
	}

	public void ApplyAvatarAndNameRules(ModelParameters IHEFAMAFBIA)
	{
		if (IHEFAMAFBIA == null)
		{
			LLLOJBFMONN.Error("RulesInspector::ApplyAvatarAndNameRules ERROR - modelParameters is NULL");
		}
		foreach (AvatarRule item in _avatarRules)
		{
			IHEFAMAFBIA.HNKFHGOOKEG = item.get_Name();
		}
		foreach (NameRule item2 in _nameRules)
		{
			IHEFAMAFBIA.BMFLPBLAFLK = item2.get_Name();
		}
	}

	public void CheckButtonRules(GameController LPGANKOAPJL)
	{
		if (LPGANKOAPJL == null)
		{
			LLLOJBFMONN.Error("RulesInspector::checkButtonRules ERROR - gameController is NULL");
			return;
		}
		foreach (NoButtonRule item in _noButtonRules)
		{
			if (item.HHHPGLLBPMF())
			{
				ApplyButtonRule(item, LPGANKOAPJL);
			}
		}
	}

	public void ApplyButtonRule(NoButtonRule HNBFMAKFJAM, GameController LPGANKOAPJL)
	{
		switch (HNBFMAKFJAM.KBINIBAGEFM())
		{
		case NoButtonRule.AHIDMNNEAEC.ButtonTypePunch:
			LPGANKOAPJL.SetPunchEnabled(false);
			break;
		case NoButtonRule.AHIDMNNEAEC.ButtonTypeKick:
			LPGANKOAPJL.SetKickEnabled(false);
			break;
		}
	}

	public void CheckChangeFightRules(FightList KGKDKENMAOA)
	{
		if (_changeFightRules.Count <= 0)
		{
			return;
		}
		ChangeFightRule iJCOGNNJLFA = null;
		foreach (ChangeFightRule item in _changeFightRules)
		{
			if (item.HHHPGLLBPMF())
			{
				iJCOGNNJLFA = item;
			}
		}
		if (iJCOGNNJLFA != null)
		{
			ApplyChangeFightRule(_changeFightRules[_changeFightRules.Count - 1], KGKDKENMAOA);
		}
	}

	public void ApplyChangeFightRule(ChangeFightRule HNBFMAKFJAM, FightList KGKDKENMAOA)
	{
		int num = HNBFMAKFJAM.NNMOHPAAFGI();
		if (num > 0)
		{
			KGKDKENMAOA.BDBBNECNMBP = num;
		}
		int num2 = HNBFMAKFJAM.IBHBDDFGEDN();
		if (num2 > 0)
		{
			KGKDKENMAOA.RoundTime = (ObscuredInt)(num2);
		}
	}

	public void ResetRandomRules()
	{
		ClearRandomRules();
		if (CurrentRound == 1)
		{
			if (_hasRandomSeed)
			{
				NekkiMath.KACCBCCEPGB(_randomRuleSeed);
			}
			else
			{
				NekkiMath.KACCBCCEPGB();
			}
			ResetRandomRules(RandomRule.EOAOMBKFMPF.REFRESH_EACH_FIGHT);
		}
		NekkiMath.KACCBCCEPGB();
		ResetRandomRules(RandomRule.EOAOMBKFMPF.REFRESH_EACH_ROUND);
		PutRandomRules();
	}

	public void ResetRules(int round)
	{
		ClearRules();
		CurrentRound = round;
		SetRulesActivity();
		ResetRandomRules();
		RefillPerksFromRules();
	}

	public void PrepareItemRules(List<ItemRule> JIILGONALOA)
	{
		foreach (ItemRule item in JIILGONALOA)
		{
			if (item != null && item.get_Type() == Rule.BCBLLMPAMLP.RuleRandomAquiredItem)
			{
				RandomAquiredItemRule kJNNJGGKBCO = (RandomAquiredItemRule)item;
				kJNNJGGKBCO.PINICFPAOAK();
			}
		}
	}

	public List<ItemRule> GetItemRules()
	{
		return _itemRules;
	}

	public List<ItemRule> GetPlayerItemRules()
	{
		return _playerItemRules;
	}

	public List<ItemRule> GetEnemyItemRules()
	{
		return _enemyItemRules;
	}

	public List<PerkInfoItem> GetPlayerPerks()
	{
		return _playerPerks;
	}

	public List<PerkInfoItem> GetEnemyPerks()
	{
		return _enemyPerks;
	}

	public List<NoPerksRule> GetPlayerNoPerks()
	{
		return _playerNoPerksRules;
	}

	public List<NoPerksRule> GetEnemyNoPerks()
	{
		return _enemyNoPerksRules;
	}

	protected void RulePassed(InFightRule HNBFMAKFJAM)
	{
		bool flag = false;
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleRegeneration:
		{
			float num2 = ((RegenerationRule)HNBFMAKFJAM).BIGCPKBIJNA();
			num2 /= (float)GameUtils.GGBABPJBGJB();
			if (_fight.UpdateLife(HNBFMAKFJAM.EDAKADCHOLE(), num2))
			{
				_fight.HAANFNBPMBE(HNBFMAKFJAM);
			}
			break;
		}
		case Rule.BCBLLMPAMLP.RuleLifeSteal:
		{
			float num = ((LifeStealRule)HNBFMAKFJAM).FGJOBADADEB();
			num /= (float)GameUtils.GGBABPJBGJB();
			if (_fight.UpdateLife(HNBFMAKFJAM.EDAKADCHOLE(), num))
			{
				_fight.HAANFNBPMBE(HNBFMAKFJAM);
			}
			break;
		}
		case Rule.BCBLLMPAMLP.RuleRingout:
		case Rule.BCBLLMPAMLP.RuleHotGround:
			if (HNBFMAKFJAM.OBDNDAEPPNN())
			{
				_fight.ALNNLCAKCAF(HNBFMAKFJAM.EDAKADCHOLE());
			}
			_fight.HAANFNBPMBE(HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleLoseFall:
		case Rule.BCBLLMPAMLP.RuleTimeoutWin:
		case Rule.BCBLLMPAMLP.RuleWinStyle:
		case Rule.BCBLLMPAMLP.RuleWinCombo:
		case Rule.BCBLLMPAMLP.RuleWinShock:
			_fight.HAANFNBPMBE(HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RulePoints:
			_fight.UpdatePointsTable(((PointsRule)HNBFMAKFJAM).BDHKJEFJNFJ(), ((PointsRule)HNBFMAKFJAM).MHCBPGMIEEH());
			if (((PointsRule)HNBFMAKFJAM).FEIKKONCLFE())
			{
				_fight.HAANFNBPMBE(HNBFMAKFJAM);
			}
			break;
		case Rule.BCBLLMPAMLP.RuleCrazy:
		case Rule.BCBLLMPAMLP.RuleCombo:
			flag = true;
			break;
		}
		if (flag)
		{
			CheckDamageRules(HNBFMAKFJAM.EDAKADCHOLE());
		}
	}

	protected void CheckRulesRender()
	{
		foreach (InFightRule item in _renderRules)
		{
			if (!item.HHHPGLLBPMF())
			{
				continue;
			}
			switch (item.get_Type())
			{
			case Rule.BCBLLMPAMLP.RuleHotGround:
				if (((HotGroundRule)item).HADLDHHEOKM)
				{
					if (_fight.preFight != null)
					{
						_fight.preFight.ViewerUpdateHotGroundTimer(((HotGroundRule)item).NNOHILNKJEN(), item.EDAKADCHOLE());
					}
					((HotGroundRule)item).HADLDHHEOKM = false;
				}
				break;
			case Rule.BCBLLMPAMLP.RuleDarkness:
				_fight.DBIHABKLFHP(((DarknessRule)item).CFNAMMODOAA());
				break;
			case Rule.BCBLLMPAMLP.RuleRandomArea:
			{
				RandomAreaRule dFAONBFDMKA = (RandomAreaRule)item;
				_fight.UpdatePerkActivationArea(dFAONBFDMKA.BOCHPMJBLGA(), dFAONBFDMKA.CFNAMMODOAA(), dFAONBFDMKA.JFFONEBNBMP());
				break;
			}
			}
		}
	}

	protected void CheckDamageRules(RuleAppliance EJPOJJKKICO)
	{
		if (EJPOJJKKICO == RuleAppliance.ApplianceAll)
		{
			CheckDamageRules(RuleAppliance.AppliancePlayer);
			CheckDamageRules(RuleAppliance.ApplianceOpponent);
			return;
		}
		bool flag = true;
		foreach (InFightRule item in _damageRules)
		{
			if (item.HHHPGLLBPMF() && item.EDAKADCHOLE() == EJPOJJKKICO)
			{
				flag = flag && !((DamageRule)item).BKEAKKCDMMN();
			}
		}
		RuleAppliance eJPOJJKKICO = ((EJPOJJKKICO != RuleAppliance.AppliancePlayer) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		_fight.AMLOPBMHPHC(eJPOJJKKICO).MABELGMBHEA(!flag);
	}

	protected void CheckResistanceRules()
	{
		Model fGCODGKLHED = _fight.AMLOPBMHPHC(RuleAppliance.AppliancePlayer);
		Model fGCODGKLHED2 = _fight.AMLOPBMHPHC(RuleAppliance.ApplianceOpponent);
		float num = 1f;
		float num2 = 1f;
		foreach (InFightRule item in _resistanceRules)
		{
			ResistanceRule hCOHJNFLKIF = item as ResistanceRule;
			if (hCOHJNFLKIF != null)
			{
				string gOHIIMFFFJI = hCOHJNFLKIF.DJBFLJAIKLI();
				int num3 = hCOHJNFLKIF.GLBEGDFMDBO();
				int num4 = ListSF.CCDKHLAMKKO().IJCGBPDAAJF(gOHIIMFFFJI);
				if (num4 < num3)
				{
					float num5 = Mathf.Pow(2f, (float)(num3 - num4) / GameUtils.CHOGPMPEDIC());
					float num6 = Mathf.Pow(2f, (float)(num4 - num3) / GameUtils.CHOGPMPEDIC());
					num *= num6;
					num2 *= num5;
				}
			}
		}
		fGCODGKLHED.OLGNPKCPKOJ(num);
		fGCODGKLHED2.OLGNPKCPKOJ(num2);
	}

	protected void SetItemRules(List<ItemRule> GEEJLFGCKNJ)
	{
		_itemRules.AddRange(GEEJLFGCKNJ);
		foreach (ItemRule item in _itemRules)
		{
			_rules.Add(item);
		}
	}

	protected void SetNoButtonRules(List<NoButtonRule> PINLMLCCFPH)
	{
		_noButtonRules.AddRange(PINLMLCCFPH);
		foreach (NoButtonRule item in _noButtonRules)
		{
			_rules.Add(item);
		}
	}

	protected void SetNoAnimationRules(List<NoAnimationRule> IMAJKIFPLNM)
	{
		_noAnimationRules.AddRange(IMAJKIFPLNM);
		foreach (NoAnimationRule item in _noAnimationRules)
		{
			_rules.Add(item);
		}
	}

	protected void SetInFightRules(List<InFightRule> JIILGONALOA)
	{
		foreach (InFightRule item in JIILGONALOA)
		{
			SetInFightRule(item);
		}
	}

	protected void SetInFightRule(InFightRule HNBFMAKFJAM)
	{
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.RenderEvent))
		{
			_renderRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.CollisionEvent))
		{
			_collisionRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.HitEvent))
		{
			_hitRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.AnimationStartEvent))
		{
			_animationRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.PhysicsStartEvent))
		{
			_physicsRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.CrazyEvent))
		{
			_crazyRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.StrikeEvent))
		{
			_strikeRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.TimeoutEvent))
		{
			_timeoutRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.DamageCheckEvent))
		{
			_damageRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.ComboEvent))
		{
			_comboRules.Add(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.ResistanceCheckEvent))
		{
			_resistanceRules.Add(HNBFMAKFJAM);
		}
		_inFightRules.AddIfNotExist(HNBFMAKFJAM);
		_rules.AddIfNotExist(HNBFMAKFJAM);
	}

	protected void DeactivateInFightRule(InFightRule HNBFMAKFJAM)
	{
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.RenderEvent))
		{
			_renderRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.CollisionEvent))
		{
			_collisionRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.HitEvent))
		{
			_hitRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.AnimationStartEvent))
		{
			_animationRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.PhysicsStartEvent))
		{
			_physicsRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.CrazyEvent))
		{
			_crazyRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.StrikeEvent))
		{
			_strikeRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.TimeoutEvent))
		{
			_timeoutRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.DamageCheckEvent))
		{
			_damageRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.ComboEvent))
		{
			_comboRules.Remove(HNBFMAKFJAM);
		}
		if (HNBFMAKFJAM.PMBJPCMHJOA(FightEvent.ResistanceCheckEvent))
		{
			_resistanceRules.Remove(HNBFMAKFJAM);
		}
	}

	protected void SetRandomRules(List<RandomRule> GOAJNDLFBDN)
	{
		_randomRules.AddRange(GOAJNDLFBDN);
	}

	protected void RemoveRule(Rule HNBFMAKFJAM)
	{
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleComplex:
			RemoveComplexRule((ComplexRule)HNBFMAKFJAM);
			return;
		case Rule.BCBLLMPAMLP.RuleItem:
		case Rule.BCBLLMPAMLP.RuleEquipItem:
		case Rule.BCBLLMPAMLP.RuleRandomAquiredItem:
			_itemRules.Remove((ItemRule)HNBFMAKFJAM);
			_playerItemRules.Remove((ItemRule)HNBFMAKFJAM);
			_enemyItemRules.Remove((ItemRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleNoButton:
			_noButtonRules.Remove((NoButtonRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleNoAnimation:
			_noAnimationRules.Remove((NoAnimationRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleChangeFight:
			_changeFightRules.Remove((ChangeFightRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleRingout:
		case Rule.BCBLLMPAMLP.RuleHotGround:
		case Rule.BCBLLMPAMLP.RuleLoseFall:
		case Rule.BCBLLMPAMLP.RuleRegeneration:
		case Rule.BCBLLMPAMLP.RuleAttributes:
		case Rule.BCBLLMPAMLP.RuleDamageFactor:
		case Rule.BCBLLMPAMLP.RuleRemoveInterval:
		case Rule.BCBLLMPAMLP.RuleCrazy:
		case Rule.BCBLLMPAMLP.RuleLifeSteal:
		case Rule.BCBLLMPAMLP.RuleNoHealthBar:
		case Rule.BCBLLMPAMLP.RuleCombo:
		case Rule.BCBLLMPAMLP.RuleTimeoutWin:
		case Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound:
		case Rule.BCBLLMPAMLP.RuleNoBulletsReplenishment:
		case Rule.BCBLLMPAMLP.RuleInvulnerability:
		case Rule.BCBLLMPAMLP.RuleResistance:
			RemoveInFightRule((InFightRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleDarkness:
		case Rule.BCBLLMPAMLP.RulePoints:
		case Rule.BCBLLMPAMLP.RuleInvertJoystick:
		case Rule.BCBLLMPAMLP.RuleRandomArea:
			RemoveInFightRule((InFightRule)HNBFMAKFJAM, true);
			break;
		case Rule.BCBLLMPAMLP.RuleAvatar:
			_avatarRules.Remove((AvatarRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleName:
			_nameRules.Remove((NameRule)HNBFMAKFJAM);
			break;
		}
		_rules.Remove(HNBFMAKFJAM);
	}

	protected void RemoveComplexRule(ComplexRule FPMPFCGEBKE)
	{
		List<Rule> list = FPMPFCGEBKE.BONNMLEJBJH();
		foreach (Rule item in list)
		{
			RemoveRule(item);
		}
		_rules.Remove(FPMPFCGEBKE);
	}

	protected void PutPerkFromRule(PerkRule HNBFMAKFJAM)
	{
		switch (HNBFMAKFJAM.EDAKADCHOLE())
		{
		case RuleAppliance.AppliancePlayer:
			_playerPerks.Add(HNBFMAKFJAM.GNIICEKAJKC());
			break;
		case RuleAppliance.ApplianceOpponent:
			_enemyPerks.Add(HNBFMAKFJAM.GNIICEKAJKC());
			break;
		default:
			LLLOJBFMONN.Error("RulesInspector::putPerkFromRule ERROR - wrong rule appliance %i", HNBFMAKFJAM.EDAKADCHOLE());
			break;
		}
	}

	protected void PutNoPerkFromRule(NoPerksRule HNBFMAKFJAM)
	{
		switch (HNBFMAKFJAM.EDAKADCHOLE())
		{
		case RuleAppliance.AppliancePlayer:
			_playerNoPerksRules.Add(HNBFMAKFJAM);
			break;
		case RuleAppliance.ApplianceOpponent:
			_enemyNoPerksRules.Add(HNBFMAKFJAM);
			break;
		case RuleAppliance.ApplianceAll:
			_playerNoPerksRules.Add(HNBFMAKFJAM);
			_enemyNoPerksRules.Add(HNBFMAKFJAM);
			break;
		default:
			LLLOJBFMONN.Error("RulesInspector::putPerkFromRule ERROR - wrong rule appliance %i", HNBFMAKFJAM.EDAKADCHOLE());
			break;
		}
	}

	protected void RefillPerksFromRules()
	{
		_playerPerks.Clear();
		_enemyPerks.Clear();
		foreach (Rule item in _rules)
		{
			if (item.HHHPGLLBPMF())
			{
				if (item.get_Type() == Rule.BCBLLMPAMLP.RulePerk)
				{
					PutPerkFromRule((PerkRule)item);
				}
				else if (item.get_Type() == Rule.BCBLLMPAMLP.RuleNoPerks)
				{
					PutNoPerkFromRule((NoPerksRule)item);
				}
			}
		}
	}

	protected void ClearRandomRules()
	{
		_rules.FindAll((Rule DHDMNHCIPEH) => DHDMNHCIPEH.IsRandom).ForEach((Rule DHDMNHCIPEH) =>
		{
			RemoveRule(DHDMNHCIPEH);
		});
	}

	protected void ResetRandomRules(RandomRule.EOAOMBKFMPF LFLGCDNKNJI)
	{
		foreach (RandomRule item in _randomRules)
		{
			if (item.HHHPGLLBPMF() && item.EPMBMBMNJIA() == LFLGCDNKNJI)
			{
				item.OIOJKNKDFJM();
			}
		}
	}

	protected void PutRandomRules()
	{
		foreach (RandomRule item in _randomRules)
		{
			if (item.HHHPGLLBPMF())
			{
				PutRule(item.GHLEKCGJAEP());
			}
		}
	}

	protected void PutRule(Rule HNBFMAKFJAM, bool HLEIILHFBKP = false)
	{
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleRandom:
			_randomRules.Add((RandomRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleComplex:
			PutComplexRule((ComplexRule)HNBFMAKFJAM);
			return;
		case Rule.BCBLLMPAMLP.RuleItem:
		case Rule.BCBLLMPAMLP.RuleEquipItem:
		case Rule.BCBLLMPAMLP.RuleRandomAquiredItem:
			PutItemRule((ItemRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleNoButton:
			_noButtonRules.Add((NoButtonRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleNoAnimation:
			_noAnimationRules.Add((NoAnimationRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleChangeFight:
			_changeFightRules.Add((ChangeFightRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleRingout:
		case Rule.BCBLLMPAMLP.RuleHotGround:
		case Rule.BCBLLMPAMLP.RuleLoseFall:
		case Rule.BCBLLMPAMLP.RuleRegeneration:
		case Rule.BCBLLMPAMLP.RuleAttributes:
		case Rule.BCBLLMPAMLP.RuleDamageFactor:
		case Rule.BCBLLMPAMLP.RuleRemoveInterval:
		case Rule.BCBLLMPAMLP.RuleCrazy:
		case Rule.BCBLLMPAMLP.RuleLifeSteal:
		case Rule.BCBLLMPAMLP.RuleNoHealthBar:
		case Rule.BCBLLMPAMLP.RuleCombo:
		case Rule.BCBLLMPAMLP.RuleTimeoutWin:
		case Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound:
		case Rule.BCBLLMPAMLP.RuleNoBulletsReplenishment:
		case Rule.BCBLLMPAMLP.RulePerk:
		case Rule.BCBLLMPAMLP.RuleNoPerks:
		case Rule.BCBLLMPAMLP.RuleWinStyle:
		case Rule.BCBLLMPAMLP.RuleWinCombo:
		case Rule.BCBLLMPAMLP.RuleWinShock:
		case Rule.BCBLLMPAMLP.RuleTactic:
		case Rule.BCBLLMPAMLP.RuleInvulnerability:
		case Rule.BCBLLMPAMLP.RuleResistance:
			PutInFightRule((InFightRule)HNBFMAKFJAM);
			return;
		case Rule.BCBLLMPAMLP.RuleDarkness:
		case Rule.BCBLLMPAMLP.RulePoints:
			PutInFightRule((InFightRule)HNBFMAKFJAM, true);
			return;
		case Rule.BCBLLMPAMLP.RuleDescription:
		case Rule.BCBLLMPAMLP.RuleRatingEvaluation:
		case Rule.BCBLLMPAMLP.RuleCurrencyCost:
		case Rule.BCBLLMPAMLP.RuleRaidCurrencyCost:
			return;
		case Rule.BCBLLMPAMLP.RuleInvertJoystick:
			PutInFightRule((InFightRule)HNBFMAKFJAM, true);
			break;
		case Rule.BCBLLMPAMLP.RuleRandomArea:
			PutInFightRule((InFightRule)HNBFMAKFJAM, true);
			break;
		case Rule.BCBLLMPAMLP.RuleAvatar:
			_avatarRules.Add((AvatarRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleName:
			_nameRules.Add((NameRule)HNBFMAKFJAM);
			break;
		default:
			LLLOJBFMONN.Error("RulesInspector::putRule ERROR - wrong rule type %i", HNBFMAKFJAM.get_Type());
			break;
		}
		if (HNBFMAKFJAM.get_Type() != Rule.BCBLLMPAMLP.RuleRandom)
		{
			_rules.AddIfNotExist(HNBFMAKFJAM);
		}
	}

	protected void PutComplexRule(ComplexRule HNBFMAKFJAM)
	{
		List<Rule> list = HNBFMAKFJAM.BONNMLEJBJH();
		foreach (Rule item in list)
		{
			PutRule(item);
		}
		_rules.AddIfNotExist(HNBFMAKFJAM);
	}

	protected void PutItemRule(ItemRule BICJICMJNMC)
	{
		switch (BICJICMJNMC.EDAKADCHOLE())
		{
		case RuleAppliance.AppliancePlayer:
			_playerItemRules.Add(BICJICMJNMC);
			break;
		case RuleAppliance.ApplianceOpponent:
			_enemyItemRules.Add(BICJICMJNMC);
			break;
		case RuleAppliance.ApplianceAll:
			_playerItemRules.Add(BICJICMJNMC);
			_enemyItemRules.Add(BICJICMJNMC);
			break;
		}
		_itemRules.Add(BICJICMJNMC);
	}

	protected void PutInFightRule(InFightRule MGEAFPEKMMC, bool CNKAMHAILKG = false)
	{
		if (!CNKAMHAILKG && MGEAFPEKMMC.EDAKADCHOLE() == RuleAppliance.ApplianceAll)
		{
			InFightRule aAJIFBJLJOA = MGEAFPEKMMC.Copy();
			InFightRule aAJIFBJLJOA2 = MGEAFPEKMMC.Copy();
			aAJIFBJLJOA.MOEAPHGDNAB(RuleAppliance.AppliancePlayer);
			aAJIFBJLJOA2.MOEAPHGDNAB(RuleAppliance.ApplianceOpponent);
			aAJIFBJLJOA.ParentRule = MGEAFPEKMMC;
			aAJIFBJLJOA2.ParentRule = MGEAFPEKMMC;
			SetInFightRule(aAJIFBJLJOA);
			SetInFightRule(aAJIFBJLJOA2);
		}
		else
		{
			SetInFightRule(MGEAFPEKMMC);
		}
	}

	protected void RemoveInFightRule(InFightRule HNBFMAKFJAM, bool CNKAMHAILKG = false)
	{
		if (!CNKAMHAILKG && HNBFMAKFJAM.EDAKADCHOLE() == RuleAppliance.ApplianceAll)
		{
			int num = 0;
			while (num < _inFightRules.Count)
			{
				InFightRule aAJIFBJLJOA = _inFightRules[num];
				if (aAJIFBJLJOA.ParentRule == HNBFMAKFJAM)
				{
					_inFightRules.Remove(aAJIFBJLJOA);
					DeactivateInFightRule(aAJIFBJLJOA);
				}
				else
				{
					num++;
				}
			}
			return;
		}
		for (int i = 0; i < _inFightRules.Count; i++)
		{
			InFightRule aAJIFBJLJOA2 = _inFightRules[i];
			if (HNBFMAKFJAM == aAJIFBJLJOA2)
			{
				_inFightRules.Remove(aAJIFBJLJOA2);
				DeactivateInFightRule(aAJIFBJLJOA2);
				break;
			}
		}
	}

	protected void SetRandomRuleSeed(int OKGKLCLEDFN)
	{
		_randomRuleSeed = OKGKLCLEDFN;
	}

	protected void SetHasRandomSeed(bool value)
	{
		_hasRandomSeed = value;
	}
}
