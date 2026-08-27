using System.Collections.Generic;
using System.Xml;

public class QuestActionForeach : QuestAction
{
	public enum PDIAEAEHHPE
	{
		FOREACH_NONE = 0,
		FOREACH_ITEMS = 1,
		FOREACH_DELIVERY_ITEMS = 2,
		FOREACH_DELIVERY_UPGRADES = 3,
		FOREACH_PAID_ITEMS = 4,
		FOREACH_BATTLES = 5,
		FOREACH_DELIVERY_ENCHANTMENTS = 6
	}

	private bool LNENABHHABO;

	private bool LIJKDJEAJJJ;

	private QuestStage DOKAIKMLLDK;

	private List<string> nodes = new List<string>();

	private int index = -1;

	private int PEEOEOMEBFG;

	private QuestParameters NFIKJCJGMBB;

	private string name;

	private PDIAEAEHHPE LFLGCDNKNJI;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		LFLGCDNKNJI = GetType(EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK(string.Empty));
		name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		index = -1;
		PEEOEOMEBFG = 0;
		NFIKJCJGMBB = GFIHPBCEEOB;
		DOKAIKMLLDK = ListSF.ELEBLBJKDBI().PBGCEEBDBGG(name);
		nodes.Clear();
		switch (LFLGCDNKNJI)
		{
		case PDIAEAEHHPE.FOREACH_ITEMS:
			FGFAOPOODJA();
			break;
		case PDIAEAEHHPE.FOREACH_DELIVERY_ITEMS:
			RunDeliveryItems(false);
			break;
		case PDIAEAEHHPE.FOREACH_DELIVERY_UPGRADES:
			RunDeliveryItems(true);
			break;
		case PDIAEAEHHPE.FOREACH_PAID_ITEMS:
			KINMIFFFGDA();
			break;
		case PDIAEAEHHPE.FOREACH_BATTLES:
			AKFLMFMBPKD();
			break;
		case PDIAEAEHHPE.FOREACH_DELIVERY_ENCHANTMENTS:
			OBAKPOLFCCP();
			break;
		}
		PEEOEOMEBFG = nodes.Count;
		LNENABHHABO = true;
		AKPKHLBCOFB();
	}

	private void AKPKHLBCOFB()
	{
		LIJKDJEAJJJ = true;
		while (LNENABHHABO)
		{
			Run();
		}
		LIJKDJEAJJJ = false;
	}

	private void Run()
	{
		LNENABHHABO = false;
		index++;
		if (DOKAIKMLLDK == null || index > PEEOEOMEBFG - 1)
		{
			if (LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_ITEMS || LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_UPGRADES)
			{
				Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
				nKGLHEGIKKP.KHCNHPCPFII().HHGJMMHMEMP.RemoveListener(JOEFPOMBMPB);
				switch (LFLGCDNKNJI)
				{
				case PDIAEAEHHPE.FOREACH_DELIVERY_ITEMS:
					nKGLHEGIKKP.KHCNHPCPFII().EHDCCPKOANN().Clear();
					break;
				case PDIAEAEHHPE.FOREACH_DELIVERY_UPGRADES:
					nKGLHEGIKKP.KHCNHPCPFII().MPACCEAFDOH().Clear();
					break;
				case PDIAEAEHHPE.FOREACH_DELIVERY_ENCHANTMENTS:
					nKGLHEGIKKP.KHCNHPCPFII().LFADKPKKFMP.Clear();
					break;
				}
			}
			else if (LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_ENCHANTMENTS)
			{
				Roster nKGLHEGIKKP2 = ListSF.CCDKHLAMKKO();
				nKGLHEGIKKP2.RemoveEventListener(1, BNIJKKOOAEL);
				nKGLHEGIKKP2.KHCNHPCPFII().IJFJMMCFIGH();
			}
			OGIJONMKABB();
			LNENABHHABO = false;
		}
		else
		{
			NFIKJCJGMBB.PFKPHBPBPAF = nodes[index];
			if (DOKAIKMLLDK.Compare(NFIKJCJGMBB))
			{
				DOKAIKMLLDK.AddEventListener(1, OnQuestComplete);
				DOKAIKMLLDK.MHHNIPBJNAD(NFIKJCJGMBB, false);
			}
			else
			{
				LNENABHHABO = true;
			}
		}
	}

	private void OnQuestComplete(object data)
	{
		QuestStage mLLKDGBEGJI = (QuestStage)data;
		mLLKDGBEGJI.RemoveEventListener(1, OnQuestComplete);
		LNENABHHABO = true;
		if (!LIJKDJEAJJJ)
		{
			AKPKHLBCOFB();
		}
	}

	private void FGFAOPOODJA()
	{
		List<UserItem> list = ListSF.CCDKHLAMKKO().KHCNHPCPFII().DJBOFEEKJMP();
		foreach (UserItem item in list)
		{
			nodes.Add(item.get_Name());
		}
	}

	private void RunDeliveryItems(bool EIOPLHKAEPK)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		List<UserItem> list = ((!EIOPLHKAEPK) ? nKGLHEGIKKP.KHCNHPCPFII().EHDCCPKOANN() : nKGLHEGIKKP.KHCNHPCPFII().MPACCEAFDOH());
		foreach (UserItem item in list)
		{
			nodes.Add(item.get_Name());
		}
		nKGLHEGIKKP.KHCNHPCPFII().HHGJMMHMEMP.AddListener(JOEFPOMBMPB);
	}

	private void KINMIFFFGDA()
	{
		List<ItemInfo> list = ListSF.DJBOFEEKJMP().ONFMAJEAACM("RealMoneyItem");
		if (list == null)
		{
			return;
		}
		foreach (ItemInfo item in list)
		{
			nodes.Add(item.Name);
		}
	}

	private void OBAKPOLFCCP()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		List<RecipeItemInfo> lFADKPKKFMP = nKGLHEGIKKP.KHCNHPCPFII().LFADKPKKFMP;
		foreach (RecipeItemInfo item in lFADKPKKFMP)
		{
			nodes.Add(item.ToString());
		}
		nKGLHEGIKKP.AddEventListener(1, BNIJKKOOAEL);
	}

	private void AKFLMFMBPKD()
	{
		List<Battle> list = ListSF.ELEBLBJKDBI().MMCHMBIKIEP();
		foreach (Battle item in list)
		{
			nodes.Add(item.OJDNDADJBID());
		}
	}

	private void JOEFPOMBMPB(object data)
	{
		if (data != null)
		{
			UserItem dKCHDHMLKHN = (UserItem)data;
			if ((LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_ITEMS && !dKCHDHMLKHN.DBKKJGBJOEO()) || (LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_UPGRADES && dKCHDHMLKHN.DBKKJGBJOEO()))
			{
				nodes.Add(dKCHDHMLKHN.get_Name());
			}
			PEEOEOMEBFG = nodes.Count;
		}
	}

	private void BNIJKKOOAEL(object data)
	{
		if (data != null)
		{
			RecipeItemInfo bNJOCBKNPMG = (RecipeItemInfo)data;
			if (LFLGCDNKNJI == PDIAEAEHHPE.FOREACH_DELIVERY_ENCHANTMENTS)
			{
				nodes.Add(bNJOCBKNPMG.ToString());
				PEEOEOMEBFG = nodes.Count;
			}
		}
	}

	private PDIAEAEHHPE GetType(string _type)
	{
		switch (_type)
		{
		case "Items":
			return PDIAEAEHHPE.FOREACH_ITEMS;
		case "DeliveryItems":
			return PDIAEAEHHPE.FOREACH_DELIVERY_ITEMS;
		case "PaidItems":
			return PDIAEAEHHPE.FOREACH_PAID_ITEMS;
		case "DeliveryUpgrades":
			return PDIAEAEHHPE.FOREACH_DELIVERY_UPGRADES;
		case "Battles":
			return PDIAEAEHHPE.FOREACH_BATTLES;
		case "DeliveryEnchantments":
			return PDIAEAEHHPE.FOREACH_DELIVERY_ENCHANTMENTS;
		default:
			return PDIAEAEHHPE.FOREACH_NONE;
		}
	}
}
