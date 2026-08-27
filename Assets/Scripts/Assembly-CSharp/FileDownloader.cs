using System;

public class FileDownloader
{
	private static FileDownloader _Instance;

	private Action<bool> _resultCallback;

	private Action<float> _progressCallback;

	private string EBLEJDDMDAO;

	private string _name;

	private float _timeout = 120f;

	private int _size;

	public static FileDownloader BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public static FileDownloader ELEBLBJKDBI()
	{
		if (_Instance == null)
		{
			_Instance = new FileDownloader();
		}
		return _Instance;
	}

	public void EMANDFAOCNO(string BEPKJNKCKPH, string name, string IMFMPLFADCE, Action<bool> HKHNPNNDHFP, Action<float> OODDBFJDGJO = null, int PEEOEOMEBFG = 0)
	{
		_resultCallback = HKHNPNNDHFP;
		_progressCallback = OODDBFJDGJO;
		EBLEJDDMDAO = IMFMPLFADCE;
		_name = name;
		_size = PEEOEOMEBFG;
		NekkiWebHelper.EMANDFAOCNO(BEPKJNKCKPH, string.Format("{0}/{1}", EBLEJDDMDAO, _name), CBKDIFCLCMO, OnError, OGLIKFCADME, null, _timeout);
	}

	private void CBKDIFCLCMO(NekkiWebRequest DCJLKCFKCOM)
	{
		if (_resultCallback != null)
		{
			_resultCallback(true);
		}
	}

	private void OnError(NekkiWebRequest DCJLKCFKCOM)
	{
		if (_resultCallback != null)
		{
			_resultCallback(false);
		}
	}

	private void OGLIKFCADME(NekkiWebRequest DCJLKCFKCOM)
	{
		if (_progressCallback != null)
		{
			if (_size > 0)
			{
				float obj = (float)DCJLKCFKCOM.ECJPLFFAMJO() / (float)_size;
				_progressCallback(obj);
			}
			else
			{
				_progressCallback(DCJLKCFKCOM.ALDEPEHMGNK());
			}
		}
	}
}
