using System;
using System.Collections;
using UnityEngine;

public class Routiner : MonoBehaviour
{
	private static Routiner EDAPJLKMFPC;

	private Action FJHOJLPOBJA;

	private const float LMJAKBLNLHN = 0.0001f;

	public static void Init()
	{
		if (!EDAPJLKMFPC || !EDAPJLKMFPC.gameObject)
		{
			EDAPJLKMFPC = new GameObject("_routine").AddComponent<Routiner>();
			StaticObjectsManager.AddObject(EDAPJLKMFPC.gameObject, false);
		}
	}

	public static Coroutine Go(IEnumerator DCOLKHNLFNI)
	{
		Init();
		return EDAPJLKMFPC.StartCoroutine(DCOLKHNLFNI);
	}

	public static void Stop(Coroutine DCOLKHNLFNI)
	{
		if (!(EDAPJLKMFPC == null) && DCOLKHNLFNI != null)
		{
			EDAPJLKMFPC.StopCoroutine(DCOLKHNLFNI);
		}
	}

	public static void AddUpdate(Action IBODMPMJELJ)
	{
		Init();
		Routiner eDAPJLKMFPC = EDAPJLKMFPC;
		eDAPJLKMFPC.FJHOJLPOBJA = (Action)Delegate.Combine(eDAPJLKMFPC.FJHOJLPOBJA, IBODMPMJELJ);
	}

	private void Update()
	{
		if (EDAPJLKMFPC.FJHOJLPOBJA != null)
		{
			EDAPJLKMFPC.FJHOJLPOBJA();
		}
	}

	public static Coroutine GoDelayed(Action IBODMPMJELJ, float IHDMLLNEGIK)
	{
		Init();
		return EDAPJLKMFPC.StartCoroutine(EDAPJLKMFPC.CEKNIMELBPB(IBODMPMJELJ, IHDMLLNEGIK));
	}

	private IEnumerator CEKNIMELBPB(Action IBODMPMJELJ, float IHDMLLNEGIK)
	{
		IEnumerator enumerator = NFDPAGGCDNH(IHDMLLNEGIK);
		while (enumerator.MoveNext())
		{
			yield return enumerator.Current;
		}
		IBODMPMJELJ();
	}

	public static Coroutine GoDelayed(IEnumerator DCOLKHNLFNI, float IHDMLLNEGIK)
	{
		Init();
		return EDAPJLKMFPC.StartCoroutine(EDAPJLKMFPC.CEKNIMELBPB(DCOLKHNLFNI, IHDMLLNEGIK));
	}

	private IEnumerator CEKNIMELBPB(IEnumerator DCOLKHNLFNI, float IHDMLLNEGIK)
	{
		IEnumerator enumerator = NFDPAGGCDNH(IHDMLLNEGIK);
		while (enumerator.MoveNext())
		{
			yield return enumerator.Current;
		}
		while (DCOLKHNLFNI.MoveNext())
		{
			yield return DCOLKHNLFNI.Current;
		}
	}

	private static IEnumerator NFDPAGGCDNH(float IHDMLLNEGIK)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup <= realtimeSinceStartup + IHDMLLNEGIK + 0.0001f)
		{
			yield return null;
		}
	}
}
