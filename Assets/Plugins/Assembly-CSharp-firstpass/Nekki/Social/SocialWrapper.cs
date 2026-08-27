using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Nekki.Social
{
	public class SocialWrapper : MonoBehaviour
	{
		private string _currentUserID;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static UserInfo IMGFNMPBGAK;

		private static SocialWrapper EDAPJLKMFPC;

		private static Action<DFIPCKIEILP> PDAPAKFEFJA;

		private bool _initDone;

		private Callbacks OHFANOBDGMN;

		internal static UserInfo LFIBBPIPPFJ
		{
			get
			{
				return NLEKLPFPLPC();
			}
			private set
			{
				IMJMHDPIBDI(value);
			}
		}

		public bool JLJPBPKCCBE
		{
			get
			{
				return get_Initialized();
			}
		}

		internal static UserInfo NLEKLPFPLPC()
		{
			return IMGFNMPBGAK;
		}

		private static void IMJMHDPIBDI(UserInfo value)
		{
			IMGFNMPBGAK = value;
		}

		internal static SocialWrapper Init(Callbacks EODBKOHACMO, Action<DFIPCKIEILP> GOLAPDHMKGC)
		{
			GameObject gameObject = GameObject.Find("_social");
			if (!gameObject)
			{
				gameObject = new GameObject("_social");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			SocialWrapper component = gameObject.GetComponent<SocialWrapper>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			EDAPJLKMFPC = gameObject.AddComponent<SocialWrapper>();
			EDAPJLKMFPC.OHFANOBDGMN = EODBKOHACMO;
			PDAPAKFEFJA = GOLAPDHMKGC;
			return EDAPJLKMFPC;
		}

		public bool get_Initialized()
		{
			return _initDone;
		}

		protected virtual void Start()
		{
			UnityEngine.Debug.Log("SocialWrapper Start");
			PDOFECBDMHO();
		}

		private void PDOFECBDMHO()
		{
			Application.ExternalCall("RequestSocialNetworkInfo");
		}

		internal void LHCIEKHPNGB(string EMBBNNBFODN)
		{
			if (!_initDone && EMBBNNBFODN.Contains("|"))
			{
				_initDone = true;
				string[] array = EMBBNNBFODN.Split('|');
				PIHBADCBECE(array[0], array[1]);
			}
		}

		protected virtual void PIHBADCBECE(string IDMBNOHJOAH, string AOKMNKOIMHI)
		{
			_currentUserID = AOKMNKOIMHI;
			if (IDMBNOHJOAH != null && IDMBNOHJOAH == "VK")
			{
				OHFANOBDGMN.AJHHICIMCDF(DFIPCKIEILP.VKontakte, AOKMNKOIMHI);
				PDAPAKFEFJA(DFIPCKIEILP.VKontakte);
			}
			else
			{
				OHFANOBDGMN.AJHHICIMCDF(DFIPCKIEILP.None, AOKMNKOIMHI);
				PDAPAKFEFJA(DFIPCKIEILP.None);
			}
		}

		internal void RequestUsersInfo(string[] JAIEEFOCDAA)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < JAIEEFOCDAA.Length; i++)
			{
				stringBuilder.Append(JAIEEFOCDAA[i]);
				if (i < JAIEEFOCDAA.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			Application.ExternalCall("RequestUsersInfo", stringBuilder.ToString());
		}

		internal void KHMOHOCHHNK(string BBNKIBKPBLO)
		{
			UserInfo jPKEEFNNAAP = new UserInfo(BBNKIBKPBLO);
			if (jPKEEFNNAAP.NDLJPNCIJIP() == _currentUserID)
			{
				IMJMHDPIBDI(jPKEEFNNAAP);
			}
			OHFANOBDGMN.FAHEIIBEFGE(jPKEEFNNAAP);
		}

		internal void KCBMPAILEIN()
		{
			Application.ExternalCall("RequestFriends");
		}

		internal void JFLDJPKOFAP(string BBNKIBKPBLO)
		{
			OHFANOBDGMN.MIMEMJDABNC(UserInfo.GetInfos(BBNKIBKPBLO));
		}

		internal void EIPHCAEGONE(string BBNKIBKPBLO)
		{
			OHFANOBDGMN.JKJFMHDBJCC(UserInfo.GetInfos(BBNKIBKPBLO));
		}

		internal void NKCLMBADENN()
		{
			Application.ExternalCall("Invite");
		}

		internal void RequestBookmark(bool DPJFKNNHONA)
		{
			Application.ExternalCall("RequestBookmark", (!DPJFKNNHONA) ? "false" : "true");
		}

		internal void AGIOLFLGFML(string state)
		{
			OHFANOBDGMN.LNCNJPOAMLL(state.Equals("true"));
		}

		internal void CNKJLMJAFNL()
		{
			Application.ExternalCall("RequestCheckGroupMembership");
		}

		internal void CKIOFOJHKMJ(string state)
		{
			OHFANOBDGMN.EEBEIFHLHLM(state.Equals("true"));
		}

		internal void MGFOEBCEKNB(string FFHABDMFMMC, string LIOGIBJBHAH, string DMNBDBJNKME)
		{
			Application.ExternalCall("RequestWallPost", FFHABDMFMMC, LIOGIBJBHAH, DMNBDBJNKME);
		}

		internal void ICCDKFEMJJC(string ADFFKCBJDMP)
		{
			OHFANOBDGMN.BACNFJLHGNG(ADFFKCBJDMP);
		}

		internal void Buy(string BGFOJFBFJIA)
		{
			Application.ExternalCall("Buy", BGFOJFBFJIA);
		}

		internal void HINBPONHLHD(string DACBPIHLFFD)
		{
			OHFANOBDGMN.BDKEHMPGDDH(DACBPIHLFFD);
		}

		internal void IFCHDCBOJHH()
		{
			OHFANOBDGMN.OBJFKDMJLJD(true);
		}

		internal void KLGKGPICDPJ()
		{
			OHFANOBDGMN.OBJFKDMJLJD(false);
		}

		internal void GNAEAMAEPBK(string BDKHPMOHIMN)
		{
			string[] array = BDKHPMOHIMN.Split('|');
			int num = int.Parse(array[0]);
			int num2 = int.Parse(array[1]);
			Screen.SetResolution(num, num2, false);
			AdvLog.Log(string.Format("Resolution changed to ({0}x{1})", num, num2));
		}

		private void OnDestroy()
		{
			Delete();
		}

		public static void Delete()
		{
			IMJMHDPIBDI(null);
			EDAPJLKMFPC = null;
		}
	}
}
