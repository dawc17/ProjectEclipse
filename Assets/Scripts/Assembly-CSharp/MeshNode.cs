using System.Collections.Generic;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

public class MeshNode
{
	public int[] Triangles;

	public Vector3[] Vertices;

	private List<ModelNode> MFONEBKEMAD = new List<ModelNode>();

	private List<int> _TrianglesList = new List<int>();

	public void LPEPFNNPCBK(ModelNode FJKBEFJGAHF, ModelNode GMHJFPCFFMM, ModelNode PNMPELDMCJF)
	{
		int item = ENNKELDABMG(FJKBEFJGAHF);
		int item2 = ENNKELDABMG(GMHJFPCFFMM);
		int item3 = ENNKELDABMG(PNMPELDMCJF);
		_TrianglesList.Add(item);
		_TrianglesList.Add(item2);
		_TrianglesList.Add(item3);
	}

	private int ENNKELDABMG(ModelNode MEEAKLDGLDF)
	{
		if (MFONEBKEMAD.Contains(MEEAKLDGLDF))
		{
			return MFONEBKEMAD.IndexOf(MEEAKLDGLDF);
		}
		MFONEBKEMAD.Add(MEEAKLDGLDF);
		return MFONEBKEMAD.Count - 1;
	}

	public void Init()
	{
		Triangles = _TrianglesList.ToArray();
		Vertices = new Vector3[MFONEBKEMAD.Count];
		_TrianglesList = null;
	}

	public void Render(float alpha)
	{
		for (int i = 0; i < Vertices.Length; i++)
		{
			float x;
			float y;
			float z;
			FightInterpolation.SamplePosition(MFONEBKEMAD[i], alpha, out x, out y, out z);
			Vertices[i].Set(x, y, 0f);
		}
	}
}
