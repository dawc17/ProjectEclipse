using System;
using Nekki.SF2.GUI;
using UnityEngine;

public class TextTimer : global::EventDispatcher<object>
{
	public class TimerDataStruct
	{
		public object Data;

		private TextTimer OIHKOMFCFME;

		public long Time
		{
			get
			{
				return CCCIFDLEMPI();
			}
			set
			{
				ABIELBGOLCA(value);
			}
		}

		public TimerDataStruct(TextTimer EDAIGLJNLJE, object data = null)
		{
			Data = data;
			OIHKOMFCFME = EDAIGLJNLJE;
		}

		public long CCCIFDLEMPI()
		{
			return OIHKOMFCFME.Time;
		}

		public void ABIELBGOLCA(long value)
		{
			OIHKOMFCFME.Time = value;
		}
	}

	public string HNECCLNDKJL = ":";

	public string DEJKIIKMGAO = "d";

	public string ELFDOAOLMOA = string.Empty;

	public bool CBCBKMHGLEF = true;

	public bool PMNNBLHOCPH = true;

	public bool KFMCBOHHFNH = true;

	public bool DAPJNFEGFJL;

	public bool NPODIGENMMO = true;

	public bool LHOKGJNFELC = true;

	public bool CLFKEPDADAM = true;

	public bool HAFGMOFEJGI = true;

	public bool DNLOOJOACNE = true;

	private TimerDataStruct MNOHGIGAMEB;

	public Color Color = Color.black;

	private object _data;

	public long Time;

	public Action<object> Delegate;

	private TimerLabel _timerLabel;

	public TextTimer(Action<object> ODDEOFKLIAG = null)
	{
		Delegate = ODDEOFKLIAG;
		MNOHGIGAMEB = new TimerDataStruct(this, _data);
	}

	public object CHIGLEKCFFN()
	{
		return _data;
	}

	public void set_Data(object value)
	{
		_data = value;
		MNOHGIGAMEB.Data = _data;
	}

	public TimerLabel EDAKEMEHFIC()
	{
		return _timerLabel;
	}

	public void set_Label(TimerLabel value)
	{
		_timerLabel = value;
		if (_timerLabel != null)
		{
			_timerLabel.set_CurrentTime(Time);
		}
	}

	public void JLPMOKPFECK()
	{
		if (Delegate != null)
		{
			Delegate(MNOHGIGAMEB);
		}
		if (_timerLabel != null)
		{
			_timerLabel.set_CurrentTime(Time);
		}
	}
}
