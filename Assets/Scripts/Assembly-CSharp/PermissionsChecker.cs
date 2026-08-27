using System;
using Nekki.SF2.Core.Permissions;

public static class PermissionsChecker
{
	public static bool LBMPMKBLOGP(string[] OHELAJEAOLC, Action<string[], PermissionRequestResult> PLFCFPHMKJM, PermissionDialog ECHGBOPIDEP)
	{
		if (!PermissionsManager.get_Current().CheckPermissions(OHELAJEAOLC))
		{
			PermissionsManager.get_Current().RequestPermissions(OHELAJEAOLC, PLFCFPHMKJM, ECHGBOPIDEP);
			return false;
		}
		return true;
	}

	public static bool LBMPMKBLOGP(string[] OHELAJEAOLC, Action<string[], PermissionRequestResult> PLFCFPHMKJM, PermissionDialog PLOPABNJAAB, PermissionDialog ECHGBOPIDEP)
	{
		if (!PermissionsManager.get_Current().CheckPermissions(OHELAJEAOLC))
		{
			PermissionsManager.get_Current().RequestPermissionsWithExplanation(OHELAJEAOLC, PLFCFPHMKJM, PLOPABNJAAB, ECHGBOPIDEP);
			return false;
		}
		return true;
	}
}
