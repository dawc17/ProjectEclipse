using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ExtentionBehaviour : MonoBehaviour
{
	public class CallEventArgs
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object JFELNKAOJEO;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int MJDIHAAHDIC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object NKLJPKKFKEH;

		public int MOFKKABEFEB
		{
			get
			{
				return EMCEPDNKAPK();
			}
			private set
			{
				set_Event(value);
			}
		}

		public object Content
		{
			get
			{
				return DIPHGGNBBFM();
			}
			private set
			{
				EOBBNCNJKCP(value);
			}
		}

		public CallEventArgs(int IILOLJJLLGH, object DMNBDBJNKME, object target)
		{
			set_Target(target);
			EOBBNCNJKCP(DMNBDBJNKME);
			set_Event(IILOLJJLLGH);
		}

		public object IDPLAGOELKE()
		{
			return JFELNKAOJEO;
		}

		private void set_Target(object value)
		{
			JFELNKAOJEO = value;
		}

		public int EMCEPDNKAPK()
		{
			return MJDIHAAHDIC;
		}

		private void set_Event(int value)
		{
			MJDIHAAHDIC = value;
		}

		public object DIPHGGNBBFM()
		{
			return NKLJPKKFKEH;
		}

		private void EOBBNCNJKCP(object value)
		{
			NKLJPKKFKEH = value;
		}

		public CallEventArgs SwitchTarget(object target)
		{
			set_Target(target);
			return this;
		}
	}

	public enum FBIAIDGAHHO
	{
		SimpeEvent = 0
	}

	private GameObject _gameObject;

	private Transform _transform;

	private Renderer _renderer;

	private Animator _animator;

	private readonly Dictionary<int, List<Action<CallEventArgs>>> GJKNDHMHLCL = new Dictionary<int, List<Action<CallEventArgs>>>();

	public GameObject DONFADGOEDE
	{
		get
		{
			return get_gameObject();
		}
	}

	public Transform KGOIHPPNFGC
	{
		get
		{
			return get_transform();
		}
	}

	public Renderer KHMOPNAHLNK
	{
		get
		{
			return get_renderer();
		}
	}

	public Animator PIIKKKEANMK
	{
		get
		{
			return get_animator();
		}
	}

	protected void Log(object LIOGIBJBHAH, UnityEngine.Object BBNKIBKPBLO = null)
	{
		AdvLog.Log(LIOGIBJBHAH, BBNKIBKPBLO ?? this);
	}

	protected void LOPHFKMOPAA(object LIOGIBJBHAH, UnityEngine.Object BBNKIBKPBLO = null)
	{
		AdvLog.LOPHFKMOPAA(LIOGIBJBHAH, BBNKIBKPBLO ?? this);
	}

	protected void CCOFFJPPAKC(object LIOGIBJBHAH, UnityEngine.Object BBNKIBKPBLO = null)
	{
		AdvLog.CCOFFJPPAKC(LIOGIBJBHAH, BBNKIBKPBLO ?? this);
	}

	protected void LogException(Exception MPFFFAOGBJE, UnityEngine.Object BBNKIBKPBLO = null)
	{
		AdvLog.LogException(MPFFFAOGBJE, BBNKIBKPBLO ?? this);
	}

	public GameObject get_gameObject()
	{
		if (!_gameObject)
		{
			_gameObject = base.gameObject;
		}
		return _gameObject;
	}

	public Transform get_transform()
	{
		if (!_transform)
		{
			_transform = base.transform;
		}
		return _transform;
	}

	public Renderer get_renderer()
	{
		if (!_renderer)
		{
			_renderer = GetComponent<Renderer>();
		}
		return _renderer;
	}

	public Animator get_animator()
	{
		if (!_animator)
		{
			_animator = GetComponent<Animator>();
		}
		return _animator;
	}

	protected void Invoke(Action IBODMPMJELJ, float GNAONAPDDLD)
	{
		StartCoroutine(InvokeActionRoutine(IBODMPMJELJ, GNAONAPDDLD));
	}

	private IEnumerator InvokeActionRoutine(Action IBODMPMJELJ, float GNAONAPDDLD)
	{
		yield return new WaitForSeconds(GNAONAPDDLD);
		IBODMPMJELJ();
	}

	public void addEventListener(int IILOLJJLLGH, Action<CallEventArgs> callback)
	{
		if (!GJKNDHMHLCL.ContainsKey(IILOLJJLLGH))
		{
			GJKNDHMHLCL.Add(IILOLJJLLGH, new List<Action<CallEventArgs>>());
		}
		GJKNDHMHLCL[IILOLJJLLGH].Add(callback);
	}

	public void addEventListener(int[] IILOLJJLLGH, Action<CallEventArgs> callback)
	{
		for (int i = 0; i < IILOLJJLLGH.Length; i++)
		{
			addEventListener(IILOLJJLLGH[i], callback);
		}
	}

	private void ELCIDNJGFHP()
	{
		GJKNDHMHLCL.Clear();
	}

	public void removeEvent(int IILOLJJLLGH)
	{
		if (GJKNDHMHLCL.ContainsKey(IILOLJJLLGH))
		{
			GJKNDHMHLCL.Remove(IILOLJJLLGH);
		}
	}

	public void removeEventListener(int IILOLJJLLGH, Action<CallEventArgs> callback)
	{
		if (GJKNDHMHLCL.ContainsKey(IILOLJJLLGH))
		{
			while (GJKNDHMHLCL[IILOLJJLLGH].Contains(callback))
			{
				GJKNDHMHLCL[IILOLJJLLGH].Remove(callback);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		ELCIDNJGFHP();
	}

	public void callEvent(int IILOLJJLLGH, object DMNBDBJNKME = null)
	{
		if (!GJKNDHMHLCL.ContainsKey(IILOLJJLLGH))
		{
			return;
		}
		List<Action<CallEventArgs>> list = new List<Action<CallEventArgs>>(GJKNDHMHLCL[IILOLJJLLGH]);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && GJKNDHMHLCL.ContainsKey(IILOLJJLLGH) && GJKNDHMHLCL[IILOLJJLLGH].Contains(list[i]))
			{
				list[i](new CallEventArgs(IILOLJJLLGH, DMNBDBJNKME, this));
			}
		}
	}
}
