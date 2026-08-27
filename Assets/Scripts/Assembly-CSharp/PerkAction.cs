using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkAction : PerkObject
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string IHIPCGBIEDI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HCHALPNMNMK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ActionType KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool IJEKABFBKLF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PerkTrigger MIDNGGPKKFL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension PJKLJNAGNCJ;

	public string JPHFMOHOPHM
	{
		get
		{
			return FDEKGNPKJFL();
		}
		protected set
		{
			IHMGKCPKCDD(value);
		}
	}

	public bool FEAHANGDAMK
	{
		get
		{
			return NKAEEFNNBEN();
		}
		protected set
		{
			set_Modificator(value);
		}
	}

	public PerkTrigger KOMFMPAHNCO
	{
		get
		{
			return GNDAFILBLIB();
		}
		protected set
		{
			CONNEMFGHMM(value);
		}
	}

	public FunctionExtension OCFKLCDIEBF
	{
		get
		{
			return BFJEFNHKPJI();
		}
		protected set
		{
			set_Frames(value);
		}
	}

	public PerkAction()
	{
	}

	public PerkAction(PerkAction NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Name(NOLFMPDGCOC.get_Name());
		IHMGKCPKCDD(NOLFMPDGCOC.FDEKGNPKJFL());
		set_Namespace(NOLFMPDGCOC.IONIEDIPEGB());
		set_Type(NOLFMPDGCOC.get_Type());
		set_Modificator(NOLFMPDGCOC.NKAEEFNNBEN());
		CONNEMFGHMM(NOLFMPDGCOC.GNDAFILBLIB());
		set_Frames(NOLFMPDGCOC.BFJEFNHKPJI());
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	protected void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public string FDEKGNPKJFL()
	{
		return IHIPCGBIEDI;
	}

	protected void IHMGKCPKCDD(string value)
	{
		IHIPCGBIEDI = value;
	}

	public string IONIEDIPEGB()
	{
		return HCHALPNMNMK;
	}

	protected void set_Namespace(string value)
	{
		HCHALPNMNMK = value;
	}

	public ActionType get_Type()
	{
		return KAHHEBMBCFA;
	}

	protected void set_Type(ActionType value)
	{
		KAHHEBMBCFA = value;
	}

	public bool NKAEEFNNBEN()
	{
		return IJEKABFBKLF;
	}

	protected void set_Modificator(bool value)
	{
		IJEKABFBKLF = value;
	}

	public PerkTrigger GNDAFILBLIB()
	{
		return MIDNGGPKKFL;
	}

	protected void CONNEMFGHMM(PerkTrigger value)
	{
		MIDNGGPKKFL = value;
	}

	public FunctionExtension BFJEFNHKPJI()
	{
		return PJKLJNAGNCJ;
	}

	protected void set_Frames(FunctionExtension value)
	{
		PJKLJNAGNCJ = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		IHMGKCPKCDD(node.Name);
		set_Name(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		string text = node.Attributes["Frames"].CIPOICEEIBK(string.Empty);
		if (text != null && !text.Equals(string.Empty))
		{
			set_Frames(new FunctionExtension());
			BFJEFNHKPJI().Parse(text);
			BFJEFNHKPJI().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			BFJEFNHKPJI().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			BFJEFNHKPJI().set_Target(this);
		}
		set_Namespace(node.Attributes["Namespace"].CIPOICEEIBK(string.Empty));
	}

	public Model NKLMKGFAGFG(Model ACENLMONNPA)
	{
		if (IHJJBIDMEMB == PlayerType.PLAYER_ME)
		{
			return ACENLMONNPA;
		}
		if (IHJJBIDMEMB == PlayerType.PLAYER_ENEMY)
		{
			return ACENLMONNPA.EGGEACCDAEK();
		}
		return null;
	}

	public static List<PerkAction> Create(XmlNode node, PerkInfoItem AEFFHJGMNFI, PerkTrigger CPBHKJFPFJB)
	{
		List<PerkAction> list = new List<PerkAction>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkAction nPEKHPDCPPO = null;
			switch (childNode.Name)
			{
			case "ModIcon":
				nPEKHPDCPPO = new PerkActionShowIcon();
				break;
			case "ModAttributes":
				nPEKHPDCPPO = new PerkActionSetAttributes();
				break;
			case "ModFlag":
				nPEKHPDCPPO = new PerkActionFlag();
				break;
			case "ClearMods":
				nPEKHPDCPPO = new PerkActionClearAction();
				break;
			case "DisableInterval":
				nPEKHPDCPPO = new PerkActionDisableInterval();
				break;
			case "SetHit":
				nPEKHPDCPPO = new PerkActionSetHit();
				break;
			case "AddBullets":
				nPEKHPDCPPO = new PerkActionAddBullets();
				break;
			case "AddMagicCharge":
				nPEKHPDCPPO = new PerkActionAddMagicCharge();
				break;
			case "SetModFrames":
				nPEKHPDCPPO = new PerkActionSetModFrames();
				break;
			case "ApplyModEffect":
				nPEKHPDCPPO = new PerkActionSetModEffect();
				break;
			case "ModHealthChange":
				nPEKHPDCPPO = new ModHealthChange();
				break;
			case "Provoke":
				nPEKHPDCPPO = new PerkActionProvoke();
				break;
			case "SetTactic":
				nPEKHPDCPPO = new PerkActionSetTactics();
				break;
			case "Lifesteal":
				nPEKHPDCPPO = new PerkActionLifesteal();
				break;
			case "ModInvisibility":
				nPEKHPDCPPO = new ModInvisibility();
				break;
			case "ModVariable":
				nPEKHPDCPPO = new PerkActionVariable();
				break;
			case "SetVariable":
			case "SetRangeVariable":
				nPEKHPDCPPO = new PerkActionSetVariable();
				break;
			case "SetModVariable":
				nPEKHPDCPPO = new PerkActionVariable();
				break;
			case "SetCooldown":
				nPEKHPDCPPO = new PerkActionSetCooldown();
				break;
			case "ChangeImpulse":
				nPEKHPDCPPO = new PerkActionChangeImpulse();
				break;
			case "ChangeHitEffectScale":
				nPEKHPDCPPO = new PerkActionChangeHitEffectScale();
				break;
			case "ChangeAdditionalDamageValue":
				nPEKHPDCPPO = new PerkActionChangeAdditionalDamageValue();
				break;
			case "ChangeModelColor":
				nPEKHPDCPPO = new PerkActionChangeModelColor();
				break;
			case "SlowModel":
				nPEKHPDCPPO = new PerkActionSlowModel();
				break;
			case "TurnOffCollision":
				nPEKHPDCPPO = new PerkActionTurnOffCollision();
				break;
			case "Switch":
				nPEKHPDCPPO = new PerkActionSwitch();
				break;
			case "MarkPerkAsUsed":
				nPEKHPDCPPO = new PerkActionMarkUsed();
				break;
			case "PerkArea":
				nPEKHPDCPPO = new PerkActionArea();
				break;
			case "MoveModel":
				nPEKHPDCPPO = new PerkActionMoveModel();
				break;
			case "SetMovesVariable":
				nPEKHPDCPPO = new PerkActionSetMovesVariable();
				break;
			case "StealMagicMod":
				nPEKHPDCPPO = new PerkActionStealMagic();
				break;
			}
			if (nPEKHPDCPPO != null)
			{
				nPEKHPDCPPO.JMOIMIHPBOM(AEFFHJGMNFI);
				nPEKHPDCPPO.CONNEMFGHMM(CPBHKJFPFJB);
				nPEKHPDCPPO.Parse(childNode);
				list.Add(nPEKHPDCPPO);
			}
		}
		return list;
	}

	public static PerkAction Clone(PerkAction IBODMPMJELJ, PerkInfoItem AEFFHJGMNFI, PerkTrigger CPBHKJFPFJB)
	{
		PerkAction nPEKHPDCPPO = null;
		switch (IBODMPMJELJ.get_Type())
		{
		case ActionType.ACTION_SHOW_ICONS:
			nPEKHPDCPPO = new PerkActionShowIcon((PerkActionShowIcon)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_ATTRIBUTES:
			nPEKHPDCPPO = new PerkActionSetAttributes((PerkActionSetAttributes)IBODMPMJELJ);
			break;
		case ActionType.ACTION_FLAG:
			nPEKHPDCPPO = new PerkActionFlag((PerkActionFlag)IBODMPMJELJ);
			break;
		case ActionType.ACTION_CLEAR_ACTION:
			nPEKHPDCPPO = new PerkActionClearAction((PerkActionClearAction)IBODMPMJELJ);
			break;
		case ActionType.ACTION_DISABLE_INTERVAL:
			nPEKHPDCPPO = new PerkActionDisableInterval((PerkActionDisableInterval)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_HIT:
			nPEKHPDCPPO = new PerkActionSetHit((PerkActionSetHit)IBODMPMJELJ);
			break;
		case ActionType.ACTION_ADD_BULLETS:
			nPEKHPDCPPO = new PerkActionAddBullets((PerkActionAddBullets)IBODMPMJELJ);
			break;
		case ActionType.ACTION_ADD_MAGIC:
			nPEKHPDCPPO = new PerkActionAddMagicCharge((PerkActionAddMagicCharge)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_MOD_FRAMES:
			nPEKHPDCPPO = new PerkActionSetModFrames((PerkActionSetModFrames)IBODMPMJELJ);
			break;
		case ActionType.ACTION_MOD_EFFECT:
			nPEKHPDCPPO = new PerkActionSetModEffect((PerkActionSetModEffect)IBODMPMJELJ);
			break;
		case ActionType.ACTION_MOD_HEALTH_CHANGE:
			nPEKHPDCPPO = new ModHealthChange((ModHealthChange)IBODMPMJELJ);
			break;
		case ActionType.ACTION_PROVOKE:
			nPEKHPDCPPO = new PerkActionProvoke((PerkActionProvoke)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_TACTICS:
			nPEKHPDCPPO = new PerkActionSetTactics((PerkActionSetTactics)IBODMPMJELJ);
			break;
		case ActionType.ACTION_LIFE_STEAL:
			nPEKHPDCPPO = new PerkActionLifesteal((PerkActionLifesteal)IBODMPMJELJ);
			break;
		case ActionType.ACTION_INVISIBILITY:
			nPEKHPDCPPO = new ModInvisibility((ModInvisibility)IBODMPMJELJ);
			break;
		case ActionType.ACTION_VARIABLE:
			nPEKHPDCPPO = new PerkActionVariable((PerkActionVariable)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_VARIABLE:
			nPEKHPDCPPO = new PerkActionSetVariable((PerkActionSetVariable)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_COOLDOWN:
			nPEKHPDCPPO = new PerkActionSetCooldown((PerkActionSetCooldown)IBODMPMJELJ);
			break;
		case ActionType.ACTION_CHANGE_IMPULSE:
			nPEKHPDCPPO = new PerkActionChangeImpulse((PerkActionChangeImpulse)IBODMPMJELJ);
			break;
		case ActionType.ACTION_CHANGE_HIT_EFFECT_SCALE:
			nPEKHPDCPPO = new PerkActionChangeHitEffectScale((PerkActionChangeHitEffectScale)IBODMPMJELJ);
			break;
		case ActionType.ACTION_CHANGE_ADD_DAMAGE_VALUE:
			nPEKHPDCPPO = new PerkActionChangeAdditionalDamageValue((PerkActionChangeAdditionalDamageValue)IBODMPMJELJ);
			break;
		case ActionType.ACTION_CHANGE_MODEL_COLOR:
			nPEKHPDCPPO = new PerkActionChangeModelColor((PerkActionChangeModelColor)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SLOW_MODEL:
			nPEKHPDCPPO = new PerkActionSlowModel((PerkActionSlowModel)IBODMPMJELJ);
			break;
		case ActionType.ACTION_TURN_OFF_COLLISION:
			nPEKHPDCPPO = new PerkActionTurnOffCollision((PerkActionTurnOffCollision)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SWITCH:
			nPEKHPDCPPO = new PerkActionSwitch((PerkActionSwitch)IBODMPMJELJ);
			break;
		case ActionType.ACTION_MARK_PERK_USED:
			nPEKHPDCPPO = new PerkActionMarkUsed((PerkActionMarkUsed)IBODMPMJELJ);
			break;
		case ActionType.ACTION_PERK_AREA:
			nPEKHPDCPPO = new PerkActionArea((PerkActionArea)IBODMPMJELJ);
			break;
		case ActionType.ACTION_MOVE_MODEL:
			nPEKHPDCPPO = new PerkActionMoveModel((PerkActionMoveModel)IBODMPMJELJ);
			break;
		case ActionType.ACTION_SET_MOVES_VARIABLE:
			nPEKHPDCPPO = new PerkActionSetMovesVariable((PerkActionSetMovesVariable)IBODMPMJELJ);
			break;
		case ActionType.ACTION_STEAL_MAGIC:
			nPEKHPDCPPO = new PerkActionStealMagic((PerkActionStealMagic)IBODMPMJELJ);
			break;
		default:
			LLLOJBFMONN.Error("PerkAction.Clone PerkAction type is ActionType.ACTION_NONE");
			break;
		}
		if (nPEKHPDCPPO != null)
		{
			nPEKHPDCPPO.JMOIMIHPBOM(AEFFHJGMNFI);
			nPEKHPDCPPO.CONNEMFGHMM(CPBHKJFPFJB);
		}
		return nPEKHPDCPPO;
	}
}

public class PerkActionChangeModelColor : PerkActionModificator
{
	public UnityEngine.Color Color = UnityEngine.Color.white;

	public PerkActionChangeModelColor() { }
	public PerkActionChangeModelColor(PerkActionChangeModelColor source) : base(source) { Color = source.Color; }

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_CHANGE_MODEL_COLOR);
		string value = node.Attributes["Color"].CIPOICEEIBK("#FFFFFFFF").TrimStart('#');
		uint packed;
		if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber,
			System.Globalization.CultureInfo.InvariantCulture, out packed))
		{
			if (value.Length <= 6)
				packed = (packed << 8) | 255u;
			Color = new UnityEngine.Color32((byte)(packed >> 24), (byte)(packed >> 16),
				(byte)(packed >> 8), (byte)packed);
		}
	}
}

public class PerkActionSlowModel : PerkActionModificator
{
	public int Speed = 1;
	public bool IsRulePerk;

	public PerkActionSlowModel() { }
	public PerkActionSlowModel(PerkActionSlowModel source) : base(source)
	{
		Speed = source.Speed;
		IsRulePerk = source.IsRulePerk;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SLOW_MODEL);
		Speed = System.Math.Max(1, node.Attributes["Speed"].ParseInt(1));
		IsRulePerk = node.Attributes["IsRulePerk"].ParseBool();
	}
}

public class PerkActionTurnOffCollision : PerkActionModificator
{
	public PerkActionTurnOffCollision() { }
	public PerkActionTurnOffCollision(PerkActionTurnOffCollision source) : base(source) { }
	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_TURN_OFF_COLLISION);
	}
}

public class PerkActionMarkUsed : PerkAction
{
	public PerkActionMarkUsed() { }
	public PerkActionMarkUsed(PerkActionMarkUsed source) : base(source) { }
	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_MARK_PERK_USED);
	}
}

