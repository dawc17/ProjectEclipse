using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredDouble : IEquatable<ObscuredDouble>, IFormattable
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct OGDAPDCFLOF
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public byte NMAJNHKJJEM;

			[FieldOffset(1)]
			public byte ONNJMGGPHEL;

			[FieldOffset(2)]
			public byte NFOJBJJOOPO;

			[FieldOffset(3)]
			public byte PLCIDFPMNPL;

			[FieldOffset(4)]
			public byte PLIKJEOPJOB;

			[FieldOffset(5)]
			public byte AGPBOFNNOKO;

			[FieldOffset(6)]
			public byte LDPKGJADPMI;

			[FieldOffset(7)]
			public byte PHHFMLPDCBO;
		}

		private static long cryptoKey = 210987L;

		[SerializeField]
		private long currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private double fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredDouble(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0.0;
			inited = true;
		}

		public static void SetNewCryptoKey(long CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static long Encrypt(double value)
		{
			return Encrypt(value, cryptoKey);
		}

		public static long Encrypt(double value, long KGBGENDIMBC)
		{
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				d = value
			};
			oGDAPDCFLOF.l ^= KGBGENDIMBC;
			return oGDAPDCFLOF.l;
		}

		private static byte[] InternalEncrypt(double value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(double value, long KGBGENDIMBC)
		{
			long num = KGBGENDIMBC;
			if (num == 0)
			{
				num = cryptoKey;
			}
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				d = value
			};
			oGDAPDCFLOF.l ^= num;
			return new byte[8] { oGDAPDCFLOF.NMAJNHKJJEM, oGDAPDCFLOF.ONNJMGGPHEL, oGDAPDCFLOF.NFOJBJJOOPO, oGDAPDCFLOF.PLCIDFPMNPL, oGDAPDCFLOF.PLIKJEOPJOB, oGDAPDCFLOF.AGPBOFNNOKO, oGDAPDCFLOF.LDPKGJADPMI, oGDAPDCFLOF.PHHFMLPDCBO };
		}

		public static double Decrypt(long value)
		{
			return Decrypt(value, cryptoKey);
		}

		public static double Decrypt(long value, long KGBGENDIMBC)
		{
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				l = (value ^ KGBGENDIMBC)
			};
			return oGDAPDCFLOF.d;
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
			double bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = InternalEncrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public long ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3],
				PLIKJEOPJOB = hiddenValue[4],
				AGPBOFNNOKO = hiddenValue[5],
				LDPKGJADPMI = hiddenValue[6],
				PHHFMLPDCBO = hiddenValue[7]
			};
			return oGDAPDCFLOF.l;
		}

		public void SetEncrypted(long ANGFOBEKKKD)
		{
			inited = true;
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				l = ANGFOBEKKKD
			};
			hiddenValue = new byte[8] { oGDAPDCFLOF.NMAJNHKJJEM, oGDAPDCFLOF.ONNJMGGPHEL, oGDAPDCFLOF.NFOJBJJOOPO, oGDAPDCFLOF.PLCIDFPMNPL, oGDAPDCFLOF.PLIKJEOPJOB, oGDAPDCFLOF.AGPBOFNNOKO, oGDAPDCFLOF.LDPKGJADPMI, oGDAPDCFLOF.PHHFMLPDCBO };
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private double GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0.0);
				fakeValue = 0.0;
				inited = true;
			}
			OGDAPDCFLOF oGDAPDCFLOF = new OGDAPDCFLOF
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3],
				PLIKJEOPJOB = hiddenValue[4],
				AGPBOFNNOKO = hiddenValue[5],
				LDPKGJADPMI = hiddenValue[6],
				PHHFMLPDCBO = hiddenValue[7]
			};
			oGDAPDCFLOF.l ^= currentCryptoKey;
			double oFMGDFKHPDO = oGDAPDCFLOF.d;
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0.0 && Math.Abs(oFMGDFKHPDO - fakeValue) > 1E-06)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return oFMGDFKHPDO;
		}

		public static implicit operator ObscuredDouble(double value)
		{
			ObscuredDouble result = new ObscuredDouble(InternalEncrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.GEKBGBJOMIA();
		}

		[SpecialName]
		public static ObscuredDouble ALEAHDHGCJL(ObscuredDouble NILNDHEKNLJ)
		{
			double bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1.0;
			NILNDHEKNLJ.hiddenValue = InternalEncrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredDouble DDKOKLNFNPB(ObscuredDouble NILNDHEKNLJ)
		{
			double bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1.0;
			NILNDHEKNLJ.hiddenValue = InternalEncrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
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

		public override bool Equals(object AOMLCBHAJJH)
		{
			if (!(AOMLCBHAJJH is ObscuredDouble))
			{
				return false;
			}
			return Equals((ObscuredDouble)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredDouble AOMLCBHAJJH)
		{
			return AOMLCBHAJJH.GEKBGBJOMIA().Equals(GEKBGBJOMIA());
		}

		public override int GetHashCode()
		{
			return GEKBGBJOMIA().GetHashCode();
		}
	}
}
