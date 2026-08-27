using UnityEngine;

public class SysDlg : MonoBehaviour
{
	private static string _message;

	private static bool _exit;

	private static SysDlg EDAPJLKMFPC;

	private static Texture2D _t;

	public static void Show(string LIOGIBJBHAH, bool ONBPFALHBAN)
	{
		if (!_t)
		{
			_t = new Texture2D(1, 1);
			_t.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.8f));
		}
		_message = LIOGIBJBHAH;
		_exit = ONBPFALHBAN;
		if (!EDAPJLKMFPC)
		{
			EDAPJLKMFPC = new GameObject("_message").AddComponent<SysDlg>();
		}
		Time.timeScale = 0f;
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			if (touches[i].phase == TouchPhase.Ended && new Rect((float)Screen.width / 10f, (float)Screen.height / 8f * 6f, (float)Screen.width / 10f * 8f, (float)Screen.height / 8f).Contains(touches[i].position))
			{
				Close();
			}
		}
	}

	private void Close()
	{
		if (_exit)
		{
			Application.Quit();
			return;
		}
		Time.timeScale = 1f;
		Object.Destroy(base.gameObject);
	}

	private void OnGUI()
	{
		GUI.depth = -100000;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _t);
		GUI.Label(new Rect((float)Screen.width / 10f, (float)Screen.height / 8f, (float)Screen.width / 10f * 8f, (float)Screen.height / 8f * 8f), string.Format("<color=white><size={1}>{0}</size></color>", _message, ((Screen.width <= Screen.height) ? Screen.height : Screen.width) / 40));
		GUI.skin.label.alignment = alignment;
		if (GUI.Button(new Rect((float)Screen.width / 10f, (float)Screen.height / 8f * 6f, (float)Screen.width / 10f * 8f, (float)Screen.height / 8f), "OK"))
		{
			Close();
		}
	}
}
