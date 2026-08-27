using System;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector3
	{
		[Serializable]
		public struct RawEncryptedVector3
		{
			public int x;

			public int y;

			public int z;
		}

		private static int cryptoKey = 120207;

		private static readonly Vector3 initialFakeValue = Vector3.zero;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private RawEncryptedVector3 hiddenValue;

		[SerializeField]
		private Vector3 fakeValue;

		[SerializeField]
		private bool inited;

		public float x
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.x);
				if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && Math.Abs(num - fakeValue.x) > ObscuredCheatingDetector.get_Instance().vector3Epsilon)
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
				if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && Math.Abs(num - fakeValue.y) > ObscuredCheatingDetector.get_Instance().vector3Epsilon)
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

		public float z
		{
			get
			{
				float num = InternalDecryptField(hiddenValue.z);
				if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(initialFakeValue) && Math.Abs(num - fakeValue.z) > ObscuredCheatingDetector.get_Instance().vector3Epsilon)
				{
					ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
				}
				return num;
			}
			set
			{
				hiddenValue.z = InternalEncryptField(value);
				if (ObscuredCheatingDetector.NMACGEJHPDN())
				{
					fakeValue.z = value;
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
				case 2:
					return z;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
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
				case 2:
					z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
		}

		private ObscuredVector3(RawEncryptedVector3 ANGFOBEKKKD)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = ANGFOBEKKKD;
			fakeValue = initialFakeValue;
			inited = true;
		}

		public static void SetNewCryptoKey(int CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static RawEncryptedVector3 Encrypt(Vector3 value)
		{
			return Encrypt(value, 0);
		}

		public static RawEncryptedVector3 Encrypt(Vector3 value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			RawEncryptedVector3 result = default(RawEncryptedVector3);
			result.x = ObscuredFloat.Encrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Encrypt(value.y, KGBGENDIMBC);
			result.z = ObscuredFloat.Encrypt(value.z, KGBGENDIMBC);
			return result;
		}

		public static Vector3 Decrypt(RawEncryptedVector3 value)
		{
			return Decrypt(value, 0);
		}

		public static Vector3 Decrypt(RawEncryptedVector3 value, int KGBGENDIMBC)
		{
			if (KGBGENDIMBC == 0)
			{
				KGBGENDIMBC = cryptoKey;
			}
			Vector3 result = default(Vector3);
			result.x = ObscuredFloat.Decrypt(value.x, KGBGENDIMBC);
			result.y = ObscuredFloat.Decrypt(value.y, KGBGENDIMBC);
			result.z = ObscuredFloat.Decrypt(value.z, KGBGENDIMBC);
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
			Vector3 bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = Encrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public RawEncryptedVector3 ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			return hiddenValue;
		}

		public void SetEncrypted(RawEncryptedVector3 ANGFOBEKKKD)
		{
			inited = true;
			hiddenValue = ANGFOBEKKKD;
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private Vector3 GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(initialFakeValue, cryptoKey);
				fakeValue = initialFakeValue;
				inited = true;
			}
			Vector3 vector = default(Vector3);
			vector.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
			vector.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN() && !fakeValue.Equals(Vector3.zero) && !CompareVectorsWithTolerance(vector, fakeValue))
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return vector;
		}

		private bool CompareVectorsWithTolerance(Vector3 DDDIMIIGCDI, Vector3 MIALLDMOOMA)
		{
			float vector3Epsilon = ObscuredCheatingDetector.get_Instance().vector3Epsilon;
			return Math.Abs(DDDIMIIGCDI.x - MIALLDMOOMA.x) < vector3Epsilon && Math.Abs(DDDIMIIGCDI.y - MIALLDMOOMA.y) < vector3Epsilon && Math.Abs(DDDIMIIGCDI.z - MIALLDMOOMA.z) < vector3Epsilon;
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

		public static implicit operator ObscuredVector3(Vector3 value)
		{
			ObscuredVector3 result = new ObscuredVector3(Encrypt(value, cryptoKey));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredVector3 PHEFFKMOOCM(ObscuredVector3 LHBNIMGFKIB, ObscuredVector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() + AAOIAEJJINO.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 PHEFFKMOOCM(Vector3 LHBNIMGFKIB, ObscuredVector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB + AAOIAEJJINO.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 PHEFFKMOOCM(ObscuredVector3 LHBNIMGFKIB, Vector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() + AAOIAEJJINO);
		}

		[SpecialName]
		public static ObscuredVector3 MJOKEBGPHKB(ObscuredVector3 LHBNIMGFKIB, ObscuredVector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() - AAOIAEJJINO.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 MJOKEBGPHKB(Vector3 LHBNIMGFKIB, ObscuredVector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB - AAOIAEJJINO.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 MJOKEBGPHKB(ObscuredVector3 LHBNIMGFKIB, Vector3 AAOIAEJJINO)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() - AAOIAEJJINO);
		}

		[SpecialName]
		public static ObscuredVector3 op_UnaryNegation(ObscuredVector3 LHBNIMGFKIB)
		{
			return (ObscuredVector3)(-LHBNIMGFKIB.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 op_Multiply(ObscuredVector3 LHBNIMGFKIB, float d)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() * d);
		}

		[SpecialName]
		public static ObscuredVector3 op_Multiply(float d, ObscuredVector3 LHBNIMGFKIB)
		{
			return (ObscuredVector3)(d * LHBNIMGFKIB.GEKBGBJOMIA());
		}

		[SpecialName]
		public static ObscuredVector3 GFLOJCCKHAO(ObscuredVector3 LHBNIMGFKIB, float d)
		{
			return (ObscuredVector3)(LHBNIMGFKIB.GEKBGBJOMIA() / d);
		}

		[SpecialName]
		public static bool LFPMCJPCJBD(ObscuredVector3 FBENKEEDIKJ, ObscuredVector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ.GEKBGBJOMIA() == PGKPNBGIGEI.GEKBGBJOMIA();
		}

		[SpecialName]
		public static bool LFPMCJPCJBD(Vector3 FBENKEEDIKJ, ObscuredVector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ == PGKPNBGIGEI.GEKBGBJOMIA();
		}

		[SpecialName]
		public static bool LFPMCJPCJBD(ObscuredVector3 FBENKEEDIKJ, Vector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ.GEKBGBJOMIA() == PGKPNBGIGEI;
		}

		[SpecialName]
		public static bool GLCJKGIOIEC(ObscuredVector3 FBENKEEDIKJ, ObscuredVector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ.GEKBGBJOMIA() != PGKPNBGIGEI.GEKBGBJOMIA();
		}

		[SpecialName]
		public static bool GLCJKGIOIEC(Vector3 FBENKEEDIKJ, ObscuredVector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ != PGKPNBGIGEI.GEKBGBJOMIA();
		}

		[SpecialName]
		public static bool GLCJKGIOIEC(ObscuredVector3 FBENKEEDIKJ, Vector3 PGKPNBGIGEI)
		{
			return FBENKEEDIKJ.GEKBGBJOMIA() != PGKPNBGIGEI;
		}

		public override bool Equals(object NOLFMPDGCOC)
		{
			return GEKBGBJOMIA().Equals(NOLFMPDGCOC);
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
