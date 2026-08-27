using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredLong : IEquatable<ObscuredLong>, IFormattable
	{
		private static long cryptoKey = 444442L;

		[SerializeField]
		private long currentCryptoKey;

		[SerializeField]
		private long hiddenValue;

		[SerializeField]
		private long fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredLong(long value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0L;
			inited = true;
		}

		public static void SetNewCryptoKey(long CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static long Encrypt(long value)
		{
			return Encrypt(value, 0L);
		}

		public static long Decrypt(long value)
		{
			return Decrypt(value, 0L);
		}

		public static long Encrypt(long value, long KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ KGBGENDIMBC;
		}

		public static long Decrypt(long value, long KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ KGBGENDIMBC;
		}

		public void PKOKLDGAPEI()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = Encrypt(GEKBGBJOMIA(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void GMCADPGOCHM()
		{
			long bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public long ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(long ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private long GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0L);
				fakeValue = 0L;
				inited = true;
			}
			long num = Decrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0 && num != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return num;
		}

		public static implicit operator ObscuredLong(long value)
		{
			ObscuredLong result = new ObscuredLong(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator long(ObscuredLong value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredLong ALEAHDHGCJL(ObscuredLong NILNDHEKNLJ)
		{
			long bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredLong DDKOKLNFNPB(ObscuredLong NILNDHEKNLJ)
		{
			long bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredLong))
			{
				return false;
			}
			return Equals((ObscuredLong)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredLong AOMLCBHAJJH)
		{
			if (currentCryptoKey == AOMLCBHAJJH.currentCryptoKey)
			{
				return hiddenValue == AOMLCBHAJJH.hiddenValue;
			}
			return Decrypt(hiddenValue, currentCryptoKey) == Decrypt(AOMLCBHAJJH.hiddenValue, AOMLCBHAJJH.currentCryptoKey);
		}

		public override int GetHashCode()
		{
			return GEKBGBJOMIA().GetHashCode();
		}

		public override string ToString()
		{
			return GEKBGBJOMIA().ToString();
		}

		public string ToString(string LBOHOKIBHOH)
		{
			return GEKBGBJOMIA().ToString(LBOHOKIBHOH);
		}

		public string ToString(IFormatProvider EEGMFLOPLLH)
		{
			return GEKBGBJOMIA().ToString(EEGMFLOPLLH);
		}

		public string ToString(string LBOHOKIBHOH, IFormatProvider EEGMFLOPLLH)
		{
			return GEKBGBJOMIA().ToString(LBOHOKIBHOH, EEGMFLOPLLH);
		}
	}
}
