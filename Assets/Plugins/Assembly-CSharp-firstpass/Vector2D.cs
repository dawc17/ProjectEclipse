using UnityEngine;

public class Vector2D
{
	public static void FOJIKCBCAHC(Vector2 MLGFPMDKOHD, Vector2 DMMNCDKPCCI, EquationLine EGKHHBMCGMK)
	{
		float num = Vector2.Distance(MLGFPMDKOHD, DMMNCDKPCCI);
		EGKHHBMCGMK.LHBNIMGFKIB = (MLGFPMDKOHD.y - DMMNCDKPCCI.y) / num;
		EGKHHBMCGMK.AAOIAEJJINO = (DMMNCDKPCCI.x - MLGFPMDKOHD.x) / num;
		EGKHHBMCGMK.ILHDJDNPFKH = 0f - (EGKHHBMCGMK.LHBNIMGFKIB * MLGFPMDKOHD.x + EGKHHBMCGMK.AAOIAEJJINO * MLGFPMDKOHD.y);
	}

	public static EquationLine FOJIKCBCAHC(Vector2 MLGFPMDKOHD, Vector2 DMMNCDKPCCI)
	{
		EquationLine kEDCEHBPOIM = null;
		FOJIKCBCAHC(MLGFPMDKOHD, DMMNCDKPCCI, kEDCEHBPOIM);
		return kEDCEHBPOIM;
	}

	private static float KHMHNLKAENC(float value)
	{
		return (!(value < 0f)) ? value : (0f - value);
	}

	public static bool PDFOIOBDHJE(Vector2 HICHONIJHKL, Vector2 LNPFHLPCLOP, float KLDFJGIKIHG, Vector2 NMAJNHKJJEM, Vector2 ONNJMGGPHEL, float MGCKDDGGCBI, ref Vector2 DNJCFHNICBH, ref Vector2 LOFMLNLKFLB, EquationLine JHMHDMOADMA, EquationLine GIMOMPLMEJH)
	{
		Vector2 vector = HICHONIJHKL;
		Vector2 vector2 = LNPFHLPCLOP;
		Vector2 vector3 = NMAJNHKJJEM;
		Vector2 vector4 = ONNJMGGPHEL;
		float num = KLDFJGIKIHG + MGCKDDGGCBI;
		if (num == 0f)
		{
			if (CAJCGEMHDLN(vector3, vector4, vector, vector2, DNJCFHNICBH))
			{
				LOFMLNLKFLB = DNJCFHNICBH;
				return true;
			}
			return false;
		}
		EquationLine kEDCEHBPOIM = ((GIMOMPLMEJH == null) ? FOJIKCBCAHC(NMAJNHKJJEM, ONNJMGGPHEL) : GIMOMPLMEJH);
		float num2 = kEDCEHBPOIM.LHBNIMGFKIB * vector.x + kEDCEHBPOIM.AAOIAEJJINO * vector.y + kEDCEHBPOIM.ILHDJDNPFKH;
		float num3 = kEDCEHBPOIM.LHBNIMGFKIB * vector2.x + kEDCEHBPOIM.AAOIAEJJINO * vector2.y + kEDCEHBPOIM.ILHDJDNPFKH;
		if (0f <= num2 * num3 && num < KHMHNLKAENC(num2) && num < KHMHNLKAENC(num3))
		{
			return false;
		}
		EquationLine kEDCEHBPOIM2 = ((JHMHDMOADMA == null) ? FOJIKCBCAHC(HICHONIJHKL, LNPFHLPCLOP) : JHMHDMOADMA);
		float num4 = kEDCEHBPOIM2.LHBNIMGFKIB * vector3.x + kEDCEHBPOIM2.AAOIAEJJINO * vector3.y + kEDCEHBPOIM2.ILHDJDNPFKH;
		float num5 = kEDCEHBPOIM2.LHBNIMGFKIB * vector4.x + kEDCEHBPOIM2.AAOIAEJJINO * vector4.y + kEDCEHBPOIM2.ILHDJDNPFKH;
		if (0f <= num4 * num5 && num < KHMHNLKAENC(num4) && num < KHMHNLKAENC(num5))
		{
			return false;
		}
		if (num4 * num5 < 0f && num2 * num3 < 0f)
		{
			float num6 = num4 / (num4 - num5);
			DNJCFHNICBH = vector4 - vector3;
			DNJCFHNICBH *= num6;
			DNJCFHNICBH += vector3;
			LOFMLNLKFLB = DNJCFHNICBH;
			return true;
		}
		if (DCPBKLDKIHD(num2, num, kEDCEHBPOIM, vector, ref LOFMLNLKFLB, vector3, vector4))
		{
			DNJCFHNICBH = vector;
			return true;
		}
		if (DCPBKLDKIHD(num3, num, kEDCEHBPOIM, vector2, ref LOFMLNLKFLB, vector3, vector4))
		{
			DNJCFHNICBH = vector2;
			return true;
		}
		if (DCPBKLDKIHD(num4, num, kEDCEHBPOIM2, vector3, ref LOFMLNLKFLB, vector, vector2))
		{
			DNJCFHNICBH = vector3;
			LOFMLNLKFLB = vector3;
			return true;
		}
		if (DCPBKLDKIHD(num5, num, kEDCEHBPOIM2, vector4, ref LOFMLNLKFLB, vector, vector2))
		{
			DNJCFHNICBH = vector4;
			LOFMLNLKFLB = vector4;
			return true;
		}
		return false;
	}

