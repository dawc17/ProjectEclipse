using System;
using UnityEngine;

namespace Nekki.Utils
{
	public class GlobalTimer : ExtentionBehaviour
	{
		public const int TICK = 0;

		private static GlobalTimer EDAPJLKMFPC;

		private static TimeSpan DGLAAIIIOLE;

		private static float MPAGBGEENIJ;

		private static DateTime JMIPAPNMNIP;

		private static bool AAMLMIEHEIO;

		private static Action NOIAHFNMDFC;

		private static Action GEKNEIFHPAI;

		private static bool IGNLAKOKBPN;

		private static bool MFCFEFAMOLE;

		private static bool ECBPAOEJPBI;

		private float DGAKCPDHKJM;

		public static GlobalTimer BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static DateTime HLBLKMPNKOO
		{
			get
			{
				return get_LocalizedNow();
			}
		}

		public static DateTime LAHJJHEOKAF
		{
			get
			{
				return get_Now();
			}
		}

		public static long NNOHILNKJEN
		{
			get
			{
				return get_GetTime();
			}
		}

		public static long LIGDBHAGCDG
		{
			get
			{
				return get_LocalTimeUTC();
			}
		}

		public static bool NGIJGICHDEG
		{
			get
			{
				return get_IsSynchronized();
			}
		}

		public static bool KNCJBAHIAGI
		{
			get
			{
				return get_IsRequestInProgress();
			}
		}

		public static bool GHACJDFEAGE
		{
			get
			{
				return get_IsLastRequestSuccessful();
			}
		}

		public static GlobalTimer get_Instance()
		{
			if (!EDAPJLKMFPC)
			{
				Init();
			}
			return EDAPJLKMFPC;
		}

		public static DateTime get_LocalizedNow()
		{
			return get_Now().ToLocalTime();
		}

		public static DateTime get_Now()
		{
			return JMIPAPNMNIP.AddSeconds(Time.unscaledTime - MPAGBGEENIJ);
		}

		public static long get_GetTime()
		{
			return ConvertToUnixTimestamp(get_Now());
		}

		public static long get_LocalTimeUTC()
		{
			return (long)Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
		}

		public static bool get_IsSynchronized()
		{
			return AAMLMIEHEIO;
		}

		public static bool get_IsRequestInProgress()
		{
			return IGNLAKOKBPN;
		}

		public static bool get_IsLastRequestSuccessful()
		{
			return MFCFEFAMOLE;
		}

		public static long ConvertToUnixTimestamp(DateTime CIODNJIEKKK)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long)Math.Floor((CIODNJIEKKK.ToUniversalTime() - dateTime).TotalSeconds);
		}

		public static void Init(bool GCPIOLHKMAI = false)
		{
			if (!EDAPJLKMFPC)
			{
				EDAPJLKMFPC = new GameObject("_timer").AddComponent<GlobalTimer>();
				UnityEngine.Object.DontDestroyOnLoad(EDAPJLKMFPC.get_gameObject());
			}
			DGLAAIIIOLE = default(TimeSpan);
			AAMLMIEHEIO = false;
			MFCFEFAMOLE = false;
			ECBPAOEJPBI = GCPIOLHKMAI;
			if (!ECBPAOEJPBI)
			{
				ServerTimeSync();
			}
		}

		public static void ServerTimeSync(Action AFMCMJDBDIN = null, Action onError = null)
		{
			// Local clock only; keep timer callbacks and elapsed-time gameplay working.
			IGNLAKOKBPN = true;
			NOIAHFNMDFC = AFMCMJDBDIN;
			GEKNEIFHPAI = onError;
			ANFPDNJJKGB(ConvertToUnixTimestamp(DateTime.UtcNow));
		}

		public static void ServerTimeExtended(long CFGPDFHPGJP)
		{
			DateTime jMIPAPNMNIP = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(CFGPDFHPGJP);
			jMIPAPNMNIP += timeSpan;
			JMIPAPNMNIP = jMIPAPNMNIP;
			MPAGBGEENIJ = Time.unscaledTime;
			DGLAAIIIOLE = JMIPAPNMNIP - DateTime.Now;
			AAMLMIEHEIO = true;
			IGNLAKOKBPN = false;
			MFCFEFAMOLE = true;
			if (NOIAHFNMDFC != null)
			{
				NOIAHFNMDFC();
				NOIAHFNMDFC = null;
				GEKNEIFHPAI = null;
			}
		}

		private static void ANFPDNJJKGB(long time)
		{
			JMIPAPNMNIP = UnixTimeStampToDateTime(time);
			MPAGBGEENIJ = Time.unscaledTime;
			DGLAAIIIOLE = JMIPAPNMNIP - DateTime.Now;
			AAMLMIEHEIO = true;
			IGNLAKOKBPN = false;
			MFCFEFAMOLE = true;
			if (NOIAHFNMDFC != null)
			{
				NOIAHFNMDFC();
				NOIAHFNMDFC = null;
				GEKNEIFHPAI = null;
			}
		}

		private static void JAIJHNAIKJE(object LIOGIBJBHAH)
		{
			JMIPAPNMNIP = DateTime.Now;
			MPAGBGEENIJ = Time.unscaledTime;
			DGLAAIIIOLE = JMIPAPNMNIP - DateTime.Now;
			AAMLMIEHEIO = false;
			IGNLAKOKBPN = false;
			MFCFEFAMOLE = false;
			if (GEKNEIFHPAI != null)
			{
				GEKNEIFHPAI();
				NOIAHFNMDFC = null;
				GEKNEIFHPAI = null;
			}
			AdvLog.CCOFFJPPAKC(LIOGIBJBHAH);
		}

		public static DateTime UnixTimeStampToDateTime(double NNBJNDAFEDH)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(NNBJNDAFEDH);
		}

		public static DateTime UnixTimeStampToDateTimeLocal(double NNBJNDAFEDH)
		{
			return UnixTimeStampToDateTime(NNBJNDAFEDH).ToLocalTime();
		}

		private void Update()
		{
			if (DGAKCPDHKJM + 1f < Time.unscaledTime)
			{
				DGAKCPDHKJM = Time.unscaledTime;
				// Invoke on the live component. During scene/application teardown the
				// static singleton is cleared before Unity delivers the final Update.
				callEvent(0, get_Now());
			}
		}

		private void OnApplicationPause(bool OHCAIDHJHKC)
		{
			if (!OHCAIDHJHKC && !ECBPAOEJPBI)
			{
				ServerTimeSync();
			}
		}

		private new void OnDestroy()
		{
			EDAPJLKMFPC = null;
			StopAllCoroutines();
			base.OnDestroy();
		}
	}
}
