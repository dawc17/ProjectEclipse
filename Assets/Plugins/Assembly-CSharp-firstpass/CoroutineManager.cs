using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
	private class LEPFPPAGHCO
	{
		private static Dictionary<IEnumerator, LEPFPPAGHCO> _Coroutines = new Dictionary<IEnumerator, LEPFPPAGHCO>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IEnumerator LCHPADNHGAC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool MDLALOPMKHF;

		public IEnumerator BCLGCMNJMKD
		{
			get
			{
				return FFHBAIFLBMN();
			}
			private set
			{
				set_Routine(value);
			}
		}

		public bool OEDPHHDKECI
		{
			get
			{
				return NMACGEJHPDN();
			}
			private set
			{
				set_IsRunning(value);
			}
		}

		public object BLOOLFFMKFI
		{
			get
			{
				return AOJJOEHEPGM();
			}
		}

		public LEPFPPAGHCO(IEnumerator BBMAOMICECF)
		{
			set_Routine(BBMAOMICECF);
			set_IsRunning(true);
			_Coroutines.Add(BBMAOMICECF, this);
		}

		public static LEPFPPAGHCO EMKADAPENNE(IEnumerator BBMAOMICECF)
		{
			LEPFPPAGHCO value = null;
			_Coroutines.TryGetValue(BBMAOMICECF, out value);
			return value;
		}

		public IEnumerator FFHBAIFLBMN()
		{
			return LCHPADNHGAC;
		}

		private void set_Routine(IEnumerator value)
		{
			LCHPADNHGAC = value;
		}

		public bool NMACGEJHPDN()
		{
			return MDLALOPMKHF;
		}

		private void set_IsRunning(bool value)
		{
			MDLALOPMKHF = value;
		}

		public bool PCCMLADDNDG()
		{
			if (FFHBAIFLBMN() != null && FFHBAIFLBMN().MoveNext())
			{
				return true;
			}
			Stop();
			return false;
		}

		public void Stop()
		{
			if (NMACGEJHPDN())
			{
				set_IsRunning(false);
				_Coroutines.Remove(FFHBAIFLBMN());
			}
		}

		public object AOJJOEHEPGM()
		{
			return FFHBAIFLBMN().Current;
		}
	}

	private static CoroutineManager _Current;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool PGAHCGBPNDB;

	public static CoroutineManager BLOOLFFMKFI
	{
		get
		{
			return get_Current();
		}
	}

	public bool FPJLHEMGNNB
	{
		get
		{
			return get_IsPaused();
		}
		private set
		{
			AHIEFDIHONK(value);
		}
	}

	public static CoroutineManager get_Current()
	{
		if (_Current == null)
		{
			_Current = new GameObject("[CoroutineManager]").AddComponent<CoroutineManager>();
			Object.DontDestroyOnLoad(_Current.gameObject);
		}
		return _Current;
	}

	public bool get_IsPaused()
	{
		return PGAHCGBPNDB;
	}

	private void AHIEFDIHONK(bool value)
	{
		PGAHCGBPNDB = value;
	}

	public void StartRoutine(IEnumerator BBMAOMICECF)
	{
		LEPFPPAGHCO cCCLFIBGGDD = new LEPFPPAGHCO(BBMAOMICECF);
		StartCoroutine(NAONPKJJKNH(cCCLFIBGGDD));
	}

	public void StopRoutine(IEnumerator BBMAOMICECF)
	{
		LEPFPPAGHCO lEPFPPAGHCO = LEPFPPAGHCO.EMKADAPENNE(BBMAOMICECF);
		if (lEPFPPAGHCO != null)
		{
			lEPFPPAGHCO.Stop();
		}
		StopCoroutine(BBMAOMICECF);
	}

	private IEnumerator NAONPKJJKNH(LEPFPPAGHCO CCCLFIBGGDD)
	{
		yield return null;
		while (CCCLFIBGGDD.NMACGEJHPDN())
		{
			if (get_IsPaused())
			{
				yield return null;
			}
			else if (CCCLFIBGGDD.PCCMLADDNDG())
			{
				yield return CCCLFIBGGDD.AOJJOEHEPGM();
			}
		}
	}

	private void Awake()
	{
		AHIEFDIHONK(false);
	}

	private void OnApplicationPause(bool OIBJJLBCEHA)
	{
		AHIEFDIHONK(OIBJJLBCEHA);
	}
}
