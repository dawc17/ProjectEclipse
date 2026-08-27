using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class LampsPanel : SFMonoBehaviour<object>
	{
		public enum EALAEJEKNOO
		{
			OnClickLamp = 0
		}

		public const float LAMP_PADDING_BOT = -55f;

		public const float LAMP_DISTANCE = 22f;

		public const float LAMP_WIDTH = 68f;

		private List<Button> ICHBMJODKAB = new List<Button>();

		private List<ResolutionImage> EDADANMDDCH = new List<ResolutionImage>();

		private List<Button> JMPJJMHGIKF = new List<Button>();

		private List<ResolutionImage> CGCLPOOGOGG = new List<ResolutionImage>();

		[SerializeField]
		private ResolutionImage _lampOn;

		private int HPOLPFPFNHL;

		private int GCCPALLIFBO = 255;

		private int DDAMPCFEAKL;

		private int ALHECCDENMO;

		private bool BDIPNCMGEEN;

		private bool LOGDIAPFANM = true;

		private bool MBIEJLCBCIM = true;

		private bool HPCHHDHGDAA;

		[SerializeField]
		private LabelAlias _locationName;

		[SerializeField]
		private GameObject _lampButtonPrefab;

		[SerializeField]
		private GameObject _lampIndicatorPrefab;

		[SerializeField]
		private GameObject _lampsContainer;

		[SerializeField]
		private GameObject _indicatorsContainer;

		public void Init()
		{
			ALHECCDENMO = MapGUI.JHLMDGBGGEP.CPLJCIFJAGN;
			_locationName.set_text("???");
		}

		public void ClearLamps()
		{
			foreach (Button item in ICHBMJODKAB)
			{
				Object.Destroy(item.gameObject);
			}
			ICHBMJODKAB.Clear();
			foreach (ResolutionImage item2 in EDADANMDDCH)
			{
				Object.Destroy(item2.gameObject);
			}
			EDADANMDDCH.Clear();
		}

		public void AddLamps(int JPCFOCCOIHL)
		{
			float num = 0f;
			float num2 = 68f * (float)JPCFOCCOIHL + 22f * (float)(JPCFOCCOIHL - 1);
			for (int i = 0; i < JPCFOCCOIHL; i++)
			{
				GameObject gameObject = Object.Instantiate(_lampButtonPrefab);
				SFButton component = gameObject.GetComponent<SFButton>();
				component.gameObject.transform.SetParent(_lampsContainer.transform, false);
				component.ButtonId = i;
				component.AddEventListener(2, OnLampClicked);
				float x = num - num2 / 2f + (float)i * 90f + 34f;
				float y = -55f;
				component.transform.localPosition = new Vector3(x, y);
				ICHBMJODKAB.Add(component);
				GameObject gameObject2 = Object.Instantiate(_lampIndicatorPrefab);
				ResolutionImage component2 = gameObject2.GetComponent<ResolutionImage>();
				component2.gameObject.transform.SetParent(_indicatorsContainer.transform, false);
				component2.transform.localPosition = new Vector3(x, y);
				UIExtensions.HNIHBGAOAIH(component2, 0f);
				component2.raycastTarget = false;
				EDADANMDDCH.Add(component2);
			}
			HPOLPFPFNHL = 0;
			if (ICHBMJODKAB.Count > 0)
			{
				_lampOn.transform.localPosition = ICHBMJODKAB[0].transform.localPosition;
			}
		}

		public List<Button> GetLamps()
		{
			return ICHBMJODKAB;
		}

		public void SetCurrentZone(int index, string ABJMDKJHJCP)
		{
			if (ICHBMJODKAB.Count != 0)
			{
				_lampOn.transform.localPosition = ICHBMJODKAB[index].transform.localPosition;
				HPOLPFPFNHL = index;
				_locationName.SetAlias(ABJMDKJHJCP);
			}
		}

		public int GetCurrentLamp()
		{
			return HPOLPFPFNHL;
		}

		public void Flashing()
		{
			if (ALHECCDENMO > 0)
			{
				ALHECCDENMO--;
				return;
			}
			int iEKAFNFKBNE = MapGUI.JHLMDGBGGEP.IEKAFNFKBNE;
			int hPJHAIALGHN = MapGUI.JHLMDGBGGEP.HPJHAIALGHN;
			if (hPJHAIALGHN <= 0)
			{
				return;
			}
			if (MBIEJLCBCIM)
			{
				ChangeLampOpacity(JMPJJMHGIKF);
				if (GCCPALLIFBO <= iEKAFNFKBNE && HPCHHDHGDAA)
				{
					BDIPNCMGEEN = true;
				}
				if (GCCPALLIFBO <= 0)
				{
					MBIEJLCBCIM = false;
				}
			}
			if (BDIPNCMGEEN)
			{
				ChangeLampIndicatorOpacity(CGCLPOOGOGG);
				if (DDAMPCFEAKL <= iEKAFNFKBNE && LOGDIAPFANM)
				{
					MBIEJLCBCIM = true;
				}
				if (DDAMPCFEAKL <= 0)
				{
					BDIPNCMGEEN = false;
				}
			}
		}

		public void CheckOpenZones(List<ZoneScrollItem> LLOGFBNDHNF)
		{
			JMPJJMHGIKF.Clear();
			CGCLPOOGOGG.Clear();
			for (int i = 0; i < LLOGFBNDHNF.Count; i++)
			{
				Zone zone = LLOGFBNDHNF[i].get_Zone();
				if (!zone.AMBLIADMEOC())
				{
					if (!MapScene.IsZoneOpen(zone))
					{
						break;
					}
					if (MapScene.IsZoneHaveDontCompleteBattle(zone))
					{
						JMPJJMHGIKF.Add(ICHBMJODKAB[i]);
						CGCLPOOGOGG.Add(EDADANMDDCH[i]);
					}
				}
			}
		}

		public virtual void SetTouchEnabled(bool MINKNLEJMKF)
		{
			foreach (Button item in ICHBMJODKAB)
			{
				item.interactable = MINKNLEJMKF;
			}
		}

		public void SetLampsVisible(bool value)
		{
			foreach (Button item in ICHBMJODKAB)
			{
				if (item != null)
				{
					item.gameObject.SetActive(value);
				}
			}
			foreach (ResolutionImage item2 in EDADANMDDCH)
			{
				if (item2 != null)
				{
					item2.gameObject.SetActive(value);
				}
			}
			foreach (Button item3 in JMPJJMHGIKF)
			{
				if (item3 != null)
				{
					item3.gameObject.SetActive(value);
				}
			}
			foreach (ResolutionImage item4 in CGCLPOOGOGG)
			{
				if (item4 != null)
				{
					item4.gameObject.SetActive(value);
				}
			}
			if (_lampOn != null)
			{
				_lampOn.gameObject.SetActive(value);
			}
		}

		private void OnLampClicked(object data)
		{
			CallEvent(0, (int)data);
		}

		private void ChangeLampOpacity(List<Button> BBHOCFECAEM)
		{
			int hPJHAIALGHN = MapGUI.JHLMDGBGGEP.HPJHAIALGHN;
			if (hPJHAIALGHN <= 0)
			{
				return;
			}
			int num = 255 / hPJHAIALGHN;
			if (HPCHHDHGDAA)
			{
				GCCPALLIFBO -= num;
				if (GCCPALLIFBO <= 0)
				{
					GCCPALLIFBO = 0;
					HPCHHDHGDAA = false;
				}
			}
			else
			{
				GCCPALLIFBO += num;
				if (GCCPALLIFBO >= 255)
				{
					GCCPALLIFBO = 255;
					HPCHHDHGDAA = true;
					ALHECCDENMO = MapGUI.JHLMDGBGGEP.CPLJCIFJAGN;
				}
			}
			for (int i = 0; i < BBHOCFECAEM.Count; i++)
			{
				BBHOCFECAEM[i].targetGraphic.HNIHBGAOAIH(GCCPALLIFBO / 255);
			}
		}

		private void ChangeLampIndicatorOpacity(List<ResolutionImage> IBMGHIHLOHP)
		{
			int hPJHAIALGHN = MapGUI.JHLMDGBGGEP.HPJHAIALGHN;
			if (hPJHAIALGHN <= 0)
			{
				return;
			}
			int num = 255 / hPJHAIALGHN;
			if (LOGDIAPFANM)
			{
				DDAMPCFEAKL -= num;
				if (DDAMPCFEAKL <= 0)
				{
					DDAMPCFEAKL = 0;
					LOGDIAPFANM = false;
				}
			}
			else
			{
				DDAMPCFEAKL += num;
				if (DDAMPCFEAKL >= 255)
				{
					DDAMPCFEAKL = 255;
					LOGDIAPFANM = true;
					ALHECCDENMO = MapGUI.JHLMDGBGGEP.CPLJCIFJAGN;
				}
			}
			for (int i = 0; i < IBMGHIHLOHP.Count; i++)
			{
				IBMGHIHLOHP[i].gameObject.SetActive(true);
				UIExtensions.HNIHBGAOAIH(IBMGHIHLOHP[i], DDAMPCFEAKL / 255);
			}
		}
	}
}
