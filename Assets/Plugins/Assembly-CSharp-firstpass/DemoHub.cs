using UnityEngine;

internal class DemoHub : Hub
{
	private float longRunningJobProgress;

	private string BDKGKIDENLD = "Not Started!";

	private string PHBPEDLIAHI = string.Empty;

	private string PEHHMKLDCIA = string.Empty;

	private string NMNIPAJGJEH = string.Empty;

	private string MELKHOCJFKD = string.Empty;

	private string ECACBKPAHAK = string.Empty;

	private string GFAHEKCBLIF = string.Empty;

	private string DFLJODIKLEC = string.Empty;

	private string AFFOCCDOAFH = string.Empty;

	private string NMIIENFDDKD = string.Empty;

	private string LPDPBBDNMLH = string.Empty;

	private string ANGAKMDNJEE = string.Empty;

	private string KPAPCGMIHNG = string.Empty;

	private string IHNKPDBBHIE = string.Empty;

	private string MCGCCGAPGFK = string.Empty;

	private string IGHEAAICFLE = string.Empty;

	private string GINNBAGEJLG = string.Empty;

	private GUIMessageList GGEMCGHOEBD = new GUIMessageList();

	public DemoHub()
		: base("demo")
	{
		JPJAFMLNALO("invoke", Invoke);
		JPJAFMLNALO("signal", IANOIACGNAM);
		JPJAFMLNALO("groupAdded", HNGKPFGMNIA);
		JPJAFMLNALO("fromArbitraryCode", CCBJFGDFGJK);
	}

	public void ReportProgress(string EHCLMBADLKH)
	{
		Call("reportProgress", ENCKNNJMOME, null, INPEMAAGEME, EHCLMBADLKH);
	}

	public void INPEMAAGEME(Hub CGFIJCNNCKP, ClientMessage JBEJKCPHFJP, ProgressMessage progress)
	{
		longRunningJobProgress = (float)progress.ALDEPEHMGNK();
		BDKGKIDENLD = progress.ALDEPEHMGNK() + "%";
	}

	public void ENCKNNJMOME(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		BDKGKIDENLD = DCJLKCFKCOM.LBAIENGDLDJ().ToString();
		BGFONCHEPJK();
	}

	public void BGFONCHEPJK()
	{
		Call("multipleCalls");
	}

	public void LACEDJJCILL()
	{
		Call("dynamicTask", APPGLEMDACM, CBDBEDBEIHP);
	}

	private void CBDBEDBEIHP(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, FailureMessage DCJLKCFKCOM)
	{
		NMNIPAJGJEH = string.Format("The dynamic task failed :( {0}", DCJLKCFKCOM.LCHHLEOPONE());
	}

	private void APPGLEMDACM(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		NMNIPAJGJEH = string.Format("The dynamic task! {0}", DCJLKCFKCOM.LBAIENGDLDJ());
	}

	public void MDGBAIHKMPE()
	{
		Call("addToGroups");
	}

