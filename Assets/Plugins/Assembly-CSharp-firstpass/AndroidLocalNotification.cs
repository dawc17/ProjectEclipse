using UnityEngine;

public class AndroidLocalNotification
{
	public enum OIBFFILFNNB
	{
		Inexact = 0,
		Exact = 1,
		ExactAndAllowWhileIdle = 2
	}

	private static string CJEBLHDIJGP()
	{
		return JJHBPGBNFPC.DKDDEIHHMJP(new byte[128]
		{
			81, 107, 67, 67, 156, 185, 73, 198, 152, 218,
			37, 217, 110, 245, 89, 191, 150, 199, 136, 53,
			147, 205, 157, 224, 221, 99, 120, 132, 244, 117,
			5, 45, 33, 24, 115, 191, 247, 21, 175, 181,
			208, 0, 226, 175, 245, 244, 4, 169, 44, 91,
			178, 179, 204, 243, 55, 228, 132, 37, 252, 117,
			178, 178, 195, 96, 74, 221, 246, 217, 145, 9,
			224, 121, 135, 47, 79, 147, 86, 192, 225, 60,
			193, 141, 162, 89, 82, 68, 158, 143, 32, 150,
			103, 117, 94, 120, 83, 118, 68, 168, 255, 251,
			39, 222, 184, 90, 197, 48, 193, 16, 178, 163,
			184, 252, 213, 222, 26, 230, 198, 166, 162, 122,
			157, 98, 202, 17, 184, 153, 217, 74
		}, false);
	}

	private static string EMIDJPIBPKK()
	{
		return JJHBPGBNFPC.DKDDEIHHMJP(new byte[128]
		{
			100, 34, 209, 163, 88, 249, 11, 238, 36, 48,
			5, 74, 218, 140, 52, 48, 136, 26, 39, 132,
			201, 167, 192, 69, 47, 144, 181, 222, 10, 234,
			26, 22, 102, 140, 242, 50, 247, 55, 102, 45,
			134, 79, 28, 132, 252, 98, 16, 68, 179, 155,
			41, 18, 45, 59, 42, 201, 236, 88, 57, 187,
			9, 157, 52, 156, 253, 138, 251, 139, 59, 17,
			153, 113, 52, 20, 83, 37, 110, 215, 40, 152,
			39, 70, 252, 75, 178, 192, 224, 7, 81, 54,
			81, 96, 7, 206, 143, 16, 221, 93, 134, 139,
			211, 150, 70, 88, 216, 107, 112, 136, 208, 104,
			22, 238, 48, 94, 26, 81, 136, 99, 128, 125,
			239, 94, 245, 95, 133, 158, 229, 63
		}, false);
	}

	public static void GAMLNBGMCHB(int OKNNNLIPODI, string PEMOECLNECD, string LIOGIBJBHAH, long ENDPMCNJPEA)
	{
		ADDHDJPBKGD(OKNNNLIPODI, ENDPMCNJPEA, PEMOECLNECD, LIOGIBJBHAH, Color.black);
	}

	private static void ADDHDJPBKGD(int OKNNNLIPODI, long ENDPMCNJPEA, string PEMOECLNECD, string LIOGIBJBHAH, Color32 NPCPKCNJCOM, bool LGLFOBEIPKB = true, bool CEGOKEEKHDP = true, bool KILOFHBEDKP = true, OIBFFILFNNB GFGGECPLIID = OIBFFILFNNB.Inexact)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP());
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetNotification", OKNNNLIPODI, ENDPMCNJPEA * 1000, PEMOECLNECD, LIOGIBJBHAH, LIOGIBJBHAH, LGLFOBEIPKB ? 1 : 0, CEGOKEEKHDP ? 1 : 0, KILOFHBEDKP ? 1 : 0, "app_icon", "notify_icon_small", NPCPKCNJCOM.r * 65536 + NPCPKCNJCOM.g * 256 + NPCPKCNJCOM.b, (int)GFGGECPLIID, EMIDJPIBPKK());
		}
	}

	public static void MKOEHNJBKNM(int OKNNNLIPODI)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(CJEBLHDIJGP());
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelNotification", OKNNNLIPODI);
		}
	}
}
