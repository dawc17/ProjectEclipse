using System.Xml;

public class ItemSetItem
{
	public string Name;

	public float FOAHMAOBFEA;

	public float DNGPAHCJFOK;

	public float NPKMJMCLDAH;

	public float IHAHIEHHNCG;

	public float IJEAEHOKLAF;

	public ItemInfo OFMCNLBFIDF;

	public ItemSetItem(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		FOAHMAOBFEA = node.Attributes["Scale"].ParseFloat();
		DNGPAHCJFOK = node.Attributes["Rotate"].ParseFloat();
		NPKMJMCLDAH = node.Attributes["X"].ParseFloat();
		IHAHIEHHNCG = node.Attributes["Y"].ParseFloat();
		IJEAEHOKLAF = node.Attributes["IconsY"].ParseFloat();
		OFMCNLBFIDF = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(Name);
	}
}
