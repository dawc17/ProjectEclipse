using System.IO;
using UnityEngine;

public class NekkiZipTest : MonoBehaviour
{
	private string INDMCOHEGMB;

	private void Log(object NBEKIHKMJJG)
	{
		INDMCOHEGMB = INDMCOHEGMB + NBEKIHKMJJG.ToString() + "\n";
	}

	private void Start()
	{
		Log("-init");
		string text = Application.persistentDataPath + "/test.txt";
		string text2 = Application.persistentDataPath + "/test.zip";
		Log(text);
		Log(text2);
		File.WriteAllText(text, "alloha ha ha");
		Compressor.DDIDIMMDPDN(text2, text);
		Log("-zipping");
		File.Delete(text);
		Log("source file is exsists = " + File.Exists(text));
		Log("-unzipping");
		Compressor.OGANCHANAMK(text2, text);
		Log("source file is exsists = " + File.Exists(text));
		string text3 = File.ReadAllText(text);
		Log("content = '" + text3 + "'");
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), INDMCOHEGMB);
	}
}
