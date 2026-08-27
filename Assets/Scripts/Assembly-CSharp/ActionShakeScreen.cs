using System.Xml;

public class ActionShakeScreen : ActionAnimation
{
	private GameUtils.HitEffect PMAEMMNLJJL = new GameUtils.HitEffect();

	public GameUtils.HitEffect NFOANIDAFGD
	{
		get
		{
			return CBNIELBJDAO();
		}
	}

	public ActionShakeScreen(XmlNode node)
		: base(FADAJCEEKIO.SHAKE_SCREEN)
	{
		Parse(node);
	}

	public GameUtils.HitEffect CBNIELBJDAO()
	{
		return PMAEMMNLJJL;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		PMAEMMNLJJL.NHKPODHHDPF = node.Attributes["PauseTime"].ParseInt();
		PMAEMMNLJJL.OFJCKMNLAEP = node.Attributes["EffectTime"].ParseInt();
		PMAEMMNLJJL.FMICELIGLPG = node.Attributes["AmplitudeX"].ParseFloat();
		PMAEMMNLJJL.PPKAMOILNLN = node.Attributes["AmplitudeY"].ParseFloat();
		PMAEMMNLJJL.KFEMKHHANDC = node.Attributes["FrequencyX"].ParseFloat();
		PMAEMMNLJJL.GGJBPLHAHFH = node.Attributes["FrequencyY"].ParseFloat();
	}
}
