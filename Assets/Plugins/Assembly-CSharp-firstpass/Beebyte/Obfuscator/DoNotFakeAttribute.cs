using System;

namespace Beebyte.Obfuscator
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class DoNotFakeAttribute : Attribute
	{
	}
}
