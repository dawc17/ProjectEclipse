using System.IO;

public sealed class HTTPMultiPartForm : HTTPFormBase
{
	private string Boundary;

	private byte[] CachedData;

	public HTTPMultiPartForm()
	{
		Boundary = GetHashCode().ToString("X");
	}

	public override void PrepareRequest(HTTPRequest ONOCIELLAPL)
	{
		ONOCIELLAPL.MMPFBNNMGED("Content-Type", "multipart/form-data; boundary=\"" + Boundary + "\"");
	}

	public override byte[] GDENFGNLFKL()
	{
		if (CachedData != null)
		{
			return CachedData;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			for (int i = 0; i < CKOJIABCEBP().Count; i++)
			{
				HTTPFieldData iIMHHCDGJOL = CKOJIABCEBP()[i];
				memoryStream.WriteLine("--" + Boundary);
				memoryStream.WriteLine("Content-Disposition: form-data; name=\"" + iIMHHCDGJOL.get_Name() + "\"" + (string.IsNullOrEmpty(iIMHHCDGJOL.EPDMGFELIMC()) ? string.Empty : ("; filename=\"" + iIMHHCDGJOL.EPDMGFELIMC() + "\"")));
				if (!string.IsNullOrEmpty(iIMHHCDGJOL.DIHKMAKOHGN()))
				{
					memoryStream.WriteLine("Content-Type: " + iIMHHCDGJOL.DIHKMAKOHGN());
				}
				memoryStream.WriteLine("Content-Length: " + iIMHHCDGJOL.NLHGDFGNIHB().Length);
				memoryStream.WriteLine();
				memoryStream.Write(iIMHHCDGJOL.NLHGDFGNIHB(), 0, iIMHHCDGJOL.NLHGDFGNIHB().Length);
				memoryStream.Write(HTTPRequest.HGBANJPCEPF, 0, HTTPRequest.HGBANJPCEPF.Length);
			}
			memoryStream.WriteLine("--" + Boundary + "--");
			AKIGPOBCEOC(false);
			return CachedData = memoryStream.ToArray();
		}
	}
}
