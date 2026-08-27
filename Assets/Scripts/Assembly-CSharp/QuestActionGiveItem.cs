using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Shop;

public class QuestActionGiveItem : QuestAction
{
	private string ICAGIOIDCBL = string.Empty;

	private string MLIGIDIHBHP = string.Empty;

	private string JKFHICPKOPA = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		MLIGIDIHBHP = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		JKFHICPKOPA = EPKLCPOEELO.Attributes["PutOn"].CIPOICEEIBK(string.Empty);
		ICAGIOIDCBL = EPKLCPOEELO.Attributes["Quantity"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(MLIGIDIHBHP, lNIDLHOIHIM);
		string gOHIIMFFFJI = string.Empty;
		int num = 0;
		ItemInfo dJKEECEOCJB = null;
		ItemInfo dJKEECEOCJB2 = null;
		List<string> list = new List<string>(lNIDLHOIHIM.resultSTR.Split('|'));
		int count = list.Count;
		if (count > 0)
		{
			gOHIIMFFFJI = list[0];
		}
		if (list.Count >= 2)
		{
			num = list[1].ToInt();
		}
		dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(gOHIIMFFFJI);
		if (dJKEECEOCJB != null && num > 0)
		{
			dJKEECEOCJB2 = dJKEECEOCJB.HIOBANJPMKF(num);
		}
		if (dJKEECEOCJB == null)
		{
			LLLOJBFMONN.Error("QuestActionGiveItem - cant find item \"%s\" with upgrade \"%i\"", lNIDLHOIHIM.resultSTR, num);
			return;
		}
		bool flag = false;
		kKDGLNECFHA.MCPIOGALBMK(JKFHICPKOPA, lNIDLHOIHIM);
		bool flag2 = lNIDLHOIHIM.resultNumber > 0.0;
		kKDGLNECFHA.MCPIOGALBMK(ICAGIOIDCBL, lNIDLHOIHIM);
		int num2 = (int)lNIDLHOIHIM.resultNumber;
		ListSF oPLPFMFAGMN = ListSF.ELEBLBJKDBI();
		if (flag2)
		{
			ListSF.FAAAGBACKAE(dJKEECEOCJB);
		}
		if (num2 > 0 || (num2 <= 0 && ListSF.CMGOCLGHNLH(dJKEECEOCJB.Name) == null))
		{
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			UserItem dKCHDHMLKHN = ListSF.GEFDJDIINND(dJKEECEOCJB, num2, 0L, flag2);
			if (dJKEECEOCJB2 != null)
			{
				dKCHDHMLKHN.HJONIDFKNJH("Upgrade");
				dKCHDHMLKHN.FMMDLMGHPIB(dJKEECEOCJB2.OBJDGBBFJOO);
				dKCHDHMLKHN.CDFODJBJIPI(nKGLHEGIKKP.PINDEKDNCNL());
			}
			if (dJKEECEOCJB.MHGODOLNDLE <= nKGLHEGIKKP.PINDEKDNCNL())
			{
				dJKEECEOCJB.BEBDMOEIEJN(true);
			}
		}
		else if (num2 == 0)
		{
			if (flag2)
			{
				UserItem nDMCFNGEPOA = ListSF.CMGOCLGHNLH(dJKEECEOCJB.Name);
				flag = ListSF.AFGHCIDFAHB(nDMCFNGEPOA, true);
			}
		}
		else
		{
			UserItem nDMCFNGEPOA2 = ListSF.CMGOCLGHNLH(dJKEECEOCJB.Name);
			ListSF.ADIFNIKODHH(nDMCFNGEPOA2, Math.Abs(num2));
		}
		if (flag)
		{
			ShopScene current = Scene<ShopScene>.get_Current();
			if (current != null)
			{
				ItemAction pCKPFBFHKJH = ItemAction.Item_Equip;
			}
		}
		if (num2 <= 0 && !flag2)
		{
			UserItem dKCHDHMLKHN2 = ListSF.CMGOCLGHNLH(dJKEECEOCJB.Name);
			if (dKCHDHMLKHN2 != null)
			{
				dKCHDHMLKHN2.BHKHOJPANHE().BEBDMOEIEJN(false);
				ListSF.CCDKHLAMKKO().KGFJPLKOABI();
			}
		}
		ListSF.ELEBLBJKDBI().JLCGOODFKAK(dJKEECEOCJB2);
		OGIJONMKABB();
	}
}
