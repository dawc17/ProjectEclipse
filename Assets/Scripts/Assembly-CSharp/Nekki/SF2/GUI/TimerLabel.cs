using System;
using System.Collections.Generic;
using System.Text;
using Nekki.Utils;
using UnityEngine;

namespace Nekki.SF2.GUI
{
	public class TimerLabel : LabelAlias
	{
		public bool IsSeconds = true;

		public bool IsMinutes = true;

		public bool IsHours = true;

		public bool IsDays;

		public bool IsSecondsZero = true;

		public bool IsMinutesZero = true;

		public bool IsHoursZero = true;

		public bool IsDaysZero = true;

		public bool UseDaysDelimiter = true;

		public int SegmentsDate = -1;

		public string Delimiter = ":";

		private string JHJEPPMLLLK;

		public string DaysString = "d ";

		private string IPGNJBLGGDL;

		public string HoursString;

		private string HNNFMKCCFND;

		public string MinutesString;

		private string KBEBEBHFGAJ;

		public string SecondsString;

		private string NCPCLAFFPBE;

		[SerializeField]
		private long _currentTime;

		public string MKFBIJMBDGD
		{
			get
			{
				return get_DelimiterAlias();
			}
			set
			{
				set_DelimiterAlias(value);
			}
		}

		public string DFFANDFIDEK
		{
			get
			{
				return get_DaysStringAlias();
			}
			set
			{
				set_DaysStringAlias(value);
			}
		}

		public string KNCBNKIIGCJ
		{
			get
			{
				return get_HoursStringAlias();
			}
			set
			{
				set_HoursStringAlias(value);
			}
		}

		public string BPEEILFMBNK
		{
			get
			{
				return get_MinutesStringAlias();
			}
			set
			{
				set_MinutesStringAlias(value);
			}
		}

		public string GNMLNGLCNAI
		{
			get
			{
				return get_SecondsStringAlias();
			}
			set
			{
				set_SecondsStringAlias(value);
			}
		}

		public long INOFJEFCNEE
		{
			get
			{
				return get_CurrentTime();
			}
			set
			{
				set_CurrentTime(value);
			}
		}

		public string get_DelimiterAlias()
		{
			return JHJEPPMLLLK;
		}

		public void set_DelimiterAlias(string value)
		{
			JHJEPPMLLLK = value;
			Delimiter = LocalizationManager.GetString(JHJEPPMLLLK);
		}

		public string get_DaysStringAlias()
		{
			return IPGNJBLGGDL;
		}

		public void set_DaysStringAlias(string value)
		{
			IPGNJBLGGDL = value;
			DaysString = LocalizationManager.GetString(IPGNJBLGGDL);
		}

		public string get_HoursStringAlias()
		{
			return HNNFMKCCFND;
		}

		public void set_HoursStringAlias(string value)
		{
			HNNFMKCCFND = value;
			HoursString = LocalizationManager.GetString(HNNFMKCCFND);
		}

		public string get_MinutesStringAlias()
		{
			return KBEBEBHFGAJ;
		}

		public void set_MinutesStringAlias(string value)
		{
			KBEBEBHFGAJ = value;
			MinutesString = LocalizationManager.GetString(KBEBEBHFGAJ);
		}

		public string get_SecondsStringAlias()
		{
			return NCPCLAFFPBE;
		}

		public void set_SecondsStringAlias(string value)
		{
			NCPCLAFFPBE = value;
			SecondsString = LocalizationManager.GetString(NCPCLAFFPBE);
		}

		public long get_CurrentTime()
		{
			return _currentTime;
		}

