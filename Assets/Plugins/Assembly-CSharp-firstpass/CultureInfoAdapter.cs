using System;
using System.Globalization;

internal sealed class CultureInfoAdapter : CultureInfo
{
	private readonly IFormatProvider CHEOFBCLNDF;

	public CultureInfoAdapter(CultureInfo OOLKOFLIGGN, IFormatProvider EEGMFLOPLLH)
		: base(OOLKOFLIGGN.LCID)
	{
		CHEOFBCLNDF = EEGMFLOPLLH;
	}

	public override object GetFormat(Type HHNCMCGMOGP)
	{
		return CHEOFBCLNDF.GetFormat(HHNCMCGMOGP);
	}
}
