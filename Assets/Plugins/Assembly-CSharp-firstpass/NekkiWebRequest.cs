using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class NekkiWebRequest
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<NekkiWebRequest> OnSuccessful = delegate
	{
	};

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<NekkiWebRequest> onErrorField = delegate
	{
	};

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<NekkiWebRequest> OnProgress = delegate
	{
	};

	private object _externalData;

	private Coroutine IFFCDIDKGCB;

	private UnityWebRequest AGGJFGBFMEH;

	private NekkiWebHandler KLHDMNAMAGH;

	private bool OHLIBLCPNEI;

	private bool CBBLPPKALMA;

	private float DKNHEPEDDNG;

	private readonly float ECCNMMJJKPC;

	private int _currentPosition;

	private NekkiUri JDNMLANMPPK;

	private bool JIHAABLAAOL;

	private bool NCOLKNHMCPJ;

	public bool ECBDGAJHOPF
	{
		get
		{
			return KPBEHFEBJJN();
		}
	}

	public bool KKIAPBNCOFH
	{
		get
		{
			return BPILNIINAGK();
		}
	}

	public bool DJIBNNOHJNO
	{
		get
		{
			return OAOPGDNDGMF();
		}
	}

	public string Url
	{
		get
		{
			return KLMLKCKNNFD();
		}
	}

	public string Error
	{
		get
		{
			return KNFHEKMDFGM();
		}
	}

	public byte[] IOBFHIMBDKM
	{
		get
		{
			return KJBFBPBCAOH();
		}
	}

	public string GGDJIPKMKFC
	{
		get
		{
			return ILMJJEMPKCN();
		}
	}

	public float OIOANIMIIIA
	{
		get
		{
			return ALDEPEHMGNK();
		}
	}

	public int NOPMKIAONBO
	{
		get
		{
			return GFLHMBOBICA();
		}
	}

	public int JJCKADKCDIF
	{
		get
		{
			return ECJPLFFAMJO();
		}
	}

	public event Action<NekkiWebRequest> CBKDIFCLCMO
	{
		add
		{
			FGJPDDAPFME(value);
		}
		remove
		{
			HDCJMHKBLIC(value);
		}
	}

	public event Action<NekkiWebRequest> OnError
	{
		add
		{
			BJDMHEHILEO(value);
		}
		remove
		{
			LEIDAIFMPCE(value);
		}
	}

	public event Action<NekkiWebRequest> OGLIKFCADME
	{
		add
		{
			JIMPDHAOECM(value);
		}
		remove
		{
			ECLIHLAOFNK(value);
		}
	}

	public NekkiWebRequest(float DGDKHFPEHOG = 5f)
	{
		ECCNMMJJKPC = DGDKHFPEHOG;
		Reset();
	}

	public bool KPBEHFEBJJN()
	{
		return AGGJFGBFMEH != null && (AGGJFGBFMEH.isDone || OAOPGDNDGMF());
	}

	public bool BPILNIINAGK()
	{
		return KPBEHFEBJJN() && !OAOPGDNDGMF();
	}

	public bool OAOPGDNDGMF()
	{
		return NCOLKNHMCPJ || AGGJFGBFMEH.isNetworkError || CBBLPPKALMA || (!AALFCHMFOEH() && !ECLICGHFOOP());
	}

	public string KLMLKCKNNFD()
	{
		return JDNMLANMPPK.OriginalString;
	}

	public string KNFHEKMDFGM()
	{
		return IJEOAHEAEDC();
	}

	public byte[] KJBFBPBCAOH()
	{
		return AGGJFGBFMEH.downloadHandler.data;
	}

	public string ILMJJEMPKCN()
	{
		return AGGJFGBFMEH.downloadHandler.text;
	}

	public float ALDEPEHMGNK()
	{
		return (GFLHMBOBICA() <= 0) ? 0f : ((float)ECJPLFFAMJO() / (float)GFLHMBOBICA());
	}

	public int GFLHMBOBICA()
	{
		return (!NekkiUtils.JDIKHMODKKF()) ? KLHDMNAMAGH.GFLHMBOBICA() : 0;
	}

	public int ECJPLFFAMJO()
	{
		return (!NekkiUtils.JDIKHMODKKF()) ? KLHDMNAMAGH.ECJPLFFAMJO() : ((int)AGGJFGBFMEH.downloadedBytes);
	}

	public void FGJPDDAPFME(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = OnSuccessful;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnSuccessful, (Action<NekkiWebRequest>)Delegate.Combine(action2, value), action);
		}
		while ((object)action != action2);
	}

	public void HDCJMHKBLIC(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = OnSuccessful;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnSuccessful, (Action<NekkiWebRequest>)Delegate.Remove(action2, value), action);
		}
		while ((object)action != action2);
	}

	public void BJDMHEHILEO(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = onErrorField;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref onErrorField, (Action<NekkiWebRequest>)Delegate.Combine(action2, value), action);
		}
		while ((object)action != action2);
	}

	public void LEIDAIFMPCE(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = onErrorField;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref onErrorField, (Action<NekkiWebRequest>)Delegate.Remove(action2, value), action);
		}
		while ((object)action != action2);
	}

	public void JIMPDHAOECM(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = OnProgress;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnProgress, (Action<NekkiWebRequest>)Delegate.Combine(action2, value), action);
		}
		while ((object)action != action2);
	}

	public void ECLIHLAOFNK(Action<NekkiWebRequest> value)
	{
		Action<NekkiWebRequest> action = OnProgress;
		Action<NekkiWebRequest> action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnProgress, (Action<NekkiWebRequest>)Delegate.Remove(action2, value), action);
		}
		while ((object)action != action2);
	}

	public virtual void Send(string JJEOAIKCKAM, bool GHIGJJCMEDI)
	{
		JIHAABLAAOL = GHIGJJCMEDI;
		Send(UnityWebRequest.Get(JJEOAIKCKAM));
	}

	public virtual void Send(string JJEOAIKCKAM, string MHAANIPLCJD, bool GHIGJJCMEDI)
	{
		JIHAABLAAOL = GHIGJJCMEDI;
		Send(UnityWebRequest.Post(JJEOAIKCKAM, MHAANIPLCJD));
	}

	public virtual void Send(string JJEOAIKCKAM, Dictionary<string, string> MHAANIPLCJD, bool GHIGJJCMEDI)
	{
		JIHAABLAAOL = GHIGJJCMEDI;
		Send(UnityWebRequest.Post(JJEOAIKCKAM, MHAANIPLCJD));
	}

	private void Send(UnityWebRequest DLILAFJFLAI)
	{
		JDNMLANMPPK = new NekkiUri(DLILAFJFLAI.url);
		KLHDMNAMAGH = GBMIBHMCDHC(JDNMLANMPPK);
		DKNHEPEDDNG = Time.realtimeSinceStartup;
		AGGJFGBFMEH = DLILAFJFLAI;
		if (NekkiUtils.JDIKHMODKKF())
		{
			GPAGLIBCPFC();
			return;
		}
		AGGJFGBFMEH.downloadHandler = KLHDMNAMAGH;
		IFFCDIDKGCB = Routiner.Go(PHIJHLDLJGO());
	}

	private IEnumerator PHIJHLDLJGO()
	{
		EMPGOCGHMBI();
		while (!KPBEHFEBJJN())
		{
			yield return new WaitForSecondsRealtime(1f / 60f);
			DFGFNFFMCEC();
		}
		FPKKPFBDEAG();
	}

	private void GPAGLIBCPFC()
	{
		EMPGOCGHMBI();
		while (!KPBEHFEBJJN())
		{
			DFGFNFFMCEC();
		}
		FPKKPFBDEAG();
	}

	private void EMPGOCGHMBI()
	{
		NCOLKNHMCPJ = false;
		if (!OfflineServices.IsLocalContent(KLMLKCKNNFD()))
		{
			NCOLKNHMCPJ = true;
			return; // Completion/error handling remains in the normal request loop.
		}
		Log("WebRequest Send " + KLMLKCKNNFD());
		if (JIHAABLAAOL && !CertificateValidator.GLHLIEOFFLN(KLMLKCKNNFD()))
		{
			NCOLKNHMCPJ = true;
			SendError(true);
		}
		else
		{
			AGGJFGBFMEH.Send();
		}
	}

	private void DFGFNFFMCEC()
	{
		if (_currentPosition != ECJPLFFAMJO())
		{
			_currentPosition = ECJPLFFAMJO();
			DKNHEPEDDNG = Time.realtimeSinceStartup;
		}
		else
		{
			CBBLPPKALMA = Time.realtimeSinceStartup - DKNHEPEDDNG >= ECCNMMJJKPC;
		}
		HMDOPBIDDMC();
	}

	private void FPKKPFBDEAG()
	{
		if (KLHDMNAMAGH != null && NekkiUtils.JDIKHMODKKF())
		{
			KLHDMNAMAGH.GEJLNPIEDPF();
		}
		if (BPILNIINAGK())
		{
			JBEBPLHMDPF();
		}
		else
		{
			SendError();
		}
		AKLEEMEHBIC();
	}

	protected virtual void JBEBPLHMDPF()
	{
		Log("WebRequest Successful " + KLMLKCKNNFD());
		OnSuccessful.FEEGJDJIFEF(this);
		DHFMAAFDMMM();
	}

	protected virtual void SendError(bool BALCNGAKGKN = false)
	{
		string text = "WebRequest Error " + KLMLKCKNNFD() + " " + KNFHEKMDFGM();
		if (BALCNGAKGKN)
		{
			UnityEngine.Debug.LogError(text);
		}
		else
		{
			Log(text);
		}
		onErrorField.FEEGJDJIFEF(this);
		DHFMAAFDMMM();
	}

	protected virtual void HMDOPBIDDMC()
	{
		OnProgress.FEEGJDJIFEF(this);
	}

	protected virtual NekkiWebHandler GBMIBHMCDHC(NekkiUri KJHNCLAJMLO)
	{
		return new NekkiWebHandlerRequest(KJHNCLAJMLO);
	}

	public void AKLEEMEHBIC(bool CMBGKNNPACJ = false)
	{
		if (CMBGKNNPACJ)
		{
			SendError();
		}
		if (!OHLIBLCPNEI)
		{
			AGGJFGBFMEH.Abort();
			KLHDMNAMAGH.AKLEEMEHBIC();
			OHLIBLCPNEI = true;
			_externalData = null;
		}
		if (IFFCDIDKGCB != null)
		{
			Routiner.Stop(IFFCDIDKGCB);
			IFFCDIDKGCB = null;
		}
		Reset();
	}

	private void Reset()
	{
		OHLIBLCPNEI = false;
		CBBLPPKALMA = false;
		DKNHEPEDDNG = 0f;
		_currentPosition = 0;
	}

	private void DHFMAAFDMMM()
	{
		OnSuccessful = null;
		onErrorField = null;
		OnSuccessful = null;
	}

	private void Log(string value)
	{
		UnityEngine.Debug.Log(value);
	}

	private bool AALFCHMFOEH()
	{
		return 200 <= AGGJFGBFMEH.responseCode && AGGJFGBFMEH.responseCode < 300;
	}

	private bool ECLICGHFOOP()
	{
		return !AGGJFGBFMEH.isDone;
	}

	private string IJEOAHEAEDC()
	{
		if (NCOLKNHMCPJ)
		{
			if (!OfflineServices.IsLocalContent(KLMLKCKNNFD())) return OfflineServices.Unavailable;
			return "HTTPS certificate check error";
		}
		if (CBBLPPKALMA)
		{
			return "Failed with timeout";
		}
		if (!AALFCHMFOEH())
		{
			return "Failed with responseCode - " + AGGJFGBFMEH.responseCode + " " + AGGJFGBFMEH.error;
		}
		return AGGJFGBFMEH.error;
	}

	public T DNMMOIIDLOO<T>() where T : class
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(ILMJJEMPKCN());
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("NekkiWeb - Error Parse JSON (" + ex.Message + ")");
		}
		return (T)null;
	}

	public void SetExternalData(object value)
	{
		_externalData = value;
	}

	public T AMDAHKFJDNG<T>()
	{
		return (T)_externalData;
	}
}
