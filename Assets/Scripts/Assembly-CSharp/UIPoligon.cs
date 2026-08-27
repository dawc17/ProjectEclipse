using System.Collections.Generic;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

public class UIPoligon : UIFigure
{
	[SerializeField]
	private List<Vector2> _Points = new List<Vector2>();

	[SerializeField]
	private bool _UseTriangleFan = true;

	public List<Vector2> CMJMECFMIKP
	{
		get
		{
			return get_Points();
		}
	}

	public List<Vector2> get_Points()
	{
		return _Points;
	}

	protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
	{
		base.OnPopulateMesh(DHJBOKKAOJK);
		List<UIVertex> list = new List<UIVertex>(_Points.Count);
		for (int i = 0; i < _Points.Count; i++)
		{
			list.Add(AddVertex(_Points[i]));
		}
		DHJBOKKAOJK.Clear();
		DHJBOKKAOJK.AddUIVertexStream(list, (!_UseTriangleFan) ? FigureTopology.AMJOJPPFIEB(list.Count - 2) : FigureTopology.NGPPLGNODNB(list.Count - 2));
	}

	private UIVertex AddVertex(Vector3 GIAEPIIIMDH)
	{
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.position = new Vector2(GIAEPIIIMDH.x, GIAEPIIIMDH.y);
		simpleVert.uv0 = new Vector2(0f, 0f);
		simpleVert.color = color;
		return simpleVert;
	}
}
