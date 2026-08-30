using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public static class ResponseExtension
{
	public static void IHOOAEHGMFO(this HttpListenerResponse GIHDDAKBMHE, string NILNDHEKNLJ, string LFLGCDNKNJI = "text/plain")
	{
		GIHDDAKBMHE.StatusCode = 200;
		GIHDDAKBMHE.StatusDescription = "OK";
		if (!string.IsNullOrEmpty(NILNDHEKNLJ))
		{
			byte[] bytes = Encoding.UTF8.GetBytes(NILNDHEKNLJ);
			GIHDDAKBMHE.ContentLength64 = bytes.Length;
			GIHDDAKBMHE.ContentType = LFLGCDNKNJI;
			GIHDDAKBMHE.OutputStream.Write(bytes, 0, bytes.Length);
		}
	}

	public static void FJPANBOJJDI(this HttpListenerResponse GIHDDAKBMHE, byte[] KPAMPCLHCEN)
	{
		GIHDDAKBMHE.StatusCode = 200;
		GIHDDAKBMHE.StatusDescription = "OK";
		GIHDDAKBMHE.ContentLength64 = KPAMPCLHCEN.Length;
		GIHDDAKBMHE.OutputStream.Write(KPAMPCLHCEN, 0, KPAMPCLHCEN.Length);
	}

	public static void CDAFFJPMLCG(this HttpListenerResponse GIHDDAKBMHE, string path, string LFLGCDNKNJI = "application/octet-stream", bool GLKLLFAAAKI = false)
	{
		using (FileStream fileStream = File.OpenRead(path))
		{
			GIHDDAKBMHE.StatusCode = 200;
			GIHDDAKBMHE.StatusDescription = "OK";
			GIHDDAKBMHE.ContentLength64 = fileStream.Length;
			GIHDDAKBMHE.ContentType = LFLGCDNKNJI;
			if (GLKLLFAAAKI)
			{
				GIHDDAKBMHE.AddHeader("Content-disposition", string.Format("attachment; filename={0}", Path.GetFileName(path)));
			}
			byte[] array = new byte[65536];
			int count;
			while ((count = fileStream.Read(array, 0, array.Length)) > 0)
			{
				System.Threading.Thread.Sleep(0);
				GIHDDAKBMHE.OutputStream.Write(array, 0, count);
			}
		}
	}
}
