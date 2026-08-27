using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class StaticObjectsManager : MonoBehaviour
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnApplicationQuitEvent;

	private static StaticObjectsManager EDAPJLKMFPC;

	private Transform _transform;

	private HashSet<GameObject> _staticObjects;

	protected static StaticObjectsManager BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public static event Action NCPKFJJOLKC
	{
		add
		{
			add_OnApplicationQuitEvent(value);
		}
		remove
		{
			remove_OnApplicationQuitEvent(value);
		}
	}

	public static void add_OnApplicationQuitEvent(Action value)
	{
		Action action = OnApplicationQuitEvent;
		Action action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnApplicationQuitEvent, (Action)Delegate.Combine(action2, value), action);
		}
		while ((object)action != action2);
	}

	public static void remove_OnApplicationQuitEvent(Action value)
	{
		Action action = OnApplicationQuitEvent;
		Action action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnApplicationQuitEvent, (Action)Delegate.Remove(action2, value), action);
		}
		while ((object)action != action2);
	}

	protected static StaticObjectsManager ELEBLBJKDBI()
	{
		if (EDAPJLKMFPC == null)
		{
			GameObject gameObject = new GameObject("STATIC_OBJECTS");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			EDAPJLKMFPC = gameObject.AddComponent<StaticObjectsManager>();
			EDAPJLKMFPC._transform = gameObject.transform;
			EDAPJLKMFPC._staticObjects = new HashSet<GameObject>();
		}
		return EDAPJLKMFPC;
	}

	public static void AddObject(GameObject ODMLDMOAOLN, bool DLIBCKLEOFM = true)
	{
		if (!ELEBLBJKDBI()._staticObjects.Contains(ODMLDMOAOLN))
		{
			if (DLIBCKLEOFM)
			{
				ODMLDMOAOLN.transform.parent = ELEBLBJKDBI()._transform;
			}
			else
			{
				UnityEngine.Object.DontDestroyOnLoad(ODMLDMOAOLN);
			}
			if (ODMLDMOAOLN.name[0] != '_')
			{
				ODMLDMOAOLN.name = "_" + ODMLDMOAOLN.name;
			}
			ELEBLBJKDBI()._staticObjects.Add(ODMLDMOAOLN);
		}
	}

	public static void RemoveObject(GameObject ODMLDMOAOLN)
	{
		if (ELEBLBJKDBI()._staticObjects.Contains(ODMLDMOAOLN))
		{
			ELEBLBJKDBI()._staticObjects.Remove(ODMLDMOAOLN);
			UnityEngine.Object.Destroy(ODMLDMOAOLN);
		}
	}

	public static void Clear()
	{
		foreach (GameObject item in ELEBLBJKDBI()._staticObjects)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item);
			}
		}
		if ((bool)ELEBLBJKDBI())
		{
			UnityEngine.Object.Destroy(ELEBLBJKDBI().gameObject);
		}
		EDAPJLKMFPC = null;
	}

	private void OnApplicationQuit()
	{
		OnApplicationQuitEvent.FEEGJDJIFEF();
	}
}
