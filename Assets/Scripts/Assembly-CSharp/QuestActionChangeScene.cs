using System.Xml;

public class QuestActionChangeScene : QuestAction
{
	private string _Destination = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_Destination = EPKLCPOEELO.Attributes["Destination"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string empty = string.Empty;
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(_Destination, lNIDLHOIHIM);
		empty = lNIDLHOIHIM.ToString();
		Module.ELEBLBJKDBI().AddEventListener(1, DOHEMBEEHBB);
		ScreenType kAHMHPNJBGI = Module.DFDEMKONNKK(empty);
		OCOEIOEDCLE(kAHMHPNJBGI);
	}

	private void DOHEMBEEHBB(object data)
	{
		OGIJONMKABB();
		Module.ELEBLBJKDBI().RemoveEventListener(1, DOHEMBEEHBB);
	}

	private void OCOEIOEDCLE(ScreenType KAHMHPNJBGI)
	{
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		bool flag = false;
		bool flag2 = KAHMHPNJBGI == iPKNDMINFMJ;
		bool flag3 = KAHMHPNJBGI != ScreenType.ModuleFight;
		if (flag2)
		{
			flag = true;
		}
		else if (flag3)
		{
			flag = !Module.DLOKJOHNDID(KAHMHPNJBGI);
		}
		else
		{
			Module.DLOKJOHNDID(ScreenType.ModuleDojo);
		}
		if (flag)
		{
			DOHEMBEEHBB(0);
		}
	}
}
