using System;
using System.Text;

public sealed class HTTPUrlEncodedForm : HTTPFormBase
{
	private byte[] CachedData;

	public override void PrepareRequest(HTTPRequest ONOCIELLAPL)
	{
		ONOCIELLAPL.MMPFBNNMGED("Content-Type", "application/x-www-form-urlencoded");
	}

	public override byte[] GDENFGNLFKL()
	{
		if (CachedData != null && !JODDPBJOEJN())
		{
			return CachedData;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < CKOJIABCEBP().Count; i++)
		{
			HTTPFieldData iIMHHCDGJOL = CKOJIABCEBP()[i];
			if (i > 0)
			{
				stringBuilder.Append("&");
			}
			stringBuilder.Append(Uri.EscapeDataString(iIMHHCDGJOL.get_Name()));
			stringBuilder.Append("=");
			if (!string.IsNullOrEmpty(iIMHHCDGJOL.ILMJJEMPKCN()) || iIMHHCDGJOL.CLBEEBOFBMA() == null)
			{
				stringBuilder.Append(Uri.EscapeDataString(iIMHHCDGJOL.ILMJJEMPKCN()));
			}
			else
			{
				stringBuilder.Append(Uri.EscapeDataString(Encoding.UTF8.GetString(iIMHHCDGJOL.CLBEEBOFBMA(), 0, iIMHHCDGJOL.CLBEEBOFBMA().Length)));
			}
		}
		AKIGPOBCEOC(false);
		return CachedData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
	}
}
