namespace QiHe.Yaml.Grammar
{
	public class BlockScalarModifier
	{
		public char Indent;

		public char Chomp;

		public int GetIndent()
		{
			if (Indent > '0' && Indent <= '9')
			{
				return Indent - 48;
			}
			return 1;
		}

		public ChompingMethod GetChompingMethod()
		{
			switch (Chomp)
			{
			case '-':
				return ChompingMethod.Strip;
			case '+':
				return ChompingMethod.Keep;
			default:
				return ChompingMethod.Clip;
			}
		}
	}
}
