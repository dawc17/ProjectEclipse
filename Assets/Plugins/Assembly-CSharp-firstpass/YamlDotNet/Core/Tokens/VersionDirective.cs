using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class VersionDirective : Token
	{
		private readonly Version version;

		public Version Version
		{
			get
			{
				return version;
			}
		}

		public VersionDirective(Version version)
			: this(version, Mark.Empty, Mark.Empty)
		{
		}

		public VersionDirective(Version version, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			version = version;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			VersionDirective versionDirective = AOMLCBHAJJH as VersionDirective;
			return versionDirective != null && version.Equals(versionDirective.version);
		}

		public override int GetHashCode()
		{
			return version.GetHashCode();
		}
	}
}
