using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;

public interface IScanner
{
	Mark CurrentPosition { get; }

	Token Current { get; }

	bool PCCMLADDNDG();
}
