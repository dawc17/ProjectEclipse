using System.Xml;
using UnityEngine;

public class QuestActionOpenUrl : QuestAction
{
	private string BEPKJNKCKPH = string.Empty;

	private string CJGCIONCAGD = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		BEPKJNKCKPH = EPKLCPOEELO.Attributes["URL"].CIPOICEEIBK(string.Empty);
		CJGCIONCAGD = EPKLCPOEELO.Attributes["ALT_URL"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		bool flag = false;
		if (BEPKJNKCKPH != string.Empty)
		{
			ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
			QuestCondition kKDGLNECFHA = new QuestCondition();
			kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
			kKDGLNECFHA.MCPIOGALBMK(BEPKJNKCKPH, lNIDLHOIHIM);
			string text = lNIDLHOIHIM.ToString();
			if (text != string.Empty)
			{
				Application.OpenURL(text);
			}
		}
		if (flag && CJGCIONCAGD != string.Empty)
		{
			ConditionExtension.CompareResult lNIDLHOIHIM2 = new ConditionExtension.CompareResult();
			QuestCondition kKDGLNECFHA2 = new QuestCondition();
			kKDGLNECFHA2.LIMHBJBEEIA(GFIHPBCEEOB);
			kKDGLNECFHA2.MCPIOGALBMK(CJGCIONCAGD, lNIDLHOIHIM2);
			string text2 = lNIDLHOIHIM2.ToString();
			if (text2 != string.Empty)
			{
				Application.OpenURL(text2);
			}
		}
		OGIJONMKABB();
	}
}
