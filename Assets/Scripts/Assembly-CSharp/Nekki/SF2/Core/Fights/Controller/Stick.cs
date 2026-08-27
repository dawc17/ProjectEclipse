using System;
using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.SF2.Core.Fights.Controller
{
	public class Stick : SFMonoBehaviour<object>, IEventSystemHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		public enum GDMCBGPPGJM
		{
			OnStickBegan = 0,
			OnStickChange = 1,
			OnStickEnd = 2
		}

		[SerializeField]
		private Image _normalTexture;

		[SerializeField]
		private Image _selectedTexture;

		[SerializeField]
		private Image _normalController;

		[SerializeField]
		private Image _selectedController;

		[SerializeField]
		private Image _flashing;

		private FightCID COFEJAHLFBF;

		private float BNFDJKONDBP;

		private float ELNNNILDIFB;

		private float EJBALDKCFBB;

		private float HBHNJPMNMIM;

		private float OMMKBOAPGDP;

		private float FAFNDMLGDJI;

		private float FKBJMDLAOMM;

		private int HCMOIDIJNMD;

		private int DMEAFBMAGDH = 10;

		private bool CANIGBPEKFA;

		private bool MGNLBNLCDAI;

		private float PPGLPONABFM;

		private float JINLDLGFECA;

		private float BALIBHFOIFA;

		private float GAGPDEDMDPI;

		private float IKIAEPNIPEK;

		private float MJGLHIPDJCP;

		private float MILNPJLOFHG;

		private List<global::Pair<float, float>> LGNJNBJEAJE = new List<global::Pair<float, float>>();

		private bool JMKEKMFHKBG;

		private Vector2 PPBJCHEEOFB = default(Vector2);

		private void Start()
		{
			Init();
		}

		private void Update()
		{
			if (!CANIGBPEKFA || !_flashing)
			{
				return;
			}
			_flashing.color = new Color(_flashing.color.r, _flashing.color.g, _flashing.color.b, (float)HCMOIDIJNMD / 255f);
			if (MGNLBNLCDAI)
			{
				if (HCMOIDIJNMD < 250)
				{
					HCMOIDIJNMD += DMEAFBMAGDH;
					return;
				}
				MGNLBNLCDAI = false;
				if (HCMOIDIJNMD > 250)
				{
					HCMOIDIJNMD = 250;
				}
			}
			else if (HCMOIDIJNMD > 0)
			{
				HCMOIDIJNMD -= DMEAFBMAGDH;
			}
			else
			{
				MGNLBNLCDAI = true;
				if (HCMOIDIJNMD < 0)
				{
					HCMOIDIJNMD = 0;
				}
			}
		}

		public void Init()
		{
			PPGLPONABFM = AssemblyController.AMBFLNIFDHO();
			PPGLPONABFM = 55f;
			if (PPGLPONABFM < 0f)
			{
				PPGLPONABFM = 0f;
			}
			if (PPGLPONABFM > 90f)
			{
				PPGLPONABFM = 90f;
			}
			JINLDLGFECA = 90f - PPGLPONABFM;
			BALIBHFOIFA = PPGLPONABFM * (float)Math.PI / 180f;
			GAGPDEDMDPI = JINLDLGFECA * (float)Math.PI / 180f;
			IKIAEPNIPEK = Mathf.Cos(BALIBHFOIFA / 2f);
			MJGLHIPDJCP = Mathf.Sin(BALIBHFOIFA / 2f);
			MILNPJLOFHG = Mathf.Tan(GAGPDEDMDPI);
			NFGCDODNFOH();
			SetJoystickRadius(_selectedTexture.rectTransform.rect.width / 2f);
			SetStopRadius(FAFNDMLGDJI);
			SetSafeRadius(FAFNDMLGDJI / 2f);
			SetMovementRadius(FAFNDMLGDJI * AssemblyController.LJPECNLDCNO());
			SetMovementRadius(FAFNDMLGDJI * 0.5f);
			_selectedTexture.gameObject.SetActive(false);
			LHKEJOONODP(false);
			if (_flashing != null)
			{
				_flashing.color = new Color(_flashing.color.r, _flashing.color.g, _flashing.color.b, 0f);
				_flashing.gameObject.SetActive(CANIGBPEKFA);
			}
		}

		public void SetSafeRadius(float value)
		{
			BNFDJKONDBP = value;
			ELNNNILDIFB = BNFDJKONDBP * BNFDJKONDBP;
		}

		public float GetSafeRadius()
		{
			return BNFDJKONDBP;
		}

		public void SetMovementRadius(float value)
		{
			EJBALDKCFBB = value;
			HBHNJPMNMIM = EJBALDKCFBB * EJBALDKCFBB;
		}

		public float GetMovementRadius()
		{
			return EJBALDKCFBB;
		}

		public void SetStopRadius(float value)
		{
			OMMKBOAPGDP = value;
		}

		public float GetStopRadius()
		{
			return OMMKBOAPGDP;
		}

		public void SetJoystickRadius(float value)
		{
			FAFNDMLGDJI = value;
			FKBJMDLAOMM = FAFNDMLGDJI * FAFNDMLGDJI;
		}

		public float GetJoystickRadius()
		{
			return FAFNDMLGDJI;
		}

		public bool GetIsFlashing()
		{
			return CANIGBPEKFA;
		}

		public void SetIsFlashing(bool value)
		{
			if (CANIGBPEKFA != value)
			{
				CANIGBPEKFA = value;
				if ((bool)_flashing)
				{
					_flashing.gameObject.SetActive(value);
				}
				HCMOIDIJNMD = 0;
				MGNLBNLCDAI = true;
			}
		}

		public bool GetIsRising()
		{
			return MGNLBNLCDAI;
		}

		public void SetIsRising(bool value)
		{
			MGNLBNLCDAI = value;
		}

		public int GetOpacityCounter()
		{
			return HCMOIDIJNMD;
		}

		public void SetOpacityCounter(int value)
		{
			HCMOIDIJNMD = value;
		}

		public int GetFlashingSpeed()
		{
			return DMEAFBMAGDH;
		}

		public void SetFlashingSpeed(int value)
		{
			DMEAFBMAGDH = value;
		}

		public List<global::Pair<float, float>> GetQuadrantsAngles()
		{
			return LGNJNBJEAJE;
		}

		private float EDFBJIILGJB(Vector2 NAAPALOFBCI)
		{
			return NAAPALOFBCI.x * NAAPALOFBCI.x + NAAPALOFBCI.y * NAAPALOFBCI.y;
		}

		public void TT()
		{
			if (COFEJAHLFBF != FightCID.QuadrantZero)
			{
				FCKDDEIIPEN(GDMCBGPPGJM.OnStickChange, COFEJAHLFBF);
			}
		}

		public void OnPointerDown(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint);
			float num = EDFBJIILGJB(localPoint);
			if (num <= FKBJMDLAOMM)
			{
				if (num <= HBHNJPMNMIM)
				{
					JMKEKMFHKBG = true;
					PPBJCHEEOFB = localPoint;
					COFEJAHLFBF = FightCID.QuadrantZero;
					KAEKFLCGIOG(default(Vector2));
				}
				else
				{
					COFEJAHLFBF = GLPAFFMMHKC(localPoint);
					KAEKFLCGIOG(localPoint);
				}
				_normalTexture.gameObject.SetActive(false);
				_selectedTexture.gameObject.SetActive(true);
				if ((bool)_flashing)
				{
					_flashing.gameObject.SetActive(false);
				}
				LHKEJOONODP(true);
				FCKDDEIIPEN(GDMCBGPPGJM.OnStickBegan, COFEJAHLFBF);
			}
		}

		public void OnDrag(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint);
			if (JMKEKMFHKBG)
			{
				localPoint.x -= PPBJCHEEOFB.x;
				localPoint.y -= PPBJCHEEOFB.y;
			}
			KAEKFLCGIOG(localPoint);
			FightCID eCHINOPKGGI = GLPAFFMMHKC(localPoint);
			if (COFEJAHLFBF != eCHINOPKGGI)
			{
				FCKDDEIIPEN(GDMCBGPPGJM.OnStickEnd, COFEJAHLFBF);
			}
			COFEJAHLFBF = eCHINOPKGGI;
			FCKDDEIIPEN(GDMCBGPPGJM.OnStickChange, COFEJAHLFBF);
		}

		public void OnPointerUp(PointerEventData BHOLFGOGPCP)
		{
			JMKEKMFHKBG = false;
			LHKEJOONODP(false);
			if ((bool)_flashing)
			{
				_flashing.gameObject.SetActive(CANIGBPEKFA);
			}
			FCKDDEIIPEN(GDMCBGPPGJM.OnStickEnd, COFEJAHLFBF);
			COFEJAHLFBF = FightCID.QuadrantZero;
		}

		private void KAEKFLCGIOG(Vector2 DGEJJGMMODA)
		{
			DGEJJGMMODA = Vector2.ClampMagnitude(DGEJJGMMODA, OMMKBOAPGDP);
			_selectedController.transform.localPosition = DGEJJGMMODA;
		}

		private FightCID GLPAFFMMHKC(Vector2 NAAPALOFBCI)
		{
			FightCID eCHINOPKGGI = FightCID.QuadrantZero;
			float num = NAAPALOFBCI.x * IKIAEPNIPEK + NAAPALOFBCI.y * MJGLHIPDJCP;
			float num2 = NAAPALOFBCI.y * IKIAEPNIPEK - NAAPALOFBCI.x * MJGLHIPDJCP;
			if (EDFBJIILGJB(NAAPALOFBCI) < ELNNNILDIFB)
			{
				return FightCID.QuadrantZero;
			}
			bool flag = num >= 0f;
			bool flag2 = num2 >= 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (flag && flag2)
			{
				eCHINOPKGGI = FightCID.QuadrantUp;
				num3 = Mathf.Abs(num);
				num4 = Mathf.Abs(num2);
			}
			else if (flag && !flag2)
			{
				eCHINOPKGGI = FightCID.QuadrantUpForward;
				num3 = Mathf.Abs(num2);
				num4 = Mathf.Abs(num);
			}
			else if (!flag && !flag2)
			{
				eCHINOPKGGI = FightCID.QuadrantForward;
				num3 = Mathf.Abs(num);
				num4 = Mathf.Abs(num2);
			}
			else if (!flag && flag2)
			{
				eCHINOPKGGI = FightCID.QuadrantDownForward;
				num3 = Mathf.Abs(num2);
				num4 = Mathf.Abs(num);
			}
			bool flag3 = false;
			if (JINLDLGFECA != 90f)
			{
				float num5 = num3 * MILNPJLOFHG;
				if (num4 <= num5)
				{
					flag3 = true;
				}
			}
			else
			{
				flag3 = true;
			}
			switch (eCHINOPKGGI)
			{
			case FightCID.QuadrantUp:
				return (!flag3) ? FightCID.QuadrantUp : FightCID.QuadrantUpForward;
			case FightCID.QuadrantUpForward:
				return flag3 ? FightCID.QuadrantDownForward : FightCID.QuadrantForward;
			case FightCID.QuadrantForward:
				return flag3 ? FightCID.QuadrantDownBack : FightCID.QuadrantDown;
			case FightCID.QuadrantDownForward:
				return flag3 ? FightCID.QuadrantUpBack : FightCID.QuadrantBack;
			default:
				return FightCID.QuadrantZero;
			}
		}

		private void LHKEJOONODP(bool NMFDJAMAOHN)
		{
			_normalController.gameObject.SetActive(!NMFDJAMAOHN);
			_selectedController.gameObject.SetActive(NMFDJAMAOHN);
			_normalTexture.gameObject.SetActive(!NMFDJAMAOHN);
			_selectedTexture.gameObject.SetActive(NMFDJAMAOHN);
		}

		private void FCKDDEIIPEN(GDMCBGPPGJM DOPHKKGNAEF, FightCID KJPGKHJNOMC)
		{
			CBBEIGACPPD cBBEIGACPPD = new CBBEIGACPPD();
			cBBEIGACPPD.Index = 0;
			cBBEIGACPPD.KMOPCKPBHIA = KJPGKHJNOMC;
			CallEvent((int)DOPHKKGNAEF, cBBEIGACPPD);
		}

		private void NFGCDODNFOH()
		{
			float num = BALIBHFOIFA / 2f + GAGPDEDMDPI + BALIBHFOIFA;
			LGNJNBJEAJE.Clear();
			for (int i = 0; i < 8; i++)
			{
				float num2;
				if (i == 0)
				{
					num2 = num;
				}
				else
				{
					global::Pair<float, float> cCKLNOPEKHO = LGNJNBJEAJE[i - 1];
					num2 = cCKLNOPEKHO.Second;
				}
				float pOFHDGJAFMP = ((i % 2 != 0) ? (num2 + BALIBHFOIFA) : (num2 + GAGPDEDMDPI));
				global::Pair<float, float> item = new global::Pair<float, float>(num2, pOFHDGJAFMP);
				LGNJNBJEAJE.Add(item);
			}
		}
	}
}
