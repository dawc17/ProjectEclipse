using System;

namespace AppStoresSupport
{
	[Serializable]
	public class AppStoreSetting
	{
		public string AppID = string.Empty;

		public string AppKey = string.Empty;

		public bool IsTestMode;
	}
}
