using System.Diagnostics;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class PointsTable : MonoBehaviour
	{
		private int DJGPHOLGJDA;

		private int HBOFMJCFOKK;

		private int LOMKKEAMMIG;

		private Vector2 textSizeDelta = new Vector2(200f, 200f);

		[SerializeField]
		private LabelAlias leftScoreText;

		[SerializeField]
		private LabelAlias delimiterText;

		[SerializeField]
		private LabelAlias rightScoreText;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PointsTableType KAHHEBMBCFA;

		public int LJKGBLBEGAM
		{
			get
			{
				return get_LeftScore();
			}
			set
			{
				set_LeftScore(value);
			}
		}

		public int KDENPNFJNJL
		{
			get
			{
				return get_RightScore();
			}
			set
			{
				set_RightScore(value);
			}
		}

		public int get_LeftScore()
		{
			return DJGPHOLGJDA;
		}

		public void set_LeftScore(int value)
		{
			if (DJGPHOLGJDA != value && leftScoreText != null)
			{
				DJGPHOLGJDA = value;
				leftScoreText.set_text(DJGPHOLGJDA.ToString());
			}
		}

		public int get_RightScore()
		{
			return HBOFMJCFOKK;
		}

		public void set_RightScore(int value)
		{
			if (HBOFMJCFOKK != value && rightScoreText != null)
			{
				HBOFMJCFOKK = value;
				rightScoreText.set_text(HBOFMJCFOKK.ToString());
			}
		}

		public PointsTableType get_Type()
		{
			return KAHHEBMBCFA;
		}

		private void set_Type(PointsTableType value)
		{
			KAHHEBMBCFA = value;
		}

		public void Init(PointsTableType LFLGCDNKNJI, int LOMKKEAMMIG = 0, int CFMPJLLNCFF = 120)
		{
			set_Type(LFLGCDNKNJI);
			this.LOMKKEAMMIG = LOMKKEAMMIG;
			switch (LFLGCDNKNJI)
			{
			case PointsTableType.POINTS_TABLE_CONTEST:
				leftScoreText.set_text(DJGPHOLGJDA.ToString());
				rightScoreText.set_text(HBOFMJCFOKK.ToString());
				delimiterText.set_text(":");
				break;
			case PointsTableType.POINTS_TABLE_SCORE:
				leftScoreText.set_text(DJGPHOLGJDA.ToString());
				rightScoreText.set_text(LOMKKEAMMIG.ToString());
				delimiterText.set_text("/");
				break;
			}
		}
	}
}