	public void GetValue()
	{
		Call("getValue", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			MELKHOCJFKD = string.Format("The value is {0} after 5 seconds", DCJLKCFKCOM.LBAIENGDLDJ());
		});
	}

	public void GEEDKDFKMOI()
	{
		Call("taskWithException", null, (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, FailureMessage JDONBAPIJCG) =>
		{
			ECACBKPAHAK = string.Format("Error: {0}", JDONBAPIJCG.LCHHLEOPONE());
		});
	}

	public void MBDPOHEEJGO()
	{
		Call("genericTaskWithException", null, (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, FailureMessage JDONBAPIJCG) =>
		{
			GFAHEKCBLIF = string.Format("Error: {0}", JDONBAPIJCG.LCHHLEOPONE());
		});
	}

	public void FBDHPAIGJBG()
	{
		Call("synchronousException", null, (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, FailureMessage JDONBAPIJCG) =>
		{
			DFLJODIKLEC = string.Format("Error: {0}", JDONBAPIJCG.LCHHLEOPONE());
		});
	}

	public void ILAAHBOFIIM(object FAFBDKBGDNM)
	{
		Call("passingDynamicComplex", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			AFFOCCDOAFH = string.Format("The person's age is {0}", DCJLKCFKCOM.LBAIENGDLDJ());
		}, FAFBDKBGDNM);
	}

	public void SimpleArray(int[] HFPDMGAEJJE)
	{
		Call("simpleArray", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			NMIIENFDDKD = "Simple array works!";
		}, HFPDMGAEJJE);
	}

	public void MJLIGOHOKLH(object FAFBDKBGDNM)
	{
		Call("complexType", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			LPDPBBDNMLH = string.Format("Complex Type -> {0}", ((IHub)this).BAFGHLCPPHM.IBNMFHGHIBI().Encode(FLBBFDNHJAJ()["person"]));
		}, FAFBDKBGDNM);
	}

	public void ComplexArray(object[] OEOJFDNOEAO)
	{
		Call("ComplexArray", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			ANGAKMDNJEE = "Complex Array Works!";
		}, new object[1] { OEOJFDNOEAO });
	}

	public void Overload()
	{
		Call("Overload", MDKHINHJHNG);
	}

	private void MDKHINHJHNG(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		KPAPCGMIHNG = "Void Overload called";
		Overload(101);
	}

	public void Overload(int number)
	{
		Call("Overload", FIOFLIGAFMC, number);
	}

	private void FIOFLIGAFMC(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		IHNKPDBBHIE = string.Format("Overload with return value called => {0}", DCJLKCFKCOM.LBAIENGDLDJ().ToString());
	}

	public void LLBPNJCDCBB()
	{
		Call("readStateValue", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			MCGCCGAPGFK = string.Format("Read some state! => {0}", DCJLKCFKCOM.LBAIENGDLDJ());
		});
	}

	public void IHIBHBFIMCJ()
	{
		Call("plainTask", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			IGHEAAICFLE = "Plain Task Result";
		});
	}

	public void PJKALCNNLBD()
	{
		Call("genericTaskWithContinueWith", (Hub CGFIJCNNCKP, ClientMessage CKEHOEGLMBM, ResultMessage DCJLKCFKCOM) =>
		{
			GINNBAGEJLG = DCJLKCFKCOM.LBAIENGDLDJ().ToString();
		});
	}

	private void CCBJFGDFGJK(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		PHBPEDLIAHI = BOPGDKGIGHM.FNKPHEHFKEI()[0] as string;
	}

	private void HNGKPFGMNIA(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		if (!string.IsNullOrEmpty(PEHHMKLDCIA))
		{
			PEHHMKLDCIA = "Group Already Added!";
		}
		else
		{
			PEHHMKLDCIA = "Group Added!";
		}
	}

	private void IANOIACGNAM(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		NMNIPAJGJEH = string.Format("The dynamic task! {0}", BOPGDKGIGHM.FNKPHEHFKEI()[0]);
	}

	private void Invoke(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		GGEMCGHOEBD.Add(string.Format("{0} client state index -> {1}", BOPGDKGIGHM.FNKPHEHFKEI()[0], FLBBFDNHJAJ()["index"]));
	}

	public void MCAIPGEPMDE()
	{
		GUILayout.Label("Arbitrary Code");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(string.Format("Sending {0} from arbitrary code without the hub itself!", PHBPEDLIAHI));
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Group Added");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(PEHHMKLDCIA);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Dynamic Task");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(NMNIPAJGJEH);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Report Progress");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Label(BDKGKIDENLD);
		GUILayout.HorizontalSlider(longRunningJobProgress, 0f, 100f);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(MELKHOCJFKD);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Task With Exception");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(ECACBKPAHAK);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task With Exception");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(GFAHEKCBLIF);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Synchronous Exception");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(DFLJODIKLEC);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Invoking hub method with dynamic");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(AFFOCCDOAFH);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Simple Array");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(NMIIENFDDKD);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Complex Type");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(LPDPBBDNMLH);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Complex Array");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(ANGAKMDNJEE);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Overloads");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Label(KPAPCGMIHNG);
		GUILayout.Label(IHNKPDBBHIE);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Read State Value");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(MCGCCGAPGFK);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Plain Task");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(IGHEAAICFLE);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Generic Task With ContinueWith");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.Label(GINNBAGEJLG);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.Label("Message Pump");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GGEMCGHOEBD.MCAIPGEPMDE(Screen.width - 40, 270f);
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}
}
