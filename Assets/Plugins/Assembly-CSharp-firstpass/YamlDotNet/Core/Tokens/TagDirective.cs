using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class TagDirective : Token
	{
		private readonly string handle;

		private readonly string prefix;

		private static readonly Regex tagHandleValidator = new Regex("^!([0-9A-Za-z_\\-]*!)?$", RegexOptions.None);

		public string Handle
		{
			get
			{
				return handle;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
		}

		public TagDirective(string FODGADCGDBH, string JMOHMLIGHHD)
			: this(FODGADCGDBH, JMOHMLIGHHD, Mark.Empty, Mark.Empty)
		{
		}

		public TagDirective(string FODGADCGDBH, string JMOHMLIGHHD, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			if (string.IsNullOrEmpty(FODGADCGDBH))
			{
				throw new ArgumentNullException("handle", "Tag handle must not be empty.");
			}
			if (!tagHandleValidator.IsMatch(FODGADCGDBH))
			{
				throw new ArgumentException("Tag handle must start and end with '!' and contain alphanumerical characters only.", "handle");
			}
			handle = FODGADCGDBH;
			if (string.IsNullOrEmpty(JMOHMLIGHHD))
			{
				throw new ArgumentNullException("prefix", "Tag prefix must not be empty.");
			}
			prefix = JMOHMLIGHHD;
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			TagDirective tagDirective = AOMLCBHAJJH as TagDirective;
			return tagDirective != null && handle.Equals(tagDirective.handle) && prefix.Equals(tagDirective.prefix);
		}

		public override int GetHashCode()
		{
			return handle.GetHashCode() ^ prefix.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} => {1}", handle, prefix);
		}
	}
}
