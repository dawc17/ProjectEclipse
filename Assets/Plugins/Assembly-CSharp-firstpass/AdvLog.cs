using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class AdvLog
{
	public enum NKKBMLLIKHH
	{
		Log = 0,
		Warn = 1,
		Error = 2
	}

	private static bool _logNow;

	private static string _filePath;

	public static bool LGJMHNJEPDK
	{
		get
		{
			return PNBOAKLOFCE();
		}
		set
		{
			set_LogNow(value);
		}
	}

	public static bool PNBOAKLOFCE()
	{
		return _logNow;
	}

	public static void set_LogNow(bool value)
	{
		if (_logNow != value)
		{
			_logNow = value;
			if (_logNow)
			{
				Init();
			}
		}
	}

	private static void Init()
	{
		if (string.IsNullOrEmpty(_filePath))
		{
			Application.logMessageReceived += ApplicationLogSubsctiption;
			_filePath = Path.Combine(GlobalPaths.MNACDIFKBDG(), string.Format("log_{0}.log", DateTime.Now.ToString("yy_MM_dd__hh_mm_ss")));
			File.WriteAllText(_filePath, string.Empty);
		}
	}

	private static void ApplicationLogSubsctiption(string IOFGGOCEIAM, string HHLCHHIFDCM, LogType LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case LogType.Log:
			CNBFNEHEDKP(NKKBMLLIKHH.Log, IOFGGOCEIAM + " - " + HHLCHHIFDCM);
			break;
		case LogType.Warning:
			CNBFNEHEDKP(NKKBMLLIKHH.Warn, IOFGGOCEIAM + " - " + HHLCHHIFDCM);
			break;
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			CNBFNEHEDKP(NKKBMLLIKHH.Error, IOFGGOCEIAM + " - " + HHLCHHIFDCM);
			break;
		}
	}

	public static void EmailLog(string CCELBJICOKG, string IEJOMILJAOK)
	{
		if (!string.IsNullOrEmpty(_filePath))
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress("logs@nekkimobile.ru", IEJOMILJAOK);
			mailMessage.To.Add(CCELBJICOKG);
			mailMessage.Attachments.Add(new Attachment(_filePath));
			mailMessage.Subject = string.Format("Log from [{0}:{1}:{2}] {3}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier);
			mailMessage.Body = "see log in attachment";
			SmtpClient smtpClient = new SmtpClient("mail.nekkimobile.ru");
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential("logs@nekkimobile.ru", "o99hSASo") as ICredentialsByHost;
			smtpClient.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = (object JDCCBCNFENK, X509Certificate POHBEPBAMIO, X509Chain GCONPBMJDFL, SslPolicyErrors BFOEIHJDKEL) => true;
			smtpClient.Send(mailMessage);
			Debug.Log("success");
		}
	}

	private static void CNBFNEHEDKP(NKKBMLLIKHH GNLOCMLBNHF, object LIOGIBJBHAH)
	{
		if (LIOGIBJBHAH != null && !string.IsNullOrEmpty(LIOGIBJBHAH.ToString()) && PNBOAKLOFCE())
		{
			PushToFile(string.Format("[{0}:{1}] {2}", DateTime.Now, GNLOCMLBNHF, LIOGIBJBHAH));
		}
	}

	private static void PushToFile(string LIOGIBJBHAH)
	{
		File.AppendAllText(_filePath, LIOGIBJBHAH);
	}

	public static void Log(object LIOGIBJBHAH)
	{
		if (PNBOAKLOFCE())
		{
			Debug.Log(LIOGIBJBHAH);
		}
	}

	public static void Log(object LIOGIBJBHAH, UnityEngine.Object PDCAHMPCPOC)
	{
		if (PNBOAKLOFCE())
		{
			Debug.Log(LIOGIBJBHAH, PDCAHMPCPOC);
		}
	}

	public static void HOGCGGPHKFC(UnityEngine.Object PDCAHMPCPOC, string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogFormat(PDCAHMPCPOC, LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void HOGCGGPHKFC(string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogFormat(LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void LOPHFKMOPAA(object LIOGIBJBHAH)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogWarning(LIOGIBJBHAH);
		}
	}

	public static void LOPHFKMOPAA(object LIOGIBJBHAH, UnityEngine.Object PDCAHMPCPOC)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogWarning(LIOGIBJBHAH, PDCAHMPCPOC);
		}
	}

	public static void FCGBAOEMDAI(UnityEngine.Object PDCAHMPCPOC, string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogWarningFormat(PDCAHMPCPOC, LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void FCGBAOEMDAI(string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogWarningFormat(LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void CCOFFJPPAKC(object LIOGIBJBHAH)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogError(LIOGIBJBHAH);
		}
	}

	public static void CCOFFJPPAKC(object LIOGIBJBHAH, UnityEngine.Object PDCAHMPCPOC)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogError(LIOGIBJBHAH, PDCAHMPCPOC);
		}
	}

	public static void DJGJADAFGKK(UnityEngine.Object PDCAHMPCPOC, string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogErrorFormat(PDCAHMPCPOC, LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void DJGJADAFGKK(string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogErrorFormat(LBOHOKIBHOH, LKIOKGCNKHE);
		}
	}

	public static void LogException(Exception MPFFFAOGBJE)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogException(MPFFFAOGBJE);
		}
	}

	public static void LogException(Exception MPFFFAOGBJE, UnityEngine.Object PDCAHMPCPOC)
	{
		if (PNBOAKLOFCE())
		{
			Debug.LogException(MPFFFAOGBJE, PDCAHMPCPOC);
		}
	}

	public static void Assert(bool IOFGGOCEIAM)
	{
		if (PNBOAKLOFCE())
		{
		}
	}

	public static void Assert(bool IOFGGOCEIAM, string LIOGIBJBHAH)
	{
		if (PNBOAKLOFCE())
		{
		}
	}

	public static void Assert(bool IOFGGOCEIAM, string LBOHOKIBHOH, params object[] LKIOKGCNKHE)
	{
		if (PNBOAKLOFCE())
		{
		}
	}
}
