using System;
using System.Xml;

public class Aspect
{
	private string _name = string.Empty;

	private string DHLNMJCGJMO = string.Empty;

	private float _base;

	public string Attribute
	{
		get
		{
			return EJPCHOLGGJJ();
		}
	}

	public string get_Name()
	{
		return _name;
	}

	public string EJPCHOLGGJJ()
	{
		return DHLNMJCGJMO;
	}

	public void Parse(XmlNode node)
	{
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		DHLNMJCGJMO = node.Attributes["Attribute"].CIPOICEEIBK(string.Empty);
		_base = node.Attributes["Base"].ParseFloat();
	}

	public int GetValue(double OGGIEHGBGPB, int GNLOCMLBNHF, int BIJKNKAJBHH, double NCKIPNEDHJN)
	{
		double num = (OGGIEHGBGPB - (double)(GNLOCMLBNHF * BIJKNKAJBHH)) / NCKIPNEDHJN;
		double num2 = ((!(num < 0.0)) ? ((double)_base * (2.0 - Math.Pow(2.0, 0.0 - num))) : ((double)_base * Math.Pow(2.0, num)));
		return (int)num2;
	}
}
