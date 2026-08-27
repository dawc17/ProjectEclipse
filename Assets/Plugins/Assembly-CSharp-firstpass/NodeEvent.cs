using System;
using System.Text.RegularExpressions;
using YamlDotNet.Core;

public abstract class NodeEvent : ParsingEvent
{
	internal static readonly Regex anchorValidator = new Regex("^[0-9a-zA-Z_\\-]+$", RegexOptions.None);

	private readonly string KOLNNNLOCFE;

	private readonly string EDLADAAKMDF;

	public string BJKKJNDLDJN
	{
		get
		{
			return HCPOJDFJFMM();
		}
	}

	public string DDFDDHGJFBO
	{
		get
		{
			return LOIGCKFONHJ();
		}
	}

	public abstract bool MKCFJALADOA { get; }

	protected NodeEvent(string KOLNNNLOCFE, string EDLADAAKMDF, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
		if (KOLNNNLOCFE != null)
		{
			if (KOLNNNLOCFE.Length == 0)
			{
				throw new ArgumentException("Anchor value must not be empty.", "anchor");
			}
			if (!anchorValidator.IsMatch(KOLNNNLOCFE))
			{
				throw new ArgumentException("Anchor value must contain alphanumerical characters only.", "anchor");
			}
		}
		if (EDLADAAKMDF != null && EDLADAAKMDF.Length == 0)
		{
			throw new ArgumentException("Tag value must not be empty.", "tag");
		}
		this.KOLNNNLOCFE = KOLNNNLOCFE;
		this.EDLADAAKMDF = EDLADAAKMDF;
	}

	protected NodeEvent(string KOLNNNLOCFE, string EDLADAAKMDF)
		: this(KOLNNNLOCFE, EDLADAAKMDF, Mark.Empty, Mark.Empty)
	{
	}

	public string HCPOJDFJFMM()
	{
		return KOLNNNLOCFE;
	}

	public string LOIGCKFONHJ()
	{
		return EDLADAAKMDF;
	}

	public abstract bool DOHAHEHOCLN();
}
