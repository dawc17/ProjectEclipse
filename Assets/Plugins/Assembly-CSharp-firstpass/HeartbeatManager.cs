using System;
using System.Collections.Generic;

public sealed class HeartbeatManager
{
	private List<IHeartbeat> KBHEHJBEGNK = new List<IHeartbeat>();

	private IHeartbeat[] KHPPOHKBABK;

	private DateTime LastUpdate = DateTime.MinValue;

	public void ELAHFBCGAGL(IHeartbeat JJACIFLDCAE)
	{
		lock (KBHEHJBEGNK)
		{
			if (!KBHEHJBEGNK.Contains(JJACIFLDCAE))
			{
				KBHEHJBEGNK.Add(JJACIFLDCAE);
			}
		}
	}

	public void HKMBDKKHPCB(IHeartbeat JJACIFLDCAE)
	{
		lock (KBHEHJBEGNK)
		{
			KBHEHJBEGNK.Remove(JJACIFLDCAE);
		}
	}

	public void JLPMOKPFECK()
	{
		if (LastUpdate == DateTime.MinValue)
		{
			LastUpdate = DateTime.UtcNow;
			return;
		}
		TimeSpan oJOKANCMPLG = DateTime.UtcNow - LastUpdate;
		LastUpdate = DateTime.UtcNow;
		int num = 0;
		lock (KBHEHJBEGNK)
		{
			if (KHPPOHKBABK == null || KHPPOHKBABK.Length < KBHEHJBEGNK.Count)
			{
				Array.Resize(ref KHPPOHKBABK, KBHEHJBEGNK.Count);
			}
			KBHEHJBEGNK.CopyTo(0, KHPPOHKBABK, 0, KBHEHJBEGNK.Count);
			num = KBHEHJBEGNK.Count;
		}
		for (int i = 0; i < num; i++)
		{
			try
			{
				KHPPOHKBABK[i].OnHeartbeatUpdate(oJOKANCMPLG);
			}
			catch
			{
			}
		}
	}
}
