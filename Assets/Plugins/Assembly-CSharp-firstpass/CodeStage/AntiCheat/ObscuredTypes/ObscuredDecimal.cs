using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredDecimal : IEquatable<ObscuredDecimal>, IFormattable
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct FFMMPKOPPGG
		{
			[FieldOffset(0)]
			public decimal d;

			[FieldOffset(0)]
			public long FGFLFAHLOEI;

			[FieldOffset(8)]
			public long KNGJOHMHNDF;

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

			[FieldOffset(8)]
			public byte CGOBDKHAHIH;

			[FieldOffset(9)]
			public byte KCHIHIPLDEG;

			[FieldOffset(10)]
			public byte BNGHKEOHKCA;

			[FieldOffset(11)]
			public byte IIJELFHMGHA;

			[FieldOffset(12)]
			public byte BHDLOJBBIEK;

			[FieldOffset(13)]
			public byte EMNECOBJKIE;

			[FieldOffset(14)]
			public byte KFLLILNIJHL;

			[FieldOffset(15)]
			public byte KCLIJDPNNGH;
		}

		private static long cryptoKey = 209208L;

		private long currentCryptoKey;

		private byte[] hiddenValue;

		private decimal fakeValue;

		private bool inited;

		private ObscuredDecimal(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0m;
			inited = true;
		}

		public static void SetNewCryptoKey(long CNOFJICCAHK)
		{
			cryptoKey = CNOFJICCAHK;
		}

		public static decimal Encrypt(decimal value)
		{
			return Encrypt(value, cryptoKey);
		}

		public static decimal Encrypt(decimal value, long KGBGENDIMBC)
		{
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				d = value
			};
			fFMMPKOPPGG.FGFLFAHLOEI ^= KGBGENDIMBC;
			fFMMPKOPPGG.KNGJOHMHNDF ^= KGBGENDIMBC;
			return fFMMPKOPPGG.d;
		}

		private static byte[] InternalEncrypt(decimal value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(decimal value, long KGBGENDIMBC)
		{
			long num = KGBGENDIMBC;
			if (num == 0)
			{
				num = cryptoKey;
			}
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				d = value
			};
			fFMMPKOPPGG.FGFLFAHLOEI ^= num;
			fFMMPKOPPGG.KNGJOHMHNDF ^= num;
			return new byte[16]
			{
				fFMMPKOPPGG.NMAJNHKJJEM, fFMMPKOPPGG.ONNJMGGPHEL, fFMMPKOPPGG.NFOJBJJOOPO, fFMMPKOPPGG.PLCIDFPMNPL, fFMMPKOPPGG.PLIKJEOPJOB, fFMMPKOPPGG.AGPBOFNNOKO, fFMMPKOPPGG.LDPKGJADPMI, fFMMPKOPPGG.PHHFMLPDCBO, fFMMPKOPPGG.CGOBDKHAHIH, fFMMPKOPPGG.KCHIHIPLDEG,
				fFMMPKOPPGG.BNGHKEOHKCA, fFMMPKOPPGG.IIJELFHMGHA, fFMMPKOPPGG.BHDLOJBBIEK, fFMMPKOPPGG.EMNECOBJKIE, fFMMPKOPPGG.KFLLILNIJHL, fFMMPKOPPGG.KCLIJDPNNGH
			};
		}

		public static decimal Decrypt(decimal value)
		{
			return Decrypt(value, cryptoKey);
		}

		public static decimal Decrypt(decimal value, long KGBGENDIMBC)
		{
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				d = value
			};
			fFMMPKOPPGG.FGFLFAHLOEI ^= KGBGENDIMBC;
			fFMMPKOPPGG.KNGJOHMHNDF ^= KGBGENDIMBC;
			return fFMMPKOPPGG.d;
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
			decimal bAINMLLIKOL = GEKBGBJOMIA();
			currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			hiddenValue = InternalEncrypt(bAINMLLIKOL, currentCryptoKey);
		}

		public decimal ECEBFGCJIDA()
		{
			PKOKLDGAPEI();
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3],
				PLIKJEOPJOB = hiddenValue[4],
				AGPBOFNNOKO = hiddenValue[5],
				LDPKGJADPMI = hiddenValue[6],
				PHHFMLPDCBO = hiddenValue[7],
				CGOBDKHAHIH = hiddenValue[8],
				KCHIHIPLDEG = hiddenValue[9],
				BNGHKEOHKCA = hiddenValue[10],
				IIJELFHMGHA = hiddenValue[11],
				BHDLOJBBIEK = hiddenValue[12],
				EMNECOBJKIE = hiddenValue[13],
				KFLLILNIJHL = hiddenValue[14],
				KCLIJDPNNGH = hiddenValue[15]
			};
			return fFMMPKOPPGG.d;
		}

		public void SetEncrypted(decimal ANGFOBEKKKD)
		{
			inited = true;
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				d = ANGFOBEKKKD
			};
			hiddenValue = new byte[16]
			{
				fFMMPKOPPGG.NMAJNHKJJEM, fFMMPKOPPGG.ONNJMGGPHEL, fFMMPKOPPGG.NFOJBJJOOPO, fFMMPKOPPGG.PLCIDFPMNPL, fFMMPKOPPGG.PLIKJEOPJOB, fFMMPKOPPGG.AGPBOFNNOKO, fFMMPKOPPGG.LDPKGJADPMI, fFMMPKOPPGG.PHHFMLPDCBO, fFMMPKOPPGG.CGOBDKHAHIH, fFMMPKOPPGG.KCHIHIPLDEG,
				fFMMPKOPPGG.BNGHKEOHKCA, fFMMPKOPPGG.IIJELFHMGHA, fFMMPKOPPGG.BHDLOJBBIEK, fFMMPKOPPGG.EMNECOBJKIE, fFMMPKOPPGG.KFLLILNIJHL, fFMMPKOPPGG.KCLIJDPNNGH
			};
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				fakeValue = GEKBGBJOMIA();
			}
		}

		private decimal GEKBGBJOMIA()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0m);
				fakeValue = 0m;
				inited = true;
			}
			FFMMPKOPPGG fFMMPKOPPGG = new FFMMPKOPPGG
			{
				NMAJNHKJJEM = hiddenValue[0],
				ONNJMGGPHEL = hiddenValue[1],
				NFOJBJJOOPO = hiddenValue[2],
				PLCIDFPMNPL = hiddenValue[3],
				PLIKJEOPJOB = hiddenValue[4],
				AGPBOFNNOKO = hiddenValue[5],
				LDPKGJADPMI = hiddenValue[6],
				PHHFMLPDCBO = hiddenValue[7],
				CGOBDKHAHIH = hiddenValue[8],
				KCHIHIPLDEG = hiddenValue[9],
				BNGHKEOHKCA = hiddenValue[10],
				IIJELFHMGHA = hiddenValue[11],
				BHDLOJBBIEK = hiddenValue[12],
				EMNECOBJKIE = hiddenValue[13],
				KFLLILNIJHL = hiddenValue[14],
				KCLIJDPNNGH = hiddenValue[15]
			};
			fFMMPKOPPGG.FGFLFAHLOEI ^= currentCryptoKey;
			fFMMPKOPPGG.KNGJOHMHNDF ^= currentCryptoKey;
			decimal oFMGDFKHPDO = fFMMPKOPPGG.d;
			if (ObscuredCheatingDetector.NMACGEJHPDN() && fakeValue != 0m && oFMGDFKHPDO != fakeValue)
			{
				ObscuredCheatingDetector.get_Instance().MCDANNDOEIK();
			}
			return oFMGDFKHPDO;
		}

		public static implicit operator ObscuredDecimal(decimal value)
		{
			ObscuredDecimal result = new ObscuredDecimal(InternalEncrypt(value));
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.GEKBGBJOMIA();
		}

		public static explicit operator ObscuredDecimal(ObscuredFloat f)
		{
			return (ObscuredDecimal)((decimal)(float)(f));
		}

		[SpecialName]
		public static ObscuredDecimal ALEAHDHGCJL(ObscuredDecimal NILNDHEKNLJ)
		{
			decimal bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() + 1m;
			NILNDHEKNLJ.hiddenValue = InternalEncrypt(bAINMLLIKOL, NILNDHEKNLJ.currentCryptoKey);
			if (ObscuredCheatingDetector.NMACGEJHPDN())
			{
				NILNDHEKNLJ.fakeValue = bAINMLLIKOL;
			}
			return NILNDHEKNLJ;
		}

		[SpecialName]
		public static ObscuredDecimal DDKOKLNFNPB(ObscuredDecimal NILNDHEKNLJ)
		{
			decimal bAINMLLIKOL = NILNDHEKNLJ.GEKBGBJOMIA() - 1m;
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
			if (!(AOMLCBHAJJH is ObscuredDecimal))
			{
				return false;
			}
			return Equals((ObscuredDecimal)AOMLCBHAJJH);
		}

		public bool Equals(ObscuredDecimal AOMLCBHAJJH)
		{
			return AOMLCBHAJJH.GEKBGBJOMIA().Equals(GEKBGBJOMIA());
		}

		public override int GetHashCode()
		{
			return GEKBGBJOMIA().GetHashCode();
		}
	}
}
