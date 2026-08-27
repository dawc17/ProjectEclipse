using System.Collections.Generic;
using Nekki.SF2.GUI;

namespace Nekki.SF2.Core.Tutorials
{
	public class TutorialRaid : SFMonoBehaviour<object>
	{
		public enum AMFHDCJOOHE
		{
			ON_TUTORIAL_RAID_COMPLETE = 0
		}

		private List<TutorialAction> MLALLKGGODN = new List<TutorialAction>();

		private int FEHIABBOPPL = -1;

		private bool PPFECHDPFEJ;

		public TutorialRaid()
		{
			Init();
		}

		public void Run()
		{
			PPFECHDPFEJ = true;
		}

		public virtual bool Init()
		{
			return true;
		}

		public void Clear()
		{
			foreach (TutorialAction item in MLALLKGGODN)
			{
			}
			MLALLKGGODN.Clear();
		}

		public virtual void Draw()
		{
			if (PPFECHDPFEJ)
			{
				PPFECHDPFEJ = false;
				FEHIABBOPPL++;
				if (MLALLKGGODN.Count - 1 >= FEHIABBOPPL)
				{
					MLALLKGGODN[FEHIABBOPPL].Run();
				}
				else
				{
					OGIJONMKABB();
				}
			}
		}

		private void NLJLHHNPCAO(TutorialAction IBODMPMJELJ)
		{
			IBODMPMJELJ.AddEventListener(0, OnActionComplete);
			MLALLKGGODN.Add(IBODMPMJELJ);
		}

		private void OnActionComplete(object data)
		{
			if (data == null)
			{
				PPFECHDPFEJ = true;
			}
			else
			{
				OGIJONMKABB();
			}
		}

		private void OGIJONMKABB()
		{
			CallEvent(0, null);
		}
	}
}
