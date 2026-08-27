using System;

namespace Beebyte.Obfuscator
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate)]
	public class RenameAttribute : Attribute
	{
		private readonly string target;

		private RenameAttribute()
		{
		}

		public RenameAttribute(string target)
		{
			this.target = target;
		}

		public string JMMBNMFPFMK()
		{
			return target;
		}
	}
}
