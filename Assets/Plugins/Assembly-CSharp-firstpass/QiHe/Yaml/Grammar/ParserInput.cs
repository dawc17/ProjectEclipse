namespace QiHe.Yaml.Grammar
{
	public interface ParserInput<T>
	{
		int Length { get; }

		bool HasInput(int pos);

		T GetInputSymbol(int pos);

		T[] GetSubSection(int position, int length);

		string FormErrorMessage(int position, string message);
	}
}