public class PerkActionSwitch : PerkAction
{
	public class Branch
	{
		public FunctionExtension Value;
		public List<PerkAction> Actions;
	}

	public FunctionExtension Value;
	public readonly List<Branch> Cases = new List<Branch>();
	public List<PerkAction> DefaultActions = new List<PerkAction>();

	public PerkActionSwitch() { }
	public PerkActionSwitch(PerkActionSwitch source) : base(source)
	{
		Value = source.Value;
		Cases.AddRange(source.Cases);
		DefaultActions.AddRange(source.DefaultActions);
	}

	private FunctionExtension ParseFunction(string expression)
	{
		FunctionExtension function = new FunctionExtension();
		function.Parse(expression);
		function.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
		function.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
		function.set_Target(this);
		return function;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SWITCH);
		Value = ParseFunction(node.Attributes["Value"].CIPOICEEIBK("0"));
		foreach (XmlNode branchNode in node.ChildNodes)
		{
			if (branchNode.Name == "Case")
			{
				Branch branch = new Branch();
				branch.Value = ParseFunction(branchNode.Attributes["Value"].CIPOICEEIBK("0"));
				branch.Actions = Create(branchNode, JMDLAMHAJLN(), GNDAFILBLIB());
				Cases.Add(branch);
			}
			else if (branchNode.Name == "Default")
			{
				DefaultActions = Create(branchNode, JMDLAMHAJLN(), GNDAFILBLIB());
			}
		}
	}

	public List<PerkAction> SelectActions()
	{
		string actual = Value.IBCPKBBAFNH().DCJLKCFKCOM;
		foreach (Branch branch in Cases)
		{
			string expected = branch.Value.IBCPKBBAFNH().DCJLKCFKCOM;
			float actualNumber;
			float expectedNumber;
			if ((float.TryParse(actual, out actualNumber) && float.TryParse(expected, out expectedNumber) &&
				System.Math.Abs(actualNumber - expectedNumber) < 0.0001f) || actual == expected)
				return branch.Actions;
		}
		return DefaultActions;
	}
}

