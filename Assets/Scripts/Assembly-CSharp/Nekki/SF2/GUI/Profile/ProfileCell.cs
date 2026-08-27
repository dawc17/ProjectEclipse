namespace Nekki.SF2.GUI.Profile
{
	public abstract class ProfileCell : TableViewCell
	{
		public override void SetHighlighted()
		{
		}

		public override void SetSelected()
		{
		}

		public override void Display()
		{
		}

		public abstract SubItem GetFirstIcon();

		public abstract void UpdateState();

		public abstract void Clear();
	}
}
