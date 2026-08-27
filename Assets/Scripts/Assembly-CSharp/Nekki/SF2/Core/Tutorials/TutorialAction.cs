using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nekki.SF2.Core.Tutorials
{
	public class TutorialAction : SFMonoBehaviour<object>
	{
		public enum HEGMELMMJEG
		{
			ACTION_EVENT_ON_COMPLETE = 0
		}

		protected Scene GGEKAKEKBEH;

		private GameObject LJKFMGOCNLE;

		private List<GameObject> MMNBKIIDOKA = new List<GameObject>();

		private List<global::Pair<GameObject, int>> AFIMKNBGNLM;

		public virtual void Run()
		{
			GGEKAKEKBEH = Module.ELEBLBJKDBI().HMGDPCPPEFC();
		}

		protected virtual bool CHDEIEMINPF()
		{
			return true;
		}

		protected void OGIJONMKABB(int BNPIIOAIBGN = 0)
		{
			CallEvent(0, BNPIIOAIBGN);
		}

		protected void OCEFGJJMKOC(Button KLNKEPMAGKF, bool KOHDJNFJLGH)
		{
			if (!KOHDJNFJLGH)
			{
			}
		}

		protected void FHAAJLCMPFD()
		{
			Object.Destroy(LJKFMGOCNLE);
		}

		protected void GIFJDKOJFEO(GameObject target, float KDGOIIIHPCL, float AMKFJMOMNNB, float DOBNKCHMKGE = 0f)
		{
		}

		protected void EAJHMLAGNNO()
		{
			foreach (GameObject item in MMNBKIIDOKA)
			{
				Object.Destroy(item);
			}
			MMNBKIIDOKA.Clear();
		}

		protected void BCBNPEFIEEG(GameObject GBIOHMNNEJI)
		{
			global::Pair<GameObject, int> item = new global::Pair<GameObject, int>(GBIOHMNNEJI, 0);
			AFIMKNBGNLM.Add(item);
		}
	}
}
