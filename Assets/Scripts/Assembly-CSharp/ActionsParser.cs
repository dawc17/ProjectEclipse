using System.Xml;

public static class ActionsParser
{
	public static ActionAnimation Create(XmlNode node)
	{
		string name = node.Name;
		ActionAnimation result = null;
		switch (name)
		{
		case "Sound":
			result = new ActionSound(node);
			break;
		case "RandomSound":
			result = new ActionRandomSound(node);
			break;
		case "CreatePlayer":
			result = new ActionCreateModel(node);
			break;
		case "Delete":
			result = new ActionDelete(node);
			break;
		case "Effect":
			result = new ActionEffect(node);
			break;
		case "StopEffect":
			result = new ActionStopEffect(node);
			break;
		case "StopFollowEffect":
			result = new ActionStopFollowEffect(node);
			break;
		case "AddBullets":
			result = new ActionAddBullets(node);
			break;
		case "TryOnEnd":
			result = new ActionTryOnEnd(node);
			break;
		case "ShakeScreen":
			result = new ActionShakeScreen(node);
			break;
		case "HitEffect":
			result = new ActionHitEffect(node);
			break;
		case "StopSound":
			result = new ActionStopSound(node);
			break;
		case "ZoomEffect":
			result = new ActionZoomEffect(node);
			break;
		case "SetCooldown":
			result = new ActionSetCooldown(node);
			break;
		case "SetEndStage":
			result = new ActionSetEndStage(node);
			break;
		case "PlayAnimation":
			result = new ActionPlayAnimation(node);
			break;
		case "ModFlag":
			result = new ActionAnimationModFlag(node);
			break;
		case "CameraWeight":
			result = new Eclipse.Content.CameraWeightMoveAction(node);
			break;
		case "EnableBossAbility":
			result = new Eclipse.Content.EnableBossAbilityMoveAction(node);
			break;
		default:
			LLLOJBFMONN.Error("ERROR: ActionsParser::create - no action \"{0}\" found", name);
			break;
		}
		return result;
	}
}

// Compatibility bridge for newer move XML. These one-frame flags are emitted
// by projectile strikes and consumed by perk conditions in the same/next frame.
public class ActionAnimationModFlag : ActionAnimation
{
	public string FlagName { get; private set; }
	public int Frames { get; private set; }

	public ActionAnimationModFlag(XmlNode node) : base(FADAJCEEKIO.MOD_FLAG)
	{
		Parse(node);
		FlagName = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Frames = node.Attributes["Frames"].ParseInt(1);
	}

	public override void Visit(Model model)
	{
		Model target = model.NMGNPBMFJKP(OJLDHGKPLNC());
		if (target == null)
			target = model;
		target.AddTransientPerkFlag(FlagName, Frames);
	}
}

public class ActionSetEndStage : ActionAnimation
{
	public ActionSetEndStage(XmlNode node)
		: base(FADAJCEEKIO.SET_END_STAGE)
	{
		Parse(node);
	}

	public override void Visit(Model model)
	{
		model.OPPIKLBKMPN(this);
	}
}

public class ActionPlayAnimation : ActionAnimation
{
	public string AnimationName { get; private set; }
	public string ChildName { get; private set; }
	public bool ForcePlay { get; private set; }

	public ActionPlayAnimation(XmlNode node)
		: base(FADAJCEEKIO.PLAY_ANIMATION)
	{
		Parse(node);
		AnimationName = node.Attributes["Animation"].CIPOICEEIBK(string.Empty);
		ChildName = node.Attributes["ChildName"].CIPOICEEIBK(string.Empty);
		ForcePlay = node.Attributes["ForcePlay"].ParseBool();
	}

	public override void Visit(Model model)
	{
		model.OPPIKLBKMPN(this);
	}
}