public class PerkActionArea : PerkActionModificator
{
	public float Width;
	public string FileName = string.Empty;
	public float ShiftY;
	public FunctionExtension PositionX;

	public PerkActionArea() { }
	public PerkActionArea(PerkActionArea source) : base(source)
	{
		Width = source.Width; FileName = source.FileName; ShiftY = source.ShiftY; PositionX = source.PositionX;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_PERK_AREA);
		Width = node.Attributes["Width"].ParseFloat(400f);
		FileName = node.Attributes["FileName"].CIPOICEEIBK(string.Empty);
		ShiftY = node.Attributes["ShiftY"].ParseFloat();
		PositionX = new FunctionExtension();
		PositionX.Parse(node.Attributes["PositionX"].CIPOICEEIBK("0"));
		PositionX.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
		PositionX.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
		PositionX.set_Target(this);
	}
}

public class PerkActionMoveModel : PerkAction
{
	public FunctionExtension OffsetX;
	public PerkActionMoveModel() { }
	public PerkActionMoveModel(PerkActionMoveModel source) : base(source) { OffsetX = source.OffsetX; }
	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_MOVE_MODEL);
		OffsetX = new FunctionExtension();
		OffsetX.Parse(node.Attributes["PositionOffsetX"].CIPOICEEIBK("0"));
		OffsetX.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
		OffsetX.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
		OffsetX.set_Target(this);
	}
}

public class PerkActionSetMovesVariable : PerkAction
{
	public FunctionExtension Value;
	public PerkActionSetMovesVariable() { }
	public PerkActionSetMovesVariable(PerkActionSetMovesVariable source) : base(source) { Value = source.Value; }
	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Name(node.Attributes["Variable"].CIPOICEEIBK(string.Empty));
		set_Type(ActionType.ACTION_SET_MOVES_VARIABLE);
		Value = new FunctionExtension();
		Value.Parse(node.Attributes["Value"].CIPOICEEIBK("0"));
		Value.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
		Value.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
		Value.set_Target(this);
	}
}

public class PerkActionStealMagic : PerkActionModificator
{
	public FunctionExtension MagicName;
	public PerkActionStealMagic() { }
	public PerkActionStealMagic(PerkActionStealMagic source) : base(source) { MagicName = source.MagicName; }
	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_STEAL_MAGIC);
		MagicName = new FunctionExtension();
		MagicName.Parse(node.Attributes["MagicName"].CIPOICEEIBK(string.Empty));
		MagicName.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
		MagicName.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
		MagicName.set_Target(this);
	}
}
