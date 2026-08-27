using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Version
	{
		public int Major { get; private set; }

		public int Minor { get; private set; }

		public Version(int IBGMIGIFNJM, int LDKAECLLDNG)
		{
			Major = IBGMIGIFNJM;
			Minor = LDKAECLLDNG;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			Version version = AOMLCBHAJJH as Version;
			return version != null && Major == version.Major && Minor == version.Minor;
		}

		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode();
		}
	}
}
