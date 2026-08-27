using System.Diagnostics;
using UnityEngine;

public class WaitForTime : CustomYieldInstruction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float OCOBNPGODHJ;

	public float BEOBDJHNHIO
	{
		get
		{
			return FJKGKLJGIJI();
		}
		private set
		{
			DKLGPGDJPGO(value);
		}
	}

	public WaitForTime(float KLOJJKNBHCL)
	{
		DKLGPGDJPGO(KLOJJKNBHCL);
	}

	public float FJKGKLJGIJI()
	{
		return OCOBNPGODHJ;
	}

	private void DKLGPGDJPGO(float value)
	{
		OCOBNPGODHJ = value;
	}

	public override bool keepWaiting
	{
		get
		{
			if (!CoroutineManager.get_Current().get_IsPaused())
			{
				DKLGPGDJPGO(FJKGKLJGIJI() - Time.deltaTime);
			}
			return FJKGKLJGIJI() >= 1E-07f;
		}
	}
}
