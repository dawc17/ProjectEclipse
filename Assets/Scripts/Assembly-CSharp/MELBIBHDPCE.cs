using System;
using System.Xml;
using Nekki.Utils;

public abstract class MELBIBHDPCE : global::EventDispatcher<object>
{
	public enum JBFAPJNANCE
	{
		onSaveXMLRequired = 0
	}

	protected XmlNode _node;

	private bool ECMCOJJGOHF;

	protected string IOIKDNGEDAD = string.Empty;

	protected int AIMMOLKAFOA;

	protected float MMFDIOPGHDJ;

	protected int NMAIHGPBMIP;

	public string EENGNCJLAEI
	{
		get
		{
			return KNGJJEOLFHF();
		}
		set
		{
			HCMLOIDALKC(value);
		}
	}

	public int LHCDFOEAKMJ
	{
		get
		{
			return BPOHPIJMFMA();
		}
		set
		{
			IIONCCLMLPI(value);
		}
	}

	public float JJOOINALKLK
	{
		get
		{
			return INPAOPFFKEJ();
		}
		set
		{
			PBEJGHOIPKC(value);
		}
	}

	public int GFJNLFPAAEI
	{
		get
		{
			return MNDJBCMLJHF();
		}
		set
		{
			IKIHAIKLLOK(value);
		}
	}

	protected MELBIBHDPCE()
	{
		GlobalTimer.get_Instance().addEventListener(0, ILFBDHDMHPD);
	}

	public MELBIBHDPCE(XmlNode node)
		: this()
	{
		_node = node;
		AIMMOLKAFOA = node.Attributes["InstallID"].ParseInt();
		if (AIMMOLKAFOA == 0)
		{
			IIONCCLMLPI((int)new RandomGenerator((uint)DateTime.Now.Millisecond).DADGADIAJHI());
		}
		MMFDIOPGHDJ = node.Attributes["TotalPaymentSum"].ParseFloat();
		NMAIHGPBMIP = node.Attributes["PaymentCount"].ParseInt();
	}

	public string KNGJJEOLFHF()
	{
		return IOIKDNGEDAD;
	}

	public void HCMLOIDALKC(string value)
	{
		IOIKDNGEDAD = value;
		EMDLLIGKONG("ServerUserID", IOIKDNGEDAD);
	}

	public int BPOHPIJMFMA()
	{
		return AIMMOLKAFOA;
	}

	public void IIONCCLMLPI(int value)
	{
		AIMMOLKAFOA = value;
		EMDLLIGKONG("InstallID", AIMMOLKAFOA);
	}

	public float INPAOPFFKEJ()
	{
		return MMFDIOPGHDJ;
	}

	public void PBEJGHOIPKC(float value)
	{
		if (value > MMFDIOPGHDJ)
		{
			MMFDIOPGHDJ = value;
			EMDLLIGKONG("TotalPaymentSum", MMFDIOPGHDJ);
		}
	}

	public int MNDJBCMLJHF()
	{
		return NMAIHGPBMIP;
	}

	public void IKIHAIKLLOK(int value)
	{
		if (value > NMAIHGPBMIP)
		{
			NMAIHGPBMIP = value;
			EMDLLIGKONG("PaymentCount", NMAIHGPBMIP);
		}
	}

	protected void EMDLLIGKONG(string name, long value)
	{
		EMDLLIGKONG(name, value.ToString());
	}

	protected void EMDLLIGKONG(string name, float value)
	{
		EMDLLIGKONG(name, value.ToString());
	}

	protected void EMDLLIGKONG(string name, string value)
	{
		if (_node.Attributes[name] != null)
		{
			_node.Attributes[name].Value = value;
		}
		else
		{
			_node.LLIKNHNLGJJ(name).Value = value;
		}
		GGGEHAGCLGC();
	}

	protected void EMDLLIGKONG(string name, bool value)
	{
		EMDLLIGKONG(name, (!value) ? "0" : "1");
	}

	public void GGGEHAGCLGC(bool AEGFPEGKLPJ = false)
	{
		if (AEGFPEGKLPJ)
		{
			CallEvent(0, null);
		}
		else
		{
			ECMCOJJGOHF = true;
		}
	}

	private void ILFBDHDMHPD(ExtentionBehaviour.CallEventArgs JKOCDNPPJDG)
	{
		if (ECMCOJJGOHF)
		{
			CallEvent(0, null);
			ECMCOJJGOHF = false;
		}
	}
}
