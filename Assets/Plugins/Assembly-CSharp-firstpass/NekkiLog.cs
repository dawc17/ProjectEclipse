using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class NekkiLog : MonoBehaviour
{
	public enum FKOLAIBBDOL : byte
	{
		Log = 0,
		Warning = 1,
		Error = 2,
		Exception = 3,
		Assert = 4
	}

	private static string KHHDGDJEOMP = string.Empty;

	private static StringBuilder _items = new StringBuilder();

	private static DateTime _now;

	private static NekkiLog EDAPJLKMFPC;

	private static bool FOCMMGJPNKB;

	private static string _fileName = "log";

	private static bool MOIBEJDDBLK = true;

	private static bool HHPCLONKLLF = true;

	private static bool ODFLEIBIPMB = true;

	private static FKOLAIBBDOL FFJAMOKDGLA = FKOLAIBBDOL.Log;

	private bool IBBMAINEMIM;

	private static readonly object LOCKER = new object();

	private static string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
	}

	private static string NGMCEBMMKHP
	{
		get
		{
			return DEIEDODNANN();
		}
	}

	private static string EPDMGFELIMC()
	{
		if (!HHPCLONKLLF)
		{
			return string.Format("{0}.nekkilog", _fileName);
		}
		return string.Format("{3} {0}.{1}.{2}.nekkilog", _now.Year.ToString("0000"), _now.Month.ToString("00"), _now.Day.ToString("00"), _fileName);
	}

	private static string DEIEDODNANN()
	{
		if (string.IsNullOrEmpty(KHHDGDJEOMP))
		{
			return Path.Combine(Application.persistentDataPath, EPDMGFELIMC());
		}
		return Path.Combine(KHHDGDJEOMP, EPDMGFELIMC());
	}

	public static void Init(string KOBDDMHGOPJ, string PMFEIPCHENB, bool DHFCJMJBFDP, bool AJDGNMMKEBE, bool AODCELGDHPO, FKOLAIBBDOL JJAFNMOOCKJ)
	{
		if ((bool)EDAPJLKMFPC)
		{
			EDAPJLKMFPC.Write();
		}
		KHHDGDJEOMP = KOBDDMHGOPJ.TrimEnd('/').TrimEnd('\\');
		FFJAMOKDGLA = JJAFNMOOCKJ;
		HHPCLONKLLF = AJDGNMMKEBE;
		ODFLEIBIPMB = AODCELGDHPO;
		FOCMMGJPNKB = DHFCJMJBFDP;
		if (!string.IsNullOrEmpty(PMFEIPCHENB))
		{
			_fileName = PMFEIPCHENB;
		}
		KJGIHCKDOLO();
		MOIBEJDDBLK = false;
		if (!Directory.Exists(KOBDDMHGOPJ))
		{
			Directory.CreateDirectory(KOBDDMHGOPJ);
		}
		FileInfo fileInfo = new FileInfo(DEIEDODNANN());
		if (!fileInfo.Exists)
		{
			FileStream fileStream = fileInfo.Create();
			fileStream.Close();
		}
	}

	private static void KJGIHCKDOLO()
	{
		if (!EDAPJLKMFPC)
		{
			GameObject gameObject = new GameObject("_log");
			EDAPJLKMFPC = gameObject.AddComponent<NekkiLog>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			EDAPJLKMFPC.Update();
			EDAPJLKMFPC.StartCoroutine(EDAPJLKMFPC.GGGEHAGCLGC());
			Application.logMessageReceived += _unityLogCallback;
		}
	}

	private void OnDestroy()
	{
		Stop();
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(GGGEHAGCLGC());
	}

	private static void _unityLogCallback(string IOFGGOCEIAM, string HHLCHHIFDCM, LogType LFLGCDNKNJI)
	{
		if (FOCMMGJPNKB)
		{
			switch (LFLGCDNKNJI)
			{
			case LogType.Error:
				Error(IOFGGOCEIAM, HHLCHHIFDCM);
				break;
			case LogType.Assert:
				Assert(IOFGGOCEIAM, HHLCHHIFDCM);
				break;
			case LogType.Warning:
				Warning(IOFGGOCEIAM, HHLCHHIFDCM);
				break;
			case LogType.Log:
				Log(IOFGGOCEIAM, HHLCHHIFDCM);
				break;
			case LogType.Exception:
				COHEDILAHFD(IOFGGOCEIAM, HHLCHHIFDCM);
				break;
			}
		}
	}

	private void Update()
	{
		if (ODFLEIBIPMB || HHPCLONKLLF)
		{
			_now = DateTime.Now;
		}
	}

	public static void Log(object LIOGIBJBHAH, string HHLCHHIFDCM = null)
	{
		FCLECKJKEII(FKOLAIBBDOL.Log, LIOGIBJBHAH, HHLCHHIFDCM);
	}

	public static void Warning(object LIOGIBJBHAH, string HHLCHHIFDCM = null)
	{
		FCLECKJKEII(FKOLAIBBDOL.Warning, LIOGIBJBHAH, HHLCHHIFDCM);
	}

	public static void Error(object LIOGIBJBHAH, string HHLCHHIFDCM = null)
	{
		FCLECKJKEII(FKOLAIBBDOL.Error, LIOGIBJBHAH, HHLCHHIFDCM);
	}

	public static void Exception(Exception MPFFFAOGBJE)
	{
		FCLECKJKEII(FKOLAIBBDOL.Exception, MPFFFAOGBJE.Message, MPFFFAOGBJE.StackTrace);
	}

	private static void COHEDILAHFD(object LIOGIBJBHAH, string HHLCHHIFDCM)
	{
		FCLECKJKEII(FKOLAIBBDOL.Exception, LIOGIBJBHAH, HHLCHHIFDCM);
	}

	public static void Assert(object LIOGIBJBHAH, string HHLCHHIFDCM = null)
	{
		FCLECKJKEII(FKOLAIBBDOL.Assert, LIOGIBJBHAH, HHLCHHIFDCM);
	}

	public static void Stop()
	{
		EDAPJLKMFPC.Write();
		MOIBEJDDBLK = true;
	}

	private static string LEBKHJNLJBE(FKOLAIBBDOL GNLOCMLBNHF, object IOFGGOCEIAM, string HHLCHHIFDCM = null)
	{
		object obj = ((!string.IsNullOrEmpty(HHLCHHIFDCM)) ? string.Format("{0} at: {1}", IOFGGOCEIAM, HHLCHHIFDCM) : IOFGGOCEIAM);
		if (!ODFLEIBIPMB)
		{
			return string.Format("{0}\n", obj);
		}
		return string.Format("[{3}] [{0}:{1}:{2}] {4}\n", _now.Hour.ToString("00"), _now.Minute.ToString("00"), _now.Second.ToString("00"), GNLOCMLBNHF, obj);
	}

	private static void FCLECKJKEII(FKOLAIBBDOL GNLOCMLBNHF, object LIOGIBJBHAH, string HHLCHHIFDCM = null)
	{
		if (MOIBEJDDBLK)
		{
			AdvLog.LOPHFKMOPAA("you must init log system first!");
		}
		else if ((int)GNLOCMLBNHF >= (int)FFJAMOKDGLA)
		{
			KJGIHCKDOLO();
			lock (LOCKER)
			{
				_items.Append(LEBKHJNLJBE(GNLOCMLBNHF, LIOGIBJBHAH, HHLCHHIFDCM));
			}
		}
	}

	private IEnumerator GGGEHAGCLGC()
	{
		while ((bool)base.transform)
		{
			yield return new WaitForSeconds(1f);
			Write();
		}
	}

	private void Write()
	{
		if (IBBMAINEMIM)
		{
			return;
		}
		IBBMAINEMIM = true;
		string value;
		lock (LOCKER)
		{
			value = _items.ToString();
			_items = new StringBuilder();
		}
		try
		{
			FileInfo fileInfo = new FileInfo(DEIEDODNANN());
			if (!fileInfo.Exists)
			{
				FileStream fileStream = fileInfo.Create();
				fileStream.Close();
			}
			StreamWriter streamWriter = fileInfo.AppendText();
			streamWriter.Write(value);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			MonoBehaviour.print(ex.Message);
		}
		IBBMAINEMIM = false;
	}
}
