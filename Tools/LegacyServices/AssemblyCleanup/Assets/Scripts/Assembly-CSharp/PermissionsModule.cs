using Nekki.SF2.Core;
using UnityEngine;

public class PermissionsModule : LoadingModule
{
	private static readonly string[] _Permissions = new string[2] { "android.permission.GET_ACCOUNTS", "android.permission.READ_PHONE_STATE" };

	private static readonly PermissionDialog MDLKEOLKNJG = PermissionDialog.JJLLCGBEJLF("Permissions Request", "Dear player, in the following screen you will be asked to grant access to Contacts and Phone Calls on your device. This permissions is used only to obtain your account name. We will not access any of your contacts or make telephone calls. Your consent is needed to provide you a more enjoyable gameplay and correct application running. Thank you in advance!", "OK");

	private static readonly PermissionDialog OCIFKIOONOI = PermissionDialog.HMCLCLAJHGC("Permissions Error", "Please confirm this permissions since they are needed to run the game", "RETRY", "QUIT", "SETTINGS");

	private bool _CheckStarted;

	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL && !_CheckStarted)
		{
			_CheckStarted = true;
			Debug.Log("PermissionsModule.CheckStarted");
			if (PermissionsChecker.LBMPMKBLOGP(_Permissions, AOHJPDDPJJJ, MDLKEOLKNJG, OCIFKIOONOI))
			{
				CHIHBINEGFL = true;
			}
		}
	}

	private void AOHJPDDPJJJ(string[] OHELAJEAOLC, PermissionRequestResult AMKKLMOONEP)
	{
		if (AMKKLMOONEP == PermissionRequestResult.Granded)
		{
			CHIHBINEGFL = true;
		}
		else
		{
			ApplicationController.Quit();
		}
	}
}
