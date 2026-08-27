using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredBool : IEquatable<ObscuredBool>
	{
		private static byte cryptoKey = 215;

		[SerializeField]
		private byte currentCryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private bool fakeValue;

		[SerializeField]
		private bool fakeValueChanged;

		[SerializeField]
		private bool inited;

		private ObscuredBool(int value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = false;
			fakeValueChanged = false;
			inited = true;
		}

		public static void SetNewCryptoKey(byte CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static int Encrypt(bool value)
		{
			return Encrypt(value, 0);
		}

		public static int Encrypt(bool value, byte KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			int num = ((!value) ? 181 : 213);
			return num ^ KGBGENDIMBC;
		}

		public static bool Decrypt(int value)
		{
			return Decrypt(value, 0);
		}

		public static bool Decrypt(int value, byte KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			value ^= KGBGENDIMBC;
			return value != 181;
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
			bool bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = (byte)UnityEngine.Random.Range(0, 255);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public int ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(int ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
				fakeValueChanged = true;
			}
		}

		private bool GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(false);
				fakeValue = false;
				fakeValueChanged = true;
				inited = true;
			}
			int num = hiddenValue;
			num ^= currentCryptoKey;
			bool flag = num != 181;
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValueChanged && flag != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return flag;
		}

		public static implicit operator ObscuredBool(bool value)
		{
			ObscuredBool result = new ObscuredBool(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
				result.fakeValueChanged = true;
			}
			return result;
		}

		public static implicit operator bool(ObscuredBool value)
		{
			return value.GEKBGBJOMIA();
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredBool))
			{
				return false;
			}
			return Equals((ObscuredBool)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredBool AOMLCBHAJJH)
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
	}
}
