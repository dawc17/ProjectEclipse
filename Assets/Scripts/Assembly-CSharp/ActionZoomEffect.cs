using System.Xml;

public class ActionZoomEffect : ActionAnimation
{
	private GameUtils.ZoomEffect BBJLALGOMLM = new GameUtils.ZoomEffect();

	public GameUtils.ZoomEffect KIBLEHOADMI
	{
		get
		{
			return DJDCBEMKLIP();
		}
	}

	public ActionZoomEffect(XmlNode node)
		: base(FADAJCEEKIO.ZOOM_EFFECT)
	{
		Parse(node);
	}

	public GameUtils.ZoomEffect DJDCBEMKLIP()
	{
		return BBJLALGOMLM;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		BBJLALGOMLM.OFJCKMNLAEP = node.Attributes["EffectTime"].ParseInt();
		BBJLALGOMLM.JCNPAOMNJCL = node.Attributes["ZoomScale"].ParseFloat();
	}
}
