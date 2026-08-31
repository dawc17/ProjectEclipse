using System.Collections.Generic;
using System.Xml;
using Nekki.Utils;
using UnityEngine.Events;

public class UserItems
{
	public class LNLLFLPJIEM : UnityEvent<List<UserItem>>
	{
	}

	public class DNJBDLFKHPG : UnityEvent<UserItem>
	{
	}

	public LNLLFLPJIEM LHNOOJJNDPK = new LNLLFLPJIEM();

	public DNJBDLFKHPG HHGJMMHMEMP = new DNJBDLFKHPG();

	private List<UserItem> _items = new List<UserItem>();

	private readonly List<string> _missingModItemIds = new List<string>();

	public IReadOnlyList<string> MissingModItemIds => _missingModItemIds.AsReadOnly();

	private List<UserItem> HBLLBGLBDGI = new List<UserItem>();

	public List<UserItem> CGJGNEADJBH = new List<UserItem>();

	public List<UserItem> KDIJGMMNFHH = new List<UserItem>();

	public List<RecipeItemInfo> LFADKPKKFMP = new List<RecipeItemInfo>();

	public List<UserItem> OJIAKDDCGLB
	{
		get
		{
			return DJBOFEEKJMP();
		}
	}

	public List<UserItem> JAHAJKEAECA
	{
		get
		{
			return EHDCCPKOANN();
		}
	}

	public List<UserItem> KJBABANBOEF
	{
		get
		{
			return MPACCEAFDOH();
		}
	}

	public UserItems()
	{
		GlobalTimer.get_Instance().addEventListener(0, ILFBDHDMHPD);
	}

	public List<UserItem> DJBOFEEKJMP()
	{
		return _items;
	}

	public List<UserItem> EHDCCPKOANN()
	{
		return HBLLBGLBDGI;
	}

	public List<UserItem> MPACCEAFDOH()
	{
		return KDIJGMMNFHH;
	}