		public void set_CurrentTime(long value)
		{
			if (_currentTime != value)
			{
				_currentTime = value;
				DDLIPEKBHED();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GlobalTimer.get_Instance().addEventListener(0, OnTimerTick);
			LocalizationManager.LKFNMDCLMCD(OCLBJLPOKLB);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			GlobalTimer.get_Instance().removeEventListener(0, OnTimerTick);
			LocalizationManager.FFIJPHDLPCF(OCLBJLPOKLB);
		}

		private void OCLBJLPOKLB()
		{
			if (!string.IsNullOrEmpty(IPGNJBLGGDL))
			{
				set_DaysStringAlias(IPGNJBLGGDL);
			}
			if (!string.IsNullOrEmpty(HNNFMKCCFND))
			{
				set_HoursStringAlias(HNNFMKCCFND);
			}
			if (!string.IsNullOrEmpty(KBEBEBHFGAJ))
			{
				set_MinutesStringAlias(KBEBEBHFGAJ);
			}
			if (!string.IsNullOrEmpty(NCPCLAFFPBE))
			{
				set_SecondsStringAlias(NCPCLAFFPBE);
			}
			if (!string.IsNullOrEmpty(JHJEPPMLLLK))
			{
				set_DelimiterAlias(JHJEPPMLLLK);
			}
		}

		public void OnTimerTick(object data)
		{
			if (0 < _currentTime)
			{
				_currentTime--;
				DDLIPEKBHED();
			}
		}

		private void DDLIPEKBHED()
		{
			set_text(GetTimeString(_currentTime, IsSeconds, IsMinutes, IsHours, IsDays, Delimiter, DaysString, UseDaysDelimiter, IsSecondsZero, IsMinutesZero, IsHoursZero, IsDaysZero, HoursString, MinutesString, SecondsString, SegmentsDate));
		}

		public static string GetTimeString(long time, bool OEIFDLECBPO = true, bool BGDHLOMPPJB = true, bool LMBINLCHPAL = true, bool ANLFBBLJMJH = false, string OBABIEFGCAK = ":", string CNINNGINAEN = "", bool INCHCBDKMIP = true, bool AOGNIMJAMMJ = true, bool DINFJGFHNEC = true, bool DJBAELOAHIC = true, bool FAFKECJEOIH = true, string GOOEIOKNPBK = "", string HCLLKPNBOBO = "", string OHKCBFJCINL = "", int IEFLCNGMHBM = 3)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(time);
			List<object> list = new List<object>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			bool flag = ANLFBBLJMJH && timeSpan.TotalDays >= 1.0 && IEFLCNGMHBM > num;
			if (flag)
			{
				num++;
			}
			bool flag2 = LMBINLCHPAL && timeSpan.TotalHours >= 1.0 && IEFLCNGMHBM > num;
			if (flag2)
			{
				num++;
			}
			bool flag3 = BGDHLOMPPJB && IEFLCNGMHBM > num;
			if (flag3)
			{
				num++;
			}
			bool flag4 = OEIFDLECBPO && IEFLCNGMHBM > num;
			int num2 = 0;
			if (flag)
			{
				list.Add(timeSpan.Days);
				stringBuilder.Append('{');
				stringBuilder.Append(num2++);
				stringBuilder.Append((!FAFKECJEOIH) ? ":D}" : ":D2}");
				stringBuilder.Append(CNINNGINAEN);
				if (INCHCBDKMIP && (flag2 || flag3 || flag4))
				{
					stringBuilder.Append(OBABIEFGCAK);
				}
			}
			if (flag2)
			{
				list.Add(timeSpan.Hours);
				stringBuilder.Append('{');
				stringBuilder.Append(num2++);
				stringBuilder.Append((!DJBAELOAHIC) ? ":D}" : ":D2}");
				stringBuilder.Append(GOOEIOKNPBK);
				if (flag3 || flag4)
				{
					stringBuilder.Append(OBABIEFGCAK);
				}
			}
			if (flag3)
			{
				list.Add(timeSpan.Minutes);
				stringBuilder.Append('{');
				stringBuilder.Append(num2++);
				stringBuilder.Append((!DINFJGFHNEC) ? ":D}" : ":D2}");
				stringBuilder.Append(HCLLKPNBOBO);
				if (flag4)
				{
					stringBuilder.Append(OBABIEFGCAK);
				}
			}
			if (flag4)
			{
				list.Add(timeSpan.Seconds);
				stringBuilder.Append('{');
				stringBuilder.Append(num2++);
				stringBuilder.Append((!AOGNIMJAMMJ) ? ":D}" : ":D2}");
				stringBuilder.Append(OHKCBFJCINL);
			}
			return string.Format(stringBuilder.ToString(), list.ToArray());
		}
	}
}
