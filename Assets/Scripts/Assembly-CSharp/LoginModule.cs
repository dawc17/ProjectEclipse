using System;

public class LoginModule : LoadingModule
{
	private bool IADDNBMPDGL;

	public override void Start()
	{
		base.Start();
		IADDNBMPDGL = false;
	}

	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL && !IADDNBMPDGL)
		{
			GameUtils.CGFHDKDJCPL();
			NetworkController fDJHFPIFMIK = NetworkController.ELEBLBJKDBI();
			fDJHFPIFMIK.OnLoginComplete = (Action<object>)Delegate.Combine(fDJHFPIFMIK.OnLoginComplete, new Action<object>(OnLoginComplete));
			ListSF.ELEBLBJKDBI().IAAELKAKHPN();
			IADDNBMPDGL = true;
		}
	}

	private void OnLoginComplete(object data)
	{
		NetworkController fDJHFPIFMIK = NetworkController.ELEBLBJKDBI();
		fDJHFPIFMIK.OnLoginComplete = (Action<object>)Delegate.Remove(fDJHFPIFMIK.OnLoginComplete, new Action<object>(OnLoginComplete));
		GameUtils.OBJEKOBDMOE = true;
		CHIHBINEGFL = true;
	}
}
