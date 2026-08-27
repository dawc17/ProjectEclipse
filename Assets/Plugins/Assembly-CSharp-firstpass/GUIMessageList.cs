using System.Collections.Generic;
using UnityEngine;

public class GUIMessageList
{
	private List<string> messages = new List<string>();

	private Vector2 scrollPos;

	public void MCAIPGEPMDE()
	{
		MCAIPGEPMDE(Screen.width, 0f);
	}

	public void MCAIPGEPMDE(float IIMDMHKPJJN, float JKKFHOLODHB)
	{
		scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(JKKFHOLODHB));
		for (int i = 0; i < messages.Count; i++)
		{
			GUILayout.Label(messages[i], GUILayout.MinWidth(IIMDMHKPJJN));
		}
		GUILayout.EndScrollView();
	}

	public void Add(string CKEHOEGLMBM)
	{
		messages.Add(CKEHOEGLMBM);
		scrollPos = new Vector2(scrollPos.x, float.MaxValue);
	}

	public void Clear()
	{
		messages.Clear();
	}
}
