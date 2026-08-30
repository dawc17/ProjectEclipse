using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

public static class RemoteLicenseCache
{
	private static uint _Seed;

	private static NekkiRandom LMEHCPNNAFA;

	private static bool ELBGLDMGENH;

	private static bool GKDDLHNGFBP;

	public static bool GFBMBNAIJEJ
	{
		get
		{
			return GGLBNKFOKKH();
		}
	}

	public static bool PAALBMOFKHD
	{
		get
		{
			return LHJJMGKDKKM();
		}
	}

	public static bool NNPLEHKHDND
	{
		get
		{
			return DJBEPKDFIAI();
		}
		set
		{
			FNNBCANLNKE(value);
		}
	}

	private static string IFJOIBDOBPL()
	{
		return SF2Paths.APHDBIBDMDG() + DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			2, 219, 188, 213, 205, 64, 171, 85, 38, 233,
			117, 24, 29, 37, 138, 196, 19, 70, 218, 114,
			6, 122, 115, 169, 123, 27, 5, 48, 205, 237,
			70, 18, 252, 101, 103, 155, 225, 96, 247, 117,
			176, 53, 84, 99, 9, 245, 191, 240, 84, 101,
			15, 246, 20, 81, 117, 72, 83, 61, 95, 218,
			177, 240, 136, 106, 158, 213, 6, 34, 138, 171,
			201, 228, 206, 67, 27, 253, 98, 23, 151, 89,
			140, 74, 195, 110, 42, 59, 230, 121, 229, 205,
			206, 50, 137, 229, 212, 144, 175, 75, 171, 88,
			44, 132, 85, 137, 227, 206, 7, 35, 105, 85,
			78, 39, 135, 165, 14, 150, 172, 23, 217, 89,
			20, 167, 0, 25, 215, 106, 240, 107
		}, false);
	}

	private static string EFBLOHCNMHP()
	{
		GIJMAGGFKFP();
		return LMEHCPNNAFA.randomString(20, DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			43, 214, 170, 136, 174, 17, 145, 86, 41, 226,
			46, 246, 59, 15, 58, 163, 162, 90, 229, 186,
			212, 117, 59, 236, 171, 184, 243, 191, 216, 173,
			58, 145, 67, 167, 62, 252, 26, 211, 53, 210,
			228, 47, 101, 199, 127, 157, 149, 22, 113, 192,
			218, 87, 100, 51, 53, 141, 150, 63, 122, 232,
			166, 240, 11, 187, 237, 186, 64, 119, 37, 108,
			7, 227, 187, 171, 240, 24, 137, 242, 208, 199,
			29, 32, 225, 229, 148, 161, 0, 201, 54, 33,
			8, 221, 113, 54, 248, 137, 221, 24, 1, 154,
			255, 83, 159, 204, 246, 29, 116, 235, 128, 171,
			29, 22, 29, 3, 245, 30, 149, 23, 169, 92,
			209, 46, 52, 42, 196, 82, 214, 122
		}, false));
	}

	private static string GetValueName(bool FNOHFMIHLBK)
	{
		return (!FNOHFMIHLBK) ? DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			99, 109, 19, 152, 161, 199, 90, 251, 143, 131,
			155, 236, 188, 210, 32, 79, 195, 201, 135, 36,
			218, 94, 115, 204, 255, 212, 188, 159, 157, 189,
			62, 125, 19, 241, 238, 196, 134, 95, 208, 2,
			190, 196, 245, 46, 150, 17, 107, 195, 246, 24,
			97, 20, 83, 46, 236, 128, 223, 54, 186, 241,
			149, 232, 66, 93, 95, 87, 164, 247, 181, 93,
			213, 111, 33, 22, 33, 189, 50, 85, 252, 188,
			129, 163, 117, 21, 153, 86, 225, 158, 53, 112,
			55, 108, 235, 140, 185, 87, 184, 31, 68, 42,
			233, 179, 57, 29, 33, 175, 254, 167, 157, 40,
			240, 57, 159, 70, 230, 22, 45, 179, 35, 177,
			254, 177, 244, 32, 220, 37, 234, 57
		}, false) : DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			152, 35, 232, 21, 216, 40, 104, 170, 227, 222,
			193, 87, 158, 10, 248, 46, 168, 254, 121, 171,
			197, 83, 50, 69, 43, 2, 189, 213, 241, 166,
			77, 22, 191, 132, 242, 61, 10, 42, 86, 165,
			66, 169, 102, 241, 85, 230, 121, 3, 28, 138,
			78, 136, 232, 161, 166, 73, 128, 16, 172, 108,
			2, 58, 173, 247, 22, 64, 188, 194, 169, 20,
			16, 251, 117, 84, 143, 59, 241, 122, 215, 81,
			240, 189, 31, 74, 152, 40, 42, 138, 3, 139,
			194, 240, 203, 44, 22, 25, 104, 58, 153, 29,
			110, 30, 220, 212, 250, 236, 172, 147, 246, 90,
			44, 158, 5, 107, 74, 171, 78, 57, 208, 10,
			187, 105, 77, 5, 44, 51, 77, 255
		}, false);
	}

	private static string ILCLEJCLIDN()
	{
		return DMMJEFCLAJF.DKDDEIHHMJP(new byte[128]
		{
			168, 185, 63, 94, 246, 233, 79, 22, 73, 91,
			249, 44, 3, 6, 54, 143, 135, 135, 58, 114,
			4, 50, 239, 242, 93, 169, 101, 121, 4, 152,
			224, 159, 46, 113, 52, 201, 44, 156, 197, 44,
			197, 135, 214, 178, 212, 238, 66, 39, 192, 221,
			22, 94, 250, 100, 214, 176, 235, 0, 23, 217,
			9, 215, 120, 48, 154, 54, 169, 68, 200, 21,
			136, 205, 140, 134, 143, 229, 138, 193, 208, 170,
			229, 207, 235, 93, 53, 177, 221, 250, 158, 245,
			96, 2, 45, 172, 148, 82, 242, 32, 77, 212,
			145, 54, 97, 167, 235, 112, 49, 144, 179, 33,
			42, 112, 177, 217, 45, 143, 144, 115, 152, 114,
			93, 173, 224, 155, 63, 173, 61, 242
		}, false);
	}

	private static string BMFCEKOGPHH()
	{
		return SystemProperties.IJOILMDCIMI() + ILCLEJCLIDN();
	}

	public static bool GGLBNKFOKKH()
	{
		return HCEPBIAOJKG.GFBMBNAIJEJ(IFJOIBDOBPL());
	}

	public static bool LHJJMGKDKKM()
	{
		return ELBGLDMGENH;
	}

	public static bool DJBEPKDFIAI()
	{
		return GKDDLHNGFBP;
	}

	public static void FNNBCANLNKE(bool value)
	{
		GKDDLHNGFBP = value;
		GGGEHAGCLGC();
	}

	public static void Init(uint LIGKOGEGBEB)
	{
		_Seed = LIGKOGEGBEB;
		ELBGLDMGENH = false;
		GKDDLHNGFBP = false;
		LoadFromFile();
	}

	private static void GIJMAGGFKFP()
	{
		LMEHCPNNAFA = new NekkiRandom(_Seed);
	}

	private static void LoadFromFile()
	{
		if (!HCEPBIAOJKG.GFBMBNAIJEJ(IFJOIBDOBPL()))
		{
			return;
		}
		byte[] array = AESUtils.DecryptFileToBytes(Constants.ECHOPKKPDFD, Constants.MCCEADFMLGA, IFJOIBDOBPL(), true);
		if (array == null)
		{
			Debug.Log("[RemoteLicenseCache] LoadFromFile - FAILED(decrypt error)!");
			return;
		}
		XmlDocument xmlDocument = XmlUtils.BOJDEHMPJIL(array);
		if (xmlDocument == null)
		{
			Debug.Log("[RemoteLicenseCache] LoadFromFile - FAILED(parse error)!");
			return;
		}
		XmlNode xmlNode = xmlDocument["Root"]["Data"];
		ELBGLDMGENH = true;
		GKDDLHNGFBP = xmlNode.Attributes[EFBLOHCNMHP()].CIPOICEEIBK() == MD5Utils.INPENHNJBGJ(GetValueName(true), BMFCEKOGPHH());
		Debug.Log("[RemoteLicenseCache] LoadFromFile!");
	}

	public static void GGGEHAGCLGC()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
		XmlElement xmlElement = xmlDocument.CreateElement("Root");
		xmlDocument.AppendChild(xmlElement);
		XmlElement xmlElement2 = xmlDocument.CreateElement("Data");
		xmlElement.AppendChild(xmlElement2);
		string jBAPBBGCOGG = BMFCEKOGPHH();
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(new KeyValuePair<string, string>(EFBLOHCNMHP(), MD5Utils.INPENHNJBGJ(GetValueName(GKDDLHNGFBP), jBAPBBGCOGG)));
		int length = list[0].Key.Length;
		int length2 = GetValueName(GKDDLHNGFBP).Length;
		for (int i = 0; i < 100; i++)
		{
			list.Add(new KeyValuePair<string, string>(LMEHCPNNAFA.randomString(length), MD5Utils.INPENHNJBGJ(LMEHCPNNAFA.randomString(length2), jBAPBBGCOGG)));
		}
		LMEHCPNNAFA.ShuffleList(list);
		foreach (KeyValuePair<string, string> item in list)
		{
			try
			{
				xmlElement2.SetAttribute(item.Key, item.Value);
			}
			catch
			{
			}
		}
		try
		{
			AESUtils.EncryptBytesToFile(Encoding.UTF8.GetBytes(xmlDocument.OuterXml), Constants.ECHOPKKPDFD, Constants.MCCEADFMLGA, IFJOIBDOBPL());
		}
		catch
		{
		}
		Debug.Log("[RemoteLicenseCache] SaveToFile!");
	}
}
