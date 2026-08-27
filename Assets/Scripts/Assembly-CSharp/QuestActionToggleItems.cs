using System.Collections.Generic;
using System.Xml;

public class QuestActionToggleItems : QuestAction
{
	private string _toggle;

	private string IIPJNGBMJJP;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_toggle = EPKLCPOEELO.Attributes["Toggle"].CIPOICEEIBK("on");
		IIPJNGBMJJP = EPKLCPOEELO.Attributes["Label"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(IIPJNGBMJJP, lNIDLHOIHIM);
		string iBBAMMHHBFE = lNIDLHOIHIM.resultSTR;
		kKDGLNECFHA.MCPIOGALBMK(_toggle, lNIDLHOIHIM);
		bool flag = lNIDLHOIHIM.resultSTR == "on";
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (flag)
		{
			if (nKGLHEGIKKP.AddShopLock(iBBAMMHHBFE, true))
			{
				LNGDHCMJOMH(iBBAMMHHBFE, true);
			}
		}
		else if (nKGLHEGIKKP.OAHDKIDMOCG(iBBAMMHHBFE))
		{
			LNGDHCMJOMH(iBBAMMHHBFE, false);
		}
		OGIJONMKABB();
	}

	private void LNGDHCMJOMH(string ECNLPLIBNHF, bool PEJELKNFEKJ)
	{
		if (PEJELKNFEKJ)
		{
			List<ItemInfo> list = ListSF.DJBOFEEKJMP().HCDLKHKBEPF();
			int num = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			{
				foreach (ItemInfo item in list)
				{
					if (item.DCHJDPCEODD && item.MMHIKEIDDNB == ECNLPLIBNHF)
					{
						ListSF.DJBOFEEKJMP().SetNewAddItem(item, true, (!(item.Type == "RealMoneyItem")) ? num : item.MHGODOLNDLE);
					}
				}
				return;
			}
		}
		ListSF.DJBOFEEKJMP().MJICEAIDCGP(ECNLPLIBNHF);
	}
}
