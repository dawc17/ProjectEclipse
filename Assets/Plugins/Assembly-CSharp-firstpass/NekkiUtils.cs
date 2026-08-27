using System.Globalization;
using UnityEngine;

public class NekkiUtils
{
	public enum FHOMOMIOLAK
	{
		NONE = 0,
		UNKNOWN = 1,
		PHONE = 2,
		TABLET = 3,
		CONSOLE = 4,
		DESKTOP = 5
	}

	private static FHOMOMIOLAK GLDLEHNOHIM;

	public static Vector2 GetVector2FromString(string IGGFGLLIGCG, char DPOEFEMLAKD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		string[] array = IGGFGLLIGCG.Split(DPOEFEMLAKD);
		return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}

	public static Vector3 GetVector3FromString(string IGGFGLLIGCG, char DPOEFEMLAKD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		string[] array = IGGFGLLIGCG.Split(DPOEFEMLAKD);
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}

	public static Vector4 GetVector4FromString(string IGGFGLLIGCG, char DPOEFEMLAKD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		string[] array = IGGFGLLIGCG.Split(DPOEFEMLAKD);
		return new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	public static Matrix4x4 NPOAOFGPJLD(string IGGFGLLIGCG, char DPOEFEMLAKD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		string[] array = IGGFGLLIGCG.Split(DPOEFEMLAKD);
		Matrix4x4 result = default(Matrix4x4);
		short num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				result[j, i] = float.Parse(array[num]);
				num++;
			}
		}
		return result;
	}

	public static Matrix4x4 GHNDKDEDOGH(string IGGFGLLIGCG, char DPOEFEMLAKD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		string[] array = IGGFGLLIGCG.Split(DPOEFEMLAKD);
		Matrix4x4 result = default(Matrix4x4);
		short num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				result[i, j] = float.Parse(array[num]);
				num++;
			}
		}
		return result;
	}

	public static string LKFECIDBIEA(string IGGFGLLIGCG, char DPOEFEMLAKD, int PLBNNDIKAJO, int count, out int JIEODOJKGLD)
	{
		IGGFGLLIGCG = IGGFGLLIGCG.Trim();
		int num = 0;
		int num2 = PLBNNDIKAJO;
		while (num < count && num2 < IGGFGLLIGCG.Length)
		{
			if (IGGFGLLIGCG[num2] == DPOEFEMLAKD)
			{
				num++;
			}
			num2++;
		}
		JIEODOJKGLD = num2;
		if (num == count)
		{
			return IGGFGLLIGCG.Substring(PLBNNDIKAJO, num2 - PLBNNDIKAJO);
		}
		return IGGFGLLIGCG.Substring(PLBNNDIKAJO);
	}

	public static Quaternion GetQuaternionFromMatrix(Matrix4x4 NHBBGODHBEF)
	{
		return Quaternion.LookRotation(NHBBGODHBEF.GetColumn(2), NHBBGODHBEF.GetColumn(1));
	}

	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	public static Color HexToColor(string IJGJLEJKMBJ)
	{
		byte r = byte.Parse(IJGJLEJKMBJ.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(IJGJLEJKMBJ.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(IJGJLEJKMBJ.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static Color IntToColor(int color)
	{
		return new Color
		{
			r = (float)((color & 0xFF0000) >> 16) / 255f,
			g = (float)((color & 0xFF00) >> 8) / 255f,
			b = (float)(color & 0xFF) / 255f,
			a = 1f
		};
	}

	public static int ColorToInt(Color color)
	{
		return ((int)(color.r * 255f) << 16) + ((int)(color.g * 255f) << 8) + (int)(color.b * 255f);
	}

	public static void KKCLIIOIKAD()
	{
		if (SystemInfo.deviceType != DeviceType.Handheld)
		{
			switch (SystemInfo.deviceType)
			{
			case DeviceType.Unknown:
				GLDLEHNOHIM = FHOMOMIOLAK.UNKNOWN;
				break;
			case DeviceType.Console:
				GLDLEHNOHIM = FHOMOMIOLAK.CONSOLE;
				break;
			case DeviceType.Desktop:
				GLDLEHNOHIM = FHOMOMIOLAK.DESKTOP;
				break;
			}
			return;
		}
		float num = ((Screen.width <= Screen.height) ? ((float)Screen.height) : ((float)Screen.width));
		if (num < 800f)
		{
			GLDLEHNOHIM = FHOMOMIOLAK.PHONE;
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			float f = (float)Screen.width / Screen.dpi;
			float f2 = (float)Screen.height / Screen.dpi;
			float num2 = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
			if (num2 >= 6.5f)
			{
				GLDLEHNOHIM = FHOMOMIOLAK.TABLET;
			}
		}
		GLDLEHNOHIM = FHOMOMIOLAK.PHONE;
	}

	public static FHOMOMIOLAK BBGLNMLEOLG()
	{
		if (GLDLEHNOHIM == FHOMOMIOLAK.NONE)
		{
			KKCLIIOIKAD();
		}
		return GLDLEHNOHIM;
	}

	public static bool JGLKJECFHED()
	{
		return BBGLNMLEOLG() == FHOMOMIOLAK.TABLET;
	}

	public static bool ONGKCNAICGI()
	{
		return BBGLNMLEOLG() == FHOMOMIOLAK.PHONE;
	}

	public static bool JDIKHMODKKF()
	{
		return Application.isEditor && !Application.isPlaying;
	}

	public static string NBFOIPELHOI()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
			return "standaloneMacOSX";
		case RuntimePlatform.OSXPlayer:
			return "standaloneMacOSX";
		case RuntimePlatform.WindowsPlayer:
			return "standaloneWindows";
		case RuntimePlatform.WindowsEditor:
			return "standaloneWindows";
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		case RuntimePlatform.Android:
			return "android";
		default:
			return string.Empty;
		}
	}
}
