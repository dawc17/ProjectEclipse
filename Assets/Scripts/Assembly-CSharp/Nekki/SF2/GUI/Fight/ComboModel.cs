using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class ComboModel : SFMonoBehaviour<ComboModel.KEDKBADCLOD>
	{
		public enum HKFFDEOCCIE
		{
			ON_COMBO_CHANGE = 0
		}

		public struct KEDKBADCLOD
		{
			public int GKAEJDCDMHC;

			public ComboTypeEvent Type;
		}

		public class ComboNode
		{
			public ComboItem Target;

			public int Count;

			public bool ACMKEEJFLJC;

			public bool MCPGOCBBNIK;

			public ComboTypes Type = ComboTypes.TypeCombo;
		}

		[SerializeField]
		private GameObject _comboItemPrefab;

		private ComboStatistic ODOJIOOGLJM = new ComboStatistic();

		private const float APCKONPPGJH = 40f;

		private const float LCPALHECMOF = 30f;

		private const float NNICJDBBIIB = 0f;

		private float OBFPHEHJNDM = 600f;

		private float IMJDFEHPIKM = 245f;

		private Vector2 EEDDAHKHDNK;

		private Vector2 DLIMPJAPMOK;

		private readonly Vector2 ALLNKANPNBL = new Vector2(0f, -40f);

		private List<ComboNode> OBKMHFLBGLE = new List<ComboNode>();

		private bool _fightPause;

		private int MPAJCNBPGCE;

		private int GHJAHHNABOC;

		private int GHPGBLHFOKB;

		private int MAOHKAOBHKO;

		private ScreenModel.JEDPGMIGGKK CHNAJMLHHPI;

		public ComboStatistic JKEGIADAKJG
		{
			get
			{
				return get_ComboStatistic();
			}
			set
			{
				set_ComboStatistic(value);
			}
		}

		public ComboStatistic get_ComboStatistic()
		{
			return ODOJIOOGLJM;
		}

		public void set_ComboStatistic(ComboStatistic value)
		{
			ODOJIOOGLJM = value;
		}

		public void Init(ScreenModel.JEDPGMIGGKK NPEAOKLDJHA)
		{
			CHNAJMLHHPI = NPEAOKLDJHA;
			GHJAHHNABOC = KKIIIIKBFAK(ComboTypes.TypeCombo);
			if (CHNAJMLHHPI == ScreenModel.JEDPGMIGGKK.TYPE_LEFT)
			{
				OBFPHEHJNDM *= -1f;
				EEDDAHKHDNK = new Vector2(0f, 0.5f);
				DLIMPJAPMOK = new Vector2(0f, 0.5f);
			}
			else
			{
				EEDDAHKHDNK = new Vector2(1f, 0.5f);
				DLIMPJAPMOK = new Vector2(1f, 0.5f);
			}
		}

		private ComboItem BDBLCIILHHH(ComboTypes LFLGCDNKNJI)
		{
			if (_comboItemPrefab == null)
			{
				return null;
			}
			ComboItem component = Object.Instantiate(_comboItemPrefab).GetComponent<ComboItem>();
			component.Init(LFLGCDNKNJI, CHNAJMLHHPI);
			return component;
		}

		public ComboNode CreateCritical()
		{
			ODOJIOOGLJM.IGMFLCNOKPA++;
			return GLJMJOACEIP(BDBLCIILHHH(ComboTypes.TypeCritical));
		}

		public ComboNode CreateShock()
		{
			ODOJIOOGLJM.OGMOILIMCOM++;
			return GLJMJOACEIP(BDBLCIILHHH(ComboTypes.TypeShock));
		}

		public ComboNode CreateFirstStrike()
		{
			ODOJIOOGLJM.MOLDOOIJELI++;
			return GLJMJOACEIP(BDBLCIILHHH(ComboTypes.TypeFirstStrike));
		}

		public ComboNode CreateHeadStrike()
		{
			ODOJIOOGLJM.BAHCDHKAJBB++;
			return GLJMJOACEIP(BDBLCIILHHH(ComboTypes.TypeHead));
		}

		public ComboNode CreateComboStrike(int value)
		{
			ComboItem comboItem = BDBLCIILHHH(ComboTypes.TypeCombo);
			comboItem.UpdateCount(value);
			return GLJMJOACEIP(comboItem);
		}

		public ComboNode CreateHotGroundTimer(int value)
		{
			ComboItem comboItem = BDBLCIILHHH(ComboTypes.TypeHotGroundTimer);
			comboItem.UpdateCount(value);
			return GLJMJOACEIP(comboItem);
		}

		private ComboNode GLJMJOACEIP(ComboItem target)
		{
			Vector2 aLLNKANPNBL = ALLNKANPNBL;
			float x = aLLNKANPNBL.x;
			Vector2 aLLNKANPNBL2 = ALLNKANPNBL;
			Vector2 vector = new Vector2(x, aLLNKANPNBL2.y);
			target.get_rectTransform().pivot = ((CHNAJMLHHPI != ScreenModel.JEDPGMIGGKK.TYPE_LEFT) ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f));
			vector.x = OBFPHEHJNDM;
			target.get_rectTransform().anchorMin = EEDDAHKHDNK;
			target.get_rectTransform().anchorMax = DLIMPJAPMOK;
			if (OBKMHFLBGLE.Count > 0)
			{
				RectTransform rectTransform = OBKMHFLBGLE[OBKMHFLBGLE.Count - 1].Target.get_rectTransform();
				RectTransform rectTransform2 = target.get_rectTransform();
				vector.y = rectTransform.localPosition.y;
				vector.y -= rectTransform.rect.height * rectTransform.pivot.y;
				vector.y -= rectTransform2.rect.height * rectTransform2.pivot.y;
				vector.y -= 40f;
			}
			target.transform.SetParent(base.transform, false);
			target.transform.localPosition = vector;
			ComboNode iNEGMMHCDGN = new ComboNode();
			iNEGMMHCDGN.Count = KKIIIIKBFAK(target.get_ComboType());
			iNEGMMHCDGN.Target = target;
			iNEGMMHCDGN.Type = target.get_ComboType();
			iNEGMMHCDGN.MCPGOCBBNIK = target.get_ComboType() == ComboTypes.TypeHotGroundTimer;
			OBKMHFLBGLE.Add(iNEGMMHCDGN);
			return iNEGMMHCDGN;
		}

		public void AddCrazyStyle(int value)
		{
			ODOJIOOGLJM.BPBDGAPENAK = (FightStatistics.EMKEIEJMONM)Mathf.Max((int)ODOJIOOGLJM.BPBDGAPENAK, value);
			ODOJIOOGLJM.StatisticCrazyStyleToString = ODOJIOOGLJM.OLONAJAOFOA();
		}

		public void AddPerfect()
		{
			ODOJIOOGLJM.JDKFHFOJKPI++;
		}

		private void BECPPOBCCLO()
		{
			ODOJIOOGLJM.NFKHLNHIIKH++;
		}

		public void ResetComboStrike()
		{
			GHPGBLHFOKB = 0;
		}

		public void UpdateHotGroundTimer(int time)
		{
			MAOHKAOBHKO = time;
			ComboNode iNEGMMHCDGN = OBKMHFLBGLE.Find((ComboNode DHDMNHCIPEH) => DHDMNHCIPEH.Type == ComboTypes.TypeHotGroundTimer);
			if (iNEGMMHCDGN == null)
			{
				iNEGMMHCDGN = CreateHotGroundTimer(MAOHKAOBHKO);
			}
			iNEGMMHCDGN.Count = KKIIIIKBFAK(iNEGMMHCDGN.Type);
			iNEGMMHCDGN.Target.UpdateCount(MAOHKAOBHKO);
		}

		public void OnFightPause(bool value)
		{
			_fightPause = value;
		}

		public void UpdateCombo(int value, int HFMKKLJGPPN)
		{
			GHPGBLHFOKB = value;
			if (GHPGBLHFOKB > 0)
			{
				MPAJCNBPGCE = 0;
			}
			if (GHPGBLHFOKB >= GameUtils.NPDOLGNNINO())
			{
				ComboNode iNEGMMHCDGN = OBKMHFLBGLE.Find((ComboNode DHDMNHCIPEH) => DHDMNHCIPEH.Type == ComboTypes.TypeCombo);
				bool flag = false;
				if (iNEGMMHCDGN == null)
				{
					iNEGMMHCDGN = CreateComboStrike(GHPGBLHFOKB);
					flag = true;
				}
				iNEGMMHCDGN.Count = KKIIIIKBFAK(iNEGMMHCDGN.Type, HFMKKLJGPPN);
				iNEGMMHCDGN.Target.UpdateCount(GHPGBLHFOKB);
				ODOJIOOGLJM.KKJHBKBMPGN = Mathf.Max(ODOJIOOGLJM.KKJHBKBMPGN, GHPGBLHFOKB);
				LJNNMGPGDOO((!flag) ? ComboTypeEvent.COMBO_INCREASE : ComboTypeEvent.COMBO_START);
			}
		}

		public void RemoveAllCombo()
		{
			foreach (ComboNode item in OBKMHFLBGLE)
			{
				item.Target.gameObject.SetActive(false);
				Object.Destroy(item.Target.gameObject);
			}
			OBKMHFLBGLE.Clear();
		}

		private void LJNNMGPGDOO(ComboTypeEvent LFLGCDNKNJI)
		{
			CallEvent(0, new KEDKBADCLOD
			{
				GKAEJDCDMHC = GHPGBLHFOKB,
				Type = LFLGCDNKNJI
			});
		}

		private int KKIIIIKBFAK(ComboTypes LFLGCDNKNJI, int HFMKKLJGPPN = 0)
		{
			switch (LFLGCDNKNJI)
			{
			case ComboTypes.TypeCombo:
				return Mathf.Max(GameUtils.KCBHAMHLGBC() + HFMKKLJGPPN, 0);
			case ComboTypes.TypeHotGroundTimer:
				return GameUtils.LDHIBCJCHFK();
			default:
				return GameUtils.MLAHKALHANF();
			}
		}

		private bool CBICGICCCOM(ComboNode node)
		{
			return node.MCPGOCBBNIK || !_fightPause;
		}

		public bool MoveTo(ComboItem target, Vector2 IPMPAMAHLJG, float ALCFJHNPDGL)
		{
			Vector2 vector = target.transform.localPosition;
			if (CHNAJMLHHPI == ScreenModel.JEDPGMIGGKK.TYPE_RIGHT)
			{
				IPMPAMAHLJG.x *= -1f;
			}
			float num = ((!(IPMPAMAHLJG.x > vector.x)) ? (0f - ALCFJHNPDGL) : ALCFJHNPDGL);
			float num2 = ((!(IPMPAMAHLJG.y > vector.y)) ? (0f - ALCFJHNPDGL) : ALCFJHNPDGL);
			float num3 = Mathf.Abs(IPMPAMAHLJG.x - vector.x);
			float num4 = Mathf.Abs(IPMPAMAHLJG.y - vector.y);
			bool flag = false;
			if (num3 > 0f)
			{
				vector.x = ((!(num3 > Mathf.Abs(num))) ? IPMPAMAHLJG.x : (vector.x + num));
				flag = true;
			}
			if (num4 > 0f)
			{
				vector.y = ((!(num4 > Mathf.Abs(num2))) ? IPMPAMAHLJG.y : (vector.y + num2));
				flag = true;
			}
			if (flag)
			{
				target.transform.localPosition = vector;
			}
			return vector == IPMPAMAHLJG;
		}

		private void BLMPDANIEDN()
		{
			Vector2 aLLNKANPNBL = ALLNKANPNBL;
			float x = aLLNKANPNBL.x;
			Vector2 aLLNKANPNBL2 = ALLNKANPNBL;
			Vector2 iPMPAMAHLJG = new Vector2(x, aLLNKANPNBL2.y);
			foreach (ComboNode item in OBKMHFLBGLE)
			{
				RectTransform rectTransform = item.Target.get_rectTransform();
				iPMPAMAHLJG.x = rectTransform.localPosition.x;
				if (item.ACMKEEJFLJC)
				{
					if (iPMPAMAHLJG.y != 0f)
					{
						iPMPAMAHLJG.y -= rectTransform.rect.height * rectTransform.pivot.y;
					}
					MoveTo(item.Target, iPMPAMAHLJG, 0f);
					iPMPAMAHLJG.y -= rectTransform.rect.height * rectTransform.pivot.y;
					iPMPAMAHLJG.y -= 40f;
				}
				else
				{
					iPMPAMAHLJG.y = rectTransform.localPosition.y;
					iPMPAMAHLJG.y -= rectTransform.rect.height * rectTransform.pivot.y;
					iPMPAMAHLJG.y -= 40f;
				}
			}
		}

		private void NADGMCEPDAK()
		{
			List<ComboNode> list = new List<ComboNode>();
			foreach (ComboNode item in OBKMHFLBGLE)
			{
				if (!CBICGICCCOM(item))
				{
					continue;
				}
				if (item.Count > 0)
				{
					item.ACMKEEJFLJC = false;
					Vector2 iPMPAMAHLJG = item.Target.transform.localPosition;
					iPMPAMAHLJG.x = IMJDFEHPIKM;
					if (MoveTo(item.Target, iPMPAMAHLJG, 30f) && (item.Type != ComboTypes.TypeHotGroundTimer || GameUtils.GGBABPJBGJB() == 1))
					{
						item.ACMKEEJFLJC = true;
						item.Count--;
					}
				}
				else
				{
					item.ACMKEEJFLJC = false;
					Vector2 iPMPAMAHLJG2 = item.Target.transform.localPosition;
					iPMPAMAHLJG2.x = 0f - item.Target.get_rectTransform().rect.width;
					if (MoveTo(item.Target, iPMPAMAHLJG2, 30f))
					{
						list.Add(item);
					}
				}
			}
			foreach (ComboNode item2 in list)
			{
				OBKMHFLBGLE.Remove(item2);
				item2.Target.gameObject.SetActive(false);
				Object.Destroy(item2.Target.gameObject);
			}
		}

		public void Render()
		{
			NADGMCEPDAK();
			BLMPDANIEDN();
			if (GHPGBLHFOKB <= 0)
			{
				return;
			}
			if (MPAJCNBPGCE >= GHJAHHNABOC)
			{
				if (GHPGBLHFOKB >= GameUtils.NPDOLGNNINO())
				{
					LJNNMGPGDOO(ComboTypeEvent.COMBO_STOP);
				}
				MPAJCNBPGCE = 0;
				GHPGBLHFOKB = 0;
			}
			else
			{
				MPAJCNBPGCE++;
			}
		}
	}
}
