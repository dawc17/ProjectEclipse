using System;
using UnityEngine;

internal class HorizontalLayout : IDisposable
{
	public HorizontalLayout(params GUILayoutOption[] LHONCAIFCAF)
	{
		GUILayout.BeginHorizontal(LHONCAIFCAF);
	}

	public void Dispose()
	{
		GUILayout.EndHorizontal();
	}
}
