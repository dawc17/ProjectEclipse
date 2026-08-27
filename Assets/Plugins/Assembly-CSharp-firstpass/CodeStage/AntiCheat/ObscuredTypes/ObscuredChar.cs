using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		private static char cryptoKey = '—';

		private char currentCryptoKey;

		private char hiddenValue;

		private char fakeValue;

		private bool inited;

		private ObscuredChar(char value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = '\0';
			inited = true;
		}

		public static void SetNewCryptoKey(char CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static char EncryptDecrypt(char value)
		{
			return EncryptDecrypt(value, '\0');
		}

		public static char EncryptDecrypt(char value, char KGBGENDIMBC)
		{
			if (KGBGENDIMBC == '\0')
			{
				return (char)(value ^ cryptoKey);
			}
			return (char)(value ^ KGBGENDIMBC);
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
			char bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = (char)UnityEngine.Random.Range(0, 65535);
			hiddenValue = EncryptDecrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public char ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(char ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private char GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt('\0');
				fakeValue = '\0';
				inited = true;
			}
			char c = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0 && c != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return c;
		}

		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator char(ObscuredChar value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredChar ALEAHDHGCJL(ObscuredChar NILNDHEKNLJ)
		{
			char bAINMLLIKOL = (char)(NILNDHEKNLJ.GEKBGBJOMIA() + 1);
			NILNDHEKNLJ.hiddenValue = EncryptDecrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredChar DDKOKLNFNPB(ObscuredChar NILNDHEKNLJ)
		{
			char bAINMLLIKOL = (char)(NILNDHEKNLJ.GEKBGBJOMIA() - 1);
			NILNDHEKNLJ.hiddenValue = EncryptDecrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredChar))
			{
				return false;
			}
			return Equals((ObscuredChar)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredChar AOMLCBHAJJH)
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

		public string ToString(IFormatProvider EEGMFLOPLLH)
		{
			return GEKBGBJOMIA().ToString(EEGMFLOPLLH);
		}

		public override int GetHashCode()
		{
			return GEKBGBJOMIA().GetHashCode();
		}
	}
}
