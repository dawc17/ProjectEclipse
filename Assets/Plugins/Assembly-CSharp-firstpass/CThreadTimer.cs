using System;
using System.Timers;

public class CThreadTimer
{
	public delegate void LMCCGEMFINC();

	private LMCCGEMFINC AMOEOOKHEEB;

	private Timer _myTimer;

	private bool _completed;

	public bool HDDGEKLLDJF
	{
		get
		{
			return FPFGBHOKEKO();
		}
	}

	public CThreadTimer(LMCCGEMFINC callback, float CPEBIEHDNIO, bool LGFKGJFHHCH)
	{
		AMOEOOKHEEB = (LMCCGEMFINC)Delegate.Combine(AMOEOOKHEEB, callback);
		_myTimer = new Timer();
		_myTimer.Elapsed += ONKHFODDMNL;
		_myTimer.Interval = CPEBIEHDNIO;
		if (LGFKGJFHHCH)
		{
			this.LGFKGJFHHCH();
		}
	}

	public bool FPFGBHOKEKO()
	{
		return _completed;
	}

	public void LGFKGJFHHCH()
	{
		_completed = false;
		_myTimer.Start();
	}

	public void LGFKGJFHHCH(float CPEBIEHDNIO)
	{
		_completed = false;
		_myTimer.Interval = CPEBIEHDNIO;
		_myTimer.Start();
	}

	public bool GFOEBDACOLN()
	{
		if (!_completed)
		{
			_myTimer.Enabled = false;
		}
		return !_completed;
	}

	public bool DIGLJEJBDPH()
	{
		if (!_completed)
		{
			_myTimer.Enabled = true;
		}
		return !_completed;
	}

	private void ONKHFODDMNL(object BBNKIBKPBLO, ElapsedEventArgs FOPOKALJIIJ)
	{
		_completed = false;
		AMOEOOKHEEB();
		_myTimer.Dispose();
	}
}