	public static bool DCPBKLDKIHD(float OIOMNNFMDOO, float JBLFLFOGDFI, EquationLine EGKHHBMCGMK, Vector2 NAAPALOFBCI, ref Vector2 CIMNFFDLIJO, Vector2 ILENLCMAMBH, Vector2 PCLFFOBJJFO)
	{
		if (KHMHNLKAENC(OIOMNNFMDOO) <= JBLFLFOGDFI)
		{
			CIMNFFDLIJO.x = NAAPALOFBCI.x - OIOMNNFMDOO * EGKHHBMCGMK.LHBNIMGFKIB;
			CIMNFFDLIJO.y = NAAPALOFBCI.y - OIOMNNFMDOO * EGKHHBMCGMK.AAOIAEJJINO;
			return (((PCLFFOBJJFO.x <= CIMNFFDLIJO.x && CIMNFFDLIJO.x <= ILENLCMAMBH.x) || (ILENLCMAMBH.x <= CIMNFFDLIJO.x && CIMNFFDLIJO.x <= PCLFFOBJJFO.x)) && ((PCLFFOBJJFO.y <= CIMNFFDLIJO.y && CIMNFFDLIJO.y <= ILENLCMAMBH.y) || (ILENLCMAMBH.y <= CIMNFFDLIJO.y && CIMNFFDLIJO.y <= PCLFFOBJJFO.y))) || (NAAPALOFBCI.x - ILENLCMAMBH.x) * (NAAPALOFBCI.x - ILENLCMAMBH.x) + (NAAPALOFBCI.y - ILENLCMAMBH.y) * (NAAPALOFBCI.y - ILENLCMAMBH.y) <= JBLFLFOGDFI * JBLFLFOGDFI || (NAAPALOFBCI.x - PCLFFOBJJFO.x) * (NAAPALOFBCI.x - PCLFFOBJJFO.x) + (NAAPALOFBCI.y - PCLFFOBJJFO.y) * (NAAPALOFBCI.y - PCLFFOBJJFO.y) <= JBLFLFOGDFI * JBLFLFOGDFI;
		}
		return false;
	}

