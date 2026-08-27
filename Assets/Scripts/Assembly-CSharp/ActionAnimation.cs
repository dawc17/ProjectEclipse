using System.Collections.Generic;
using System.Xml;

public class ActionAnimation
{
	public enum FADAJCEEKIO
	{
		CREATE_MODEL = 0,
		DELETE = 1,
		SOUND = 2,
		STOP_SOUND = 3,
		RANDOM_SOUND = 4,
		EFFECT = 5,
		STOP_EFFECT = 6,
		STOP_FOLLOW_EFFECT = 7,
		ADD_BULLETS = 8,
		SHAKE_SCREEN = 9,
		HIT_EFFECT = 10,
		ZOOM_EFFECT = 11,
		SET_COOLDOWN = 12,
		SET_END_STAGE = 13,
		PLAY_ANIMATION = 14,
		MOD_FLAG = 15
	}

	public enum JKEBPJCEEKM
	{
		START_FRAME = 0,
		START_EVENT = 1
	}

	public class ActionStartParameters
	{
		public JKEBPJCEEKM CGEPLPNFABA;

		public int Frame;

		public EventAnimation.EECEJKADLCK MOFKKABEFEB;
	}

	private FADAJCEEKIO KCIIELDOBOM;

	private ActionStartParameters GANELHAJFAO = new ActionStartParameters();

	private Model _Model;

	private ModelType.KEIDBIOIFGA OOFFOILONLO;

	// Recent move data can select between several effects/sounds on the same
	// frame by attaching a Conditions block to the individual action.  The
	// original decompilation only parsed conditions belonging to a Move, which
	// caused every variant (acid + frost clouds, all three auras, etc.) to run.
	private readonly List<ConditionAnimation> _Conditions = new List<ConditionAnimation>();

	public Model KJDFJPBIGJC
	{
		get
		{
			return get_Model();
		}
		set
		{
			set_Model(value);
		}
	}

	public ModelType.KEIDBIOIFGA EFNJGJLEPNK
	{
		get
		{
			return OJLDHGKPLNC();
		}
	}

	public ActionAnimation(FADAJCEEKIO LFLGCDNKNJI)
	{
		KCIIELDOBOM = LFLGCDNKNJI;
		_Model = null;
	}

	public FADAJCEEKIO get_Type()
	{
		return KCIIELDOBOM;
	}

	public Model get_Model()
	{
		return _Model;
	}

	public void set_Model(Model value)
	{
		_Model = value;
	}

	public ModelType.KEIDBIOIFGA OJLDHGKPLNC()
	{
		return OOFFOILONLO;
	}

	public bool NeedStart(int frame)
	{
		return GANELHAJFAO.CGEPLPNFABA == JKEBPJCEEKM.START_FRAME && GANELHAJFAO.Frame == frame;
	}

	public bool NeedStart(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
	{
		return GANELHAJFAO.CGEPLPNFABA == JKEBPJCEEKM.START_EVENT && GANELHAJFAO.MOFKKABEFEB == LFLGCDNKNJI;
	}

	public bool CanVisit(Model model)
	{
		if (model == null)
			return false;
		ModelConditions modelConditions = model.EBABHGHPLFK();
		if (modelConditions == null)
			return false;
		foreach (ConditionAnimation condition in _Conditions)
		{
			bool matches;
			if (condition.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList list = condition as ConditionList;
				matches = list != null && list.DJEJMGCMPPH(modelConditions, model, null);
			}
			else
			{
				Model target = condition.DKDAKGDMHAL(model, condition.FHBAPKNECOM());
				ModelConditions targetConditions = (target != null) ? target.EBABHGHPLFK() : null;
				matches = targetConditions != null && condition.IsEqual(targetConditions);
			}
			if (!matches)
				return false;
		}
		return true;
	}

	public int GetConditionCount()
	{
		return _Conditions.Count;
	}

	public virtual void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected virtual void Parse(XmlNode node)
	{
		XmlAttribute xmlAttribute = node.Attributes["Frame"];
		if (xmlAttribute != null)
		{
			GANELHAJFAO.CGEPLPNFABA = JKEBPJCEEKM.START_FRAME;
			GANELHAJFAO.Frame = xmlAttribute.ParseInt();
		}
		else
		{
			GANELHAJFAO.CGEPLPNFABA = JKEBPJCEEKM.START_EVENT;
			string gOHIIMFFFJI = node.Attributes["Event"].CIPOICEEIBK(string.Empty);
			GANELHAJFAO.MOFKKABEFEB = EventAnimation.IOPCBLBFLKB(gOHIIMFFFJI);
		}
		OOFFOILONLO = ModelType.EHFNOBFLAHI(node.Attributes["Player"].CIPOICEEIBK("Me"));
		XmlNode conditions = node["Conditions"];
		if (conditions != null)
			ConditionsParser.ParseInside(_Conditions, conditions);
	}
}
