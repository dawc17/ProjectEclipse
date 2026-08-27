using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DrawFunctions
{
	public static void CJOMPMCKCJP(VertexHelper KMONCKJAPBB, Vector2 GIAEPIIIMDH, Vector2 LPEMPCEJFIN, float AMNCLCPADOO, float IFIOLDFCLIE, int KFDMFGBLBCO, Color OHJKNABLCMF)
	{
		KMONCKJAPBB.Clear();
		List<UIVertex> list = new List<UIVertex>();
		float num = (IFIOLDFCLIE - AMNCLCPADOO) / (float)KFDMFGBLBCO;
		list.Add(FJHDOJHFEJH(GIAEPIIIMDH, new Vector2(0.5f, 0.5f), OHJKNABLCMF));
		for (int i = 0; i <= KFDMFGBLBCO; i++)
		{
			float f = AMNCLCPADOO + (float)i * num;
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			float x = num2 * LPEMPCEJFIN.x + GIAEPIIIMDH.x;
			float y = num3 * LPEMPCEJFIN.y + GIAEPIIIMDH.y;
			list.Add(FJHDOJHFEJH(FGFOGDLPAIC: new Vector2(num2 * 0.5f + 0.5f, num3 * 0.5f + 0.5f), GIAEPIIIMDH: new Vector2(x, y), OHJKNABLCMF: OHJKNABLCMF));
		}
		KMONCKJAPBB.AddUIVertexStream(list, FigureTopology.NGPPLGNODNB(KFDMFGBLBCO));
	}

	public static void GAGFKBFLHHE(VertexHelper DHJBOKKAOJK, Vector2 GIAEPIIIMDH, Vector2 LPEMPCEJFIN, float JGAPNGHPJGJ, float AMNCLCPADOO, float IFIOLDFCLIE, int KFDMFGBLBCO, Color OHJKNABLCMF, Color MIHJJEICDDD)
	{
		DHJBOKKAOJK.Clear();
		List<UIVertex> list = new List<UIVertex>();
		float num = (IFIOLDFCLIE - AMNCLCPADOO) / (float)KFDMFGBLBCO;
		Vector2 vector = new Vector2(LPEMPCEJFIN.x - JGAPNGHPJGJ, LPEMPCEJFIN.y - JGAPNGHPJGJ);
		for (int i = 0; i <= KFDMFGBLBCO; i++)
		{
			float f = AMNCLCPADOO + (float)i * num;
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			Color oHJKNABLCMF = Color.Lerp(OHJKNABLCMF, MIHJJEICDDD, (float)i / (float)KFDMFGBLBCO);
			Vector2 gIAEPIIIMDH = new Vector2(num2 * LPEMPCEJFIN.x + GIAEPIIIMDH.x, num3 * LPEMPCEJFIN.y + GIAEPIIIMDH.y);
			Vector2 fGFOGDLPAIC = new Vector2(num2 * 0.5f + 0.5f, num3 * 0.5f + 0.5f);
			list.Add(FJHDOJHFEJH(gIAEPIIIMDH, fGFOGDLPAIC, oHJKNABLCMF));
			Vector2 gIAEPIIIMDH2 = new Vector2(num2 * vector.x + GIAEPIIIMDH.x, num3 * vector.y + GIAEPIIIMDH.y);
			Vector2 vector2 = new Vector2(1f - JGAPNGHPJGJ / LPEMPCEJFIN.x, 1f - JGAPNGHPJGJ / LPEMPCEJFIN.y);
			Vector2 fGFOGDLPAIC2 = new Vector2(num2 * vector2.x * 0.5f + 0.5f, num3 * vector2.y * 0.5f + 0.5f);
			list.Add(FJHDOJHFEJH(gIAEPIIIMDH2, fGFOGDLPAIC2, oHJKNABLCMF));
		}
		DHJBOKKAOJK.AddUIVertexStream(list, FigureTopology.AMJOJPPFIEB(KFDMFGBLBCO * 2));
	}

	public static void FBFOFHOLLKI(VertexHelper DHJBOKKAOJK, List<Vector2> LGKJBIEDKBO, float JGAPNGHPJGJ, Color OHJKNABLCMF, bool EEIKKKPLPAA = false)
	{
		DHJBOKKAOJK.Clear();
		List<UIVertex> list = new List<UIVertex>();
		List<Vector2> list2 = new List<Vector2>();
		if (LGKJBIEDKBO.Count < 2)
		{
			return;
		}
		float num = LGKJBIEDKBO[LGKJBIEDKBO.Count - 1].x - LGKJBIEDKBO[0].x;
		for (int i = 1; i < LGKJBIEDKBO.Count; i++)
		{
			list2.Add(LGKJBIEDKBO[i] - LGKJBIEDKBO[i - 1]);
			int index = i - 1;
			Vector2 vector = new Vector2(0f - list2[i - 1].y, list2[i - 1].x);
			list2[index] = vector.normalized;
			if (list2[i - 1].magnitude == 0f && i > 1)
			{
				list2[i - 1] = list2[i - 2];
			}
		}
		for (int num2 = LGKJBIEDKBO.Count - 2; num2 > 0; num2--)
		{
			if (list2[num2 - 1].magnitude == 0f && num2 > 0)
			{
				list2[num2 - 1] = list2[num2];
			}
		}
		int num3 = 1;
		for (int j = 0; j < LGKJBIEDKBO.Count; j++)
		{
			Vector2 vector2 = list2[(j <= 0) ? j : (j - 1)];
			Vector2 vector3 = list2[(j >= LGKJBIEDKBO.Count - 1) ? (j - 1) : j];
			Vector2 vector4 = LGKJBIEDKBO[j];
			float x = ((!EEIKKKPLPAA) ? ((float)((j % 2 == 0) ? 1 : 0)) : ((vector4.x - LGKJBIEDKBO[0].x) / num));
			Vector2 fGFOGDLPAIC = new Vector2(x, 0f);
			float num4 = Mathf.Abs(Mathf.Cos((float)Math.PI / 180f * Vector2.Angle(vector2, vector3) / 2f));
			Vector2 vector5 = num3 * (vector2 + vector3).normalized / num4 * JGAPNGHPJGJ;
			if (vector5.magnitude == 0f)
			{
				vector5 = num3 * vector2 * JGAPNGHPJGJ;
				num3 *= -1;
			}
			Vector2 fGFOGDLPAIC2 = new Vector2(x, 1f);
			list.Add(FJHDOJHFEJH(vector4 - vector5 / 2f, fGFOGDLPAIC, OHJKNABLCMF));
			list.Add(FJHDOJHFEJH(vector4 + vector5 / 2f, fGFOGDLPAIC2, OHJKNABLCMF));
		}
		DHJBOKKAOJK.AddUIVertexStream(list, FigureTopology.AMJOJPPFIEB((LGKJBIEDKBO.Count - 1) * 2));
	}

	private static UIVertex FJHDOJHFEJH(Vector2 GIAEPIIIMDH, Vector2 FGFOGDLPAIC, Color OHJKNABLCMF)
	{
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.position = GIAEPIIIMDH;
		simpleVert.uv0 = FGFOGDLPAIC;
		simpleVert.color = OHJKNABLCMF;
		return simpleVert;
	}
}
