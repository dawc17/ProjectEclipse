using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredUInt : IEquatable<ObscuredUInt>, IFormattable
	{
		private static uint cryptoKey = 240513u;

		private uint currentCryptoKey;

		private uint hiddenValue;

		private uint fakeValue;

		private bool inited;

		private ObscuredUInt(uint value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0u;
			inited = true;
		}

		public static void SetNewCryptoKey(uint CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static uint Encrypt(uint value)
		{
			return Encrypt(value, 0u);
		}

		public static uint Decrypt(uint value)
		{
			return Decrypt(value, 0u);
		}

		public static uint Encrypt(uint value, uint KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ KGBGENDIMBC;
		}

		public static uint Decrypt(uint value, uint KGBGENDIMBC)
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
			uint bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = (uint)UnityEngine.Random.Range(0, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public uint ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(uint ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private uint GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0u);
				fakeValue = 0u;
				inited = true;
			}
			uint num = Decrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0 && num != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return num;
		}

		public static implicit operator ObscuredUInt(uint value)
		{
			ObscuredUInt result = new ObscuredUInt(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator uint(ObscuredUInt value)
		{
			return value.GEKBGBJOMIA();
		}

		public static explicit operator ObscuredInt(ObscuredUInt value)
		{
			return (ObscuredInt)((int)value.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredUInt ALEAHDHGCJL(ObscuredUInt NILNDHEKNLJ)
		{
			uint bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredUInt DDKOKLNFNPB(ObscuredUInt NILNDHEKNLJ)
		{
			uint bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1;
			NILNDHEKNLJ.hiddenValue = Encrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredUInt))
			{
				return false;
			}
			return Equals((ObscuredUInt)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredUInt AOMLCBHAJJH)
		{
			if (currentCryptoKey == AOMLCBHAJJH.currentCryptoKey)
			{
				return hiddenValue == AOMLCBHAJJH.hiddenValue;
			}
			return Decrypt(hiddenValue, currentCryptoKey) == Decrypt(AOMLCBHAJJH.hiddenValue, AOMLCBHAJJH.currentCryptoKey);
		}

		public override string ToString()
		{
			return GEKBGBJOMIA().ToString();
		}

		public string ToString(string LBOHOKIBHOH)
		{
			return GEKBGBJOMIA().ToString(LBOHOKIBHOH);
		}

		public override int GetHashCode()
		{
			return GEKBGBJOMIA().GetHashCode();
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
