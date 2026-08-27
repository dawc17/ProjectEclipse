namespace QiHe.Yaml.Grammar
{
	public class Scalar : DataItem
	{
		public string Text;

		public Scalar()
		{
			Text = string.Empty;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
