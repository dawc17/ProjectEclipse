using System.Xml;

public class EventParser
{
	public static EventAnimation Create(XmlNode node)
	{
		EventAnimation result = null;
		string name = node.Name;
		switch (name)
		{
		case "RoundStageStart":
			result = new EventRoundStage();
			break;
		case "KeyPressed":
			result = new EventKeyPressed();
			break;
		case "KeyReleased":
			result = new EventKeyReleased();
			break;
		case "AnimationEnd":
			result = new EventAnimationEnd();
			break;
		case "AnimationStart":
			result = new EventAnimationStart();
			break;
		case "IntervalEnd":
			result = new EventIntervalEnd();
			break;
		case "IntervalStart":
			result = new EventIntervalStart();
			break;
		case "Hit":
			result = new EventHit();
			break;
		case "Strike":
			result = new EventAnimation(EventAnimation.EECEJKADLCK.EVENT_STRIKE);
			break;
		case "EveryFrame":
			result = new EventAnimation(EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME);
			break;
		case "Birth":
			result = new EventAnimation(EventAnimation.EECEJKADLCK.EVENT_BIRTH);
			break;
		case "ModExpires":
			result = new EventModExpires();
			break;
		default:
			LLLOJBFMONN.Error("MovesParser::eventsParse - unknown event \"{0}\"", name);
			break;
		}
		return result;
	}
}
