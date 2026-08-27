using System;
using System.Diagnostics;

public class GooglePlayLicenseServerResponse
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int JJNGFHNJGJM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string EJCFIKPMGJI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AEPPEGMGNEO;

	public int BBACOCNJMBN
	{
		get
		{
			return IJIAKJOCIAG();
		}
		private set
		{
			set_ResponseCode(value);
		}
	}

	public string NBFJDDEMHBD
	{
		get
		{
			return GCHMKEIIAPJ();
		}
		private set
		{
			HLEOALPMAMN(value);
		}
	}

	public string FBFKEJEOELM
	{
		get
		{
			return MCDDGNJEKEO();
		}
		private set
		{
			PAHOBGBPBCG(value);
		}
	}

	public int IJIAKJOCIAG()
	{
		return JJNGFHNJGJM;
	}

	private void set_ResponseCode(int value)
	{
		JJNGFHNJGJM = value;
	}

	public string GCHMKEIIAPJ()
	{
		return EJCFIKPMGJI;
	}

	private void HLEOALPMAMN(string value)
	{
		EJCFIKPMGJI = value;
	}

	public string MCDDGNJEKEO()
	{
		return AEPPEGMGNEO;
	}

	private void PAHOBGBPBCG(string value)
	{
		AEPPEGMGNEO = value;
	}

	public static GooglePlayLicenseServerResponse Parse(string MGDHJCDGOLJ)
	{
		if (string.IsNullOrEmpty(MGDHJCDGOLJ))
		{
			return null;
		}
		char[] separator = new char[1] { '\n' };
		string[] array = MGDHJCDGOLJ.Split(separator, StringSplitOptions.None);
		if (MGDHJCDGOLJ.Length < 3)
		{
			return null;
		}
		GooglePlayLicenseServerResponse gEBIMFAJMGA = new GooglePlayLicenseServerResponse();
		gEBIMFAJMGA.set_ResponseCode(array[0].ToInt());
		gEBIMFAJMGA.HLEOALPMAMN(array[1]);
		gEBIMFAJMGA.PAHOBGBPBCG(array[2]);
		return gEBIMFAJMGA;
	}

	public override string ToString()
	{
		return string.Format("[GooglePlayLicenseServerResponse: ResponseCode={0}, SignedData={1}, Signature={2}]", IJIAKJOCIAG(), GCHMKEIIAPJ(), MCDDGNJEKEO());
	}
}
