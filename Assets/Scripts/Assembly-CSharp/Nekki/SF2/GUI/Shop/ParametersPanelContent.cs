using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Shop
{
	public class ParametersPanelContent : SidePanelContent
	{
		[SerializeField]
		private BaseScrollContent _baseScrollContent;

		[SerializeField]
		private ItemsScroll _itemsScroll;

		[SerializeField]
		private GameObject _scrollItemPrefab;

		private List<BaseScrollItem> IOHGFGNNCFA = new List<BaseScrollItem>();

		private ModelParameters EFDDBGENGKI;

		private EquippedItemsStruct NCPDKIEDPHN = new EquippedItemsStruct();

		public override void Init()
		{
			HFAOJIJIGHE();
			if (_baseScrollContent != null && IOHGFGNNCFA != null)
			{
				_baseScrollContent.SetItems(IOHGFGNNCFA);
			}
			else
			{
				LLLOJBFMONN.Error("ParametersPanelContent.Init _baseScrollContent or _items is null");
			}
			if (_itemsScroll != null)
			{
				_itemsScroll.Init();
			}
			else
			{
				LLLOJBFMONN.Error("ParametersPanelContent.Init _itemsScroll is null");
			}
		}

		private bool GBKCPGEBANK(WarriorAttribute FFLFOELEKIG)
		{
			return FFLFOELEKIG.GDCBBAHKCIE || FFLFOELEKIG.GDECIAJAFHH;
		}

		private void HFAOJIJIGHE()
		{
			EFDDBGENGKI = new ModelParameters(ListSF.CCDKHLAMKKO().get_Parameters());
			if (EFDDBGENGKI == null)
			{
				LLLOJBFMONN.Error("ParametersPanelContent.CreateItems modelParameters is null");
				return;
			}
			EFDDBGENGKI.NOBKKLBJFIL();
			foreach (WarriorAttribute item in GameUtils.BGENALLCKII.IBLHIAHECLK)
			{
				if (!GBKCPGEBANK(item))
				{
					GameObject gameObject = Object.Instantiate(_scrollItemPrefab);
					ParameterScrollItem component = gameObject.GetComponent<ParameterScrollItem>();
					if (component != null)
					{
						bool eIAKNKDEEKA = false;
						int OEMALIFPGPO = 0;
						EFDDBGENGKI.IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO);
						component.gameObject.name = string.Format("ParameterScrollItem({0})", item.get_Name());
						component.Init(item.get_Name(), item.MJBPMLCLMFN, OEMALIFPGPO, OEMALIFPGPO, eIAKNKDEEKA);
						IOHGFGNNCFA.Add(component);
					}
				}
			}
		}

		public void UpdateParameters(ItemInfo PJDAGCBPLJE)
		{
			KCDFCHGDJBJ(PJDAGCBPLJE, 0f);
		}

		public void UpdateParametersWithDuration(ItemInfo PJDAGCBPLJE)
		{
			KCDFCHGDJBJ(PJDAGCBPLJE, 2f);
		}

		protected void KCDFCHGDJBJ(ItemInfo PJDAGCBPLJE, float _Duration)
		{
			ModelParameters kIKOGDEPGHB = ListSF.CCDKHLAMKKO().get_Parameters();
			kIKOGDEPGHB.ALBOCOGOBCN(NCPDKIEDPHN);
			EFDDBGENGKI.ALGDEEKFPKK(NCPDKIEDPHN);
			EFDDBGENGKI.OLLNIKFPMKE(PJDAGCBPLJE.Type, PJDAGCBPLJE);
			kIKOGDEPGHB.NOBKKLBJFIL();
			EFDDBGENGKI.NOBKKLBJFIL();
			foreach (ParameterScrollItem item in IOHGFGNNCFA)
			{
				string attributeName = item.get_AttributeName();
				int OEMALIFPGPO = 0;
				kIKOGDEPGHB.IBLHIAHECLK.Get(attributeName, ref OEMALIFPGPO);
				int OEMALIFPGPO2 = 0;
				EFDDBGENGKI.IBLHIAHECLK.Get(attributeName, ref OEMALIFPGPO2);
				item.SetValue(OEMALIFPGPO, OEMALIFPGPO2, _Duration);
			}
		}
	}
}
