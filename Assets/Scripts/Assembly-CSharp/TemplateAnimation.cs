using System.Collections.Generic;
using System.Xml;

public class TemplateAnimation
{
	private string _Name;

	private List<InfoAnimation> LKBADGFHJHK = new List<InfoAnimation>();

	public List<InfoAnimation> AAHADKFKDPN
	{
		get
		{
			return LDEBJOPLCKO();
		}
	}

	public TemplateAnimation(XmlNode node)
	{
		_Name = XmlUtils.ParseString(node.Attributes["Name"]);
	}

	public TemplateAnimation(InfoAnimation EDMCLHEOJGD)
	{
		_Name = EDMCLHEOJGD.Name;
		MBJCDIDIBDJ(EDMCLHEOJGD);
	}

	public string get_Name()
	{
		return _Name;
	}

	public List<InfoAnimation> LDEBJOPLCKO()
	{
		return LKBADGFHJHK;
	}

	public void MBJCDIDIBDJ(InfoAnimation DBOLBEOCEME)
	{
		LKBADGFHJHK.AddIfNotExist(DBOLBEOCEME);
		DBOLBEOCEME.AddTemplateName(_Name);
	}
}
