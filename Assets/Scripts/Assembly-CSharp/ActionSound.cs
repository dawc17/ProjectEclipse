using System.Xml;

public class ActionSound : ActionAnimation
{
	private string _Name;

	private bool _AnyGender;

	private string _Gender;

	private bool IMOEOKFPPMJ;

	private float _Volume;

	public bool BEMCJINPJKA
	{
		get
		{
			return DBIOMDEIIKI();
		}
	}

	public float FLJPEPPDICN
	{
		get
		{
			return AFKMLMCCJLI();
		}
	}

	public ActionSound(XmlNode node)
		: base(FADAJCEEKIO.SOUND)
	{
		Parse(node);
	}

	public string get_Name()
	{
		return _Name;
	}

	public bool DBIOMDEIIKI()
	{
		return IMOEOKFPPMJ;
	}

	public float AFKMLMCCJLI()
	{
		return _Volume;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	public bool SameGender(string EMENABICHED)
	{
		return _AnyGender || EMENABICHED == _Gender;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_Volume = node.Attributes["Volume"].ParseFloat(1f);
		IMOEOKFPPMJ = node.Attributes["Looped"].ParseBool();
		XmlAttribute xmlAttribute = node.Attributes["Voice"];
		_AnyGender = xmlAttribute == null;
		_Gender = xmlAttribute.CIPOICEEIBK(string.Empty);
	}
}
