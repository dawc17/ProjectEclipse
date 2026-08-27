using UnityEngine;

internal class TypedDemoHub : Hub
{
	private string HIJMAONIMJB = string.Empty;

	private string NHIKDBMALBH = string.Empty;

	public TypedDemoHub()
		: base("typeddemohub")
	{
		JPJAFMLNALO("Echo", OBHMNCMCEIO);
	}

	private void OBHMNCMCEIO(Hub CGFIJCNNCKP, MethodCallMessage BOPGDKGIGHM)
	{
		NHIKDBMALBH = string.Format("{0} #{1} triggered!", BOPGDKGIGHM.FNKPHEHFKEI()[0], BOPGDKGIGHM.FNKPHEHFKEI()[1]);
	}

	public void OBHMNCMCEIO(string CKEHOEGLMBM)
	{
		Call("echo", BOANOIDOPEK, CKEHOEGLMBM);
	}

	private void BOANOIDOPEK(Hub CGFIJCNNCKP, ClientMessage BKNEELNMDHH, ResultMessage DCJLKCFKCOM)
	{
		HIJMAONIMJB = "TypedDemoHub.Echo(string message) invoked!";
	}

	public void MCAIPGEPMDE()
	{
		GUILayout.Label("Typed callback");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Label(HIJMAONIMJB);
		GUILayout.Label(NHIKDBMALBH);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}
}
