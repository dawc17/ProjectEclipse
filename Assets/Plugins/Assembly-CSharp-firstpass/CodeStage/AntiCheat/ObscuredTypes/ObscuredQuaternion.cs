using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredQuaternion
	{
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			public int x;

			public int y;

			public int z;

			public int w;
		}

		private static int cryptoKey = 120205;

		private static readonly Quaternion initialFakeValue = Quaternion.identity;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedQuaternion hiddenValue;

		[SerializeField]
		private Quaternion fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredQuaternion(RawEncryptedQuaternion value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = initialFakeValue;
			inited = true;
		}

		public static void SetNewCryptoKey(int CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		public static RawEncryptedQuaternion Encrypt(Quaternion value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			RawEncryptedQuaternion result = default(RawEncryptedQuaternion);
			result.x = ObscuredFloat.Encrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Encrypt(value.y, KGBGENDIMBC);
			result.z = ObscuredFloat.Encrypt(value.z, KGBGENDIMBC);
			result.w = ObscuredFloat.Encrypt(value.w, KGBGENDIMBC);
			return result;
		}

		public static Quaternion Decrypt(RawEncryptedQuaternion value)
		{
			return Decrypt(value, 0);
		}

		public static Quaternion Decrypt(RawEncryptedQuaternion value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			Quaternion result = default(Quaternion);
			result.x = ObscuredFloat.Decrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Decrypt(value.y, KGBGENDIMBC);
			result.z = ObscuredFloat.Decrypt(value.z, KGBGENDIMBC);
			result.w = ObscuredFloat.Decrypt(value.w, KGBGENDIMBC);
			return result;
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
			Quaternion bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public RawEncryptedQuaternion ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(RawEncryptedQuaternion ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private Quaternion GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(initialFakeValue);
				fakeValue = initialFakeValue;
				inited = true;
			}
			Quaternion quaternion = default(Quaternion);
			quaternion.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
			quaternion.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
			quaternion.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);
			quaternion.w = ObscuredFloat.Decrypt(hiddenValue.w, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && !FHCMDNDNDGH(quaternion, fakeValue))
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return quaternion;
		}

		private bool FHCMDNDNDGH(Quaternion GOFJCIABOEC, Quaternion ENPEGNEGAPE)
		{
			float quaternionEpsilon = ObscuredCheatingDetector.get_Instance().quaternionEpsilon;
			return Math.Abs(GOFJCIABOEC.x - ENPEGNEGAPE.x) < quaternionEpsilon && Math.Abs(GOFJCIABOEC.y - ENPEGNEGAPE.y) < quaternionEpsilon && Math.Abs(GOFJCIABOEC.z - ENPEGNEGAPE.z) < quaternionEpsilon && Math.Abs(GOFJCIABOEC.w - ENPEGNEGAPE.w) < quaternionEpsilon;
		}

		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			ObscuredQuaternion result = new ObscuredQuaternion(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.GEKBGBJOMIA();
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
	}
}
