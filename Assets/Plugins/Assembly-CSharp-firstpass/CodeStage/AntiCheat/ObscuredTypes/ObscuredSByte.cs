using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredSByte : IEquatable<ObscuredSByte>, IFormattable
	{
		private static sbyte cryptoKey = 112;

		private sbyte currentCryptoKey;

		private sbyte hiddenValue;

		private sbyte fakeValue;

		private bool inited;

		private ObscuredSByte(sbyte value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(sbyte CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static sbyte EncryptDecrypt(sbyte value)
		{
			return EncryptDecrypt(value, 0);
		}

		public static sbyte EncryptDecrypt(sbyte value, sbyte KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				return (sbyte)(value ^ cryptoKey);
			}
			return (sbyte)(value ^ KGBGENDIMBC);
		}

		public void PKOKLDGAPEI()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = EncryptDecrypt(GEKBGBJOMIA(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void GMCADPGOCHM()
		{
			sbyte bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = (sbyte)UnityEngine.Random.Range(-128, 127);
			hiddenValue = EncryptDecrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public sbyte ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(sbyte ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private sbyte GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}
			sbyte b = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0 && b != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return b;
		}

		public static implicit operator ObscuredSByte(sbyte value)
		{
			ObscuredSByte result = new ObscuredSByte(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator sbyte(ObscuredSByte value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredSByte ALEAHDHGCJL(ObscuredSByte NILNDHEKNLJ)
		{
			sbyte bAINMLLIKOL = (sbyte)(NILNDHEKNLJ.GEKBGBJOMIA() + 1);
			NILNDHEKNLJ.hiddenValue = EncryptDecrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredSByte DDKOKLNFNPB(ObscuredSByte NILNDHEKNLJ)
		{
			sbyte bAINMLLIKOL = (sbyte)(NILNDHEKNLJ.GEKBGBJOMIA() - 1);
			NILNDHEKNLJ.hiddenValue = EncryptDecrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredSByte))
			{
				return false;
			}
			return Equals((ObscuredSByte)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredSByte AOMLCBHAJJH)
		{
			if (currentCryptoKey == AOMLCBHAJJH.currentCryptoKey)
			{
				return hiddenValue == AOMLCBHAJJH.hiddenValue;
			}
			return EncryptDecrypt(hiddenValue, currentCryptoKey) == EncryptDecrypt(AOMLCBHAJJH.hiddenValue, AOMLCBHAJJH.currentCryptoKey);
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