	public void Parse(XmlNode EPGOOPEHFMO)
	{
		_missingModItemIds.Clear();
		if (EPGOOPEHFMO == null)
		{
			return;
		}
		foreach (XmlNode childNode in EPGOOPEHFMO.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element) continue;
			if (Eclipse.Modding.ModSaveData.IsMissingItem(childNode, name => ListSF.DJBOFEEKJMP().KCCDBEEKBCG(name) != null))
			{
				_missingModItemIds.Add(childNode.Attributes["Name"].Value);
				continue; // Preserve the entire save node, but do not run delivery/equipment logic on it.
			}
			UserItem item = new UserItem(childNode);
			if (Eclipse.Modding.ModSaveData.IsExternalItem(item.get_Name()))
			{
				ItemInfo definition = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(item.get_Name());
				XmlAttribute equipped = EPGOOPEHFMO.ParentNode?.Attributes?[definition.Type];
				if (equipped != null) item.JBLKCIBKMKB(equipped.Value == item.get_Name());
			}
			GEFDJDIINND(item);
		}
		if (_missingModItemIds.Count > 0)
			UnityEngine.Debug.LogWarning("[ModSave] Preserved " + _missingModItemIds.Count +
				" unavailable mod item(s) in the save: " + string.Join(", ", _missingModItemIds));
	}

	public UserItem GEFDJDIINND(UserItem value, bool HPCLCADMKCG = false)
	{
		if (value.BHKHOJPANHE() != null && value.BHKHOJPANHE().Type == "Seal")
		{
			value.BHKHOJPANHE().BEBDMOEIEJN(true);
		}
		if (HPCLCADMKCG)
		{
			UserItem dKCHDHMLKHN = _items.Find((UserItem DHDMNHCIPEH) => DHDMNHCIPEH.get_Name().Equals(value.get_Name()));
			if (dKCHDHMLKHN != null)
			{
				int index = _items.IndexOf(dKCHDHMLKHN);
				_items.Insert(index, value);
				_items.Remove(dKCHDHMLKHN);
			}
			else
			{
				_items.Add(value);
			}
		}
		else
		{
			_items.Add(value);
		}
		if (value.IJGAOHJNLAH() > 0)
		{
			HBLLBGLBDGI.Add(value);
		}
		return value;
	}

	public UserItem CMGOCLGHNLH(ItemInfo item)
	{
		return (item == null) ? null : CMGOCLGHNLH(item.Name);
	}

	public UserItem CMGOCLGHNLH(string name)
	{
		foreach (UserItem item in _items)
		{
			if (item.get_Name() == name)
			{
				return item;
			}
		}
		if (Eclipse.Modding.ModSaveData.IsExternalItem(name))
		{
			ItemInfo requested = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(name);
			if (requested != null)
			{
				foreach (UserItem item in _items)
				{
					if (!Eclipse.Modding.ModSaveData.IsExternalItem(item.get_Name())) continue;
					if (ListSF.DJBOFEEKJMP().KCCDBEEKBCG(item.get_Name()) == requested) return item;
				}
			}
		}
		return null;
	}

	public List<UserItem> HOPBBLJLHOB(string LFLGCDNKNJI, string GIGAFKGDKNH = "", bool isActive = true)
	{
		List<UserItem> list = new List<UserItem>();
		foreach (UserItem item in _items)
		{
			ItemInfo dJKEECEOCJB = item.BHKHOJPANHE();
			if (dJKEECEOCJB != null)
			{
				bool flag = dJKEECEOCJB.Type.Equals(LFLGCDNKNJI);
				bool flag2 = dJKEECEOCJB.MDPPNGIEJGD.Equals(GIGAFKGDKNH) || GIGAFKGDKNH.Equals(string.Empty);
				bool flag3 = dJKEECEOCJB.DCHJDPCEODD || !isActive;
				if (flag && flag2 && flag3)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public bool MHMFKLLIFEJ(ItemInfo item)
	{
		UserItem dKCHDHMLKHN = CMGOCLGHNLH(item);
		return dKCHDHMLKHN != null && dKCHDHMLKHN.OFOPFCJNEBL() > 0;
	}

	public void JALMHIICOPB(ItemInfo item, bool GHLLDFNGMAE)
	{
		if (item == null)
		{
			return;
		}
		string gOHIIMFFFJI = GameUtils.GetDefaultItem(item.Type);
		UserItem dKCHDHMLKHN = CMGOCLGHNLH(gOHIIMFFFJI);
		if (dKCHDHMLKHN == null || dKCHDHMLKHN.BHKHOJPANHE() == null)
		{
			return;
		}
		ListSF.CCDKHLAMKKO().get_Parameters().OLLNIKFPMKE(dKCHDHMLKHN.BHKHOJPANHE().Type, dKCHDHMLKHN.BHKHOJPANHE());
		if (GHLLDFNGMAE)
		{
			UserItem dKCHDHMLKHN2 = CMGOCLGHNLH(item);
			if (dKCHDHMLKHN2 != null)
			{
				dKCHDHMLKHN2.JBLKCIBKMKB(false);
				dKCHDHMLKHN.JBLKCIBKMKB(true);
				ListSF.CCDKHLAMKKO().BMADIJMPENJ(dKCHDHMLKHN);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
			}
		}
	}

	public void EEDJEDBMIMI(ItemInfo item, bool GHLLDFNGMAE)
	{
		if (item == null)
		{
			return;
		}
		ListSF.CCDKHLAMKKO().get_Parameters().OLLNIKFPMKE(item.Type, item);
		if (!GHLLDFNGMAE)
		{
			return;
		}
		UserItem dKCHDHMLKHN = CMGOCLGHNLH(item);
		if (dKCHDHMLKHN != null && dKCHDHMLKHN.GKGIKMCMCPB())
		{
			List<UserItem> list = HOPBBLJLHOB(item.Type, string.Empty);
			list.ForEach((UserItem DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.JBLKCIBKMKB(false);
			});
			dKCHDHMLKHN.JBLKCIBKMKB(true);
			ListSF.CCDKHLAMKKO().BMADIJMPENJ(dKCHDHMLKHN);
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public void HOMCPNCGPDB(List<ItemInfo> HELFDCAIJNE)
	{
		ModelParameters kIKOGDEPGHB = ListSF.CCDKHLAMKKO().get_Parameters();
		if (kIKOGDEPGHB == null)
		{
			return;
		}
		kIKOGDEPGHB.HEKILHEHMMH.Clear();
		foreach (ItemInfo item in HELFDCAIJNE)
		{
			UserItem dKCHDHMLKHN = CMGOCLGHNLH(item);
			if (dKCHDHMLKHN != null)
			{
				dKCHDHMLKHN.KIGHKCOCJFJ(item);
				if (dKCHDHMLKHN.EFMFGEPDAOP() && item.Type.Equals("Decorate"))
				{
					kIKOGDEPGHB.HEKILHEHMMH.Add(item);
				}
			}
		}
		NHJAHNDOLAE();
	}

	public void NHJAHNDOLAE()
	{
		_items.ForEach((UserItem DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
		});
	}

	public void DINFNDFAJMB()
	{
		long localTimeUTC = GlobalTimer.get_LocalTimeUTC();
		List<UserItem> list = new List<UserItem>();
		bool flag = false;
		foreach (UserItem item in HBLLBGLBDGI)
		{
			if (item.IJGAOHJNLAH() <= 0)
			{
				list.Add(item);
			}
			else if (item.IJGAOHJNLAH() <= localTimeUTC)
			{
				GBLHFNGPIOF(item);
				list.Add(item);
				flag = true;
			}
		}
		if (list.Count > 0)
		{
			if (flag)
			{
				LHNOOJJNDPK.Invoke(list);
			}
			list.ForEach((UserItem DHDMNHCIPEH) =>
			{
				HBLLBGLBDGI.Remove(DHDMNHCIPEH);
			});
		}
	}

	public void GBLHFNGPIOF(UserItem NDMCFNGEPOA)
	{
		if (NDMCFNGEPOA != null && NDMCFNGEPOA.IJGAOHJNLAH() > 0)
		{
			if (NDMCFNGEPOA.DBKKJGBJOEO())
			{
				MPACCEAFDOH().Add(NDMCFNGEPOA);
			}
			else
			{
				EHDCCPKOANN().Add(NDMCFNGEPOA);
			}
			HHGJMMHMEMP.Invoke(NDMCFNGEPOA);
			QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
			hHKLFIIBIFF.DLKPBAJDHBO = NDMCFNGEPOA.BHKHOJPANHE();
			if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DELIVERY))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			}
			if (NDMCFNGEPOA.OFOPFCJNEBL() <= 0)
			{
				NDMCFNGEPOA.CHILOKHFALD(1);
				NDMCFNGEPOA.set_DeliveryTime(-1L);
			}
			if (NDMCFNGEPOA.EIMMBNNMBCN() > 0 && NDMCFNGEPOA.EIMMBNNMBCN() > NDMCFNGEPOA.DHNNCAEEMLL())
			{
				NDMCFNGEPOA.FMMDLMGHPIB(NDMCFNGEPOA.EIMMBNNMBCN());
				NDMCFNGEPOA.set_DeliveryTime(-1L);
			}
			NDMCFNGEPOA.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public UserItem PKKKAFIHHMI(string LFLGCDNKNJI)
	{
		List<UserItem> list = new List<UserItem>();
		foreach (UserItem item in _items)
		{
			ItemInfo dJKEECEOCJB = item.BHKHOJPANHE();
			if (dJKEECEOCJB != null && dJKEECEOCJB.Type == LFLGCDNKNJI)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			int index = NekkiMath.randomInt(list.Count);
			return list[index];
		}
		return null;
	}

	public List<UserItem> HECLNLNGFCD()
	{
		List<UserItem> list = new List<UserItem>();
		foreach (UserItem item in _items)
		{
			if (item.IJGAOHJNLAH() > 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<UserItem> JCMOHPFKPBO()
	{
		List<UserItem> list = new List<UserItem>();
		foreach (UserItem item in _items)
		{
			if (item.EFMFGEPDAOP())
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<RecipeItemInfo> PHKEAPFEOLP()
	{
		List<RecipeItemInfo> list = new List<RecipeItemInfo>();
		foreach (UserItem item in _items)
		{
			if (item.PHDBCIHJKON() != null)
			{
				list.Add(item.PHDBCIHJKON());
			}
		}
		return list;
	}

	public void UpdateLockItems(int OMHDLKNHNMJ)
	{
		List<ItemInfo> list = ListSF.DJBOFEEKJMP().HCDLKHKBEPF();
		foreach (ItemInfo item in list)
		{
			if (item.DCHJDPCEODD)
			{
				ListSF.DJBOFEEKJMP().SetNewAddItem(item, true, OMHDLKNHNMJ);
			}
		}
	}

	public void PHMMJIENGEP()
	{
		foreach (UserItem item in _items)
		{
			GJGJALKONNA(item, item.BHKHOJPANHE());
		}
	}

	public void GJGJALKONNA(UserItem NDMCFNGEPOA, ItemInfo PJDAGCBPLJE)
	{
		int num = NDMCFNGEPOA.DHNNCAEEMLL();
		if (PJDAGCBPLJE == null || PJDAGCBPLJE.OBJDGBBFJOO == num)
		{
			return;
		}
		int num2 = int.MaxValue;
		int num3 = int.MinValue;
		int num4 = 0;
		bool flag = false;
		NDMCFNGEPOA.HJONIDFKNJH("Upgrade");
		List<UpgradeData> list = PJDAGCBPLJE.DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			num4 = item.OGLHOJNMEBD.AKKLOMFOLNO;
			if (num4 == num)
			{
				return;
			}
			if (num4 > num && num4 <= num2)
			{
				num2 = num4;
				flag = true;
			}
			if (num4 >= num3)
			{
				num3 = num4;
			}
		}
		NDMCFNGEPOA.FMMDLMGHPIB((!flag) ? num3 : num2);
		NDMCFNGEPOA.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
	}

	private void ILFBDHDMHPD(ExtentionBehaviour.CallEventArgs JKOCDNPPJDG)
	{
		DINFNDFAJMB();
	}

	public void IJFJMMCFIGH()
	{
		LFADKPKKFMP.Clear();
	}
}
