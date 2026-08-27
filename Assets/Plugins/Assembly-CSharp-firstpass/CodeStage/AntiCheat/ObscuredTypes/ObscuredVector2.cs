using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector2
	{
		[Serializable]
		public struct RawEncryptedVector2
		{
			public int x;

			public int y;
		}

		private static int cryptoKey = 120206;

		private static readonly Vector2 initialFakeValue = Vector2.zero;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedVector2 hiddenValue;

		[SerializeField]
		private Vector2 fakeValue;

		[SerializeField]
		private bool inited;

		public float x
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.x);
				if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && Math.Abs(num - fakeValue.x) > ObscuredCheatingDetector.get_Instance().vector2Epsilon)
				{
					ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
				}
				return num;
			}
			set
			{
				hiddenValue.x = InternalEncryptField(value);
				if (ObscuredCheatingDetector.NMACGEJHPDN())
				{
					fakeValue.x = value;
				}
			}
		}

		public float y
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.y);
				if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && Math.Abs(num - fakeValue.y) > ObscuredCheatingDetector.get_Instance().vector2Epsilon)
				{
					ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
				}
				return num;
			}
			set
			{
				hiddenValue.y = InternalEncryptField(value);
				if (ObscuredCheatingDetector.NMACGEJHPDN())
				{
					fakeValue.y = value;
				}
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return x;
				case 1:
					return y;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
			}
		}

		private ObscuredVector2(RawEncryptedVector2 value)
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

		public static RawEncryptedVector2 Encrypt(Vector2 value)
		{
			return Encrypt(value, 0);
		}

		public static RawEncryptedVector2 Encrypt(Vector2 value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			RawEncryptedVector2 result = default(RawEncryptedVector2);
			result.x = ObscuredFloat.Encrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Encrypt(value.y, KGBGENDIMBC);
			return result;
		}

		public static Vector2 Decrypt(RawEncryptedVector2 value)
		{
			return Decrypt(value, 0);
		}

		public static Vector2 Decrypt(RawEncryptedVector2 value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			Vector2 result = default(Vector2);
			result.x = ObscuredFloat.Decrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Decrypt(value.y, KGBGENDIMBC);
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
			Vector2 bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public RawEncryptedVector2 ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(RawEncryptedVector2 ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private Vector2 GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(initialFakeValue);
				fakeValue = initialFakeValue;
				inited = true;
			}
			Vector2 vector = default(Vector2);
			vector.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && !CompareVectorsWithTolerance(vector, fakeValue))
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return vector;
		}

		private bool CompareVectorsWithTolerance(Vector2 DDDIMIIGCDI, Vector2 MIALLDMOOMA)
		{
			float vector2Epsilon = ObscuredCheatingDetector.get_Instance().vector2Epsilon;
			return Math.Abs(DDDIMIIGCDI.x - MIALLDMOOMA.x) < vector2Epsilon && Math.Abs(DDDIMIIGCDI.y - MIALLDMOOMA.y) < vector2Epsilon;
		}

		private float InternalDecryptField(int ANGFOBEKKKD)
		{
			int kGBGENDIMBC = cryptoKey;
			if (currentCryptoKey != cryptoKey)
			{
				kGBGENDIMBC = currentCryptoKey;
			}
			return ObscuredFloat.Decrypt(ANGFOBEKKKD, kGBGENDIMBC);
		}

		private int InternalEncryptField(float ANGFOBEKKKD)
		{
			return ObscuredFloat.Encrypt(ANGFOBEKKKD, cryptoKey);
		}

		public static implicit operator ObscuredVector2(Vector2 value)
		{
			ObscuredVector2 result = new ObscuredVector2(Encrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector2(ObscuredVector2 value)
		{
			return value.GEKBGBJOMIA();
		}

		public static implicit operator Vector3(ObscuredVector2 value)
		{
			Vector2 vector = value.GEKBGBJOMIA();
			return new Vector3(vector.x, vector.y, 0f);
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
