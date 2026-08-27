using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class QualityCondition
{
	private readonly List<ComparisonExpression> PCLCNLJDPOK = new List<ComparisonExpression>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		private set
		{
			set_Name(value);
		}
	}

	public QualityCondition(XmlNode node)
	{
		if (node.Attributes != null)
		{
			set_Name(node.Attributes["Name"].Value);
		}
		for (int i = 0; i < node.ChildNodes.Count; i++)
		{
			PCLCNLJDPOK.Add(new ComparisonExpression(node.ChildNodes[i]));
		}
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public bool CEHMBJOALEM()
	{
		for (int i = 0; i < PCLCNLJDPOK.Count; i++)
		{
			if (!PCLCNLJDPOK[i].Compare())
			{
				return false;
			}
		}
		return true;
	}
}
