using System.Collections.Generic;
using UnityEngine;

internal class BaseHub : Hub
{
	private string Title;

	private GUIMessageList messages = new GUIMessageList();

	public BaseHub(string name, string PEMOECLNECD)
		: base(name)
	{
		Title = PEMOECLNECD;
		JPJAFMLNALO("joined", NMDCHONANFC);
		JPJAFMLNALO("rejoined", KPENOPMFLFA);
		JPJAFMLNALO("left", EDCHBILGFLD);
		JPJAFMLNALO("invoked", PANMBANILIG);
	}

	private void NMDCHONANFC(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		Dictionary<string, object> dictionary = BOPGDKGIGHM.FNKPHEHFKEI()[2] as Dictionary<string, object>;
		messages.Add(string.Format("{0} joined at {1}\n\tIsAuthenticated: {2} IsAdmin: {3} UserName: {4}", BOPGDKGIGHM.FNKPHEHFKEI()[0], BOPGDKGIGHM.FNKPHEHFKEI()[1], dictionary["IsAuthenticated"], dictionary["IsAdmin"], dictionary["UserName"]));
	}

	private void KPENOPMFLFA(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		messages.Add(string.Format("{0} reconnected at {1}", BOPGDKGIGHM.FNKPHEHFKEI()[0], BOPGDKGIGHM.FNKPHEHFKEI()[1]));
	}

	private void EDCHBILGFLD(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		messages.Add(string.Format("{0} left at {1}", BOPGDKGIGHM.FNKPHEHFKEI()[0], BOPGDKGIGHM.FNKPHEHFKEI()[1]));
	}

	private void PANMBANILIG(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		messages.Add(string.Format("{0} invoked hub method at {1}", BOPGDKGIGHM.FNKPHEHFKEI()[0], BOPGDKGIGHM.FNKPHEHFKEI()[1]));
	}

	public void IJOCHDFBMJN()
	{
		Call("invokedFromClient", OPPHIKBOIIA, CODAJKAAGBB);
	}

	private void OPPHIKBOIIA(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		AdvLog.Log(CGFIJCNNCKP.get_Name() + " invokedFromClient success!");
	}

	private void CODAJKAAGBB(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, FailureMessage DCJLKCFKCOM)
	{
		AdvLog.LOPHFKMOPAA(CGFIJCNNCKP.get_Name() + " " + DCJLKCFKCOM.LCHHLEOPONE());
	}

	public void MCAIPGEPMDE()
	{
		GUILayout.Label(Title);
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		messages.MCAIPGEPMDE(Screen.width - 20, 100f);
		GUILayout.EndHorizontal();
	}
}