	public static bool CAJCGEMHDLN(Vector2 IEKADOOKFKG, Vector2 LDKCOIHONPG, Vector2 AOOIPCJPALH, Vector2 IGMCGOFHHCJ, Vector2 DCJLKCFKCOM)
	{
		if ((IEKADOOKFKG.x == LDKCOIHONPG.x && IEKADOOKFKG.y == LDKCOIHONPG.y) || (AOOIPCJPALH.x == IGMCGOFHHCJ.x && AOOIPCJPALH.y == IGMCGOFHHCJ.y))
		{
			return false;
		}
		float num = LDKCOIHONPG.x - IEKADOOKFKG.x;
		float num2 = LDKCOIHONPG.y - IEKADOOKFKG.y;
		float num3 = IGMCGOFHHCJ.x - AOOIPCJPALH.x;
		float num4 = IGMCGOFHHCJ.y - AOOIPCJPALH.y;
		float num5 = IEKADOOKFKG.x - AOOIPCJPALH.x;
		float num6 = IEKADOOKFKG.y - AOOIPCJPALH.y;
		float num7 = num4 * num - num3 * num2;
		float num8 = num3 * num6 - num4 * num5;
		float num9 = num * num6 - num2 * num5;
		if (num7 == 0f)
		{
			if (num8 != 0f && num9 != 0f)
			{
				return false;
			}
			float x;
			float x2;
			if (IEKADOOKFKG.x < LDKCOIHONPG.x)
			{
				x = IEKADOOKFKG.x;
				x2 = LDKCOIHONPG.x;
			}
			else
			{
				x = LDKCOIHONPG.x;
				x2 = IEKADOOKFKG.x;
			}
			float x3;
			float x4;
			if (AOOIPCJPALH.x < IGMCGOFHHCJ.x)
			{
				x3 = AOOIPCJPALH.x;
				x4 = IGMCGOFHHCJ.x;
			}
			else
			{
				x3 = IGMCGOFHHCJ.x;
				x4 = AOOIPCJPALH.x;
			}
			if (x > x4 || x3 > x2)
			{
				return false;
			}
			if (IEKADOOKFKG.y < LDKCOIHONPG.y)
			{
				x = IEKADOOKFKG.y;
				x2 = LDKCOIHONPG.y;
			}
			else
			{
				x = LDKCOIHONPG.y;
				x2 = IEKADOOKFKG.y;
			}
			if (AOOIPCJPALH.y < IGMCGOFHHCJ.y)
			{
				x3 = AOOIPCJPALH.y;
				x4 = IGMCGOFHHCJ.y;
			}
			else
			{
				x3 = IGMCGOFHHCJ.y;
				x4 = AOOIPCJPALH.y;
			}
			if (x > x4 || x3 > x2)
			{
				return false;
			}
			num7 = 1f;
		}
		num8 /= num7;
		num9 /= num7;
		if (num8 >= 0f && num8 <= 1f && num9 >= 0f && num9 <= 1f)
		{
			DCJLKCFKCOM.x = IEKADOOKFKG.x + num8 * (LDKCOIHONPG.x - IEKADOOKFKG.x);
			DCJLKCFKCOM.y = IEKADOOKFKG.y + num8 * (LDKCOIHONPG.y - IEKADOOKFKG.y);
			return true;
		}
		return false;
	}

	public static float GetAngle2DDegreeSigned(Vector2 KKIKIDNALOL, Vector3 NMADGDHJBGB)
	{
		return GetAngle2DRadianSigned(KKIKIDNALOL, NMADGDHJBGB) * 57.29578f;
	}

	public static float GetAngle2DRadianSigned(Vector2 LHBNIMGFKIB, Vector2 AAOIAEJJINO)
	{
		float num = LHBNIMGFKIB.x * AAOIAEJJINO.y - LHBNIMGFKIB.y * AAOIAEJJINO.x;
		float num2 = LHBNIMGFKIB.x * AAOIAEJJINO.x + LHBNIMGFKIB.y * AAOIAEJJINO.y;
		float num3 = 1f / Mathf.Sqrt(num * num + num2 * num2);
		return Mathf.Atan2(num * num3, num2 * num3);
	}
}
