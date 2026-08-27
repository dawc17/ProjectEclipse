using System;
using UnityEngine;

internal class VerticalLayout : IDisposable
{
	public VerticalLayout(params GUILayoutOption[] LHONCAIFCAF)
	{
		GUILayout.BeginVertical(LHONCAIFCAF);
	}

	public VerticalLayout(GUIStyle KIGNIBIMLKK)
	{
		GUILayout.BeginVertical(KIGNIBIMLKK);
	}

	public void Dispose()
	{
		GUILayout.EndHorizontal();
	}
}
