using System;
using System.Diagnostics;
using UnityEngine;

public sealed class SampleDescriptor
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FACBGDJBENJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Type KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string GHOHHEPOKHM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BMFPPGBCFMO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string GEOFNNHICMG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HILJNEINPDH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private GameObject OHKBFKDFEDN;

	public bool GDJOOEAAJBH
	{
		get
		{
			return AFDFPBKGIIL();
		}
		set
		{
			FPEMDEEFJEL(value);
		}
	}

	public string LONIGNIEBHJ
	{
		get
		{
			return IFBOMKBDANN();
		}
		set
		{
			MAKOKOKCOOB(value);
		}
	}

	public string MGNNJPBCOGD
	{
		get
		{
			return GJOAJAIJHOE();
		}
		set
		{
			set_Description(value);
		}
	}

	public string EDAPEBIEBMG
	{
		get
		{
			return PHHICBFIMJE();
		}
		set
		{
			OMPKDJLKOLH(value);
		}
	}

	public bool JPMGJAMMLOA
	{
		get
		{
			return NHMPPLCPEEP();
		}
		set
		{
			OBNFCPCDNEJ(value);
		}
	}

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
		set
		{
			set_UnityObject(value);
		}
	}

	public bool OEDPHHDKECI
	{
		get
		{
			return NMACGEJHPDN();
		}
	}

	public SampleDescriptor(Type LFLGCDNKNJI, string IEJOMILJAOK, string EMDJGBHIAIA, string LCNMGAJENGL)
	{
		set_Type(LFLGCDNKNJI);
		MAKOKOKCOOB(IEJOMILJAOK);
		set_Description(EMDJGBHIAIA);
		OMPKDJLKOLH(LCNMGAJENGL);
	}

	public bool AFDFPBKGIIL()
	{
		return FACBGDJBENJ;
	}

	public void FPEMDEEFJEL(bool value)
	{
		FACBGDJBENJ = value;
	}

	public Type get_Type()
	{
		return KAHHEBMBCFA;
	}

	public void set_Type(Type value)
	{
		KAHHEBMBCFA = value;
	}

	public string IFBOMKBDANN()
	{
		return GHOHHEPOKHM;
	}

	public void MAKOKOKCOOB(string value)
	{
		GHOHHEPOKHM = value;
	}

	public string GJOAJAIJHOE()
	{
		return BMFPPGBCFMO;
	}

	public void set_Description(string value)
	{
		BMFPPGBCFMO = value;
	}

	public string PHHICBFIMJE()
	{
		return GEOFNNHICMG;
	}

	public void OMPKDJLKOLH(string value)
	{
		GEOFNNHICMG = value;
	}

	public bool NHMPPLCPEEP()
	{
		return HILJNEINPDH;
	}

	public void OBNFCPCDNEJ(bool value)
	{
		HILJNEINPDH = value;
	}

	public GameObject MJNPBMOAFML()
	{
		return OHKBFKDFEDN;
	}

	public void set_UnityObject(GameObject value)
	{
		OHKBFKDFEDN = value;
	}

	public bool NMACGEJHPDN()
	{
		return MJNPBMOAFML() != null;
	}

	public void CreateUnityObject()
	{
		if (!(MJNPBMOAFML() != null))
		{
			set_UnityObject(new GameObject(IFBOMKBDANN()));
			MJNPBMOAFML().AddComponent(get_Type());
		}
	}

	public void EHDDIIAKFGI()
	{
		if (MJNPBMOAFML() != null)
		{
			UnityEngine.Object.Destroy(MJNPBMOAFML());
			set_UnityObject(null);
		}
	}
}
