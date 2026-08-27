using System.Collections;
using System.Text;
using UnityEngine;

public class SendMeLog : MonoBehaviour
{
	private static string _email;

	private static bool _active;

	private static GameObject _obj;

	private static readonly StringBuilder Log = new StringBuilder();

	public static void Init(string KCCCJAINPIG, float HBIAPEIOOHI = 0f)
	{
		_email = KCCCJAINPIG;
		if (!_obj)
		{
			Application.logMessageReceived += HNDEILJJGJB;
			_active = true;
			_obj = new GameObject("_sendMeLog", typeof(SendMeLog));
			Object.DontDestroyOnLoad(_obj);
		}
		if (HBIAPEIOOHI > 0.1f)
		{
			SendMeLog component = _obj.GetComponent<SendMeLog>();
			component.StartCoroutine(component.LKJNBAFMFKG(HBIAPEIOOHI));
		}
	}

	private static void HNDEILJJGJB(string IOFGGOCEIAM, string BPANNMHCGBC, LogType LFLGCDNKNJI)
	{
		if (_active)
		{
			Log.Append(string.Format("[{0}] {1}{2}\n", LFLGCDNKNJI, IOFGGOCEIAM, (!string.IsNullOrEmpty(BPANNMHCGBC)) ? string.Format(" ({0})", BPANNMHCGBC) : string.Empty));
		}
	}

	public static void Stop()
	{
		Send();
		_active = false;
	}

	private IEnumerator LKJNBAFMFKG(float time)
	{
		yield return new WaitForSeconds(time);
		Stop();
	}

	private static void Send()
	{
		if (_active)
		{
			string text = WWW.EscapeURL(string.Format("Log from [{0}:{1}:{2}] {3}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier)).Replace("+", "%20");
			string text2 = WWW.EscapeURL(Log.ToString()).Replace("+", "%20");
			string url = "mailto:" + _email + "?subject=" + text + "&body=" + text2;
			Application.OpenURL(url);
		}
	}
}
