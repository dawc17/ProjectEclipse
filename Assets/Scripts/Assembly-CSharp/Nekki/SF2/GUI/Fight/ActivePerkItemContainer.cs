using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class ActivePerkItemContainer : MonoBehaviour
	{
		private const float MOVE_SPEED = 10f;

		private float HMDCLMMEHPF;

		private float LBLANBELAPG;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string JCJNNJPNPJE;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float EHAJOKOMBPK;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool NCGLMKOGBCB;

		private List<ActivePerkItem> _activePerks = new List<ActivePerkItem>();

		public string GJONJADIAJM
		{
			get
			{
				return get_Stack();
			}
			set
			{
				set_Stack(value);
			}
		}

		public float HIMIJMKFIPH
		{
			get
			{
				return get_FinishPosX();
			}
			set
			{
				set_FinishPosX(value);
			}
		}

		public bool IEEIFCFIGAD
		{
			get
			{
				return get_NeedDelete();
			}
			private set
			{
				set_NeedDelete(value);
			}
		}

		public string get_Stack()
		{
			return JCJNNJPNPJE;
		}

		public void set_Stack(string value)
		{
			JCJNNJPNPJE = value;
		}

		public float get_FinishPosX()
		{
			return EHAJOKOMBPK;
		}

		public void set_FinishPosX(float value)
		{
			EHAJOKOMBPK = value;
		}

		public bool get_NeedDelete()
		{
			return NCGLMKOGBCB;
		}

		private void set_NeedDelete(bool value)
		{
			NCGLMKOGBCB = value;
		}

		public void Init(float HLBMDDOPKKL = 0f, float ELAKEOGEDPN = 0f)
		{
			HMDCLMMEHPF = HLBMDDOPKKL;
			LBLANBELAPG = ELAKEOGEDPN;
			set_FinishPosX(0f);
			set_NeedDelete(false);
		}

		public void AddActivePerk(ActivePerkItem AEFFHJGMNFI)
		{
			AEFFHJGMNFI.transform.SetParent(base.transform, false);
			AEFFHJGMNFI.set_PulseCount(0);
			_activePerks.Add(AEFFHJGMNFI);
			_activePerks.Sort();
			ActivePerkItem activePerkItem = _activePerks[_activePerks.Count - 1];
			activePerkItem.set_PulseCount(activePerkItem.get_PulseCount() + 1);
			RectTransform rectTransform = base.transform as RectTransform;
			RectTransform rectTransform2 = AEFFHJGMNFI.transform as RectTransform;
			if (rectTransform != null && rectTransform2 != null)
			{
				rectTransform.sizeDelta = rectTransform2.sizeDelta;
			}
		}

		private void JNBECGKCNBB()
		{
			Vector2 vector = base.transform.localPosition;
			if (vector.x != get_FinishPosX())
			{
				bool flag = get_FinishPosX() < vector.x;
				vector.x += ((!flag) ? 10f : (-10f));
				if (flag != get_FinishPosX() < vector.x)
				{
					vector.x = get_FinishPosX();
				}
				base.transform.localPosition = vector;
			}
		}

		public void Destroy()
		{
			set_NeedDelete(true);
			base.gameObject.SetActive(false);
			Object.Destroy(base.gameObject);
		}

		public void Render()
		{
			JNBECGKCNBB();
			List<ActivePerkItem> list = new List<ActivePerkItem>();
			foreach (ActivePerkItem item in _activePerks)
			{
				item.Render();
				if (item.get_NeedDelete())
				{
					list.Add(item);
				}
			}
			list.ForEach((ActivePerkItem DHDMNHCIPEH) =>
			{
				_activePerks.Remove(DHDMNHCIPEH);
			});
			if (_activePerks.Count == 0)
			{
				Destroy();
			}
		}
	}
}
