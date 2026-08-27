using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredULong : IEquatable<ObscuredULong>, IFormattable
	{
		private static ulong cryptoKey = 444443uL;

		private ulong currentCryptoKey;

		private ulong hiddenValue;

		private ulong fakeValue;

		private bool inited;

		private ObscuredULong(ulong value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0uL;
			inited = true;
		}

		public static void SetNewCryptoKey(ulong CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static ulong Encrypt(ulong value)
		{
			return Encrypt(value, 0uL);
		}

		public static ulong Decrypt(ulong value)
		{
			return Decrypt(value, 0uL);
		}

		public static ulong Encrypt(ulong value, ulong KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ KGBGENDIMBC;
		}

		public static ulong Decrypt(ulong value, ulong KGBGENDIMBC)
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
			ulong bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = (ulong)UnityEngine.Random.Range(0, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public ulong ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(ulong ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private ulong GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0uL);
				fakeValue = 0uL;
				inited = true;
			}
			ulong num = Decrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0 && num != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return num;
		}

		public static implicit operator ObscuredULong(ulong value)
		{
			ObscuredULong result = new ObscuredULong(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ulong(ObscuredULong value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredULong ALEAHDHGCJL(ObscuredULong NILNDHEKNLJ)
		{
			ulong bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredULong DDKOKLNFNPB(ObscuredULong NILNDHEKNLJ)
		{
			ulong bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredULong))
			{
				return false;
			}
			return Equals((ObscuredULong)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredULong AOMLCBHAJJH)
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
