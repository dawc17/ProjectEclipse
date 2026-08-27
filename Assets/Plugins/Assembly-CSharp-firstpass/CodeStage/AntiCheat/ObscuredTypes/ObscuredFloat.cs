using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredFloat : IEquatable<ObscuredFloat>, IFormattable
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct EHLBLJICLKD
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte NMAJNHKJJEM;

			[FieldOffset(1)]
			public byte ONNJMGGPHEL;

			[FieldOffset(2)]
			public byte NFOJBJJOOPO;

			[FieldOffset(3)]
			public byte PLCIDFPMNPL;
		}

		private static int cryptoKey = 230887;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private float fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredFloat(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0f;
			inited = true;
		}

		public static void SetNewCryptoKey(int CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static int Encrypt(float value)
		{
			return Encrypt(value, cryptoKey);
		}

		public static int Encrypt(float value, int KGBGENDIMBC)
		{
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				f = value
			};
			eHLBLJICLKD.i ^= KGBGENDIMBC;
			return eHLBLJICLKD.i;
		}

		private static byte[] InternalEncrypt(float value)
		{
			return InternalEncrypt(value, 0);
		}

		private static byte[] InternalEncrypt(float value, int KGBGENDIMBC)
		{
			int num = KGBGENDIMBC;
			if (num == 0)
			{
				num = cryptoKey;
			}
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				f = value
			};
			eHLBLJICLKD.i ^= num;
			return new byte[4] { eHLBLJICLKD.NMAJNHKJJEM, eHLBLJICLKD.ONNJMGGPHEL, eHLBLJICLKD.NFOJBJJOOPO, eHLBLJICLKD.PLCIDFPMNPL };
		}

		public static float Decrypt(int value)
		{
			return Decrypt(value, cryptoKey);
		}

		public static float Decrypt(int value, int KGBGENDIMBC)
		{
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				i = (value ^ KGBGENDIMBC)
			};
			return eHLBLJICLKD.f;
		}

		public void PKOKLDGAPEI()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalEncrypt(GEKBGBJOMIA(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void GMCADPGOCHM()
		{
			float bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = InternalEncrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public int ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3]
			};
			return eHLBLJICLKD.i;
		}

		public void SetEncrypted(int ANGFOBEKKKD)
		{
			inited = true;
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				i = ANGFOBEKKKD
			};
			hiddenValue = new byte[4] { eHLBLJICLKD.NMAJNHKJJEM, eHLBLJICLKD.ONNJMGGPHEL, eHLBLJICLKD.NFOJBJJOOPO, eHLBLJICLKD.PLCIDFPMNPL };
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private float GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0f);
				fakeValue = 0f;
				inited = true;
			}
			EHLBLJICLKD eHLBLJICLKD = new EHLBLJICLKD
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3]
			};
			eHLBLJICLKD.i ^= currentCryptoKey;
			float jKBEIEPBHOD = eHLBLJICLKD.f;
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0f && Math.Abs(jKBEIEPBHOD - fakeValue) > ObscuredCheatingDetector.get_Instance().floatEpsilon)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return jKBEIEPBHOD;
		}

		public static implicit operator ObscuredFloat(float value)
		{
			ObscuredFloat result = new ObscuredFloat(InternalEncrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator float(ObscuredFloat value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredFloat ALEAHDHGCJL(ObscuredFloat NILNDHEKNLJ)
		{
			float bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1f;
			NILNDHEKNLJ.hiddenValue = InternalEncrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredFloat DDKOKLNFNPB(ObscuredFloat NILNDHEKNLJ)
		{
			float bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1f;
			NILNDHEKNLJ.hiddenValue = InternalEncrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredFloat))
			{
				return false;
			}
			return Equals((ObscuredFloat)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredFloat AOMLCBHAJJH)
		{
			double num = AOMLCBHAJJH.GEKBGBJOMIA();
			double obj = GEKBGBJOMIA();
			return num.Equals(obj);
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
