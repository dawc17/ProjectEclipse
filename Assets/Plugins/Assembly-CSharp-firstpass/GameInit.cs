using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public abstract class GameInit
{
	public delegate void KNHFNPECPED();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static KNHFNPECPED InitializeDone;

	public static event KNHFNPECPED GACEDPMJOIC
	{
		add
		{
			BEFDGPNJIDH(value);
		}
		remove
		{
			NBAODLKAKKE(value);
		}
	}

	public static void BEFDGPNJIDH(KNHFNPECPED value)
	{
		KNHFNPECPED kNHFNPECPED = InitializeDone;
		KNHFNPECPED kNHFNPECPED2;
		do
		{
			kNHFNPECPED2 = kNHFNPECPED;
			kNHFNPECPED = Interlocked.CompareExchange(ref InitializeDone, (KNHFNPECPED)Delegate.Combine(kNHFNPECPED2, value), kNHFNPECPED);
		}
		while ((object)kNHFNPECPED != kNHFNPECPED2);
	}

	public static void NBAODLKAKKE(KNHFNPECPED value)
	{
		KNHFNPECPED kNHFNPECPED = InitializeDone;
		KNHFNPECPED kNHFNPECPED2;
		do
		{
			kNHFNPECPED2 = kNHFNPECPED;
			kNHFNPECPED = Interlocked.CompareExchange(ref InitializeDone, (KNHFNPECPED)Delegate.Remove(kNHFNPECPED2, value), kNHFNPECPED);
		}
		while ((object)kNHFNPECPED != kNHFNPECPED2);
	}

	private static void COLMDNAPFKJ()
	{
		KNHFNPECPED initializeDone = InitializeDone;
		if (initializeDone != null)
		{
			initializeDone();
		}
	}

	protected void IMMENGDGOOC()
	{
		COLMDNAPFKJ();
	}

	public virtual void ELAHFBCGAGL(params Action[] AFENHJFICNN)
	{
		foreach (Action action in AFENHJFICNN)
		{
			Action IBODMPMJELJ = action;
			BEFDGPNJIDH(() =>
			{
				IBODMPMJELJ();
			});
		}
	}

	public virtual void EHAJODIAFEG()
	{
		SF2DisplayFrameRate.Apply();
		IMMENGDGOOC();
	}

	public abstract void Init(params Action[] AFENHJFICNN);
}
